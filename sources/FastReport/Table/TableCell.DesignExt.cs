using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Utils;
using FastReport.Design;

namespace FastReport.Table
{
  partial class TableCell
  {
    #region Properties
    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override AnchorStyles Anchor
    {
      get { return base.Anchor; }
      set { base.Anchor = value; }
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
    public new bool CanGrow
    {
      get { return base.CanGrow; }
      set { base.CanGrow = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new bool CanShrink
    {
      get { return base.CanShrink; }
      set { base.CanShrink = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new bool GrowToBottom
    {
      get { return base.GrowToBottom; }
      set { base.GrowToBottom = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new bool AutoWidth
    {
      get { return base.AutoWidth; }
      set { base.AutoWidth = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new Duplicates Duplicates
    {
      get { return base.Duplicates; }
      set { base.Duplicates = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new ShiftMode ShiftMode
    {
      get { return base.ShiftMode; }
      set { base.ShiftMode = value; }
    }

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
    public override float Top
    {
      get { return base.Top; }
      set { base.Top = value; }
    }
    #endregion
    
    #region Public Methods
    /// <inheritdoc/>
    public override void HandleMouseHover(FRMouseEventArgs e)
    {
      if (PointInObject(new PointF(e.x, e.y)) && !Table.IsInsideSpan(this))
        e.handled = true;
    }

    /// <inheritdoc/>
    public override void HandleMouseDown(FRMouseEventArgs e)
    {
      // do nothing
    }

    /// <inheritdoc/>
    public override void HandleMouseMove(FRMouseEventArgs e)
    {
      // do nothing
    }

    /// <inheritdoc/>
    public override void HandleMouseUp(FRMouseEventArgs e)
    {
      // do nothing
    }

    /// <inheritdoc/>
    public override void HandleDragOver(FRMouseEventArgs e)
    {
      // do nothing
    }

    /// <inheritdoc/>
    public override void HandleDragDrop(FRMouseEventArgs e)
    {
      // do nothing
    }

    /// <inheritdoc/>
    public override void HandleDoubleClick()
    {
      Table.HandleCellDoubleClick(this);
    }

    /// <inheritdoc/>
    public override ContextMenuBase GetContextMenu()
    {
      return Table.GetCellContextMenu(this);
    }

    /// <inheritdoc/>
    public override SmartTagBase GetSmartTag()
    {
      return Table.GetCellSmartTag(this);
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
