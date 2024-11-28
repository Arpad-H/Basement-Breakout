#ifndef VOLUMETRIC_INCLUDED
#define VOLUMETRIC_INCLUDED

#include "../FluidUtilsShaderGraph.hlsl"

static const float VOL_EPSILON = 0.00000001;

void IGN_float(in int pixelX, in int pixelY, in int frame, out float noise)
{
    frame = fmod(frame,64); // need to periodically reset frame to avoid numerical issues
    float x = float(pixelX) + 5.588238f * float(frame);
    float y = float(pixelY) + 5.588238f * float(frame);
    noise = fmod(52.9829189f * fmod(0.06711056f*x + 0.00583715f*y, 1.0), 1.0);
}

float3 AdvectUV(in float2 uv, in float2 velocity, in float time, in float offset, in float phase)
{
    float3 flowUV;
    float progress = frac(time + phase * 0.5);
    flowUV.xy = uv - velocity * (progress + offset);
    flowUV.xy += (time - progress) * 0.25;
    flowUV.z = 1 - abs(1 - 2 * progress);
    return flowUV;
}

void RayBoxIntersection_float(in float3 rayOrigin, in float3 rayDirection, in float4 lightVector,
                              in float3 boxSize, 
                              in float3 scenePosition,
                              in int numSteps,
                              out float3 rayIntersection, out float3 intersection, out float3 direction, out float4 light)
{
    // ensure a minimum size for the box:
    boxSize = max(boxSize, VOL_EPSILON); 

    // convert all input from object to volume (normalized) space. 
    rayOrigin /= boxSize;
    rayDirection /= boxSize;
    scenePosition /= boxSize;
    lightVector.xyz /= boxSize;
    
    direction = rayDirection / max(length(rayDirection), VOL_EPSILON);
    light = lightVector;

    // calculate intersection of the ray with the volume box:
    float3 invraydir = 1 / direction;
    float3 firstintersections = (-0.5 - rayOrigin) * invraydir;
    float3 secondintersections = (0.5 - rayOrigin) * invraydir;
    float3 closest = min(firstintersections, secondintersections);
    float3 furthest = max(firstintersections, secondintersections);

    // calculate coords in the ray:
    float t0 = max(closest.x, max(closest.y, closest.z));
    float t1 = min(furthest.x, min(furthest.y, furthest.z));
    
    // clamp end of the ray to the scene depth:
    t1 = min(t1, length(scenePosition - rayOrigin));
    
    // align sample to camera plane:
    float planeoffset = 1 - frac( ( t0 - length(rayOrigin) ) * numSteps );
    float t0aligned = t0 + (planeoffset / numSteps); 

    rayIntersection = float3(t0, t0aligned, t1);
    intersection = rayOrigin;
}

float4 SampleDensity(in Texture2D tex, in SamplerState sm, 
                    in float3 sampleCoord, 
                    in float volumeOffset, 
                    in float volumeExtrusion,
                    in float volumeFalloff)
{
    float4 sample = SAMPLE_TEXTURE2D_LOD(tex,sm,sampleCoord.xy,0);
    float x = abs(sampleCoord.z - volumeOffset);
    float thickness = volumeExtrusion * sample.a;
    float falloff = 1 - smoothstep(thickness - volumeFalloff,thickness,x);

    return sample * falloff;
}

float SampleNoise(in Texture2D vel, in Texture3D noise, in SamplerState sm, 
                  in float3 sampleCoord, 
                  in float3 volumeSize, 
                  in float noiseScale,
                  in float noisePow,
                  in float2 advection)
{
    float2 velo = SAMPLE_TEXTURE2D_LOD(vel,sm,sampleCoord.xy,0).xy * advection.x;
    float time = _Time.y * advection.y;
    float3 noiseUV = sampleCoord * volumeSize / noiseScale;
    float3 flowuv1 = AdvectUV(noiseUV.xy, velo, time, -0.5, 0);
    float3 flowuv2 = AdvectUV(noiseUV.xy, velo, time, -0.5, 1);
    float n = SAMPLE_TEXTURE3D_LOD(noise,sm,float3(flowuv1.xy,noiseUV.z),0).r * flowuv1.z +
              SAMPLE_TEXTURE3D_LOD(noise,sm,float3(flowuv2.xy,noiseUV.z),0).r * flowuv2.z;

    return pow(abs(n),noisePow);
}

