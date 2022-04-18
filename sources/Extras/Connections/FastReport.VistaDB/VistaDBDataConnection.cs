using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using VistaDB.Provider;
using VistaDB;
using FastReport.Data.ConnectionEditors;
using System.Data;

namespace FastReport.Data
{
  public class VistaDBDataConnection : DataConnectionBase
  {
    private void GetDBObjectNames(string name, List<string> list)
    {
      DataTable schema = null;
      DbConnection connection = GetConnection();
      try
      {
        OpenConnection(connection);
        schema = connection.GetSchema(name, null);
      }
      finally
      {
        DisposeConnection(connection);
      }
      foreach (DataRow row in schema.Rows)
      {
        list.Add(row["TABLE_NAME"].ToString());
      }
    }

    public override string[] GetTableNames()
    {
      List<string> list = new List<string>();
      GetDBObjectNames(VistaDBConnection.SCHEMA_TABLES, list);
      GetDBObjectNames(VistaDBConnection.SCHEMA_VIEWS, list);
      return list.ToArray();
    }

    public override string QuoteIdentifier(string value, DbConnection connection)
    {
      return "\"" + value + "\"";
    }

    protected override string GetConnectionStringWithLoginInfo(string userName, string password)
    {
      VistaDBConnectionStringBuilder builder = new VistaDBConnectionStringBuilder(ConnectionString);

      builder.Password = password;

      return builder.ToString();
    }

    public override Type GetConnectionType()
    {
      return typeof(VistaDBConnection);
    }

    public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection,
      CommandParameterCollection parameters)
    {
      VistaDBDataAdapter adapter = new VistaDBDataAdapter(selectCommand, connection as VistaDBConnection);
      foreach (CommandParameter p in parameters)
      {
        VistaDBParameter parameter = adapter.SelectCommand.Parameters.Add(p.Name, (VistaDBType)p.DataType, p.Size);
        object value = p.Value;
        parameter.Value = value is Variant ? ((Variant)value).Value : value;
      }
      return adapter;
    }

    public override Type GetParameterType()
    {
      return typeof(VistaDBType);
    }

    public override string GetConnectionId()
    {
      VistaDBConnectionStringBuilder builder = new VistaDBConnectionStringBuilder(ConnectionString);
      string info = "";
      try
      {
        info = builder.DataSource;
      }
      catch
      {
      }
      return "VistaDB: " + info;
    }

    public override ConnectionEditorBase GetEditor()
    {
      return new VistaDBConnectionEditor();
    }
  }
}
