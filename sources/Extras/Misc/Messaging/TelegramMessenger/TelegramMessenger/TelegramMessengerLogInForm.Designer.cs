namespace FastReport.Messaging
{
    partial class TelegramMessengerLogInForm
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
            this.okBtn = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.senderPhoneTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(12, 73);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(307, 28);
            this.okBtn.TabIndex = 16;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_ClickAsync);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(111, 20);
            this.label6.TabIndex = 13;
            this.label6.Text = "Sender Phone";
            // 
            // senderPhoneTextBox
            // 
            this.senderPhoneTextBox.Location = new System.Drawing.Point(12, 30);
            this.senderPhoneTextBox.Name = "senderPhoneTextBox";
            this.senderPhoneTextBox.Size = new System.Drawing.Size(307, 26);
            this.senderPhoneTextBox.TabIndex = 12;
            this.senderPhoneTextBox.TextChanged += new System.EventHandler(this.senderPhoneTextBox_TextChanged);
            // 
            // TelegramMessengerLogInForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(331, 124);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.senderPhoneTextBox);
            this.Name = "TelegramMessengerLogInForm";
            this.Text = "Log In";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TelegramMessengerInfoForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox senderPhoneTextBox;
    }
}