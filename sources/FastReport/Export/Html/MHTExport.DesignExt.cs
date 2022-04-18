namespace FastReport.Export.Mht
{
    partial class MHTExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.MHTExportForm dialog = new FastReport.Forms.MHTExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}