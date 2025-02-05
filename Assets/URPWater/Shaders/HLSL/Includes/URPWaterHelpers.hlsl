#ifndef URPWATER_HELPERS_INCLUDED
#define URPWATER_HELPERS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"  
#include "URPWaterVariables.hlsl"


float4 DualAnimatedUV(float2 uv, float4 tilings, float4 speeds) 
{
	float4 coords;

	coords.xy = uv * tilings.xy;
	coords.zw = uv * tilings.zw;

	#if _WORLD_UV
	coords += speeds * _Time.x;
	#else
	coords += frac(speeds * _Time.x);
	#endif

	return coords;
}

float2 AnimatedUV(float2 uv, float2 tilings, float2 speeds)
{
	float2 coords;

	coords.xy = uv * tilings.xy;

	#if _WORLD_UV
	coords += speeds * _Time.xx;
	#else
	coords += frac(speeds * _Time.xx);
	#endif

	return coords;
}

float UVEdgeMask(float2 uv, float maskSize) 
{
	float2 edgeMaskUV = abs(uv * 2 - 1);
	float edgeMask = 1 - max(edgeMaskUV.x, edgeMaskUV.y);
	return smoothstep(0, maskSize, edgeMask);
}


float ComputePixelDepth(float3 worldPos) 
{
	return - TransformWorldToView(worldPos).z;
}

// From DeclareDepthTexture.hlsl
float SampleRawDepth(float2 uv)
{
	/*
	#if UNITY_REVERSED_Z
		real depth = SampleSceneDepth(uv);
	#else
		// Adjust z to match NDC for OpenGL
		real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(uv));
	#endif
	*/

	//Manual mode in case Unity breaks this feature
	//UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraDepthTexture, sampler_ScreenTextures_linear_clamp, uv);

	return SampleSceneDepth(uv);
}

float Linear01Depth(float z) 
{
	return 1.0 / (_ZBufferParams.x * z + _ZBufferParams.y);
}

float InvLerp(float a, float b, float v)
{
	return (v - a) / (b - a);
}

float RawDepthToLinear(float rawDepth) 
{
	#if _ORTHO_ON
		float persp = LinearEyeDepth(rawDepth, _ZBufferParams);
		float orthoLinearDepth = _ProjectionParams.x > 0 ? rawDepth : 1 - rawDepth;
		return lerp(_ProjectionParams.y, _ProjectionParams.z, orthoLinearDepth);
	#else
		return LinearEyeDepth(rawDepth, _ZBufferParams);
	#endif
}

float SampleDepth(float2 uv) 
{
	return RawDepthToLinear(SampleRawDepth(uv));
}

float3 ProjectedWorldPos(GlobalData data, Varyings IN)
{
	float3 rawWorldViewDir = _WorldSpaceCameraPos.xyz - data.worldPosition;
	float4 rawScreenPos = ComputeScreenPos(IN.pos, _ProjectionParams.x);
	float eyeDepth = data.refractionData.a;

	float3 pos = rawWorldViewDir / rawScreenPos.a;
	pos *= eyeDepth;
	pos -= _WorldSpaceCameraPos.xyz;

	return pos;
}

float3 ViewSpacePosAtPixelPosition(float2 pos, float2 offset)
{

	float2 uv = pos * _CameraDepthTexture_TexelSize.xy + offset;
	float3 viewSpaceRay = mul(unity_CameraInvProjection, float4(uv * 2.0 - 1.0, 1.0, 1.0) * _ProjectionParams.z).xyz;
	float4 rawDepth = SampleSceneDepth(uv);//UNITY_SAMPLE_TEXTURE2D_LOD(_CameraDepthTexture, sampler_pointTextures_point_clamp, uv, 0.0);
	return viewSpaceRay * Linear01Depth(rawDepth.r, _ZBufferParams);
}


float3 NormalFromDepthFast(float4 pos, float2 offset)
{
	float3 vpc = ViewSpacePosAtPixelPosition(pos.xy, offset);
	float3 vpl = ViewSpacePosAtPixelPosition(pos.xy + float2(-1, 0), offset);
	float3 vpd = ViewSpacePosAtPixelPosition(pos.xy + float2(0, -1), offset);

	// get view space normal from the cross product of the two smallest offsets
	float3 viewNormal = normalize(-cross(vpc - vpd, vpc - vpl));
	
	// transform normal from view space to world space
	return mul((float3x3)unity_MatrixInvV, viewNormal);

	// if needed, this will detect the sky
	// float rawDepth = _CameraDepthTexture.Load(int3(i.pos.xy, 0)).r;
	// if (rawDepth == 0.0)
	// WorldNormal = float3(0,0,0);
}



