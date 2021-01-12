using System;
using System.Windows.Forms;
using SNESFromScratch.AudioProcessing;
using SNESFromScratch.CentralMemory;
using SNESFromScratch.CPU;
using SNESFromScratch.IO;
using SNESFromScratch.PictureProcessing;
using SNESFromScratch.Rendering;
using SNESFromScratch.ROM;

namespace SNESFromScratch.SNESSystem
{
    public class SNESSystem : ISNESSystem
    {
        public ICPU CPU { get; }
        public IIO IO { get; }
        public IROM ROM { get; }
        public IPPU PPU { get; }
        public ISPC700 APU { get; }
        public IRenderer Renderer { get; }
        public IDMA DMA { get; }
        public int ScanLine { get; private set; }
        public int PPUDot { get; private set;  }

        private bool _isExecuting;
        private readonly IFPS _fps;
        private const double APUCyclesPerLine = 65.0;   //1.024 MHz Clock
        private const double CPUCyclesPerLine = 1364.0; //21.477 MHz Clock

        public SNESSystem(IROM rom, ICPU cpu, IDMA dma, IPPU ppu, IIO ioPort, IRenderer renderer, IFPS fps, ISPC700 spc700)
        {
            ROM = rom;
            CPU = cpu;
            APU = spc700;
            CPU.SetSystem(this);
            IO = ioPort;
            IO.SetSystem(this);
            PPU = ppu;
            PPU.SetSystem(this);
            Renderer = renderer;
            _fps = fps;
            DMA = dma;
            DMA.SetSystem(this);
        }

        public void SetKeyDown(SNESButton button)
        {
            IO.Joy1 |= (int) button;
        }

        public void SetKeyUp(SNESButton button)
        {
            IO.Joy1 &= ~(int) button;
        }

        public void LoadROM(string fileName, Control form)
        {
            ROM.LoadRom(fileName);
            if (ROM.IsPAL())
            {
                PPU.Stat78 = 0x11;
            }
            APU.Reset();
            IO.Reset();
            PPU.Reset();
            CPU.Reset();
            Run(form);
        }

        public void StopEmulation()
        {
            _isExecuting = false;
        }

        private void Run(Control form)
        {
            _isExecuting = true;
            while (_isExecuting)
            {
                IO.NMIRead &= ~0x80;
                IO.HVBJoy &= ~0x80;
                if ((PPU.IniDisp & 0x80) == 0)
                {
                    PPU.Stat77 &= ~0x80;
                }
                DMA.HDMAReset();
                for (ScanLine = 0; ScanLine <= 261; ScanLine++)
                {
                    IO.HVBJoy &= ~0x40;
                    if (ScanLine == 225)
                    {
                        if ((IO.NMITimeEnabled & 0x80) != 0)
                        {
                            CPU.DoNMI();
                        }
                        IO.NMIRead |= 0x80;
                        IO.HVBJoy |= 0x80;
                        if ((PPU.IniDisp & 0x80) == 0)
                        {
                            PPU.OAMAddress = PPU.OAMReload;
                        }

                        if ((PPU.Stat78 & 0x80) != 0)
                        {
                            PPU.Stat78 &= ~0x80;
                        }
                        else
                        {
                            PPU.Stat78 |= 0x80;
                        }
                    }
                    if (ScanLine == IO.VerticalTime & IO.HVIRQ == 2)
                    {
                        IO.TimeUp |= 0x80;
                    }
                    switch (ScanLine)
                    {
                        case 226:
                            IO.HVBJoy |= 1;
                            break;
                        case 229:
                            IO.HVBJoy &= ~1;
                            break;
                    }
                    if (ScanLine > 0 & ScanLine < 225)
                    {
                        PPU.Render(ScanLine);
                    }
                    while (CPU.Cycles < CPUCyclesPerLine)
                    {
                        CPU.IRQPending = (IO.TimeUp & 0x80) != 0;
                        if (IO.MDMAEnabled != 0)
                        {
                            DMA.DMATransfer();
                        }
                        else
                        {
                            CPU.ExecuteStep();
                        }
                        APU.Execute((int) Math.Round(CPU.Cycles / CPUCyclesPerLine * APUCyclesPerLine));
                        int oldDot = PPUDot;
                        PPUDot = CPU.Cycles >> 2;
                        if (InRange(oldDot, PPUDot, 274))
                        {
                            if (ScanLine < 224)
                            {
                                DMA.HDMATransfer();
                            }
                            IO.HVBJoy |= 0x40;
                        }
                        if (InRange(oldDot, PPUDot, IO.HorizontalTime))
                        {
                            bool hvIRQ = ScanLine == IO.VerticalTime && IO.HVIRQ == 3;
                            if (hvIRQ || IO.HVIRQ == 1)
                            {
                                IO.TimeUp |= 0x80;
                            }
                        }
                    }
                    APU.Cycles -= (int) APUCyclesPerLine;
                    CPU.Cycles -= (int) CPUCyclesPerLine;
                }
                Renderer.RenderBuffer(PPU.BackBuffer);
                _fps.LockFramerate();
                form.Text = _fps.GetFPS();
                Application.DoEvents();
            }
        }

        private static bool InRange(int min, int max, int value)
        {
            if (min > max)
            {
                return value > min || value <= max;
            }
            return min < value && max >= value;
        }
    }
}