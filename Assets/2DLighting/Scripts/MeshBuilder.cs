using UnityEngine;
using System.Collections;
using System;

public class MeshBuilder
{
    public Vector3[] Vertices;
    public int[] Triangles;
    int VerticesCount = 0;
    int TriangleCount = 0;

    public MeshBuilder() : this(0, 0)
    {

    }

    public MeshBuilder(int vertCount, int triangleCount)
    {
        Vertices = new Vector3[vertCount];
        Triangles = new int[triangleCount * 3];
    }

    public void AddVertsAndTriangles(Vector3[] vertices, int[] triangles)
    {
        if(vertices.Length + VerticesCount > Vertices.Length)
            Array.Resize(ref Vertices, vertices.Length + VerticesCount);
        if (triangles.Length + TriangleCount > Triangles.Length)
            Array.Resize(ref Triangles, triangles.Length + TriangleCount);

        var offset = VerticesCount;
        for (var i = 0; i < vertices.Length; i++)
            Vertices[offset + i] = vertices[i];
        for (var i = 0; i < triangles.Length; i++)
            Triangles[TriangleCount + i] = triangles[i] + offset;
        VerticesCount += vertices.Length;
        TriangleCount += triangles.Length;
    }

    public void AddCopiedMesh(Mesh mesh)
    {
        AddVertsAndTriangles(mesh.vertices, mesh.triangles);
    }

    public Mesh ToMesh(Mesh mesh)
    {
        mesh.Clear();
        mesh.vertices = Vertices;
        mesh.triangles = Triangles;
        return mesh;
    }

    public Mesh ToMesh()
    {
        return ToMesh(new Mesh());
    }
}
