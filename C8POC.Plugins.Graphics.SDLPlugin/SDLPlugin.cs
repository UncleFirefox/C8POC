// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SDLPlugin.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   SDL implementation of graphics plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Plugins.Graphics.SDLPlugin
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using C8POC.Interfaces;

    using SdlDotNet.Graphics;

    /// <summary>
    /// SDL implementation of graphics plugin
    /// </summary>
    [Export(typeof(IGraphicsPlugin))]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("NameSpace", "C8POC.Plugins.Graphics.SDLPlugin.SDLPlugin")]
    [ExportMetadata("Description", "Graphics plugin based on SDL")]
    public class SdlPlugin : IGraphicsPlugin
    {
        #region Fields

        /// <summary>
        ///     Pixel height
        /// </summary>
        private const int PixelHeight = 10;

        /// <summary>
        ///     Pixel width
        /// </summary>
        private const int PixelWidth = 10;

        /// <summary>
        ///     White pixel surface
        /// </summary>
        private Surface whitePixel;

        #endregion

        #region Public Events

        /// <summary>
        ///     Event raised when the screen is closed by the user
        /// </summary>
        public event GraphicsExitEventHandler GraphicsExit;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the form has been closed by code
        /// </summary>
        private bool FormClosedByCode { get; set; }

        /// <summary>
        ///     Gets or sets the Form in which render takes places
        /// </summary>
        private RenderForm RenderForm { get; set; }

        /// <summary>
        ///     Gets or sets the Surface that takes the rendering
        /// </summary>
        private Surface RenderSurface { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Show an about message
        /// </summary>
        public void AboutPlugin()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Configures the plugin
        /// </summary>
        /// <param name="currentConfiguration">
        /// The current Configuration.
        /// </param>
        /// <returns>
        /// Dictionary containing parameters
        /// </returns>
        public IDictionary<string, string> Configure(IDictionary<string, string> currentConfiguration)
        {
            return new Dictionary<string, string> { { "TruchaKey", "TruchaValue" } };
        }

        /// <summary>
        ///     Action deactivating the plugin
        /// </summary>
        public void DisablePlugin()
        {
            if (!this.FormClosedByCode && this.GraphicsExit != null)
            {
                this.FormClosedByCode = true;

                // The thread safe way to close the form :)
                if (this.RenderForm.InvokeRequired)
                {
                    this.RenderForm.BeginInvoke(new Action(() => this.RenderForm.Close()));
                    return;
                }

                this.RenderForm.Close();
            }
        }

        /// <summary>
        /// Drawing event
        /// </summary>
        /// <param name="graphics">
        /// A bit array containing graphics
        /// </param>
        public void Draw(BitArray graphics)
        {
            this.RenderSurface.Fill(Color.Black);

            // Go through each pixel on the screen
            for (int y = 0; y < C8Constants.ResolutionHeight; y++)
            {
                for (int x = 0; x < C8Constants.ResolutionWidth; x++)
                {
                    if (this.GetPixelState(graphics, x, y))
                    {
                        this.RenderSurface.Blit(
                            this.whitePixel, 
                            new Rectangle(x * PixelWidth, y * PixelWidth, PixelWidth, PixelHeight));
                    }
                }
            }

            this.RenderSurface.Update();

            if (this.RenderForm.InvokeRequired)
            {
                this.RenderForm.BeginInvoke(new Action(this.RenderSurfaceInWindow));
            }
            else
            {
                this.RenderSurfaceInWindow();
            }
        }

        /// <summary>
        /// Enables the plugin
        /// </summary>
        /// <param name="parameters">
        /// Plugin configuration parameters
        /// </param>
        public void EnablePlugin(IDictionary<string, string> parameters)
        {
            this.RenderForm = new RenderForm();
            this.RenderSurface = Video.CreateRgbSurface(
                C8Constants.ResolutionWidth * PixelWidth, C8Constants.ResolutionHeight * PixelHeight);

            this.whitePixel = Video.CreateRgbSurface(PixelWidth, PixelHeight);
            this.whitePixel.Fill(Color.White);

            this.RenderForm.Closed += this.RenderFormClosed;

            this.RenderForm.Show();
        }

        /// <summary>
        ///     Gets the default plugin configuration
        /// </summary>
        /// <returns>Default plugin configuration</returns>
        public IDictionary<string, string> GetDefaultPluginConfiguration()
        {
            return null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the state of a pixel, take into account that
        ///     screen starts at upper left corner (0,0) and ends at lower right corner (63,31)
        /// </summary>
        /// <param name="graphics">
        /// The graphics.
        /// </param>
        /// <param name="x">
        /// Horizontal position
        /// </param>
        /// <param name="y">
        /// Vertical position
        /// </param>
        /// <returns>
        /// If the pixel set or not
        /// </returns>
        private bool GetPixelState(BitArray graphics, int x, int y)
        {
            return graphics[x + (C8Constants.ResolutionWidth * y)];
                
                // C8Constants.ResolutionWidth is the resolution width of the screen
        }

        /// <summary>
        /// Determines if the graphics form is open
        /// </summary>
        /// <param name="form">
        /// The form to check
        /// </param>
        /// <returns>
        /// Is the form open?
        /// </returns>
        private bool IsFormOpen(RenderForm form)
        {
            return Application.OpenForms.Cast<Form>().Any(x => x.GetType() == form.GetType());
        }

        /// <summary>
        /// Event raised when the form is closed
        /// </summary>
        /// <param name="sender"> The form
        /// </param>
        /// <param name="e"> Some event arguments
        /// </param>
        private void RenderFormClosed(object sender, EventArgs e)
        {
            if (this.GraphicsExit != null && this.IsFormOpen(this.RenderForm))
            {
                this.FormClosedByCode = true;
                this.GraphicsExit();
            }
        }

        /// <summary>
        /// The render surface in window.
        /// </summary>
        private void RenderSurfaceInWindow()
        {
            this.RenderForm.surfaceControlC8.Blit(this.RenderSurface);
            this.RenderForm.surfaceControlC8.Update();
        }

        #endregion
    }
}