using Map;
using Map.DataGen;
using Map.Management;
using Map.Manager;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public MapGenerator mapGenerator;
    public EndlessTerrain endlessTerrain;
    public CellGenerator cellGenerator;
    private void Awake()
    {
        if(Instance != null) Destroy(Instance.gameObject);
        Instance = this;
    }
}
