using System.Collections;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [Header("General")]
    public int cellSize;
    public int mapWidth;
    public int mapHeight;

    [Header("Noise")]
    public float noiseScale;
    public int octaves;
    public float lacunarity;
    [Range(0.1f,1f)]public float persistence;
    public float ampliHeight;
    public AnimationCurve heightCurve;
    public int seed;
    public Vector2 offset;

    [Header("Chunk")]
    public bool genrateLabel;

    public TerrainType[] regions;

    public enum DrawMode { NoiseMap, ColourMap, Mesh }
    public DrawMode drawMode;

    public bool autoUpdate;

    GenerateMesh generateMesh;


    public void GenerateMap()
    { 
        float[,] noiseMap = GenerateNoiseMap();

        Color[] colourMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindAnyObjectByType<MapDisplay>();
        generateMesh = FindAnyObjectByType<GenerateMesh>();
        if(drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if(drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
        }
        else if(drawMode == DrawMode.Mesh) 
        {
            generateMesh.GenerateGrid(cellSize, noiseMap, ampliHeight, heightCurve, genrateLabel);
        }

    }

    public void UpdateMap()
    {
        float[,] noiseMap = GenerateNoiseMap();
        generateMesh.UpdateGrid(noiseMap,ampliHeight,heightCurve);
    }

    public void DestroyMap()
    {
        generateMesh.DestroyGrid();
    }


    public float[,] GenerateNoiseMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, lacunarity, persistence, offset);
        return noiseMap;
    }


    private void OnValidate()
    {
        if (mapHeight < 1) { mapHeight = 1; }
        if (mapWidth < 1) { mapWidth = 1; }
        if (noiseScale < 1) { noiseScale = 1;}
        if (octaves < 0) { octaves = 0; }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}
