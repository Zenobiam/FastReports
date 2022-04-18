using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;
using FastReport.Utils;
using FastReport.TypeEditors;
using FastReport.TypeConverters;
using FastReport.Format;
using FastReport.Controls;

namespace FastReport.Data
{
  partial class Column
  {
    #region Public Methods
    /// <inheritdoc/>
    public override void Delete()
    {
      if (Calculated)
        Dispose();
      else
        base.Delete();
    }

    internal int GetImageIndex()
    {
      if (Calculated)
        return 230;
      if (this is DataSourceBase)
        return 222;
      if (Columns.Count > 0)
        return 233;
      return DataTreeHelper.GetTypeImageIndex(DataType);  
    }

    internal Type GetBindableControlType()
    {
      switch (BindableControl)
      {
        case ColumnBindableControl.Text:
          return typeof(TextObject);

        case ColumnBindableControl.Picture:
          return typeof(PictureObject);

        case ColumnBindableControl.CheckBox:
          return typeof(CheckBoxObject);

        case ColumnBindableControl.RichText:
          return typeof(RichObject);
        case ColumnBindableControl.Custom:
          Type controlType = RegisteredObjects.FindType(CustomBindableControl);
          if (controlType == null)
            return typeof(TextObject);
          return controlType;  
      }
      return null;
    }

    #endregion
  }
}
