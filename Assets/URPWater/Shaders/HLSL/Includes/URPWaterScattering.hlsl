//-----------------------------------------------------------------------------
// Copyright(C) Yan Verde - All Rights Reserved
// Copyright protected under Unity Asset Store EULA
// Refer to https://unity3d.com/legal/as_terms for more informations
// -----------------------------------------------------------------------------
// URP Water
// Author : Yan Verde
// Date : April 10, 2021
// -----------------------------------------------------------------------------


#ifndef URPWATER_SCATTERING_INCLUDED
#define URPWATER_SCATTERING_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "URPWaterVariables.hlsl"
#include "URPWaterHelpers.hlsl"


void ComputeScattering(inout GlobalData data, Varyings IN)
{	
	#if _SCATTERING_ON

		Light mainLight = data.mainLight;

		float3 lightColor = mainLight.color;

		// --- Translucency ---
		float3 L = mainLight.direction;
		float3 V = data.worldViewDir;
		float3 N = IN.worldNormal;// data.worldNormal;

		#if _DOUBLE_SIDED_ON
		L.y = lerp(-L.y, L.y, data.vFace);
		#endif


		float3 H = normalize(L + N * _ScatteringRangeMin);
		float VdotH = pow(saturate(max(0, dot(V, -H))), _ScatteringRangeMax);
		
		float scatterMask = saturate(VdotH) * _ScatteringIntensity;
		

		#if _DISPLACEMENTMODE_GERSTNER && _CAPS_SCATTERING_ON

		float3 worldN = lerp(N, data.worldNormal, _CapsScatterNormals);
		float NdotV = dot(worldN, V);
		float waveScatter = smoothstep(_CapsScatteringRangeMin, _CapsScatteringRangeMax, saturate(IN.texcoord3.y));
		float fresnel = max(0, NdotV * NdotV);
		fresnel *= 1- dot(float3(0,1,0), V);

		scatterMask += saturate(fresnel) * waveScatter * _CapsScatteringIntensity;
		#endif

		float3 scatterColor = _ScatteringColor.rgb * saturate(scatterMask);

		data.scattering = scatterColor * saturate(lightColor);


		#if _DOUBLE_SIDED_ON
		data.scattering = lerp(data.scattering * 0.25, data.scattering, data.vFace);
		#endif


	#endif
}

#endif