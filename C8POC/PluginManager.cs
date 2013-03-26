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
        /// Disables the selected plugins
        /// </summary>
        public void StopPluginsExecution()
        {
            if (this.SelectedGraphicsPlugin != null)
            {
                this.SelectedGraphicsPlugin.DisablePlugin();
            }

            if (this.SelectedKeyboardPlugin != null)
            {
                this.SelectedKeyboardPlugin.DisablePlugin();
            }

            if (this.SelectedSoundPlugin != null)
            {
                this.SelectedSoundPlugin.DisablePlugin();
            }
        }

        /// <summary>
        /// Enables all the plugins
        /// </summary>
        public void StartPluginsExecution()
        {
            if (this.SelectedGraphicsPlugin != null)
            {
                this.SelectedGraphicsPlugin.EnablePlugin();
            }

            if (this.SelectedKeyboardPlugin != null)
            {
                this.SelectedKeyboardPlugin.EnablePlugin();
            }

            if (this.SelectedSoundPlugin != null)
            {
                this.SelectedSoundPlugin.EnablePlugin();
            }
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

            if (this.SoundPlugins.Any())
            {
                this.SelectedSoundPlugin = this.SoundPlugins.FirstOrDefault();
            }

            if (this.GraphicsPlugins.Any())
            {
                this.SelectedGraphicsPlugin = this.GraphicsPlugins.FirstOrDefault();
            }

            if (this.KeyboardPlugins.Any())
            {
                this.SelectedKeyboardPlugin = this.KeyboardPlugins.FirstOrDefault();
            }
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

        /// <summary>
        /// Gets or sets the currently assigned Sound Plugin for execution
        /// </summary>
        public ISoundPlugin SelectedSoundPlugin { get; set; }

        /// <summary>
        /// Gets or sets the currently assigned Graphics Plugin for execution
        /// </summary>
        public IGraphicsPlugin SelectedGraphicsPlugin { get; set; }

        /// <summary>
        /// Gets or sets the currently assigned Keyboard Plugin for execution
        /// </summary>
        public IKeyboardPlugin SelectedKeyboardPlugin { get; set; }

        #endregion
    }
}