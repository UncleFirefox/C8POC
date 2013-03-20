using System;
using System.Collections;
using System.Collections.Generic;
using C8POC.Interfaces;

namespace C8POC.WinFormsUI
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Windows.Forms;
    using System.Linq;

    public partial class PluginSettings : Form
    {
        public PluginSettings()
        {
            this.InitializeComponent();
            this.LoadPluginAssemblies();
        }

        private void LoadPluginAssemblies()
        {
            //Approach with MEF
            PluginContainer builder = new PluginContainer();
            DirectoryCatalog catalog = new DirectoryCatalog("Plugins", "*.dll");
            CompositionContainer container = new CompositionContainer(catalog);
            container.ComposeParts(builder);

            LoadPluginsInCombo(builder.SoundPlugins, comboBoxSound);
            LoadPluginsInCombo(builder.GraphicsPlugins,comboBoxGraphics);
            LoadPluginsInCombo(builder.KeyboardPlugins, comboBoxKeyInput);
        }

        private void LoadPluginsInCombo(IEnumerable<IPlugin> pluginList, ComboBox combo)
        {
            if (pluginList.Any())
            {
                var plugins = pluginList.ToDictionary(x => x, x => x.PluginDescription);
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
