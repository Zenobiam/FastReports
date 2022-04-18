namespace FastReport.Export.LaTeX
{
    partial class LaTeXExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.LaTeXExportForm dialog = new FastReport.Forms.LaTeXExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}