using System.Collections.Generic;
using System.Linq;
using Map.DataGen;
using Unity.VisualScripting;
using UnityEngine;

namespace Map.Management
{
    public class EndlessTerrain : MonoBehaviour
    {
        private const float MaxViewDst = 300f;
        public Transform viewer;

        private static Vector2 _viewerPosition;
        private static MapGenerator _mapGenerator;
        private int _chunkSize;
        private int _chunkVisibleInViewDst;

        private readonly Dictionary<Vector2, TerrainChunk> _terrainChunkDic = new();
        private readonly List<TerrainChunk> _terrainChunkVisibleLastUpdate = new();
    
        //private static readonly Vector2 RealSize = new(110.8512517f, 96);
        //private static readonly Vector2 RealSize = new(55.42562586f, 48);
        private static readonly Vector2 RealSize = new(27.71281292f, 24);


        private void Start()
        {
            _mapGenerator = FindAnyObjectByType<MapGenerator>();
            _chunkSize = _mapGenerator.chunkSize;
            _chunkVisibleInViewDst = Mathf.RoundToInt(MaxViewDst / 64f);
        }


        private void Update()
        {
            var position = viewer.position;
            _viewerPosition = new Vector2(position.x, position.z);
            UpdateVisibleChunk();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void UpdateVisibleChunk()
        {

            foreach (var chunkVisibleLastUpdate in _terrainChunkVisibleLastUpdate.ToList().Where(chunkVisibleLastUpdate => !chunkVisibleLastUpdate.CheckDst()))
            {
                chunkVisibleLastUpdate.SetVisible(false);
                _terrainChunkVisibleLastUpdate.Remove(chunkVisibleLastUpdate);
            }

        
            var currentChunkCoordX = Mathf.RoundToInt(_viewerPosition.x / RealSize.x);
            var currentChunkCoordY = Mathf.RoundToInt(_viewerPosition.y / RealSize.y);
        
            for (var yOffset = -_chunkVisibleInViewDst; yOffset <= _chunkVisibleInViewDst; yOffset++)
            {

                for (var xOffset = -_chunkVisibleInViewDst; xOffset < _chunkVisibleInViewDst; xOffset++)
                {
                    Vector2 viewedChunkCoord = new(currentChunkCoordX - xOffset, currentChunkCoordY - yOffset);
                    if (_terrainChunkDic.ContainsKey(viewedChunkCoord))
                    {
                        _terrainChunkDic[viewedChunkCoord].SetVisible(_terrainChunkDic[viewedChunkCoord].CheckDst());
                        if (_terrainChunkDic[viewedChunkCoord].IsVisible() &&
                            !_terrainChunkVisibleLastUpdate.Contains(_terrainChunkDic[viewedChunkCoord]))
                        {
                            _terrainChunkVisibleLastUpdate.Add(_terrainChunkDic[viewedChunkCoord]);
                        }
                    }
                    else
                    {
                        _terrainChunkDic.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, RealSize, transform, _chunkSize));
                    }
                }
            }
        }


        private class TerrainChunk
        {
            private readonly GameObject _chunkObject;
            private Bounds _bounds;

            private MapData _mapData;

            public TerrainChunk(Vector2 coord,Vector2 realSize, Transform parent, int chunkSize)
            {
                var position = coord * realSize - realSize / 2;
                _bounds = new Bounds(new Vector2(coord.x * realSize.x, coord.y * realSize.y), realSize);
                _mapGenerator.RequestMapData(new Vector2(coord.x * chunkSize,-coord.y * chunkSize ), OnMapDataReceived);
            
                Vector3 positionV3 = new(position.x, 0.0f, position.y);
                _chunkObject = new GameObject("Terrain Chunk")
                {
                    transform =
                    {
                        position = positionV3,
                        parent = parent
                    }
                };
                SetVisible(false);   
            }

            private void OnMapDataReceived(MapData data)
            {
                _mapGenerator.RequestCellsData(data, OnCellDataReceived);
                _mapData = data;
            }

            private void OnCellDataReceived(CellsData cellsData)
            {
                for (var i = 0; i < cellsData.Cells.Length; i++)
                {
                    var cell = cellsData.CreateCell(i, _chunkObject.transform);
                    cell.GetComponent();
                    cellsData.SetPosition(i);

                    cell.RecoverMeshRenderer().sharedMaterial = _mapData.MaterialMap[i];
                    var meshFilter = cell.RecoverMeshFilter();
                    cell.AddComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
                }
            }

            public bool CheckDst()
            {
                var viewerDstFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(_viewerPosition));
                return viewerDstFromNearestEdge <= MaxViewDst;
            }

            public void SetVisible(bool visible)
            {
                _chunkObject.SetActive(visible);
            }

            public bool IsVisible()
            {
                return _chunkObject.activeSelf;
            }
        }
    }
}


