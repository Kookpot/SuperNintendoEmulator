using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SNESFromScratch.Rendering
{
    public class Renderer : IRenderer
    {
        private PictureBox Control;

        public void RenderBuffer(int[] buffer)
        {
            const int width = 256;
            const int height = 224;
            var img = new Bitmap(width, height);
            var bmpRect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = img.LockBits(bmpRect, ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            Marshal.Copy(buffer, 0, bitmapData.Scan0, buffer.Length);
            img.UnlockBits(bitmapData);
            var old = Control.Image;
            Control.Image = img;
            old?.Dispose();
        }

        public void SetTargetControl(PictureBox control)
        {
            Control = control;
        }
    }
}