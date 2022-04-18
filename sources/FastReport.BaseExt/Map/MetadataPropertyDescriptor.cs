using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace FastReport.Map
{
  internal class MetadataPropertyDescriptor : PropertyDescriptor
  {
    private ShapeSpatialData metadata;
    private string key;

    public override string Category
    {
      get { return "Spatial"; }
    }

    public override string DisplayName
    {
      get { return key; }
    }

    public override string Description
    {
      get { return ""; }
    }

    public override Type ComponentType
    {
      get { return metadata.GetType(); }
    }

    public override bool IsReadOnly
    {
      get { return false; }
    }

    public override Type PropertyType
    {
      get { return typeof(string); }
    }

    public override bool CanResetValue(object component)
    {
      return false;
    }

    public override object GetValue(object component)
    {
      return metadata.GetValue(key);
    }

    public override void ResetValue(object component)
    {
    }

    public override void SetValue(object component, object value)
    {
      metadata.SetValue(key, (string)value);
    }

    public override bool ShouldSerializeValue(object component)
    {
      return false;
    }

    public MetadataPropertyDescriptor(ShapeSpatialData metadata, string key, Attribute[] attributes)
      : base(key, attributes)
    {
            this.metadata = metadata;
            this.key = key;
    }
  }

}
