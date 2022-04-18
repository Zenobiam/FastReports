using FastReport.Data.ConnectionEditors;
using FastReport.Utils;
using System;
using System.Windows.Forms;

namespace FastReport.Data.JsonConnection
{
    partial class JsonDataSourceConnection
    {
        #region Private Fields

        public JsonDataSourceConnectionEditor Editor { get; set; }

        #endregion Private Fields

        #region Public Methods

        /// <inheritdoc/>
        public override string GetConnectionId()
        {
            JsonDataSourceConnectionStringBuilder builder = new JsonDataSourceConnectionStringBuilder(ConnectionString);
            string[] json = builder.Json.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (json.Length > 1)
                return "JSON: " + json[1];
            else return "JSON: ";
        }

        /// <inheritdoc/>
        public override ConnectionEditorBase GetEditor()
        {
            return Editor = new JsonDataSourceConnectionEditor();
        }

        /// <inheritdoc/>
        public override void TestConnection()
        {
            jsonInternal = null;
            InitConnection();
            if (!String.IsNullOrEmpty(jsonSchemaString) && Editor != null && String.IsNullOrEmpty(Editor.JsonSchema))
            {
                Editor.JsonSchema = jsonSchemaString;
                Editor.IsJsonChanged = false;
            }
            else if (Editor.IsJsonChanged)
            {
                string askText = Res.Get("ConnectionEditors,Json,ConfirmChanges");
                DialogResult askResult = FRMessageBox.Confirm(askText, MessageBoxButtons.YesNo);
                if (askResult == DialogResult.Yes)
                {
                    jsonInternal = null;
                    jsonSchema = null;
                    jsonSchemaString = "";
                    InitConnection(true);
                    Editor.JsonSchema = jsonSchemaString;
                    Editor.IsJsonChanged = false;
                }
            }
        }

        #endregion Public Methods
    }
}