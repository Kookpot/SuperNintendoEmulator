namespace KSNES.Rendering
{
    public interface IRenderer
    {
        void RenderBuffer(int[] buffer);
        void SetTargetControl(IHasWidthAndHeight box);
    }
}