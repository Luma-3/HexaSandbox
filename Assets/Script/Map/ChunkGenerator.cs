using UnityEngine;

public static class ChunkGenerator
{

    public static CellsData GenerateChunk(int size,float[,] noiseMap, float ampliHeight, AnimationCurve curve, HexagonCell hexaPrefab)
    {
        AnimationCurve heightCurve = new(curve.keys);
        var width = noiseMap.GetLength(0);
        var height = noiseMap.GetLength(1);

        CellsData cellsData = new(size, width, height, hexaPrefab);
 
        // Loop over each row and column to create a hexagon at the appropriate position
        for (int y = 0, i = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                //Stock Coordinates of the future cell
                cellsData.CoordCell(i, x, y);

                //Stock Position and height of the future cell
                cellsData.PositionCell(i++, x, y, noiseMap, ampliHeight, heightCurve);
            }
        }
        return cellsData;
    }
}

public class CellsData
{
    private readonly int _size;
    public readonly HexagonCell[] Cells;
    private readonly HexaCoordinates[] _coords;
    private readonly Vector3[] _positions;
    private readonly Vector3[] _scales;
    private readonly HexagonCell _prefab;


    public CellsData(int size, int mapWidth, int mapHeight, HexagonCell prefab)
    {
        var chunkSize = mapWidth * mapHeight;
        _size = size;
        Cells = new HexagonCell[chunkSize];
        _coords = new HexaCoordinates[chunkSize];
        _positions = new Vector3[chunkSize];
        _scales = new Vector3[chunkSize];
        _prefab = prefab;
    }

    public void CoordCell(int countCell, int x, int y)
    {
        _coords[countCell] = HexaCoordinates.FromOffsetCoordinates(x, y);
    }

    public void PositionCell(int countCell, int x, int y, float[,] noiseMap, float ampliHeight, AnimationCurve heightCurve)
    {
        //Map Position based on Coordinate
        Vector3 position;
        // ReSharper disable once PossibleLossOfFraction
        position.x = (x + y * 0.5f - y / 2) * _size * (0.866025404f * 2);
        position.y = 0f;
        position.z = y * _size * 1.5f;
        _positions[countCell] = position;


        //Add Curve to Height Map
        var hexHeight = heightCurve.Evaluate(noiseMap[x, y]);

        //Set Height from Scale
        Vector3 scale;
        scale.x = _size;
        scale.y = _size;
        scale.z = hexHeight * ampliHeight + 0.1f;
        _scales[countCell] = scale;
    }

    public HexagonCell CreateCell(int countCell, Transform parent)
    {
        if (Cells[countCell]) return Cells[countCell];
        Cells[countCell] = Object.Instantiate(_prefab, parent);
        Cells[countCell].coordinates = _coords[countCell];

        return Cells[countCell];
    }

    public void SetPosition(int countCell)
    {
        Cells[countCell].transform.localPosition = _positions[countCell];
        Cells[countCell].transform.localScale = _scales[countCell];
    }
}
