using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Fluxy
{
    public class FluxyEditorUtils
    {
        public static void DrawHorizontalGUILine(int height = 1)
        {
            EditorGUILayout.Separator();

            Rect rect = GUILayoutUtility.GetRect(10, height, GUILayout.ExpandWidth(true));
            rect.height = height;
            rect.xMin = 0;
            rect.xMax = EditorGUIUtility.currentViewWidth;

            Color lineColor = EditorGUIUtility.isProSkin ? new Color(0.10196f, 0.10196f, 0.10196f, 1) : new Color(0.5f, 0.5f, 0.5f, 1);
            EditorGUI.DrawRect(rect, lineColor);
            GUILayout.Space(4);
        }

        public static void CreateObject(GameObject go, GameObject parent)
        {
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

            if (parent != null)
            {
                GameObjectUtility.SetParentAndAlign(go, parent as GameObject);
                Undo.SetTransformParent(go.transform, parent.transform, "Parent " + go.name);
            }

            GameObjectUtility.EnsureUniqueNameForSibling(go);

            // We have to fix up the undo name since the name of the object was only known after reparenting it.
            Undo.SetCurrentGroupName("Create " + go.name);

        }

        // Helper function that returns a solver GameObject, preferably a parent of the selection:
        public static FluxySolver GetOrCreateSolverObject()
        {
            GameObject selectedGo = Selection.activeGameObject;

            // Try to find a gameobject that is the selected GO or one if its parents.
            FluxySolver solver = (selectedGo != null) ? selectedGo.GetComponentInParent<FluxySolver>() : null;
            if (IsValidSolver(solver))
                return solver;

            // No solver in selection or its parents? Then use any valid solver.
            // We have to find all loaded solvers, not just the ones in main scenes.
            FluxySolver[] solverArray = StageUtility.GetCurrentStageHandle().FindComponentsOfType<FluxySolver>();
            for (int i = 0; i < solverArray.Length; i++)
                if (IsValidSolver(solverArray[i]))
                    return solverArray[i];

            // No solver in the scene at all? Then create a new one.
            return CreateNewSolver();
        }

        public static FluxySolver CreateNewSolver()
        {
            // Root for the actors.
            var root = new GameObject("Fluxy Solver");
            FluxySolver solver = root.AddComponent<FluxySolver>();

            // Works for all stages.
            StageUtility.PlaceGameObjectInCurrentStage(root);
            Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

            return solver;
        }

        static bool IsValidSolver(FluxySolver solver)
        {
            if (solver == null || !solver.gameObject.activeInHierarchy)
                return false;

            if (EditorUtility.IsPersistent(solver) || (solver.hideFlags & HideFlags.HideInHierarchy) != 0)
                return false;

            if (StageUtility.GetStageHandle(solver.gameObject) != StageUtility.GetCurrentStageHandle())
                return false;

            return true;
        }
    }
}
