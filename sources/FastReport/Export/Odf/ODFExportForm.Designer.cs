using FastReport.Controls;

namespace FastReport.Forms
{
    partial class ODFExportForm
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
            this.lblCompliance = new System.Windows.Forms.Label();
            this.cbOdfStandard = new System.Windows.Forms.ComboBox();
            this.cbPageBreaks = new System.Windows.Forms.CheckBox();
            this.cbWysiwyg = new System.Windows.Forms.CheckBox();
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
            this.pcPages.Location = new System.Drawing.Point(0, 1);
            this.pcPages.Size = new System.Drawing.Size(284, 234);
            // 
            // panPages
            // 
            this.panPages.Controls.Add(this.gbOptions);
            this.panPages.Size = new System.Drawing.Size(284, 234);
            this.panPages.Controls.SetChildIndex(this.gbPageRange, 0);
            this.panPages.Controls.SetChildIndex(this.gbOptions, 0);
            // 
            // cbOpenAfter
            // 
            this.cbOpenAfter.AutoSize = true;
            this.cbOpenAfter.Location = new System.Drawing.Point(8, 241);
            this.cbOpenAfter.Size = new System.Drawing.Size(114, 17);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(108, 262);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(193, 261);
            this.btnCancel.TabIndex = 1;
            // 
            // gbOptions
            // 
            this.gbOptions.Controls.Add(this.lblCompliance);
            this.gbOptions.Controls.Add(this.cbOdfStandard);
            this.gbOptions.Controls.Add(this.cbPageBreaks);
            this.gbOptions.Controls.Add(this.cbWysiwyg);
            this.gbOptions.Location = new System.Drawing.Point(8, 136);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(260, 96);
            this.gbOptions.TabIndex = 5;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Options";
            // 
            // lblCompliance
            // 
            this.lblCompliance.AutoSize = true;
            this.lblCompliance.Location = new System.Drawing.Point(9, 22);
            this.lblCompliance.Name = "lblCompliance";
            this.lblCompliance.Size = new System.Drawing.Size(95, 13);
            this.lblCompliance.TabIndex = 0;
            this.lblCompliance.Text = "ODF Compliance:  ";
            // 
            // cbOdfStandard
            // 
            this.cbOdfStandard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOdfStandard.FormattingEnabled = true;
            this.cbOdfStandard.Items.AddRange(new object[] {
            "ODF 1.0/1.1",
            "ODF 1.2",
            "XODF 1.0/1.1",
            "XODF 1.2"});
            this.cbOdfStandard.Location = new System.Drawing.Point(104, 19);
            this.cbOdfStandard.Name = "cbOdfStandard";
            this.cbOdfStandard.Size = new System.Drawing.Size(85, 21);
            this.cbOdfStandard.TabIndex = 1;
            // 
            // cbPageBreaks
            // 
            this.cbPageBreaks.AutoSize = true;
            this.cbPageBreaks.Checked = true;
            this.cbPageBreaks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPageBreaks.Location = new System.Drawing.Point(12, 71);
            this.cbPageBreaks.Name = "cbPageBreaks";
            this.cbPageBreaks.Size = new System.Drawing.Size(85, 17);
            this.cbPageBreaks.TabIndex = 3;
            this.cbPageBreaks.Text = "Page breaks";
            this.cbPageBreaks.UseVisualStyleBackColor = true;
            // 
            // cbWysiwyg
            // 
            this.cbWysiwyg.AutoSize = true;
            this.cbWysiwyg.Checked = true;
            this.cbWysiwyg.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbWysiwyg.Location = new System.Drawing.Point(12, 47);
            this.cbWysiwyg.Name = "cbWysiwyg";
            this.cbWysiwyg.Size = new System.Drawing.Size(69, 17);
            this.cbWysiwyg.TabIndex = 2;
            this.cbWysiwyg.Text = "Wysiwyg";
            this.cbWysiwyg.UseVisualStyleBackColor = true;
            // 
            // ODFExportForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(273, 291);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "ODFExportForm";
            this.OpenAfterVisible = true;
            this.Text = "Export to Open Office";
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
        private System.Windows.Forms.CheckBox cbWysiwyg;
        private System.Windows.Forms.CheckBox cbPageBreaks;
        private System.Windows.Forms.ComboBox cbOdfStandard;
        private System.Windows.Forms.Label lblCompliance;

    }
}
