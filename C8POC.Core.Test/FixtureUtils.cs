// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FixtureUtils.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Defines the FixtureUtils type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Core.Test
{
    using C8POC.Core.Domain.Entities;
    using C8POC.Interfaces.Domain.Entities;

    /// <summary>
    /// The fixture utils.
    /// </summary>
    public class FixtureUtils
    {
        /// <summary>
        /// Gets a default MachineState instance
        /// </summary>
        /// <returns>
        /// A machine state instance
        /// </returns>
        public static IMachineState DefaultMachineState()
        {
            var result = new C8MachineState();
            return result;
        }
    }
}
