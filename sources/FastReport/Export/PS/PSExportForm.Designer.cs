using FastReport.Controls;

namespace FastReport.Forms
{
    partial class PSExportForm
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
            this.gbOptions = new System.Windows.Forms.GroupBox();
            this.lblQuality = new System.Windows.Forms.Label();
            this.nudJpegQuality = new System.Windows.Forms.NumericUpDown();
            this.chImages = new ScalableCheckBox();
            this.chPages = new ScalableCheckBox();
            this.chCurves = new ScalableCheckBox();
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
            this.gbPageRange.Size = new System.Drawing.Size(260, 141);
            // 
            // pcPages
            // 
            this.pcPages.Location = new System.Drawing.Point(0, 0);
            // 
            // panPages
            // 
            this.panPages.Dock = System.Windows.Forms.DockStyle.None;
            this.panPages.Size = new System.Drawing.Size(276, 148);
            // 
            // cbOpenAfter
            // 
            this.cbOpenAfter.Location = new System.Drawing.Point(8, 282);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(113, 302);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(193, 302);
            this.btnCancel.TabIndex = 1;
            // 
            // gbOptions
            // 
            this.gbOptions.Controls.Add(this.lblQuality);
            this.gbOptions.Controls.Add(this.nudJpegQuality);
            this.gbOptions.Controls.Add(this.chImages);
            this.gbOptions.Controls.Add(this.chPages);
            this.gbOptions.Controls.Add(this.chCurves);
            this.gbOptions.Location = new System.Drawing.Point(8, 151);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(260, 125);
            this.gbOptions.TabIndex = 7;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Options";
            // 
            // lblQuality
            // 
            this.lblQuality.AutoSize = true;
            this.lblQuality.Location = new System.Drawing.Point(12, 92);
            this.lblQuality.Name = "lblQuality";
            this.lblQuality.Size = new System.Drawing.Size(72, 13);
            this.lblQuality.TabIndex = 11;
            this.lblQuality.Text = "Jpeg quality :";
            // 
            // nudJpegQuality
            // 
            this.nudJpegQuality.Location = new System.Drawing.Point(112, 90);
            this.nudJpegQuality.Name = "nudJpegQuality";
            this.nudJpegQuality.Size = new System.Drawing.Size(43, 20);
            this.nudJpegQuality.TabIndex = 10;
            this.nudJpegQuality.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // chImages
            // 
            this.chImages.AutoSize = true;
            this.chImages.Location = new System.Drawing.Point(15, 67);
            this.chImages.Name = "chImages";
            this.chImages.Size = new System.Drawing.Size(140, 17);
            this.chImages.TabIndex = 9;
            this.chImages.Text = "Save images separately";
            this.chImages.UseVisualStyleBackColor = true;
            // 
            // chPages
            // 
            this.chPages.AutoSize = true;
            this.chPages.Location = new System.Drawing.Point(15, 43);
            this.chPages.Name = "chPages";
            this.chPages.Size = new System.Drawing.Size(133, 17);
            this.chPages.TabIndex = 8;
            this.chPages.Text = "Pages in different files";
            this.chPages.UseVisualStyleBackColor = true;
            // 
            // chCurves
            // 
            this.chCurves.AutoSize = true;
            this.chCurves.Location = new System.Drawing.Point(15, 19);
            this.chCurves.Name = "chCurves";
            this.chCurves.Size = new System.Drawing.Size(94, 17);
            this.chCurves.TabIndex = 7;
            this.chCurves.Text = "Text in curves";
            this.chCurves.UseVisualStyleBackColor = true;
            // 
            // PSExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(276, 335);
            this.Controls.Add(this.gbOptions);
            this.Name = "PSExportForm";
            this.OpenAfterVisible = true;
            this.Text = "Export to PostScript";
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

        private System.Windows.Forms.GroupBox gbOptions;
        private System.Windows.Forms.CheckBox chCurves;
        private System.Windows.Forms.CheckBox chImages;
        private System.Windows.Forms.CheckBox chPages;
        private System.Windows.Forms.Label lblQuality;
        private System.Windows.Forms.NumericUpDown nudJpegQuality;
    }
}
