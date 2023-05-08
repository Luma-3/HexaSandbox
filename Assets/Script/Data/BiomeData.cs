using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "ScriptableObject/CreateBiome", order = 1)]
    public class BiomeData : ScriptableObject
    {
        public new string name;
        public int id;
        public float amplificationHeight;
        public AnimationCurve heightCurve;
    }
}
