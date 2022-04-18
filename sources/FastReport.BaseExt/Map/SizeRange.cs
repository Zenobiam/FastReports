using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using FastReport.Utils;

namespace FastReport.Map
{
  /// <summary>
  /// Represents a set of size ranges used to draw points based on analytical value.
  /// </summary>
  [TypeConverter(typeof(FastReport.TypeConverters.FRExpandableObjectConverter))]
  public class SizeRanges
  {
    #region Fields
    private List<SizeRange> ranges;
    private float startSize;
    private float endSize;
    #endregion // Fields

    #region Properties
     /// <summary>
    /// Gets the list of ranges.
    /// </summary>
    public List<SizeRange> Ranges
    {
      get { return ranges; }
    }

    /// <summary>
    /// Gets or sets the number of ranges.
    /// </summary>
    public int RangeCount
    {
      get { return Ranges.Count; }
      set
      {
        if (Ranges.Count < value)
        {
          while (Ranges.Count < value)
            Ranges.Add(new SizeRange());
        }
        else if (Ranges.Count > value)
        {
          while (Ranges.Count > value)
            Ranges.RemoveAt(Ranges.Count - 1);
        }
      }
    }

    /// <summary>
    /// Gets or sets the start size.
    /// </summary>
    public float StartSize
    {
      get { return startSize; }
      set { startSize = value; }
    }

    /// <summary>
    /// Gets or sets the end size.
    /// </summary>
    public float EndSize
    {
      get { return endSize; }
      set { endSize = value; }
    }

    /// <summary>
    /// Gets or sets ranges as a string.
    /// </summary>
    [Browsable(false)]
    public string RangesAsString
    {
      get { return GetRangesAsString(); }
      set { SetRangesAsString(value); }
    }
    #endregion // Properties

    #region Private Methods
    private string GetRangesAsString()
    {
      StringBuilder result = new StringBuilder();
      foreach (SizeRange range in Ranges)
      {
        result.Append(range.GetAsString()).Append("\r\n");
      }
      if (result.Length > 2)
        result.Remove(result.Length - 2, 2);
      return result.ToString();
    }

    private void SetRangesAsString(string value)
    {
      Ranges.Clear();
      if (String.IsNullOrEmpty(value))
        return;
      string[] values = value.Split(new string[] { "\r\n" }, StringSplitOptions.None);
      foreach (string val in values)
      {
        SizeRange range = new SizeRange();
        range.SetAsString(val);
        Ranges.Add(range);
      }
    }
    #endregion // Private Methods

    #region Public Methods
    /// <summary>
    /// Copies the contents of another SizeRanges.
    /// </summary>
    /// <param name="src">The SizeRanges instance to copy the contents from.</param>
    public void Assign(SizeRanges src)
    {
      StartSize = src.StartSize;
      EndSize = src.EndSize;
      RangeCount = src.RangeCount;
      for (int i = 0; i < RangeCount; i++)
        Ranges[i].Assign(src.Ranges[i]);
    }

    /// <summary>
    /// Gets a size associated with given analytical value.
    /// </summary>
    /// <param name="value">The analytical value.</param>
    /// <returns>The size associated with this value, or 0 if no association found.</returns>
    public float GetSize(double value)
    {
      foreach (SizeRange range in Ranges)
      {
        if (value >= range.StartValue && value < range.EndValue)
          return range.Size;
      }
      return 0;
    }

    internal void Fill(double min, double max)
    {
      double delta = (max - min) / RangeCount;
      float sizeDelta = (EndSize - StartSize) / RangeCount;
      for (int i = 0; i < RangeCount; i++)
      {
        SizeRange range = Ranges[i];
        if (range.IsSizeEmpty)
          range.Size = StartSize + sizeDelta * i;
        if (range.IsStartValueEmpty)
          range.StartValue = min + delta * i;
        // make last EndValue bigger to fit largest data value in this range
        if (range.IsEndValueEmpty)
          range.EndValue = min + delta * (i + 1) + (i == RangeCount - 1 ? 0.1 : 0);
      }
    }
    
