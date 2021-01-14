
using System.Windows.Forms;

namespace SNESFromScratch.Rendering
{
    partial class InputChangerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SaveButton = new System.Windows.Forms.Button();
            this.LeftLabel = new System.Windows.Forms.Label();
            this.RightLabel = new System.Windows.Forms.Label();
            this.LeftTextBox = new System.Windows.Forms.TextBox();
            this.RightTextBox = new System.Windows.Forms.TextBox();
            this.DownTextBox = new System.Windows.Forms.TextBox();
            this.UpTextBox = new System.Windows.Forms.TextBox();
            this.DownLabel = new System.Windows.Forms.Label();
            this.UpLabel = new System.Windows.Forms.Label();
            this.YTextBox = new System.Windows.Forms.TextBox();
            this.XTextBox = new System.Windows.Forms.TextBox();
            this.YLabel = new System.Windows.Forms.Label();
            this.XLabel = new System.Windows.Forms.Label();
            this.StartTextBox = new System.Windows.Forms.TextBox();
            this.SelectTextBox = new System.Windows.Forms.TextBox();
            this.StartLabel = new System.Windows.Forms.Label();
            this.SelectLabel = new System.Windows.Forms.Label();
            this.BTextBox = new System.Windows.Forms.TextBox();
            this.ATextBox = new System.Windows.Forms.TextBox();
            this.BLabel = new System.Windows.Forms.Label();
            this.ALabel = new System.Windows.Forms.Label();
            this.RTextBox = new System.Windows.Forms.TextBox();
            this.LTextBox = new System.Windows.Forms.TextBox();
            this.RLabel = new System.Windows.Forms.Label();
            this.LLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(256, 221);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 31);
            this.SaveButton.TabIndex = 0;
            this.SaveButton.Text = "Apply";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveClick);
            // 
            // LeftLabel
            // 
            this.LeftLabel.Location = new System.Drawing.Point(10, 15);
            this.LeftLabel.Name = "LeftLabel";
            this.LeftLabel.Size = new System.Drawing.Size(50, 17);
            this.LeftLabel.TabIndex = 1;
            this.LeftLabel.Text = "Left";
            this.LeftLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RightLabel
            // 
            this.RightLabel.Location = new System.Drawing.Point(10, 47);
            this.RightLabel.Name = "RightLabel";
            this.RightLabel.Size = new System.Drawing.Size(50, 17);
            this.RightLabel.TabIndex = 2;
            this.RightLabel.Text = "Right";
            this.RightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LeftTextBox
            // 
            this.LeftTextBox.Location = new System.Drawing.Point(64, 12);
            this.LeftTextBox.Name = "LeftTextBox";
            this.LeftTextBox.Size = new System.Drawing.Size(100, 22);
            this.LeftTextBox.TabIndex = 1;
            this.LeftTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.LeftTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxClicked);
            this.LeftTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyPressedAnywhere);
            // 
            // RightTextBox
            // 
            this.RightTextBox.Location = new System.Drawing.Point(64, 44);
            this.RightTextBox.Name = "RightTextBox";
            this.RightTextBox.Size = new System.Drawing.Size(100, 22);
            this.RightTextBox.TabIndex = 2;
            this.RightTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.RightTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxClicked);
            this.RightTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyPressedAnywhere);
            // 
            // DownTextBox
            // 
            this.DownTextBox.Location = new System.Drawing.Point(64, 108);
            this.DownTextBox.Name = "DownTextBox";
            this.DownTextBox.Size = new System.Drawing.Size(100, 22);
            this.DownTextBox.TabIndex = 4;
            this.DownTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.DownTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxClicked);
            this.DownTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyPressedAnywhere);
            // 
            // UpTextBox
            // 
            this.UpTextBox.Location = new System.Drawing.Point(64, 76);
            this.UpTextBox.Name = "UpTextBox";
            this.UpTextBox.Size = new System.Drawing.Size(100, 22);
            this.UpTextBox.TabIndex = 3;
            this.UpTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.UpTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxClicked);
            this.UpTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyPressedAnywhere);
            // 
            // DownLabel
            // 
            this.DownLabel.Location = new System.Drawing.Point(10, 111);
            this.DownLabel.Name = "DownLabel";
            this.DownLabel.Size = new System.Drawing.Size(50, 17);
            this.DownLabel.TabIndex = 6;
            this.DownLabel.Text = "Down";
            this.DownLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // UpLabel
            // 
            this.UpLabel.Location = new System.Drawing.Point(10, 79);
            this.UpLabel.Name = "UpLabel";
            this.UpLabel.Size = new System.Drawing.Size(50, 17);
            this.UpLabel.TabIndex = 5;
            this.UpLabel.Text = "Up";
            this.UpLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // YTextBox
            // 
            this.YTextBox.Location = new System.Drawing.Point(231, 44);
            this.YTextBox.Name = "YTextBox";
            this.YTextBox.Size = new System.Drawing.Size(100, 22);
            this.YTextBox.TabIndex = 8;
            this.YTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.YTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxClicked);
            this.YTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyPressedAnywhere);
            // 
            // XTextBox
            // 
            this.XTextBox.Location = new System.Drawing.Point(231, 12);
            this.XTextBox.Name = "XTextBox";
            this.XTextBox.Size = new System.Drawing.Size(100, 22);
            this.XTextBox.TabIndex = 7;
            this.XTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.XTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxClicked);
            this.XTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyPressedAnywhere);
            // 
            // YLabel
            // 
            this.YLabel.Location = new System.Drawing.Point(177, 47);
            this.YLabel.Name = "YLabel";
            this.YLabel.Size = new System.Drawing.Size(50, 17);
            this.YLabel.TabIndex = 10;
            this.YLabel.Text = "Y";
            this.YLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // XLabel
            // 
            this.XLabel.Location = new System.Drawing.Point(177, 15);
            this.XLabel.Name = "XLabel";
            this.XLabel.Size = new System.Drawing.Size(50, 17);
            this.XLabel.TabIndex = 9;
            this.XLabel.Text = "X";
            this.XLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // StartTextBox
            // 
            this.StartTextBox.Location = new System.Drawing.Point(64, 172);
            this.StartTextBox.Name = "StartTextBox";
            this.StartTextBox.Size = new System.Drawing.Size(100, 22);
            this.StartTextBox.TabIndex = 6;
            this.StartTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.StartTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxClicked);
            this.StartTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyPressedAnywhere);
            // 
            // SelectTextBox
            // 
            this.SelectTextBox.Location = new System.Drawing.Point(64, 140);
            this.SelectTextBox.Name = "SelectTextBox";
            this.SelectTextBox.Size = new System.Drawing.Size(100, 22);
            this.SelectTextBox.TabIndex = 5;
            this.SelectTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.SelectTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxClicked);
            this.SelectTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyPressedAnywhere);
            // 
            // StartLabel
            // 
            this.StartLabel.Location = new System.Drawing.Point(10, 175);
            this.StartLabel.Name = "StartLabel";
            this.StartLabel.Size = new System.Drawing.Size(50, 17);
            this.StartLabel.TabIndex = 14;
            this.StartLabel.Text = "Start";
            this.StartLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SelectLabel
            // 
            this.SelectLabel.Location = new System.Drawing.Point(10, 143);
            this.SelectLabel.Name = "SelectLabel";
            this.SelectLabel.Size = new System.Drawing.Size(50, 17);
            this.SelectLabel.TabIndex = 13;
            this.SelectLabel.Text = "Select";
            this.SelectLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BTextBox
            // 
            this.BTextBox.Location = new System.Drawing.Point(231, 108);
            this.BTextBox.Name = "BTextBox";
            this.BTextBox.Size = new System.Drawing.Size(100, 22);
            this.BTextBox.TabIndex = 10;
            this.BTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.BTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxClicked);
            this.BTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyPressedAnywhere);
            // 
            // ATextBox
            // 
            this.ATextBox.Location = new System.Drawing.Point(231, 76);
            this.ATextBox.Name = "ATextBox";
            this.ATextBox.Size = new System.Drawing.Size(100, 22);
            this.ATextBox.TabIndex = 9;
            this.ATextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ATextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxClicked);
            this.ATextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyPressedAnywhere);
            // 
            // BLabel
            // 
            this.BLabel.Location = new System.Drawing.Point(177, 111);
            this.BLabel.Name = "BLabel";
            this.BLabel.Size = new System.Drawing.Size(50, 17);
            this.BLabel.TabIndex = 18;
            this.BLabel.Text = "B";
            this.BLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ALabel
            // 
            this.ALabel.Location = new System.Drawing.Point(177, 79);
            this.ALabel.Name = "ALabel";
            this.ALabel.Size = new System.Drawing.Size(50, 17);
            this.ALabel.TabIndex = 17;
            this.ALabel.Text = "A";
            this.ALabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RTextBox
            // 
            this.RTextBox.Location = new System.Drawing.Point(231, 172);
            this.RTextBox.Name = "RTextBox";
            this.RTextBox.Size = new System.Drawing.Size(100, 22);
            this.RTextBox.TabIndex = 20;
            this.RTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.RTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxClicked);
            this.RTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyPressedAnywhere);
            // 
            // LTextBox
            // 
            this.LTextBox.Location = new System.Drawing.Point(231, 140);
            this.LTextBox.Name = "LTextBox";
            this.LTextBox.Size = new System.Drawing.Size(100, 22);
            this.LTextBox.TabIndex = 19;
            this.LTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.LTextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextBoxClicked);
            this.LTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyPressedAnywhere);
            // 
            // RLabel
            // 
            this.RLabel.Location = new System.Drawing.Point(177, 175);
            this.RLabel.Name = "RLabel";
            this.RLabel.Size = new System.Drawing.Size(50, 17);
            this.RLabel.TabIndex = 22;
            this.RLabel.Text = "R";
            this.RLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LLabel
            // 
            this.LLabel.Location = new System.Drawing.Point(177, 143);
            this.LLabel.Name = "LLabel";
            this.LLabel.Size = new System.Drawing.Size(50, 17);
            this.LLabel.TabIndex = 11;
            this.LLabel.Text = "L";
            this.LLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // InputChangerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(366, 276);
            this.Controls.Add(this.RTextBox);
            this.Controls.Add(this.LTextBox);
            this.Controls.Add(this.RLabel);
            this.Controls.Add(this.LLabel);
            this.Controls.Add(this.BTextBox);
            this.Controls.Add(this.ATextBox);
            this.Controls.Add(this.BLabel);
            this.Controls.Add(this.ALabel);
            this.Controls.Add(this.StartTextBox);
            this.Controls.Add(this.SelectTextBox);
            this.Controls.Add(this.StartLabel);
            this.Controls.Add(this.SelectLabel);
            this.Controls.Add(this.YTextBox);
            this.Controls.Add(this.XTextBox);
            this.Controls.Add(this.YLabel);
            this.Controls.Add(this.XLabel);
            this.Controls.Add(this.DownTextBox);
            this.Controls.Add(this.UpTextBox);
            this.Controls.Add(this.DownLabel);
            this.Controls.Add(this.UpLabel);
            this.Controls.Add(this.RightTextBox);
            this.Controls.Add(this.LeftTextBox);
            this.Controls.Add(this.RightLabel);
            this.Controls.Add(this.LeftLabel);
            this.Controls.Add(this.SaveButton);
            this.Name = "InputChangerForm";
            this.Text = "InputChangerForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Label LeftLabel;
        private System.Windows.Forms.Label RightLabel;
        private System.Windows.Forms.TextBox LeftTextBox;
        private System.Windows.Forms.TextBox RightTextBox;
        private System.Windows.Forms.TextBox DownTextBox;
        private System.Windows.Forms.TextBox UpTextBox;
        private System.Windows.Forms.Label DownLabel;
        private System.Windows.Forms.Label UpLabel;
        private System.Windows.Forms.TextBox YTextBox;
        private System.Windows.Forms.TextBox XTextBox;
        private System.Windows.Forms.Label YLabel;
        private System.Windows.Forms.Label XLabel;
        private System.Windows.Forms.TextBox StartTextBox;
        private System.Windows.Forms.TextBox SelectTextBox;
        private System.Windows.Forms.Label StartLabel;
        private System.Windows.Forms.Label SelectLabel;
        private System.Windows.Forms.TextBox BTextBox;
        private System.Windows.Forms.TextBox ATextBox;
        private System.Windows.Forms.Label BLabel;
        private System.Windows.Forms.Label ALabel;
        private System.Windows.Forms.TextBox RTextBox;
        private System.Windows.Forms.TextBox LTextBox;
        private System.Windows.Forms.Label RLabel;
        private System.Windows.Forms.Label LLabel;
    }
}