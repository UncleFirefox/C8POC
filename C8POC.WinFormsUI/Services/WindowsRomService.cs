// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowsRomService.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   The windows rom service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.WinFormsUI.Services
{
    using System;
    using System.IO;

    using C8POC.Interfaces;

    /// <summary>
    /// The windows rom service.
    /// </summary>
    public class WindowsRomService : IRomService
    {
        /// <summary>
        /// Loads a ROM, from a given path, in memory
        /// </summary>
        /// <param name="romPath">
        /// The rom path.
        /// </param>
        /// <param name="machineState">
        /// The machine state.
        /// </param>
        public void LoadRom(string romPath, IMachineState machineState)
        {
            if (File.Exists(romPath))
            {
                var rom = new FileStream(romPath, FileMode.Open);

                if (rom.Length == 0)
                {
                    throw new Exception(string.Format("File '{0}' empty or damaged", romPath));
                }

                int index;

                // Load rom starting at 0x200
                for (index = 0; index < rom.Length; index++)
                {
                    machineState.Memory[C8Constants.StartRomAddress + index] = (byte)rom.ReadByte();
                }

                machineState.NumberOfOpcodeBytes = index;

                rom.Close();
            }
            else
            {
                throw new FileNotFoundException(string.Format("The file '{0}' does not exist", romPath));
            }
        }
    }
}
