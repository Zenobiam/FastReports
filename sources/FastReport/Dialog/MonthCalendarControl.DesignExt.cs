using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Utils;
using System.Drawing.Drawing2D;

namespace FastReport.Dialog
{
  partial class MonthCalendarControl
  {
    #region Protected Methods
    /// <inheritdoc/>
    protected override bool ShouldSerializeBackColor()
    {
      return BackColor != SystemColors.Window;
    }

    /// <inheritdoc/>
    protected override bool ShouldSerializeForeColor()
    {
      return ForeColor != SystemColors.WindowText;
    }

    /// <inheritdoc/>
    protected bool ShouldSerializeCalendarDimensions()
    {
      return CalendarDimensions.Width != 1 || CalendarDimensions.Height != 1;
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
            //g.DrawRectangle(pen, AbsLeft * e.ScaleX - 2, AbsTop * e.ScaleY - 2, bounds.Width * e.ScaleX + 3, bounds.Height * e.ScaleY + 3);
            g.DrawRectangle(pen, AbsLeft * e.ScaleX - 2, AbsTop * e.ScaleY - 2, bounds.Width + 3, bounds.Height + 3);
            if (selectionPoints.Length == 1)
                DrawSelectionPoint(e, p, b, selectionPoints[0].x, selectionPoints[0].y);
            else
            {
                foreach (SelectionPoint pt in selectionPoints)
                {
                    DrawSelectionPoint(e, p, b, pt.x, pt.y);
                }
            }
        }
          

    /// <inheritdoc/>
    protected override SelectionPoint[] GetSelectionPoints()
    {
      return new SelectionPoint[] { new SelectionPoint(AbsLeft - 2, AbsTop - 2, SizingPoint.None) };
    }
    #endregion

    #region Public methods

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            if(Control.Size != bounds.Size)
                bounds = new RectangleF(AbsBounds.Left, AbsBounds.Top, Control.Width, Control.Height);
            if (Control.Width > 0 && Control.Height > 0)
            {
                using (Bitmap bmp = DrawUtils.DrawToBitmap(Control, true))
                {
                    //using (Bitmap bmpScale = new Bitmap(bmp, new Size((int)bounds.Width, (int)bounds.Height)))
                        e.Graphics.DrawImage(bmp, (int)AbsLeft * e.ScaleX, (int)AbsTop * e.ScaleY);
                }
            }
            if (IsDesigning)
            {
                if (IsAncestor)
                    e.Graphics.DrawImage(Res.GetImage(99), (int)(AbsRight * e.ScaleX - 9), (int)(AbsTop * e.ScaleY + 2));
            }
        }
        
        ///<inheritdoc/>
        public override bool PointInObject(PointF point)
        {
            return new RectangleF(bounds.X,bounds.Y, bounds.Width/DpiScale, bounds.Height / DpiScale).Contains(point);
        }     

    #endregion

  }
}
