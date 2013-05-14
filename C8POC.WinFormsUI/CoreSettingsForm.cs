// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoreSettingsForm.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   The core settings form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.WinFormsUI
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The core settings form.
    /// </summary>
    public partial class CoreSettingsForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreSettingsForm"/> class. 
        /// </summary>
        public CoreSettingsForm()
        {
            this.InitializeComponent();

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

            this.DialogResult = DialogResult.OK;

            this.Close();
        }
    }
}
