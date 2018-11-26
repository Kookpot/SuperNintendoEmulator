namespace SuperNintendo.Core
{
    public static class GUI
    {
        #region properties

        #endregion

        #region Public Methods

        public static ushort BuildPixel(uint R, uint G, uint B)
        {
            return (ushort) ((R << 10) + (G << 5) + B);
        }

        public static void SetUIColor(uint R, uint G, uint B)
        {
            //TODO
        }

        #endregion
    }
}