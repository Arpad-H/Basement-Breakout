using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DistortionRenderFeature : ScriptableRendererFeature
{
    class DistortionRenderPass : ScriptableRenderPass
    {
        private Material material;
        private RenderTargetIdentifier source;
        private RenderTargetHandle tempTexture;
        private ProfilingSampler profilingSampler = new ProfilingSampler("DistortionPass");

        public DistortionRenderPass(Material mat)
        {
            material = mat;
            tempTexture.Init("_TempDistortionTexture");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            source = renderingData.cameraData.renderer.cameraColorTargetHandle;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("DistortionPass");
            using (new ProfilingScope(cmd, profilingSampler))
            {
                RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
                cmd.GetTemporaryRT(tempTexture.id, opaqueDesc, FilterMode.Bilinear);

                // Apply distortion effect
                Blit(cmd, source, tempTexture.Identifier(), material);

                // Blit back to screen
                Blit(cmd, tempTexture.Identifier(), source);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }

    DistortionRenderPass distortionPass;
    public Material distortionMaterial;

    public override void Create()
    {
        distortionPass = new DistortionRenderPass(distortionMaterial);
        distortionPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (distortionMaterial == null) return;
        renderer.EnqueuePass(distortionPass);
    }
}
