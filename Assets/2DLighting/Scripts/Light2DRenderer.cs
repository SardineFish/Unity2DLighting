using UnityEngine;
using System.Collections;
using System.Linq;

namespace Lighting2D
{
    [ExecuteInEditMode]
    [ImageEffectAllowedInSceneView]
    [RequireComponent(typeof(Camera))]
    public class Light2DRenderer : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnPreRender()
        {
            var camera = GetComponent<Camera>();
            var profile = LightSystem.Instance.SetupCamera(camera);
            LightSystem.Instance.RenderDeffer(profile);
        }
        /*
#if UNITY_EDITOR
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (UnityEditor.SceneView.GetAllSceneCameras().Any(camera => camera == Camera.current))
            {
                LightSystem.Instance.DeferredImageLighting(source, destination);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
#endif*/

    }

}