using FastReport.Dialog;
using System.Windows.Forms;

namespace FastReport.Engine
{
    public partial class ReportEngine
    {
        #region Private Methods

        private bool RunDialog(DialogPage page)
        {
            return page.ShowDialog() == DialogResult.OK;
        }

        private bool RunDialogs()
        {
            foreach (PageBase page in Report.Pages)
            {
                if (page is DialogPage)
                {
                    DialogPage dialogPage = page as DialogPage;
                    if (dialogPage.Visible && !RunDialog(dialogPage))
                        return false;
                }
            }
            return true;
        }

        #endregion Private Methods
    }
}
