using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents;
using FastReport.DevComponents.DotNetBar;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Design.Toolbars
{
#if !MONO
    internal class UndoRedoDropDown : ItemContainer
#else
    internal class UndoRedoDropDown : ToolStripDropDown
#endif
    {
        private bool updating;
        protected Designer designer;
        protected ListBox lbxActions;
        protected Label lblUndoRedo;
#if !MONO
        protected ButtonItem parent;
        protected ControlContainerItem actionsHost;
        protected ControlContainerItem labelHost;
#else
        protected ToolbarBase toolbar;
        protected ToolStripControlHost actionsHost;
        protected ToolStripControlHost labelHost;
#endif

        private void SetSelected(int index)
        {
            if (updating)
                return;
            updating = true;
            int saveTop = lbxActions.TopIndex;
            lbxActions.BeginUpdate();
            lbxActions.SelectedIndices.Clear();
            if (lbxActions.Items.Count > 0)
            {
                for (int i = index; i >= 0; i--)
                    lbxActions.SelectedIndices.Add(i);
            }
            lbxActions.TopIndex = saveTop;
            lbxActions.EndUpdate();
            UpdateLabel();
            updating = false;
        }

#if !MONO
        public override void UpdateDpiDependencies()
        {
            base.UpdateDpiDependencies();
            lbxActions.Size = DpiHelper.ConvertUnits(new Size(150, 200));
            lblUndoRedo.Size = DpiHelper.ConvertUnits(new Size(150, 30));
        }
#endif

        private void lbxActions_MouseMove(object sender, MouseEventArgs e)
        {
            int index = lbxActions.IndexFromPoint(e.X, e.Y);
            SetSelected(index);
        }

        private void lbxActions_MouseDown(object sender, MouseEventArgs e)
        {
            int actionsCount = lbxActions.SelectedItems.Count + 1;
#if !MONO
            parent.ClosePopup();
#else
            Close(ToolStripDropDownCloseReason.ItemClicked);
#endif
            DoUndoRedo(actionsCount);
        }

        private void UpdateSizes()
        {
            float maxWidth = DrawUtils.MeasureString(String.Format(Res.Get("Designer,UndoRedo,UndoN"), 100), lblUndoRedo.Font).Width + 10;
            for (int i = 0; i < lbxActions.Items.Count; i++)
            {
                string s = (string)lbxActions.Items[i];
                float width = DrawUtils.MeasureString(s, lbxActions.Font).Width;
                if (width > maxWidth)
                    maxWidth = width;
            }
            float maxHeight = (lbxActions.Items.Count > DpiHelper.ConvertUnits(10) ? DpiHelper.ConvertUnits(10) : lbxActions.Items.Count) * lbxActions.ItemHeight;
            actionsHost.Size = new Size((int)maxWidth + DpiHelper.ConvertUnits(10), (int)maxHeight);
#if !MONO
            lblUndoRedo.Size = new Size((int)maxWidth + DpiHelper.ConvertUnits(10), DpiHelper.ConvertUnits(20));
#else
            labelHost.Size = new Size((int)maxWidth + 10, 20);
#endif
        }

        protected virtual void PopulateItems()
        {
        }

        protected virtual void UpdateLabel()
        {
        }

        protected virtual void DoUndoRedo(int actionsCount)
        {
        }

#if !MONO
        private void parent_PopupOpen(object sender, PopupOpenEventArgs e)
        {
#else
        protected override void OnOpening(CancelEventArgs e)
        {
            base.OnOpening(e);
#endif
            PopulateItems();
            UpdateSizes();
            SetSelected(0);
        }

#if !MONO
        public UndoRedoDropDown(Designer designer, ButtonItem parent)
#else
        public UndoRedoDropDown(Designer designer, ToolbarBase toolbar)
            : base()
#endif
        {
            this.designer = designer;
#if !MONO
            this.parent = parent;

            LayoutOrientation = eOrientation.Vertical;
            parent.PopupType = ePopupType.ToolBar;
            parent.SubItems.Add(this);
            parent.PopupOpen += new DotNetBarManager.PopupOpenEventHandler(parent_PopupOpen);

            lbxActions = new ListBox();
            lbxActions.Size = DpiHelper.ConvertUnits(new Size(150, 200));
            lbxActions.BorderStyle = BorderStyle.None;
            lbxActions.SelectionMode = SelectionMode.MultiSimple;
            lbxActions.MouseMove += new MouseEventHandler(lbxActions_MouseMove);
            lbxActions.MouseDown += new MouseEventHandler(lbxActions_MouseDown);

            actionsHost = new ControlContainerItem();
            actionsHost.Control = lbxActions;

            SubItems.Add(actionsHost);

            lblUndoRedo = new Label();
            lblUndoRedo.AutoSize = false;
            lblUndoRedo.Size = DpiHelper.ConvertUnits(new Size(150, 30));
            lblUndoRedo.TextAlign = ContentAlignment.MiddleCenter;
            lblUndoRedo.BackColor = Color.Transparent;

            labelHost = new ControlContainerItem();
            labelHost.Control = lblUndoRedo;

            SubItems.Add(labelHost);
#else
            this.toolbar = toolbar;
            Renderer = toolbar.Renderer;
            Padding = new Padding(1);
            Font = toolbar.Font;
            lbxActions = new ListBox();
            lbxActions.Dock = DockStyle.Fill;
            lbxActions.BorderStyle = BorderStyle.None;
            lbxActions.SelectionMode = SelectionMode.MultiSimple;
            lbxActions.MouseMove += new MouseEventHandler(lbxActions_MouseMove);
            lbxActions.MouseDown += new MouseEventHandler(lbxActions_MouseDown);
            actionsHost = new ToolStripControlHost(lbxActions);
            actionsHost.AutoSize = false;
            actionsHost.Size = new Size(150, 250);
            Items.Add(actionsHost);
            lblUndoRedo = new Label();
            lblUndoRedo.Dock = DockStyle.Fill;
            lblUndoRedo.AutoSize = false;
            lblUndoRedo.TextAlign = ContentAlignment.MiddleCenter;
            lblUndoRedo.Text = "test";
            lblUndoRedo.BackColor = Color.Transparent;
            labelHost = new ToolStripControlHost(lblUndoRedo);
            labelHost.AutoSize = false;
            labelHost.Size = new Size(100, 30);
            Items.Add(labelHost);

#endif
        }

    }


    internal class UndoDropDown : UndoRedoDropDown
    {
        protected override void PopulateItems()
        {
            lbxActions.Items.Clear();
            if (designer.ActiveReport != null)
            {
                string[] undoNames = designer.ActiveReportTab.UndoRedo.UndoNames;
                foreach (string s in undoNames)
                {
                    lbxActions.Items.Add(s);
                }
            }
        }

        protected override void UpdateLabel()
        {
            lblUndoRedo.Text = String.Format(Res.Get("Designer,UndoRedo,UndoN"), lbxActions.SelectedItems.Count);
        }

        protected override void DoUndoRedo(int actionsCount)
        {
            designer.cmdUndo.Undo(actionsCount);
        }

#if !MONO
        public UndoDropDown(Designer designer, ButtonItem parent)
            : base(designer, parent)
#else
        public UndoDropDown(Designer designer, ToolbarBase toolbar)
            : base(designer, toolbar)
#endif
        {
        }
    }


    internal class RedoDropDown : UndoRedoDropDown
    {
        protected override void PopulateItems()
        {
            lbxActions.Items.Clear();
            if (designer.ActiveReport != null)
            {
                string[] undoNames = designer.ActiveReportTab.UndoRedo.RedoNames;
                foreach (string s in undoNames)
                {
                    lbxActions.Items.Add(s);
                }
            }
        }

        protected override void UpdateLabel()
        {
            lblUndoRedo.Text = String.Format(Res.Get("Designer,UndoRedo,RedoN"), lbxActions.SelectedItems.Count);
        }

        protected override void DoUndoRedo(int actionsCount)
        {
            designer.cmdRedo.Redo(actionsCount);
        }

#if !MONO
        public RedoDropDown(Designer designer, ButtonItem parent)
            : base(designer, parent)
#else
        public RedoDropDown(Designer designer, ToolbarBase toolbar)
            : base(designer, toolbar)
#endif
        {
        }
    }


}
