namespace C8POC.WinFormsUI
{
    partial class CoreSettingsForm
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
            this.labelFPS = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownFramesPerSecond = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownCyclesPerFrame = new System.Windows.Forms.NumericUpDown();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.checkBoxDisassembler = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFramesPerSecond)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCyclesPerFrame)).BeginInit();
            this.SuspendLayout();
            // 
            // labelFPS
            // 
            this.labelFPS.AutoSize = true;
            this.labelFPS.Location = new System.Drawing.Point(12, 9);
            this.labelFPS.Name = "labelFPS";
            this.labelFPS.Size = new System.Drawing.Size(97, 13);
            this.labelFPS.TabIndex = 2;
            this.labelFPS.Text = "Frames per second";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Cycles per frame";
            // 
            // numericUpDownFramesPerSecond
            // 
            this.numericUpDownFramesPerSecond.Location = new System.Drawing.Point(15, 25);
            this.numericUpDownFramesPerSecond.Name = "numericUpDownFramesPerSecond";
            this.numericUpDownFramesPerSecond.Size = new System.Drawing.Size(257, 20);
            this.numericUpDownFramesPerSecond.TabIndex = 4;
            // 
            // numericUpDownCyclesPerFrame
            // 
            this.numericUpDownCyclesPerFrame.Location = new System.Drawing.Point(15, 72);
            this.numericUpDownCyclesPerFrame.Name = "numericUpDownCyclesPerFrame";
            this.numericUpDownCyclesPerFrame.Size = new System.Drawing.Size(257, 20);
            this.numericUpDownCyclesPerFrame.TabIndex = 5;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(196, 130);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(115, 130);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 7;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.ButtonOkClick);
            // 
            // checkBoxDisassembler
            // 
            this.checkBoxDisassembler.AutoSize = true;
            this.checkBoxDisassembler.Location = new System.Drawing.Point(15, 98);
            this.checkBoxDisassembler.Name = "checkBoxDisassembler";
            this.checkBoxDisassembler.Size = new System.Drawing.Size(122, 17);
            this.checkBoxDisassembler.TabIndex = 8;
            this.checkBoxDisassembler.Text = "Enable disassembler";
            this.checkBoxDisassembler.UseVisualStyleBackColor = true;
            // 
            // CoreSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(284, 165);
            this.Controls.Add(this.checkBoxDisassembler);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.numericUpDownCyclesPerFrame);
            this.Controls.Add(this.numericUpDownFramesPerSecond);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelFPS);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "CoreSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Core Settings";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFramesPerSecond)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCyclesPerFrame)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelFPS;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDownFramesPerSecond;
        private System.Windows.Forms.NumericUpDown numericUpDownCyclesPerFrame;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.CheckBox checkBoxDisassembler;
    }
}