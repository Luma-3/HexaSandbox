using UnityEngine;

namespace Script.Map
{
    [System.Serializable]
    public struct HexaCoordinates
    {
        [SerializeField]
        private int x, z;


        public int X
        {
            get { return x;}
        }

        public int Z
        {
            get { return z;}
        }

        public int Y
        {
            get
            {
                return -X - Z;
            }
        }

        public HexaCoordinates(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public static HexaCoordinates FromOffsetCoordinates(int x, int z)
        {
            return new HexaCoordinates(x - z / 2, z);
        }

        public override string ToString()
        {
            return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
        }

        public string ToStringOnSeparateLines()
        {
            return X.ToString()+ "\n" + Y.ToString() + "\n" + Z.ToString();
        }

    }
}


