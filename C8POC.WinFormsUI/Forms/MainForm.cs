// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   The main form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.WinFormsUI.Forms
{
    using System;
    using System.Windows.Forms;

    using Autofac;

    using C8POC.Interfaces.Domain.Engines;
    using C8POC.WinFormsUI.Container;
    using C8POC.WinFormsUI.Services;

    /// <summary>
    /// The main form.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The engine mediator.
        /// </summary>
        private IEngineMediator engineMediator;

        /// <summary>
        /// The disassembler form
        /// </summary>
        private DisassemblerForm disassemblerForm;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
            this.disassemblerForm = new DisassemblerForm();

            // The first time we have to notice if the disassembler was enabled or not
            var windowsConfigurationService = new WindowsConfigurationService();
            var engineConfiguration = windowsConfigurationService.GetEngineConfiguration();

            var enableDisassembler = engineConfiguration.ContainsKey("DisassemblerEnabled")
                                     && engineConfiguration["DisassemblerEnabled"] == "True";

            this.ResolveEngine(enableDisassembler);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resolves the engine via Dependency Injection
        /// </summary>
        /// <param name="disassemblerEnabled">
        /// Indicates if the disassembler should be enabled
        /// </param>
        private void ResolveEngine(bool disassemblerEnabled)
        {
            var builder = new C8WindowsContainer();

            if (disassemblerEnabled)
            {
                builder.EnableDisassembler(this.disassemblerForm);
            }

            var container = builder.Build();

            this.engineMediator = container.Resolve<IEngineMediator>();
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
            this.engineMediator.StopEmulation();
            this.Close();
        }

        /// <summary>
        /// Event raised when the main form is closed
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            this.engineMediator.StopEmulation();
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
                this.engineMediator.LoadRomToEngine(this.openFileDialogRom.FileName);

                if (this.disassemblerForm.Visible)
                {
                    // TODO: Clean the gridview :)
                    this.disassemblerForm.CleanGridView();
                }

                this.engineMediator.StartEmulation();
                // this.Hide();
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
            var form = new PluginSettings(this.engineMediator.InputOutputEngine.PluginService, this.engineMediator.ConfigurationEngine.ConfigurationService);

            if (form.ShowDialog() == DialogResult.OK)
            {
                // We should tell the engine to reload the plugins
                this.engineMediator.InputOutputEngine.LoadPlugins();
            }
        }

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
                this.engineMediator.ConfigurationEngine.ConfigurationService.SaveEngineConfiguration();

                if (form.IsDisassemblerEnabled && !this.disassemblerForm.Visible)
                {
                    this.ResolveEngine(true);
                }
                else if (!form.IsDisassemblerEnabled && this.disassemblerForm.Visible)
                {
                    this.disassemblerForm.Hide();
                    this.ResolveEngine(false);
                }
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

        #endregion
    }
}