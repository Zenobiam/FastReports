using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using FastReport.Design;
using FastReport.TypeConverters;
using FastReport.Design.PageDesigners.Page;
using FastReport.Data;
using FastReport.TypeEditors;
using FastReport.Utils;
using System.Drawing;

namespace FastReport.Table
{
  partial class TableColumn
  {
    #region Properties
    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override float Top
    {
      get { return base.Top; }
      set { base.Top = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override float Height
    {
      get { return base.Height; }
      set { base.Height = value; }
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
      return new SelectionPoint[] {};
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override ContextMenuBase GetContextMenu()
    {
      return (Parent as TableBase).GetColumnContextMenu(this);
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
