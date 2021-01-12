using SNESFromScratch.SNESSystem;

namespace SNESFromScratch.PictureProcessing
{
    public interface IPPU : IHasAccessToSystem
    {
        void Reset();
        void Render(int line);
        void Write8(int address, int value);
        int Read8(int address);
        int IniDisp { get; }
        int Stat77 { get; set; }
        int Stat78 { get; set; }
        int OAMAddress { get; set; }
        int OAMReload { get; }
        int[] BackBuffer { get; }
        byte[] VRAM { get; }
        int OpHorizontalCounter { get; set; }
        int OpVerticalCounter { get; set; }
    }
}