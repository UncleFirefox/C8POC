// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowsPluginService.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Class managing plugins,
//   It contains the plugins loaded and which ones are selected
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace C8POC.WinFormsUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Configuration;
    using System.IO;
    using System.Linq;

    using C8POC.Core.Domain.Engines;
    using C8POC.Core.Properties;
    using C8POC.Interfaces.Domain.Plugins;
    using C8POC.Interfaces.Domain.Services;

    /// <summary>
    ///     Class managing plugins,
    /// </summary>
    public class WindowsPluginService : IPluginService
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsPluginService"/> class. 
        /// </summary>
        public WindowsPluginService()
        {
            this.LoadPluginsFromAssemblies();
        }

        #endregion

        #region Public Properties

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the list of graphic plugins it could be done with IEnumerable lazy loading
        /// </summary>
        [ImportMany(typeof(IGraphicsPlugin))]
        private IEnumerable<Lazy<IGraphicsPlugin, IPluginMetadata>> GraphicsPlugins { get; set; }

        /// <summary>
        ///     Gets or sets the list of keyboard plugins
        /// </summary>
        [ImportMany(typeof(IKeyboardPlugin))]
        private IEnumerable<Lazy<IKeyboardPlugin, IPluginMetadata>> KeyboardPlugins { get; set; }

        /// <summary>
        ///     Gets or sets the list of sound plugins
        /// </summary>
        [ImportMany(typeof(ISoundPlugin))]
        private IEnumerable<Lazy<ISoundPlugin, IPluginMetadata>> SoundPlugins { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the saved configuration of the engine
        /// </summary>
        /// <returns>
        ///     Dictionary containing the configuration
        /// </returns>
        public IDictionary<string, string> GetEngineConfiguration()
        {
            string configurationFullPath = this.GetClassConfigurationFullPath(typeof(C8Engine));

            if (File.Exists(configurationFullPath))
            {
                var map = new ExeConfigurationFileMap { ExeConfigFilename = configurationFullPath };
                Configuration engineconfig = ConfigurationManager.OpenMappedExeConfiguration(
                    map, ConfigurationUserLevel.None);

                return this.GetDictionaryFromAppSettings(engineconfig.AppSettings);
            }

            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets a plugin with a given Name Space
        /// </summary>
        /// <typeparam name="T">
        /// Plugin type for the parameter
        /// </typeparam>
        /// <param name="nameSpace">
        /// The name space
        /// </param>
        /// <returns>
        /// A typed plugin or null if no plugins are found
        /// </returns>
        public T GetPluginByNameSpace<T>(string nameSpace) where T : class, IPlugin
        {
            Type type = typeof(T);
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
        /// <param name="plugin">
        /// The plugin to get the configuration from
        /// </param>
        /// <returns>
        /// A dictionary of parameters for the configuration
        /// </returns>
        public IDictionary<string, string> GetPluginConfiguration(IPlugin plugin)
        {
            string configurationFullPath = this.GetClassConfigurationFullPath(plugin.GetType());

            if (File.Exists(configurationFullPath))
            {
                var map = new ExeConfigurationFileMap { ExeConfigFilename = configurationFullPath };
                Configuration pluginConfig = ConfigurationManager.OpenMappedExeConfiguration(
                    map, ConfigurationUserLevel.None);

                return this.GetDictionaryFromAppSettings(pluginConfig.AppSettings);
            }

            return plugin.GetDefaultPluginConfiguration();
        }

        /// <summary>
        /// The get plugins of type.
        /// </summary>
        /// <typeparam name="T"> Type of plugin
        /// </typeparam>
        /// <returns>
        /// The requested list of plugins
        /// </returns>
        public IEnumerable<Lazy<T, IPluginMetadata>> GetPluginsOfType<T>() where T : class, IPlugin
        {
            Type type = typeof(T);

            if (type == typeof(IGraphicsPlugin))
            {
                return (IEnumerable<Lazy<T, IPluginMetadata>>)this.GraphicsPlugins;
            }

            if (type == typeof(ISoundPlugin))
            {
                return (IEnumerable<Lazy<T, IPluginMetadata>>)this.SoundPlugins;
            }

            if (type == typeof(IKeyboardPlugin))
            {
                return (IEnumerable<Lazy<T, IPluginMetadata>>)this.KeyboardPlugins;
            }

            return null;
        }

        /// <summary>
        ///     Saves the engine configuration
        /// </summary>
        public void SaveEngineConfiguration()
        {
            Configuration engineconfig = this.GetClassConfiguration(typeof(C8Engine));

            foreach (SettingsProperty currentProperty in Settings.Default.Properties)
            {
                engineconfig.AppSettings.Settings.Add(
                    currentProperty.Name, Settings.Default[currentProperty.Name].ToString());
            }

            engineconfig.Save();
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
            Configuration pluginConfig = this.GetClassConfiguration(plugin.GetType());

            foreach (var keyvalue in pluginConfiguration)
            {
                pluginConfig.AppSettings.Settings.Add(keyvalue.Key, keyvalue.Value);
            }

            pluginConfig.Save();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the class configuration file
        /// </summary>
        /// <param name="classType">
        /// The class type
        /// </param>
        /// <returns>
        /// A configuration structure with proper paths
        /// </returns>
        private Configuration GetClassConfiguration(Type classType)
        {
            var map = new ExeConfigurationFileMap { ExeConfigFilename = this.GetClassConfigurationFullPath(classType) };
            Configuration engineconfig = ConfigurationManager.OpenMappedExeConfiguration(
                map, ConfigurationUserLevel.None);

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
            string configurationFileName = string.Format(
                "{0}{1}", typeOfClass.Assembly.ManifestModule.ScopeName, ".config");
            string configurationFullPath =
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                    @"C8POC\" + configurationFileName);

            return configurationFullPath;
        }

        /// <summary>
        /// Gets a parsed dictionary from appSettings for plugin configuration
        /// </summary>
        /// <param name="appSettings">
        /// Application settings section
        /// </param>
        /// <returns>
        /// A mapped dictionary
        /// </returns>
        private IDictionary<string, string> GetDictionaryFromAppSettings(AppSettingsSection appSettings)
        {
            return appSettings.Settings.Cast<KeyValueConfigurationElement>()
                              .ToDictionary(variable => variable.Key, variable => variable.Value);
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