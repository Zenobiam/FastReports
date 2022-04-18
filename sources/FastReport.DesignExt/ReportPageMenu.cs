using System;
using FastReport.Design;

namespace FastReport
{
    internal class ReportPageMenu : ComponentBaseMenu
    {
        #region Private Methods

        private void miCopy_Click(object sender, EventArgs e)
        {
            Designer.cmdCopyPage.Invoke();
        }

        private void miDelete_Click(object sender, EventArgs e)
        {
            Designer.cmdDeletePage.Invoke();
        }

        #endregion // Private Methods

        #region Constructors

        public ReportPageMenu(Designer designer) : base(designer)
        {
            miEdit.Visible = Designer.cmdEdit.Enabled;
            miCut.Visible = false;
            miPaste.Visible = false;
            miBringToFront.Visible = false;
            miSendToBack.Visible = false;
            
            miCopy.Enabled = Designer.cmdCopyPage.Enabled;
            miDelete.Enabled = Designer.cmdDeletePage.Enabled;

            miCopy.Click += miCopy_Click;
            miDelete.Click += miDelete_Click;
        }

        #endregion // Constructors
    }
}
