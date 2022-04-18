using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using FastReport.Design;
using FastReport.TypeConverters;
using FastReport.Data;
using FastReport.TypeEditors;
using FastReport.Utils;
using FastReport.Design.PageDesigners.Page;
using System.Drawing;

namespace FastReport.Table
{
  partial class TableRow
  {
    #region Properties
    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override float Left
    {
      get { return base.Left; }
      set { base.Left = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override float Width
    {
      get { return base.Width; }
      set { base.Width = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override DockStyle Dock
    {
      get { return base.Dock; }
      set { base.Dock = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override AnchorStyles Anchor
    {
      get { return base.Anchor; }
      set { base.Anchor = value; }
    }
    #endregion

    #region Protected Methods
    /// <inheritdoc/>
    protected override SelectionPoint[] GetSelectionPoints()
    {
      return new SelectionPoint[] { };
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override ContextMenuBase GetContextMenu()
    {
      return (Parent as TableBase).GetRowContextMenu(this);
    }

    /// <inheritdoc/>
    public override void SelectionChanged()
    {
      base.SelectionChanged();
      if (Parent != null)
        Parent.SelectionChanged();
    }
    #endregion
  }
}
