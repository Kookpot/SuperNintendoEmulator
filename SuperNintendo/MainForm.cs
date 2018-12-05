using System;
using System.Windows.Forms;
using SuperNintendo.Core.GFX;
using SuperNintendo.Core.Memory;

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

            //GFX.Pitch = EXT_PITCH;
            //GFX.RealPPL = EXT_PITCH;
            //GFX.Screen = (uint16*)(ScreenBuffer);

            //InitializeCriticalSection(&GUI.SoundCritSect);
            //GUI.SoundSyncEvent = CreateEvent(NULL, TRUE, TRUE, NULL);
            //CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);

            //S9xInitAPU();

            //WinDisplayReset();
            //ReInitSound();

            //if (GUI.FullScreen)
            //{
            //    GUI.FullScreen = false;
            //    ToggleFullScreen();
            //}

            //TIMECAPS tc;
            //if (timeGetDevCaps(&tc, sizeof(TIMECAPS)) == TIMERR_NOERROR)
            //{
            //    wSoundTimerRes = min(max(tc.wPeriodMin, 1), tc.wPeriodMax);
            //    timeBeginPeriod(wSoundTimerRes);
            //}
            //else
            //{
            //    wSoundTimerRes = 5;
            //    timeBeginPeriod(wSoundTimerRes);
            //}

            //QueryPerformanceFrequency((LARGE_INTEGER*)&PCBase);
            //QueryPerformanceCounter((LARGE_INTEGER*)&PCStart);
            //PCEnd = PCStart;
            //PCEndTicks = timeGetTime() * 1000;
            //PCStartTicks = timeGetTime() * 1000;
            //PCFrameTime = PCFrameTimeNTSC = (__int64)((float)PCBase / 59.948743718592964824120603015098f);
            //PCFrameTimePAL = PCBase / 50;


            //GUI.hFrameTimer = timeSetEvent(20, 0, (LPTIMECALLBACK)FrameTimer, 0, TIME_PERIODIC);

            //if (GUI.JoystickHotkeys || GUI.BackgroundInput)
            //    GUI.hHotkeyTimer = timeSetEvent(32, 0, (LPTIMECALLBACK)HotkeyTimer, 0, TIME_PERIODIC);
            //else
            //    GUI.hHotkeyTimer = 0;

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
                //InitSound();
                //ResetFrameTimer();
            }
        }
    }
}