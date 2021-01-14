namespace SNESFromScratch.Rendering
{
    partial class MainForm
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
            this.PicScreen = new System.Windows.Forms.PictureBox();
            this.MMenu = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.PicScreen)).BeginInit();
            this.MMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // PicScreen
            // 
            this.PicScreen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PicScreen.BackColor = System.Drawing.Color.Black;
            this.PicScreen.Location = new System.Drawing.Point(0, 30);
            this.PicScreen.Margin = new System.Windows.Forms.Padding(4);
            this.PicScreen.Name = "PicScreen";
            this.PicScreen.Size = new System.Drawing.Size(341, 276);
            this.PicScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PicScreen.TabIndex = 0;
            this.PicScreen.TabStop = false;
            // 
            // MMenu
            // 
            this.MMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.OptionsToolStripMenuItem});
            this.MMenu.Location = new System.Drawing.Point(0, 0);
            this.MMenu.Name = "MMenu";
            this.MMenu.Size = new System.Drawing.Size(341, 28);
            this.MMenu.TabIndex = 1;
            this.MMenu.Text = "Menu";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenROMToolStripMenuItem,
            this.ToolStripMenuItem1,
            this.ExitToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.FileToolStripMenuItem.Text = "&File";
            // 
            // OpenROMToolStripMenuItem
            // 
            this.OpenROMToolStripMenuItem.Name = "OpenROMToolStripMenuItem";
            this.OpenROMToolStripMenuItem.Size = new System.Drawing.Size(165, 26);
            this.OpenROMToolStripMenuItem.Text = "&Open ROM";
            // 
            // ToolStripMenuItem1
            // 
            this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            this.ToolStripMenuItem1.Size = new System.Drawing.Size(162, 6);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(165, 26);
            this.ExitToolStripMenuItem.Text = "&Exit";
            // 
            // OptionsToolStripMenuItem
            // 
            this.OptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.InputToolStripMenuItem});
            this.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem";
            this.OptionsToolStripMenuItem.Size = new System.Drawing.Size(75, 24);
            this.OptionsToolStripMenuItem.Text = "&Options";
            // 
            // InputToolStripMenuItem
            // 
            this.InputToolStripMenuItem.Name = "InputToolStripMenuItem";
            this.InputToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.InputToolStripMenuItem.Text = "&Input";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 305);
            this.Controls.Add(this.PicScreen);
            this.Controls.Add(this.MMenu);
            this.MainMenuStrip = this.MMenu;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SNESFromScratch";
            ((System.ComponentModel.ISupportInitialize)(this.PicScreen)).EndInit();
            this.MMenu.ResumeLayout(false);
            this.MMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public System.Windows.Forms.PictureBox PicScreen; 
        private System.Windows.Forms.MenuStrip MMenu;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenROMToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem InputToolStripMenuItem;

        #endregion
    }
}