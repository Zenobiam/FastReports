namespace FastReport.Export.PS
{
    partial class PSExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.PSExportForm dialog = new FastReport.Forms.PSExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}