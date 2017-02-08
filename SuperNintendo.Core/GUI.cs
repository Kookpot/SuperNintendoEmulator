using System;
using System.Drawing;

namespace SuperNintendo.Core
{
    /// <summary>
    /// gui
    /// </summary>
    public static class GUI
    {
        #region properties

        public static Color InfoColor {get;set;}

        #endregion

        #region Public Methods

        /// <summary>
        /// build pixel
        /// </summary>
        /// <param name="intR">red</param>
        /// <param name="intG">green</param>
        /// <param name="intB">blue</param>
        /// <returns></returns>
        public static UInt16 BUILDPIXEL(UInt32 intR, UInt32 intG, UInt32 intB)
        {
            return (UInt16) intR << 10 + intG << 5 + intB;
        }

        #endregion
    }
}
