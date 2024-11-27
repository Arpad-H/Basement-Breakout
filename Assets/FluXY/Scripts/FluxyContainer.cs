using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Fluxy
{
    [AddComponentMenu("Physics/FluXY/Container", 800)]
    [ExecuteInEditMode]
    [ExecutionOrder(9998)]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class FluxyContainer : MonoBehaviour
    {
        [Serializable]
        public struct BoundaryConditions
        {
            public enum BoundaryType
            {
                Open,
                Solid,
                Periodic
            };

            public BoundaryType horizontalBoundary;
            public BoundaryType verticalBoundary;

            public static implicit operator Vector4(BoundaryConditions b)
            {
                return new Vector4(b.horizontalBoundary == BoundaryType.Periodic ? 1 : 0,
                                   b.verticalBoundary == BoundaryType.Periodic ? 1 : 0,
                                   b.horizontalBoundary == BoundaryType.Solid ? 1 : 0,
                                   b.verticalBoundary == BoundaryType.Solid ? 1 : 0);
            }
        }

        [Serializable]
        public struct EdgeFalloff
        {
            [Min(0)]
            public float densityEdgeWidth;
            [Min(0)]
            public float densityFalloffRate;
            [Min(0)]
            public float velocityEdgeWidth;
            [Min(0)]
            public float velocityFalloffRate;

            public static implicit operator Vector4(EdgeFalloff d)
            {
                return new Vector4(d.densityEdgeWidth, d.densityFalloffRate, d.velocityEdgeWidth, d.velocityFalloffRate);
            }
        }

        public enum LookAtMode
        {
            LookAt,
            CopyOrientation
        }

        public enum ContainerShape
        {
            Plane,
            Volume,
            Custom
        }

        /// <summary>
        /// Shape of the container: can be flat, can be a volume, or can be a custom mesh.
        /// </summary>
        [Tooltip("Shape of the container: can be flat, can be a volume, or can be a custom mesh.")]
        public ContainerShape containerShape = ContainerShape.Plane;

        /// <summary>
        /// Amount of subdivisions in the plane mesh.
        /// </summary>
        [Tooltip("Amount of subdivisions in the plane mesh.")]
        public Vector2Int subdivisions = Vector2Int.one;

        /// <summary>
        /// Custom mesh used by the container. If null, a subdivided plane will be used instead.
        /// </summary>
        [Tooltip("Custom mesh used by the container. If null, a subdivided plane will be used instead.")]
        public Mesh customMesh = null;

        /// <summary>
        /// Size of the container in local space.
        /// </summary>
        [Tooltip("Size of the container in local space.")]
        public Vector3 size = Vector3.one;

        /// <summary>
        /// Method using for facing the lookAt transform: look to it, or copy its orientation.
        /// </summary>
        [Tooltip("Method using for facing the lookAt transform: look to it, or copy its orientation.")]
        public LookAtMode lookAtMode = LookAtMode.LookAt;

        /// <summary>
        /// Transform that the container should be facing at all times. If unused, you can set the container's rotation manually.
        /// </summary>
        [Tooltip("Transform that the container should be facing at all times. If unused, you can set the container's rotation manually.")]
        public Transform lookAt;

        /// <summary>
        /// Transform used as raycasting origin for splatting targets onto this container. If unused, simple planar projection will be used instead.
        /// </summary>
        [Tooltip("Transform used as raycasting origin for splatting targets onto this container. If unused, simple planar projection will be used instead.")]
        public Transform projectFrom;

        /// <summary>
        /// Texture used for clearing the container's density buffer.
        /// </summary>
        [Tooltip("Texture used for clearing the container's density buffer.")]
        public Texture2D clearTexture;

        /// <summary>
        /// Color used to tint the clear texture.
        /// </summary>
        [Tooltip("Color used to tint the clear texture.")]
        public Color clearColor;

        /// <summary>
        /// Normal map used to determine container surface normal.
        /// </summary>
        [Tooltip("Normal map used to determine container surface normal.")]
        public Texture2D surfaceNormals;

        /// <summary>
        /// Tiling of surface normals.
        /// </summary>
        [Tooltip("Tiling of surface normal map.")]
        public Vector2 normalTiling = Vector2.one;

        /// <summary>
        /// Intensity of the surface normals.
        /// </summary>
        [Range(0, 1)]
        [Tooltip("Intensity of the surface normals.")]
        public float normalScale = 1;

        /// <summary>
        /// Falloff controls for density and velocity near container edges.
        /// </summary>
        [Tooltip("Falloff controls for density and velocity near container edges.")]
        public EdgeFalloff edgeFalloff;

        /// <summary>
        /// Falloff controls for density and velocity near container edges.
        /// </summary>
        [Tooltip("Falloff controls for density and velocity near container edges.")]
        public BoundaryConditions boundaries;

        /// <summary>
        /// Scale (0%-100%) of container's velocity. If set to zero, containers will be regarded as static.
        /// </summary>
        [Range(0, 1)]
        [Tooltip("Scale (0%-100%) of container's velocity. If set to zero, containers will be regarded as static.")]
        public float velocityScale = 1;

        /// <summary>
        /// Scale (0%-100%) of container's acceleration. This controls how much world-space inertia affects the fluid.
        /// </summary>
        [Range(0, 1)]
        [Tooltip("Scale (0%-100%) of container's acceleration. This controls how much world-space inertia affects the fluid.")]
        public float accelerationScale = 1;

        /// <summary>
        /// Local-space positional offset applied to the fluid.
        /// </summary>
        [Tooltip("Local-space positional offset applied to the fluid.")]
        public Vector2 positionOffset = Vector2.zero;

        /// <summary>
        /// World-space gravity applied to the fluid.
        /// </summary>
        [Tooltip("World-space gravity applied to the fluid.")]
        public Vector3 gravity = Vector3.zero;

        /// <summary>
        /// World-space external force applied to the fluid.
        /// </summary>
        [Tooltip("World-space external force applied to the fluid.")]
        public Vector3 externalForce = Vector3.zero;

        /// <summary>
        /// Lightsource used for volume rendering.
        /// </summary>
        [Tooltip("Lightsource used for volume rendering.")]
        public Light lightSource = null;

        /// <summary>
        /// List of targets that should be splatted onto this container.
        /// </summary>
        [Tooltip("List of targets that should be splatted onto this container.")]
#if UNITY_2020_2_OR_NEWER
        [NonReorderable]
#endif
        public FluxyTarget[] targets;

        /// <summary>
        /// Scales fluid pressure.
        /// </summary>
        [Range(0, 1)]
        [Tooltip("Scales fluid pressure.")]
        public float pressure = 1;

        /// <summary>
        /// Scales fluid viscosity.
        /// </summary>
        [Range(0, 1)]
        [Tooltip("Scales fluid viscosity.")]
        public float viscosity = 0;

        /// <summary>
        /// Amount of turbulence (vorticity) in the fluid.
        /// </summary>
        [Tooltip("Amount of turbulence (vorticity) in the fluid.")]
        public float turbulence = 5;

        /// <summary>
        /// Amount of adhesion to the container's surface.
        /// </summary>
        [Range(0, 1)]
        [Tooltip("Amount of adhesion to the container's surface.")]
        public float adhesion = 0;

        /// <summary>
        /// Amount of surface tension. Higher values will make the fluid tend to form round shapes.
        /// </summary>
        [Range(0, 1)]
        [Tooltip("Amount of surface tension. Higher values will make the fluid tend to form round shapes.")]
        public float surfaceTension = 0;

        /// <summary>
        /// Upwards buoyant force applied to fluid. It is directly proportional to the contents the density buffer's alpha channel (temperature).
        /// </summary>
        [Tooltip("Upwards buoyant force applied to fluid. It is directly proportional to the contents the density buffer's alpha channel (temperature).")]
        public float buoyancy = 1;

        /// <summary>
        /// Amount of density dissipated per second.
        /// </summary>
        [Tooltip("Amount of density dissipated per second.")]
        public Vector4 dissipation = Vector4.zero;     // rate at which state channels decrease.

        [SerializeField] [HideInInspector] private FluxySolver m_Solver;

        private Renderer m_Renderer;
        private MeshFilter m_Filter;
        private MaterialPropertyBlock propertyBlock;

        private Vector3 m_Velocity;
        private Vector3 m_AngularVelocity;
        private Vector3 oldPosition;
        private Quaternion oldRotation;
        private Vector3 oldVelocity;

        protected Mesh proceduralMesh;
        protected Vector3[] vertices;
        protected Vector3[] normals;
        protected Vector4[] tangents;
        protected Vector2[] uvs;
        protected int[] triangles;

        public delegate void ContainerCallback(FluxyContainer container);
        public event ContainerCallback OnFrameEnded;

        public FluxySolver solver
        {
            get { return m_Solver; }
            set
            {
                m_Solver = value;
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                {
                    SetSolver(m_Solver, true);
                }
            }
        }

        public Vector3 velocity
        {
            get { return m_Velocity; }
        }

        public Vector3 angularVelocity
        {
            get { return m_AngularVelocity; }
        }

        public Renderer containerRenderer
        {
            get { return m_Renderer; }
        }

        public Mesh containerMesh
        {
            get { return customMesh != null ? customMesh : proceduralMesh; }
        }

        protected virtual void OnEnable()
        {
            m_Renderer = GetComponent<Renderer>();
            m_Filter = GetComponent<MeshFilter>();
            propertyBlock = new MaterialPropertyBlock();

            UpdateContainerShape();
            SetSolver(m_Solver, false);
            oldPosition = transform.position;
            oldRotation = transform.rotation;
        }

        protected virtual void OnDisable()
        {
            DestroyImmediate(proceduralMesh);
            SetSolver(null, false);
        }

        protected virtual void Start()
        {
            if (Application.isPlaying)
                Clear();
        }

        protected virtual void OnValidate()
        {
            subdivisions.x = Mathf.Max(1, subdivisions.x);
            subdivisions.y = Mathf.Max(1, subdivisions.y);
        }

        public virtual void UpdateContainerShape()
        {
            switch (containerShape)
            {
                case ContainerShape.Plane: BuildPlaneMesh(); break;
                case ContainerShape.Volume: BuildVolumeMesh(); break;
                case ContainerShape.Custom: BuildCustomMesh(); break;
            }
        }

        protected void BuildPlaneMesh()
        {
            if (proceduralMesh == null)
            {
                proceduralMesh = new Mesh();
                proceduralMesh.name = "FluidContainer";
                if (m_Filter != null)
                    m_Filter.sharedMesh = proceduralMesh;
            }

            // create a new mesh:
            proceduralMesh.Clear();

            subdivisions.x = Mathf.Max(1, subdivisions.x);
            subdivisions.y = Mathf.Max(1, subdivisions.y);
            Vector2 quadSize = new Vector2(1.0f / subdivisions.x, 1.0f / subdivisions.y);

            int vertexCount = (subdivisions.x + 1) * (subdivisions.y + 1);
            int triangleCount = subdivisions.x * subdivisions.y * 2;

            if (vertexCount > 65535)
                proceduralMesh.indexFormat = IndexFormat.UInt32;
            else
                proceduralMesh.indexFormat = IndexFormat.UInt16;

            vertices = new Vector3[vertexCount];
            normals = new Vector3[vertexCount];
            tangents = new Vector4[vertexCount];
            uvs = new Vector2[vertexCount];
            triangles = new int[triangleCount * 3];

            // generate vertices:
            // for each row:
            for (int y = 0; y < subdivisions.y + 1; ++y)
            {
                // for each column:
                for (int x = 0; x < subdivisions.x + 1; ++x)
                {
                    int v = y * (subdivisions.x + 1) + x;
                    vertices[v] = new Vector3((quadSize.x * x - 0.5f) * size.x, (quadSize.y * y - 0.5f) * size.y, 0);
                    normals[v] = -Vector3.forward;
                    tangents[v] = new Vector4(1, 0, 0, -1);
                    uvs[v] = new Vector3(x / (float)subdivisions.x, y / (float)subdivisions.y);
                }
            }

            // generate triangle faces:
            // for each row:
            for (int y = 0; y < subdivisions.y; ++y)
            {
                // for each column:
                for (int x = 0; x < subdivisions.x; ++x)
                {

                    int face = (y * (subdivisions.x + 1) + x);
                    int t = (y * subdivisions.x + x) * 6;

                    triangles[t] = face + subdivisions.x + 1;
                    triangles[t + 1] = face + 1;
                    triangles[t + 2] = face;

                    triangles[t + 3] = face + subdivisions.x + 2;
                    triangles[t + 4] = face + 1;
                    triangles[t + 5] = face + subdivisions.x + 1;
                }
            }

            proceduralMesh.SetVertices(vertices);
            proceduralMesh.SetNormals(normals);
            proceduralMesh.SetTangents(tangents);
            proceduralMesh.SetUVs(0, uvs);
            proceduralMesh.SetIndices(triangles, MeshTopology.Triangles, 0);
            proceduralMesh.RecalculateNormals();
        }

        protected void BuildVolumeMesh()
        {
            if (proceduralMesh == null)
            {
                proceduralMesh = new Mesh();
                proceduralMesh.name = "FluidContainer";
                GetComponent<MeshFilter>().sharedMesh = proceduralMesh;
            }

            // create a new mesh:
            proceduralMesh.Clear();

            int vertexCount = 24;
            int triangleCount = 8;

            proceduralMesh.indexFormat = IndexFormat.UInt32;

            tangents = null;
            uvs = new Vector2[vertexCount];
            triangles = new int[triangleCount * 3];

            Vector3[] c = new Vector3[8];

            float length = size.x;
            float width = size.y;
            float height = size.z;
            c[0] = new Vector3(-length * .5f, -width * .5f, height * .5f);
            c[1] = new Vector3(length * .5f, -width * .5f, height * .5f);
            c[2] = new Vector3(length * .5f, -width * .5f, -height * .5f);
            c[3] = new Vector3(-length * .5f, -width * .5f, -height * .5f);

            c[4] = new Vector3(-length * .5f, width * .5f, height * .5f);
            c[5] = new Vector3(length * .5f, width * .5f, height * .5f);
            c[6] = new Vector3(length * .5f, width * .5f, -height * .5f);
            c[7] = new Vector3(-length * .5f, width * .5f, -height * .5f);

            vertices = new Vector3[]
            {
                c[0], c[1], c[2], c[3], // Bottom
	            c[7], c[4], c[0], c[3], // Left
	            c[4], c[5], c[1], c[0], // Front
	            c[6], c[7], c[3], c[2], // Back
	            c[5], c[6], c[2], c[1], // Right
	            c[7], c[6], c[5], c[4]  // Top
            };

            Vector3 up = -Vector3.up;
            Vector3 down = -Vector3.down;
            Vector3 forward = -Vector3.forward;
            Vector3 back = -Vector3.back;
            Vector3 left = -Vector3.left;
            Vector3 right = -Vector3.right;

            normals = new Vector3[]
            {
                down, down, down, down,             // Bottom
	            left, left, left, left,             // Left
	            forward, forward, forward, forward,	// Front
	            back, back, back, back,             // Back
	            right, right, right, right,         // Right
	            up, up, up, up	                    // Top
            };

            Vector2 uv00 = new Vector2(0f, 0f);
            Vector2 uv10 = new Vector2(1f, 0f);
            Vector2 uv01 = new Vector2(0f, 1f);
            Vector2 uv11 = new Vector2(1f, 1f);

            uvs = new Vector2[]
            {
                uv11, uv01, uv00, uv10, // Bottom
	            uv11, uv01, uv00, uv10, // Left
	            uv11, uv01, uv00, uv10, // Front
	            uv11, uv01, uv00, uv10, // Back	        
	            uv11, uv01, uv00, uv10, // Right 
	            uv11, uv01, uv00, uv10  // Top
            };

            triangles = new int[]
            {
                0, 1, 3,        1, 2, 3,        // Bottom	
	            4, 5, 7,        5, 6, 7,        // Left
	            8, 9, 11,       9, 10, 11,      // Front
	            12, 13, 15,     13, 14, 15,     // Back
	            16, 17, 19,     17, 18, 19,	    // Right
	            20, 21, 23,     21, 22, 23,	    // Top
            };

            proceduralMesh.SetVertices(vertices);
            proceduralMesh.SetNormals(normals);
            proceduralMesh.SetUVs(0, uvs);
            proceduralMesh.SetIndices(triangles, MeshTopology.Triangles, 0);
        }

        protected void BuildCustomMesh()
        {
            if (proceduralMesh != null)
                DestroyImmediate(proceduralMesh);

            if (customMesh != null)
                GetComponent<MeshFilter>().sharedMesh = customMesh;
        }

        private void SetSolver(FluxySolver newSolver, bool setMember)
        {
            if (m_Solver != null)
                m_Solver.UnregisterContainer(this);

            if (setMember)
                m_Solver = newSolver;

            if (m_Solver != null && isActiveAndEnabled)
                m_Solver.RegisterContainer(this);
        }

        public virtual void Clear()
        {
            if (m_Solver != null)
            {
                int id = m_Solver.GetContainerID(this);
                m_Solver.ClearContainer(id);
            }
        }

        public Vector3 TransformWorldVectorToUVSpace(in Vector3 vector, in Vector4 uvRect)
        {
            if (Mathf.Abs(size.x) < FluxyUtils.epsilon || Mathf.Abs(size.y) < FluxyUtils.epsilon)
                return Vector3.zero;

            var v = transform.InverseTransformVector(vector);
            v.x *= uvRect.z / size.x;
            v.y *= uvRect.w / size.y;
            return v;
        }

        public Vector3 TransformUVVectorToWorldSpace(in Vector3 vector, in Vector4 uvRect)
        {
            if (Mathf.Abs(uvRect.z) < FluxyUtils.epsilon || Mathf.Abs(uvRect.w) < FluxyUtils.epsilon)
                return Vector3.zero;

            var v = vector;
            v.x *= size.x / uvRect.z;
            v.y *= size.y / uvRect.w;
            return transform.TransformVector(v);
        }

        public Vector3 TransformWorldPointToUVSpace(in Vector3 point, in Vector4 uvRect)
        {
            var v = transform.InverseTransformPoint(point);

            v.x += size.x * 0.5f;
            v.y += size.y * 0.5f;

            v.x *= uvRect.z / size.x;
            v.y *= uvRect.w / size.y;

            v.x += uvRect.x;
            v.y += uvRect.y;

            return v;
        }

        public virtual void UpdateTransform()
        {
            if (lookAt != null)
            {
                if (lookAtMode == LookAtMode.LookAt)
                    transform.rotation = Quaternion.LookRotation(transform.position - lookAt.position, Vector3.up);
                else
                    transform.rotation = Quaternion.LookRotation(lookAt.forward, lookAt.up);
            }
            Shader.SetGlobalVector("_ContainerSize", size);
        }

        public virtual void UpdateMaterial(int tile, FluxyStorage.Framebuffer fb)
        {
            if (m_Renderer == null || m_Renderer.sharedMaterial == null || fb == null)
                return;

            containerRenderer.GetPropertyBlock(propertyBlock);

            propertyBlock.SetInt("_TileIndex", tile);
            propertyBlock.SetTexture("_MainTex", fb.stateA);
            propertyBlock.SetTexture("_Velocity", fb.velocityA);

            if (lightSource != null && lightSource.isActiveAndEnabled && lightSource.type == LightType.Directional)
            {
                m_Renderer.sharedMaterial.EnableKeyword("_LIGHTSOURCE_DIRECTIONAL");
                m_Renderer.sharedMaterial.DisableKeyword("_LIGHTSOURCE_POINT");
                m_Renderer.sharedMaterial.DisableKeyword("_LIGHTSOURCE_NONE");

                propertyBlock.SetVector("_LightVector", transform.InverseTransformDirection(lightSource.transform.forward));
                propertyBlock.SetVector("_LightColor", lightSource.color * lightSource.intensity);

            }
            else if (lightSource != null && lightSource.isActiveAndEnabled && lightSource.type == LightType.Point)
            {
                m_Renderer.sharedMaterial.DisableKeyword("_LIGHTSOURCE_DIRECTIONAL");
                m_Renderer.sharedMaterial.EnableKeyword("_LIGHTSOURCE_POINT");
                m_Renderer.sharedMaterial.DisableKeyword("_LIGHTSOURCE_NONE");

                Vector4 lightVec = transform.InverseTransformPoint(lightSource.transform.position);
                lightVec.w = 1f / Mathf.Max(lightSource.range * lightSource.range, 0.00001f);
                propertyBlock.SetVector("_LightVector", lightVec);
                propertyBlock.SetVector("_LightColor", lightSource.color * lightSource.intensity);
            }
            else
            {
                m_Renderer.sharedMaterial.DisableKeyword("_LIGHTSOURCE_DIRECTIONAL");
                m_Renderer.sharedMaterial.DisableKeyword("_LIGHTSOURCE_POINT");
                m_Renderer.sharedMaterial.EnableKeyword("_LIGHTSOURCE_NONE");
                propertyBlock.SetVector("_LightColor", Color.white);
            }

            containerRenderer.SetPropertyBlock(propertyBlock);
        }

        public virtual Vector4 ProjectTarget(in Vector3 targetPosition, Vector2 projectionSize, float aspectRatio, bool scaleWithDistance = true)
        {
            var origin = GetProjectionOrigin(targetPosition);

            Ray ray = new Ray(origin, targetPosition - origin);

            if (TryGetComponent(out MeshCollider meshCollider) && meshCollider.enabled)
            {
                if (meshCollider.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    float scale = 1;
                    if (scaleWithDistance)
                    {
                        ray = new Ray(origin, (targetPosition + transform.right * 0.01f) - origin);
                        if (meshCollider.Raycast(ray, out RaycastHit secondHit, Mathf.Infinity))
                            scale = Vector3.Distance(hit.point, secondHit.point) / 0.01f;
                    }

                    return new Vector4(hit.textureCoord.x - 0.5f, hit.textureCoord.y - 0.5f, projectionSize.x * scale * aspectRatio, projectionSize.y * scale);
                }
            }
            else
            {
                Plane p = new Plane(transform.forward, transform.position);

                if (p.Raycast(ray, out float dist))
                {
                    var point = ray.GetPoint(dist);
                    var local = transform.InverseTransformPoint(point) / (Vector2)size;

                    float scale = 1;
                    if (scaleWithDistance)
                    {
                        ray = new Ray(origin, (targetPosition + transform.right) - origin);
                        if (p.Raycast(ray, out float dist2))
                        {
                            var point2 = ray.GetPoint(dist2);
                            var local2 = transform.InverseTransformPoint(point2) / (Vector2)size;
                            scale = Vector2.Distance(local, local2);
                        }
                    }

                    return new Vector4(local.x, local.y, projectionSize.x * scale * aspectRatio, projectionSize.y * scale);
                }
            }

            return Vector4.zero;
        }

        private Vector3 GetProjectionOrigin(in Vector3 targetPosition)
        {
            // get projection origin position:
            if (projectFrom != null)
                return projectFrom.position;
            else
                return targetPosition + transform.forward;
        }

        public Vector3 UpdateVelocityAndGetAcceleration()
        {
            m_Velocity = (transform.position - oldPosition) / Time.deltaTime;

            Quaternion rotationDelta = transform.rotation * Quaternion.Inverse(oldRotation);
            m_AngularVelocity = new Vector3(rotationDelta.x, rotationDelta.y, rotationDelta.z) * 2.0f / Time.deltaTime;

            return (m_Velocity - oldVelocity) / Time.deltaTime;
        }

        private void ResetVelocityAndAcceleration()
        {
            oldVelocity = m_Velocity;
            oldRotation = transform.rotation;
            oldPosition = transform.position;
        }

        protected virtual void LateUpdate()
        {
            ResetVelocityAndAcceleration();
            OnFrameEnded?.Invoke(this);
        }

        public Vector3 GetVelocityAt(Vector3 worldPosition)
        {
            if (solver != null && (solver.readable & FluxySolver.ReadbackMode.Velocity) != 0)
            {
                var fb = solver.framebuffer;
                if (fb != null)
                {
                    var rect = solver.GetContainerUVRect(this);
                    var uv = TransformWorldPointToUVSpace(worldPosition, rect);
                    var color = solver.velocityReadbackTexture.GetPixelBilinear(uv.x, uv.y);

                    return TransformUVVectorToWorldSpace(new Vector3(color.r, color.g, 0), rect);
                }
            }

            return Vector3.zero;
        }

        public Vector4 GetDensityAt(Vector3 worldPosition)
        {
            if (solver != null && (solver.readable & FluxySolver.ReadbackMode.Density) != 0)
            {
                var fb = solver.framebuffer;
                if (fb != null)
                {
                    var rect = solver.GetContainerUVRect(this);
                    var uv = TransformWorldPointToUVSpace(worldPosition, rect);
                    var color = solver.densityReadbackTexture.GetPixelBilinear(uv.x, uv.y);

                    // no need to transform to world space (unlike velocity vectors) since densities are scalar values.
                    return color;
                }
            }

            return Vector4.zero;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (customMesh == null)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(Vector3.zero, size);
            }
        }

    }
}
