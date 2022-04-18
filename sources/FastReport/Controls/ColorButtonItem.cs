using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Controls;
using FastReport.DevComponents.DotNetBar;
using FastReport.DevComponents;

namespace FastReport.Controls
{
  internal class ColorButtonItem : ButtonItem
  {
    private Bitmap bitmap;
    protected ControlContainerItem host;
    private ColorSelector colorSelector;
    private Color defaultColor;

    public Color Color
    {
      get { return colorSelector.Color; }
      set { colorSelector.Color = value; }
    }

    public Color DefaultColor
    {
      get { return defaultColor; }
      set
      {
        defaultColor = value;
        if (bitmap != null)
        {
            Graphics g = Graphics.FromImage(bitmap);
            Color color = value == Color.Transparent ? Color.White : value;
            Brush b = new SolidBrush(color);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillRectangle(b, 1, DpiHelper.ConvertUnits(12.5f), DpiHelper.ConvertUnits(15f), DpiHelper.ConvertUnits(3f));
            b.Dispose();
            g.Dispose();
        }
      }
    }

    public new int ImageIndex
    {
      get { return -1; }
      set
      {
        bitmap = Res.GetImage(value);
        Image = bitmap;
      }
    }
    
    public void SetStyle(UIStyle style)
    {
      colorSelector.SetStyle(style);
    }

    private void FColorSelector_ColorSelected(object sender, EventArgs e)
    {
      Color = colorSelector.Color;
      DefaultColor = Color;
      ClosePopup();
      RaiseClick();
    }

    public ColorButtonItem()
    {
      PopupType = ePopupType.ToolBar;
      colorSelector = new ColorSelector();
      colorSelector.ColorSelected += new EventHandler(FColorSelector_ColorSelected);
      host = new ControlContainerItem();
      host.Control = colorSelector;
      SubItems.Add(host);
            DpiHelper.DpiChanged += DpiHelper_DpiChanged;
    }

        private void DpiHelper_DpiChanged(object sender, EventArgs e)
        {
            colorSelector.ReinitDpiSize();
        }

        public ColorButtonItem(int imageIndex, Color defaultColor)
      : this()
    {
      ImageIndex = imageIndex;
      DefaultColor = defaultColor;
    }

  }
}
