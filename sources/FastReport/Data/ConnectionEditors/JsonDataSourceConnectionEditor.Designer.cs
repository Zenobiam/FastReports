using FastReport.Controls;

namespace FastReport.Data.ConnectionEditors
{
    partial class JsonDataSourceConnectionEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbConnection = new System.Windows.Forms.GroupBox();
            this.lbHeaders = new System.Windows.Forms.Label();
            this.dgvHeaders = new System.Windows.Forms.DataGridView();
            this.colHeader = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colContent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lbJsonSchema = new System.Windows.Forms.Label();
            this.tbJsonSchema = new FastReport.Controls.TextBoxButton();
            this.lbEncoding = new System.Windows.Forms.Label();
            this.cbEnconding = new System.Windows.Forms.ComboBox();
            this.lbJson = new System.Windows.Forms.Label();
            this.tbJson = new FastReport.Controls.TextBoxButton();
            this.gbConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHeaders)).BeginInit();
            this.SuspendLayout();
            // 
            // gbConnection
            // 
            this.gbConnection.Controls.Add(this.lbHeaders);
            this.gbConnection.Controls.Add(this.dgvHeaders);
            this.gbConnection.Controls.Add(this.lbJsonSchema);
            this.gbConnection.Controls.Add(this.tbJsonSchema);
            this.gbConnection.Controls.Add(this.lbEncoding);
            this.gbConnection.Controls.Add(this.cbEnconding);
            this.gbConnection.Controls.Add(this.lbJson);
            this.gbConnection.Controls.Add(this.tbJson);
            this.gbConnection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gbConnection.Location = new System.Drawing.Point(8, 4);
            this.gbConnection.Margin = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.gbConnection.Name = "gbConnection";
            this.gbConnection.Size = new System.Drawing.Size(320, 233);
            this.gbConnection.TabIndex = 0;
            this.gbConnection.TabStop = false;
            this.gbConnection.Text = "Connection settings";
            // 
            // lbHeaders
            // 
            this.lbHeaders.AutoSize = true;
            this.lbHeaders.Location = new System.Drawing.Point(11, 139);
            this.lbHeaders.Name = "lbHeaders";
            this.lbHeaders.Size = new System.Drawing.Size(47, 13);
            this.lbHeaders.TabIndex = 12;
            this.lbHeaders.Text = "Headers";
            // 
            // dgvHeaders
            // 
            this.dgvHeaders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHeaders.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHeader,
            this.colContent});
            this.dgvHeaders.Location = new System.Drawing.Point(11, 156);
            this.dgvHeaders.Name = "dgvHeaders";
            this.dgvHeaders.RowHeadersVisible = false;
            this.dgvHeaders.Size = new System.Drawing.Size(303, 71);
            this.dgvHeaders.TabIndex = 11;
            // 
            // colHeader
            // 
            this.colHeader.HeaderText = "Header";
            this.colHeader.Name = "colHeader";
            // 
            // colContent
            // 
            this.colContent.HeaderText = "Content";
            this.colContent.Name = "colContent";
            this.colContent.Width = 150;
            // 
            // lbJsonSchema
            // 
            this.lbJsonSchema.AutoSize = true;
            this.lbJsonSchema.Location = new System.Drawing.Point(8, 94);
            this.lbJsonSchema.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.lbJsonSchema.Name = "lbJsonSchema";
            this.lbJsonSchema.Size = new System.Drawing.Size(71, 13);
            this.lbJsonSchema.TabIndex = 10;
            this.lbJsonSchema.Text = "Json Schema";
            // 
            // tbJsonSchema
            // 
            this.tbJsonSchema.ButtonText = "...";
            this.tbJsonSchema.Image = null;
            this.tbJsonSchema.Location = new System.Drawing.Point(11, 110);
            this.tbJsonSchema.Margin = new System.Windows.Forms.Padding(8, 3, 3, 5);
            this.tbJsonSchema.Name = "tbJsonSchema";
            this.tbJsonSchema.Size = new System.Drawing.Size(303, 20);
            this.tbJsonSchema.TabIndex = 8;
            this.tbJsonSchema.ButtonClick += new System.EventHandler(this.BtnJsonSchema_Click);
            // 
            // lbEncoding
            // 
            this.lbEncoding.AutoSize = true;
            this.lbEncoding.Location = new System.Drawing.Point(8, 25);
            this.lbEncoding.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            this.lbEncoding.Name = "lbEncoding";
            this.lbEncoding.Size = new System.Drawing.Size(52, 13);
            this.lbEncoding.TabIndex = 4;
            this.lbEncoding.Text = "Encoding";
            // 
            // cbEnconding
            // 
            this.cbEnconding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEnconding.FormattingEnabled = true;
            this.cbEnconding.Location = new System.Drawing.Point(172, 22);
            this.cbEnconding.Margin = new System.Windows.Forms.Padding(3, 3, 8, 3);
            this.cbEnconding.Name = "cbEnconding";
            this.cbEnconding.Size = new System.Drawing.Size(142, 21);
            this.cbEnconding.TabIndex = 3;
            this.cbEnconding.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // lbJson
            // 
            this.lbJson.AutoSize = true;
            this.lbJson.Location = new System.Drawing.Point(8, 48);
            this.lbJson.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.lbJson.Name = "lbJson";
            this.lbJson.Size = new System.Drawing.Size(32, 13);
            this.lbJson.TabIndex = 2;
            this.lbJson.Text = "Json ";
            // 
            // tbJson
            // 
            this.tbJson.ButtonText = "...";
            this.tbJson.Image = null;
            this.tbJson.Location = new System.Drawing.Point(11, 64);
            this.tbJson.Margin = new System.Windows.Forms.Padding(8, 3, 0, 5);
            this.tbJson.Name = "tbJson";
            this.tbJson.Size = new System.Drawing.Size(303, 20);
            this.tbJson.TabIndex = 0;
            this.tbJson.ButtonClick += new System.EventHandler(this.BtnJson_Click);
            // 
            // JsonDataSourceConnectionEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.gbConnection);
            this.Name = "JsonDataSourceConnectionEditor";
            this.Size = new System.Drawing.Size(336, 241);
            this.gbConnection.ResumeLayout(false);
            this.gbConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHeaders)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbConnection;
        private TextBoxButton tbJson;
        private System.Windows.Forms.Label lbJson;
        private System.Windows.Forms.Label lbJsonSchema;
        private TextBoxButton tbJsonSchema;
        private System.Windows.Forms.Label lbEncoding;
        private System.Windows.Forms.ComboBox cbEnconding;
        private System.Windows.Forms.Label lbHeaders;
        private System.Windows.Forms.DataGridView dgvHeaders;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHeader;
        private System.Windows.Forms.DataGridViewTextBoxColumn colContent;
    }
}
