using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using FastReport.Utils;
using FastReport.Forms;
using FastReport.Dialog;
using FastReport.Design.ToolWindows;
using FastReport.Data;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Design.PageDesigners.Dialog
{
    internal class DialogWorkspace : DesignWorkspaceBase
    {
        #region Fields
        #endregion

        #region Properties
        public static Grid Grid = new Grid();
        public static bool ShowGrid;
        public static bool SnapToGrid;

        public DialogPage Page
        {
            get { return PageDesigner.Page as DialogPage; }
        }

        public override Point Offset
        {
            get
            {
                Form form = Page.Form;
                Point offset = new Point(0, 0);
                offset = form.PointToScreen(offset);
                offset.X -= form.Left - 10;
                offset.Y -= form.Top - 10;
                return offset;
            }
        }
        #endregion

        #region Private Methods
        private void UpdateName()
        {
            if (Page.Name == "")
                Text = Page.ClassName + (Page.ZOrder + 1).ToString();
            else
                Text = Page.Name;
        }

        private RectangleF GetSelectedRect()
        {
            RectangleF result = new RectangleF(100000, 100000, -100000, -100000);
            foreach (Base obj in Designer.SelectedObjects)
            {
                if (obj is ComponentBase)
                {
                    ComponentBase c = obj as ComponentBase;
                    if (c.Left < result.Left)
                        result.X = c.Left;
                    if (c.Right > result.Right)
                        result.Width = c.Right - result.Left;
                    if (c.Top < result.Top)
                        result.Y = c.Top;
                    if (c.Bottom > result.Bottom)
                        result.Height = c.Bottom - result.Top;
                }
                else if (obj is DialogPage)
                {
                    result = new RectangleF(0, 0, Page.Width, Page.Height);
                }
            }
            return result;
        }

        private void UpdateStatusBar()
        {
            RectangleF selectedRect = GetSelectedRect();
            bool IsPageOrReportSelected = Designer.SelectedObjects.IsPageSelected || Designer.SelectedObjects.IsReportSelected;
            string location = IsPageOrReportSelected ? "" :
              selectedRect.Left.ToString() + "; " + selectedRect.Top.ToString();
            string size = IsPageOrReportSelected ? "" :
              selectedRect.Width.ToString() + "; " + selectedRect.Height.ToString();
            Designer.ShowStatus(location, size, "");
        }

        private void AfterDragDrop(Point location)
        {
            DataFilterBaseControl control = Designer.SelectedObjects[0] as DataFilterBaseControl;

            if (control is TextBoxControl)
            {
                // show menu with different control types
                ContextMenuStrip menu = new ContextMenuStrip();
                menu.Renderer = Config.DesignerSettings.ToolStripRenderer;

                Type[] controlTypes = new Type[] {
        typeof(TextBoxControl), typeof(MaskedTextBoxControl),
        typeof(ComboBoxControl), typeof(CheckedListBoxControl),
        typeof(ListBoxControl), typeof(DataSelectorControl) };

                foreach (Type controlType in controlTypes)
                {
                    ObjectInfo info = RegisteredObjects.FindObject(controlType);
                    ToolStripMenuItem item = new ToolStripMenuItem(Res.Get(info.Text), Res.GetImage(info.ImageIndex));
                    menu.Items.Add(item);
                    item.Tag = controlType;
                    item.Click += new EventHandler(ControlTypeItem_Click);
                }

                menu.Show(this, new Point((int)control.AbsLeft + Offset.X, (int)control.AbsBottom + Offset.Y));
            }
            else
            {
                // look for another controls of same type in a form that bound to the same DataColumn
                // and setup the FilterOperation
                foreach (Base c in Page.AllObjects)
                {
                    if (c != control && c.GetType() == control.GetType() &&
                      (c as DataFilterBaseControl).DataColumn == control.DataColumn)
                    {
                        (c as DataFilterBaseControl).FilterOperation = FilterOperation.GreaterThanOrEqual;
                        control.FilterOperation = FilterOperation.LessThanOrEqual;
                        Designer.SetModified(this, "Change");
                        break;
                    }
                }
            }
        }

        private void ControlTypeItem_Click(object sender, EventArgs e)
        {
            DataFilterBaseControl control = Designer.SelectedObjects[0] as DataFilterBaseControl;
            Type controlType = (sender as ToolStripMenuItem).Tag as Type;
            DataFilterBaseControl newControl = Activator.CreateInstance(controlType) as DataFilterBaseControl;

            newControl.Parent = control.Parent;
            newControl.Location = control.Location;
            newControl.DataColumn = control.DataColumn;
            newControl.ReportParameter = control.ReportParameter;

            control.Dispose();
            newControl.CreateUniqueName();

            Designer.SelectedObjects.Clear();
            Designer.SelectedObjects.Add(newControl);
            Designer.SetModified(this, "Insert");
        }

        private void DrawSelectionRect(FRPaintEventArgs e)
        {
            RectangleF rect = NormalizeSelectionRect();
            IGraphics g = e.Graphics;
            Brush b = Report.GraphicCache.GetBrush(Color.FromArgb(80, SystemColors.Highlight));
            float scale = e.ScaleX;
            g.FillRectangle(b, rect.Left * scale, rect.Top * scale, rect.Width * scale, rect.Height * scale);
            Pen pen = Report.GraphicCache.GetPen(SystemColors.Highlight, 1, DashStyle.Dash);
            g.DrawRectangle(pen, rect.Left * scale, rect.Top * scale, rect.Width * scale, rect.Height * scale);
        }
        #endregion

        #region Protected Methods
        protected override float GridSnapSize
        {
            get { return Grid.SnapSize; }
        }

        protected override bool CheckGridStep()
        {
            bool al = SnapToGrid;
            if ((ModifierKeys & Keys.Alt) > 0)
                al = !al;

            bool result = true;
            float kx = eventArgs.delta.X;
            float ky = eventArgs.delta.Y;
            if (al)
            {
                result = kx >= GridSnapSize || kx <= -GridSnapSize || ky >= GridSnapSize || ky <= -GridSnapSize;
                if (result)
                {
                    kx = (int)(kx / GridSnapSize) * GridSnapSize;
                    ky = (int)(ky / GridSnapSize) * GridSnapSize;
                }
            }
            else
            {
                result = kx != 0 || ky != 0;
            }
            eventArgs.delta.X = kx;
            eventArgs.delta.Y = ky;
            return result;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Locked || Page == null)
                return;

            Size = new Size((int)(Page.Width * DpiHelper.Multiplier) + 20, (int)(Page.Height * DpiHelper.Multiplier) + 20);
            Graphics g = e.Graphics;
            FRPaintEventArgs paintArgs = new FRPaintEventArgs(g, DpiHelper.Multiplier, DpiHelper.Multiplier, Report.GraphicCache);

            // check if workspace is active (in the mdi mode). 
            ObjectCollection objects = Designer.Objects;
            if (Designer.ActiveReport != Report)
            {
                objects = Page.AllObjects;
                objects.Add(Page);
            }

            // draw form
            Page.SetDesigning(true);
            g.DrawImage(Page.FormBitmap, 10, 10);

            g.TranslateTransform(Offset.X, Offset.Y);
            if (ShowGrid)
                Grid.Draw(e.Graphics, new Rectangle(0, 0, (int)Page.ClientSize.Width, (int)Page.ClientSize.Height));

            // draw objects
            foreach (Base obj in objects)
            {
                if (obj is ComponentBase)
                {
                    obj.SetDesigning(true);
                    (obj as ComponentBase).Draw(paintArgs);
                }
            }
            // draw selection
            if (!mouseDown && Designer.ActiveReport == Report)
            {
                foreach (Base obj in Designer.SelectedObjects)
                {
                    if (obj is ComponentBase)
                        (obj as ComponentBase).DrawSelection(paintArgs);
                }
            }
            virtualGuides.Draw(g);
            if (mode2 == WorkspaceMode2.SelectionRect)
                DrawSelectionRect(paintArgs);
        }

        protected override void ShowLocationSizeToolTip(int x, int y)
        {
            string s = "";
            RectangleF selectedRect = GetSelectedRect();
            if (mode2 == WorkspaceMode2.Move)
                s = selectedRect.Left.ToString() + "; " + selectedRect.Top.ToString();
            else if (mode2 == WorkspaceMode2.Size)
                s = selectedRect.Width.ToString() + "; " + selectedRect.Height.ToString();
            ShowToolTip(s, x, y);
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);
            if (!Designer.cmdInsert.Enabled)
            {
                drgevent.Effect = DragDropEffects.None;
                return;
            }

            if (mode1 != WorkspaceMode1.DragDrop)
            {
                DictionaryWindow.DraggedItem item = DictionaryWindow.DragUtils.GetOne(drgevent);
                if (item == null)
                    return;
                Type dataType = null;
                if (item.obj is Column)
                    dataType = (item.obj as Column).DataType;
                else if (item.obj is Parameter)
                    dataType = (item.obj as Parameter).DataType;
                else
                    return;

                // determine type of control to insert
                Type controlType = typeof(NumericUpDownControl);
                if (dataType == typeof(string) || dataType == typeof(char))
                    controlType = typeof(TextBoxControl);
                else if (dataType == typeof(DateTime) || dataType == typeof(TimeSpan))
                    controlType = typeof(DateTimePickerControl);
                else if (dataType == typeof(bool))
                    controlType = typeof(CheckBoxControl);
                else if (dataType == typeof(byte[]))
                    controlType = null;

                if (controlType == null)
                {
                    drgevent.Effect = DragDropEffects.None;
                    return;
                }

                // create label and control
                bool needCreateLabel = controlType != typeof(CheckBoxControl);
                if (needCreateLabel)
                {
                    Designer.InsertObject(new ObjectInfo[] { 
            RegisteredObjects.FindObject(controlType),
            RegisteredObjects.FindObject(typeof(LabelControl)) }, InsertFrom.Dictionary);
                }
                else
                {
                    Designer.InsertObject(RegisteredObjects.FindObject(controlType), InsertFrom.Dictionary);
                }

                // setup control datafiltering
                DataFilterBaseControl control = Designer.SelectedObjects[0] as DataFilterBaseControl;
                control.Top += (16 / Grid.SnapSize) * Grid.SnapSize;
                if (item.obj is Column)
                    control.DataColumn = item.text;
                else if (item.obj is Parameter)
                    control.ReportParameter = item.text;

                // setup label text
                string labelText = "";
                if (item.obj is Column)
                    labelText = (item.obj as Column).Alias;
                else if (item.obj is Parameter)
                {
                    labelText = (item.obj as Parameter).Description;
                    if (String.IsNullOrEmpty(labelText))
                        labelText = (item.obj as Parameter).Name;
                }

                if (needCreateLabel)
                {
                    LabelControl label = Designer.SelectedObjects[1] as LabelControl;
                    label.Text = labelText;
                }
                else
                    control.Text = labelText;

                eventArgs.DragSource = control;
            }

            mode1 = WorkspaceMode1.DragDrop;
            Point pt = PointToClient(new Point(drgevent.X, drgevent.Y));
            OnMouseMove(new MouseEventArgs(MouseButtons.Left, 0, pt.X, pt.Y, 0));
            drgevent.Effect = drgevent.AllowedEffect;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
            DictionaryWindow.DraggedItem item = DictionaryWindow.DragUtils.GetOne(drgevent);
            if (item == null)
                return;
            Point pt = PointToClient(new Point(drgevent.X, drgevent.Y));
            OnMouseUp(new MouseEventArgs(MouseButtons.Left, 0, pt.X, pt.Y, 0));
            if (eventArgs.DragSource != null)
                AfterDragDrop(pt);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
            CancelPaste();
        }
        #endregion

        #region Public Methods
        public override float GetScale()
        {
            return DpiHelper.Multiplier;
        }

        protected override Base GetParentForPastedObjects()
        {
            return Page;
        }

        public override void Paste(ObjectCollection list, InsertFrom source)
        {
            base.Paste(list, source);
            // find left-top edge of inserted objects
            float minLeft = 100000;
            float minTop = 100000;
            foreach (Base c in list)
            {
                if (c is ComponentBase)
                {
                    ComponentBase c1 = c as ComponentBase;
                    if (c1.Left < minLeft)
                        minLeft = c1.Left;
                    if (c1.Top < minTop)
                        minTop = c1.Top;
                }
            }
            foreach (Base c in list)
            {
                // correct the left-top
                if (c is ComponentBase)
                {
                    ComponentBase c1 = c as ComponentBase;
                    c1.Left -= minLeft + Grid.SnapSize * 1000;
                    c1.Top -= minTop + Grid.SnapSize * 1000;
                }
            }
            mode1 = WorkspaceMode1.Insert;
            mode2 = WorkspaceMode2.Move;
            eventArgs.activeObject = null;
            OnMouseDown(new MouseEventArgs(MouseButtons.Left, 0,
              Offset.X + 10 - (int)(Grid.SnapSize * 1000 * DpiHelper.Multiplier), Offset.Y + 10 - (int)(Grid.SnapSize * 1000 * DpiHelper.Multiplier), 0));
        }

        public override void Refresh()
        {
            UpdateStatusBar();
            base.Refresh();
        }

        public void UpdateDpiDependencies()
        {
            Page.ResetFormBitmap();

            foreach (Base obj in Designer.Objects)
                if (obj is DialogControl)
                    (obj as DialogControl).ReinitDpiSize();
        }
        #endregion

        public DialogWorkspace(PageDesignerBase pageDesigner)
            : base(pageDesigner)
        {
            SnapToGrid = true;
            BackColor = SystemColors.Window;
        }
    }
}
