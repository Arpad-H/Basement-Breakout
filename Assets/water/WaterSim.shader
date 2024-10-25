Shader "Custom/WebGLWaveShader"
{
    Properties {}
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
                float4 worldPos : WORLD_POS;
            };

            sampler2D _MainTex;
            float4 _Center;
            float _Radius;
            float _Strength;
            float4 _Delta;
            int _OperationMode;

            half Fresnel(float3 viewDir, float3 worldNormal, float power)
            {
                return pow(1.0 - saturate(dot(viewDir, worldNormal)), power);
            }


            v2f vert(appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 normal;
                float2 coord = i.uv;
                float4 info = tex2D(_MainTex, coord);
                _Center = normalize(_Center);
                if (_OperationMode == 0)
                {
                    // Drop Mode
                    float drop = max(0.0, 1.0 - length(_Center.xy - coord) / _Radius);
                    drop = 0.5 - cos(drop * 3.14159265359) * 0.5;
                    info.r += drop * _Strength;
                }
                else if (_OperationMode == 1)
                {
                    // Update Mode
                    float2 dx = float2(_Delta.x, 0.0);
                    float2 dy = float2(0.0, _Delta.y);
                    float average = (
                        tex2D(_MainTex, coord - dx).r +
                        tex2D(_MainTex, coord - dy).r +
                        tex2D(_MainTex, coord + dx).r +
                        tex2D(_MainTex, coord + dy).r
                    ) * 0.25;

                    info.g += (average - info.r) * 0.5;
                    info.g *= 0.995; // Damping
                    info.r += info.g;
                }

                // Normal Mode
                float hLeft = tex2D(_MainTex, coord + float2(-_Delta.x, 0)).r; // Height to the left
                float hRight = tex2D(_MainTex, coord + float2(_Delta.x, 0)).r; // Height to the right
                float hDown = tex2D(_MainTex, coord + float2(0, -_Delta.y)).r; // Height below
                float hUp = tex2D(_MainTex, coord + float2(0, _Delta.y)).r; // Height above

                float3 dx = float3(2.0 * _Delta.x, hRight - hLeft, 0); // Change in height in the x direction
                float3 dy = float3(0, hUp - hDown, 2.0 * _Delta.y); // Change in height in the y direction

                normal = normalize(cross(dy, dx));

                //return info;

                // Calculate the view direction and normal
                float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

                float fresnel = Fresnel(viewDir, normal, 1);
                float4 waterColor = float4(0.0, 0.3, 0.8, 0.5); // Adjust color to your liking
                float4 reflectionColor = float4(0.0, 0.5, 0.8, 0.5); // Adjust color to your liking
                //return float4(fresnel,0,0, 1);
                return info;
            }
            ENDCG
        }
    }
}