using Map.Coordinate;
using Script.Map;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace Map.DataGen
{
    public static class ChunkGenerator
    {

        public static CellsData GenerateChunk(int size,float[,] noiseMap, float ampliHeight, AnimationCurve curve, HexagonCell hexaPrefab)
        {
            AnimationCurve heightCurve = new(curve.keys);
            var width = noiseMap.GetLength(0);
            var height = noiseMap.GetLength(1);

            CellsData cellsData = new(width, height);
 
            // Loop over each row and column to create a hexagon at the appropriate position
            for (int y = 0, i = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    //Stock Coordinates of the future cell
                    cellsData.CoordCell(i, x, y);

                    //Stock Position and height of the future cell
                    cellsData.PositionCell(size,i++, x, y, noiseMap, ampliHeight, heightCurve);
                }
            }
            return cellsData;
        }
    }

    public class CellsData
    {
        public readonly int cellNumber;
        public readonly HexaCoordinates[] Coords;
        public readonly Vector3[] Positions;
        public readonly Vector3[] Scales;


        public CellsData(int mapWidth, int mapHeight)
        {
            var chunkSize = mapWidth * mapHeight;
            cellNumber = mapWidth * mapHeight;
            Coords = new HexaCoordinates[chunkSize];
            Positions = new Vector3[chunkSize];
            Scales = new Vector3[chunkSize];
        }

        public void CoordCell(int countCell, int x, int y)
        {
            Coords[countCell] = HexaCoordinates.FromOffsetCoordinates(x, y);
        }

        public void PositionCell(int cellSize, int countCell, int x, int y, float[,] noiseMap, float ampliHeight, AnimationCurve heightCurve)
        {
            //Map Position based on Coordinate
            Vector3 position;
            // ReSharper disable once PossibleLossOfFraction
            position.x = (x + y * 0.5f - y / 2) * cellSize * (0.866025404f * 2);
            position.y = 0f;
            position.z = y * cellSize * 1.5f;
            Positions[countCell] = position;


            //Add Curve to Height Map
            var hexHeight = heightCurve.Evaluate(noiseMap[x, y]);

            //Set Height from Scale
            Vector3 scale;
            scale.x = cellSize;
            scale.y = cellSize;
            scale.z = hexHeight * ampliHeight + 0.1f;
            Scales[countCell] = scale;
        }
    }
}