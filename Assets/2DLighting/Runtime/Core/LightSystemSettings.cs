using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Lighting2D
{
    [Serializable]
    public class LightSystemSettings
    {
        [SerializeField]
        [ColorUsage(false, true)]
        Color m_GlobalLight = Color.black;
        [SerializeField]
        float m_ExposureLimit = -1;
        [SerializeField]
        float m_LightMapResolutionScale = 1;
        [SerializeField]
        RenderTextureFormat m_LightMapFormat = RenderTextureFormat.ARGB32;
        [SerializeField]
        float m_ShadowMapResolutionScale = 1;
        [SerializeField]
        RenderTextureFormat m_ShadowMapFormat = RenderTextureFormat.R8;
        
        public Color GlobalLight
        {
            get => m_GlobalLight;
            set => m_GlobalLight = value;
        }
        public float ExposureLimit
        {
            get => m_ExposureLimit;
            set => m_ExposureLimit = value;
        }
        public float LightMapResolutionScale
        {
            get => m_LightMapResolutionScale;
            set => m_LightMapResolutionScale = value;
        }
        public float ShadowMapResolutionScale
        {
            get => m_ShadowMapResolutionScale;
            set => m_ShadowMapResolutionScale = value;
        }

        public RenderTextureFormat LightMapFormat
        {
            get => m_LightMapFormat;
            set => m_LightMapFormat = value;
        }

        public RenderTextureFormat ShadowMapFormat
        {
            get => m_ShadowMapFormat;
            set => m_ShadowMapFormat = value;
        }

        Lazy<Material> lightMaterial = new Lazy<Material>(() => new Material(Shader.Find("Lighting2D/DeferredLighting")));
        public Material LightMaterial => lightMaterial.Value;

        Lazy<Material> shadowMaterial = new Lazy<Material>(() => new Material(Shader.Find("Lighting2D/Shadow")));
        public Material ShadowMaterial => shadowMaterial.Value;
        
    }
}
