using TMPro;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("General")]
    public int width;
    public int height;

    [Header("Noise")]
    public float noiseScale;
    public int octaves;
    public float lacunarity;
    public float persistence;

    [Header("Hexagonal")]
    public GameObject hexPrefab;
    public TextMeshProUGUI labelHexPrefab;
    public float ampliHeight;

    private readonly float hexSize = 1f;
    Canvas gridCanvas;
    //public bool autoUpdate;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, noiseScale, octaves, lacunarity, persistence);

        MapDisplay display = FindAnyObjectByType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);

        GenerateGrid(noiseMap);
    }

    public void GenerateGrid(float[,] noiseMap)
    {
        Label();

        // Loop over each row and column to create a hexagon at the appropriate position
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Calculate the position of the hexagon
                float SampleX = x * hexSize * 1.5f;
                float SampleY = y * hexSize * Mathf.Sqrt(3) + (x % 2 == 0 ? 0 : hexSize * Mathf.Sqrt(3) / 2);
                Vector3 pos = new(SampleX, 0f, SampleY);

                // Create a new hexagon at this position
                GameObject hex = Instantiate(hexPrefab, pos, Quaternion.identity, transform);
                hex.transform.localScale = new Vector3(hexSize, hexSize, hexSize);
                HexagonGenarator hexScript = hex.GetComponent<HexagonGenarator>();
                hexScript.GenerateMesh();

                TextMeshProUGUI label = Instantiate(labelHexPrefab);
                label.rectTransform.SetParent(gridCanvas.transform, false);
                
                label.text = x.ToString() + "\n" + y.ToString();

                float hexHeight = noiseMap[x, y];
                hexScript.SetHeight(hexHeight * ampliHeight);

                label.rectTransform.localPosition =
                    new Vector3(pos.x, pos.z, -hexHeight);

            }
        }


    }

    public void Label()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
    }
}
