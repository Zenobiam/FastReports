using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FastReport.Design.PageDesigners.Dialog
{
  internal class Grid : GridBase
  {
    private float snapSize;
    private Bitmap gridBmp;

    public override float SnapSize
    {
      get { return snapSize; }
      set 
      {
        snapSize = value;
        ResetGridBmp();
      }
    }

    private void ResetGridBmp()
    {
      gridBmp = new Bitmap(1, 1);
    }

    public void Draw(Graphics g, Rectangle visibleArea)
    {
      if (visibleArea.Width > 0 && visibleArea.Height > 0 && SnapSize > 2)
      {
        int i = 0;
        if (gridBmp.Width != visibleArea.Width)
        {
          gridBmp = new Bitmap(visibleArea.Width, 1);
          // draw points on one line
          i = 0;
          while (i < visibleArea.Width)
          {
            gridBmp.SetPixel(i, 0, Color.Gray);
            i += (int)SnapSize;
          }
        }

        // draw lines
        i = visibleArea.Top;
        while (i < visibleArea.Bottom)
        {
          g.DrawImage(gridBmp, visibleArea.Left, i);
          i += (int)SnapSize;
        }
      }
    }

    public Grid()
    {
      SnapSize = 8;
    }
  }
}
