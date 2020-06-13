using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Lighting2D
{
    public struct LightRenderingData
    {
        public Camera camera;
        public RenderTargetIdentifier lightmap;
        public RenderTargetIdentifier shadowmap;
        public RenderTargetIdentifier cameraColorTarget;
        public LightSystemSettings settings;
    }
}
