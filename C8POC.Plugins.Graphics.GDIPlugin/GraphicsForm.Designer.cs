namespace C8POC.Plugins.Graphics.GDIPlugin
{
    partial class GraphicsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.renderingPanel = new C8POC.WinFormsUI.DoubleBufferedPanel();
            this.SuspendLayout();
            // 
            // renderingPanel
            // 
            this.renderingPanel.BackColor = System.Drawing.Color.Black;
            this.renderingPanel.Location = new System.Drawing.Point(0, 0);
            this.renderingPanel.Name = "renderingPanel";
            this.renderingPanel.Size = new System.Drawing.Size(640, 320);
            this.renderingPanel.TabIndex = 0;
            // 
            // GraphicsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(640, 320);
            this.Controls.Add(this.renderingPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "GraphicsForm";
            this.Text = "GDI Plugin";
            this.ResumeLayout(false);

        }

        #endregion

        public WinFormsUI.DoubleBufferedPanel renderingPanel;
    }
}