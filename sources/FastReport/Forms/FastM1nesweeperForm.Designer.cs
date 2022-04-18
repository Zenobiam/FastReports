namespace FastReport.Forms
{
    partial class FastM1nesweeperForm
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
            this.lWelcome = new System.Windows.Forms.Label();
            this.cbDifficulty = new System.Windows.Forms.ComboBox();
            this.gbSettings = new System.Windows.Forms.GroupBox();
            this.lColumns = new System.Windows.Forms.Label();
            this.lRows = new System.Windows.Forms.Label();
            this.lBombs = new System.Windows.Forms.Label();
            this.tbRows = new System.Windows.Forms.NumericUpDown();
            this.tbColumns = new System.Windows.Forms.NumericUpDown();
            this.tbBombs = new System.Windows.Forms.NumericUpDown();
            this.lDifficulty = new System.Windows.Forms.Label();
            this.gbSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbRows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbColumns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBombs)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(127, 177);
            this.btnOk.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(207, 177);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            // 
            // lWelcome
            // 
            this.lWelcome.AutoSize = true;
            this.lWelcome.Location = new System.Drawing.Point(8, 14);
            this.lWelcome.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lWelcome.Name = "lWelcome";
            this.lWelcome.Size = new System.Drawing.Size(228, 13);
            this.lWelcome.TabIndex = 2;
            this.lWelcome.Text = "Create a new Fast M1nesweeper report game";
            // 
            // cbDifficulty
            // 
            this.cbDifficulty.FormattingEnabled = true;
            this.cbDifficulty.Location = new System.Drawing.Point(129, 17);
            this.cbDifficulty.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbDifficulty.Name = "cbDifficulty";
            this.cbDifficulty.Size = new System.Drawing.Size(137, 21);
            this.cbDifficulty.TabIndex = 3;
            this.cbDifficulty.SelectedIndexChanged += new System.EventHandler(this.cbDifficulty_SelectedIndexChanged);
            // 
            // gbSettings
            // 
            this.gbSettings.Controls.Add(this.lColumns);
            this.gbSettings.Controls.Add(this.lRows);
            this.gbSettings.Controls.Add(this.lBombs);
            this.gbSettings.Controls.Add(this.tbRows);
            this.gbSettings.Controls.Add(this.tbColumns);
            this.gbSettings.Controls.Add(this.tbBombs);
            this.gbSettings.Controls.Add(this.lDifficulty);
            this.gbSettings.Controls.Add(this.cbDifficulty);
            this.gbSettings.Location = new System.Drawing.Point(11, 45);
            this.gbSettings.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gbSettings.Name = "gbSettings";
            this.gbSettings.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gbSettings.Size = new System.Drawing.Size(268, 117);
            this.gbSettings.TabIndex = 4;
            this.gbSettings.TabStop = false;
            this.gbSettings.Text = "Settings";
            // 
            // lColumns
            // 
            this.lColumns.AutoSize = true;
            this.lColumns.Location = new System.Drawing.Point(4, 63);
            this.lColumns.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lColumns.Name = "lColumns";
            this.lColumns.Size = new System.Drawing.Size(47, 13);
            this.lColumns.TabIndex = 10;
            this.lColumns.Text = "Columns";
            // 
            // lRows
            // 
            this.lRows.AutoSize = true;
            this.lRows.Location = new System.Drawing.Point(4, 85);
            this.lRows.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lRows.Name = "lRows";
            this.lRows.Size = new System.Drawing.Size(33, 13);
            this.lRows.TabIndex = 9;
            this.lRows.Text = "Rows";
            // 
            // lBombs
            // 
            this.lBombs.AutoSize = true;
            this.lBombs.Location = new System.Drawing.Point(4, 41);
            this.lBombs.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lBombs.Name = "lBombs";
            this.lBombs.Size = new System.Drawing.Size(38, 13);
            this.lBombs.TabIndex = 8;
            this.lBombs.Text = "Bombs";
            // 
            // tbRows
            // 
            this.tbRows.Location = new System.Drawing.Point(129, 83);
            this.tbRows.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbRows.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.tbRows.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.tbRows.Name = "tbRows";
            this.tbRows.Size = new System.Drawing.Size(135, 20);
            this.tbRows.TabIndex = 7;
            this.tbRows.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.tbRows.ValueChanged += new System.EventHandler(this.Settings_ValueChanged);
            // 
            // tbColumns
            // 
            this.tbColumns.Location = new System.Drawing.Point(129, 61);
            this.tbColumns.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbColumns.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.tbColumns.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.tbColumns.Name = "tbColumns";
            this.tbColumns.Size = new System.Drawing.Size(135, 20);
            this.tbColumns.TabIndex = 6;
            this.tbColumns.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.tbColumns.ValueChanged += new System.EventHandler(this.Settings_ValueChanged);
            // 
            // tbBombs
            // 
            this.tbBombs.Location = new System.Drawing.Point(129, 39);
            this.tbBombs.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbBombs.Maximum = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.tbBombs.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tbBombs.Name = "tbBombs";
            this.tbBombs.Size = new System.Drawing.Size(135, 20);
            this.tbBombs.TabIndex = 5;
            this.tbBombs.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.tbBombs.ValueChanged += new System.EventHandler(this.Settings_ValueChanged);
            // 
            // lDifficulty
            // 
            this.lDifficulty.AutoSize = true;
            this.lDifficulty.Location = new System.Drawing.Point(4, 19);
            this.lDifficulty.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lDifficulty.Name = "lDifficulty";
            this.lDifficulty.Size = new System.Drawing.Size(49, 13);
            this.lDifficulty.TabIndex = 4;
            this.lDifficulty.Text = "Difficulty";
            // 
            // FastM1nesweeperForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.ClientSize = new System.Drawing.Size(290, 215);
            this.Controls.Add(this.gbSettings);
            this.Controls.Add(this.lWelcome);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FastM1nesweeperForm";
            this.Controls.SetChildIndex(this.lWelcome, 0);
            this.Controls.SetChildIndex(this.gbSettings, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.gbSettings.ResumeLayout(false);
            this.gbSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbRows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbColumns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBombs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lWelcome;
        private System.Windows.Forms.ComboBox cbDifficulty;
        private System.Windows.Forms.GroupBox gbSettings;
        private System.Windows.Forms.Label lColumns;
        private System.Windows.Forms.Label lRows;
        private System.Windows.Forms.Label lBombs;
        private System.Windows.Forms.NumericUpDown tbRows;
        private System.Windows.Forms.NumericUpDown tbColumns;
        private System.Windows.Forms.NumericUpDown tbBombs;
        private System.Windows.Forms.Label lDifficulty;
    }
}