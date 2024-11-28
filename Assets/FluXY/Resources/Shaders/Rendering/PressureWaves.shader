Shader "Fluxy/Rendering/Pressure Waves"
{
    Properties
    {
        _Color ("Color", Color) = (0,0.5,1, 1)
        _Falloff("Edge Falloff", Range(0.0, 1.0)) = 0.2
        _InvFade ("Soft Particles Factor", Range(0.01,8.0)) = 1.0
    }
    SubShader
    {
        Cull Off
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

        ZWrite Off

        Pass
        {
            Name "Render"
            Blend One OneMinusSrcAlpha
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles

            #include "UnityCG.cginc"
            #include "../FluidUtils.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

                #ifdef SOFTPARTICLES_ON
                float4 projPos : TEXCOORD1;
                #endif
            };

            float4 _Detail_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                #ifdef SOFTPARTICLES_ON
                o.projPos = ComputeScreenPos (o.vertex);
                COMPUTE_EYEDEPTH(o.projPos.z);
                #endif

                return o;
            }
           
            sampler2D _MainTex;
  
            float4 _Color;
            float _Falloff;
            float _InvFade;

            #ifdef SOFTPARTICLES_ON
            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
            #endif
            
            float4 frag (v2f i) : SV_Target
            {
                float2 uv = TileToUV(i.uv,_TileIndex);
                float4 velocity = tex2D(_Velocity,uv);
                
                float3 normal;
                NormalFromPressure_float(uv,1,normal);
                float curvature = 1 - dot(normal,float3(0,0,1));
                float4 color = _Color * curvature;
                
                #ifdef SOFTPARTICLES_ON
                float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                float partZ = i.projPos.z;
                float fade = saturate (_InvFade * (sceneZ-partZ));
                color.a *= fade;
                #endif
                
                // edge falloff
                float falloff = SquareFalloff(i.uv,_Falloff);
                
                return color * falloff;
            }
            ENDHLSL
        }
      
    }
}
