// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginManager.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Class managing plugins,
//   It contains the plugins loaded and which ones are selected
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Configuration;
    using System.IO;
    using System.Linq;

    using C8POC.Interfaces;

    /// <summary>
    ///     Class managing plugins,
    ///     It contains the plugins loaded and which ones are selected
    /// </summary>
    public class PluginManager
    {
        #region Static Fields

        /// <summary>
        ///     Private instance of Plugin Manager
        /// </summary>
        private static PluginManager instance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="PluginManager" /> class from being created.
        /// </summary>
        private PluginManager()
        {
            this.LoadPluginsFromAssemblies();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the instance of PluginManager
        /// </summary>
        public static PluginManager Instance
        {
            get
            {
                return instance ?? (instance = new PluginManager());
            }
        }

        /// <summary>
        ///     Gets or sets the list of graphic plugins it could be done with IEnumerable lazy loading
        /// </summary>
        [ImportMany(typeof(IGraphicsPlugin))]
        public IEnumerable<Lazy<IGraphicsPlugin, IPluginMetadata>> GraphicsPlugins { get; set; }

        /// <summary>
        ///     Gets or sets the list of keyboard plugins
        /// </summary>
        [ImportMany(typeof(IKeyboardPlugin))]
        public IEnumerable<Lazy<IKeyboardPlugin, IPluginMetadata>> KeyboardPlugins { get; set; }

        /// <summary>
        ///     Gets or sets the list of sound plugins
        /// </summary>
        [ImportMany(typeof(ISoundPlugin))]
        public IEnumerable<Lazy<ISoundPlugin, IPluginMetadata>> SoundPlugins { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a plugin with a given Name Space
        /// </summary>
        /// <typeparam name="T">Plugin type for the parameter</typeparam>
        /// <param name="nameSpace">The name space</param>
        /// <returns>A typed plugin or null if no plugins are found</returns>
        public T GetPluginByNameSpace<T>(string nameSpace) where T : class, IPlugin
        {
            var type = typeof(T);
            IPlugin plugin = null;

            if (type == typeof(IGraphicsPlugin) && this.GraphicsPlugins.Any(x => x.Metadata.NameSpace == nameSpace))
            {
                plugin = this.GraphicsPlugins.First(x => x.Metadata.NameSpace == nameSpace).Value;
            }
            else if (type == typeof(ISoundPlugin) && this.SoundPlugins.Any(x => x.Metadata.NameSpace == nameSpace))
            {
                plugin = this.SoundPlugins.First(x => x.Metadata.NameSpace == nameSpace).Value;
            }
            else if (type == typeof(IKeyboardPlugin) && this.KeyboardPlugins.Any(x => x.Metadata.NameSpace == nameSpace))
            {
                plugin = this.KeyboardPlugins.First(x => x.Metadata.NameSpace == nameSpace).Value;
            }

            return (T)plugin;
        }

        /// <summary>
        /// Get the parameters configuration for the specified plugin
        /// </summary>
        /// <param name="plugin">The plugin to get the configuration from</param>
        /// <returns>A dictionary of parameters for the configuration</returns>
        public IDictionary<string, string> GetPluginConfiguration(IPlugin plugin)
        {
            var configurationFullPath = this.GetClassConfigurationFullPath(plugin.GetType());

            if (File.Exists(configurationFullPath))
            {
                var map = new ExeConfigurationFileMap { ExeConfigFilename = configurationFullPath };
                Configuration pluginConfig = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

                return this.GetDictionaryFromAppSettings(pluginConfig.AppSettings);
            }

            return plugin.GetDefaultPluginConfiguration();
        }

        /// <summary>
        /// Save the plugin configuration to a storage
        /// </summary>
        /// <param name="pluginConfiguration">
        /// Dictionary of parameters for the plugin
        /// </param>
        /// <param name="plugin">
        /// The plugin.
        /// </param>
        public void SavePluginConfiguration(IDictionary<string, string> pluginConfiguration, IPlugin plugin)
        {
            var pluginConfig = this.GetClassConfiguration(plugin.GetType());

            foreach (var keyvalue in pluginConfiguration)
            {
                pluginConfig.AppSettings.Settings.Add(keyvalue.Key, keyvalue.Value);
            }

            pluginConfig.Save();
        }

        /// <summary>
        /// Gets the saved configuration of the engine
        /// </summary>
        /// <returns>
        /// Dictionary containing the configuration
        /// </returns>
        public IDictionary<string, string> GetEngineConfiguration()
        {
            var configurationFullPath = this.GetClassConfigurationFullPath(typeof(C8Engine));

            if (File.Exists(configurationFullPath))
            {
                var map = new ExeConfigurationFileMap { ExeConfigFilename = configurationFullPath };
                Configuration engineconfig = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

                return this.GetDictionaryFromAppSettings(engineconfig.AppSettings); 
            }

            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Saves the engine configuration
        /// </summary>
        public void SaveEngineConfiguration()
        {
            var engineconfig = this.GetClassConfiguration(typeof(C8Engine));

            foreach (SettingsProperty currentProperty in Properties.Settings.Default.Properties)
            {
                engineconfig.AppSettings.Settings.Add(currentProperty.Name, Properties.Settings.Default[currentProperty.Name].ToString());
            }

            engineconfig.Save();
        }

        /// <summary>
        /// Gets the class configuration file
        /// </summary>
        /// <param name="classType">The class type</param>
        /// <returns>A configuration structure with proper paths</returns>
        private Configuration GetClassConfiguration(Type classType)
        {
            var map = new ExeConfigurationFileMap { ExeConfigFilename = this.GetClassConfigurationFullPath(classType) };
            Configuration engineconfig = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

            if (engineconfig.HasFile)
            {
                engineconfig.AppSettings.Settings.Clear();
            }

            return engineconfig;
        }

        /// <summary>
        /// Gets the full path of a configuration file of a given class inside a DLL
        /// </summary>
        /// <param name="typeOfClass">
        /// The type Of Class.
        /// </param>
        /// <returns>
        /// Full path to save the plugin
        /// </returns>
        private string GetClassConfigurationFullPath(Type typeOfClass)
        {
            string configurationFileName = string.Format("{0}{1}", typeOfClass.Assembly.ManifestModule.ScopeName, ".config");
            string configurationFullPath =
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"C8POC\" + configurationFileName);

            return configurationFullPath;
        }

        /// <summary>
        /// Gets a parsed dictionary from appSettings for plugin configuration
        /// </summary>
        /// <param name="appSettings">Application settings section</param>
        /// <returns>A mapped dictionary</returns>
        private IDictionary<string, string> GetDictionaryFromAppSettings(AppSettingsSection appSettings)
        {
            return appSettings.Settings.Cast<KeyValueConfigurationElement>().ToDictionary(variable => variable.Key, variable => variable.Value);
        }

        /// <summary>
        ///     Gets the assemblies inside the Plugins folder of the exe
        /// </summary>
        private void LoadPluginsFromAssemblies()
        {
            // Approach with MEF
            var catalog = new DirectoryCatalog("Plugins", "*.dll");
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }

        #endregion
    }
}