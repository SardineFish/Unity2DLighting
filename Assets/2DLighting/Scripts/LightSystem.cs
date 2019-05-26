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
        public Material LightingMaterial;
        Dictionary<Camera, Light2DProfile> commandBuffers = new Dictionary<Camera, Light2DProfile>();
        public int BloomIteration = 2;

        // Use this for initialization
        void Start()
        {
            commandBuffers = new Dictionary<Camera, Light2DProfile>();
            if (!LightingMaterial)
                LightingMaterial = new Material(Shader.Find("Lighting2D/DeferredLighting"));
        }

        // Update is called once per frame
        void Update()
        {

        }

        public Light2DProfile SetupCamera(Camera camera)
        {
            if (!commandBuffers.ContainsKey(camera))
            {
                camera.RemoveAllCommandBuffers();
                var profile = new Light2DProfile()
                {
                    Camera = camera,
                    CommandBuffer = new CommandBuffer(),
                    LightMap = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 0, RenderTextureFormat.ARGBFloat),
                };
                profile.LightMap.antiAliasing = 1;
                commandBuffers[camera] = profile;
                profile.CommandBuffer.name = "2D Lighting";
                profile.LightMap.name = "Light Map";
                camera.AddCommandBuffer(CameraEvent.BeforeImageEffects, profile.CommandBuffer);
            }
            return commandBuffers[camera];
        }

        public void RenderDeffer(Light2DProfile profile)
        {
            
            var camera = profile.Camera;
            var cmd = profile.CommandBuffer;
            cmd.Clear();

            // !IMPORTANT! to prevent blit image upside down
            // STUPID UNITY
            var useMSAA = profile.Camera.allowMSAA && QualitySettings.antiAliasing > 0 ? 1 : 0;
            cmd.SetGlobalInt("_UseMSAA", useMSAA);
#if UNITY_EDITOR
            if (UnityEditor.SceneView.GetAllSceneCameras().Any(cmr => cmr == Camera.current))
            {
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

            cmd.GetTemporaryRT(shadowMap, -1, -1);

            cmd.SetRenderTarget(shadowMap);

            var lights = GameObject.FindObjectsOfType<Light2D>();
            foreach (var light in lights)
            {
                cmd.SetRenderTarget(shadowMap);
                cmd.ClearRenderTarget(true, true, Color.white);
                if(light.LightShadows != LightShadows.None)
                {
                    light.RenderShadow(cmd);
                }
                cmd.SetRenderTarget(lightMap);
                light.RenderLight(cmd);
            }

            cmd.SetGlobalTexture("_LightMap", lightMap);
            cmd.Blit(diffuse, BuiltinRenderTextureType.CameraTarget, LightingMaterial, 0);

            cmd.ReleaseTemporaryRT(shadowMap);
            cmd.ReleaseTemporaryRT(diffuse);
            cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget);
        }
    }

}