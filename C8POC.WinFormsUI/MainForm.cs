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

        public MainForm()
        {
            InitializeComponent();
            emulator = new C8Engine();
        }

        private void OpenRomToolStripMenuItemClick(object sender, EventArgs e)
        {
            if(openFileDialogRom.ShowDialog() == DialogResult.OK)
            {
                emulator.LoadEmulator(openFileDialogRom.FileName);
            }
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
