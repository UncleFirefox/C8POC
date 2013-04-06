namespace C8POC
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;

    using C8POC.Interfaces;

    /// <summary>
    /// Class managing plugins, 
    /// It contains the plugins loaded and which ones are selected
    /// </summary>
    public class PluginManager
    {
        #region Constructor and Methods

        /// <summary>
        /// Prevents a default instance of the <see cref="PluginManager"/> class from being created. 
        /// </summary>
        private PluginManager()
        {
            this.LoadPluginsFromAssemblies();
        }

        /// <summary>
        /// Gets the instance of PluginManager
        /// </summary>
        public static PluginManager Instance
        {
            get { return instance ?? (instance = new PluginManager()); }
        }

        /// <summary>
        /// Gets the assemblies inside the Plugins folder of the exe
        /// </summary>
        private void LoadPluginsFromAssemblies()
        {
            // Approach with MEF
            var catalog = new DirectoryCatalog("Plugins", "*.dll");
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Private instance of Plugin Manager
        /// </summary>
        private static PluginManager instance;

        /// <summary>
        /// Gets or sets the list of graphic plugins it could be done with IEnumerable lazy loading
        /// </summary>
        [ImportMany(typeof(IGraphicsPlugin))]
        public IEnumerable<IGraphicsPlugin> GraphicsPlugins { get; set; }

        /// <summary>
        /// Gets or sets the list of keyboard plugins
        /// </summary>
        [ImportMany(typeof(IKeyboardPlugin))]
        public IEnumerable<IKeyboardPlugin> KeyboardPlugins { get; set; }

        /// <summary>
        /// Gets or sets the list of sound plugins
        /// </summary>
        [ImportMany(typeof(ISoundPlugin))]
        public IEnumerable<ISoundPlugin> SoundPlugins { get; set; }

        #endregion
    }
}