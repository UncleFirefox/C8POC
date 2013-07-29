// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationEngine.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Defines the IConfigurationEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Interfaces
{
    /// <summary>
    /// The Configuration Engine interface.
    /// </summary>
    public interface IConfigurationEngine
    {
        /// <summary>
        /// Gets or sets the engine mediator.
        /// </summary>
        IEngineMediator EngineMediator { get; set; }

        /// <summary>
        /// Gets or sets the configuration service.
        /// </summary>
        IConfigurationService ConfigurationService { get; set; }

        /// <summary>
        /// The set mediator.
        /// </summary>
        /// <param name="engineMediator">
        /// The engine mediator.
        /// </param>
        void SetMediator(IEngineMediator engineMediator);

        /// <summary>
        /// The get configuration key of type.
        /// </summary>
        /// <param name="configurationKey">
        /// The configuration key.
        /// </param>
        /// <typeparam name="T">
        /// Type of parameter
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T GetConfigurationKeyOfType<T>(string configurationKey);
    }
}