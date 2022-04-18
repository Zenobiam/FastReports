using System.Windows.Forms;

namespace FastReport.Forms
{
    partial class DxfExportForm
    {
        private ComboBox cbFillMode;
        private Label lblDxfFillMode;
        private NumericUpDown nuBarcodesGap;
        private Label label1;

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
            this.cbFillMode = new System.Windows.Forms.ComboBox();
            this.lblDxfFillMode = new System.Windows.Forms.Label();
            this.nuBarcodesGap = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.gbPageRange.SuspendLayout();
            this.pcPages.SuspendLayout();
            this.panPages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nuBarcodesGap)).BeginInit();
            this.SuspendLayout();
            //
            // panPages
            //
            //this.panPages.Location = new System.Drawing.Point(12, 12);
            this.Width = 340;
            // 
            // cbOpenAfter
            // 
            this.cbOpenAfter.Location = new System.Drawing.Point(12, 258);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(152, 286);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(232, 286);
            // 
            // cbFillMode
            // 
            this.cbFillMode.FormattingEnabled = true;
            this.cbFillMode.Items.AddRange(new object[] {
            "Border",
            "Solid"});
            this.cbFillMode.Location = new System.Drawing.Point(159, 177);
            this.cbFillMode.Name = "cbFillMode";
            this.cbFillMode.Size = new System.Drawing.Size(121, 21);
            this.cbFillMode.TabIndex = 6;
            this.cbFillMode.SelectedIndex = 0;
            this.cbFillMode.DropDownStyle = ComboBoxStyle.DropDownList;
            // 
            // lblDxfFillMode
            // 
            this.lblDxfFillMode.AutoSize = true;
            this.lblDxfFillMode.Location = new System.Drawing.Point(18, 180);
            this.lblDxfFillMode.Name = "lblDxfFillMode";
            this.lblDxfFillMode.Size = new System.Drawing.Size(52, 13);
            this.lblDxfFillMode.TabIndex = 7;
            this.lblDxfFillMode.Text = "Fill mode:";
            // 
            // nuBarcodesGap
            // 
            this.nuBarcodesGap.Increment = 0.01M;
            this.nuBarcodesGap.DecimalPlaces = 2;
            this.nuBarcodesGap.Location = new System.Drawing.Point(159, 216);
            this.nuBarcodesGap.Name = "nuBarcodesGap";
            this.nuBarcodesGap.Size = new System.Drawing.Size(120, 20);
            this.nuBarcodesGap.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 218);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Barcodes gap (mm):";
            // 
            // DxfExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(319, 319);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nuBarcodesGap);
            this.Controls.Add(this.lblDxfFillMode);
            this.Controls.Add(this.cbFillMode);
            this.Name = "DxfExportForm";
            this.OpenAfterVisible = true;
            this.Controls.SetChildIndex(this.pcPages, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.cbOpenAfter, 0);
            this.Controls.SetChildIndex(this.cbFillMode, 0);
            this.Controls.SetChildIndex(this.lblDxfFillMode, 0);
            this.Controls.SetChildIndex(this.nuBarcodesGap, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.gbPageRange.ResumeLayout(false);
            this.gbPageRange.PerformLayout();
            this.pcPages.ResumeLayout(false);
            this.panPages.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nuBarcodesGap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion // Windows Form Designer generated code
    }
}