namespace FastReport.Forms
{
    partial class DataViewForm
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
            this.stStatus = new System.Windows.Forms.StatusStrip();
            this.lblRows = new System.Windows.Forms.Label();
            this.btnPrior = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnLast = new System.Windows.Forms.Button();
            this.btnFirst = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // stStatus
            // 
            this.stStatus.AutoSize = false;
            this.stStatus.Location = new System.Drawing.Point(0, 336);
            this.stStatus.Name = "stStatus";
            this.stStatus.Size = new System.Drawing.Size(584, 25);
            this.stStatus.TabIndex = 0;
            // 
            // lblRows
            // 
            this.lblRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRows.AutoSize = true;
            this.lblRows.Location = new System.Drawing.Point(12, 343);
            this.lblRows.Name = "lblRows";
            this.lblRows.Size = new System.Drawing.Size(44, 13);
            this.lblRows.TabIndex = 1;
            this.lblRows.Text = "0 row(s)";
            // 
            // btnPrior
            // 
            this.btnPrior.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrior.Location = new System.Drawing.Point(431, 338);
            this.btnPrior.Name = "btnPrior";
            this.btnPrior.Size = new System.Drawing.Size(40, 23);
            this.btnPrior.TabIndex = 2;
            this.btnPrior.TabStop = false;
            this.btnPrior.Text = "Prior";
            this.btnPrior.UseVisualStyleBackColor = true;
            this.btnPrior.Click += new System.EventHandler(this.btnPrior_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(477, 338);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(40, 23);
            this.btnNext.TabIndex = 3;
            this.btnNext.TabStop = false;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnLast
            // 
            this.btnLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLast.Location = new System.Drawing.Point(523, 338);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(40, 23);
            this.btnLast.TabIndex = 4;
            this.btnLast.TabStop = false;
            this.btnLast.Text = "Last";
            this.btnLast.UseVisualStyleBackColor = true;
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            // 
            // btnFirst
            // 
            this.btnFirst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFirst.Location = new System.Drawing.Point(385, 338);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(40, 23);
            this.btnFirst.TabIndex = 5;
            this.btnFirst.TabStop = false;
            this.btnFirst.Text = "First";
            this.btnFirst.UseVisualStyleBackColor = true;
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // DataViewForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.btnFirst);
            this.Controls.Add(this.btnLast);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPrior);
            this.Controls.Add(this.lblRows);
            this.Controls.Add(this.stStatus);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "DataViewForm";
            this.Text = "View Data";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DataViewForm_FormClosed);
            this.Shown += new System.EventHandler(this.DataViewForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip stStatus;
        private System.Windows.Forms.Label lblRows;
        private System.Windows.Forms.Button btnPrior;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnLast;
        private System.Windows.Forms.Button btnFirst;
    }
}