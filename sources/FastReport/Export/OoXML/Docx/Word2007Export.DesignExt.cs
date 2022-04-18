namespace FastReport.Export.OoXML
{
    partial class Word2007Export
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.Word2007ExportForm dialog = new FastReport.Forms.Word2007ExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}