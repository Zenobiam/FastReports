using FastReport.Table;
using FastReport.Utils;
using System;
using System.ComponentModel;

namespace FastReport.CrossView
{
  partial class CrossViewObject : TableBase
  {
    internal enum CrossViewElement
    {
      None,
      Column,
      Row,
      Cell
    }

    #region Properties
    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override int ColumnCount
    {
      get { return base.ColumnCount; }
      set { base.ColumnCount = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override int RowCount
    {
      get { return base.RowCount; }
      set { base.RowCount = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new int FixedRows
    {
      get { return base.FixedRows; }
      set { base.FixedRows = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new int FixedColumns
    {
      get { return base.FixedColumns; }
      set { base.FixedColumns = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new bool CanBreak
    {
      get { return base.CanBreak; }
      set { base.CanBreak = value; }
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
    public new bool RepeatHeaders
    {
      get { return base.RepeatHeaders; }
      set { base.RepeatHeaders = value; }
    }

    /// <inheritdoc/>
    [Browsable(true)]
    [DefaultValue(true)]
    [Category("Behavior")]
    public override bool RepeatRowHeaders
    {
      get { return base.RepeatRowHeaders; }
      set { base.RepeatRowHeaders = value; }
    }

    /// <inheritdoc/>
    [Browsable(true)]
    [DefaultValue(true)]
    [Category("Behavior")]
    public override bool RepeatColumnHeaders
    {
      get { return base.RepeatColumnHeaders; }
      set { base.RepeatColumnHeaders = value; }
    }
    #endregion

    #region Private Methods
    private void RefreshTemplate(bool reset)
    {
      Helper.UpdateDescriptors();

      for (int x = 0; x < Helper.TemplateBodyWidth; x++)
      {
        for (int y = 0; y < Helper.TemplateBodyHeight; y++)
        {
          TableCell cell = this[x + FixedColumns, y + FixedRows];
          if (reset)
            cell.Text = "";
          else
            cell.SetFlags(Flags.CanEdit, false);
        }
      }

      //            CrossViewElement element;
      //            CrossViewDescriptor descriptor;
      //            bool isTotal;

      for (int x = 0; x < ColumnCount; x++)
      {
        for (int y = 0; y < RowCount; y++)
        {
          TableCell cell = this[x, y];
          //                    GetCrossViewElement(cell, out element, out descriptor, out isTotal);
          //                    bool enableSmartTag = descriptor != null && !isTotal;
          bool enableSmartTag = false;
          cell.SetFlags(Flags.HasSmartTag, enableSmartTag);
        }
      }
    }
    #endregion
    #region Public Methods
    /// <inheritdoc/>
    public override void Draw(FRPaintEventArgs e)
    {
      base.Draw(e);
      /*
                  if (FSelectedCell != null)
                      DrawSelectedCellFrame(e, FSelectedCell);
      */
      if (!IsResultCrossView)
        RefreshTemplate(false);
    }

    /// <inheritdoc/>
    public override void OnBeforeInsert(int flags)
    {
      BuildTemplate();
    }

    #endregion
  }
}
