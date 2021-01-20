using System;
using System.Windows.Forms;
using SNESFromScratch.AudioProcessing;
using SNESFromScratch.CentralMemory;
using SNESFromScratch.CPU;
using SNESFromScratch.IO;
using SNESFromScratch.PictureProcessing;
using SNESFromScratch.ROM;

namespace SNESFromScratch.SNESSystem
{
    public interface ISNESSystem
    {
        void LoadROM(string fileName, Control form);
        ICPU CPU { get; }
        IIO IO { get; }
        ISPC700 APU { get; }
        IROM ROM { get; }
        IPPU PPU { get; }
        IDMA DMA { get; }
        void StopEmulation();
        void ResumeEmulation();
        void SetKeyDown(SNESButton button);
        void SetKeyUp(SNESButton button);
        int ScanLine { get; }
        int PPUDot { get; }
        void Run(Control form);
        bool IsRunning();
        void Merge(ISNESSystem system);
        event EventHandler FrameRendered;
        string FileName { get; set; }
        string GameName { get; set; }
    }
}