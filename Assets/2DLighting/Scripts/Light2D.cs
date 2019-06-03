using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

namespace Lighting2D
{
    public enum LightType
    {
        Analytical,
        Textured,
    }
    [ExecuteInEditMode]
    public class Light2D : Light2DBase
    {
        public LightType LightType = LightType.Analytical;
        [Range(-1, 1)]
        public float Attenuation = 0;
        public Color LightColor = Color.white;
        public float Intensity = 1;
        public Texture LightTexture;
        public Mesh Mesh;
        private Material LightMaterial;

        void Reset()
        {
            LightMaterial = new Material(Shader.Find("Lighting2D/2DLight"));
        }
        private void Awake()
        {
            Reset();
            var halfRange = LightDistance / 2;
            Mesh = new Mesh();
            Mesh.vertices = new Vector3[]
            {
                new Vector3(-halfRange, -halfRange, 0),
                new Vector3(halfRange, -halfRange, 0),
                new Vector3(-halfRange, halfRange, 0),
                new Vector3(halfRange, halfRange, 0),
            };
            Mesh.triangles = new int[]
            {
                0, 2, 1,
                2, 3, 1,
            };
            Mesh.RecalculateNormals();
            Mesh.uv = new Vector2[]
            {
                new Vector2 (0, 0),
                new Vector2 (1, 0),
                new Vector2 (0, 1),
                new Vector2 (1, 1),
            };
            Mesh.MarkDynamic();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateMesh();
        }

        public void UpdateMesh()
        {
            Mesh.vertices = new Vector3[]
            {
                new Vector3(-LightDistance, -LightDistance, 0),
                new Vector3(LightDistance, -LightDistance, 0),
                new Vector3(-LightDistance, LightDistance, 0),
                new Vector3(LightDistance, LightDistance, 0),
            };
        }

        public override void RenderLight(CommandBuffer cmd)
        {
            if (!LightMaterial)
                LightMaterial = new Material(Shader.Find("Lighting2D/AnalyticLight"));
            LightMaterial.SetTexture("_MainTex", LightTexture);
            LightMaterial.SetColor("_Color", LightColor);
            LightMaterial.SetFloat("_Attenuation", Attenuation);
            LightMaterial.SetFloat("_Intensity", Intensity);

            cmd.SetGlobalVector("_2DLightPos", transform.position);
            cmd.SetGlobalFloat("_LightRange", LightDistance);
            cmd.SetGlobalFloat("_Intensity", Intensity);
            var trs = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            switch(LightType)
            {
                case LightType.Analytical:
                    cmd.DrawMesh(Mesh, trs, LightMaterial, 0, 0);
                    break;
                case LightType.Textured:
                    cmd.DrawMesh(Mesh, trs, LightMaterial, 0, 1);
                    break;
            }
        }
    }
}