using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace C8POC.WinFormsUI
{
    public partial class MainForm : Form
    {
        private C8Engine emulator;
        private Brush brush = new SolidBrush(Color.White);

        public MainForm()
        {
            InitializeComponent();
            emulator = new C8Engine();
            emulator.ScreenChanged += emulator_ScreenChanged;
            this.KeyDown += MainForm_KeyDown;
            this.KeyUp += MainForm_KeyUp;
        }

        void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            emulator.KeyUp(e.KeyValue);
        }

        void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            emulator.KeyDown(e.KeyValue);
        }

        void emulator_ScreenChanged(object sender, EventArgs e)
        {
            var rectangles = new List<Rectangle>();

            // Se pinta la pantalla
            for (int y = 0; y < 32; y++)
            {
                for (var x = 0; x < 64; x++)
                {
                    if(((C8Engine)sender).GetPixelState(x,y))
                    {
                        rectangles.Add(new Rectangle(x * 10, y * 10, 10, 10));
                    }
                }
            }

            if (rectangles.Count > 0)
            {
                using (Graphics gfx = panelGraphics.CreateGraphics())
                {
                    gfx.Clear(Color.Black);
                    gfx.FillRectangles(brush, rectangles.ToArray());
                }
            }
        }

        private void OpenRomToolStripMenuItemClick(object sender, EventArgs e)
        {
            if(openFileDialogRom.ShowDialog() == DialogResult.OK)
            {
                emulator.LoadEmulator(openFileDialogRom.FileName);
                emulator.StartEmulator();
            }
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            emulator.StopEmulator();
            Close();
        }

        private void menuStripMainWindow_MenuActivate(object sender, EventArgs e)
        {
            emulator.StopEmulator();
        }

        private void menuStripMainWindow_MenuDeactivate(object sender, EventArgs e)
        {
            emulator.StartEmulator();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            emulator.StopEmulator();
        }

        private void panelGraphics_Paint(object sender, PaintEventArgs e)
        {
        }

        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            emulator.StopEmulator();
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            emulator.StartEmulator();
        }

        private void pluginSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PluginSettings form = new PluginSettings();
            form.ShowDialog();
        }
    }
}
