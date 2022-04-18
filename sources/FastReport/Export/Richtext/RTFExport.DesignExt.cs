namespace FastReport.Export.RichText
{
    partial class RTFExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.RTFExportForm dialog = new FastReport.Forms.RTFExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}