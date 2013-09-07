// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationService.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Defines the IConfigurationService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Interfaces.Infrastructure.Services
{
    using System.Collections.Generic;

    /// <summary>
    /// Configuration Service Interface
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        ///     Gets the saved configuration of the engine
        /// </summary>
        /// <returns>
        ///     Dictionary containing the configuration
        /// </returns>
        IDictionary<string, string> GetEngineConfiguration();

        /// <summary>
        ///     Saves the engine configuration
        /// </summary>
        void SaveEngineConfiguration();
    }
}
