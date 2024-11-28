Shader "Fluxy/Editor/Flipbooks/FlipbookPreview"
{
    Properties
    {
        _MainTex ("Density flipbook", 2D) = "white" {}
        _Velocity ("Velocity flipbook", 2D) = "white" {}    
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
            #pragma multi_compile __ INTERPOLATION

            #include "UnityCG.cginc"
            #include "../../../Resources/Shaders/FluidUtils.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 clipUV : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            float4 _Detail_ST;
            uniform float4x4 unity_GUIClipTextureMatrix;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                float3 eyePos = UnityObjectToViewPos(v.vertex);
                o.clipUV = mul(unity_GUIClipTextureMatrix, float4(eyePos.xy, 0, 1.0));

                o.uv = v.uv;

                return o;
            }
           
            sampler2D _MainTex;
            sampler2D _GUIClipTexture;
            float _PlaybackSpeed;
            float _Duration;
            float _AspectRatio;
            float _Interpolation;
            int _FrameCount;
            int _Columns;
            
            float4 frag (v2f i) : SV_Target
            {
                float frameDuration = _Duration / _FrameCount;
                int rows = ceil(_FrameCount / (float)_Columns);
                float currentFrame = _Time.y / frameDuration * _PlaybackSpeed;
                float2 uv = FlipbookUV(i.uv, rows, _Columns, _FrameCount, currentFrame);
            
                #if INTERPOLATION
                    float frameProgress = frac(currentFrame);
                    float2 frameSize = float2(_Columns, rows);
                    float2 uv2 = FlipbookUV(i.uv, rows, _Columns, _FrameCount, currentFrame + 1);
                   
                    float2 velocity = UnpackVelocity(tex2D(_Velocity,uv).xy);
                    float2 velocity2 = UnpackVelocity(tex2D(_Velocity,uv2).xy);
                    float4 state = tex2D(_MainTex, uv - velocity * frameProgress * frameDuration / frameSize * _Interpolation);
                    float4 state2 = tex2D(_MainTex, uv2 + velocity2 * (1 - frameProgress) * frameDuration / frameSize * _Interpolation);
                    state = lerp(state, state2, frameProgress);
                #else
                    float4 state = tex2D(_MainTex, uv);
                #endif

                float3 col1 = float3(0.1,0.1,0.1);
                float3 col2 = float3(0.2,0.2,0.2);
                float total = floor(i.uv.x * _AspectRatio * 16) + floor(i.uv.y * 16);
                float4 checker = float4(lerp(0.1, 0.2, step(fmod(total, 2.0), 0.5)).xxx, 1.0);

                return lerp(checker, state, state.a) * tex2D(_GUIClipTexture, i.clipUV).a;
            }
            ENDHLSL
        }
      
    }
}
