using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.DevComponents.DotNetBar;
using System.Globalization;
using FastReport.DevComponents;

namespace FastReport.Controls
{
  internal class FontSizeComboBoxItem : ComboBoxItem
  {
    private float fontSize;
    private bool updating;

    public event EventHandler SizeSelected;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float FontSize
    {
      get
      {
        fontSize = Converter.StringToFloat(Text, true);
        UpdateText();
        return fontSize;
      }
      set
      {
        fontSize = value;
        UpdateText();
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
            ItemHeight = DpiHelper.ConvertUnits(14);
            ComboWidth = DpiHelper.ConvertUnits(40);
        }

        private void UpdateText()
    {
      updating = true;
      Text = Converter.DecreasePrecision(fontSize, 2).ToString();
      updating = false;
    }

    private void OnSizeSelected()
    {
      if (updating)
        return;
      if (SizeSelected != null)
        SizeSelected(this, EventArgs.Empty);
    }

    private void ComboBoxEx_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        OnSizeSelected();
    }

    private void ComboBoxEx_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (Enabled)
        OnSizeSelected();
    }

    private void ComboBoxEx_EnabledChanged(object sender, EventArgs e)
    {
      if (ComboBoxEx.Enabled)
        ComboBoxEx.DropDownStyle = ComboBoxStyle.DropDown;
      else
      {
        ComboBoxEx.DropDownStyle = ComboBoxStyle.DropDownList;
        ComboBoxEx.SelectedIndex = -1;
      }  
    }

    public FontSizeComboBoxItem()
    {
      ComboBoxEx.DropDownStyle = ComboBoxStyle.DropDown;
      ComboBoxEx.KeyDown += new KeyEventHandler(ComboBoxEx_KeyDown);
      ComboBoxEx.SelectedIndexChanged += new EventHandler(ComboBoxEx_SelectedIndexChanged);
      ComboBoxEx.EnabledChanged += new EventHandler(ComboBoxEx_EnabledChanged);
      ComboBoxEx.DrawMode = DrawMode.OwnerDrawVariable;
      ItemHeight = DpiHelper.ConvertUnits(14);
      ComboWidth = DpiHelper.ConvertUnits(40);
      Items.AddRange(new string[] {
        "5", "6", "7", "8", "9", "10", "11", "12", "14", "16", "18", "20", 
        "22", "24", "26", "28", "36", "48", "72"});
    }
  }
}
