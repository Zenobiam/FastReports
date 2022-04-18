using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents.DotNetBar;
#endif

namespace FastReport.Design.Toolbars
{
    internal class StandardToolbar : ToolbarBase
    {
        #region Fields
#if !MONO
        public ButtonItem btnNew;
        public ButtonItem btnOpen;
        public ButtonItem btnSave;
        public ButtonItem btnSaveAll;
        public ButtonItem btnPreview;
        public ButtonItem btnNewPage;
        public ButtonItem btnNewDialog;
        public ButtonItem btnCopyPage;
        public ButtonItem btnDeletePage;
        public ButtonItem btnPageSetup;
        public ButtonItem btnCut;
        public ButtonItem btnCopy;
        public ButtonItem btnPaste;
        public ButtonItem btnFormatPainter;
        public ButtonItem btnUndo;
        public ButtonItem btnRedo;
        public ButtonItem btnGroup;
        public ButtonItem btnUngroup;
        private UndoDropDown undoDropDown;
        private RedoDropDown redoDropDown;
        private Timer clipboardTimer;
        private Timer previewTimer;
#else
        public ToolStripButton btnNew;
        public ToolStripSplitButton btnOpen;
        public ToolStripButton btnSave;
        public ToolStripButton btnSaveAll;
        public ToolStripButton btnPreview;
        public ToolStripButton btnNewPage;
        public ToolStripButton btnNewDialog;
        public ToolStripButton btnCopyPage;
        public ToolStripButton btnDeletePage;
        public ToolStripButton btnPageSetup;
        public ToolStripButton btnCut;
        public ToolStripButton btnCopy;
        public ToolStripButton btnPaste;
        public ToolStripButton btnFormatPainter;
        public ToolStripSplitButton btnUndo;
        public ToolStripSplitButton btnRedo;
        public ToolStripButton btnGroup;
        public ToolStripButton btnUngroup;
        private UndoDropDown undoDropDown;
        private RedoDropDown redoDropDown;
        private Timer clipboardTimer;
        private Timer previewTimer;
#endif
        #endregion

        #region Private Methods
        private void UpdateControls()
        {
            btnNew.Enabled = Designer.cmdNew.Enabled;
            btnOpen.Enabled = Designer.cmdOpen.Enabled;
            btnSave.Enabled = Designer.cmdSave.Enabled;
            btnSaveAll.Enabled = Designer.cmdSaveAll.Enabled;
            btnPreview.Enabled = Designer.cmdPreview.Enabled;
            btnNewPage.Enabled = Designer.cmdNewPage.Enabled;
#if COMMUNITY
      btnNewDialog.Enabled = false;
#else
            btnNewDialog.Enabled = Designer.cmdNewDialog.Enabled;
#endif
            btnCopyPage.Enabled = Designer.cmdCopyPage.Enabled;
            btnDeletePage.Enabled = Designer.cmdDeletePage.Enabled;
            btnPageSetup.Enabled = Designer.cmdPageSetup.Enabled;
            btnCut.Enabled = Designer.cmdCut.Enabled;
            btnCopy.Enabled = Designer.cmdCopy.Enabled;
            btnFormatPainter.Enabled = Designer.cmdFormatPainter.Enabled;
            btnFormatPainter.Checked = Designer.FormatPainter;
            btnUndo.Enabled = Designer.cmdUndo.Enabled;
            btnRedo.Enabled = Designer.cmdRedo.Enabled;
            btnGroup.Enabled = Designer.cmdGroup.Enabled;
            btnUngroup.Enabled = Designer.cmdUngroup.Enabled;
#if MONO
            PopulateRecentFiles();
#endif
        }

#if !MONO
        private void btnOpen_PopupOpen(object sender, PopupOpenEventArgs e)
        {
            btnOpen.SubItems.Clear();
            if (Designer.cmdRecentFiles.Enabled)
            {
                foreach (string s in Designer.RecentFiles)
                {
                    ButtonItem item = new ButtonItem();
                    item.Text = s;
                    item.Click += recentItem_Click;
                    btnOpen.SubItems.Insert(0, item);
                }
            }
            if (btnOpen.SubItems.Count == 0)
                btnOpen.SubItems.Add(new ButtonItem());
        }

        private void recentItem_Click(object sender, EventArgs e)
        {
            Designer.cmdOpen.LoadFile((sender as ButtonItem).Text);
        }
#else
        private void PopulateRecentFiles()
        {
            btnOpen.DropDown.Items.Clear();
            if (Designer.cmdRecentFiles.Enabled)
            {
                foreach (string s in Designer.RecentFiles)
                {
                    btnOpen.DropDown.Items.Insert(0, new ToolStripMenuItem(s));
                }
            }
        }

        private void btnOpen_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            btnOpen.DropDown.Close();
            Designer.cmdOpen.LoadFile(e.ClickedItem.Text);
        }
#endif

