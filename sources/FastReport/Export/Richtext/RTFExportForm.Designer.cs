using FastReport.Controls;

namespace FastReport.Forms
{
    partial class RTFExportForm
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
            this.lblPictures = new System.Windows.Forms.Label();
            this.cbPageBreaks = new ScalableCheckBox();
            this.cbWysiwyg = new ScalableCheckBox();
            this.cbbPictures = new System.Windows.Forms.ComboBox();
            this.cbbRTF = new System.Windows.Forms.ComboBox();
            this.lblRTF = new System.Windows.Forms.Label();
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
            this.pcPages.Controls.Add(this.cbbPictures);
            this.pcPages.Controls.Add(this.cbbRTF);
            this.pcPages.Controls.Add(this.lblRTF);
            this.pcPages.Location = new System.Drawing.Point(0, 0);
            this.pcPages.Size = new System.Drawing.Size(276, 269);
            this.pcPages.Controls.SetChildIndex(this.panPages, 0);
            this.pcPages.Controls.SetChildIndex(this.lblRTF, 0);
            this.pcPages.Controls.SetChildIndex(this.cbbRTF, 0);
            this.pcPages.Controls.SetChildIndex(this.cbbPictures, 0);
            // 
            // panPages
            // 
            this.panPages.Controls.Add(this.gbOptions);
            this.panPages.Dock = System.Windows.Forms.DockStyle.None;
            this.panPages.Size = new System.Drawing.Size(284, 272);
            this.panPages.Controls.SetChildIndex(this.gbOptions, 0);
            this.panPages.Controls.SetChildIndex(this.gbPageRange, 0);
            // 
            // cbOpenAfter
            // 
            this.cbOpenAfter.Location = new System.Drawing.Point(9, 273);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(109, 297);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(189, 297);
            this.btnCancel.TabIndex = 1;
            // 
            // gbOptions
            // 
            this.gbOptions.Controls.Add(this.lblPictures);
            this.gbOptions.Controls.Add(this.cbPageBreaks);
            this.gbOptions.Controls.Add(this.cbWysiwyg);
            this.gbOptions.Location = new System.Drawing.Point(8, 136);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(260, 129);
            this.gbOptions.TabIndex = 5;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Options";
            // 
            // lblPictures
            // 
            this.lblPictures.AutoSize = true;
            this.lblPictures.Location = new System.Drawing.Point(12, 70);
            this.lblPictures.Name = "lblPictures";
            this.lblPictures.Size = new System.Drawing.Size(45, 13);
            this.lblPictures.TabIndex = 3;
            this.lblPictures.Text = "Pictures";
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
            // cbbPictures
            // 
            this.cbbPictures.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbPictures.FormattingEnabled = true;
            this.cbbPictures.Items.AddRange(new object[] {
            "None",
            "Metafile",
            "Jpeg",
            "Png"});
            this.cbbPictures.Location = new System.Drawing.Point(134, 201);
            this.cbbPictures.Name = "cbbPictures";
            this.cbbPictures.Size = new System.Drawing.Size(121, 21);
            this.cbbPictures.TabIndex = 4;
            // 
            // cbbRTF
            // 
            this.cbbRTF.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbRTF.FormattingEnabled = true;
            this.cbbRTF.Items.AddRange(new object[] {
            "Picture",
            "Embedded RTF"});
            this.cbbRTF.Location = new System.Drawing.Point(134, 228);
            this.cbbRTF.Name = "cbbRTF";
            this.cbbRTF.Size = new System.Drawing.Size(121, 21);
            this.cbbRTF.TabIndex = 6;
            // 
            // lblRTF
            // 
            this.lblRTF.AutoSize = true;
            this.lblRTF.Location = new System.Drawing.Point(20, 233);
            this.lblRTF.Name = "lblRTF";
            this.lblRTF.Size = new System.Drawing.Size(73, 13);
            this.lblRTF.TabIndex = 5;
            this.lblRTF.Text = "RTF object as";
            // 
            // RTFExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(276, 329);
            this.Name = "RTFExportForm";
            this.OpenAfterVisible = true;
            this.Text = "Export to Rich Text";
            this.gbPageRange.ResumeLayout(false);
            this.gbPageRange.PerformLayout();
            this.pcPages.ResumeLayout(false);
            this.pcPages.PerformLayout();
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
        private System.Windows.Forms.ComboBox cbbPictures;
        private System.Windows.Forms.Label lblPictures;
        private System.Windows.Forms.ComboBox cbbRTF;
        private System.Windows.Forms.Label lblRTF;
    }
}
