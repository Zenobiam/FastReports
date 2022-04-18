using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace FastReport.Controls
{
  /// <summary>
  /// Represents a popup window that alows to choose a color.
  /// </summary>
  /// <remarks>
  /// This control may be useful if you write own components for FastReport.
  /// </remarks>
  public class ColorPopup : PopupWindow
  {
    private ColorSelector selector;

    /// <summary>
    /// This event is raised when you select a color.
    /// </summary>
    public event EventHandler ColorSelected;

    /// <summary>
    /// Gets or sets the selected color.
    /// </summary>
    public Color Color
    {
      get { return selector.Color; }
      set { selector.Color = value; }
    }

    private void FSelector_ColorSelected(object sender, EventArgs e)
    {
      Close();
      if (ColorSelected != null)
        ColorSelected(this, EventArgs.Empty);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorPopup"/> class with default settings.
    /// </summary>
    /// <param name="ownerForm">The main form that owns this popup control.</param>
    public ColorPopup(Form ownerForm) : base(ownerForm)
    {
      selector = new ColorSelector();
      Controls.Add(selector);
      Font = ownerForm.Font;
      ClientSize = selector.Size;
      BackColor = SystemColors.Window;
      selector.ColorSelected += new EventHandler(FSelector_ColorSelected);
    }

  }
}
