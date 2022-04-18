using System;
using System.ComponentModel;
using System.Data.Common;
using FastReport.Data.ConnectionEditors;
using System.Collections.Generic;
using FastReport.SharePoint;
using System.Data;
using FastReport.Design;
using System.IO;

namespace FastReport.Data
{
    /// <summary>
    /// Represents a connection to SharePointWebSite.
    /// </summary>
    /// <example>This example shows how to add a new connection to the report.
    /// <code>
    /// Report report = new Report();
    /// SharePointDataConnection conn = new SharePointDataConnection();
    /// conn.connectionstring = "site =
    /// conn.url = @"http://{url-address}/";
    /// conn.username = @"{User Name}";
    /// conn.password = @"{*********}";
    /// conn.listid = "02914986-c503-4be6-9e8b-19068628d774";
    /// report.Dictionary.Connections.Add(conn);
    /// conn.CreateAllTables();
    /// </code>
    /// </example>
    public class SharePointDataConnection : DataConnectionBase
    {
        SimpleRESTClient FRestClient;

        internal SimpleRESTClient RestClient
        {
            get
            {
                if (FRestClient == null)
                    FRestClient = new SimpleRESTClient();
                return FRestClient;
            }
        }
        /// <inheritdoc/>
        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return value;
            //throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override ConnectionEditorBase GetEditor()
        {
            return new SharePointConnectionEditor();
        }


        /// <inheritdoc/>
        protected override string GetConnectionStringWithLoginInfo(string userName, string password)
        {
            SharePointConnectionStringBuilder builder = new SharePointConnectionStringBuilder(ConnectionString);

            builder.UserName = userName;
            builder.Password = password;

            return builder.ToString();
        }

        /// <inheritdoc/>
        public override string[] GetTableNames()
        {
            //List<string> list = new List<string>();
            
            SharePointConnectionStringBuilder builder = new SharePointConnectionStringBuilder(ConnectionString);
            RestClient.check(builder.Url, builder.UserName, builder.Password, builder.Office365);
            return RestClient.getLists();
            //return list.ToArray();
        }

        /// <inheritdoc/>
        public override void TestConnection()
        {
            SharePointConnectionStringBuilder builder = new SharePointConnectionStringBuilder(ConnectionString);
            RestClient.check(builder.Url, builder.UserName, builder.Password, builder.Office365);
            RestClient.testConnection();

        }

        /// <inheritdoc/>
        public override void FillTableSchema(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            //string url = table.TableName.Substring(table.TableName.LastIndexOf(@"://") -4);
            SharePointConnectionStringBuilder builder = new SharePointConnectionStringBuilder(ConnectionString);
            RestClient.check(builder.Url, builder.UserName, builder.Password, builder.Office365);
            RestClient.fillFields(table);

        }

        /// <inheritdoc/>
        public override void FillTableData(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            SharePointConnectionStringBuilder builder = new SharePointConnectionStringBuilder(ConnectionString);
            RestClient.check(builder.Url, builder.UserName, builder.Password, builder.Office365);
            RestClient.fillTable(table);
        }

    }
}