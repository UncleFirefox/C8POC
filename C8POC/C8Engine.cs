using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using C8POC.Properties;
using MicroLibrary;

namespace C8POC
{
    public class C8Engine
    {
        #region Emulator Constants

        private readonly Byte[] chip8FontSet
            = new Byte[]
                  {
                      0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
                      0x20, 0x60, 0x20, 0x20, 0x70, // 1
                      0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
                      0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
                      0x90, 0x90, 0xF0, 0x10, 0x10, // 4
                      0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
                      0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
                      0xF0, 0x10, 0x20, 0x40, 0x40, // 7
                      0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
                      0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
                      0xF0, 0x90, 0xF0, 0x90, 0x90, // A
                      0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
                      0xF0, 0x80, 0x80, 0x80, 0xF0, // C
                      0xE0, 0x90, 0x90, 0x90, 0xE0, // D
                      0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
                      0xF0, 0x80, 0xF0, 0x80, 0x80 // F
                  };

        private const ushort StartRomAdress = 0x200;
        private const ushort ResolutionWidth = 64;
        private const ushort ResolutionHeight = 32;

        #endregion

        #region Emulator Properties

        private Dictionary<ushort, Action> instructionMap = new Dictionary<ushort, Action>();
        private ushort currentOpcode;
        private Byte[] memory = new Byte[4096];

        private ushort[] vRegisters = new ushort[16];

        private ushort indexRegister;
        private ushort programCounter;

        private BitArray graphics = new BitArray(ResolutionWidth*ResolutionHeight, false);

        private ushort delayTimer;
        private ushort soundTimer;

        private Stack<ushort> stack = new Stack<ushort>(16);
        private BitArray keys = new BitArray(16);

        /// <summary>
        /// Get y value in opcodes like 5xy0 etc
        /// </summary>
        private ushort XRegisterFromCurrentOpcode
        {
            get { return (ushort)((currentOpcode & 0x0F00) >> 8); }
        }

        /// <summary>
        /// Gets y value in opcodes like 5xy0 etc
        /// </summary>
        private ushort YRegisterFromCurrentOpcode
        {
            get { return (ushort)((currentOpcode & 0x00F0) >> 4); }
        }

        /// <summary>
        /// Gets or sets the draw flag
        /// </summary>
        private bool IsDrawFlagSet { get; set; }

        /// <summary>
        /// Gets the state of a pixel, take into account that
        /// screen starts at upper left corner (0,0) and ends at lower right corner (63,31)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool GetPixelState(int x, int y)
        {
            return graphics[x + (ResolutionWidth * y)];
        }

        /// <summary>
        /// Timer in charge of handling the number of cycles per second
        /// </summary>
        private MicroTimer MicroTimer { get; set; }

        /// <summary>
        /// Determines if it has an executable ROM
        /// </summary>
        private bool HasRomLoaded()
        {
            return memory.Skip(StartRomAdress).Any(x => x != 0);
        }

        #endregion

        #region I/O Handling

        /// <summary>
        /// Event handler for screen changes
        /// </summary>
        public delegate void ScreenChangeEventHandler(BitArray graphics);

        /// <summary>
        /// Event that will be raised every that that the screen needs to be refreshed
        /// </summary>
        public event ScreenChangeEventHandler ScreenChanged;

        /// <summary>
        /// Event handler for sound generation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void SoundGenerateEventHandler();

        /// <summary>
        /// Event that will be raised when a beep is generated
        /// </summary>
        public event SoundGenerateEventHandler SoundGenerated;

        /// <summary>
        /// Sets a pressed key
        /// </summary>
        /// <param name="keyValue">The pressed keycode</param>
        public void KeyDown(byte keyIndex)
        {
            keys[keyIndex] = true;
        }

        /// <summary>
        /// Unsets a pressed key
        /// </summary>
        /// <param name="KeyValue">The released keycode</param>
        public void KeyUp(byte keyIndex)
        {
            keys[keyIndex] = false;
        }

