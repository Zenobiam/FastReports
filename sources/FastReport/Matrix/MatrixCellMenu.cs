using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;

namespace FastReport.Matrix
{
  internal class MatrixCellMenu : MatrixCellMenuBase
  {
    private ContextMenuItem miFunction;
    private ContextMenuItem miFunctionNone;
    private ContextMenuItem miFunctionSum;
    private ContextMenuItem miFunctionMin;
    private ContextMenuItem miFunctionMax;
    private ContextMenuItem miFunctionAvg;
    private ContextMenuItem miFunctionCount;
        private ContextMenuItem miFunctionCountDistinct;
        private ContextMenuItem miFunctionCustom;
    private ContextMenuItem miPercent;
    private ContextMenuItem miPercentNone;
    private ContextMenuItem miPercentColumnTotal;
    private ContextMenuItem miPercentRowTotal;
    private ContextMenuItem miPercentGrandTotal;

    private void Function_Click(object sender, EventArgs e)
    {
      MatrixAggregateFunction function = (MatrixAggregateFunction)(sender as ContextMenuItem).Tag;
      (Descriptor as MatrixCellDescriptor).Function = function;
      Change();
    }

    private void Percent_Click(object sender, EventArgs e)
    {
      MatrixPercent percent = (MatrixPercent)(sender as ContextMenuItem).Tag;
      (Descriptor as MatrixCellDescriptor).Percent = percent;
      Change();
    }

    public MatrixCellMenu(MatrixObject matrix, MatrixElement element, MatrixDescriptor descriptor)
      : base(matrix, element, descriptor)
    {
      MyRes res = new MyRes("Forms,TotalEditor");

      miFunction = CreateMenuItem(Res.GetImage(132), Res.Get("ComponentMenu,MatrixCell,Function"), null);
      miFunctionNone = CreateMenuItem(null, Res.Get("Misc,None"), new EventHandler(Function_Click));
      miFunctionNone.CheckOnClick = true;
      miFunctionNone.Tag = MatrixAggregateFunction.None;
      miFunctionSum = CreateMenuItem(null, res.Get("Sum"), new EventHandler(Function_Click));
      miFunctionSum.CheckOnClick = true;
      miFunctionSum.Tag = MatrixAggregateFunction.Sum;
      miFunctionMin = CreateMenuItem(null, res.Get("Min"), new EventHandler(Function_Click));
      miFunctionMin.CheckOnClick = true;
      miFunctionMin.Tag = MatrixAggregateFunction.Min;
      miFunctionMax = CreateMenuItem(null, res.Get("Max"), new EventHandler(Function_Click));
      miFunctionMax.CheckOnClick = true;
      miFunctionMax.Tag = MatrixAggregateFunction.Max;
      miFunctionAvg = CreateMenuItem(null, res.Get("Avg"), new EventHandler(Function_Click));
      miFunctionAvg.CheckOnClick = true;
      miFunctionAvg.Tag = MatrixAggregateFunction.Avg;
      miFunctionCount = CreateMenuItem(null, res.Get("Count"), new EventHandler(Function_Click));
      miFunctionCount.CheckOnClick = true;
      miFunctionCount.Tag = MatrixAggregateFunction.Count;
            miFunctionCountDistinct = CreateMenuItem(null, res.Get("CountDistinct"), new EventHandler(Function_Click));
            miFunctionCountDistinct.CheckOnClick = true;
            miFunctionCountDistinct.Tag = MatrixAggregateFunction.CountDistinct;
      miFunctionCustom = CreateMenuItem(null, res.Get("Custom"), new EventHandler(Function_Click));
      miFunctionCustom.CheckOnClick = true;
      miFunctionCustom.Tag = MatrixAggregateFunction.Custom;

      miFunction.DropDownItems.AddRange(new ContextMenuItem[] {
        miFunctionNone, miFunctionSum, miFunctionMin, miFunctionMax, miFunctionAvg, miFunctionCount, miFunctionCountDistinct, miFunctionCustom });

      res = new MyRes("ComponentMenu,MatrixCell");
      miPercent = CreateMenuItem(null, res.Get("Percent"), null);
      miPercentNone = CreateMenuItem(null, Res.Get("Misc,None"), new EventHandler(Percent_Click));
      miPercentNone.CheckOnClick = true;
      miPercentNone.Tag = MatrixPercent.None;
      miPercentColumnTotal = CreateMenuItem(null, res.Get("PercentColumnTotal"), new EventHandler(Percent_Click));
      miPercentColumnTotal.CheckOnClick = true;
      miPercentColumnTotal.Tag = MatrixPercent.ColumnTotal;
      miPercentRowTotal = CreateMenuItem(null, res.Get("PercentRowTotal"), new EventHandler(Percent_Click));
      miPercentRowTotal.CheckOnClick = true;
      miPercentRowTotal.Tag = MatrixPercent.RowTotal;
      miPercentGrandTotal = CreateMenuItem(null, res.Get("PercentGrandTotal"), new EventHandler(Percent_Click));
      miPercentGrandTotal.CheckOnClick = true;
      miPercentGrandTotal.Tag = MatrixPercent.GrandTotal;

      miPercent.DropDownItems.AddRange(new ContextMenuItem[] {
        miPercentNone, miPercentColumnTotal, miPercentRowTotal, miPercentGrandTotal });
      
      int insertIndex = Items.IndexOf(miDelete);
      Items.Insert(insertIndex, miFunction);
      Items.Insert(insertIndex + 1, miPercent);
      
      MatrixAggregateFunction function = (Descriptor as MatrixCellDescriptor).Function;
      foreach (ContextMenuItem item in miFunction.DropDownItems)
      {
        if ((MatrixAggregateFunction)item.Tag == function)
        {
          (item as ContextMenuItem).Checked = true;
          break;
        }
      }

      MatrixPercent percent = (Descriptor as MatrixCellDescriptor).Percent;
      foreach (ContextMenuItem item in miPercent.DropDownItems)
      {
        if ((MatrixPercent)item.Tag == percent)
        {
          (item as ContextMenuItem).Checked = true;
          break;
        }
      }
    }
  }
}
