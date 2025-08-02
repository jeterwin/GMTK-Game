using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Experimental.Rendering; // sometimes needed for RTHandle, but URP has it in newer versions

public class GrayscaleFadeFeature : ScriptableRendererFeature
{
    class GrayscaleFadePass : ScriptableRenderPass
    {
        static readonly int FadeAmountID = Shader.PropertyToID("_FadeAmount");
        private Material material;

        // Use RTHandle instead of RenderTargetHandle for modern URP
        private RTHandle temporaryColorTexture;

        private RTHandle source;

        public float fadeAmount = 0f;

        public GrayscaleFadePass(Material mat)
        {
            this.material = mat;
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public void Setup(RTHandle source)
        {
            this.source = source;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // Allocate temporary RT handle
            if (temporaryColorTexture == null || temporaryColorTexture.rt == null)
            {
                temporaryColorTexture = RTHandles.Alloc(
                    cameraTextureDescriptor.width,
                    cameraTextureDescriptor.height,
                    colorFormat: GraphicsFormatUtility.GetGraphicsFormat(cameraTextureDescriptor.colorFormat, false),
                    dimension: TextureDimension.Tex2D,
                    useDynamicScale: true,
                    name: "_TemporaryColorTexture"
                );
            }
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null)
            {
                Debug.LogWarning("Missing material for GrayscaleFadePass.");
                return;
            }

            RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            CommandBuffer cmd = CommandBufferPool.Get("GrayscaleFade");

            material.SetFloat(FadeAmountID, fadeAmount);

            // Blit from source to temporary RT using grayscale material
            cmd.Blit(source, temporaryColorTexture, material);

            // Blit back from temporary RT to source
            cmd.Blit(temporaryColorTexture, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            // Don't release here; RTHandles.Alloc manages lifecycle
        }
    }

    [System.Serializable]
    public class GrayscaleSettings
    {
        public Material grayscaleMaterial = null;
    }

    public GrayscaleSettings settings = new GrayscaleSettings();

    GrayscaleFadePass grayscalePass;

    public override void Create()
    {
        if (settings.grayscaleMaterial == null)
        {
            Debug.LogError("GrayscaleFadeFeature missing material!");
            return;
        }
        grayscalePass = new GrayscaleFadePass(settings.grayscaleMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (grayscalePass == null)
            return;

        // Modern URP renderer exposes cameraColorTargetHandle (RTHandle)

        renderer.EnqueuePass(grayscalePass);
    }

    // Call externally to update fade amount dynamically
    public void SetFadeAmount(float amount)
    {
        if (grayscalePass != null)
            grayscalePass.fadeAmount = Mathf.Clamp01(amount);
    }
}
