using TMPro;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexagonCell : MonoBehaviour
{

    public HexaCoordinates coordinates;
    public TextMeshProUGUI label;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }
    public MeshRenderer RecoverComponent()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        return _meshRenderer;
    }
    
    
}
