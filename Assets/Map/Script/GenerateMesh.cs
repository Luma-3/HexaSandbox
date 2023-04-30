using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class GenerateMesh : MonoBehaviour
{

    public HexagonCell hexaPrefab;
    public TextMeshProUGUI labelPrefab;

    private int width;
    private int height;
    CellData cellData;

    public void GenerateGrid(int size,float[,] noiseMap, TerrainType[] regions, float ampliHeight, AnimationCurve heightCurve, bool label)
    {
        
        width = noiseMap.GetLength(0);
        height = noiseMap.GetLength(1);

        cellData = new(size, width, height);


        Canvas gridCanvas = GetComponentInChildren<Canvas>();
        
        // Loop over each row and column to create a hexagon at the appropriate position
        for (int y = 0, i = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //Stock Coordinates of the futur cell
                cellData.CoordCell(i, x, y);

                //Stock Position and height of the futur cell
                cellData.SetPositionCell(i, x, y, noiseMap, ampliHeight, heightCurve);

                if (label)
                {
                    cellData.CreateLabelCell(i, Instantiate(labelPrefab, gridCanvas.transform));
                }

                MeshRenderer renderer = cellData.CreateCell(i++, Instantiate(hexaPrefab, transform));

                cellData.TextureCell(regions, x, y, noiseMap, renderer);

            }
        }
    }


    public void UpdateGrid(float[,] noiseMap,TerrainType[] regions, float ampliHeight, AnimationCurve heightCurve)
    {
        for (int y = 0, i = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cellData.SetPositionCell(i, x, y, noiseMap, ampliHeight, heightCurve);
                cellData.CreateCell(i);
                cellData.TextureCell(regions, x, y, noiseMap, cellData._cells[i++].RecoverComponent());
            }
        }
    }

    public void DestroyGrid()
    {
        for (int i = 0; i < cellData._cells.Length; i++)
        {
            DestroyImmediate(cellData._cells[i].gameObject);
            if (cellData._labels[i] != null)
            {
                DestroyImmediate(cellData._labels[i].gameObject);
            }
        }
    }
}

public class CellData
{
    [SerializeField] Canvas gridCanvas;

    private readonly int _size;
    public readonly HexagonCell[] _cells;
    private readonly HexaCoordinates[] _coords;
    private readonly Vector3[] _postions;
    private readonly Vector3[] _scales;
    public readonly TextMeshProUGUI[] _labels;


    public CellData(int size, int mapWidth, int mapHeight)
    {
        int gridSize = mapWidth * mapHeight;
        _size = size;
        _cells = new HexagonCell[gridSize];
        _coords = new HexaCoordinates[gridSize];
        _postions = new Vector3[gridSize];
        _scales = new Vector3[gridSize];
        _labels = new TextMeshProUGUI[gridSize];
    }

    public void CoordCell(int countCell, int x, int y)
    {
        _coords[countCell] = HexaCoordinates.FromOffsetCoordinates(x, y);
    }

    public void SetPositionCell(int countCell, int x, int y, float[,] noiseMap, float ampliHeight, AnimationCurve heightCurve)
    {
        //Map Posistion based on Coordinate
        Vector3 position;
        position.x = (x + y * 0.5f - y / 2) * _size * (0.866025404f * 2);
        position.y = 0f;
        position.z = y * _size * 1.5f;
        _postions[countCell] = position;


        //Add Curve to Height Map
        float hexHeight = heightCurve.Evaluate(noiseMap[x, y]);

        //Set Height from Scale
        Vector3 scale;
        scale.x = _size;
        scale.y = _size;
        scale.z = hexHeight * ampliHeight + 0.1f;
        _scales[countCell] = scale;
    }

    public void CreateLabelCell(int countCell, TextMeshProUGUI label)
    {
        _labels[countCell] = label;
    }

    public MeshRenderer CreateCell(int countCell, [Optional] HexagonCell cell)
    {
        if (_cells[countCell] == null)
        {
            _cells[countCell] = cell;
            cell.coordinates = _coords[countCell];
        }

        _cells[countCell].transform.localPosition = _postions[countCell];
        _cells[countCell].transform.localScale = _scales[countCell];

        if (_labels[countCell] != null)
        {
            _cells[countCell].label = _labels[countCell];
            _labels[countCell].rectTransform.localPosition = new Vector3(_postions[countCell].x, _postions[countCell].z, -_scales[countCell].z);
            _labels[countCell].text = _cells[countCell].coordinates.ToStringOnSeparateLines();   
        }

        return _cells[countCell].RecoverComponent();
    }

    public void TextureCell(TerrainType[] regions, int x, int y, float[,] noiseMap, MeshRenderer renderer)
    {
        float currentHeigth = noiseMap[x,y];
        for (int i = 0; i < regions.Length; i++)
        {
            if (currentHeigth <= regions[i].height)
            {
                renderer.sharedMaterial = regions[i].material;
                break;
            }
        }
    }
}
