using FastReport.Controls;

namespace FastReport.Forms
{
    partial class FormsFonts
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
            this.buttonX1 = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.formsSetComboBox = new System.Windows.Forms.ComboBox();
            this.labelSettingsFor = new System.Windows.Forms.Label();
            this.defaultsButton = new System.Windows.Forms.Button();
            this.elementsListBox = new System.Windows.Forms.ListBox();
            this.formElementLabel = new System.Windows.Forms.Label();
            this.italicCheckBox = new FastReport.Controls.ScalableCheckBox();
            this.boldCheckBox = new FastReport.Controls.ScalableCheckBox();
            this.sizeComboBox = new System.Windows.Forms.ComboBox();
            this.sizeLabel = new System.Windows.Forms.Label();
            this.fontComboBox = new System.Windows.Forms.ComboBox();
            this.fontLabel = new System.Windows.Forms.Label();
            this.ExampleBox = new System.Windows.Forms.RichTextBox();
            this.sampleLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonX1
            // 
            this.buttonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1.Location = new System.Drawing.Point(0, 0);
            this.buttonX1.Name = "buttonX1";
            this.buttonX1.Size = new System.Drawing.Size(75, 23);
            this.buttonX1.TabIndex = 0;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(453, 281);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(358, 281);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // formsSetComboBox
            // 
            this.formsSetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.formsSetComboBox.FormattingEnabled = true;
            this.formsSetComboBox.Location = new System.Drawing.Point(21, 30);
            this.formsSetComboBox.Name = "formsSetComboBox";
            this.formsSetComboBox.Size = new System.Drawing.Size(339, 21);
            this.formsSetComboBox.TabIndex = 2;
            this.formsSetComboBox.SelectedIndexChanged += new System.EventHandler(this.FormSetComboBox_SelectedIndexChanged);
            // 
            // labelSettingsFor
            // 
            this.labelSettingsFor.AutoSize = true;
            this.labelSettingsFor.Location = new System.Drawing.Point(18, 14);
            this.labelSettingsFor.Name = "labelSettingsFor";
            this.labelSettingsFor.Size = new System.Drawing.Size(88, 13);
            this.labelSettingsFor.TabIndex = 3;
            this.labelSettingsFor.Text = "Show settings for";
            // 
            // defaultsButton
            // 
            this.defaultsButton.Location = new System.Drawing.Point(378, 29);
            this.defaultsButton.Name = "defaultsButton";
            this.defaultsButton.Size = new System.Drawing.Size(150, 23);
            this.defaultsButton.TabIndex = 4;
            this.defaultsButton.Text = "Use Defaults";
            this.defaultsButton.UseVisualStyleBackColor = true;
            this.defaultsButton.Click += new System.EventHandler(this.DefaultsButton_Click);
            // 
            // elementsListBox
            // 
            this.elementsListBox.FormattingEnabled = true;
            this.elementsListBox.Location = new System.Drawing.Point(21, 127);
            this.elementsListBox.Name = "elementsListBox";
            this.elementsListBox.Size = new System.Drawing.Size(289, 147);
            this.elementsListBox.TabIndex = 5;
            this.elementsListBox.SelectedIndexChanged += new System.EventHandler(this.ElementsListBox_SelectedIndexChanged);
            // 
            // formElementLabel
            // 
            this.formElementLabel.AutoSize = true;
            this.formElementLabel.Location = new System.Drawing.Point(18, 111);
            this.formElementLabel.Name = "formElementLabel";
            this.formElementLabel.Size = new System.Drawing.Size(68, 13);
            this.formElementLabel.TabIndex = 6;
            this.formElementLabel.Text = "Display items";
            // 
            // italicCheckBox
            // 
            this.italicCheckBox.Location = new System.Drawing.Point(329, 131);
            this.italicCheckBox.Name = "italicCheckBox";
            this.italicCheckBox.Size = new System.Drawing.Size(43, 17);
            this.italicCheckBox.TabIndex = 8;
            this.italicCheckBox.Text = "Italic";
            this.italicCheckBox.UseVisualStyleBackColor = true;
            this.italicCheckBox.CheckedChanged += new System.EventHandler(this.OptionChanged);
            // 
            // boldCheckBox
            // 
            this.boldCheckBox.Location = new System.Drawing.Point(329, 154);
            this.boldCheckBox.Name = "boldCheckBox";
            this.boldCheckBox.Size = new System.Drawing.Size(42, 17);
            this.boldCheckBox.TabIndex = 9;
            this.boldCheckBox.Text = "Bold";
            this.boldCheckBox.UseVisualStyleBackColor = true;
            this.boldCheckBox.CheckedChanged += new System.EventHandler(this.OptionChanged);
            // 
            // sizeComboBox
            // 
            this.sizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sizeComboBox.FormattingEnabled = true;
            this.sizeComboBox.Location = new System.Drawing.Point(378, 76);
            this.sizeComboBox.Name = "sizeComboBox";
            this.sizeComboBox.Size = new System.Drawing.Size(150, 21);
            this.sizeComboBox.TabIndex = 10;
            this.sizeComboBox.SelectedIndexChanged += new System.EventHandler(this.OptionChanged);
            // 
            // sizeLabel
            // 
            this.sizeLabel.AutoSize = true;
            this.sizeLabel.Location = new System.Drawing.Point(375, 60);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(27, 13);
            this.sizeLabel.TabIndex = 11;
            this.sizeLabel.Text = "Size";
            // 
            // fontComboBox
            // 
            this.fontComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontComboBox.FormattingEnabled = true;
            this.fontComboBox.Location = new System.Drawing.Point(21, 76);
            this.fontComboBox.Name = "fontComboBox";
            this.fontComboBox.Size = new System.Drawing.Size(339, 21);
            this.fontComboBox.TabIndex = 12;
            this.fontComboBox.SelectedIndexChanged += new System.EventHandler(this.OptionChanged);
            // 
            // fontLabel
            // 
            this.fontLabel.AutoSize = true;
            this.fontLabel.Location = new System.Drawing.Point(18, 60);
            this.fontLabel.Name = "fontLabel";
            this.fontLabel.Size = new System.Drawing.Size(28, 13);
            this.fontLabel.TabIndex = 13;
            this.fontLabel.Text = "Font";
            // 
            // ExampleBox
            // 
            this.ExampleBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ExampleBox.Enabled = false;
            this.ExampleBox.Location = new System.Drawing.Point(329, 202);
            this.ExampleBox.Multiline = false;
            this.ExampleBox.Name = "ExampleBox";
            this.ExampleBox.ReadOnly = true;
            this.ExampleBox.Size = new System.Drawing.Size(207, 72);
            this.ExampleBox.TabIndex = 14;
            this.ExampleBox.Text = "Fast Report";
            // 
            // sampleLabel
            // 
            this.sampleLabel.AutoSize = true;
            this.sampleLabel.Location = new System.Drawing.Point(326, 184);
            this.sampleLabel.Name = "sampleLabel";
            this.sampleLabel.Size = new System.Drawing.Size(42, 13);
            this.sampleLabel.TabIndex = 15;
            this.sampleLabel.Text = "Sample";
            // 
            // FormsFonts
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(548, 316);
            this.Controls.Add(this.sampleLabel);
            this.Controls.Add(this.ExampleBox);
            this.Controls.Add(this.fontLabel);
            this.Controls.Add(this.fontComboBox);
            this.Controls.Add(this.sizeLabel);
            this.Controls.Add(this.sizeComboBox);
            this.Controls.Add(this.boldCheckBox);
            this.Controls.Add(this.italicCheckBox);
            this.Controls.Add(this.formElementLabel);
            this.Controls.Add(this.elementsListBox);
            this.Controls.Add(this.defaultsButton);
            this.Controls.Add(this.labelSettingsFor);
            this.Controls.Add(this.formsSetComboBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormsFonts";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fonts Options Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonX1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ComboBox formsSetComboBox;
        private System.Windows.Forms.Label labelSettingsFor;
        private System.Windows.Forms.Button defaultsButton;
        private System.Windows.Forms.ListBox elementsListBox;
        private System.Windows.Forms.Label formElementLabel;
        private System.Windows.Forms.ComboBox sizeComboBox;
        private System.Windows.Forms.Label sizeLabel;
        private System.Windows.Forms.ComboBox fontComboBox;
        private System.Windows.Forms.Label fontLabel;
        private System.Windows.Forms.RichTextBox ExampleBox;
        private System.Windows.Forms.Label sampleLabel;
        private ScalableCheckBox italicCheckBox;
        private ScalableCheckBox boldCheckBox;
    }
}