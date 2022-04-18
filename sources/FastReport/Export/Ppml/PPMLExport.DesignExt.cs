namespace FastReport.Export.Ppml
{
    partial class PPMLExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.PPMLExportForm dialog = new FastReport.Forms.PPMLExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}