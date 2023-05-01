using UnityEngine;

public static class ChunkGenerator
{

    public static CellsData GenerateChunk(int size,float[,] noiseMap, float ampliHeight, AnimationCurve _heightCurve, HexagonCell hexaPrefab)
    {
        AnimationCurve heightCurve = new(_heightCurve.keys);
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        CellsData cellData = new(size, width, height, hexaPrefab);
 
        // Loop over each row and column to create a hexagon at the appropriate position
        for (int y = 0, i = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //Stock Coordinates of the futur cell
                cellData.CoordCell(i, x, y);

                //Stock Position and height of the futur cell
                cellData.PositionCell(i++, x, y, noiseMap, ampliHeight, heightCurve);
            }
        }
        return cellData;
    }
}

public class CellsData
{
    [SerializeField] Canvas gridCanvas;

    private readonly int _size;
    public readonly HexagonCell[] _cells;
    private readonly HexaCoordinates[] _coords;
    private readonly Vector3[] _postions;
    private readonly Vector3[] _scales;
    public HexagonCell _prefab;


    public CellsData(int size, int mapWidth, int mapHeight, HexagonCell prefab)
    {
        int chunkSize = mapWidth * mapHeight;
        _size = size;
        _cells = new HexagonCell[chunkSize];
        _coords = new HexaCoordinates[chunkSize];
        _postions = new Vector3[chunkSize];
        _scales = new Vector3[chunkSize];
        _prefab = prefab;
    }

    public void CoordCell(int countCell, int x, int y)
    {
        _coords[countCell] = HexaCoordinates.FromOffsetCoordinates(x, y);
    }

    public void PositionCell(int countCell, int x, int y, float[,] noiseMap, float ampliHeight, AnimationCurve heightCurve)
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

    public HexagonCell CreateCell(int countCell)
    {
        if (_cells[countCell] == null)
        {

            _cells[countCell] = Object.Instantiate(_prefab);
            _cells[countCell].coordinates = _coords[countCell];
        }

        return _cells[countCell];
    }

    public void SetPosition(int countCell)
    {
        _cells[countCell].transform.localPosition = _postions[countCell];
        _cells[countCell].transform.localScale = _scales[countCell];
    }
}
