using System;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDst = 150f;
    public Transform viewer;

    public static Vector2 viewerPosition;
    static MapGenerator mapGenerator;
    private int chunkSize;
    private int cellSize;
    private int chunkVisibleInViewDst;
    readonly Dictionary<Vector2, TerrainChunk> terrainChunkDic = new();
    readonly List<TerrainChunk> terrianChunkVisibleLastUpdate = new();
    Vector2 realSize = new(110.8512517f, 96);


    void Start()
    {
        mapGenerator = FindAnyObjectByType<MapGenerator>();
        chunkSize = mapGenerator.chunkSize;
        cellSize = mapGenerator.cellSize;
        chunkVisibleInViewDst = Mathf.RoundToInt(maxViewDst / 64f);
    }


    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunk();
    }

    void UpdateVisibleChunk()
    {
        for (int i = 0; i < terrianChunkVisibleLastUpdate.Count; i++)
        {
            terrianChunkVisibleLastUpdate[i].SetVisible(false);
        }
        terrianChunkVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / realSize.x);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / realSize.y);

        for (int yOffset = -chunkVisibleInViewDst; yOffset <= chunkVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunkVisibleInViewDst; xOffset < chunkVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new(currentChunkCoordX - xOffset, currentChunkCoordY - yOffset);

                if (terrainChunkDic.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDic[viewedChunkCoord].UpdateTerrainChunk();
                    if (terrainChunkDic[viewedChunkCoord].IsVisible())
                    {
                        terrianChunkVisibleLastUpdate.Add(terrainChunkDic[viewedChunkCoord]);
                    }
                }
                else
                {
                    terrainChunkDic.Add(viewedChunkCoord, new(viewedChunkCoord, realSize, transform, chunkSize));
                }
            }
        }
    }


    public class TerrainChunk
    {
        readonly GameObject chunkObject;
        Vector2 position;
        Bounds bounds;

        MapData mapData;

        public TerrainChunk(Vector2 coord,Vector2 realSize, Transform parent, int chunkSize)
        {

            position = coord * realSize - realSize / 2;
            bounds = new Bounds(new Vector2(coord.x * realSize.x, coord.y * realSize.y), realSize);
            mapGenerator.RequestMapData(new Vector2(coord.x * chunkSize,-coord.y * chunkSize ), OnMapDataReceived);
            
            Vector3 positionV3 = new(position.x, 0.0f, position.y);
            chunkObject = new GameObject("Terrain Chunk");
            chunkObject.transform.position = positionV3;
            chunkObject.transform.parent = parent;
            SetVisible(false);   
        }

        void OnMapDataReceived(MapData mapData)
        {
            mapGenerator.RequestCellsData(mapData, OnCellDataReceived);
            this.mapData = mapData;
        }

        void OnCellDataReceived(CellsData cellsData)
        {
            for (int i = 0; i < cellsData._cells.Length; i++)
            {
                HexagonCell cell = cellsData.CreateCell(i);
                cell.transform.parent = chunkObject.transform;

                cellsData.SetPosition(i);
                MeshRenderer renderer = cellsData._cells[i].RecoverComponent();
                renderer.sharedMaterial = mapData.materialMap[i];
            }
        }

        public void UpdateTerrainChunk()
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            //Debug.Log("no squrt : " + bounds.SqrDistance(viewerPosition) + " sqrt : " + viewerDstFromNearestEdge);
            bool visible = viewerDstFromNearestEdge <= maxViewDst;
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            chunkObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return chunkObject.activeSelf;
        }
    }
}


