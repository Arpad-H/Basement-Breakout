using UnityEngine;
using System;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEditor;

namespace URPWater
{
    [ExecuteAlways]
    public class URPWaterDynamicEffects : MonoBehaviour
    {
        [Serializable]
        public enum Quality
        {
            VeryHigh,
            High,
            Medium,
            Low
        }

        [Serializable]
        public enum DebugRenderTexture
        {
            Off,
            RawTexture,
            CompTexture
        }

        [Header("Base")]
        [SerializeField]
        [Tooltip("Effect texture resolution\nVery High: 2048\nHigh: 1024\nMedium: 512\nLow: 256")]
        private Quality _Quality = Quality.High;

        [SerializeField]
        [Tooltip("Layers that will be used to capture the dynamic effects.")]
        private LayerMask _LayerMask = 2;

        [SerializeField]
        [Tooltip("Size of the capture region.")]
        private float _CaptureSize = 10f;

        [SerializeField]
        [Tooltip("Y distance of the capture camera from the target.")]
        private float _CaptureDistance = 10f;

        [SerializeField]
        [Range(0, 60)]
        [Tooltip("Limits the update of the render texture. Set this to 0 for no limits.")]
        private int _CapturePerSeconds = 0;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float _EdgeMaskSize = 0.1f;


        [Header("Normals")]
        [SerializeField]
        private bool _EnableNormalBlur = false;

        [SerializeField]
        [Range(0, 4)]
        private float _NormalBlur = 1;
        
        [SerializeField]
        [Range(0, 2)]
        private float _NormalStrength = 1;

        #if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField]
        private bool _ShowRegion = false;
        #endif
        
        [SerializeField]
        private DebugRenderTexture _DebugMode = DebugRenderTexture.Off;

        private RenderTexture _DynamicEffectsTexture;
        private RenderTexture _CompRT;
        private RenderTexture _TempRT1;
        private RenderTexture _TempRT2;


        private readonly int _NormalSharpId = Shader.PropertyToID("_NormalMap");
        private readonly int _BaseTexId = Shader.PropertyToID("_BaseTex");
        private readonly int _DynamicEffectsTextureId = Shader.PropertyToID("_DynamicEffectsTexture");
        private readonly int _DynamicEffectsParams = Shader.PropertyToID("_DynamicEffectsParams");
        private readonly int _NormalStrengthId = Shader.PropertyToID("_NormalStrength");
        private readonly int _NormalBlurId = Shader.PropertyToID("_BlurStrength");
        private readonly int _DebugTextureId = Shader.PropertyToID("_DebugTexture");

        private Camera _DynamicEffectsCamera;
        private Shader _EffectsShader;
        private Material _EffectsMaterial;
        private GameObject _DebugGO = null;
        private bool _EnableRender = false;

        const RenderTextureFormat _HdrFormat = RenderTextureFormat.ARGB32;

        public static event Action<ScriptableRenderContext, Camera> BeginDynamicEffects;


        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += ComputeEffects;

            if (_DebugGO == null)
            {
                _DebugGO = gameObject.GetComponentInChildren<Canvas>(true).gameObject;
            }

            if (_CapturePerSeconds > 0)
            {
                InvokeRepeating("ResetTime", 0f, 1f / _CapturePerSeconds);
            }
        }

