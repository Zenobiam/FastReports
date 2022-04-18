using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using FastReport.Utils;
using FastReport.Forms;
using FastReport.Data.ConnectionEditors;
using System.Windows.Forms;

namespace FastReport.Data
{
  partial class OdbcDataConnection
  {
    private void GetDBObjectNames(string name, List<string> list)
    {
      DataTable schema = null;
      DbConnection conn = GetConnection();
      try
      {
        using (OdbcCommandBuilder builder = new OdbcCommandBuilder())
        {
          OpenConnection(conn);
          schema = conn.GetSchema("Tables");

          foreach (DataRow row in schema.Rows)
          {
            string tableType = GetTableType(row);
            if (String.Compare(tableType, name) == 0)
            {
              string tableName = GetTableName(row);
              string schemaName = "";
              if (schema.Columns.IndexOf("TABLE_SCHEM") != -1)
                schemaName = row["TABLE_SCHEM"].ToString();
              if (String.IsNullOrEmpty(schemaName))
                list.Add(tableName);
              else
                list.Add(schemaName + "." + builder.QuoteIdentifier(tableName, conn as OdbcConnection));
            }
          }
        }
      }
            catch(Exception ex)
            {

            }
      finally
      {
        DisposeConnection(conn);
      }
    }

        private string GetTableType(DataRow row)
        {
            if (row.Table.Columns.Contains("TABLE_TYPE"))
                return row["TABLE_TYPE"].ToString();
            for(int i = 0; i < row.Table.Columns.Count; i++)
            {
                if (row.Table.Columns[i].ColumnName.ToLower().Contains("type"))
                    return row[i].ToString();
            }
            return string.Empty;
        }

        private string GetTableName(DataRow row)
        {
            if (row.Table.Columns.Contains("TABLE_NAME"))
                return row["TABLE_NAME"].ToString();
            for (int i = 0; i < row.Table.Columns.Count; i++)
            {
                if (row.Table.Columns[i].ColumnName.ToLower().Contains("table") && row.Table.Columns[i].ColumnName.ToLower().Contains("name"))
                    return row[i].ToString();
            }
            return string.Empty;
        }

    /// <inheritdoc/>
    public override string[] GetTableNames()
    {
      List<string> list = new List<string>();
      GetDBObjectNames("TABLE", list);
      GetDBObjectNames("SYSTEM TABLE", list);
      GetDBObjectNames("VIEW", list);
      return list.ToArray();
    }

    /// <inheritdoc/>
    public override int GetDefaultParameterType()
    {
      return (int)OdbcType.VarChar;
    }

    /// <inheritdoc/>
    public override ConnectionEditorBase GetEditor()
    {
      return new OdbcConnectionEditor();
    }

    /// <inheritdoc/>
    public override string GetConnectionId()
    {
      return "ODBC: " + ConnectionString;
    }
  }
}
