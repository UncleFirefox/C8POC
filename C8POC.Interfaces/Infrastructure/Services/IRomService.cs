// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRomService.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Defines the IRomService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Interfaces.Infrastructure.Services
{
    using C8POC.Interfaces.Domain.Entities;

    /// <summary>
    /// The RomService interface.
    /// </summary>
    public interface IRomService
    {
        /// <summary>
        /// Loads a ROM, from a given path, in memory
        /// </summary>
        /// <param name="romPath">
        /// The rom Path.
        /// </param>
        /// <param name="machineState">
        /// The machine State.
        /// </param>
        void LoadRom(string romPath, IMachineState machineState);
    }
}
