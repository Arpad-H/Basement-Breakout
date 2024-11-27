#ifndef FLUIDUTILS_INCLUDED
#define FLUIDUTILS_INCLUDED

#define MAX_TILES 17

sampler2D _TileID;
sampler2D _Velocity;
float4 _Velocity_TexelSize;

float4 _TileData[MAX_TILES];
float _Pressure[MAX_TILES];
float _VortConf[MAX_TILES];
float _Viscosity[MAX_TILES];
float _Adhesion[MAX_TILES];
float _SurfaceTension[MAX_TILES];
float4 _Dissipation[MAX_TILES];
float4 _EdgeFalloff[MAX_TILES];
float4 _Buoyancy[MAX_TILES];
float4 _ExternalForce[MAX_TILES];
float4 _Offsets[MAX_TILES];
int4 _WrapMode[MAX_TILES];

float4 _SplatTransform;
float _SplatRotation;
float4 _SplatWeights;
int _TileIndex;

struct appdata_lean
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f_lean
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
}; 

float EncodeTileID()
{
    return _TileIndex;
}

int GetTileID(in float2 uv)
{
    int id = tex2D(_TileID,uv).r;

    // discard if we're not in any tile:
    if (id < 1) discard;
    return id;
}

// gets a 2-component velocity vector in the 0-1 range and converts it to -1,1.
float2 UnpackVelocity(in float2 vel)
{
    return vel * 2 - 1;
}

void UnpackNormalAndProjectVelocity(in sampler2D normal, in float2 uv, in float normalScale, inout float2 vel)
{
    float3 n = UnpackNormal(tex2D(normal,uv));
    n = lerp(float3(0,0,1), n, normalScale);
    vel -= min(0,dot(vel,n)) * n;
}

// builds a normal vector from a texture's alpha channel (meant to read pressure from a velocity texture)
void NormalFromPressure_float(in float2 uv, in float scale, out float3 normal)
{
    float3 ts = float3(_Velocity_TexelSize.xy, 0);
    float2 uv0 = uv + ts.xz;
    float2 uv1 = uv + ts.zy;
    float h = tex2D(_Velocity, uv).a;
    float h0 = tex2D(_Velocity, uv0).a;
    float h1 = tex2D(_Velocity, uv1).a;

    float3 p0 = float3 (ts.xz, (h0 - h) * scale);
    float3 p1 = float3 (ts.zy, (h1 - h) * scale);

    normal = normalize (cross (p0, p1));
}

float2 RotateVector(in float2 vel)
{
    float sin_ = sin (_SplatRotation);
    float cos_ = cos (_SplatRotation);
    float2x2 rotationMatrix = float2x2( cos_, -sin_, sin_, cos_);
    return mul(vel,rotationMatrix);
}

// nearest neighbor lookup, can wrap or clamp to edge texel or 0:
float4 tex2D_nearest(in sampler2D t, in float2 uv, in float4 texelSize, in float4 tile, in int4 wrapmask)
{    
    // wrap uvs:
    uv.x = lerp(uv.x, tile.x + tile.z * frac((uv.x - tile.x) / tile.z), wrapmask.x);
    uv.y = lerp(uv.y, tile.y + tile.w * frac((uv.y - tile.y) / tile.w), wrapmask.y);

    // determine if uvs are inside the tile:    
    int bx = lerp(1, step(tile.x, uv.x) - step(tile.x + tile.z, uv.x), wrapmask.z);
    int by = lerp(1, step(tile.y, uv.y) - step(tile.y + tile.w, uv.y), wrapmask.w);

    // clamp uvs
    uv.x = clamp(uv.x, tile.x + texelSize.x * 0.5, tile.x + tile.z - texelSize.x * 0.5);
    uv.y = clamp(uv.y, tile.y + texelSize.y * 0.5, tile.y + tile.w - texelSize.y * 0.5);

    return lerp(0, tex2D(t, uv), bx * by);
}

