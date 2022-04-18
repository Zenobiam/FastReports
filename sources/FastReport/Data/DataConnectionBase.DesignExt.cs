using FastReport.Data.ConnectionEditors;
using FastReport.Forms;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Windows.Forms;

namespace FastReport.Data
{
    partial class DataConnectionBase : IHasEditor
    {
        #region Public Methods

        /// <inheritdoc/>
        public override void Delete()
        {
            Dispose();
        }

        /// <summary>
        /// Gets a string that will identify a connection in the Data Wizard.
        /// </summary>
        /// <returns>The string that contains the connection type and some meaningful information.</returns>
        public virtual string GetConnectionId()
        {
            return "";
        }

        /// <summary>
        /// Gets the default type for a new parameter.
        /// </summary>
        /// <returns>The integer representation of a parameter type.</returns>
        public virtual int GetDefaultParameterType()
        {
            return 0;
        }

        /// <summary>
        /// Gets a control that will be used to edit the connection properties.
        /// </summary>
        /// <returns>The editor's control.</returns>
        public virtual ConnectionEditorBase GetEditor()
        {
            return null;
        }

        /// <inheritdoc/>
        public bool InvokeEditor()
        {
            using (DataWizardForm form = new DataWizardForm(Report))
            {
                form.Connection = this;
                form.EditMode = true;
                return form.ShowDialog() == DialogResult.OK;
            }
        }

        /// <summary>
        /// Tests the connection.
        /// </summary>
        /// <remarks>
        /// If test connection is not successful, this method throws an exception. Catch this exception to
        /// show an error message.
        /// </remarks>
        public virtual void TestConnection()
        {
            DbConnection conn = GetConnection();
            if (conn != null)
            {
                try
                {
                    OpenConnection(conn);
                }
                finally
                {
                    DisposeConnection(conn);
                }
            }
        }

        #endregion Public Methods

        #region Internal Methods

        internal string GetQuotationChars()
        {
            DbConnection conn = GetConnection();
            try
            {
                OpenConnection(conn);
                return QuoteIdentifier("", conn);
            }
            finally
            {
                DisposeConnection(conn);
            }
        }

        #endregion Internal Methods

        #region Private Methods

        private void FilterTables(List<string> tableNames)
        {
            // filter tables
            for (int i = 0; i < tableNames.Count; i++)
            {
                Design.FilterConnectionTablesEventArgs e = new Design.FilterConnectionTablesEventArgs(this, tableNames[i]);
                Config.DesignerSettings.OnFilterConnectionTables(this, e);
                if (e.Skip)
                {
                    tableNames.RemoveAt(i);
                    i--;
                }
            }
        }

        private DbConnection GetDefaultConnection()
        {
            // if the ApplicationConnection is set, use it
            if (Config.DesignerSettings.ApplicationConnectionType == this.GetType())
                return Config.DesignerSettings.ApplicationConnection;
            return null;
        }

        private bool ShouldNotDispose(DbConnection connection)
        {
            // if this is the ApplicationConnection, do not dispose it
            return connection == Config.DesignerSettings.ApplicationConnection;
        }

        private void ShowLoginForm(string lastConnectionString)
        {
            if (String.IsNullOrEmpty(lastConnectionString))
            {
                using (AskLoginPasswordForm form = new AskLoginPasswordForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                        lastConnectionString = GetConnectionStringWithLoginInfo(form.Login, form.Password);
                }
            }
        }

        #endregion Private Methods
    }
}