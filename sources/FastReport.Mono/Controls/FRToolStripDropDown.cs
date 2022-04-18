using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace FastReport.Controls
{
  internal class FRToolStripDropDown : ToolStripDropDown
  {
    private ToolStripControlHost FControlHost;
    
    protected override void OnLayout(LayoutEventArgs e)
    {
      // mono bugs workaround
      Size suggestedSize = GetPreferredSize(Size.Empty);
      if (AutoSize && suggestedSize != Size)
      {
        Size = suggestedSize;
      }
      SetDisplayedItems();
      OnLayoutCompleted(EventArgs.Empty);
      Invalidate();
    }

    public FRToolStripDropDown(Control control)
    {
      FControlHost = new ToolStripControlHost(control);
      FControlHost.AutoSize = false;
      FControlHost.Size = control.Size;
      Items.Add(FControlHost);
      AutoSize = false;
      Size = new Size(control.Size.Width + 2, control.Size.Height + 2);
      control.Location = new Point(1, 1);
    }
  }
}