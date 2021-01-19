using System;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Factory = SharpDX.Direct2D1.Factory;

namespace SNESFromScratch.Rendering
{
    public class SharpDXRenderer : IRenderer
    {
        private WindowRenderTarget _renderTarget2D;
        private PictureBox _box;

        public void RenderBuffer(int[] buffer)
        {
            var widthOfBox = _box.Width;
            var heightOfBox = _box.Height;
            var newWidth = Math.Min(widthOfBox, heightOfBox * 256 / 224);
            var newHeight = newWidth * 224 / 256;
            var leftOffSet = (_box.Width - newWidth) / 2;
            var topOffSet = (_box.Height - newHeight) / 2;

            var d2dBitmap = Bitmap.New(_renderTarget2D, new Size2(256, 224), buffer, new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)));
            if (_renderTarget2D.Size.Width != _box.Width || _renderTarget2D.Size.Height != _box.Height)
            {
                _renderTarget2D.Resize(new Size2(_box.Width, _box.Height));
            }

            _renderTarget2D.BeginDraw();
            _renderTarget2D.AntialiasMode = AntialiasMode.Aliased;
            _renderTarget2D.DrawBitmap(d2dBitmap, new RawRectangleF(leftOffSet, topOffSet, leftOffSet + newWidth, topOffSet + newHeight),  1.0f, BitmapInterpolationMode.Linear);
            try
            {
                _renderTarget2D.EndDraw();
            }
            catch (SharpDXException ex) when ((uint)ex.HResult == 0x8899000C)
            {
            }
            d2dBitmap.Dispose();
        }

        public void SetTargetControl(PictureBox box)
        {
            _box = box;
            var factory2D = new Factory(FactoryType.MultiThreaded, DebugLevel.None);
            var properties = new HwndRenderTargetProperties
            {
                Hwnd = _box.Handle,
                PixelSize = new Size2(512, 448),
                PresentOptions = PresentOptions.None
            };
            _renderTarget2D = new WindowRenderTarget(factory2D,
                new RenderTargetProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)), properties)
            {
                AntialiasMode = AntialiasMode.PerPrimitive
            };
        }
    }
}