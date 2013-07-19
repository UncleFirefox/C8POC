// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WpfPlugin.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Graphics plugin with GDI primitives for Windows Forms
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.Plugins.Graphics.WPFPlugin
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using C8POC.Interfaces;

    /// <summary>
    /// Graphics plugin using WPF
    /// </summary>
    [Export(typeof(IGraphicsPlugin))]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("NameSpace", "C8POC.Plugins.Graphics.WPFPlugin.WpfPlugin")]
    [ExportMetadata("Description", "Graphics plugin based on WPF")]
    public class WpfPlugin : IGraphicsPlugin
    {
        #region Properties

        /// <summary>
        /// The brush.
        /// </summary>
        private readonly Brush brush = new SolidColorBrush(Colors.White);
         
        /// <summary>
        /// From that will be displayed when the plugin is activated
        /// </summary>
        private GraphicsForm graphicsForm;

        #endregion

        #region IGraphicsPlugin Members

        /// <summary>
        /// Prompts for a configuration
        /// </summary>
        public void Configure()
        {
        }

        /// <summary>
        /// Gets the about plugin window
        /// </summary>
        public void AboutPlugin()
        {
        }

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="currentConfiguration">
        /// The current configuration.
        /// </param>
        /// <returns>
        /// The <see cref="IDictionary"/>.
        /// </returns>
        public IDictionary<string, string> Configure(IDictionary<string, string> currentConfiguration)
        {
            return null;
        }

        /// <summary>
        /// Disables de plugin
        /// </summary>
        public void DisablePlugin()
        {
        }

        /// <summary>
        /// The enable plugin.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public void EnablePlugin(IDictionary<string, string> parameters)
        {
            this.graphicsForm = new GraphicsForm();
            this.graphicsForm.Closed += this.GraphicsFormClosed;
            this.graphicsForm.Show();
        }

        /// <summary>
        /// The get default plugin configuration.
        /// </summary>
        /// <returns>
        /// The <see cref="IDictionary"/>.
        /// </returns>
        public IDictionary<string, string> GetDefaultPluginConfiguration()
        {
            return null;
        }

        /// <summary>
        /// Drawing implementation for the plugin
        /// </summary>
        /// <param name="graphics">The graphics array</param>
        public void Draw(BitArray graphics)
        {
            this.graphicsForm.Dispatcher.Invoke(new Action(() => this.DrawInternal(graphics)));             
        }

        private void DrawInternal(BitArray graphics)
        {
            this.graphicsForm.canvas.Children.Clear();

            // Go through each pixel on the screen
            for (var y = 0; y < C8Constants.ResolutionHeight; y++)
            {
                for (var x = 0; x < C8Constants.ResolutionWidth; x++)
                {
                    if (!GetPixelState(graphics, x, y))
                    {
                        continue;
                    }

                    var rectangle = new Rectangle { Height = 10, Width = 10, Fill = this.brush };

                    Canvas.SetLeft(rectangle, x * 10);
                    Canvas.SetTop(rectangle, y * 10);

                    this.graphicsForm.canvas.Children.Add(rectangle);
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

        /// <summary>
        /// The graphics exit.
        /// </summary>
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
        /// Vertical positionB
        /// </param>
        /// <returns>
        /// If the pixel set or not
        /// </returns>
        private static bool GetPixelState(BitArray graphics, int x, int y)
        {
            return graphics[x + (C8Constants.ResolutionWidth * y)]; //C8Constants.ResolutionWidth is the resolution width of the screen
        }

        /// <summary>
        /// Gets or sets a value indicating whether the form has been closed by code
        /// </summary>
        private bool FormClosedByCode { get; set; }

        #endregion
    }
}
