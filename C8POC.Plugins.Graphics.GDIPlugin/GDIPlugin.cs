using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using C8POC.Interfaces;
using System.Collections;

namespace C8POC.Plugins.Graphics.GDIPlugin
{
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

        public void EnablePlugin()
        {
            _form = new GraphicsForm();
            _form.Show();
        }

        public void DisablePlugin()
        {
            _form.Close();
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

        #endregion

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
    }
}