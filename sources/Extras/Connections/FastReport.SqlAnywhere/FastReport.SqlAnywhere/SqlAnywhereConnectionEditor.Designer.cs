namespace FastReport.SqlAnywhere
{
    partial class SqlAnywhereConnectionEditor
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
            this.tbDatabase = new System.Windows.Forms.TextBox();
            this.gbDatabase = new System.Windows.Forms.GroupBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.btnAdvanced = new System.Windows.Forms.Button();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.gbServer = new System.Windows.Forms.GroupBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.tbServer = new System.Windows.Forms.TextBox();
            this.gbDatabase.SuspendLayout();
            this.gbServer.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbDatabase
            // 
            this.tbDatabase.Location = new System.Drawing.Point(12, 40);
            this.tbDatabase.Name = "tbDatabase";
            this.tbDatabase.Size = new System.Drawing.Size(296, 20);
            this.tbDatabase.TabIndex = 0;
            // 
            // gbDatabase
            // 
            this.gbDatabase.Controls.Add(this.lblDatabase);
            this.gbDatabase.Controls.Add(this.tbDatabase);
            this.gbDatabase.Location = new System.Drawing.Point(8, 144);
            this.gbDatabase.Name = "gbDatabase";
            this.gbDatabase.Size = new System.Drawing.Size(320, 76);
            this.gbDatabase.TabIndex = 5;
            this.gbDatabase.TabStop = false;
            this.gbDatabase.Text = "Database";
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(12, 20);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(57, 13);
            this.lblDatabase.TabIndex = 3;
            this.lblDatabase.Text = "Database:";
            // 
            // btnAdvanced
            // 
            this.btnAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdvanced.AutoSize = true;
            this.btnAdvanced.Location = new System.Drawing.Point(252, 228);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Size = new System.Drawing.Size(77, 23);
            this.btnAdvanced.TabIndex = 4;
            this.btnAdvanced.Text = "Advanced...";
            this.btnAdvanced.UseVisualStyleBackColor = true;
            this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
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
            // gbServer
            // 
            this.gbServer.Controls.Add(this.lblServer);
            this.gbServer.Controls.Add(this.tbServer);
            this.gbServer.Controls.Add(this.tbUserName);
            this.gbServer.Controls.Add(this.tbPassword);
            this.gbServer.Controls.Add(this.lblUserName);
            this.gbServer.Controls.Add(this.lblPassword);
            this.gbServer.Location = new System.Drawing.Point(8, 12);
            this.gbServer.Name = "gbServer";
            this.gbServer.Size = new System.Drawing.Size(320, 128);
            this.gbServer.TabIndex = 6;
            this.gbServer.TabStop = false;
            this.gbServer.Text = "Server";
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(12, 20);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(72, 13);
            this.lblServer.TabIndex = 4;
            this.lblServer.Text = "Server name:";
            // 
            // tbServer
            // 
            this.tbServer.Location = new System.Drawing.Point(12, 40);
            this.tbServer.Name = "tbServer";
            this.tbServer.Size = new System.Drawing.Size(296, 20);
            this.tbServer.TabIndex = 0;
            // 
            // SqlAnywhereConnectionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbDatabase);
            this.Controls.Add(this.btnAdvanced);
            this.Controls.Add(this.gbServer);
            this.Name = "SqlAnywhereConnectionEditor";
            this.Size = new System.Drawing.Size(336, 263);
            this.gbDatabase.ResumeLayout(false);
            this.gbDatabase.PerformLayout();
            this.gbServer.ResumeLayout(false);
            this.gbServer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDatabase;
        private System.Windows.Forms.GroupBox gbDatabase;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.Button btnAdvanced;
        private System.Windows.Forms.TextBox tbUserName;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.GroupBox gbServer;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.TextBox tbServer;
    }
}
