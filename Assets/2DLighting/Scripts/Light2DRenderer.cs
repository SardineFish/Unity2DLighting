using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Lighting2D
{
    [ExecuteInEditMode]
    [ImageEffectAllowedInSceneView]
    [RequireComponent(typeof(Camera))]
    public class Light2DRenderer : MonoBehaviour
    {
        private void OnPreRender()
        {
            var camera = GetComponent<Camera>();
            var profile = LightSystem.Instance.SetupCamera(camera);
            LightSystem.Instance.RenderDeffer(profile);
        }

    }

}