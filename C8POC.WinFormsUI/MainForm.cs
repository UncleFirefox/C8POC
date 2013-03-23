// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="">
//   
// </copyright>
// <summary>
//   The main form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;

namespace C8POC.WinFormsUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// The main form.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Constants and Fields

        /// <summary>
        /// The brush.
        /// </summary>
        private Brush brush = new SolidBrush(Color.White);

        /// <summary>
        /// The emulator.
        /// </summary>
        private C8Engine emulator;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
            this.emulator = new C8Engine();
            this.emulator.ScreenChanged += this.emulator_ScreenChanged;
            this.KeyDown += this.MainForm_KeyDown;
            this.KeyUp += this.MainForm_KeyUp;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The exit tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.emulator.StopEmulator();
            this.Close();
        }

        /// <summary>
        /// The main form_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.emulator.StopEmulator();
        }

        /// <summary>
        /// The main form_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            this.emulator.KeyDown(e.KeyValue);
        }

        /// <summary>
        /// The main form_ key up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            this.emulator.KeyUp(e.KeyValue);
        }

        /// <summary>
        /// The main form_ resize begin.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            this.emulator.StopEmulator();
        }

        /// <summary>
        /// The main form_ resize end.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            this.emulator.StartEmulator();
        }

        /// <summary>
        /// The open rom tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OpenRomToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.openFileDialogRom.ShowDialog() == DialogResult.OK)
            {
                this.emulator.LoadEmulator(this.openFileDialogRom.FileName);
                this.emulator.StartEmulator();
            }
        }

        /// <summary>
        /// The emulator_ screen changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void emulator_ScreenChanged(BitArray graphics)
        {
            var rectangles = new List<Rectangle>();

            // Se pinta la pantalla
            for (int y = 0; y < 32; y++)
            {
                for (var x = 0; x < 64; x++)
                {
                    if (GetPixelState(graphics,x,y))
                    {
                        rectangles.Add(new Rectangle(x * 10, y * 10, 10, 10));
                    }
                }
            }

            if (rectangles.Count > 0)
            {
                using (Graphics gfx = this.panelGraphics.CreateGraphics())
                {
                    gfx.Clear(Color.Black);
                    gfx.FillRectangles(this.brush, rectangles.ToArray());
                }
            }
        }

        /// <summary>
        /// Gets the state of a pixel, take into account that
        /// screen starts at upper left corner (0,0) and ends at lower right corner (63,31)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool GetPixelState(BitArray graphics, int x, int y)
        {
            return graphics[x + (64 * y)]; //64 is the resolution width of the screen
        }

        /// <summary>
        /// The menu strip main window_ menu activate.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void menuStripMainWindow_MenuActivate(object sender, EventArgs e)
        {
            this.emulator.StopEmulator();
        }

        /// <summary>
        /// The menu strip main window_ menu deactivate.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void menuStripMainWindow_MenuDeactivate(object sender, EventArgs e)
        {
            this.emulator.StartEmulator();
        }

        /// <summary>
        /// The panel graphics_ paint.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void panelGraphics_Paint(object sender, PaintEventArgs e)
        {
        }

        /// <summary>
        /// The plugin settings tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void pluginSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PluginSettings form = new PluginSettings();
            form.ShowDialog();
        }

        #endregion
    }
}