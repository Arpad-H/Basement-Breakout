//-----------------------------------------------------------------------------
// Copyright(C) Yan Verde - All Rights Reserved
// Copyright protected under Unity Asset Store EULA
// Refer to https://unity3d.com/legal/as_terms for more informations
// -----------------------------------------------------------------------------
// URP Water
// Author : Yan Verde
// Date : April 10, 2021
// -----------------------------------------------------------------------------

#ifndef URPWATER_ALPHA_INCLUDED
#define URPWATER_ALPHA_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "URPWaterVariables.hlsl"
#include "URPWaterHelpers.hlsl"


void ComputeAlpha(inout GlobalData data, Varyings IN)
{	
	#if _ALPHA_MASK_ON || _EDGEFADE_ON
	float mask = 1;
	#endif

	#if _ALPHA_MASK_ON 
	mask *= IN.color.a;
	#endif

	#if _EDGEFADE_ON
	float edgeMask = 1 - DistanceFade(data.sceneDepth, data.pixelDepth, 0, max(0, _EdgeSize));
	mask *= edgeMask;
	#endif

	#if _ALPHA_MASK_ON || _EDGEFADE_ON
	data.finalColor = lerp(data.clearColor, data.finalColor, mask);
	#endif
}

#endif