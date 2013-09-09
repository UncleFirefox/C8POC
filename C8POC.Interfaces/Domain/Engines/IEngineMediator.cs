// -----------------------------------------------------------------------
// <copyright file="IEngineMediator.cs" company="AlFranco">
// Albert Rodriguez Franco 2013
// </copyright>
// -----------------------------------------------------------------------

namespace C8POC.Interfaces.Domain.Engines
{
    using C8POC.Interfaces.Domain.Entities;
    using C8POC.Interfaces.Domain.Plugins;
    using C8POC.Interfaces.Infrastructure.Services;

    /// <summary>
    /// Represents the central piece of the engine
    /// Deals with execution and input/output
    /// </summary>
    public interface IEngineMediator
    {
        /// <summary>
        /// Gets or sets the machine state.
        /// </summary>
        IMachineState MachineState { get; set; }

        /// <summary>
        /// Gets or sets the execution engine.
        /// </summary>
        IExecutionEngine ExecutionEngine { get; set; }

        /// <summary>
        /// Gets or sets the input output engine.
        /// </summary>
        IInputOutputEngine InputOutputEngine { get; set; }

        /// <summary>
        /// Gets or sets the configuration engine.
        /// </summary>
        IConfigurationEngine ConfigurationEngine { get; set; }

        /// <summary>
        /// Gets or sets the rom service.
        /// </summary>
        IRomService RomService { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether if the emulator should be running, important to control the game loop
        /// </summary>
        bool IsRunning { get; set; }

        /// <summary>
        /// Loads the ROM into the engine
        /// </summary>
        /// <param name="filePath">
        /// Full path of the ROM
        /// </param>
        void LoadRomToEngine(string filePath);

        /// <summary>
        /// Starts the execution of the engine
        /// </summary>
        void StartEmulation();

        /// <summary>
        /// Stops the execution of the engine
        /// </summary>
        void StopEmulation();

        /// <summary>
        /// Sends the draw event to the input/output engine
        /// </summary>
        void DrawGraphics();

        /// <summary>
        /// Sends the generate sound event to the input/output engine
        /// </summary>
        void GenerateSound();

        /// <summary>
        /// Tells the input/output engine to activate the plugins
        /// </summary>
        void StartPluginsExecution();

        /// <summary>
        /// Tells the input/output engine to deactivate the plugins
        /// </summary>
        void StopPluginsExecution();

        /// <summary>
        /// The get saved plugin name space of type.
        /// </summary>
        /// <typeparam name="T">
        /// Type of plugin
        /// </typeparam>
        /// <returns>
        /// Full namespace of the saved plugin
        /// </returns>
        string GetSavedPluginNameSpaceOfType<T>() where T : class, IPlugin;
    }
}
