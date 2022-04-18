using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using FastReport.Data;

namespace FastReport.Map
{
  /// <summary>
  /// Represents the spatial data of a shape.
  /// </summary>
  [TypeConverter(typeof(ShapeSpatialDataConverter))]
  public class ShapeSpatialData
  {
    #region Fields
    private Dictionary<string, string> dictionary;
    #endregion

    #region Public Methods
    internal string GetAsString()
    {
      StringBuilder result = new StringBuilder();
      foreach (KeyValuePair<string, string> keyValue in dictionary)
      {
        result.Append(keyValue.Key).Append("=").Append(keyValue.Value).Append("\r\n");
      }
      if (result.Length > 2)
        result.Remove(result.Length - 2, 2);
      return result.ToString();
    }

    internal void SetAsString(string value)
    {
      dictionary.Clear();
      if (String.IsNullOrEmpty(value))
        return;

      string[] lines = value.Split(new string[] { "\r\n" }, StringSplitOptions.None);
      foreach (string line in lines)
      {
        string[] keyValue = line.Split(new char[] { '=' });
        if (keyValue != null && keyValue.Length == 2)
          dictionary.Add(keyValue[0], keyValue[1]);
      }
    }

    /// <summary>
    /// Copies contents from another spatial data object.
    /// </summary>
    /// <param name="src">The object to copy contents from.</param>
    public void Assign(ShapeSpatialData src)
    {
      SetAsString(src.GetAsString());
    }

    /// <summary>
    /// Compares two spatial data objects. 
    /// </summary>
    /// <param name="src">The spatial object to compare with.</param>
    /// <returns><b>true</b> if spatial objects are identical.</returns>
    public bool IsEqual(ShapeSpatialData src)
    {
      if (src == null)
        return false;
      return GetAsString() == src.GetAsString();
    }
    
    /// <summary>
    /// Gets a value by its key.
    /// </summary>
    /// <param name="key">The key of value.</param>
    /// <returns>The value.</returns>
    public string GetValue(string key)
    {
      if (dictionary.ContainsKey(key))
        return dictionary[key];
      return "";
    }

    /// <summary>
    /// Sets a value by its key.
    /// </summary>
    /// <param name="key">The key of value.</param>
    /// <param name="value">The value.</param>
    public void SetValue(string key, string value)
    {
      dictionary[key] = value;
    }

    /// <summary>
    /// Gets a list of keys.
    /// </summary>
    /// <returns>The list of keys.</returns>
    public List<string> GetKeys()
    {
      List<string> result = new List<string>();
      foreach (string key in dictionary.Keys)
      {
        result.Add(key);
      }
      return result;
    }

    #endregion // Public Methods


    /// <summary>
    /// Creates a new instance of the <see cref="ShapeSpatialData"/> class.
    /// </summary>
    public ShapeSpatialData()
    {
      dictionary = new Dictionary<string, string>();
    }
  }
}
