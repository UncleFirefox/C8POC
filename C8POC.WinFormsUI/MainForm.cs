// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="">
//   
// </copyright>
// <summary>
//   The main form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.WinFormsUI
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The main form.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Constants and Fields

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

            // Link output events to plugins
            this.emulator.ScreenChanged += PluginManager.Instance.SelectedGraphicsPlugin.Draw;
            this.emulator.SoundGenerated += PluginManager.Instance.SelectedSoundPlugin.GenerateSound;

            // Link key input events to plugins
            PluginManager.Instance.SelectedKeyboardPlugin.KeyUp += this.emulator.KeyUp;
            PluginManager.Instance.SelectedKeyboardPlugin.KeyDown += this.emulator.KeyDown;
            PluginManager.Instance.SelectedKeyboardPlugin.KeyStopEmulation += this.StopEmulation;

            // Link Graphics Plugin Closing Window
            PluginManager.Instance.SelectedGraphicsPlugin.GraphicsExit += this.StopEmulation;
        }

        #endregion

        #region Methods

        private void StartEmulation()
        {
            PluginManager.Instance.StartPluginsExecution();
            this.emulator.StartEmulator();
        }

        private void StopEmulation()
        {
            this.emulator.StopEmulator();
            PluginManager.Instance.StopPluginsExecution();
        }


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
            this.StopEmulation();
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
                this.StartEmulation();
            }
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