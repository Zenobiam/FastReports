using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Dialog
{
    partial class GroupBoxControl
    {
        #region Properties
        /// <inheritdoc/>
        public override float Width
        {
            get { return base.Width; }
            set
            {
                if (!IsDesigning || !HasRestriction(Restrictions.DontResize))
                {
                    Control.Width = (int)value;
                    DrawControl.Width = DpiHelper.ConvertUnits(Control.Width);
                }
            }
        }

        /// <inheritdoc/>
        public override float Height
        {
            get { return base.Height; }
            set
            {
                if (!IsDesigning || !HasRestriction(Restrictions.DontResize))
                {
                    Control.Height = (int)value;
                    DrawControl.Height = DpiHelper.ConvertUnits(Control.Height);
                }
            }
        }
        #endregion


        #region Public Methods
        ///<inheritdoc/>
        public override void ReinitDpiSize()
        {
            base.ReinitDpiSize();
            DrawControl.Size = DpiHelper.ConvertUnits(Control.Size);
        }

        /// <inheritdoc/>
        public override void ScaleControl()
        {
            base.ScaleControl();
            Control.Size = DpiHelper.ConvertUnits(Control.Size);
        }

        #endregion
    }
}
