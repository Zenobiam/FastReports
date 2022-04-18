using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using FastReport.Utils;
using FastReport.Forms;
using FastReport.Data.ConnectionEditors;

namespace FastReport.Data
{
  partial class OleDbDataConnection
  {
    private void GetDBObjectNames(string name, List<string> list)
    {
      DataTable schema = null;
      DbConnection conn = GetConnection();
      try
      {
        using (OleDbCommandBuilder builder = new OleDbCommandBuilder())
        {
          OpenConnection(conn);
          schema = conn.GetSchema("Tables", new string[] { null, null, null, name });

          foreach (DataRow row in schema.Rows)
          {
            string tableName = row["TABLE_NAME"].ToString();
            string schemaName = row["TABLE_SCHEMA"].ToString();
            if (String.IsNullOrEmpty(schemaName))
              list.Add(tableName);
            else
              list.Add(schemaName + "." + builder.QuoteIdentifier(tableName, conn as OleDbConnection));
          }
        }
      }
      finally
      {
        DisposeConnection(conn);
      }
    }

    /// <inheritdoc/>
    public override string[] GetTableNames()
    {
      List<string> list = new List<string>();
      GetDBObjectNames("TABLE", list);
      GetDBObjectNames("VIEW", list);
      return list.ToArray();
    }

    /// <inheritdoc/>
    public override int GetDefaultParameterType()
    {
      return (int)OleDbType.VarChar;
    }

    /// <inheritdoc/>
    public override ConnectionEditorBase GetEditor()
    {
      return new OleDbConnectionEditor();
    }

    /// <inheritdoc/>
    public override string GetConnectionId()
    {
      return "OleDb: " + ConnectionString;
    }
  }
}
