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
  partial class ButtonControl
  {
    #region Properties
        /// <inheritdoc/>
        public override float Width 
        {
            get
            {
                return base.Width;
            }
            set
            {
                base.Width = value;
                if (!IsDesigning || !HasRestriction(Restrictions.DontResize))
                {
                    if(!AutoSize)
                        DrawControl.Width = (int)(value * DpiHelper.Multiplier);
                }
            }
        }

        /// <inheritdoc/>
        public override float Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                base.Height = value;
                if (!IsDesigning || !HasRestriction(Restrictions.DontResize))
                {
                    if (!AutoSize)
                        DrawControl.Height = (int)(value * DpiHelper.Multiplier);
                }
            }
        }
    /// <inheritdoc/>
    [DefaultValue(false)]
    public override bool AutoSize
    {
      get { return base.AutoSize; }
      set { base.AutoSize = value; (DrawControl as Button).AutoSize = value; if (value) DrawControl.Size = DrawControl.PreferredSize; }
    }

    /// <inheritdoc/>
    [DefaultValue(ContentAlignment.MiddleCenter)]
    public override ContentAlignment TextAlign
    {
      get { return base.TextAlign; }
      set { base.TextAlign = value; (DrawControl as Button).TextAlign = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new bool AutoFilter
    {
      get { return base.AutoFilter; }
      set { base.AutoFilter = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new string DataColumn
    {
      get { return base.DataColumn; }
      set { base.DataColumn = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    public new string ReportParameter
    {
      get { return base.ReportParameter; }
      set { base.ReportParameter = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new FilterOperation FilterOperation
    {
      get { return base.FilterOperation; }
      set { base.FilterOperation = value; }
    }
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public override void ScaleControl()
        {
            base.ScaleControl();
            if (!(Control as Button).AutoSize)
                Control.Size = DpiHelper.ConvertUnits(Button.Size);
        }

        ///<inheritdoc/>
        public override void ReinitDpiSize()
        {
            base.ReinitDpiSize();
            DrawControl.Size = DpiHelper.ConvertUnits(Control.Size);
        }

        /// <inheritdoc/>
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

        #region Protected methods

        ///<inheritdoc/>
        protected override SelectionPoint[] GetSelectionPoints()
        {
            if (!AutoSize)
                return base.GetSelectionPoints();
            else
                return new SelectionPoint[]
                {
                    new SelectionPoint(AbsLeft - 2, AbsTop - 2, SizingPoint.None)
                };
        }
        #endregion
    }
}
