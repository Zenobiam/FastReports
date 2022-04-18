using FastReport.Controls;

namespace FastReport.Forms
{
    partial class Excel2007ExportForm
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
            //this.pcPages = new FastReport.Controls.PageControl();
            this.gbOptions = new System.Windows.Forms.GroupBox();
            this.lblFontScale = new System.Windows.Forms.Label();
            this.nudFontScale = new System.Windows.Forms.NumericUpDown();
            this.cbPrintOptimized = new ScalableCheckBox();
            this.cbSeamless = new ScalableCheckBox();
            this.cbDataOnly = new ScalableCheckBox();
            this.cbPageBreaks = new ScalableCheckBox();
            this.cbWysiwyg = new ScalableCheckBox();
            this.cbSplitPages = new ScalableCheckBox();
            this.lblPrintScaling = new System.Windows.Forms.Label();
            this.cbPrintScaling = new System.Windows.Forms.ComboBox();
            this.gbPageRange.SuspendLayout();
            this.pcPages.SuspendLayout();
            this.panPages.SuspendLayout();
            this.gbOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFontScale)).BeginInit();
            this.SuspendLayout();
            // 
            // gbPageRange
            // 
            this.gbPageRange.Location = new System.Drawing.Point(8, 4);
            // 
            // pcPages
            // 
            this.pcPages.Controls.Add(this.panPages);
            this.pcPages.HighlightPageIndex = -1;
            this.pcPages.Location = new System.Drawing.Point(0, 0);
            this.pcPages.Name = "pcPages";
            this.pcPages.SelectorWidth = 0;
            this.pcPages.Size = new System.Drawing.Size(276, 380);
            this.pcPages.TabIndex = 1;
            this.pcPages.Text = "pageControl1";
            // 
            // panPages
            // 
            this.panPages.Controls.Add(this.gbOptions);
            this.panPages.Dock = System.Windows.Forms.DockStyle.None;
            this.panPages.Size = new System.Drawing.Size(284, 382);
            this.panPages.Controls.SetChildIndex(this.gbPageRange, 0);
            this.panPages.Controls.SetChildIndex(this.gbOptions, 0);
            // 
            // cbOpenAfter
            // 
            this.cbOpenAfter.Location = new System.Drawing.Point(9, 376);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(113, 399);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(193, 399);
            this.btnCancel.TabIndex = 1;
            // 
            // gbOptions
            // 
            this.gbOptions.Controls.Add(this.cbSplitPages);
            this.gbOptions.Controls.Add(this.cbPrintScaling);
            this.gbOptions.Controls.Add(this.lblPrintScaling);
            this.gbOptions.Controls.Add(this.lblFontScale);
            this.gbOptions.Controls.Add(this.nudFontScale);
            this.gbOptions.Controls.Add(this.cbPrintOptimized);
            this.gbOptions.Controls.Add(this.cbSeamless);
            this.gbOptions.Controls.Add(this.cbDataOnly);
            this.gbOptions.Controls.Add(this.cbPageBreaks);
            this.gbOptions.Controls.Add(this.cbWysiwyg);
            this.gbOptions.Location = new System.Drawing.Point(8, 136);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(260, 236);
            this.gbOptions.TabIndex = 5;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Options";
            // 
            // lblFontScale
            // 
            this.lblFontScale.AutoSize = true;
            this.lblFontScale.Location = new System.Drawing.Point(12, 160);
            this.lblFontScale.Name = "lblFontScale";
            this.lblFontScale.Size = new System.Drawing.Size(56, 13);
            this.lblFontScale.TabIndex = 7;
            this.lblFontScale.Text = "Font scale";
            // 
            // nudFontScale
            // 
            this.nudFontScale.DecimalPlaces = 2;
            this.nudFontScale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudFontScale.Location = new System.Drawing.Point(128, 158);
            this.nudFontScale.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudFontScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudFontScale.Name = "nudFontScale";
            this.nudFontScale.Size = new System.Drawing.Size(61, 20);
            this.nudFontScale.TabIndex = 6;
            this.nudFontScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            // 
            // cbPrintOptimized
            // 
            this.cbPrintOptimized.AutoSize = true;
            this.cbPrintOptimized.Location = new System.Drawing.Point(12, 114);
            this.cbPrintOptimized.Name = "cbPrintOptimized";
            this.cbPrintOptimized.Size = new System.Drawing.Size(96, 17);
            this.cbPrintOptimized.TabIndex = 5;
            this.cbPrintOptimized.Text = "Print optimized";
            this.cbPrintOptimized.UseVisualStyleBackColor = true;
            // 
            // cbSeamless
            // 
            this.cbSeamless.AutoSize = true;
            this.cbSeamless.Location = new System.Drawing.Point(12, 91);
            this.cbSeamless.Name = "cbSeamless";
            this.cbSeamless.Size = new System.Drawing.Size(70, 17);
            this.cbSeamless.TabIndex = 4;
            this.cbSeamless.Text = "Seamless";
            this.cbSeamless.UseVisualStyleBackColor = true;
            // 
            // cbDataOnly
            // 
            this.cbDataOnly.AutoSize = true;
            this.cbDataOnly.Location = new System.Drawing.Point(12, 68);
            this.cbDataOnly.Name = "cbDataOnly";
            this.cbDataOnly.Size = new System.Drawing.Size(72, 17);
            this.cbDataOnly.TabIndex = 3;
            this.cbDataOnly.Text = "Data only";
            this.cbDataOnly.UseVisualStyleBackColor = true;
            // 
            // cbPageBreaks
            // 
            this.cbPageBreaks.AutoSize = true;
            this.cbPageBreaks.Checked = true;
            this.cbPageBreaks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPageBreaks.Location = new System.Drawing.Point(12, 44);
            this.cbPageBreaks.Name = "cbPageBreaks";
            this.cbPageBreaks.Size = new System.Drawing.Size(85, 17);
            this.cbPageBreaks.TabIndex = 2;
            this.cbPageBreaks.Text = "Page breaks";
            this.cbPageBreaks.UseVisualStyleBackColor = true;
            // 
            // cbWysiwyg
            // 
            this.cbWysiwyg.AutoSize = true;
            this.cbWysiwyg.Checked = true;
            this.cbWysiwyg.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbWysiwyg.Location = new System.Drawing.Point(12, 20);
            this.cbWysiwyg.Name = "cbWysiwyg";
            this.cbWysiwyg.Size = new System.Drawing.Size(69, 17);
            this.cbWysiwyg.TabIndex = 1;
            this.cbWysiwyg.Text = "Wysiwyg";
            this.cbWysiwyg.UseVisualStyleBackColor = true;
            // 
            // cbSplitPages
            // 
            this.cbSplitPages.AutoSize = true;
            this.cbSplitPages.Location = new System.Drawing.Point(12, 138);
            this.cbSplitPages.Name = "cbSplitPages";
            this.cbSplitPages.Size = new System.Drawing.Size(78, 17);
            this.cbSplitPages.TabIndex = 10;
            this.cbSplitPages.Text = "Split pages";
            this.cbSplitPages.UseVisualStyleBackColor = true;
            // 
            // lblPrintScaling
            // 
            this.lblPrintScaling.AutoSize = true;
            this.lblPrintScaling.Location = new System.Drawing.Point(12, 183);
            this.lblPrintScaling.Name = "lblPrintScaling";
            this.lblPrintScaling.Size = new System.Drawing.Size(65, 13);
            this.lblPrintScaling.TabIndex = 8;
            this.lblPrintScaling.Text = "Print Scaling";
            // 
            // cbPrintScaling
            // 
            this.cbPrintScaling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPrintScaling.FormattingEnabled = true;
            this.cbPrintScaling.Location = new System.Drawing.Point(12, 200);
            this.cbPrintScaling.Name = "cbPrintScaling";
            this.cbPrintScaling.Size = new System.Drawing.Size(236, 21);
            this.cbPrintScaling.TabIndex = 9;
            // 
            // Excel2007ExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(276, 431);
            this.Name = "Excel2007ExportForm";
            this.OpenAfterVisible = true;
            this.Text = "Export to Excel 2007";
            //this.Controls.SetChildIndex(this.pcPages, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.cbOpenAfter, 0);
            this.gbPageRange.ResumeLayout(false);
            this.gbPageRange.PerformLayout();
            this.pcPages.ResumeLayout(false);
            this.panPages.ResumeLayout(false);
            this.gbOptions.ResumeLayout(false);
            this.gbOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFontScale)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbOptions;
        private System.Windows.Forms.CheckBox cbWysiwyg;
        private System.Windows.Forms.CheckBox cbPageBreaks;
      private System.Windows.Forms.CheckBox cbDataOnly;
      private System.Windows.Forms.CheckBox cbSeamless;
        private System.Windows.Forms.CheckBox cbPrintOptimized;
        private System.Windows.Forms.NumericUpDown nudFontScale;
        private System.Windows.Forms.Label lblFontScale;
        //private Controls.PageControl pcPages;
        private System.Windows.Forms.Label lblPrintScaling;
        private System.Windows.Forms.ComboBox cbPrintScaling;
        private System.Windows.Forms.CheckBox cbSplitPages;
    }
}
