using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Utils;
using FastReport.DevComponents.DotNetBar;
using FastReport.DevComponents;

namespace FastReport.Controls
{
  internal class StyleComboBoxItem : ComboBoxItem
  {
    private bool updating;
    private Report report;

    public event EventHandler StyleSelected;
    
    public new string Style
    {
      get 
      {
        if (ComboBoxEx.Text == Res.Get("Designer,Toolbar,Style,NoStyle"))
          return "";
        return ComboBoxEx.Text;
      }
      set 
      { 
        updating = true;
        if (value == null)
          value = "";
        int i = Items.IndexOf(value);
        if (i != -1)
          SelectedIndex = i;
        else
        {
          if (String.IsNullOrEmpty(value))
            value = Res.Get("Designer,Toolbar,Style,SelectStyle");
          ComboBoxEx.Text = value;
        }  
        updating = false;
      }
    }
    
    public Report Report
    {
      get { return report; }
      set
      {
        report = value;
        if (value != null)
          UpdateItems();
      }
    }

        public override void UpdateDpiDependencies()
        {
            base.UpdateDpiDependencies();
            ItemHeight = DpiHelper.ConvertUnits(14);
            ComboWidth = DpiHelper.ConvertUnits(110);
            DropDownWidth = DpiHelper.ConvertUnits(150);
            DropDownHeight = DpiHelper.ConvertUnits(300);
        }

        private void ComboBox_MeasureItem(object sender, MeasureItemEventArgs e)
    {
      e.ItemHeight = 32;
    }

    private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
    {
      e.DrawBackground();
      Graphics g = e.Graphics;

      if ((e.State & DrawItemState.ComboBoxEdit) > 0)
      {
        TextRenderer.DrawText(g, ComboBoxEx.Text, e.Font, e.Bounds.Location, e.ForeColor, e.BackColor);
      }
      else if (e.Index >= 0)
      {
        string name = (string)Items[e.Index];
        using (TextObject sample = new TextObject())
        {
          sample.Bounds = new RectangleF(e.Bounds.Left + DpiHelper.ConvertUnits(2), e.Bounds.Top + DpiHelper.ConvertUnits(2), e.Bounds.Width - DpiHelper.ConvertUnits(4), e.Bounds.Height - DpiHelper.ConvertUnits(4));
          sample.Text = name;
          sample.HorzAlign = HorzAlign.Center;
          sample.VertAlign = VertAlign.Center;
          if (report != null)
          {
            int index = report.Styles.IndexOf(name);
            if (index != -1)
              sample.ApplyStyle(report.Styles[index]);
          }  
          using (GraphicCache cache = new GraphicCache())
          {
            sample.Draw(new FRPaintEventArgs(g, 1, 1, cache));
          }  
        }
      }  
    }

    private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (updating)
        return;
      if (StyleSelected != null)
        StyleSelected(this, EventArgs.Empty);
    }
    
    private void UpdateItems()
    {
      Items.Clear();
      Items.Add(Res.Get("Designer,Toolbar,Style,NoStyle"));
      foreach (Style s in report.Styles)
      {
        Items.Add(s.Name);
      }
    }

    public StyleComboBoxItem():base()
    {
      ComboBoxEx.DisableInternalDrawing = true;
      ComboBoxEx.DropDownStyle = ComboBoxStyle.DropDown;
      ComboBoxEx.DrawMode = DrawMode.OwnerDrawVariable;
      ItemHeight = DpiHelper.ConvertUnits(14);
      ComboBoxEx.DrawItem += new DrawItemEventHandler(ComboBox_DrawItem);
      ComboBoxEx.MeasureItem += new MeasureItemEventHandler(ComboBox_MeasureItem);
      ComboBoxEx.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
      ComboWidth = DpiHelper.ConvertUnits(110);
      DropDownWidth = DpiHelper.ConvertUnits(150);
      DropDownHeight = DpiHelper.ConvertUnits(300);
    }
  }  
}
