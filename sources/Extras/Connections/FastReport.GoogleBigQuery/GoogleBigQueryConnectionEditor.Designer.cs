namespace FastReport.Data
{
  partial class GoogleBigQueryConnectionEditor
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
            this.btnAdvanced = new System.Windows.Forms.Button();
            this.gbDatabase = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbClientSecret = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbClientID = new System.Windows.Forms.TextBox();
            this.lblDataFile = new System.Windows.Forms.Label();
            this.tbGoogleProjectID = new System.Windows.Forms.TextBox();
            this.label1 = new FastReport.Controls.LabelLine();
            this.gbDatabase.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAdvanced
            // 
            this.btnAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdvanced.AutoSize = true;
            this.btnAdvanced.Location = new System.Drawing.Point(252, 140);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Size = new System.Drawing.Size(77, 23);
            this.btnAdvanced.TabIndex = 0;
            this.btnAdvanced.Text = "Advanced...";
            this.btnAdvanced.UseVisualStyleBackColor = true;
            this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
            // 
            // gbDatabase
            // 
            this.gbDatabase.Controls.Add(this.label3);
            this.gbDatabase.Controls.Add(this.tbClientSecret);
            this.gbDatabase.Controls.Add(this.label2);
            this.gbDatabase.Controls.Add(this.tbClientID);
            this.gbDatabase.Controls.Add(this.lblDataFile);
            this.gbDatabase.Controls.Add(this.tbGoogleProjectID);
            this.gbDatabase.Location = new System.Drawing.Point(8, 4);
            this.gbDatabase.Name = "gbDatabase";
            this.gbDatabase.Size = new System.Drawing.Size(320, 128);
            this.gbDatabase.TabIndex = 1;
            this.gbDatabase.TabStop = false;
            this.gbDatabase.Text = "Database";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Client Secret:";
            // 
            // tbClientSecret
            // 
            this.tbClientSecret.Location = new System.Drawing.Point(120, 89);
            this.tbClientSecret.Name = "tbClientSecret";
            this.tbClientSecret.PasswordChar = '*';
            this.tbClientSecret.Size = new System.Drawing.Size(188, 20);
            this.tbClientSecret.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Client ID:";
            // 
            // tbClientID
            // 
            this.tbClientID.Location = new System.Drawing.Point(120, 54);
            this.tbClientID.Name = "tbClientID";
            this.tbClientID.Size = new System.Drawing.Size(188, 20);
            this.tbClientID.TabIndex = 4;
            // 
            // lblDataFile
            // 
            this.lblDataFile.AutoSize = true;
            this.lblDataFile.Location = new System.Drawing.Point(12, 20);
            this.lblDataFile.Name = "lblDataFile";
            this.lblDataFile.Size = new System.Drawing.Size(95, 13);
            this.lblDataFile.TabIndex = 3;
            this.lblDataFile.Text = "Google Project ID:";
            // 
            // tbGoogleProjectID
            // 
            this.tbGoogleProjectID.Location = new System.Drawing.Point(120, 19);
            this.tbGoogleProjectID.Name = "tbGoogleProjectID";
            this.tbGoogleProjectID.Size = new System.Drawing.Size(188, 20);
            this.tbGoogleProjectID.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 164);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(320, 17);
            this.label1.TabIndex = 2;
            // 
            // GoogleBigQueryConnectionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gbDatabase);
            this.Controls.Add(this.btnAdvanced);
            this.Name = "GoogleBigQueryConnectionEditor";
            this.Size = new System.Drawing.Size(336, 183);
            this.gbDatabase.ResumeLayout(false);
            this.gbDatabase.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnAdvanced;
    private System.Windows.Forms.GroupBox gbDatabase;
    private System.Windows.Forms.Label lblDataFile;
    private System.Windows.Forms.TextBox tbGoogleProjectID;
    private FastReport.Controls.LabelLine label1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox tbClientSecret;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox tbClientID;
  }
}
