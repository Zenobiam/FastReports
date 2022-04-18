using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Alachisoft.NosDB.Common;
using Alachisoft.NosDB.ADO.NETProvider;
using FastReport.Data.ConnectionEditors;
using Alachisoft.NosDB.Client;
using Alachisoft.NosDB.Common.Server.Engine;

namespace FastReport.Data
{ 
    
    public class NosDbDataConnection : DataConnectionBase
    {
        #region Private Methods

        private new DbConnection GetConnection()
        {
            return new NosDbConnection(ConnectionString);
        }

        private string PrepareSelectCommand(string selectCommand, string tableName, DbConnection connection)
        {
            if (String.IsNullOrEmpty(selectCommand))
            {
                selectCommand = "select * from " + QuoteIdentifier(tableName, connection);
            }
            return selectCommand;
        }
        #endregion

        #region Protected Methods
        protected override string GetConnectionStringWithLoginInfo(string userName, string password)
        {
            ConnectionStringBuilder builder = new ConnectionStringBuilder(ConnectionString);

            builder.UserId = userName;
            builder.Password = password;

            return builder.ToString();
        }
        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public override void FillTableSchema(DataTable table, string selectCommand,
      CommandParameterCollection parameters)
        {
            DbConnection conn = GetConnection();
            try
            {
                OpenConnection(conn);
                // prepare select command
                selectCommand = PrepareSelectCommand(selectCommand, table.TableName, conn);
                // read the table schema
                using (DbDataAdapter adapter = GetAdapter(selectCommand, conn, parameters))
                {
                    adapter.SelectCommand.CommandTimeout = CommandTimeout;
                    Database db = NosDB.GetDatabase(ConnectionString);
                    IDBCollectionReader reader = db.ExecuteReader(adapter.SelectCommand.CommandText);
                    reader.ReadNext();
                    IJSONDocument doc = reader.GetDocument();
                    foreach (var d in doc)
                    {
                        Type tp = (d.Value == null || d.Value == "") ? typeof(string) : d.Value.GetType();
                        table.Columns.Add(new DataColumn(d.Key, tp));
                    }                        
                }
            }
            finally
            {
                DisposeConnection(conn);
            }
        }

        /// <inheritdoc/>
        public override void FillTableData(DataTable table, string selectCommand,
     CommandParameterCollection parameters)
        {
            DbConnection conn = GetConnection();
            try
            {
                OpenConnection(conn);
                // prepare select command
                selectCommand = PrepareSelectCommand(selectCommand, table.TableName, conn);
                // read the table
                using (DbDataAdapter adapter = GetAdapter(selectCommand, conn, parameters))
                {
                    adapter.SelectCommand.CommandTimeout = CommandTimeout;
                    table.Clear();
                    Database db = NosDB.GetDatabase(ConnectionString);
                    IDBCollectionReader reader = db.ExecuteReader(adapter.SelectCommand.CommandText);
                    while (reader.ReadNext())
                    {
                        IJSONDocument doc = reader.GetDocument();
                        List<object> ctnt = new List<object>();
                        foreach (var d in doc)
                            ctnt.Add(d.Value);
                        DataRow row = table.NewRow();
                        row.ItemArray = ctnt.ToArray();
                        table.Rows.Add(row);
                    }
                }
            }
            finally
            {
                DisposeConnection(conn);
            }
        }     

        public override string[] GetTableNames()
        {
            return NosDbConnectionEditor.CollectionNames;
        }

        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return "\"" + value + "\"";
        }

        public override Type GetConnectionType()
        {
            return typeof(NosDbConnection);
        }

        /// <inheritdoc/>
        public override void TestConnection()
        {
            DbConnection connection = GetConnection();
            connection.Open();
            connection.Close();
            connection.Dispose();
        }

        public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection,
          CommandParameterCollection parameters)
        {
            NosDataAdapter adapter = new NosDataAdapter(selectCommand, connection as NosDbConnection);
            foreach (CommandParameter p in parameters)
            {
                NosDataParameter param = new NosDataParameter();
                param.ParameterName = p.Name;
                param.DbType = (DbType)p.DataType;
                param.Size = p.Size;
                param.Value = p.Value;
                adapter.SelectCommand.Parameters.Add(param);
            }
            return adapter;
        }

        public override Type GetParameterType()
        {
            return typeof(DbType);
        }

        public override string GetConnectionId()
        {
            ConnectionStringBuilder builder = new ConnectionStringBuilder(ConnectionString);
            string info = "";
            try
            {
                info = builder.Database;
            }
            catch
            {
            }
            return "NosDb: " + info;
        }

        public override ConnectionEditorBase GetEditor()
        {
            return new NosDbConnectionEditor();
        }
        #endregion
    }
}
