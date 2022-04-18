using FastReport.Controls;

namespace FastReport.Forms
{
    partial class SVGExportForm
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
            this.cbToMultipleFiles = new ScalableCheckBox();
            this.lblImageFormat = new System.Windows.Forms.Label();
            this.cbEmbdImgs = new ScalableCheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.gbPageRange.SuspendLayout();
            this.pcPages.SuspendLayout();
            this.panPages.SuspendLayout();
            this.gbOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbPageRange
            // 
            this.gbPageRange.Location = new System.Drawing.Point(8, 4);
            // 
            // pcPages
            // 
            this.pcPages.Location = new System.Drawing.Point(0, 0);
            this.pcPages.Size = new System.Drawing.Size(276, 137);
            // 
            // panPages
            // 
            this.panPages.Dock = System.Windows.Forms.DockStyle.None;
            this.panPages.Size = new System.Drawing.Size(284, 137);
            // 
            // cbOpenAfter
            // 
            this.cbOpenAfter.Location = new System.Drawing.Point(9, 254);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(112, 277);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(193, 277);
            this.btnCancel.TabIndex = 1;
            // 
            // gbOptions
            // 
            this.gbOptions.Controls.Add(this.cbToMultipleFiles);
            this.gbOptions.Controls.Add(this.lblImageFormat);
            this.gbOptions.Controls.Add(this.cbEmbdImgs);
            this.gbOptions.Controls.Add(this.comboBox1);
            this.gbOptions.Location = new System.Drawing.Point(8, 138);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(260, 108);
            this.gbOptions.TabIndex = 5;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Options";
            // 
            // cbToMultipleFiles
            // 
            this.cbToMultipleFiles.AutoSize = true;
            this.cbToMultipleFiles.Location = new System.Drawing.Point(15, 78);
            this.cbToMultipleFiles.Name = "cbToMultipleFiles";
            this.cbToMultipleFiles.Size = new System.Drawing.Size(132, 17);
            this.cbToMultipleFiles.TabIndex = 7;
            this.cbToMultipleFiles.Text = "Export to multiple files";
            this.cbToMultipleFiles.UseVisualStyleBackColor = true;
            // 
            // lblImageFormat
            // 
            this.lblImageFormat.AutoSize = true;
            this.lblImageFormat.Location = new System.Drawing.Point(12, 24);
            this.lblImageFormat.Name = "lblImageFormat";
            this.lblImageFormat.Size = new System.Drawing.Size(76, 13);
            this.lblImageFormat.TabIndex = 1;
            this.lblImageFormat.Text = "Image format:";
            // 
            // cbEmbdImgs
            // 
            this.cbEmbdImgs.AutoSize = true;
            this.cbEmbdImgs.Location = new System.Drawing.Point(15, 53);
            this.cbEmbdImgs.Name = "cbEmbdImgs";
            this.cbEmbdImgs.Size = new System.Drawing.Size(94, 17);
            this.cbEmbdImgs.TabIndex = 6;
            this.cbEmbdImgs.Text = "Embed images";
            this.cbEmbdImgs.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Png",
            "Jpeg"});
            this.comboBox1.Location = new System.Drawing.Point(104, 20);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(144, 21);
            this.comboBox1.TabIndex = 0;
            // 
            // SVGExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(276, 307);
            this.Controls.Add(this.gbOptions);
            this.Name = "SVGExportForm";
            this.OpenAfterVisible = true;
            this.Text = "Export to SVG";
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbOptions;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label lblImageFormat;
        private System.Windows.Forms.CheckBox cbEmbdImgs;
        private System.Windows.Forms.CheckBox cbToMultipleFiles;
    }
}
