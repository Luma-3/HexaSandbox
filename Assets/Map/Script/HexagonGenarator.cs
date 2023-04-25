using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexagonGenarator : MonoBehaviour
{
    private readonly int radius = 1;
    private readonly int vertices = 6;

    public List<HexagonGenarator> neighbors = new();

    public void GenerateMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[this.vertices];
        int[] triangles = new int[(this.vertices - 2) * 3];

        for (int i = 0; i < this.vertices; i++)
        {
            float x = radius * Mathf.Cos(i * 2 * Mathf.PI / this.vertices);
            float y = radius * Mathf.Sin(i * 2 * Mathf.PI / this.vertices);
            vertices[i] = new Vector3(x, 0, -y);
        }

        int triangleIndex = 0;
        for (int i = 1; i < this.vertices - 1; i++)
        {
            triangles[triangleIndex++] = 0;
            triangles[triangleIndex++] = i;
            triangles[triangleIndex++] = i + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public void SetHeight(float height)
    {
        Vector3 position = transform.position;
        position.y = height;
        transform.position = position;
    }

}
