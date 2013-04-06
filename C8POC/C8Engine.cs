using System.ComponentModel;
using System.Linq;

namespace C8POC
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    using Properties;

    using MicroLibrary;
    using Interfaces;

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
            : this(new C8MachineState())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="C8Engine"/> class. 
        /// Engine constructor with an injected machine state
        /// </summary>
        /// <param name="machState">
        /// An injected machine state
        /// </param>
        public C8Engine(C8MachineState machState)
        {
            // The InstructionMap is loaded once!!
            this.SetUpInstructionMap();

            //Loads the plugins
            this.LoadPlugins();

            // Configure the timer
            this.InitializeTimer();

            // Injects the machine state
            this.machineState = machState;
        }

        #endregion

        #region Emulator Properties

        /// <summary>
        /// Gets or sets a value indicating whether if the emulator should be running, important to control the timer
        /// </summary>
        private bool IsRunning { get; set; }

        /// <summary>
        /// State of the machine (including registers etc)
        /// </summary>
        private C8MachineState machineState;

        /// <summary>
        /// Contains the mapping between an opcode and the function that should be executed
        /// </summary>
        private Dictionary<ushort, Action> instructionMap = new Dictionary<ushort, Action>();

        /// <summary>
        /// Timer in charge of handling the number of cycles per second
        /// </summary>
        private MicroTimer MicroTimer { get; set; }

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

        private void LoadPlugins()
        {
            this.SelectedGraphicsPlugin = PluginManager.Instance.GraphicsPlugins.FirstOrDefault();
            this.SelectedSoundPlugin = PluginManager.Instance.SoundPlugins.FirstOrDefault();
            this.SelectedKeyboardPlugin = PluginManager.Instance.KeyboardPlugins.FirstOrDefault();

            this.LinkPluginEvents();
        }

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
        /// Starts the timer if a correct rom has been loaded
        /// </summary>
        public void StartEmulator()
        {
            if (machineState.HasRomLoaded())
            {
                this.IsRunning = true;
                this.StartPluginsExecution();
                MicroTimer.Start();
            }
        }

        /// <summary>
        /// Stops the timer execution
        /// </summary>
        public void StopEmulator()
        {
            this.IsRunning = false;

            if (MicroTimer.Enabled)
            {
                MicroTimer.Stop();
                this.StopPluginsExecution();
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
            instructionMap.Add(0x00E0, ClearScreen);
            instructionMap.Add(0x00EE, ReturnFromSubRoutine);
            instructionMap.Add(0x1000, Jump);
            instructionMap.Add(0x2000, CallAtAdress);
            instructionMap.Add(0x3000, SkipNextInstructionIfRegisterEqualsImmediate);
            instructionMap.Add(0x4000, SkipNextInstructionIfRegisterNotEqualsImmediate);
            instructionMap.Add(0x5000, SkipNextInstructionIfRegisterEqualsRegister);
            instructionMap.Add(0x6000, LoadValueIntoRegister);
            instructionMap.Add(0x7000, AddValueIntoRegister);
            instructionMap.Add(0x8000, GoToArithmeticLogicInstruction);
            instructionMap.Add(0x8001, OrRegistersIntoRegister);
            instructionMap.Add(0x8002, AndRegistersIntoRegiter);
            instructionMap.Add(0x8003, ExclusiveOrIntoRegister);
            instructionMap.Add(0x8004, AddRegistersIntoRegister);
            instructionMap.Add(0x8005, SubstractRegisters);
            instructionMap.Add(0x8006, ShiftRegisterRight);
            instructionMap.Add(0x8007, SubstractRegistersReverse);
            instructionMap.Add(0x800E, ShiftRegisterLeft);
            instructionMap.Add(0x9000, SkipNextInstructionIfRegisterNotEqualsRegister);
            instructionMap.Add(0xA000, LoadIntoIndexRegister);
            instructionMap.Add(0xB000, JumpToV0PlusImmediate);
            instructionMap.Add(0xC000, LoadRandomIntoRegister);
            instructionMap.Add(0xD000, DrawSprite);
            instructionMap.Add(0xE000, GoToSkipRegisterInstruction);
            instructionMap.Add(0xE09E, SkipNextInstructionIfRegisterEqualsKeyPressed);
            instructionMap.Add(0xE0A1, SkipNextInstructionIfRegisterNotEqualsKeyPressed);
            instructionMap.Add(0xF000, GoToMemoryOperationInstruction);
            instructionMap.Add(0xF007, LoadTimerValueIntoRegister);
            instructionMap.Add(0xF00A, LoadKeyIntoRegister);
            instructionMap.Add(0xF015, LoadRegisterIntoDelayTimer);
            instructionMap.Add(0xF018, LoadRegisterIntoSoundTimer);
            instructionMap.Add(0xF01E, AddRegisterToIndexRegister);
            instructionMap.Add(0xF029, LoadFontSpriteLocationFromValueInRegister);
            instructionMap.Add(0xF033, LoadBcdRepresentationFromRegister);
            instructionMap.Add(0xF055, LoadAllRegistersFromValueInRegister);
            instructionMap.Add(0xF065, LoadFromValueInRegisterIntoAllRegisters);
        }

        /// <summary>
        /// Initializes the timer, it fires on every frame
        /// </summary>
        private void InitializeTimer()
        {
            MicroTimer = new MicroTimer
                             {
                                 Interval = (long) ((1.0/Settings.Default.FramesPerSecond)*1000.0*1000.0)
                             };

            this.MicroTimer.MicroTimerElapsed += this.EmulateFrame;
        }

        /// <summary>
        /// Event fired to emulate each frame
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="timerEventArgs">The event arguments</param>
        private void EmulateFrame(object sender, MicroTimerEventArgs timerEventArgs)
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

        #endregion

        #region Instruction Set

        // Instruction set taken from http://devernay.free.fr/hacks/chip8/C8TECH10.HTM#00E0

        /// <summary>
        /// Discriminates further into the instruction mapper for instructions starting with 0x0
        /// </summary>
        private void GoToRoutineStartingWithZero()
        {
            var fetchedOpcode = this.machineState.CurrentOpcode & 0xF0FF;

            if (fetchedOpcode == 0x0000)
            {
                JumpToRoutineAtAdress();
            }
            else
            {
                instructionMap[(ushort) fetchedOpcode]();
            }
        }

        /// <summary>
        /// 0nnn - SYS addr
        /// Jump to a machine code routine at nnn.
        /// This instruction is only used on the old computers on which Chip-8 was originally implemented. 
        /// It is ignored by modern interpreters.
        /// </summary>
        private void JumpToRoutineAtAdress()
        {
            //IGNORE! xD
        }

        /// <summary>
        /// 00E0 - CLS
        /// Clear the display.
        /// </summary>
        private void ClearScreen()
        {
            machineState.Graphics.SetAll(false);
            machineState.IsDrawFlagSet = true;
        }

        /// <summary>
        /// 00EE - RET
        /// Return from a subroutine.
        /// The interpreter sets the program counter to the address at the top of the machineState.Stack, 
        /// then subtracts 1 from the machineState.Stack pointer
        /// </summary>
        private void ReturnFromSubRoutine()
        {
            machineState.ProgramCounter = machineState.Stack.Pop();
        }

        /// <summary>
        /// 1nnn - JP addr
        /// Jump to location nnn.
        /// The interpreter sets the program counter to nnn.
        /// </summary>
        private void Jump()
        {
            machineState.ProgramCounter = (ushort) (machineState.CurrentOpcode & 0x0FFF);
        }

        /// <summary>
        /// 2nnn - CALL addr
        /// Call subroutine at nnn.
        /// The interpreter increments the machineState.Stack pointer, then puts the current PC on the top of the machineState.Stack. 
        /// The PC is then set to nnn.
        /// </summary>
        private void CallAtAdress()
        {
            // Program counter will be increased right after the instruction fetch 
            // So theres no need to increase the program counter before pushing
            machineState.Stack.Push(machineState.ProgramCounter);

            machineState.ProgramCounter = (ushort) (machineState.CurrentOpcode & 0x0FFF);
        }

        /// <summary>
        /// 3xkk - SE Vx, byte
        /// Skip next instruction if Vx = kk.
        /// The interpreter compares register Vx to kk, and if they are equal, increments the program counter by 2.
        /// </summary>
        private void SkipNextInstructionIfRegisterEqualsImmediate()
        {
            if (machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] ==
                (machineState.CurrentOpcode & 0x00FF))
            {
                machineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// 4xkk - SNE Vx, byte
        /// Skip next instruction if Vx != kk.
        /// The interpreter compares register Vx to kk, and if they are not equal, increments the program counter by 2.
        /// </summary>
        private void SkipNextInstructionIfRegisterNotEqualsImmediate()
        {
            if (machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] !=
                (machineState.CurrentOpcode & 0x00FF))
            {
                machineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// 5xy0 - SE Vx, Vy
        /// Skip next instruction if Vx = Vy.
        /// The interpreter compares register Vx to register Vy, and if they are equal, increments the program counter by 2.
        /// </summary>
        private void SkipNextInstructionIfRegisterEqualsRegister()
        {
            if (machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] ==
                machineState.VRegisters[machineState.YRegisterFromCurrentOpcode])
            {
                machineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// 6xkk - LD Vx, byte
        /// Set Vx = kk.
        /// The interpreter puts the value kk into register Vx.
        /// </summary>
        private void LoadValueIntoRegister()
        {
            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] =
                (ushort) (machineState.CurrentOpcode & 0x00FF);
        }

        /// <summary>
        /// 7xkk - ADD Vx, byte
        /// Set Vx = Vx + kk.
        /// Adds the value kk to the value of register Vx, then stores the result in Vx.
        /// </summary>
        private void AddValueIntoRegister()
        {
            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] +=
                (ushort) (machineState.CurrentOpcode & 0x00FF);
        }

        /// <summary>
        /// Discriminates further into the instruction mapper for instructions starting with 0x8
        /// </summary>
        private void GoToArithmeticLogicInstruction()
        {
            var filteredOpcode = (ushort) (machineState.CurrentOpcode & 0xF00F);

            if (filteredOpcode == 0x8000)
            {
                LoadRegisterIntoRegister();
            }
            else
            {
                instructionMap[filteredOpcode]();
            }
        }

        /// <summary>
        /// 8xy0 - LD Vx, Vy
        /// Set Vx = Vy.
        /// Stores the value of register Vy in register Vx.
        /// </summary>
        private void LoadRegisterIntoRegister()
        {
            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] +=
                machineState.VRegisters[machineState.YRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// 8xy1 - OR Vx, Vy
        /// Set Vx = Vx OR Vy.
        /// Performs a bitwise OR on the values of Vx and Vy, then stores the result in Vx. 
        /// A bitwise OR compares the corrseponding bits from two values, and if either bit is 1, 
        /// then the same bit in the result is also 1. Otherwise, it is 0. 
        /// </summary>
        private void OrRegistersIntoRegister()
        {
            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] |=
                machineState.VRegisters[machineState.YRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// 8xy2 - AND Vx, Vy
        /// Set Vx = Vx AND Vy.
        /// Performs a bitwise AND on the values of Vx and Vy, then stores the result in Vx. 
        /// A bitwise AND compares the corrseponding bits from two values, and if both bits are 1, 
        /// then the same bit in the result is also 1. Otherwise, it is 0.
        /// </summary>
        private void AndRegistersIntoRegiter()
        {
            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] &=
                machineState.VRegisters[machineState.YRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// 8xy3 - XOR Vx, Vy
        /// Set Vx = Vx XOR Vy.
        /// Performs a bitwise exclusive OR on the values of Vx and Vy, then stores the result in Vx. 
        /// An exclusive OR compares the corrseponding bits from two values, and if the bits are not both the same, 
        /// then the corresponding bit in the result is set to 1. Otherwise, it is 0. 
        /// </summary>
        private void ExclusiveOrIntoRegister()
        {
            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] ^=
                machineState.VRegisters[machineState.YRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// 8xy4 - ADD Vx, Vy
        /// Set Vx = Vx + Vy, set VF = carry.
        /// The values of Vx and Vy are added together. If the result is greater than 8 bits (i.e., > 255,) VF is set to 1, 
        /// otherwise 0. Only the lowest 8 bits of the result are kept, and stored in Vx.
        /// </summary>
        private void AddRegistersIntoRegister()
        {
            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] +=
                machineState.VRegisters[machineState.YRegisterFromCurrentOpcode];

            if (machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] > 0xFF)
            {
                machineState.VRegisters[0xF] = 1;
            }
            else
            {
                machineState.VRegisters[0xF] = 0;
            }
        }

        /// <summary>
        /// 8xy5 - SUB Vx, Vy
        /// Set Vx = Vx - Vy, set VF = NOT borrow.
        /// If Vx > Vy, then VF is set to 1, otherwise 0. Then Vy is subtracted from Vx, and the results stored in Vx.
        /// </summary>
        private void SubstractRegisters()
        {
            if (machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] >
                machineState.VRegisters[machineState.YRegisterFromCurrentOpcode])
            {
                machineState.VRegisters[0xF] = 1;
            }
            else
            {
                machineState.VRegisters[0xF] = 0;
            }

            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] -=
                machineState.VRegisters[machineState.YRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// 8xy6 - SHR Vx {, Vy}
        /// Set Vx = Vx SHR 1.
        /// If the least-significant bit of Vx is 1, then VF is set to 1, otherwise 0. Then Vx is divided by 2.
        /// </summary>
        private void ShiftRegisterRight()
        {
            if ((machineState.CurrentOpcode & 0x000F) == 1)
            {
                machineState.VRegisters[0xF] = 1;
            }
            else
            {
                machineState.VRegisters[0xF] = 0;
            }

            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] >>= 1;
        }

        /// <summary>
        /// 8xy7 - SUBN Vx, Vy
        /// Set Vx = Vy - Vx, set VF = NOT borrow.
        /// If Vy > Vx, then VF is set to 1, otherwise 0. Then Vx is subtracted from Vy, and the results stored in Vx.
        /// </summary>
        private void SubstractRegistersReverse()
        {
            if (machineState.VRegisters[machineState.YRegisterFromCurrentOpcode] >
                machineState.VRegisters[machineState.XRegisterFromCurrentOpcode])
            {
                machineState.VRegisters[0xF] = 1;
            }
            else
            {
                machineState.VRegisters[0xF] = 0;
            }

            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] =
                (ushort)
                (machineState.VRegisters[machineState.YRegisterFromCurrentOpcode] -
                 machineState.VRegisters[machineState.XRegisterFromCurrentOpcode]);
        }

        /// <summary>
        /// 8xyE - SHL Vx {, Vy}
        /// Set Vx = Vx SHL 1.
        /// If the most-significant bit of Vx is 1, then VF is set to 1, otherwise to 0. Then Vx is multiplied by 2.
        /// </summary>
        private void ShiftRegisterLeft()
        {
            if ((machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] & 0xF000) == 0x1000)
            {
                machineState.VRegisters[0xF] = 1;
            }
            else
            {
                machineState.VRegisters[0xF] = 0;
            }

            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] <<= 1;
        }

        /// <summary>
        /// 9xy0 - SNE Vx, Vy
        /// Skip next instruction if Vx != Vy.
        /// The values of Vx and Vy are compared, and if they are not equal, the program counter is increased by 2.
        /// </summary>
        private void SkipNextInstructionIfRegisterNotEqualsRegister()
        {
            if (machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] !=
                machineState.VRegisters[machineState.YRegisterFromCurrentOpcode])
            {
                machineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// Annn - LD I, addr
        /// Set I = nnn.
        /// The value of register I is set to nnn.
        /// </summary>
        private void LoadIntoIndexRegister()
        {
            machineState.IndexRegister = (ushort) (machineState.CurrentOpcode & 0x0FFF);
        }

        /// <summary>
        /// Bnnn - JP V0, addr
        /// Jump to location nnn + V0.
        /// The program counter is set to nnn plus the value of V0.
        /// </summary>
        private void JumpToV0PlusImmediate()
        {
            machineState.ProgramCounter = (ushort) (machineState.VRegisters[0] + (machineState.CurrentOpcode & 0x0FFF));
        }

        /// <summary>
        /// Cxkk - RND Vx, byte
        /// Set Vx = random byte AND kk.
        /// The interpreter generates a random number from 0 to 255, which is then ANDed with the value kk. 
        /// The results are stored in Vx. See instruction 8xy2 for more information on AND.
        /// </summary>
        private void LoadRandomIntoRegister()
        {
            var randomnumber = (ushort) new Random().Next(0, 255);

            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] =
                (ushort) (randomnumber & (machineState.CurrentOpcode & 0x00FF));
        }

        /// <summary>
        /// Dxyn - DRW Vx, Vy, nibble
        /// Display n-byte sprite starting at machineState.Memory location I at (Vx, Vy), set VF = collision.
        /// The interpreter reads n bytes from machineState.Memory, starting at the address stored in I. 
        /// These bytes are then displayed as sprites on screen at coordinates (Vx, Vy). 
        /// Sprites are XORed onto the existing screen. 
        /// If this causes any pixels to be erased, VF is set to 1, otherwise it is set to 0. 
        /// If the sprite is positioned so part of it is outside the coordinates of the display, it wraps around to the opposite side of the screen. 
        /// See instruction 8xy3 for more information on XOR, and section 2.4, Display, for more information on the Chip-8 screen and sprites.
        /// </summary>
        private void DrawSprite()
        {
            var numbytes = (ushort) (machineState.CurrentOpcode & 0x000F);
            var positionX = machineState.VRegisters[machineState.XRegisterFromCurrentOpcode];
            var positionY = machineState.VRegisters[machineState.YRegisterFromCurrentOpcode];

            for (int rowNum = 0; rowNum < numbytes; rowNum++)
            {
                ushort currentpixel = machineState.Memory[machineState.IndexRegister + rowNum];

                for (int colNum = 0; colNum < 8; colNum++) //We assume sprites are always 8 pixels wide
                {
                    if ((currentpixel & (0x80 >> colNum)) != 0)
                    {
                        int positioninGraphics = (positionX + colNum +
                                                  ((positionY + rowNum)*C8Constants.ResolutionWidth))
                                                 %(C8Constants.ResolutionWidth*C8Constants.ResolutionHeight);
                            // Make sure we get a value inside boundaries

                        if (machineState.Graphics[positioninGraphics])
                        {
                            //Collision!
                            machineState.VRegisters[0xF] = 1;
                        }

                        machineState.Graphics[positioninGraphics] ^= true;
                    }
                }

                machineState.IsDrawFlagSet = true;
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
        /// Ex9E - SKP Vx
        /// Skip next instruction if key with the value of Vx is pressed.
        /// Checks the keyboard, and if the key corresponding to the value of Vx is currently in the down position, 
        /// PC is increased by 2.
        /// </summary>
        private void SkipNextInstructionIfRegisterEqualsKeyPressed()
        {
            if (machineState.Keys[machineState.VRegisters[machineState.XRegisterFromCurrentOpcode]])
            {
                machineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// ExA1 - SKNP Vx
        /// Skip next instruction if key with the value of Vx is not pressed.
        /// Checks the keyboard, and if the key corresponding to the value of Vx is currently in the up position, 
        /// PC is increased by 2.
        /// </summary>
        private void SkipNextInstructionIfRegisterNotEqualsKeyPressed()
        {
            if (!machineState.Keys[machineState.VRegisters[machineState.XRegisterFromCurrentOpcode]])
            {
                machineState.IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// Discriminates better for instructionmap opcodes starting with 0xF
        /// </summary>
        private void GoToMemoryOperationInstruction()
        {
            instructionMap[(ushort) (machineState.CurrentOpcode & 0xF0FF)]();
        }

        /// <summary>
        /// Fx07 - LD Vx, DT
        /// Set Vx = delay timer value.
        /// The value of DT is placed into Vx.
        /// </summary>
        private void LoadTimerValueIntoRegister()
        {
            machineState.VRegisters[machineState.XRegisterFromCurrentOpcode] = machineState.DelayTimer;
        }

        /// <summary>
        /// Fx0A - LD Vx, K
        /// Wait for a key press, store the value of the key in Vx.
        /// All execution stops until a key is pressed, then the value of that key is stored in Vx.
        /// </summary>
        private void LoadKeyIntoRegister()
        {
            //TODO What do I do with this wait for key?
            /*while (!machineState.Keys.OfType<bool>().Any(x => x))
            {

            }*/
        }

        /// <summary>
        /// Fx15 - LD DT, Vx
        /// Set delay timer = Vx.
        /// DT is set equal to the value of Vx.
        /// </summary>
        private void LoadRegisterIntoDelayTimer()
        {
            machineState.DelayTimer = machineState.VRegisters[machineState.XRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// Fx18 - LD ST, Vx
        /// Set sound timer = Vx.
        /// ST is set equal to the value of Vx.
        /// </summary>
        private void LoadRegisterIntoSoundTimer()
        {
            machineState.SoundTimer = machineState.VRegisters[machineState.XRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// Fx1E - ADD I, Vx
        /// Set I = I + Vx.
        /// The values of I and Vx are added, and the results are stored in I.
        /// </summary>
        private void AddRegisterToIndexRegister()
        {
            machineState.IndexRegister += machineState.VRegisters[machineState.XRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// Fx29 - LD F, Vx
        /// Set I = location of sprite for digit Vx.
        /// The value of I is set to the location for the hexadecimal sprite corresponding to the value of Vx. 
        /// See section 2.4, Display, for more information on the Chip-8 hexadecimal font.
        /// </summary>
        private void LoadFontSpriteLocationFromValueInRegister()
        {
            // Comments from NGEmu about this opcode
            /*Mmm... basically, set the machineState.Memory pointer I to the location of the character of the hexadecimal stored in register VX. 
              So... if it's FA29, you look in register VA and see what value it holds. 
              If it's F129, you look in register V1 and see what value it holds.
              You can assume that the value stored in those registers only goes from 0x0 to 0xF. 
              Now... your emulator should have a table of sprite data already preset somewhere in the machineState.Memory (preferrably in the locations before 0x200). 
              These are sprite of characters from 0 to F, and they are 4x5 each in dimension. 
              Which means... mmm... the number 0 should be somewhat like this in binary data:

              1111 0000
              1001 0000
              1001 0000
              1001 0000
              1111 0000

              If you have to ask, that's because the draw instruction draws 8 bits at a time.*/

            ushort character = machineState.VRegisters[machineState.XRegisterFromCurrentOpcode];

            // We assume that the value for character goes from 0x0 to 0xF and that each digit has size 5 (5 rows per digit)
            // Fonts are loaded starting at 0x0
            machineState.IndexRegister = (ushort) (5*character);
        }

        /// <summary>
        /// Fx33 - LD B, Vx
        /// Store BCD representation of Vx in machineState.Memory locations I, I+1, and I+2.
        /// The interpreter takes the decimal value of Vx, 
        /// and places the hundreds digit in machineState.Memory at location in I, 
        /// the tens digit at location I+1, 
        /// and the ones digit at location I+2.
        /// </summary>
        private void LoadBcdRepresentationFromRegister()
        {
            machineState.Memory[machineState.IndexRegister] =
                (byte) (machineState.VRegisters[machineState.XRegisterFromCurrentOpcode]/100); //Hundreds
            machineState.Memory[machineState.IndexRegister + 1] =
                (byte) ((machineState.VRegisters[machineState.XRegisterFromCurrentOpcode]/10)%10); //Tens
            machineState.Memory[machineState.IndexRegister + 2] =
                (byte) (machineState.VRegisters[machineState.XRegisterFromCurrentOpcode]%10); //Ones
        }

        /// <summary>
        /// Fx55 - LD [I], Vx
        /// Store registers V0 through Vx in machineState.Memory starting at location I.
        /// The interpreter copies the values of registers V0 through Vx into machineState.Memory, starting at the address in I.
        /// </summary>
        private void LoadAllRegistersFromValueInRegister()
        {
            for (int i = 0; i <= machineState.XRegisterFromCurrentOpcode; i++)
            {
                machineState.Memory[machineState.IndexRegister + i] = (byte) machineState.VRegisters[i];
            }
        }

        /// <summary>
        /// Fx65 - LD Vx, [I]
        /// Read registers V0 through Vx from machineState.Memory starting at location I.
        /// The interpreter reads values from machineState.Memory starting at location I into registers V0 through Vx.
        /// </summary>
        private void LoadFromValueInRegisterIntoAllRegisters()
        {
            for (int i = 0; i <= machineState.XRegisterFromCurrentOpcode; i++)
            {
                machineState.VRegisters[i] = machineState.Memory[machineState.IndexRegister + i];
            }
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