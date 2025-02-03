//-----------------------------------------------------------------------------
// Copyright(C) Yan Verde - All Rights Reserved
// Copyright protected under Unity Asset Store EULA
// Refer to https://unity3d.com/legal/as_terms for more informations
// -----------------------------------------------------------------------------
// URP Water
// Author : Yan Verde
// Date : April 10, 2021
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class URPWaterEditor : ShaderGUI
{

    const string TEXTURE_PREFIX = "T_";
    const string VERSION = "1.2.2";


    MaterialEditor _Editor;
    MaterialProperty[] _Properties;
    Material _Target;
    bool _RefractionPanelVisible = true;
    bool _SpecularPanelVisible = true;
    bool _NormalsPanelVisible = true;
    bool _EdgeFadePanelVisible = true;
    bool _FoamPanelVisible = true;
    bool _CausticPanelVisible = true;
    bool _ScatteringPanelVisible = true;
    bool _ReflectionPanelVisible = true;
    bool _DisplacementPanelVisible = true;
    bool _DynamicEffectsPanelVisible = true;
    bool _OptionsPanelVisible = true;
    bool _TessellationPanelVisible = true;

    Gradient _RefractionGradient = new Gradient();
    List<GradientColorKey> _GradientColorKeys = new List<GradientColorKey>();

    GUIStyle _FoldoutStyle;
    GUIStyle _LabelStyle;

    //GUIContent _WaveAmplitudeLabel = new GUIContent("Amplitude", "General Amplitude of the wave. Affects position, normals, foam and scattering.");
    GUIContent _WaveEffectsLabel = new GUIContent("Wave Effect Boost", "Increase the whitecaps and scattering effects on wave caps.");
    GUIContent _CausticsAngleMask = new GUIContent("Angle Mask", "Use the normals from the geometry underwater to mask the caustics. \nThis fixes the issue were caustics will aprear to be stretching on steep angles, but is an expensive operation.");

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {

        this._Editor = materialEditor;
        this._Properties = properties;
        this._Target = _Editor.target as Material;
        _RefractionGradient = DecodeGradientFromMaterial();

        _FoldoutStyle = new GUIStyle(EditorStyles.foldout);
        _FoldoutStyle.fontStyle = FontStyle.Bold;

        _LabelStyle = new GUIStyle(EditorStyles.label);
        _LabelStyle.fontStyle = FontStyle.Bold;

        //_Editor.TextureScaleOffsetProperty(mainTex);
        //GUIContent albedoLabel = new GUIContent(mainTex.displayName);
        //_Editor.TexturePropertySingleLine(albedoLabel, mainTex);
        DrawHeader();
        DrawRefraction();
        DrawSpecular();
        DrawNormals();
        DrawReflection();
        DrawEdgeFade();
        DrawFoam();
        DrawCaustic();
        DrawScattering();
        DrawDisplacement();
        DrawDynamicEffects();
        DrawOptions();
        DrawTessellation();

        GUILayout.Label("Version: " + VERSION, _LabelStyle);
    }

    void DrawHeader()
    {

        // Use default labelWidth
        EditorGUIUtility.labelWidth = 0f;
        EditorGUIUtility.fieldWidth = 64f;

        Texture2D tex = Resources.Load("URPWaterLogo") as Texture2D;

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(tex);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


    }

    void DrawRefraction()
    {
        GUILayout.BeginVertical("GroupBox");
        EditorGUI.indentLevel += 1;
        _RefractionPanelVisible = EditorGUILayout.Foldout(_RefractionPanelVisible, "Refraction", _FoldoutStyle);
        EditorGUI.indentLevel += 1;

        if (!_RefractionPanelVisible)
        {
            EditorGUI.indentLevel -= 2;
            GUILayout.EndVertical();
            return;
        }

        var ColorMode = FindProperty("_ColorMode");
        _Editor.ShaderProperty(ColorMode, "Mode");

        if (ColorMode.floatValue == 0)
        {
            var colorA = FindProperty("_Color");
            var colorB = FindProperty("_DepthColor");

            _Editor.ShaderProperty(colorA, "Shallow Color");
            _Editor.ShaderProperty(colorB, "Depth Color");
        }


        if (ColorMode.floatValue == 1)
        {
            var rColorProperty = FindProperty("_RefractionColor");


            EditorGUI.BeginChangeCheck();


            _RefractionGradient = EditorGUILayout.GradientField("Color", _RefractionGradient);

            if (EditorGUI.EndChangeCheck())
            {

                var width = 128;
                var height = 4;
                var isNewFile = false;//rColorProperty.textureValue == null ? true : false;

                if (rColorProperty.textureValue == null)
                {
                    isNewFile = true;
                }


                // In case user duplicated the material
                if (rColorProperty.textureValue != null)
                {
                    var textureName = rColorProperty.textureValue.name;
                    var matName = TEXTURE_PREFIX + _Target.name;

                    if (matName != textureName)
                    {
                        isNewFile = true;
                        rColorProperty.textureValue = null;
                    }
                }


                var savePath = GetRefractionTexturePath(rColorProperty);

                var newTexture = GenerateGradientTexture(_RefractionGradient, width, height);
                rColorProperty.textureValue = SaveTextureAsPNG(newTexture, savePath);

                Object.DestroyImmediate(newTexture);
                //newTexture = null;
                //Resources.UnloadUnusedAssets();

                if (isNewFile == true)
                {
                    ApplyImportSettings(savePath);
                    Debug.Log("texture was saved as: " + savePath);
                }

                EncodeGradientToMaterial(_RefractionGradient);
            }
        }

        var doubleSided = FindProperty("_DoubleSided");
        if (doubleSided.floatValue == 1)
        {
            var underwaterColor = FindProperty("_UnderWaterColor");
            _Editor.ShaderProperty(underwaterColor, new GUIContent("UnderWaterColor", "RGB: Color tint of the underwater surface A: Color fog"));
        }

        var depthStart = FindProperty("_DepthStart");
        var depthEnd = FindProperty("_DepthEnd");
        var distortion = FindProperty("_Distortion");


        _Editor.ShaderProperty(depthStart, "Depth Start");
        _Editor.ShaderProperty(depthEnd, "Depth End");
        _Editor.ShaderProperty(distortion, "Distortion");



        EditorGUI.indentLevel -= 2;
        GUILayout.EndVertical();
    }

    void DrawSpecular()
    {

        GUILayout.BeginVertical("GroupBox");
        EditorGUI.indentLevel += 1;
        _SpecularPanelVisible = EditorGUILayout.Foldout(_SpecularPanelVisible, "Specular", _FoldoutStyle);
        EditorGUI.indentLevel += 1;

        if (!_SpecularPanelVisible)
        {
            EditorGUI.indentLevel -= 2;
            GUILayout.EndVertical();
            return;
        }

        var SpecColor = FindProperty("_SpecColor");
        var Smoothness = FindProperty("_Smoothness");

        _Editor.ShaderProperty(SpecColor, "Specular Color");
        _Editor.ShaderProperty(Smoothness, "Smoothness");


        EditorGUI.indentLevel -= 2;
        GUILayout.EndVertical();
    }

    void DrawNormals()
    {
        //GUILayout.BeginVertical("", GUI.skin.box);
        GUILayout.BeginVertical("GroupBox");
        EditorGUI.indentLevel += 1;
        _NormalsPanelVisible = EditorGUILayout.Foldout(_NormalsPanelVisible, "Normal Maps", _FoldoutStyle);
        EditorGUI.indentLevel += 1;

        if (!_NormalsPanelVisible)
        {
            EditorGUI.indentLevel -= 2;
            GUILayout.EndVertical();
            return;
        }

        var normalsMode = FindProperty("_NormalsMode");
        _Editor.ShaderProperty(normalsMode, "Mode");


        // Facet
        if (normalsMode.floatValue == 3)
        {
            EditorGUI.indentLevel -= 2;
            GUILayout.EndVertical();
            return;
        }

        // Normal A
        var normalA = FindProperty("_NormalMapA");
        var normalMapATilings = FindProperty("_NormalMapATilings");
        var normalMapAIntensity = FindProperty("_NormalMapAIntensity");
        _Editor.TexturePropertySingleLine(MakeLabel("NormalMap"), normalA);

        // Normal A Params
        if (normalsMode.floatValue == 0 || normalsMode.floatValue == 1)
        {
            var normalMapASpeeds = FindProperty("_NormalMapASpeeds");


            EditorGUI.indentLevel += 2;
            DrawDoubleVector2FromVector4(normalMapATilings, "Tiling A", "Tiling B");
            DrawDoubleVector2FromVector4(normalMapASpeeds, "Speed A", "Speed B");
            _Editor.ShaderProperty(normalMapAIntensity, "Intensity");
            EditorGUI.indentLevel -= 2;
        }

        // Normal B
        if (normalsMode.floatValue == 1)
        {
            var normalB = FindProperty("_NormalMapB");
            var normalBTilings = FindProperty("_NormalMapBTilings");
            var normalBSpeeds = FindProperty("_NormalMapBSpeeds");
            var normalBIntensity = FindProperty("_NormalMapBIntensity");

            _Editor.TexturePropertySingleLine(MakeLabel("NormalMap B"), normalB);
            EditorGUI.indentLevel += 2;
            DrawVector2FromVector4(normalBTilings, "Tiling");
            DrawVector2FromVector4(normalBSpeeds, "Speed");
            _Editor.ShaderProperty(normalBIntensity, "Intensity");
            EditorGUI.indentLevel -= 2;
        }

        // Flow Map
        if (normalsMode.floatValue == 2)
        {
            var flowMap = FindProperty("_FlowMap");
            var flowSpeed = FindProperty("_FlowSpeed");
            var flowIntensity = FindProperty("_FlowIntensity");
            var flowTiling = FindProperty("_FlowTiling");

            EditorGUI.indentLevel += 2;
            DrawVector2FromVector4(normalMapATilings, "Tiling");
            _Editor.ShaderProperty(normalMapAIntensity, "Intensity");
            EditorGUI.indentLevel -= 2;

            _Editor.TexturePropertySingleLine(MakeLabel("FlowMap"), flowMap);
            EditorGUI.indentLevel += 2;
            DrawDoubleVector2FromVector4(flowTiling, "Tiling", "Offset");
            _Editor.ShaderProperty(flowSpeed, "Flow Speed");
            _Editor.ShaderProperty(flowIntensity, "Flow Intensity");
            EditorGUI.indentLevel -= 2;
        }

        // Far
        var farMode = FindProperty("_NormalFar");

        _Editor.ShaderProperty(farMode, new GUIContent("Enable Far Map", "Use a different texture when the camera is far from the surface"));
        if (farMode.floatValue == 1)
        {
            EditorGUI.indentLevel += 2;

            var farMap = FindProperty("_NormalMapFar");
            var farTilings = FindProperty("_NormalMapFarTilings");
            var farSpeeds = FindProperty("_NormalMapFarSpeeds");
            var farIntensity = FindProperty("_NormalMapFarIntensity");
            var farDistance = FindProperty("_NormalFarDistance");

            _Editor.TexturePropertySingleLine(MakeLabel("Far Texture"), farMap);
            DrawDoubleVector2FromVector4(farTilings, "Far Tiling A", "Far Tiling B");
            DrawDoubleVector2FromVector4(farSpeeds, "Far Speed A", "Far Speed B");
            _Editor.ShaderProperty(farIntensity, "Far Intensity");
            _Editor.ShaderProperty(farDistance, new GUIContent("Far Distance", "Distance of transition between near and far maps."));
            EditorGUI.indentLevel -= 2;

        }


        EditorGUI.indentLevel -= 2;
        GUILayout.EndVertical();
    }

    void DrawEdgeFade()
    {

        GUILayout.BeginVertical("GroupBox");
        EditorGUI.indentLevel += 1;
        _EdgeFadePanelVisible = EditorGUILayout.Foldout(_EdgeFadePanelVisible, "Edge Fade", _FoldoutStyle);
        EditorGUI.indentLevel += 1;

        if (!_EdgeFadePanelVisible)
        {
            EditorGUI.indentLevel -= 2;
            GUILayout.EndVertical();
            return;
        }

        var enableEdggeFade = FindProperty("_EdgeFade");
        var edgeSize = FindProperty("_EdgeSize");

        _Editor.ShaderProperty(enableEdggeFade, "Enable");

        if (enableEdggeFade.floatValue == 1)
        {
            _Editor.ShaderProperty(edgeSize, "Edge Size");
        }

        EditorGUI.indentLevel -= 2;
        GUILayout.EndVertical();
    }

    void DrawFoam()
    {

        GUILayout.BeginVertical("GroupBox");
        EditorGUI.indentLevel += 1;
        _FoamPanelVisible = EditorGUILayout.Foldout(_FoamPanelVisible, "Foam", _FoldoutStyle);
        EditorGUI.indentLevel += 1;

        if (!_FoamPanelVisible)
        {
            EditorGUI.indentLevel -= 2;
            GUILayout.EndVertical();
            return;
        }

        var enableFoam = FindProperty("_Foam");
        var foamColor = FindProperty("_FoamColor");
        var foamTex = FindProperty("_FoamTex");
        var foamTiling = FindProperty("_FoamTiling");
        var foamSize = FindProperty("_FoamSize");
        var foamDistortion = FindProperty("_FoamDistortion");

        _Editor.ShaderProperty(enableFoam, "Enable");


        if (enableFoam.floatValue == 1)
        {
            _Editor.ShaderProperty(foamColor, "Color");
            _Editor.TexturePropertySingleLine(MakeLabel("Texture"), foamTex);
            EditorGUI.indentLevel += 2;
            DrawVector2FromVector4(foamTiling, "Tiling");
            //DrawVector2FromVector4(normalBSpeeds, "Speed");
            //_Editor.ShaderProperty(normalBIntensity, "Intensity");
            EditorGUI.indentLevel -= 2;

            _Editor.ShaderProperty(foamSize, "Size");
            _Editor.ShaderProperty(foamDistortion, "Distortion");

            var foamRipples = FindProperty("_FoamRipples");
            _Editor.ShaderProperty(foamRipples, "Ripples");

            if (foamRipples.floatValue == 1)
            {
                var rippleDistance = FindProperty("_FoamRippleDistance");
                var rippleSize = FindProperty("_FoamRippleSize");
                var rippleSpeed = FindProperty("_FoamRippleSpeed");
                EditorGUI.indentLevel += 2;
                _Editor.ShaderProperty(rippleDistance, "Ripple Distance");
                _Editor.ShaderProperty(rippleSize, "Ripple Size");
                _Editor.ShaderProperty(rippleSpeed, "Ripple Speed");
                EditorGUI.indentLevel -= 2;

            }
        }

        var whiteCaps = FindProperty("_FoamWhiteCaps");
        var displacementEnabled = _Target.IsKeywordEnabled("_DISPLACEMENTMODE_GERSTNER");
        if (displacementEnabled)
        {

            _Editor.ShaderProperty(whiteCaps, "White Caps");
        }

        if (whiteCaps.floatValue == 1 && displacementEnabled)
        {

            var capsIntensity = FindProperty("_FoamCapsIntensity");
            var min = FindProperty("_FoamCapsRangeMin");
            var max = FindProperty("_FoamCapsRangeMax");
            var minValue = min.floatValue;
            var maxValue = max.floatValue;

            EditorGUI.indentLevel += 2;

            EditorGUILayout.MinMaxSlider("Range", ref minValue, ref maxValue, 0f, 2f);

            min.floatValue = minValue;
            max.floatValue = maxValue;

            _Editor.ShaderProperty(capsIntensity, "Caps Intensity");
            EditorGUI.indentLevel -= 2;
        }


        EditorGUI.indentLevel -= 2;
        GUILayout.EndVertical();
    }

    void DrawScattering()
    {
        GUILayout.BeginVertical("GroupBox");
        EditorGUI.indentLevel += 1;
        _ScatteringPanelVisible = EditorGUILayout.Foldout(_ScatteringPanelVisible, "Scattering", _FoldoutStyle);
        EditorGUI.indentLevel += 1;

        if (!_ScatteringPanelVisible)
        {
            EditorGUI.indentLevel -= 2;
            GUILayout.EndVertical();
            return;
        }


        var enableScattering = FindProperty("_Scattering");

        _Editor.ShaderProperty(enableScattering, "Enable");

        if (enableScattering.floatValue == 1f)
        {
            var color = FindProperty("_ScatteringColor");
            var intensity = FindProperty("_ScatteringIntensity");
            var min = FindProperty("_ScatteringRangeMin");
            var max = FindProperty("_ScatteringRangeMax");


            _Editor.ShaderProperty(color, "Color");
            _Editor.ShaderProperty(intensity, "Intensity");

            var minValue = min.floatValue;
            var maxValue = max.floatValue;

            EditorGUILayout.MinMaxSlider("Range", ref minValue, ref maxValue, 0f, 2f);

            min.floatValue = minValue;
            max.floatValue = maxValue;
        }

        var enableCapsScattering = FindProperty("_CapsScattering");

        if (_Target.IsKeywordEnabled("_DISPLACEMENTMODE_GERSTNER"))
        {

            _Editor.ShaderProperty(enableCapsScattering, "Caps Scattering");

            if (enableCapsScattering.floatValue == 1f)
            {
                var intensity = FindProperty("_CapsScatteringIntensity");
                var min = FindProperty("_CapsScatteringRangeMin");
                var max = FindProperty("_CapsScatteringRangeMax");
                var normalInfluence = FindProperty("_CapsScatterNormals");

                EditorGUI.indentLevel += 2;
                _Editor.ShaderProperty(intensity, "Intensity");

                var minValue = min.floatValue;
                var maxValue = max.floatValue;

                EditorGUILayout.MinMaxSlider("Range", ref minValue, ref maxValue, 0f, 2f);

                min.floatValue = minValue;
                max.floatValue = maxValue;
                _Editor.ShaderProperty(normalInfluence, "Normals Influence");
                EditorGUI.indentLevel = 2;
            }

        }

        EditorGUI.indentLevel -= 2;
        GUILayout.EndVertical();
    }

    void DrawCaustic()
    {

        GUILayout.BeginVertical("GroupBox");
        EditorGUI.indentLevel += 1;
        _CausticPanelVisible = EditorGUILayout.Foldout(_CausticPanelVisible, "Caustics", _FoldoutStyle);
        EditorGUI.indentLevel += 1;

        if (!_CausticPanelVisible)
        {
            EditorGUI.indentLevel -= 2;
            GUILayout.EndVertical();
            return;
        }

        var enableCaustic = FindProperty("_Caustics");
        var causticMode = FindProperty("_CausticsMode");

        var causticTex = FindProperty("_CausticsTex");
        var causticTiling = FindProperty("_CausticsTiling");
        var causticSpeed = FindProperty("_CausticsSpeed");
        var causticIntensity = FindProperty("_CausticsIntensity");
        var causticStart = FindProperty("_CausticsStart");
        var causticEnd = FindProperty("_CausticsEnd");
        var causticAngleMask = FindProperty("_CausticsAngleMask");
        var directionMaskLabel = new GUIContent("Directional Mask", "Use the normals from the geometry underwater and light direction to mask the caustics. \nThis is an expensive operation.");
        var causticDirectionMask = FindProperty("_CausticsDirectionMask");
        //var causticDistortion = FindProperty("_CausticsDistortion");

        _Editor.ShaderProperty(enableCaustic, "Enable");

        if (enableCaustic.floatValue == 1)
        {
            _Editor.ShaderProperty(causticMode, new GUIContent("Caustics Mode", "Use 2D or 3D Texture\n3D texture gives better result at all angles but is not supported on all platforms"));
            _Editor.ShaderProperty(causticDirectionMask, directionMaskLabel);
            _Editor.ShaderProperty(causticAngleMask, _CausticsAngleMask);

            if (causticMode.floatValue == 1)
            {
                //3D Caustics
                var causticsTex3D = FindProperty("_CausticsTex3D");
                var causticsTiling3D = FindProperty("_CausticsTiling3D");
                var causticsSpeed3D = FindProperty("_CausticsSpeed3D");

                _Editor.TexturePropertySingleLine(MakeLabel("Texture"), causticsTex3D);

                EditorGUI.indentLevel = 4;
                _Editor.ShaderProperty(causticsTiling3D, "Tiling");
                _Editor.ShaderProperty(causticsSpeed3D, "Speed");

                EditorGUI.indentLevel = 2;

            }
            else
            {
                //2D Caustics
                _Editor.TexturePropertySingleLine(MakeLabel("Texture"), causticTex);

                EditorGUI.indentLevel = 4;
                DrawDoubleVector2FromVector4(causticTiling, "Tiling A", "Tiling B");
                DrawDoubleVector2FromVector4(causticSpeed, "Speed A", "Speed B");
                EditorGUI.indentLevel = 2;
            }

            _Editor.ShaderProperty(causticIntensity, "Intensity");
            _Editor.ShaderProperty(causticStart, "Start");
            _Editor.ShaderProperty(causticEnd, "End");
            //_Editor.ShaderProperty(causticDistortion, new GUIContent("Distortion", "Controls the amount of distortion the caustics will get from the surface normal."));
        }

        EditorGUI.indentLevel -= 2;
        GUILayout.EndVertical();
    }

    void DrawReflection()
    {

        GUILayout.BeginVertical("GroupBox");
        EditorGUI.indentLevel += 1;
        _ReflectionPanelVisible = EditorGUILayout.Foldout(_ReflectionPanelVisible, "Reflection", _FoldoutStyle);
        EditorGUI.indentLevel += 1;

        if (!_ReflectionPanelVisible)
        {
            EditorGUI.indentLevel -= 2;
            GUILayout.EndVertical();
            return;
        }

        var reflectionMode = FindProperty("_ReflectionMode");
        var rFresnel = FindProperty("_ReflectionFresnel");
        var rFresnelNormal = FindProperty("_ReflectionFresnelNormal");
        var rIntensity = FindProperty("_ReflectionIntensity");
        var rDistortion = FindProperty("_ReflectionDistortion");

        _Editor.ShaderProperty(reflectionMode, "Mode");

        if (reflectionMode.floatValue == 1)
        {
            var rCubeMap = FindProperty("_CubemapTexture");
            //_Editor.ShaderProperty(rCubeMap, "CubeMap");
            _Editor.TexturePropertySingleLine(MakeLabel("CubeMap"), rCubeMap);
        }


        if (reflectionMode.floatValue != 0)
        {
            var roughness = FindProperty("_ReflectionRoughness");

            _Editor.ShaderProperty(rIntensity, "Intensity");
            _Editor.ShaderProperty(rFresnel, "Fresnel");
            _Editor.ShaderProperty(rFresnelNormal, "Fresnel Normal");
            _Editor.ShaderProperty(rDistortion, "Distortion");
            _Editor.ShaderProperty(roughness, "Roughness");
        }

        EditorGUI.indentLevel -= 2;
        GUILayout.EndVertical();
    }

    void DrawDisplacement()
    {

        GUILayout.BeginVertical("GroupBox");
        EditorGUI.indentLevel += 1;
        _DisplacementPanelVisible = EditorGUILayout.Foldout(_DisplacementPanelVisible, "Displacement", _FoldoutStyle);
        EditorGUI.indentLevel += 1;

        if (!_DisplacementPanelVisible)
        {
            EditorGUI.indentLevel -= 2;
            GUILayout.EndVertical();
            return;
        }

        var displacementMode = FindProperty("_DisplacementMode");
        _Editor.ShaderProperty(displacementMode, "Mode");

        // Gerstner Wave
        if (displacementMode.floatValue == 1)
        {

            var whiteCaps = FindProperty("_FoamWhiteCaps");
            var capsScatter = FindProperty("_CapsScattering");
            var waveNormal = FindProperty("_WaveNormal");
            var effectsBoost = FindProperty("_WaveEffectsBoost");
            var fadeStart = FindProperty("_WaveStartDistance");
            var fadeEnd = FindProperty("_WaveEndDistance");

            _Editor.ShaderProperty(waveNormal, new GUIContent("Normals", "Controls how much the waves affect the normals"));

            if (whiteCaps.floatValue == 1 || capsScatter.floatValue == 1)
            {
                _Editor.ShaderProperty(effectsBoost, _WaveEffectsLabel);
            }

            
            EditorGUILayout.LabelField("Waves Parameters", _LabelStyle);
            EditorGUI.indentLevel += 2;
            DrawGersnerWave();
            EditorGUI.indentLevel -= 2;


            EditorGUILayout.LabelField("Fade Distance", _LabelStyle);
            EditorGUI.indentLevel += 2;
            _Editor.ShaderProperty(fadeStart, "Start");
            _Editor.ShaderProperty(fadeEnd, "End");
            EditorGUI.indentLevel -= 2;
        }

        EditorGUI.indentLevel -= 2;
        GUILayout.EndVertical();
    }

    void DrawGersnerWave()
    {

        var waveCount = FindProperty("_WaveCount");
        var waveLength = FindProperty("_WaveLength");
        var waveAmplitude = FindProperty("_WaveAmplitude");
        var waveSteepness = FindProperty("_WaveSteepness");
        var waveSpeed = FindProperty("_WaveSpeed");

        //var directionVector = new Vector2(vectorValues.x, vectorValues.y);

        //var deg = VectorToDegrees(directionVector);

        /*
        deg = EditorGUILayout.Slider("Angle", deg, -90.0f, 90.0f);
        var angle = deg * Mathf.Deg2Rad;
        var directionFromAngle = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)); // The direction vector values to set in the shader
        */
        waveCount.floatValue = EditorGUILayout.IntSlider("Wave Count", (int)waveCount.floatValue, 1, 32);
        _Editor.ShaderProperty(waveAmplitude, "Amplitude");
        _Editor.ShaderProperty(waveSteepness, "Steepness");
        _Editor.ShaderProperty(waveLength, "Length");
        _Editor.ShaderProperty(waveSpeed, "Speed");
    }

    void DrawTessellation()
    {

        if (!_Target.HasProperty("_Tess")) { return; }


        GUILayout.BeginVertical("GroupBox");
        EditorGUI.indentLevel += 1;
        _TessellationPanelVisible = EditorGUILayout.Foldout(_TessellationPanelVisible, "Tessellation", _FoldoutStyle);
        EditorGUI.indentLevel += 1;

        if (!_TessellationPanelVisible)
        {
            EditorGUI.indentLevel -= 2;
            GUILayout.EndVertical();
            return;
        }

        var tess = FindProperty("_Tess");
        _Editor.ShaderProperty(tess, "Tessellation Factor");


        EditorGUI.indentLevel -= 2;
        GUILayout.EndVertical();
    }

    float VectorToDegrees(Vector2 vec)
    {
        var angle = Mathf.Atan(vec.y / vec.x) * Mathf.Rad2Deg;

        if (vec.x < 0)
        {
            angle += vec.y < 0 ? -180 : 180;
        }

        return angle;
    }

    void DrawDynamicEffects()
    {

        GUILayout.BeginVertical("GroupBox");
        EditorGUI.indentLevel += 1;
        _DynamicEffectsPanelVisible = EditorGUILayout.Foldout(_DynamicEffectsPanelVisible, "Dynamic Effects", _FoldoutStyle);
        EditorGUI.indentLevel += 1;

        if (!_DynamicEffectsPanelVisible)
        {
            EditorGUI.indentLevel -= 2;
            GUILayout.EndVertical();
            return;
        }

        var enableEffects = FindProperty("_DynamicEffects");
        var enableText = new GUIContent("Enable", "Uses URP Dynamic Effects Script to render dynamic objects that will affect the water. \n\nThe color of rendered objects defines what it will affect.\n\nRed: Generates normal map and displacement.\nGreen: Generates foam.");

        _Editor.ShaderProperty(enableEffects, enableText);

        if (enableEffects.floatValue == 1)
        {

            var normalStrength = FindProperty("_DynamicNormal");
            var displacementStrength = FindProperty("_DynamicDisplacement");
            var foamStrength = FindProperty("_DynamicFoam");

            _Editor.ShaderProperty(normalStrength, "Normal Strength");
            _Editor.ShaderProperty(displacementStrength, "Displacement Strength");
            _Editor.ShaderProperty(foamStrength, "Foam Strength");


        }

        EditorGUI.indentLevel -= 2;
        GUILayout.EndVertical();
    }

    void DrawOptions()
    {
        GUILayout.BeginVertical("GroupBox");
        EditorGUI.indentLevel = 1;
        _OptionsPanelVisible = EditorGUILayout.Foldout(_OptionsPanelVisible, "Options", _FoldoutStyle);
        EditorGUI.indentLevel = 2;

        if (!_OptionsPanelVisible)
        {
            EditorGUI.indentLevel = 0;
            GUILayout.EndVertical();
            return;
        }

        var doubleSided = FindProperty("_DoubleSided");
        var zWrite = FindProperty("_ZWrite");
        var worldUV = FindProperty("_WorldUV");
        var ortho = FindProperty("_Orthographic");
        var addFoam = FindProperty("_AddFoam");
        //var speedMul = FindProperty("_SpeedMul");
        var dispMask = FindProperty("_DispMask");
        var alphaMask = FindProperty("_AlphaMask");


        switch (doubleSided.floatValue)
        {
            case 0f:
                _Target.SetFloat("_CullMode", 2);
                break;
            case 1f:
                _Target.SetFloat("_CullMode", 0);
                break;
        }

        switch (zWrite.floatValue)
        {
            case 0f:
                _Target.SetFloat("_ZWrite", 0);
                break;
            case 1f:
                _Target.SetFloat("_ZWrite", 1);
                break;
        }

        _Editor.ShaderProperty(doubleSided, "Double Sided");
        _Editor.ShaderProperty(zWrite,  MakeLabel("ZWrite", "Turn on or off ZWrite. \nThis will cause Z-Ordering issue, but will fix the depth rendering bug on some unity versions."));
        _Editor.ShaderProperty(worldUV, MakeLabel("WorldSpace UV", "Use world position to generate UV. Useful to get seamless water tiles"));
        _Editor.ShaderProperty(ortho, "Orthographic");

        EditorGUILayout.LabelField("Vertex Color Effects", _LabelStyle);
        EditorGUI.indentLevel = 3;
        _Editor.ShaderProperty(addFoam, MakeLabel("R: Add Foam", "Add foam where vertex color RED channel is higher than 0."));
        _Editor.ShaderProperty(dispMask, MakeLabel("B: Displacement Mask", "Multiply the Displacement with BLUE vertex color channel."));
        _Editor.ShaderProperty(alphaMask, MakeLabel("A: Alpha Mask", "Multiply the water Alpha with ALPHA vertex color channel."));

        // Render Queue
        EditorGUI.indentLevel = 2;
        _Target.renderQueue = EditorGUILayout.IntField("Render Queue", _Target.renderQueue);

        EditorGUI.indentLevel = 0;
        GUILayout.EndVertical();
    }

    void DrawVector2FromVector4(MaterialProperty prop, string newName, bool ZW = false)
    {
        var vectorValues = prop.vectorValue;

        var vectorA = ZW ? new Vector2(vectorValues.z, vectorValues.w) : new Vector2(vectorValues.x, vectorValues.y);

        var a = EditorGUILayout.Vector2Field(newName, vectorA);

        prop.vectorValue = ZW ? new Vector4(vectorValues.x, vectorValues.y, a.x, a.y) : new Vector4(a.x, a.y, vectorValues.z, vectorValues.w);
    }

    void DrawDoubleVector2FromVector4(MaterialProperty prop, string newNameA, string newNameB)
    {
        var vectorValues = prop.vectorValue;
        var vectorA = new Vector2(vectorValues.x, vectorValues.y);
        var vectorB = new Vector2(vectorValues.z, vectorValues.w);

        var a = EditorGUILayout.Vector2Field(newNameA, vectorA);
        var b = EditorGUILayout.Vector2Field(newNameB, vectorB);

        prop.vectorValue = new Vector4(a.x, a.y, b.x, b.y);
    }

    string GetRefractionTexturePath(MaterialProperty prop)
    {
        var rTexture = prop.textureValue;

        if (rTexture == null)
        {
            var materialPath = AssetDatabase.GetAssetPath(_Target);
            var materialName = Path.GetFileName(materialPath);
            var materialDir = materialPath.Replace(materialName, "");
            var textureName = materialName.Replace(".mat", "") + ".png";
            return materialDir + TEXTURE_PREFIX + textureName;
        }
        else
        {
            return AssetDatabase.GetAssetPath(rTexture);
        }
    }

    static Texture2D GenerateGradientTexture(Gradient gradient, int width, int height)
    {
        Texture2D newTex = new Texture2D(width, height, TextureFormat.RGB24, false);
        newTex.wrapMode = TextureWrapMode.Clamp;
        newTex.hideFlags = HideFlags.HideAndDontSave;

        if (gradient != null)
        {
            for (int w = 0; w < width; ++w)
            {
                var current = gradient.Evaluate(w / (float)width);

                for (int h = 0; h < height; ++h)
                {
                    newTex.SetPixel(w, h, current);
                }
            }
        }

        newTex.Apply();

        return newTex;
    }

    Gradient DecodeGradientFromMaterial()
    {
        var disabledKey = new Vector4(-1, -1, -1, -1);
        _GradientColorKeys.Clear();

        for (var i = 0; i < 8; i++)
        {

            var gradientProp = FindProperty("_RefractionKey" + (i + 1));
            var gradientKey = gradientProp.vectorValue;

            if (gradientKey == disabledKey)
            {
                continue;
            }


            var keyColor = new Color(gradientKey.x, gradientKey.y, gradientKey.z);
            var keyTime = gradientKey.w;
            var key = new GradientColorKey(keyColor, keyTime);
            _GradientColorKeys.Add(key);
        }


        var alphaKeys = new GradientAlphaKey[1];
        alphaKeys[0] = new GradientAlphaKey(1, 0);

        var gradient = new Gradient();
        gradient.SetKeys(_GradientColorKeys.ToArray(), alphaKeys);

        return gradient;
    }

    void EncodeGradientToMaterial(Gradient gradient)
    {
        for (var i = 0; i < 8; i++)
        {

            var gradientProp = FindProperty("_RefractionKey" + (i + 1));

            if (i >= gradient.colorKeys.Length)
            {
                gradientProp.vectorValue = new Vector4(-1, -1, -1, -1);
                continue;
            }

            var color = gradient.colorKeys[i].color;
            var time = gradient.colorKeys[i].time;
            var newData = new Vector4(color.r, color.g, color.b, time);

            gradientProp.vectorValue = newData;
        }
    }

    static Texture2D SaveTextureAsPNG(Texture2D texture, string fullPath)
    {
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);

        AssetDatabase.Refresh();

        return (Texture2D)AssetDatabase.LoadAssetAtPath(fullPath, typeof(Texture2D));
    }

    void ApplyImportSettings(string assetPath)
    {
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;

        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }

    static GUIContent staticLabel = new GUIContent();

    static GUIContent MakeLabel(string text, string tooltip = null)
    {
        staticLabel.text = text;
        staticLabel.tooltip = tooltip;
        return staticLabel;
    }

    MaterialProperty FindProperty(string name)
    {
        return FindProperty(name, _Properties);
    }
}
