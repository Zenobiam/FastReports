namespace FastReport.Export.Html
{
    partial class HTMLExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            if (!webMode)
            {
                using (FastReport.Forms.HTMLExportForm dialog = new FastReport.Forms.HTMLExportForm())
                {
                    return dialog.ShowDialog(this);
                }
            }
            else
                return true;
        }

        #endregion Public Methods
    }
}