using System;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport.Dialog
{
  partial class DialogComponentBase
  {
    #region Properties
    /// <inheritdoc/>
    [Browsable(false)]
    public override float Left
    {
      get { return base.Left; }
      set { base.Left = value; }
    }

    /// <inheritdoc/>
    [Browsable(false)]
    public override float Top
    {
      get { return base.Top; }
      set { base.Top = value; }
    }

    /// <inheritdoc/>
    [Browsable(false)]
    public override float Width
    {
      get { return base.Width; }
      set { base.Width = value; }
    }

    /// <inheritdoc/>
    [Browsable(false)]
    public override float Height
    {
      get { return base.Height; }
      set { base.Height = value; }
    }
    #endregion
  } 
}