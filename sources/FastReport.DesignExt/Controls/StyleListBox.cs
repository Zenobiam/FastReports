using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Utils;

namespace FastReport.Controls
{
  internal class StyleListBox : ListBox
  {
    private bool updating;
    private StyleCollection styles;

    public event EventHandler StyleSelected;
    
    public string Style
    {
      get 
      { 
        if (SelectedIndex < 1)
          return "";
        return (string)Items[SelectedIndex];
      }
      set 
      { 
        updating = true;
        int i = Items.IndexOf(value);
        SelectedIndex = i != -1 ? i : 0;
        updating = false;
      }
    }
    
    public StyleCollection Styles
    {
      get { return styles; }
      set
      {
        styles = value;
        if (value != null)
          UpdateItems();
      }
    }
    
    protected override void OnDrawItem(DrawItemEventArgs e)
    {
      e.DrawBackground();
      Graphics g = e.Graphics;
      if (e.Index >= 0)
      {
        string name = (string)Items[e.Index];
        using (TextObject sample = new TextObject())
        {
          sample.Bounds = new RectangleF(e.Bounds.Left + 2, e.Bounds.Top + 2, e.Bounds.Width - 4, e.Bounds.Height - 4);
          sample.Text = name;
          sample.HorzAlign = HorzAlign.Center;
          sample.VertAlign = VertAlign.Center;
          if (styles != null)
          {
            int index = styles.IndexOf(name);
            if (index != -1)
              sample.ApplyStyle(styles[index]);
          }  
          using (GraphicCache cache = new GraphicCache())
          {
            sample.Draw(new FRPaintEventArgs(g, 1, 1, cache));
          }
        }
      }
    }

    protected override void OnSelectedIndexChanged(EventArgs e)
    {
      base.OnSelectedIndexChanged(e);
      if (updating)
        return;
      if (StyleSelected != null)
        StyleSelected(this, EventArgs.Empty);
    }
    
    private void UpdateItems()
    {
      Items.Clear();
      Items.Add(Res.Get("Designer,Toolbar,Style,NoStyle"));
      foreach (Style s in styles)
      {
        Items.Add(s.Name);
      }
    }

    public StyleListBox()
    {
      DrawMode = DrawMode.OwnerDrawFixed;
      ItemHeight = 32;
      IntegralHeight = false;
      Size = new Size(150, 300);
    }
  }  
}