        // Cleanup all the objects we possibly have created
        private void OnDisable()
        {
            Cleanup();
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        private void ResetTime()
        {
            _EnableRender = true;
        }

        private void ComputeEffects(ScriptableRenderContext context, Camera camera)
        {

            // Frame limiter
            if (_CapturePerSeconds == 0 || _EnableRender)
            {
                _EnableRender = false;
            }
            else { return; }
            
            // For overlay Camera
            var cameraData = camera.GetUniversalAdditionalCameraData(); 
            
            if (cameraData.renderType != CameraRenderType.Base) 
                return;

            // we dont want to render planar reflections in reflections or previews
            var cameraType = camera.cameraType;

            if (cameraType == CameraType.Reflection || cameraType == CameraType.Preview)
                return;

            if (_DynamicEffectsCamera == null)
            {
                _DynamicEffectsCamera = CreateEffectsCamera();
            }

            CreateEffectsMaterial();

            CreateEffectsTextures(); // create and assign RenderTexture

            UpdateEffectsCamera(_DynamicEffectsCamera);

            var data = new DynamicEffectsData(); // save quality settings and lower them for the planar reflections
            data.Set(); // set quality settings

            BeginDynamicEffects?.Invoke(context, _DynamicEffectsCamera); // callback Action for dynamic effect

            #pragma warning disable CS0618
            UniversalRenderPipeline.RenderSingleCamera(context, _DynamicEffectsCamera); // render dynamic effect
            #pragma warning restore CS0618

            DrawTextures();

            data.Restore(); // restore the quality settings
            SetShaderProperties();
        }

        /// <summary>
        /// Textures manipulations using shader
        /// </summary>
        private void DrawTextures()
        {
            _EffectsMaterial.SetFloat(_NormalStrengthId, _NormalStrength);
            _EffectsMaterial.SetFloat("_EdgeMaskSize", _EdgeMaskSize);

            // Create Normal
            if (_EnableNormalBlur)
            {
                _EffectsMaterial.SetFloat(_NormalBlurId, _NormalBlur);
            }
            else
            {
                _EffectsMaterial.SetFloat(_NormalBlurId, 0.0f);
            }


            Graphics.Blit(_DynamicEffectsTexture, _CompRT, _EffectsMaterial, 0);

            if (_EnableNormalBlur)
            {
                _EffectsMaterial.SetTexture(_BaseTexId, _CompRT);
                _EffectsMaterial.SetFloat(_NormalBlurId, _NormalBlur);

                // Blur Pass 1
                Graphics.Blit(_CompRT, _TempRT1, _EffectsMaterial, 1); // Blur H
                Graphics.Blit(_TempRT1, _TempRT2, _EffectsMaterial, 2); // Blur V

                // Blur Pass 2
                Graphics.Blit(_TempRT2, _TempRT1, _EffectsMaterial, 1); // Blur H
                Graphics.Blit(_TempRT1, _TempRT2, _EffectsMaterial, 2); // Blur V

                // Composite Channels
                _EffectsMaterial.SetTexture(_BaseTexId, _DynamicEffectsTexture);
                Graphics.Blit(_TempRT2, _CompRT, _EffectsMaterial, 3);
            }
            else
            {
                _EffectsMaterial.SetTexture(_BaseTexId, _DynamicEffectsTexture);

                Graphics.Blit(_CompRT, _TempRT1);
                Graphics.Blit(_TempRT1, _CompRT, _EffectsMaterial, 3); // Composite Channels
            }
        }

        private void SetShaderProperties()
        {

            var pos = transform.position;
            var parameters = new Vector4();
            parameters.x = pos.x;
            parameters.y = pos.y;
            parameters.z = pos.z;
            parameters.w = _CaptureSize * 2.0f;
            Shader.SetGlobalVector(_DynamicEffectsParams, parameters);

            // Assign texture to water shader
            Shader.SetGlobalTexture(_DynamicEffectsTextureId, _CompRT);

            // Debug RT
            var activateDebug = _DebugMode == DebugRenderTexture.Off ? false : true;
            if (_DebugGO == null)
            {
                if (activateDebug == true)
                {
                    Debug.LogWarning("DebugRT Object not found. Please use the original prefab of the Dynamic capture to get the DebugRT object.");
                }
                return;
            }

            _DebugGO.SetActive(activateDebug);

            switch (_DebugMode)
            {
                case DebugRenderTexture.Off:
                    break;
                case DebugRenderTexture.RawTexture:
                    Shader.SetGlobalTexture(_DebugTextureId, _DynamicEffectsTexture);
                    break;
                case DebugRenderTexture.CompTexture:
                    Shader.SetGlobalTexture(_DebugTextureId, _CompRT);
                    break;
            }


        }
        #region Camera

        private void UpdateEffectsCamera(Camera cam)
        {
            cam.orthographicSize = _CaptureSize;
            cam.farClipPlane = _CaptureDistance;
        }


        private Camera CreateEffectsCamera()
        {
            var go = new GameObject("Effects Camera", typeof(Camera));
            go.hideFlags = HideFlags.HideAndDontSave;
            go.transform.parent = this.transform;
            var cameraData = go.AddComponent(typeof(UniversalAdditionalCameraData)) as UniversalAdditionalCameraData;

            cameraData.renderShadows = false;
            cameraData.requiresColorOption = CameraOverrideOption.Off;
            cameraData.requiresDepthOption = CameraOverrideOption.Off;
            cameraData.SetRenderer(0);

            var t = transform;
            var position = Vector3.zero;
            var rotation = Quaternion.Euler(new Vector3(90f, 0f, 0));


            var effectsCamera = go.GetComponent<Camera>();
            effectsCamera.transform.localPosition = position;
            effectsCamera.transform.localRotation = rotation;
            effectsCamera.cullingMask = _LayerMask;
            effectsCamera.orthographic = true;
            effectsCamera.orthographicSize = _CaptureSize;
            effectsCamera.nearClipPlane = 0f;
            effectsCamera.farClipPlane = _CaptureDistance;
            effectsCamera.clearFlags = CameraClearFlags.Color;
            effectsCamera.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            effectsCamera.aspect = 1;
            effectsCamera.depth = -100;
            effectsCamera.enabled = false;

            return effectsCamera;
        }
        #endregion

        #region Material

        private void CreateEffectsMaterial()
        {
            if (_EffectsShader == null)
            {
                _EffectsShader = _EffectsShader = Shader.Find("Hidden/URPWaterEffects");
            }

            if (_EffectsMaterial == null)
            {
                _EffectsMaterial = new Material(_EffectsShader);
            }
        }

        #endregion


        #region Texture
        private void CreateEffectsTextures()
        {

            var res = EffectsResolution(UniversalRenderPipeline.asset.renderScale);
            var x = (int)res.x;
            var y = (int)res.y;

            if (_DynamicEffectsTexture == null)
            {
                _DynamicEffectsTexture = RenderTexture.GetTemporary(x, y, 16, GraphicsFormatUtility.GetGraphicsFormat(_HdrFormat, false));
                _DynamicEffectsTexture.useMipMap = false;
                _DynamicEffectsTexture.autoGenerateMips = false;
            }

            if (_CompRT == null)
            {
                _CompRT = RenderTexture.GetTemporary(x, y, 16, GraphicsFormatUtility.GetGraphicsFormat(_HdrFormat, false));
                _CompRT.useMipMap = false;
                _CompRT.autoGenerateMips = false;
            }

            if (_TempRT1 == null)
            {
                _TempRT1 = RenderTexture.GetTemporary(x / 2, y / 2, 16, GraphicsFormatUtility.GetGraphicsFormat(_HdrFormat, false));
                _TempRT1.useMipMap = false;
                _TempRT1.autoGenerateMips = false;
            }

            if (_EnableNormalBlur && _TempRT2 == null)
            {
                _TempRT2 = RenderTexture.GetTemporary(x / 2, y / 2, 16, GraphicsFormatUtility.GetGraphicsFormat(_HdrFormat, false));
                _TempRT2.useMipMap = false;
                _TempRT2.autoGenerateMips = false;
            }


            _DynamicEffectsCamera.targetTexture = _DynamicEffectsTexture;
        }
        
        private Vector2 EffectsResolution(float scale)
        {
            var x = (int)(2048 * scale * GetScaleValue());
            var y = (int)(2048 * scale * GetScaleValue());
            return new Vector2(x, y);
        }
        
        #endregion

        private Vector3 XZPos(Vector3 pos)
        {
            return new Vector3(pos.x, pos.y + _CaptureDistance, pos.z);
        }

        private float GetScaleValue()
        {
            switch (_Quality)
            {
                case Quality.VeryHigh:
                    return 1f;
                case Quality.High:
                    return 0.5f;
                case Quality.Medium:
                    return 0.33f;
                case Quality.Low:
                    return 0.25f;
                default:
                    return 0.5f;
            }
        }

        private void Cleanup()
        {
            RenderPipelineManager.beginCameraRendering -= ComputeEffects;

            if (_DynamicEffectsCamera)
            {
                _DynamicEffectsCamera.targetTexture = null;
                SafeDestroy(_DynamicEffectsCamera.gameObject);
            }

            if (_DynamicEffectsTexture)
            {
                RenderTexture.ReleaseTemporary(_DynamicEffectsTexture);
            }

            if (_CompRT)
            {
                RenderTexture.ReleaseTemporary(_CompRT);
            }

            if (_EffectsMaterial)
            {
                SafeDestroy(_EffectsMaterial);
            }

            if (_TempRT1)
            {
                RenderTexture.ReleaseTemporary(_TempRT1);
            }

            if (_TempRT2)
            {
                RenderTexture.ReleaseTemporary(_TempRT2);
            }
        }

        private static void SafeDestroy(UnityEngine.Object obj)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(obj);
            }
            else
            {
                Destroy(obj);
            }
        }

        void OnDrawGizmos()
        {
#if UNITY_EDITOR

            if (_ShowRegion == false || _DynamicEffectsCamera == null)
            {
                return;
            }


            var pos = _DynamicEffectsCamera.transform.position;
            pos.y -= _DynamicEffectsCamera.farClipPlane * 0.5f;

            var size = new Vector3(_CaptureSize * 2.0f, _DynamicEffectsCamera.farClipPlane, _CaptureSize * 2.0f);
            Gizmos.color = new Color(0, 0.5f, 1, 0.5f);
            Gizmos.DrawCube(pos, size);
            Gizmos.color = new Color(0, 0.5f, 1, 0.9f);
            Gizmos.DrawWireCube(pos, size);
#endif
        }

        /// <summary>
        /// Get or set the size of the capture.
        /// </summary>
        public float CaptureSize
        {
            get => _CaptureSize;
            set
            {
                _CaptureSize = value;
            }
        }

        /// <summary>
        /// Get or set the Y distance range of the capture.
        /// </summary>
        public float CaptureDistance
        {
            get => _CaptureDistance;
            set
            {
                _CaptureDistance = value;
            }
        }


        class DynamicEffectsData
        {
            private readonly bool _Fog;
            private readonly int _MaxLod;
            private readonly float _LodBias;
            private readonly bool _DrawGizmos;

            public DynamicEffectsData()
            {
                _Fog = RenderSettings.fog;
                _MaxLod = QualitySettings.maximumLODLevel;
                _LodBias = QualitySettings.lodBias;
#if UNITY_EDITOR
                if (SceneView.currentDrawingSceneView != null)
                    _DrawGizmos = SceneView.currentDrawingSceneView.drawGizmos;
#endif
            }

            public void Set()
            {
                //GL.invertCulling = true;
#if UNITY_EDITOR
                if (SceneView.currentDrawingSceneView != null)
                    SceneView.currentDrawingSceneView.drawGizmos = false;
#endif
                RenderSettings.fog = false; // disable fog for now as it's incorrect with projection
                QualitySettings.maximumLODLevel = 1;
                QualitySettings.lodBias = _LodBias * 0.5f;
            }

            public void Restore()
            {
                GL.invertCulling = false;
                #if UNITY_EDITOR
                if (SceneView.currentDrawingSceneView != null)
                SceneView.currentDrawingSceneView.drawGizmos = _DrawGizmos;
                #endif
                RenderSettings.fog = _Fog;
                QualitySettings.maximumLODLevel = _MaxLod;
                QualitySettings.lodBias = _LodBias;
            }
        }

    }
}