        private void FClipboardTimer_Tick(object sender, EventArgs e)
        {
            btnPaste.Enabled = Designer.cmdPaste.Enabled;
        }

        private void FPreviewTimer_Tick(object sender, EventArgs e)
        {
            previewTimer.Stop();
            Designer.cmdPreview.Invoke(sender, e);
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            previewTimer.Start();
        }
        #endregion

        #region Protected Methods
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                clipboardTimer.Dispose();
                previewTimer.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Public Methods
        public override void SelectionChanged()
        {
            base.SelectionChanged();
            UpdateControls();
        }

        public override void UpdateContent()
        {
            base.UpdateContent();
            UpdateControls();
        }

        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Designer,Toolbar,Standard");
            Text = res.Get("");

            SetItemText(btnNew, res.Get("New"));
            SetItemText(btnOpen, res.Get("Open"));
            SetItemText(btnSave, res.Get("Save"));
            SetItemText(btnSaveAll, res.Get("SaveAll"));
            SetItemText(btnPreview, res.Get("Preview"));
            SetItemText(btnNewPage, res.Get("NewPage"));
            SetItemText(btnNewDialog, res.Get("NewDialog"));
            SetItemText(btnCopyPage, res.Get("CopyPage"));
            SetItemText(btnDeletePage, res.Get("DeletePage"));
            SetItemText(btnPageSetup, res.Get("PageSetup"));
            SetItemText(btnCut, res.Get("Cut"));
            SetItemText(btnCopy, res.Get("Copy"));
            SetItemText(btnPaste, res.Get("Paste"));
            SetItemText(btnFormatPainter, res.Get("FormatPainter"));
            SetItemText(btnUndo, res.Get("Undo"));
            SetItemText(btnRedo, res.Get("Redo"));
            SetItemText(btnGroup, res.Get("Group"));
            SetItemText(btnUngroup, res.Get("Ungroup"));
        }

#if !MONO
        public override void ReinitDpiSize()
        {
            base.ReinitDpiSize();
            btnNew.Image = Res.GetImage(4);
            btnOpen.Image = Res.GetImage(1);
            btnSave.Image = Res.GetImage(2);
            btnSaveAll.Image = Res.GetImage(178);
            btnPreview.Image = Res.GetImage(3);
            btnNewPage.Image = Res.GetImage(10);
            btnNewDialog.Image = Res.GetImage(11);
            btnCopyPage.Image = Res.GetImage(6);
            btnDeletePage.Image = Res.GetImage(12);
            btnPageSetup.Image = Res.GetImage(13);
            btnCut.Image = Res.GetImage(5);
            btnCopy.Image = Res.GetImage(6);
            btnPaste.Image = Res.GetImage(7);
            btnFormatPainter.Image = Res.GetImage(18);
            btnUndo.Image = Res.GetImage(8);
            btnRedo.Image = Res.GetImage(9);
            btnGroup.Image = Res.GetImage(17);
            btnUngroup.Image = Res.GetImage(16);
            undoDropDown.UpdateDpiDependencies();
            redoDropDown.UpdateDpiDependencies();
        }
