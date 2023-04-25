using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;

    public void DrawNoiseMap(float[,] noiseMap)
    {
        int witdh = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new(witdh, height);

        Color[] colourMap = new Color[witdh * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < witdh; x++)
            {
                colourMap[y * witdh + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }

        texture.SetPixels(colourMap);
        texture.Apply();


        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(witdh, 1, height);
    }
}
