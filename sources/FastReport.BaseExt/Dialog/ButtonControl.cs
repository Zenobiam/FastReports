using System.ComponentModel;
using FastReport.Utils;
using System.Windows.Forms;
using System.Drawing;

namespace FastReport.Dialog
{
  /// <summary>
  /// Represents a Windows button control. 
  /// Wraps the <see cref="System.Windows.Forms.Button"/> control.
  /// </summary>
  public partial class ButtonControl : ButtonBaseControl
  {
    private Button button;
    
    #region Properties
    /// <summary>
    /// Gets an internal <b>Button</b>.
    /// </summary>
    [Browsable(false)]
    public Button Button
    {
      get { return button; }
    }

    /// <summary>
    /// Gets or sets a value that is returned to the parent form when the button is clicked.
    /// Wraps the <see cref="System.Windows.Forms.Button.DialogResult"/> property.
    /// </summary>
    [DefaultValue(DialogResult.None)]
    [Category("Behavior")]
    public DialogResult DialogResult
    {
      get { return Button.DialogResult; }
      set { Button.DialogResult = value; }
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      ButtonControl c = writer.DiffObject as ButtonControl;
      base.Serialize(writer);

      if (DialogResult != c.DialogResult)
        writer.WriteValue("DialogResult", DialogResult);
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <b>ButtonControl</b> class with default settings. 
    /// </summary>
    public ButtonControl()
    {
      button = new Button();
      Control = button;
#if !FRCORE
            DrawControl = new Button();
            DrawControl.Font = DrawFont;
            
            DrawControl.Size = new Size((int)(Control.Width * DpiScale), (int)(Control.Height * DpiScale));
#endif
    }
  }
}
