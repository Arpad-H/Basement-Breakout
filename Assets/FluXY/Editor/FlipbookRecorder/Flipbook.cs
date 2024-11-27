using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Fluxy
{

    public class Flipbook
    {
        public struct FlipbookMetadata
        {
            public float duration;
            public int frameCount;
            public int columns;

            public int width;
            public int height;

            public int frameWidth;
            public int frameHeight;

            public int currentFrame;

            public bool CalculateSize(FluxyContainer container, float duration, int frameCount, int columns, float scaleX, float scaleY)
            {
                this.duration = duration;
                this.frameCount = frameCount;
                this.columns = columns;

                if(container != null && container.solver != null && frameCount > 0 && columns > 0)
                {
                    var fb = container.solver.framebuffer;
                    if (fb != null)
                    {
                        var uvRect = container.solver.GetContainerUVRect(container);
                        frameWidth = Mathf.FloorToInt(fb.stateA.width * uvRect.z * scaleX);
                        frameHeight = Mathf.FloorToInt(fb.stateA.height * uvRect.w * scaleY);

                        int rows = Mathf.CeilToInt(frameCount / (float)columns);

                        width = columns * frameWidth;
                        height = rows * frameHeight;

                        return true;
                    }
                }
                return false;
            }
        }

        private FluxyContainer container;
        public FlipbookMetadata metadata = new FlipbookMetadata();
        public RenderTexture stateBuffer;
        public RenderTexture velocityBuffer;

        public bool preview;
        public bool interpolatePreview;
        public float previewInterpolation = 1;
        public float previewPlaybackSpeed = 1;

        public string name { get; private set; }

        public Flipbook(FluxyContainer container, string name, float duration, int frameCount, int columns, float scaleX, float scaleY)
        {
            this.container = container;

            if (name == null || name == String.Empty)
                this.name = container.name;
            else
                this.name = name;

            if (metadata.CalculateSize(container, duration, frameCount, columns, scaleX, scaleY))
            {
                stateBuffer = new RenderTexture(metadata.width, metadata.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                velocityBuffer = new RenderTexture(metadata.width, metadata.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            }
        }

        private Vector4 GetFrameRect(int frame)
        {
            return new Vector4((frame % metadata.columns * metadata.frameWidth) / (float)stateBuffer.width,
                               (frame / metadata.columns * metadata.frameHeight) / (float)stateBuffer.height,
                                metadata.frameWidth / (float)stateBuffer.width,
                                metadata.frameHeight / (float)stateBuffer.height);
        }

        public bool AppendFrame(Material blitMaterial, float crossFadeAmount)
        {
            int crossFadeFrames = (int) (metadata.frameCount * Mathf.Clamp01(crossFadeAmount));

            // do not append more frames if the flipbook is finished.
            if (metadata.currentFrame >= metadata.frameCount + crossFadeFrames)
                return false;

            blitMaterial.SetInt("_TileIndex", container.solver.GetContainerID(container) + 1);

            //first pass, direct blit from solver buffers to flipbook:
            if (metadata.currentFrame < metadata.frameCount)
            {
                blitMaterial.SetVector("_FrameRect", GetFrameRect(metadata.currentFrame));
                Graphics.Blit(container.solver.framebuffer.stateA, stateBuffer, blitMaterial, 0);
                Graphics.Blit(container.solver.framebuffer.velocityA, velocityBuffer, blitMaterial, 1);
            }
            // second pass (crossfade):
            else
            {
                blitMaterial.SetVector("_FrameRect", GetFrameRect(metadata.currentFrame % metadata.frameCount));
                float crossfadeFactor = 1 - Mathf.Max(0, metadata.currentFrame - metadata.frameCount) / (float)crossFadeFrames;
                blitMaterial.SetFloat("_Opacity", crossfadeFactor);

                blitMaterial.SetTexture("_Flipbook",stateBuffer);
                var temp = RenderTexture.GetTemporary(stateBuffer.descriptor);
                Graphics.Blit(stateBuffer, temp);
                Graphics.Blit(container.solver.framebuffer.stateA, temp, blitMaterial, 2);
                Graphics.Blit(temp, stateBuffer);
                RenderTexture.ReleaseTemporary(temp);

                blitMaterial.SetTexture("_Flipbook", velocityBuffer);
                temp = RenderTexture.GetTemporary(velocityBuffer.descriptor);
                Graphics.Blit(velocityBuffer, temp);
                Graphics.Blit(container.solver.framebuffer.velocityA, temp, blitMaterial, 3);
                Graphics.Blit(temp, velocityBuffer);
                RenderTexture.ReleaseTemporary(temp);
            }

            metadata.currentFrame++;
            return true;
        }

        public void Dispose()
        {
            if (stateBuffer != null)
            {
                stateBuffer.Release();
                GameObject.DestroyImmediate(stateBuffer);
            }
            if (velocityBuffer != null)
            {
                velocityBuffer.Release();
                GameObject.DestroyImmediate(velocityBuffer);
            }
        }

        public void RenderGUIPreview(Material previewMaterial)
        {
            if (preview)
            {
                if (stateBuffer != null && velocityBuffer != null)
                {
                    if (interpolatePreview)
                        previewMaterial.EnableKeyword("INTERPOLATION");
                    else
                        previewMaterial.DisableKeyword("INTERPOLATION");

                    float aspect = metadata.frameWidth / (float)metadata.frameHeight;

                    previewMaterial.SetTexture("_Velocity", velocityBuffer);
                    previewMaterial.SetFloat("_PlaybackSpeed", previewPlaybackSpeed);
                    previewMaterial.SetFloat("_Interpolation", previewInterpolation);
                    previewMaterial.SetFloat("_Duration", metadata.duration);
                    previewMaterial.SetFloat("_AspectRatio", aspect);
                    previewMaterial.SetInteger("_FrameCount", metadata.frameCount);
                    previewMaterial.SetInteger("_Columns", metadata.columns);

                    previewPlaybackSpeed = EditorGUILayout.Slider("Playback speed", previewPlaybackSpeed, -4, 4);

                    var space = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(256));
                    EditorGUI.DrawPreviewTexture(space, stateBuffer, previewMaterial, ScaleMode.ScaleToFit, aspect);
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                float previewHeight = 256 + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                if (stateBuffer != null)
                {
                    var space = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(previewHeight));
                    EditorGUI.DrawPreviewTexture(space, stateBuffer, null, ScaleMode.ScaleToFit);
                }
                if (velocityBuffer != null)
                {
                    var space = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(previewHeight));
                    EditorGUI.DrawPreviewTexture(space, velocityBuffer, null, ScaleMode.ScaleToFit);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void SaveRTBufferAsAsset(RenderTexture rt, string dirPath, string fileName)
        {
            Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, true);
            var oldActive = RenderTexture.active;
            RenderTexture.active = rt;
            texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            texture.Apply();
            RenderTexture.active = oldActive;

            //then Save To Disk as PNG
            byte[] bytes = texture.EncodeToPNG();
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllBytes(dirPath + "/"+ fileName, bytes);
        }

        private void CreateFlipbookMaterial(string path, Texture2D stateTexture, Texture2D velocityTexture)
        {
            path += "/" + name + ".mat";

            // get the material if it exists, or create a new one:
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (!material)
            {
                material = new Material(Shader.Find("Fluxy/Rendering/Flipbooks/FlipbookPlaybackInterpolated"));
                AssetDatabase.CreateAsset(material, path);
            }

            material.SetFloat("_Duration", metadata.duration); 
            material.SetInteger("_FrameCount", metadata.frameCount);
            material.SetInteger("_Columns", metadata.columns);
            material.SetTexture("_MainTex", stateTexture);
            material.SetTexture("_Velocity", velocityTexture);
        }

        public void SaveAsAsset(string path, bool createMaterial = true)
        {
            SaveRTBufferAsAsset(stateBuffer, path, name + "_DEN.png");
            SaveRTBufferAsAsset(velocityBuffer, path, name + "_VEL.png");
            AssetDatabase.Refresh();

            string assetsFolder = "Assets/" + Path.GetRelativePath(Application.dataPath, path);
            string stateTexPath = assetsFolder + "/" + name + "_DEN.png";
            string velocityTexPath = assetsFolder + "/" + name + "_VEL.png";

            var importer = AssetImporter.GetAtPath(stateTexPath) as TextureImporter;
            if (importer != null)
            {
                importer.sRGBTexture = false;
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }

            importer = AssetImporter.GetAtPath(velocityTexPath) as TextureImporter;
            if (importer != null)
            {
                importer.sRGBTexture = false;
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }

            if (createMaterial)
            {
                var stateTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(stateTexPath);
                var velocityTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(velocityTexPath);
                CreateFlipbookMaterial(assetsFolder, stateTexture, velocityTexture);
            }

            AssetDatabase.SaveAssets();
        }
    }
}
