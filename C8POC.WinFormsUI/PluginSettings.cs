using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using C8POC.Interfaces;

namespace C8POC.WinFormsUI
{
    public partial class PluginSettings : Form
    {
        public PluginSettings()
        {
            InitializeComponent();
            LoadPluginAssemblies();
        }

        private void LoadPluginAssemblies()
        {
            LoadPluginsInCombo(PluginManager.Instance.SoundPlugins, comboBoxSound);
            LoadPluginsInCombo(PluginManager.Instance.GraphicsPlugins, comboBoxGraphics);
            LoadPluginsInCombo(PluginManager.Instance.KeyboardPlugins, comboBoxKeyInput);
        }

        private void LoadPluginsInCombo(IEnumerable<IPlugin> pluginList, ComboBox combo)
        {
            if (pluginList.Any())
            {
                Dictionary<IPlugin, string> plugins = pluginList.ToDictionary(x => x, x => x.PluginDescription);
                BindDictionaryToComboBox(combo, plugins);
            }
        }

        private static void BindDictionaryToComboBox(ComboBox combo, Dictionary<IPlugin, string> plugins)
        {
            combo.DataSource = new BindingSource(plugins, null);
            combo.DisplayMember = "Value";
            combo.ValueMember = "Key";
        }

        private void buttonSoundConfig_Click(object sender, EventArgs e)
        {
            ConfigurePlugin(comboBoxSound);
        }

        private void buttonGraphicsConfig_Click(object sender, EventArgs e)
        {
            ConfigurePlugin(comboBoxGraphics);
        }

        private void buttonKeyInputConfig_Click(object sender, EventArgs e)
        {
            ConfigurePlugin(comboBoxKeyInput);
        }

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