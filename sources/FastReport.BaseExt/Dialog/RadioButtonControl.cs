using System;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using System.Windows.Forms;

namespace FastReport.Dialog
{
  /// <summary>
  /// Enables the user to select a single option from a group of choices when paired with other RadioButton controls.
  /// Wraps the <see cref="System.Windows.Forms.RadioButton"/> control.
  /// </summary>
  public partial class RadioButtonControl : ButtonBaseControl
  {
    private RadioButton radioButton;
    private string checkedChangedEvent;

    #region Properties
    /// <summary>
    /// Occurs when the value of the Checked property changes.
    /// Wraps the <see cref="System.Windows.Forms.RadioButton.CheckedChanged"/> event.
    /// </summary>
    public event EventHandler CheckedChanged;

    /// <summary>
    /// Gets an internal <b>RadioButton</b>.
    /// </summary>
    [Browsable(false)]
    public RadioButton RadioButton
    {
      get { return radioButton; }
    }

    /// <summary>
    /// Gets or sets the location of the check box portion of the RadioButton. 
    /// Wraps the <see cref="System.Windows.Forms.RadioButton.CheckAlign"/> property.
    /// </summary>
    [DefaultValue(ContentAlignment.MiddleLeft)]
    [Category("Appearance")]
    public ContentAlignment CheckAlign
    {
      get { return RadioButton.CheckAlign; }
      set 
      { 
        RadioButton.CheckAlign = value;
#if !FRCORE
        (DrawControl as RadioButton).CheckAlign = value; 
#endif
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control is checked.
    /// Wraps the <see cref="System.Windows.Forms.RadioButton.Checked"/> property.
    /// </summary>
    [DefaultValue(false)]
    [Category("Appearance")]
    public bool Checked
    {
      get { return RadioButton.Checked; }
      set 
      { 
        RadioButton.Checked = value;
#if !FRCORE
        (DrawControl as RadioButton).Checked = value; 
#endif
      }
    }

    /// <summary>
    /// Gets or sets a script method name that will be used to handle the 
    /// <see cref="CheckedChanged"/> event.
    /// </summary>
    [Category("Events")]
    public string CheckedChangedEvent
    {
      get { return checkedChangedEvent; }
      set { checkedChangedEvent = value; }
    }
#endregion

#region Private Methods
    private void RadioButton_CheckedChanged(object sender, EventArgs e)
    {
      OnCheckedChanged(e);
    }
#endregion

#region Protected Methods

    /// <inheritdoc/>
    protected override void AttachEvents()
    {
      base.AttachEvents();
      RadioButton.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
    }

    /// <inheritdoc/>
    protected override void DetachEvents()
    {
      base.DetachEvents();
      RadioButton.CheckedChanged -= new EventHandler(RadioButton_CheckedChanged);
    }

    /// <inheritdoc/>
    protected override object GetValue()
    {
      return Checked;
    }
#endregion

#region Public Methods
    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      RadioButtonControl c = writer.DiffObject as RadioButtonControl;
      base.Serialize(writer);

      if (CheckAlign != c.CheckAlign)
        writer.WriteValue("CheckAlign", CheckAlign);
      if (Checked != c.Checked)
        writer.WriteBool("Checked", Checked);
      if (CheckedChangedEvent != c.CheckedChangedEvent)
        writer.WriteStr("CheckedChangedEvent", CheckedChangedEvent);
    }


    /// <summary>
    /// This method fires the <b>CheckedChanged</b> event and the script code connected to the <b>CheckedChangedEvent</b>.
    /// </summary>
    /// <param name="e">Event data.</param>
    public virtual void OnCheckedChanged(EventArgs e)
    {
      OnFilterChanged();
      if (CheckedChanged != null)
        CheckedChanged(this, e);
      InvokeEvent(CheckedChangedEvent, e);
    }

#endregion

    /// <summary>
    /// Initializes a new instance of the <b>RadioButtonControl</b> class with default settings. 
    /// </summary>
    public RadioButtonControl()
    {
      radioButton = new RadioButton();
      Control = radioButton;
      RadioButton.AutoSize = true;
      RadioButton.TabStop = true;
      BindableProperty = this.GetType().GetProperty("Checked");
#if !FRCORE
            DrawControl = new RadioButton();
            (DrawControl as RadioButton).AutoSize = true;
            DrawControl.Font = DrawFont;
            (DrawControl as RadioButton).TabStop = true;
#endif
    }
  }
}
