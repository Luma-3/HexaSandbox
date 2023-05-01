using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;

    GameObject chunkObject;

    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawChunk(CellsData cellData, Material[] materials)
    {
        if (chunkObject != null) 
        { 
            DestroyImmediate(chunkObject);
        }
        chunkObject = new("Terrain Chunk");
        for (int i = 0; i < cellData._cells.Length; i++)
        {
            HexagonCell cell = cellData.CreateCell(i);
            cell.transform.parent = chunkObject.transform;

            cellData.SetPosition(i);
            MeshRenderer renderer = cellData._cells[i].RecoverComponent();
            renderer.sharedMaterial = materials[i];
        }
    }
}
