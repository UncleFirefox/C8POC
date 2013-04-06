namespace C8POC.Plugins.Graphics.GDIPlugin
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using C8POC.Interfaces;

    /// <summary>
    /// Graphics plugin with GDI primitives for Windows Forms
    /// </summary>
    [Export(typeof(IGraphicsPlugin))]
    public class GdiPlugin : IGraphicsPlugin
    {
        #region Properties

        /// <summary>
        /// The brush.
        /// </summary>
        private readonly Brush brush = new SolidBrush(Color.White);

        /// <summary>
        /// From that will be displayed when the plugin is activated
        /// </summary>
        private GraphicsForm graphicsForm;
        
        #endregion
        
        #region IGraphicsPlugin Members

        /// <summary>
        /// Gets the plugin description
        /// </summary>
        public string PluginDescription
        {
            get { return "Graphics plugin with GDI primitives for Windows Forms"; }
        }

        /// <summary>
        /// Prompts for a configuration
        /// </summary>
        public void Configure()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the about plugin window
        /// </summary>
        public void AboutPlugin()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Enables the plugin
        /// </summary>
        public void EnablePlugin()
        {
            this.graphicsForm = new GraphicsForm();
            this.graphicsForm.Closed += this.GraphicsFormClosed;
            this.graphicsForm.Show();
        }

        /// <summary>
        /// Disables de plugin
        /// </summary>
        public void DisablePlugin()
        {
            if (this.graphicsForm != null && this.IsFormOpen(this.graphicsForm))
            {
                this.FormClosedByCode = true;
                
                //The thread safe way to close the form :)
                if (this.graphicsForm.InvokeRequired)
                {
                    this.graphicsForm.BeginInvoke(new Action(() => this.graphicsForm.Close()));
                    return;
                }

                this.graphicsForm.Close();
            }
        }

        /// <summary>
        /// Drawing implementation for the plugin
        /// </summary>
        /// <param name="graphics">The graphics array</param>
        public void Draw(BitArray graphics)
        {
            var rectangles = new List<Rectangle>();

            //Go through each pixel on the screen
            for (int y = 0; y < C8Constants.ResolutionHeight; y++)
            {
                for (var x = 0; x < C8Constants.ResolutionWidth; x++)
                {
                    if (GetPixelState(graphics, x, y))
                    {
                        rectangles.Add(new Rectangle(x * 10, y * 10, 10, 10));
                    }
                }
            }

            if (rectangles.Count > 0)
            {
                using (var gfx = this.graphicsForm.renderingPanel.CreateGraphics())
                {
                    gfx.Clear(Color.Black);
                    gfx.FillRectangles(this.brush, rectangles.ToArray());
                }
            }
        }

        /// <summary>
        /// Event fired when the graphics form closes
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event args</param>
        private void GraphicsFormClosed(object sender, EventArgs e)
        {
            if (!this.FormClosedByCode && this.GraphicsExit != null)
            {
                this.FormClosedByCode = false;
                this.GraphicsExit();
            }
        }

        public event GraphicsExitEventHandler GraphicsExit;

        #endregion

        #region Other methods
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
        private static bool GetPixelState(BitArray graphics, int x, int y)
        {
            return graphics[x + (C8Constants.ResolutionWidth * y)]; //C8Constants.ResolutionWidth is the resolution width of the screen
        }

        /// <summary>
        /// Determines if the graphics form is open
        /// </summary>
        /// <param name="form">The form to check</param>
        /// <returns>Is the form open?</returns>
        private bool IsFormOpen(GraphicsForm form)
        {
            return Application.OpenForms
                              .Cast<Form>()
                              .Any(x => x.GetType() == form.GetType());
        }

        /// <summary>
        /// Gets or sets a value indicating whether the form has been closed by code
        /// </summary>
        private bool FormClosedByCode { get; set; }

        #endregion
    }
}