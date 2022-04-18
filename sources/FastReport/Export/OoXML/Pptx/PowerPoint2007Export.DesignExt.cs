namespace FastReport.Export.OoXML
{
    partial class PowerPoint2007Export
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.PowerPoint2007ExportForm dialog = new FastReport.Forms.PowerPoint2007ExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}