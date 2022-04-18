using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FastReport.Utils;

namespace FastReport.Design
{
  internal class StartPageTab : DocumentWindow
  {
    private Designer designer;

    public StartPageTab(Designer designer) : base()
    {
      this.designer = designer;
      ParentControl.BackColor = SystemColors.Window;
      Text = Res.Get("Designer,StartPage");
    }
  }
}
