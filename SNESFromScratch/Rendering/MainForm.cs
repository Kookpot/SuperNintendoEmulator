using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using SNESFromScratch.SNESSystem;

namespace SNESFromScratch.Rendering
{
    public sealed partial class MainForm : Form
    {
        private readonly IFPS _fps;
        private readonly ISNESSystem _system;
        private readonly IRenderer _renderer;
        private readonly IKeyMapper _keyMapper;
        private readonly ISystemManager _systemManager;

        public MainForm(ISNESSystem system, IRenderer renderer, IFPS fps, IKeyMapper keyMapper, ISystemManager systemManager)
        {
            Load += FrmMainLoad;
            KeyDown += FrmMainKeyDown;
            KeyUp += FrmMainKeyUp;
            Closed += AppExit;
            Closing += OnClosing;
            _fps = fps;
            _system = system;
            _renderer = renderer;
            _keyMapper = keyMapper;
            _systemManager = systemManager;
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

        private void InputToolStripMenuItemClick(object sender, EventArgs e)
        {
            var popup = new InputChangerForm(_keyMapper);
            popup.ShowDialog(this);
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
            var dialog = new OpenSaveFilesDialog { SelectOnly = true, SaveSNESSystem = _system };
            dialog.ShowDialog();
            _system.FrameRendered -= SerializeSNES;
            _system.ResumeEmulation();
        }

        private void LoadGamePositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _system.StopEmulation();
            var dialog = new OpenSaveFilesDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                _system.Merge(dialog.SNESSystem);
                _system?.Run(this);
            }
        }
    }
}