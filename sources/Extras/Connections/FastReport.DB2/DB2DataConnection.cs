using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using FastReport.Data.ConnectionEditors;
using IBM.Data.DB2;

namespace FastReport.Data
{
  public class DB2DataConnection : DataConnectionBase
  {
    private void GetDBObjectNames(string name, List<string> list)
    {
      DataTable schema = null;
      DbConnection connection = GetConnection();
      try
      {
        OpenConnection(connection);
        DB2ConnectionStringBuilder builder = new DB2ConnectionStringBuilder(connection.ConnectionString);
        schema = connection.GetSchema(name, new string[] { null, builder.UserID, null, null });
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
      GetDBObjectNames("Tables", list);
      return list.ToArray();
    }

    public override string QuoteIdentifier(string value, DbConnection connection)
    {
      return "\"" + value + "\"";
    }

    protected override string GetConnectionStringWithLoginInfo(string userName, string password)
    {
      DB2ConnectionStringBuilder builder = new DB2ConnectionStringBuilder(ConnectionString);

      builder.UserID = userName;
      builder.Password = password;

      return builder.ToString();
    }

    public override Type GetConnectionType()
    {
      return typeof(DB2Connection);
    }

    public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection,
      CommandParameterCollection parameters)
    {
      DB2DataAdapter adapter = new DB2DataAdapter(selectCommand, connection as DB2Connection);
      foreach (CommandParameter p in parameters)
      {
        DB2Parameter parameter = adapter.SelectCommand.Parameters.Add(p.Name, (DB2Type)p.DataType, p.Size);
        parameter.Value = p.Value;
      }
      return adapter;
    }

    public override Type GetParameterType()
    {
      return typeof(DB2Type);
    }

    public override string GetConnectionId()
    {
      DB2ConnectionStringBuilder builder = new DB2ConnectionStringBuilder(ConnectionString);
      string info = "";
      try
      {
        info = builder.Database;
      }
      catch
      {
      }
      return "DB2: " + info;
    }

    public override ConnectionEditorBase GetEditor()
    {
      return new DB2ConnectionEditor();
    }
  
  }
}
