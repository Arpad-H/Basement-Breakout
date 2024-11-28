using UnityEditor;
using UnityEngine;

namespace Fluxy
{

    [CustomEditor(typeof(FluxyContainer), true), CanEditMultipleObjects]
    public class FluxyContainerEditor : Editor
    {

        [MenuItem("GameObject/3D Object/FluXY/Container", false, 200)]
        static void CreateFluxyContainer(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Container", typeof(FluxyContainer), typeof(FluxyTargetDetector));
            go.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Container");

            FluxyEditorUtils.CreateObject(go, menuCommand.context as GameObject);
            go.GetComponent<FluxyContainer>().solver = FluxyEditorUtils.GetOrCreateSolverObject();

            Selection.activeObject = go;
        }

        SerializedProperty m_Solver;
        SerializedProperty containerShape;
        SerializedProperty customMesh;
        SerializedProperty size;
        SerializedProperty lightSource;

        SerializedProperty subdivisions;
        SerializedProperty targetList;
        SerializedProperty clearTexture;
        SerializedProperty clearColor;
        SerializedProperty surfaceNormals;
        SerializedProperty normalTiling;
        SerializedProperty normalScale;

        SerializedProperty lookAtMode;
        SerializedProperty lookAt;
        SerializedProperty projectFrom;

        SerializedProperty boundaries;
        SerializedProperty edgeFalloff;
        SerializedProperty velocityScale;
        SerializedProperty accelerationScale;
        SerializedProperty pressure;
        SerializedProperty buoyancy;
        SerializedProperty viscosity;
        SerializedProperty adhesion;
        SerializedProperty surfaceTension;
        SerializedProperty turbulence;
        SerializedProperty dissipation;
        SerializedProperty gravity;
        SerializedProperty externalForce;
        SerializedProperty positionOffset;

        BooleanPreference shapeFoldout;
        BooleanPreference projectionFoldout;
        BooleanPreference surfaceFoldout;
        BooleanPreference boundariesFoldout;
        BooleanPreference worldFoldout;
        BooleanPreference fluidFoldout;

        public void OnEnable()
        {
            m_Solver = serializedObject.FindProperty("m_Solver");
            containerShape = serializedObject.FindProperty("containerShape");
            customMesh = serializedObject.FindProperty("customMesh");
            subdivisions = serializedObject.FindProperty("subdivisions");
            size = serializedObject.FindProperty("size");

            targetList = serializedObject.FindProperty("targets");
            clearTexture = serializedObject.FindProperty("clearTexture");
            clearColor = serializedObject.FindProperty("clearColor");
            surfaceNormals = serializedObject.FindProperty("surfaceNormals");
            normalTiling = serializedObject.FindProperty("normalTiling");
            normalScale = serializedObject.FindProperty("normalScale");

            lookAtMode = serializedObject.FindProperty("lookAtMode");
            lookAt = serializedObject.FindProperty("lookAt");
            projectFrom = serializedObject.FindProperty("projectFrom");

            boundaries = serializedObject.FindProperty("boundaries");
            velocityScale = serializedObject.FindProperty("velocityScale");
            accelerationScale = serializedObject.FindProperty("accelerationScale");

            buoyancy = serializedObject.FindProperty("buoyancy");
            edgeFalloff = serializedObject.FindProperty("edgeFalloff");
            pressure = serializedObject.FindProperty("pressure");
            viscosity = serializedObject.FindProperty("viscosity");
            adhesion = serializedObject.FindProperty("adhesion");
            surfaceTension = serializedObject.FindProperty("surfaceTension");
            turbulence = serializedObject.FindProperty("turbulence");
            dissipation = serializedObject.FindProperty("dissipation");
            gravity = serializedObject.FindProperty("gravity");
            lightSource = serializedObject.FindProperty("lightSource");
            externalForce = serializedObject.FindProperty("externalForce");
            positionOffset = serializedObject.FindProperty("positionOffset");

            shapeFoldout = new BooleanPreference($"{target.GetType()}.shapeFoldout", true);
            projectionFoldout = new BooleanPreference($"{target.GetType()}.projectionFoldout", true);
            surfaceFoldout = new BooleanPreference($"{target.GetType()}.surfaceFoldout", false);
            boundariesFoldout = new BooleanPreference($"{target.GetType()}.boundariesFoldout", false);
            worldFoldout = new BooleanPreference($"{target.GetType()}.worldFoldout", true);
            fluidFoldout = new BooleanPreference($"{target.GetType()}.fluidFoldout", true);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            var solver = m_Solver.objectReferenceValue as FluxySolver;
            if (solver != null && solver.IsFull())
                EditorGUILayout.HelpBox("This solver has reached its maximum capacity of 16 containers. Any extra containers will be ignored.", MessageType.Warning);

            if (Application.isPlaying)
                GUI.enabled = false;

            EditorGUILayout.PropertyField(m_Solver);
            GUI.enabled = true;

            shapeFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(shapeFoldout, "Shape");
            if (shapeFoldout)
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(containerShape);

                    if (containerShape.enumValueIndex == 2)
                        EditorGUILayout.PropertyField(customMesh);
                    if (containerShape.enumValueIndex == 0)
                        EditorGUILayout.PropertyField(subdivisions);

                    EditorGUILayout.PropertyField(size);
                    EditorGUI.indentLevel--;

                    if (check.changed)
                    {
                        serializedObject.ApplyModifiedProperties();
                        foreach (FluxyContainer t in targets)
                            t.UpdateContainerShape();
                    }
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            projectionFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(projectionFoldout, "Projection");
            if (projectionFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(lookAtMode);
                EditorGUILayout.PropertyField(lookAt);
                EditorGUILayout.PropertyField(projectFrom);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            surfaceFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(surfaceFoldout, "Surface");
            if (surfaceFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(clearTexture);
                EditorGUILayout.PropertyField(clearColor);
                EditorGUILayout.PropertyField(surfaceNormals);
                EditorGUILayout.PropertyField(normalTiling);
                EditorGUILayout.PropertyField(normalScale);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            boundariesFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(boundariesFoldout, "Boundaries");
            if (boundariesFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(edgeFalloff);
                EditorGUILayout.PropertyField(boundaries);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            worldFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(worldFoldout, "World");
            if (worldFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(velocityScale);
                EditorGUILayout.PropertyField(accelerationScale);
                EditorGUILayout.PropertyField(positionOffset);
                EditorGUILayout.PropertyField(gravity);
                EditorGUILayout.PropertyField(externalForce);
                EditorGUILayout.PropertyField(lightSource);
                EditorGUILayout.PropertyField(targetList);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            fluidFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(fluidFoldout, "Fluid");
            if (fluidFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(pressure);
                EditorGUILayout.PropertyField(viscosity);
                EditorGUILayout.PropertyField(adhesion);
                EditorGUILayout.PropertyField(surfaceTension);
                EditorGUILayout.PropertyField(turbulence);
                EditorGUILayout.PropertyField(buoyancy);
                EditorGUILayout.PropertyField(dissipation);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();

        }

    }

}

