using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data.Common;
using System.Data.OleDb;
using FastReport.Forms;
using FastReport.Utils;
using FastReport.Data.ConnectionEditors;

namespace FastReport.Data
{
  partial class MsAccessDataConnection
  {
    /// <inheritdoc/>
    public override ConnectionEditorBase GetEditor()
    {
      return new MsAccessConnectionEditor();
    }

    /// <inheritdoc/>
    public override string GetConnectionId()
    {
      OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder(ConnectionString);
      return "MsAccess: " + builder.DataSource;
    }
  }
}
