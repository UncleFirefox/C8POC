// --------------------------------------------------------------------------------------------------------------------
// <copyright file="C8Engine.cs" company="">
//   
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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using C8POC.Interfaces;
    using C8POC.Properties;

    /// <summary>
    /// Engine for emulator and central piece of this application
    /// </summary>
    public class C8Engine
    {
        #region Engine constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="C8Engine"/> class. 
        /// </summary>
        public C8Engine()
            : this(new C8MachineState(), new OpcodeProcessor())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="C8Engine"/> class. 
        /// Engine constructor with an injected machine state
        /// </summary>
        /// <param name="machineState">
        /// An injected machine state
        /// </param>
        /// <param name="opcodeProcessor"> An injected opcode processor </param>
        public C8Engine(IMachineState machineState, IOpcodeProcessor opcodeProcessor)
        {
            // Injects the machine state
            this.machineState = machineState;

            //Inject opcode processor and its machine state
            this.opcodeProcessor = opcodeProcessor;
            this.opcodeProcessor.MachineState = machineState;

            // The InstructionMap is loaded once!!
            this.SetUpInstructionMap();

            //Loads the plugins
            this.LoadPlugins();
        }

        #endregion

        #region Emulator Properties

        /// <summary>
        /// Gets or sets a value indicating whether if the emulator should be running, important to control the game loop
        /// </summary>
        private bool IsRunning { get; set; }

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
        /// Loaded graphics plugin
        /// </summary>
        private IGraphicsPlugin SelectedGraphicsPlugin { get; set; }

        /// <summary>
        /// Loaded sound plugin
        /// </summary>
        private ISoundPlugin SelectedSoundPlugin { get; set; }

        /// <summary>
        /// Loaded Keyboard plugin
        /// </summary>
        private IKeyboardPlugin SelectedKeyboardPlugin { get; set; }

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
            machineState.Keys[keyIndex] = true;
        }

        /// <summary>
        /// Unsets a pressed key
        /// </summary>
        /// <param name="keyIndex">The released key code</param>
        public void KeyUp(byte keyIndex)
        {
            machineState.Keys[keyIndex] = false;
        }

        /// <summary>
        /// Raises the draw event
        /// </summary>
        private void DrawGraphics()
        {
            if (ScreenChanged != null)
            {
                ScreenChanged(((BitArray) machineState.Graphics.Clone()));
            }
        }

        /// <summary>
        /// Raises the sound event
        /// </summary>
        private void GenerateSound()
        {
            if (SoundGenerated != null)
            {
                SoundGenerated();
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
                    PluginManager.Instance.GetPluginConfiguration(this.SelectedGraphicsPlugin));
            }

            if (this.SelectedKeyboardPlugin != null)
            {
                this.SelectedKeyboardPlugin.EnablePlugin(
                    PluginManager.Instance.GetPluginConfiguration(this.SelectedKeyboardPlugin));
            }

            if (this.SelectedSoundPlugin != null)
            {
                this.SelectedSoundPlugin.EnablePlugin(
                    PluginManager.Instance.GetPluginConfiguration(this.SelectedSoundPlugin));
            }
        }

        /// <summary>
        /// Loads the plugins
        /// </summary>
        private void LoadPlugins()
        {
            this.SelectedGraphicsPlugin = PluginManager.Instance.GetPluginByNameSpace<IGraphicsPlugin>(Properties.Settings.Default.SelectedGraphicsPlugin);
            this.SelectedSoundPlugin = PluginManager.Instance.GetPluginByNameSpace<ISoundPlugin>(Properties.Settings.Default.SelectedSoundPlugin);
            this.SelectedKeyboardPlugin = PluginManager.Instance.GetPluginByNameSpace<IKeyboardPlugin>(Properties.Settings.Default.SelectedKeyboardPlugin);

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

        #endregion

        #region Engine Actions

        /// <summary>
        /// Loads the emulator
        /// </summary>
        /// <param name="filePath">The rom full path</param>
        public void LoadEmulator(string filePath)
        {
            Initialize();
            LoadGame(filePath);
        }

        /// <summary>
        /// Starts the emulation using a delegate
        /// </summary>
        public void StartEmulation()
        {
            if (!machineState.HasRomLoaded())
            {
                return;
            }

            this.StartPluginsExecution();

            var engineExecutionTask = new Task(EmulatorTask);
            engineExecutionTask.ContinueWith(task => StopPluginsExecution());
            engineExecutionTask.Start();
        }

        private void EmulatorTask()
        {
            this.IsRunning = true;

            var cycleStopWatch = new Stopwatch();
            var millisecondsperframe = 1.0/(Settings.Default.FramesPerSecond)*1000.0;

            // Gets to the emulator loop
            while (this.IsRunning)
            {
                cycleStopWatch.Restart();

                EmulatorLoop();

                Thread.Sleep((int) Math.Max(0.0,
                                            millisecondsperframe - cycleStopWatch.ElapsedMilliseconds)
                    );
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
            if (machineState.DelayTimer > 0)
            {
                --machineState.DelayTimer;
            }

            if (machineState.SoundTimer > 0)
            {
                if (machineState.SoundTimer == 1)
                {
                    GenerateSound();
                }

                --machineState.SoundTimer;
            }
        }

        /// <summary>
        /// Obtains the opcode and executes the action associated to it
        /// </summary>
        private void ProcessOpcode()
        {
            try
            {
                instructionMap[(ushort) (machineState.CurrentOpcode & 0xF000)]();
            }
            catch (KeyNotFoundException)
            {
                throw new Exception(string.Format("Instruction with Opcode {0:X} is not implemented",
                                                  machineState.CurrentOpcode));
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
                    this.machineState.Memory[C8Constants.StartRomAddress + i] = (byte) rom.ReadByte();
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
            instructionMap.Add(0x0000, GoToRoutineStartingWithZero);
            instructionMap.Add(0x00E0, opcodeProcessor.ClearScreen);
            instructionMap.Add(0x00EE, opcodeProcessor.ReturnFromSubRoutine);
            instructionMap.Add(0x1000, opcodeProcessor.Jump);
            instructionMap.Add(0x2000, opcodeProcessor.CallAtAdress);
            instructionMap.Add(0x3000, opcodeProcessor.SkipNextInstructionIfRegisterEqualsImmediate);
            instructionMap.Add(0x4000, opcodeProcessor.SkipNextInstructionIfRegisterNotEqualsImmediate);
            instructionMap.Add(0x5000, opcodeProcessor.SkipNextInstructionIfRegisterEqualsRegister);
            instructionMap.Add(0x6000, opcodeProcessor.LoadValueIntoRegister);
            instructionMap.Add(0x7000, opcodeProcessor.AddValueIntoRegister);
            instructionMap.Add(0x8000, GoToArithmeticLogicInstruction);
            instructionMap.Add(0x8001, opcodeProcessor.OrRegistersIntoRegister);
            instructionMap.Add(0x8002, opcodeProcessor.AndRegistersIntoRegiter);
            instructionMap.Add(0x8003, opcodeProcessor.ExclusiveOrIntoRegister);
            instructionMap.Add(0x8004, opcodeProcessor.AddRegistersIntoRegister);
            instructionMap.Add(0x8005, opcodeProcessor.SubstractRegisters);
            instructionMap.Add(0x8006, opcodeProcessor.ShiftRegisterRight);
            instructionMap.Add(0x8007, opcodeProcessor.SubstractRegistersReverse);
            instructionMap.Add(0x800E, opcodeProcessor.ShiftRegisterLeft);
            instructionMap.Add(0x9000, opcodeProcessor.SkipNextInstructionIfRegisterNotEqualsRegister);
            instructionMap.Add(0xA000, opcodeProcessor.LoadIntoIndexRegister);
            instructionMap.Add(0xB000, opcodeProcessor.JumpToV0PlusImmediate);
            instructionMap.Add(0xC000, opcodeProcessor.LoadRandomIntoRegister);
            instructionMap.Add(0xD000, opcodeProcessor.DrawSprite);
            instructionMap.Add(0xE000, GoToSkipRegisterInstruction);
            instructionMap.Add(0xE09E, opcodeProcessor.SkipNextInstructionIfRegisterEqualsKeyPressed);
            instructionMap.Add(0xE0A1, opcodeProcessor.SkipNextInstructionIfRegisterNotEqualsKeyPressed);
            instructionMap.Add(0xF000, GoToMemoryOperationInstruction);
            instructionMap.Add(0xF007, opcodeProcessor.LoadTimerValueIntoRegister);
            instructionMap.Add(0xF00A, opcodeProcessor.LoadKeyIntoRegister);
            instructionMap.Add(0xF015, opcodeProcessor.LoadRegisterIntoDelayTimer);
            instructionMap.Add(0xF018, opcodeProcessor.LoadRegisterIntoSoundTimer);
            instructionMap.Add(0xF01E, opcodeProcessor.AddRegisterToIndexRegister);
            instructionMap.Add(0xF029, opcodeProcessor.LoadFontSpriteLocationFromValueInRegister);
            instructionMap.Add(0xF033, opcodeProcessor.LoadBcdRepresentationFromRegister);
            instructionMap.Add(0xF055, opcodeProcessor.LoadAllRegistersFromValueInRegister);
            instructionMap.Add(0xF065, opcodeProcessor.LoadFromValueInRegisterIntoAllRegisters);
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
                opcodeProcessor.JumpToRoutineAtAdress();
            }
            else
            {
                instructionMap[(ushort) fetchedOpcode]();
            }
        }

        /// <summary>
        /// Discriminates further into the instruction mapper for instructions starting with 0x8
        /// </summary>
        private void GoToArithmeticLogicInstruction()
        {
            var filteredOpcode = (ushort) (machineState.CurrentOpcode & 0xF00F);

            if (filteredOpcode == 0x8000)
            {
                opcodeProcessor.LoadRegisterIntoRegister();
            }
            else
            {
                instructionMap[filteredOpcode]();
            }
        }

        /// <summary>
        /// Discriminates instructions starting with 0xE
        /// </summary>
        private void GoToSkipRegisterInstruction()
        {
            instructionMap[(ushort) (machineState.CurrentOpcode & 0xF0FF)]();
        }

        /// <summary>
        /// Discriminates better for instructionmap opcodes starting with 0xF
        /// </summary>
        private void GoToMemoryOperationInstruction()
        {
            instructionMap[(ushort) (machineState.CurrentOpcode & 0xF0FF)]();
        }

        #endregion

        public static T GetTfromString<T>(string mystring)
        {
            var typeConverter = TypeDescriptor.GetConverter(typeof (T));
            try
            {
                var returnValue = (T)(typeConverter.ConvertFromInvariantString(mystring));
                return returnValue;
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}