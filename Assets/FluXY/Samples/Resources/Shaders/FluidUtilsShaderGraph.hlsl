#ifndef FLUIDUTILSSG_INCLUDED
#define FLUIDUTILSSG_INCLUDED

float4 _TileData[17];

// Convert from tile coords to uv coords.
void TileToUV_float(in float2 uv, in int index, out float2 coords)
{
    float4 tile = _TileData[index];
    coords = tile.xy + uv * tile.zw;
}

// flowmap lookup, wrapping and clamping defined by sampler.
void AdvectTexture_float(in Texture2D tex, in SamplerState sm, in float2 uv, in float2 velocity, in float speedScale, in float offset, out float4 color)
{
    velocity *= speedScale;

    float phase1 = frac(_Time.y);
    float phase2 = frac(_Time.y + 0.5);
    
    float2 flowUV1 = uv - velocity * (phase1 + offset);
    float2 flowUV2 = uv - velocity * (phase2 + offset);

    flowUV1 += (_Time.y - phase1) * 0.25;
    flowUV2 += (_Time.y - phase2) * 0.25 + 0.5;

    float4 color1 = tex.Sample(sm, flowUV1);
    float4 color2 = tex.Sample(sm, flowUV2);

    color1 *= 0.5*(1 + cos(6.2832 * (phase1+0.5)));
    color2 *= 0.5*(1 + cos(6.2832 * (phase2+0.5)));

    color = color1 + color2;
}
#endif