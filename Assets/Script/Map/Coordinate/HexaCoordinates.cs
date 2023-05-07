using UnityEngine;

namespace Map.Coordinate
{
    [System.Serializable]
    public struct HexaCoordinates
    {
        [SerializeField]
        private int x, z;


        public int X => x;

        public int Z => z;

        public int Y => -x - z;

        public HexaCoordinates(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public static HexaCoordinates FromOffsetCoordinates(int x, int z)
        {
            return new HexaCoordinates(x - z /2,  z);
        }

        public static HexaCoordinates FromChunkCoord(int x, int z, int chunkX, int chunkZ)
        {
            return new HexaCoordinates(chunkX * 8 + x - 4 * chunkZ , chunkZ * 8 + z);
        }

        public override string ToString()
        {
            return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
        }

    }
}


