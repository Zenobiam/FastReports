using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using FastReport.Data.ConnectionEditors;
using Raven.Client;
using Raven.Client.Document;
using System.Net;
using System.ComponentModel;
using Raven.Json.Linq;
using Raven.Imports.Newtonsoft.Json.Linq;

namespace FastReport.Data
{
    /// <summary>
    /// Represents a connection to RavenDB database.
    /// </summary>
    public class RavenDBDataConnection : DataConnectionBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        [Category("Data")]
        public string DatabaseName
        {
            get
            {
                RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
                return builder.DatabaseName;
            }
            set
            {
                RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
                builder.DatabaseName = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the host url.
        /// </summary>
        [Category("Data")]
        public string Host
        {
            get
            {
                RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
                return builder.Host;
            }
            set
            {
                RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
                builder.Host = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Category("Data")]
        public string Username
        {
            get
            {
                RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
                return builder.Username;
            }
            set
            {
                RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
                builder.Username = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Category("Data")]
        public string Password
        {
            get
            {
                RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
                return builder.Password;
            }
            set
            {
                RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
                builder.Password = value;
                ConnectionString = builder.ToString();
            }
        }

        #endregion Properties

        #region Private Methods

        private Type GetSystemType(JTokenType type)
        {
            Type result = typeof(string);
            switch (type)
            {
                case JTokenType.Boolean:
                    result = typeof(bool);
                    break;
                case JTokenType.Date:
                    result = typeof(DateTime);
                    break;
                case JTokenType.Float:
                    result = typeof(double);
                    break;
                case JTokenType.Integer:
                    result = typeof(long);
                    break;
            }
            return result;
        }

        #endregion Private Methods

        #region Protected Methods

        /// <inheritdoc/>
        protected override DataSet CreateDataSet()
        {
            DataSet dataset = base.CreateDataSet();
            using (IDocumentStore store = new DocumentStore()
            { Url = Host, DefaultDatabase = DatabaseName, Credentials = new NetworkCredential(Username, Password) })
            {
                store.Initialize();
                IDocumentSession session = store.OpenSession();
                //store.ExecuteIndex(new RavenDocumentsByEntityName());
                List<string> entityNames = store.DatabaseCommands.GetTerms("Raven/DocumentsByEntityName", "Tag", "", 1024).ToList();
                foreach (string name in entityNames)
                {
                    DataTable table = new DataTable(name);
                    // get all rows of the table
                    RavenJObject[] objects = session.Advanced.LoadStartingWith<RavenJObject>(name, null, 0, Int32.MaxValue);
                    // create table columns
                    foreach (string key in objects[0].Keys)
                    {
                        RavenJToken value;
                        objects[0].TryGetValue(key, out value);
                        if (value is RavenJObject)
                        {
                            RavenJObject obj = value as RavenJObject;
                            foreach (string subKey in obj.Keys)
                            {
                                RavenJToken subValue;
                                obj.TryGetValue(subKey, out subValue);

                                DataColumn column = new DataColumn();
                                column.ColumnName = key + "." + subKey;
                                column.DataType = typeof(string);

                                // try to get column type from database
                                column.DataType = GetSystemType(subValue.Type);

                                table.Columns.Add(column);
                            }
                        }
                        else if (value is RavenJArray)
                        {
                            DataColumn column = new DataColumn();
                            column.ColumnName = key;
                            column.DataType = typeof(Array);

                            // try to get column type from database
                            //column.DataType = GetSystemType(value.Type);

                            table.Columns.Add(column);
                        }
                        else
                        {
                            DataColumn column = new DataColumn();
                            column.ColumnName = key;
                            column.DataType = typeof(string);

                            // try to get column type from database
                            column.DataType = GetSystemType(value.Type);

                            table.Columns.Add(column);
                        }
                    }
                    // add table rows
                    foreach (RavenJObject obj in objects)
                    {
                        DataRow row = table.NewRow();
                        // add row cells
                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            RavenJToken value;
                            if (table.Columns[i].ColumnName.Contains("."))
                            {
                                string[] parts = table.Columns[i].ColumnName.Split(".".ToCharArray());
                                obj.TryGetValue(parts[0], out value);
                                if (value is RavenJObject)
                                {
                                    RavenJToken subValue;
                                    (value as RavenJObject).TryGetValue(parts[1], out subValue);
                                    switch (subValue.Type)
                                    {
                                        case JTokenType.Boolean:
                                            bool b;
                                            bool.TryParse(subValue.ToString(), out b);
                                            row[i] = b;
                                            break;
                                        case JTokenType.Date:
                                            DateTime date;
                                            DateTime.TryParse(subValue.ToString(), out date);
                                            row[i] = date;
                                            break;
                                        case JTokenType.Float:
                                            double d;
                                            double.TryParse(subValue.ToString(), out d);
                                            row[i] = d;
                                            break;
                                        case JTokenType.Integer:
                                            long l;
                                            long.TryParse(subValue.ToString(), out l);
                                            row[i] = l;
                                            break;
                                        default:
                                            row[i] = subValue.ToString();
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                obj.TryGetValue(table.Columns[i].ColumnName, out value);
                                switch (value.Type)
                                {
                                    case JTokenType.Boolean:
                                        bool b;
                                        bool.TryParse(value.ToString(), out b);
                                        row[i] = b;
                                        break;
                                    case JTokenType.Date:
                                        DateTime date;
                                        DateTime.TryParse(value.ToString(), out date);
                                        row[i] = date;
                                        break;
                                    case JTokenType.Float:
                                        double d;
                                        double.TryParse(value.ToString(), out d);
                                        row[i] = d;
                                        break;
                                    case JTokenType.Integer:
                                        long l;
                                        long.TryParse(value.ToString(), out l);
                                        row[i] = l;
                                        break;
                                    case JTokenType.Array:
                                        RavenJArray array = value as RavenJArray;
                                        string[] values = new string[array.Length];
                                        for (int j = 0; j < array.Length; j++)
                                        {
                                            values[j] = array[j].ToString();
                                        }
                                        row[i] = values;
                                        break;
                                    default:
                                        row[i] = value.ToString();
                                        break;
                                }
                            }
                        }
                        table.Rows.Add(row);
                    }
                    dataset.Tables.Add(table);
                }
            }

            return dataset;
        }

        #endregion Protected Methods

        #region Public Methods

        /// <inheritdoc/>
        public override void CreateTable(TableDataSource source)
        {
            if (DataSet.Tables.Contains(source.TableName))
            {
                source.Table = DataSet.Tables[source.TableName];
                base.CreateTable(source);
            }
            else
            {
                source.Table = null;
            }
        }

        /// <inheritdoc/>
        public override string[] GetTableNames()
        {
            string[] result = new string[DataSet.Tables.Count];
            for (int i = 0; i < DataSet.Tables.Count; i++)
            {
                result[i] = DataSet.Tables[i].TableName;
            }
            return result;
        }

        /// <inheritdoc/>
        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return value;
        }

        /// <inheritdoc/>
        public override Type GetConnectionType()
        {
            return typeof(RavenDBDataConnection);
        }

        /// <inheritdoc/>
        public override string GetConnectionId()
        {
            RavenDBConnectionStringBuilder builder = new RavenDBConnectionStringBuilder(ConnectionString);
            return "RavenDB: " + builder.ToString();
        }

        /// <inheritdoc/>
        public override ConnectionEditorBase GetEditor()
        {
            return new RavenDBConnectionEditor();
        }

        /// <inheritdoc/>
        public override void TestConnection()
        {
            using (IDocumentStore store = new DocumentStore()
            { Url = Host, DefaultDatabase = DatabaseName, Credentials = new NetworkCredential(Username, Password) })
            {
                store.Initialize();
                IDocumentSession session = store.OpenSession();
            }
        }

        #endregion Public Methods
    }
}
