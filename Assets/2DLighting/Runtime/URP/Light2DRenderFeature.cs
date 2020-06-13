using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Lighting2D
{
    public class Light2DRenderFeature : UnityEngine.Rendering.Universal.ScriptableRendererFeature
    {
        [SerializeField]
        LightSystemSettings m_Settings;
        public LightSystemSettings Settings => m_Settings;

        RenderPass light2dRenderPass;
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var pass = new RenderPass(Settings, renderer.cameraColorTarget);
            pass.renderPassEvent = RenderPassEvent.AfterRendering;
            renderer.EnqueuePass(pass);
        }

        public override void Create()
        {

        }


        public class RenderPass : ScriptableRenderPass
        {
            RenderTargetIdentifier cameraColorTarget;
            LightSystemSettings settings;
            Light2DPass pass;
            public RenderPass(LightSystemSettings settings, RenderTargetIdentifier colorTarget)
            {
                this.settings = settings;
                pass = new Light2DPass();
                cameraColorTarget = colorTarget;
            }
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var cmd = CommandBufferPool.Get("Lighting 2D");

                var data = new LightRenderingData()
                {
                    camera = renderingData.cameraData.camera,
                    cameraColorTarget = cameraColorTarget,
                    settings = settings,
                };

                pass.Render(cmd, ref data);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }
    }

}
