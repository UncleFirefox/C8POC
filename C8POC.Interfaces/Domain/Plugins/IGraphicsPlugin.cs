// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGraphicsPlugin.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Event handler for key input press
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Interfaces.Domain.Plugins
{
    using System.Collections;

    /// <summary>
    ///     Event handler for key input press
    /// </summary>
    public delegate void GraphicsExitEventHandler();

    /// <summary>
    ///     Interface for graphics plugins
    /// </summary>
    public interface IGraphicsPlugin : IPlugin
    {
        #region Public Events

        /// <summary>
        ///     Action that will stop emulation when the graphics screen is closed
        /// </summary>
        event GraphicsExitEventHandler GraphicsExit;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Action to Draw
        /// </summary>
        /// <param name="graphics">
        /// The graphics array.
        /// </param>
        void Draw(BitArray graphics);

        #endregion
    }
}