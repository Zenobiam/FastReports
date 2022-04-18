using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Utils;
using System.Drawing.Drawing2D;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Dialog
{
  partial class PictureBoxControl
  {
    #region Properties
    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new string Text
    {
      get { return base.Text; }
      set { base.Text = value; }
    }
        /// <inheritdoc/>
        public override float Width 
        {
            get { return base.Width; }
            set { base.Width = value; DrawControl.Width = (int)DpiHelper.ConvertUnits(value); } 
        }

        /// <inheritdoc/>
        public override float Height
        {
            get { return base.Height; }
            set { base.Height = value; DrawControl.Height = (int)DpiHelper.ConvertUnits(value); }
        }
    #endregion

    #region Private Methods
    private bool ShouldSerializeImage()
    {
      return Image != null;
    }
    #endregion

    #region Protected Methods
    /// <inheritdoc/>
    protected override SelectionPoint[] GetSelectionPoints()
    {
      if (PictureBox.SizeMode == PictureBoxSizeMode.AutoSize)
        return new SelectionPoint[] { new SelectionPoint(AbsLeft - 2, AbsTop - 2, SizingPoint.None) };
      return base.GetSelectionPoints();
    }
    #endregion

    #region Public Methods
    
        ///<inheritdoc/>
        public override void ScaleControl()
        {
            base.ScaleControl();
            Control.Size = DpiHelper.ConvertUnits(Control.Size);
        }

        ///<inheritdoc/>
        public override void ReinitDpiSize()
        {
            base.ReinitDpiSize();
            DrawControl.Size = DpiHelper.ConvertUnits(Control.Size);
        }
    #endregion
  }
}
