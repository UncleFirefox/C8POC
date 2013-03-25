using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using C8POC.Interfaces;
using System.Collections;
using System.Linq;

namespace C8POC.Plugins.Graphics.GDIPlugin
{
    using System.Windows.Forms;

    /// <summary>
    /// Graphics plugin with GDI primitives for Windows Forms
    /// </summary>
    [Export(typeof(IGraphicsPlugin))]
    public class GdiPlugin : IGraphicsPlugin
    {
        #region Properties

        /// <summary>
        /// From that will be displayed when the plugin is activated
        /// </summary>
        private GraphicsForm _form;
        
        /// <summary>
        /// The brush.
        /// </summary>
        private readonly Brush _brush = new SolidBrush(Color.White);
        
        #endregion
        
        #region IGraphicsPlugin Members

        public string PluginDescription
        {
            get { return "Graphics plugin with GDI primitives for Windows Forms"; }
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
        /// Enables the plugin
        /// </summary>
        public void EnablePlugin()
        {
            _form = new GraphicsForm();
            _form.Closed += GraphicsFormClosed;
            _form.Show();
        }

        void GraphicsFormClosed(object sender, EventArgs e)
        {
            if (!this.formClosedByCode && GraphicsExit != null)
            {
                formClosedByCode = false;
                GraphicsExit();
            }
        }

        /// <summary>
        /// Disables de plugin
        /// </summary>
        public void DisablePlugin()
        {
            if (this._form != null && this.IsFormOpen(this._form))
            {
                this.formClosedByCode = true;
                this._form.Close();
            }
        }

        public void Draw(BitArray graphics)
        {
            var rectangles = new List<Rectangle>();

            //Go through each pixel on the screen
            for (int y = 0; y < 32; y++)
            {
                for (var x = 0; x < 64; x++)
                {
                    if (GetPixelState(graphics,x, y))
                    {
                        rectangles.Add(new Rectangle(x * 10, y * 10, 10, 10));
                    }
                }
            }

            if (rectangles.Count > 0)
            {
                using (var gfx = this._form.renderingPanel.CreateGraphics())
                {
                    gfx.Clear(Color.Black);
                    gfx.FillRectangles(this._brush, rectangles.ToArray());
                }
            }
        }

        public event GraphicsExitEventHandler GraphicsExit;

        #endregion

        #region Other methods
        /// <summary>
        /// Gets the state of a pixel, take into account that
        /// screen starts at upper left corner (0,0) and ends at lower right corner (63,31)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static bool GetPixelState(BitArray graphics, int x, int y)
        {
            return graphics[x + (64 * y)]; //64 is the resolution width of the screen
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

        private bool formClosedByCode { get; set; }

        #endregion
    }
}