using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FastReport.Table
{
    internal enum MouseMode
    {
        None,
        SelectColumn,
        SelectRow,
        SelectCell,
        ResizeColumn,
        ResizeRow
    }

    partial class TableBase
    {
        #region Private Fields

        private static Cursor curDownArrow = ResourceLoader.GetCursor("DownArrow.cur");
        private static Cursor curRightArrow = ResourceLoader.GetCursor("RightArrow.cur");
        private TableClipboard clipboard;
        private MouseMode mouseMode;
        private Point startSelectionPoint;

        #endregion Private Fields

        //private static float FLeftRtl;

        #region Public Properties

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new BreakableComponent BreakTo
        {
            get { return base.BreakTo; }
            set { base.BreakTo = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new bool CanGrow
        {
            get { return base.CanGrow; }
            set { base.CanGrow = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new bool CanShrink
        {
            get { return base.CanShrink; }
            set { base.CanShrink = value; }
        }

        /// <inheritdoc/>
        public override float Height
        {
            get { return base.Height; }
            set
            {
                if (IsDesigning && !lockColumnRowChange)
                {
                    foreach (TableRow r in Rows)
                    {
                        r.Height += (value - base.Height) / Rows.Count;
                    }
                }
                base.Height = value;
            }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new Hyperlink Hyperlink
        {
            get { return base.Hyperlink; }
        }

        /// <inheritdoc/>
        public override bool IsSelected
        {
            get
            {
                if (Report == null)
                    return false;
                return Report.Designer.SelectedObjects.IndexOf(this) != -1 || IsInternalSelected;
            }
        }

        /// <inheritdoc/>
        public override float Width
        {
            get { return base.Width; }
            set
            {
                if (IsDesigning && !lockColumnRowChange)
                {
                    foreach (TableColumn c in Columns)
                    {
                        c.Width += (value - base.Width) / Columns.Count;
                    }
                }
                base.Width = value;
            }
        }

        #endregion Public Properties

        #region Internal Properties

        internal TableClipboard Clipboard
        {
            get
            {
                if (clipboard == null)
                    clipboard = new TableClipboard(this);
                return clipboard;
            }
        }

        internal MouseMode MouseMode
        {
            get { return mouseMode; }
            set { mouseMode = value; }
        }

        #endregion Internal Properties

        #region Private Properties

        private bool IsInternalSelected
        {
            get
            {
                if (Report == null)
                    return false;
                SelectedObjectCollection selection = Report.Designer.SelectedObjects;
                return selection.Count > 0 && (
                  (selection[0] is TableRow && (selection[0] as TableRow).Parent == this) ||
                  (selection[0] is TableColumn && (selection[0] as TableColumn).Parent == this) ||
                  (selection[0] is TableCell && (selection[0] as TableCell).Parent != null && (selection[0] as TableCell).Parent.Parent == this));
            }
        }

        #endregion Private Properties

        #region Public Methods

        /// <inheritdoc/>
        public override ContextMenuBase GetContextMenu()
        {
            return new TableObjectMenu(Report.Designer);
        }

        /// <inheritdoc/>
        public override void HandleKeyDown(Control sender, KeyEventArgs e)
        {
            SelectedObjectCollection selection = Report.Designer.SelectedObjects;
            if (!IsSelected || !(selection[0] is TableCell))
                return;

            TableCell topCell = selection[0] as TableCell;
            int left = topCell.Address.X;
            int top = topCell.Address.Y;
            bool selectionChanged = false;

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    topCell.HandleKeyDown(sender, e);
                    break;

                case Keys.Delete:
                    foreach (Base c in selection)
                    {
                        if (c is TableCell)
                            (c as TableCell).Text = "";
                    }
                    Report.Designer.SetModified(null, "Change", Name);
                    break;

                case Keys.Up:
                    top--;
                    selectionChanged = true;
                    break;

                case Keys.Down:
                    top += topCell.RowSpan;
                    selectionChanged = true;
                    break;

                case Keys.Left:
                    left--;
                    selectionChanged = true;
                    break;

                case Keys.Right:
                    left += topCell.ColSpan;
                    selectionChanged = true;
                    break;
            }

            if (selectionChanged)
            {
                if (left < 0)
                    left = 0;
                if (left >= Columns.Count)
                    left = Columns.Count - 1;
                if (top < 0)
                    top = 0;
                if (top >= Rows.Count)
                    top = Rows.Count - 1;

                mouseMode = MouseMode.SelectCell;
                SetSelection(left, top, left, top);
                Report.Designer.SelectionChanged(null);
            }
            e.Handled = true;
        }

        /// <inheritdoc/>
        public override void HandleMouseDown(FRMouseEventArgs e)
        {
            if (mouseMode == MouseMode.None)
            {
                HandleMouseHover(e);
                if (e.handled)
                {
                    e.mode = WorkspaceMode2.Move;
                    if (IsSelected)
                    {
                        SelectedObjectCollection selection = Report.Designer.SelectedObjects;
                        if (!selection.Contains(this))
                        {
                            selection.Clear();
                            selection.Add(this);
                        }
                    }
                }
                else
                {
                    if (PointInObject(new PointF(e.x, e.y)))
                    {
                        e.handled = true;
                        e.mode = WorkspaceMode2.Custom;
                        e.activeObject = this;
                        mouseMode = MouseMode.SelectCell;
                        SelectedObjectCollection selection = Report.Designer.SelectedObjects;

                        if (e.button == MouseButtons.Left)
                        {
                            startSelectionPoint = GetAddressAtMousePoint(new PointF(e.x, e.y), true);
                            TableCell cell = this[startSelectionPoint.X, startSelectionPoint.Y];

                            if (e.modifierKeys == Keys.Shift)
                            {
                                // toggle selection
                                if (selection.Contains(cell))
                                {
                                    if (selection.Count > 1)
                                        selection.Remove(cell);
                                }
                                else
                                    selection.Add(cell);
                            }
                            else
                            {
                                selection.Clear();
                                selection.Add(cell);
                            }
                        }
                        else if (e.button == MouseButtons.Right)
                        {
                            Point selectionPoint = GetAddressAtMousePoint(new PointF(e.x, e.y), true);
                            Rectangle selectionRect = GetSelectionRect();
                            if (!selectionRect.Contains(selectionPoint))
                            {
                                selection.Clear();
                                selection.Add(this[selectionPoint.X, selectionPoint.Y]);
                            }
                        }
                    }
                }
            }
            else if (e.button == MouseButtons.Left)
            {
                startSelectionPoint = GetAddressAtMousePoint(new PointF(e.x, e.y), false);
                if (mouseMode == MouseMode.SelectColumn)
                    SetSelection(startSelectionPoint.X, 0, startSelectionPoint.X, Rows.Count - 1);
                else if (mouseMode == MouseMode.SelectRow)
                    SetSelection(0, startSelectionPoint.Y, Columns.Count - 1, startSelectionPoint.Y);
            }
        }

        /// <inheritdoc/>
        public override void HandleMouseHover(FRMouseEventArgs e)
        {
            if (IsSelected && new RectangleF(AbsLeft + 8, AbsTop - 8, 16, 16).Contains(new PointF(e.x, e.y)))
            {
                e.handled = true;
                e.cursor = Cursors.SizeAll;
            }
        }

        /// <inheritdoc/>
        public override void HandleMouseMove(FRMouseEventArgs e)
        {
            base.HandleMouseMove(e);
            if (!e.handled && e.button == MouseButtons.None)
            {
                PointF point = new PointF(e.x, e.y);
                mouseMode = MouseMode.None;
                // don't process if mouse is over move area
                HandleMouseHover(e);
                if (e.handled)
                {
                    e.handled = false;
                    return;
                }

                // check column resize or select
                if (IsSelected)
                {
                    float left = AbsLeft;
                    for (int x = 0; x < Columns.Count; x++)
                    {
                        float width = Columns[x].Width;
                        left += width;
                        if (point.Y > AbsTop && point.Y < AbsBottom && point.X > left - 3 && point.X < left + 3)
                        {
                            // column resize
                            PointF pt = new PointF(e.x + 3, e.y);
                            Point pt1 = GetAddressAtMousePoint(pt, false);
                            Point pt2 = GetAddressAtMousePoint(pt, true);
                            // check if we are in span
                            if (pt1 == pt2)
                            {
                                mouseMode = MouseMode.ResizeColumn;
                                e.cursor = Cursors.VSplit;
                                e.data = Columns[x];
                                break;
                            }
                        }
                        else if (point.Y > AbsTop - 8 && point.Y < AbsTop + 2 && point.X > left - width && point.X < left)
                        {
                            // column select
                            mouseMode = MouseMode.SelectColumn;
                            e.cursor = curDownArrow;
                            e.data = Columns[x];
                            break;
                        }
                    }

                    // check row resize or select
                    if (mouseMode == MouseMode.None)
                    {
                        float top = AbsTop;
                        for (int y = 0; y < Rows.Count; y++)
                        {
                            float height = Rows[y].Height;
                            top += height;
                            if (point.X > AbsLeft && point.X < AbsRight && point.Y > top - 3 && point.Y < top + 3)
                            {
                                // row resize
                                PointF pt = new PointF(e.x, e.y + 3);
                                Point pt1 = GetAddressAtMousePoint(pt, false);
                                Point pt2 = GetAddressAtMousePoint(pt, true);
                                // check if we are in span
                                if (pt1 == pt2)
                                {
                                    mouseMode = MouseMode.ResizeRow;
                                    e.cursor = Cursors.HSplit;
                                    e.data = Rows[y];
                                    break;
                                }
                            }
                            else if (point.X > AbsLeft - 8 && point.X < AbsLeft + 2 && point.Y > top - height && point.Y < top)
                            {
                                // row select
                                mouseMode = MouseMode.SelectRow;
                                e.cursor = curRightArrow;
                                e.data = Rows[y];
                                break;
                            }
                        }
                    }
                }

                if (mouseMode != MouseMode.None)
                {
                    e.handled = true;
                    e.mode = WorkspaceMode2.Custom;
                }
            }
            else if (e.mode == WorkspaceMode2.Custom && e.button == MouseButtons.Left)
            {
                switch (mouseMode)
                {
                    case MouseMode.SelectColumn:
                        SetSelection(startSelectionPoint.X, 0,
                          GetAddressAtMousePoint(new PointF(e.x, e.y), false).X, Rows.Count - 1);
                        break;

                    case MouseMode.SelectRow:
                        SetSelection(0, startSelectionPoint.Y,
                          Columns.Count - 1, GetAddressAtMousePoint(new PointF(e.x, e.y), false).Y);
                        break;

                    case MouseMode.SelectCell:
                        Point pt = GetAddressAtMousePoint(new PointF(e.x, e.y), false);
                        SetSelection(startSelectionPoint.X, startSelectionPoint.Y, pt.X, pt.Y);
                        break;

                    case MouseMode.ResizeColumn:
                        TableColumn col = e.data as TableColumn;
                        col.Width += e.delta.X;
                        if ((e.modifierKeys & Keys.Control) != 0 && col.Index < ColumnCount - 1)
                        {
                            TableColumn nextCol = Columns[col.Index + 1];
                            nextCol.Width -= e.delta.X;
                        }
                        if (col.Width <= 0)
                            col.Width = 1;
                        break;

                    case MouseMode.ResizeRow:
                        TableRow row = e.data as TableRow;
                        row.Height += e.delta.Y;
                        if ((e.modifierKeys & Keys.Control) != 0 && row.Index < RowCount - 1)
                        {
                            TableRow nextRow = Rows[row.Index + 1];
                            nextRow.Height -= e.delta.Y;
                        }
                        if (row.Height <= 0)
                            row.Height = 1;
                        break;
                }
            }
        }

        /// <inheritdoc/>
        public override void HandleMouseUp(FRMouseEventArgs e)
        {
            base.HandleMouseUp(e);
            if (mouseMode == MouseMode.ResizeRow)
            {
                // update band's height
                if (Parent is BandBase)
                    (Parent as BandBase).FixHeight();
                Report.Designer.SetModified(null, "Size", (e.data as TableRow).Name);
            }
            if (mouseMode == MouseMode.ResizeColumn)
                Report.Designer.SetModified(null, "Size", (e.data as TableColumn).Name);
            mouseMode = MouseMode.None;
        }

        /// <inheritdoc/>
        public override void OnAfterInsert(InsertFrom source)
        {
            CreateUniqueNames();
        }

        /// <inheritdoc/>
        public override void OnBeforeInsert(int flags)
        {
            Width = TableColumn.DefaultWidth * 5;
            Height = TableRow.DefaultHeight * 5;
            RowCount = 5;
            ColumnCount = 5;
        }

        #endregion Public Methods

        #region Internal Methods

        internal virtual ContextMenuBase GetCellContextMenu(TableCell cell)
        {
            return null;
        }

        internal virtual SmartTagBase GetCellSmartTag(TableCell cell)
        {
            return null;
        }

        internal virtual ContextMenuBase GetColumnContextMenu(TableColumn column)
        {
            return null;
        }

        internal virtual ContextMenuBase GetRowContextMenu(TableRow row)
        {
            return null;
        }

        internal Rectangle GetSelectionRect()
        {
            SelectedObjectCollection selection = Report.Designer.SelectedObjects;
            int minX = 1000;
            int minY = 1000;
            int maxX = 0;
            int maxY = 0;
            foreach (Base c in selection)
            {
                if (c is TableCell)
                {
                    TableCell cell = c as TableCell;
                    Point a = cell.Address;
                    if (a.X < minX)
                        minX = a.X;
                    if (a.X > maxX)
                        maxX = a.X;
                    if (a.Y < minY)
                        minY = a.Y;
                    if (a.Y > maxY)
                        maxY = a.Y;
                }
            }
            return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }

        internal virtual void HandleCellDoubleClick(TableCell cell)
        {
            // do nothing
        }

        #endregion Internal Methods

        #region Private Methods

        private void DrawDesign(FRPaintEventArgs e)
        {
            if (IsDesigning && IsSelected)
                e.Graphics.DrawImage(Res.GetImage(75), (int)(AbsLeft * e.ScaleX + 8), (int)(AbsTop * e.ScaleY - 8));
        }

        private void DrawDesign_Borders(FRPaintEventArgs e)
        {
            if (IsDesigning)
                DrawCells(e, DrawDesignBorders);
        }

        private void DrawDesign_BordersRtl(FRPaintEventArgs e)
        {
            if (IsDesigning)
                DrawCellsRtl(e, DrawDesignBorders);
        }

        private void DrawDesign_SelectedCells(FRPaintEventArgs e)
        {
            if (IsDesigning && IsSelected)
            {
                DrawCells(e, DrawSelectedCells);
                DrawSelectedRowsColumns(e);
            }
        }

        private void DrawDesign_SelectedCellsRtl(FRPaintEventArgs e)
        {
            if (IsDesigning && IsSelected)
            {
                DrawCellsRtl(e, DrawSelectedCells);
                DrawSelectedRowsColumns(e);
            }
        }

        private void DrawDesignBorders(FRPaintEventArgs e, TableCell cell)
        {
            if (cell.Fill is SolidFill && (cell.Fill.IsTransparent ||
              (cell.Fill as SolidFill).Color == Color.White))
            {
                cell.DrawMarkers(e, MarkerStyle.Rectangle);
            }
        }

        private void DrawSelectedCells(FRPaintEventArgs e, TableCell cell)
        {
            if (Report.Designer.SelectedObjects.Contains(cell))
            {
                Brush brush = e.Cache.GetBrush(Color.FromArgb(128, SystemColors.Highlight));
                e.Graphics.FillRectangle(brush, cell.AbsLeft * e.ScaleX, cell.AbsTop * e.ScaleY,
                  cell.Width * e.ScaleX, cell.Height * e.ScaleY);
            }
        }

        private void DrawSelectedRowsColumns(FRPaintEventArgs e)
        {
            IGraphics g = e.Graphics;
            SelectedObjectCollection selection = Report.Designer.SelectedObjects;
            Brush brush = e.Cache.GetBrush(Color.FromArgb(128, SystemColors.Highlight));

            foreach (Base c in selection)
            {
                if (c is TableRow)
                {
                    TableRow row = c as TableRow;
                    g.FillRectangle(brush, AbsLeft * e.ScaleX, (row.Top + AbsTop) * e.ScaleY,
                      Width * e.ScaleX, row.Height * e.ScaleY);
                }
                else if (c is TableColumn)
                {
                    TableColumn col = c as TableColumn;
                    g.FillRectangle(brush, (col.Left + AbsLeft) * e.ScaleX, AbsTop * e.ScaleY,
                      col.Width * e.ScaleX, Height * e.ScaleY);
                }
            }
        }

        private Point GetAddressAtMousePoint(PointF pt, bool checkSpan)
        {
            Point result = new Point();
            float left = AbsLeft;
            for (int x = 0; x < Columns.Count; x++)
            {
                float width = Columns[x].Width;
                if (pt.X >= left && pt.X < left + width)
                {
                    result.X = x;
                    break;
                }
                left += width;
            }
            if (pt.X >= AbsRight)
                result.X = Columns.Count - 1;

            float top = AbsTop;
            for (int y = 0; y < Rows.Count; y++)
            {
                float height = Rows[y].Height;
                if (pt.Y >= top && pt.Y < top + height)
                {
                    result.Y = y;
                    break;
                }
                top += height;
            }
            if (pt.Y >= AbsBottom)
                result.Y = Rows.Count - 1;

            if (checkSpan)
            {
                List<Rectangle> spans = GetSpanList();
                foreach (Rectangle span in spans)
                {
                    if (span.Contains(result))
                    {
                        result = span.Location;
                        break;
                    }
                }
            }

            return result;
        }

        private void SetSelection(int x1, int y1, int x2, int y2)
        {
            Rectangle rect = new Rectangle();
            rect.X = x1 < x2 ? x1 : x2;
            rect.Y = y1 < y2 ? y1 : y2;
            rect.Width = (int)Math.Abs(x1 - x2) + 1;
            rect.Height = (int)Math.Abs(y1 - y2) + 1;

            SelectedObjectCollection selection = Report.Designer.SelectedObjects;
            selection.Clear();
            if (mouseMode == MouseMode.SelectRow)
            {
                for (int y = rect.Top; y < rect.Bottom; y++)
                {
                    selection.Add(Rows[y]);
                }
            }
            else if (mouseMode == MouseMode.SelectColumn)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    selection.Add(Columns[x]);
                }
            }
            else if (mouseMode == MouseMode.SelectCell)
            {
                List<Rectangle> spans = GetSpanList();

                // widen selection if spans are inside
                foreach (Rectangle span in spans)
                {
                    if (rect.IntersectsWith(span))
                    {
                        if (span.X < rect.X)
                        {
                            rect.Width += rect.X - span.X;
                            rect.X = span.X;
                        }
                        if (span.Right > rect.Right)
                            rect.Width += span.Right - rect.Right;
                        if (span.Y < rect.Y)
                        {
                            rect.Height += rect.Y - span.Y;
                            rect.Y = span.Y;
                        }
                        if (span.Bottom > rect.Bottom)
                            rect.Height += span.Bottom - rect.Bottom;
                    }
                }

                for (int x = rect.Left; x < rect.Right; x++)
                {
                    for (int y = rect.Top; y < rect.Bottom; y++)
                    {
                        selection.Add(this[x, y]);
                    }
                }
            }
        }

        #endregion Private Methods
    }
}