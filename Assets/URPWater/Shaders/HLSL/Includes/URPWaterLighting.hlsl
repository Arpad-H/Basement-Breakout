//-----------------------------------------------------------------------------
// Copyright(C) Yan Verde - All Rights Reserved
// Copyright protected under Unity Asset Store EULA
// Refer to https://unity3d.com/legal/as_terms for more informations
// -----------------------------------------------------------------------------
// URP Water
// Author : Yan Verde
// Date : April 10, 2021
// -----------------------------------------------------------------------------

#ifndef URPWATER_LIGHTING_INCLUDED
#define URPWATER_LIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

#include "URPWaterVariables.hlsl"
#include "URPWaterHelpers.hlsl"

// ShadowCoord: TransformWorldToShadowCoord(worldPosition);
// WorldNormal: TransformObjectToWorldNormal(v.normal);
// worldViewDir: SafeNormalize(_WorldSpaceCameraPos.xyz - worldPos)
// worldPos: TransformObjectToWorld(v.vertex.xyz);
// ShadowCoord: TransformWorldToShadowCoord(WorldPos);
// ShadowSample: MainLightRealtimeShadow(shadowCoord);


//From unity file: 
//ShaderLibrary\Shadows.hlsl
half SampleShadows(Varyings IN, GlobalData data)
{
	#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
		float4 shadowCoord = IN.shadowCoord;
	#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
		float4 shadowCoord = TransformWorldToShadowCoord(data.worldPosition);
	#else
		float4 shadowCoord = float4(0, 0, 0, 0);
	#endif

	shadowCoord.xz += data.worldNormal.xz * 0.01;

	return MainLightRealtimeShadow(shadowCoord);
}

void ComputeCaustics(out float3 causticColor, inout GlobalData data, Varyings IN, float3 Ambient)
{

	#if UNITY_REVERSED_Z
		real depth = data.rawDepthDst;
	#else
		// Adjust Z to match NDC for OpenGL ([-1, 1])
		real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, data.rawDepthDst);
	#endif

	float3 worldUV = ComputeWorldSpacePosition(data.refractionUV, depth, UNITY_MATRIX_I_VP);


	float causticFade = DistanceFade(data.refractionData.a, data.pixelDepth, _CausticsStart, _CausticsEnd);

	#if _CAUSTICSMODE_3D
	float3 speed = _CausticsSpeed3D.xyz * _Time.x;

	float3 UV3D = worldUV * _CausticsTiling3D + speed;
	float3 CausticMix = SAMPLE_TEXTURE3D(_CausticsTex3D, URPWater_linear_repeat_sampler, UV3D).rgb;

	#else
	float4 offsets = frac(_CausticsSpeed * _Time.x);
	float3 CausticsA = SAMPLE_TEXTURE2D(_CausticsTex, URPWater_linear_repeat_sampler, worldUV.xz * _CausticsTiling.xy + offsets.xy).rgb;
	float3 CausticsB = SAMPLE_TEXTURE2D(_CausticsTex, URPWater_linear_repeat_sampler, worldUV.xz * _CausticsTiling.zw + offsets.zw).rgb;
	float3 CausticMix = min(CausticsA, CausticsB);
	#endif


	// Normals
	#if _CAUSTICS_ANGLE_ON || _CAUSTICS_DIRECTION_ON
	float3 angleMask = NormalFromDepthFast(IN.pos, data.refractionOffset);

	#if _CAUSTICS_ANGLE_ON
		causticFade *= saturate(angleMask.y * angleMask.y);
	#endif

	#if _CAUSTICS_DIRECTION_ON
		float dotMask = dot(data.mainLight.direction, angleMask);
		causticFade *= saturate(dotMask);
	#endif

	#endif

	// Caustic projection for shadow
	//Light mainLight = GetMainLight(TransformWorldToShadowCoord(worldUV));
	//float shadow = mainLight.shadowAttenuation;

	#if (defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE) || defined(_MAIN_LIGHT_SHADOWS_SCREEN))
	half shadow = SampleShadows(IN, data);
	#else
	half shadow = 1.0;
	#endif

	#if _FOAM_ON
	causticFade *= 1 - data.foamMask * 2;
	#endif

	causticColor = CausticMix * max(0,_CausticsIntensity) * causticFade * (shadow + Ambient * 0.5);

}

// =================================================================
// Directional light computations
// =================================================================
float3 ComputeMainLightDiffuse(float3 direction, float3 worldNormal)
{
	return saturate(dot(worldNormal, direction));
}

float3 ComputeMainLightSpecular(
	Light mainLight,
	float3 worldNormal,
	float3 worldViewDir,
	float3 specular,
	float smoothness)
{

	smoothness = exp2(10 * smoothness + 1);

	/*
	float reflectiveFactor = max(0.0, dot(-worldViewDir, reflect(mainLight.direction, worldNormal)));
	float3 spec = pow(reflectiveFactor, smoothness);

	return mainLight.color * specular * spec;
	*/

	// Unity spec
	return LightingSpecular(mainLight.color, mainLight.direction, worldNormal, worldViewDir, float4(specular, 0), smoothness);
}

