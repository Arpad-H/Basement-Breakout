using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Fluxy
{

    public class FluxyFlipbookRecorderWindow : EditorWindow
    {
        string flipbookName = null;
        float duration = 4;
        int frameCount = 16;
        float loopCrossFade = 0.5f;
        int columns = 4;
        Vector2 scale = Vector2.one;

        bool recording = false;
        bool waitingForPlayMode = false;

        FluxyContainer selectedContainer = null;
        FluxyContainer recordingContainer = null;
        Flipbook currentFlipbook = null;

        Flipbook.FlipbookMetadata previewInfo = new Flipbook.FlipbookMetadata();
        List<Flipbook> takes = new List<Flipbook>();
        int framePadding = 0;
        int frameCounter = 0;

        string outputFolder;
        Vector2 scrollPos;

        Material blitMaterial;
        Material previewMaterial;
        Texture2D recordIcon;
        Texture2D stopIcon;
        Texture2D playIcon;
        Texture2D saveMaterialIcon;
        Texture2D saveTexturesIcon;
        Texture2D discardIcon;

        [MenuItem("Window/FluXY/FlipbookRecorder")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(FluxyFlipbookRecorderWindow), false, "Fluxy Flipbook Recorder");
        }

        private void OnEnable()
        {
            blitMaterial = Resources.Load<Material>("Materials/FlipbookBlit");
            previewMaterial = Resources.Load<Material>("Materials/FlipbookPreview");
            recordIcon = Resources.Load<Texture2D>("Icons/Fluxy Record Icon");
            stopIcon = Resources.Load<Texture2D>("Icons/Fluxy Stop Icon");
            playIcon = Resources.Load<Texture2D>("Icons/Fluxy PlayRecord Icon");
            saveMaterialIcon = Resources.Load<Texture2D>("Icons/Fluxy SaveMaterial Icon");
            saveTexturesIcon = Resources.Load<Texture2D>("Icons/Fluxy SaveTextures Icon");
            discardIcon = Resources.Load<Texture2D>("Icons/Fluxy Delete Icon");
            outputFolder = Application.dataPath;

            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged; 
        }

        private void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
        {
            if (waitingForPlayMode && obj == PlayModeStateChange.EnteredPlayMode)
            {
                waitingForPlayMode = false;
                StartRecording(selectedContainer);
            }
            else if (obj == PlayModeStateChange.ExitingPlayMode)
            {
                StopRecording();
            }
        }

        private void OnDestroy()
        {
            EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
            DiscardAllTakes();
        }

        void OnSelectionChange()
        {
            Repaint();
        }

        public void Update()
        {
            foreach (var flipbook in takes)
                if (flipbook.preview)
                {
                    Repaint();
                    return;
                }
        }

        void OnGUI()
        {
            if (Selection.activeGameObject != null)
                selectedContainer = Selection.activeGameObject.GetComponent<FluxyContainer>();
            else
                selectedContainer = null;

            EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical(GUILayout.Width(192));

                    GUI.enabled = selectedContainer != null;
                    DrawToolBar();
                    GUI.enabled = true;

                    previewInfo.CalculateSize(selectedContainer, duration, frameCount, columns, scale.x, scale.y);
                    DrawFlipbookInfo(previewInfo);

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();

                    var wideMode = EditorGUIUtility.wideMode;
                    EditorGUIUtility.wideMode = true;
                    EditorGUILayout.LabelField("Generation", EditorStyles.boldLabel);

                    // Initialize flipbook name if it's null:
                    if (flipbookName == null && selectedContainer != null)
                        flipbookName = selectedContainer.name;

                    flipbookName = EditorGUILayout.TextField("Name", flipbookName);
                    duration = Mathf.Max(0, EditorGUILayout.FloatField("Duration (seconds)", duration));
                    frameCount = Mathf.Max(1, EditorGUILayout.IntField("Frame count", frameCount));
                    loopCrossFade = EditorGUILayout.Slider("Loop crossfade", loopCrossFade, 0, 1);

                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("Layout", EditorStyles.boldLabel);
                    columns = Mathf.Max(1, EditorGUILayout.IntField("Columns", columns));
                    scale = Vector2.Max(new Vector2(0.1f,0.1f), EditorGUILayout.Vector2Field("Scale", scale));
                    EditorGUIUtility.wideMode = wideMode;

                EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            FluxyEditorUtils.DrawHorizontalGUILine();

            if (selectedContainer == null)
            {
                DrawEmptyState();
            }
            else
            {
                DrawCurrentPreview();

                DrawTakeList();
            }


        }

        private void DrawEmptyState()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("Select a FluxyContainer in the scene to begin recording flipbooks.", EditorStyles.centeredGreyMiniLabel);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        private void DrawToolBar()
        {
            EditorGUILayout.BeginHorizontal();

            if (!recording)
            {
                if (EditorApplication.isPlaying)
                {
                    if (GUILayout.Button(new GUIContent(" Record take ", recordIcon), GUILayout.Height(64)))
                        StartRecording(selectedContainer);
                }
                else 
                {
                    if (GUILayout.Button(new GUIContent(" Play and record", playIcon), GUILayout.Height(64)))
                    {
                        waitingForPlayMode = true;
                        EditorApplication.isPlaying = true;
                    }
                }
            }
            else
            {
                if (GUILayout.Button(new GUIContent(" Stop recording ", stopIcon), GUILayout.Height(64)))
                    StopRecording();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DiscardAllTakes()
        {
            foreach (var flipbook in takes)
                flipbook.Dispose();
            takes.Clear();
        }

        private void DrawTakeList()
        {
            // list of flipbooks:
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            for (int i = takes.Count - 1; i >= 0; --i)
                if (DrawFlipbookTake(i,takes[i]))
                {
                    if (takes[i] != null)
                        takes[i].Dispose();
                    takes.RemoveAt(i);
                }

            EditorGUILayout.EndScrollView();
        }

        private void DrawCurrentPreview()
        {
            if (currentFlipbook == null)
                return;

            GUILayout.BeginHorizontal();

                GUILayout.BeginVertical(GUILayout.Width(192));

                    EditorGUILayout.LabelField("Recording Take #" + (takes.Count + 1) + "...", EditorStyles.boldLabel, GUILayout.MaxWidth(192));

                    DrawFlipbookInfo(currentFlipbook.metadata);

                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                    currentFlipbook.RenderGUIPreview(previewMaterial);
                GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            FluxyEditorUtils.DrawHorizontalGUILine();
        }

        private void DrawFlipbookInfo(Flipbook.FlipbookMetadata info)
        {
            if (EditorApplication.isPlaying)
            {

                EditorGUILayout.LabelField(new GUIContent(
                "Size: " + info.width + " x " + info.height + "\n" +
                "Frame size: " + info.frameWidth + " x " + info.frameHeight + "\n" +
                "Frame count: " + info.frameCount)
                , EditorStyles.helpBox);
            }
            else
            {
                EditorGUILayout.LabelField(new GUIContent(
                "No output info available.")
                , EditorStyles.helpBox);
            }
        }

        private bool DrawFlipbookTake(int index, Flipbook flipbook)
        {
            bool discard = false;
            if (flipbook == null)
                return true;

            GUILayout.BeginHorizontal();

                GUILayout.BeginVertical(GUILayout.Width(192));

                    EditorGUILayout.LabelField("Take #" + (index + 1) + " (" + flipbook.name + ")", EditorStyles.boldLabel, GUILayout.MaxWidth(192));

                    if (GUILayout.Button(new GUIContent(" Save material", saveMaterialIcon)))
                    {
                        outputFolder = EditorUtility.SaveFolderPanel("Save flipbook material and textures", outputFolder, flipbook.name);
                        if (outputFolder.Length > 0)
                            flipbook.SaveAsAsset(outputFolder);
                        GUIUtility.ExitGUI();
                    }

                    if (GUILayout.Button(new GUIContent(" Save textures", saveTexturesIcon)))
                    {
                        outputFolder = EditorUtility.SaveFolderPanel("Save flipbook textures", outputFolder, flipbook.name);
                        if (outputFolder.Length > 0)
                            flipbook.SaveAsAsset(outputFolder, false);
                        GUIUtility.ExitGUI();
                    }

                    if (GUILayout.Button(new GUIContent(" Discard", discardIcon)))
                        discard = true;

                    flipbook.preview = GUILayout.Toggle(flipbook.preview, "Preview");
                    GUI.enabled = flipbook.preview;
                    flipbook.interpolatePreview = GUILayout.Toggle(flipbook.interpolatePreview, "Interpolate");
                    GUI.enabled = flipbook.interpolatePreview;
                    flipbook.previewInterpolation = EditorGUILayout.Slider(flipbook.previewInterpolation,0,1);
                    GUI.enabled = true;

                    DrawFlipbookInfo(flipbook.metadata);

                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                flipbook.RenderGUIPreview(previewMaterial);
                GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            FluxyEditorUtils.DrawHorizontalGUILine();
            return discard;
        }

        private void StartRecording(FluxyContainer container)
        {
            if (!recording && container != null)
            {
                recording = true;
                Time.captureFramerate = 60;

                // frames to wait between page captures:
                framePadding = (int)(duration * 60f / frameCount);

                recordingContainer = container;
                currentFlipbook = new Flipbook(recordingContainer, flipbookName, duration, frameCount, columns, scale.x, scale.y);

                recordingContainer.OnFrameEnded += Container_OnFrameEnded;
            }
        }

        private void StopRecording()
        {
            if (recording && recordingContainer != null)
            {
                recording = false;
                Time.captureFramerate = 0;

                takes.Add(currentFlipbook);
                currentFlipbook = null;

                recordingContainer.OnFrameEnded -= Container_OnFrameEnded;
                recordingContainer = null;
            }
        }

        private void Container_OnFrameEnded(FluxyContainer container)
        {
            if (recording)
            {
                if (EditorApplication.isPlaying && !EditorApplication.isPaused && currentFlipbook != null)
                {
                    frameCounter++;
                    if (frameCounter >= framePadding)
                    {
                        frameCounter = 0;

                        if (!currentFlipbook.AppendFrame(blitMaterial, loopCrossFade))
                            StopRecording();

                        Repaint();
                    }
                }
            }
        }
    }
}
