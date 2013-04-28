// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginManager.cs" company="AlFranco">
//   Albert Rodríguez Franco 2013
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
        private IDictionary<string, string> GetPluginConfiguration(IPlugin plugin)
        {
            return null;
        }

        /// <summary>
        /// Save the plugin configuration to a storage
        /// </summary>
        /// <param name="pluginConfiguration">Dictionary of parameters for the plugin</param>
        public void SavePluginConfiguration(IDictionary<string, string> pluginConfiguration)
        {
            
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