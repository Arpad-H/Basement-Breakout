Shader "Fluxy/Editor/BlitFlipbook"
{

    Properties
    {
        _MainTex ("Texture", any) = "" {}
    }
   
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Fog { Mode off }

        Pass
        {
            Name "Blit state"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            float4 _FrameRect;

           /*float2 VertexToFrame(in float2 v)
            {
                return (_FrameRect.xy + (v + 1) * 0.5 * _FrameRect.zw) * 2 - 1;
            }*/

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex.xy = VertexToFrame(o.vertex.xy, _FrameRect);
                o.uv = TRANSFORM_TEX(v.uv.xy, _MainTex);
                return o;
            }
   
            float4 frag (v2f i) : SV_Target
            {
                float2 uv = TileToUV(i.uv,_TileIndex);        
                return tex2D(_MainTex, uv);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Blit velocity"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
                float4 vertex : SV_POSITION;
            };

         
            sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            float4 _FrameRect;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex.xy = VertexToFrame(o.vertex.xy, _FrameRect);
                o.uv = TRANSFORM_TEX(v.uv.xy, _MainTex);

                return o;
            }
   
            float4 frag (v2f i) : SV_Target
            {
                float2 uv = TileToUV(i.uv,_TileIndex); 
                float4 velocity = tex2D(_MainTex, uv);         
                return float4(velocity.rg * 0.5 + 0.5,velocity.a * 2,1);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Crossfade state"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
                float2 flipbookCoord: TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _Flipbook;
            sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            float4 _FrameRect;
            float _Opacity;
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex.xy = VertexToFrame(o.vertex.xy, _FrameRect);
                o.flipbookCoord = o.vertex.xy *0.5+0.5;
                o.flipbookCoord.y = 1 - o.flipbookCoord.y;
                o.uv = TRANSFORM_TEX(v.uv.xy, _MainTex);
                return o;
            }
   
            float4 frag (v2f i) : SV_Target
            {
                float2 uv = TileToUV(i.uv,_TileIndex);        
                return lerp(tex2D(_Flipbook, i.flipbookCoord), tex2D(_MainTex, uv), _Opacity);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Crossfade velocity"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
                float2 flipbookCoord: TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _Flipbook;
            sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            float4 _FrameRect;
            float _Opacity;
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex.xy = VertexToFrame(o.vertex.xy, _FrameRect);
                o.flipbookCoord = o.vertex.xy *0.5+0.5;
                o.flipbookCoord.y = 1 - o.flipbookCoord.y;
                o.uv = TRANSFORM_TEX(v.uv.xy, _MainTex);
                return o;
            }
   
            float4 frag (v2f i) : SV_Target
            {
                float2 uv = TileToUV(i.uv,_TileIndex);   
  
float4 velocity = tex2D(_MainTex, uv);   
velocity = float4(velocity.rg * 0.5 + 0.5,velocity.a * 2,1);
   
                return lerp(tex2D(_Flipbook, i.flipbookCoord), velocity, _Opacity);
            }
            ENDHLSL
        }
      
    }
    Fallback Off
}
