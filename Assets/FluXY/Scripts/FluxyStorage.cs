using System.Collections.Generic;
using UnityEngine;

namespace Fluxy
{

    [CreateAssetMenu(fileName = "FluxyStorage", menuName = "FluXY/FluxyStorage", order = 1)]
    public class FluxyStorage : ScriptableObject
    {
        public const int minFramebufferSize = 32;
        public const int bytesPerMbyte = 1048576;

        public class Framebuffer
        {
            public RenderTexture velocityA;    // r = velocityX, g = velocityY, b = density, a = temperature
            public RenderTexture velocityB;
            public RenderTexture stateA;       // r: divergence g: pressure b: curl a: unused (id?)
            public RenderTexture stateB;
            public RenderTexture tileID;       // r= tileID.
            public int desiredResolution = 256;
            public int stateSupersampling = 1;

            public Framebuffer(int desiredResolution, int stateSupersampling = 1)
            {
                this.desiredResolution = desiredResolution;
                this.stateSupersampling = Mathf.Max(1, stateSupersampling);
            }
        }

        public enum FluidTexturePrecision
        {
            Float,
            Half,
            Fixed
        }

        /// <summary>
        /// Memory budget, expressed in megabytes. The combined memory used by all solvers sharing this asset
        /// will not be larger than this value. Note that supersampling is not taken into account.
        /// </summary>
        [Tooltip("Memory budget, expressed in megabytes. The combined memory used by all solvers sharing this asset will not be larger than this value. Note that supersampling is not taken into account.")]
        public int memoryBudget = 32;

        /// <summary>
        /// Precision of the density textures.
        /// </summary>
        [Tooltip("Precision of the density textures.")]
        public FluidTexturePrecision densityPrecision = FluidTexturePrecision.Half;

        /// <summary>
        /// Precision of the velocity textures.
        /// </summary>
        [Tooltip("Precision of the velocity textures.")]
        public FluidTexturePrecision velocityPrecision = FluidTexturePrecision.Half;

        /// <summary>
        /// List of framebuffers being managed. Might contain null entries.
        /// </summary>
        public List<Framebuffer> framebuffers = new List<Framebuffer>();

        /// <summary>
        /// Requests a framebuffer of a specific resolution. A smaller framebuffer
        /// might be returned, depending on the amount of available memory.
        /// </summary>
        /// <param name="desiredResolution"></param>
        public int RequestFramebuffer(int desiredResolution, int stateSupersampling)
        {
            Framebuffer fb = new Framebuffer(desiredResolution, stateSupersampling);

            // find first empty slot:
            int id = 0;
            for (; id < framebuffers.Count; ++id)
                if (framebuffers[id] == null) break;

            if (id == framebuffers.Count)
                framebuffers.Add(fb);
            else
                framebuffers[id] = fb;

            ResizeStorage();

            return id;
        }

        /// <summary>
        /// Disposes of a framebuffer storage. Optionally reallocates all other
        /// framebuffers to take full advantage of the memory budget.
        /// </summary>
        /// <param name="framebufferID"></param>
        /// <param name="expand"></param>
        public void DisposeFramebuffer(int framebufferID, bool expand = true)
        {
            if (framebufferID >= 0 && framebufferID < framebuffers.Count)
            {
                var fb = framebuffers[framebufferID];
                if (fb != null)
                {
                    RenderTexture.ReleaseTemporary(fb.velocityA);
                    RenderTexture.ReleaseTemporary(fb.velocityB);
                    RenderTexture.ReleaseTemporary(fb.stateA);
                    RenderTexture.ReleaseTemporary(fb.stateB);
                    framebuffers[framebufferID] = null;

                    if (expand)
                        ResizeStorage();
                }
            }
        }

        private int PrevPowerTwo(int x)
        {
            if (x == 0)
                return 0;

            x |= (x >> 1);
            x |= (x >> 2);
            x |= (x >> 4);
            x |= (x >> 8);
            x |= (x >> 16);
            return x - (x >> 1);
        }

        /// <summary>
        /// Resizes all existing framebuffers to meet the memory budget.
        /// </summary>
        public void ResizeStorage()
        {
            // get bytes per pixel for both textures:
            int densityPixelSize = GetBytesPerPixel(densityPrecision);
            int velocityPixelSize = GetBytesPerPixel(velocityPrecision);

            // calculate maximum amount of pixels:
            float pixelBudget = (memoryBudget * bytesPerMbyte) / (float)(2 * (densityPixelSize + velocityPixelSize));

            // 1. sum all areas.
            float totalRes = 0;
            for (int i = 0; i < framebuffers.Count; ++i)
                if (framebuffers[i] != null)
                    totalRes += framebuffers[i].desiredResolution;

            // 2. divide each one by total sum, and reallocate it.
            for (int i = 0; i < framebuffers.Count; ++i)
            {
                if (framebuffers[i] != null)
                {
                    float weight = framebuffers[i].desiredResolution / totalRes;

                    // calculate new resolution from weight:
                    int maxResolution = Mathf.FloorToInt(Mathf.Sqrt(pixelBudget * weight));
                    int resolution = Mathf.Min(framebuffers[i].desiredResolution, maxResolution);

                    int quantizedRes = Mathf.Max(minFramebufferSize, PrevPowerTwo(resolution));
                    ReallocateFramebuffer(i, quantizedRes);
                }
            }
        }

