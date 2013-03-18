using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C8POC.Plugins.Sound.ConsoleBeep
{
    using C8POC.Interfaces;

    public class ConsoleSoundBeepPlugin : ISoundPlugin
    {
        public void Configure()
        {
            throw new NotImplementedException();
        }

        public void AboutPlugin()
        {
            throw new NotImplementedException();
        }

        public void GenerateSound()
        {
            Console.Beep();
        }
    }
}
