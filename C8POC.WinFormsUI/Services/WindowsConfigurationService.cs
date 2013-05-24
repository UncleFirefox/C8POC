// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowsConfigurationService.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Configuration service
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.WinFormsUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;

    using C8POC.Interfaces;
    using C8POC.Properties;

    /// <summary>
    /// Configuration service
    /// </summary>
    public class WindowsConfigurationService : IConfigurationService
    {
        /// <summary>
        ///     Gets the saved configuration of the engine
        /// </summary>
        /// <returns>
        ///     Dictionary containing the configuration
        /// </returns>
        public IDictionary<string, string> GetEngineConfiguration()
        {
            string configurationFullPath = this.GetClassConfigurationFullPath(typeof(C8Engine));

            if (File.Exists(configurationFullPath))
            {
                var map = new ExeConfigurationFileMap { ExeConfigFilename = configurationFullPath };
                Configuration engineconfig = ConfigurationManager.OpenMappedExeConfiguration(
                    map, ConfigurationUserLevel.None);

                return this.GetDictionaryFromAppSettings(engineconfig.AppSettings);
            }

            return new Dictionary<string, string>();
        }

        /// <summary>
        ///     Saves the engine configuration
        /// </summary>
        public void SaveEngineConfiguration()
        {
            Configuration engineconfig = this.GetClassConfiguration(typeof(C8Engine));

            foreach (SettingsProperty currentProperty in Settings.Default.Properties)
            {
                engineconfig.AppSettings.Settings.Add(
                    currentProperty.Name, Settings.Default[currentProperty.Name].ToString());
            }

            engineconfig.Save();
        }

        /// <summary>
        /// Gets the class configuration file
        /// </summary>
        /// <param name="classType">
        /// The class type
        /// </param>
        /// <returns>
        /// A configuration structure with proper paths
        /// </returns>
        private Configuration GetClassConfiguration(Type classType)
        {
            var map = new ExeConfigurationFileMap { ExeConfigFilename = this.GetClassConfigurationFullPath(classType) };
            Configuration engineconfig = ConfigurationManager.OpenMappedExeConfiguration(
                map, ConfigurationUserLevel.None);

            if (engineconfig.HasFile)
            {
                engineconfig.AppSettings.Settings.Clear();
            }

            return engineconfig;
        }

        /// <summary>
        /// Gets the full path of a configuration file of a given class inside a DLL
        /// </summary>
        /// <param name="typeOfClass">
        /// The type Of Class.
        /// </param>
        /// <returns>
        /// Full path to save the plugin
        /// </returns>
        private string GetClassConfigurationFullPath(Type typeOfClass)
        {
            string configurationFileName = string.Format(
                "{0}{1}", typeOfClass.Assembly.ManifestModule.ScopeName, ".config");
            string configurationFullPath =
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"C8POC\" + configurationFileName);

            return configurationFullPath;
        }

        /// <summary>
        /// Gets a parsed dictionary from appSettings for plugin configuration
        /// </summary>
        /// <param name="appSettings">
        /// Application settings section
        /// </param>
        /// <returns>
        /// A mapped dictionary
        /// </returns>
        private IDictionary<string, string> GetDictionaryFromAppSettings(AppSettingsSection appSettings)
        {
            return appSettings.Settings.Cast<KeyValueConfigurationElement>()
                              .ToDictionary(variable => variable.Key, variable => variable.Value);
        }
    }
}
