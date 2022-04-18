namespace FastReport.Export.Json
{
    partial class JsonExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.JsonExportForm dialog = new FastReport.Forms.JsonExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}