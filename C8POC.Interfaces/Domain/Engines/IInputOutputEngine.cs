// -----------------------------------------------------------------------
// <copyright file="IInputOutputEngine.cs" company="AlFranco">
// Albert Rodriguez Franco 2013
// </copyright>
// -----------------------------------------------------------------------

namespace C8POC.Interfaces.Domain.Engines
{
    using System.Collections;

    using C8POC.Interfaces.Domain.Plugins;
    using C8POC.Interfaces.Domain.Services;

    /// <summary>
    /// Event handler for screen changes
    /// </summary>
    /// <param name="graphics">Graphics array</param>
    public delegate void ScreenChangeEventHandler(BitArray graphics);

    /// <summary>
    /// Event handler for sound generation
    /// </summary>
    public delegate void SoundGenerateEventHandler();

    /// <summary>
    /// Represents the engine in charge of input/output operations
    /// </summary>
    public interface IInputOutputEngine : IMediatedEngine
    {
        /// <summary>
        /// Event that will be raised every time that the screen needs to be refreshed
        /// </summary>
        event ScreenChangeEventHandler ScreenChanged;

        /// <summary>
        /// Event that will be raised when a beep is generated
        /// </summary>
        event SoundGenerateEventHandler SoundGenerated;

        /// <summary>
        /// Gets or sets the plugin service.
        /// </summary>
        IPluginService PluginService { get; set; }

        /// <summary>
        /// Gets or sets a loaded graphics plugin
        /// </summary>
        IGraphicsPlugin SelectedGraphicsPlugin { get; set; }

        /// <summary>
        /// Gets or sets a loaded sound plugin
        /// </summary>
        ISoundPlugin SelectedSoundPlugin { get; set; }

        /// <summary>
        /// Gets or sets a loaded Keyboard plugin
        /// </summary>
        IKeyboardPlugin SelectedKeyboardPlugin { get; set; }

        /// <summary>
        /// Loads the plugins based on the configuration of the system
        /// </summary>
        void LoadPlugins();

        /// <summary>
        /// Sets a pressed key
        /// </summary>
        /// <param name="keyIndex">The pressed key code</param>
        void KeyDown(byte keyIndex);

        /// <summary>
        /// Unsets a pressed key
        /// </summary>
        /// <param name="keyIndex">The released key code</param>
        void KeyUp(byte keyIndex);

        /// <summary>
        /// Raises the draw event
        /// </summary>
        void DrawGraphics();

        /// <summary>
        /// Raises the sound event
        /// </summary>
        void GenerateSound();

        /// <summary>
        /// Starts the execution of plugins
        /// </summary>
        void StartPluginsExecution();

        /// <summary>
        /// Stops the execution of plugins
        /// </summary>
        void StopPluginsExecution();
    }
}
