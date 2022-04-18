using FastReport.Controls;

namespace FastReport.Forms
{
    partial class CsvExportForm
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
            this.cbEscapeQuotes = new ScalableCheckBox();
            this.cbNoQuotes = new ScalableCheckBox();
            this.cbDataOnly = new ScalableCheckBox();
            this.cbbCodepage = new System.Windows.Forms.ComboBox();
            this.lblCodepage = new System.Windows.Forms.Label();
            this.tbSeparator = new System.Windows.Forms.TextBox();
            this.lblSeparator = new System.Windows.Forms.Label();
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
            this.pcPages.Size = new System.Drawing.Size(276, 135);
            // 
            // panPages
            // 
            this.panPages.Dock = System.Windows.Forms.DockStyle.None;
            this.panPages.Size = new System.Drawing.Size(276, 134);
            // 
            // cbOpenAfter
            // 
            this.cbOpenAfter.Location = new System.Drawing.Point(9, 293);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(112, 316);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(192, 316);
            this.btnCancel.TabIndex = 1;
            // 
            // gbOptions
            // 
            this.gbOptions.Controls.Add(this.cbEscapeQuotes);
            this.gbOptions.Controls.Add(this.cbNoQuotes);
            this.gbOptions.Controls.Add(this.cbDataOnly);
            this.gbOptions.Controls.Add(this.cbbCodepage);
            this.gbOptions.Controls.Add(this.lblCodepage);
            this.gbOptions.Controls.Add(this.tbSeparator);
            this.gbOptions.Controls.Add(this.lblSeparator);
            this.gbOptions.Location = new System.Drawing.Point(9, 138);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(260, 149);
            this.gbOptions.TabIndex = 13;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Options";
            // 
            // cbEscapeQuotes
            // 
            this.cbEscapeQuotes.AutoSize = true;
            this.cbEscapeQuotes.Location = new System.Drawing.Point(12, 122);
            this.cbEscapeQuotes.Name = "cbEscapeQuotes";
            this.cbEscapeQuotes.Size = new System.Drawing.Size(139, 17);
            this.cbEscapeQuotes.TabIndex = 12;
            this.cbEscapeQuotes.Text = "Screen quotation marks";
            this.cbEscapeQuotes.UseVisualStyleBackColor = true;
            // 
            // cbNoQuotes
            // 
            this.cbNoQuotes.AutoSize = true;
            this.cbNoQuotes.Location = new System.Drawing.Point(12, 99);
            this.cbNoQuotes.Name = "cbNoQuotes";
            this.cbNoQuotes.Size = new System.Drawing.Size(140, 17);
            this.cbNoQuotes.TabIndex = 10;
            this.cbNoQuotes.Text = "Disable quotation marks";
            this.cbNoQuotes.UseVisualStyleBackColor = true;
            this.cbNoQuotes.CheckedChanged += new System.EventHandler(this.cbNoQuotes_CheckedChanged);
            // 
            // cbDataOnly
            // 
            this.cbDataOnly.AutoSize = true;
            this.cbDataOnly.Location = new System.Drawing.Point(12, 76);
            this.cbDataOnly.Name = "cbDataOnly";
            this.cbDataOnly.Size = new System.Drawing.Size(72, 17);
            this.cbDataOnly.TabIndex = 9;
            this.cbDataOnly.Text = "Data only";
            this.cbDataOnly.UseVisualStyleBackColor = true;
            // 
            // cbbCodepage
            // 
            this.cbbCodepage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbCodepage.FormattingEnabled = true;
            this.cbbCodepage.Items.AddRange(new object[] {
            "Default",
            "Unicode",
            "OEM"});
            this.cbbCodepage.Location = new System.Drawing.Point(92, 44);
            this.cbbCodepage.Name = "cbbCodepage";
            this.cbbCodepage.Size = new System.Drawing.Size(121, 21);
            this.cbbCodepage.TabIndex = 8;
            // 
            // lblCodepage
            // 
            this.lblCodepage.AutoSize = true;
            this.lblCodepage.Location = new System.Drawing.Point(12, 48);
            this.lblCodepage.Name = "lblCodepage";
            this.lblCodepage.Size = new System.Drawing.Size(56, 13);
            this.lblCodepage.TabIndex = 7;
            this.lblCodepage.Text = "Codepage";
            // 
            // tbSeparator
            // 
            this.tbSeparator.Location = new System.Drawing.Point(92, 16);
            this.tbSeparator.Name = "tbSeparator";
            this.tbSeparator.Size = new System.Drawing.Size(24, 20);
            this.tbSeparator.TabIndex = 6;
            this.tbSeparator.Text = ";";
            this.tbSeparator.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblSeparator
            // 
            this.lblSeparator.AutoSize = true;
            this.lblSeparator.Location = new System.Drawing.Point(12, 20);
            this.lblSeparator.Name = "lblSeparator";
            this.lblSeparator.Size = new System.Drawing.Size(55, 13);
            this.lblSeparator.TabIndex = 5;
            this.lblSeparator.Text = "Separator";
            // 
            // CsvExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(277, 347);
            this.Controls.Add(this.gbOptions);
            this.Name = "CsvExportForm";
            this.OpenAfterVisible = true;
            this.Text = "Export to CSV";
            this.Controls.SetChildIndex(this.pcPages, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.cbOpenAfter, 0);
            this.Controls.SetChildIndex(this.gbOptions, 0);
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
        private System.Windows.Forms.CheckBox cbEscapeQuotes;
        private System.Windows.Forms.CheckBox cbNoQuotes;
        private System.Windows.Forms.CheckBox cbDataOnly;
        private System.Windows.Forms.ComboBox cbbCodepage;
        private System.Windows.Forms.Label lblCodepage;
        private System.Windows.Forms.TextBox tbSeparator;
        private System.Windows.Forms.Label lblSeparator;
    }
}
