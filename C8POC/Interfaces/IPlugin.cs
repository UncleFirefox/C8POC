// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPlugin.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Base interface for plugins
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    ///     Base interface for plugins
    /// </summary>
    public interface IPlugin
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Loads the about configuration plugin
        /// </summary>
        void AboutPlugin();

        /// <summary>
        ///     Boots up the configuration for the plugin
        /// </summary>
        /// <returns>
        ///     A dictionary with the resulting configuration
        /// </returns>
        IDictionary<string, string> Configure();

        /// <summary>
        ///     Action to disable a plugin when the emulator stops running
        /// </summary>
        void DisablePlugin();

        /// <summary>
        /// Action to enable a plugin when the emulator starts running
        /// </summary>
        /// <param name="parameters">
        /// Configuration parameters
        /// </param>
        void EnablePlugin(IDictionary<string, string> parameters);

        /// <summary>
        ///     Gets the default plugin configuration
        /// </summary>
        /// <returns>The default parameters for plugin configuration</returns>
        IDictionary<string, string> GetDefaultPluginConfiguration();

        #endregion
    }
}