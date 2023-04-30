using UnityEngine;

public class MapDisplay : MonoBehaviour             
{
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(Texture2D texture)
    {
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
