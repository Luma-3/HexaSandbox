using TMPro;
using UnityEngine;

namespace Script.Map
{
    [RequireComponent(typeof(MeshFilter))]
    public class HexagonCell : MonoBehaviour
    {

        public HexaCoordinates coordinates;
        public TextMeshProUGUI label;

        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;

        public void GetComponent()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }
        public MeshRenderer RecoverMeshRenderer()
        {
            return _meshRenderer;
        }
    
        public MeshFilter RecoverMeshFilter()
        {
            return _meshFilter;
        }
    }
}
