using System.Windows.Forms;

namespace SNESFromScratch.Rendering
{
    public interface IRenderer
    {
        void RenderBuffer(int[] buffer);
        void SetTargetControl(PictureBox control);
    }
}