using TMPro;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("General")]
    public int width;
    public int height;

    [Header("Noise")]
    public float noiseScale;
    public int octaves;
    public float lacunarity;
    public float persistence;

    [Header("Hexagonal")]
    public HexagonCell hexPrefab;
    public TextMeshProUGUI labelHexPrefab;
    public float ampliHeight;

    private readonly float hexSize = 1f;
    Canvas gridCanvas;

    HexagonCell[] cells;


    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, noiseScale, octaves, lacunarity, persistence);

        MapDisplay display = FindAnyObjectByType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);

        GenerateGrid(noiseMap);
    }

    public void GenerateGrid(float[,] noiseMap)
    {
        gridCanvas = GetComponentInChildren<Canvas>();

        cells = new HexagonCell[height * width];

        // Loop over each row and column to create a hexagon at the appropriate position
        for (int y = 0, i = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Create a new hexagon
                HexagonCell cell = CreateCell(x, y, i++);
                //Set Position and height of the hexagon
                Vector3 cellPos = SetPositionCell(cell, x, y, noiseMap);
                cell.GenerateSurface();

                CreateLabelCell(cell, cellPos, x, y);
            }
        }

        Generate2nd();

    }

    public void Generate2nd()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].UpdateQuad();
        }
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

    private Vector3 SetPositionCell(HexagonCell cell, int x, int y, float[,] noiseMap)
    {
        float hexHeight = noiseMap[x, y];
        Vector3 position;
        position.x = (x + y * 0.5f - y / 2) * hexSize * (0.866025404f * 2);
        position.y = hexHeight * ampliHeight;
        position.z = y * hexSize * 1.5f;

        cell.transform.localPosition = position;
        return cell.transform.position;
    }

    private void CreateLabelCell(HexagonCell cell, Vector3 cellPos, int x, int y)
    {
        TextMeshProUGUI label = Instantiate(labelHexPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);

        label.text = cell.coordinates.ToStringOnSeparateLines();

        label.rectTransform.localPosition =
            new Vector3(cellPos.x, cellPos.z, -cellPos.y);
    }
}
