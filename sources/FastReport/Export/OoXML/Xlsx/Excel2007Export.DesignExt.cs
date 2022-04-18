namespace FastReport.Export.OoXML
{
    partial class Excel2007Export
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.Excel2007ExportForm dialog = new FastReport.Forms.Excel2007ExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}