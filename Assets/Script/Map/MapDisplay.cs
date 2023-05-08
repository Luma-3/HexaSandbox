using Map.DataGen;
using Map.Manager;
using UnityEngine;

namespace Map
{
    public class MapDisplay : MonoBehaviour
    {
        public Renderer textureRenderer;

        private GameObject _chunkObject;

        private CellGenerator _cellGenerator;

        private void Start()
        {
            _cellGenerator = GameManager.Instance.cellGenerator;
        }

        public void DrawTexture(Texture2D texture)
        {
            textureRenderer.sharedMaterial.mainTexture = texture;
            textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
        }

        public void DrawChunk(MapData mapData, CellsData cellsData)
        {
            if (_chunkObject) DestroyImmediate(_chunkObject);

            _chunkObject = new GameObject("Terrain Chunk");
            
            var cell = new HexagonCell[cellsData.CellNumber];
            for (var i = 0; i < cellsData.CellNumber; i++)
            {
                cell[i] = _cellGenerator.CreateCell(_chunkObject.transform);
                _cellGenerator.ApplyData(mapData, cellsData, cell[i], i);

            }
        }
    }
}
