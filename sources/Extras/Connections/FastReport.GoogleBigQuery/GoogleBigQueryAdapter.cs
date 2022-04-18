using System;
using System.Data;
using System.Collections.Generic;
using System.Data.Common;
using System.ComponentModel;
using System.Collections;

namespace GoogleBigQuery
{
    public class GbqConnection : DbConnection
    {
        private ConnectionState state = ConnectionState.Closed;
        private string serverVersion = String.Empty;
        private string dataSource = String.Empty;
        private string database = String.Empty;
        private string connectionString;

        [BrowsableAttribute(false)]
        public override ConnectionState State { get { return state; } }
        [BrowsableAttribute(false)]
        public override string ServerVersion { get { return serverVersion; } }
        public override string DataSource { get { return dataSource; } }
        public override string Database { get { return database;} }
        [SettingsBindableAttribute(true)]
        public override string ConnectionString 
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        public override void Open()
        {
            //
            // not implemented
        }

        public override void Close()
        {
            //
            // not implemented
        }

        protected override DbCommand CreateDbCommand()
        {            
            return new GbqCommand();
        }

        public override void ChangeDatabase(string databaseName)
        {
            //
            // not implemented
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            //
            // not implemented
            return null;
        }

    }

    public class GbqDataReader : DbDataReader
    {
        private int recordsAffected = 0;
        private bool isClosed = true;
        private bool hasRows = false;
        private int fieldCount = 10;
        private int depth = 0;

        public override int RecordsAffected 
        {
            get { return recordsAffected; }
        }

        public override bool IsClosed 
        {
            get { return isClosed; } 
        }

        public override bool HasRows 
        {
            get { return hasRows; }
        }

        public override int FieldCount 
        {
            get { return fieldCount; }
        }

        public override int Depth 
        {
            get { return depth; }
        }

        public override Object this[int ordinal] 
        {
            // not implemented
            get { return null; }
        }

        public override Object this[string name] 
        {
            // not implemented
            get { return null; }
        }

        public override bool Read()
        {
            // not implemented
            return true;
        }

        public override bool NextResult()
        {
            // not implemented
            return true;
        }

        public override bool IsDBNull(int ordinal)
        {
            // not implemented
            return true;
        }

        public override int GetValues(Object[] values)
        {
            // not implemented
            return 0;
        }

        public override Object GetValue(int ordinal)
        {
            // not implemented
            return null;
        }

        public override string GetString(int ordinal)
        {
            // not implemented
            return String.Empty;
        }

        public override long GetInt64(int ordinal)
        {
            // not implemented
            return 0;
        }

        public override int GetInt32(int ordinal)
        {
            // not implemented
            return 0;
        }

        public override short GetInt16(int ordinal)
        {
            // not implemented
            return 0;
        }

        public override Guid GetGuid(int ordinal)
        {
            // not implemented
            return Guid.NewGuid();
        }

        public override float GetFloat(int ordinal)
        {
            // not implemented
            return 0f;
        }

        public override double GetDouble(int ordinal)
        {
            // not implemented
            return 0f;
        }

        public override decimal GetDecimal(int ordinal)
        {
            // not implemented
            return 0;
        }

        public override DateTime GetDateTime(int ordinal)
        {
            // not implemented
            return DateTime.Now;
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            // not implemented
            return 0;
        }

        public override char GetChar(int ordinal)
        {
            // not implemented
            return ' ';
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            // not implemented
            return 0;
        }

        public override byte GetByte(int ordinal)
        {
            // not implemented
            return 0;
        }

        public override bool GetBoolean(int ordinal)
        {
            // not implemented
            return false;
        }

        public override int GetOrdinal(string name)
        {
            // not implemented
            return 0;
        }

        public override string GetName(int ordinal)
        {
            // not implemented
            return ordinal.ToString();
        }

        public override Type GetFieldType(int ordinal)
        {
            // not implemented
            return typeof(string);
        }

        public override IEnumerator GetEnumerator()
        {
            // not implemented
            return null;
        }

        public override string GetDataTypeName(int ordinal)
        {
            // not implemented
            return typeof(string).ToString();
        }

        public override DataTable GetSchemaTable()
        {
            // not implemented
            return new DataTable();
        }

        public override void Close()
        {
            // not implemented
        }

    }

    public class GbqCommand : DbCommand
    {
        private int commandTimeout;
        private UpdateRowSource updatedRowSource;
        private bool designTimeVisible;
        private DbTransaction dbTransaction;
        private DbParameterCollection dbParameterCollection = null;
        private GbqConnection dbConnection;
        private CommandType commandType;
        private string commandText;

        public override int CommandTimeout 
        {
            get { return commandTimeout; }
            set { commandTimeout = value; }
        }

        public override UpdateRowSource UpdatedRowSource 
        {
            get { return updatedRowSource; }
            set { updatedRowSource = value; } 
        }

        [BrowsableAttribute(false)]
        public override bool DesignTimeVisible 
        {
            get { return designTimeVisible; }
            set { designTimeVisible = value; } 
        }

        protected override DbTransaction DbTransaction 
        {
            get { return dbTransaction; }
            set { dbTransaction = value; }
        }

        protected override DbParameterCollection DbParameterCollection 
        {
            get { return dbParameterCollection; } 
        }

        protected override DbConnection DbConnection 
        {
            get { return dbConnection; }
            set { dbConnection = value as GbqConnection; }
        }

        public override CommandType CommandType 
        {
            get { return commandType; }
            set { commandType = value; } 
        }

        public override string CommandText 
        {
            get { return commandText; }
            set { commandText = value; } 
        }


        public override void Prepare()
        {
            //
            // not implemented
        }

        public override Object ExecuteScalar()
        {
            // not implemented
            return null;
        }

        public override int ExecuteNonQuery()
        {
            // not implemented
            return 0;
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            // not implemented
            return new GbqDataReader(); 
        }

        protected override DbParameter CreateDbParameter()
        {
            // not implemented
            return null;
        }

        public override void Cancel()
        {
            //
            // not implemented
        }

        public GbqCommand()
        {
            Connection = new GbqConnection();
        }
    }

    public class GbqDataAdapter : DbDataAdapter
    {
        private GbqCommand selectCommand = new GbqCommand();

        //[BrowsableAttribute(false)]
        //public override DbCommand SelectCommand
        //{
        //    get { return selectCommand; }
        //    set { selectCommand = value; }
        //}

        //public DataTable FillSchema(DataTable dataTable, SchemaType schemaType)
        //{
        //    return null;
        //}


        public GbqDataAdapter(string selectCommand, GbqConnection connection)
        {
            SelectCommand = new GbqCommand();
        }
    }

    public class GbqDbType
    {
        //
        // not implemented
    }

    public class GbqConnectionStringBuilder : DbConnectionStringBuilder
    {        
        #region Public Properties
        
        public string ProjectID
        {
            get { return this["ProjectID"] as string; }
            set { this["ProjectID"] = value; }
        }

        public string ClientID
        {
            get { return this["ClientID"] as string; }
            set { this["ClientID"] = value; }
        }

        public string ClientSecret
        {
            get { return this["ClientSecret"] as string; }
            set { this["ClientSecret"] = value; }
        }

        #endregion

        public GbqConnectionStringBuilder(string connectionString)
        {
            this.ConnectionString = connectionString;            
        }
    }
}