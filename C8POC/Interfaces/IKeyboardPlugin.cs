namespace C8POC.Interfaces
{
    /// <summary>
    /// Event handler raised when the user releases a key
    /// </summary>
    /// <param name="keyIndex">The released key index</param>
    public delegate void KeyUpEventHandler(byte keyIndex);

    /// <summary>
    /// Event handler raised when the user presses a key
    /// </summary>
    /// <param name="keyIndex">They pressed key index</param>
    public delegate void KeyDownEventHandler(byte keyIndex);

    /// <summary>
    /// Event handler for the key that stops the emulation
    /// </summary>
    public delegate void KeyStopEmulationEventHandler();

    /// <summary>
    /// Interface that any keyboard plugin should implement
    /// </summary>
    public interface IKeyboardPlugin : IPlugin
    {
        /// <summary>
        /// Event that will be raised every time that a key is pressed
        /// </summary>
        event KeyUpEventHandler KeyUp;

        /// <summary>
        /// Event that will be raised every time that a key is released
        /// </summary>
        event KeyDownEventHandler KeyDown;

        /// <summary>
        /// Event that will be raised every time that the stop emulation key is pressed
        /// </summary>
        event KeyStopEmulationEventHandler KeyStopEmulation;
    }
}