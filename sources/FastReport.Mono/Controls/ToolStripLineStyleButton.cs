using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using System.Drawing.Drawing2D;

namespace FastReport.Controls
{
  internal class ToolStripLineStyleButton : ToolStripDropDownButton
  {
    private LineStyle FLineStyle;
    private ItemSelector FSelector;
    public event EventHandler StyleSelected;

    public LineStyle LineStyle
    {
      get { return FLineStyle; }
      set
      {
        FLineStyle = value;
        FSelector.SelectedIndex = (int)value;
      }
    }

    private void AddItem(LineStyle style)
    {
      Bitmap bmp = new Bitmap(88, 14);
      using (Graphics g = Graphics.FromImage(bmp))
      {
        using (Pen pen = new Pen(Color.Black, 2))
        {
          DashStyle[] styles = new DashStyle[] { 
            DashStyle.Solid, DashStyle.Dash, DashStyle.Dot, DashStyle.DashDot, DashStyle.DashDotDot, DashStyle.Solid };
          pen.DashStyle = styles[(int)style];
          if (style == LineStyle.Double)
          {
            pen.Width = 5;
            pen.CompoundArray = new float[] { 0, 0.4f, 0.6f, 1 };
          }
          g.DrawLine(pen, 4, 7, 84, 7);
        }
      }
      FSelector.AddItem(bmp, "");
    }

    private void FSelector_ItemSelected(object sender, EventArgs e)
    {
      FLineStyle = (LineStyle)FSelector.SelectedIndex;
      DropDown.Close();
      if (StyleSelected != null)
        StyleSelected(this, EventArgs.Empty);
    }

    private void AddItems()
    {
      AddItem(LineStyle.Solid);
      AddItem(LineStyle.Dash);
      AddItem(LineStyle.Dot);
      AddItem(LineStyle.DashDot);
      AddItem(LineStyle.DashDotDot);
      AddItem(LineStyle.Double);
      FSelector.CalcSize();
    }

    public ToolStripLineStyleButton()
    {
      DisplayStyle = ToolStripItemDisplayStyle.Image;
      ImageAlign = ContentAlignment.MiddleLeft;
      if (!Config.IsRunningOnMono)
      {
        AutoSize = false;
        Size = new Size(32, 22);
      }

      FSelector = new ItemSelector();
      FSelector.ItemSelected += new EventHandler(FSelector_ItemSelected);
      AddItems();
      DropDown = new FRToolStripDropDown(FSelector);
    }
  }
}