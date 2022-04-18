namespace FastReport.Cloud.StorageClient.GoogleDrive
{
    partial class ClientInfoForm
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
            this.labelClientId = new System.Windows.Forms.Label();
            this.labelClientSecret = new System.Windows.Forms.Label();
            this.tbClientId = new System.Windows.Forms.TextBox();
            this.tbClientSecret = new System.Windows.Forms.TextBox();
            this.labelKey = new System.Windows.Forms.Label();
            this.tbClientKey = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(0, 0);
            this.btnOk.Visible = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(307, 64);
            // 
            // labelClientId
            // 
            this.labelClientId.AutoSize = true;
            this.labelClientId.Location = new System.Drawing.Point(12, 15);
            this.labelClientId.Name = "labelClientId";
            this.labelClientId.Size = new System.Drawing.Size(52, 13);
            this.labelClientId.TabIndex = 1;
            this.labelClientId.Text = "Client ID:";
            // 
            // labelClientSecret
            // 
            this.labelClientSecret.AutoSize = true;
            this.labelClientSecret.Location = new System.Drawing.Point(12, 41);
            this.labelClientSecret.Name = "labelClientSecret";
            this.labelClientSecret.Size = new System.Drawing.Size(72, 13);
            this.labelClientSecret.TabIndex = 2;
            this.labelClientSecret.Text = "Client Secret:";
            // 
            // tbClientId
            // 
            this.tbClientId.Location = new System.Drawing.Point(142, 12);
            this.tbClientId.Name = "tbClientId";
            this.tbClientId.Size = new System.Drawing.Size(240, 20);
            this.tbClientId.TabIndex = 3;
            // 
            // tbClientSecret
            // 
            this.tbClientSecret.Location = new System.Drawing.Point(142, 38);
            this.tbClientSecret.Name = "tbClientSecret";
            this.tbClientSecret.Size = new System.Drawing.Size(240, 20);
            this.tbClientSecret.TabIndex = 4;
            // 
            // labelKey
            // 
            this.labelKey.AutoSize = true;
            this.labelKey.Location = new System.Drawing.Point(12, 64);
            this.labelKey.Name = "labelKey";
            this.labelKey.Size = new System.Drawing.Size(82, 13);
            this.labelKey.TabIndex = 2;
            this.labelKey.Text = "Client AuthKey:";
            this.labelKey.Visible = false;
            // 
            // tbClientKey
            // 
            this.tbClientKey.Location = new System.Drawing.Point(142, 64);
            this.tbClientKey.Name = "tbClientKey";
            this.tbClientKey.Size = new System.Drawing.Size(240, 20);
            this.tbClientKey.TabIndex = 4;
            this.tbClientKey.Visible = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(226, 64);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // ClientInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(394, 99);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbClientId);
            this.Controls.Add(this.tbClientKey);
            this.Controls.Add(this.tbClientSecret);
            this.Controls.Add(this.labelKey);
            this.Controls.Add(this.labelClientSecret);
            this.Controls.Add(this.labelClientId);
            this.Name = "ClientInfoForm";
            this.Text = "Client Info";
            this.Controls.SetChildIndex(this.labelClientId, 0);
            this.Controls.SetChildIndex(this.labelClientSecret, 0);
            this.Controls.SetChildIndex(this.labelKey, 0);
            this.Controls.SetChildIndex(this.tbClientSecret, 0);
            this.Controls.SetChildIndex(this.tbClientKey, 0);
            this.Controls.SetChildIndex(this.tbClientId, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.button1, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelClientId;
        private System.Windows.Forms.Label labelClientSecret;
        private System.Windows.Forms.TextBox tbClientId;
        private System.Windows.Forms.TextBox tbClientSecret;
        private System.Windows.Forms.Label labelKey;
        private System.Windows.Forms.TextBox tbClientKey;
        private System.Windows.Forms.Button button1;
    }
}