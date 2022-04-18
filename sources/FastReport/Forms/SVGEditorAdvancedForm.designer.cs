using FastReport.Controls;
using System.Windows.Forms;

namespace FastReport.Forms
{
    partial class SVGEditorAdvancedForm
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnReset = new System.Windows.Forms.Button();
            this.panelTop = new System.Windows.Forms.Panel();
            this.gbColor = new System.Windows.Forms.GroupBox();
            this.rbGrayscale = new System.Windows.Forms.RadioButton();
            this.rbNone = new System.Windows.Forms.RadioButton();
            this.gbResize = new System.Windows.Forms.GroupBox();
            this.NUpDownWidth = new System.Windows.Forms.NumericUpDown();
            this.lblWidth = new System.Windows.Forms.Label();
            this.NUpDownHeight = new System.Windows.Forms.NumericUpDown();
            this.lblHeight = new System.Windows.Forms.Label();
            this.NUpDownMinX = new System.Windows.Forms.NumericUpDown();
            this.lblMinX = new System.Windows.Forms.Label();
            this.NUpDownMinY = new System.Windows.Forms.NumericUpDown();
            this.lblMinY = new System.Windows.Forms.Label();
            this.cbAspectRatio = new FastReport.Controls.ScalableCheckBox();
            this.panelMiddle = new System.Windows.Forms.Panel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.panelBottom.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.gbColor.SuspendLayout();
            this.gbResize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUpDownWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUpDownHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUpDownMinX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUpDownMinY)).BeginInit();
            this.panelMiddle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(705, 20);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(112, 35);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(988, 20);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 35);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panelBottom.Controls.Add(this.btnReset);
            this.panelBottom.Controls.Add(this.btnOK);
            this.panelBottom.Controls.Add(this.btnCancel);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 779);
            this.panelBottom.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1119, 74);
            this.panelBottom.TabIndex = 11;
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Location = new System.Drawing.Point(826, 20);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(153, 35);
            this.btnReset.TabIndex = 8;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // panelTop
            // 
            this.panelTop.AutoScroll = true;
            this.panelTop.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panelTop.Controls.Add(this.gbColor);
            this.panelTop.Controls.Add(this.gbResize);
            this.panelTop.Controls.Add(this.cbAspectRatio);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1119, 203);
            this.panelTop.TabIndex = 12;
            // 
            // gbColor
            // 
            this.gbColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbColor.Controls.Add(this.rbGrayscale);
            this.gbColor.Controls.Add(this.rbNone);
            this.gbColor.Location = new System.Drawing.Point(433, 18);
            this.gbColor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbColor.Name = "gbColor";
            this.gbColor.Padding = new System.Windows.Forms.Padding(15);
            this.gbColor.Size = new System.Drawing.Size(666, 130);
            this.gbColor.TabIndex = 19;
            this.gbColor.TabStop = false;
            this.gbColor.Text = "Color";
            // 
            // rbGrayscale
            // 
            this.rbGrayscale.AutoSize = true;
            this.rbGrayscale.Location = new System.Drawing.Point(20, 71);
            this.rbGrayscale.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rbGrayscale.Name = "rbGrayscale";
            this.rbGrayscale.Size = new System.Drawing.Size(72, 17);
            this.rbGrayscale.TabIndex = 5;
            this.rbGrayscale.Text = "Grayscale";
            this.rbGrayscale.UseVisualStyleBackColor = true;
            // 
            // rbNone
            // 
            this.rbNone.AutoSize = true;
            this.rbNone.Checked = true;
            this.rbNone.Location = new System.Drawing.Point(20, 35);
            this.rbNone.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rbNone.Name = "rbNone";
            this.rbNone.Size = new System.Drawing.Size(51, 17);
            this.rbNone.TabIndex = 4;
            this.rbNone.TabStop = true;
            this.rbNone.Text = "None";
            this.rbNone.UseVisualStyleBackColor = true;
            // 
            // gbResize
            // 
            this.gbResize.Controls.Add(this.NUpDownWidth);
            this.gbResize.Controls.Add(this.lblWidth);
            this.gbResize.Controls.Add(this.NUpDownHeight);
            this.gbResize.Controls.Add(this.lblHeight);
            this.gbResize.Controls.Add(this.NUpDownMinX);
            this.gbResize.Controls.Add(this.lblMinX);
            this.gbResize.Controls.Add(this.NUpDownMinY);
            this.gbResize.Controls.Add(this.lblMinY);
            this.gbResize.Location = new System.Drawing.Point(13, 18);
            this.gbResize.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbResize.Name = "gbResize";
            this.gbResize.Padding = new System.Windows.Forms.Padding(15);
            this.gbResize.Size = new System.Drawing.Size(395, 130);
            this.gbResize.TabIndex = 17;
            this.gbResize.TabStop = false;
            this.gbResize.Text = "ViewBox";
            // 
            // NUpDownWidth
            // 
            this.NUpDownWidth.Location = new System.Drawing.Point(283, 32);
            this.NUpDownWidth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.NUpDownWidth.Name = "NUpDownWidth";
            this.NUpDownWidth.Size = new System.Drawing.Size(93, 20);
            this.NUpDownWidth.TabIndex = 2;
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.Location = new System.Drawing.Point(215, 33);
            this.lblWidth.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(38, 13);
            this.lblWidth.TabIndex = 17;
            this.lblWidth.Text = "Width:";
            // 
            // NUpDownHeight
            // 
            this.NUpDownHeight.Location = new System.Drawing.Point(283, 77);
            this.NUpDownHeight.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.NUpDownHeight.Name = "NUpDownHeight";
            this.NUpDownHeight.Size = new System.Drawing.Size(93, 20);
            this.NUpDownHeight.TabIndex = 3;
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.Location = new System.Drawing.Point(215, 80);
            this.lblHeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(41, 13);
            this.lblHeight.TabIndex = 18;
            this.lblHeight.Text = "Height:";
            // 
            // NUpDownMinX
            // 
            this.NUpDownMinX.Location = new System.Drawing.Point(89, 31);
            this.NUpDownMinX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.NUpDownMinX.Name = "NUpDownMinX";
            this.NUpDownMinX.Size = new System.Drawing.Size(93, 20);
            this.NUpDownMinX.TabIndex = 0;
            // 
            // lblMinX
            // 
            this.lblMinX.AutoSize = true;
            this.lblMinX.Location = new System.Drawing.Point(17, 34);
            this.lblMinX.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMinX.Name = "lblMinX";
            this.lblMinX.Size = new System.Drawing.Size(34, 13);
            this.lblMinX.TabIndex = 12;
            this.lblMinX.Text = "MinX:";
            // 
            // NUpDownMinY
            // 
            this.NUpDownMinY.Location = new System.Drawing.Point(89, 77);
            this.NUpDownMinY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.NUpDownMinY.Name = "NUpDownMinY";
            this.NUpDownMinY.Size = new System.Drawing.Size(93, 20);
            this.NUpDownMinY.TabIndex = 1;
            // 
            // lblMinY
            // 
            this.lblMinY.AutoSize = true;
            this.lblMinY.Location = new System.Drawing.Point(17, 80);
            this.lblMinY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMinY.Name = "lblMinY";
            this.lblMinY.Size = new System.Drawing.Size(34, 13);
            this.lblMinY.TabIndex = 13;
            this.lblMinY.Text = "MinY:";
            // 
            // cbAspectRatio
            // 
            this.cbAspectRatio.Checked = true;
            this.cbAspectRatio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAspectRatio.Location = new System.Drawing.Point(13, 158);
            this.cbAspectRatio.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbAspectRatio.Name = "cbAspectRatio";
            this.cbAspectRatio.Size = new System.Drawing.Size(111, 24);
            this.cbAspectRatio.TabIndex = 6;
            this.cbAspectRatio.Text = "Keep aspect ratio";
            this.cbAspectRatio.UseVisualStyleBackColor = true;
            // 
            // panelMiddle
            // 
            this.panelMiddle.AutoScroll = true;
            this.panelMiddle.Controls.Add(this.pictureBox);
            this.panelMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMiddle.Location = new System.Drawing.Point(0, 203);
            this.panelMiddle.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelMiddle.Name = "panelMiddle";
            this.panelMiddle.Size = new System.Drawing.Size(1119, 576);
            this.panelMiddle.TabIndex = 13;
            // 
            // pictureBox
            // 
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(1119, 576);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // statusBar
            // 
            this.statusBar.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusBar.Location = new System.Drawing.Point(0, 853);
            this.statusBar.Name = "statusBar";
            this.statusBar.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusBar.Size = new System.Drawing.Size(1119, 22);
            this.statusBar.TabIndex = 14;
            this.statusBar.Text = "statusStrip1";
            // 
            // SVGEditorAdvancedForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1119, 875);
            this.Controls.Add(this.panelMiddle);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.statusBar);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "SVGEditorAdvancedForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Title";
            this.panelBottom.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.gbColor.ResumeLayout(false);
            this.gbColor.PerformLayout();
            this.gbResize.ResumeLayout(false);
            this.gbResize.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUpDownWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUpDownHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUpDownMinX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUpDownMinY)).EndInit();
            this.panelMiddle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.GroupBox gbColor;
        private System.Windows.Forms.GroupBox gbResize;
        private System.Windows.Forms.NumericUpDown NUpDownMinX;
        private System.Windows.Forms.Label lblMinX;
        private System.Windows.Forms.NumericUpDown NUpDownMinY;
        private System.Windows.Forms.Label lblMinY;
        private System.Windows.Forms.Panel panelMiddle;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.RadioButton rbGrayscale;
        private System.Windows.Forms.RadioButton rbNone;
        private NumericUpDown NUpDownWidth;
        private Label lblWidth;
        private NumericUpDown NUpDownHeight;
        private Label lblHeight;
        private ScalableCheckBox cbAspectRatio;
    }
}