using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Globalization;
using FastReport.Data.ConnectionEditors;
using GoogleBigQuery;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util;
using Google.Apis.Bigquery.v2;
using Google.Apis.Services;
using Google.Apis.Bigquery.v2.Data;

namespace FastReport.Data
{
    /// <summary>
    /// Represents a connection to Google Big Query databases
    /// </summary>
    /// <example>This example shows how to add a new connection to the report.
    /// <code>
    /// Report report1;
    /// GoogleBigQueryDataConnection conn = new GoogleBigQueryDataConnection();
    /// conn.ConnectionString = @"ProjectID=your_project_id;ClientID=your_client_id;ClientSecret=your_secret";
    /// report1.Dictionary.Connections.Add(conn);
    /// conn.CreateAllTables();
    /// </code>
    /// </example>
    public class GoogleBigQueryDataConnection : DataConnectionBase
    {
        private BigqueryService Service = null;
        private string apiProjectID = String.Empty;

        private NumberFormatInfo numberFormat = new NumberFormatInfo();       

        public string ApiProjectID
        {
            get { return apiProjectID; }
            set { apiProjectID = value; }
        }

        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return "[" + value + "]";
        }

        public override Type GetParameterType()
        {
            return typeof(DbType);
        }

        public override string GetConnectionId()
        {            
            GbqConnectionStringBuilder builder = new GbqConnectionStringBuilder(ConnectionString);
            string info = "";
            try
            {
                info = builder.ProjectID;
            }
            catch
            {
            }
            return "GoogleBigQuery: " + info;
        }

        private string PrepareParameters(string selectCommand, CommandParameterCollection parameters)
        {
            if (parameters.Count > 0)
            {
                string[] parts = selectCommand.Split('?');
                if (parts.Length > 1)
                {
                    StringBuilder sql = new StringBuilder();
                    int paramIndex = 0;
                    sql.Append(parts[0]);
                    for (int i = 1; i < parts.Length; i++)
                    {
                        if(paramIndex < parameters.Count)
                           sql.Append(GetParamString(parameters[paramIndex++]));
                        sql.Append(parts[i]);
                    }
                    return sql.ToString();
                }
                else
                    return selectCommand;
            }
            else
                return selectCommand;
        }

        private string GetParamString(CommandParameter commandParameter)
        {
            string value = Convert.ToString(commandParameter.Value, numberFormat);
            if (commandParameter.Value.GetType().FullName == "System.String")
                value = "\"" + value + "\"";
            return value;
        }
        
        private void Init()
        {
            GbqConnectionStringBuilder builder = new GbqConnectionStringBuilder(ConnectionString);
            apiProjectID = builder.ProjectID;

            MemoryStream stream = new MemoryStream();

            string prep_secrets = "{ \"installed\": { \"client_id\": \"" + builder.ClientID +
                "\", \"client_secret\": \"" +
                builder.ClientSecret +
                "\" } }";

            byte[] bytes = Encoding.Default.GetBytes(prep_secrets);
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;

            UserCredential credential;
            GoogleWebAuthorizationBroker.Folder = "Bigquery.Auth.Store";
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                new[] { BigqueryService.Scope.Bigquery },
                "user", CancellationToken.None).Result;

            // Initialize the service
            Service = new BigqueryService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "FastReport"
            });

            numberFormat.NumberGroupSeparator = String.Empty;
            numberFormat.NumberDecimalSeparator = ".";
        }

        public override void FillTableSchema(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            Init();

            string queryText = PrepareParameters(selectCommand, parameters);

            if (queryText.ToLower().IndexOf("limit") == -1)
            {
                int i = queryText.LastIndexOf(';');
                if (i == -1)
                    i = queryText.Length;
                queryText = queryText.Substring(0, i) + " LIMIT 1;";
            }

            QueryRequest request = new QueryRequest();
            request.Query = queryText;
            request.UseQueryCache = true;

            QueryResponse queryResult = Service.Jobs.Query(request, apiProjectID).Execute();

            TableSchema schema = queryResult.Schema;
            foreach (TableFieldSchema field in schema.Fields)
            {
                DataColumn dataColumn = new DataColumn(field.Name);
                table.Columns.Add(dataColumn);
                dataColumn.AllowDBNull = (field.Mode == "NULLABLE");

                if (field.Type == "INTEGER")
                {
                    dataColumn.DataType = System.Type.GetType("System.Int32");
                }
                else if (field.Type == "FLOAT")
                {
                    dataColumn.DataType = System.Type.GetType("System.Double");
                }
                else if (field.Type == "BOOLEAN")
                {
                    dataColumn.DataType = System.Type.GetType("System.Boolean");
                }
                else
                {
                    dataColumn.DataType = System.Type.GetType("System.String");
                }
            }
        }

        public override void FillTableData(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            string queryText = PrepareParameters(selectCommand, parameters);

            QueryRequest request = new QueryRequest();
            request.Query = queryText;
            request.UseQueryCache = true;

            QueryResponse queryResult = Service.Jobs.Query(request, apiProjectID).Execute();

            if (queryResult.Rows != null)
            {
                foreach (TableRow row in queryResult.Rows)
                {
                    DataRow dataRow = table.NewRow();

                    table.Rows.Add(dataRow);
                    for (int i = 0; i < row.F.Count; i++)
                    {
                        if (table.Columns[i].DataType.FullName == "System.String")
                        {
                            dataRow[i] = row.F[i].V;
                        }
                        else if (table.Columns[i].DataType.FullName == "System.Int32")
                        {
                            dataRow[i] = Convert.ToInt32(row.F[i].V, numberFormat);
                        }
                        else if (table.Columns[i].DataType.FullName == "System.Double")
                        {
                            dataRow[i] = Convert.ToDouble(row.F[i].V, numberFormat);
                        }
                        else if (table.Columns[i].DataType.FullName == "System.Boolean")
                        {
                            dataRow[i] = Convert.ToBoolean(row.F[i].V);
                        }
                    }
                }
            }
        }

        public override ConnectionEditorBase GetEditor()
        {
            return new GoogleBigQueryConnectionEditor();
        }
    }
}