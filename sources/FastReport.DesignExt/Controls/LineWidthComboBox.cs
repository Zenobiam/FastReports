using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport.Controls
{
  internal class LineWidthComboBox : ComboBox
  {
    private float lineWidth;
    private float[] stdWidths;
    
    public event EventHandler WidthSelected;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new ObjectCollection Items
    {
      get { return base.Items; }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float LineWidth
    {
      get 
      { 
        string s = Text;
        if (s.StartsWith("."))
          s = "0" + s;
        float width = String.IsNullOrEmpty(s) ? 0 : Converter.StringToFloat(s);
        LineWidth = width;
        return width; 
      }
      set 
      { 
        lineWidth = value;
        bool found = false;
        for (int i = 0; i < stdWidths.Length; i++)
        {
          if (Math.Abs(value - stdWidths[i]) < 1e-4)
          {
            Text = (string)Items[i];
            found = true;
            break;
          }
        }
        if (!found)
        {
          SelectedIndex = -1;
          Text = Converter.DecreasePrecision(lineWidth, 3).ToString();
        }
      }
    }

    private void OnWidthSelected()
    {
      if (WidthSelected != null)
        WidthSelected(this, EventArgs.Empty);
    }
    
    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        float w = LineWidth;
        OnWidthSelected();
      }  
    }

    protected override void OnSelectedIndexChanged(EventArgs e)
    {
      base.OnSelectedIndexChanged(e);
      OnWidthSelected();
    }
    
    public LineWidthComboBox()
    {
      stdWidths = new float[] { 0.25f, 0.5f, 1, 1.5f, 2, 3, 4, 6 };
      Items.AddRange(new string[] {
        "0.25", "0.5", "1", "1.5", "2", "3", "4", "6" });
    }
  }
}
