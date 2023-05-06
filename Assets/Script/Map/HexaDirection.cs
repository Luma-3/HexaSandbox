namespace Script.Map
{
    public enum HexaDirection
    {
        NE, E, SE, SW, W, NW
    }

    public static class HexadirectionExtensions
    {
        public static HexaDirection Opposite(this HexaDirection direction)
        {
            return (int)direction < 3 ? (direction + 3) : (direction - 3);
        }
    }
}