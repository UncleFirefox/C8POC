// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemWindowsKeyboardPlugin.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Implementation of a keyboard plugin using Windows Hooks
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Plugins.Keyboard.SystemWindowsKeyboard
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows.Forms;

    using C8POC.Interfaces;

    using MouseKeyboardActivityMonitor;
    using MouseKeyboardActivityMonitor.WinApi;

    /// <summary>
    ///     Implementation of a keyboard plugin using Windows Hooks
    /// </summary>
    [Export(typeof(IKeyboardPlugin))]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("NameSpace", "C8POC.Plugins.Keyboard.SystemWindowsKeyboard.SystemWindowsKeyboardPlugin")]
    [ExportMetadata("Description", "Windows Hooks System Implementation of Key Input Plugin")]
    public class SystemWindowsKeyboardPlugin : IKeyboardPlugin
    {
        #region Fields

        /// <summary>
        ///     Local mapper between actual key codes and keys in the emulator
        /// </summary>
        private readonly Dictionary<int, byte> keyMap = new Dictionary<int, byte>();

        /// <summary>
        ///     Keyboard listener for hooks
        /// </summary>
        private readonly KeyboardHookListener keyboardHookManager = new KeyboardHookListener(new GlobalHooker());

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SystemWindowsKeyboardPlugin" /> class.
        /// </summary>
        public SystemWindowsKeyboardPlugin()
        {
            this.SetUpDefaultKeyMap();
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Event that will fire on key down
        /// </summary>
        public event KeyDownEventHandler KeyDown;

        /// <summary>
        ///     Event that will fire when the exit key is pressed
        /// </summary>
        public event KeyStopEmulationEventHandler KeyStopEmulation;

        /// <summary>
        ///     Event that will fire on key up
        /// </summary>
        public event KeyUpEventHandler KeyUp;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets an about message
        /// </summary>
        public void AboutPlugin()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a prompt for configuration
        /// </summary>
        /// <returns>
        /// Dictionary containing parameters
        /// </returns>
        public IDictionary<string, string> Configure()
        {
            return null;
        }

        /// <summary>
        ///     Stop capturing key input
        /// </summary>
        public void DisablePlugin()
        {
            this.keyboardHookManager.Enabled = false;
            this.keyboardHookManager.KeyUp -= this.HookManagerOnKeyUp;
            this.keyboardHookManager.KeyDown -= this.HookManagerOnKeyDown;
        }

        /// <summary>
        /// Enables the plugin
        /// </summary>
        /// <param name="parameters">
        /// Plugin configuration parameters
        /// </param>
        public void EnablePlugin(IDictionary<string, string> parameters)
        {
            this.keyboardHookManager.KeyUp += this.HookManagerOnKeyUp;
            this.keyboardHookManager.KeyDown += this.HookManagerOnKeyDown;
            this.keyboardHookManager.Enabled = true;
        }

        /// <summary>
        ///     Gets the default configuration for the plugin
        /// </summary>
        /// <returns>The default plugin configuration</returns>
        public IDictionary<string, string> GetDefaultPluginConfiguration()
        {
            return null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Hook for key pressing
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void HookManagerOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (this.KeyStopEmulation != null)
                {
                    this.KeyStopEmulation();
                }

                return;
            }

            byte mappedKeyIndex;

            if (this.keyMap.TryGetValue(e.KeyValue, out mappedKeyIndex))
            {
                if (this.KeyDown != null)
                {
                    this.KeyDown(mappedKeyIndex);
                }
            }
        }

        /// <summary>
        /// Hook for key releasing
        /// </summary>
        /// <param name="sender">
        /// The sender
        /// </param>
        /// <param name="e">
        /// Key event args
        /// </param>
        private void HookManagerOnKeyUp(object sender, KeyEventArgs e)
        {
            byte mappedKeyIndex;

            if (this.keyMap.TryGetValue(e.KeyValue, out mappedKeyIndex))
            {
                if (this.KeyUp != null)
                {
                    this.KeyUp(mappedKeyIndex);
                }
            }
        }

        /// <summary>
        ///     Sets a default keyboard in groups of four 1-4,Q-R,A-F,Z-V
        /// </summary>
        private void SetUpDefaultKeyMap()
        {
            this.keyMap.Add(49, 0x0);
            this.keyMap.Add(50, 0x1);
            this.keyMap.Add(51, 0x2);
            this.keyMap.Add(52, 0x3);
            this.keyMap.Add(81, 0x4);
            this.keyMap.Add(87, 0x5);
            this.keyMap.Add(69, 0x6);
            this.keyMap.Add(82, 0x7);
            this.keyMap.Add(65, 0x8);
            this.keyMap.Add(83, 0x9);
            this.keyMap.Add(68, 0xA);
            this.keyMap.Add(70, 0xB);
            this.keyMap.Add(90, 0xC);
            this.keyMap.Add(88, 0xD);
            this.keyMap.Add(67, 0xE);
            this.keyMap.Add(86, 0xF);
        }

        #endregion
    }
}