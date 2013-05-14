// --------------------------------------------------------------------------------------------------------------------
// <copyright file="C8Engine.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Engine for emulator and central piece of this application
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using C8POC.Interfaces;
    using C8POC.Properties;

    /// <summary>
    /// Engine for emulator and central piece of this application
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Opcode is a correct word...")]
    public class C8Engine
    {
        #region Emulator Properties

        /// <summary>
        /// Gets the plugin service.
        /// </summary>
        public IPluginService PluginService { get; private set; }

        /// <summary>
        /// State of the machine (including registers etc)
        /// </summary>
        private IMachineState machineState;

        /// <summary>
        /// Object processing opcode
        /// </summary>
        private IOpcodeProcessor opcodeProcessor;

        /// <summary>
        /// Contains the mapping between an opcode and the function that should be executed
        /// </summary>
        private Dictionary<ushort, Action> instructionMap = new Dictionary<ushort, Action>();

        /// <summary>
        /// Gets or sets a value indicating whether if the emulator should be running, important to control the game loop
        /// </summary>
        private bool IsRunning { get; set; }

        /// <summary>
        /// Gets or sets a loaded graphics plugin
        /// </summary>
        private IGraphicsPlugin SelectedGraphicsPlugin { get; set; }

        /// <summary>
        /// Gets or sets a loaded sound plugin
        /// </summary>
        private ISoundPlugin SelectedSoundPlugin { get; set; }

        /// <summary>
        /// Gets or sets a loaded Keyboard plugin
        /// </summary>
        private IKeyboardPlugin SelectedKeyboardPlugin { get; set; }

        #endregion

        #region Engine constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="C8Engine"/> class. 
        /// </summary>
        public C8Engine()
            : this(new C8MachineState(), new OpcodeProcessor(), new PluginService())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="C8Engine"/> class. 
        /// Engine constructor with an injected machine state
        /// </summary>
        /// <param name="machineState">
        /// An injected machine state
        /// </param>
        /// <param name="opcodeProcessor">
        /// An injected opcode processor 
        /// </param>
        /// <param name="pluginService">
        /// The plugin Service.
        /// </param>
        public C8Engine(IMachineState machineState, IOpcodeProcessor opcodeProcessor, IPluginService pluginService)
        {
            // Injects the machine state
            this.machineState = machineState;

            // Inject opcode processor and its machine state
            this.opcodeProcessor = opcodeProcessor;
            this.opcodeProcessor.MachineState = machineState;

            // Inject the plugin service
            this.PluginService = pluginService;

            // The InstructionMap is loaded once!!
            this.SetUpInstructionMap();

            // Loads the user saved configuration
            this.LoadSavedEngineSettings();

            // Loads the plugins
            this.LoadPlugins();
        }

        #endregion

        #region I/O Handling

        /// <summary>
        /// Event handler for screen changes
        /// </summary>
        /// <param name="graphics">Graphics array</param>
        public delegate void ScreenChangeEventHandler(BitArray graphics);

        /// <summary>
        /// Event that will be raised every that that the screen needs to be refreshed
        /// </summary>
        public event ScreenChangeEventHandler ScreenChanged;

        /// <summary>
        /// Event handler for sound generation
        /// </summary>
        public delegate void SoundGenerateEventHandler();

        /// <summary>
        /// Event that will be raised when a beep is generated
        /// </summary>
        public event SoundGenerateEventHandler SoundGenerated;

        /// <summary>
        /// Sets a pressed key
        /// </summary>
        /// <param name="keyIndex">The pressed key code</param>
        public void KeyDown(byte keyIndex)
        {
            this.machineState.Keys[keyIndex] = true;
        }

        /// <summary>
        /// Unsets a pressed key
        /// </summary>
        /// <param name="keyIndex">The released key code</param>
        public void KeyUp(byte keyIndex)
        {
            this.machineState.Keys[keyIndex] = false;
        }

        /// <summary>
        /// Raises the draw event
        /// </summary>
        private void DrawGraphics()
        {
            if (this.ScreenChanged != null)
            {
                this.ScreenChanged((BitArray)this.machineState.Graphics.Clone());
            }
        }

        /// <summary>
        /// Raises the sound event
        /// </summary>
        private void GenerateSound()
        {
            if (this.SoundGenerated != null)
            {
                this.SoundGenerated();
            }
        }

        #endregion

        #region Plugin Handling

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
        /// Loads the plugins
        /// </summary>
        public void LoadPlugins()
        {
            this.UnLinkPluginEvents();

            this.SelectedGraphicsPlugin = this.PluginService.GetPluginByNameSpace<IGraphicsPlugin>(Settings.Default.SelectedGraphicsPlugin);
            this.SelectedSoundPlugin = this.PluginService.GetPluginByNameSpace<ISoundPlugin>(Settings.Default.SelectedSoundPlugin);
            this.SelectedKeyboardPlugin = this.PluginService.GetPluginByNameSpace<IKeyboardPlugin>(Settings.Default.SelectedKeyboardPlugin);

            this.LinkPluginEvents();
        }

        /// <summary>
        /// The link plugin events.
        /// </summary>
        private void LinkPluginEvents()
        {
            if (this.SelectedGraphicsPlugin != null)
            {
                this.ScreenChanged += this.SelectedGraphicsPlugin.Draw;
                this.SelectedGraphicsPlugin.GraphicsExit += this.StopEmulator;
            }

            if (this.SelectedSoundPlugin != null)
            {
                this.SoundGenerated += this.SelectedSoundPlugin.GenerateSound;
            }

            if (this.SelectedKeyboardPlugin != null)
            {
                this.SelectedKeyboardPlugin.KeyUp += this.KeyUp;
                this.SelectedKeyboardPlugin.KeyDown += this.KeyDown;
                this.SelectedKeyboardPlugin.KeyStopEmulation += this.StopEmulator;
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

        #endregion

        #region Engine Actions

        /// <summary>
        /// Loads the emulator
        /// </summary>
        /// <param name="filePath">The rom full path</param>
        public void LoadEmulator(string filePath)
        {
            this.Initialize();
            this.LoadGame(filePath);
        }

        /// <summary>
        /// Starts the emulation using a delegate
        /// </summary>
        public void StartEmulation()
        {
            if (!this.machineState.HasRomLoaded())
            {
                return;
            }

            this.StartPluginsExecution();

            var engineExecutionTask = new Task(this.EmulatorTask);
            engineExecutionTask.ContinueWith(task => this.StopPluginsExecution());
            engineExecutionTask.Start();
        }

        /// <summary>
        /// The emulator task.
        /// </summary>
        private void EmulatorTask()
        {
            this.IsRunning = true;

            var cycleStopWatch = new Stopwatch();
            var millisecondsperframe = 1.0 / Settings.Default.FramesPerSecond * 1000.0;

            // Gets to the emulator loop
            while (this.IsRunning)
            {
                cycleStopWatch.Restart();

                this.EmulatorLoop();

                Thread.Sleep((int)Math.Max(0.0, millisecondsperframe - cycleStopWatch.ElapsedMilliseconds));
            }
        }

        /// <summary>
        /// Stops the timer execution
        /// </summary>
        public void StopEmulator()
        {
            this.IsRunning = false;
        }

        /// <summary>
        /// This is where all the action happens its kind of simillar to the GameLoop
        /// </summary>
        private void EmulatorLoop()
        {
            for (int cycleNum = 0; cycleNum < Settings.Default.CyclesPerFrame; cycleNum++)
            {
                this.EmulateCycle();

                if (!this.IsRunning)
                {
                    this.StopEmulator();
                    return;
                }
            }

            if (this.machineState.IsDrawFlagSet)
            {
                this.DrawGraphics();
                this.machineState.IsDrawFlagSet = false;
            }
        }

        /// <summary>
        /// Emulates a cycle
        /// </summary>
        private void EmulateCycle()
        {
            // Get Opcode located at program counter
            this.machineState.FetchOpcode();

            // Processes the opcode
            this.ProcessOpcode();

            // Update timers
            this.UpdateTimers();
        }

        /// <summary>
        /// Timer update every cycle
        /// </summary>
        private void UpdateTimers()
        {
            if (this.machineState.DelayTimer > 0)
            {
                --this.machineState.DelayTimer;
            }

            if (this.machineState.SoundTimer > 0)
            {
                if (this.machineState.SoundTimer == 1)
                {
                    this.GenerateSound();
                }

                --this.machineState.SoundTimer;
            }
        }

        /// <summary>
        /// Obtains the opcode and executes the action associated to it
        /// </summary>
        private void ProcessOpcode()
        {
            try
            {
                this.instructionMap[(ushort)(this.machineState.CurrentOpcode & 0xF000)]();
            }
            catch (KeyNotFoundException)
            {
                throw new Exception(
                    string.Format("Instruction with Opcode {0:X} is not implemented", this.machineState.CurrentOpcode));
            }
        }

        /// <summary>
        /// Loads a Chip8 ROM file in machineState.Memory
        /// </summary>
        /// <param name="filePath">Full path of rom</param>
        private void LoadGame(string filePath)
        {
            if (File.Exists(filePath))
            {
                var rom = new FileStream(filePath, FileMode.Open);

                if (rom.Length == 0)
                {
                    throw new Exception(string.Format("File '{0}' empty or damaged", filePath));
                }

                // Load rom starting at 0x200
                for (int i = 0; i < rom.Length; i++)
                {
                    this.machineState.Memory[C8Constants.StartRomAddress + i] = (byte)rom.ReadByte();
                }

                rom.Close();
            }
            else
            {
                throw new FileNotFoundException(string.Format("The file '{0}' does not exist", filePath));
            }
        }

        /// <summary>
        /// Set the emulator to a clean start state
        /// </summary>
        private void Initialize()
        {
            this.machineState.CleanMachineState();
        }

        /// <summary>
        /// Sets the instruction map up for fast opcode decoding
        /// </summary>
        private void SetUpInstructionMap()
        {
            this.instructionMap.Add(0x0000, this.GoToRoutineStartingWithZero);
            this.instructionMap.Add(0x00E0, this.opcodeProcessor.ClearScreen);
            this.instructionMap.Add(0x00EE, this.opcodeProcessor.ReturnFromSubRoutine);
            this.instructionMap.Add(0x1000, this.opcodeProcessor.Jump);
            this.instructionMap.Add(0x2000, this.opcodeProcessor.CallAtAdress);
            this.instructionMap.Add(0x3000, this.opcodeProcessor.SkipNextInstructionIfRegisterEqualsImmediate);
            this.instructionMap.Add(0x4000, this.opcodeProcessor.SkipNextInstructionIfRegisterNotEqualsImmediate);
            this.instructionMap.Add(0x5000, this.opcodeProcessor.SkipNextInstructionIfRegisterEqualsRegister);
            this.instructionMap.Add(0x6000, this.opcodeProcessor.LoadValueIntoRegister);
            this.instructionMap.Add(0x7000, this.opcodeProcessor.AddValueIntoRegister);
            this.instructionMap.Add(0x8000, this.GoToArithmeticLogicInstruction);
            this.instructionMap.Add(0x8001, this.opcodeProcessor.OrRegistersIntoRegister);
            this.instructionMap.Add(0x8002, this.opcodeProcessor.AndRegistersIntoRegiter);
            this.instructionMap.Add(0x8003, this.opcodeProcessor.ExclusiveOrIntoRegister);
            this.instructionMap.Add(0x8004, this.opcodeProcessor.AddRegistersIntoRegister);
            this.instructionMap.Add(0x8005, this.opcodeProcessor.SubstractRegisters);
            this.instructionMap.Add(0x8006, this.opcodeProcessor.ShiftRegisterRight);
            this.instructionMap.Add(0x8007, this.opcodeProcessor.SubstractRegistersReverse);
            this.instructionMap.Add(0x800E, this.opcodeProcessor.ShiftRegisterLeft);
            this.instructionMap.Add(0x9000, this.opcodeProcessor.SkipNextInstructionIfRegisterNotEqualsRegister);
            this.instructionMap.Add(0xA000, this.opcodeProcessor.LoadIntoIndexRegister);
            this.instructionMap.Add(0xB000, this.opcodeProcessor.JumpToV0PlusImmediate);
            this.instructionMap.Add(0xC000, this.opcodeProcessor.LoadRandomIntoRegister);
            this.instructionMap.Add(0xD000, this.opcodeProcessor.DrawSprite);
            this.instructionMap.Add(0xE000, this.GoToSkipRegisterInstruction);
            this.instructionMap.Add(0xE09E, this.opcodeProcessor.SkipNextInstructionIfRegisterEqualsKeyPressed);
            this.instructionMap.Add(0xE0A1, this.opcodeProcessor.SkipNextInstructionIfRegisterNotEqualsKeyPressed);
            this.instructionMap.Add(0xF000, this.GoToMemoryOperationInstruction);
            this.instructionMap.Add(0xF007, this.opcodeProcessor.LoadTimerValueIntoRegister);
            this.instructionMap.Add(0xF00A, this.opcodeProcessor.LoadKeyIntoRegister);
            this.instructionMap.Add(0xF015, this.opcodeProcessor.LoadRegisterIntoDelayTimer);
            this.instructionMap.Add(0xF018, this.opcodeProcessor.LoadRegisterIntoSoundTimer);
            this.instructionMap.Add(0xF01E, this.opcodeProcessor.AddRegisterToIndexRegister);
            this.instructionMap.Add(0xF029, this.opcodeProcessor.LoadFontSpriteLocationFromValueInRegister);
            this.instructionMap.Add(0xF033, this.opcodeProcessor.LoadBcdRepresentationFromRegister);
            this.instructionMap.Add(0xF055, this.opcodeProcessor.LoadAllRegistersFromValueInRegister);
            this.instructionMap.Add(0xF065, this.opcodeProcessor.LoadFromValueInRegisterIntoAllRegisters);
        }

        /// <summary>
        /// Loads the saved settings by the user
        /// </summary>
        private void LoadSavedEngineSettings()
        {
            var savedSettings = this.PluginService.GetEngineConfiguration();

            var validSettings = savedSettings.Where(x => Settings.Default.Properties.Cast<SettingsProperty>().Any(setting => setting.Name == x.Key));

            foreach (var validSetting in validSettings)
            {
                var valueType = Settings.Default[validSetting.Key].GetType();
                Settings.Default[validSetting.Key] = this.GetValueWithGivenType(validSetting.Value, valueType);
            }
        }

        /// <summary>
        /// The get value with given type.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="requiredType">
        /// The required type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private object GetValueWithGivenType(string value, Type requiredType)
        {
            var foo = TypeDescriptor.GetConverter(requiredType);
            return foo.ConvertFromInvariantString(value);
        }

        #endregion

        #region Instruction Map Set

        /// <summary>
        /// Discriminates further into the instruction mapper for instructions starting with 0x0
        /// </summary>
        private void GoToRoutineStartingWithZero()
        {
            var fetchedOpcode = this.machineState.CurrentOpcode & 0xF0FF;

            if (fetchedOpcode == 0x0000)
            {
                this.opcodeProcessor.JumpToRoutineAtAdress();
            }
            else
            {
                this.instructionMap[(ushort)fetchedOpcode]();
            }
        }

        /// <summary>
        /// Discriminates further into the instruction mapper for instructions starting with 0x8
        /// </summary>
        private void GoToArithmeticLogicInstruction()
        {
            var filteredOpcode = (ushort)(this.machineState.CurrentOpcode & 0xF00F);

            if (filteredOpcode == 0x8000)
            {
                this.opcodeProcessor.LoadRegisterIntoRegister();
            }
            else
            {
                this.instructionMap[filteredOpcode]();
            }
        }

        /// <summary>
        /// Discriminates instructions starting with 0xE
        /// </summary>
        private void GoToSkipRegisterInstruction()
        {
            this.instructionMap[(ushort)(this.machineState.CurrentOpcode & 0xF0FF)]();
        }

        /// <summary>
        /// Discriminates better for instructionmap opcodes starting with 0xF
        /// </summary>
        private void GoToMemoryOperationInstruction()
        {
            this.instructionMap[(ushort)(this.machineState.CurrentOpcode & 0xF0FF)]();
        }

        #endregion
    }
}