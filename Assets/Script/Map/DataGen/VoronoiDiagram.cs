using Map.Manager;
using UnityEngine;

namespace Map.DataGen
{
    public static class VoronoiDiagram
    {
        public static int[] GenerateDiagram(int mapSize, int biomeSize, int seed, BiomeType[] biome, Vector2Int offset)
        {
            var id = new int[biomeSize * biomeSize];
            
            var pixelPerCell = mapSize / biomeSize;
            var pointPos = GeneratePoints(biomeSize,pixelPerCell, seed, biome, id, offset);
            
            var voronoiID = new int[mapSize * mapSize];

            for (var i = 0; i < mapSize; i++)
            {
                for (var j = 0; j < mapSize; j++)
                {
                    var gridX = i / pixelPerCell + offset.x;
                    var gridY = j / pixelPerCell + offset.y;
                    
                    var nearestDistance = Mathf.Infinity;
                    var nearestPoint = new Vector2Int();
                    for (var a = -1; a < 2; a++)
                    {
                        for (var b = -1; b < 2; b++)
                        {
                            var x = gridX + a ;
                            var y = gridY + b ;
                            
                            if (x < 0 || y < 0 || x >= biomeSize || y >= biomeSize) continue;
                            
                            var distance = Vector2Int.Distance(new Vector2Int(i, j), pointPos[y, x]);
                            if (distance < nearestDistance)
                            {
                                nearestDistance = distance;
                                nearestPoint = new Vector2Int(x, y);
                            }
                        }
                    }
                    voronoiID[j * mapSize + i] = id[nearestPoint.y * biomeSize + nearestPoint.x];
                }
            }

            return voronoiID;
        }


        private static Vector2Int[,] GeneratePoints(int biomeSize, int pixelPerCell, int seed, BiomeType[] biome, int[] id, Vector2Int offset)
        {
            var prng = new System.Random(seed);
            
            var pointPos = new Vector2Int[biomeSize, biomeSize];
            for (var y = 0; y < biomeSize; y++)
            {
                for (var x = 0; x < biomeSize; x++)
                {
                    var rngX = prng.Next(0, pixelPerCell) + offset.x;
                    var rngY = prng.Next(0, pixelPerCell) + offset.y;
                    var rngBiome = prng.Next(0, biome.Length);
                    pointPos[y, x] = new Vector2Int(x * pixelPerCell + rngX, y * pixelPerCell + rngY);
                    id[y * biomeSize + x] = biome[rngBiome].biomeData.id;
                }
            }
            return pointPos;
        }
    }
}

