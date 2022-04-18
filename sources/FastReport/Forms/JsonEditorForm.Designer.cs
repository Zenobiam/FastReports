using FastReport.Controls;

namespace FastReport.Forms
{
    partial class JsonEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JsonEditorForm));
            this.tbJson = new System.Windows.Forms.TextBox();
            this.cbWordWrap = new FastReport.Controls.ScalableCheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.cbEnconding = new System.Windows.Forms.ToolStripComboBox();
            this.btnFormat = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(138, 246);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(219, 246);
            // 
            // tbJson
            // 
            this.tbJson.AcceptsReturn = true;
            this.tbJson.AcceptsTab = true;
            this.tbJson.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbJson.Location = new System.Drawing.Point(12, 28);
            this.tbJson.MaxLength = 2147483647;
            this.tbJson.Multiline = true;
            this.tbJson.Name = "tbJson";
            this.tbJson.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbJson.Size = new System.Drawing.Size(282, 190);
            this.tbJson.TabIndex = 1;
            this.tbJson.WordWrap = false;
            this.tbJson.TextChanged += new System.EventHandler(this.TbJson_TextChanged);
            // 
            // cbWordWrap
            // 
            this.cbWordWrap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbWordWrap.Location = new System.Drawing.Point(12, 224);
            this.cbWordWrap.Name = "cbWordWrap";
            this.cbWordWrap.Size = new System.Drawing.Size(74, 17);
            this.cbWordWrap.TabIndex = 2;
            this.cbWordWrap.Text = "Word wrap";
            this.cbWordWrap.UseVisualStyleBackColor = true;
            this.cbWordWrap.CheckedChanged += new System.EventHandler(this.CbWordWrap_CheckedChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOpen,
            this.btnSave,
            this.cbEnconding,
            this.btnFormat});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(306, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.Image = ((System.Drawing.Image)(resources.GetObject("btnOpen.Image")));
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(23, 22);
            this.btnOpen.Text = "toolStripButton1";
            this.btnOpen.Click += new System.EventHandler(this.BtnOpen_Click);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "toolStripButton2";
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // cbEnconding
            // 
            this.cbEnconding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEnconding.Name = "cbEnconding";
            this.cbEnconding.Size = new System.Drawing.Size(121, 25);
            this.cbEnconding.SelectedIndexChanged += new System.EventHandler(this.CbEnconding_SelectedIndexChanged);
            // 
            // btnFormat
            // 
            this.btnFormat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFormat.Image = ((System.Drawing.Image)(resources.GetObject("btnFormat.Image")));
            this.btnFormat.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFormat.Name = "btnFormat";
            this.btnFormat.Size = new System.Drawing.Size(23, 22);
            this.btnFormat.Text = "toolStripButton3";
            this.btnFormat.Click += new System.EventHandler(this.BtnFormat_Click);
            // 
            // JsonEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(306, 281);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.cbWordWrap);
            this.Controls.Add(this.tbJson);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MinimumSize = new System.Drawing.Size(320, 320);
            this.Name = "JsonEditorForm";
            this.ShowIcon = false;
            this.Text = "TextOnlyEditorForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JsonEditorForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.JsonEditorForm_FormClosed);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.tbJson, 0);
            this.Controls.SetChildIndex(this.cbWordWrap, 0);
            this.Controls.SetChildIndex(this.toolStrip1, 0);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbJson;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnFormat;
        private System.Windows.Forms.ToolStripComboBox cbEnconding;
        private ScalableCheckBox cbWordWrap;
    }
}