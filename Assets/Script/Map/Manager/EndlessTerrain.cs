using System.Collections.Generic;
using System.Linq;
using Map.Coordinate;
using Map.DataGen;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace Map.Management
{
    public class EndlessTerrain : MonoBehaviour
    {
        private const float MaxViewDst = 300f;
        public Transform viewer;
        public Chunk chunk;

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
            ViewerPosToVector2();
            
            UpdateChunkVisibleLastFrame();
            
            UpdateVisibleChunk();
        }

        private void ViewerPosToVector2()
        {
            var position = viewer.position;
            _viewerPosition = new Vector2(position.x, position.z);
        }
        
        private void UpdateVisibleChunk()
        {
            for (var yOffset = -_chunkVisibleInViewDst; yOffset <= _chunkVisibleInViewDst; yOffset++)
            {
                for (var xOffset = -_chunkVisibleInViewDst; xOffset < _chunkVisibleInViewDst; xOffset++)
                {
                    var chunkCoord = CreateChunkCoord(xOffset, yOffset);
                    CheckChunk(chunkCoord);
                }
            }
        }

        private static Vector2Int CreateChunkCoord(int xOffset, int yOffset)
        {
            var currentChunkCoordX = Mathf.RoundToInt(_viewerPosition.x / RealSize.x);
            var currentChunkCoordY = Mathf.RoundToInt(_viewerPosition.y / RealSize.y);

            var viewedChunkCoord = new Vector2Int(currentChunkCoordX - xOffset, currentChunkCoordY - yOffset);

            return viewedChunkCoord;
        }
        

        private void UpdateChunkVisibleLastFrame()
        {
            foreach (var chunkVisibleLastUpdate in _terrainChunkVisibleLastUpdate.ToList().Where(chunkVisibleLastUpdate => !chunkVisibleLastUpdate.CheckDst()))
            {
                chunkVisibleLastUpdate.SetVisible(false);
                _terrainChunkVisibleLastUpdate.Remove(chunkVisibleLastUpdate);
            }
        }
        private void CheckChunk(Vector2Int chunkCoord)
        {
            if (_terrainChunkDic.ContainsKey(chunkCoord))
            {
                EnableDisableChunk(chunkCoord);
                
                if (_terrainChunkDic[chunkCoord].IsVisible() &&
                    !_terrainChunkVisibleLastUpdate.Contains(_terrainChunkDic[chunkCoord]))
                {
                    AddChunkToList(chunkCoord);
                }
            }
            else
            {
                AddChunkToDictionary(chunkCoord);
            }
        }

        private void EnableDisableChunk(Vector2Int chunkCoord)
        {
            _terrainChunkDic[chunkCoord].SetVisible(_terrainChunkDic[chunkCoord].CheckDst());
        }


        private void AddChunkToDictionary(Vector2Int chunkCoord)
        {
            _terrainChunkDic.Add(chunkCoord, new TerrainChunk(chunkCoord, RealSize, transform, _chunkSize, chunk));
        }

        private void AddChunkToList(Vector2Int chunkCoord)
        {
            _terrainChunkVisibleLastUpdate.Add(_terrainChunkDic[chunkCoord]);
        }

        

        private class TerrainChunk
        {
            private readonly Chunk _chunkObject;
            private Bounds _bounds;

            private MapData _mapData;

            public TerrainChunk(Vector2Int coord,Vector2 realSize, Transform parent, int chunkSize, Chunk chunkPrefab)
            {
                var position = coord * realSize - realSize / 2;
                _bounds = new Bounds(new Vector2(coord.x * realSize.x, coord.y * realSize.y), realSize);
                _mapGenerator.RequestMapData(new Vector2(coord.x * chunkSize,-coord.y * chunkSize ), OnMapDataReceived);
                
            
                Vector3 positionV3 = new(position.x, 0.0f, position.y);
                _chunkObject = Instantiate(chunkPrefab, positionV3, quaternion.identity, parent);
                _chunkObject.Coordinates = ChunkCoordinates.Coord(coord.x, coord.y);
                
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
                _chunkObject.gameObject.SetActive(visible);
            }

            public bool IsVisible()
            {
                return _chunkObject.gameObject.activeSelf;
            }
        }
    }
}


