namespace FastReport.Export.Csv
{
    partial class CSVExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.CsvExportForm dialog = new FastReport.Forms.CsvExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}