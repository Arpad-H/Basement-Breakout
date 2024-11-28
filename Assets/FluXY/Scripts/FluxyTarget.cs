using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Fluxy
{
    [AddComponentMenu("Physics/FluXY/Target", 800)]
    [ExecutionOrder(9999)]
    public class FluxyTarget : MonoBehaviour
    {
        /// <summary>
        /// Material used to splat this target's velocity and density onto containers.
        /// </summary>
        [Tooltip("Material used to splat this target's velocity and density onto containers.")]
        public Material splatMaterial;

        /// <summary>
        /// Amount of splats per update step.
        /// </summary>
        [Tooltip("Amount of splats per update step.")]
        [Min(0)]
        [FormerlySerializedAs("temporalSamples")]
        public int rateOverSteps = 1;

        /// <summary>
        /// Amount of splats per second.
        /// </summary>
        [Tooltip("Amount of splats per second.")]
        [Min(0)]
        public float rateOverTime = 0;

        /// <summary>
        /// Amount of splats per distance unit.
        /// </summary>
        [Tooltip("Amount of splats per distance unit.")]
        [Min(0)]
        public float rateOverDistance = 0;

        /// <summary>
        /// Manually override target splat position.
        /// </summary>
        [Tooltip("Manually override target splat position.")]
        public bool overridePosition = false;

        /// <summary>
        /// Coordinate to splat this target at when overriding splat position.
        /// </summary>
        [Tooltip("Coordinate to splat this target at when overriding splat position.")]
        public Vector2 position = Vector2.zero;

        /// <summary>
        /// Randomization applied to the splat position.
        /// </summary>
        [Tooltip("Randomization applied to the splat position.")]
        [Range(0, 1)]
        public float positionRandomness = 0;

        /// <summary>
        /// Manually override target splat position.
        /// </summary>
        [Tooltip("Manually override target splat rotation.")]
        public bool overrideRotation = true;

        /// <summary>
        /// Rotation of the target's shape when splatted.
        /// </summary>
        [Tooltip("Rotation of the target's shape when splatted.")]
        public float rotation = 0;

        /// <summary>
        /// Randomization applied to the splat rotation.
        /// </summary>
        [Tooltip("Randomization applied to the splat rotation.")]
        [Range(0, 1)]
        public float rotationRandomness = 0;

        /// <summary>
        /// Scales splat size based on distance from the target to the container's surface.
        /// </summary>
        [Tooltip("Scales splat size based on distance from the target to the container's surface.")]
        public bool scaleWithDistance = true;

        /// <summary>
        /// Scales splat based on maximum transform scale value.
        /// </summary>
        [Tooltip("Scales splat based on maximum transform scale value.")]
        public bool scaleWithTransform = false;

        /// <summary>
        /// Scale of the target's shape when splatted.
        /// </summary>
        [Tooltip("Scale of the target's shape when splatted.")]
        public Vector2 scale = new Vector2(0.1f,0.1f);

        /// <summary>
        /// Randomization applied to the splat size.
        /// </summary>
        [Tooltip("Randomization applied to the splat size.")]
        [Range(0, 1)]
        [FormerlySerializedAs("sizeRandomness")]
        public float scaleRandomness = 0;

        /// <summary>
        /// Amount of velocity splatted.
        /// </summary>
        [Range(0,1)]
        public float velocityWeight = 1;

        /// <summary>
        /// Texture defining the target's splat shape.
        /// </summary>
        [Tooltip("Texture defining the target's splat shape.")]
        public Texture velocityTexture;

        /// <summary>
        /// Maximum relative velocity between a container and this target.
        /// </summary>
        [Min(0)]
        [Tooltip("Maximum relative velocity between a container and this target.")]
        public float maxRelativeVelocity = 8;

        /// <summary>
        /// Local-space scale applied to this target's velocity vector.
        /// </summary>
        [Tooltip("Local-space scale applied to this target's velocity vector.")]
        public Vector3 velocityScale = Vector3.one;

        /// <summary>
        /// Maximum relative angular velocity between a container and this target.
        /// </summary>
        [Min(0)]
        [Tooltip("Maximum relative angular velocity between a container and this target.")]
        public float maxRelativeAngularVelocity = 12;

        /// <summary>
        /// Scale applied to this target's angular velocity.
        /// </summary>
        [Tooltip("Scale applied to this target's angular velocity.")]
        public float angularVelocityScale = 1;

        /// <summary>
        /// Local-space force applied by this target, regardless of its velocity
        /// </summary>
        [Tooltip("Local-space force applied by this target, regardless of its velocity")]
        public Vector3 force = Vector3.zero;

        /// <summary>
        /// Local-space torque applied by this target, regardless of its angular velocity
        /// </summary>
        [Tooltip("Local-space torque applied by this target, regardless of its angular velocity")]
        public float torque = 0;

        /// <summary>
        /// Amount of density splatted.
        /// </summary>
        [Range(0, 1)]
        public float densityWeight = 1;

        /// <summary>
        /// Texture defining the target's splat shape.
        /// </summary>
        [Tooltip("Texture defining the target's splat shape.")]
        public Texture densityTexture;

        /// <summary>
        /// Blend mode used for source fragments.
        /// </summary>
        [Tooltip("Blend mode used for source fragments.")]
        public BlendMode srcBlend = BlendMode.SrcAlpha;

        /// <summary>
        /// Blend mode used for destination fragments.
        /// </summary>
        [Tooltip("Blend mode used for destination fragments.")]
        public BlendMode dstBlend = BlendMode.OneMinusSrcAlpha;

        /// <summary>
        /// Blend operation used when splatting density.
        /// </summary>
        [Tooltip("Blend operation used when splatting density.")]
        public BlendOp blendOp = BlendOp.Add;

        /// <summary>
        /// Color splatted by this target onto the container's density buffer.
        /// </summary>
        [Tooltip("Color splatted by this target onto the container's density buffer.")]
        public Color color = Color.white;

        /// <summary>
        /// Texture used to generate density and velocity noise.
        /// </summary>
        [Tooltip("Texture used to generate density and velocity noise.")]
        public Texture noiseTexture;

        /// <summary>
        /// Amount of scalar noise modulating density.
        /// </summary>
        [Min(0)]
        [Tooltip("Amount of scalar noise modulating density.")]
        public float densityNoise = 0;

        /// <summary>
        /// Non-zero values animate noise by offsetting it.
        /// </summary>
        [Min(0)]
        [Tooltip("Non-zero values animate noise by offsetting it.")]
        public float densityNoiseOffset = 0;

        /// <summary>
        /// Tiling scale of density noise.
        /// </summary>
        [Min(0)]
        [Tooltip("Tiling scale of density noise.")]
        public float densityNoiseTiling= 1;


        /// <summary>
        /// Amount of curl noise added to velocity.
        /// </summary>
        [Min(0)]
        [Tooltip("Amount of curl noise added to velocity.")]
        public float velocityNoise = 0;

        /// <summary>
        /// Non-zero values animate noise by offsetting it.
        /// </summary>
        [Min(0)]
        [Tooltip("Non-zero values animate noise by offsetting it.")]
        public float velocityNoiseOffset = 0;

        /// <summary>
        /// Tiling scale of velocity noise.
        /// </summary>
        [Min(0)]
        [Tooltip("Tiling scale of velocity noise.")]
        public float velocityNoiseTiling = 1;

        private Vector3 oldPosition;
        private Quaternion oldRotation;
        private float timeAccumulator = 0;
        private float distanceAccumulator = 0;
        private int timeSplats = 0;

        public delegate void SplatCallback(FluxyTarget target, FluxyContainer container, FluxyStorage.Framebuffer fb, in Vector4 rect);
        public event SplatCallback OnSplat;

        public Vector3 velocity
        {
            get { return (transform.position - oldPosition) / Time.deltaTime; }
        }

        public Vector3 angularVelocity
        {
            get
            {
                Quaternion rotationDelta = transform.rotation * Quaternion.Inverse(oldRotation);
                return new Vector3(rotationDelta.x, rotationDelta.y, rotationDelta.z) * 2.0f / Time.deltaTime;
            }
        }

        public void OnEnable()
        {
            SetOldState();
        }

        private void Update()
        {
            // update rate over time:
            if (rateOverTime > 0)
            {
                timeAccumulator += Time.deltaTime * rateOverTime;
                timeSplats = Mathf.FloorToInt(timeAccumulator);
                timeAccumulator -= timeSplats;
            }
            else
            {
                timeAccumulator = 0;
                timeSplats = 0;
            }
        }

        private void LateUpdate()
        {
            SetOldState();
        }

        private void SetOldState()
        {
            oldPosition = transform.position;
            oldRotation = transform.rotation;
        }

        private float GetAspectRatio()
        {
            if (densityTexture != null)
                return densityTexture.width / (float)densityTexture.height;
            else if (velocityTexture != null)
                return velocityTexture.width / (float)velocityTexture.height;
            else return 1;
        }

        public virtual void Splat(FluxyContainer container, FluxyStorage.Framebuffer fb, in int tileIndex, in Vector4 rect)
        {
            if (splatMaterial != null && isActiveAndEnabled)
            {
                Quaternion worldToContainerR = Quaternion.Inverse(container.transform.rotation);

                var lossyScale = transform.lossyScale;
                float maxScale = Mathf.Max(Mathf.Max(lossyScale.x, lossyScale.y), lossyScale.z);

                // calculate relative velocities in UV space:
                Vector3 relativeVelocity = container.TransformWorldVectorToUVSpace(velocity - container.velocity * container.velocityScale, rect);
                float relativeAngularVel = container.TransformWorldVectorToUVSpace(angularVelocity, rect).z -
                                           container.TransformWorldVectorToUVSpace(container.angularVelocity, rect).z * container.velocityScale;

                // clamp and scale linear vel:
                float speed = relativeVelocity.magnitude;
                if (speed > FluxyUtils.epsilon)
                {
                    relativeVelocity /= speed;
                    relativeVelocity *= Mathf.Min(speed, maxRelativeVelocity);
                }
                Vector4 vel = Vector3.Scale(relativeVelocity, velocityScale) + force;
                vel.w = velocityWeight;

                // clamp and scale angular vel:
                relativeAngularVel = Mathf.Clamp(relativeAngularVel, -maxRelativeAngularVelocity, maxRelativeAngularVelocity) * angularVelocityScale;
                relativeAngularVel += torque;

                // pass tile index and blend mode to shader.
                splatMaterial.SetInt("_TileIndex", tileIndex);
                splatMaterial.SetInt("_SrcBlend", (int)srcBlend);
                splatMaterial.SetInt("_DstBlend", (int)dstBlend);
                splatMaterial.SetInt("_BlendOp", (int)blendOp);
                splatMaterial.SetTexture("_Noise", noiseTexture);

                var velocityNoiseParams = new Vector3(velocityNoise, velocityNoiseOffset, velocityNoiseTiling);
                var densityNoiseParams = new Vector3(densityNoise, densityNoiseOffset, densityNoiseTiling);

                // calculate texture aspect ratio:
                float aspectRatio = GetAspectRatio();

                // update rate over distance:
                int distanceSplats;
                if (rateOverDistance > 0)
                {
                    distanceAccumulator += Vector3.Distance(transform.position, oldPosition) * rateOverDistance;
                    distanceSplats = Mathf.FloorToInt(distanceAccumulator);
                    distanceAccumulator -= distanceSplats;
                }
                else
                {
                    distanceAccumulator = 0;
                    distanceSplats = 0;
                }

                // calculate amount of splats:
                int totalSplats = rateOverSteps + timeSplats + distanceSplats;

                // calculate splat color:
                var splatColor = new Color(color.r, color.g, color.b, color.a * densityWeight);
                if (QualitySettings.activeColorSpace == ColorSpace.Linear)
                    splatColor = splatColor.linear;

                // splat multiple times, interpolating between last position and current position:
                for (int i = 1; i <= totalSplats; ++i)
                {
                    float interpolationFactor = i / (float)totalSplats;

                    // randomize position/scale/orientation:
                    Vector4 randomOffset = (Vector4)Random.insideUnitCircle * positionRandomness;
                    float randomScale = Random.Range(-scaleRandomness, scaleRandomness) * 0.5f;
                    float randomRotation = Random.Range(-rotationRandomness, rotationRandomness) * Mathf.PI;

                    // pass splat rotation to shader (using lerp instead of slerp as rotation angle will be small).
                    float orientation = rotation;
                    if (!overrideRotation)
                        orientation = -(worldToContainerR * Quaternion.Lerp(oldRotation, transform.rotation, interpolationFactor)).eulerAngles.z;
                    splatMaterial.SetFloat("_SplatRotation", orientation * Mathf.Deg2Rad + randomRotation);

                    // pass splat scale to shader:
                    Vector2 projectionSize = scale + new Vector2(randomScale, randomScale);
                    if (scaleWithTransform)
                        projectionSize *= maxScale;

                    // pass splat position to shader:
                    Vector4 projection;
                    if (overridePosition)
                        projection = new Vector4(position.x, position.y, projectionSize.x * aspectRatio, projectionSize.y);
                    else
                    {
                        Vector3 targetPos = Vector3.Lerp(oldPosition, transform.position, interpolationFactor);
                        projection = container.ProjectTarget(targetPos, projectionSize, aspectRatio, scaleWithDistance);
                    }
                    splatMaterial.SetVector("_SplatTransform", projection + randomOffset);

                    // splat state:
                    splatMaterial.SetVector("_DensityNoiseParams", densityNoiseParams);
                    splatMaterial.SetVector("_SplatWeights", splatColor);
                    Graphics.Blit(densityTexture, fb.stateA, splatMaterial, 0);

                    // splat velocity
                    splatMaterial.SetVector("_VelocityNoiseParams", velocityNoiseParams);
                    splatMaterial.SetFloat("_AngularVelocity", relativeAngularVel);
                    splatMaterial.SetVector("_SplatWeights", vel);
                    splatMaterial.SetTexture("_Velocity", velocityTexture);
                    Graphics.Blit(densityTexture, fb.velocityA, splatMaterial, 1);
                }

                OnSplat?.Invoke(this, container, fb, rect);
            }
        }
    }
}
