using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using KSNES.SNESSystem;
using KSNES.Rendering;

namespace KSNES.GUI
{
    public sealed partial class MainForm : Form
    {
        private readonly IFPS _fps;
        private ISNESSystem _system;
        private readonly IRenderer _renderer;
        private readonly IKeyMapper _keyMapper;
        private readonly OpenSaveFilesDialog _dialog;

        public MainForm(ISNESSystem system, IRenderer renderer, IFPS fps, IKeyMapper keyMapper, OpenSaveFilesDialog dialog)
        {
            Load += FrmMainLoad;
            KeyDown += FrmMainKeyDown;
            KeyUp += FrmMainKeyUp;
            Closed += AppExit;
            Closing += OnClosing;
            _fps = fps;
            _system = system;
            _system.FrameRendered += LockFPS;
            _renderer = renderer;
            _keyMapper = keyMapper;
            _dialog = dialog;
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
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
            InputToolStripMenuItem.Click += InputToolStripMenuItemClick;
        }

        private void FrmMainKeyDown(object sender, KeyEventArgs e)
        {
            KeyAction(e, _system.SetKeyDown);
        }

        private void FrmMainKeyUp(object sender, KeyEventArgs e)
        {
            KeyAction(e, _system.SetKeyUp);
        }

        private void KeyAction(KeyEventArgs e, Action<SNESButton> act)
        {
            var snesButton = _keyMapper.Map(e.KeyCode);
            if (snesButton.HasValue)
            {
                act(snesButton.Value);
            }
        }

        private void OpenROMToolStripMenuItemClick(object sender, EventArgs e)
        {
            _system.StopEmulation();
            var fileName = string.Empty;
            using (var openFileDialog = new OpenFileDialog { Title = "Open ROM of Super Nintendo", Filter = "ROM SMC/SFC|*.smc;*.sfc" })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(openFileDialog.FileName))
                    {
                        fileName = openFileDialog.FileName;
                    }
                }
                else
                {
                    _system.ResumeEmulation();
                }
            }
            if (!string.IsNullOrEmpty(fileName))
            {
                _system.LoadROM(fileName);
            }
        }
        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }

        private void InputToolStripMenuItemClick(object sender, EventArgs e)
        {
            _system.StopEmulation();
            var popup = new InputChangerForm(_keyMapper);
            popup.ShowDialog(this);
            _system.ResumeEmulation();
        }

        private void SaveGamePositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_system.IsRunning())
            {
                _system.FrameRendered += SerializeSNES;
            }
        }

        private void SerializeSNES(object sender, EventArgs arg)
        {
            _system.StopEmulation();
            _dialog.SelectOnly = true;
            _dialog.SaveSNESSystem = _system;
            _dialog.ShowDialog();
            _system.FrameRendered -= SerializeSNES;
            _system.ResumeEmulation();
        }

        private void LoadGamePositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _system.StopEmulation();
            _dialog.SelectOnly = false;
            _dialog.SaveSNESSystem = null;
            var result = _dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                _system.FrameRendered -= LockFPS;
                _system =_system.Merge(_dialog.SNESSystem);
                _system.FrameRendered += LockFPS;
                _system?.Run();
            }
            else
            {
                _system.ResumeEmulation();
            }
        }

        public void LockFPS(object sender, EventArgs arg)
        {
            Application.DoEvents();
            Text = _fps.GetFPS();
            Application.DoEvents();
        }
    }
}