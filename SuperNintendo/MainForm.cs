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
                                CPU.DoHEventProcessing();
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
                            CPU.DoHEventProcessing();
                    }

                    if (!CPU.CheckFlag(Core.CPU.Constants.IRQ))
                    {
                        /* The flag pushed onto the stack is the new value */
                        CheckForIRQChange();
                        CPU.OpcodeIRQ();
                    }
                }

                /* Change IRQ flag for instructions that set it only on last cycle */
                CheckForIRQChange();

                if ((CPUState.Flags & Core.CPU.Constants.SCAN_KEYS_FLAG) > 0)
                    break;

                byte op;
                Action[] Opcodes;

		        if (CPUState.PCBase != null)
		        {
			        op = CPUState.PCBase[Registers.PCw];
			        CPUState.Cycles += CPUState.MemSpeed;
			        Opcodes = ICPU.Opcodes;
		        }
		        else
		        {
			        op = Memory.GetByte(Registers.PBPC);
                    Memory.OpenBus = op;
			        Opcodes = CPU.OpcodesSlow;
		        }

		        if ((Registers.PCw & Core.Memory.Constants.MASK) + ICPU.OpLengths[op] >= Core.Memory.Constants.BLOCK_SIZE)
		        {
			        MappingData oldPCBase = CPUState.PCBase;

                    CPUState.PCBase = new MappingData { MemoryLink = Memory.GetBasePointer(ICPU.ShiftedPB + ((ushort)(Registers.PCw + 4))) };
			        if (oldPCBase != CPUState.PCBase || (Registers.PCw & ~Core.Memory.Constants.MASK) == (0xffff & ~Core.Memory.Constants.MASK))
				        Opcodes = CPU.OpcodesSlow;
		        }

		        Registers.PCw++;
		        Opcodes[op]();
                Application.DoEvents();
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