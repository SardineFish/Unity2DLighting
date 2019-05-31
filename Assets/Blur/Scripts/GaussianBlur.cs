using UnityEngine;
using UnityEngine.Rendering;

public class GaussianBlur : MonoBehaviour
{
    public static void Blur(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dst, Material material)
    {
        var blurTexture = Shader.PropertyToID("__GaussianBlurTexture");
        cmd.GetTemporaryRT(blurTexture, -1, -1);
        cmd.SetGlobalVector("_BlurDirection", new Vector2(1, 0));
        cmd.Blit(src, blurTexture, material);
        cmd.SetGlobalVector("_BlurDirection", new Vector2(0, 1));
        cmd.Blit(blurTexture, dst, material);
        cmd.ReleaseTemporaryRT(blurTexture);
    }
}