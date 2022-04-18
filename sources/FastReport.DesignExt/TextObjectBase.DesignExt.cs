using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using FastReport.Utils;
using FastReport.Format;
using FastReport.TypeEditors;
using System.Drawing.Design;

namespace FastReport
{
  partial class TextObjectBase
  {
    #region Private Methods
    private bool ShouldSerializeBrackets()
    {
      return Brackets != "[,]";
    }

    private bool ShouldSerializePadding()
    {
      return Padding != new Padding(2, 0, 2, 0);
    }

    private bool ShouldSerializeFormat()
    {
      return (formats.Count == 1 && !(formats[0] is GeneralFormat)) || formats.Count > 1;
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void AssignFormat(ReportComponentBase source)
    {
      base.AssignFormat(source);
      TextObjectBase src = source as TextObjectBase;
      if (src == null)
        return;

      Formats.Assign(src.Formats);
    }
    #endregion
  }
}
