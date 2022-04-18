using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport.Dialog
{
  partial class ButtonBaseControl
  {
    #region Properties
    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new bool AutoFill
    {
      get { return base.AutoFill; }
      set { base.AutoFill = value; }
    }
    #endregion

    #region Private Methods
    private bool ShouldSerializeImage()
    {
      return Image != null;
    }
    #endregion
  }
}
