using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SNESFromScratch.Rendering
{
    public class GDIRenderer : IRenderer
    {
        private PictureBox _box;

        public void RenderBuffer(int[] buffer)
        {
            const int width = 256;
            const int height = 224;
            var img = new Bitmap(width, height);
            var bmpRect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = img.LockBits(bmpRect, ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            Marshal.Copy(buffer, 0, bitmapData.Scan0, buffer.Length);
            img.UnlockBits(bitmapData);
            var old = _box.Image;
            _box.Image = img;
            old?.Dispose();
        }

        public void SetTargetControl(PictureBox box)
        {
            _box = box;
        }
    }
}