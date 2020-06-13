using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

namespace Lighting2D
{   
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteLight : Light2DBase
    {
        public float Intensity = 1;
        public override void RenderLight(CommandBuffer cmd, ref LightRenderingData data)
        {
            cmd.SetRenderTarget(data.lightmap);

            var renderer = GetComponent<SpriteRenderer>();
            renderer.material.color *= Intensity;
            cmd.DrawRenderer(renderer, renderer.material);
            renderer.material.color = renderer.color;
        }
    }

}