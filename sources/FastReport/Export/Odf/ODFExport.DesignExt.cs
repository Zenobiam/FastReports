namespace FastReport.Export.Odf
{
    partial class ODFExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.ODFExportForm dialog = new FastReport.Forms.ODFExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}