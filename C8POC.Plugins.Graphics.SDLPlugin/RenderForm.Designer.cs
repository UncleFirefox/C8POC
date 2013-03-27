namespace C8POC.Plugins.Graphics.SDLPlugin
{
    partial class RenderForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RenderForm));
            this.surfaceControlC8 = new C8POC.Plugins.Graphics.SDLPlugin.C8SurfaceControl();
            ((System.ComponentModel.ISupportInitialize)(this.surfaceControlC8)).BeginInit();
            this.SuspendLayout();
            // 
            // surfaceControlC8
            // 
            this.surfaceControlC8.AccessibleDescription = "SdlDotNet SurfaceControl";
            this.surfaceControlC8.AccessibleName = "SurfaceControl";
            this.surfaceControlC8.AccessibleRole = System.Windows.Forms.AccessibleRole.Graphic;
            this.surfaceControlC8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.surfaceControlC8.BackColor = System.Drawing.Color.Black;
            this.surfaceControlC8.Image = ((System.Drawing.Image)(resources.GetObject("surfaceControlC8.Image")));
            this.surfaceControlC8.InitialImage = null;
            this.surfaceControlC8.Location = new System.Drawing.Point(-1, 0);
            this.surfaceControlC8.Name = "surfaceControlC8";
            this.surfaceControlC8.Size = new System.Drawing.Size(640, 320);
            this.surfaceControlC8.TabIndex = 0;
            this.surfaceControlC8.TabStop = false;
            // 
            // RenderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(639, 321);
            this.Controls.Add(this.surfaceControlC8);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "RenderForm";
            this.Text = "RenderForm";
            ((System.ComponentModel.ISupportInitialize)(this.surfaceControlC8)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public C8SurfaceControl surfaceControlC8;

    }
}