using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Lighting2D
{
    public class Light2DPass
    {
        int ShaderIDLightMap = Shader.PropertyToID("_2DLightMap");
        int ShaderIDShadowMap = Shader.PropertyToID("_2DShadowMap");
        public Light2DPass()
        {
        }
        public void Render(CommandBuffer cmd, ref LightRenderingData data)
        {
            var lightmap = ShaderIDLightMap;
            var shadowmap = ShaderIDShadowMap;
            var screenSize = new Vector2(data.camera.pixelWidth, data.camera.pixelHeight);
            cmd.GetTemporaryRT(
                lightmap,
                Mathf.FloorToInt(screenSize.x * data.settings.LightMapResolutionScale),
                Mathf.FloorToInt(screenSize.y * data.settings.LightMapResolutionScale),
                0,
                FilterMode.Bilinear,
                data.settings.LightMapFormat);
            cmd.GetTemporaryRT(
                shadowmap,
                Mathf.FloorToInt(screenSize.x * data.settings.ShadowMapResolutionScale),
                Mathf.FloorToInt(screenSize.y * data.settings.ShadowMapResolutionScale),
                0,
                FilterMode.Bilinear,
                data.settings.ShadowMapFormat);
            data.lightmap = lightmap;
            data.shadowmap = shadowmap;

            cmd.SetRenderTarget(lightmap);
            cmd.ClearRenderTarget(true, true, Color.black);

            bool shouldClearShadowMap = true;
            foreach (var light in Light2DBase.AssetsManager.Assets)
            {
                if (!light.enabled || !light.gameObject.activeInHierarchy)
                    continue;
                if (shouldClearShadowMap)
                {
                    cmd.SetRenderTarget(shadowmap);
                    cmd.ClearRenderTarget(true, true, Color.black);
                    shouldClearShadowMap = false;
                }
                if (light.LightShadows != LightShadows.None)
                {
                    light.RenderShadow(cmd, ref data);
                    shouldClearShadowMap = true;
                }
                light.RenderLight(cmd, ref data);
            }

            cmd.SetGlobalFloat("_ExposureLimit", data.settings.ExposureLimit);
            cmd.SetGlobalTexture("_LightMap", lightmap);
            cmd.SetGlobalColor("_GlobalLight", data.settings.GlobalLight);
            cmd.Blit(BuiltinRenderTextureType.None, data.cameraColorTarget, ShaderPool.Get("Lighting2D/DeferredLighting"), 0);

            cmd.ReleaseTemporaryRT(lightmap);
            cmd.ReleaseTemporaryRT(shadowmap);
        }
    }
}
