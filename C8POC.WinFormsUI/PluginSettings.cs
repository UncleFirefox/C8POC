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
            LoadPluginsInCombo(PluginManager.Instance.SoundPlugins, comboBoxSound);
            LoadPluginsInCombo(PluginManager.Instance.GraphicsPlugins, comboBoxGraphics);
            LoadPluginsInCombo(PluginManager.Instance.KeyboardPlugins, comboBoxKeyInput);
        }

        /// <summary>
        /// Loads plugins inside the combo
        /// </summary>
        /// <param name="pluginList">
        /// The plugin list.
        /// </param>
        /// <param name="combo">
        /// The combo.
        /// </param>
        private void LoadPluginsInCombo(IEnumerable<IPlugin> pluginList, ComboBox combo)
        {
            if (pluginList.Any())
            {
                Dictionary<IPlugin, string> plugins = pluginList.ToDictionary(x => x, x => x.PluginDescription);
                BindDictionaryToComboBox(combo, plugins);
            }
        }

        /// <summary>
        /// Binds the dictionary to the proper combo
        /// </summary>
        /// <param name="combo">
        /// The combo.
        /// </param>
        /// <param name="plugins">
        /// The plugins.
        /// </param>
        private static void BindDictionaryToComboBox(ComboBox combo, Dictionary<IPlugin, string> plugins)
        {
            combo.DataSource = new BindingSource(plugins, null);
            combo.DisplayMember = "Value";
            combo.ValueMember = "Key";
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
            var plugin = comboBox.SelectedValue as IPlugin;

            if (plugin != null)
            {
                plugin.Configure();
            }
        }
    }
}