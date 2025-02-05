//-----------------------------------------------------------------------------
// Copyright(C) Yan Verde - All Rights Reserved
// Copyright protected under Unity Asset Store EULA
// Refer to https://unity3d.com/legal/as_terms for more informations
// -----------------------------------------------------------------------------
// URP Water
// Author : Yan Verde
// Date : April 10, 2021
// -----------------------------------------------------------------------------

#ifndef URPWATER_WAVE_INCLUDED
#define URPWATER_WAVE_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"   
#include "URPWaterHelpers.hlsl"
#include "URPWaterVariables.hlsl"


static uint DCount = 7;
static float2 Directions[] =
{
	float2(0.53, 0.45),
	float2(-0.209, 0.4),
	float2(-0.125, 0.592),
	float2(0.482, -0.876),
	float2(-0.729, -0.694),
	float2(-0.248, 0.968),
	float2(0.844, -0.538)
};

static uint LCount = 6;
static float Lengths[] =
{
	3.56,
	2.85,
	2.10,
	1.30,
	1.10,
	1.2
};

static uint SCount = 5;
static float SteepnessRange[] =
{
	1.0,
	1.8,
	1.6,
	1.25,
	0.5
};

static uint SpCount = 9;
static float Speeds[] =
{
	0.62,
	-0.8,
	0.45,
	-0.75,
	0.88,
	0.70,
	-0.56,
	0.35,
	-0.71
};

struct WaveData {
	float4 wave;
	float speed;
};

struct Wave
{
	float Length;
	float Steepness;
	float Speed;
	float Amplitude;
	float2 Direction;
};


//#define SOLVE_NORMAL_Z 1
#define SteepnessThreshold 1
#define GRAVITY 9.8

void SingleGerstnerWave(float3 WorldPos, Wave w, out float3 WPO, out float3 Normal)
{

	float dispersion = 6.28318 / w.Length;
	float c = sqrt(GRAVITY / dispersion) * w.Speed;
	float2 d = w.Direction;
	float Steepness = w.Steepness;
	float Speed = w.Speed;

	float f = dispersion * (dot(d, WorldPos.xz) - c * _Time.x);

	float cf;
	float sf;
	sincos(f, sf, cf);

	float a = Steepness / (dispersion * 1.5);
	float wKA = a * dispersion;

	WPO.xz = d.xy * (a * cf);
	WPO.y = a * sf;

	Normal.xz = -(cf * wKA * d);
	Normal.y = 0;

	/*
	#if SOLVE_NORMAL_Z
		Normal.y = sf * w.Steepness * saturate((a * SteepnessThreshold) / w.Length);
	#else
		Normal.y = 0;
	#endif
	*/
}

void ComputeWaves(inout Attributes v)
{
	
	#if _DISPLACEMENTMODE_GERSTNER
	
	float3 WorldOffset;
	float3 WorldNormal;
	float3 wPos = TransformObjectToWorld(v.vertex.xyz);

	float steepnessAtten = 1.0;
	

	//for start
	UNITY_LOOP
	for (int i = 0; i < _WaveCount; i++)
	{

		float3 currentOffset;
		float3 currentNormal;

		float steepnessMul = lerp(1.0, 0.1, (1.0 / 32.0) * i);

		Wave w;
		w.Length = Lengths[i % LCount] * _WaveLength;
		w.Steepness = SteepnessRange[i % SCount] * _WaveSteepness * steepnessMul;
		w.Speed = Speeds[i % SpCount] * _WaveSpeed;
		w.Direction = normalize(Directions[i % DCount]);

		SingleGerstnerWave(wPos, w, currentOffset, currentNormal);

		WorldOffset += currentOffset;
		WorldNormal += currentNormal;
	}
	//for end

	
	float amplitude = _WaveAmplitude;

	// Distance Fade
	float cameraDistance = length(_WorldSpaceCameraPos - wPos);
	float distanceMask = 1.0 - saturate(InverseLerp(_WaveStartDistance, _WaveEndDistance, cameraDistance));
	amplitude *= distanceMask;


	#if _DISPLACEMENT_MASK_ON
	amplitude *= v.color.b;
	#endif

	#if _OCEAN_MASK_ON
	float2 pos = _OceanInfos.xy;
	float2 size = _OceanInfos.zw;

	float2 uv = CaptureUV(wPos.xz, pos, size);
	amplitude *= CaptureMask(uv, 0.4);
	#endif


	float fakeOffset = (WorldOffset.y + _WaveEffectsBoost) - v.vertex.y;

	v.waveHeight = amplitude * fakeOffset;

	v.vertex.xyz = TransformWorldToObject(wPos + WorldOffset * amplitude.xxx);

	WorldNormal.xyz = normalize(float3(WorldNormal.x, 1.0 - WorldNormal.y, WorldNormal.z));
	v.normal = lerp(v.normal, WorldNormal, amplitude * _WaveNormal);

	#endif
}

#endif
