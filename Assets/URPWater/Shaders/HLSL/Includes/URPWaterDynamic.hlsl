#ifndef URPWATER_DYNAMIC_INCLUDED
#define URPWATER_DYNAMIC_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "URPWaterVariables.hlsl"
#include "URPWaterHelpers.hlsl"


void ComputeDynamicData(inout GlobalData data, Varyings IN)
{	
	#if _DYNAMIC_EFFECTS_ON
	data.dynamicData = SAMPLE_TEXTURE2D(_DynamicEffectsTexture, URPWater_linear_clamp_sampler, IN.projectionUV);
	#endif
}

#endif