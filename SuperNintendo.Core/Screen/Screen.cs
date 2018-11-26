namespace SuperNintendo.Core.Screen
{
    public static class Screen
    {
        public static byte[] ScreenBuf = new byte[Constants.EXT_PITCH * Constants.EXT_HEIGHT_WITH_CENTERING];
        public static int ScreenBuffer = Constants.EXT_OFFSET_WITH_CENTERING;
    }
}