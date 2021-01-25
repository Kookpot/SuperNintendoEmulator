using System;
using KSNES.AudioProcessing;
using KSNES.CPU;
using KSNES.PictureProcessing;
using KSNES.Rendering;
using KSNES.ROM;

namespace KSNES.SNESSystem
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
        void LoadROM(string fileName);
        void Run();
        IRenderer Renderer { get; set; }
        IAudioHandler AudioHandler { get; set; }
        bool IsRunning();
        event EventHandler FrameRendered;
        bool PPULatch { get; }
        int OpenBus { get; }
        int XPos { get; }
        int YPos { get; }
    }
}