using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Lighting2D
{
    public abstract class Light2DBase : MonoBehaviour
    {
        public LightShadows LightShadows = LightShadows.None;
        public float ShadowRange = 20;
        public float LightSize = 1;
        public abstract void RenderLight(CommandBuffer cmd);

        [SerializeField]
        private bool DebugShadow = false;

        protected Material shadowMat;

        private Mesh ShadowMesh;

        void Reset()
        {
            tempMesh = new Mesh();
            tempMesh.name = "Sub Shadow Mesh";
            ShadowMesh = new Mesh();
            ShadowMesh.name = "Shadow Mesh";
            ShadowMesh.Clear();
            shadowMat = new Material(Shader.Find("Lighting2D/Shadow"));
        }

        protected virtual void Start()
        {
            tempMesh = new Mesh();
            tempMesh.name = "Sub Shadow Mesh";
            ShadowMesh = new Mesh();
            ShadowMesh.name = "Shadow Mesh";
            ShadowMesh.Clear();
            shadowMat = new Material(Shader.Find("Lighting2D/Shadow"));
        }

        private Collider2D[] shadowCasters = new Collider2D[100];

        public void GenerateShadowMesh()
        {
        }

        Mesh tempMesh;
        public Mesh PolygonShadowMesh(PolygonCollider2D pol)
        {
            var points = pol.GetPath(0);
            var z = new Vector3(0, 0, 1);
            MeshBuilder meshBuilder = new MeshBuilder(5 * points.Length, 3 * points.Length);
            var R_2 = Mathf.Pow(ShadowRange, 2);
            var r_2 = Mathf.Pow(LightSize, 2);
            for (var i = 0; i < points.Length; i++)
            {
                // transform points from collider space to light space
                Vector3 p0 = transform.worldToLocalMatrix.MultiplyPoint(pol.transform.localToWorldMatrix.MultiplyPoint(points[(i + 1) % points.Length]));
                Vector3 p1 = transform.worldToLocalMatrix.MultiplyPoint(pol.transform.localToWorldMatrix.MultiplyPoint(points[i]));
                p0.z = p1.z = 0;
                var ang0 = Mathf.Asin(LightSize / p0.magnitude); // Angle between lightDir & tangent of light circle
                var ang1 = Mathf.Asin(LightSize / p1.magnitude); // Angle between lightDir & tangent of light circle
                Vector3 shadowA = MathUtility.Rotate(p0, -ang0).normalized * (Mathf.Sqrt(R_2 - r_2) - p0.magnitude * Mathf.Cos(ang0));
                Vector3 shadowB = MathUtility.Rotate(p1, ang1).normalized * (Mathf.Sqrt(R_2 - r_2) - p1.magnitude * Mathf.Cos(ang1));
                shadowA += p0;
                shadowB += p1;
                var OC = (shadowA + shadowB) / 2;
                Vector3 shadowR = OC.normalized * (R_2 / OC.magnitude);
                Vector3 normal = Vector3.Cross(z, p1 - p0);

                int meshType = 0;
                if (Vector3.Cross(p1 - p0, shadowB - p1).z >= 0)
                {
                    meshType |= 1;
                }
                if(Vector3.Cross(p0 - shadowA, p1 - p0).z >=0)
                {
                    meshType |= 2;
                }

                if (meshType == 0)
                {
                    meshBuilder.AddVertsAndTriangles(new Vector3[]
                    {
                        p0,
                        p1,
                        shadowB,
                        shadowA,
                        shadowR,
                    }, new int[] {
                        0, 3, 4,
                        1, 0, 4,
                        1, 4, 2,
                    });
                }
                else if (meshType == 1) // merge p0->p1 & p1->shadowB
                {
                    meshBuilder.AddVertsAndTriangles(new Vector3[]
                    {
                        p0,
                        shadowB,
                        shadowA,
                        shadowR,
                    }, new int[] {
                        0, 2, 3,
                        0, 3, 1
                    });
                }
                else if (meshType == 2) // merge shadowA->p0 & p0->p1
                {
                    meshBuilder.AddVertsAndTriangles(new Vector3[]
                    {
                        p1,
                        shadowB,
                        shadowA,
                        shadowR,
                    }, new int[] {
                        0, 2, 3,
                        0, 3, 1
                    });
                }
                else if (meshType == 3) // cross
                {
                    meshBuilder.AddVertsAndTriangles(new Vector3[]
                    {
                        p1,
                        p0,
                        shadowB,
                        shadowA,
                        shadowR,
                    }, new int[] {
                        0, 3, 4,
                        1, 0, 4,
                        1, 4, 2,
                    });
                }
                if(DebugShadow)
                {
                    Debug.DrawLine(transform.localToWorldMatrix.MultiplyPoint(p0), transform.localToWorldMatrix.MultiplyPoint(p1), Color.red);
                    Debug.DrawLine(transform.localToWorldMatrix.MultiplyPoint(p1), transform.localToWorldMatrix.MultiplyPoint(shadowB), Color.green);
                    Debug.DrawLine(transform.localToWorldMatrix.MultiplyPoint(p0), transform.localToWorldMatrix.MultiplyPoint(shadowA), Color.blue);
                    Debug.DrawLine(transform.localToWorldMatrix.MultiplyPoint(shadowA), transform.localToWorldMatrix.MultiplyPoint(shadowR), Color.white);
                    Debug.DrawLine(transform.localToWorldMatrix.MultiplyPoint(shadowB), transform.localToWorldMatrix.MultiplyPoint(shadowR), Color.white);
                    return meshBuilder.ToMesh(ShadowMesh);
                }
            }
            var mesh = meshBuilder.ToMesh(tempMesh);
            mesh.RecalculateNormals();
            return mesh;
        }

        List<Mesh> subShadowMesh = new List<Mesh>(256);

        public void RenderShadow(CommandBuffer cmd)
        {
            if(shadowMat == null)
                shadowMat = new Material(Shader.Find("Lighting2D/Shadow"));
            if (!ShadowMesh)
                ShadowMesh = new Mesh();
            if (!tempMesh)
                tempMesh = new Mesh();
            ShadowMesh.Clear();
            //subShadowMesh.ForEach(mesh => mesh.Clear());
            //subShadowMesh.Clear();
            var meshBuilder = new MeshBuilder();
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, ShadowRange, shadowCasters);
            CombineInstance[] combineArr = new CombineInstance[count];
            for (var i = 0; i < count; i++)
            {
                Collider2D caster = shadowCasters[i];
                if (caster is PolygonCollider2D)
                {
                    var mesh = PolygonShadowMesh(caster as PolygonCollider2D);
                    combineArr[i].mesh = mesh;
                    meshBuilder.AddCopiedMesh(mesh);
                    mesh.Clear();
                }
            }
            ShadowMesh = meshBuilder.ToMesh(ShadowMesh);
            //ShadowMesh.CombineMeshes(combineArr.Where(combine => combine.mesh != null).ToArray());
            cmd.DrawMesh(ShadowMesh, Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale), shadowMat);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, LightSize);
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, ShadowRange);
        }
    }
}