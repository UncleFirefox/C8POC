using System.Collections;

namespace C8POC.Interfaces
{
    /// <summary>
    /// Event handler for key input press
    /// </summary>
    public delegate void KeyUpEventHandler(byte keyIndex);

    /// <summary>
    /// Event handler for key input release
    /// </summary>
    public delegate void KeyDownEventHandler(byte keyIndex);

    /// <summary>
    /// Event handler for the key that stops the emulation
    /// </summary>
    public delegate void KeyStopEmulationEventHandler();

    public interface IKeyboardPlugin : IPlugin
    {
        /// <summary>
        /// Event that will be raised every time that a key is pressed
        /// </summary>
        event KeyUpEventHandler KeyUp;

        /// <summary>
        /// Event that will be raised every time that a key is released
        /// </summary>
        event KeyUpEventHandler KeyDown;

        /// <summary>
        /// Event that will be raised every time that the stop emulation key is pressed
        /// </summary>
        event KeyStopEmulationEventHandler KeyStopEmulation;
    }
}