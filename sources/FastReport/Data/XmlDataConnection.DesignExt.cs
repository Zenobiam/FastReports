using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Forms;
using FastReport.Data.ConnectionEditors;
using System.Data.Common;

namespace FastReport.Data
{
  partial class XmlDataConnection
  {
    #region Public Methods
    /// <inheritdoc/>
    public override ConnectionEditorBase GetEditor()
    {
      return new XmlConnectionEditor();
    }

    /// <inheritdoc/>
    public override string GetConnectionId()
    {
      return "Xml: " + XmlFile;
    }

    /// <inheritdoc/>
    public override string[] GetTableNames()
    {
      string[] result = new string[DataSet.Tables.Count];
      for (int i = 0; i < DataSet.Tables.Count; i++)
      {
        result[i] = DataSet.Tables[i].TableName;
      }
      return result;
    }

    /// <inheritdoc/>
    public override void TestConnection()
    {
      using (DataSet dataset = CreateDataSet())
      {
      }
    }
    #endregion
  }
}
