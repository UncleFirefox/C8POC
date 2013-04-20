namespace C8POC.WinFormsUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using C8POC.Interfaces;

    /// <summary>
    /// Plugin settings form
    /// </summary>
    public partial class PluginSettings : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginSettings"/> class.
        /// </summary>
        public PluginSettings()
        {
            this.InitializeComponent();
            this.LoadPluginAssemblies();
        }

        /// <summary>
        /// Loads plugins inside of combos
        /// </summary>
        private void LoadPluginAssemblies()
        {
            if (PluginManager.Instance.SoundPlugins.Any())
            {
                var plugins = PluginManager.Instance.SoundPlugins.ToDictionary(x => x, x => x.Metadata.Description);
                comboBoxSound.DataSource = new BindingSource(plugins, null);
                comboBoxSound.DisplayMember = "Value";
                comboBoxSound.ValueMember = "Key";
            }

            if (PluginManager.Instance.GraphicsPlugins.Any())
            {
                var plugins = PluginManager.Instance.GraphicsPlugins.ToDictionary(x => x, x => x.Metadata.Description);
                comboBoxGraphics.DataSource = new BindingSource(plugins, null);
                comboBoxGraphics.DisplayMember = "Value";
                comboBoxGraphics.ValueMember = "Key";
            }

            if (PluginManager.Instance.KeyboardPlugins.Any())
            {
                var plugins = PluginManager.Instance.KeyboardPlugins.ToDictionary(x => x, x => x.Metadata.Description);
                comboBoxKeyInput.DataSource = new BindingSource(plugins, null);
                comboBoxKeyInput.DisplayMember = "Value";
                comboBoxKeyInput.ValueMember = "Key";
            }
        }

        /// <summary>
        /// Configures the selected sound plugin
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void ButtonSoundConfigClick(object sender, EventArgs e)
        {
            this.ConfigurePlugin(comboBoxSound);
        }

        /// <summary>
        /// Configures the selected graphics plugin
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void ButtonGraphicsConfigClick(object sender, EventArgs e)
        {
            this.ConfigurePlugin(comboBoxGraphics);
        }

        /// <summary>
        /// Configures the selected key input plugin
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void ButtonKeyInputConfigClick(object sender, EventArgs e)
        {
            this.ConfigurePlugin(comboBoxKeyInput);
        }

        /// <summary>
        /// Configures the plugin of a given combo box
        /// </summary>
        /// <param name="comboBox"></param>
        private void ConfigurePlugin(ComboBox comboBox)
        {
            var plugin = comboBox.SelectedValue as Lazy<IPlugin,IPluginMetadata>;

            if (plugin != null)
            {
                plugin.Value.Configure();
            }
        }
    }
}