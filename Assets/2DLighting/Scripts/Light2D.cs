using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

namespace Lighting2D
{
    [ExecuteInEditMode]
    public class Light2D : Light2DBase
    {
        public float Intensity = 1;
        public float Range = 5;
        public Mesh Mesh;
        public Material LightMaterial;
        private void Awake()
        {
            var halfRange = Range / 2;
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
        // Use this for initialization
        void Start()
        {

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
                new Vector3(-Range, -Range, 0),
                new Vector3(Range, -Range, 0),
                new Vector3(-Range, Range, 0),
                new Vector3(Range, Range, 0),
            };
        }

        public override void RenderLight(CommandBuffer cmd)
        {
            if (!LightMaterial)
                LightMaterial = new Material(Shader.Find("Lighting2D/AnalyticLight"));
            cmd.SetGlobalVector("_2DLightPos", transform.position);
            cmd.SetGlobalFloat("_LightRange", Range);
            cmd.SetGlobalFloat("_Intensity", Intensity);
            var trs = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            cmd.DrawMesh(Mesh, trs, LightMaterial);
        }
    }
}