using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Dialog
{
  partial class PanelControl
  {
    #region Properties
    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override string Text
    {
      get { return base.Text; }
      set { base.Text = value; }
    }
        
        ///<inheritdoc/>
        public override float Width 
        { 
            get { return base.Width; }
            set 
            {
                if (!IsDesigning || !HasRestriction(Restrictions.DontResize))
                {
                    Control.Width = (int)value;
                    DrawControl.Width = (int)DpiHelper.ConvertUnits(value);
                }
            } 
        }

        ///<inheritdoc/>
        public override float Height
        {
            get { return base.Height; }
            set
            {
                if (!IsDesigning || !HasRestriction(Restrictions.DontResize))
                {
                    Control.Height = (int)value;
                    DrawControl.Height = (int)DpiHelper.ConvertUnits(value);
                }
            }
        }
        #endregion

        #region Public methods
        ///<inheritdoc/>
        public override void ReinitDpiSize()
        {
            base.ReinitDpiSize();
            DrawControl.Size = DpiHelper.ConvertUnits(Control.Size);
        }

        ///<inheritdoc/>
        public override void ScaleControl()
        {
            base.ScaleControl();
            Control.Size = DpiHelper.ConvertUnits(Control.Size);
        }
        #endregion

  }
}
