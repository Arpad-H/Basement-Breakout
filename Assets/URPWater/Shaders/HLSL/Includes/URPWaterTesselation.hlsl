//-----------------------------------------------------------------------------
// Copyright(C) Yan Verde - All Rights Reserved
// Copyright protected under Unity Asset Store EULA
// Refer to https://unity3d.com/legal/as_terms for more informations
// -----------------------------------------------------------------------------
// URP Water
// Author : Yan Verde
// Date : April 10, 2021
// -----------------------------------------------------------------------------

#ifndef URPWATER_TESSELATION_INCLUDED
#define URPWATER_TESSELATION_INCLUDED
//#ifdef UNITY_CAN_COMPILE_TESSELLATION
#ifdef TESSELLATION_ON
#define UNITY_CAN_COMPILE_TESSELLATION 1

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

float UnityCalcEdgeTessFactor(float3 wpos0, float3 wpos1, float edgeLen)
{
	// distance to edge center
	float dist = distance(0.5 * (wpos0 + wpos1), _WorldSpaceCameraPos);
	// length of the edge
	float len = distance(wpos0, wpos1);
	// edgeLen is approximate desired size in pixels
	float f = max(len * _ScreenParams.y / (edgeLen * dist), 1.0);
	return f;
}

// Desired edge length based tessellation:
// Approximate resulting edge length in pixels is "edgeLength".
// Does not take viewing FOV into account, just flat out divides factor by distance.
float4 UnityEdgeLengthBasedTess(float4 v0, float4 v1, float4 v2, float edgeLength)
{
	float3 pos0 = mul(unity_ObjectToWorld, v0).xyz;
	float3 pos1 = mul(unity_ObjectToWorld, v1).xyz;
	float3 pos2 = mul(unity_ObjectToWorld, v2).xyz;
	float4 tess;
	tess.x = UnityCalcEdgeTessFactor(pos1, pos2, edgeLength);
	tess.y = UnityCalcEdgeTessFactor(pos2, pos0, edgeLength);
	tess.z = UnityCalcEdgeTessFactor(pos0, pos1, edgeLength);
	tess.w = (tess.x + tess.y + tess.z) / 3.0f;
	return tess;
}


struct TessVertex 
{
	float4 vertex 	: INTERNALTESSPOS;
	float3 normal 	: NORMAL;
	float4 tangent 	: TANGENT;
	float2 texcoord : TEXCOORD0;
#ifdef _DISPLACEMENTMODE_GERSTNER
	float2 waveHeight : TEXCOORD1;
#endif
	float4 color 	: COLOR;

	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct OutputPatchConstant 
{
	float edge[3]: SV_TessFactor;
	float inside : SV_InsideTessFactor;
};

TessVertex tessvert(Attributes v) 
{
	TessVertex o;

	o.vertex = v.vertex;
	o.normal = v.normal;
	o.tangent = v.tangent;
	o.texcoord = v.texcoord;
#ifdef _DISPLACEMENTMODE_GERSTNER
	o.waveHeight = v.waveHeight;
#endif
	o.color 	= v.color;

	UNITY_TRANSFER_INSTANCE_ID(v, o);

	return o;
}

float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2) 
{
	return UnityEdgeLengthBasedTess(v.vertex, v1.vertex, v2.vertex, 32 - _Tess);
}

OutputPatchConstant hullconst(InputPatch<TessVertex, 3> v) 
{
	OutputPatchConstant o;
	float4 ts = Tessellation(v[0], v[1], v[2]);
	o.edge[0] = ts.x;
	o.edge[1] = ts.y;
	o.edge[2] = ts.z;
	o.inside = ts.w;
	return o;
}

[domain("tri")]
[partitioning("fractional_odd")]
[outputtopology("triangle_cw")]
[patchconstantfunc("hullconst")]
[outputcontrolpoints(3)]
TessVertex HullFunction(InputPatch<TessVertex, 3> v, uint id : SV_OutputControlPointID) 
{
	return v[id];
}

[domain("tri")]
Varyings DomainFunction(OutputPatchConstant tessFactors, const OutputPatch<TessVertex, 3> patch, float3 bary : SV_DomainLocation) 
{
	#define DOMAIN_INTERPOLATE(fieldName) v.fieldName = patch[0].fieldName * bary.x + patch[1].fieldName * bary.y + patch[2].fieldName * bary.z;

	Attributes v = (Attributes)0;
	UNITY_TRANSFER_INSTANCE_ID(patch[0], v);


	DOMAIN_INTERPOLATE(vertex);
	DOMAIN_INTERPOLATE(texcoord);
	DOMAIN_INTERPOLATE(tangent);
	DOMAIN_INTERPOLATE(normal);
	DOMAIN_INTERPOLATE(color);
	#ifdef _DISPLACEMENTMODE_GERSTNER
	v.waveHeight = patch[0].waveHeight.x * bary.x + patch[1].waveHeight.x * bary.y + patch[2].waveHeight.x * bary.z;
	#endif

	Varyings o = vert(v);
	
	return o;
}
#endif
#endif