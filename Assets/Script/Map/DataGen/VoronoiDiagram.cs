using Map.Management;
using UnityEngine;

namespace Map.DataGen
{
    public static class VoronoiDiagram
    {
        public static Color[] GenerateDiagram(int mapSize, int biomeSize, int seed, BiomeType[] biome)
        {
            var colour = new Color[biomeSize * biomeSize];
            
            var pixelPerCell = mapSize / biomeSize;
            var pointPos = GeneratePoints(biomeSize,pixelPerCell, seed, biome, colour);

            
            var vonoroiColours = new Color[mapSize * mapSize];

            for (var i = 0; i < mapSize; i++)
            {
                for (var j = 0; j < mapSize; j++)
                {
                    var gridX = i / pixelPerCell;
                    var gridY = j / pixelPerCell;
                    
                    var nearestDistance = Mathf.Infinity;
                    var nearestPoint = new Vector2Int();
                    for (var a = -1; a < 2; a++)
                    {
                        for (var b = -1; b < 2; b++)
                        {
                            var x = gridX + a;
                            var y = gridY + b;
                            
                            if (x < 0 || y < 0 || x >= biomeSize || y >= biomeSize) continue;
                            
                            var distance = Vector2Int.Distance(new Vector2Int(i, j), pointPos[y, x]);
                            if (distance < nearestDistance)
                            {
                                nearestDistance = distance;
                                nearestPoint = new Vector2Int(x, y);
                            }
                        }
                        

                        vonoroiColours[j * mapSize + i] = colour[nearestPoint.y * biomeSize + nearestPoint.x];
                    }
                }
            }

            return vonoroiColours;
        }


        private static Vector2Int[,] GeneratePoints(int biomeSize, int pixelPerCell, int seed, BiomeType[] biome, Color[] colour)
        {
            var prng = new System.Random(seed);
            
            var pointPos = new Vector2Int[biomeSize, biomeSize];
            for (var y = 0; y < biomeSize; y++)
            {
                for (var x = 0; x < biomeSize; x++)
                {
                    var rng = prng.Next(0, pixelPerCell);
                    var rngColor = prng.Next(0, biome.Length);
                    pointPos[y, x] = new Vector2Int(x * pixelPerCell + rng, y * pixelPerCell + rng);
                    colour[y * biomeSize + x] = biome[rngColor].colour;
                }
            }
            return pointPos;
        }
    }
}

