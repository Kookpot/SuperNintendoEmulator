namespace SuperNintendo.Core.PPU
{
    public static class IPPU
    {
        public static ClipData[,] Clip = new ClipData[2,6];
        public static bool ColorsChanged;
        public static bool OBJChanged;
        public static bool Interlace;
        public static bool InterlaceOBJ;
        public static bool PseudoHires;
        public static bool DoubleWidthPixels;
        public static bool DoubleHeightPixels;
        public static int CurrentLine;
        public static int PreviousLine;
        public static byte[] XB;
        public static uint[] Red = new uint[256];
        public static uint[] Green = new uint[256];
        public static uint[] Blue = new uint[256];
        public static ushort[] ScreenColors = new ushort[256];
        public static byte MaxBrightness;
        public static bool RenderThisFrame;
        public static int RenderedScreenWidth;
        public static uint FrameCount;
        public static uint RenderedFramesCount;
        public static uint DisplayedRenderedFrameCount;
        public static uint SkippedFrames;
        public static uint FrameSkip;
        public static uint TotalEmulatedFrames;
        public static int RenderedScreenHeight;

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
            new byte[Constants.MAX_2BIT_TILES],
            new byte[Constants.MAX_4BIT_TILES],
            new byte[Constants.MAX_8BIT_TILES],
            new byte[Constants.MAX_2BIT_TILES],
            new byte[Constants.MAX_2BIT_TILES],
            new byte[Constants.MAX_4BIT_TILES],
            new byte[Constants.MAX_4BIT_TILES]
        };

        public static void FlushRedraw()
        {
            if (PreviousLine != CurrentLine)
                UpdateScreen();
        }

        private static void UpdateScreen()
        {

        }
    }
}