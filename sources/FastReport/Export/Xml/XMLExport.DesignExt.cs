namespace FastReport.Export.Xml
{
    partial class XMLExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.XMLExportForm dialog = new FastReport.Forms.XMLExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}