#endif
#endregion

        public StandardToolbar(Designer designer)
            : base(designer)
        {
            Name = "StandardToolbar";

#if !MONO
            btnNew = CreateButton("btnStdNew", Res.GetImage(4), Designer.cmdNew.Invoke);
            btnOpen = CreateButton("btnStdOpen", Res.GetImage(1), Designer.cmdOpen.Invoke);
            btnOpen.PopupOpen += btnOpen_PopupOpen;
            btnOpen.SubItems.Add(new ButtonItem());
            btnSave = CreateButton("btnStdSave", Res.GetImage(2), Designer.cmdSave.Invoke);
            btnSaveAll = CreateButton("btnStdSaveAll", Res.GetImage(178), Designer.cmdSaveAll.Invoke);
            btnPreview = CreateButton("btnStdPreview", Res.GetImage(3), btnPreview_Click);
            btnNewPage = CreateButton("btnStdNewPage", Res.GetImage(10), Designer.cmdNewPage.Invoke);
            btnNewPage.BeginGroup = true;
            btnNewDialog = CreateButton("btnStdNewDialog", Res.GetImage(11), Designer.cmdNewDialog.Invoke);
            btnCopyPage = CreateButton("btnStdCopyPage", Res.GetImage(6), Designer.cmdCopyPage.Invoke);
            btnDeletePage = CreateButton("btnStdDeletePage", Res.GetImage(12), Designer.cmdDeletePage.Invoke);
            btnPageSetup = CreateButton("btnStdPageSetup", Res.GetImage(13), Designer.cmdPageSetup.Invoke);
            btnCut = CreateButton("btnStdCut", Res.GetImage(5), Designer.cmdCut.Invoke);
            btnCut.BeginGroup = true;
            btnCopy = CreateButton("btnStdCopy", Res.GetImage(6), Designer.cmdCopy.Invoke);
            btnPaste = CreateButton("btnStdPaste", Res.GetImage(7), Designer.cmdPaste.Invoke);
            btnFormatPainter = CreateButton("btnStdFormatPainter", Res.GetImage(18), Designer.cmdFormatPainter.Invoke);
            btnFormatPainter.AutoCheckOnClick = true;
            btnUndo = CreateButton("btnStdUndo", Res.GetImage(8), Designer.cmdUndo.Invoke);
            btnUndo.BeginGroup = true;
            btnRedo = CreateButton("btnStdRedo", Res.GetImage(9), Designer.cmdRedo.Invoke);
            btnGroup = CreateButton("btnStdGroup", Res.GetImage(17), Designer.cmdGroup.Invoke);
            btnGroup.BeginGroup = true;
            btnUngroup = CreateButton("btnStdUngroup", Res.GetImage(16), Designer.cmdUngroup.Invoke);
            undoDropDown = new UndoDropDown(Designer, btnUndo);
            redoDropDown = new RedoDropDown(Designer, btnRedo);

            Items.AddRange(new BaseItem[] {
        btnNew, btnOpen, btnSave, btnSaveAll, btnPreview,
        btnNewPage, btnNewDialog, btnCopyPage, btnDeletePage, btnPageSetup,
        btnCut, btnCopy, btnPaste, btnFormatPainter,
        btnUndo, btnRedo, 
        btnGroup, btnUngroup, CustomizeItem });
#else
            btnNew = CreateButton("btnStdNew", Res.GetImage(4), Designer.cmdNew.Invoke);
            btnOpen = CreateSplitButton("btnStdOpen", Res.GetImage(1), null);
            btnOpen.ButtonClick += Designer.cmdOpen.Invoke;
            btnOpen.DropDownItemClicked += btnOpen_DropDownItemClicked;
            btnSave = CreateButton("btnStdSave", Res.GetImage(2), Designer.cmdSave.Invoke);
            btnSaveAll = CreateButton("btnStdSaveAll", Res.GetImage(178), Designer.cmdSaveAll.Invoke);
            btnPreview = CreateButton("btnStdPreview", Res.GetImage(3), btnPreview_Click);
            btnNewPage = CreateButton("btnStdNewPage", Res.GetImage(10), Designer.cmdNewPage.Invoke);
            btnNewDialog = CreateButton("btnStdNewDialog", Res.GetImage(11), Designer.cmdNewDialog.Invoke);
            btnCopyPage = CreateButton("btnStdCopyPage", Res.GetImage(6), Designer.cmdCopyPage.Invoke);
            btnDeletePage = CreateButton("btnStdDeletePage", Res.GetImage(12), Designer.cmdDeletePage.Invoke);
            btnPageSetup = CreateButton("btnStdPageSetup", Res.GetImage(13), Designer.cmdPageSetup.Invoke);
            btnCut = CreateButton("btnStdCut", Res.GetImage(5), Designer.cmdCut.Invoke);
            btnCopy = CreateButton("btnStdCopy", Res.GetImage(6), Designer.cmdCopy.Invoke);
            btnPaste = CreateButton("btnStdPaste", Res.GetImage(7), Designer.cmdPaste.Invoke);
            btnFormatPainter = CreateButton("btnStdFormatPainter", Res.GetImage(18), Designer.cmdFormatPainter.Invoke);
            btnFormatPainter.CheckOnClick = true;
            btnUndo = CreateSplitButton("btnStdUndo", Res.GetImage(8), Designer.cmdUndo.Invoke);
            btnRedo = CreateSplitButton("btnStdRedo", Res.GetImage(9), Designer.cmdRedo.Invoke);
            btnGroup = CreateButton("btnStdGroup", Res.GetImage(17), Designer.cmdGroup.Invoke);
            btnUngroup = CreateButton("btnStdUngroup", Res.GetImage(16), Designer.cmdUngroup.Invoke);
            undoDropDown = new UndoDropDown(designer, this);
            redoDropDown = new RedoDropDown(designer, this);
            btnUndo.DropDown = undoDropDown;
            btnRedo.DropDown = redoDropDown;

            Items.AddRange(new ToolStripItem[] {
        btnNew, btnOpen, btnSave, btnSaveAll, btnPreview, new ToolStripSeparator(),
        btnNewPage, btnNewDialog, btnCopyPage, btnDeletePage, btnPageSetup, new ToolStripSeparator(),
        btnCut, btnCopy, btnPaste, btnFormatPainter, new ToolStripSeparator(),
        btnUndo, btnRedo, new ToolStripSeparator(),
        btnGroup, btnUngroup });
#endif

            Localize();

            clipboardTimer = new Timer();
            clipboardTimer.Interval = 500;
            clipboardTimer.Tick += FClipboardTimer_Tick;
            clipboardTimer.Start();

            previewTimer = new Timer();
            previewTimer.Interval = 20;
            previewTimer.Tick += FPreviewTimer_Tick;
        }
    }

}
