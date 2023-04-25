using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth , int mapHeight, float scale, int octaves, float lacunarity, float persistence)
    {

        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0) scale = 0.0001f;

        float minNoiseValue = float.MaxValue;
        float maxNoiseValue = float.MinValue;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float frequency = 1f;
                float amplitude = 1f;
                float noiseValue = 0f;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * frequency;
                    float sampleY = y / scale * frequency;

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
