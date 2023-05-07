using System.Collections.Generic;
using System.Linq;
using Map.Coordinate;
using Map.DataGen;
using Map.Management;
using Unity.Mathematics;
using UnityEngine;

namespace Map.Manager
{
    public class EndlessTerrain : MonoBehaviour
    {
        private const float MaxViewDst = 300f;
        public Transform viewer;
        public Chunk chunk;

        private static Vector2 _viewerPosition;
        private int _chunkSize;
        private int _chunkVisibleInViewDst;

        private readonly Dictionary<Vector2, TerrainChunk> _terrainChunkDic = new();
        private readonly List<TerrainChunk> _terrainChunkVisibleLastUpdate = new();
    
        //private static readonly Vector2 RealSize = new(110.8512517f, 96);
        //private static readonly Vector2 RealSize = new(55.42562586f, 48);
        private static readonly Vector2 RealSize = new(27.71281292f, 24);
        
        
        private static MapGenerator _mapGenerator;
        private static CellGenerator _cellGenerator;

        private void Awake()
        {
            _mapGenerator = GameManager.Instance.mapGenerator;
            _cellGenerator = GameManager.Instance.cellGenerator;
        }

        private void Start()
        {
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
            private readonly Chunk _chunk;
            private readonly ChunkCoordinates _coord;
            private Bounds _bounds;

            private MapData _mapData;

            public TerrainChunk(Vector2Int coord,Vector2 realSize, Transform parent, int chunkSize, Chunk chunkPrefab)
            {
                _coord = ChunkCoordinates.Coord(coord.x,coord.y);
                
                var position = coord * realSize - realSize / 2;
                _bounds = new Bounds(new Vector2(coord.x * realSize.x, coord.y * realSize.y), realSize);
                _mapGenerator.RequestMapData(new Vector2(coord.x * chunkSize,-coord.y * chunkSize ), OnMapDataReceived);
                
            
                Vector3 positionV3 = new(position.x, 0.0f, position.y);
                _chunk = Instantiate(chunkPrefab, positionV3, quaternion.identity, parent);
                _chunk.Coordinates = ChunkCoordinates.Coord(coord.x, coord.y);
                
                SetVisible(false);   
            }

            private void OnMapDataReceived(MapData data)
            {
                _mapGenerator.RequestCellsData(data, OnCellDataReceived);
                _mapData = data;
            }

            private void OnCellDataReceived(CellsData cellsData)
            {
                var cell = new HexagonCell[cellsData.cellNumber];
                
                
                for (var i = 0; i < cellsData.cellNumber; i++)
                {
                    cell[i] = _cellGenerator.CreateCell(_chunk.transform);
                    CalculateCellCoord(cellsData, i);
                    lock (cell[i].GetComponent())
                    {
                        _cellGenerator.ApplyData(_mapData, cellsData, cell[i], i);
                    }
                }
            }

            private void CalculateCellCoord(CellsData data, int count)
            {
                var cellCoord = data.Coords[count];
                data.Coords[count] = HexaCoordinates.FromChunkCoord(cellCoord.X, cellCoord.Z, _coord.X, _coord.Z);
            }

            public bool CheckDst()
            {
                var viewerDstFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(_viewerPosition));
                return viewerDstFromNearestEdge <= MaxViewDst;
            }

            public void SetVisible(bool visible)
            {
                _chunk.gameObject.SetActive(visible);
            }

            public bool IsVisible()
            {
                return _chunk.gameObject.activeSelf;
            }
        }
    }
}


