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
    public partial class CoreSettingsForm : Form
    {
        public CoreSettingsForm()
        {
            InitializeComponent();

            this.numericUpDownCyclesPerFrame.Value = C8POC.Properties.Settings.Default.CyclesPerFrame;
            this.numericUpDownFramesPerSecond.Value = C8POC.Properties.Settings.Default.FramesPerSecond;
        }

        /// <summary>
        /// Confirms the changes
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonOkClick(object sender, EventArgs e)
        {
            C8POC.Properties.Settings.Default.CyclesPerFrame = long.Parse(this.numericUpDownCyclesPerFrame.Text);
            C8POC.Properties.Settings.Default.FramesPerSecond = long.Parse(this.numericUpDownFramesPerSecond.Text);

            this.Close();
        }
    }
}
