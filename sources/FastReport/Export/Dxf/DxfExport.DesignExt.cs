namespace FastReport.Export.Dxf
{
    public partial class DxfExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (Forms.DxfExportForm dialog = new Forms.DxfExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods
    }
}