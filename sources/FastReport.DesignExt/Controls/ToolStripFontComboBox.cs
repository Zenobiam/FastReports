using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Controls
{
  internal class ToolStripFontComboBox : ToolStripComboBox
  {
    private List<string> mruFonts;
    private List<string> existingFonts;
    private FontStyle[] styles;
    private string fontName;

    public event EventHandler FontSelected;
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string FontName
    {
      get { return (string)ComboBox.SelectedItem; }
      set 
      { 
        fontName = value;
        int i = ComboBox.Items.IndexOf(value);
        if (i != -1)
          ComboBox.SelectedIndex = i;
        else
          ComboBox.SelectedItem = null;
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string MruFonts
    {
      get 
      {
        string result = "";
        foreach (string s in mruFonts)
          result += "," + s;
        if (result.StartsWith(","))
          result = result.Substring(1);
        return result;  
      }
      set
      {
        mruFonts.Clear();
        if (!String.IsNullOrEmpty(value))
        {
          string[] fonts = value.Split(new char[] { ',' });
          foreach (string s in fonts)
            mruFonts.Add(s);
        }
        UpdateFonts();  
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new System.Windows.Forms.ComboBox.ObjectCollection Items
    {
      get { return base.Items; }
    }

    private FontStyle GetFirstAvailableFontStyle(FontFamily family)
    {
      foreach (FontStyle style in styles)
      {
        if (family.IsStyleAvailable(style))
          return style;
      }
      return FontStyle.Regular;
    }
    
    private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
    {
      if (!Enabled)
        return;

      Graphics g = e.Graphics;
      e.DrawBackground();

      if ((e.State & DrawItemState.ComboBoxEdit) > 0)
      {
        if (!String.IsNullOrEmpty(fontName))
          TextRenderer.DrawText(g, fontName, e.Font, e.Bounds.Location, e.ForeColor);
      }
      else if (e.Index >= 0)
      {
        string name = (string)Items[e.Index];
        if (!existingFonts.Contains(name))
          return;

        using (FontFamily family = new FontFamily(name))
        using (Font font = new Font(name, DpiHelper.ConvertUnits(14, true), GetFirstAvailableFontStyle(family)))
        {
          g.DrawImage(Res.GetImage(59), e.Bounds.X + DpiHelper.ConvertUnits(2, true), e.Bounds.Y + DpiHelper.ConvertUnits(2, true));

          LOGFONT lf = new LOGFONT();
          font.ToLogFont(lf);
          SizeF sz;

          if (lf.lfCharSet == 2)
          {
            sz = g.MeasureString(name, e.Font);
            int w = (int)sz.Width;
            TextRenderer.DrawText(g, name, e.Font, new Point(e.Bounds.X + DpiHelper.ConvertUnits(20), e.Bounds.Y + (e.Bounds.Height - (int)sz.Height) / 2), e.ForeColor);
            sz = g.MeasureString(name, font);
            TextRenderer.DrawText(g, name, font, new Point(e.Bounds.X + w + DpiHelper.ConvertUnits(28), e.Bounds.Y + (e.Bounds.Height - (int)sz.Height) / 2), e.ForeColor);
          }
          else
          {
            sz = g.MeasureString(name, font);
            TextRenderer.DrawText(g, name, font, new Point(e.Bounds.X + DpiHelper.ConvertUnits(20), e.Bounds.Y + (e.Bounds.Height - (int)sz.Height) / 2), e.ForeColor);
          }


          if (e.Index == mruFonts.Count - 1)
          {
            g.DrawLine(Pens.Gray, e.Bounds.Left, e.Bounds.Bottom - DpiHelper.ConvertUnits(3, true), e.Bounds.Right, e.Bounds.Bottom - DpiHelper.ConvertUnits(3, true));
            g.DrawLine(Pens.Gray, e.Bounds.Left, e.Bounds.Bottom - DpiHelper.ConvertUnits(1, true), e.Bounds.Right, e.Bounds.Bottom - DpiHelper.ConvertUnits(1, true));
          }
        }
      }
    }

    private void ComboBox_MeasureItem(object sender, MeasureItemEventArgs e)
    {
      e.ItemHeight = DpiHelper.ConvertUnits(18);
    }

    private void ComboBox_SelectionChangeCommitted(object sender, EventArgs e)
    {
      OnFontSelected();

      if (mruFonts.Contains(FontName))
        mruFonts.Remove(FontName);
      mruFonts.Insert(0, FontName);
      while (mruFonts.Count > 5)
        mruFonts.RemoveAt(5);
      UpdateFonts();
    }

    private void OnFontSelected()
    {
      if (FontSelected != null)
        FontSelected(this, EventArgs.Empty);
    }

    private void UpdateFonts()
    {
      Items.Clear();
      foreach (string s in mruFonts)
      {
        if (existingFonts.Contains(s))
          Items.Add(s);
      }
      foreach (string s in existingFonts)
      {
        Items.Add(s);
      }
    }

    public ToolStripFontComboBox()
    {
      mruFonts = new List<string>();
      existingFonts = new List<string>();
      styles = new FontStyle[] { FontStyle.Regular, FontStyle.Bold, FontStyle.Italic, 
        FontStyle.Strikeout, FontStyle.Underline };

      foreach (FontFamily family in FontFamily.Families)
      {
        existingFonts.Add(family.Name);
      }

      AutoSize = false;
      ComboBox.DrawMode = DrawMode.OwnerDrawVariable;
      ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      ComboBox.ItemHeight = DpiHelper.ConvertUnits(18);
      ComboBox.DrawItem += new DrawItemEventHandler(ComboBox_DrawItem);
      ComboBox.MeasureItem += new MeasureItemEventHandler(ComboBox_MeasureItem);
      ComboBox.SelectionChangeCommitted += new EventHandler(ComboBox_SelectionChangeCommitted);
      Size = new Size(131, 25);
      DropDownHeight = DpiHelper.ConvertUnits(302, true);
      DropDownWidth = DpiHelper.ConvertUnits(270);
      UpdateFonts();
    }

        public void UpdateDpiDependencies()
        {
            ComboBox.ItemHeight = DpiHelper.ConvertUnits(18);
        }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class LOGFONT
    {
      public int lfHeight;
      public int lfWidth;
      public int lfEscapement;
      public int lfOrientation;
      public int lfWeight;
      public byte lfItalic;
      public byte lfUnderline;
      public byte lfStrikeOut;
      public byte lfCharSet;
      public byte lfOutPrecision;
      public byte lfClipPrecision;
      public byte lfQuality;
      public byte lfPitchAndFamily;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string lfFaceName;
    }
  }
}
