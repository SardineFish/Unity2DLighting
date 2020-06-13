using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Lighting2D
{
    public struct Light2DProfile
    {
        public Camera Camera;
        public CommandBuffer CommandBuffer;
        public RenderTexture LightMap;
        public RenderTexture ScreenImage;
    }
}
