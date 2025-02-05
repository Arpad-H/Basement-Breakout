//-----------------------------------------------------------------------------
// Copyright(C) Yan Verde - All Rights Reserved
// Copyright protected under Unity Asset Store EULA
// Refer to https://unity3d.com/legal/as_terms for more informations
// -----------------------------------------------------------------------------
// URP Water
// Author : Yan Verde
// Date : April 10, 2021
// -----------------------------------------------------------------------------

Shader "URPWater/Standard"
{
	Properties
	{
		//[Header(Refraction)]
		[KeywordEnum(Colors, Gradient)]
		_ColorMode("Color: Mode", Float) = 0

		_Color("Color", color) = (0.6,1,0.8,1)
		_DepthColor("DepthColor", color) = (0,0.26,0.4,1)
		_UnderWaterColor("UnderWaterColor", color) = (0.25, 0.33, 0.45, 0.25)
		_RefractionColor("Refraction Ramp", 2D) = "white" {}
		_DepthStart("Depth Start", float) = 0
		_DepthEnd("Depth End", float) = 1
		//_AmbientColor("AmbientColor", color) = (1,1,1,1)
		_Distortion("Distortion", range(0,128)) = 32
		_Smoothness("Smoothness", range(0,1)) = 0.5
		_SpecColor("SpecularColor", color) = (1,1,1,1)

		// Normals
		[KeywordEnum(Single,Dual,FlowMap,Facet)]
		_NormalsMode("NormalsMode", Float) = 0

		_NormalMapA("Normal Map A", 2D) = "bump" {}
		_NormalMapATilings("Normal Map A: Tilings", vector) = (1,1,1,1)
		_NormalMapASpeeds("Normal Map A: Speeds", vector) = (1,1,0.5,0.5)
		_NormalMapAIntensity("Normal Map A: Intensity", Range(0,1)) = 1

		_NormalMapB("Normal Map B", 2D) = "bump" {}
		_NormalMapBTilings("Normal Map B: Tilings", vector) = (1,1,1,1)
		_NormalMapBSpeeds("Normal Map B: Speeds", vector) = (1,1,1,1)
		_NormalMapBIntensity("Normal Map B: Intensity", Range(0,1)) = 1

		_FlowMap("Flow Map ", 2D) = "grey" {}
		_FlowTiling("Flow Tiling", vector) = (1,1,0,0)
		_FlowSpeed("Flow Speed", float) = 5
		_FlowIntensity("Flow Intensity", float) = 0.1

		[Toggle(_NORMAL_FAR_ON)] _NormalFar("Enable Far Map", Float) = 0
		_NormalMapFar("Normal Map Far", 2D) = "bump" {}
		_NormalMapFarTilings("Normal Map Far: Tilings", vector) = (1,1,0.5,0.5)
		_NormalMapFarSpeeds("Normal Map Far: Speeds", vector) = (1,1,1,1)
		_NormalMapFarIntensity("Normal Map Far: Intensity", Range(0,1)) = 1
		_NormalFarDistance("Fade Distance", float) = 200


		// EdgeFade
		[Toggle(_EDGEFADE_ON)] _EdgeFade("Enable EdgeFade", Float) = 0
		_EdgeSize("EdgeFade: Size", float) = 1

		// Foam
		[Toggle(_FOAM_ON)] _Foam("Enable Foam", Float) = 0
		_FoamColor("Foam: Color", color) = (1,1,1,1)
		_FoamTex("Foam: Texture", 2D) = "white" {}
		_FoamTiling("Foam: Tiling", vector) = (1,1,1,1)
		_FoamSize("Foam: Size", float) = 0.1
		[Toggle(_FOAM_RIPPLE_ON)] _FoamRipples("Enable Foam Ripples", Float) = 0
		_FoamRippleDistance("Foam: RippleDistance", float) = 1
		_FoamRippleSize("Foam: RippleSize", float) = 1
		_FoamRippleSpeed("Foam: RippleSpeed", float) = 0.1
		_FoamDistortion("Foam: Distortion", float) = 0.1

		[Toggle(_FOAM_WHITECAPS_ON)] _FoamWhiteCaps("Enable White Caps", Float) = 0
		_FoamCapsIntensity("Foam: Caps Intensity", float) = 1
		_FoamCapsRangeMin("Foam: Caps Range Min", float) = 0
		_FoamCapsRangeMax("Foam: Caps Range Max", float) = 1

		// Caustics
		[Toggle(_CAUSTICS_ON)] _Caustics("Enable Caustics", Float) = 0
		[KeywordEnum(2D, 3D)] _CausticsMode("Use 3D Texture", Float) = 0
		[Toggle(_CAUSTICS_ANGLE_ON)] _CausticsAngleMask("Caustics Angle Mask", Float) = 0
		[Toggle(_CAUSTICS_DIRECTION_ON)] _CausticsDirectionMask("Caustics Direction Mask", Float) = 0

		_CausticsTex("Caustics Texture", 2D) = "black" {}
		_CausticsTex3D("Caustics Texture", 3D) = "black" {}
		_CausticsTiling3D("Caustics Tiling", float) = 0.5
		_CausticsSpeed3D("Caustics Speed", vector) = (1,1,1,1)
		_CausticsIntensity("Intensity", float) = 4
		_CausticsTiling("Caustics Tiling", vector) = (1,1,1,1)
		_CausticsStart("Start", float) = 0
		_CausticsEnd("End", float) = 1
		_CausticsSpeed("Speed", vector) = (1,1,-1,-1)
		_CausticsDistortion("Distortion", Range(0,1)) = 0.1

		// Scattering
		[Toggle(_SCATTERING_ON)] _Scattering("Enable Scattering", Float) = 0
		_ScatteringColor("Scattering: Color", color) = (1,1,1,1)
		_ScatteringIntensity("Scattering: Intensity", Float) = 1
		_ScatteringRangeMin("Scattering: Min", Float) = 0
		_ScatteringRangeMax("Scattering: Max", Float) = 1

		[Toggle(_CAPS_SCATTERING_ON)] _CapsScattering("Enable Caps Scattering", Float) = 0
		_CapsScatteringIntensity("Intensity", Float) = 1
		_CapsScatteringRangeMin("Min", Float) = 0
		_CapsScatteringRangeMax("Max", Float) = 1
		_CapsScatterNormals("Normals", Range(0,1)) = 0

		//[Header(Reflections)]
		[KeywordEnum(Off,CubeMap,Probes,RealTime)]
		_ReflectionMode("Reflection: Mode", Float) = 0
		_CubemapTexture("Reflection: Cubemap", CUBE) = "" {}
		_ReflectionFresnel("Reflection: Fresnel", Range(0,16)) = 4
		_ReflectionFresnelNormal("Reflection: Fresnel Normal", Range(0,1)) = 0.25
		_ReflectionIntensity("Reflection: Intensity", Range(0,1)) = 1
		_ReflectionDistortion("Reflection: Distortion", Range(0,1)) = 0.5
		_ReflectionRoughness("Reflection: Roughness", Range(0,1)) = 0.1

		[KeywordEnum(Off,Gerstner)]
		_DisplacementMode("Displacement: Mode", Float) = 0
		
		_WaveNormal("Wave Normal", Range(0,1)) = 1
		_WaveEffectsBoost("Effect Boost", float) = 0
		_WaveCount("Count", Range(1,32)) = 1
		_WaveLength("Length", float) = 8
		_WaveAmplitude("Amplitude", Range(0, 1)) = 1
		_WaveSteepness("Steepness", Range(0,1)) = 0.5
		_WaveSpeed("Speed", float) = 10
		_WaveStartDistance("Start Distance", float) = 0
		_WaveEndDistance("End Distance", float) = 1000

		[Toggle(_DYNAMIC_EFFECTS_ON)] _DynamicEffects("Enable Dynamic Effects", Float) = 0
		_DynamicNormal("Normal Strength", range(0, 4)) = 1
		_DynamicDisplacement("Displacement Strength", float) = 1
		_DynamicFoam("Foam Strength", range(0,1)) = 1

		// Options
		[Toggle(_DOUBLE_SIDED_ON)] _DoubleSided("Double Sided", Float) = 0
		[Toggle(_ZWRITE_ON)] _ZWrite("ZWrite", Float) = 1
		[Toggle(_WORLD_UV)] _WorldUV("WorldSpace UV", Float) = 0
		[Toggle(_ORTHO_ON)] _Orthographic("Orthographic", Float) = 0


		// Vertex Paint
		[Toggle(_ADD_FOAM_ON)] _AddFoam("R: Add Foam", Float) = 0
		[Toggle(_SPEED_MUL_ON)] _SpeedMul("G: Speed Multiplier", Float) = 0
		[Toggle(_DISPLACEMENT_MASK_ON)] _DispMask("B: Displacement Mask", Float) = 0
		[Toggle(_ALPHA_MASK_ON)] _AlphaMask("A: Alpha Mask", Float) = 0

		// Gradient Keys
		[HideInInspector] _RefractionKey1("Key1", vector) = (-1,-1,-1,-1)
		[HideInInspector] _RefractionKey2("Key2", vector) = (-1,-1,-1,-1)
		[HideInInspector] _RefractionKey3("Key3", vector) = (-1,-1,-1,-1)
		[HideInInspector] _RefractionKey4("Key4", vector) = (-1,-1,-1,-1)
		[HideInInspector] _RefractionKey5("Key5", vector) = (-1,-1,-1,-1)
		[HideInInspector] _RefractionKey6("Key6", vector) = (-1,-1,-1,-1)
		[HideInInspector] _RefractionKey7("Key7", vector) = (-1,-1,-1,-1)
		[HideInInspector] _RefractionKey8("Key8", vector) = (-1,-1,-1,-1)
		[HideInInspector] _CullMode("CullMode", float) = 0

		// Ocean Info
		[HideInInspector] _OceanInfos("OceanInfo", vector) = (0,0,0,0)

	}

	SubShader
	{
		Tags{
		"RenderType" = "Transparent"
		"Queue" = "Transparent"
		"RenderPipeline" = "UniversalPipeline"
		}

		Pass
		{
			Name "Front"
			Tags { "LightMode" = "UniversalForward" }
			Cull [_CullMode]
			ZWrite [_ZWrite]

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#define MAINPASS	

			// Lighting
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

			// URP Water Options
			#pragma shader_feature_local _COLORMODE_COLORS _COLORMODE_GRADIENT
			#pragma shader_feature_local _NORMALSMODE_SINGLE _NORMALSMODE_DUAL _NORMALSMODE_FLOWMAP _NORMALSMODE_FACET
			#pragma shader_feature_local _NORMAL_FAR_ON
			#pragma shader_feature_local _ _REFLECTIONMODE_CUBEMAP _REFLECTIONMODE_PROBES _REFLECTIONMODE_REALTIME

			#pragma shader_feature_local _CAUSTICS_ON
			#pragma shader_feature_local _CAUSTICSMODE_2D _CAUSTICSMODE_3D
			#pragma shader_feature_local _CAUSTICS_ANGLE_ON
			#pragma shader_feature_local _CAUSTICS_DIRECTION_ON
		
			#pragma shader_feature_local _SCATTERING_ON
			#pragma shader_feature_local _CAPS_SCATTERING_ON
			#pragma shader_feature_local _EDGEFADE_ON
			#pragma shader_feature_local _FOAM_ON
			#pragma shader_feature_local _FOAM_RIPPLE_ON
			#pragma shader_feature_local _FOAM_WHITECAPS_ON
			#pragma shader_feature_local _DISPLACEMENTMODE_GERSTNER
			#pragma shader_feature_local _DYNAMIC_EFFECTS_ON
		
			#pragma shader_feature_local _DOUBLE_SIDED_ON
			#pragma shader_feature_local _ZWRITE_ON
			#pragma shader_feature_local _WORLD_UV
			#pragma shader_feature_local _ORTHO_ON


			#pragma shader_feature_local _ADD_FOAM_ON
			#pragma shader_feature_local _SPEED_MUL_ON
			#pragma shader_feature_local _DISPLACEMENT_MASK_ON
			#pragma shader_feature_local _ALPHA_MASK_ON
			#pragma multi_compile _ _OCEAN_MASK_ON

			// GPU Instancing
			#pragma multi_compile_instancing
			//Fog
			#pragma multi_compile_fog
			#pragma target 3.0

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			// Enviro Support
			//#include "../../../Enviro - Sky and Weather/Core/Resources/Shaders/Core/EnviroFogCore.hlsl"


			//#include "Includes/URPWaterFog.hlsl"  
			#include "Includes/URPWaterVariables.hlsl"  
			#include "Includes/URPWaterHelpers.hlsl"  
			#include "Includes/URPWaterLighting.hlsl"
			#include "Includes/URPWaterScattering.hlsl"
			#include "Includes/URPWaterRefraction.hlsl"
			#include "Includes/URPWaterReflections.hlsl" 
			#include "Includes/URPWaterNormals.hlsl"
			#include "Includes/URPWaterFoam.hlsl"
			#include "Includes/URPWaterAlpha.hlsl"
			#include "Includes/URPWaterWave.hlsl"
			#include "Includes/URPWaterDynamic.hlsl"
			#include "Includes/URPWaterCommon.hlsl"  

			
		
			ENDHLSL
		}
	}
	CustomEditor "URPWaterEditor"
}
