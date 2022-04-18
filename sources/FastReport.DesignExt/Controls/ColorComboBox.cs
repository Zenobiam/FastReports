using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Controls
{
  /// <summary>
  /// Represents a combobox that allows to choose a color.
  /// </summary>
  /// <remarks>
  /// This control may be useful if you write own components for FastReport.
  /// </remarks>
  [ToolboxItem(false)]
  public class ColorComboBox : UserControl
  {
    private ComboBox combo;
    private ColorDropDown dropDown;
    private bool showColorName;
        private float controlRatio;

    /// <summary>
    /// This event is raised when you select a color.
    /// </summary>
    public event EventHandler ColorSelected;

    /// <summary>
    /// Gets or sets the selected color.
    /// </summary>
    public Color Color
    {
      get { return dropDown.Color; }
      set 
      { 
        dropDown.Color = value; 
        Refresh();
      }
    }
    
    /// <summary>
    /// Gets or sets value indicating whether it is necessary to show a color name in a combobox.
    /// </summary>
    [DefaultValue(false)]
    [Category("Appearance")]
    public bool ShowColorName
    {
      get { return showColorName; }
      set 
      { 
        showColorName = value; 
        Refresh();
      }
    }

    private void FDropDown_ColorSelected(object sender, EventArgs e)
    {
      Refresh();
      if (ColorSelected != null)
        ColorSelected(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
      //combo.Font = Font;
      //height = combo.Height;
      base.SetBoundsCore(x, y, width, height, specified);
      combo.Width = Width;
    }

    /// <inheritdoc/>
    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics g = e.Graphics;
      using (Bitmap bmp = new Bitmap(Width, Height))
      {
        using (Graphics gBmp = Graphics.FromImage(bmp))
        {
          //gBmp.FillRectangle(SystemBrushes.Control, 0, 0, Width, Height);
        }
        
        combo.Enabled = Enabled;
        combo.DrawToBitmap(bmp, new Rectangle(0, 0, Width, Height));
        
        g.DrawImage(bmp, 0, 0);
        using (Brush b = new SolidBrush(Color))
        {
                    int xy = combo.Height / 2 - (combo.Height - (int)(controlRatio * 6)) / 2;
          Rectangle rect = new Rectangle(xy, xy, (int)(controlRatio * 24), combo.Height - (int)(controlRatio * 6));
          if (Enabled)
            g.FillRectangle(b, rect);
          g.DrawRectangle(SystemPens.ControlDark, rect);
          
          if (ShowColorName)
          {
            string colorName = Converter.ToString(Color);
            TextRenderer.DrawText(g, colorName, Font, new Point(rect.Right + (int)(controlRatio * 8), rect.Top), SystemColors.ControlText);
          }
        }
      }
    }

    /// <inheritdoc/>
    protected override void OnMouseUp(MouseEventArgs e)
    {
      dropDown.Show(this, new Point(0, combo.Height));
    }

#if !MONO
        /// <summary>
        /// Updates the scaling ratio of the control.
        /// </summary>
        public void UpdateControlRatio(float ratio)
        {
            combo.Font = DpiHelper.ParseFontSize(combo.Font, 8, ratio);
            controlRatio = ratio;//DpiHelper.GetMultiplierForScreen(Screen.FromControl(this));
            DpiHelper.RescaleWithNewDpi(dropDown.ReinitDpiSize, ratio);
        }
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorComboBox"/> class with default settings.
    /// </summary>
    public ColorComboBox()
    {
      combo = new ComboBox();
      combo.IntegralHeight = false;
      dropDown = new ColorDropDown();
      dropDown.ColorSelected += new EventHandler(FDropDown_ColorSelected);
      SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            controlRatio = DpiHelper.Multiplier;
    }
  }
}
