using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;

namespace FastReport.Controls
{
  internal class ItemSelector : UserControl
  {
    private List<ItemSelectorItem> FItems;
    private int FSelectedIndex;
    private Size FItemSize;

    public event EventHandler ItemSelected;
    
    public int SelectedIndex
    {
      get { return FSelectedIndex; }
      set { FSelectedIndex = value; }
    }

    private void DrawHighlight(Graphics g, Rectangle rect)
    {
      using (Brush brush = new SolidBrush(Color.FromArgb(193, 210, 238)))
      using (Pen pen = new Pen(Color.FromArgb(49, 106, 197)))
      {
        g.FillRectangle(brush, rect);
        g.DrawRectangle(pen, rect);
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics g = e.Graphics;

      for (int i = 0; i < FItems.Count; i++)
      {
        ItemSelectorItem item = FItems[i];
        Rectangle rect = new Rectangle(3, i * FItemSize.Height + 3, FItemSize.Width, FItemSize.Height);
        if (i == FSelectedIndex)
          DrawHighlight(g, rect);
        g.DrawImage(item.Image, rect.Left + 3, rect.Top + 3);
        if (!String.IsNullOrEmpty(item.Text))
          TextRenderer.DrawText(g, item.Text, Font, new Point(rect.Left + item.Image.Width + 6, rect.Top + 3), SystemColors.WindowText);
      }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      FSelectedIndex = e.Y / FItemSize.Height;
      Refresh();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      if (ItemSelected != null)
        ItemSelected(this, EventArgs.Empty);
    }

    public void AddItem(Image image, string text)
    {
      FItems.Add(new ItemSelectorItem(image, text));
    }

    public void CalcSize()
    {
      int maxWidth = 0;
      foreach (ItemSelectorItem item in FItems)
      {
        int width = item.Image.Width + 6;
        if (!String.IsNullOrEmpty(item.Text))
          width += TextRenderer.MeasureText(item.Text, Font).Width + 3;
        if (width > maxWidth)
          maxWidth = width;
      }
      
      FItemSize = new Size(maxWidth, FItems[0].Image.Height + 6);
      Size = new Size(FItemSize.Width + 7, FItemSize.Height * FItems.Count + 7);
    }

    public ItemSelector()
    {
      FItems = new List<ItemSelectorItem>();
      SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
      BackColor = SystemColors.Window;
      FSelectedIndex = -1;
    }


    private class ItemSelectorItem
    {
      public Image Image;
      public string Text;

      public ItemSelectorItem(Image image, string text)
      {
        Image = image;
        Text = text;
      }
    }
  }
}
