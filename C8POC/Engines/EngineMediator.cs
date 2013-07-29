// -----------------------------------------------------------------------
// <copyright file="EngineMediator.cs" company="adp">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace C8POC.Engines
{
    using C8POC.Interfaces;

    /// <summary>
    /// Represents the central piece of the engine
    /// Deals with execution and input/output
    /// </summary>
    public class EngineMediator : IEngineMediator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EngineMediator"/> class.
        /// </summary>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        /// <param name="executionEngine">
        /// The execution engine.
        /// </param>
        /// <param name="inputOutputEngine">
        /// The input output engine.
        /// </param>
        /// <param name="configurationEngine">
        /// The configuration engine.
        /// </param>
        /// <param name="romService">
        /// The rom service.
        /// </param>
        public EngineMediator(
            IMachineState machineState,
            IExecutionEngine executionEngine,
            IInputOutputEngine inputOutputEngine,
            IConfigurationEngine configurationEngine,
            IRomService romService)
        {
            this.MachineState = machineState;
            this.ExecutionEngine = executionEngine;
            this.InputOutputEngine = inputOutputEngine;
            this.ConfigurationEngine = configurationEngine;
            this.RomService = romService;

            // Assign the mediator to the engines, avoiding circular dependency with IoC
            this.ExecutionEngine.SetMediator(this);
            this.InputOutputEngine.SetMediator(this);
            this.ConfigurationEngine.SetMediator(this);
        }

        /// <summary>
        /// Gets or sets the machine state.
        /// </summary>
        public IMachineState MachineState { get; set; }

        /// <summary>
        /// Gets or sets the execution engine.
        /// </summary>
        public IExecutionEngine ExecutionEngine { get; set; }

        /// <summary>
        /// Gets or sets the input output engine.
        /// </summary>
        public IInputOutputEngine InputOutputEngine { get; set; }

        /// <summary>
        /// Gets or sets the configuration engine.
        /// </summary>
        public IConfigurationEngine ConfigurationEngine { get; set; }

        /// <summary>
        /// Gets or sets the rom service.
        /// </summary>
        public IRomService RomService { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether if the emulator should be running, 
        /// Important to control the game loop
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Loads the ROM into the engine
        /// </summary>
        /// <param name="filePath">
        /// Full path of the ROM
        /// </param>
        public void LoadRomToEngine(string filePath)
        {
            this.MachineState.CleanMachineState();
            this.RomService.LoadRom(filePath, this.MachineState);
        }

        /// <summary>
        /// Starts the execution of the engine
        /// </summary>
        public void StartEmulation()
        {
            if (!this.MachineState.HasRomLoaded())
            {
                return;
            }

            this.ExecutionEngine.StartEmulatorExecution();
        }

        /// <summary>
        /// Stops the execution of the engine
        /// </summary>
        public void StopEmulation()
        {
            this.IsRunning = false;
        }

        /// <summary>
        /// Sends the draw event to the input/output engine
        /// </summary>
        public void DrawGraphics()
        {
            this.InputOutputEngine.DrawGraphics();
        }

        /// <summary>
        /// Sends the generate sound event to the input/output engine
        /// </summary>
        public void GenerateSound()
        {
            this.InputOutputEngine.GenerateSound();
        }

        /// <summary>
        /// Tells the input/output engine to activate the plugins
        /// </summary>
        public void StartPluginsExecution()
        {
            this.InputOutputEngine.StartPluginsExecution();
        }

        /// <summary>
        /// Tells the input/output engine to deactivate the plugins
        /// </summary>
        public void StopPluginsExecution()
        {
            this.InputOutputEngine.StopPluginsExecution();
        }

        /// <summary>
        /// The get saved plugin name space of type.
        /// </summary>
        /// <typeparam name="T">
        /// Type of plugin
        /// </typeparam>
        /// <returns>
        /// Full namespace of the saved plugin
        /// </returns>
        public string GetSavedPluginNameSpaceOfType<T>() where T : class, IPlugin
        {
            var type = typeof(T);

            if (type == typeof(IGraphicsPlugin))
            {
                return
                    this.ConfigurationEngine.GetConfigurationKeyOfType<string>(
                        ConfigurationParameters.SelectedGraphicsPlugin);
            }

            if (type == typeof(ISoundPlugin))
            {
                return
                    this.ConfigurationEngine.GetConfigurationKeyOfType<string>(
                        ConfigurationParameters.SelectedSoundPlugin);
            }

            if (type == typeof(IKeyboardPlugin))
            {
                return
                    this.ConfigurationEngine.GetConfigurationKeyOfType<string>(
                        ConfigurationParameters.SelectedKeyboardPlugin);
            }

            return null;
        }
    }
}
