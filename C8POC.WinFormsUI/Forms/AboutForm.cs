// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AboutForm.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the AboutForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.WinFormsUI
{
    using System.Diagnostics;
    using System.Windows.Forms;

    /// <summary>
    /// The about form.
    /// </summary>
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The link label github project_ link clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void linkLabelGithubProject_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var sInfo = new ProcessStartInfo("https://github.com/AlFranco/C8POC");
            Process.Start(sInfo);
        }
    }
}
