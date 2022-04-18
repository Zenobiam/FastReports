using FastReport.Data;
using FastReport.Data.ConnectionEditors;
using Sap.Data.SQLAnywhere;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace FastReport.SqlAnywhere
{
    public class SqlAnywhereDataConnection : DataConnectionBase
    {
        private void GetDBObjectNames(string name, List<string> list)
        {
            DataTable schema = null;
            string databaseName = "";
            DbConnection connection = GetConnection();
            try
            {
                OpenConnection(connection);
                SAConnectionStringBuilder builder = new SAConnectionStringBuilder(ConnectionString);
                schema = connection.GetSchema(name);
                databaseName = builder.DatabaseName;
            }
            finally
            {
                DisposeConnection(connection);
            }
            foreach (DataRow row in schema.Rows)
            {
                if (row["TABLE_TYPE"].ToString() == "BASE" || row["TABLE_TYPE"].ToString() == "VIEW" )
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
            SAConnectionStringBuilder builder = new SAConnectionStringBuilder(ConnectionString);

            builder.UserID = userName;
            builder.Password = password;

            return builder.ToString();
        }
        public override Type GetConnectionType()
        {
            return typeof(SAConnection);
        }
        public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection,
          CommandParameterCollection parameters)
        {
            SADataAdapter adapter = new SADataAdapter(selectCommand, connection as SAConnection);
            foreach (CommandParameter p in parameters)
            {
                SAParameter parameter = adapter.SelectCommand.Parameters.Add(p.Name, (SADbType)p.DataType, p.Size);
                parameter.Value = p.Value;
            }
            return adapter;
        }
        public override Type GetParameterType()
        {
            return typeof(SADbType);
        }
        public override string GetConnectionId()
        {
            SAConnectionStringBuilder builder = new SAConnectionStringBuilder(ConnectionString);
            string info = "";
            try
            {
                info = builder.DatabaseName;
            }
            catch
            {
            }
            return "SqlAnywhere: " + info;
        }
        public override ConnectionEditorBase GetEditor()
        {
            return new SqlAnywhereConnectionEditor();
        }
    }
}
