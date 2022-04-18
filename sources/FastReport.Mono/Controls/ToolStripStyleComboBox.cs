using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Utils;

namespace FastReport.Controls
{
  internal class ToolStripStyleComboBox : ToolStripComboBox
  {
    private bool FUpdating;
    private Report FReport;

    public event EventHandler StyleSelected;
    
    public string Style
    {
      get 
      { 
        if (ComboBox.Text == Res.Get("Designer,Style,NoStyle"))
          return "";
        return ComboBox.Text;
      }
      set 
      { 
        FUpdating = true;
        if (value == null)
          value = "";
        int i = ComboBox.Items.IndexOf(value);
        if (i != -1)
          ComboBox.SelectedIndex = i;
        else
        {
          if (String.IsNullOrEmpty(value))
            value = Res.Get("Designer,Toolbar,Style,NoStyle");
          ComboBox.Text = value; 
        }  
        FUpdating = false;
      }
    }
    
    public Report Report
    {
      get { return FReport; }
      set
      {
        FReport = value;
        if (value != null)
          UpdateItems();
      }
    }
    
    private void ComboBox_MeasureItem(object sender, MeasureItemEventArgs e)
    {
      e.ItemHeight = 32;
    }

    private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
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
          if (FReport != null)
          {
            int index = FReport.Styles.IndexOf(name);
            if (index != -1)
              sample.ApplyStyle(FReport.Styles[index]);
          }  
          using (GraphicCache cache = new GraphicCache())
          {
            sample.Draw(new FRPaintEventArgs(g, 1, 1, cache));
          }  
        }
      }
    }

    private void ComboBox_TextChanged(object sender, EventArgs e)
    {
      if (FUpdating)
        return;
      if (StyleSelected != null)
        StyleSelected(this, EventArgs.Empty);
    }
    
    private void UpdateItems()
    {
      Items.Clear();
      Items.Add(Res.Get("Designer,Style,NoStyle"));
      foreach (Style s in FReport.Styles)
      {
        Items.Add(s.Name);
      }
    }

    public ToolStripStyleComboBox()
    {
      AutoSize = false;
      ComboBox.DrawMode = DrawMode.OwnerDrawVariable;
      ComboBox.ItemHeight = 13;
      ComboBox.DrawItem += new DrawItemEventHandler(ComboBox_DrawItem);
      ComboBox.MeasureItem += new MeasureItemEventHandler(ComboBox_MeasureItem);
      ComboBox.TextChanged += new EventHandler(ComboBox_TextChanged);
      Size = new Size(100, 25);
      DropDownWidth = 150;
      DropDownHeight = 300;
    }
  }  
}
