using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using C8POC.Interfaces;
using Gma.UserActivityMonitor;

namespace C8POC.Plugins.Keyboard.SystemWindowsKeyboard
{
    public class SystemWindowsKeyboardPlugin : IKeyboardPlugin
    {
        public string PluginDescription
        {
            get { return "Windows System Implementation of Key Input Plugin"; }
        }

        public void Configure()
        {
            throw new NotImplementedException();
        }

        public void AboutPlugin()
        {
            throw new NotImplementedException();
        }

        public void EnablePlugin()
        {
            HookManager.KeyUp += HookManagerOnKeyUp;
            HookManager.KeyDown += HookManager_KeyDown;
        }

        void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HookManagerOnKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            throw new NotImplementedException();
        }

        public void DisablePlugin()
        {
            throw new NotImplementedException();
        }

        public void KeyDown()
        {
            throw new NotImplementedException();
        }

        public void KeyUp()
        {
            throw new NotImplementedException();
        }
    }
}
