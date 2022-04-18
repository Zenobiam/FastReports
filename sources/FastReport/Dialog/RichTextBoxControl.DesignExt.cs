using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Dialog
{
    public partial class RichTextBoxControl
    {
        #region Public methods
        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            if (Control.Width > 0 && Control.Height > 0)
            {
                using (Bitmap bmp = DrawUtils.DrawToBitmap(Control, true))
                {
                    using (Bitmap bmpScale = new Bitmap(bmp, DpiHelper.ConvertUnits(bmp.Size)))
                    {
                        e.Graphics.DrawImage(bmpScale, (int)AbsLeft * e.ScaleX, (int)AbsTop * e.ScaleY);
                    }
                }
            }
        }

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
