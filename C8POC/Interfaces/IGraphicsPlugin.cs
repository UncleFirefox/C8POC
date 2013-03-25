using System.Collections;
namespace C8POC.Interfaces
{
    /// <summary>
    /// Event handler for key input press
    /// </summary>
    public delegate void GraphicsExitEventHandler();

    /// <summary>
    /// Interface for graphics plugins
    /// </summary>
    public interface IGraphicsPlugin : IPlugin
    {
        /// <summary>
        /// Action that will stop emulation when the graphics screen is closed
        /// </summary>
        event GraphicsExitEventHandler GraphicsExit;

        /// <summary>
        /// Action to Draw
        /// </summary>
        /// <param name="graphics">
        /// The graphics array.
        /// </param>
        void Draw(BitArray graphics);
    }
}