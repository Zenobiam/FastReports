namespace FastReport.Forms
{
    partial class MessageBoxWithEditorForm
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
            this.tbMessage = new System.Windows.Forms.TextBox();
            this.lbDescription = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(248, 267);
            this.btnOk.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(161, 267);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnCancel.Visible = false;
            // 
            // tbMessage
            // 
            this.tbMessage.AcceptsReturn = true;
            this.tbMessage.Location = new System.Drawing.Point(10, 52);
            this.tbMessage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbMessage.Multiline = true;
            this.tbMessage.Name = "tbMessage";
            this.tbMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbMessage.Size = new System.Drawing.Size(314, 206);
            this.tbMessage.TabIndex = 1;
            // 
            // lbDescription
            // 
            this.lbDescription.Location = new System.Drawing.Point(10, 13);
            this.lbDescription.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbDescription.Name = "lbDescription";
            this.lbDescription.Size = new System.Drawing.Size(313, 35);
            this.lbDescription.TabIndex = 2;
            this.lbDescription.Text = "Replace \'report1.fpx report2.fpx ... reportN.fpx\' with your own reports and copy " +
    "the following comands:";
            // 
            // MessageBoxWithEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(333, 301);
            this.Controls.Add(this.lbDescription);
            this.Controls.Add(this.tbMessage);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "MessageBoxWithEditorForm";
            this.Text = "MessageBoxWithEditorForm";
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.tbMessage, 0);
            this.Controls.SetChildIndex(this.lbDescription, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbMessage;
        private System.Windows.Forms.Label lbDescription;
    }
}