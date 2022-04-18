namespace FastReport.Forms
{
    partial class HpglExportForm
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
            this.lblFillMode = new System.Windows.Forms.Label();
            this.cbFillMode = new System.Windows.Forms.ComboBox();
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
            this.pcPages.Size = new System.Drawing.Size(276, 272);
            // 
            // panPages
            // 
            this.panPages.Controls.Add(this.gbOptions);
            this.panPages.Dock = System.Windows.Forms.DockStyle.None;
            this.panPages.Size = new System.Drawing.Size(276, 268);
            this.panPages.Controls.SetChildIndex(this.gbPageRange, 0);
            this.panPages.Controls.SetChildIndex(this.gbOptions, 0);
            // 
            // cbOpenAfter
            // 
            this.cbOpenAfter.Location = new System.Drawing.Point(12, 273);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(108, 296);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(189, 296);
            this.btnCancel.TabIndex = 1;
            // 
            // gbOptions
            // 
            this.gbOptions.Controls.Add(this.cbFillMode);
            this.gbOptions.Controls.Add(this.lblFillMode);
            this.gbOptions.Location = new System.Drawing.Point(8, 136);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(260, 128);
            this.gbOptions.TabIndex = 5;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Options";
            // 
            // lblFillMode
            // 
            this.lblFillMode.AutoSize = true;
            this.lblFillMode.Location = new System.Drawing.Point(12, 24);
            this.lblFillMode.Name = "lblFillMode";
            this.lblFillMode.Size = new System.Drawing.Size(48, 13);
            this.lblFillMode.TabIndex = 7;
            this.lblFillMode.Text = "Fill Mode";
            // 
            // cbFillMode
            // 
            this.cbFillMode.FormattingEnabled = true;
            this.cbFillMode.Items.AddRange(new object[] {
            "Solid",
            "Border"});
            this.cbFillMode.Location = new System.Drawing.Point(104, 21);
            this.cbFillMode.Name = "cbFillMode";
            this.cbFillMode.Size = new System.Drawing.Size(144, 21);
            this.cbFillMode.TabIndex = 8;
            // 
            // HpglExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.ClientSize = new System.Drawing.Size(276, 331);
            this.Name = "HpglExportForm";
            this.OpenAfterVisible = true;
            this.Text = "Export to dBase";
            this.gbPageRange.ResumeLayout(false);
            this.gbPageRange.PerformLayout();
            this.pcPages.ResumeLayout(false);
            this.panPages.ResumeLayout(false);
            this.gbOptions.ResumeLayout(false);
            this.gbOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion // Windows Form Designer generated code

        private System.Windows.Forms.GroupBox gbOptions;
        private System.Windows.Forms.Label lblFillMode;
        private System.Windows.Forms.ComboBox cbFillMode;
    }
}