half3 NormalFromDepth(float4 pos, float2 offset)
{
	// get current pixel's view space position
	half3 viewSpacePos_c = ViewSpacePosAtPixelPosition(pos.xy, offset);

	// get view space position at 1 pixel offsets in each major direction
	half3 viewSpacePos_l = ViewSpacePosAtPixelPosition(pos.xy + float2(-1.0, 0.0), offset);
	half3 viewSpacePos_r = ViewSpacePosAtPixelPosition(pos.xy + float2(1.0, 0.0), offset);
	half3 viewSpacePos_d = ViewSpacePosAtPixelPosition(pos.xy + float2(0.0, -1.0), offset);
	half3 viewSpacePos_u = ViewSpacePosAtPixelPosition(pos.xy + float2(0.0, 1.0), offset);

	// get the difference between the current and each offset position
	half3 l = viewSpacePos_c - viewSpacePos_l;
	half3 r = viewSpacePos_r - viewSpacePos_c;
	half3 d = viewSpacePos_c - viewSpacePos_d;
	half3 u = viewSpacePos_u - viewSpacePos_c;

	// pick horizontal and vertical diff with the smallest z difference
	half3 h = abs(l.z) < abs(r.z) ? l : r;
	half3 v = abs(d.z) < abs(u.z) ? d : u;

	// get view space normal from the cross product of the two smallest offsets
	half3 viewNormal = normalize(cross(h, v));

	// transform normal from view space to world space
	return mul((float3x3)unity_MatrixInvV, viewNormal);
}



float3 WorldNormal(float3 t0, float3 t1, float3 t2, float3 bump)
{
	return normalize(float3(dot(t0, bump), dot(t1, bump), dot(t2, bump)));
}


float3 HeightToNormal(float height, float3 normal, float3 pos)
{
	float3 worldDirivativeX = ddx(pos);
	float3 worldDirivativeY = ddy(pos);
	float3 crossX = cross(normal, worldDirivativeX);
	float3 crossY = cross(normal, worldDirivativeY);
	float3 d = abs(dot(crossY, worldDirivativeX));
	float3 inToNormal = ((((height + ddx(height)) - height) * crossY) + (((height + ddy(height)) - height) * crossX)) * sign(d);
	inToNormal.y *= -1.0;
	return normalize((d * normal) - inToNormal);
}

/*
float3 WorldToTangentNormalVector(Input IN, float3 normal) {
	float3 t2w0 = WorldNormal(IN, float3(1, 0, 0));
	float3 t2w1 = WorldNormal(IN, float3(0, 1, 0));
	float3 t2w2 = WorldNormal(IN, float3(0, 0, 1));
	float3x3 t2w = float3x3(t2w0, t2w1, t2w2);
	return normalize(mul(t2w, normal));
}
*/

float DistanceFade(float depth, float pixelDepth, float start, float end)
{
	float dist = ((depth - pixelDepth) - end) / (start - end);
	return saturate(dist);
}

float2 OffsetUV(float2 uv, float2 offset)
{
	#ifdef UNITY_Z_0_FAR_FROM_CLIPSPACE
		
		uv.xy = offset + uv.xy;
		//TODO: have to find use case
		//uv.xy = offset * UNITY_Z_0_FAR_FROM_CLIPSPACE(IN.pos.z) + uv.xy;
	#else
		uv.xy = offset + uv.xy;
	#endif

	return uv.xy;
}

float2 OffsetDepth(float2 uv, float2 offset)
{
	uv.xy = offset + uv.xy;
	return uv.xy;
}

half2 DistortionUVs(half depth, float3 normalWS)
{
	half3 viewNormal = mul((float3x3)GetWorldToHClipMatrix(), -normalWS).xyz;

	return viewNormal.xz * saturate((depth) * 0.005);
}

float smootherstep(float x) {
	x = saturate(x);
	return saturate(x * x * x * (x * (6 * x - 15) + 10));
}

float InverseLerp(float A, float B, float T)
{
	return (T - A) / (B - A);
}

float2 ProjectionUV(float4 captureParams, float3 worldPosition ) 
{
	float3 simPos = worldPosition - captureParams.xyz;
	float3 simScale = simPos / captureParams.w;
	return simScale.xz + 0.5;
}

float2 CaptureUV(float2 worldPosition, float2 capturePosition, float2 captureSize) 
{
	float2 simPos = worldPosition.xy - capturePosition.xy;
	float2 uv = simPos / captureSize.xy;

	return uv.xy + 0.5;
}

float CaptureMask(float2 uv, float size) 
{
	float2 edgeMaskUV = abs(uv * 2 - 1);
	float edgeMask = 1 - max(edgeMaskUV.x, edgeMaskUV.y);
	float mask = smoothstep(0, size, edgeMask);

	return saturate(mask);
}

void Applyfog(inout float3 color, float3 positionWS)
{
	float3 inColor = color;

	#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
	//float viewZ = -TransformWorldToView(TransformObjectToWorld(positionOS)).z;
	float viewZ = -TransformWorldToView(positionWS).z;
	float nearZ0ToFarZ = max(viewZ - _ProjectionParams.y, 0);
	// ComputeFogFactorZ0ToFar returns the fog "occlusion" (0 for full fog and 1 for no fog) so this has to be inverted for density.

	float density = 1.0f - ComputeFogIntensity(ComputeFogFactorZ0ToFar(nearZ0ToFarZ));
	color = lerp(color, unity_FogColor.rgb, density);

	#else
	color = color;
	#endif

}

#endif