Shader "Custom/UnderwaterDistortion"
{
    Properties
    {
        _DistortionStrength ("Distortion Strength", Range(0, 0.1)) = 0.02
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _WaterDepth ("Water Depth", Float) = 0.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            float _DistortionStrength;
            float _WaterDepth;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv * 5.0).rg;
                float2 distortion = (noise - 0.5) * _DistortionStrength;

                float2 uvDistorted = IN.uv + distortion;

                half4 color = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uvDistorted);
                
                // Slight blue tint for underwater effect
                color.rgb = lerp(color.rgb, float3(0.1, 0.3, 0.6), _WaterDepth);

                return color;
            }
            ENDHLSL
        }
    }
}
