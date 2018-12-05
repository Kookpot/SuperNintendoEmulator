namespace SuperNintendo.Core.Screen
{
    public static class Screen
    {
        public static byte[] ScreenBuf = new byte[PPU.Constants.EXT_PITCH * PPU.Constants.EXT_HEIGHT_WITH_CENTERING];
        public static int ScreenBuffer = PPU.Constants.EXT_OFFSET_WITH_CENTERING;
    }
}