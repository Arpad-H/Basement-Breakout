Shader "Custom/SimpleWaveShader"
{
    Properties
    {
        _NoiseTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldPos : WORLD_POSITION;
                float3 vertNormal : NORMAL;
                float2 scrollOffset : SCROLL_OFFSET;
            };

            //WAVE GENERATION
            float _Wave1Amplitude;
            float _Wave1Length;
            float _Wave1Speed;
            float _Wave2Amplitude;
            float _Wave2Length;
            float _Wave2Speed;
            float _Wave3Amplitude;
            float _Wave3Length;
            float _Wave3Speed;

            //WATER EFFECTS
            float4 _Color; 
            float _NormalStrength;
            float _FresnelPower;
            float _RefractionStrength;
            float _ReflectionStrength;
            float4 _SpecularColor;
            float _Shininess;
            float _WaveSpeed;

            sampler2D _NoiseTex;
            float _NoiseScale;
            float _NoiseStrength;
            float4 _ScrollSpeed;


            half Fresnel(float3 viewDir, float3 worldNormal, float power)
            {
                return pow(1.0 - saturate(dot(viewDir, worldNormal)), 2);
            }

            float CalculateWaveHeight(float3 pos, float4 noiseValue)
            {
                float wave = _Wave1Amplitude * sin(pos.x * _Wave1Length + _Time.y * _Wave1Speed)
                    + _Wave2Amplitude * sin(pos.x * _Wave2Length + _Time.y * _Wave2Speed)
                    + _Wave3Amplitude * sin(pos.z * _Wave3Length + _Time.y * _Wave3Speed);

                return wave * _NoiseStrength * (noiseValue.r + noiseValue.g + noiseValue.b) / 3.0;
            }

            float3 CalculateNormal(float3 worldPos, float noiseValue)
            {
                float offset = 0.01; 

                float waveHeightCenter = CalculateWaveHeight(worldPos, noiseValue);
                float waveHeightX = CalculateWaveHeight(worldPos + float3(offset, 0, 0), noiseValue);
                float waveHeightZ = CalculateWaveHeight(worldPos + float3(0, 0, offset), noiseValue);

                
                float3 tangentX = float3(1, (waveHeightX - waveHeightCenter) / offset, 0);
                float3 tangentZ = float3(0, (waveHeightZ - waveHeightCenter) / offset, 1);

               
                return normalize(cross(tangentZ, tangentX));
            }


            v2f vert(appdata v)
            {
                v2f o;

                o.uv = v.uv;
                float2 scrollOffset = _Time.y * _ScrollSpeed.xy;
                o.scrollOffset = scrollOffset;

                float4 noiseValue = tex2Dlod(_NoiseTex, float4((v.uv * _NoiseScale + scrollOffset), 0, 0));


                v.vertex.y += CalculateWaveHeight(v.vertex, noiseValue);
                o.vertNormal = CalculateNormal(v.vertex, noiseValue);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float fresnelEffect = Fresnel(normalize(UnityWorldSpaceViewDir(i.worldPos)), normalize(i.vertNormal),
                _FresnelPower);
                float4 baseColor = _Color;
                float4 reflectionColor = _ReflectionStrength * fresnelEffect * _SpecularColor;
                float4 refractionColor = _RefractionStrength * (1.0 - fresnelEffect) * _Color;
               // return lerp(refractionColor, reflectionColor, fresnelEffect) * baseColor;
                // return float4(UnityWorldSpaceViewDir(i.vertex),1);
                return float4(fresnelEffect,0,0, 1);
                return float4(1, 1, 1, 1) * tex2D(_NoiseTex, i.uv + i.scrollOffset) + float4(0, 0, 1, 0);
            }
            ENDCG
        }
    }
}