namespace C8POC.WinFormsUI
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
            this.menuStripMainWindow = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.coreSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pluginSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialogRom = new System.Windows.Forms.OpenFileDialog();
            this.emulatorStatusStrip = new System.Windows.Forms.StatusStrip();
            this.labelFps = new System.Windows.Forms.ToolStripStatusLabel();
            this.labelStatusEmulator = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStripMainWindow.SuspendLayout();
            this.emulatorStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMainWindow
            // 
            this.menuStripMainWindow.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolStripConfig,
            this.helpToolStripMenuItem});
            this.menuStripMainWindow.Location = new System.Drawing.Point(0, 0);
            this.menuStripMainWindow.Name = "menuStripMainWindow";
            this.menuStripMainWindow.Size = new System.Drawing.Size(301, 24);
            this.menuStripMainWindow.TabIndex = 0;
            this.menuStripMainWindow.Text = "menuStripMainWindow";
            this.menuStripMainWindow.MenuActivate += new System.EventHandler(this.menuStripMainWindow_MenuActivate);
            this.menuStripMainWindow.MenuDeactivate += new System.EventHandler(this.menuStripMainWindow_MenuDeactivate);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openRomToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openRomToolStripMenuItem
            // 
            this.openRomToolStripMenuItem.Name = "openRomToolStripMenuItem";
            this.openRomToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.openRomToolStripMenuItem.Text = "Open Rom...";
            this.openRomToolStripMenuItem.Click += new System.EventHandler(this.OpenRomToolStripMenuItemClick);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // toolStripConfig
            // 
            this.toolStripConfig.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.coreSettingsToolStripMenuItem,
            this.pluginSettingsToolStripMenuItem});
            this.toolStripConfig.Name = "toolStripConfig";
            this.toolStripConfig.Size = new System.Drawing.Size(55, 20);
            this.toolStripConfig.Text = "Config";
            // 
            // coreSettingsToolStripMenuItem
            // 
            this.coreSettingsToolStripMenuItem.Name = "coreSettingsToolStripMenuItem";
            this.coreSettingsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.coreSettingsToolStripMenuItem.Text = "Core Settings...";
            // 
            // pluginSettingsToolStripMenuItem
            // 
            this.pluginSettingsToolStripMenuItem.Name = "pluginSettingsToolStripMenuItem";
            this.pluginSettingsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.pluginSettingsToolStripMenuItem.Text = "Plugin Settings...";
            this.pluginSettingsToolStripMenuItem.Click += new System.EventHandler(this.pluginSettingsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // openFileDialogRom
            // 
            this.openFileDialogRom.Title = "Open Rom";
            // 
            // emulatorStatusStrip
            // 
            this.emulatorStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelFps,
            this.labelStatusEmulator});
            this.emulatorStatusStrip.Location = new System.Drawing.Point(0, 159);
            this.emulatorStatusStrip.Name = "emulatorStatusStrip";
            this.emulatorStatusStrip.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.emulatorStatusStrip.Size = new System.Drawing.Size(301, 22);
            this.emulatorStatusStrip.SizingGrip = false;
            this.emulatorStatusStrip.TabIndex = 1;
            this.emulatorStatusStrip.Text = "statusStrip1";
            // 
            // labelFps
            // 
            this.labelFps.BackColor = System.Drawing.SystemColors.Control;
            this.labelFps.Name = "labelFps";
            this.labelFps.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelFps.Size = new System.Drawing.Size(30, 17);
            this.labelFps.Text = "IDLE";
            // 
            // labelStatusEmulator
            // 
            this.labelStatusEmulator.BackColor = System.Drawing.SystemColors.Control;
            this.labelStatusEmulator.Name = "labelStatusEmulator";
            this.labelStatusEmulator.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelStatusEmulator.Size = new System.Drawing.Size(42, 17);
            this.labelStatusEmulator.Text = "Status:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(301, 181);
            this.Controls.Add(this.emulatorStatusStrip);
            this.Controls.Add(this.menuStripMainWindow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MainMenuStrip = this.menuStripMainWindow;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Chip8 Proof Of Concept";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResizeBegin += new System.EventHandler(this.MainForm_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
            this.menuStripMainWindow.ResumeLayout(false);
            this.menuStripMainWindow.PerformLayout();
            this.emulatorStatusStrip.ResumeLayout(false);
            this.emulatorStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMainWindow;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialogRom;
        private System.Windows.Forms.StatusStrip emulatorStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel labelFps;
        private System.Windows.Forms.ToolStripStatusLabel labelStatusEmulator;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripConfig;
        private System.Windows.Forms.ToolStripMenuItem coreSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pluginSettingsToolStripMenuItem;

    }
}

