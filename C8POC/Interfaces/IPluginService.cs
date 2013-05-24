// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPluginService.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   The PluginService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Interfaces
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Plugin Service interface
    /// </summary>
    public interface IPluginService
    {
        /// <summary>
        /// Gets a plugin with a given Name Space
        /// </summary>
        /// <typeparam name="T">
        /// Plugin type for the parameter
        /// </typeparam>
        /// <param name="nameSpace">
        /// The name space
        /// </param>
        /// <returns>
        /// A typed plugin or null if no plugins are found
        /// </returns>
        T GetPluginByNameSpace<T>(string nameSpace) where T : class, IPlugin;

        /// <summary>
        /// Get the parameters configuration for the specified plugin
        /// </summary>
        /// <param name="plugin">
        /// The plugin to get the configuration from
        /// </param>
        /// <returns>
        /// A dictionary of parameters for the configuration
        /// </returns>
        IDictionary<string, string> GetPluginConfiguration(IPlugin plugin);

        /// <summary>
        /// The get plugins of type.
        /// </summary>
        /// <typeparam name="T"> Type of plugin
        /// </typeparam>
        /// <returns>
        /// The requested list of plugins
        /// </returns>
        IEnumerable<Lazy<T, IPluginMetadata>> GetPluginsOfType<T>() where T : class, IPlugin;

        /// <summary>
        /// Save the plugin configuration to a storage
        /// </summary>
        /// <param name="pluginConfiguration">
        /// Dictionary of parameters for the plugin
        /// </param>
        /// <param name="plugin">
        /// The plugin.
        /// </param>
        void SavePluginConfiguration(IDictionary<string, string> pluginConfiguration, IPlugin plugin);
    }
}