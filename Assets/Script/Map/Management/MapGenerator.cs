using System;
using System.Collections.Generic;
using System.Threading;
using Script.Map.DataGen;
using UnityEngine;

namespace Script.Map.Management
{
    public class MapGenerator : MonoBehaviour
    {

        [Header("General")]
        public HexagonCell cellPrefab;
        public int chunkSize;
        public int cellSize;

        [Header("Noise")]
        public Noise.NormalizeMode normalizeMode;
        public float noiseScale;
        public int octaves;
        public float lacunarity;
        [Range(0.1f,1f)]public float persistence;
        public float ampliHeight;
        public AnimationCurve heightCurve;
        public int seed;
        public Vector2 offset;
    

        public TerrainType[] regions;

        public enum DrawMode { NoiseMap, ColourMap, Mesh }
        public DrawMode drawMode;

        public bool autoUpdate;

        Queue<MapThreadInfo<MapData>> mapDataThreadInfosQueue = new();
        Queue<MapThreadInfo<CellsData>> cellsDataThreadInfosQueue = new();

        public void DrawMapInEditor()
        {
            MapData mapData = GenerateMapData(Vector2.zero);

            MapDisplay display = FindAnyObjectByType<MapDisplay>();
            if (drawMode == DrawMode.NoiseMap)
            {
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.noiseMap));
            }
            else if (drawMode == DrawMode.ColourMap)
            {
                display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, chunkSize, chunkSize));
            }
            else if (drawMode == DrawMode.Mesh)
            {
                display.DrawChunk(ChunkGenerator.GenerateChunk(cellSize, mapData.noiseMap, ampliHeight, heightCurve, cellPrefab), mapData.materialMap);
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

        public void MapDataThread(Vector2 center, Action<MapData> callback)
        {
            MapData mapData = GenerateMapData(center);
            lock (mapDataThreadInfosQueue)
            {
                mapDataThreadInfosQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
            }

        }

        public void RequestCellsData (MapData mapData, Action<CellsData> callback)
        {
            ThreadStart threadStart = delegate
            {
                CellsDataThread(mapData, callback);
            };

            new Thread(threadStart).Start();
        }

        public void CellsDataThread(MapData mapData, Action<CellsData> callback)
        {
            var cellsData = ChunkGenerator.GenerateChunk(cellSize, mapData.noiseMap, ampliHeight, heightCurve, cellPrefab);
            lock (cellsDataThreadInfosQueue)
            {
                cellsDataThreadInfosQueue.Enqueue(new MapThreadInfo<CellsData>(callback,cellsData));
            }
        }

        private void Update()
        {
            if(mapDataThreadInfosQueue.Count > 0)
            {
                for (var i = 0; i < mapDataThreadInfosQueue.Count; i++)
                {
                    var threadInfo = mapDataThreadInfosQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }

            if (cellsDataThreadInfosQueue.Count > 0);
            {
                for (var i = 0; i < cellsDataThreadInfosQueue.Count; i++)
                {
                    var threadInfo = cellsDataThreadInfosQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }
        }


        private MapData GenerateMapData(Vector2 center)
        { 
            var noiseMap = Noise.GenerateNoiseMap(chunkSize, chunkSize, seed, noiseScale, octaves, persistence, lacunarity, center + offset, normalizeMode);

            var colourMap = new Color[chunkSize * chunkSize];
            var materials = new Material[chunkSize * chunkSize];
            for (var y = 0; y < chunkSize; y++)
            {
                for (var x = 0; x < chunkSize; x++)
                {
                    var currentHeight = noiseMap[x, y];
                    for (var i = 0; i < regions.Length; i++)
                    {
                        if(currentHeight >= regions[i].height)
                        {
                            colourMap[y * chunkSize + x] = regions[i].colour;
                            materials[y * chunkSize + x] = regions[i].material; 
                            //Debug.Log(y * chunkSize + x);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return new MapData(noiseMap, colourMap, materials);
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
            public readonly Action<T> callback; 
            public readonly T parameter;

            public MapThreadInfo(Action<T> callback, T parameter)
            {
                this.callback = callback;
                this.parameter = parameter;
            }
        }
    }

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color colour;
        public Material material;
    }

    public struct MapData
    {
        public readonly float[,] noiseMap;
        public readonly Color[] colourMap;
        public readonly Material[] materialMap;

        public MapData(float[,] noiseMap, Color[] colourMap, Material[] materialMap)
        {
            this.noiseMap = noiseMap;
            this.colourMap = colourMap;
            this.materialMap = materialMap;
        }
    }
}