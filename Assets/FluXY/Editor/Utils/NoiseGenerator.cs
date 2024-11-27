using System;
using UnityEngine;
using UnityEditor;

namespace Fluxy
{
    public class NoiseGenerator
    {
        [MenuItem("FluXYNoise/3DTexture")]
        static void CreateTexture3D()
        {
            // Configure the texture
            int size = 128;
            TextureFormat format = TextureFormat.R8;
            TextureWrapMode wrapMode = TextureWrapMode.Repeat;

            // Create the texture and apply the configuration
            Texture3D texture = new Texture3D(size, size, size, format, false);
            texture.wrapMode = wrapMode;

            // Create a 3-dimensional array to store color data
            Color[] colors = new Color[size * size * size];

            // Populate the array so that the x, y, and z values of the texture will map to red, blue, and green colors
            for (int z = 0; z < size; z++)
            {
                int zOffset = z * size * size;
                for (int y = 0; y < size; y++)
                {
                    int yOffset = y * size;
                    for (int x = 0; x < size; x++)
                    {
                        /*colors[x + yOffset + zOffset] = new Color(x * inverseResolution,
                            y * inverseResolution, z * inverseResolution, 1.0f);*/
                        var pos = new Vector3((x / (float)size), (y / (float)size), (z / (float)size));
                        float n = fBM(pos, 10.0f, 4);
                        colors[x + yOffset + zOffset] = new Color(n, n, n, 1);
                    }
                }
            }

            // Copy the color values to the texture
            texture.SetPixels(colors);

            // Apply the changes to the texture and upload the updated texture to the GPU
            texture.Apply();

            // Save the texture to your Unity Project
            AssetDatabase.CreateAsset(texture, "Assets/Example3DTexture.asset");
        }

        static float Fract(float f)
        {
            return f - (float)Math.Truncate(f);
        }

        static Vector3 Fract(Vector3 v)
        {
            return new Vector3(Fract(v.x), Fract(v.y), Fract(v.z));
        }

        static Vector3 hash33(Vector3 p3)
        {
            Vector3 p = Fract(Vector3.Scale(p3 , new Vector3(.1031f, .11369f, .13787f)));
            p += Vector3.one * Vector3.Dot(p, new Vector3(p.y,p.x,p.z) + Vector3.one * 19.19f);
            return -Vector3.one + 2.0f * Fract(new Vector3((p.x + p.y) * p.z, (p.x + p.z) * p.y, (p.y + p.z) * p.x));
        }

        // stacks up multiple octaves of worley noise:
        public static float fBM(in Vector3 x, float scale, int octaves)
        {
            octaves = Mathf.Max(1,octaves);
            float val = 0;
            float amplitude = 0.5f;
            float norm = 0;

            for (int i = 0; i < octaves; ++i)
            {
                val += worley(x, scale) * amplitude;
                norm += amplitude;
                scale *= 2.0f;
                amplitude *= 0.5f;
            }
            return val / norm;
        }

        // returns 3D tileable worley noise
        public static float worley(in Vector3 p, float scale)
        {
            // grid
            Vector3 id = new Vector3Int(Mathf.FloorToInt(p.x * scale), Mathf.FloorToInt(p.y * scale), Mathf.FloorToInt(p.z * scale));
            Vector3 fd = Fract(p * scale);

            float minimalDist = 1f;

            for (float x = -1f; x <= 1f; x++)
            {
                for (float y = -1f; y <= 1f; y++)
                {
                    for (float z = -1f; z <= 1f; z++)
                    {
                        Vector3 coord = new Vector3(x, y, z);
                        Vector3 cell = id + coord;
                        Vector3 rId = hash33(new Vector3(Mathf.Repeat(cell.x, scale),
                                                         Mathf.Repeat(cell.y, scale),
                                                         Mathf.Repeat(cell.z, scale))) * 0.5f + Vector3.one * 0.5f;

                        Vector3 r = coord + rId - fd;

                        float d = Vector3.Dot(r, r);

                        if (d < minimalDist)
                        {
                            minimalDist = d;
                        }
                    }
                }
            }

            return 1.0f - minimalDist;
        }
    }

}