using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Data;
using System.Windows.Forms;

namespace FastReport.Dialog
{
  /// <summary>
  /// Represents a Windows control that allows the user to select a date and a time and to display the date and time with a specified format.
  /// Wraps the <see cref="System.Windows.Forms.DateTimePicker"/> control.
  /// </summary>
  public partial class DateTimePickerControl : DataFilterBaseControl
  {
    private DateTimePicker dateTimePicker;
    private string valueChangedEvent;

    #region Properties
    /// <summary>
    /// Occurs after the date has been changed.
    /// Wraps the <see cref="System.Windows.Forms.DateTimePicker.ValueChanged"/> event.
    /// </summary>
    public event EventHandler ValueChanged;

    /// <summary>
    /// Gets an internal <b>DateTimePicker</b>.
    /// </summary>
    [Browsable(false)]
    public DateTimePicker DateTimePicker
    {
      get { return dateTimePicker; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the Value property has been set with a valid date/time value and the displayed value is able to be updated.
    /// Wraps the <see cref="System.Windows.Forms.DateTimePicker.Checked"/> property.
    /// </summary>
    [DefaultValue(false)]
    [Category("Behavior")]
    public bool Checked
    {
      get { return DateTimePicker.Checked; }
      set 
      { 
        DateTimePicker.Checked = value;
#if !FRCORE
        (DrawControl as DateTimePicker).Checked = value; 
#endif
      }
    }

    /// <summary>
    /// Gets or sets the custom date/time format string.
    /// Wraps the <see cref="System.Windows.Forms.DateTimePicker.CustomFormat"/> property.
    /// </summary>
    [Category("Data")]
    public string CustomFormat
    {
      get { return DateTimePicker.CustomFormat; }
      set 
      { 
        DateTimePicker.CustomFormat = value;
#if !FRCORE
        (DrawControl as DateTimePicker).CustomFormat = value; 
#endif
      }
    }

    /// <summary>
    /// Gets or sets the alignment of the drop-down calendar on the DateTimePicker control.
    /// Wraps the <see cref="System.Windows.Forms.DateTimePicker.DropDownAlign"/> property.
    /// </summary>
    [DefaultValue(LeftRightAlignment.Left)]
    [Category("Appearance")]
    public LeftRightAlignment DropDownAlign 
    {
      get { return DateTimePicker.DropDownAlign; }
      set 
      { 
        DateTimePicker.DropDownAlign = value;
#if !FRCORE
        (DrawControl as DateTimePicker).DropDownAlign = value; 
#endif
      }
    }

    /// <summary>
    /// Gets or sets the format of the date and time displayed in the control.
    /// Wraps the <see cref="System.Windows.Forms.DateTimePicker.Format"/> property.
    /// </summary>
    [DefaultValue(DateTimePickerFormat.Long)]
    [Category("Data")]
    public DateTimePickerFormat Format
    {
      get { return DateTimePicker.Format; }
      set 
      { 
        DateTimePicker.Format = value;
#if !FRCORE
        (DrawControl as DateTimePicker).Format = value; 
#endif
      }
    }

    /// <summary>
    /// Gets or sets the maximum date and time that can be selected in the control.
    /// Wraps the <see cref="System.Windows.Forms.DateTimePicker.MaxDate"/> property.
    /// </summary>
    [Category("Data")]
    public DateTime MaxDate
    {
      get { return DateTimePicker.MaxDate; }
      set 
      { 
        DateTimePicker.MaxDate = value;
#if !FRCORE
        (DrawControl as DateTimePicker).MaxDate = value; 
#endif
      }
    }

    /// <summary>
    /// Gets or sets the minimum date and time that can be selected in the control.
    /// Wraps the <see cref="System.Windows.Forms.DateTimePicker.MinDate"/> property.
    /// </summary>
    [Category("Data")]
    public DateTime MinDate
    {
      get { return DateTimePicker.MinDate; }
      set 
      { 
        DateTimePicker.MinDate = value;
#if !FRCORE
        (DrawControl as DateTimePicker).MinDate = value; 
#endif
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether a check box is displayed to the left of the selected date.
    /// Wraps the <see cref="System.Windows.Forms.DateTimePicker.ShowCheckBox"/> property.
    /// </summary>
    [DefaultValue(false)]
    [Category("Appearance")]
    public bool ShowCheckBox
    {
      get { return DateTimePicker.ShowCheckBox; }
      set 
      { 
        DateTimePicker.ShowCheckBox = value;
#if !FRCORE
        (DrawControl as DateTimePicker).ShowCheckBox = value; 
#endif
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether a spin button control (also known as an up-down control) is used to adjust the date/time value.
    /// Wraps the <see cref="System.Windows.Forms.DateTimePicker.ShowUpDown"/> property.
    /// </summary>
    [DefaultValue(false)]
    [Category("Appearance")]
    public bool ShowUpDown
    {
      get { return DateTimePicker.ShowUpDown; }
      set 
      { 
        DateTimePicker.ShowUpDown = value;
#if !FRCORE
        (DrawControl as DateTimePicker).ShowUpDown = value; 
#endif
      }
    }

    /// <summary>
    /// Gets or sets the date/time value assigned to the control.
    /// Wraps the <see cref="System.Windows.Forms.DateTimePicker.Value"/> property.
    /// </summary>
    [Category("Data")]
    public DateTime Value
    {
      get { return DateTimePicker.Value; }
      set 
      { 
        DateTimePicker.Value = value;
#if !FRCORE
        (DrawControl as DateTimePicker).Value = value; 
#endif
      }
    }

    /// <summary>
    /// Gets or sets a script method name that will be used to handle the 
    /// <see cref="ValueChanged"/> event.
    /// </summary>
    [Category("Events")]
    public string ValueChangedEvent
    {
      get { return valueChangedEvent; }
      set { valueChangedEvent = value; }
    }
#endregion

#region Private Methods
    private void DateTimePicker_ValueChanged(object sender, EventArgs e)
    {
      OnValueChanged(e);
    }
#endregion

#region Protected Methods
    /// <inheritdoc/>
    protected override void AttachEvents()
    {
      base.AttachEvents();
      DateTimePicker.ValueChanged += DateTimePicker_ValueChanged;
    }

    /// <inheritdoc/>
    protected override void DetachEvents()
    {
      base.DetachEvents();
      DateTimePicker.ValueChanged -= DateTimePicker_ValueChanged;
    }

    /// <inheritdoc/>
    protected override object GetValue()
    {
      if (Format == DateTimePickerFormat.Long || Format == DateTimePickerFormat.Short)
        return new DateTime(Value.Year, Value.Month, Value.Day);
      return Value;
    }
#endregion

#region Public Methods
    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      DateTimePickerControl c = writer.DiffObject as DateTimePickerControl;
      base.Serialize(writer);

      if (Checked != c.Checked)
        writer.WriteBool("Checked", Checked);
      if (CustomFormat != c.CustomFormat)
        writer.WriteStr("CustomFormat", CustomFormat);
      if (DropDownAlign != c.DropDownAlign)
        writer.WriteValue("DropDownAlign", DropDownAlign);
      if (Format != c.Format)
        writer.WriteValue("Format", Format);
      if (MaxDate != c.MaxDate)
        writer.WriteValue("MaxDate", MaxDate);
      if (MinDate != c.MinDate)
        writer.WriteValue("MinDate", MinDate);
      if (ShowCheckBox != c.ShowCheckBox)
        writer.WriteBool("ShowCheckBox", ShowCheckBox);
      if (ShowUpDown != c.ShowUpDown)
        writer.WriteBool("ShowUpDown", ShowUpDown);
      if (Value != c.Value)
        writer.WriteValue("Value", Value);
      if (ValueChangedEvent != c.ValueChangedEvent)
        writer.WriteStr("ValueChangedEvent", ValueChangedEvent);
    }

    /// <summary>
    /// This method fires the <b>ValueChanged</b> event and the script code connected to the <b>ValueChangedEvent</b>.
    /// </summary>
    /// <param name="e">Event data.</param>
    public virtual void OnValueChanged(EventArgs e)
    {
      OnFilterChanged();
      if (ValueChanged != null)
        ValueChanged(this, e);
      InvokeEvent(ValueChangedEvent, e);
    }
#endregion

    /// <summary>
    /// Initializes a new instance of the <b>DateTimePickerControl</b> class with default settings. 
    /// </summary>
    public DateTimePickerControl()
    {
      dateTimePicker = new DateTimePicker();
      Control = dateTimePicker;
      BindableProperty = this.GetType().GetProperty("Value");
#if !FRCORE
            DrawControl = new DateTimePicker();
            DrawControl.Font = DrawFont;
            DrawControl.Width = (int)(Control.Width * DpiScale);
#endif
    }
  }
}
