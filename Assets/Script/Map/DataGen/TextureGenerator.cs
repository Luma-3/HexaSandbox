using UnityEngine;

namespace Map.DataGen
{
    public static class TextureGenerator{

        public static Texture2D TextureFromColour(Color[] colourMap, int width, int height)
        {
            Texture2D texture = new(width, height)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            texture.SetPixels(colourMap);
            texture.Apply();
            return texture;
        }

        public static Texture2D TextureFromHeightMap(float[,] heightMap)
        {
            var width = heightMap.GetLength(0);
            var height = heightMap.GetLength(1);

            var colourMap = new Color[width * height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
                }
            }

            return TextureFromColour(colourMap, width, height);
        }
        
    }
}