        /// <summary>
        /// Returns a framebuffer given its ID.
        /// </summary>
        /// <param name="framebufferID"></param>
        /// <returns></returns>
        public Framebuffer GetFramebuffer(int framebufferID)
        {
            if (framebufferID >= 0 && framebufferID < framebuffers.Count)
                return framebuffers[framebufferID];
            return null;
        }

        /// <summary>
        /// Given a FluidTexturePrecision enum, returns a matching RenderTextureFormat. In case the appropiate texture format is not
        /// supported by the system, it will return the default format (usually ARGB32).
        /// </summary>
        /// <param name="precision"></param>
        /// <returns></returns>
        private RenderTextureFormat GetRenderTextureFormat(FluidTexturePrecision precision)
        {
            switch(precision)
            {
                case FluidTexturePrecision.Float:
                {
                    if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBFloat))
                        return RenderTextureFormat.ARGBFloat;
                    if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
                        return RenderTextureFormat.ARGBHalf;
                    return RenderTextureFormat.ARGB32;
                }

                case FluidTexturePrecision.Half:
                {
                    if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
                        return RenderTextureFormat.ARGBHalf;
                    return RenderTextureFormat.ARGB32;
                }
                case FluidTexturePrecision.Fixed:
                default:
                return RenderTextureFormat.ARGB32;
            }
        }

        private int GetBytesPerPixel(FluidTexturePrecision precision)
        {
            switch (precision)
            {
                case FluidTexturePrecision.Float:
                    {
                        if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBFloat))
                            return 16;
                        if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
                            return 8;
                        return 4;
                    }

                case FluidTexturePrecision.Half:
                    {
                        if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
                            return 8;
                        return 4;
                    }
                case FluidTexturePrecision.Fixed:
                default:
                    return 4;
            }
        }

        /// <summary>
        /// Allocates new RenderTextures for a framebuffer, copying the contents
        /// of the old textures (if any).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resolution"></param>
        private void ReallocateFramebuffer(int id, int resolution)
        {
            var fb = framebuffers[id];

            if (fb.stateA != null && fb.stateA.width == resolution * fb.stateSupersampling)
                return;

            var densityFormat = GetRenderTextureFormat(densityPrecision);
            var velocityFormat = GetRenderTextureFormat(velocityPrecision);

            // create new buffers:
            var velocityA = RenderTexture.GetTemporary(resolution, resolution, 0, velocityFormat, RenderTextureReadWrite.Linear);
            var velocityB = RenderTexture.GetTemporary(resolution, resolution, 0, velocityFormat, RenderTextureReadWrite.Linear);
            var stateA = RenderTexture.GetTemporary(resolution * fb.stateSupersampling, resolution * fb.stateSupersampling, 0, densityFormat, RenderTextureReadWrite.Linear);
            var stateB = RenderTexture.GetTemporary(resolution * fb.stateSupersampling, resolution * fb.stateSupersampling, 0, densityFormat, RenderTextureReadWrite.Linear);
            var tileID = RenderTexture.GetTemporary(resolution, resolution, 0, RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear);

            velocityA.filterMode = FilterMode.Point;
            velocityB.filterMode = FilterMode.Point;
            stateA.filterMode = FilterMode.Point;
            stateB.filterMode = FilterMode.Point;
            tileID.filterMode = FilterMode.Point;

            // blit old contents into new buffers (note: only main buffers)
            if (fb.velocityA != null)
            {
                Graphics.Blit(fb.velocityA, velocityA);
                Graphics.Blit(fb.stateA, stateA);
                Graphics.Blit(fb.tileID, tileID);
            }
            // or clear new buffers:
            else
            {
                var previousActive = RenderTexture.active;
                RenderTexture.active = velocityA;
                GL.Clear(false, true, Color.clear);
                RenderTexture.active = stateA;
                GL.Clear(false, true, Color.clear);
                RenderTexture.active = tileID;
                GL.Clear(false, true, Color.clear);
                RenderTexture.active = previousActive;
            }

            // replace old buffers with new ones:
            RenderTexture.ReleaseTemporary(fb.velocityA);
            RenderTexture.ReleaseTemporary(fb.velocityB);
            RenderTexture.ReleaseTemporary(fb.stateA);
            RenderTexture.ReleaseTemporary(fb.stateB);
            RenderTexture.ReleaseTemporary(fb.tileID);

            fb.velocityA = velocityA;
            fb.velocityB = velocityB;
            fb.stateA = stateA;
            fb.stateB = stateB;
            fb.tileID = tileID;
        }
    }
}
