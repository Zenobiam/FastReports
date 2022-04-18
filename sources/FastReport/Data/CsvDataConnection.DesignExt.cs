using System.Data;
using FastReport.Data.ConnectionEditors;

namespace FastReport.Data
{
    partial class CsvDataConnection
    {
        #region Public Methods

        /// <inheritdoc/>
        public override void TestConnection()
        {
            using (DataSet dataset = CreateDataSet())
            {
            }
        }

        /// <inheritdoc/>
        public override ConnectionEditorBase GetEditor()
        {
            return new CsvConnectionEditor();
        }

        /// <inheritdoc/>
        public override string GetConnectionId()
        {
            return "Csv: " + CsvFile;
        }

        #endregion Public Methods
    }
}
