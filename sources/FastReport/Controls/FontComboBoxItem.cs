using System;
using System.Collections.Generic;
using System.Text;
using FastReport.DevComponents.DotNetBar;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.DevComponents;

namespace FastReport.Controls
{
  internal class FontComboBoxItem : ComboBoxItem
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
      get { return (string)SelectedItem; }
      set
      {
        fontName = value;
        int i = Items.IndexOf(value);
        if (i != -1)
          SelectedIndex = i;
        else
          SelectedItem = null;
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

        public override void UpdateDpiDependencies()
        {
            base.UpdateDpiDependencies();
            ComboWidth = DpiHelper.ConvertUnits(130);
            DropDownHeight = DpiHelper.ConvertUnits(302);
            DropDownWidth = DpiHelper.ConvertUnits(370);
            ItemHeight = DpiHelper.ConvertUnits(14);
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
      if ((e.State & DrawItemState.Disabled) > 0)
        return;

      Graphics g = e.Graphics;
      e.DrawBackground();

      if ((e.State & DrawItemState.ComboBoxEdit) > 0)
      {
        TextRenderer.DrawText(g, Text, e.Font, e.Bounds.Location, e.ForeColor);
      }
      else if (e.Index >= 0)
      {
        string name = (string)Items[e.Index];
        if (!existingFonts.Contains(name))
          return;

        using (FontFamily family = new FontFamily(name))
        using (Font font = DpiHelper.ConvertUnits(new Font(name, 14, GetFirstAvailableFontStyle(family)), true))
        {
          g.DrawImage(Res.GetImage(59), e.Bounds.X + DpiHelper.ConvertUnits(2), e.Bounds.Y + DpiHelper.ConvertUnits(2));

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
            g.DrawLine(Pens.Gray, e.Bounds.Left, e.Bounds.Bottom - 3, e.Bounds.Right, e.Bounds.Bottom - 3);
            g.DrawLine(Pens.Gray, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
          }
        }
      }
    }

    private void ComboBox_MeasureItem(object sender, MeasureItemEventArgs e)
    {
      e.ItemHeight = DpiHelper.ConvertUnits(20);
    }

    private void ComboBox_SelectionChangeCommitted(object sender, EventArgs e)
    {
      OnFontSelected();

      string fontName = FontName;
      if (mruFonts.Contains(fontName))
        mruFonts.Remove(fontName);
      mruFonts.Insert(0, fontName);
      while (mruFonts.Count > 5)
        mruFonts.RemoveAt(5);
      UpdateFonts();
      Text = fontName;
    }

    private void ComboBoxEx_EnabledChanged(object sender, EventArgs e)
    {
      if (ComboBoxEx.Enabled)
        ComboBoxEx.DropDownStyle = ComboBoxStyle.DropDown;
      else
        ComboBoxEx.DropDownStyle = ComboBoxStyle.DropDownList;
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

    public FontComboBoxItem()
    {
      mruFonts = new List<string>();
      existingFonts = new List<string>();
      styles = new FontStyle[] { FontStyle.Regular, FontStyle.Bold, FontStyle.Italic, 
        FontStyle.Strikeout, FontStyle.Underline };

      foreach (FontFamily family in FontFamily.Families)
      {
        existingFonts.Add(family.Name);
      }

      ComboBoxEx.DrawMode = DrawMode.OwnerDrawVariable;
      ComboBoxEx.DropDownStyle = ComboBoxStyle.DropDown;
      ItemHeight = DpiHelper.ConvertUnits(14);
      ComboBoxEx.DrawItem += new DrawItemEventHandler(ComboBox_DrawItem);
      ComboBoxEx.MeasureItem += new MeasureItemEventHandler(ComboBox_MeasureItem);
      ComboBoxEx.SelectionChangeCommitted += new EventHandler(ComboBox_SelectionChangeCommitted);
      ComboBoxEx.EnabledChanged += new EventHandler(ComboBoxEx_EnabledChanged);
      ComboWidth = DpiHelper.ConvertUnits(130);
      DropDownHeight = DpiHelper.ConvertUnits(302);
      DropDownWidth = DpiHelper.ConvertUnits(370);
      ComboBoxEx.DisableInternalDrawing = true;
      UpdateFonts();
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
