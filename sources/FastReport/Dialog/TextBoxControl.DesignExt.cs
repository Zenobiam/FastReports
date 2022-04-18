using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
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
  partial class TextBoxControl
  {

        #region Properties
        /// <inheritdoc/>
        public override float Width
        {
            get { return base.Width; }
            set
            {
                if (!IsDesigning || !HasRestriction(Restrictions.DontResize))
                {
                    Control.Width = (int)value;
                    DrawControl.Width = DpiHelper.ConvertUnits(Control.Width);
                }
            }
        }

        /// <inheritdoc/>
        public override float Height
        {
            get { return base.Height; }
            set
            {
                if (!IsDesigning || !HasRestriction(Restrictions.DontResize))
                {
                    Control.Height = (int)value;
                    DrawControl.Height = DpiHelper.ConvertUnits(Control.Height);
                }
            }
        }

        #endregion

    #region Protected Methods

    /// <inheritdoc/>
    protected override bool ShouldSerializeBackColor()
    {
      return BackColor != SystemColors.Window;
    }

    /// <inheritdoc/>
    protected override bool ShouldSerializeCursor()
    {
      return Cursor != Cursors.IBeam;
    }

    /// <inheritdoc/>
    protected override bool ShouldSerializeForeColor()
    {
      return ForeColor != SystemColors.WindowText;
    }

    /// <inheritdoc/>
    protected override SelectionPoint[] GetSelectionPoints()
    {
      if (!TextBox.Multiline)
        return new SelectionPoint[] { 
          new SelectionPoint(AbsLeft - 2, AbsTop + (Height / DpiHelper.Multiplier) / 2, SizingPoint.LeftCenter),
          new SelectionPoint(AbsLeft + Width + 1, AbsTop + (Height / DpiHelper.Multiplier) / 2, SizingPoint.RightCenter) };
      return base.GetSelectionPoints();
    }

        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void ScaleControl()
        {
            base.ScaleControl();
            if (!(Control as TextBox).Multiline)
                Control.Width = DpiHelper.ConvertUnits(Control.Width);
            else
                Control.Size = DpiHelper.ConvertUnits(Control.Size);
        }

        ///<inheritdoc/>
        public override void ReinitDpiSize()
        {
            base.ReinitDpiSize();
            DrawControl.Size = DpiHelper.ConvertUnits(Control.Size);
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
            g.DrawRectangle(pen, AbsLeft * e.ScaleX - 2, AbsTop * e.ScaleY - 2, DrawControl.Width + 3, DrawControl.Height + 3);
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

        public override void Draw(FRPaintEventArgs e)
        {
            Size sz = textBox.Size;
            if (textBox.Multiline) 
                textBox.Size = DpiHelper.ConvertUnits(sz);
            else 
                textBox.Width = DpiHelper.ConvertUnits(textBox.Width);
            base.Draw(e);
            Control.Size = sz;
        }
        #endregion
    }
}
