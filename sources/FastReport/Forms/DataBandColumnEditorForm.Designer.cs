using System;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Controls;

namespace FastReport.Forms
{
    partial class DataBandColumnEditorForm
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
            this.gbColumns = new System.Windows.Forms.GroupBox();
            this.tbColumnCount = new System.Windows.Forms.TextBox();
            this.tbWidth = new System.Windows.Forms.TextBox();
            this.tbColumnMinRowCount = new System.Windows.Forms.TextBox();
            this.cbxColumnLayout = new System.Windows.Forms.ComboBox();
            this.lblWidth = new System.Windows.Forms.Label();
            this.lblMinRowCount = new System.Windows.Forms.Label();
            this.lblLayout = new System.Windows.Forms.Label();
            this.lblColumnCount = new System.Windows.Forms.Label();
            this.gbColumns.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(140, 146);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(221, 146);
            // 
            // gbColumns
            // 
            this.gbColumns.Controls.Add(this.tbColumnCount);
            this.gbColumns.Controls.Add(this.tbWidth);
            this.gbColumns.Controls.Add(this.tbColumnMinRowCount);
            this.gbColumns.Controls.Add(this.cbxColumnLayout);
            this.gbColumns.Controls.Add(this.lblWidth);
            this.gbColumns.Controls.Add(this.lblMinRowCount);
            this.gbColumns.Controls.Add(this.lblLayout);
            this.gbColumns.Controls.Add(this.lblColumnCount);
            this.gbColumns.Location = new System.Drawing.Point(8, 4);
            this.gbColumns.Name = "gbColumns";
            this.gbColumns.Size = new System.Drawing.Size(288, 136);
            this.gbColumns.TabIndex = 0;
            this.gbColumns.TabStop = false;
            this.gbColumns.Text = "Columns";
            // 
            // tbColumnCount
            // 
            this.tbColumnCount.Location = new System.Drawing.Point(120, 19);
            this.tbColumnCount.MinimumSize = new System.Drawing.Size(100, 21);
            this.tbColumnCount.Name = "tbColumnCount";
            this.tbColumnCount.Size = new System.Drawing.Size(162, 20);
            this.tbColumnCount.TabIndex = 9;
            this.tbColumnCount.TextChanged += new System.EventHandler(this.tbColumnCount_TextChanged);
            // 
            // tbWidth
            // 
            this.tbWidth.Location = new System.Drawing.Point(120, 100);
            this.tbWidth.MinimumSize = new System.Drawing.Size(100, 21);
            this.tbWidth.Name = "tbWidth";
            this.tbWidth.Size = new System.Drawing.Size(162, 20);
            this.tbWidth.TabIndex = 8;
            this.tbWidth.TextChanged += new System.EventHandler(this.tbWidth_TextChanged);
            // 
            // tbColumnMinRowCount
            // 
            this.tbColumnMinRowCount.Location = new System.Drawing.Point(120, 73);
            this.tbColumnMinRowCount.MinimumSize = new System.Drawing.Size(100, 21);
            this.tbColumnMinRowCount.Name = "tbColumnMinRowCount";
            this.tbColumnMinRowCount.Size = new System.Drawing.Size(162, 20);
            this.tbColumnMinRowCount.TabIndex = 7;
            this.tbColumnMinRowCount.TextChanged += new System.EventHandler(this.tbColumnMinRowCount_TextChanged);
            // 
            // cbxColumnLayout
            // 
            this.cbxColumnLayout.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxColumnLayout.FormattingEnabled = true;
            this.cbxColumnLayout.Location = new System.Drawing.Point(120, 46);
            this.cbxColumnLayout.Name = "cbxColumnLayout";
            this.cbxColumnLayout.Size = new System.Drawing.Size(162, 21);
            this.cbxColumnLayout.TabIndex = 6;
            this.cbxColumnLayout.SelectedIndexChanged += new System.EventHandler(this.cbxColumnLayout_SelectedIndexChanged);
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.Location = new System.Drawing.Point(12, 103);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(35, 13);
            this.lblWidth.TabIndex = 4;
            this.lblWidth.Text = "Width";
            // 
            // lblMinRowCount
            // 
            this.lblMinRowCount.AutoSize = true;
            this.lblMinRowCount.Location = new System.Drawing.Point(12, 76);
            this.lblMinRowCount.Name = "lblMinRowCount";
            this.lblMinRowCount.Size = new System.Drawing.Size(73, 13);
            this.lblMinRowCount.TabIndex = 3;
            this.lblMinRowCount.Text = "MinRowCount";
            // 
            // lblLayout
            // 
            this.lblLayout.AutoSize = true;
            this.lblLayout.Location = new System.Drawing.Point(12, 49);
            this.lblLayout.Name = "lblLayout";
            this.lblLayout.Size = new System.Drawing.Size(40, 13);
            this.lblLayout.TabIndex = 2;
            this.lblLayout.Text = "Layout";
            // 
            // lblColumnCount
            // 
            this.lblColumnCount.AutoSize = true;
            this.lblColumnCount.Location = new System.Drawing.Point(12, 22);
            this.lblColumnCount.Name = "lblColumnCount";
            this.lblColumnCount.Size = new System.Drawing.Size(36, 13);
            this.lblColumnCount.TabIndex = 1;
            this.lblColumnCount.Text = "Count";
            // 
            // DataBandColumnEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(305, 178);
            this.Controls.Add(this.gbColumns);
            this.Name = "DataBandColumnEditorForm";
            this.Text = "Data Band Column Editor";
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.gbColumns, 0);
            this.gbColumns.ResumeLayout(false);
            this.gbColumns.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox gbColumns;
        private Label lblWidth;
        private Label lblMinRowCount;
        private Label lblLayout;
        private Label lblColumnCount;
        private TextBox tbColumnCount;
        private TextBox tbWidth;
        private TextBox tbColumnMinRowCount;
        private ComboBox cbxColumnLayout;
    }
}