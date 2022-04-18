namespace FastReport.Gauge
{
  partial class GaugeEditorForm
    {
    #region Fields

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

        #endregion // Fields

        internal Controls.PageControlPage PgGeneral { get { return pgGeneral; } }

    #region Protected Methods

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing"><b>true</b> if managed resources should be disposed. Otherwise, <b>false</b>.</param>
        protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #endregion // Protected Methods

    #region Windows Forms Designer Generated Code

    private void InitializeComponent()
    {
            this.colorComboBox4 = new FastReport.Controls.ColorComboBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.pgPointer = new FastReport.Controls.PageControlPage();
            this.btnPointerFill = new System.Windows.Forms.Button();
            this.lblPtrBorderColor = new System.Windows.Forms.Label();
            this.cbxPointerBorderColor = new FastReport.Controls.ColorComboBox();
            this.pgScale = new FastReport.Controls.PageControlPage();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.gpBoxMnr = new System.Windows.Forms.GroupBox();
            this.numericUpDowMinorWidth = new System.Windows.Forms.NumericUpDown();
            this.cbxMinorTicksColor = new FastReport.Controls.ColorComboBox();
            this.lblColorMnr = new System.Windows.Forms.Label();
            this.lblWidthMnr = new System.Windows.Forms.Label();
            this.gpBoxMjr = new System.Windows.Forms.GroupBox();
            this.cbxMajorTicksColor = new FastReport.Controls.ColorComboBox();
            this.numericUpDowMajorWidth = new System.Windows.Forms.NumericUpDown();
            this.lblColorMjr = new System.Windows.Forms.Label();
            this.lblWidthMjr = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.buttonFill = new System.Windows.Forms.Button();
            this.lblScaleFont = new System.Windows.Forms.Label();
            this.textBoxButtonScaleFont = new FastReport.Controls.TextBoxButton();
            this.pageControl1 = new FastReport.Controls.PageControl();
            this.pgGeneral = new FastReport.Controls.PageControlPage();
            this.btnExpression = new System.Windows.Forms.Button();
            this.btnGeneralFill = new System.Windows.Forms.Button();
            this.btnGeneralBorder = new System.Windows.Forms.Button();
            this.pgLabel = new FastReport.Controls.PageControlPage();
            this.tbLabelText = new System.Windows.Forms.TextBox();
            this.lblLabelText = new System.Windows.Forms.Label();
            this.lblLabelFont = new System.Windows.Forms.Label();
            this.textBoxButtonLabelFont = new FastReport.Controls.TextBoxButton();
            this.lblLblColor = new System.Windows.Forms.Label();
            this.cbLabelColor = new FastReport.Controls.ColorComboBox();
            this.pgPointer.SuspendLayout();
            this.pgScale.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.gpBoxMnr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDowMinorWidth)).BeginInit();
            this.gpBoxMjr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDowMajorWidth)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.pageControl1.SuspendLayout();
            this.pgGeneral.SuspendLayout();
            this.pgLabel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(276, 402);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(359, 402);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            // 
            // colorComboBox4
            // 
            this.colorComboBox4.Color = System.Drawing.Color.Transparent;
            this.colorComboBox4.Location = new System.Drawing.Point(190, 68);
            this.colorComboBox4.Margin = new System.Windows.Forms.Padding(4);
            this.colorComboBox4.Name = "colorComboBox4";
            this.colorComboBox4.ShowColorName = true;
            this.colorComboBox4.Size = new System.Drawing.Size(196, 21);
            this.colorComboBox4.TabIndex = 30;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(190, 30);
            this.textBox4.Margin = new System.Windows.Forms.Padding(4);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(196, 20);
            this.textBox4.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 72);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 20);
            this.label1.TabIndex = 31;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 33);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(56, 19);
            this.label10.TabIndex = 15;
            // 
            // pgPointer
            // 
            this.pgPointer.BackColor = System.Drawing.SystemColors.Window;
            this.pgPointer.Controls.Add(this.btnPointerFill);
            this.pgPointer.Controls.Add(this.lblPtrBorderColor);
            this.pgPointer.Controls.Add(this.cbxPointerBorderColor);
            this.pgPointer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgPointer.Location = new System.Drawing.Point(120, 1);
            this.pgPointer.Name = "pgPointer";
            this.pgPointer.Size = new System.Drawing.Size(302, 381);
            this.pgPointer.TabIndex = 1;
            this.pgPointer.Text = "Pointer";
            // 
            // btnPointerFill
            // 
            this.btnPointerFill.Location = new System.Drawing.Point(151, 46);
            this.btnPointerFill.Name = "btnPointerFill";
            this.btnPointerFill.Size = new System.Drawing.Size(132, 23);
            this.btnPointerFill.TabIndex = 1;
            this.btnPointerFill.Text = "Fill...";
            this.btnPointerFill.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPointerFill.UseVisualStyleBackColor = true;
            this.btnPointerFill.Click += new System.EventHandler(this.btnPointerFill_Click);
            // 
            // lblPtrBorderColor
            // 
            this.lblPtrBorderColor.AutoSize = true;
            this.lblPtrBorderColor.Location = new System.Drawing.Point(35, 20);
            this.lblPtrBorderColor.Name = "lblPtrBorderColor";
            this.lblPtrBorderColor.Size = new System.Drawing.Size(69, 13);
            this.lblPtrBorderColor.TabIndex = 16;
            this.lblPtrBorderColor.Text = "Border color:";
            // 
            // cbxPointerBorderColor
            // 
            this.cbxPointerBorderColor.Color = System.Drawing.Color.Transparent;
            this.cbxPointerBorderColor.Location = new System.Drawing.Point(151, 16);
            this.cbxPointerBorderColor.Name = "cbxPointerBorderColor";
            this.cbxPointerBorderColor.ShowColorName = true;
            this.cbxPointerBorderColor.Size = new System.Drawing.Size(132, 21);
            this.cbxPointerBorderColor.TabIndex = 17;
            // 
            // pgScale
            // 
            this.pgScale.BackColor = System.Drawing.SystemColors.Window;
            this.pgScale.Controls.Add(this.tabControl3);
            this.pgScale.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgScale.Location = new System.Drawing.Point(120, 1);
            this.pgScale.Name = "pgScale";
            this.pgScale.Size = new System.Drawing.Size(302, 381);
            this.pgScale.TabIndex = 0;
            this.pgScale.Text = "Scale";
            // 
            // tabControl3
            // 
            this.tabControl3.Controls.Add(this.tabPage2);
            this.tabControl3.Controls.Add(this.tabPage4);
            this.tabControl3.Location = new System.Drawing.Point(5, 8);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 1;
            this.tabControl3.Size = new System.Drawing.Size(284, 371);
            this.tabControl3.TabIndex = 15;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.gpBoxMnr);
            this.tabPage2.Controls.Add(this.gpBoxMjr);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(276, 345);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "Ticks";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // gpBoxMnr
            // 
            this.gpBoxMnr.Controls.Add(this.numericUpDowMinorWidth);
            this.gpBoxMnr.Controls.Add(this.cbxMinorTicksColor);
            this.gpBoxMnr.Controls.Add(this.lblColorMnr);
            this.gpBoxMnr.Controls.Add(this.lblWidthMnr);
            this.gpBoxMnr.Location = new System.Drawing.Point(5, 102);
            this.gpBoxMnr.Margin = new System.Windows.Forms.Padding(2);
            this.gpBoxMnr.Name = "gpBoxMnr";
            this.gpBoxMnr.Padding = new System.Windows.Forms.Padding(2);
            this.gpBoxMnr.Size = new System.Drawing.Size(263, 89);
            this.gpBoxMnr.TabIndex = 33;
            this.gpBoxMnr.TabStop = false;
            this.gpBoxMnr.Text = "Minor Ticks";
            // 
            // numericUpDowMinorWidth
            // 
            this.numericUpDowMinorWidth.Location = new System.Drawing.Point(127, 21);
            this.numericUpDowMinorWidth.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDowMinorWidth.Name = "numericUpDowMinorWidth";
            this.numericUpDowMinorWidth.Size = new System.Drawing.Size(131, 20);
            this.numericUpDowMinorWidth.TabIndex = 35;
            // 
            // cbxMinorTicksColor
            // 
            this.cbxMinorTicksColor.Color = System.Drawing.Color.Transparent;
            this.cbxMinorTicksColor.Location = new System.Drawing.Point(127, 45);
            this.cbxMinorTicksColor.Name = "cbxMinorTicksColor";
            this.cbxMinorTicksColor.ShowColorName = true;
            this.cbxMinorTicksColor.Size = new System.Drawing.Size(131, 21);
            this.cbxMinorTicksColor.TabIndex = 30;
            // 
            // lblColorMnr
            // 
            this.lblColorMnr.AutoSize = true;
            this.lblColorMnr.Location = new System.Drawing.Point(11, 48);
            this.lblColorMnr.Name = "lblColorMnr";
            this.lblColorMnr.Size = new System.Drawing.Size(36, 13);
            this.lblColorMnr.TabIndex = 31;
            this.lblColorMnr.Text = "Color:";
            // 
            // lblWidthMnr
            // 
            this.lblWidthMnr.AutoSize = true;
            this.lblWidthMnr.Location = new System.Drawing.Point(11, 22);
            this.lblWidthMnr.Name = "lblWidthMnr";
            this.lblWidthMnr.Size = new System.Drawing.Size(39, 13);
            this.lblWidthMnr.TabIndex = 15;
            this.lblWidthMnr.Text = "Width:";
            // 
            // gpBoxMjr
            // 
            this.gpBoxMjr.Controls.Add(this.cbxMajorTicksColor);
            this.gpBoxMjr.Controls.Add(this.numericUpDowMajorWidth);
            this.gpBoxMjr.Controls.Add(this.lblColorMjr);
            this.gpBoxMjr.Controls.Add(this.lblWidthMjr);
            this.gpBoxMjr.Location = new System.Drawing.Point(5, 9);
            this.gpBoxMjr.Margin = new System.Windows.Forms.Padding(2);
            this.gpBoxMjr.Name = "gpBoxMjr";
            this.gpBoxMjr.Padding = new System.Windows.Forms.Padding(2);
            this.gpBoxMjr.Size = new System.Drawing.Size(263, 89);
            this.gpBoxMjr.TabIndex = 32;
            this.gpBoxMjr.TabStop = false;
            this.gpBoxMjr.Text = "Major Ticks";
            // 
            // cbxMajorTicksColor
            // 
            this.cbxMajorTicksColor.Color = System.Drawing.Color.Transparent;
            this.cbxMajorTicksColor.Location = new System.Drawing.Point(127, 45);
            this.cbxMajorTicksColor.Name = "cbxMajorTicksColor";
            this.cbxMajorTicksColor.ShowColorName = true;
            this.cbxMajorTicksColor.Size = new System.Drawing.Size(131, 21);
            this.cbxMajorTicksColor.TabIndex = 30;
            // 
            // numericUpDowMajorWidth
            // 
            this.numericUpDowMajorWidth.Location = new System.Drawing.Point(127, 21);
            this.numericUpDowMajorWidth.Margin = new System.Windows.Forms.Padding(2);
            this.numericUpDowMajorWidth.Name = "numericUpDowMajorWidth";
            this.numericUpDowMajorWidth.Size = new System.Drawing.Size(131, 20);
            this.numericUpDowMajorWidth.TabIndex = 34;
            // 
            // lblColorMjr
            // 
            this.lblColorMjr.AutoSize = true;
            this.lblColorMjr.Location = new System.Drawing.Point(11, 48);
            this.lblColorMjr.Name = "lblColorMjr";
            this.lblColorMjr.Size = new System.Drawing.Size(36, 13);
            this.lblColorMjr.TabIndex = 31;
            this.lblColorMjr.Text = "Color:";
            // 
            // lblWidthMjr
            // 
            this.lblWidthMjr.AutoSize = true;
            this.lblWidthMjr.Location = new System.Drawing.Point(11, 22);
            this.lblWidthMjr.Name = "lblWidthMjr";
            this.lblWidthMjr.Size = new System.Drawing.Size(39, 13);
            this.lblWidthMjr.TabIndex = 15;
            this.lblWidthMjr.Text = "Width:";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.buttonFill);
            this.tabPage4.Controls.Add(this.lblScaleFont);
            this.tabPage4.Controls.Add(this.textBoxButtonScaleFont);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(276, 345);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Text";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // buttonFill
            // 
            this.buttonFill.Location = new System.Drawing.Point(61, 43);
            this.buttonFill.Name = "buttonFill";
            this.buttonFill.Size = new System.Drawing.Size(199, 23);
            this.buttonFill.TabIndex = 15;
            this.buttonFill.Text = "Text fill...";
            this.buttonFill.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonFill.UseVisualStyleBackColor = true;
            this.buttonFill.Click += new System.EventHandler(this.buttonFill_Click);
            // 
            // lblScaleFont
            // 
            this.lblScaleFont.AutoSize = true;
            this.lblScaleFont.Location = new System.Drawing.Point(12, 20);
            this.lblScaleFont.Name = "lblScaleFont";
            this.lblScaleFont.Size = new System.Drawing.Size(33, 13);
            this.lblScaleFont.TabIndex = 13;
            this.lblScaleFont.Text = "Font:";
            // 
            // textBoxButtonScaleFont
            // 
            this.textBoxButtonScaleFont.Image = null;
            this.textBoxButtonScaleFont.Location = new System.Drawing.Point(61, 16);
            this.textBoxButtonScaleFont.Name = "textBoxButtonScaleFont";
            this.textBoxButtonScaleFont.Size = new System.Drawing.Size(199, 21);
            this.textBoxButtonScaleFont.TabIndex = 14;
            this.textBoxButtonScaleFont.ButtonClick += new System.EventHandler(this.textBoxButtonScaleFont_ButtonClick);
            // 
            // pageControl1
            // 
            this.pageControl1.Controls.Add(this.pgGeneral);
            this.pageControl1.Controls.Add(this.pgScale);
            this.pageControl1.Controls.Add(this.pgPointer);
            this.pageControl1.Controls.Add(this.pgLabel);
            this.pageControl1.HighlightPageIndex = -1;
            this.pageControl1.Location = new System.Drawing.Point(12, 12);
            this.pageControl1.Name = "pageControl1";
            this.pageControl1.SelectorWidth = 120;
            this.pageControl1.Size = new System.Drawing.Size(423, 383);
            this.pageControl1.TabIndex = 1;
            this.pageControl1.Text = "pageControl1";
            // 
            // pgGeneral
            // 
            this.pgGeneral.BackColor = System.Drawing.SystemColors.Window;
            this.pgGeneral.Controls.Add(this.btnExpression);
            this.pgGeneral.Controls.Add(this.btnGeneralFill);
            this.pgGeneral.Controls.Add(this.btnGeneralBorder);
            this.pgGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgGeneral.Location = new System.Drawing.Point(120, 1);
            this.pgGeneral.Name = "pgGeneral";
            this.pgGeneral.Size = new System.Drawing.Size(302, 381);
            this.pgGeneral.TabIndex = 1;
            this.pgGeneral.Text = "General";
            // 
            // btnExpression
            // 
            this.btnExpression.Location = new System.Drawing.Point(21, 8);
            this.btnExpression.Name = "btnExpression";
            this.btnExpression.Size = new System.Drawing.Size(262, 23);
            this.btnExpression.TabIndex = 4;
            this.btnExpression.Text = "Expression...";
            this.btnExpression.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExpression.UseVisualStyleBackColor = true;
            this.btnExpression.Click += new System.EventHandler(this.btnExpression_Click);
            // 
            // btnGeneralFill
            // 
            this.btnGeneralFill.Location = new System.Drawing.Point(163, 41);
            this.btnGeneralFill.Name = "btnGeneralFill";
            this.btnGeneralFill.Size = new System.Drawing.Size(120, 23);
            this.btnGeneralFill.TabIndex = 2;
            this.btnGeneralFill.Text = "Fill...";
            this.btnGeneralFill.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGeneralFill.UseVisualStyleBackColor = true;
            this.btnGeneralFill.Click += new System.EventHandler(this.btnGeneralFill_Click);
            // 
            // btnGeneralBorder
            // 
            this.btnGeneralBorder.Location = new System.Drawing.Point(21, 41);
            this.btnGeneralBorder.Name = "btnGeneralBorder";
            this.btnGeneralBorder.Size = new System.Drawing.Size(120, 23);
            this.btnGeneralBorder.TabIndex = 3;
            this.btnGeneralBorder.Text = "Border...";
            this.btnGeneralBorder.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGeneralBorder.UseVisualStyleBackColor = true;
            this.btnGeneralBorder.Click += new System.EventHandler(this.btnGeneralBorder_Click);
            // 
            // pgLabel
            // 
            this.pgLabel.BackColor = System.Drawing.SystemColors.Window;
            this.pgLabel.Controls.Add(this.tbLabelText);
            this.pgLabel.Controls.Add(this.lblLabelText);
            this.pgLabel.Controls.Add(this.lblLabelFont);
            this.pgLabel.Controls.Add(this.textBoxButtonLabelFont);
            this.pgLabel.Controls.Add(this.lblLblColor);
            this.pgLabel.Controls.Add(this.cbLabelColor);
            this.pgLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgLabel.Location = new System.Drawing.Point(120, 1);
            this.pgLabel.Name = "pgLabel";
            this.pgLabel.Size = new System.Drawing.Size(302, 381);
            this.pgLabel.TabIndex = 2;
            this.pgLabel.Text = "Label";
            // 
            // tbLabelText
            // 
            this.tbLabelText.Location = new System.Drawing.Point(123, 75);
            this.tbLabelText.Margin = new System.Windows.Forms.Padding(2);
            this.tbLabelText.Name = "tbLabelText";
            this.tbLabelText.Size = new System.Drawing.Size(161, 20);
            this.tbLabelText.TabIndex = 21;
            // 
            // lblLabelText
            // 
            this.lblLabelText.AutoSize = true;
            this.lblLabelText.Location = new System.Drawing.Point(35, 80);
            this.lblLabelText.Name = "lblLabelText";
            this.lblLabelText.Size = new System.Drawing.Size(33, 13);
            this.lblLabelText.TabIndex = 20;
            this.lblLabelText.Text = "Text:";
            // 
            // lblLabelFont
            // 
            this.lblLabelFont.AutoSize = true;
            this.lblLabelFont.Location = new System.Drawing.Point(35, 51);
            this.lblLabelFont.Name = "lblLabelFont";
            this.lblLabelFont.Size = new System.Drawing.Size(33, 13);
            this.lblLabelFont.TabIndex = 18;
            this.lblLabelFont.Text = "Font:";
            // 
            // textBoxButtonLabelFont
            // 
            this.textBoxButtonLabelFont.Image = null;
            this.textBoxButtonLabelFont.Location = new System.Drawing.Point(123, 47);
            this.textBoxButtonLabelFont.Name = "textBoxButtonLabelFont";
            this.textBoxButtonLabelFont.Size = new System.Drawing.Size(159, 21);
            this.textBoxButtonLabelFont.TabIndex = 19;
            this.textBoxButtonLabelFont.ButtonClick += new System.EventHandler(this.textBoxButtonLabelFont_ButtonClick);
            // 
            // lblLblColor
            // 
            this.lblLblColor.AutoSize = true;
            this.lblLblColor.Location = new System.Drawing.Point(35, 20);
            this.lblLblColor.Name = "lblLblColor";
            this.lblLblColor.Size = new System.Drawing.Size(62, 13);
            this.lblLblColor.TabIndex = 16;
            this.lblLblColor.Text = "Label color:";
            // 
            // cbLabelColor
            // 
            this.cbLabelColor.Color = System.Drawing.Color.Transparent;
            this.cbLabelColor.Location = new System.Drawing.Point(123, 16);
            this.cbLabelColor.Name = "cbLabelColor";
            this.cbLabelColor.ShowColorName = true;
            this.cbLabelColor.Size = new System.Drawing.Size(159, 21);
            this.cbLabelColor.TabIndex = 17;
            // 
            // GaugeEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(447, 435);
            this.Controls.Add(this.pageControl1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "GaugeEditorForm";
            this.Text = "Edit Gauge";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GaugeEditorForm_FormClosing);
            this.Controls.SetChildIndex(this.pageControl1, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.pgPointer.ResumeLayout(false);
            this.pgPointer.PerformLayout();
            this.pgScale.ResumeLayout(false);
            this.tabControl3.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.gpBoxMnr.ResumeLayout(false);
            this.gpBoxMnr.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDowMinorWidth)).EndInit();
            this.gpBoxMjr.ResumeLayout(false);
            this.gpBoxMjr.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDowMajorWidth)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.pageControl1.ResumeLayout(false);
            this.pgGeneral.ResumeLayout(false);
            this.pgLabel.ResumeLayout(false);
            this.pgLabel.PerformLayout();
            this.ResumeLayout(false);

    }


        #endregion // Windows Forms Designer Generated Code

        private Controls.ColorComboBox colorComboBox4;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label10;
        private Controls.ColorComboBox cbxMinorTicksColor;
        private System.Windows.Forms.Label lblColorMnr;
        private System.Windows.Forms.Label lblWidthMnr;
        private Controls.ColorComboBox cbxMajorTicksColor;
        private System.Windows.Forms.Label lblColorMjr;
        private System.Windows.Forms.Label lblWidthMjr;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label lblScaleFont;
        private Controls.TextBoxButton textBoxButtonScaleFont;
        private System.Windows.Forms.NumericUpDown numericUpDowMinorWidth;
        private System.Windows.Forms.NumericUpDown numericUpDowMajorWidth;
        private System.Windows.Forms.Button buttonFill;
        internal Controls.PageControlPage pgGeneral;
        internal System.Windows.Forms.TabControl tabControl3;
        internal Controls.PageControl pageControl1;
        internal Controls.PageControlPage pgScale;
        internal System.Windows.Forms.TabPage tabPage2;
        internal System.Windows.Forms.TextBox tbLabelText;
        internal System.Windows.Forms.Label lblLabelText;
        internal System.Windows.Forms.Label lblLabelFont;
        internal Controls.TextBoxButton textBoxButtonLabelFont;
        internal System.Windows.Forms.Label lblLblColor;
        internal Controls.ColorComboBox cbLabelColor;
        internal Controls.PageControlPage pgLabel;
        internal System.Windows.Forms.Label lblPtrBorderColor;
        internal System.Windows.Forms.Button btnPointerFill;
        internal Controls.PageControlPage pgPointer;
        internal Controls.ColorComboBox cbxPointerBorderColor;
        internal System.Windows.Forms.Button btnGeneralFill;
        private System.Windows.Forms.Button btnExpression;
        internal System.Windows.Forms.Button btnGeneralBorder;
        internal System.Windows.Forms.GroupBox gpBoxMnr;
        internal System.Windows.Forms.GroupBox gpBoxMjr;
    }
}