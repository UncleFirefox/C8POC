// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPluginMetadata.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   The Plugin Metadata interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Interfaces
{
    /// <summary>
    /// The Plugin Metadata interface.
    /// </summary>
    public interface IPluginMetadata
    {
        #region Public Properties

        /// <summary>
        /// Gets the description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the name space.
        /// </summary>
        string NameSpace { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        string Version { get; }

        #endregion
    }
}