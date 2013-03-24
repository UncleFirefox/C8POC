using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using C8POC.Interfaces;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;

namespace C8POC.Plugins.Keyboard.SystemWindowsKeyboard
{
    [Export(typeof(IKeyboardPlugin))]
    public class SystemWindowsKeyboardPlugin : IKeyboardPlugin
    {
        private readonly KeyboardHookListener _mKeyboardHookManager = new KeyboardHookListener(new GlobalHooker());

        public SystemWindowsKeyboardPlugin()
        {
            SetUpDefaultKeyMap();
        }

        public string PluginDescription
        {
            get { return "Windows Hooks System Implementation of Key Input Plugin"; }
        }

        public void Configure()
        {
            throw new NotImplementedException();
        }

        public void AboutPlugin()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Start capturing key input
        /// </summary>
        public void EnablePlugin()
        {
            _mKeyboardHookManager.Enabled = true;
            _mKeyboardHookManager.KeyUp += HookManagerOnKeyUp;
            _mKeyboardHookManager.KeyDown += HookManagerOnKeyDown;
        }

        private void HookManagerOnKeyDown(object sender, KeyEventArgs e)
        {
            byte mappedKeyIndex;

            if (this.keyMap.TryGetValue(e.KeyValue, out mappedKeyIndex))
            {
                if (this.KeyDown != null)
                {
                    this.KeyDown(mappedKeyIndex);
                }
            }
        }

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
        /// Stop capturing key input
        /// </summary>
        public void DisablePlugin()
        {
            _mKeyboardHookManager.Enabled = false;
        }

        public event KeyUpEventHandler KeyUp;
        public event KeyUpEventHandler KeyDown;

        /// <summary>
        /// Sets a default keyboard in groups of four 1-4,Q-R,A-F,Z-V
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

        private Dictionary<int, byte> keyMap = new Dictionary<int, byte>(); 
    }
}
