using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport.Data
{
  partial class DataComponentBase
  {
    #region Public Methods
    /// <inheritdoc/>
    public override void Delete()
    {
      Enabled = false;
    }
    #endregion
  }
}
