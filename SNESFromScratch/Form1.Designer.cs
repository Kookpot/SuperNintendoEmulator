using System.Windows.Forms;

namespace SNESFromScratch
{
    partial class Form1
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
            this.SaveGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.SlotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.ScreenshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DebugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DumpVRAMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DumpObjRAMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.LimitFPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.PicScreen)).BeginInit();
            this.MMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // PicScreen
            // 
            this.PicScreen.BackColor = System.Drawing.Color.Black;
            this.PicScreen.Location = new System.Drawing.Point(0, 24);
            this.PicScreen.Name = "PicScreen";
            this.PicScreen.Size = new System.Drawing.Size(256, 224);
            this.PicScreen.TabIndex = 0;
            this.PicScreen.TabStop = false;
            // 
            // MMenu
            // 
            this.MMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.OptionsToolStripMenuItem,
            this.HelpToolStripMenuItem});
            this.MMenu.Location = new System.Drawing.Point(0, 0);
            this.MMenu.Name = "MMenu";
            this.MMenu.Size = new System.Drawing.Size(256, 24);
            this.MMenu.TabIndex = 1;
            this.MMenu.Text = "Menu";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenROMToolStripMenuItem,
            this.ToolStripMenuItem1,
            this.SaveGameToolStripMenuItem,
            this.LoadGameToolStripMenuItem,
            this.ToolStripMenuItem2,
            this.SlotToolStripMenuItem,
            this.ToolStripMenuItem3,
            this.ScreenshotToolStripMenuItem,
            this.ToolStripMenuItem4,
            this.ExitToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.FileToolStripMenuItem.Text = "&File";
            // 
            // OpenROMToolStripMenuItem
            // 
            this.OpenROMToolStripMenuItem.Name = "OpenROMToolStripMenuItem";
            this.OpenROMToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.OpenROMToolStripMenuItem.Text = "&Open ROM";
            // 
            // ToolStripMenuItem1
            // 
            this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            this.ToolStripMenuItem1.Size = new System.Drawing.Size(136, 6);
            // 
            // SaveGameToolStripMenuItem
            // 
            this.SaveGameToolStripMenuItem.Name = "SaveGameToolStripMenuItem";
            this.SaveGameToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.SaveGameToolStripMenuItem.Text = "&Save Game";
            // 
            // LoadGameToolStripMenuItem
            // 
            this.LoadGameToolStripMenuItem.Name = "LoadGameToolStripMenuItem";
            this.LoadGameToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.LoadGameToolStripMenuItem.Text = "&Load Game";
            // 
            // ToolStripMenuItem2
            // 
            this.ToolStripMenuItem2.Name = "ToolStripMenuItem2";
            this.ToolStripMenuItem2.Size = new System.Drawing.Size(136, 6);
            // 
            // SlotToolStripMenuItem
            // 
            this.SlotToolStripMenuItem.Name = "SlotToolStripMenuItem";
            this.SlotToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.SlotToolStripMenuItem.Text = "&Slot";
            // 
            // ToolStripMenuItem3
            // 
            this.ToolStripMenuItem3.Name = "ToolStripMenuItem3";
            this.ToolStripMenuItem3.Size = new System.Drawing.Size(136, 6);
            // 
            // ScreenshotToolStripMenuItem
            // 
            this.ScreenshotToolStripMenuItem.Name = "ScreenshotToolStripMenuItem";
            this.ScreenshotToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.ScreenshotToolStripMenuItem.Text = "&Screenshot";
            // 
            // ToolStripMenuItem4
            // 
            this.ToolStripMenuItem4.Name = "ToolStripMenuItem4";
            this.ToolStripMenuItem4.Size = new System.Drawing.Size(136, 6);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.ExitToolStripMenuItem.Text = "&Exit";
            // 
            // OptionsToolStripMenuItem
            // 
            this.OptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DebugToolStripMenuItem,
            this.DumpVRAMToolStripMenuItem,
            this.DumpObjRAMToolStripMenuItem,
            this.ToolStripMenuItem5,
            this.LimitFPSToolStripMenuItem});
            this.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem";
            this.OptionsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.OptionsToolStripMenuItem.Text = "&Options";
            // 
            // DebugToolStripMenuItem
            // 
            this.DebugToolStripMenuItem.Name = "DebugToolStripMenuItem";
            this.DebugToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.DebugToolStripMenuItem.Text = "Debug";
            // 
            // DumpVRAMToolStripMenuItem
            // 
            this.DumpVRAMToolStripMenuItem.Name = "DumpVRAMToolStripMenuItem";
            this.DumpVRAMToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.DumpVRAMToolStripMenuItem.Text = "&Dump VRAM";
            // 
            // DumpObjRAMToolStripMenuItem
            // 
            this.DumpObjRAMToolStripMenuItem.Name = "DumpObjRAMToolStripMenuItem";
            this.DumpObjRAMToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.DumpObjRAMToolStripMenuItem.Text = "Dump ObjRAM";
            // 
            // ToolStripMenuItem5
            // 
            this.ToolStripMenuItem5.Name = "ToolStripMenuItem5";
            this.ToolStripMenuItem5.Size = new System.Drawing.Size(151, 6);
            // 
            // LimitFPSToolStripMenuItem
            // 
            this.LimitFPSToolStripMenuItem.Name = "LimitFPSToolStripMenuItem";
            this.LimitFPSToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.LimitFPSToolStripMenuItem.Text = "&Limit FPS";
            // 
            // HelpToolStripMenuItem
            // 
            this.HelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AboutToolStripMenuItem});
            this.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
            this.HelpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.HelpToolStripMenuItem.Text = "&Help";
            // 
            // AboutToolStripMenuItem
            // 
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            this.AboutToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.AboutToolStripMenuItem.Text = "&About";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(256, 248);
            this.Controls.Add(this.PicScreen);
            this.Controls.Add(this.MMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.MMenu;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SNES.net";
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
        private System.Windows.Forms.ToolStripMenuItem SaveGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LoadGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator ToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem SlotToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator ToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem ScreenshotToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator ToolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DebugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem; 
        private System.Windows.Forms.ToolStripSeparator ToolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem LimitFPSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DumpVRAMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DumpObjRAMToolStripMenuItem;

        #endregion
    }
}

