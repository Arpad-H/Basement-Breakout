//-----------------------------------------------------------------------------
// Copyright(C) Yan Verde - All Rights Reserved
// Copyright protected under Unity Asset Store EULA
// Refer to https://unity3d.com/legal/as_terms for more informations
// -----------------------------------------------------------------------------
// URP Water
// Author : Yan Verde
// Date : April 10, 2021
// -----------------------------------------------------------------------------

#ifndef URPWATER_FOAM_INCLUDED
#define URPWATER_FOAM_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"   
#include "URPWaterHelpers.hlsl"
#include "URPWaterVariables.hlsl"

void ComputeFoam(inout GlobalData data, Varyings IN)
{

	#if _FOAM_ON
		float2 foamDistortion = data.worldNormal.xz * _FoamDistortion;

		float edgeMask = DistanceFade(data.sceneDepth, data.pixelDepth, 0, max(0,_FoamSize));
		float foamTex = SAMPLE_TEXTURE2D(_FoamTex, URPWater_trilinear_repeat_sampler, IN.texcoord1.zw + foamDistortion).r;

		#if _DISPLACEMENTMODE_GERSTNER && _FOAM_WHITECAPS_ON
			float capMask = saturate(IN.texcoord3.y * _FoamCapsIntensity);
			edgeMask += smoothstep(_FoamCapsRangeMin, _FoamCapsRangeMax, capMask * capMask);
		#endif

		#if _DYNAMIC_EFFECTS_ON
			edgeMask += data.dynamicData.b * _DynamicFoam;
		#endif

		#if _ADD_FOAM_ON
			edgeMask = max(edgeMask, IN.color.r);
		#endif

		#if _FOAM_RIPPLE_ON
			float rippleMask = DistanceFade(data.sceneDepth, data.pixelDepth, 0, max(0, _FoamRippleDistance));
			float rippleSpeed =  1 - _FoamRippleSpeed - 1;
			float rippleSize = _FoamRippleSize + _FoamSize;
			float ripples = rippleMask * saturate(sin((rippleMask - _Time.y * rippleSpeed + foamDistortion) * rippleSize * URP_WATER_PI) ).r;

			data.foamMask = foamTex * max(edgeMask, ripples);
		#else

			data.foamMask = foamTex * edgeMask;
		#endif



	#endif
}

#endif
