using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SNESFromScratch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Load += FrmMainLoad;
            Closed += AppExit;
        }

        private void AppExit(object obj, EventArgs args)
        {
            _c65816.SnesOn = false;
            Application.Exit();
        }

        private ROM _rom;
        private FPS _fps;
        private IOPort _ioPort;
        private PPU _ppu;
        private SPU _spu;
        private C65816 _c65816;

        private void FrmMainLoad(Object sender, EventArgs e)
        {
            InitializeComponent();
            MMenu.Renderer = new CustomMenu();
            _fps = new FPS();
            _fps.HiResTimerInitialize();


            Show();
            _rom = new ROM();
            _ioPort = new IOPort();
            _ppu = new PPU();
            _spu = new SPU();
            _c65816 = new C65816(_rom, _ppu, _spu, _ioPort, _fps, Application.StartupPath, this);
            _ioPort.C65816 = _c65816;
            _ppu.C65816 = _c65816;
            _ppu.ROM = _rom;
            _ppu.FrmMain = this;
            _spu.C65816 = _c65816;

            OpenROMToolStripMenuItem.Click += OpenROMToolStripMenuItemClick;
            ScreenshotToolStripMenuItem.Click += ScreenshotToolStripMenuItemClick;
            ExitToolStripMenuItem.Click += ExitToolStripMenuItemClick;
            LimitFPSToolStripMenuItem.Click += LimitFPSToolStripMenuItemClick;
            DebugToolStripMenuItem.Click += DebugToolStripMenuItemClick;
            DumpVRAMToolStripMenuItem.Click += DumpVRAMToolStripMenuItemClick;
            DumpObjRAMToolStripMenuItem.Click += DumpObjRAMToolStripMenuItemClick;
        }

        private void OpenROMToolStripMenuItemClick(Object sender, EventArgs e)
        {
            var openDlg = new OpenFileDialog {Title = "Open ROM of Super Nintendo", Filter = "ROM SMC|*.smc"};
            if (openDlg.ShowDialog() != DialogResult.OK) return;
            if (File.Exists(openDlg.FileName))
            {
                _rom.LoadRom(openDlg.FileName);
                _c65816.Reset65816();
                _ppu.ResetPPU();
                _ioPort.ResetIO();
                _c65816.SnesOn = true;
                _c65816.MainLoop();
            }
        }

        private void ScreenshotToolStripMenuItemClick(Object sender, EventArgs e)
        {
            _ppu.TakeScreenshot = true;
        }

        private void ExitToolStripMenuItemClick(Object sender, EventArgs e)
        {
            Close();
        }

        private void LimitFPSToolStripMenuItemClick(Object sender, EventArgs e)
        {
            _fps.LimitFPS = !_fps.LimitFPS;
            LimitFPSToolStripMenuItem.Checked = _fps.LimitFPS;
        }

        private void DebugToolStripMenuItemClick(Object sender, EventArgs e)
        {
            _c65816.Debug = !_c65816.Debug;
            DebugToolStripMenuItem.Checked = _c65816.Debug;
        }

        private void DumpVRAMToolStripMenuItemClick(Object sender, EventArgs e)
        {
            var saveDlg = new SaveFileDialog {Title = "Save VRAM", Filter = "Bin|*.bin"};
            if (saveDlg.ShowDialog() != DialogResult.OK) return;
            File.WriteAllBytes(saveDlg.FileName, _ppu.VRAM);
        }

        private void DumpObjRAMToolStripMenuItemClick(Object sender, EventArgs e)
        {
            var saveDlg = new SaveFileDialog { Title = "Save ObjRAM", Filter = "Bin|*.bin" };
            if (saveDlg.ShowDialog() != DialogResult.OK) return;
            File.WriteAllBytes(saveDlg.FileName, _ppu.ObjRAM);
        }
    }
}
