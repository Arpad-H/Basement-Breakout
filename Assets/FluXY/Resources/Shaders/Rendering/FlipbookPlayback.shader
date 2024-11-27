Shader "Fluxy/Rendering/Flipbooks/FlipbookPlayback"
{
    Properties
    {
        _MainTex ("Density flipbook", 2D) = "white" {}
        _Velocity ("Velocity flipbook", 2D) = "white" {}
        _Detail ("Detail", 2D) = "white" {}
        _Gradient ("Gradient", 2D) = "white" {}
        
        _Duration ("Duration (seconds)", Float) = 2
        _FrameCount ("Total frames", Integer) = 16
        _Columns ("Columns", Integer) = 4

        _PlaybackSpeed ("Playback Speed", Float) = 1
        _PlaybackOffset("Playback Offset", Float) = 0

        _DetailAdvection("Detail Advection", Range(0.0, 1.0)) = 0.5
        _Additiveness("Additiveness", Range(0.0, 1.0)) = 0
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
            sampler2D _Detail;
            sampler2D _Gradient;
            
            float _DetailAdvection;
            float _Additiveness;
            float _Falloff;
            float _InvFade;

            float _Duration;
            int _FrameCount;
            int _Columns;

            float _PlaybackSpeed;
            float _PlaybackOffset;

            #ifdef SOFTPARTICLES_ON
            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
            #endif
            
            float4 frag (v2f i) : SV_Target
            {
                float frameDuration = _Duration / _FrameCount;
                int rows = ceil(_FrameCount / (float)_Columns);
                float currentFrame = (_Time.y + _PlaybackOffset) / frameDuration * _PlaybackSpeed;
                float2 uv = FlipbookUV(i.uv, rows, _Columns, _FrameCount, currentFrame);
           
                float2 velocity = UnpackVelocity(tex2D(_Velocity,uv).xy);
                float4 state = tex2D(_MainTex, uv);

                // look up detail map:
                float detail = tex2D_flowmap(_Detail,TRANSFORM_TEX(i.uv, _Detail), velocity, _DetailAdvection).r;
                
                // lookup gradient:
                float4 grad = tex2D(_Gradient, float2(state.a * detail.r,0.5f));
                
                #ifdef SOFTPARTICLES_ON
                float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                float partZ = i.projPos.z;
                float fade = saturate (_InvFade * (sceneZ-partZ));
                state.a *= fade;
                #endif
                
                // calculate additiveness (using premultiplied alpha)
                float4 color = AlphaAdditiveBlend(grad * state,_Additiveness);

                // edge falloff
                float falloff = SquareFalloff(i.uv,_Falloff);
                
                return color * falloff;
            }
            ENDHLSL
        }
      
    }
}
