using System;
using System.Windows.Forms;
using SuperNintendo.Core;
using SuperNintendo.Core.CPU;
using SuperNintendo.Core.GFX;
using SuperNintendo.Core.Memory;
using SuperNintendo.Core.Sound;
using SuperNintendo.Core.Timings;

namespace SuperNintendo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            InitSnes();
        }

        private void InitSnes()
        {
            //GUI.ControlForced = 0xff;
            Memory.Init();
            Memory.PostRomInitFunc = PostRomInit;

            Core.Sound.Sound.ReInitSound();

            //wSoundTimerRes = min(max(tc.wPeriodMin, 1), tc.wPeriodMax);

            //QueryPerformanceFrequency((LARGE_INTEGER*)&PCBase);
            //QueryPerformanceCounter((LARGE_INTEGER*)&PCStart);
            //PCEnd = PCStart;
            //PCEndTicks = timeGetTime() * 1000;
            //PCStartTicks = timeGetTime() * 1000;
            //PCFrameTime = PCFrameTimeNTSC = (__int64)((float)PCBase / 59.948743718592964824120603015098f);
            //PCFrameTimePAL = PCBase / 50;
            Settings.StopEmulation = true;

            //GUI.hFrameTimer = timeSetEvent(20, 0, (LPTIMECALLBACK)FrameTimer, 0, TIME_PERIODIC);

            //GUI.FrameTimerSemaphore = CreateSemaphore(NULL, 0, 10, NULL);
            //GUI.ServerTimerSemaphore = CreateSemaphore(NULL, 0, 10, NULL);

            //SetupDefaultKeymap();

            //DWORD lastTime = timeGetTime();

            //MSG msg;

            //game loop
            //while (true)
            //{
            //    if (run_loop)
            //    {
            //        ProcessInput();
            //        S9xMainLoop();
            //        GUI.FrameCount++;
            //    }
            //}

            //loop_exit:

            //Settings.StopEmulation = true;

            //// stop sound playback
            //CloseSoundDevice();
        }

        private void PostRomInit()
        {
            GFX.Screen = new ushort[Core.PPU.IPPU.RenderedScreenHeight]; //might be reference....
        }

        private void OpenToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            if (openRomFileDialog.ShowDialog() == DialogResult.OK)
            {
                openToolStripMenuItem.Enabled = false;
                Memory.LoadROM(openRomFileDialog.OpenFile());
                Sound.ReInitSound();
                ResetFrameTimer();
                var file = new System.IO.FileInfo(openRomFileDialog.FileName);
                var fileNameRAM = $"{file.Name}.srm";
                Memory.LoadSRAM(fileNameRAM);
                MainLoop();
            }
        }

        private void CheckForIRQChange()
        { 
	        if (Timings.IRQFlagChanging > 0)
	        {
		        if (Timings.IRQFlagChanging == IRQ.IRQ_CLEAR_FLAG)
			        CPU.ClearIRQ();
		        else if (Timings.IRQFlagChanging == IRQ.IRQ_SET_FLAG)
                    CPU.SetIRQ();
                Timings.IRQFlagChanging = IRQ.IRQ_NONE;
	        }
        }

        private void MainLoop()
        {
            while(true)
            {
                if (CPUState.NMIPending)
                {
                    if (Timings.NMITriggerPos <= CPUState.Cycles)
                    {
                        CPUState.NMIPending = false;
                        Timings.NMITriggerPos = 0xffff;
                        if (CPUState.WaitingForInterrupt)
                        {
                            CPUState.WaitingForInterrupt = false;
                            Registers.PCw++;
                            CPUState.Cycles += Core.CPU.Constants.TWO_CYCLES + Core.Memory.Constants.ONE_DOT_CYCLE / 2;
                            while (CPUState.Cycles >= CPUState.NextEvent)
                                DoHEventProcessing();
                        }

                        CheckForIRQChange();
                        CPU.OpcodeNMI();
                    }
                }

                if (CPUState.Cycles >= Timings.NextIRQTimer)
                {

                    Core.PPU.SPPU.UpdateIRQPositions(false);
                    CPUState.IRQLine = true;
                }

                if (CPUState.IRQLine || CPUState.IRQExternal)
                {
                    if (CPUState.WaitingForInterrupt)
                    {
                        CPUState.WaitingForInterrupt = false;
                        Registers.PCw++;
                        CPUState.Cycles += Core.CPU.Constants.TWO_CYCLES + Core.Memory.Constants.ONE_DOT_CYCLE / 2;
                        while (CPUState.Cycles >= CPUState.NextEvent)
                            DoHEventProcessing();
                    }

                    if (!CPU.CheckFlag(IRQ))
                    {
                        /* The flag pushed onto the stack is the new value */
                        CheckForIRQChange();
                        CPU.OpcodeIRQ();
                    }
                }

                /* Change IRQ flag for instructions that set it only on last cycle */
                CheckForIRQChange();

                VBlank = false;
                for (var scanline = 0; scanline <= 261; scanline++)
                {
                    CurrentLine = scanline;
                    HBlank = false;
                    if (!_waiDisable && !STPDisable)
                    {
                        if (_ioPort.IRQEnable == 2 && scanline == _ioPort.VCount)
                        {
                            IRQ();
                        }
                        Execute65816(CyclesPerScanline - HBlankCycles);
                        HBlank = true;
                        _ioPort.HBlankDMA(scanline);
                        if ((_ioPort.IRQEnable == 3 && scanline == _ioPort.VCount) || (_ioPort.IRQEnable == 1))
                        {
                            IRQ();
                        }
                        Execute65816(HBlankCycles);
                    }
                    if (scanline < 224)
                    {
                        _ppu.RenderScanline(scanline);
                    }
                    else
                    {
                        switch (scanline)
                        {
                            case 224:
                                _ioPort.ControllerReady = true;
                                _ppu.ObjRAMAddress = _ppu.ObjRAMFirstAddress;
                                VBlank = true;
                                if (_ioPort.NMIEnable) { NMI(); }
                                break;
                            case 227:
                                _ioPort.ControllerReady = false;
                                break;
                        }
                    }
                }
                _ppu.Blit();
                if (_fps.LimitFPS) { _fps.LockFramerate(60); }
                _form.Text = _fps.GetFPS();
                System.Windows.Forms.Application.DoEvents();
            }
        }

        private void ResetFrameTimer()
        {
            //QueryPerformanceCounter((LARGE_INTEGER*)&PCStart);
            //PCStartTicks = timeGetTime() * 1000;
            //PCFrameTime = PCFrameTimeNTSC;

            //// determines if we can do sound sync
            //Settings.FrameTime == Settings.FrameTimeNTSC;

            //if (GUI.hFrameTimer)
            //    timeKillEvent(GUI.hFrameTimer);

            //GUI.hFrameTimer = timeSetEvent((Settings.FrameTime + 500) / 1000, 0, (LPTIMECALLBACK)FrameTimer, 0, TIME_PERIODIC);
        }
    }
}