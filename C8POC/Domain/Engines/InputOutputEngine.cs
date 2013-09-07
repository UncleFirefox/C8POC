// -----------------------------------------------------------------------
// <copyright file="InputOutputEngine.cs" company="AlFranco">
// Albert Rodriguez Franco 2013
// </copyright>
// -----------------------------------------------------------------------

namespace C8POC.Domain.Engines
{
    using System.Collections;
    using System.Linq;

    using C8POC.Interfaces;
    using C8POC.Interfaces.Domain.Engines;
    using C8POC.Interfaces.Domain.Plugins;
    using C8POC.Interfaces.Domain.Services;

    /// <summary>
    /// Represents the engine in charge of input/output operations
    /// </summary>
    public class InputOutputEngine : IInputOutputEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputOutputEngine"/> class.
        /// </summary>
        /// <param name="pluginService">
        /// The plugin service.
        /// </param>
        public InputOutputEngine(IPluginService pluginService)
        {
            this.PluginService = pluginService;
        }

        /// <summary>
        /// The screen changed.
        /// </summary>
        public event ScreenChangeEventHandler ScreenChanged;

        /// <summary>
        /// The sound generated.
        /// </summary>
        public event SoundGenerateEventHandler SoundGenerated;

        /// <summary>
        /// Gets or sets the engine mediator.
        /// </summary>
        public IEngineMediator EngineMediator { get; set; }

        /// <summary>
        /// Gets or sets the plugin service.
        /// </summary>
        public IPluginService PluginService { get; set; }

        /// <summary>
        /// Gets or sets a loaded graphics plugin
        /// </summary>
        public IGraphicsPlugin SelectedGraphicsPlugin { get; set; }

        /// <summary>
        /// Gets or sets a loaded sound plugin
        /// </summary>
        public ISoundPlugin SelectedSoundPlugin { get; set; }

        /// <summary>
        /// Gets or sets a loaded Keyboard plugin
        /// </summary>
        public IKeyboardPlugin SelectedKeyboardPlugin { get; set; }

        /// <summary>
        /// Sets the mediator for the engine
        /// </summary>
        /// <param name="engineMediator">
        /// The engine mediator.
        /// </param>
        public void SetMediator(IEngineMediator engineMediator)
        {
            this.EngineMediator = engineMediator;
            this.LoadPlugins();
        }

        /// <summary>
        /// Loads the plugins based on the configuration of the system
        /// </summary>
        public void LoadPlugins()
        {
            this.UnLinkPluginEvents();

            var graphicsNameSpaceSavedPlugin = this.EngineMediator.GetSavedPluginNameSpaceOfType<IGraphicsPlugin>();
            this.SelectedGraphicsPlugin =
                !string.IsNullOrEmpty(graphicsNameSpaceSavedPlugin) ? this.PluginService.GetPluginByNameSpace<IGraphicsPlugin>(graphicsNameSpaceSavedPlugin) : null;

            var soundNameSpaceSavedPlugin = this.EngineMediator.GetSavedPluginNameSpaceOfType<ISoundPlugin>();
            this.SelectedSoundPlugin =
                !string.IsNullOrEmpty(soundNameSpaceSavedPlugin) ? this.PluginService.GetPluginByNameSpace<ISoundPlugin>(soundNameSpaceSavedPlugin) : null;

            var keyboardNameSpaceSavedPlugin = this.EngineMediator.GetSavedPluginNameSpaceOfType<IKeyboardPlugin>();
            this.SelectedKeyboardPlugin =
                !string.IsNullOrEmpty(keyboardNameSpaceSavedPlugin) ? this.PluginService.GetPluginByNameSpace<IKeyboardPlugin>(keyboardNameSpaceSavedPlugin) : null;

            this.LinkPluginEvents();
        }

        /// <summary>
        /// Sets a pressed key
        /// </summary>
        /// <param name="keyIndex">The pressed key code</param>
        public void KeyDown(byte keyIndex)
        {
            this.EngineMediator.MachineState.Keys[keyIndex] = true;
        }

        /// <summary>
        /// Unsets a pressed key
        /// </summary>
        /// <param name="keyIndex">The released key code</param>
        public void KeyUp(byte keyIndex)
        {
            this.EngineMediator.MachineState.Keys[keyIndex] = false;
        }

        /// <summary>
        /// Raises the draw event
        /// </summary>
        public void DrawGraphics()
        {
            if (this.ScreenChanged != null)
            {
                this.ScreenChanged((BitArray)this.EngineMediator.MachineState.Graphics.Clone());
            }
        }

        /// <summary>
        /// Raises the sound event
        /// </summary>
        public void GenerateSound()
        {
            if (this.SoundGenerated != null)
            {
                this.SoundGenerated();
            }
        }

        /// <summary>
        /// Starts the execution of plugins
        /// </summary>
        public void StartPluginsExecution()
        {
            if (this.SelectedGraphicsPlugin != null)
            {
                this.SelectedGraphicsPlugin.EnablePlugin(
                    this.PluginService.GetPluginConfiguration(this.SelectedGraphicsPlugin));
            }

            if (this.SelectedKeyboardPlugin != null)
            {
                this.SelectedKeyboardPlugin.EnablePlugin(
                    this.PluginService.GetPluginConfiguration(this.SelectedKeyboardPlugin));
            }

            if (this.SelectedSoundPlugin != null)
            {
                this.SelectedSoundPlugin.EnablePlugin(
                    this.PluginService.GetPluginConfiguration(this.SelectedSoundPlugin));
            }
        }

        /// <summary>
        /// Stops the execution of plugins
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
        /// The link plugin events.
        /// </summary>
        private void LinkPluginEvents()
        {
            if (this.SelectedGraphicsPlugin != null)
            {
                this.ScreenChanged += this.SelectedGraphicsPlugin.Draw;
                this.SelectedGraphicsPlugin.GraphicsExit += this.EngineMediator.StopEmulation;
            }

            if (this.SelectedSoundPlugin != null)
            {
                this.SoundGenerated += this.SelectedSoundPlugin.GenerateSound;
            }

            if (this.SelectedKeyboardPlugin != null)
            {
                this.SelectedKeyboardPlugin.KeyUp += this.KeyUp;
                this.SelectedKeyboardPlugin.KeyDown += this.KeyDown;
                this.SelectedKeyboardPlugin.KeyStopEmulation += this.EngineMediator.StopEmulation;
            }
        }

        /// <summary>
        /// Unlinks all the events in the Engine class when plugins are reloaded
        /// </summary>
        private void UnLinkPluginEvents()
        {
            if (this.ScreenChanged != null)
            {
                this.ScreenChanged.GetInvocationList()
                    .ToList()
                    .ForEach(x => this.ScreenChanged -= x as ScreenChangeEventHandler);
            }

            if (this.SoundGenerated != null)
            {
                this.SoundGenerated.GetInvocationList()
                    .ToList()
                    .ForEach(x => this.SoundGenerated -= x as SoundGenerateEventHandler);
            }
        }
    }
}
