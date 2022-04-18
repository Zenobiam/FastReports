namespace FastReport.Export.Zpl
{
    partial class ZplExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.ZplExportForm dialog = new FastReport.Forms.ZplExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}