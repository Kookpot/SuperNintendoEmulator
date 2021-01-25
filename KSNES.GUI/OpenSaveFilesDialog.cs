using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KSNES.Rendering;
using Newtonsoft.Json;
using KSNES.SNESSystem;

namespace KSNES.GUI
{
    public partial class OpenSaveFilesDialog : Form
    {
        private class SaveFileInfo
        {
            public string Name;
            public string GameName;
            public DateTime DateTime;
            public string Path;

            public override string ToString()
            {
                return $"{GameName} : {Name}";
            }
        }

        private List<SaveFileInfo> _lst = new List<SaveFileInfo>();

        private readonly IRenderer _renderer;

        private SaveFileInfo _selectedObject;

        public ISNESSystem SNESSystem { get; private set; }

        public ISNESSystem SaveSNESSystem { get; set; }
        private readonly ISystemManager _systemManager;

        private bool _selectOnly;
        public bool SelectOnly
        {
            get => _selectOnly;
            set
            {
                _selectOnly = value;
                if (_selectOnly)
                {
                    LoadButton.Text = "Overwrite";
                    AddButton.Visible = true;
                }
            }
        }

        public OpenSaveFilesDialog(ISystemManager systemManager)
        {
            DialogResult = DialogResult.None;
            _renderer = new GDIRenderer();
            _systemManager = systemManager;
            InitializeComponent();
        }

        private void Add(object sender, EventArgs args)
        {
            var input = string.Empty;
            if (InputDialog.ShowInputDialog(ref input) == DialogResult.OK)
            {
                var fileName = $"SaveGames/{SaveSNESSystem.GameName}_{input.Trim()}.json";
                _systemManager.Write(fileName, SaveSNESSystem);
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void OnFilterChange(object sender, EventArgs args)
        {
            listBox1.Items.Clear();

            List<SaveFileInfo> lst = _lst.Where(x => x.GameName.Contains(FilterTextBox.Text) || x.Name.Contains(FilterTextBox.Text))
                .OrderByDescending(x => x.DateTime).ToList();

            foreach (var obj in lst)
            {
                listBox1.Items.Add(obj);
            }
        }

        private void Reload(object sender, EventArgs arg)
        {
            var fileOpenDialog = new OpenFileDialog{ Title = "Open ROM of Super Nintendo", Filter = "ROM SMC/SFC|*.smc;*.sfc" };
            DialogResult result = fileOpenDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                PathTextBox.Text = fileOpenDialog.SafeFileName;
            }
        }

        private void LoadGame(object sender, EventArgs arg)
        {
            if (SelectOnly)
            {
                _systemManager.Write(_selectedObject.Path, SaveSNESSystem);
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void OnPathChange(object sender, EventArgs args)
        {
            SNESSystem.FileName = PathTextBox.Text;
            _systemManager.Write(_selectedObject.Path, SNESSystem);
        }

        private void ObjectSelected(object sender, EventArgs arg)
        {
            pictureBox1.Image = null;
            ErrorLabel.Text = string.Empty;

            if (listBox1.SelectedItem != null)
            {
                _selectedObject = (SaveFileInfo) listBox1.SelectedItem;
                DateLabel.Text = $"Last modified: {_selectedObject.DateTime:dd/MM/yyyy HH:mm:ss}";
                GameNameLabel.Text = $"Game : {_selectedObject.GameName}";
                NameLabel.Text = $"Save Name: {_selectedObject.Name}";
                try
                {
                    SNESSystem = _systemManager.Load(_selectedObject.Path);
                    SNESSystem.GameName = _selectedObject.GameName;
                    if (new FileInfo(SNESSystem.FileName).Exists)
                    {
                        PathLabel.Text = $"Path to game : {SNESSystem.FileName}";
                        PathLabel.ForeColor = Color.Black;
                        PathLabel.Visible = true;
                        LoadButton.Visible = true;
                        ReloadButton.Visible = false;
                        PathTextBox.Visible = false;
                    }
                    else
                    {
                        PathLabel.Visible = false;
                        PathLabel.Text = $"Path to game not found : {SNESSystem.FileName}";
                        PathLabel.ForeColor = Color.Red;
                        PathTextBox.Text = SNESSystem.FileName;
                        PathTextBox.Visible = !SelectOnly;
                        ReloadButton.Visible = !SelectOnly;
                        LoadButton.Visible = SelectOnly;
                    }

                    _renderer.RenderBuffer(SNESSystem.PPU.GetPixels());
                }
                catch
                {
                    ErrorLabel.Text = "Corrupt game position file";
                    LoadButton.Visible = SelectOnly;
                    ReloadButton.Visible = false;
                    PathTextBox.Visible = false;
                }
            }
        }

        private void OpenSaveFilesDialog_Load(object sender, EventArgs e)
        {
            _lst.Clear();
            listBox1.Items.Clear();
            _renderer.SetTargetControl(pictureBox1);
            listBox1.SelectedIndexChanged += ObjectSelected;
            ReloadButton.Click += Reload;
            PathTextBox.TextChanged += OnPathChange;
            LoadButton.Click += LoadGame;
            FilterTextBox.TextChanged += OnFilterChange;
            AddButton.Click += Add;

            var folder = new DirectoryInfo("SaveGames");
            if (!folder.Exists)
            {
                folder.Create();
            }

            foreach (FileInfo file in folder.GetFiles("*.json"))
            {
                var info = new SaveFileInfo
                {
                    DateTime = file.LastWriteTime,
                    GameName = file.Name.Split('_')[0],
                    Name = file.Name.Split('_')[1],
                    Path = file.FullName
                };
                info.Name = info.Name.Remove(info.Name.LastIndexOf(".", StringComparison.CurrentCultureIgnoreCase));
                _lst.Add(info);
            }

            if (SelectOnly)
            {
                _lst = _lst.Where(x => x.GameName.Equals(SaveSNESSystem.GameName)).ToList();
            }

            List<SaveFileInfo> lst = _lst.OrderByDescending(x => x.DateTime).ToList();
            foreach (SaveFileInfo obj in lst)
            {
                listBox1.Items.Add(obj);
            }
        }
    }
}