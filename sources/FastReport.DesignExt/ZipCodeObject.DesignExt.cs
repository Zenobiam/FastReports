using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Drawing;
using FastReport.Utils;
using FastReport.TypeEditors;
using System.Drawing.Drawing2D;
using FastReport.TypeConverters;

namespace FastReport
{
  partial class ZipCodeObject : ReportComponentBase
  {
    #region Protected Methods
    /// <inheritdoc/>
    protected override SelectionPoint[] GetSelectionPoints()
    {
      return new SelectionPoint[] { new SelectionPoint(AbsLeft, AbsTop, SizingPoint.LeftTop) };
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override SizeF GetPreferredSize()
    {
      if ((Page as ReportPage).IsImperialUnitsUsed)
        return new SizeF(Units.Inches * 2, Units.Inches * 0.5f);
      return new SizeF(Units.Centimeters * 5, Units.Centimeters * 1);
    }

    /// <inheritdoc/>
    public override void OnAfterInsert(InsertFrom source)
    {
      base.OnAfterInsert(source);
      if (source != InsertFrom.Clipboard)
        Border.Width = 3;
    }

    /// <inheritdoc/>
    public override SmartTagBase GetSmartTag()
    {
      return new ZipCodeSmartTag(this);
    }

    /// <inheritdoc/>
    public override ContextMenuBase GetContextMenu()
    {
      return new ZipCodeObjectMenu(Report.Designer);
    }
    #endregion

  }
}
