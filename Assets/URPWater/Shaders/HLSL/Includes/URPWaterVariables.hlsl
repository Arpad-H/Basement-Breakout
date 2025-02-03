//-----------------------------------------------------------------------------
// Copyright(C) Yan Verde - All Rights Reserved
// Copyright protected under Unity Asset Store EULA
// Refer to https://unity3d.com/legal/as_terms for more informations
// -----------------------------------------------------------------------------
// URP Water
// Author : Yan Verde
// Date : April 10, 2021
// -----------------------------------------------------------------------------

#ifndef URPWATER_VARIABLES_INCLUDED
#define URPWATER_VARIABLES_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"   
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

/*
#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
	#define UNITY_DECLARE_SCREENSPACE_TEXTURE(textureName) TEXTURE2D_ARRAY(textureName);
	#define UNITY_DECLARE_SCREENSPACE_TEXTURE_FLOAT(textureName) TEXTURE2D_ARRAY_FLOAT(textureName);
		//UNITY_SAMPLE_TEX2DARRAY(tex, float3((uv).xy, (float)unity_StereoEyeIndex))
	#define UNITY_SAMPLE_SCREENSPACE_TEXTURE(tex, samplerName, uv) SAMPLE_TEXTURE2D_ARRAY(tex, samplerName, uv.xy, (float)unity_StereoEyeIndex);
	#define UNITY_SAMPLE_TEXTURE2D_LOD(textureName, samplerName, coord2, lod) SAMPLE_TEXTURE2D_ARRAY_LOD(textureName, samplerName, coord2, (float)unity_StereoEyeIndex, lod);
#else
	#define UNITY_DECLARE_SCREENSPACE_TEXTURE(tex) TEXTURE2D(tex);
	#define UNITY_DECLARE_SCREENSPACE_TEXTURE_FLOAT(tex) TEXTURE2D_FLOAT(tex);
	#define UNITY_SAMPLE_SCREENSPACE_TEXTURE(tex, samplerName, uv) SAMPLE_TEXTURE2D(tex, samplerName, uv);
	#define UNITY_SAMPLE_TEXTURE2D_LOD(tex, samplerName, coord2, lod) SAMPLE_TEXTURE2D_LOD(tex, samplerName, coord2, lod);
#endif
*/

#define URP_WATER_PI 3.1415926535

TEXTURE2D(_CausticsTex);
TEXTURE3D(_CausticsTex3D);
TEXTURE2D(_FoamTex);
TEXTURE2D(_NormalMapA);
TEXTURE2D(_NormalMapB);
TEXTURE2D(_NormalMapFar);
TEXTURE2D(_FlowMap);

//UNITY_DECLARE_SCREENSPACE_TEXTURE_FLOAT(_CameraDepthTexture); // VR
//UNITY_DECLARE_SCREENSPACE_TEXTURE(_CameraOpaqueTexture); // VR

TEXTURE2D(_RefractionColor);
TEXTURE2D(_DynamicEffectsTexture);

SAMPLER(sampler_CameraOpaqueTexture_linear_clamp);
SAMPLER(sampler_ScreenTextures_linear_clamp);
SAMPLER(sampler_pointTextures_point_clamp);

TEXTURE2D(_ReflectionTexture);

TEXTURECUBE(_CubemapTexture);
SAMPLER(sampler_CubemapTexture);

SamplerState URPWater_trilinear_repeat_sampler;
SamplerState URPWater_linear_repeat_sampler;
SamplerState URPWater_linear_clamp_sampler;

#if UNITY_VERSION <= 202310 || UNITY_VERSION < 202320
float4 _CameraOpaqueTexture_TexelSize;
float4 _CameraDepthTexture_TexelSize;
#endif

float4 _DynamicEffectsTexture_TexelSize;
float4 _DynamicEffectsParams;


UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)

	float4 _NormalMapA_ST;
	float4 _NormalMapASpeeds;
	float4 _NormalMapATilings;
	float4 _NormalMapB_ST;
	float4 _NormalMapBSpeeds;
	float4 _NormalMapBTilings;
	float4 _NormalMapFarTilings;
	float4 _NormalMapFarSpeeds;
	float4 _FlowTiling;
	float4 _CausticsTiling;
	float4 _Color;
	float4 _DepthColor;
	float4 _UnderWaterColor;
	//float4 _AmbientColor;
	float4 _SpecColor;
	float4 _CausticsSpeed;
	float4 _CausticsSpeed3D;
	float4 _ScatteringColor;
	float4 _FoamColor;
	float4 _FoamTiling;
	float4 _OceanInfos;


	float _CausticsStart;
	float _CausticsEnd;
	float _CausticsTiling3D;
	float _CausticsDistortion;
	float _Distortion;
	float _Smoothness;
	float _NormalMapAIntensity;
	float _NormalMapBIntensity;
	float _NormalMapFarIntensity;
	float _NormalFarDistance;
	float _FlowSpeed;
	float _FlowIntensity;
	float _DepthStart;
	float _DepthEnd;
	float _ReflectionDistortion;
	float _ReflectionFresnelNormal;
	float _ReflectionRoughness;
	float _EdgeSize;
	float _CausticsIntensity;
	float _ReflectionFresnel;
	float _ReflectionIntensity;
	float _FoamSize;
	float _FoamRippleDistance;
	float _FoamRippleSize;
	float _FoamRippleSpeed;
	float _FoamDistortion;
	float _FoamCapsRangeMin;
	float _FoamCapsRangeMax;
	float _FoamCapsIntensity;
	float _ScatteringIntensity;
	float _ScatteringRangeMin;
	float _ScatteringRangeMax;
	float _CapsScatteringIntensity;
	float _CapsScatteringRangeMin;
	float _CapsScatteringRangeMax;
	float _CapsScatterNormals;
	float _DynamicNormal;
	float _DynamicFoam;
	float _DynamicDisplacement;

	float _WaveNormal;
	float _WaveEffectsBoost;
	float _WaveCount;
	float _WaveLength;
	float _WaveAmplitude;
	float _WaveSteepness;
	float _WaveSpeed;
	float _WaveStartDistance;
	float _WaveEndDistance;

	float _Tess;

UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)


struct Attributes
{
	float4 vertex		: POSITION;
	float4 color		: COLOR;
	float3 normal		: NORMAL;
	float4 tangent 		: TANGENT;
	float2 texcoord		: TEXCOORD0;

	#if _DISPLACEMENTMODE_GERSTNER
	float waveHeight : TEXCOORD1;
	#endif

	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VaryingsTest
{
	float4 pos				: SV_POSITION;
	float4 color			: COLOR;
	float3 worldNormal		: NORMAL;


	float4 texcoord			: TEXCOORD0;

	#if _NORMALSMODE_DUAL || _FOAM_ON
	float4 texcoord1		: TEXCOORD1;
	#endif

	#if _NORMAL_FAR_ON
	float4 texcoord2 : TEXCOORD2;
	#endif

	//  X: Far Distance, Y: Waves Height Z: W:
	#if _NORMAL_FAR_ON || _DISPLACEMENTMODE_GERSTNER
	float4 texcoord3 : TEXCOORD3;
	#endif

	float4 screenCoord	: TEXCOORD4;
	half4 normal	: TEXCOORD5;    // xyz: normal, w: viewDir.x
	half4 tangent	: TEXCOORD6;    // xyz: tangent, w: viewDir.y
	half4 bitangent : TEXCOORD7;    // xyz: binormal, w: viewDir.z
	#if _DYNAMIC_EFFECTS_ON
	float2 projectionUV : TEXCOORD8;
	#endif

	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
};

struct Varyings
{
	float4 pos				: SV_POSITION;
	float4 color			: COLOR;
	float3 worldNormal		: NORMAL;

	float4 texcoord			: TEXCOORD0;

	#if _NORMALSMODE_DUAL || _FOAM_ON
	float4 texcoord1		: TEXCOORD1;
	#endif

	#if _NORMAL_FAR_ON
	float4 texcoord2 : TEXCOORD2;
	#endif

	//  X: Far Distance, Y: Waves Height Z: W:
	#if _NORMAL_FAR_ON || _DISPLACEMENTMODE_GERSTNER
	float4 texcoord3 : TEXCOORD3;
	#endif

	float4 screenCoord	: TEXCOORD4;

	half4 normal	: TEXCOORD5;    // xyz: normal, w: viewDir.x
	half4 tangent	: TEXCOORD6;    // xyz: tangent, w: viewDir.y
	half4 bitangent : TEXCOORD7;    // xyz: binormal, w: viewDir.z

