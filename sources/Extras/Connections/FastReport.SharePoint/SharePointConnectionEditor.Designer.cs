namespace FastReport.Data
{
    partial class SharePointConnectionEditor 
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbServer = new System.Windows.Forms.GroupBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.tbServer = new System.Windows.Forms.TextBox();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.cbOffice = new System.Windows.Forms.CheckBox();
            this.gbServer.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbServer
            // 
            this.gbServer.Controls.Add(this.cbOffice);
            this.gbServer.Controls.Add(this.lblServer);
            this.gbServer.Controls.Add(this.tbServer);
            this.gbServer.Controls.Add(this.tbUserName);
            this.gbServer.Controls.Add(this.tbPassword);
            this.gbServer.Controls.Add(this.lblUserName);
            this.gbServer.Controls.Add(this.lblPassword);
            this.gbServer.Location = new System.Drawing.Point(8, 3);
            this.gbServer.Name = "gbServer";
            this.gbServer.Size = new System.Drawing.Size(320, 152);
            this.gbServer.TabIndex = 5;
            this.gbServer.TabStop = false;
            this.gbServer.Text = "Server";
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(12, 20);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(65, 13);
            this.lblServer.TabIndex = 4;
            this.lblServer.Text = "Server URL:";
            // 
            // tbServer
            // 
            this.tbServer.Location = new System.Drawing.Point(12, 40);
            this.tbServer.Name = "tbServer";
            this.tbServer.Size = new System.Drawing.Size(296, 20);
            this.tbServer.TabIndex = 0;
            // 
            // tbUserName
            // 
            this.tbUserName.Location = new System.Drawing.Point(120, 72);
            this.tbUserName.Name = "tbUserName";
            this.tbUserName.Size = new System.Drawing.Size(188, 20);
            this.tbUserName.TabIndex = 1;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(120, 96);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(188, 20);
            this.tbPassword.TabIndex = 2;
            this.tbPassword.UseSystemPasswordChar = true;
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(12, 76);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(62, 13);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "User name:";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(12, 100);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(57, 13);
            this.lblPassword.TabIndex = 1;
            this.lblPassword.Text = "Password:";
            // 
            // cbOffice
            // 
            this.cbOffice.AutoSize = true;
            this.cbOffice.Location = new System.Drawing.Point(120, 122);
            this.cbOffice.Name = "cbOffice";
            this.cbOffice.Size = new System.Drawing.Size(76, 17);
            this.cbOffice.TabIndex = 5;
            this.cbOffice.Text = "Office 365";
            this.cbOffice.UseVisualStyleBackColor = true;
            // 
            // SharePointConnectionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbServer);
            this.Name = "SharePointConnectionEditor";
            this.Size = new System.Drawing.Size(336, 166);
            this.gbServer.ResumeLayout(false);
            this.gbServer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbServer;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.TextBox tbServer;
        private System.Windows.Forms.TextBox tbUserName;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.CheckBox cbOffice;
    }
}
