using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using FastReport.Forms;
using FastReport.Data.ConnectionEditors;

namespace FastReport.Data
{
  partial class MsSqlDataConnection
  {
    /// <inheritdoc/>
    public override int GetDefaultParameterType()
    {
      return (int)SqlDbType.VarChar;
    }

    /// <inheritdoc/>
    public override ConnectionEditorBase GetEditor()
    {
      return new MsSqlConnectionEditor();
    }

    /// <inheritdoc/>
    public override string GetConnectionId()
    {
      SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
      string info = builder.InitialCatalog;
      if (String.IsNullOrEmpty(info))
        info = builder.AttachDBFilename;
      return "MS SQL: " + info;
    }
  }
}
