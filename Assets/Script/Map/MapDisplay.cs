using Script.Map.DataGen;
using UnityEngine;

namespace Script.Map
{
    public class MapDisplay : MonoBehaviour
    {
        public Renderer textureRenderer;

        private GameObject _chunkObject;

        public void DrawTexture(Texture2D texture)
        {
            textureRenderer.sharedMaterial.mainTexture = texture;
            textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
        }

        public void DrawChunk(CellsData cellData, Material[] materials)
        {
            if (_chunkObject) 
            { 
                DestroyImmediate(_chunkObject);
            }
            _chunkObject = new GameObject("Terrain Chunk");
            for (var i = 0; i < cellData.Cells.Length; i++)
            {
                var cell = cellData.CreateCell(i, _chunkObject.transform);

                cellData.SetPosition(i);
                var meshRenderer = cell.RecoverMeshRenderer();
                meshRenderer.sharedMaterial = materials[i];
            
            }
        }
    }
}
