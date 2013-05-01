// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginSettings.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Plugin settings form
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.WinFormsUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using C8POC.Interfaces;

    /// <summary>
    ///     Plugin settings form
    /// </summary>
    public partial class PluginSettings : Form
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PluginSettings" /> class.
        /// </summary>
        public PluginSettings()
        {
            this.InitializeComponent();
            this.BindAssembliesToComboBox();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Configures the selected graphics plugin
        /// </summary>
        /// <param name="sender">
        /// The sender
        /// </param>
        /// <param name="e">
        /// Event args
        /// </param>
        private void ButtonGraphicsConfigClick(object sender, EventArgs e)
        {
            this.ConfigurePlugin<IGraphicsPlugin>(this.comboBoxGraphics);
        }

        /// <summary>
        /// Configures the selected key input plugin
        /// </summary>
        /// <param name="sender">
        /// The sender
        /// </param>
        /// <param name="e">
        /// Event args
        /// </param>
        private void ButtonKeyInputConfigClick(object sender, EventArgs e)
        {
            this.ConfigurePlugin<IKeyboardPlugin>(this.comboBoxKeyInput);
        }

        /// <summary>
        /// Configures the selected sound plugin
        /// </summary>
        /// <param name="sender">
        /// The sender
        /// </param>
        /// <param name="e">
        /// Event args
        /// </param>
        private void ButtonSoundConfigClick(object sender, EventArgs e)
        {
            this.ConfigurePlugin<ISoundPlugin>(this.comboBoxSound);
        }

        /// <summary>
        /// Configures the plugin of a given combo box
        /// </summary>
        /// <typeparam name="T">
        /// Type of plugin to configure
        /// </typeparam>
        /// <param name="comboBox">
        /// Combo box in which plugins are contained
        /// </param>
        private void ConfigurePlugin<T>(ComboBox comboBox) where T : class, IPlugin
        {
            var plugin = comboBox.SelectedValue as Lazy<T, IPluginMetadata>;

            if (plugin != null)
            {
                var parameters = plugin.Value.Configure(PluginManager.Instance.GetPluginConfiguration(plugin.Value));

                if (parameters != null)
                {
                    PluginManager.Instance.SavePluginConfiguration(parameters, plugin.Value);
                }
            }
        }

        /// <summary>
        ///     Loads plugins inside of combos
        /// </summary>
        private void BindAssembliesToComboBox()
        {
            this.BindPluginsToComboBox(this.comboBoxSound, PluginManager.Instance.SoundPlugins);
            this.BindPluginsToComboBox(this.comboBoxGraphics, PluginManager.Instance.GraphicsPlugins);
            this.BindPluginsToComboBox(this.comboBoxKeyInput, PluginManager.Instance.KeyboardPlugins);
        }

        /// <summary>
        /// Binds the collections of plugins to the combo box
        /// </summary>
        /// <typeparam name="T">Type of plugin</typeparam>
        /// <param name="comboBox">The destination combo box</param>
        /// <param name="pluginCollection">The collection of plugins</param>
        private void BindPluginsToComboBox<T>(ComboBox comboBox, IEnumerable<Lazy<T, IPluginMetadata>> pluginCollection)
            where T : class, IPlugin
        {
            var plugins = pluginCollection.ToDictionary(x => x, x => x.Metadata.Description);
            comboBox.DataSource = new BindingSource(plugins, null);
            comboBox.DisplayMember = "Value";
            comboBox.ValueMember = "Key";
        }

        #endregion
    }
}