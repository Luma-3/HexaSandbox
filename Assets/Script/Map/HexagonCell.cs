using Map.Coordinate;
using UnityEngine;

namespace Map
{
    [RequireComponent(typeof(MeshFilter))]
    public class HexagonCell : MonoBehaviour
    {

        public HexaCoordinates coordinates;
        //public TextMeshProUGUI label;

        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;

        private void Awake()
        {
            GetComponent();
        }

        public MeshFilter GetComponent()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshCollider = GetComponent<MeshCollider>();
            return _meshFilter;
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
