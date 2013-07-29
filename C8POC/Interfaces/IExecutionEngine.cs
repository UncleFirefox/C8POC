// -----------------------------------------------------------------------
// <copyright file="IExecutionEngine.cs" company="AlFranco">
// Albert Rodriguez Franco 2013
// </copyright>
// -----------------------------------------------------------------------

namespace C8POC.Interfaces
{
    /// <summary>
    /// Represents the engine in charge of the execution
    /// </summary>
    public interface IExecutionEngine
    {
        /// <summary>
        /// Gets or sets the mediator.
        /// </summary>
        IEngineMediator EngineMediator { get; set; }

        /// <summary>
        /// Gets or sets the opcode map service.
        /// </summary>
        IOpcodeMapService OpcodeMapService { get; set; }

        /// <summary>
        /// Sets the mediator for the engine
        /// </summary>
        /// <param name="engineMediator">
        /// The engine mediator.
        /// </param>
        void SetMediator(IEngineMediator engineMediator);

        /// <summary>
        /// Starts the execution of the emulator
        /// </summary>
        void StartEmulatorExecution();
    }
}
