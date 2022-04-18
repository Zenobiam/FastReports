using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using FastReport.Data.ConnectionEditors;
using System.Data;
using System.Data.SQLite;

namespace FastReport.Data
{
    public class SQLiteDataConnection : DataConnectionBase
    {
        private void GetDBObjectNames(string name, List<string> list)
        {
            DataTable schema = null;
            DbConnection connection = GetConnection();
            try
            {
                OpenConnection(connection);
                schema = connection.GetSchema(name);
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
            GetDBObjectNames("Views", list);
            return list.ToArray();
        }

        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return "`" + value + "`";
        }

        protected override string GetConnectionStringWithLoginInfo(string userName, string password)
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder(ConnectionString);
            builder.Password = password;
            return builder.ToString();
        }

        public override Type GetConnectionType()
        {
            return typeof(SQLiteConnection);
        }

        public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection,
            CommandParameterCollection parameters)
        {
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(selectCommand, connection as SQLiteConnection);
            foreach (CommandParameter p in parameters)
            {
                SQLiteParameter parameter = adapter.SelectCommand.Parameters.Add(p.Name, (DbType)p.DataType, p.Size);
                parameter.Value = p.Value;
            }
            return adapter;
        }

        public override Type GetParameterType()
        {
            return typeof(DbType);
        }

        public override string GetConnectionId()
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder(ConnectionString);
            string info = "";
            try
            {
                info = builder.DataSource ?? String.Empty;
            }
            catch
            {
            }
            return "SQLite: " + info;
        }

        public override ConnectionEditorBase GetEditor()
        {
            return new SQLiteConnectionEditor();
        }

    }
}
