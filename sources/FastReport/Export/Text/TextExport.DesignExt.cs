using FastReport.Forms;

namespace FastReport.Export.Text
{
    partial class TextExport
    {
        #region Public Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.TextExportForm dialog = new FastReport.Forms.TextExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void FinishInternal()
        {
            if (printAfterExport)
                TextExportPrint.PrintStream(
                    PrinterName,
                    Report.ReportInfo.Name.Length == 0 ? "FastReport .NET Text export" : Report.ReportInfo.Name,
                    Copies,
                    Stream);
        }

        private bool IsAborted(object progress)
        {
            Utils.Config.DoEvent();
            return progress is ProgressForm && (progress as ProgressForm).Aborted;
        }

        #endregion Private Methods
    }
}