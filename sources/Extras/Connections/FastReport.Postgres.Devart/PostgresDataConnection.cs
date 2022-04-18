using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using FastReport.Data.ConnectionEditors;
using Devart.Data;
using Devart.Data.PostgreSql;
using System.Data;

namespace FastReport.Data
{
  public class PostgresDataConnection : DataConnectionBase
  {
    private void GetDBObjectNames(string name, List<string> list)
    {
      DataTable schema = null;
      DbConnection connection = GetConnection();
      try
      {
        OpenConnection(connection);
        schema = connection.GetSchema("Tables", new string[] { null, "public", null, name });
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
      GetDBObjectNames("BASE TABLE", list);
      GetDBObjectNames("VIEW", list);
      return list.ToArray();
    }

    public override string QuoteIdentifier(string value, DbConnection connection)
    {
      return "\"" + value + "\"";
    }

    protected override string GetConnectionStringWithLoginInfo(string userName, string password)
    {
      PgSqlConnectionStringBuilder builder = new PgSqlConnectionStringBuilder(ConnectionString);

      builder.UserId = userName;
      builder.Password = password;

      return builder.ToString();
    }

    public override Type GetConnectionType()
    {
      return typeof(PgSqlConnection);
    }

    public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection,
      CommandParameterCollection parameters)
    {
      PgSqlDataAdapter adapter = new PgSqlDataAdapter(selectCommand, connection as PgSqlConnection);
      foreach (CommandParameter p in parameters)
      {
        PgSqlParameter parameter = adapter.SelectCommand.Parameters.Add(p.Name, (PgSqlType)p.DataType, p.Size);
        parameter.Value = p.Value;
      }
      return adapter;
    }

    public override Type GetParameterType()
    {
      return typeof(PgSqlType);
    }

    public override string GetConnectionId()
    {
      PgSqlConnectionStringBuilder builder = new PgSqlConnectionStringBuilder(ConnectionString);
      string info = "";
      try
      {
        info = builder.Database;
      }
      catch
      {
      }
      return "Postgres: " + info;
    }

    public override ConnectionEditorBase GetEditor()
    {
      return new PostgresConnectionEditor();
    }
  }
}
