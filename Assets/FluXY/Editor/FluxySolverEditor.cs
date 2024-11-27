using UnityEditor;
using UnityEngine;

namespace Fluxy
{

    [CustomEditor(typeof(FluxySolver), true), CanEditMultipleObjects]
    public class FluxySolverEditor : Editor
    {
        [MenuItem("GameObject/3D Object/FluXY/Solver", false, 200)]
        static void CreateFluxySolver(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Solver", typeof(FluxySolver));
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        SerializedProperty storage;
        SerializedProperty disposeWhenCulled;
        SerializedProperty readable;
        SerializedProperty desiredResolution;
        SerializedProperty densitySupersampling;

        SerializedProperty simulationMaterial;
        SerializedProperty maxTimestep;
        SerializedProperty maxSteps;
        SerializedProperty pressureSolver;
        SerializedProperty pressureIterations;

        private Material previewVelocityMaterial;
        private Material previewStateMaterial;
        private bool showPreview = false;

        public void OnEnable()
        {
            storage = serializedObject.FindProperty("storage");
            desiredResolution = serializedObject.FindProperty("desiredResolution");
            densitySupersampling = serializedObject.FindProperty("densitySupersampling");
            disposeWhenCulled = serializedObject.FindProperty("disposeWhenCulled");
            readable = serializedObject.FindProperty("readable");

            simulationMaterial = serializedObject.FindProperty("simulationMaterial");
            maxTimestep = serializedObject.FindProperty("maxTimestep");
            maxSteps = serializedObject.FindProperty("maxSteps");
            pressureSolver = serializedObject.FindProperty("pressureSolver");
            pressureIterations = serializedObject.FindProperty("pressureIterations");

            previewVelocityMaterial = Resources.Load<Material>("Materials/PreviewVelocity");
            previewStateMaterial = Resources.Load<Material>("Materials/PreviewState");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField(storage);
            EditorGUILayout.PropertyField(desiredResolution);
            EditorGUILayout.PropertyField(densitySupersampling);
            EditorGUILayout.PropertyField(disposeWhenCulled);
            EditorGUILayout.PropertyField(readable);

            EditorGUILayout.PropertyField(simulationMaterial);
            EditorGUILayout.PropertyField(maxTimestep);
            EditorGUILayout.PropertyField(maxSteps);
            EditorGUILayout.PropertyField(pressureSolver);

            if (pressureSolver.enumValueIndex == 1)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(pressureIterations);
                EditorGUI.indentLevel--;
            }

            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            showPreview = EditorGUILayout.Foldout(showPreview, "Buffer preview");
            if (showPreview)
            {
                var solver = target as FluxySolver;
                if (solver != null && solver.framebuffer != null)
                {
                    GUILayout.BeginHorizontal();
                    if (solver.framebuffer.stateA != null)
                    {
                        var space = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(256));
                        EditorGUI.DrawPreviewTexture(space, solver.framebuffer.stateA, previewStateMaterial, ScaleMode.ScaleToFit);
                    }
                    if (solver.framebuffer.velocityA != null)
                    {
                        var space = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(256));
                        EditorGUI.DrawPreviewTexture(space, solver.framebuffer.velocityA, previewVelocityMaterial, ScaleMode.ScaleToFit);
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.HelpBox("Solver not running, enter play mode to see the preview.", MessageType.Info);
                }
            }
        }

        public override bool RequiresConstantRepaint()
        {
            var solver = target as FluxySolver;
            return (showPreview && solver != null && solver.framebuffer != null &&
                    solver.framebuffer.velocityA != null && solver.framebuffer.stateA != null);
        }

    }

}

