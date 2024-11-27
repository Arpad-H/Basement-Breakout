Shader "Fluxy/Simulation/FluidSimulation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Normals ("Normals", 2D) = "bump" {}
    }
    SubShader
    {
       
        Cull Off ZWrite Off ZTest Always

        HLSLINCLUDE
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "../FluidUtils.hlsl"
        ENDHLSL
        
        Pass
        {
            Name "AdvectState"

            HLSLPROGRAM

            v2f_lean vert (appdata_lean v)
            {
                v2f_lean o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
          
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _DeltaTime;
           
            float4 frag (v2f_lean i) : SV_Target
            {
                int tileID = GetTileID(i.uv);
                float4 wrapmode = _WrapMode[tileID];

                // read velocity field at current texel 
                // (use bilinear interpolation as velocity tex can have different resolution)
                float2 vel = tex2D_bilinear(_Velocity, i.uv, _Velocity_TexelSize, _TileData[tileID], wrapmode).rg;

                // trace back in time and read state values:                
                float2 sourceUV =  i.uv + _Offsets[tileID] - vel * _DeltaTime;
                float4 advected = tex2D_bilinear(_MainTex, sourceUV, _MainTex_TexelSize, _TileData[tileID], wrapmode); 

                // scale velocity with density at source to simulate adhesion:
                vel *= saturate((1 - _Adhesion[tileID])*2 - 1 + advected.a);

                return tex2D_bilinear(_MainTex, i.uv + _Offsets[tileID] - vel * _DeltaTime , _MainTex_TexelSize, _TileData[tileID], wrapmode); 
            }
            ENDHLSL
        }

        Pass
        {
            Name "AdvectVelocity"

            HLSLPROGRAM

            v2f_lean vert (appdata_lean v)
            {
                v2f_lean o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
          
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _DeltaTime;
           
            float4 frag (v2f_lean i) : SV_Target
            {
                int tileID = GetTileID(i.uv);
                float4 wrapmode = float4(_WrapMode[tileID].xy,1,1);

                // read velocity field at current texel:
                float2 vel = tex2D(_MainTex, i.uv).rg;

                // trace back in time and read interpolated values:                   
                return tex2D_bilinear(_MainTex, i.uv - vel * _DeltaTime + _Offsets[tileID], _MainTex_TexelSize, _TileData[tileID], wrapmode); 
            }
            ENDHLSL
        }

        Pass
        {
            Name "Dissipation"

            HLSLPROGRAM

            v2f_lean vert (appdata_lean v)
            {
                v2f_lean o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
          
            sampler2D _MainTex;
            float _DeltaTime;
           
            float4 frag (v2f_lean i) : SV_Target
            {
                int tileID = GetTileID(i.uv);
                float2 tileUV = UVToTile(i.uv,tileID);
                
                float falloff = (1 - SquareFalloff(tileUV,_EdgeFalloff[tileID].x)) * _EdgeFalloff[tileID].y;
                return saturate(tex2D(_MainTex, i.uv) - (_Dissipation[tileID] + falloff) * _DeltaTime);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Curl"
            
            HLSLPROGRAM

            v2f_lean vert (appdata_lean v)
            {
                v2f_lean o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _DeltaTime;
           
            float4 frag (v2f_lean i) : SV_Target
            {
                int tileID = GetTileID(i.uv);
                float2 offset = _MainTex_TexelSize.xy;
                
                float4 rect = _TileData[tileID];
                int4 wrapmask = _WrapMode[tileID] * float4(1,1,0,0);
                float4 vC = tex2D(_MainTex, i.uv);

                // calculate curl:
                float vL =  tex2D_nearest(_MainTex, i.uv + float2(-offset.x,0), _MainTex_TexelSize, rect, wrapmask).g;
                float vR =  tex2D_nearest(_MainTex, i.uv + float2(offset.x,0) , _MainTex_TexelSize, rect, wrapmask).g;
                float vB =  tex2D_nearest(_MainTex, i.uv + float2(0,-offset.y), _MainTex_TexelSize, rect, wrapmask).r;
                float vT =  tex2D_nearest(_MainTex, i.uv + float2(0,offset.y) , _MainTex_TexelSize, rect, wrapmask).r;
                
                vC.b = (vL - vR + vT - vB) * 0.5;
                return vC;
                 
            }
            ENDHLSL
        }

        Pass
        {
            Name "DensityGradient"

            ColorMask RG

            HLSLPROGRAM
            v2f_lean vert (appdata_lean v)
            {
                v2f_lean o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
           
            float4 frag (v2f_lean i) : SV_Target
            {
                int tileID = GetTileID(i.uv);
                float3 offset = float3(_MainTex_TexelSize.xy,0);
                
                float4 rect = _TileData[tileID];
                int4 wrapmask = _WrapMode[tileID];
                
                float hb0 = tex2D_nearest(_MainTex, i.uv - offset.xz, _MainTex_TexelSize, rect, wrapmask).a;
                float hb1 = tex2D_nearest(_MainTex, i.uv - offset.zy, _MainTex_TexelSize, rect, wrapmask).a;
                float h0 = tex2D_nearest(_MainTex, i.uv + offset.xz, _MainTex_TexelSize, rect, wrapmask).a;
                float h1 = tex2D_nearest(_MainTex, i.uv + offset.zy, _MainTex_TexelSize, rect, wrapmask).a;

                float3 p0 = float3 ( offset.xz, (h0 - hb0));
                float3 p1 = float3 ( offset.zy, (h1 - hb1));

                float3 nrm = normalize (cross (p0, p1));
                
                return float4(nrm.xy,0,0);
                              
            }
            ENDHLSL
        }
       
        Pass
        {
            Name "Divergence"

            HLSLPROGRAM

            v2f_lean vert (appdata_lean v)
            {
                v2f_lean o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
           
            float4 frag (v2f_lean i) : SV_Target
            {
                int tileID = GetTileID(i.uv);
                float2 offset = _MainTex_TexelSize.xy;

                float4 rect = _TileData[tileID];
                int4 wrapmask = _WrapMode[tileID];
                float4 vC = tex2D(_MainTex, i.uv);

                // calculate divergence:
                float vL =  tex2D_nearest(_MainTex, i.uv + float2(-offset.x,0), _MainTex_TexelSize, rect, wrapmask).r;
                float vR =  tex2D_nearest(_MainTex, i.uv + float2(offset.x, 0), _MainTex_TexelSize, rect, wrapmask).r;
                float vB =  tex2D_nearest(_MainTex, i.uv + float2(0,-offset.y), _MainTex_TexelSize, rect, wrapmask).g;
                float vT =  tex2D_nearest(_MainTex, i.uv + float2(0, offset.y), _MainTex_TexelSize, rect, wrapmask).g;
                
                vC.b = -(vR - vL + vT - vB) * 0.5;
                return vC;
                 
            }
            ENDHLSL
        }

        Pass
        {
            Name "Pressure"

            HLSLPROGRAM

            v2f_lean vert (appdata_lean v)
            {
                v2f_lean o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            static const float weights[7] = {
            -0.57843719174,
            -0.36519596949,
            -0.23187988879,
            -0.14529589353,
            -0.08816487385,
            -0.05184872885,
            -0.02906462467
            };

            float2 axis;

            float4 frag (v2f_lean i) : SV_Target
            {
                int tileID = GetTileID(i.uv);
                float2 offset = _MainTex_TexelSize.xy * axis;

                float4 c = tex2D(_MainTex, i.uv);
                c.a *= weights[0];
                
                float4 rect = _TileData[tileID];
                int4 wrapmask = _WrapMode[tileID] * float4(1,1,0,0);
                c.a += tex2D_nearest(_MainTex, i.uv - offset * 6, _MainTex_TexelSize, rect, wrapmask).a * weights[6];
                c.a += tex2D_nearest(_MainTex, i.uv - offset * 5, _MainTex_TexelSize, rect, wrapmask).a * weights[5];
                c.a += tex2D_nearest(_MainTex, i.uv - offset * 4, _MainTex_TexelSize, rect, wrapmask).a * weights[4];
                c.a += tex2D_nearest(_MainTex, i.uv - offset * 3, _MainTex_TexelSize, rect, wrapmask).a * weights[3];
                c.a += tex2D_nearest(_MainTex, i.uv - offset * 2, _MainTex_TexelSize, rect, wrapmask).a * weights[2];
                c.a += tex2D_nearest(_MainTex, i.uv - offset, _MainTex_TexelSize, rect, wrapmask).a * weights[1];
                c.a += tex2D_nearest(_MainTex, i.uv + offset, _MainTex_TexelSize, rect, wrapmask).a * weights[1];
                c.a += tex2D_nearest(_MainTex, i.uv + offset * 2, _MainTex_TexelSize, rect, wrapmask).a * weights[2];
                c.a += tex2D_nearest(_MainTex, i.uv + offset * 3, _MainTex_TexelSize, rect, wrapmask).a * weights[3];
                c.a += tex2D_nearest(_MainTex, i.uv + offset * 4, _MainTex_TexelSize, rect, wrapmask).a * weights[4];
                c.a += tex2D_nearest(_MainTex, i.uv + offset * 5, _MainTex_TexelSize, rect, wrapmask).a * weights[5];
                c.a += tex2D_nearest(_MainTex, i.uv + offset * 6, _MainTex_TexelSize, rect, wrapmask).a * weights[6];

                return c;
                 
            }
            ENDHLSL
        }

        Pass
        {
            Name "SubtractPressureGradient"
            Blend One One

            HLSLPROGRAM

            v2f_lean vert (appdata_lean v)
            {
                v2f_lean o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float4 frag (v2f_lean i) : SV_Target
            {
                int tileID = GetTileID(i.uv);
                float2 offset = _MainTex_TexelSize.xy;

                float4 rect = _TileData[tileID];
                int4 wrapmask = _WrapMode[tileID] * float4(1,1,0,0);
                float pL =  tex2D_nearest(_MainTex, i.uv + float2(-offset.x,0), _MainTex_TexelSize, rect, wrapmask).a;
                float pR =  tex2D_nearest(_MainTex, i.uv + float2(offset.x, 0), _MainTex_TexelSize, rect, wrapmask).a;
                float pB =  tex2D_nearest(_MainTex, i.uv + float2(0,-offset.y), _MainTex_TexelSize, rect, wrapmask).a;
                float pT =  tex2D_nearest(_MainTex, i.uv + float2(0, offset.y), _MainTex_TexelSize, rect, wrapmask).a;
                
                return float4(-float2(pR - pL, pT - pB) * 0.5 * _Pressure[tileID], 0, 0);
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "Splat ID"

            HLSLPROGRAM

            sampler2D _MainTex;

            v2f_lean vert (appdata_lean v)
            {
                v2f_lean o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex.xy = VertexToTile(o.vertex.xy,_TileIndex);
                o.uv = v.uv;
                return o;
            }
   
            float4 frag (v2f_lean i) : SV_Target
            {         
                return EncodeTileID();
            }
            ENDHLSL
        }

        Pass
        {
            Name "Clear State"

            HLSLPROGRAM
            
            sampler2D _MainTex;
            float4 _ClearColor;

            v2f_lean vert (appdata_lean v)
            {
                v2f_lean o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex.xy = VertexToTile(o.vertex.xy,_TileIndex);
                o.uv = v.uv;
                return o;
            }
   
            float4 frag (v2f_lean i) : SV_Target
            {         
                return tex2D(_MainTex,i.uv) * _ClearColor;
            }
            ENDHLSL
        }

        Pass
        {
            Name "JacobiIteration"

            HLSLPROGRAM
            v2f_lean vert (appdata_lean v)
            {
                v2f_lean o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
           
            float4 frag (v2f_lean i) : SV_Target
            {
                int tileID = GetTileID(i.uv);
                float2 offset = _MainTex_TexelSize.xy;
                
                float4 rect = _TileData[tileID];
                int4 wrapmask = _WrapMode[tileID];
                float4 xC = tex2D(_MainTex, i.uv);

                // calculate pressure from divergence:
                float xL = tex2D_nearest(_MainTex, i.uv + float2(-offset.x,0),_MainTex_TexelSize, rect, wrapmask).a;
                float xR = tex2D_nearest(_MainTex, i.uv + float2(offset.x,0),_MainTex_TexelSize, rect, wrapmask).a;
                float xB = tex2D_nearest(_MainTex, i.uv + float2(0,-offset.y),_MainTex_TexelSize, rect, wrapmask).a;
                float xT = tex2D_nearest(_MainTex, i.uv + float2(0,offset.y),_MainTex_TexelSize, rect, wrapmask).a;
                
                xC.a = (xL + xR + xB + xT + xC.b) * 0.25;
                return xC;
                 
            }
            ENDHLSL
        }

        Pass
        {
            Name "CopyDivergenceToPressure"

            HLSLPROGRAM

            v2f_lean vert (appdata_lean v)
            {
                v2f_lean o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
           
            float4 frag (v2f_lean i) : SV_Target
            {
                float4 vC = tex2D(_MainTex, i.uv);
                vC.a = vC.b;
                return vC;
            }
            ENDHLSL
        }

        Pass
        {
            Name "ExternalForces"

            HLSLPROGRAM
           
            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                half3x3 worldToTangent : TEXCOORD1; 
                float4 vertex : SV_POSITION;
            }; 
            
            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = mul(UNITY_MATRIX_P, float4(v.uv.xy,0,1));
                o.vertex.xy = VertexToTile(o.vertex.xy,_TileIndex);
          
                // UnityObjectToWorldNormal without normalization:
                half3 wNormal = mul(v.normal, (float3x3)unity_WorldToObject);

                // UnityObjectToWorldDir without normalization:
                half3 wTangent = mul((float3x3)unity_ObjectToWorld, v.tangent.xyz);

                // normalize avoiding div by zero:
                wNormal /= max(length(wNormal),0.00001);
                wTangent /= max(length(wTangent),0.00001);

                // compute bitangent from cross product of normal and tangent
                half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(wNormal, wTangent) * tangentSign;

                // output the tangent space matrix
                o.worldToTangent = half3x3(wTangent, wBitangent, wNormal);

                return o;
            }

            sampler2D _MainTex;
            sampler2D _State;
            sampler2D _Normals;
            float4 _MainTex_TexelSize;

            float _NormalScale;
            float2 _NormalTiling;

            float _DeltaTime;
           
            float4 frag (v2f i) : SV_Target
            {
                float2 uv = TileToUV(i.uv,_TileIndex);
                float2 offset = _MainTex_TexelSize.xy;
                float4 rect = _TileData[_TileIndex];
                
                // calculate curvature (divergence of density gradient):
                int4 wrapmask = _WrapMode[_TileIndex];
                float2 grad = tex2D(_State, uv).rg;
                float l = tex2D_nearest(_State, uv + float2(-offset.x,0),_MainTex_TexelSize, rect, wrapmask).r;
                float r = tex2D_nearest(_State, uv + float2(offset.x,0),_MainTex_TexelSize, rect, wrapmask).r;
                float b = tex2D_nearest(_State, uv + float2(0,-offset.y),_MainTex_TexelSize, rect, wrapmask).g;
                float t = tex2D_nearest(_State, uv + float2(0,offset.y),_MainTex_TexelSize, rect, wrapmask).g;
                float curv = -(r - l + t - b) * 0.5;
                
                // calculate curl gradient:
                int4 curlWrapmask = _WrapMode[_TileIndex] * float4(1,1,0,0);
                float vL =  tex2D_nearest(_MainTex, uv + float2(-offset.x,0), _MainTex_TexelSize, rect, curlWrapmask).b;
                float vR =  tex2D_nearest(_MainTex, uv + float2(offset.x, 0), _MainTex_TexelSize, rect, curlWrapmask).b;
                float vB =  tex2D_nearest(_MainTex, uv + float2(0,-offset.y), _MainTex_TexelSize, rect, curlWrapmask).b;
                float vT =  tex2D_nearest(_MainTex, uv + float2(0, offset.y), _MainTex_TexelSize, rect, curlWrapmask).b;

                // cross(N, W) = N.yx * W;
                float2 curlGrad = float2(abs(vB) - abs(vT), abs(vR) - abs(vL)) * 0.5;
                float curlMag = length(curlGrad);

                // sample velocity at current texel:
                float4 vel = tex2D(_MainTex, uv);

                // viscosity (velocity dissipation)
                vel.rg *= _Viscosity[_TileIndex];

                // vorticity confinement:
                float2 vortConf = curlMag > 0.001 ? _VortConf[_TileIndex] * vel.b * curlGrad / curlMag : float2(0,0);

                // surface tension: first term controls drop shape (higher = rounder), second term controls drop size (higher = smaller)
                float2 surfTension = grad.rg * curv * 2 * _SurfaceTension[_TileIndex]; 
                         
                // transform external forces from world to tangent space:
                float3 externalForce = mul(i.worldToTangent, _ExternalForce[_TileIndex]); 

                // add all forces together:
                vel.rg += (externalForce.xy + surfTension + vortConf + _Buoyancy[_TileIndex] * tex2D(_State,uv).a) * _DeltaTime;

                // unpack container surface normal, project velocity vector to surface:
                UnpackNormalAndProjectVelocity(_Normals, i.uv * _NormalTiling, _NormalScale, vel.rg);

                // damping (falloff):
                float falloff = (1 - SquareFalloff(i.uv,_EdgeFalloff[_TileIndex].z)) * _EdgeFalloff[_TileIndex].w;
                vel.rg -= vel.rg * _DeltaTime * falloff;
                
                return vel;
                 
            }
            ENDHLSL
        }
    }
}
