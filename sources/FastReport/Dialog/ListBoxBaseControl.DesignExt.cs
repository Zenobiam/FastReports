using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;
using System.Data;
using FastReport.Utils;
using FastReport.TypeEditors;
using FastReport.Data;
using FastReport.TypeConverters;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Dialog
{
  partial class ListBoxBaseControl
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

        #region Protected Methods
        /// <inheritdoc/>
        protected override bool ShouldSerializeBackColor()
    {
      return BackColor != SystemColors.Window;
    }

    /// <inheritdoc/>
    protected override bool ShouldSerializeForeColor()
    {
      return ForeColor != SystemColors.WindowText;
    }
        #endregion

        #region Public Methods

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            if (DrawControl != null)
            {
                if (DrawControl.Font.Size != DrawFont.Size)
                    DrawControl.Font = DrawFont;
                if (Items != (DrawControl as ListBox).Items)
                {
                    (DrawControl as ListBox).Items.Clear();
                    object[] objcts = new object[Items.Count];
                    Items.CopyTo(objcts, 0);
                    (DrawControl as ListBox).Items.AddRange(objcts);
                }
                if ((DrawControl as ListBox).SelectedIndex != ListBox.SelectedIndex)
                    (DrawControl as ListBox).SelectedIndex = ListBox.SelectedIndex;

                if (DrawControl.Width > 0 && DrawControl.Height > 0)
                    using (Bitmap bmp = DrawUtils.DrawToBitmap(DrawControl, true))
                    {
                        e.Graphics.DrawImage(bmp, (int)AbsLeft * e.ScaleX, (int)AbsTop * e.ScaleY);
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
