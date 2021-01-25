using System;
using System.Drawing;
using System.Windows.Forms;
using KSNES.SNESSystem;

namespace KSNES.GUI
{
    public partial class InputChangerForm : Form
    {
        private readonly IKeyMapper _keyMapper;
        private Control _currentControl;

        public InputChangerForm(IKeyMapper keyMapper)
        {
            InitializeComponent();
            _keyMapper = keyMapper;
            Prep(ATextBox, _keyMapper.ReverseMap(SNESButton.A), Keys.A);
            Prep(BTextBox, _keyMapper.ReverseMap(SNESButton.B), Keys.B);
            Prep(XTextBox, _keyMapper.ReverseMap(SNESButton.X), Keys.X);
            Prep(YTextBox, _keyMapper.ReverseMap(SNESButton.Y), Keys.Y);
            Prep(LTextBox, _keyMapper.ReverseMap(SNESButton.L), Keys.L);
            Prep(RTextBox, _keyMapper.ReverseMap(SNESButton.R), Keys.R);
            Prep(StartTextBox, _keyMapper.ReverseMap(SNESButton.Start), Keys.End);
            Prep(SelectTextBox, _keyMapper.ReverseMap(SNESButton.Sel), Keys.Tab);
            Prep(LeftTextBox, _keyMapper.ReverseMap(SNESButton.Left), Keys.Left);
            Prep(RightTextBox, _keyMapper.ReverseMap(SNESButton.Right), Keys.Right);
            Prep(UpTextBox, _keyMapper.ReverseMap(SNESButton.Up), Keys.Up);
            Prep(DownTextBox, _keyMapper.ReverseMap(SNESButton.Down), Keys.Down);
        }

        private static void Prep(Control box, Keys? key, Keys fallBack)
        {
            box.Text = key?.ToString() ?? fallBack.ToString();
            box.Tag = key ?? fallBack;
        }

        private void SaveClick(object sender, EventArgs e)
        {
            try
            {
                _keyMapper.Clear();
                _keyMapper.ChangeMap((Keys) ATextBox.Tag, SNESButton.A);
                _keyMapper.ChangeMap((Keys) BTextBox.Tag, SNESButton.B);
                _keyMapper.ChangeMap((Keys) XTextBox.Tag, SNESButton.X);
                _keyMapper.ChangeMap((Keys) YTextBox.Tag, SNESButton.Y);
                _keyMapper.ChangeMap((Keys) LTextBox.Tag, SNESButton.L);
                _keyMapper.ChangeMap((Keys) RTextBox.Tag, SNESButton.R);
                _keyMapper.ChangeMap((Keys) SelectTextBox.Tag, SNESButton.Sel);
                _keyMapper.ChangeMap((Keys) StartTextBox.Tag, SNESButton.Start);
                _keyMapper.ChangeMap((Keys) UpTextBox.Tag, SNESButton.Up);
                _keyMapper.ChangeMap((Keys) DownTextBox.Tag, SNESButton.Down);
                _keyMapper.ChangeMap((Keys) LeftTextBox.Tag, SNESButton.Left);
                _keyMapper.ChangeMap((Keys) RightTextBox.Tag, SNESButton.Right);
                _keyMapper.SaveChanges();
                Close();
            }
            catch { }
        }

        private void TextBoxClicked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_currentControl != null)
                {
                    _currentControl.BackColor = Color.White;
                    _currentControl.Text = ((Keys) _currentControl.Tag).ToString();
                }
                _currentControl = (Control) sender;
                _currentControl.Text = string.Empty;
                _currentControl.BackColor = Color.Red;
            }
        }

        private void KeyPressedAnywhere(object sender, KeyEventArgs e)
        {
            if (_currentControl != null)
            {
                _currentControl.Tag = e.KeyCode;
                _currentControl.Text = e.KeyCode.ToString();
                _currentControl.BackColor = Color.White;
                _currentControl = null;
            }
        }
    }
}