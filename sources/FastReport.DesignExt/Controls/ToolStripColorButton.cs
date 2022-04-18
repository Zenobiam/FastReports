using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Controls;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Controls
{
  internal class ToolStripColorButton : ToolStripSplitButton
  {
    private Bitmap bitmap;
    private ColorDropDown dropDown;
    private Color defaultColor;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color Color
    {
      get { return dropDown.Color; }
      set { dropDown.Color = value; }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color DefaultColor
    {
      get { return defaultColor; }
      set
      {
        defaultColor = value;
        Graphics g = Graphics.FromImage(bitmap);
        Color color = value == Color.Transparent ? Color.White : value;
        Brush b = new SolidBrush(color);
        g.FillRectangle(b, DpiHelper.ConvertUnits(1), DpiHelper.ConvertUnits(13), DpiHelper.ConvertUnits(15), DpiHelper.ConvertUnits(3));
        b.Dispose();
        g.Dispose();
        Invalidate();
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new int ImageIndex
    {
      get { return -1; }
      set
      {
        bitmap = Res.GetImage(value);
        Image = bitmap;
      }
    }
    
    private void FDropDown_ColorSelected(object sender, EventArgs e)
    {
      Color = dropDown.Color;
      DefaultColor = Color;
      OnButtonClick(EventArgs.Empty);
    }
    
    public void SetStyle(UIStyle style)
    {
      dropDown.SetStyle(style);
    }

    public ToolStripColorButton()
    {
      dropDown = new ColorDropDown();
      dropDown.ColorSelected += new EventHandler(FDropDown_ColorSelected);
      DropDown = dropDown;
    }
    
    public ToolStripColorButton(int imageIndex, Color defaultColor) : this()
    {
      ImageIndex = imageIndex;
      DefaultColor = defaultColor;
      DisplayStyle = ToolStripItemDisplayStyle.Image;
    }

  }
}
