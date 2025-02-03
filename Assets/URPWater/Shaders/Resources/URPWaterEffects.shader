// Copyright(C) Yan Verde - All Rights Reserved
// Copyright protected under Unity Asset Store EULA
// Refer to https://unity3d.com/legal/as_terms for more informations
// -----------------------------------------------------------------------------
// URP Water
// Author : Yan Verde
// Date : April 10, 2021
// -----------------------------------------------------------------------------

Shader "Hidden/URPWaterEffects"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_EdgeMaskSize ("EdgeMask Size", float) = 0.1
    }

    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

		CGINCLUDE

		CBUFFER_START(UnityPerMaterial)
		sampler2D _MainTex;
		float4 _MainTex_TexelSize;
		float _EdgeMaskSize;
		CBUFFER_END

		sampler2D _BaseTex;
		sampler2D _NormalMap;
		
		float _NormalStrength;
		float _BlurStrength;
		
		

		float3 RemapNormals(float3 from, float3 to, float3 value) {
			return (value - from) / (to - from);
		}

		float3 SafeNormalize(float3 inVec)
		{
			float dp3 = max(0.001, dot(inVec, inVec));
			return inVec * rsqrt(dp3);
		}

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
			
		};

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			return o;
		}

		ENDCG


        Pass
        {
			name "Base"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			float4 frag(v2f i) : SV_Target
			{
				// Original
				float3 duv = float3(_MainTex_TexelSize.xy, 0);
				float4 v0 = tex2D(_MainTex, i.uv);

				// Create Normal
				float v1 = tex2D(_MainTex, i.uv - duv.xz).r;
				float v2 = tex2D(_MainTex, i.uv + duv.xz).r;
				float v3 = tex2D(_MainTex, i.uv - duv.zy).r;
				float v4 = tex2D(_MainTex, i.uv + duv.zy).r;

				float BlurMult = lerp(1, 16, _BlurStrength * 0.25);
				float3 n = SafeNormalize(float3(v2 - v1, v4 - v3, 0.0)) * _NormalStrength * BlurMult;
				
				// Blend with heightmap for smooth borders
				n *= v0.rrr;

				// Put back to 0-1 range in order to use non HDR texture.
				n = saturate(RemapNormals(-1, 1, n));

				return float4(n.x, n.y, v0.b, v0.a);
			}
            ENDCG
        }

		Pass
		{
			name "BlurV"
			CGPROGRAM
			
			#pragma vertex vertBlur
			#pragma fragment fragBlur

			#define BLUR_H
			#include "UnityCG.cginc"
			#include "URPWaterBlur.hlsl"
			

			ENDCG
		}

		Pass
		{
			name "BlurV"
			CGPROGRAM

			#pragma vertex vertBlur
			#pragma fragment fragBlur

			#define BLUR_V
			#include "UnityCG.cginc"
			#include "URPWaterBlur.hlsl"
			

			ENDCG
		}

		Pass
		{
			name "CompositeBlur"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragComp

			#include "UnityCG.cginc"

			float4 fragComp(v2f i) : SV_Target
			{
				// Blurred / Normals
				float4 NormalsTex = tex2D(_MainTex, i.uv);
				// Original
				float4 BaseTex = tex2D(_BaseTex, i.uv);

				float2 edgeMaskUV = abs(i.uv * 2 - 1);
				float edgeMask = 1 - max(edgeMaskUV.x, edgeMaskUV.y);
				float mask = smoothstep(0, _EdgeMaskSize, edgeMask);

				BaseTex.ba *= mask;
				NormalsTex.a *= mask;

				return float4(NormalsTex.rg, BaseTex.b, NormalsTex.a);
			}

			ENDCG
		}
    }
}
