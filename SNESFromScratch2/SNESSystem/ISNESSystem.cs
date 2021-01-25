using System;
using System.Windows.Forms;
using SNESFromScratch2.AudioProcessing;
using SNESFromScratch2.CPU;
using SNESFromScratch2.PictureProcessing;
using SNESFromScratch2.Rendering;
using SNESFromScratch2.ROM;

namespace SNESFromScratch2.SNESSystem
{
    public interface ISNESSystem
    {
        int Read(int addr, bool dma = false);
        void Write(int addr, int value, bool dma = false);
        ISNESSystem Merge(ISNESSystem system);
        string FileName { get; set; }
        string GameName { get; set; }
        ICPU CPU { get; }
        IPPU PPU { get; }
        IAPU APU { get; }
        IROM ROM { get; set; }
        void StopEmulation();
        void ResumeEmulation();
        void SetKeyDown(SNESButton button);
        void SetKeyUp(SNESButton button);
        void LoadROM(string fileName, Control form);
        void Run(Control form);
        IRenderer Renderer { get; set; }
        IAudioHandler AudioHandler { get; set; }
        IFPS FPS { get; set; }
        bool IsRunning();
        event EventHandler FrameRendered;
        bool PPULatch { get; }
        int OpenBus { get; }
        int XPos { get; }
        int YPos { get; }
    }
}