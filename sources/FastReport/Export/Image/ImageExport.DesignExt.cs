namespace FastReport.Export.Image
{
    partial class ImageExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.ImageExportForm dialog = new FastReport.Forms.ImageExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}