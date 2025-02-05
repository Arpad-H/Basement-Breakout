#ifndef URPWATER_BLUR_INCLUDED
#define URPWATER_BLUR_INCLUDED

//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

static const float _Offset[] = { 0.0, 1.0, 2.0, 3.0, 4.0 };
static const float _Weight[] = { 0.2270270270, 0.1945945946, 0.1216216216,0.0540540541, 0.0162162162 };


struct appdataBlur
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
};


struct v2fBlur
{
	float4 vertex : SV_POSITION;
	float2 uv : TEXCOORD0;
};

v2fBlur vertBlur(appdataBlur v)
{
	v2fBlur o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = v.uv;

	return o;
}

float4 fragBlur(v2fBlur IN) : SV_Target
{
	float2 Offset = float2(1,0);

	#ifdef BLUR_H
	Offset = float2(1, 0);
	#endif

	#ifdef BLUR_V
	Offset = float2(0, 1);
	#endif

	Offset *= _MainTex_TexelSize.xy * _BlurStrength;


	float4 SharpColor = tex2D(_MainTex, IN.uv);
	float4 FragmentColor = SharpColor * _Weight[0];

	
	UNITY_LOOP
	for (int i = 1; i < 5; i++)
	{
		float2 o = _Offset[i] * Offset;

		FragmentColor += tex2D(_MainTex, IN.uv + o) * _Weight[i];
		FragmentColor += tex2D(_MainTex, IN.uv - o) * _Weight[i];
	}

	FragmentColor = saturate(FragmentColor);
	FragmentColor.b = 0.5;

	return FragmentColor;	
}

#endif