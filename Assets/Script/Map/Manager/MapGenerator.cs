using System;
using System.Collections.Generic;
using System.Threading;
using Data;
using Map.Coordinate;
using Map.DataGen;
using UnityEngine;

namespace Map.Manager
{
    public class MapGenerator : MonoBehaviour
    {

        [Header("General")]
        public int chunkSize;
        public int cellSize;

        [Header("Noise")]
        public Noise.NormalizeMode normalizeMode;
        public float noiseScale;
        public int octaves;
        public float lacunarity;
        [Range(0.1f,1f)]public float persistence;
        public BiomeData biome;
        public int seed;
        public Vector2 offset;

        [Header("Biome")] 
        public int chunkBiomeSize;
        public int biomeSize;
        public Vector2Int biomesOffset;
        

        public TerrainType[] regions;
        public BiomeType[] biomes;

        public enum DrawMode { NoiseMap, ColourMap, Mesh, BiomeMap }
        public DrawMode drawMode;

        public bool autoUpdate;

        private readonly Queue<MapThreadInfo<MapData>> _mapDataThreadInfosQueue = new();
        private readonly Queue<MapThreadInfo<CellsData>> _cellsDataThreadInfosQueue = new();

        public void DrawMapInEditor()
        {
            var mapData = GenerateMapData(Vector2.zero);

            var display = FindAnyObjectByType<MapDisplay>();
            
            switch (drawMode)
            {
                case DrawMode.NoiseMap:
                    display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.NoiseMap));
                    break;
                case DrawMode.ColourMap:
                    display.DrawTexture(TextureGenerator.TextureFromColour(mapData.ColourMap, chunkSize, chunkSize));
                    break;
                case DrawMode.Mesh:
                    display.DrawChunk(
                        mapData, ChunkGenerator.GenerateChunk(cellSize, mapData.NoiseMap, biome));
                    break;
                case DrawMode.BiomeMap:
                    display.DrawTexture(TextureGenerator.TextureFromColour(mapData.BiomeColorMap,chunkBiomeSize,chunkBiomeSize));
                    break;
            }
        }

        public void RequestMapData(Vector2 center, Action<MapData> callback)
        {
            ThreadStart threadStart = delegate
            {
                MapDataThread(center, callback);
            };

            new Thread(threadStart).Start();
        }

        private void MapDataThread(Vector2 center, Action<MapData> callback)
        {
            var mapData = GenerateMapData(center);
            lock (_mapDataThreadInfosQueue)
            {
                _mapDataThreadInfosQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
            }

        }

        public void RequestCellsData (MapData mapData, Action<CellsData> callback, ChunkCoordinates chunkCoordinates)
        {
            ThreadStart threadStart = delegate
            {
                CellsDataThread(mapData, callback, chunkCoordinates);
            };

            new Thread(threadStart).Start();
        }

        private void CellsDataThread(MapData mapData, Action<CellsData> callback, ChunkCoordinates chunkCoordinates)
        {
            var cellsData = ChunkGenerator.GenerateChunk(cellSize, mapData.NoiseMap, mapData.BiomeMap[Mathf.Abs(chunkCoordinates.Z * chunkBiomeSize + chunkCoordinates.X)]);
            lock (_cellsDataThreadInfosQueue)
            {
                _cellsDataThreadInfosQueue.Enqueue(new MapThreadInfo<CellsData>(callback,cellsData));
            }
        }

        private void Update()
        {
            if(_mapDataThreadInfosQueue.Count > 0)
            {
                for (var i = 0; i < _mapDataThreadInfosQueue.Count; i++)
                {
                    var threadInfo = _mapDataThreadInfosQueue.Dequeue();
                    threadInfo.Callback(threadInfo.Parameter);
                }
            }

            if (_cellsDataThreadInfosQueue.Count > 0)
            {
                for (var i = 0; i < _cellsDataThreadInfosQueue.Count; i++)
                {
                    var threadInfo = _cellsDataThreadInfosQueue.Dequeue();
                    threadInfo.Callback(threadInfo.Parameter);
                }
            }
        }


        private MapData GenerateMapData(Vector2 center)
        { 
            var noiseMap = Noise.GenerateNoiseMap(chunkSize, chunkSize, seed, noiseScale, octaves, persistence, lacunarity, center + offset, normalizeMode);
            var biomesID = VoronoiDiagram.GenerateDiagram(chunkBiomeSize, biomeSize, seed, biomes, biomesOffset);
            
            var colourMap = new Color[chunkSize * chunkSize];
            var materials = new Material[chunkSize * chunkSize];
            
            var colourBiome = new Color[chunkBiomeSize * chunkBiomeSize];
            var biomeMap = new BiomeData[chunkBiomeSize * chunkBiomeSize];
            
            for (var y = 0; y < chunkSize; y++)
            {
                for (var x = 0; x < chunkSize; x++)
                {
                    var currentHeight = noiseMap[x, y];
                    for (var i = 0; i < regions.Length; i++)
                    {
                        if (currentHeight >= regions[i].height)
                        {
                            colourMap[y * chunkSize + x] = regions[i].colour;
                            materials[y * chunkSize + x] = regions[i].material;
                        }
                        else break;
                    }
                }
            }

            for (var y = 0; y < chunkBiomeSize; y++)
            {
                for (var x = 0; x < chunkBiomeSize; x++)
                {
                    var currentBiome = biomesID[x * chunkBiomeSize + y];
                    colourBiome[y * chunkBiomeSize + x] = biomes[currentBiome].colour;
                    biomeMap[y * chunkBiomeSize + x] = biomes[currentBiome].biomeData;

                }
            }
            
            
            return new MapData(noiseMap, biomeMap, colourBiome, colourMap, materials);
        }


        private void OnValidate()
        {
            if (chunkSize < 1) { chunkSize = 1; }
            if (chunkSize < 1) { chunkSize = 1; }
            if (noiseScale < 1) { noiseScale = 1;}
            if (octaves < 0) { octaves = 0; }
        }

        private struct MapThreadInfo<T>
        {
            public readonly Action<T> Callback; 
            public readonly T Parameter;

            public MapThreadInfo(Action<T> callback, T parameter)
            {
                Callback = callback;
                Parameter = parameter;
            }
        }
    }

    [Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color colour;
        public Material material;
    }
    
    [Serializable]
    public struct BiomeType
    {
        public string name;
        public Color colour;
        public BiomeData biomeData;
    }

    public struct MapData
    {
        public readonly float[,] NoiseMap;
        public readonly BiomeData[] BiomeMap;
        public readonly Color[] BiomeColorMap;
        public readonly Color[] ColourMap;
        public readonly Material[] MaterialMap;

        public MapData(float[,] noiseMap, BiomeData[] biomeMap, Color[] biomeColorMap, Color[] colourMap, Material[] materialMap)
        {
            NoiseMap = noiseMap;
            BiomeMap = biomeMap;
            ColourMap = colourMap;
            MaterialMap = materialMap;
            BiomeColorMap = biomeColorMap;
        }
    }
}