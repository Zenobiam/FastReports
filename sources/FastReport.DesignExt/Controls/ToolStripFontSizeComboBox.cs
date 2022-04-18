using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport.Controls
{
  internal class ToolStripFontSizeComboBox : FRToolStripComboBox
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

    private void UpdateText()
    {
      updating = true;
      // mono fix
      ComboBox.SelectedIndex = 1;
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
    
    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        OnSizeSelected();
    }

    protected override void OnSelectedIndexChanged(EventArgs e)
    {
      OnSizeSelected();
    }
    
    public ToolStripFontSizeComboBox()
    {
      Size = new Size(50, 25);
      Items.AddRange(new string[] {
        "5", "6", "7", "8", "9", "10", "11", "12", "14", "16", "18", "20", 
        "22", "24", "26", "28", "36", "48", "72"});
    }

  }
}
