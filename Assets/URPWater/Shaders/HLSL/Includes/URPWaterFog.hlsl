#ifndef URPWATER_FOG_INCLUDED
#define URPWATER_FOG_INCLUDED

	//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"   
	//#include "URPWaterHelpers.hlsl"
	//#include "URPWaterVariables.hlsl"



	// ------------------------------------------------------------------
	//  Fog helpers
	//
	//  multi_compile_fog Will compile fog variants.
	//  UNITY_FOG_COORDS(texcoordindex) Declares the fog data interpolator.
	//  UNITY_TRANSFER_FOG(outputStruct,clipspacePos) Outputs fog data from the vertex shader.
	//  UNITY_APPLY_FOG(fogData,col) Applies fog to color "col". Automatically applies black fog when in forward-additive pass.
	//  Can also use UNITY_APPLY_FOG_COLOR to supply your own fog color.

	// In case someone by accident tries to compile fog code in one of the g-buffer or shadow passes:
	// treat it as fog is off.
	#if defined(UNITY_PASS_PREPASSBASE) || defined(UNITY_PASS_DEFERRED) || defined(UNITY_PASS_SHADOWCASTER)
	#undef FOG_LINEAR
	#undef FOG_EXP
	#undef FOG_EXP2
	#endif

	/*
	#if defined(UNITY_REVERSED_Z)
		#if UNITY_REVERSED_Z == 1
			//D3d with reversed Z => z clip range is [near, 0] -> remapping to [0, far]
			//max is required to protect ourselves from near plane not being correct/meaningfull in case of oblique matrices.
			#define UNITY_Z_0_FAR_FROM_CLIPSPACE(coord) max(((1.0-(coord)/_ProjectionParams.y)*_ProjectionParams.z),0)
		#else
			//GL with reversed z => z clip range is [near, -far] -> should remap in theory but dont do it in practice to save some perf (range is close enough)
			#define UNITY_Z_0_FAR_FROM_CLIPSPACE(coord) max(-(coord), 0)
		#endif
	#elif UNITY_UV_STARTS_AT_TOP
		//D3d without reversed z => z clip range is [0, far] -> nothing to do
		#define UNITY_Z_0_FAR_FROM_CLIPSPACE(coord) (coord)
	#else
		//Opengl => z clip range is [-near, far] -> should remap in theory but dont do it in practice to save some perf (range is close enough)
		#define UNITY_Z_0_FAR_FROM_CLIPSPACE(coord) (coord)
	#endif
	*/
	#if defined(FOG_LINEAR)
		// factor = (end-z)/(end-start) = z * (-1/(end-start)) + (end/(end-start))
		#define UNITY_CALC_FOG_FACTOR_RAW(coord) float unityFogFactor = (coord) * unity_FogParams.z + unity_FogParams.w
	#elif defined(FOG_EXP)
		// factor = exp(-density*z)
		#define UNITY_CALC_FOG_FACTOR_RAW(coord) float unityFogFactor = unity_FogParams.y * (coord); unityFogFactor = exp2(-unityFogFactor)
	#elif defined(FOG_EXP2)
		// factor = exp(-(density*z)^2)
		#define UNITY_CALC_FOG_FACTOR_RAW(coord) float unityFogFactor = unity_FogParams.x * (coord); unityFogFactor = exp2(-unityFogFactor*unityFogFactor)
	#else
		#define UNITY_CALC_FOG_FACTOR_RAW(coord) float unityFogFactor = 0.0
	#endif

	#define UNITY_CALC_FOG_FACTOR(coord) UNITY_CALC_FOG_FACTOR_RAW(UNITY_Z_0_FAR_FROM_CLIPSPACE(coord))

	#define UNITY_FOG_COORDS_PACKED(idx, vectype) vectype fogCoord : TEXCOORD##idx;

	#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
		#define UNITY_FOG_COORDS(idx) UNITY_FOG_COORDS_PACKED(idx, float1)

		#if (SHADER_TARGET < 30) || defined(SHADER_API_MOBILE)
			// mobile or SM2.0: calculate fog factor per-vertex
			#define UNITY_TRANSFER_FOG(o,outpos) UNITY_CALC_FOG_FACTOR((outpos).z); o.fogCoord.x = unityFogFactor
			#define UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,outpos) UNITY_CALC_FOG_FACTOR((outpos).z); o.tSpace1.y = tangentSign; o.tSpace2.y = unityFogFactor
			#define UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,outpos) UNITY_CALC_FOG_FACTOR((outpos).z); o.worldPos.w = unityFogFactor
			#define UNITY_TRANSFER_FOG_COMBINED_WITH_EYE_VEC(o,outpos) UNITY_CALC_FOG_FACTOR((outpos).z); o.eyeVec.w = unityFogFactor
		#else
			// SM3.0 and PC/console: calculate fog distance per-vertex, and fog factor per-pixel
			#define UNITY_TRANSFER_FOG(o,outpos) o.fogCoord.x = (outpos).z
			#define UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,outpos) o.tSpace2.y = (outpos).z
			#define UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,outpos) o.worldPos.w = (outpos).z
			#define UNITY_TRANSFER_FOG_COMBINED_WITH_EYE_VEC(o,outpos) o.eyeVec.w = (outpos).z
		#endif
	#else
		#define UNITY_FOG_COORDS(idx)
		#define UNITY_TRANSFER_FOG(o,outpos)
		#define UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,outpos)
		#define UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,outpos)
		#define UNITY_TRANSFER_FOG_COMBINED_WITH_EYE_VEC(o,outpos)
	#endif

	#define UNITY_FOG_LERP_COLOR(col,fogCol,fogFac) col.rgb = lerp((fogCol).rgb, (col).rgb, saturate(fogFac))


	#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
		#if (SHADER_TARGET < 30) || defined(SHADER_API_MOBILE)
			// mobile or SM2.0: fog factor was already calculated per-vertex, so just lerp the color
			#define UNITY_APPLY_FOG_COLOR(coord,col,fogCol) UNITY_FOG_LERP_COLOR(col,fogCol,(coord).x)
		#else
			// SM3.0 and PC/console: calculate fog factor and lerp fog color
			#define UNITY_APPLY_FOG_COLOR(coord,col,fogCol) UNITY_CALC_FOG_FACTOR((coord).x); UNITY_FOG_LERP_COLOR(col,fogCol,unityFogFactor)
		#endif
		#define UNITY_EXTRACT_FOG(name) float _unity_fogCoord = name.fogCoord
		#define UNITY_EXTRACT_FOG_FROM_TSPACE(name) float _unity_fogCoord = name.tSpace2.y
		#define UNITY_EXTRACT_FOG_FROM_WORLD_POS(name) float _unity_fogCoord = name.worldPos.w
		#define UNITY_EXTRACT_FOG_FROM_EYE_VEC(name) float _unity_fogCoord = name.eyeVec.w
	#else
		#define UNITY_APPLY_FOG_COLOR(coord,col,fogCol)
		#define UNITY_EXTRACT_FOG(name)
		#define UNITY_EXTRACT_FOG_FROM_TSPACE(name)
		#define UNITY_EXTRACT_FOG_FROM_WORLD_POS(name)
		#define UNITY_EXTRACT_FOG_FROM_EYE_VEC(name)
	#endif


	#define UNITY_APPLY_FOG(coord,col) UNITY_APPLY_FOG_COLOR(coord,col,unity_FogColor)


#endif
