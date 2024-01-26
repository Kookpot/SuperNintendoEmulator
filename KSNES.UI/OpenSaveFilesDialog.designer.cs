
namespace KSNES.GUI
{
    partial class OpenSaveFilesDialog
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.pictureBox1 = new SNESPictureBox();
            this.FilterTextBox = new System.Windows.Forms.TextBox();
            this.DateLabel = new System.Windows.Forms.Label();
            this.GameNameLabel = new System.Windows.Forms.Label();
            this.PathLabel = new System.Windows.Forms.Label();
            this.LoadButton = new System.Windows.Forms.Button();
            this.ErrorLabel = new System.Windows.Forms.Label();
            this.PathTextBox = new System.Windows.Forms.TextBox();
            this.ReloadButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.NameLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Location = new System.Drawing.Point(13, 45);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(259, 436);
            this.listBox1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(504, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(341, 276);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // FilterTextBox
            // 
            this.FilterTextBox.Location = new System.Drawing.Point(13, 13);
            this.FilterTextBox.Name = "FilterTextBox";
            this.FilterTextBox.Size = new System.Drawing.Size(259, 22);
            this.FilterTextBox.TabIndex = 2;
            // 
            // DateLabel
            // 
            this.DateLabel.AutoSize = true;
            this.DateLabel.Location = new System.Drawing.Point(296, 308);
            this.DateLabel.Name = "DateLabel";
            this.DateLabel.Size = new System.Drawing.Size(0, 17);
            this.DateLabel.TabIndex = 3;
            // 
            // GameNameLabel
            // 
            this.GameNameLabel.AutoSize = true;
            this.GameNameLabel.Location = new System.Drawing.Point(296, 342);
            this.GameNameLabel.Name = "GameNameLabel";
            this.GameNameLabel.Size = new System.Drawing.Size(0, 17);
            this.GameNameLabel.TabIndex = 4;
            // 
            // PathLabel
            // 
            this.PathLabel.AutoSize = true;
            this.PathLabel.Location = new System.Drawing.Point(296, 423);
            this.PathLabel.Name = "PathLabel";
            this.PathLabel.Size = new System.Drawing.Size(0, 17);
            this.PathLabel.TabIndex = 5;
            // 
            // LoadButton
            // 
            this.LoadButton.Location = new System.Drawing.Point(321, 36);
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(82, 23);
            this.LoadButton.TabIndex = 6;
            this.LoadButton.Text = "Load";
            this.LoadButton.UseVisualStyleBackColor = true;
            this.LoadButton.Visible = false;
            // 
            // ErrorLabel
            // 
            this.ErrorLabel.AutoSize = true;
            this.ErrorLabel.ForeColor = System.Drawing.Color.Red;
            this.ErrorLabel.Location = new System.Drawing.Point(321, 80);
            this.ErrorLabel.Name = "ErrorLabel";
            this.ErrorLabel.Size = new System.Drawing.Size(0, 17);
            this.ErrorLabel.TabIndex = 7;
            // 
            // PathTextBox
            // 
            this.PathTextBox.Location = new System.Drawing.Point(299, 456);
            this.PathTextBox.Name = "PathTextBox";
            this.PathTextBox.Size = new System.Drawing.Size(326, 22);
            this.PathTextBox.TabIndex = 8;
            this.PathTextBox.Visible = false;
            // 
            // ReloadButton
            // 
            this.ReloadButton.Location = new System.Drawing.Point(631, 456);
            this.ReloadButton.Name = "ReloadButton";
            this.ReloadButton.Size = new System.Drawing.Size(75, 23);
            this.ReloadButton.TabIndex = 9;
            this.ReloadButton.Text = "Reload";
            this.ReloadButton.UseVisualStyleBackColor = true;
            this.ReloadButton.Visible = false;
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(279, 45);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(22, 22);
            this.AddButton.TabIndex = 10;
            this.AddButton.Text = "+";
            this.AddButton.UseMnemonic = false;
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Visible = false;
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(296, 379);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(0, 17);
            this.NameLabel.TabIndex = 11;
            // 
            // OpenSaveFilesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(871, 506);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.ReloadButton);
            this.Controls.Add(this.PathTextBox);
            this.Controls.Add(this.ErrorLabel);
            this.Controls.Add(this.LoadButton);
            this.Controls.Add(this.PathLabel);
            this.Controls.Add(this.GameNameLabel);
            this.Controls.Add(this.DateLabel);
            this.Controls.Add(this.FilterTextBox);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.listBox1);
            this.Name = "OpenSaveFilesDialog";
            this.Text = "OpenSaveFilesDialog";
            this.Load += new System.EventHandler(this.OpenSaveFilesDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private SNESPictureBox pictureBox1;
        private System.Windows.Forms.TextBox FilterTextBox;
        private System.Windows.Forms.Label DateLabel;
        private System.Windows.Forms.Label GameNameLabel;
        private System.Windows.Forms.Label PathLabel;
        private System.Windows.Forms.Button LoadButton;
        private System.Windows.Forms.Label ErrorLabel;
        private System.Windows.Forms.TextBox PathTextBox;
        private System.Windows.Forms.Button ReloadButton;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Label NameLabel;
    }
}