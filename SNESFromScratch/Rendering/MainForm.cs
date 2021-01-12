using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using SNESFromScratch.SNESSystem;

namespace SNESFromScratch.Rendering
{
    public partial class MainForm : Form
    {
        private readonly IFPS _fps;
        private readonly ISNESSystem _system;
        private readonly IRenderer _renderer;

        public MainForm(ISNESSystem system, IRenderer renderer, IFPS fps)
        {
            Load += FrmMainLoad;
            KeyDown += FrmMainKeyDown;
            KeyUp += FrmMainKeyUp;
            Closed += AppExit;
            Closing += OnClosing;
            _fps = fps;
            _system = system;
            _renderer = renderer;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            _system.StopEmulation();
        }

        private static void AppExit(object sender, EventArgs args)
        {
            Application.Exit();
        }

        private void FrmMainLoad(object sender, EventArgs e)
        {
            InitializeComponent();
            _fps.HiResTimerInitialize();

            Show();

            _renderer.SetTargetControl(PicScreen);

            OpenROMToolStripMenuItem.Click += OpenROMToolStripMenuItemClick;
            ExitToolStripMenuItem.Click += ExitToolStripMenuItemClick;
            DumpVRAMToolStripMenuItem.Click += DumpVRAMToolStripMenuItemClick;
        }

        private void FrmMainKeyDown(object sender, KeyEventArgs e)
        {
            KeyAction(e, _system.SetKeyDown);
        }

        private void FrmMainKeyUp(object sender, KeyEventArgs e)
        {
            KeyAction(e, _system.SetKeyUp);
        }

        private static void KeyAction(KeyEventArgs e, Action<SNESButton> act)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    act(SNESButton.Up);
                    break;
                case Keys.Down:
                    act(SNESButton.Down);
                    break;
                case Keys.Left:
                    act(SNESButton.Left);
                    break;
                case Keys.Right:
                    act(SNESButton.Right);
                    break;
                case Keys.Return:
                    act(SNESButton.Start);
                    break;
                case Keys.Tab:
                    act(SNESButton.Sel);
                    break;
                case Keys.S:
                    act(SNESButton.Y);
                    break;
                case Keys.X:
                    act(SNESButton.B);
                    break;
                case Keys.D:
                    act(SNESButton.X);
                    break;
                case Keys.C:
                    act(SNESButton.A);
                    break;
                case Keys.Q:
                    act(SNESButton.L);
                    break;
                case Keys.W:
                    act(SNESButton.R);
                    break;
            }
        }

        private void OpenROMToolStripMenuItemClick(object sender, EventArgs e)
        {
            var fileName = string.Empty;
            using (var openFileDialog = new OpenFileDialog { Title = "Open ROM of Super Nintendo", Filter = "ROM SMC|*.smc|SFC|*.sfc" })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(openFileDialog.FileName))
                    {
                        fileName = openFileDialog.FileName;
                    }
                }
            }
            if (!string.IsNullOrEmpty(fileName))
            {
                _system.LoadROM(fileName, this);
            }
        }
        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }

        private void DumpVRAMToolStripMenuItemClick(object sender, EventArgs e)
        {
            File.WriteAllBytes(@"D:\vramnew.bin", _system.PPU.VRAM);
        }
    }
}