// bilinear lookup, clamping to border:
float4 tex2D_bilinear(in sampler2D t, in float2 uv, in float4 texelSize, in float4 tile, in int4 wrapmask)
{
    float2 st = uv * texelSize.zw - 0.5;
    float2 f = frac(st);
    uv = floor(st);
    
    float4 uv_min_max = float4((uv + 0.5f) /texelSize.zw, (uv + 1.5f) /texelSize.zw);
   
    float4 texelA = tex2D_nearest(t, uv_min_max.xy, texelSize, tile, wrapmask);
    float4 texelB = tex2D_nearest(t, uv_min_max.xw, texelSize, tile, wrapmask);
    float4 texelC = tex2D_nearest(t, uv_min_max.zy, texelSize, tile, wrapmask);
    float4 texelD = tex2D_nearest(t, uv_min_max.zw, texelSize, tile, wrapmask);

    return lerp(lerp(texelA, texelB, f.y), lerp(texelC, texelD, f.y), f.x);
}

// flowmap lookup, wrapping and clamping defined by sampler.
float4 tex2D_flowmap(in sampler2D tex, in float2 uv, in float2 velocity, float speedScale)
{
    velocity *= speedScale;

    float phase1 = frac(_Time.y);
    float phase2 = frac(_Time.y + 0.5);

    float2 flowUV1 = uv - velocity * phase1;
    float2 flowUV2 = uv - velocity * phase2;

    float4 color1 = tex2D(tex, flowUV1);
    float4 color2 = tex2D(tex, flowUV2);

    float factor = abs((phase1 - 0.5) * 2);
    return lerp(color1,color2,factor);
}

float SquareFalloff(in float2 uv, in float falloff)
{
    float2 marquee = max((abs(uv - 0.5) * 2 - (1 - falloff)) / falloff,0);
    return saturate(1 - length(marquee)) ;
}

float2 VertexToFrame(in float2 v, in float4 frame)
{
    // flip y coordinate to match clip and uv space:
    #if UNITY_UV_STARTS_AT_TOP
    frame.y = 1 - frame.y;
    frame.w *= -1;
    v.y *= -1;
    #endif

    float2 pos = frame.xy + (v + 1) * 0.5 * frame.zw;
    return pos*2-1;
}

float2 VertexToTile(in float2 v, in int index)
{
    return VertexToFrame(v,_TileData[index]);
}

float4 AlphaAdditiveBlend(in float4 color, float bias)
{
    float additiveness = saturate((1 - bias) * 2 - color.a);
    return float4(color.r,color.g,color.b, additiveness) * color.a;
}

float2 VertexToSplat(in float2 v, in int index)
{
    float4 tile = _TileData[index];
    float2 aspect = float2(1,tile.z/tile.w);
    
    float sin_ = sin (_SplatRotation);
    float cos_ = cos (_SplatRotation);
    float2x2 rotationMatrix = float2x2( cos_, -sin_, sin_, cos_);

    // flip y coordinate to match clip and uv space:
    #if UNITY_UV_STARTS_AT_TOP
    tile.y = 1 - tile.y;
    tile.w *= -1;
    v.y *= -1;
    #endif

    float2 pos = tile.xy + ((mul(v * _SplatTransform.zw,rotationMatrix) * aspect + 1) * 0.5 + _SplatTransform.xy) * tile.zw;
    return pos*2-1;
}

// Convert from uv coords to tile coords
float2 UVToTile(in float2 uv, in int index)
{
    float4 tile = _TileData[index];
    return (uv - tile.xy) / tile.zw;
}

// Convert from tile coords to uv coords.
float2 TileToUV(in float2 uv, in int index)
{
    float4 tile = _TileData[index];
    return tile.xy + uv * tile.zw;
}

float2 FlipbookUV(in float2 uv, int rows, int columns, int totalFrames, int frame)
{
    frame -= totalFrames * floor(frame/(float)totalFrames); //modulo supporting negative numbers
    float2 frameCount = float2(1.0, 1.0) / float2(columns, rows);
    float frameY = floor(frame * frameCount.x);
    float frameX = frame - columns * frameY;
    return (uv + float2(frameX, frameY)) * frameCount;
}

#endif