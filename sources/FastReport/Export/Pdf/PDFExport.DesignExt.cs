namespace FastReport.Export.Pdf
{
    partial class PDFExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.PDFExportForm dialog = new FastReport.Forms.PDFExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}