// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISoundPlugin.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Interface that any sound plugin should implement
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Interfaces.Domain.Plugins
{
    /// <summary>
    ///     Interface that any sound plugin should implement
    /// </summary>
    public interface ISoundPlugin : IPlugin
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Action raised to generate a sound
        /// </summary>
        void GenerateSound();

        #endregion
    }
}