namespace FastReport.Messaging
{
    partial class TelegramMessengerForm
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
            this.signOutBtn = new System.Windows.Forms.Button();
            this.recipientPhoneTextBox = new System.Windows.Forms.TextBox();
            this.recipientPhoneLbl = new System.Windows.Forms.Label();
            this.pgFile.SuspendLayout();
            this.pgProxy.SuspendLayout();
            this.pageControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pgFile
            // 
            this.pgFile.Controls.Add(this.recipientPhoneLbl);
            this.pgFile.Controls.Add(this.recipientPhoneTextBox);
            this.pgFile.Controls.SetChildIndex(this.labelFileType, 0);
            this.pgFile.Controls.SetChildIndex(this.cbFileType, 0);
            this.pgFile.Controls.SetChildIndex(this.buttonSettings, 0);
            this.pgFile.Controls.SetChildIndex(this.recipientPhoneTextBox, 0);
            this.pgFile.Controls.SetChildIndex(this.recipientPhoneLbl, 0);
            // 
            // pgProxy
            // 
            this.pgProxy.Visible = false;
            // 
            // cbFileType
            // 
            this.cbFileType.Size = new System.Drawing.Size(220, 27);
            // 
            // pageControl1
            // 
            this.pageControl1.Location = new System.Drawing.Point(12, 11);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(290, 138);
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click_1);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(412, 138);
            // 
            // signOutBtn
            // 
            this.signOutBtn.Location = new System.Drawing.Point(4, 138);
            this.signOutBtn.Margin = this.btnOk.Margin;
            this.signOutBtn.Name = "signOutBtn";
            this.signOutBtn.Size = this.btnOk.Size;
            this.signOutBtn.TabIndex = 3;
            this.signOutBtn.Text = "Sign Out";
            this.signOutBtn.UseVisualStyleBackColor = true;
            this.signOutBtn.Click += new System.EventHandler(this.signOutBtn_Click_1);
            // 
            // recipientPhoneTextBox
            // 
            this.recipientPhoneTextBox.Location = new System.Drawing.Point(178, 78);
            this.recipientPhoneTextBox.Name = "recipientPhoneTextBox";
            this.recipientPhoneTextBox.Size = new System.Drawing.Size(220, 27);
            this.recipientPhoneTextBox.TabIndex = 3;
            this.recipientPhoneTextBox.TextChanged += new System.EventHandler(this.recipientPhoneTextBox_TextChanged);
            // 
            // recipientPhoneLbl
            // 
            this.recipientPhoneLbl.AutoSize = true;
            this.recipientPhoneLbl.Location = new System.Drawing.Point(22, 81);
            this.recipientPhoneLbl.Name = "recipientPhoneLbl";
            this.recipientPhoneLbl.Size = new System.Drawing.Size(128, 19);
            this.recipientPhoneLbl.TabIndex = 4;
            this.recipientPhoneLbl.Text = "Recipient Phone:";
            // 
            // TelegramMessengerForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(540, 184);
            this.Controls.Add(this.signOutBtn);
            this.Name = "TelegramMessengerForm";
            this.TopMost = true;
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.pageControl1, 0);
            this.Controls.SetChildIndex(this.signOutBtn, 0);
            this.pgFile.ResumeLayout(false);
            this.pgFile.PerformLayout();
            this.pgProxy.ResumeLayout(false);
            this.pgProxy.PerformLayout();
            this.pageControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Button signOutBtn;
        private System.Windows.Forms.Label recipientPhoneLbl;
        private System.Windows.Forms.TextBox recipientPhoneTextBox;
    }
}