        /// <summary>
        /// Raises the draw event
        /// </summary>
        private void DrawGraphics()
        {
            if (ScreenChanged != null)
            {
                ScreenChanged(((BitArray)graphics.Clone()));
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

        #region Engine constructor

        public C8Engine()
        {
            //The InstructionMap is loaded once!!
            SetUpInstructionMap();

            //Configure the timer
            InitializeTimer();

            // Load fontset should be loaded just once
            LoadFontSet();
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
            if (HasRomLoaded())
            {
                MicroTimer.Start();
            }
        }

        /// <summary>
        /// Stops the timer execution
        /// </summary>
        public void StopEmulator()
        {
            if (MicroTimer.Enabled)
            {
                MicroTimer.Stop();
            }
        }

        /// <summary>
        /// Emulates a cycle
        /// </summary>
        private void EmulateCycle()
        {
            //Get Opcode located at program counter
            FetchOpcode();

            //Processes the opcode
            ProcessOpcode();

            //Update timers
            UpdateTimers();
        }

        /// <summary>
        /// Timer update every cycle
        /// </summary>
        private void UpdateTimers()
        {
            if (delayTimer > 0)
            {
                --delayTimer;
            }

            if (soundTimer > 0)
            {
                if (soundTimer == 1)
                {
                    GenerateSound();
                }

                --soundTimer;
            }
        }

        /// <summary>
        /// Obtains the opcode and executes the action associated to it
        /// </summary>
        private void ProcessOpcode()
        {
            try
            {
                instructionMap[(ushort) (currentOpcode & 0xF000)]();
            }
            catch (KeyNotFoundException)
            {
                throw new Exception(string.Format("Instruction with Opcode {0:X} is not implemented", currentOpcode));
            }
        }

        /// <summary>
        /// Fetches the opcode in memory using the program counter
        /// Program counter auto increases after opcode fetching for next instruction
        /// </summary>
        private void FetchOpcode()
        {
            currentOpcode = memory[programCounter];
            currentOpcode <<= 8;
            currentOpcode |= memory[programCounter + 1];

            IncreaseProgramCounter();
        }

        /// <summary>
        /// Loads a Chip8 ROM file in memory
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
                    memory[StartRomAdress + i] = (byte) rom.ReadByte();
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
            SetupGraphics();
            SetupInput();

            programCounter = StartRomAdress; // Program counter starts at 0x200
            currentOpcode = 0; // Reset current currentOpcode	
            indexRegister = 0; // Reset index register

            // Clear stack
            stack.Clear();

            // Clear registers V0-VF
            Array.Clear(vRegisters,0,vRegisters.Length);

            // Clear memory
            Array.Clear(memory, StartRomAdress, memory.Length - StartRomAdress);

            // Reset timers
            soundTimer = 0;
            delayTimer = 0;
        }

        /// <summary>
        /// Loads the font set in memory
        /// </summary>
        private void LoadFontSet()
        {
            for (var i = 0; i < chip8FontSet.Length; ++i)
            {
                memory[i] = chip8FontSet[i];
            }
        }

        /// <summary>
        /// Initializes keyboard structure
        /// </summary>
        private void SetupInput()
        {
            keys.SetAll(false);
        }

        /// <summary>
        /// Initializes graphics
        /// </summary>
        private void SetupGraphics()
        {
            graphics.SetAll(false);
            DrawGraphics();
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
            instructionMap.Add(0xE000, GoToSkipRegisterInstruction );
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
        /// Increases the program counter
        /// </summary>
        private void IncreaseProgramCounter()
        {
            programCounter += 2;
        }

        /// <summary>
        /// Initializes the timer, it fires on every frame
        /// </summary>
        private void InitializeTimer()
        {
            MicroTimer = new MicroTimer
                             {Interval = (long)((1.0/Settings.Default.FramesPerSecond)*1000.0*1000.0)};
            
            MicroTimer.MicroTimerElapsed += EmulateFrame;
        }

        /// <summary>
        /// Event fired to emulate each frame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="timerEventArgs"></param>
        private void EmulateFrame(object sender, MicroTimerEventArgs timerEventArgs)
        {
            for (int operationNum = 0; operationNum < Settings.Default.OperationsPerFrame; operationNum++)
            {
                EmulateCycle();
            }

            if (IsDrawFlagSet)
            {
                DrawGraphics();
                IsDrawFlagSet = false;
            }
        }

        #endregion

        #region Instruction Set

        // Instruction set taken from http://devernay.free.fr/hacks/chip8/C8TECH10.HTM#00E0

        /// <summary>
        /// Discriminates further into the instruction mapper for instructions starting with 0x8
        /// </summary>
        private void GoToRoutineStartingWithZero()
        {
            var fetchedOpcode = currentOpcode & 0xF0FF;
            
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
            graphics.SetAll(false);
            IsDrawFlagSet = true;
        }

        /// <summary>
        /// 00EE - RET
        /// Return from a subroutine.
        /// The interpreter sets the program counter to the address at the top of the stack, 
        /// then subtracts 1 from the stack pointer
        /// </summary>
        private void ReturnFromSubRoutine()
        {
            programCounter = stack.Pop();
        }

        /// <summary>
        /// 1nnn - JP addr
        /// Jump to location nnn.
        /// The interpreter sets the program counter to nnn.
        /// </summary>
        private void Jump()
        {
            programCounter = (ushort) (currentOpcode & 0x0FFF);
        }

        /// <summary>
        /// 2nnn - CALL addr
        /// Call subroutine at nnn.
        /// The interpreter increments the stack pointer, then puts the current PC on the top of the stack. 
        /// The PC is then set to nnn.
        /// </summary>
        private void CallAtAdress()
        {
            // Program counter will be increased right after the instruction fetch 
            // So theres no need to increase the program counter before pushing
            stack.Push(programCounter);

            programCounter = (ushort) (currentOpcode & 0x0FFF);
        }

        /// <summary>
        /// 3xkk - SE Vx, byte
        /// Skip next instruction if Vx = kk.
        /// The interpreter compares register Vx to kk, and if they are equal, increments the program counter by 2.
        /// </summary>
        private void SkipNextInstructionIfRegisterEqualsImmediate()
        {
            if (vRegisters[XRegisterFromCurrentOpcode] == (currentOpcode & 0x00FF))
            {
                IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// 4xkk - SNE Vx, byte
        /// Skip next instruction if Vx != kk.
        /// The interpreter compares register Vx to kk, and if they are not equal, increments the program counter by 2.
        /// </summary>
        private void SkipNextInstructionIfRegisterNotEqualsImmediate()
        {
            if (vRegisters[XRegisterFromCurrentOpcode] != (currentOpcode & 0x00FF))
            {
                IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// 5xy0 - SE Vx, Vy
        /// Skip next instruction if Vx = Vy.
        /// The interpreter compares register Vx to register Vy, and if they are equal, increments the program counter by 2.
        /// </summary>
        private void SkipNextInstructionIfRegisterEqualsRegister()
        {
            if (vRegisters[XRegisterFromCurrentOpcode] == vRegisters[YRegisterFromCurrentOpcode])
            {
                IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// 6xkk - LD Vx, byte
        /// Set Vx = kk.
        /// The interpreter puts the value kk into register Vx.
        /// </summary>
        private void LoadValueIntoRegister()
        {
            vRegisters[XRegisterFromCurrentOpcode] = (ushort) (currentOpcode & 0x00FF);
        }

        /// <summary>
        /// 7xkk - ADD Vx, byte
        /// Set Vx = Vx + kk.
        /// Adds the value kk to the value of register Vx, then stores the result in Vx.
        /// </summary>
        private void AddValueIntoRegister()
        {
            vRegisters[XRegisterFromCurrentOpcode] += (ushort) (currentOpcode & 0x00FF);
        }

        /// <summary>
        /// Discriminates further into the instruction mapper for instructions starting with 0x8
        /// </summary>
        private void GoToArithmeticLogicInstruction()
        {
            var filteredOpcode = (ushort)(currentOpcode & 0xF00F);
            
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
            vRegisters[XRegisterFromCurrentOpcode] += vRegisters[YRegisterFromCurrentOpcode];
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
            vRegisters[XRegisterFromCurrentOpcode] |= vRegisters[YRegisterFromCurrentOpcode];
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
            vRegisters[XRegisterFromCurrentOpcode] &= vRegisters[YRegisterFromCurrentOpcode];
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
            vRegisters[XRegisterFromCurrentOpcode] ^= vRegisters[YRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// 8xy4 - ADD Vx, Vy
        /// Set Vx = Vx + Vy, set VF = carry.
        /// The values of Vx and Vy are added together. If the result is greater than 8 bits (i.e., > 255,) VF is set to 1, 
        /// otherwise 0. Only the lowest 8 bits of the result are kept, and stored in Vx.
        /// </summary>
        private void AddRegistersIntoRegister()
        {
            vRegisters[XRegisterFromCurrentOpcode] += vRegisters[YRegisterFromCurrentOpcode];

            if (vRegisters[XRegisterFromCurrentOpcode] > 0xFF)
            {
                vRegisters[0xF] = 1;
            }
            else
            {
                vRegisters[0xF] = 0;
            }
        }

        /// <summary>
        /// 8xy5 - SUB Vx, Vy
        /// Set Vx = Vx - Vy, set VF = NOT borrow.
        /// If Vx > Vy, then VF is set to 1, otherwise 0. Then Vy is subtracted from Vx, and the results stored in Vx.
        /// </summary>
        private void SubstractRegisters()
        {
            if (vRegisters[XRegisterFromCurrentOpcode] > vRegisters[YRegisterFromCurrentOpcode])
            {
                vRegisters[0xF] = 1;
            }
            else
            {
                vRegisters[0xF] = 0;
            }

            vRegisters[XRegisterFromCurrentOpcode] -= vRegisters[YRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// 8xy6 - SHR Vx {, Vy}
        /// Set Vx = Vx SHR 1.
        /// If the least-significant bit of Vx is 1, then VF is set to 1, otherwise 0. Then Vx is divided by 2.
        /// </summary>
        private void ShiftRegisterRight()
        {
            if ((currentOpcode & 0x000F) == 1)
            {
                vRegisters[0xF] = 1;
            }
            else
            {
                vRegisters[0xF] = 0;
            }

            vRegisters[XRegisterFromCurrentOpcode] >>= 1;
        }

        /// <summary>
        /// 8xy7 - SUBN Vx, Vy
        /// Set Vx = Vy - Vx, set VF = NOT borrow.
        /// If Vy > Vx, then VF is set to 1, otherwise 0. Then Vx is subtracted from Vy, and the results stored in Vx.
        /// </summary>
        private void SubstractRegistersReverse()
        {
            if (vRegisters[YRegisterFromCurrentOpcode] > vRegisters[XRegisterFromCurrentOpcode])
            {
                vRegisters[0xF] = 1;
            }
            else
            {
                vRegisters[0xF] = 0;
            }

            vRegisters[XRegisterFromCurrentOpcode] =
                (ushort) (vRegisters[YRegisterFromCurrentOpcode] - vRegisters[XRegisterFromCurrentOpcode]);
        }

        /// <summary>
        /// 8xyE - SHL Vx {, Vy}
        /// Set Vx = Vx SHL 1.
        /// If the most-significant bit of Vx is 1, then VF is set to 1, otherwise to 0. Then Vx is multiplied by 2.
        /// </summary>
        private void ShiftRegisterLeft()
        {
            if ((vRegisters[XRegisterFromCurrentOpcode] & 0xF000) == 0x1000)
            {
                vRegisters[0xF] = 1;
            }
            else
            {
                vRegisters[0xF] = 0;
            }

            vRegisters[XRegisterFromCurrentOpcode] <<= 1;
        }

        /// <summary>
        /// 9xy0 - SNE Vx, Vy
        /// Skip next instruction if Vx != Vy.
        /// The values of Vx and Vy are compared, and if they are not equal, the program counter is increased by 2.
        /// </summary>
        private void SkipNextInstructionIfRegisterNotEqualsRegister()
        {
            if (vRegisters[XRegisterFromCurrentOpcode] != vRegisters[YRegisterFromCurrentOpcode])
            {
                IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// Annn - LD I, addr
        /// Set I = nnn.
        /// The value of register I is set to nnn.
        /// </summary>
        private void LoadIntoIndexRegister()
        {
            indexRegister = (ushort) (currentOpcode & 0x0FFF);
        }

        /// <summary>
        /// Bnnn - JP V0, addr
        /// Jump to location nnn + V0.
        /// The program counter is set to nnn plus the value of V0.
        /// </summary>
        private void JumpToV0PlusImmediate()
        {
            programCounter = (ushort) (vRegisters[0] + (currentOpcode & 0x0FFF));
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

            vRegisters[XRegisterFromCurrentOpcode] = (ushort) (randomnumber & (currentOpcode & 0x00FF));
        }

        /// <summary>
        /// Dxyn - DRW Vx, Vy, nibble
        /// Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision.
        /// The interpreter reads n bytes from memory, starting at the address stored in I. 
        /// These bytes are then displayed as sprites on screen at coordinates (Vx, Vy). 
        /// Sprites are XORed onto the existing screen. 
        /// If this causes any pixels to be erased, VF is set to 1, otherwise it is set to 0. 
        /// If the sprite is positioned so part of it is outside the coordinates of the display, it wraps around to the opposite side of the screen. 
        /// See instruction 8xy3 for more information on XOR, and section 2.4, Display, for more information on the Chip-8 screen and sprites.
        /// </summary>
        private void DrawSprite()
        {
            var numbytes = (ushort) (currentOpcode & 0x000F);
            var positionX = vRegisters[XRegisterFromCurrentOpcode];
            var positionY = vRegisters[YRegisterFromCurrentOpcode];

            for (int rowNum = 0; rowNum < numbytes; rowNum++)
            {
                ushort currentpixel = memory[indexRegister + rowNum];

                for (int colNum = 0; colNum < 8; colNum++) //We assume sprites are always 8 pixels wide
                {
                    if ((currentpixel & (0x80 >> colNum)) != 0)
                    {
                        int positioninGraphics = (positionX + colNum + ((positionY + rowNum)*ResolutionWidth))
                            %(ResolutionWidth*ResolutionHeight); // Make sure we get a value inside boundaries

                        if (graphics[positioninGraphics])
                        {
                            //Collision!
                            vRegisters[0xF] = 1;
                        }

                        graphics[positioninGraphics] ^= true;
                    }
                }

                IsDrawFlagSet = true;
            }
        }

        /// <summary>
        /// Discriminates instructions starting with 0xE
        /// </summary>
        private void GoToSkipRegisterInstruction()
        {
            instructionMap[(ushort) (currentOpcode & 0xF0FF)]();
        }

        /// <summary>
        /// Ex9E - SKP Vx
        /// Skip next instruction if key with the value of Vx is pressed.
        /// Checks the keyboard, and if the key corresponding to the value of Vx is currently in the down position, 
        /// PC is increased by 2.
        /// </summary>
        private void SkipNextInstructionIfRegisterEqualsKeyPressed()
        {
            if (keys[vRegisters[XRegisterFromCurrentOpcode]])
            {
                IncreaseProgramCounter();
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
            if (!keys[vRegisters[XRegisterFromCurrentOpcode]])
            {
                IncreaseProgramCounter();
            }
        }

        /// <summary>
        /// Discriminates better for instructionmap opcodes starting with 0xF
        /// </summary>
        private void GoToMemoryOperationInstruction()
        {
            instructionMap[(ushort) (currentOpcode & 0xF0FF)]();
        }

        /// <summary>
        /// Fx07 - LD Vx, DT
        /// Set Vx = delay timer value.
        /// The value of DT is placed into Vx.
        /// </summary>
        private void LoadTimerValueIntoRegister()
        {
            vRegisters[XRegisterFromCurrentOpcode] = delayTimer;
        }

        /// <summary>
        /// Fx0A - LD Vx, K
        /// Wait for a key press, store the value of the key in Vx.
        /// All execution stops until a key is pressed, then the value of that key is stored in Vx.
        /// </summary>
        private void LoadKeyIntoRegister()
        {
            //TODO What do I do with this wait for key?
            /*while (!keys.OfType<bool>().Any(x => x))
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
            delayTimer = vRegisters[XRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// Fx18 - LD ST, Vx
        /// Set sound timer = Vx.
        /// ST is set equal to the value of Vx.
        /// </summary>
        private void LoadRegisterIntoSoundTimer()
        {
            soundTimer = vRegisters[XRegisterFromCurrentOpcode];
        }

        /// <summary>
        /// Fx1E - ADD I, Vx
        /// Set I = I + Vx.
        /// The values of I and Vx are added, and the results are stored in I.
        /// </summary>
        private void AddRegisterToIndexRegister()
        {
            indexRegister += vRegisters[XRegisterFromCurrentOpcode];
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
            /*Mmm... basically, set the memory pointer I to the location of the character of the hexadecimal stored in register VX. 
              So... if it's FA29, you look in register VA and see what value it holds. 
              If it's F129, you look in register V1 and see what value it holds.
              You can assume that the value stored in those registers only goes from 0x0 to 0xF. 
              Now... your emulator should have a table of sprite data already preset somewhere in the memory (preferrably in the locations before 0x200). 
              These are sprite of characters from 0 to F, and they are 4x5 each in dimension. 
              Which means... mmm... the number 0 should be somewhat like this in binary data:

              1111 0000
              1001 0000
              1001 0000
              1001 0000
              1111 0000

              If you have to ask, that's because the draw instruction draws 8 bits at a time.*/

            ushort character = vRegisters[XRegisterFromCurrentOpcode];

            // We assume that the value for character goes from 0x0 to 0xF and that each digit has size 5 (5 rows per digit)
            // Fonts are loaded starting at 0x0
            indexRegister = (ushort) (5*character);
        }

        /// <summary>
        /// Fx33 - LD B, Vx
        /// Store BCD representation of Vx in memory locations I, I+1, and I+2.
        /// The interpreter takes the decimal value of Vx, 
        /// and places the hundreds digit in memory at location in I, 
        /// the tens digit at location I+1, 
        /// and the ones digit at location I+2.
        /// </summary>
        private void LoadBcdRepresentationFromRegister()
        {
            memory[indexRegister] = (byte) (vRegisters[XRegisterFromCurrentOpcode]/100); //Hundreds
            memory[indexRegister + 1] = (byte) ((vRegisters[XRegisterFromCurrentOpcode]/10)%10); //Tens
            memory[indexRegister + 2] = (byte) (vRegisters[XRegisterFromCurrentOpcode]%10); //Ones
        }

        /// <summary>
        /// Fx55 - LD [I], Vx
        /// Store registers V0 through Vx in memory starting at location I.
        /// The interpreter copies the values of registers V0 through Vx into memory, starting at the address in I.
        /// </summary>
        private void LoadAllRegistersFromValueInRegister()
        {
            for (int i = 0; i <= XRegisterFromCurrentOpcode; i++)
            {
                memory[indexRegister + i] = (byte) vRegisters[i];
            }
        }

        /// <summary>
        /// Fx65 - LD Vx, [I]
        /// Read registers V0 through Vx from memory starting at location I.
        /// The interpreter reads values from memory starting at location I into registers V0 through Vx.
        /// </summary>
        private void LoadFromValueInRegisterIntoAllRegisters()
        {
            for (int i = 0; i <= XRegisterFromCurrentOpcode; i++)
            {
                vRegisters[i] = memory[indexRegister + i];
            }
        }

        #endregion
    }
}
