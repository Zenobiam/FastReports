using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;
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
  partial class GridControl
  {
    #region Properties
    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override Color BackColor
    {
      get { return base.BackColor; }
      set { base.BackColor = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override Color ForeColor
    {
      get { return base.ForeColor; }
      set { base.ForeColor = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override string Text
    {
      get { return base.Text; }
      set { base.Text = value; }
    }
    #endregion

    #region Private Methods
    private bool ShouldSerializeBackgroundColor()
    {
      return BackgroundColor != SystemColors.AppWorkspace;
    }

    private bool ShouldSerializeGridColor()
    {
      return GridColor != SystemColors.ControlDark;
    }
    #endregion

    #region Public Methods
        
        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        ///<inheritdoc/>
        public override void ScaleControl()
        {
            Control.Scale(new SizeF(DpiHelper.Multiplier, DpiHelper.Multiplier));
            return;
            base.ScaleControl();
            List<int> sizes = new List<int>();
            foreach (DataGridViewColumn column in (Control as DataGridView).Columns)
            {
                sizes.Add(column.Width);
            }
            foreach (DataGridViewRow row in (Control as DataGridView).Rows)
            {
                sizes.Add(row.Height);
            }
            Control.Size = DpiHelper.ConvertUnits(Control.Size);
            int counter = 0;
            foreach (DataGridViewColumn column in (Control as DataGridView).Columns)
            {
                column.Width = DpiHelper.ConvertUnits(sizes[counter]);
                counter++;
            }
            foreach (DataGridViewRow row in (Control as DataGridView).Rows)
            {
                row.Height = DpiHelper.ConvertUnits(sizes[counter]);
                counter++;
            }
        }

        ///<inheritdoc/>
        public override void ReinitDpiSize()
        {
            base.ReinitDpiSize();
            DrawControl.Size = DpiHelper.ConvertUnits(Control.Size);
        }

        /// <inheritdoc/>
        public override ContextMenuBase GetContextMenu()
    {
      return new GridControlMenu(Report.Designer);
    }
    #endregion
  }
}
