using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FastReport.Design.PageDesigners
{
  internal abstract class GridBase
  {
    public abstract float SnapSize
    {
      get;
      set; 
    }
  }
}
