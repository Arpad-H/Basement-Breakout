using UnityEditor;
using UnityEngine;

namespace Fluxy
{

    [CustomEditor(typeof(FluxyTarget), true), CanEditMultipleObjects]
    public class FluxyTargetEditor : Editor
    {
        [MenuItem("GameObject/3D Object/FluXY/Target", false, 200)]
        static void CreateFluxyTarget(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Target", typeof(FluxyTarget), typeof(SphereCollider));
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        SerializedProperty splatMaterial;
        SerializedProperty rateOverSteps;
        SerializedProperty rateOverTime;
        SerializedProperty rateOverDistance;

        SerializedProperty scaleWithDistance;
        SerializedProperty scaleWithTransform;
        SerializedProperty overridePosition;
        SerializedProperty overrideRotation;
        SerializedProperty position;
        SerializedProperty rotation;
        SerializedProperty scale;
        SerializedProperty positionRandomness;
        SerializedProperty scaleRandomness;
        SerializedProperty rotationRandomness;

        SerializedProperty velocityWeight;
        SerializedProperty velocityTexture;
        SerializedProperty maxRelativeVelocity;
        SerializedProperty maxRelativeAngularVelocity;
        SerializedProperty velocityScale;
        SerializedProperty angularVelocityScale;
        SerializedProperty force;
        SerializedProperty torque;

        SerializedProperty densityWeight;
        SerializedProperty densityTexture;
        SerializedProperty srcBlend;
        SerializedProperty dstBlend;
        SerializedProperty blendOp;
        SerializedProperty color;

        SerializedProperty noiseTexture;
        SerializedProperty densityNoise;
        SerializedProperty densityNoiseOffset;
        SerializedProperty densityNoiseTiling;
        SerializedProperty velocityNoise;
        SerializedProperty velocityNoiseOffset;
        SerializedProperty velocityNoiseTiling;

        BooleanPreference splatFoldout;
        BooleanPreference placementFoldout;
        BooleanPreference velocityFoldout;
        BooleanPreference densityFoldout;
        BooleanPreference noiseFoldout;

        public void OnEnable()
        {
            splatMaterial = serializedObject.FindProperty("splatMaterial");
            rateOverSteps = serializedObject.FindProperty("rateOverSteps");
            rateOverTime = serializedObject.FindProperty("rateOverTime");
            rateOverDistance = serializedObject.FindProperty("rateOverDistance");

            scaleWithDistance = serializedObject.FindProperty("scaleWithDistance");
            scaleWithTransform = serializedObject.FindProperty("scaleWithTransform");
            overridePosition = serializedObject.FindProperty("overridePosition");
            overrideRotation = serializedObject.FindProperty("overrideRotation");
            position = serializedObject.FindProperty("position");
            rotation = serializedObject.FindProperty("rotation");
            scale = serializedObject.FindProperty("scale");
            positionRandomness = serializedObject.FindProperty("positionRandomness");
            scaleRandomness = serializedObject.FindProperty("scaleRandomness");
            rotationRandomness = serializedObject.FindProperty("rotationRandomness");

            velocityWeight = serializedObject.FindProperty("velocityWeight");
            velocityTexture = serializedObject.FindProperty("velocityTexture");
            maxRelativeVelocity = serializedObject.FindProperty("maxRelativeVelocity");
            maxRelativeAngularVelocity = serializedObject.FindProperty("maxRelativeAngularVelocity");
            velocityScale = serializedObject.FindProperty("velocityScale");
            angularVelocityScale = serializedObject.FindProperty("angularVelocityScale");
            force = serializedObject.FindProperty("force");
            torque = serializedObject.FindProperty("torque");

            densityWeight = serializedObject.FindProperty("densityWeight");
            densityTexture = serializedObject.FindProperty("densityTexture");
            srcBlend = serializedObject.FindProperty("srcBlend");
            dstBlend = serializedObject.FindProperty("dstBlend");
            blendOp = serializedObject.FindProperty("blendOp");
            color = serializedObject.FindProperty("color");

            noiseTexture = serializedObject.FindProperty("noiseTexture");
            velocityNoise = serializedObject.FindProperty("velocityNoise");
            velocityNoiseOffset = serializedObject.FindProperty("velocityNoiseOffset");
            velocityNoiseTiling = serializedObject.FindProperty("velocityNoiseTiling");
            densityNoise = serializedObject.FindProperty("densityNoise");
            densityNoiseOffset = serializedObject.FindProperty("densityNoiseOffset");
            densityNoiseTiling = serializedObject.FindProperty("densityNoiseTiling");

            splatFoldout = new BooleanPreference($"{target.GetType()}.splatFoldout", true);
            placementFoldout = new BooleanPreference($"{target.GetType()}.placementFoldout", true);
            velocityFoldout = new BooleanPreference($"{target.GetType()}.velocityFoldout", false);
            densityFoldout = new BooleanPreference($"{target.GetType()}.densityFoldout", false);
            noiseFoldout = new BooleanPreference($"{target.GetType()}.noiseFoldout", false);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField(splatMaterial);

            splatFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(splatFoldout, "Timing");
            if (splatFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(rateOverSteps);
                EditorGUILayout.PropertyField(rateOverTime);
                EditorGUILayout.PropertyField(rateOverDistance);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            placementFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(placementFoldout, "Placement");
            if (placementFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(overridePosition);
                if (overridePosition.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(position);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(positionRandomness);

                EditorGUILayout.PropertyField(overrideRotation);
                if (overrideRotation.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(rotation);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(rotationRandomness);

                EditorGUILayout.PropertyField(scaleWithDistance);
                EditorGUILayout.PropertyField(scaleWithTransform);
                EditorGUILayout.PropertyField(scale);
                EditorGUILayout.PropertyField(scaleRandomness);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            velocityFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(velocityFoldout, "Velocity");
            if (velocityFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(velocityWeight);
                EditorGUILayout.PropertyField(velocityTexture);
                EditorGUILayout.PropertyField(maxRelativeVelocity);
                EditorGUILayout.PropertyField(maxRelativeAngularVelocity);
                EditorGUILayout.PropertyField(velocityScale);
                EditorGUILayout.PropertyField(angularVelocityScale);
                EditorGUILayout.PropertyField(force);
                EditorGUILayout.PropertyField(torque);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            densityFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(densityFoldout, "Density");
            if (densityFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(densityWeight);
                EditorGUILayout.PropertyField(densityTexture);
                EditorGUILayout.PropertyField(srcBlend);
                EditorGUILayout.PropertyField(dstBlend);
                EditorGUILayout.PropertyField(blendOp);
                EditorGUILayout.PropertyField(color);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            noiseFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(noiseFoldout, "Noise");
            if (noiseFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(noiseTexture);
                EditorGUILayout.PropertyField(velocityNoise);
                EditorGUILayout.PropertyField(velocityNoiseOffset);
                EditorGUILayout.PropertyField(velocityNoiseTiling);
                EditorGUILayout.PropertyField(densityNoise);
                EditorGUILayout.PropertyField(densityNoiseOffset);
                EditorGUILayout.PropertyField(densityNoiseTiling);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();

        }

    }

}

