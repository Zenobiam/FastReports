using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Data;
using System.Drawing.Drawing2D;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Dialog
{
  partial class CheckBoxControl
  {
        #region Protected Methods

    /// <inheritdoc/>
    protected override SelectionPoint[] GetSelectionPoints()
    {
      if (CheckBox.AutoSize)
        return new SelectionPoint[] { new SelectionPoint(AbsLeft - 2, AbsTop - 2, SizingPoint.None) };
      return base.GetSelectionPoints();
    }
        #endregion

        #region Public Methods        

        ///<inheritdoc/>
        public override void ReinitDpiSize()
        {
            base.ReinitDpiSize();
            if (!AutoSize)
                DrawControl.Size = DpiHelper.ConvertUnits(Control.Size);
            else
                DrawControl.Size = DrawControl.PreferredSize;
        }

        /// <inheritdoc/>
        public override void ScaleControl()
        {
            base.ScaleControl();
            if (!(Control as ButtonBase).AutoSize)
                Control.Size = DpiHelper.ConvertUnits(Control.Size);
        }

        ///<inheritdoc/>
        public override void DrawSelection(FRPaintEventArgs e)
        {
            IGraphics g = e.Graphics;
            bool firstSelected = Report.Designer.SelectedObjects.IndexOf(this) == 0;
            Pen p = firstSelected ? Pens.Black : Pens.White;
            Brush b = firstSelected ? Brushes.White : Brushes.Black;
            SelectionPoint[] selectionPoints = GetSelectionPoints();

            Pen pen = e.Cache.GetPen(Color.Gray, 1, DashStyle.Dot); 
            float tempFont = Control.Font.Size;
            Control.Font = new Font(Control.Font.Name, DrawFont.Size, Control.Font.Style);
            if (CheckBox.AutoSize)
                g.DrawRectangle(pen, AbsLeft * e.ScaleX - 2, AbsTop * e.ScaleY - 2, Width + 3, Height + 3);
            else
                g.DrawRectangle(pen, AbsLeft * e.ScaleX - 2, AbsTop * e.ScaleY - 2, Width * e.ScaleX + 3, Height * e.ScaleY + 3);
            if (selectionPoints.Length == 1)
                DrawSelectionPoint(e, p, b, selectionPoints[0].x, selectionPoints[0].y);
            else
            {
                foreach (SelectionPoint pt in selectionPoints)
                {
                    DrawSelectionPoint(e, p, b, pt.x, pt.y);
                }
            }
            Control.Font = new Font(Control.Font.Name, tempFont, Control.Font.Style);
        }

        public override void Draw(FRPaintEventArgs e)
        {
            if (Control.Width > 0 && Control.Height > 0)
            {
                float tempFont = Control.Font.Size;
                Control.Font = new Font(Control.Font.Name, DrawFont.Size, Control.Font.Style);
                Size sz = Control.Size;
                if (!AutoSize && Control.Size != new Size((int)(Control.Width * e.ScaleX), (int)(Control.Height * e.ScaleY)))
                    Control.Size = new Size((int)(Control.Width * e.ScaleX), (int)(Control.Height * e.ScaleY));
                using (Bitmap bmp = DrawUtils.DrawToBitmap(Control, true))
                {
                    e.Graphics.DrawImage(bmp, (int)AbsLeft * e.ScaleX, (int)AbsTop * e.ScaleY);
                }
                Control.Font = new Font(Control.Font.Name, tempFont, Control.Font.Style);
                if (!AutoSize)
                    Control.Size = sz;
            }
        }

        #endregion
    }
}
