using System;
using System.Collections.Generic;
using C8POC.Models;
using Microsoft.AspNet.SignalR;

namespace One
{
    public class EmulatorHub : Hub
    {
        public void ReceivedKey(string key)
        {
            //Send key to emulators
        }

        public IList<Pixel> Paint(Guid emulatorId)
        {
            //Paint client
            throw new NotImplementedException();
        }

        public Guid InitilizeEmulator()
        {
            //Start emulator and return the guid
            throw new NotImplementedException();
        }

        public void LoadRom(Guid emulatorId)
        {
            
        }
    }
}