using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using FastReport.Table;
using FastReport.Data;
using FastReport.Utils;
using FastReport.Forms;

namespace FastReport.Matrix
{
    internal enum MatrixElement
    {
        None,
        Column,
        Row,
        Cell
    }

    partial class MatrixObject
    {
        #region Fields
        internal TableCell selectedCell;
        private bool dragSelectedCell;
        internal DragInfo dragInfo;
        #endregion

        #region Properties
        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public override int ColumnCount
        {
            get { return base.ColumnCount; }
            set { base.ColumnCount = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public override int RowCount
        {
            get { return base.RowCount; }
            set { base.RowCount = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new int FixedRows
        {
            get { return base.FixedRows; }
            set { base.FixedRows = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new int FixedColumns
        {
            get { return base.FixedColumns; }
            set { base.FixedColumns = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new bool CanBreak
        {
            get { return base.CanBreak; }
            set { base.CanBreak = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new bool GrowToBottom
        {
            get { return base.GrowToBottom; }
            set { base.GrowToBottom = value; }
        }

        private bool DragCellMode
        {
            get { return selectedCell != null && dragSelectedCell; }
        }
        #endregion

        #region Private Methods
        internal void RefreshTemplate(bool reset)
        {
            Helper.UpdateDescriptors();

            for (int x = 0; x < Helper.BodyWidth; x++)
            {
                for (int y = 0; y < Helper.BodyHeight; y++)
                {
                    TableCell cell = this[x + FixedColumns, y + FixedRows];
                    if (reset)
                        cell.Text = "";
                    else
                        cell.SetFlags(Flags.CanEdit, false);
                }
            }

            MatrixElement element;
            MatrixDescriptor descriptor;
            bool isTotal;

            for (int x = 0; x < ColumnCount; x++)
            {
                for (int y = 0; y < RowCount; y++)
                {
                    TableCell cell = this[x, y];
                    GetMatrixElement(cell, out element, out descriptor, out isTotal);
                    bool enableSmartTag = descriptor != null && !isTotal;
                    cell.SetFlags(Flags.HasSmartTag, enableSmartTag);
                }
            }
        }

        internal void DrawSelectedCellFrame(FRPaintEventArgs e, TableCell cell)
        {
            Pen p = e.Cache.GetPen(Color.Black, 3, System.Drawing.Drawing2D.DashStyle.Solid);
            e.Graphics.DrawRectangle(p, cell.AbsLeft * e.ScaleX, cell.AbsTop * e.ScaleY,
              cell.Width * e.ScaleX, cell.Height * e.ScaleY);
        }

        internal void DrawDragIndicator(FRPaintEventArgs e)
        {
            dragInfo.targetCell.DrawDragAcceptFrame(e, Color.Red);

            if (dragInfo.indicator.Width > 0 || dragInfo.indicator.Height > 0)
            {
                IGraphics g = e.Graphics;
                int left = (int)Math.Round((dragInfo.indicator.Left + AbsLeft) * e.ScaleX);
                int top = (int)Math.Round((dragInfo.indicator.Top + AbsTop) * e.ScaleY);
                int right = (int)Math.Round(dragInfo.indicator.Width * e.ScaleX) + left;
                int bottom = (int)Math.Round(dragInfo.indicator.Height * e.ScaleY) + top;

                Pen p = e.Cache.GetPen(Color.Red, 1, DashStyle.Solid);
                g.DrawLine(p, left, top, right, bottom);

                p = Pens.Red;
                Brush b = Brushes.Red;

                if (dragInfo.indicator.Width == 0)
                {
                    Point[] poly = new Point[] {
            new Point(left, top),
            new Point(left - 4, top - 4),
            new Point(left + 4, top - 4),
            new Point(left, top) };
                    g.FillPolygon(b, poly);
                    g.DrawPolygon(p, poly);

                    poly = new Point[] {
            new Point(left, bottom),
            new Point(left - 4, bottom + 4),
            new Point(left + 4, bottom + 4),
            new Point(left, bottom) };
                    g.FillPolygon(b, poly);
                    g.DrawPolygon(p, poly);
                }
                else
                {
                    Point[] poly = new Point[] {
            new Point(left, top),
            new Point(left - 4, top - 4),
            new Point(left - 4, top + 4),
            new Point(left, top) };
                    g.FillPolygon(b, poly);
                    g.DrawPolygon(p, poly);

                    poly = new Point[] {
            new Point(right, top),
            new Point(right + 4, top - 4),
            new Point(right + 4, top + 4),
            new Point(right, top) };
                    g.FillPolygon(b, poly);
                    g.DrawPolygon(p, poly);
                }
            }
        }

        private void GetMatrixElement(TableCell cell, out MatrixElement element, out MatrixDescriptor descriptor,
          out bool isTotal)
        {
            element = MatrixElement.None;
            descriptor = null;
            isTotal = false;

            bool noColumns = Data.Columns.Count == 0;
            bool noRows = Data.Rows.Count == 0;
            bool noCells = Data.Cells.Count == 0;

            // create temporary descriptors
            if (noColumns)
                Data.Columns.Add(new MatrixHeaderDescriptor("", false));
            if (noRows)
                Data.Rows.Add(new MatrixHeaderDescriptor("", false));
            if (noCells)
                Data.Cells.Add(new MatrixCellDescriptor());

            Helper.UpdateDescriptors();

            foreach (MatrixHeaderDescriptor descr in Data.Columns)
            {
                if (descr.TemplateCell == cell || descr.TemplateTotalCell == cell)
                {
                    element = MatrixElement.Column;
                    if (!noColumns)
                        descriptor = descr;
                    isTotal = descr.TemplateTotalCell == cell;
                }
            }

            foreach (MatrixHeaderDescriptor descr in Data.Rows)
            {
                if (descr.TemplateCell == cell || descr.TemplateTotalCell == cell)
                {
                    element = MatrixElement.Row;
                    if (!noRows)
                        descriptor = descr;
                    isTotal = descr.TemplateTotalCell == cell;
                }
            }

            foreach (MatrixCellDescriptor descr in Data.Cells)
            {
                if (descr.TemplateCell == cell)
                {
                    element = MatrixElement.Cell;
                    if (!noCells)
                        descriptor = descr;
                }
            }

            if (cell.Address.X >= FixedColumns && cell.Address.Y >= FixedRows)
                element = MatrixElement.Cell;

            if (noColumns)
                Data.Columns.Clear();
            if (noRows)
                Data.Rows.Clear();
            if (noCells)
                Data.Cells.Clear();
        }
        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        protected override SelectionPoint[] GetSelectionPoints()
        {
            if (AutoSize)
                return new SelectionPoint[] { new SelectionPoint(AbsLeft, AbsTop, SizingPoint.LeftTop) };
            return base.GetSelectionPoints();
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            if (IsDesigning && AutoSize)
            {
                AutoSize = true;
                CalcHeight();
            }

            base.Draw(e);

            if (selectedCell != null)
                DrawSelectedCellFrame(e, selectedCell);

            if (dragInfo.target != MatrixElement.None)
                DrawDragIndicator(e);

            if (!IsResultMatrix)
                RefreshTemplate(false);
        }

        /// <inheritdoc/>
        public override void SelectionChanged()
        {
            base.SelectionChanged();

            this.selectedCell = null;
            dragInfo.source = MatrixElement.None;
            dragInfo.sourceDescriptor = null;
            if (!IsSelected || Report.Designer.SelectedObjects.Count != 1)
                return;

            TableCell selectedCell = Report.Designer.SelectedObjects[0] as TableCell;
            if (selectedCell == null)
                return;

            bool isTotal;
            GetMatrixElement(selectedCell, out dragInfo.source, out dragInfo.sourceDescriptor, out isTotal);

            if (dragInfo.sourceDescriptor == null || isTotal)
            {
                dragInfo.source = MatrixElement.None;
                dragInfo.sourceDescriptor = null;
            }
            else
                this.selectedCell = selectedCell;
        }

        /// <inheritdoc/>
        public override void HandleMouseDown(FRMouseEventArgs e)
        {
            if (DragCellMode)
            {
                e.handled = true;
                e.mode = WorkspaceMode2.Custom;
                e.activeObject = this;
            }
            else
                base.HandleMouseDown(e);
        }

        /// <inheritdoc/>
        public override void HandleMouseMove(FRMouseEventArgs e)
        {
            if (DragCellMode && e.button == MouseButtons.Left)
            {
                e.DragSource = selectedCell;
                HandleDragOver(e);
            }
            else
            {
                base.HandleMouseMove(e);

                if (AutoSize && (MouseMode == MouseMode.ResizeColumn || MouseMode == MouseMode.ResizeRow))
                {
                    MouseMode = MouseMode.None;
                    e.handled = false;
                    e.mode = WorkspaceMode2.None;
                    e.cursor = Cursors.Default;
                }

                if (selectedCell != null && e.button == MouseButtons.None)
                {
                    PointF point = new PointF(e.x, e.y);
                    RectangleF innerRect = selectedCell.AbsBounds;
                    RectangleF outerRect = innerRect;
                    innerRect.Inflate(-3, -3);
                    outerRect.Inflate(3, 3);

                    dragSelectedCell = outerRect.Contains(point) && !innerRect.Contains(point);
                    if (dragSelectedCell)
                    {
                        e.handled = true;
                        e.cursor = Cursors.SizeAll;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void HandleMouseUp(FRMouseEventArgs e)
        {
            if (DragCellMode)
            {
                HandleDragDrop(e);
                Report.Designer.SetModified(this, "Change");
                selectedCell = null;
                dragSelectedCell = false;
            }
            else
                base.HandleMouseUp(e);
        }

        /// <inheritdoc/>
        public override void HandleDragOver(FRMouseEventArgs e)
        {
            // matrix that is defined in the base report cannot be configured such way.
            if (IsAncestor)
                return;

            dragInfo.target = MatrixElement.None;
            dragInfo.indicator = Rectangle.Empty;
            if (!(e.DragSource is TextObject))
                return;

            bool noColumns = Data.Columns.Count == 0;
            bool noRows = Data.Rows.Count == 0;
            bool noCells = Data.Cells.Count == 0;

            // create temporary descriptors
            if (noColumns)
                Data.Columns.Add(new MatrixHeaderDescriptor("", false));
            if (noRows)
                Data.Rows.Add(new MatrixHeaderDescriptor("", false));
            if (noCells)
                Data.Cells.Add(new MatrixCellDescriptor());

            Helper.UpdateDescriptors();
            PointF point = new PointF(e.x, e.y);

            // determine the location where to insert a new item: column, row or cell
            foreach (MatrixHeaderDescriptor descr in Data.Columns)
            {
                TableCell cell = descr.TemplateCell;
                if (cell != e.DragSource && cell.PointInObject(point))
                {
                    dragInfo.target = MatrixElement.Column;
                    dragInfo.targetIndex = Data.Columns.IndexOf(descr) + 1;
                    dragInfo.targetCell = cell;
                    float top = cell.Bottom;
                    float width = cell.Width;

                    if (noColumns || point.Y < cell.AbsTop + cell.Height / 2)
                    {
                        if (descr.TemplateTotalCell != null)
                            width += descr.TemplateTotalCell.Width;
                        top = cell.Top;
                        dragInfo.targetIndex--;
                    }

                    dragInfo.indicator = new RectangleF(cell.Left, top, noColumns ? 0 : width, 0);
                    e.dragMessage = Res.Get("ComponentsMisc,Matrix,NewColumn");
                }
            }

            foreach (MatrixHeaderDescriptor descr in Data.Rows)
            {
                TableCell cell = descr.TemplateCell;
                if (cell != e.DragSource && cell.PointInObject(point))
                {
                    dragInfo.target = MatrixElement.Row;
                    dragInfo.targetIndex = Data.Rows.IndexOf(descr) + 1;
                    dragInfo.targetCell = cell;
                    float left = cell.Right;
                    float height = cell.Height;

                    if (noRows || point.X < cell.AbsLeft + cell.Width / 2)
                    {
                        if (descr.TemplateTotalCell != null)
                            height += descr.TemplateTotalCell.Height;
                        left = cell.Left;
                        dragInfo.targetIndex--;
                    }

                    dragInfo.indicator = new RectangleF(left, cell.Top, 0, noRows ? 0 : height);
                    e.dragMessage = Res.Get("ComponentsMisc,Matrix,NewRow");
                }
            }

            foreach (MatrixCellDescriptor descr in Data.Cells)
            {
                TableCell cell = descr.TemplateCell;
                if (cell != e.DragSource && cell.PointInObject(point))
                {
                    dragInfo.target = MatrixElement.Cell;
                    dragInfo.targetCell = cell;
                    bool preferLeftRight = Math.Min(point.X - cell.AbsLeft, cell.AbsRight - point.X) < 10;

                    if (Data.Cells.Count < 2 || CellsSideBySide)
                    {
                        dragInfo.targetIndex = Data.Cells.IndexOf(descr) + 1;
                        dragInfo.cellsSideBySide = true;
                        float left = cell.Right;

                        if (point.X < cell.AbsLeft + cell.Width / 2)
                        {
                            left = cell.Left;
                            dragInfo.targetIndex--;
                        }

                        dragInfo.indicator = new RectangleF(left, cell.Top, 0, cell.Height);
                    }
                    if ((Data.Cells.Count < 2 && !preferLeftRight) || (Data.Cells.Count >= 2 && !CellsSideBySide))
                    {
                        dragInfo.targetIndex = Data.Cells.IndexOf(descr) + 1;
                        dragInfo.cellsSideBySide = false;
                        float top = cell.Bottom;

                        if (point.Y < cell.AbsTop + cell.Height / 2)
                        {
                            top = cell.Top;
                            dragInfo.targetIndex--;
                        }

                        dragInfo.indicator = new RectangleF(cell.Left, top, cell.Width, 0);
                    }

                    if (noCells)
                    {
                        dragInfo.targetIndex = 0;
                        dragInfo.indicator = RectangleF.Empty;
                    }

                    e.dragMessage = Res.Get("ComponentsMisc,Matrix,NewCell");
                }
            }

            if (noColumns)
                Data.Columns.Clear();
            if (noRows)
                Data.Rows.Clear();
            if (noCells)
                Data.Cells.Clear();

            e.handled = PointInObject(point);
        }

        /// <inheritdoc/>
        public override void HandleDragDrop(FRMouseEventArgs e)
        {
            if (dragInfo.target != MatrixElement.None)
            {
                string draggedText = (e.DragSource as TextObject).Text;
                MatrixDescriptor sourceDescr = dragInfo.sourceDescriptor;
                MatrixDescriptor targetDescr = null;

                // insert new item.
                switch (dragInfo.target)
                {
                    case MatrixElement.Column:
                        targetDescr = new MatrixHeaderDescriptor(draggedText);
                        if (sourceDescr != null)
                            targetDescr.Assign(sourceDescr);
                        if (dragInfo.source == MatrixElement.Cell)
                            targetDescr.TemplateCell = null;
                        Data.Columns.Insert(dragInfo.targetIndex, targetDescr as MatrixHeaderDescriptor);
                        break;

                    case MatrixElement.Row:
                        targetDescr = new MatrixHeaderDescriptor(draggedText);
                        if (sourceDescr != null)
                            targetDescr.Assign(sourceDescr);
                        if (dragInfo.source == MatrixElement.Cell)
                            targetDescr.TemplateCell = null;
                        Data.Rows.Insert(dragInfo.targetIndex, targetDescr as MatrixHeaderDescriptor);
                        break;

                    case MatrixElement.Cell:
                        targetDescr = new MatrixCellDescriptor(draggedText);
                        if (sourceDescr != null)
                            targetDescr.Assign(sourceDescr);
                        if (dragInfo.source != MatrixElement.Cell)
                            targetDescr.TemplateCell = null;
                        CellsSideBySide = dragInfo.cellsSideBySide;
                        Data.Cells.Insert(dragInfo.targetIndex, targetDescr as MatrixCellDescriptor);
                        break;
                }

                // remove source item
                switch (dragInfo.source)
                {
                    case MatrixElement.Column:
                        Data.Columns.Remove(sourceDescr as MatrixHeaderDescriptor);
                        break;

                    case MatrixElement.Row:
                        Data.Rows.Remove(sourceDescr as MatrixHeaderDescriptor);
                        break;

                    case MatrixElement.Cell:
                        Data.Cells.Remove(sourceDescr as MatrixCellDescriptor);
                        break;
                }

                if (DataSource == null)
                {
                    if (draggedText.StartsWith("[") && draggedText.EndsWith("]"))
                        draggedText = draggedText.Substring(1, draggedText.Length - 2);
                    DataSource = DataHelper.GetDataSource(Report.Dictionary, draggedText);
                }

                Helper.BuildTemplate();
                if (targetDescr != null)
                {
                    Report.Designer.SelectedObjects.Clear();
                    Report.Designer.SelectedObjects.Add(targetDescr.TemplateCell);
                }
            }

            dragInfo.target = MatrixElement.None;
            dragInfo.source = MatrixElement.None;
        }

        /// <inheritdoc/>
        public override void OnBeforeInsert(int flags)
        {
            BuildTemplate();
        }

        /// <inheritdoc/>
        public override ContextMenuBase GetContextMenu()
        {
            return new MatrixObjectMenu(Report.Designer);
        }

        internal override ContextMenuBase GetCellContextMenu(TableCell cell)
        {
            if (Report.Designer.SelectedObjects.Count == 1)
            {
                MatrixElement element;
                MatrixDescriptor descriptor;
                bool isTotal;
                GetMatrixElement(cell, out element, out descriptor, out isTotal);

                switch (element)
                {
                    case MatrixElement.Column:
                    case MatrixElement.Row:
                        if (isTotal)
                            return new MatrixTotalMenu(this, element, descriptor);
                        else if (descriptor != null)
                            return new MatrixHeaderMenu(this, element, descriptor);
                        break;

                    case MatrixElement.Cell:
                        if (descriptor != null)
                            return new MatrixCellMenu(this, element, descriptor);
                        break;
                }
            }

            return new MatrixCellMenuBase(this, MatrixElement.None, null);
        }

        internal override SmartTagBase GetCellSmartTag(TableCell cell)
        {
            MatrixElement element;
            MatrixDescriptor descriptor;
            bool isTotal;
            GetMatrixElement(cell, out element, out descriptor, out isTotal);

            if (descriptor != null && !isTotal)
                return new MatrixCellSmartTag(this, descriptor);
            return null;
        }

        internal override void HandleCellDoubleClick(TableCell cell)
        {
            MatrixElement element;
            MatrixDescriptor descriptor;
            bool isTotal;
            GetMatrixElement(cell, out element, out descriptor, out isTotal);

            if (descriptor != null && !isTotal)
            {
                using (ExpressionEditorForm form = new ExpressionEditorForm(Report))
                {
                    form.ExpressionText = descriptor.Expression;
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        descriptor.Expression = form.ExpressionText;
                        BuildTemplate();
                        Report.Designer.SetModified(cell, "Change");
                    }
                }
            }
            else
            {
                if (cell.HasFlag(Flags.CanEdit) && !cell.HasRestriction(Restrictions.DontEdit) && cell.InvokeEditor())
                    Report.Designer.SetModified(cell, "Change");
            }
        }

        /// <inheritdoc/>
        public override void HandleKeyDown(Control sender, KeyEventArgs e)
        {
            bool myKey = false;
            SelectedObjectCollection selection = Report.Designer.SelectedObjects;
            if (!IsSelected || !(selection[0] is TableCell))
                return;

            TableCell cell = selection[0] as TableCell;
            MatrixElement element;
            MatrixDescriptor descriptor;
            bool isTotal;
            GetMatrixElement(cell, out element, out descriptor, out isTotal);

            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if (element != MatrixElement.None)
                    {
                        if (descriptor != null && !IsAncestor)
                        {
                            if (isTotal)
                                (descriptor as MatrixHeaderDescriptor).Totals = false;
                            else
                            {
                                switch (element)
                                {
                                    case MatrixElement.Column:
                                        Data.Columns.Remove(descriptor as MatrixHeaderDescriptor);
                                        break;

                                    case MatrixElement.Row:
                                        Data.Rows.Remove(descriptor as MatrixHeaderDescriptor);
                                        break;

                                    case MatrixElement.Cell:
                                        Data.Cells.Remove(descriptor as MatrixCellDescriptor);
                                        break;
                                }
                            }
                        }

                        myKey = true;
                    }
                    break;

                case Keys.Enter:
                    if (descriptor != null && !isTotal)
                    {
                        HandleCellDoubleClick(cell);
                        myKey = true;
                    }
                    break;
            }

            if (myKey)
            {
                e.Handled = true;
                BuildTemplate();
                Report.Designer.SetModified(this, "Change");
            }
            else
                base.HandleKeyDown(sender, e);
        }
        #endregion

        private void InitDesign()
        {
            dragInfo = new DragInfo();
        }


        internal class DragInfo
        {
            public MatrixElement source;
            public MatrixElement target;
            public MatrixDescriptor sourceDescriptor;
            public int targetIndex;
            public RectangleF indicator;
            public bool cellsSideBySide;
            public TableCell targetCell;
        }
    }
}