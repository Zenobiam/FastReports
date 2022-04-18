namespace FastReport.Export.Hpgl
{
    partial class HpglExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.HpglExportForm dialog = new FastReport.Forms.HpglExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}