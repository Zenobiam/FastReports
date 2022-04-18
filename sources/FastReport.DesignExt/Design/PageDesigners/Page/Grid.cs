using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FastReport.Utils;

namespace FastReport.Design.PageDesigners.Page
{
  internal class Grid : GridBase
  {
    private PageUnits gridUnits;
    private bool dotted;
    private float snapSizeMillimeters;
    private float snapSizeCentimeters;
    private float snapSizeInches;
    private float snapSizeHundrethsOfInch;
    private float snapSize;
    private Bitmap gridBmp;

    public PageUnits GridUnits
    {
      get { return gridUnits; }
      set
      {
        gridUnits = value;
        UpdateGridSize();
      }
    }

    public bool Dotted
    {
      get { return dotted; }
      set { dotted = value; }
    }
    
    public override float SnapSize
    {
      get { return snapSize; }
      set
      {
        switch (GridUnits)
        {
          case PageUnits.Millimeters:
            SnapSizeMillimeters = value;
            break;
          case PageUnits.Centimeters:
            SnapSizeCentimeters = value;
            break;
          case PageUnits.Inches:
            SnapSizeInches = value;
            break;
          case PageUnits.HundrethsOfInch:
            SnapSizeHundrethsOfInch = value;
            break;    
        }
      }
    }

    public float SnapSizeMillimeters
    {
      get { return snapSizeMillimeters; }
      set 
      {
        snapSizeMillimeters = value;
        UpdateGridSize();
      }
    }

    public float SnapSizeCentimeters
    {
      get { return snapSizeCentimeters; }
      set
      {
        snapSizeCentimeters = value;
        UpdateGridSize();
      }
    }

    public float SnapSizeInches
    {
      get { return snapSizeInches; }
      set
      {
        snapSizeInches = value;
        UpdateGridSize();
      }
    }

    public float SnapSizeHundrethsOfInch
    {
      get { return snapSizeHundrethsOfInch; }
      set
      {
        snapSizeHundrethsOfInch = value;
        UpdateGridSize();
      }
    }

    private void UpdateGridSize()
    {
      switch (gridUnits)
      {
        case PageUnits.Millimeters:
          snapSize = snapSizeMillimeters * Units.Millimeters;
          break;
        case PageUnits.Centimeters:
          snapSize = snapSizeCentimeters * Units.Centimeters;
          break;
        case PageUnits.Inches:
          snapSize = snapSizeInches * Units.Inches;
          break;
        case PageUnits.HundrethsOfInch:
          snapSize = snapSizeHundrethsOfInch * Units.HundrethsOfInch;
          break;
      }
      ResetGridBmp();
    }
    
    private void ResetGridBmp()
    {
      gridBmp = new Bitmap(1, 1);
    }

    private void DrawLinesGrid(Graphics g, RectangleF visibleArea, float scale)
    {
      Pen linePen;
      Pen pen5 = new Pen(Color.FromArgb(255, 0xF8, 0xF8, 0xF8));
      Pen pen10 = new Pen(Color.FromArgb(255, 0xE8, 0xE8, 0xE8));

      float dx = GridUnits == PageUnits.Millimeters || GridUnits == PageUnits.Centimeters ? 
        Units.Millimeters * scale : Units.TenthsOfInch * scale;
      float i = visibleArea.Left;
      int i1 = 0;

      while (i < visibleArea.Right)
      {
        if (i1 % 10 == 0)
          linePen = pen10;
        else if (i1 % 5 == 0)
          linePen = pen5;
        else
          linePen = null;
        if (linePen != null)
          g.DrawLine(linePen, i, visibleArea.Top, i, visibleArea.Bottom);
        i += dx;
        i1++;
      }

      i = visibleArea.Top;
      i1 = 0;
      while (i < visibleArea.Bottom)
      {
        if (i1 % 10 == 0)
          linePen = pen10;
        else if (i1 % 5 == 0)
          linePen = pen5;
        else
          linePen = null;
        if (linePen != null)
          g.DrawLine(linePen, visibleArea.Left, i, visibleArea.Right, i);
        i += dx;
        i1++;
      }

      pen5.Dispose();
      pen10.Dispose();
    }

    private void DrawDotGrid(Graphics g, RectangleF visibleArea, float scale)
    {
      float dx = snapSize * scale;
      float dy = dx;

      if (visibleArea.Width > 0 && visibleArea.Height > 0 && dx > 2 && dy > 2)
      {
        float i = 0;
        if (gridBmp.Width != (int)visibleArea.Width)
        {
          gridBmp = new Bitmap((int)visibleArea.Width, 1);
          // draw points on one line
          i = 0;
          while (i < (int)visibleArea.Width - 1)
          {
            gridBmp.SetPixel((int)Math.Round(i), 0, Color.Silver);
            i += dx;
          }
        }

        // draw lines
        i = visibleArea.Top;
        while (i < visibleArea.Bottom - 1)
        {
          g.DrawImage(gridBmp, (int)Math.Round(visibleArea.Left), (int)Math.Round(i));
          i += dy;
        }
      }
    }
    
    public void Draw(Graphics g, RectangleF visibleArea, float scale)
    {
      if (dotted)
        DrawDotGrid(g, visibleArea, scale);
      else
        DrawLinesGrid(g, visibleArea, scale);
    }
    
    public Grid()
    {
      gridUnits = PageUnits.Centimeters;
      dotted = true;
      snapSizeMillimeters = 2.5f;
      snapSizeCentimeters = 0.25f;
      snapSizeInches = 0.1f;
      snapSizeHundrethsOfInch = 10f;
      UpdateGridSize();
    }
  }
}
