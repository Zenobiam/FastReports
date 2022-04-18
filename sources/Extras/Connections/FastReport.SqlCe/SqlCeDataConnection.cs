using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using FastReport;
using FastReport.Data;
using FastReport.Data.ConnectionEditors;

namespace FastReport.Data
{
  public class SqlCeDataConnection : DataConnectionBase
  {
    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new int CommandTimeout
    {
      get { return base.CommandTimeout; }
      set { base.CommandTimeout = value; }
    }
    
    /// <inheritdoc/>
    protected override string GetConnectionStringWithLoginInfo(string userName, string password)
    {
      return ConnectionString;
    }

    /// <inheritdoc/>
    public override string QuoteIdentifier(string value, DbConnection connection)
    {
      return "\"" + value + "\"";
    }

    /// <inheritdoc/>
    public override string[] GetTableNames()
    {
      List<string> list = new List<string>();

      SqlCeConnection connection = (SqlCeConnection)GetConnection();
      try
      {
        OpenConnection(connection);
        using (SqlCeCommand cc = connection.CreateCommand())
        {
          cc.CommandText = "select TABLE_NAME from INFORMATION_SCHEMA.TABLES";
          using (SqlCeDataReader drd = cc.ExecuteReader())
          {
            while (drd.Read())
            {
              list.Add(drd.GetString(0));
            }
          }
        }
      }
      finally
      {
        DisposeConnection(connection);
      }
      return list.ToArray();
    }

    /// <inheritdoc/>
    public override Type GetConnectionType()
    {
      return typeof(SqlCeConnection);
    }

    /// <inheritdoc/>
    public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection, FastReport.Data.CommandParameterCollection parameters)
    {
      SqlCeDataAdapter adapter = new SqlCeDataAdapter(selectCommand, connection as SqlCeConnection);
      foreach (CommandParameter p in parameters)
      {
        SqlCeParameter parameter = adapter.SelectCommand.Parameters.Add(p.Name, (SqlDbType)p.DataType, p.Size);
        parameter.Value = p.Value;
      }
      return adapter;
    }

    /// <inheritdoc/>
    public override ConnectionEditorBase GetEditor()
    {
      return new SqlCeConnectionEditor();
    }

    /// <inheritdoc/>
    public override Type GetParameterType()
    {
      return typeof(SqlDbType);
    }

    /// <inheritdoc/>
    public override int GetDefaultParameterType()
    {
      return (int)SqlDbType.VarChar;
    }

    /// <inheritdoc/>
    public override string GetConnectionId()
    {
      SqlCeConnectionStringBuilder builder = new SqlCeConnectionStringBuilder(ConnectionString);
      return "SQL CE: " + builder.DataSource;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlCeDataConnection"/> class with default settings.
    /// </summary>
    public SqlCeDataConnection()
    {
      CommandTimeout = 0;
    }
  }
}
