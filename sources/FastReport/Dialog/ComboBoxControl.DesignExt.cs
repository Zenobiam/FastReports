using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Data;
using System.Collections;
using FastReport.Utils;
using FastReport.TypeEditors;
using FastReport.Data;
using FastReport.TypeConverters;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Dialog
{
  partial class ComboBoxControl
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
                    DrawControl.Width = DpiHelper.ConvertUnits((int)value);
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
    protected override bool ShouldSerializeForeColor()
    {
      return ForeColor != SystemColors.WindowText;
    }

    /// <inheritdoc/>
    protected override SelectionPoint[] GetSelectionPoints()
    {
      if (ComboBox.DrawMode == DrawMode.Normal)
        return new SelectionPoint[] { 
          new SelectionPoint(AbsLeft - 2, AbsTop + (DrawControl.Height / DpiHelper.Multiplier) / 2, SizingPoint.LeftCenter),
          new SelectionPoint(AbsLeft + Width + 1, AbsTop + (DrawControl.Height / DpiHelper.Multiplier) / 2, SizingPoint.RightCenter) };
      return base.GetSelectionPoints();
    }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void ScaleControl()
        {
            base.ScaleControl();
            Control.Width = DpiHelper.ConvertUnits(Control.Width);
        }

        ///<inheritdoc/>
        public override void ReinitDpiSize()
        {
            base.ReinitDpiSize();
            DrawControl.Width = DpiHelper.ConvertUnits(Control.Width);
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
        #endregion
    }
}
