namespace FastReport.Export.BIFF8
{
    partial class Excel2003Document
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.Excel2003ExportForm dialog = new FastReport.Forms.Excel2003ExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}