float4 SampleVolume(in Texture2D tex, in Texture2D vel, in Texture3D noise, in SamplerState sm, in SamplerState noiseSampler,
                    in float3 sampleCoord, 
                    in float volumeOffset, 
                    in float volumeExtrusion, 
                    in float volumeFalloff,
                    in float3 volumeSize,
                    in float noiseScale,
                    in float noisePow,
                    in float2 advection)
{
    float2 uv;
    TileToUV_float(sampleCoord.xy, _TileIndex, uv);

    float4 density = SampleDensity(tex, sm, float3(uv,sampleCoord.z), volumeOffset, volumeExtrusion, volumeFalloff);

    #if _NOISE_ON
        density *= SampleNoise(vel, noise, noiseSampler, sampleCoord, volumeSize, noiseScale, noisePow, advection);
    #endif

    return density;
}

void SingleScattering(in Texture2D tex, in Texture2D vel, in Texture3D noise, in SamplerState sm, in SamplerState noiseSampler, 
                      in float3 volumeSize, in float volumeOffset, in float volumeExtrusion, in float volumeFalloff,
                      in float3 rayOrigin, in float rayOffset, in float stepSize,
                      in float4 lightVector, in float4 lightColor, in float lightSteps, in float lightStepSize, 
                      in float densityScale, 
                      in float3 ambientDirection,
                      in float3 ambientColor,
                      in float ambientDensity, 
                      in float3 shadowExtinction,
                      in float shadowDensity,
                      in float shadowThreshold,
                      in float noiseScale,
                      in float noisePow,
                      in float2 advection,
                      inout float3 scattering, inout float transmittance)
{
    float3 sampleCoords = rayOrigin + 0.5;
    float4 sample = SampleVolume(tex, vel, noise, sm, noiseSampler, sampleCoords, volumeOffset, volumeExtrusion, volumeFalloff, volumeSize, noiseScale, noisePow, advection) * densityScale;
    float extinction = sample.a;

    if (extinction > 0.001)
    {
        float currentTransmittance = exp(-extinction * stepSize);

        #if defined(_LIGHTSOURCE_DIRECTIONAL) || defined(_LIGHTSOURCE_POINT)

            // calculate light direction vector:
            float3 lightDirection;
            #if defined(_LIGHTSOURCE_POINT)
                lightDirection = (lightVector.xyz - rayOrigin) * volumeSize;
            #else
                lightDirection = -lightVector.xyz;
            #endif 

            float distanceSqr = max(dot(lightDirection, lightDirection),0.0001);
            lightDirection /= sqrt(distanceSqr);

            // calculate attenuation factor:
            float attenuation = 1;
            #if defined (_LIGHTSOURCE_POINT)
                half factor = distanceSqr * lightVector.w;
                half rangeAtten = saturate(1 - factor * factor);
                attenuation = (rangeAtten * rangeAtten) / distanceSqr;
            #endif

            lightDirection /= volumeSize;
            lightDirection *= lightStepSize;

            // step in the direction of the light source:
            float shadowAccum = 0;
            float3 lightRayOrigin = rayOrigin + lightDirection * rayOffset;
            for (int j = 0; j < lightSteps; ++j)
            {
                lightRayOrigin += lightDirection;

                float3 lightSampleCoords = lightRayOrigin + 0.5;
                float lightSample = SampleVolume(tex, vel, noise, sm, noiseSampler, lightSampleCoords, volumeOffset, volumeExtrusion, volumeFalloff, volumeSize, noiseScale, noisePow, advection).a;
                shadowAccum += lightSample;
                
                if(shadowAccum > shadowThreshold)
                    break;
            }
            float3 shadow = exp(-shadowAccum * shadowDensity / shadowExtinction) * attenuation;
            float3 luminance = shadow * lightColor;

            //ambient light
            float amb = 0; 
            float ambientStepSize = 0.01;
            sampleCoords += ambientDirection * rayOffset * ambientStepSize;
            amb += SampleVolume(tex, vel, noise, sm, noiseSampler, saturate(sampleCoords + ambientDirection * ambientStepSize), volumeOffset, volumeExtrusion, volumeFalloff, volumeSize, noiseScale, noisePow, advection).a;
            amb += SampleVolume(tex, vel, noise, sm, noiseSampler, saturate(sampleCoords + ambientDirection * ambientStepSize * 2), volumeOffset, volumeExtrusion, volumeFalloff, volumeSize, noiseScale, noisePow, advection).a;
            amb += SampleVolume(tex, vel, noise, sm, noiseSampler, saturate(sampleCoords + ambientDirection * ambientStepSize * 3), volumeOffset, volumeExtrusion, volumeFalloff, volumeSize, noiseScale, noisePow, advection).a;
            luminance += exp(-amb * ambientDensity) * ambientColor;

        #else
            float3 luminance = float3(1,1,1);
        #endif

        // analytical integration:
        float3 intScattering = (luminance - luminance * currentTransmittance) / max(extinction, 0.00001);
        scattering += transmittance * sample.rgb * intScattering;
        
        transmittance *= currentTransmittance;
    }
}

