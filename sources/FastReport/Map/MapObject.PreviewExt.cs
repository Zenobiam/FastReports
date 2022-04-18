using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.IO;
using FastReport.Utils;
using FastReport.TypeEditors;
using FastReport.Map.Import.Shp;
using FastReport.Map.Import.Osm;

namespace FastReport.Map
{
  partial class MapObject
  {
    #region Fields
    private bool isPanning;
    private bool panned;
    private Point lastMousePoint;
    private bool needPreviewPageModify;
    private ShapeBase hotPoint;
    #endregion // Fields

    #region Properties
    internal ShapeBase HotPoint
    {
      get { return hotPoint; }
      set
      {
        if (hotPoint != value)
          Page.Refresh();
        hotPoint = value;
      }
    }
    #endregion

    #region Preview mouse support
    /// <inheritdoc/>
    public override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      lastMousePoint = e.Location;
      isPanning = true;
      panned = false;
    }

    /// <inheritdoc/>
    public override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (!IsEmpty)
      {
        if (isPanning)
        {
          int deltaX = e.X - lastMousePoint.X;
          int deltaY = e.Y - lastMousePoint.Y;
          if (Math.Abs(deltaX) > 3 || Math.Abs(deltaY) > 3)
          {
            OffsetX += deltaX / Zoom;
            OffsetY += deltaY / Zoom;
            panned = true;
            lastMousePoint = e.Location;
            needPreviewPageModify = true;
            Page.Refresh();
          }
        }
        else
        {
          if (Hyperlink.Kind == HyperlinkKind.DetailPage || Hyperlink.Kind == HyperlinkKind.DetailReport)
          {
            foreach (MapLayer layer in Layers)
            {
              ShapeBase shape = layer.HitTest(new PointF(e.X + AbsLeft, e.Y + AbsTop));
              if (shape != null && !shape.IsValueEmpty)
              {
                HotPoint = shape;
                Hyperlink.Value = HotPoint.SpatialValue;
                Cursor = Cursors.Hand;
                return;
              }
            }
          
            HotPoint = null;
            Hyperlink.Value = "";
            Cursor = Cursors.Default;
          }
        }
      }
    }

    /// <inheritdoc/>
    public override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      // prevent hyperlink invoke while panning
      if (panned)
        Hyperlink.Value = "";
      isPanning = false;
      panned = false;
    }

    /// <inheritdoc/>
    public override void OnMouseWheel(MouseEventArgs e)
    {
      base.OnMouseWheel(e);
      if (e.Delta < 0)
        ZoomOut();
      else
        ZoomIn();
      needPreviewPageModify = true;
    }

    /// <inheritdoc/>
    public override void OnMouseEnter(EventArgs e)
    {
      base.OnMouseEnter(e);
      needPreviewPageModify = false;
    }

    /// <inheritdoc/>
    public override void OnMouseLeave(EventArgs e)
    {
      base.OnMouseLeave(e);
      HotPoint = null;
      if (needPreviewPageModify)
        Page.Modify();
    }
    #endregion
  }
}