	#if _DYNAMIC_EFFECTS_ON
	float2 projectionUV : TEXCOORD8;
	#endif
	
	#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
	float4 shadowCoord : TEXCOORD9;
	#endif

	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
};


void InitializeVertexData(inout Varyings IN)
{
	IN.pos = float4(0, 0, 0, 0);
	IN.color = float4(0, 0, 0, 0);
	IN.worldNormal = float3(0, 1, 0);
	IN.texcoord = float4(0, 0, 0, 0);

	#if _NORMALSMODE_DUAL || _FOAM_ON
	IN.texcoord1 = float4(0, 0, 0, 0);
	#endif

	#if _NORMAL_FAR_ON
	IN.texcoord2 = float4(0, 0, 0, 0);
	#endif
	#if _NORMAL_FAR_ON || _DISPLACEMENTMODE_GERSTNER
	IN.texcoord3 = float4(0, 0, 0, 0);
	#endif

	IN.screenCoord = float4(0, 0, 0, 0);

	IN.normal = half4(0, 1, 0, 0);    // xyz: normal, w: viewDir.x
	IN.tangent = half4(0, 1, 0, 0);     // xyz: tangent, w: viewDir.y
	IN.bitangent = half4(0, 1, 0, 0);     // xyz: binormal, w: viewDir.z

	#if _DYNAMIC_EFFECTS_ON
	float2 projectionUV = float2(0, 0);
	#endif

	#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
	IN.shadowCoord = float4(0, 0, 0, 0);
	#endif
}


struct GlobalData 
{
	float depth;		// Remapped Depth
	float sceneDepth;	// Linear Depth
	float rawDepthDst;	// Raw Depth Distorted
	float pixelDepth;
	float foamMask;
	float2 refractionOffset;
	float2 refractionUV;
	float3 finalColor;
	float4 refractionData;	// RGB: Refraction Color A: Refraction Depth
	float3 clearColor;		// RGB: Clear Color
	float3 shadowColor;
	float3 worldPosition;
	float3 worldNormal;
	float3 worldViewDir;
	float4 screenUV;

	#if _DYNAMIC_EFFECTS_ON
	float4 dynamicData;
	#endif

	#if _DOUBLE_SIDED_ON
	float vFace;
	#endif

	#if _SCATTERING_ON
	float3 scattering;
	#endif

	Light mainLight;
	float3 addLight;
	real3x3 tangentToWorld;

	// Debug purpose only
	float4 debug;
};

void InitializeGlobalData(inout GlobalData data, Varyings IN)
{
	data.worldPosition = float3(IN.normal.w, IN.tangent.w, IN.bitangent.w);
	data.worldNormal = float3(0, 1, 0);
	data.worldViewDir = GetWorldSpaceNormalizeViewDir(data.worldPosition);		//SafeNormalize(_WorldSpaceCameraPos.xyz - data.worldPosition);
	data.screenUV = float4(IN.screenCoord.xyz / IN.screenCoord.w, IN.pos.z);	//ComputeScreenPos(TransformWorldToHClip(data.worldPosition), _ProjectionParams.x);
	data.depth = 0;
	data.sceneDepth = 0;
	data.rawDepthDst = 0;
	data.pixelDepth = -TransformWorldToView(data.worldPosition).z; //IN.screenCoord.a;
	data.foamMask = 0;
	data.refractionOffset = float2(0, 0);
	data.refractionUV = float2(0, 0);
	data.refractionData = float4(0, 0, 0 ,0);
	data.clearColor = float3(1, 1, 1);
	data.finalColor = float3(1, 1, 1);
	data.shadowColor = float3(1, 1, 1);


	#if _DYNAMIC_EFFECTS_ON
	data.dynamicData = float4(0, 0, 0, 0);
	#endif

	#if _DOUBLE_SIDED_ON
	data.vFace = 1;
	#endif

	#if _SCATTERING_ON
	data.scattering = float3(0,0,0);
	#endif

	data.mainLight = GetMainLight(TransformWorldToShadowCoord(data.worldPosition));
	data.addLight = float3(0, 0, 0);
	data.tangentToWorld = float3x3(IN.tangent.xyz, IN.bitangent.xyz, IN.normal.xyz);

	data.debug = float4 (0, 0, 0, 1);
}



#endif