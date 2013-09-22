// -----------------------------------------------------------------------
// <copyright file="IExecutionEngine.cs" company="AlFranco">
// Albert Rodriguez Franco 2013
// </copyright>
// -----------------------------------------------------------------------

namespace C8POC.Interfaces.Domain.Engines
{
    using C8POC.Interfaces.Domain.Services;

    /// <summary>
    /// Represents the engine in charge of the execution
    /// </summary>
    public interface IExecutionEngine : IMediatedEngine
    {
        /// <summary>
        /// Gets or sets the opcode map service.
        /// </summary>
        IOpcodeMapService OpcodeMapService { get; set; }

        /// <summary>
        /// Starts the execution of the emulator
        /// </summary>
        void StartEmulatorExecution();
    }
}
