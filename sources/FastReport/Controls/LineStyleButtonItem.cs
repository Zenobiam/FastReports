using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using FastReport.DevComponents.DotNetBar;
using FastReport.DevComponents;

namespace FastReport.Controls
{
  internal class LineStyleButtonItem : ButtonItem
  {
    private LineStyle lineStyle;
    private ItemContainer container;
    public event EventHandler StyleSelected;

    public LineStyle LineStyle
    {
      get { return lineStyle; }
      set
      {
        lineStyle = value;
        foreach (BaseItem item in container.SubItems)
        {
          (item as ButtonItem).Checked = (LineStyle)(item as ButtonItem).Tag == value;
        }
      }
    }

    private void AddSubItem(LineStyle style)
    {
      ButtonItem item = new ButtonItem();
      
      Bitmap bmp = new Bitmap(88, 14);
      using (Graphics g = Graphics.FromImage(bmp))
      {
        using (Pen pen = new Pen(Color.Black, DpiHelper.ConvertUnits(2)))
        {
          DashStyle[] styles = new DashStyle[] { 
            DashStyle.Solid, DashStyle.Dash, DashStyle.Dot, DashStyle.DashDot, DashStyle.DashDotDot, DashStyle.Solid };
          pen.DashStyle = styles[(int)style];
          if (style == LineStyle.Double)
          {
            pen.Width = DpiHelper.ConvertUnits(5);
            pen.CompoundArray = new float[] { 0, 0.4f, 0.6f, 1 };
          }
          g.DrawLine(pen, DpiHelper.ConvertUnits(4), DpiHelper.ConvertUnits(7), DpiHelper.ConvertUnits(84), DpiHelper.ConvertUnits(7));
        }
      }
      
      item.Image = bmp;
      item.Tag = style;
      item.Click += new EventHandler(item_Click);
      container.SubItems.Add(item);
    }

    private void item_Click(object sender, EventArgs e)
    {
      lineStyle = (LineStyle)(sender as ButtonItem).Tag;
      if (StyleSelected != null)
        StyleSelected(this, EventArgs.Empty);
    }
    
    private void UpdateItems()
    {
      container = new ItemContainer();
      container.LayoutOrientation = eOrientation.Vertical;
      SubItems.Add(container);
      AddSubItem(LineStyle.Solid);
      AddSubItem(LineStyle.Dash);
      AddSubItem(LineStyle.Dot);
      AddSubItem(LineStyle.DashDot);
      AddSubItem(LineStyle.DashDotDot);
      AddSubItem(LineStyle.Double);
            container.WidthInternal = 500;
            WidthInternal = 500;
    }

        public override void UpdateDpiDependencies()
        {
            base.UpdateDpiDependencies();
            SubItems.Clear();
            UpdateItems();
        }

        public LineStyleButtonItem()
    {
      AutoExpandOnClick = true;
      PopupType = ePopupType.ToolBar;
      UpdateItems();
    }
  }
}
