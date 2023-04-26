using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexagonCell : MonoBehaviour
{
    
    
    public HexaCoordinates coordinates;
    public TextMeshProUGUI label;

    private readonly int radius = 1;
    private readonly int numberOfVertices = 6;

    [SerializeField] HexagonCell[] neighbors;
    private Vector3[] corners;

    private List<Vector3> vertices;
    private List<int> triangles;
    private Mesh mesh;


    public void GenerateMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        mesh = new();
        meshFilter.mesh = mesh;

        vertices = new List<Vector3>();
        triangles = new List<int>((numberOfVertices - 2) * 3); 

        for (int i = 0; i < numberOfVertices; i++)
        {
            float x = radius * Mathf.Cos(i * 2 * Mathf.PI / numberOfVertices);
            float y = radius * Mathf.Sin(i * 2 * Mathf.PI / numberOfVertices);
            vertices.Add(new Vector3(y, 0, x));
        }

        for (int i = 1; i < numberOfVertices - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        GetCorner();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    public void GetCorner()
    {
        corners = new Vector3[6];

        for (int i = 0; i < 6; i++)
        {
            float angle_deg = 60 * i + 30;
            float angle_rad = Mathf.PI / 180f * angle_deg;
            corners[i] = new Vector3(radius * Mathf.Cos(angle_rad), 0f, radius * Mathf.Sin(angle_rad));
        }
    }

    public void GenerateSurface()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 v1 = GetFirstSolidCorner(i);
            Vector3 v2 = GetSecondSolidCorner(i);
            Vector3 v3 = v1;
            Vector3 v4 = v2;
            AddQuad(v1, v2, v3, v4);
        }
    }


    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v4);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v1);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();



    }

    public void UpdateQuad()
    {
        HexaDirection[] neighborDir = new HexaDirection[3] { HexaDirection.NE, HexaDirection.NW, HexaDirection.W };

        int i = 6;
        foreach (HexaDirection neighbor in neighborDir)
        {
            if (GetNeighbor(neighbor) != null)
            {

                float heightDif = GetNeighbor(neighbor).transform.localPosition.y - transform.localPosition.y;

                vertices[i] = new Vector3(vertices[i].x, heightDif, vertices[i].z);
                i += 2;
                vertices[i] = new Vector3(vertices[i].x, heightDif, vertices[i].z);
                i += 2;
            }
            else
            {
                i += 4;
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.RecalculateNormals();
    }

    public HexagonCell GetNeighbor (HexaDirection direction) // public methode for retrieve cell's neightbor (voisin)
    {
        return neighbors[(int)direction];
    }

    public void SetNieghbor (HexaDirection direction, HexagonCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public Vector3 GetFirstSolidCorner(int direction)
    {
        return corners[direction];
    }

    public Vector3 GetSecondSolidCorner(int direction)
    {
        return corners[direction + 1];
    }
}
