// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationEngine.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   The configuration engine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Core.Domain.Engines
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Linq;

    using C8POC.Core.Properties;
    using C8POC.Interfaces.Domain.Engines;
    using C8POC.Interfaces.Infrastructure.Services;

    /// <summary>
    /// The configuration engine.
    /// </summary>
    public class ConfigurationEngine : IConfigurationEngine
    {
        /// <summary>
        /// The saved settings.
        /// </summary>
        private IDictionary<string, string> savedSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationEngine"/> class.
        /// </summary>
        /// <param name="configurationService">
        /// The configuration service.
        /// </param>
        public ConfigurationEngine(IConfigurationService configurationService)
        {
            this.ConfigurationService = configurationService;
            this.LoadSavedEngineSettings();
        }

        /// <summary>
        /// Gets or sets the engine mediator.
        /// </summary>
        public IEngineMediator EngineMediator { get; set; }

        /// <summary>
        /// Gets or sets the configuration service.
        /// </summary>
        public IConfigurationService ConfigurationService { get; set; }

        /// <summary>
        /// The set mediator.
        /// </summary>
        /// <param name="engineMediator">
        /// The engine mediator.
        /// </param>
        public void SetMediator(IEngineMediator engineMediator)
        {
            this.EngineMediator = engineMediator;
        }

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
        public T GetConfigurationKeyOfType<T>(string configurationKey)
        {
            return (T)Settings.Default[configurationKey];
        }

        /// <summary>
        /// Loads the saved settings by the user
        /// </summary>
        private void LoadSavedEngineSettings()
        {
            this.savedSettings = this.ConfigurationService.GetEngineConfiguration();

            var validSettings =
                this.savedSettings.Where(
                    x => Settings.Default.Properties.Cast<SettingsProperty>().Any(setting => setting.Name == x.Key));

            foreach (var validSetting in validSettings)
            {
                var valueType = Settings.Default[validSetting.Key].GetType();
                Settings.Default[validSetting.Key] = this.GetValueWithGivenType(validSetting.Value, valueType);
            }
        }

        /// <summary>
        /// The get value with given type.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="requiredType">
        /// The required type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private object GetValueWithGivenType(string value, Type requiredType)
        {
            var foo = TypeDescriptor.GetConverter(requiredType);
            return foo.ConvertFromInvariantString(value);
        }
    }
}
