namespace SuperNintendo.Core.IPPU
{
    public static class IPPU
    {
        public static byte[][] TileCache = new byte[7][] {
            new byte[Constants.MAX_2BIT_TILES * 64],
            new byte[Constants.MAX_4BIT_TILES * 64],
            new byte[Constants.MAX_8BIT_TILES * 64],
            new byte[Constants.MAX_2BIT_TILES * 64],
            new byte[Constants.MAX_2BIT_TILES * 64],
            new byte[Constants.MAX_4BIT_TILES * 64],
            new byte[Constants.MAX_4BIT_TILES * 64]
        };

        public static byte[][] TileCached = new byte[7][] {
            new byte[Constants.MAX_2BIT_TILES ],
            new byte[Constants.MAX_4BIT_TILES],
            new byte[Constants.MAX_8BIT_TILES],
            new byte[Constants.MAX_2BIT_TILES],
            new byte[Constants.MAX_2BIT_TILES],
            new byte[Constants.MAX_4BIT_TILES],
            new byte[Constants.MAX_4BIT_TILES]
        };
    }
}