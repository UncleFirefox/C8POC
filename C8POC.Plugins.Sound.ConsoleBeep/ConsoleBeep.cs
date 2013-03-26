namespace C8POC.Plugins.Sound.ConsoleBeep
{
    using System;
    using System.ComponentModel.Composition;
    using System.Windows.Forms;

    using C8POC.Interfaces;

    /// <summary>
    /// Early implementation of a Sound Class
    /// </summary>
    [Export(typeof(ISoundPlugin))]
    public class ConsoleSoundBeepPlugin : ISoundPlugin
    {
        /// <summary>
        /// Gets the plugin description
        /// </summary>
        public string PluginDescription
        {
            get
            {
                return "Sound Plugin Based on Console.Beep vBeta";
            }
        }

        /// <summary>
        /// Configuration for plugin
        /// </summary>
        public void Configure()
        {
            MessageBox.Show("Nothing to configure", "Plugin Configuration");
        }

        /// <summary>
        /// About plugin implementation
        /// </summary>
        public void AboutPlugin()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Enable Plugin Action
        /// </summary>
        public void EnablePlugin()
        {
        }

        /// <summary>
        /// Disable plugin Action
        /// </summary>
        public void DisablePlugin()
        {
        }

        /// <summary>
        /// Action to generate sound
        /// </summary>
        public void GenerateSound()
        {
            Console.Beep();
        }
    }
}
