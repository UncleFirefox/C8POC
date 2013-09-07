// --------------------------------------------------------------------------------------------------------------------
// <copyright file="C8MachineState.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Class that wraps the Chip8 Machine State
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Domain.Entities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using C8POC.Infrastructure;
    using C8POC.Interfaces;
    using C8POC.Interfaces.Domain.Entities;

    /// <summary>
    /// Class that wraps the Chip8 Machine State
    /// </summary>
    public class C8MachineState : IMachineState
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="C8MachineState"/> class. 
        /// </summary>
        public C8MachineState()
        {
            // Memory
            this.Memory = new byte[C8Constants.MemorySize];

            // Registers
            this.VRegisters = new ushort[C8Constants.NumVRegisters];

            // Stack
            this.Stack = new Stack<ushort>(C8Constants.StackSize);

            // Graphics
            this.Graphics = new BitArray(C8Constants.ResolutionWidth * C8Constants.ResolutionHeight, false);

            // Keys
            this.Keys = new BitArray(C8Constants.NumKeys, false);

            // Load of font set
            this.LoadFontSet();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the machine memory
        /// </summary>
        public byte[] Memory { get; set; }

        /// <summary>
        /// Gets or sets the VRegisters
        /// </summary>
        public ushort[] VRegisters { get; set; }

        /// <summary>
        /// Gets or sets the Index Register
        /// </summary>
        public ushort IndexRegister { get; set; }

        /// <summary>
        /// Gets or sets the Program Counter
        /// </summary>
        public ushort ProgramCounter { get; set; }

        /// <summary>
        /// Gets or sets the graphics array
        /// </summary>
        public BitArray Graphics { get; set; }

        /// <summary>
        /// Gets or sets the stack
        /// </summary>
        public Stack<ushort> Stack { get; set; }

        /// <summary>
        /// Gets or sets the Keys Array
        /// </summary>
        public BitArray Keys { get; set; }

        /// <summary>
        /// Gets or sets the CurrentOpcode
        /// </summary>
        public ushort CurrentOpcode { get; set; }

        /// <summary>
        /// Gets or sets the Delay Timer
        /// </summary>
        public ushort DelayTimer { get; set; }

        /// <summary>
        /// Gets or sets the Sound Timer
        /// </summary>
        public ushort SoundTimer { get; set; }

        #endregion

        #region Machine Actions

        /// <summary>
        /// Gets x value in opcodes like 5xy0 etc
        /// </summary>
        public ushort XRegisterFromCurrentOpcode
        {
            get { return (ushort)((this.CurrentOpcode & 0x0F00) >> 8); }
        }

        /// <summary>
        /// Gets x value in opcodes like 5xy0 etc
        /// </summary>
        public ushort YRegisterFromCurrentOpcode
        {
            get { return (ushort)((this.CurrentOpcode & 0x00F0) >> 4); }
        }

        /// <summary>
        /// Gets or sets the number of bytes in memory for opcodes
        /// </summary>
        public int NumberOfOpcodeBytes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether if the draw flag is set
        /// </summary>
        public bool IsDrawFlagSet { get; set; }

        /// <summary>
        /// Gets the state of a pixel, take into account that
        /// screen starts at upper left corner (0,0) and ends at lower right corner (63,31)
        /// </summary>
        /// <param name="x">Horizontal on screen</param>
        /// <param name="y">Vertical on screen</param>
        /// <returns>If the pixel is set or not</returns>
        public bool GetPixelState(int x, int y)
        {
            return this.Graphics[x + (C8Constants.ResolutionWidth * y)];
        }

        /// <summary>
        /// Determines if it has an executable ROM
        /// </summary>
        /// <returns>
        /// True if the memory has a ROM loaded
        /// </returns>
        public bool HasRomLoaded()
        {
            return this.Memory.Skip(C8Constants.StartRomAddress).Any(x => x != 0);
        }

        /// <summary>
        /// Fetches the opcode in memory using the program counter
        /// Program counter auto increases after opcode fetching for next instruction
        /// </summary>
        public void FetchOpcode()
        {
            this.CurrentOpcode = this.Memory[this.ProgramCounter];
            this.CurrentOpcode <<= 8;
            this.CurrentOpcode |= this.Memory[this.ProgramCounter + 1];

            this.IncreaseProgramCounter();
        }

        /// <summary>
        /// Increases the program counter
        /// </summary>
        public void IncreaseProgramCounter()
        {
            this.ProgramCounter += 2;
        }

        /// <summary>
        /// Initializes a clean machine state
        /// </summary>
        public void CleanMachineState()
        {
            // Cleans the graphics memory
            this.Graphics.SetAll(false);

            // Cleans the input memory
            this.Keys.SetAll(false);

            // Program counter starts at 0x200
            this.ProgramCounter = C8Constants.StartRomAddress;
            
            // Reset current current opcode	
            this.CurrentOpcode = 0;
            
            // Reset index register
            this.IndexRegister = 0; 

            // Clear stack
            this.Stack.Clear();

            // Clear registers V0-VF
            Array.Clear(this.VRegisters, 0, this.VRegisters.Length);

            // Clear memory
            Array.Clear(this.Memory, C8Constants.StartRomAddress, this.Memory.Length - C8Constants.StartRomAddress);

            // Reset timers
            this.SoundTimer = 0;
            this.DelayTimer = 0;
        }

        /// <summary>
        /// Loads the font set in memory
        /// </summary>
        public void LoadFontSet()
        {
            for (var i = 0; i < C8Constants.Chip8FontSet.Length; i++)
            {
                this.Memory[i] = C8Constants.Chip8FontSet[i];
            }
        }

        #endregion
    }
}