void Raymarch_float(in Texture2D tex, in Texture2D vel, in Texture3D noise, in SamplerState sm, in SamplerState noiseSampler,
                    in float3 volumeSize, in float volumeOffset, in float volumeExtrusion, in float volumeFalloff,
                    in float3 rayIntersection, in float3 rayOrigin, in float3 rayDirection, 
                    in float4 lightVector, in float4 lightColor, in float rayOffset,
                    in float numSteps, 
                    in float lightSteps,
                    in float densityScale, 
                    in float densityThreshold,
                    in float3 ambientDirection,
                    in float3 ambientColor,
                    in float ambientDensity, 
                    in float3 shadowExtinction,
                    in float shadowDensity,
                    in float shadowThreshold,
                    in float noiseScale,
                    in float noisePow,
                    in float advectSpeed,
                    in float advectTime,
                    out float4 color)
{
    float transmittance = 1;
    float3 scattering = float3(0,0,0);
    float2 advection = float2(advectSpeed, advectTime);

    // ensure a minimum value for extinction:
    shadowExtinction = max(shadowExtinction, VOL_EPSILON);

    // ensure a minimum size for the volume:
    volumeSize = max(volumeSize, VOL_EPSILON); 
    
    // when inside the volume, the plane-aligned intersection
    // might be behind the cam so we clamp it:
    float clampedT = max(0,rayIntersection.y);

    // align ray with camera facing plane:
    rayOrigin += rayDirection * clampedT;

    // compute distance traversed by the ray inside the box:
    float volumeThickness = max(0, rayIntersection.z - clampedT);
        
    // calculate step size:
    lightSteps = clamp(lightSteps,0,64);
    float stepSize = 1 / numSteps;
    float lightStepSize = 1 / lightSteps;
        
    // scale rays by step size:
    rayDirection *= stepSize;
    shadowDensity *= lightStepSize;

    // number of steps: 
    numSteps = clamp(floor(volumeThickness / stepSize),0,64); 

    // calculate shadow threshold:
    float shadowThr = -log(shadowThreshold) / shadowDensity;

    // jitter ray origin:
    rayOrigin += rayDirection * rayOffset;
   
    // first sample:
    if (rayIntersection.y > 0)
    {
        float firstStep = min(rayIntersection.y,rayIntersection.z) - rayIntersection.x;

        SingleScattering(tex, vel, noise, sm, noiseSampler, 
                        volumeSize, volumeOffset, volumeExtrusion, volumeFalloff,
                        rayOrigin, rayOffset, firstStep,
                        lightVector, lightColor, lightSteps, lightStepSize,
                        densityScale, 
                        ambientDirection,
                        ambientColor,
                        ambientDensity, 
                        shadowExtinction,
                        shadowDensity,
                        shadowThr, noiseScale, noisePow, advection,
                        scattering, transmittance); 
    }  
                                      
    // main raymarch loop:
    for (int i = 0; i < numSteps; ++i)
    {
        rayOrigin += rayDirection;

        SingleScattering(tex, vel, noise, sm, noiseSampler, 
                        volumeSize, volumeOffset, volumeExtrusion, volumeFalloff,
                        rayOrigin, rayOffset, stepSize,
                        lightVector, lightColor, lightSteps, lightStepSize,
                        densityScale, 
                        ambientDirection,
                        ambientColor,
                        ambientDensity, 
                        shadowExtinction,
                        shadowDensity,
                        shadowThr, noiseScale, noisePow, advection,
                        scattering, transmittance);

        if (transmittance < densityThreshold)
            break;
    }

    // last sample:
    if (transmittance >= densityThreshold)
    {

        float lastStep = volumeThickness - numSteps * stepSize;
        rayOrigin += rayDirection / stepSize * lastStep;

        SingleScattering(tex, vel, noise, sm, noiseSampler, 
                        volumeSize, volumeOffset, volumeExtrusion, volumeFalloff,
                        rayOrigin, rayOffset, lastStep,
                        lightVector, lightColor, lightSteps, lightStepSize,
                        densityScale, 
                        ambientDirection,
                        ambientColor,
                        ambientDensity, 
                        shadowExtinction,
                        shadowDensity,
                        shadowThr, noiseScale, noisePow, advection,
                        scattering, transmittance);
    }
   
    color = saturate(float4(scattering,transmittance));
}

#endif