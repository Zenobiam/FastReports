using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;
using FastReport.TypeConverters;
using FastReport.Utils;
using FastReport.Design.PageDesigners.Page;

namespace FastReport
{
  partial class LineObject
  {
    #region Properties
    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override FillBase Fill
    {
      get { return base.Fill; }
      set { base.Fill = value; }
    }
    #endregion

    #region Private Methods
    private bool ShouldSerializeStartCap()
    {
      return !StartCap.Equals(new CapSettings());
    }

    private bool ShouldSerializeEndCap()
    {
      return !EndCap.Equals(new CapSettings());
    }
    #endregion

    #region Protected Methods
    /// <inheritdoc/>
    protected override SelectionPoint[] GetSelectionPoints()
    {
      return new SelectionPoint[] {
        new SelectionPoint(AbsLeft, AbsTop, SizingPoint.LeftTop),
        new SelectionPoint(AbsLeft + Width, AbsTop + Height, SizingPoint.RightBottom) };
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override bool PointInObject(PointF point)
    {
      using (Pen pen = new Pen(Color.Black, 10 / ReportWorkspace.Scale))
      using (GraphicsPath path = new GraphicsPath())
      {
        path.AddLine(AbsLeft, AbsTop, AbsRight, AbsBottom);
        return path.IsOutlineVisible(point, pen);
      }
    }

    /// <inheritdoc/>
    public override void CheckNegativeSize(FRMouseEventArgs e)
    {
      // do nothing
    }

    /// <inheritdoc/>
    public override SizeF GetPreferredSize()
    {
      return new SizeF(0, 0);
    }

    /// <inheritdoc/>
    public override void HandleMouseMove(FRMouseEventArgs e)
    {
      base.HandleMouseMove(e);
      if (e.handled)
        e.cursor = Cursors.Cross;
    }

    /// <inheritdoc/>
    public override void HandleMouseUp(FRMouseEventArgs e)
    {
      base.HandleMouseUp(e);
      if (e.mode == WorkspaceMode2.SelectionRect)
      {
        GraphicsPath path = new GraphicsPath();
        Pen pen = null;
        if (Width != 0 && Height != 0)
        {
          path.AddLine(AbsLeft, AbsTop, AbsRight, AbsBottom);
          pen = new Pen(Color.Black, 10 / ReportWorkspace.Scale);
          path.Widen(pen);
        }
        else
        {
          float d = 5 / ReportWorkspace.Scale;
          if (Width == 0)
          {
            path.AddLine(AbsLeft - d, AbsTop, AbsRight - d, AbsBottom);
            path.AddLine(AbsRight - d, AbsBottom, AbsRight + d, AbsBottom);
            path.AddLine(AbsRight + d, AbsBottom, AbsRight + d, AbsTop);
          }
          else
          {
            path.AddLine(AbsLeft, AbsTop - d, AbsRight, AbsBottom - d);
            path.AddLine(AbsRight, AbsBottom - d, AbsRight, AbsBottom + d);
            path.AddLine(AbsRight, AbsBottom + d, AbsLeft, AbsTop + d);
          }  
          path.CloseFigure();
        }
        
        Region region = new Region(path);
        e.handled = region.IsVisible(e.selectionRect);

        path.Dispose();
        region.Dispose();
        if (pen != null)
          pen.Dispose();
      }
      else if (e.mode == WorkspaceMode2.Size)
      {
        if (!Diagonal)
        {
          if (Math.Abs(Width) > Math.Abs(Height))
            Height = 0;
          else
            Width = 0;
        }
      }
    }

    /// <inheritdoc/>
    public override void OnBeforeInsert(int flags)
    {
      diagonal = flags != 0;
      if (flags == 3 || flags == 4)
        StartCap.Style = CapStyle.Arrow;
      if (flags == 2 || flags == 4)
        EndCap.Style = CapStyle.Arrow;
    }
    #endregion
  }
}
