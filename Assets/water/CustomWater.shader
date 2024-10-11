Shader "Custom/SimpleWaveShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Amplitude ("Amplitude", Float) = 0.1
        _Frequency ("Frequency", Float) = 1.0
        _Speed ("Speed", Float) = 1.0
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
            };

            float _Wave1Amplitude;
            float _Wave1Length;
            float _Wave1Speed;
            float _Wave2Amplitude;
            float _Wave2Length;
            float _Wave2Speed;
            float _Wave3Amplitude;
            float _Wave3Length;
            float _Wave3Speed;

            float4 _Color;

            v2f vert(appdata v)
            {
                v2f o;

                // Pass the UVs through to the fragment shader
                o.uv = v.uv;

                // Calculate the wave offset using a sum of sines
                float wave = _Wave1Amplitude * sin(v.vertex.x * _Wave1Length + _Time.y * _Wave1Speed)
                    + _Wave2Amplitude * sin(v.vertex.x * _Wave2Length + _Time.y * _Wave2Speed)
                    + _Wave3Amplitude * sin(v.vertex.z * _Wave3Length + _Time.y * _Wave3Speed);

                // Add the wave offset to the y-component of the vertex position
                v.vertex.y += wave;

                // Transform the vertex position to clip space
                o.vertex = UnityObjectToClipPos(v.vertex);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}