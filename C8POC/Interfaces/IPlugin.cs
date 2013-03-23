namespace C8POC.Interfaces
{
    /// <summary>
    /// Base interface for plugins
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Gets the plugin description
        /// </summary>
        string PluginDescription { get; }

        /// <summary>
        /// Boots up the configuration for the plugin
        /// </summary>
        void Configure();

        /// <summary>
        /// Loads the about configuration plugin
        /// </summary>
        void AboutPlugin();

        /// <summary>
        /// Action to enable a plugin when the emulator starts running
        /// </summary>
        void EnablePlugin();

        /// <summary>
        /// Action to disable a plugin when the emulator stops running
        /// </summary>
        void DisablePlugin();
    }
}