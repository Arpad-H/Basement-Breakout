Shader "Custom/TranslucentMaterial"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _LocalThickness ("Local Thickness", 2D) = "gray" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Distortion ("Material Light Distortion", Range(0, 1)) = 0.1
        _Power ("Power", Float) = 0.0
        _Scale ("Scale", Range(1.0, 100.0)) = 1.0
        _Attenuation ("Attenuation", Range(0.0, 1.0)) = 1.0
        _Ambient ("Ambient Light", Range(0.0, 100.0)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardTranslucent fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _LocalThickness;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float _Distortion;

        float _Power;
        float _Scale;

        float thickness;

        float _Attenuation;
        float _Ambient;

        #include "UnityPBSLighting.cginc"

        inline fixed4 LightingStandardTranslucent(SurfaceOutputStandard s, fixed3 viewDirection, UnityGI gi) {
            fixed4 standardPBR = LightingStandard(s, viewDirection, gi);

            /**************************TRANSLUCENT LIGHTING PART*******************************/
            //

            // Light Direction
            float3 L = gi.light.dir;
            // View Direction
            float3 V = viewDirection;
            // Normal of the current Vertex
            float3 N = s.Normal;

            // Half way vector
            float3 H = normalize(L + N * _Distortion);
            // Intensity of the backlight
            float I = pow(saturate(dot(V, -H)), _Power) * _Scale;

            I = _Attenuation * (I + _Ambient) * thickness;
            standardPBR.rgb = standardPBR.rgb + gi.light.color * I;
            //

            return standardPBR;
        }

        void LightingStandardTranslucent_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
        {
            LightingStandard_GI(s, data, gi);        
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            float thick = tex2D(_LocalThickness, IN.uv_MainTex).r;

            thickness = thick;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
