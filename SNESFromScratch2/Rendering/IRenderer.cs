using System.Windows.Forms;

namespace SNESFromScratch2.Rendering
{
    public interface IRenderer
    {
        void RenderBuffer(int[] buffer);
        void SetTargetControl(PictureBox box);
    }
}