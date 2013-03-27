using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using SdlDotNet.Graphics;

namespace C8POC.Plugins.Graphics.SDLPlugin
{
    using System.Collections;
    using System.ComponentModel.Composition;

    using C8POC.Interfaces;
    using SdlDotNet.Core;

    [Export(typeof(IGraphicsPlugin))]
    public class SDLPlugin : IGraphicsPlugin
    {
        /// <summary>
        /// Pixel width
        /// </summary>
        private int PixelWidth = 10;

        /// <summary>
        /// Pixel height
        /// </summary>
        private int PixelHeight = 10;

        /// <summary>
        /// White pixel surface
        /// </summary>
        private Surface whitePixel;

        /// <summary>
        /// Indicates if the screen has been closed by code
        /// </summary>
        private bool formClosedByCode { get; set; }

        /// <summary>
        /// Form in which render takes places
        /// </summary>
        private RenderForm renderForm { get; set; }

        /// <summary>
        /// Surface that takes the rendering
        /// </summary>
        private Surface renderSurface { get; set; }

        /// <summary>
        /// Gets the plugin description
        /// </summary>
        public string PluginDescription
        {
            get
            {
                return "SDL Graphics Plugin";
            }
        }

        public void Configure()
        {
            throw new NotImplementedException();
        }

        public void AboutPlugin()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Action enabling the plugin
        /// </summary>
        public void EnablePlugin()
        {
            renderForm = new RenderForm();
            renderSurface = Video.CreateRgbSurface(C8Constants.ResolutionWidth*this.PixelWidth,
                                                   C8Constants.ResolutionHeight*this.PixelHeight);
            
            whitePixel = Video.CreateRgbSurface(PixelWidth, PixelHeight);
            whitePixel.Fill(Color.White);

            renderForm.Closed += RenderFormClosed;

            this.renderForm.Show();
        }

        /// <summary>
        /// Event raised when the form is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RenderFormClosed(object sender, EventArgs e)
        {
            if (!this.formClosedByCode && this.GraphicsExit != null)
            {
                this.formClosedByCode = false;
                this.GraphicsExit();
            }
        }

        /// <summary>
        /// Action deactivating the plugin
        /// </summary>
        public void DisablePlugin()
        {
            if (this.renderForm != null && this.IsFormOpen(this.renderForm))
            {
                this.formClosedByCode = true;
                this.renderForm.Close();
            }
        }

        /// <summary>
        /// Event raised when the screen is closed by the user
        /// </summary>
        public event GraphicsExitEventHandler GraphicsExit;

        /// <summary>
        /// Drawing event
        /// </summary>
        /// <param name="graphics">A bit array containing graphics</param>
        public void Draw(BitArray graphics)
        {

            // Go through each pixel on the screen
            for (int y = 0; y < C8Constants.ResolutionHeight; y++)
            {
                for (var x = 0; x < C8Constants.ResolutionWidth; x++)
                {
                    if (this.GetPixelState(graphics, x, y))
                    {
                        renderSurface.Blit(whitePixel,
                                           new Rectangle(x*PixelWidth, y*PixelWidth, PixelWidth, PixelHeight));
                    }
                }
            }

            renderSurface.Update();
            renderForm.surfaceControlC8.Blit(renderSurface);
            renderForm.surfaceControlC8.Update();
        }

        /// <summary>
        /// Determines if the graphics form is open
        /// </summary>
        /// <param name="form">The form to check</param>
        /// <returns>Is the form open?</returns>
        private bool IsFormOpen(RenderForm form)
        {
            return Application.OpenForms
                              .Cast<Form>()
                              .Any(x => x.GetType() == form.GetType());
        }

        /// <summary>
        /// Gets the state of a pixel, take into account that
        /// screen starts at upper left corner (0,0) and ends at lower right corner (63,31)
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
            return graphics[x + (C8Constants.ResolutionWidth * y)]; //C8Constants.ResolutionWidth is the resolution width of the screen
        }
    }
}
