namespace FastReport.Export.Svg
{
    partial class SVGExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.SVGExportForm dialog = new FastReport.Forms.SVGExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}