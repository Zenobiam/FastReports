using System;
using FastReport.Forms;
namespace FastReport.Preview
{
  partial class PreviewControl
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;


    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnDesign = new System.Windows.Forms.ToolStripButton();
            this.btnPrint = new System.Windows.Forms.ToolStripButton();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEmail = new System.Windows.Forms.ToolStripButton();
            this.btnFind = new System.Windows.Forms.ToolStripButton();
            this.sep1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnZoomOut = new System.Windows.Forms.ToolStripButton();
            this.cbxZoom = new FastReport.Controls.FRToolStripComboBox();
            this.btnZoomIn = new System.Windows.Forms.ToolStripButton();
            this.sep2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnOutline = new System.Windows.Forms.ToolStripButton();
            this.btnPageSetup = new System.Windows.Forms.ToolStripButton();
            this.btnEdit = new System.Windows.Forms.ToolStripButton();
            this.btnWatermark = new System.Windows.Forms.ToolStripButton();
            this.sep3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnFirst = new System.Windows.Forms.ToolStripButton();
            this.btnPrior = new System.Windows.Forms.ToolStripButton();
            this.cbxPageNo = new System.Windows.Forms.ToolStripComboBox();
            this.lblTotalPages = new System.Windows.Forms.ToolStripLabel();
            this.btnNext = new System.Windows.Forms.ToolStripButton();
            this.btnLast = new System.Windows.Forms.ToolStripButton();
            this.sep4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnClose = new System.Windows.Forms.ToolStripButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblUrl = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblPerformance = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.outlineControl = new FastReport.Preview.OutlineControl();
            this.toolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnDesign,
            this.btnPrint,
            this.btnOpen,
            this.btnSave,
            this.btnEmail,
            this.btnFind,
            this.sep1,
            this.btnZoomOut,
            this.cbxZoom,
            this.btnZoomIn,
            this.sep2,
            this.btnOutline,
            this.btnPageSetup,
            this.btnEdit,
            this.btnWatermark,
            this.sep3,
            this.btnFirst,
            this.btnPrior,
            this.cbxPageNo,
            this.lblTotalPages,
            this.btnNext,
            this.btnLast,
            this.sep4,
            this.btnClose});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(3, 0, 1, 0);
            this.toolStrip.Size = new System.Drawing.Size(591, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // btnDesign
            // 
            this.btnDesign.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDesign.Name = "btnDesign";
            this.btnDesign.Size = new System.Drawing.Size(47, 22);
            this.btnDesign.Text = "Design";
            this.btnDesign.Click += new System.EventHandler(this.btnDesign_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(36, 22);
            this.btnPrint.Text = "Print";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(23, 22);
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSave
            // 
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(43, 22);
            this.btnSave.Text = "Save";
            // 
            // btnEmail
            // 
            this.btnEmail.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEmail.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEmail.Name = "btnEmail";
            this.btnEmail.Size = new System.Drawing.Size(23, 22);
            this.btnEmail.Click += new System.EventHandler(this.btnEmail_Click);
            // 
            // btnFind
            // 
            this.btnFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(23, 22);
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // sep1
            // 
            this.sep1.Name = "sep1";
            this.sep1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(23, 22);
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // cbxZoom
            // 
            this.cbxZoom.AutoSize = false;
            this.cbxZoom.DropDownWidth = 100;
            this.cbxZoom.Name = "cbxZoom";
            this.cbxZoom.Size = new System.Drawing.Size(58, 19);
            this.cbxZoom.SelectedIndexChanged += new System.EventHandler(this.cbxZoom_SelectedIndexChanged);
            this.cbxZoom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbxZoom_KeyDown);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(23, 22);
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // sep2
            // 
            this.sep2.Name = "sep2";
            this.sep2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnOutline
            // 
            this.btnOutline.CheckOnClick = true;
            this.btnOutline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOutline.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOutline.Name = "btnOutline";
            this.btnOutline.Size = new System.Drawing.Size(23, 22);
            this.btnOutline.Click += new System.EventHandler(this.btnOutline_Click);
            // 
            // btnPageSetup
            // 
            this.btnPageSetup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPageSetup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPageSetup.Name = "btnPageSetup";
            this.btnPageSetup.Size = new System.Drawing.Size(23, 22);
            this.btnPageSetup.Click += new System.EventHandler(this.btnPageSetup_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(23, 22);
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnWatermark
            // 
            this.btnWatermark.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnWatermark.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnWatermark.Name = "btnWatermark";
            this.btnWatermark.Size = new System.Drawing.Size(23, 22);
            this.btnWatermark.Text = "toolStripButton1";
            this.btnWatermark.Click += new System.EventHandler(this.btnWatermark_Click);
            // 
            // sep3
            // 
            this.sep3.Name = "sep3";
            this.sep3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnFirst
            // 
            this.btnFirst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFirst.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(23, 22);
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // btnPrior
            // 
            this.btnPrior.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPrior.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPrior.Name = "btnPrior";
            this.btnPrior.Size = new System.Drawing.Size(23, 22);
            this.btnPrior.Click += new System.EventHandler(this.btnPrior_Click);
            // 
            // cbxPageNo
            // 
            this.cbxPageNo.AutoSize = false;
            this.cbxPageNo.DropDownHeight = 150;
            this.cbxPageNo.DropDownWidth = 50;
            this.cbxPageNo.IntegralHeight = false;
            this.cbxPageNo.MaxDropDownItems = 16;
            this.cbxPageNo.Name = "cbxPageNo";
            this.cbxPageNo.Size = new System.Drawing.Size(58, 23);
            this.cbxPageNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbxPageNo_KeyDown);
            this.cbxPageNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbPageNo_KeyPress);
            this.cbxPageNo.MouseEnter += new System.EventHandler(this.cbxPageNo_Click);
            this.cbxPageNo.TextChanged += new System.EventHandler(this.cbxPageNo_TextChanged);
            // 
            // lblTotalPages
            // 
            this.lblTotalPages.Name = "lblTotalPages";
            this.lblTotalPages.Size = new System.Drawing.Size(27, 22);
            this.lblTotalPages.Text = "of 1";
            // 
            // btnNext
            // 
            this.btnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(23, 22);
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnLast
            // 
            this.btnLast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(23, 4);
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            // 
            // sep4
            // 
            this.sep4.Name = "sep4";
            this.sep4.Size = new System.Drawing.Size(6, 28);
            // 
            // btnClose
            // 
            this.btnClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(40, 19);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblUrl,
            this.lblPerformance});
            this.statusStrip.Location = new System.Drawing.Point(0, 258);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(591, 25);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = false;
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(200, 20);
            this.lblStatus.Text = "   ";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblUrl
            // 
            this.lblUrl.BackColor = System.Drawing.Color.Transparent;
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(13, 20);
            this.lblUrl.Text = "  ";
            // 
            // lblPerformance
            // 
            this.lblPerformance.AutoSize = false;
            this.lblPerformance.BackColor = System.Drawing.Color.Transparent;
            this.lblPerformance.Name = "lblPerformance";
            this.lblPerformance.Size = new System.Drawing.Size(363, 20);
            this.lblPerformance.Spring = true;
            this.lblPerformance.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.outlineControl);
            this.splitContainer1.Size = new System.Drawing.Size(591, 233);
            this.splitContainer1.SplitterDistance = 196;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 2;
            // 
            // outlineControl
            // 
            this.outlineControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outlineControl.Location = new System.Drawing.Point(0, 0);
            this.outlineControl.Name = "outlineControl";
            this.outlineControl.Size = new System.Drawing.Size(196, 233);
            this.outlineControl.TabIndex = 0;
            // 
            // PreviewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Name = "PreviewControl";
            this.Size = new System.Drawing.Size(591, 283);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

        #endregion

    private System.Windows.Forms.ToolStrip toolStrip;
    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.ToolStripButton btnPrint;
    private System.Windows.Forms.ToolStripButton btnOpen;
    private System.Windows.Forms.ToolStripButton btnFind;
    private System.Windows.Forms.ToolStripSeparator sep1;
    private System.Windows.Forms.ToolStripButton btnZoomOut;
    private FastReport.Controls.FRToolStripComboBox cbxZoom;
    private System.Windows.Forms.ToolStripButton btnZoomIn;
    private System.Windows.Forms.ToolStripSeparator sep2;
    private System.Windows.Forms.ToolStripButton btnOutline;
    private System.Windows.Forms.ToolStripButton btnEdit;
    private System.Windows.Forms.ToolStripSeparator sep3;
    private System.Windows.Forms.ToolStripButton btnFirst;
    private System.Windows.Forms.ToolStripButton btnPrior;
    private System.Windows.Forms.ToolStripButton btnNext;
    private System.Windows.Forms.ToolStripButton btnLast;
    private System.Windows.Forms.ToolStripComboBox cbxPageNo;
    private System.Windows.Forms.ToolStripButton btnPageSetup;
    private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    private System.Windows.Forms.ToolStripButton btnWatermark;
    private System.Windows.Forms.ToolStripStatusLabel lblUrl;
    private System.Windows.Forms.ToolStripMenuItem btnSave;
    private System.Windows.Forms.ToolStripStatusLabel lblPerformance;
    private System.Windows.Forms.ToolStripSeparator sep4;
    private System.Windows.Forms.ToolStripButton btnClose;
    private System.Windows.Forms.ToolStripLabel lblTotalPages;
    private System.Windows.Forms.ToolStripButton btnEmail;
    private System.Windows.Forms.ToolStripButton btnDesign;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private OutlineControl outlineControl;
  }
}
