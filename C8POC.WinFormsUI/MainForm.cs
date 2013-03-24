// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="">
//   
// </copyright>
// <summary>
//   The main form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;

namespace C8POC.WinFormsUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// The main form.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The brush.
        /// </summary>
        private Brush brush = new SolidBrush(Color.White);

        /// <summary>
        /// The emulator.
        /// </summary>
        private C8Engine emulator;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
            this.emulator = new C8Engine();
            
            //Link output events to plugins
            this.emulator.ScreenChanged += PluginManager.Instance.SelectedGraphicsPlugin.Draw;
            this.emulator.SoundGenerated += PluginManager.Instance.SelectedSoundPlugin.GenerateSound;

            //Link input events to plugins
            PluginManager.Instance.SelectedKeyboardPlugin.KeyUp += this.emulator.KeyUp;
            PluginManager.Instance.SelectedKeyboardPlugin.KeyDown += this.emulator.KeyDown;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The exit tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.emulator.StopEmulator();
            this.Close();
        }

        /// <summary>
        /// The main form_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.emulator.StopEmulator();
        }

        /// <summary>
        /// The main form_ resize begin.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            this.emulator.StopEmulator();
        }

        /// <summary>
        /// The main form_ resize end.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            this.emulator.StartEmulator();
        }

        /// <summary>
        /// The open rom tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OpenRomToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.openFileDialogRom.ShowDialog() == DialogResult.OK)
            {
                this.emulator.LoadEmulator(this.openFileDialogRom.FileName);
                PluginManager.Instance.SelectedGraphicsPlugin.EnablePlugin();
                PluginManager.Instance.SelectedKeyboardPlugin.EnablePlugin();
                this.emulator.StartEmulator();
            }
        }

        /// <summary>
        /// The menu strip main window_ menu activate.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void menuStripMainWindow_MenuActivate(object sender, EventArgs e)
        {
            this.emulator.StopEmulator();
        }

        /// <summary>
        /// The menu strip main window_ menu deactivate.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void menuStripMainWindow_MenuDeactivate(object sender, EventArgs e)
        {
            this.emulator.StartEmulator();
        }

        /// <summary>
        /// The plugin settings tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void pluginSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PluginSettings form = new PluginSettings();
            form.ShowDialog();
        }

        #endregion
    }
}