using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Utils;
using System.ComponentModel;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport
{
    partial class ContainerObject
    {
        #region Public Methods
        /// <inheritdoc/>
        public override void DrawSelection(FRPaintEventArgs e)
        {
            base.DrawSelection(e);
            e.Graphics.DrawImage(Res.GetImage(75), (int)(AbsLeft * e.ScaleX + DpiHelper.ConvertUnits(8)), (int)(AbsTop * e.ScaleY - DpiHelper.ConvertUnits(8)));
        }

        /// <inheritdoc/>
        public override void HandleMouseDown(FRMouseEventArgs e)
        {
            HandleMouseHover(e);
            if (e.handled)
                e.mode = WorkspaceMode2.Move;
            else
            {
                base.HandleMouseDown(e);
                if (e.handled)
                {
                    if (e.modifierKeys != Keys.Shift)
                    {
                        e.mode = WorkspaceMode2.SelectionRect;
                        e.activeObject = this;
                    }
                }
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
        public override void HandleMouseUp(FRMouseEventArgs e)
        {
            base.HandleMouseUp(e);
            if (e.activeObject == this && e.mode == WorkspaceMode2.SelectionRect)
            {
                ObjectCollection selectedList = new ObjectCollection();
                // find objects inside the selection rect
                foreach (ReportComponentBase c in Objects)
                {
                    e.handled = false;
                    c.HandleMouseUp(e);
                    // object is inside
                    if (e.handled)
                        selectedList.Add(c);
                }
                if (selectedList.Count > 0)
                    selectedList.CopyTo(Report.Designer.SelectedObjects);
            }
        }
        #endregion
    }
}
