using System;
using System.Windows.Forms;

namespace FastReport.Forms
{
    #if !COMMUNITY
    partial class ExportsOptionsEditorForm
    {
        class DoubleBufferedTreeView: TreeView
        {
            // Enabling DoobleBuffer to eliminate flickering
            protected override void OnHandleCreated(System.EventArgs e)
            {
                if (System.Environment.OSVersion.Version.Major >= 6)
                {
                    SendMessage(this.Handle, TVM_SETEXTENDEDSTYLE, (System.IntPtr)TVS_EX_DOUBLEBUFFER, (System.IntPtr)TVS_EX_DOUBLEBUFFER);
                }
                base.OnHandleCreated(e);
            }

            private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
            private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;
            private const int TVS_EX_DOUBLEBUFFER = 0x0004;

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            private static extern System.IntPtr SendMessage(System.IntPtr hWnd, int msg, System.IntPtr wp, System.IntPtr lp);

            // That is for handling double click on CheckBox
            protected override void WndProc(ref Message m)
            {
                // Suppress WM_LBUTTONDBLCLK
                if (m.Msg == 0x203) { m.Result = IntPtr.Zero; }
                else base.WndProc(ref m);
            }
        }

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
            this.tvExportsMenu = new FastReport.Forms.ExportsOptionsEditorForm.DoubleBufferedTreeView();
            this.gbExportsMenu = new System.Windows.Forms.GroupBox();
            this.btnDefaultSettings = new System.Windows.Forms.Button();
            this.gbExportsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(179, 308);
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(260, 308);
            // 
            // tvExportsMenu
            // 
            this.tvExportsMenu.AllowDrop = true;
            this.tvExportsMenu.CheckBoxes = true;
            this.tvExportsMenu.Indent = 22;
            this.tvExportsMenu.ItemHeight = 19;
            this.tvExportsMenu.Location = new System.Drawing.Point(6, 19);
            this.tvExportsMenu.Name = "tvExportsMenu";
            this.tvExportsMenu.ShowLines = false;
            this.tvExportsMenu.ShowPlusMinus = false;
            this.tvExportsMenu.ShowRootLines = false;
            this.tvExportsMenu.Size = new System.Drawing.Size(311, 264);
            this.tvExportsMenu.TabIndex = 1;
            this.tvExportsMenu.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvExportsMenu_AfterCheck);
            this.tvExportsMenu.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvExportsMenu_ItemDrag);
            this.tvExportsMenu.DragDrop += new System.Windows.Forms.DragEventHandler(this.tvExportsMenu_DragDrop);
            this.tvExportsMenu.DragEnter += new System.Windows.Forms.DragEventHandler(this.tvExportsMenu_DragEnter);
            this.tvExportsMenu.DragOver += new System.Windows.Forms.DragEventHandler(this.tvExportsMenu_DragOver);
            // 
            // gbExportsMenu
            // 
            this.gbExportsMenu.Controls.Add(this.tvExportsMenu);
            this.gbExportsMenu.Location = new System.Drawing.Point(12, 12);
            this.gbExportsMenu.Name = "gbExportsMenu";
            this.gbExportsMenu.Size = new System.Drawing.Size(323, 290);
            this.gbExportsMenu.TabIndex = 4;
            this.gbExportsMenu.TabStop = false;
            this.gbExportsMenu.Text = "Exports menu";
            // 
            // btnDefaultSettings
            // 
            this.btnDefaultSettings.Location = new System.Drawing.Point(74, 308);
            this.btnDefaultSettings.Name = "btnDefaultSettings";
            this.btnDefaultSettings.Size = new System.Drawing.Size(99, 23);
            this.btnDefaultSettings.TabIndex = 9;
            this.btnDefaultSettings.Text = "Defalut Settings";
            this.btnDefaultSettings.UseVisualStyleBackColor = true;
            this.btnDefaultSettings.Click += new System.EventHandler(this.btnDefaultSettings_Click);
            // 
            // ExportsOptionsEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(347, 343);
            this.Controls.Add(this.btnDefaultSettings);
            this.Controls.Add(this.gbExportsMenu);
            this.Name = "ExportsOptionsEditorForm";
            this.Text = "ExportsOptionsEditorForm";
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.gbExportsMenu, 0);
            this.Controls.SetChildIndex(this.btnDefaultSettings, 0);
            this.gbExportsMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

#endregion

        private DoubleBufferedTreeView tvExportsMenu;
        private GroupBox gbExportsMenu;
        private Button btnDefaultSettings;
    }
#endif
}