using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth , int mapHeight, int seed, float scale, int octaves, float lacunarity, float persistence, Vector2 offset)
    {

        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new(seed);
        Vector2[] octaveOffset = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffset[i] = new Vector2(offsetX, offsetY);
        }


        if (scale <= 0) scale = 0.0001f;

        float minNoiseValue = float.MaxValue;
        float maxNoiseValue = float.MinValue;

        float halfWidth = mapWidth / 2;
        float halfHeight = mapHeight / 2;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float frequency = 1f;
                float amplitude = 1f;
                float noiseValue = 0f;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffset[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffset[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseValue += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (noiseValue > maxNoiseValue)
                {
                    maxNoiseValue = noiseValue;
                }
                else if (noiseValue < minNoiseValue)
                {
                    minNoiseValue = noiseValue;
                }

                noiseMap[x,y] = noiseValue;
            }
        }


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x,y] = Mathf.InverseLerp(minNoiseValue,maxNoiseValue, noiseMap[x,y]);
            }
        }

        return noiseMap;

    }
}
