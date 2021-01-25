using KSNES.SNESSystem;

namespace KSNES.PictureProcessing
{
    public interface IPPU : IHasAccessToSystem
    {
        void CheckOverscan(int line);
        void RenderLine(int line);
        int Read(int adr);
        void Write(int adr, int value);
        int[] GetPixels();
        bool FrameOverscan { get; }
        int LatchedHpos { get; set; }
        int LatchedVpos { get; set; }
        bool CountersLatched { get; set; }
        void Reset();
    }
}