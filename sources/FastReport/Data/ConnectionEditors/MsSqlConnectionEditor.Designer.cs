using FastReport.Controls;

namespace FastReport.Data.ConnectionEditors
{
  partial class MsSqlConnectionEditor
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
            this.gbDatabase = new System.Windows.Forms.GroupBox();
            this.tbDatabaseFile = new FastReport.Controls.TextBoxButton();
            this.rbDatabaseFile = new FastReport.Controls.ScalableRadioButton();
            this.cbxDatabaseName = new System.Windows.Forms.ComboBox();
            this.rbDatabaseName = new FastReport.Controls.ScalableRadioButton();
            this.cbxServer = new System.Windows.Forms.ComboBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.gbServerLogon = new System.Windows.Forms.GroupBox();
            this.cbSavePassword = new System.Windows.Forms.CheckBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.rbUseSql = new FastReport.Controls.ScalableRadioButton();
            this.rbUseWindows = new FastReport.Controls.ScalableRadioButton();
            this.labelLine1 = new FastReport.Controls.LabelLine();
            this.btnAdvanced = new System.Windows.Forms.Button();
            this.gbDatabase.SuspendLayout();
            this.gbServerLogon.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbDatabase
            // 
            this.gbDatabase.Controls.Add(this.tbDatabaseFile);
            this.gbDatabase.Controls.Add(this.rbDatabaseFile);
            this.gbDatabase.Controls.Add(this.cbxDatabaseName);
            this.gbDatabase.Controls.Add(this.rbDatabaseName);
            this.gbDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gbDatabase.Location = new System.Drawing.Point(8, 200);
            this.gbDatabase.Name = "gbDatabase";
            this.gbDatabase.Size = new System.Drawing.Size(320, 128);
            this.gbDatabase.TabIndex = 7;
            this.gbDatabase.TabStop = false;
            this.gbDatabase.Text = "Connect to a database";
            // 
            // tbDatabaseFile
            // 
            this.tbDatabaseFile.ButtonText = "";
            this.tbDatabaseFile.Image = null;
            this.tbDatabaseFile.Location = new System.Drawing.Point(32, 92);
            this.tbDatabaseFile.Name = "tbDatabaseFile";
            this.tbDatabaseFile.Size = new System.Drawing.Size(276, 21);
            this.tbDatabaseFile.TabIndex = 3;
            this.tbDatabaseFile.ButtonClick += new System.EventHandler(this.tbDatabaseFile_ButtonClick);
            // 
            // rbDatabaseFile
            // 
            this.rbDatabaseFile.AutoSize = true;
            this.rbDatabaseFile.Location = new System.Drawing.Point(12, 72);
            this.rbDatabaseFile.Name = "rbDatabaseFile";
            this.rbDatabaseFile.Size = new System.Drawing.Size(131, 17);
            this.rbDatabaseFile.TabIndex = 2;
            this.rbDatabaseFile.TabStop = true;
            this.rbDatabaseFile.Text = "Attach a database file:";
            this.rbDatabaseFile.UseVisualStyleBackColor = true;
            this.rbDatabaseFile.CheckedChanged += new System.EventHandler(this.UpdateControls);
            // 
            // cbxDatabaseName
            // 
            this.cbxDatabaseName.FormattingEnabled = true;
            this.cbxDatabaseName.Location = new System.Drawing.Point(32, 40);
            this.cbxDatabaseName.Name = "cbxDatabaseName";
            this.cbxDatabaseName.Size = new System.Drawing.Size(276, 21);
            this.cbxDatabaseName.TabIndex = 1;
            this.cbxDatabaseName.DropDown += new System.EventHandler(this.cbxDatabaseName_DropDown);
            // 
            // rbDatabaseName
            // 
            this.rbDatabaseName.AutoSize = true;
            this.rbDatabaseName.Location = new System.Drawing.Point(12, 20);
            this.rbDatabaseName.Name = "rbDatabaseName";
            this.rbDatabaseName.Size = new System.Drawing.Size(182, 17);
            this.rbDatabaseName.TabIndex = 0;
            this.rbDatabaseName.TabStop = true;
            this.rbDatabaseName.Text = "Select or enter a database name:";
            this.rbDatabaseName.UseVisualStyleBackColor = true;
            this.rbDatabaseName.CheckedChanged += new System.EventHandler(this.UpdateControls);
            // 
            // cbxServer
            // 
            this.cbxServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbxServer.FormattingEnabled = true;
            this.cbxServer.Location = new System.Drawing.Point(8, 24);
            this.cbxServer.Name = "cbxServer";
            this.cbxServer.Size = new System.Drawing.Size(320, 21);
            this.cbxServer.TabIndex = 6;
            this.cbxServer.DropDown += new System.EventHandler(this.cbxServer_DropDown);
            this.cbxServer.TextChanged += new System.EventHandler(this.ServerOrLoginChanged);
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblServer.Location = new System.Drawing.Point(8, 8);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(70, 13);
            this.lblServer.TabIndex = 5;
            this.lblServer.Text = "Server name:";
            // 
            // gbServerLogon
            // 
            this.gbServerLogon.Controls.Add(this.cbSavePassword);
            this.gbServerLogon.Controls.Add(this.tbPassword);
            this.gbServerLogon.Controls.Add(this.tbUserName);
            this.gbServerLogon.Controls.Add(this.lblPassword);
            this.gbServerLogon.Controls.Add(this.lblUserName);
            this.gbServerLogon.Controls.Add(this.rbUseSql);
            this.gbServerLogon.Controls.Add(this.rbUseWindows);
            this.gbServerLogon.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gbServerLogon.Location = new System.Drawing.Point(8, 52);
            this.gbServerLogon.Name = "gbServerLogon";
            this.gbServerLogon.Size = new System.Drawing.Size(320, 144);
            this.gbServerLogon.TabIndex = 4;
            this.gbServerLogon.TabStop = false;
            this.gbServerLogon.Text = "Log on to the server";
            // 
            // cbSavePassword
            // 
            this.cbSavePassword.AutoSize = true;
            this.cbSavePassword.Location = new System.Drawing.Point(120, 116);
            this.cbSavePassword.Name = "cbSavePassword";
            this.cbSavePassword.Size = new System.Drawing.Size(115, 17);
            this.cbSavePassword.TabIndex = 4;
            this.cbSavePassword.Text = "Save my password";
            this.cbSavePassword.UseVisualStyleBackColor = true;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(120, 92);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(188, 20);
            this.tbPassword.TabIndex = 3;
            this.tbPassword.UseSystemPasswordChar = true;
            this.tbPassword.TextChanged += new System.EventHandler(this.ServerOrLoginChanged);
            // 
            // tbUserName
            // 
            this.tbUserName.Location = new System.Drawing.Point(120, 68);
            this.tbUserName.Name = "tbUserName";
            this.tbUserName.Size = new System.Drawing.Size(188, 20);
            this.tbUserName.TabIndex = 3;
            this.tbUserName.TextChanged += new System.EventHandler(this.ServerOrLoginChanged);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(28, 96);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.TabIndex = 2;
            this.lblPassword.Text = "Password:";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(28, 72);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(61, 13);
            this.lblUserName.TabIndex = 1;
            this.lblUserName.Text = "User name:";
            // 
            // rbUseSql
            // 
            this.rbUseSql.AutoSize = true;
            this.rbUseSql.Location = new System.Drawing.Point(12, 44);
            this.rbUseSql.Name = "rbUseSql";
            this.rbUseSql.Size = new System.Drawing.Size(173, 17);
            this.rbUseSql.TabIndex = 0;
            this.rbUseSql.TabStop = true;
            this.rbUseSql.Text = "Use SQL Server Authentication";
            this.rbUseSql.UseVisualStyleBackColor = true;
            this.rbUseSql.CheckedChanged += new System.EventHandler(this.ServerOrLoginChanged);
            // 
            // rbUseWindows
            // 
            this.rbUseWindows.AutoSize = true;
            this.rbUseWindows.Location = new System.Drawing.Point(12, 20);
            this.rbUseWindows.Name = "rbUseWindows";
            this.rbUseWindows.Size = new System.Drawing.Size(162, 17);
            this.rbUseWindows.TabIndex = 0;
            this.rbUseWindows.TabStop = true;
            this.rbUseWindows.Text = "Use Windows Authentication";
            this.rbUseWindows.UseVisualStyleBackColor = true;
            this.rbUseWindows.CheckedChanged += new System.EventHandler(this.ServerOrLoginChanged);
            // 
            // labelLine1
            // 
            this.labelLine1.Location = new System.Drawing.Point(8, 360);
            this.labelLine1.Name = "labelLine1";
            this.labelLine1.Size = new System.Drawing.Size(320, 16);
            this.labelLine1.TabIndex = 9;
            // 
            // btnAdvanced
            // 
            this.btnAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdvanced.AutoSize = true;
            this.btnAdvanced.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAdvanced.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnAdvanced.Location = new System.Drawing.Point(252, 336);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Size = new System.Drawing.Size(75, 23);
            this.btnAdvanced.TabIndex = 8;
            this.btnAdvanced.Text = "Advanced...";
            this.btnAdvanced.UseVisualStyleBackColor = true;
            this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
            // 
            // MsSqlConnectionEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.labelLine1);
            this.Controls.Add(this.btnAdvanced);
            this.Controls.Add(this.gbDatabase);
            this.Controls.Add(this.cbxServer);
            this.Controls.Add(this.lblServer);
            this.Controls.Add(this.gbServerLogon);
            this.Name = "MsSqlConnectionEditor";
            this.Size = new System.Drawing.Size(336, 378);
            this.gbDatabase.ResumeLayout(false);
            this.gbDatabase.PerformLayout();
            this.gbServerLogon.ResumeLayout(false);
            this.gbServerLogon.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox gbDatabase;
    private FastReport.Controls.TextBoxButton tbDatabaseFile;
    private System.Windows.Forms.ComboBox cbxDatabaseName;
    private System.Windows.Forms.ComboBox cbxServer;
    private System.Windows.Forms.Label lblServer;
    private System.Windows.Forms.GroupBox gbServerLogon;
    private System.Windows.Forms.CheckBox cbSavePassword;
    private System.Windows.Forms.TextBox tbPassword;
    private System.Windows.Forms.TextBox tbUserName;
    private System.Windows.Forms.Label lblPassword;
    private System.Windows.Forms.Label lblUserName;
    private FastReport.Controls.LabelLine labelLine1;
    private System.Windows.Forms.Button btnAdvanced;
        private ScalableRadioButton rbDatabaseFile;
        private ScalableRadioButton rbDatabaseName;
        private ScalableRadioButton rbUseSql;
        private ScalableRadioButton rbUseWindows;
    }
}
