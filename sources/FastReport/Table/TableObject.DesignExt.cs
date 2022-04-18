using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FastReport.Design;
using FastReport.Utils;
using FastReport.Engine;
using FastReport.Preview;
using FastReport.Design.PageDesigners.Page;
using FastReport.Data;
using FastReport.Controls;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FastReport.Table
{
    partial class TableObject
    {
        #region Fields
        private TableCell dragCell;
        private SelectedObjectCollection itemsBeforeMDown;
        private bool isResizing;
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            base.Draw(e);

            if (dragCell != null)
                dragCell.DrawDragAcceptFrame(e, Color.Silver);
            DrawDesign(e);
        }

        /// <inheritdoc/>
        public override void HandleDragOver(FRMouseEventArgs e)
        {
            dragCell = null;
            if (!(e.DragSource is TextObject))
                return;

            for (int y = 0; y < Rows.Count; y++)
            {
                for (int x = 0; x < Columns.Count; x++)
                {
                    TableCell cell = this[x, y];
                    if (!IsInsideSpan(cell) && cell.PointInObject(new PointF(e.x, e.y)))
                    {
                        dragCell = cell;
                        e.handled = true;
                        break;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void HandleMouseDown(FRMouseEventArgs e)
        {
            base.HandleMouseDown(e);
            HandleMouseHover(e);
            if (IsSelected && new RectangleF(AbsRight + 1, AbsBottom + 1, 16, 16).Contains(new PointF(e.x, e.y)))
            {
                e.handled = true;
                e.mode = WorkspaceMode2.Size;
                if (itemsBeforeMDown == null)
                    itemsBeforeMDown = new SelectedObjectCollection();
                Report.Designer.SelectedObjects.CopyTo(itemsBeforeMDown);
                Report.Designer.SelectedObjects.Clear();
                foreach (ComponentBase line in ChildObjects)
                {
                    if (line is TableColumn)
                        Report.Designer.SelectedObjects.Add(line);
                }

                e.activeObject = this.Rows[0].ChildObjects[0] as TableCell;
                isResizing = true;
                e.cursor = Cursors.PanSW;
            }
        }

        /// <inheritdoc/>
        public override void HandleMouseUp(FRMouseEventArgs e)
        {
            base.HandleMouseUp(e);
            if (itemsBeforeMDown != null && itemsBeforeMDown.Count != 0)
            {
                Report.Designer.SelectedObjects.Clear();

                foreach(ComponentBase item in itemsBeforeMDown)
                {
                    if (AllObjects.Contains(item))
                        Report.Designer.SelectedObjects.Add(item);
                }

                if (Report.Designer.SelectedObjects.Count == 0)
                    Report.Designer.SelectedObjects.Add(this);

                itemsBeforeMDown.Clear();
            }
            isResizing = false;
        }

        /// <inheritdoc/>
        public override void HandleMouseMove(FRMouseEventArgs e)
        {
            base.HandleMouseMove(e);
            if (IsSelected && new RectangleF(AbsRight + 2, AbsBottom + 3, 16, 16).Contains(new PointF(e.x, e.y)))
            {
                e.cursor = Cursors.PanSE;
                e.handled = true;
            }
            if (!(e.mode == WorkspaceMode2.Size && e.activeObject == this.Rows[0].ChildObjects[0] as TableCell && isResizing))
                return;

            float longestHeigh = (Rows.ToArray().ToList().Last() as TableRow).ChildObjects.ToArray().ToList().Max((x => (x as TableCell).Height));
            float longestWidth = (Rows.ToArray().ToList().Last() as TableRow).ChildObjects.ToArray().ToList().Max((x => (x as TableCell).Width));
            System.Diagnostics.Debug.WriteLine(longestHeigh);
            bool wasChanged = false;
            if (e.y > AbsBottom + 19)
            {
                TableRow row = new TableRow();
                Rows.Insert(Rows.Count, row);
                CreateUniqueNames();
                wasChanged = true;
            }
            else if (e.y < AbsBottom - longestHeigh && Rows.Count > 1)
            {
                wasChanged = true;
                Rows.RemoveAt(Rows.Count - 1);
            }

            if (e.x > AbsRight + longestWidth)
            {
                TableColumn column = new TableColumn();
                Columns.Insert(Columns.Count, column);
                CreateUniqueNames();
                wasChanged = true;
            }
            else if (e.x < AbsRight - longestWidth && Columns.Count > 1)
            {
                wasChanged = true;
                Columns.RemoveAt(Columns.Count - 1);
            }
            if (wasChanged)
            {
                Report.Designer.SelectedObjects.Clear();
                foreach (ComponentBase line in ChildObjects)
                {
                    if (line is TableColumn)
                        Report.Designer.SelectedObjects.Add(line);
                }
            }
        }

        /// <inheritdoc/>
        public override void HandleDragDrop(FRMouseEventArgs e)
        {
            dragCell.Text = (e.DragSource as TextObject).Text;
            dragCell = null;
        }

        internal override ContextMenuBase GetColumnContextMenu(TableColumn column)
        {
            return new TableColumnMenu(Report.Designer);
        }

        internal override ContextMenuBase GetRowContextMenu(TableRow row)
        {
            return new TableRowMenu(Report.Designer);
        }

        internal override ContextMenuBase GetCellContextMenu(TableCell cell)
        {
            return new TableCellMenu(Report.Designer);
        }

        internal override SmartTagBase GetCellSmartTag(TableCell cell)
        {
            return new TextObjectSmartTag(cell);
        }

        internal override void HandleCellDoubleClick(TableCell cell)
        {
            if (!cell.HasRestriction(Restrictions.DontEdit) && cell.InvokeEditor())
                Report.Designer.SetModified(this, "Change");
        }
        #endregion

        #region private methods
        private void DrawDesign(FRPaintEventArgs e)
        {
            if (IsDesigning && IsSelected)
                e.Graphics.DrawImage(Res.GetImage(152), (int)(AbsRight * e.ScaleX + 2), (int)(AbsBottom * e.ScaleY + 3));
        }
        #endregion
    }
}
