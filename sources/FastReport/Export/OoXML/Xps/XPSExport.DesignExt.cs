namespace FastReport.Export.OoXML
{
    partial class XPSExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.XpsExportForm dialog = new FastReport.Forms.XpsExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}