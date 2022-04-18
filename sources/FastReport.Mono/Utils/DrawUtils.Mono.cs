using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace FastReport.Utils
{
  partial class DrawUtils
  {
    public static Bitmap DrawToBitmap(Control control, bool children)
    {
      Bitmap bitmap = new Bitmap(control.Width, control.Height);
      using (Graphics gr = Graphics.FromImage(bitmap))
      {
        gr.Clear(control.BackColor);
      }
      control.DrawToBitmap(bitmap, new Rectangle(0, 0, control.Width, control.Height));
      return bitmap;
    }    
  }
}