    internal void SaveState()
    {
      foreach (SizeRange range in Ranges)
      {
        range.SaveState();
      }
    }

    internal void RestoreState()
    {
      foreach (SizeRange range in Ranges)
      {
        range.RestoreState();
      }
    }

    internal void Serialize(FRWriter writer, string prefix)
    {
      writer.WriteFloat(prefix + ".StartSize", StartSize);
      writer.WriteFloat(prefix + ".EndSize", EndSize);
      writer.WriteStr(prefix + ".RangesAsString", RangesAsString);
    }
    #endregion // Public Methods

    /// <summary>
    /// Initializes a new instance of the <see cref="SizeRanges"/> class.
    /// </summary>
    public SizeRanges()
    {
      ranges = new List<SizeRange>();
      startSize = 4;
      endSize = 20;
    }
  }

  /// <summary>
  /// Represents a single size range.
  /// </summary>
  public class SizeRange
  {
    #region Fields
    private float size;
    private double startValue;
    private double endValue;
    private SizeRange state;
    #endregion // Fields

    #region Properties
    /// <summary>
    /// Gets or sets size of the range.
    /// </summary>
    public float Size
    {
      get { return size; }
      set { size = value; }
    }

    /// <summary>
    /// Gets or sets start value of the range.
    /// </summary>
    public double StartValue
    {
      get { return startValue; }
      set { startValue = value; }
    }

    /// <summary>
    /// Gets or sets end value of the range.
    /// </summary>
    public double EndValue
    {
      get { return endValue; }
      set { endValue = value; }
    }

    internal bool IsSizeEmpty
    {
      get { return float.IsNaN(Size); }
    }

    internal bool IsStartValueEmpty
    {
      get { return double.IsNaN(StartValue); }
    }

    internal bool IsEndValueEmpty
    {
      get { return double.IsNaN(EndValue); }
    }
    #endregion // Properties

    #region Public Methods
    /// <summary>
    /// Copies the contents of another SizeRange.
    /// </summary>
    /// <param name="src">The SizeRange instance to copy the contents from.</param>
    public void Assign(SizeRange src)
    {
      Size = src.Size;
      StartValue = src.StartValue;
      EndValue = src.EndValue;
    }
    
    internal void SaveState()
    {
      if (state == null)
        state = new SizeRange();
      state.Assign(this);
    }

    internal void RestoreState()
    {
      if (state != null)
        Assign(state);
    }

    internal string GetAsString()
    {
      return Size.ToString(CultureInfo.InvariantCulture.NumberFormat) + ";" + 
        StartValue.ToString(CultureInfo.InvariantCulture.NumberFormat) + ";" +
        EndValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
    }

    internal void SetAsString(string value)
    {
      Reset();
      if (String.IsNullOrEmpty(value))
        return;
      string[] val = value.Split(new char[] { ';' });
      if (val.Length != 3)
        return;
      Size = float.Parse(val[0], CultureInfo.InvariantCulture.NumberFormat);
      StartValue = double.Parse(val[1], CultureInfo.InvariantCulture.NumberFormat);
      EndValue = double.Parse(val[2], CultureInfo.InvariantCulture.NumberFormat);
    }

    internal void Reset()
    {
      size = float.NaN;
      startValue = double.NaN;
      endValue = double.NaN;
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="SizeRange"/> class.
    /// </summary>
    public SizeRange()
    {
      Reset();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SizeRange"/> class with a specified parameters.
    /// </summary>
    /// <param name="size">The size of the range.</param>
    /// <param name="startValue">The start value of the range.</param>
    /// <param name="endValue">The end value of the range.</param>
    public SizeRange(float size, double startValue, double endValue)
    {
            this.size = size;
            this.startValue = startValue;
            this.endValue = endValue;
    }
  }
}
