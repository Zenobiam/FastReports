using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport.Controls
{
  /// <summary>
  /// Represents a drop-down control that allows to choose a color.
  /// </summary>
  /// <remarks>
  /// This control may be useful if you write own components for FastReport.
  /// </remarks>
  [ToolboxItem(false)]
  public class ColorDropDown : ToolStripDropDown
  {
    private ToolStripControlHost host;
    private ColorSelector colorSelector;

    /// <summary>
    /// This event is raised when you select a color.
    /// </summary>
    public event EventHandler ColorSelected;

    /// <summary>
    /// Gets or sets the selected color.
    /// </summary>
    public Color Color
    {
      get { return colorSelector.Color; }
      set { colorSelector.Color = value; }
    }

    private void FColorSelector_ColorSelected(object sender, EventArgs e)
    {
      Close();
      if (ColorSelected != null)
        ColorSelected(this, EventArgs.Empty);
    }

    private void ColorDropDown_Opening(object sender, CancelEventArgs e)
    {
      colorSelector.Localize();
    }

        internal void ReinitDpiSize()
        {
            colorSelector.ReinitDpiSize();
        }
    
    /// <summary>
    /// Sets the UI style.
    /// </summary>
    /// <param name="style">The style to set.</param>
    public void SetStyle(UIStyle style)
    {
      colorSelector.SetStyle(style);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorDropDown"/> class with default settings.
    /// </summary>
    public ColorDropDown()
    {
      Font = DrawUtils.DefaultFont;
      colorSelector = new ColorSelector();
      colorSelector.ColorSelected += new EventHandler(FColorSelector_ColorSelected);
      host = new ToolStripControlHost(colorSelector);
      Items.Add(host);
      Opening += new CancelEventHandler(ColorDropDown_Opening);
      //BackColor = Config.DesignerSettings.CustomRenderer.ControlColor;
    }
  }
}
