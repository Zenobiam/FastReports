using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Utils;
using FastReport.Data;

namespace FastReport.Dialog
{
  /// <summary>
  /// Uses a mask to distinguish between proper and improper user input.
  /// Wraps the <see cref="System.Windows.Forms.MaskedTextBox"/> control.
  /// </summary>
  public partial class MaskedTextBoxControl : DataFilterBaseControl
  {
    private MaskedTextBox maskedTextBox;

    #region Properties
    /// <summary>
    /// Gets an internal <b>MaskedTextBox</b>.
    /// </summary>
    [Browsable(false)]
    public MaskedTextBox MaskedTextBox
    {
      get { return maskedTextBox; }
    }

    /// <summary>
    /// Gets or sets the input mask to use at run time.
    /// Wraps the <see cref="System.Windows.Forms.MaskedTextBox.Mask"/> property.
    /// </summary>
    [Category("Data")]
    public string Mask
    {
      get { return MaskedTextBox.Mask; }
      set { MaskedTextBox.Mask = value; (DrawControl as MaskedTextBox).Mask = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the user is allowed to reenter literal values.
    /// Wraps the <see cref="System.Windows.Forms.MaskedTextBox.SkipLiterals"/> property.
    /// </summary>
    [DefaultValue(true)]
    [Category("Behavior")]
    public bool SkipLiterals
    {
      get { return MaskedTextBox.SkipLiterals; }
      set { MaskedTextBox.SkipLiterals = value; (DrawControl as MaskedTextBox).SkipLiterals = value; }
    }

    /// <summary>
    /// Gets or sets how text is aligned in a masked text box control.
    /// Wraps the <see cref="System.Windows.Forms.MaskedTextBox.TextAlign"/> property.
    /// </summary>
    [DefaultValue(HorizontalAlignment.Left)]
    [Category("Appearance")]
    public HorizontalAlignment TextAlign
    {
      get { return MaskedTextBox.TextAlign; }
      set { MaskedTextBox.TextAlign = value; (DrawControl as MaskedTextBox).TextAlign = value; }
    }

        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        protected override object GetValue()
    {
      return Text;
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      MaskedTextBoxControl c = writer.DiffObject as MaskedTextBoxControl;
      base.Serialize(writer);

      if (Mask != c.Mask)
        writer.WriteStr("Mask", Mask);
      if (SkipLiterals != c.SkipLiterals)
        writer.WriteBool("SkipLiterals", SkipLiterals);
      if (TextAlign != c.TextAlign)
        writer.WriteValue("TextAlign", TextAlign);
    }

    /// <inheritdoc/>
    public override void OnLeave(EventArgs e)
    {
      base.OnLeave(e);
      OnFilterChanged();
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <b>MaskedTextBoxControl</b> class with default settings. 
    /// </summary>
    public MaskedTextBoxControl()
    {
      maskedTextBox = new MaskedTextBox();
      Control = maskedTextBox;
      BindableProperty = this.GetType().GetProperty("Text");
            DrawControl = new MaskedTextBox();
            DrawControl.Width = (int)(DpiScale * Control.Width);
    }
  }
}
