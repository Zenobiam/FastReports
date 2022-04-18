using FastReport.Controls;

namespace FastReport.Forms
{
    partial class PPMLExportForm
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
            this.chCurves = new ScalableCheckBox();
            this.gbOptions = new System.Windows.Forms.GroupBox();
            this.nudJpegQuality = new System.Windows.Forms.NumericUpDown();
            this.lblQuality = new System.Windows.Forms.Label();
            this.gbPageRange.SuspendLayout();
            this.pcPages.SuspendLayout();
            this.panPages.SuspendLayout();
            this.gbOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudJpegQuality)).BeginInit();
            this.SuspendLayout();
            // 
            // gbPageRange
            // 
            this.gbPageRange.Location = new System.Drawing.Point(8, 4);
            this.gbPageRange.Size = new System.Drawing.Size(260, 122);
            // 
            // pcPages
            // 
            this.pcPages.Location = new System.Drawing.Point(0, 0);
            this.pcPages.Size = new System.Drawing.Size(273, 129);
            // 
            // panPages
            // 
            this.panPages.Size = new System.Drawing.Size(273, 129);
            // 
            // cbOpenAfter
            // 
            this.cbOpenAfter.Location = new System.Drawing.Point(8, 217);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(113, 238);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(193, 238);
            // 
            // chCurves
            // 
            this.chCurves.AutoSize = true;
            this.chCurves.Location = new System.Drawing.Point(15, 20);
            this.chCurves.Name = "chCurves";
            this.chCurves.Size = new System.Drawing.Size(94, 17);
            this.chCurves.TabIndex = 8;
            this.chCurves.Text = "Text in curves";
            this.chCurves.UseVisualStyleBackColor = true;
            // 
            // gbOptions
            // 
            this.gbOptions.Controls.Add(this.nudJpegQuality);
            this.gbOptions.Controls.Add(this.lblQuality);
            this.gbOptions.Controls.Add(this.chCurves);
            this.gbOptions.Location = new System.Drawing.Point(8, 132);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(260, 79);
            this.gbOptions.TabIndex = 6;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Options";
            // 
            // nudJpegQuality
            // 
            this.nudJpegQuality.Location = new System.Drawing.Point(105, 42);
            this.nudJpegQuality.Name = "nudJpegQuality";
            this.nudJpegQuality.Size = new System.Drawing.Size(43, 20);
            this.nudJpegQuality.TabIndex = 11;
            // 
            // lblQuality
            // 
            this.lblQuality.AutoSize = true;
            this.lblQuality.Location = new System.Drawing.Point(15, 44);
            this.lblQuality.Name = "lblQuality";
            this.lblQuality.Size = new System.Drawing.Size(65, 13);
            this.lblQuality.TabIndex = 10;
            this.lblQuality.Text = "Jpeg quality";
            // 
            // PPMLExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(276, 270);
            this.Controls.Add(this.gbOptions);
            this.Name = "PPMLExportForm";
            this.OpenAfterVisible = true;
            this.Text = "Export to PPML";
            this.Controls.SetChildIndex(this.pcPages, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.gbOptions, 0);
            this.Controls.SetChildIndex(this.cbOpenAfter, 0);
            this.gbPageRange.ResumeLayout(false);
            this.gbPageRange.PerformLayout();
            this.pcPages.ResumeLayout(false);
            this.panPages.ResumeLayout(false);
            this.gbOptions.ResumeLayout(false);
            this.gbOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudJpegQuality)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chCurves;
        private System.Windows.Forms.GroupBox gbOptions;
        private System.Windows.Forms.Label lblQuality;
        private System.Windows.Forms.NumericUpDown nudJpegQuality;
    }
}
