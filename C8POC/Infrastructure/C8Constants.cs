// --------------------------------------------------------------------------------------------------------------------
// <copyright file="C8Constants.cs" company="">
//   
// </copyright>
// <summary>
//   Class for constants
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Core.Infrastructure
{
    /// <summary>
    /// Class for constants
    /// </summary>
    public static class C8Constants
    {
        /// <summary>
        /// Address in memory where the ROM content starts
        /// </summary>
        public const ushort StartRomAddress = 0x200;

        /// <summary>
        /// Horizontal resolution of graphics screen
        /// </summary>
        public const ushort ResolutionWidth = 64;

        /// <summary>
        /// Vertical resolution of graphics screen
        /// </summary>
        public const ushort ResolutionHeight = 32;

        /// <summary>
        /// Memory size in Bytes
        /// </summary>
        public const int MemorySize = 4096;

        /// <summary>
        /// Number of V registers
        /// </summary>
        public const int NumVRegisters = 16;

        /// <summary>
        /// Size of elements in the stack
        /// </summary>
        public const int StackSize = 16;

        /// <summary>
        /// Number of keys in emulator
        /// </summary>
        public const int NumKeys = 16;

        /// <summary>
        /// Gets the default font set
        /// </summary>
        public static byte[] Chip8FontSet
        {
            get
            {
                return new byte[] 
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
            }
        }
    }
}
