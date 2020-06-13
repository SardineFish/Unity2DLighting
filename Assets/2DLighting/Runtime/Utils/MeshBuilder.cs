using UnityEngine;
using System.Collections;
using System;

namespace Lighting2D
{
    public class MeshBuilder
    {
        public Vector3[] Vertices;
        public int[] Triangles;
        public Vector2[] UV1;
        public Vector2[] UV2;
        int VerticesCount = 0;
        int TriangleCount = 0;

        public MeshBuilder() : this(0, 0)
        {

        }

        public MeshBuilder(int vertCount, int triangleCount)
        {
            Vertices = new Vector3[vertCount];
            UV1 = new Vector2[vertCount];
            UV2 = new Vector2[vertCount];
            Triangles = new int[triangleCount * 3];
        }

        public void ResizeVerts(int vertCount)
        {
            Array.Resize(ref Vertices, vertCount);
            Array.Resize(ref UV1, vertCount);
            Array.Resize(ref UV2, vertCount);
        }

        public void AddVertsAndTriangles(Vector3[] vertices, int[] triangles, Vector2[] uv1, Vector2[] uv2)
        {
            if (vertices.Length + VerticesCount > Vertices.Length)
                ResizeVerts(vertices.Length + VerticesCount);
            if (triangles.Length + TriangleCount > Triangles.Length)
                Array.Resize(ref Triangles, triangles.Length + TriangleCount);

            var offset = VerticesCount;
            for (var i = 0; i < vertices.Length; i++)
            {
                Vertices[offset + i] = vertices[i];
                UV1[offset + i] = uv1[i];
                UV2[offset + i] = uv2[i];
            }
            for (var i = 0; i < triangles.Length; i++)
                Triangles[TriangleCount + i] = triangles[i] + offset;
            VerticesCount += vertices.Length;
            TriangleCount += triangles.Length;
        }

        public void AddCopiedMesh(Mesh mesh)
        {
            AddVertsAndTriangles(mesh.vertices, mesh.triangles, mesh.uv, mesh.uv2);
        }

        public Mesh ToMesh(Mesh mesh)
        {
            mesh.Clear();
            mesh.vertices = Vertices;
            mesh.triangles = Triangles;
            mesh.uv = UV1;
            mesh.uv2 = UV2;
            return mesh;
        }

        public Mesh ToMesh()
        {
            return ToMesh(new Mesh());
        }
    }

}

