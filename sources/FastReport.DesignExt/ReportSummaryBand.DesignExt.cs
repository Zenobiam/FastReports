using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace FastReport
{
  partial class ReportSummaryBand
  {
    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new bool RepeatOnEveryPage
    {
      get { return base.RepeatOnEveryPage; }
      set { base.RepeatOnEveryPage = value; }
    }

    /// <inheritdoc/>
    public override ContextMenuBase GetContextMenu()
    {
      HeaderFooterBandBaseMenu menu = new HeaderFooterBandBaseMenu(Report.Designer);
      menu.miRepeatOnEveryPage.Visible = false;
      return menu;
    }
  
  }
}