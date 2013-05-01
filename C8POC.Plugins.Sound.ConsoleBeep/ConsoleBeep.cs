// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleBeep.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Early implementation of a Sound Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace C8POC.Plugins.Sound.ConsoleBeep
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using C8POC.Interfaces;

    /// <summary>
    ///     Early implementation of a Sound Class
    /// </summary>
    [Export(typeof(ISoundPlugin))]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("NameSpace", "C8POC.Plugins.Sound.ConsoleBeep.ConsoleSoundBeepPlugin")]
    [ExportMetadata("Description", "Sound Plugin Based on Console.Beep")]
    public class ConsoleSoundBeepPlugin : ISoundPlugin
    {
        #region Public Methods and Operators

        /// <summary>
        ///     About plugin implementation
        /// </summary>
        public void AboutPlugin()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Configuration for plugin
        /// </summary>
        /// <param name="currentConfiguration">
        /// The current Configuration.
        /// </param>
        /// <returns>
        /// Resulting configuration
        /// </returns>
        public IDictionary<string, string> Configure(IDictionary<string, string> currentConfiguration)
        {
            return null;
        }

        /// <summary>
        ///     Disable plugin Action
        /// </summary>
        public void DisablePlugin()
        {
        }

        /// <summary>
        /// Enables the plugin with specified parameters
        /// </summary>
        /// <param name="parameters">
        /// Parameters for plugin
        /// </param>
        public void EnablePlugin(IDictionary<string, string> parameters)
        {
        }

        /// <summary>
        ///     Action to generate sound
        /// </summary>
        public void GenerateSound()
        {
            Console.Beep();
        }

        /// <summary>
        ///     Gets the default configuration of the plugin
        /// </summary>
        /// <returns>Default configuration for plugin</returns>
        public IDictionary<string, string> GetDefaultPluginConfiguration()
        {
            return null;
        }

        #endregion
    }
}