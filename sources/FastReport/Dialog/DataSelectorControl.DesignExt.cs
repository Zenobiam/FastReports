using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;
using System.Data;
using FastReport.Utils;
using FastReport.TypeConverters;
using FastReport.TypeEditors;
using FastReport.Data;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Dialog
{
  partial class DataSelectorControl
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

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new bool AutoFilter
    {
      get { return base.AutoFilter; }
      set { base.AutoFilter = value; }
    }
        #endregion

        #region Public methods

        ///<inheritdoc/>
        public override void ScaleControl()
        {
            base.ScaleControl();
            Dictionary<Control, Point> locations = new Dictionary<Control, Point>();
            Dictionary<Control, Size> sizes = new Dictionary<Control, Size>();

            foreach (Control c in pnPanel.Controls)
            {
                locations.Add(c, c.Location);
                sizes.Add(c, c.Size);
                if (c is Button)
                    c.Font = new Font(DrawFont.Name, DrawFont.Size * DrawUtils.ScreenDpi / (DpiHelper.Multiplier * 96f), DrawFont.Style);
            }
            pnPanel.ClientSize = DpiHelper.ConvertUnits(pnPanel.ClientSize);

            foreach (var entry in locations)
            {
                entry.Key.Location = DpiHelper.ConvertUnits(entry.Value);
            }
            foreach (var entry in sizes)
            {
                entry.Key.Size = DpiHelper.ConvertUnits(entry.Value);
            }
        }

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            if (Control.Width > 0 && Control.Height > 0)
            {
                using (Bitmap bmp = DrawUtils.DrawToBitmap(Control, true))
                {
                    using (Bitmap bmpScaled = new Bitmap(bmp, DpiHelper.ConvertUnits(bmp.Size)))
                        e.Graphics.DrawImage(bmpScaled, (int)AbsLeft * e.ScaleX, (int)AbsTop * e.ScaleY);
                }

                foreach (Control c in pnPanel.Controls)
                {
                    using (Bitmap bmp = DrawUtils.DrawToBitmap(c, true))
                    {
                        using (Bitmap bmpScaled = new Bitmap(bmp, DpiHelper.ConvertUnits(bmp.Size)))
                            e.Graphics.DrawImage(bmpScaled, (int)AbsLeft * e.ScaleX + c.Left * e.ScaleX, (int)AbsTop * e.ScaleY + c.Top * e.ScaleY);
                    }
                }
            }


            if (IsDesigning)
            {
                if (IsAncestor)
                    e.Graphics.DrawImage(Res.GetImage(99), (int)(AbsRight * e.ScaleX - 9), (int)(AbsTop * e.ScaleY + 2));
            }
        }
        #endregion

    }
}