Light ComputeMainLight(float3 worldPos)
{
	return GetMainLight(TransformWorldToShadowCoord(worldPos));
}

// =================================================================
// Additive lights computations
// =================================================================
float3 ComputeAdditiveLight(float3 worldPos, float3 worldNormal, float3 worldViewDir, float3 specColor, float smoothness)
{
	smoothness = exp2(10 * smoothness + 1);

	int pixelLightCount = GetAdditionalLightsCount();
	float3 diffuse = float3(0, 0, 0);
	float3 specularColor = float3(0, 0, 0);

	for (int l = 0; l < pixelLightCount; ++l)
	{
		Light light = GetAdditionalLight(l, worldPos);
		half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
		diffuse += LightingLambert(attenuatedLightColor, light.direction, worldNormal) * 0.25;
		specularColor += LightingSpecular(attenuatedLightColor, light.direction, worldNormal, worldViewDir, half4(specColor, 0), smoothness);
	}

	return diffuse + specularColor;
}

void ComputeUnderWaterShading(inout GlobalData data, Varyings IN, float3 ambient) 
{

	float3 clearRefraction = data.refractionData.rgb;

	#if _COLORMODE_COLORS
		float3 shallowColor = _Color.rgb * data.refractionData.rgb;
		float3 depthColor = _DepthColor.rgb * data.shadowColor.rgb;
		data.refractionData.rgb = lerp(depthColor, shallowColor, data.depth);
	#endif

	#if _COLORMODE_GRADIENT
		float3 ramp = SAMPLE_TEXTURE2D(_RefractionColor, URPWater_linear_clamp_sampler, float2(data.depth, 0.5)).rgb * data.shadowColor.rgb;
		float3 shallowColor = ramp * data.refractionData.rgb;

		data.refractionData.rgb = lerp(ramp, shallowColor, data.depth);
	#endif

	#if _DOUBLE_SIDED_ON
		float3 underColor = lerp(clearRefraction.rgb * _UnderWaterColor.rgb, _UnderWaterColor.rgb, _UnderWaterColor.a);
		data.refractionData.rgb = lerp(underColor, data.refractionData.rgb, data.vFace);
	#endif


	float invDepth = 1 - data.depth;
	data.finalColor = lerp(data.refractionData.rgb, data.refractionData.rgb * saturate(data.mainLight.color) , invDepth);
}

// =================================================================
// Full lighting computations
// =================================================================
void ComputeLighting(inout GlobalData data, Varyings IN)
{

	Light mainLight = data.mainLight;


	float3 lightDir = mainLight.direction;
	float3 lightColor = mainLight.color;

	#if _DOUBLE_SIDED_ON
		float3 flippedLight = mainLight.direction;
		flippedLight.y = -flippedLight.y;

		mainLight.direction = lerp(flippedLight, mainLight.direction, data.vFace);
	#endif

	//float3 mainDiffuse = ComputeMainLightDiffuse(lightDir.xyz, data.worldNormal) * lightColor * 0.5 + 0.5;
	float3 mainSpecular = ComputeMainLightSpecular(mainLight, data.worldNormal, data.worldViewDir, _SpecColor.rgb, _Smoothness);
	
	#if (defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE) || defined(_MAIN_LIGHT_SHADOWS_SCREEN))
		half shadow = SampleShadows(IN, data);
	#else
		half shadow = 1.0;
	#endif

	float shadowMask = shadow;//max(data.depth, shadow);
	float3 ambient = SampleSH(data.worldNormal);
	
	float3 AddLighting	= ComputeAdditiveLight(data.worldPosition, data.worldNormal, data.worldViewDir, _SpecColor.rgb, _Smoothness);

	
	// TODO: Try using ambient color again 
	//data.shadowColor = lerp(_AmbientColor.rgb, float3(1,1,1), saturate(shadow + data.depth));
	
	// Shadow
	data.shadowColor = saturate(shadowMask + ambient * 2.0);

	// Underwater color
	ComputeUnderWaterShading(data, IN, ambient);

	//data.finalColor = lerp(data.refractionData.rgb, data.refractionData.rgb * saturate(lightColor) , invDepth);


	#if _CAUSTICS_ON
		float3 caustics = float3(1,1,1);
		ComputeCaustics(caustics, data, IN, ambient);

		#if _DOUBLE_SIDED_ON
		caustics = lerp(0, caustics, data.vFace);
		#endif

		data.finalColor += data.finalColor * caustics * saturate(length(lightColor));

	#endif

	#if _SCATTERING_ON
		data.finalColor.rgb += data.scattering * shadow;
	#endif

	#if _FOAM_ON
		mainSpecular *= 1- saturate(data.foamMask.xxx);

		float3 foamColor = _FoamColor.rgb * (lightColor + ambient) * (shadow + ambient);
		data.finalColor = lerp(data.finalColor, foamColor, data.foamMask * _FoamColor.a);
	#endif

	// Specular and additive light after reflection
	data.addLight = (AddLighting + mainSpecular) * data.shadowColor; 
	// Shadows
	data.finalColor = data.finalColor * data.shadowColor;
}


#endif