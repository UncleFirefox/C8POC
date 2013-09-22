// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMediatedEngine.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Defines the IMediatedEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Interfaces.Domain.Engines
{
    /// <summary>
    /// The Mediated Engine interface.
    /// </summary>
    public interface IMediatedEngine
    {
        /// <summary>
        /// Gets or sets the engine mediator.
        /// </summary>
        IEngineMediator EngineMediator { get; set; }

        /// <summary>
        /// The set mediator.
        /// </summary>
        /// <param name="engineMediator">
        /// The engine mediator.
        /// </param>
        void SetMediator(IEngineMediator engineMediator);
    }
}
