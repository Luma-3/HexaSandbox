using TMPro;
using UnityEngine;

public class GenerateMesh : MonoBehaviour
{
    private readonly float hexSize = 1f;

    public HexagonCell hexPrefab;
    public TextMeshProUGUI labelHexPrefab;

    int width;
    int height;

    Canvas gridCanvas;

    HexagonCell[] cells;
    Vector2[] uvs;

    public Mesh GenerateGrid(float[,] noiseMap, float ampliHeight, bool linkMesh, bool label, AnimationCurve heightCurve)
    {
        width = noiseMap.GetLength(0);
        height = noiseMap.GetLength(1);

        gridCanvas = GetComponentInChildren<Canvas>();

        cells = new HexagonCell[height * width];
        
        

        Mesh mesh = null;

        // Loop over each row and column to create a hexagon at the appropriate position
        for (int y = 0, i = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Create a new hexagon
                
                HexagonCell cell = CreateCell(x, y, i++);
                //Set Position and height of the hexagon
                Vector3 cellPos = SetPositionCell(cell, x, y, noiseMap, ampliHeight, heightCurve);
                cell.GenerateSurface();
                if (label)
                {
                    CreateLabelCell(cell, cellPos);
                }
            }
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].UpdateQuad();
        }

        if (linkMesh)
        {
            mesh = LinkMesh();
        }



        return mesh;
    }

    public Mesh LinkMesh()
    {
        MeshFilter[] meshFilter = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilter.Length];

        for (int i = 0; i < meshFilter.Length; i++)
        {
            combine[i].mesh = meshFilter[i].sharedMesh;
            combine[i].transform = meshFilter[i].transform.localToWorldMatrix;
            
            DestroyImmediate(meshFilter[i].gameObject);
        }

        Mesh mesh = new();
        mesh.CombineMeshes(combine);
        uvs = new Vector2[height * width * 18];

        for (int y = 0, j = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int i = 0; i < 18; i++)
                {
                    uvs[j] = new Vector2(x/(float)width, y/(float)height);
                    j++;
                }
            }
        }
        
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }


    private HexagonCell CreateCell(int x, int y, int countCell)
    {
        HexagonCell cell = cells[countCell] = Instantiate(hexPrefab);
        cell.transform.SetParent(transform, false);

        cell.transform.localScale = new Vector3(hexSize, hexSize, hexSize);
        cell.GenerateMesh();

        cell.coordinates = HexaCoordinates.FromOffsetCoordinates(x, y);

        if (x > 0)
        {
            cell.SetNieghbor(HexaDirection.W, cells[countCell - 1]);
        }
        if (y > 0)
        {
            if ((y & 1) == 0)
            {
                cell.SetNieghbor(HexaDirection.SE, cells[countCell - width]);
                if (x > 0)
                {
                    cell.SetNieghbor(HexaDirection.SW, cells[countCell - width - 1]);
                }
            }
            else
            {
                cell.SetNieghbor(HexaDirection.SW, cells[countCell - width]);
                if (x < width - 1)
                {
                    cell.SetNieghbor(HexaDirection.SE, cells[countCell - width + 1]);
                }
            }
        }

        return cell;

    }

    private void CreateLabelCell(HexagonCell cell, Vector3 cellPos)
    {

        TextMeshProUGUI label = Instantiate(labelHexPrefab);
        cell.label = label;
        label.rectTransform.SetParent(gridCanvas.transform, false);

        label.text = cell.coordinates.ToStringOnSeparateLines();

        label.rectTransform.localPosition =
            new Vector3(cellPos.x, cellPos.z, -cellPos.y);
    }

    private Vector3 SetPositionCell(HexagonCell cell, int x, int y, float[,] noiseMap, float ampliHeight, AnimationCurve heightCurve)
    {
        float hexHeight = heightCurve.Evaluate(noiseMap[x, y]);
        Vector3 position;
        position.x = (x + y * 0.5f - y / 2) * hexSize * (0.866025404f * 2);
        position.y = hexHeight * ampliHeight;
        position.z = y * hexSize * 1.5f;

        cell.transform.localPosition = position;
        return position;
    }

}
