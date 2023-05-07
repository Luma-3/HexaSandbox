using UnityEngine;

namespace Map.Coordinate
{
    [System.Serializable]
    public struct ChunkCoordinates
    {
        [SerializeField]
        private int _x, _z;

        public int X => _x;
        public int Z => _z;
        

        public ChunkCoordinates(int x, int z)
        {
            _x = x;
            _z = z;
        }

        public static ChunkCoordinates Coord(int x, int z)
        {
            return new ChunkCoordinates(x, z);
        }
        
        public override string ToString()
        {
            return "(" + X.ToString() + ", "  + Z.ToString() + ")";
        }
    }
}