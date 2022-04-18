using FastReport.Controls;

namespace FastReport.Forms
{
    partial class Word2007ExportForm
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
            this.cbPrintOptimized = new ScalableCheckBox();
            this.radioButtonParagraph = new ScalableRadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.cbRh = new System.Windows.Forms.ComboBox();
            this.cbWysiwyg = new ScalableCheckBox();
            this.radioButtonLayers = new ScalableRadioButton();
            this.radioButtonTable = new ScalableRadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.cbDoNotExpandShiftReturn = new ScalableCheckBox();
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
            this.pcPages.Size = new System.Drawing.Size(276, 350);
            // 
            // panPages
            // 
            this.panPages.Controls.Add(this.gbOptions);
            this.panPages.Size = new System.Drawing.Size(276, 350);
            this.panPages.Controls.SetChildIndex(this.gbPageRange, 0);
            this.panPages.Controls.SetChildIndex(this.gbOptions, 0);
            // 
            // cbOpenAfter
            // 
            this.cbOpenAfter.Location = new System.Drawing.Point(8, 356);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(108, 379);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(189, 379);
            this.btnCancel.TabIndex = 1;
            // 
            // gbOptions
            // 
            this.gbOptions.Controls.Add(this.cbDoNotExpandShiftReturn);
            this.gbOptions.Controls.Add(this.button1);
            this.gbOptions.Controls.Add(this.cbPrintOptimized);
            this.gbOptions.Controls.Add(this.radioButtonParagraph);
            this.gbOptions.Controls.Add(this.label1);
            this.gbOptions.Controls.Add(this.cbRh);
            this.gbOptions.Controls.Add(this.cbWysiwyg);
            this.gbOptions.Controls.Add(this.radioButtonLayers);
            this.gbOptions.Controls.Add(this.radioButtonTable);
            this.gbOptions.Location = new System.Drawing.Point(8, 136);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(260, 201);
            this.gbOptions.TabIndex = 5;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Options";
            // 
            // cbPrintOptimized
            // 
            this.cbPrintOptimized.Location = new System.Drawing.Point(12, 148);
            this.cbPrintOptimized.Name = "cbPrintOptimized";
            this.cbPrintOptimized.Size = new System.Drawing.Size(236, 17);
            this.cbPrintOptimized.TabIndex = 6;
            this.cbPrintOptimized.Text = "Print optimized";
            this.cbPrintOptimized.UseVisualStyleBackColor = true;
            // 
            // radioButtonParagraph
            // 
            this.radioButtonParagraph.AutoSize = true;
            this.radioButtonParagraph.Location = new System.Drawing.Point(12, 65);
            this.radioButtonParagraph.Name = "radioButtonParagraph";
            this.radioButtonParagraph.Size = new System.Drawing.Size(142, 17);
            this.radioButtonParagraph.TabIndex = 5;
            this.radioButtonParagraph.Text = "Paragraph based export";
            this.radioButtonParagraph.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 26);
            this.label1.TabIndex = 4;
            this.label1.Text = "Row height is";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbRh
            // 
            this.cbRh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRh.FormattingEnabled = true;
            this.cbRh.Items.AddRange(new object[] {
            "Exactly",
            "Minimum"});
            this.cbRh.Location = new System.Drawing.Point(127, 93);
            this.cbRh.Name = "cbRh";
            this.cbRh.Size = new System.Drawing.Size(121, 21);
            this.cbRh.TabIndex = 3;
            // 
            // cbWysiwyg
            // 
            this.cbWysiwyg.Location = new System.Drawing.Point(12, 125);
            this.cbWysiwyg.Name = "cbWysiwyg";
            this.cbWysiwyg.Size = new System.Drawing.Size(236, 17);
            this.cbWysiwyg.TabIndex = 2;
            this.cbWysiwyg.Text = "Wysiwyg";
            this.cbWysiwyg.UseVisualStyleBackColor = true;
            this.cbWysiwyg.CheckedChanged += new System.EventHandler(this.cbWysiwyg_CheckedChanged);
            // 
            // radioButtonLayers
            // 
            this.radioButtonLayers.AutoSize = true;
            this.radioButtonLayers.Location = new System.Drawing.Point(12, 42);
            this.radioButtonLayers.Name = "radioButtonLayers";
            this.radioButtonLayers.Size = new System.Drawing.Size(119, 17);
            this.radioButtonLayers.TabIndex = 1;
            this.radioButtonLayers.Text = "Layer based export";
            this.radioButtonLayers.UseVisualStyleBackColor = true;
            // 
            // radioButtonTable
            // 
            this.radioButtonTable.AutoSize = true;
            this.radioButtonTable.Checked = true;
            this.radioButtonTable.Location = new System.Drawing.Point(12, 19);
            this.radioButtonTable.Name = "radioButtonTable";
            this.radioButtonTable.Size = new System.Drawing.Size(118, 17);
            this.radioButtonTable.TabIndex = 0;
            this.radioButtonTable.TabStop = true;
            this.radioButtonTable.Text = "Table based export";
            this.radioButtonTable.UseVisualStyleBackColor = true;
            this.radioButtonTable.CheckedChanged += new System.EventHandler(this.radioButtonTable_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(181, 156);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(67, 39);
            this.button1.TabIndex = 7;
            this.button1.Text = "for extra options";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // cbDoNotExpandShiftReturn
            // 
            this.cbDoNotExpandShiftReturn.AutoSize = true;
            this.cbDoNotExpandShiftReturn.Location = new System.Drawing.Point(12, 171);
            this.cbDoNotExpandShiftReturn.Name = "cbDoNotExpandShiftReturn";
            this.cbDoNotExpandShiftReturn.Size = new System.Drawing.Size(154, 17);
            this.cbDoNotExpandShiftReturn.TabIndex = 8;
            this.cbDoNotExpandShiftReturn.Text = "Do not expand shift return";
            this.cbDoNotExpandShiftReturn.UseVisualStyleBackColor = true;
            // 
            // Word2007ExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(276, 419);
            this.Name = "Word2007ExportForm";
            this.OpenAfterVisible = true;
            this.Text = "Export to MS Word 2007";
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
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
        private System.Windows.Forms.RadioButton radioButtonLayers;
        private System.Windows.Forms.RadioButton radioButtonTable;
        private System.Windows.Forms.CheckBox cbWysiwyg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbRh;
        private System.Windows.Forms.RadioButton radioButtonParagraph;
        private System.Windows.Forms.CheckBox cbPrintOptimized;
        private System.Windows.Forms.CheckBox cbDoNotExpandShiftReturn;
        private System.Windows.Forms.Button button1;
    }
}
