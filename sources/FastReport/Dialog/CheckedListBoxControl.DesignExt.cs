using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Data;

namespace FastReport.Dialog
{
  partial class CheckedListBoxControl
  {
    #region Properties
    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override DrawMode DrawMode
    {
      get { return base.DrawMode; }
      set { base.DrawMode = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override int ItemHeight
    {
      get { return base.ItemHeight; }
      set { base.ItemHeight = value; }
    }
    #endregion
  }
}
