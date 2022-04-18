using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport
{
  partial class ChildBand
  {
    /// <inheritdoc/>
    public override ContextMenuBase GetContextMenu()
    {
      return new ChildBandMenu(Report.Designer);
    }

    /// <inheritdoc/>
    public override void Delete()
    {
      if (!CanDelete)
        return;
      // remove only this band, keep its subbands
      if (Parent is BandBase)
        (Parent as BandBase).Child = Child;
      Dispose();
    }
  }
}