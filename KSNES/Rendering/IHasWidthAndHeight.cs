using System;
using System.Drawing;

namespace KSNES.Rendering
{
    public interface IHasWidthAndHeight
    {
        IntPtr Handle { get; }
        int Width { get; }
        int Height { get; }
        Image Image { get; set; }
    }
}