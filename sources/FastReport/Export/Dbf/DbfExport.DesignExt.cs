namespace FastReport.Export.Dbf
{
    partial class DBFExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.DbfExportForm dialog = new FastReport.Forms.DbfExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}