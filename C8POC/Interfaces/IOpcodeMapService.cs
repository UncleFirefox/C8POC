// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOpcodeMapService.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Defines the IOpcodeMapService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Interfaces
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The Opcode Map Service interface.
    /// </summary>
    public interface IOpcodeMapService
    {
        /// <summary>
        /// Gets or sets the opcode processor.
        /// </summary>
        IOpcodeProcessor OpcodeProcessor { get; set; }
        
        /// <summary>
        /// Gets the instruction map
        /// </summary>
        /// <returns>
        /// An instruction map
        /// </returns>
        Dictionary<ushort, Action<IMachineState>> GetInstructionMap();
    }
}
