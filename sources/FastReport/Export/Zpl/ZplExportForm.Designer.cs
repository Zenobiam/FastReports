using FastReport.Controls;

namespace FastReport.Forms
{
    public partial class ZplExportForm : BaseExportForm
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
            this.gbPrinterSettings = new System.Windows.Forms.GroupBox();
            this.lblFontScale = new System.Windows.Forms.Label();
            this.nudFontScale = new System.Windows.Forms.NumericUpDown();
            this.cbPrintAsBitmap = new FastReport.Controls.ScalableCheckBox();
            this.cbDensity = new System.Windows.Forms.ComboBox();
            this.lblDensity = new System.Windows.Forms.Label();
            this.gbPageRange.SuspendLayout();
            this.pcPages.SuspendLayout();
            this.panPages.SuspendLayout();
            this.gbPrinterSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFontScale)).BeginInit();
            this.SuspendLayout();
            // 
            // gbPageRange
            // 
            this.gbPageRange.Location = new System.Drawing.Point(8, 4);
            // 
            // pcPages
            // 
            this.pcPages.Location = new System.Drawing.Point(0, 0);
            this.pcPages.Size = new System.Drawing.Size(275, 135);
            // 
            // panPages
            // 
            this.panPages.Size = new System.Drawing.Size(275, 135);
            // 
            // cbOpenAfter
            // 
            this.cbOpenAfter.Location = new System.Drawing.Point(8, 257);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(113, 278);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(193, 278);
            // 
            // gbPrinterSettings
            // 
            this.gbPrinterSettings.Controls.Add(this.lblFontScale);
            this.gbPrinterSettings.Controls.Add(this.nudFontScale);
            this.gbPrinterSettings.Controls.Add(this.cbPrintAsBitmap);
            this.gbPrinterSettings.Controls.Add(this.cbDensity);
            this.gbPrinterSettings.Controls.Add(this.lblDensity);
            this.gbPrinterSettings.Location = new System.Drawing.Point(8, 138);
            this.gbPrinterSettings.Name = "gbPrinterSettings";
            this.gbPrinterSettings.Size = new System.Drawing.Size(260, 113);
            this.gbPrinterSettings.TabIndex = 6;
            this.gbPrinterSettings.TabStop = false;
            this.gbPrinterSettings.Text = "Printer settings";
            // 
            // lblFontScale
            // 
            this.lblFontScale.Location = new System.Drawing.Point(9, 82);
            this.lblFontScale.Name = "lblFontScale";
            this.lblFontScale.Size = new System.Drawing.Size(74, 18);
            this.lblFontScale.TabIndex = 4;
            this.lblFontScale.Text = "Font Scale";
            // 
            // nudFontScale
            // 
            this.nudFontScale.DecimalPlaces = 2;
            this.nudFontScale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudFontScale.Location = new System.Drawing.Point(126, 79);
            this.nudFontScale.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudFontScale.Name = "nudFontScale";
            this.nudFontScale.Size = new System.Drawing.Size(122, 20);
            this.nudFontScale.TabIndex = 3;
            this.nudFontScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cbPrintAsBitmap
            // 
            this.cbPrintAsBitmap.AutoSize = true;
            this.cbPrintAsBitmap.Checked = true;
            this.cbPrintAsBitmap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPrintAsBitmap.Location = new System.Drawing.Point(12, 56);
            this.cbPrintAsBitmap.Name = "cbPrintAsBitmap";
            this.cbPrintAsBitmap.Size = new System.Drawing.Size(97, 17);
            this.cbPrintAsBitmap.TabIndex = 2;
            this.cbPrintAsBitmap.Text = "Print as Bitmap";
            this.cbPrintAsBitmap.UseVisualStyleBackColor = true;
            // 
            // cbDensity
            // 
            this.cbDensity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDensity.FormattingEnabled = true;
            this.cbDensity.Items.AddRange(new object[] {
            "6 dpmm (152 dpi)",
            "8 dpmm (203 dpi)",
            "12 dpmm (300 dpi)",
            "24 dpmm (600 dpi)"});
            this.cbDensity.Location = new System.Drawing.Point(126, 25);
            this.cbDensity.Name = "cbDensity";
            this.cbDensity.Size = new System.Drawing.Size(122, 21);
            this.cbDensity.TabIndex = 1;
            // 
            // lblDensity
            // 
            this.lblDensity.Location = new System.Drawing.Point(9, 28);
            this.lblDensity.Name = "lblDensity";
            this.lblDensity.Size = new System.Drawing.Size(74, 18);
            this.lblDensity.TabIndex = 0;
            this.lblDensity.Text = "Density";
            // 
            // ZplExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(276, 311);
            this.Controls.Add(this.gbPrinterSettings);
            this.Name = "ZplExportForm";
            this.OpenAfterVisible = true;
            this.Controls.SetChildIndex(this.pcPages, 0);
            this.Controls.SetChildIndex(this.gbPrinterSettings, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.cbOpenAfter, 0);
            this.gbPageRange.ResumeLayout(false);
            this.gbPageRange.PerformLayout();
            this.pcPages.ResumeLayout(false);
            this.panPages.ResumeLayout(false);
            this.gbPrinterSettings.ResumeLayout(false);
            this.gbPrinterSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFontScale)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbPrinterSettings;
        private System.Windows.Forms.ComboBox cbDensity;
        private System.Windows.Forms.Label lblDensity;
        private System.Windows.Forms.Label lblFontScale;
        private System.Windows.Forms.NumericUpDown nudFontScale;
        private ScalableCheckBox cbPrintAsBitmap;
    }
}