using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Data;

namespace FastReport.Dialog
{
  /// <summary>
  /// Represents a Windows spin box (also known as an up-down control) that displays numeric values.
  /// Wraps the <see cref="System.Windows.Forms.NumericUpDown"/> control.
  /// </summary>
  public partial class NumericUpDownControl : DataFilterBaseControl
  {
    private NumericUpDown numericUpDown;
    private string valueChangedEvent;

    #region Properties
    /// <summary>
    /// Occurs when the Value property has been changed in some way.
    /// Wraps the <see cref="System.Windows.Forms.NumericUpDown.ValueChanged"/> event.
    /// </summary>
    public event EventHandler ValueChanged;

    /// <summary>
    /// Gets an internal <b>NumericUpDown</b>.
    /// </summary>
    [Browsable(false)]
    public NumericUpDown NumericUpDown
    {
      get { return numericUpDown; }
    }

    /// <summary>
    /// Gets or sets the number of decimal places to display in the up-down control. 
    /// Wraps the <see cref="System.Windows.Forms.NumericUpDown.DecimalPlaces"/> property.
    /// </summary>
    [DefaultValue(0)]
    [Category("Data")]
    public int DecimalPlaces
    {
      get { return NumericUpDown.DecimalPlaces; }
      set { NumericUpDown.DecimalPlaces = value; (DrawControl as NumericUpDown).DecimalPlaces = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the up-down control should display the value it contains in hexadecimal format.
    /// Wraps the <see cref="System.Windows.Forms.NumericUpDown.Hexadecimal"/> property.
    /// </summary>
    [DefaultValue(false)]
    [Category("Appearance")]
    public bool Hexadecimal
    {
      get { return NumericUpDown.Hexadecimal; }
      set { NumericUpDown.Hexadecimal = value; (DrawControl as NumericUpDown).Hexadecimal = value; }
    }

    /// <summary>
    /// Gets or sets the value to increment or decrement the up-down control when the up or down buttons are clicked.
    /// Wraps the <see cref="System.Windows.Forms.NumericUpDown.Increment"/> property.
    /// </summary>
    [DefaultValue(1f)]
    [Category("Data")]
    public float Increment
    {
      get { return (float)NumericUpDown.Increment; }
      set { NumericUpDown.Increment = (decimal)value; (DrawControl as NumericUpDown).Increment = (decimal)value; }
    }

    /// <summary>
    /// Gets or sets the maximum value for the up-down control. 
    /// Wraps the <see cref="System.Windows.Forms.NumericUpDown.Maximum"/> property.
    /// </summary>
    [DefaultValue(typeof(decimal), "100")]
    [Category("Data")]
    public decimal Maximum
    {
      get { return NumericUpDown.Maximum; }
      set { NumericUpDown.Maximum = value; (DrawControl as NumericUpDown).Maximum = value; }
    }

    /// <summary>
    /// Gets or sets the minimum value for the up-down control. 
    /// Wraps the <see cref="System.Windows.Forms.NumericUpDown.Minimum"/> property.
    /// </summary>
    [DefaultValue(typeof(decimal), "0")]
    [Category("Data")]
    public decimal Minimum
    {
      get { return NumericUpDown.Minimum; }
      set { NumericUpDown.Minimum = value; (DrawControl as NumericUpDown).Minimum = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether a thousands separator is displayed in the up-down control when appropriate.
    /// Wraps the <see cref="System.Windows.Forms.NumericUpDown.ThousandsSeparator"/> property.
    /// </summary>
    [DefaultValue(false)]
    [Category("Data")]
    public bool ThousandsSeparator
    {
      get { return NumericUpDown.ThousandsSeparator; }
      set { NumericUpDown.ThousandsSeparator = value; (DrawControl as NumericUpDown).ThousandsSeparator = value; }
    }

    /// <summary>
    /// Gets or sets the value assigned to the up-down control.
    /// Wraps the <see cref="System.Windows.Forms.NumericUpDown.Value"/> property.
    /// </summary>
    [Category("Data")]
    public decimal Value
    {
      get { return NumericUpDown.Value; }
      set { NumericUpDown.Value = value; (DrawControl as NumericUpDown).Value = value; }
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
    private void NumericUpDown_ValueChanged(object sender, EventArgs e)
    {
      OnValueChanged(e);
    }
    #endregion

    #region Protected Methods
    /// <inheritdoc/>
    protected override void AttachEvents()
    {
      base.AttachEvents();
      NumericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
    }

    /// <inheritdoc/>
    protected override void DetachEvents()
    {
      base.DetachEvents();
      NumericUpDown.ValueChanged -= new EventHandler(NumericUpDown_ValueChanged);
    }

    /// <inheritdoc/>
    protected override object GetValue()
    {
      object value = Value;
      Column dataColumn = DataHelper.GetColumn(Report.Dictionary, DataColumn);
      if (dataColumn != null)
        value = Convert.ChangeType((decimal)value, dataColumn.DataType);
      return value;
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      NumericUpDownControl c = writer.DiffObject as NumericUpDownControl;
      base.Serialize(writer);

      if (DecimalPlaces != c.DecimalPlaces)
        writer.WriteInt("DecimalPlaces", DecimalPlaces);
      if (Hexadecimal != c.Hexadecimal)
        writer.WriteBool("Hexadecimal", Hexadecimal);
      if (Increment != c.Increment)
        writer.WriteFloat("Increment", Increment);
      if (Maximum != c.Maximum)
        writer.WriteValue("Maximum", Maximum);
      if (Minimum != c.Minimum)
        writer.WriteValue("Minimum", Minimum);
      if (ThousandsSeparator != c.ThousandsSeparator)
        writer.WriteBool("ThousandsSeparator", ThousandsSeparator);
      if (Value != c.Value)
        writer.WriteValue("Value", Value);
      if (ValueChangedEvent != c.ValueChangedEvent)
        writer.WriteStr("ValueChangedEvent", ValueChangedEvent);
    }

    /// <inheritdoc/>
    public override void OnLeave(EventArgs e)
    {
      base.OnLeave(e);
      OnFilterChanged();
    }

    /// <summary>
    /// This method fires the <b>ValueChanged</b> event and the script code connected to the <b>ValueChangedEvent</b>.
    /// </summary>
    /// <param name="e">Event data.</param>
    public virtual void OnValueChanged(EventArgs e)
    {
      if (ValueChanged != null)
        ValueChanged(this, e);
      InvokeEvent(ValueChangedEvent, e);
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <b>NumericUpDownControl</b> class with default settings. 
    /// </summary>
    public NumericUpDownControl()
    {
      numericUpDown = new NumericUpDown();
      Control = numericUpDown;
      BindableProperty = this.GetType().GetProperty("Value");

            DrawControl = new NumericUpDown();
            var field = typeof(NumericUpDown).GetField("defaultButtonsWidth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            DrawControl.Width = (int)(DpiScale * Control.Width);
            try
            {
                field.SetValue(DrawControl, (int)(16 * DpiScale));
            }
            catch { }
        }
  }
}
