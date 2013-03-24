using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using C8POC.Interfaces;

namespace C8POC
{
    /// <summary>
    /// Class managing plugins, 
    /// It contains the plugins loaded and which ones are selected
    /// </summary>
    public class PluginManager
    {
        #region Properties

        private static PluginManager instance;

        /// <summary>
        /// Gets or sets the list of graphic plugins it could be done with IEnumerable<Lazy<IGraphicsPlugin>>
        /// </summary>
        [ImportMany(typeof (IGraphicsPlugin))]
        public IEnumerable<IGraphicsPlugin> GraphicsPlugins { get; set; }

        /// <summary>
        /// Gets or sets the list of keyboard plugins
        /// </summary>
        [ImportMany(typeof (IKeyboardPlugin))]
        public IEnumerable<IKeyboardPlugin> KeyboardPlugins { get; set; }

        /// <summary>
        /// Gets or sets the list of sound plugins
        /// </summary>
        [ImportMany(typeof (ISoundPlugin))]
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

        #region Constructor and Methods

        private PluginManager()
        {
            LoadPluginsFromAssemblies();
        }

        public static PluginManager Instance
        {
            get { return instance ?? (instance = new PluginManager()); }
        }

        private void LoadPluginsFromAssemblies()
        {
            //Approach with MEF
            var catalog = new DirectoryCatalog("Plugins", "*.dll");
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            if (SoundPlugins.Any())
            {
                this.SelectedSoundPlugin = SoundPlugins.FirstOrDefault();
            }

            if(GraphicsPlugins.Any())
            {
                this.SelectedGraphicsPlugin = GraphicsPlugins.FirstOrDefault();
            }

            if (KeyboardPlugins.Any())
            {
                this.SelectedKeyboardPlugin = KeyboardPlugins.FirstOrDefault();
            }
        }

        #endregion
    }
}