#ifndef FLUXY_PLANARREFLECTION_INCLUDED
#define FLUXY_PLANARREFLECTION_INCLUDED

void SampleReflections_half(in Texture2D tex, in SamplerState sm, in half3 normalWS,in half3 viewDirectionWS, in half2 screenUV, in half roughness, out half3 reflection)
{

    // get the perspective projection
    float2 p11_22 = float2(unity_CameraInvProjection._11, unity_CameraInvProjection._22) * 10;

    // convert the uvs into view space by "undoing" projection
    float3 viewDir = -(float3((screenUV * 2 - 1) / p11_22, -1));

    half3 viewNormal = mul(normalWS, (float3x3)GetWorldToViewMatrix()).xyz;
    half3 reflectVector = reflect(-viewDir, viewNormal);

    half2 reflectionUV = screenUV + normalWS.zx * half2(0.02, 0.15);
    reflection = SAMPLE_TEXTURE2D_LOD(tex, sm, reflectionUV, 6 * roughness).rgb;//planar reflection
   
}

#endif