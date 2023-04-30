using TMPro;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexagonCell : MonoBehaviour
{

    public HexaCoordinates coordinates;
    public TextMeshProUGUI label;

    public MeshRenderer RecoverComponent()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        return renderer;
    }
}
