// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginContainer.cs" company="">
//   
// </copyright>
// <summary>
//   Class implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.WinFormsUI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using Interfaces;

    /// <summary>
    /// Class implementation
    /// </summary>
    public class PluginContainer
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the list of graphic plugins it could be done with IEnumerable<Lazy<IGraphicsPlugin>>
        /// </summary>
        [ImportMany(typeof(IGraphicsPlugin))]
        public IEnumerable<IGraphicsPlugin> GraphicsPlugins { get; set; }

        /// <summary>
        /// Gets or sets the list of keyboard plugins
        /// </summary>
        [ImportMany(typeof(IKeyboardPlugin))]
        public IEnumerable<IKeyboardPlugin> KeyboardPlugins { get; set; }

        /// <summary>
        /// Gets or sets the list of sound plugins
        /// </summary>
        [ImportMany(typeof(ISoundPlugin))]
        public IEnumerable<ISoundPlugin> SoundPlugins { get; set; }

        #endregion
    }
}