// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
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
        private readonly C8Engine emulator;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
            this.emulator = new C8Engine();
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
        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            this.emulator.StopEmulator();
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
                this.emulator.StartEmulation();
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
        private void PluginSettingsToolStripMenuItemClick(object sender, EventArgs e)
        {
            var form = new PluginSettings(this.emulator.PluginService);
            
            if (form.ShowDialog() == DialogResult.OK)
            {
                // We should tell the engine to reload the plugins
                this.emulator.LoadPlugins();
            }
        }

        #endregion

        /// <summary>
        /// Opens a core settings form
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CoreSettingsToolStripMenuItemClick(object sender, EventArgs e)
        {
            var form = new CoreSettingsForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                this.emulator.PluginService.SaveEngineConfiguration();
            }
        }

        /// <summary>
        /// The about tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            var aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }
    }
}