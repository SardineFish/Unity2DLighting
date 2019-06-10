using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Linq;

namespace Lighting2D
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    public class LightSystem : Singleton<LightSystem>
    {
        public bool PreviewInInspector = true;
        public float ExposureLimit = -1;
        public Material LightingMaterial;
        Dictionary<Camera, Light2DProfile> cameraProfiles = new Dictionary<Camera, Light2DProfile>();
        public int BloomIteration = 2;
        public int LightMapResolutionScale = 1;
        public int ShadowMapResolutionScale = 1;
        public FilterMode ShadowMapFilterMode = FilterMode.Bilinear;

        public Material gaussianMat;

        // Use this for initialization
        void Start()
        {
            cameraProfiles = new Dictionary<Camera, Light2DProfile>();
            if (!LightingMaterial)
                LightingMaterial = new Material(Shader.Find("Lighting2D/DeferredLighting"));
        }

        // Update is called once per frame
        void Update()
        {

        }

        [Lighting2D.Editor.EditorButton("Reset")]
        public void Reset()
        {
            cameraProfiles.Clear();
        }

        public Light2DProfile SetupCamera(Camera camera)
        {
            if (!cameraProfiles.ContainsKey(camera))
            {
                camera.RemoveAllCommandBuffers();
                var profile = new Light2DProfile()
                {
                    Camera = camera,
                    CommandBuffer = new CommandBuffer(),
                    LightMap = new RenderTexture(camera.pixelWidth / LightMapResolutionScale, camera.pixelHeight / LightMapResolutionScale, 0, RenderTextureFormat.ARGBFloat),
                };
                profile.LightMap.filterMode = FilterMode.Point;
                profile.LightMap.antiAliasing = 1;
                cameraProfiles[camera] = profile;
                profile.CommandBuffer.name = "2D Lighting";
                profile.LightMap.name = "Light Map";
                camera.AddCommandBuffer(CameraEvent.BeforeImageEffects, profile.CommandBuffer);
            }
            return cameraProfiles[camera];
        }

        public void RenderDeffer(Light2DProfile profile)
        {
            var camera = profile.Camera;
            var cmd = profile.CommandBuffer;
            cmd.Clear();

            cmd.BeginSample("2D Lighting");

            // !IMPORTANT! to prevent blit image upside down
            // STUPID UNITY
            var useMSAA = profile.Camera.allowMSAA && QualitySettings.antiAliasing > 0 ? 1 : 0;
            cmd.SetGlobalInt("_UseMSAA", useMSAA);
#if UNITY_EDITOR
            // In SceneView
            if (UnityEditor.SceneView.GetAllSceneCameras().Any(cmr => cmr == camera))
            {
                if (!PreviewInInspector)
                    return;
                cmd.SetGlobalInt("_SceneView", 1);
            }
            else
            {
                cmd.SetGlobalInt("_SceneView", 0);
            }
#endif

            var diffuse = Shader.PropertyToID("_Diffuse");
            cmd.GetTemporaryRT(diffuse, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 1);
            cmd.Blit(BuiltinRenderTextureType.CameraTarget, diffuse);

            var shadowMap = Shader.PropertyToID("_ShadowMap");
            var lightMap = profile.LightMap;
            cmd.SetRenderTarget(lightMap, lightMap);
            cmd.ClearRenderTarget(true, true, Color.black);

            cmd.GetTemporaryRT(shadowMap, camera.pixelWidth / ShadowMapResolutionScale, camera.pixelHeight / ShadowMapResolutionScale, 0, ShadowMapFilterMode);

            cmd.SetRenderTarget(shadowMap);
            cmd.ClearRenderTarget(true, true, Color.black);
            cmd.SetRenderTarget(lightMap);

            bool renderedShadow = false;
            var lights = GameObject.FindObjectsOfType<Light2D>();
            foreach (var light in lights)
            {
                if(renderedShadow)
                {
                    cmd.SetRenderTarget(shadowMap);
                    cmd.ClearRenderTarget(true, true, Color.black);
                }
                if (light.LightShadows != LightShadows.None)
                {
                    cmd.SetRenderTarget(shadowMap);
                    light.RenderShadow(cmd, shadowMap);
                    renderedShadow = true;
                    cmd.SetRenderTarget(lightMap);
                }
                else
                    renderedShadow = false;
                light.RenderLight(cmd);
            }

            cmd.SetGlobalFloat("_ExposureLimit", ExposureLimit);
            cmd.SetGlobalTexture("_LightMap", lightMap);
            cmd.Blit(diffuse, BuiltinRenderTextureType.CameraTarget, LightingMaterial, 0);

            cmd.ReleaseTemporaryRT(shadowMap);
            cmd.ReleaseTemporaryRT(diffuse);
            cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget);
            // GaussianBlur.Blur(256, cmd, BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget, gaussianMat);
            cmd.EndSample("2D Lighting");
        }
    }

}