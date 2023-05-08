using Map.DataGen;
using Map.Manager;
using UnityEngine;

namespace Map
{
    public class CellGenerator : MonoBehaviour
    {
        [SerializeField] private HexagonCell cellPrefab;
        
        public HexagonCell CreateCell(Transform parent)
        {
            return Instantiate(cellPrefab, parent);
        }

        public void ApplyData(MapData mapData, CellsData cellsData, HexagonCell cell, int count)
        {
            var cellTransform = cell.transform;
            cellTransform.localPosition = cellsData.Positions[count];
            cellTransform.localScale = cellsData.Scales[count];
            cell.coordinates = cellsData.Coords[count];

            cell.RecoverMeshRenderer().sharedMaterial = mapData.MaterialMap[count];

        }

        
    }
}