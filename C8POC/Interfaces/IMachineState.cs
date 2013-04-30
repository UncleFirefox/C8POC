// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMachineState.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   The MachineState interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Interfaces
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// The MachineState interface.
    /// </summary>
    public interface IMachineState
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the CurrentOpcode
        /// </summary>
        ushort CurrentOpcode { get; set; }

        /// <summary>
        ///     Gets or sets the Delay Timer
        /// </summary>
        ushort DelayTimer { get; set; }

        /// <summary>
        ///     Gets or sets the graphics array
        /// </summary>
        BitArray Graphics { get; set; }

        /// <summary>
        ///     Gets or sets the Index Register
        /// </summary>
        ushort IndexRegister { get; set; }

        /// <summary>
        ///     Gets or sets the draw flag
        /// </summary>
        bool IsDrawFlagSet { get; set; }

        /// <summary>
        ///     Gets or sets the Keys Array
        /// </summary>
        BitArray Keys { get; set; }

        /// <summary>
        ///     Gets or sets the machine memory
        /// </summary>
        byte[] Memory { get; set; }

        /// <summary>
        ///     Gets or sets the Program Counter
        /// </summary>
        ushort ProgramCounter { get; set; }

        /// <summary>
        ///     Gets or sets the Sound Timer
        /// </summary>
        ushort SoundTimer { get; set; }

        /// <summary>
        ///     Gets or sets the stack
        /// </summary>
        Stack<ushort> Stack { get; set; }

        /// <summary>
        ///     Gets or sets the VRegisters
        /// </summary>
        ushort[] VRegisters { get; set; }

        /// <summary>
        ///     Gets x value in opcodes like 5xy0 etc
        /// </summary>
        ushort XRegisterFromCurrentOpcode { get; }

        /// <summary>
        ///     Gets x value in opcodes like 5xy0 etc
        /// </summary>
        ushort YRegisterFromCurrentOpcode { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initializes a clean machine state
        /// </summary>
        void CleanMachineState();

        /// <summary>
        ///     Fetches the opcode in memory using the program counter
        ///     Program counter auto increases after opcode fetching for next instruction
        /// </summary>
        void FetchOpcode();

        /// <summary>
        /// Gets the state of a pixel, take into account that
        ///     screen starts at upper left corner (0,0) and ends at lower right corner (63,31)
        /// </summary>
        /// <param name="x">
        /// Horizontal on screen
        /// </param>
        /// <param name="y">
        /// Vertical on screen
        /// </param>
        /// <returns>
        /// If the pixel is set or not
        /// </returns>
        bool GetPixelState(int x, int y);

        /// <summary>
        ///     Determines if it has an executable ROM
        /// </summary>
        /// <returns>
        ///     True if the memory has a ROM loaded
        /// </returns>
        bool HasRomLoaded();

        /// <summary>
        ///     Increases the program counter
        /// </summary>
        void IncreaseProgramCounter();

        /// <summary>
        ///     Loads the font set in memory
        /// </summary>
        void LoadFontSet();

        #endregion
    }
}