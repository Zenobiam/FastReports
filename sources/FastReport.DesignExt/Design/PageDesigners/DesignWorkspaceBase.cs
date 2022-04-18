using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using FastReport.Data;
using FastReport.Utils;
using FastReport.TypeConverters;
using FastReport.Design.ToolWindows;
using FastReport.Format;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Design.PageDesigners
{
    internal abstract class DesignWorkspaceBase : UserControl
    {
        #region Fields
        private PageDesignerBase pageDesigner;
        protected WorkspaceMode1 mode1;
        protected WorkspaceMode2 mode2;
        protected bool mouseDown;
        protected bool mouseMoved;
        private PointF lastMousePoint;
        protected RectangleF selectionRect;
        protected FRMouseEventArgs eventArgs;
        protected VirtualGuides virtualGuides;
        private System.Windows.Forms.ToolTip toolTip;
        private SmartTagButton smartTag;
        private InsertFrom insertionSource;
        private SelectedObjectCollection selectedObjectsAtMouseDown;
        private static Cursor FFormatToolCursor;
        #endregion

        #region Properties
        public PageDesignerBase PageDesigner
        {
            get { return pageDesigner; }
        }

        public Designer Designer
        {
            get { return pageDesigner.Designer; }
        }

        public bool Locked
        {
            get { return pageDesigner.Locked; }
        }

        public Report Report
        {
            get { return pageDesigner.Report; }
        }

        public virtual Point Offset
        {
            get
            {
                return new Point(0, 0);
            }
        }

        protected abstract float GridSnapSize
        {
            get;
        }

        private GraphicCache GraphicCache
        {
            get { return Report.GraphicCache; }
        }
        #endregion

        public abstract float GetScale();
        protected abstract bool CheckGridStep();

        protected RectangleF NormalizeSelectionRect()
        {
            RectangleF result = selectionRect;
            if (selectionRect.Left > selectionRect.Right)
            {
                result.X = selectionRect.Right;
                result.Width = -selectionRect.Width;
            }
            if (selectionRect.Top > selectionRect.Bottom)
            {
                result.Y = selectionRect.Bottom;
                result.Height = -selectionRect.Height;
            }
            return result;
        }

        protected void CloneSelectedObjects()
        {
            ObjectCollection list = new ObjectCollection();
            foreach (Base c in Designer.Objects)
            {
                if (Designer.SelectedObjects.IndexOf(c) != -1)
                    if (c.HasFlag(Flags.CanCopy))
                        list.Add(c);
            }

            // prepare to create unique name
            FastNameCreator nameCreator = new FastNameCreator(Report.AllNamedObjects);
            Designer.SelectedObjects.Clear();

            foreach (Base c in list)
            {
                Base clone = Activator.CreateInstance(c.GetType()) as Base;
                clone.AssignAll(c);
                clone.Name = "";
                clone.Parent = c.Parent;

                nameCreator.CreateUniqueName(clone);
                foreach (Base c1 in clone.AllObjects)
                {
                    nameCreator.CreateUniqueName(c1);
                }

                Designer.Objects.Add(clone);
                Designer.SelectedObjects.Add(clone);
            }
        }

        protected void HandleDoubleClick()
        {
            if (Designer.SelectedObjects.Count == 1 && Designer.SelectedObjects[0] is ComponentBase)
            {
                ComponentBase c = Designer.SelectedObjects[0] as ComponentBase;
                c.HandleDoubleClick();
            }
        }

        protected void MoveSelectedObjects(int xDir, int yDir, bool smooth)
        {
            foreach (Base obj in Designer.SelectedObjects)
            {
                if (obj is ComponentBase && !(obj is PageBase))
                {
                    ComponentBase c = obj as ComponentBase;
                    c.Left += smooth ? xDir : xDir * GridSnapSize;
                    c.Top += smooth ? yDir : yDir * GridSnapSize;
                    c.CheckParent(true);
                }
            }
            virtualGuides.Check();
            Refresh();
            Designer.SetModified(pageDesigner, "Move");
        }

        protected void ResizeSelectedObjects(int xDir, int yDir, bool smooth)
        {
            foreach (Base obj in Designer.SelectedObjects)
            {
                if (obj is ComponentBase && !(obj is PageBase))
                {
                    ComponentBase c = obj as ComponentBase;
                    c.Width += smooth ? xDir : xDir * GridSnapSize;
                    c.Height += smooth ? yDir : yDir * GridSnapSize;
                    c.CheckNegativeSize(eventArgs);
                }
            }
            virtualGuides.Check();
            Refresh();
            Designer.SetModified(pageDesigner, "Size");
        }

        protected void SelectNextObject(bool forward)
        {
            int index = 0;
            if (Designer.SelectedObjects.Count != 0)
                index = Designer.Objects.IndexOf(Designer.SelectedObjects[0]);
            index += forward ? 1 : -1;
            if (index < 0)
                index = Designer.Objects.Count - 1;
            if (index > Designer.Objects.Count - 1)
                index = 0;
            Designer.SelectedObjects.Clear();
            Designer.SelectedObjects.Add(Designer.Objects[index]);
            Designer.SelectionChanged(null);
        }

        protected void ShowToolTip(string text, int x, int y)
        {
            if (toolTip == null)
                toolTip = new System.Windows.Forms.ToolTip();
            if (toolTip != null)
                toolTip.Show(text, this, x, y);
        }

        protected void HideToolTip()
        {
            if (toolTip != null)
            {
                toolTip.Hide(this);
                toolTip.Dispose();
                toolTip = null;
            }
        }

        protected abstract Base GetParentForPastedObjects();

        public virtual void Paste(ObjectCollection list, InsertFrom source)
        {
            insertionSource = source;
            pageDesigner.IsInsertingObject = true;

            // prepare to create unique name
            ObjectCollection allObjects = Report.AllNamedObjects;
            // prepare a list of existing names
            Hashtable names = new Hashtable();
            foreach (Base c in allObjects)
            {
                names[c.Name] = 0;
            }

            // since we are trying to preserve pasted object's name, add all names to the 
            // allObjects list to guarantee that FastNameCreator will work correctly
            foreach (Base c in list)
            {
                allObjects.Add(c);
                // there is an existing object with the same name. Clear the name to indicate
                // that we should create an unique name for this object
                if (names.ContainsKey(c.Name))
                    c.Name = "";
                if (c is IParent)
                {
                    foreach (Base c1 in c.AllObjects)
                    {
                        allObjects.Add(c1);
                        if (names.ContainsKey(c1.Name))
                            c1.Name = "";
                    }
                }
            }

            FastNameCreator nameCreator = new FastNameCreator(allObjects);

            // create unique names and add objects to designer's lists
            Designer.SelectedObjects.Clear();
            foreach (Base c in list)
            {
                c.Parent = GetParentForPastedObjects();
                if (c.Name == "")
                    nameCreator.CreateUniqueName(c);
                Designer.Objects.Add(c);

                // reset group index
                if (c is ComponentBase)
                    (c as ComponentBase).GroupIndex = 0;

                if (c is IParent)
                {
                    foreach (Base c1 in c.AllObjects)
                    {
                        if (c1.Name == "")
                            nameCreator.CreateUniqueName(c1);
                        Designer.Objects.Add(c1);
                    }
                }
                Designer.SelectedObjects.Add(c);
            }
        }

        public void CancelPaste()
        {
            pageDesigner.IsInsertingObject = false;
            if (mode1 != WorkspaceMode1.Select)
            {
                while (Designer.SelectedObjects.Count > 0)
                {
                    Base c = Designer.SelectedObjects[0];
                    Designer.SelectedObjects.Remove(c);
                    Designer.Objects.Remove(c);
                    c.Dispose();
                }
                mode1 = WorkspaceMode1.Select;
                mode2 = WorkspaceMode2.None;
                OnMouseUp(new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
                Designer.ResetObjectsToolbar(true);
            }
        }

        protected override void Dispose(bool disposing)
        {
            CancelPaste();
            base.Dispose(disposing);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (Locked)
                return;

            Designer.SelectedObjects.CopyTo(selectedObjectsAtMouseDown);

            mouseDown = true;
            mouseMoved = false;
            eventArgs.x = e.X / GetScale() - (int)(Offset.X / DpiHelper.Multiplier);
            eventArgs.y = e.Y / GetScale() - (int)(Offset.Y / DpiHelper.Multiplier);
            eventArgs.button = e.Button;
            eventArgs.modifierKeys = ModifierKeys;
            eventArgs.handled = false;

            if (mode2 == WorkspaceMode2.None)
            {
                // find an object under the mouse
                for (int i = Designer.Objects.Count - 1; i >= 0; i--)
                {
                    ComponentBase c = Designer.Objects[i] as ComponentBase;
                    if (c != null)
                    {
                        c.HandleMouseDown(eventArgs);
                        if (eventArgs.handled)
                        {
                            mode2 = eventArgs.mode;
                            break;
                        }
                    }
                }
            }
            else if (eventArgs.activeObject != null)
            {
                eventArgs.activeObject.HandleMouseDown(eventArgs);
            }

            if (!selectedObjectsAtMouseDown.Equals(Designer.SelectedObjects))
            {
                Designer.SelectionChanged(pageDesigner);
                Designer.SelectedObjects.CopyTo(selectedObjectsAtMouseDown);
            }

            lastMousePoint.X = eventArgs.x;
            lastMousePoint.Y = eventArgs.y;
            selectionRect = new RectangleF(eventArgs.x, eventArgs.y, 0, 0);
            virtualGuides.Create();
            smartTag.Hide();
            Refresh();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (Locked || Designer.ActiveReport != Report)
                return;

            eventArgs.x = e.X / GetScale() - (int)(Offset.X / DpiHelper.Multiplier);
            eventArgs.y = e.Y / GetScale() - (int)(Offset.Y / DpiHelper.Multiplier);
            eventArgs.button = mode1 == WorkspaceMode1.Insert ? MouseButtons.Left : e.Button;
            if (mode1 == WorkspaceMode1.Insert && mode2 == WorkspaceMode2.Move)
                Cursor = Cursors.Default;
            eventArgs.delta = new PointF(eventArgs.x - lastMousePoint.X, eventArgs.y - lastMousePoint.Y);

            if (e.Button == MouseButtons.None && mode1 == WorkspaceMode1.Select)
            {
                Cursor = Cursors.Default;
                mode2 = WorkspaceMode2.None;
                eventArgs.cursor = Cursor;
                eventArgs.mode = mode2;

                eventArgs.activeObject = null;
                eventArgs.handled = false;

                // check object's sizing points
                for (int i = Designer.Objects.Count - 1; i >= 0; i--)
                {
                    ComponentBase c = Designer.Objects[i] as ComponentBase;
                    if (c != null)
                    {
                        c.HandleMouseMove(eventArgs);
                        if (eventArgs.handled)
                        {
                            Cursor = eventArgs.cursor;
                            mode2 = eventArgs.mode;
                            eventArgs.activeObject = c;
                            break;
                        }
                    }
                }

                // no sizing points found, check hover
                if (!eventArgs.handled)
                {
                    for (int i = Designer.Objects.Count - 1; i >= 0; i--)
                    {
                        ComponentBase c = Designer.Objects[i] as ComponentBase;
                        if (c != null)
                        {
                            c.HandleMouseHover(eventArgs);
                            if (eventArgs.handled)
                            {
                                Cursor = Designer.FormatPainter ? FFormatToolCursor : eventArgs.cursor;
                                eventArgs.activeObject = c;
                                if (c.HasFlag(Flags.HasSmartTag) && !c.HasRestriction(Restrictions.DontEdit))
                                    smartTag.Show(c);
                                break;
                            }
                        }
                    }
                }

                if (eventArgs.activeObject == null || !eventArgs.activeObject.HasFlag(Flags.HasSmartTag))
                    smartTag.Hide();
            }
            else if (mode2 == WorkspaceMode2.Move || mode2 == WorkspaceMode2.Size)
            {
                // handle drag&drop from the data tree
                bool dragHandled = false;
                if (mode1 == WorkspaceMode1.DragDrop)
                {
                    eventArgs.dragTarget = null;
                    eventArgs.handled = false;
                    eventArgs.dragMessage = "";

                    for (int i = Designer.Objects.Count - 1; i >= 0; i--)
                    {
                        ComponentBase c = Designer.Objects[i] as ComponentBase;

                        if (c != null &&
                            (eventArgs.dragSources == null || Array.IndexOf(eventArgs.dragSources, c) == -1) &&
                            !c.HasRestriction(Restrictions.DontModify))
                        {
                            c.HandleDragOver(eventArgs);
                        }

                        if (eventArgs.handled)
                        {
                            eventArgs.dragTarget = c;
                            dragHandled = true;
                            eventArgs.handled = false;
                            // handle remained objects to reset its state. To do this, invert mouse location
                            eventArgs.x = -eventArgs.x;
                            eventArgs.y = -eventArgs.y;
                        }
                    }

                    if (dragHandled)
                    {
                        // revert back the mouse location
                        eventArgs.x = -eventArgs.x;
                        eventArgs.y = -eventArgs.y;
                    }

                    foreach (ComponentBase obj in eventArgs.dragSources)
                        obj.SetFlags(Flags.CanDraw, !dragHandled);
                }

                if (!dragHandled && !CheckGridStep())
                    return;

                // if insert is on and user press the mouse button and move the mouse, resize selected objects
                if (mode1 == WorkspaceMode1.Insert && e.Button == MouseButtons.Left && !mouseMoved)
                {
                    mode2 = WorkspaceMode2.Size;
                    foreach (Base c in Designer.SelectedObjects)
                    {
                        if (c is ComponentBase)
                        {
                            (c as ComponentBase).Width = 0;
                            (c as ComponentBase).Height = 0;
                        }
                    }
                    eventArgs.sizingPoint = SizingPoint.RightBottom;
                }

                // ctrl was pressed, clone selected objects
                if (mode1 == WorkspaceMode1.Select && ModifierKeys == Keys.Control && !mouseMoved)
                    CloneSelectedObjects();

                mouseMoved = true;
                eventArgs.mode = mode2;
                eventArgs.handled = false;
                // first serve the active object
                if (eventArgs.activeObject != null)
                    eventArgs.activeObject.HandleMouseMove(eventArgs);

                // if active object does not reset the handled flag, serve other objects
                if (!eventArgs.handled)
                {
                    foreach (Base c in Designer.Objects)
                    {
                        if (c is ComponentBase)
                        {
                            if (c != eventArgs.activeObject)
                                (c as ComponentBase).HandleMouseMove(eventArgs);
                        }
                    }
                }

                lastMousePoint.X += eventArgs.delta.X;
                lastMousePoint.Y += eventArgs.delta.Y;

                if (mode1 == WorkspaceMode1.DragDrop && !dragHandled)
                {
                    // correct the location of the dragged object because we skip the GridCheck
                    float offset = 0f;
                    foreach (ComponentBase obj in eventArgs.dragSources)
                    {
                        if (offset == 0f)
                        {
                            obj.Left = (int)Math.Round(obj.Left / GridSnapSize) * GridSnapSize;
                            obj.Top = (int)Math.Round(obj.Top / GridSnapSize) * GridSnapSize;
                        }
                        else
                        {
                            obj.Left = offset;
                            obj.Top = (int)Math.Round(obj.Top / GridSnapSize) * GridSnapSize;
                        }
                        offset = obj.Right + GridSnapSize;
                    }
                }

                if (mode1 != WorkspaceMode1.DragDrop || !dragHandled)
                {
                    virtualGuides.Check();
                    ShowLocationSizeToolTip(e.X + 20, e.Y + 20);
                }
                else
                {
                    virtualGuides.Clear();
                    if (String.IsNullOrEmpty(eventArgs.dragMessage))
                        HideToolTip();
                    else
                        ShowToolTip(eventArgs.dragMessage, e.X + 20, e.Y + 20);
                }

                Refresh1();
            }
            else if (mode2 == WorkspaceMode2.SelectionRect)
            {
                selectionRect = new RectangleF(selectionRect.Left, selectionRect.Top,
                  eventArgs.x - selectionRect.Left, eventArgs.y - selectionRect.Top);
                Refresh();
            }
            else if (mode2 == WorkspaceMode2.Custom)
            {
                if (!CheckGridStep())
                    return;

                mouseMoved = true;
                eventArgs.mode = mode2;
                if (eventArgs.activeObject != null)
                    eventArgs.activeObject.HandleMouseMove(eventArgs);

                lastMousePoint.X += eventArgs.delta.X;
                lastMousePoint.Y += eventArgs.delta.Y;
                Refresh();
            }
        }

        protected virtual void ShowLocationSizeToolTip(int x, int y)
        {
        }

        public virtual void UpdateDpiDependencies()
        {
            smartTag.Size = DpiHelper.ConvertUnits(new Size(10, 10));
            smartTag.Image = Res.GetImage(77);
        }

        protected virtual void Refresh1()
        {
            Refresh();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (Locked)
                return;

            HideToolTip();
            pageDesigner.IsInsertingObject = false;

            eventArgs.x = e.X / GetScale() - (int)(Offset.X / DpiHelper.Multiplier);
            eventArgs.y = e.Y / GetScale() - (int)(Offset.Y / DpiHelper.Multiplier);
            eventArgs.button = e.Button;
            eventArgs.mode = mode2;
            eventArgs.handled = false;

            if (mode2 == WorkspaceMode2.Move || mode2 == WorkspaceMode2.Size)
            {
                // handle drag&drop from the data tree
                bool dragHandled = false;
                if (mode1 == WorkspaceMode1.DragDrop)
                {
                    if (eventArgs.dragTarget != null)
                    {
                        dragHandled = true;
                        eventArgs.dragTarget.HandleDragDrop(eventArgs);

                        if (eventArgs.dragSources != null)
                        {
                            foreach (ComponentBase dragSource in eventArgs.dragSources)
                            {
                                Designer.Objects.Remove(dragSource);
                                dragSource.Dispose();
                            }
                            eventArgs.dragSources = null;
                        }

                        Designer.SelectedObjects.Clear();
                        Designer.SelectedObjects.Add(eventArgs.dragTarget);
                    }
                }

                // serve all objects
                for (int i = 0; i < Designer.Objects.Count; i++)
                {
                    ComponentBase c = Designer.Objects[i] as ComponentBase;
                    if (c != null)
                        c.HandleMouseUp(eventArgs);
                }

                // remove objects with zero size
                if (mode1 == WorkspaceMode1.Insert)
                {
                    int i = 0;
                    while (i < Designer.SelectedObjects.Count)
                    {
                        ComponentBase c = Designer.SelectedObjects[i] as ComponentBase;
                        if (c != null && c.Width == 0 && c.Height == 0)
                        {
                            Designer.Objects.Remove(c);
                            Designer.SelectedObjects.Remove(c);
                            c.Dispose();
                            i--;
                        }
                        i++;
                    }
                }

                if (mode1 != WorkspaceMode1.Select && !dragHandled)
                {
                    // during OnInsert call current context may be changed
                    WorkspaceMode1 saveMode = mode1;
                    mode1 = WorkspaceMode1.Select;
                    ObjectCollection insertedObjects = new ObjectCollection();
                    Designer.SelectedObjects.CopyTo(insertedObjects);
                    foreach (Base c in insertedObjects)
                    {
                        c.OnAfterInsert(insertionSource);
                    }
                    mode1 = saveMode;
                }

                // check if we actually move a mouse after we clicked it
                string action = mode1 != WorkspaceMode1.Select ? "Insert" : mode2 == WorkspaceMode2.Move ? "Move" : "Size";
                if (dragHandled)
                    action = "Change";
                if (mouseMoved || mode1 != WorkspaceMode1.Select)
                    Designer.SetModified(pageDesigner, action);
            }
            else if (mode2 == WorkspaceMode2.SelectionRect)
            {
                eventArgs.selectionRect = NormalizeSelectionRect();
                if (eventArgs.activeObject != null)
                    eventArgs.activeObject.HandleMouseUp(eventArgs);
            }
            else if (mode2 == WorkspaceMode2.Custom)
            {
                if (eventArgs.activeObject != null)
                    eventArgs.activeObject.HandleMouseUp(eventArgs);
            }

            bool needReset = mode1 != WorkspaceMode1.Select;
            if (!selectedObjectsAtMouseDown.Equals(Designer.SelectedObjects) || needReset)
                Designer.SelectionChanged(pageDesigner);

            mouseDown = false;
            mode1 = WorkspaceMode1.Select;
            mode2 = WorkspaceMode2.None;
            virtualGuides.Clear();
            Refresh2();
            if (needReset)
                Designer.ResetObjectsToolbar(false);

            if (e.Button == MouseButtons.Right && Designer.SelectedObjects.Count > 0)
            {
                ContextMenuBase menu = Designer.SelectedObjects[0].GetContextMenu();

                if (menu != null)
                {
                    menu.Show(this, e.Location);
                }
            }
        }

        protected virtual void Refresh2()
        {
            Refresh();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (e.Button == MouseButtons.Left)
                HandleDoubleClick();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return (keyData & Keys.Up) != 0 || (keyData & Keys.Down) != 0 ||
              (keyData & Keys.Left) != 0 || (keyData & Keys.Right) != 0 ||
              (keyData & Keys.Tab) != 0;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Config.DisableHotkeys)
                return;

            base.OnKeyDown(e);

            // serve all objects
            foreach (Base c in Designer.Objects)
            {
                if (c is ComponentBase)
                {
                    (c as ComponentBase).HandleKeyDown(this, e);
                    if (e.Handled)
                        return;
                }
            }

            int xDir = 0;
            int yDir = 0;

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Designer.ResetObjectsToolbar(true);
                    CancelPaste();
                    Designer.SelectedObjects.Clear();
                    Designer.SelectedObjects.Add(PageDesigner.Page);
                    Designer.SelectionChanged(null);
                    break;

                case Keys.Enter:
                    HandleDoubleClick();
                    break;

                case Keys.Insert:
                    if ((e.Modifiers & Keys.Control) != 0)
                        Designer.cmdCopy.Invoke();
                    else if ((e.Modifiers & Keys.Shift) != 0)
                        Designer.cmdPaste.Invoke();
                    break;

                case Keys.Delete:
                    CancelPaste();
                    if ((e.Modifiers & Keys.Shift) == 0)
                        Designer.cmdDelete.Invoke();
                    else
                        Designer.cmdCut.Invoke();
                    break;

                case Keys.Up:
                    yDir = -1;
                    break;

                case Keys.Down:
                    yDir = 1;
                    break;

                case Keys.Left:
                    xDir = -1;
                    break;

                case Keys.Right:
                    xDir = 1;
                    break;

                case Keys.Tab:
                    SelectNextObject((e.Modifiers & Keys.Shift) == 0);
                    break;
            }

            if ((e.Modifiers & Keys.Control) != 0)
            {
                switch (e.KeyCode)
                {
                    case Keys.C:
                        Designer.cmdCopy.Invoke();
                        break;

                    case Keys.X:
                        Designer.cmdCut.Invoke();
                        break;

                    case Keys.V:
                        Designer.cmdPaste.Invoke();
                        break;

                    case Keys.A:
                        Designer.cmdSelectAll.Invoke();
                        break;
                }
            }

            if (xDir != 0 || yDir != 0)
            {
                virtualGuides.Create();
                bool smooth = (e.Modifiers & Keys.Control) != 0;
                if ((e.Modifiers & Keys.Shift) != 0)
                    ResizeSelectedObjects(xDir, yDir, smooth);
                else
                    MoveSelectedObjects(xDir, yDir, smooth);
                virtualGuides.Clear();
            }
        }

        internal DesignWorkspaceBase(PageDesignerBase pageDesigner)
        {
            this.pageDesigner = pageDesigner;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            AllowDrop = true;
            eventArgs = new FRMouseEventArgs();
            selectedObjectsAtMouseDown = new SelectedObjectCollection();

            smartTag = new SmartTagButton(this);
            Controls.Add(smartTag);
            virtualGuides = new VirtualGuides(this);
        }

        static DesignWorkspaceBase()
        {
            FFormatToolCursor = ResourceLoader.GetCursor("Format.cur");
        }


        private class SmartTagButton : Button
        {
            private DesignWorkspaceBase workspace;
            private ComponentBase component;

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                Point pt = PointToScreen(new Point(10, 0));
                SmartTagBase smartTag = component.GetSmartTag();
                if (smartTag != null)
                    smartTag.Show(pt);
            }

            protected override bool IsInputKey(Keys keyData)
            {
                return (keyData & Keys.Up) != 0 || (keyData & Keys.Down) != 0 ||
                  (keyData & Keys.Left) != 0 || (keyData & Keys.Right) != 0 ||
                  (keyData & Keys.Tab) != 0;
            }

            protected override void OnKeyDown(KeyEventArgs kevent)
            {
                workspace.OnKeyDown(kevent);
            }

            public void Show(ComponentBase c)
            {
                Point loc = new Point((int)Math.Round(c.AbsRight * workspace.GetScale()) - DpiHelper.ConvertUnits(14),
                  (int)Math.Round(c.AbsTop * workspace.GetScale()) - DpiHelper.ConvertUnits(5));
                if (loc.Y < 0)
                    loc.Y = 0;
                Location = loc;
                Visible = true;
                component = c;
            }

            public SmartTagButton(DesignWorkspaceBase workspace)
            {
                this.workspace = workspace;
                Size = DpiHelper.ConvertUnits(new Size(10, 10));
                Visible = false;
                Cursor = Cursors.Hand;
                BackgroundImageLayout = ImageLayout.Tile;
                BackColor = Color.White;
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                Image = Res.GetImage(77);
            }
        }
    }
}
