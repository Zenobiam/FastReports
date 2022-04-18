using System.Windows.Forms;

namespace FastReport.Export.Email
{
    partial class EmailExport
    {
        #region Public Methods

        /// <summary>
        /// Displays the dialog box in which you can set up all parameters.
        /// </summary>
        /// <returns><b>true</b> if user pressed OK button in the dialog.</returns>
        public DialogResult ShowDialog()
        {
            using (EmailExportForm form = new EmailExportForm(this))
            {
                return form.ShowDialog();
            }
        }

        #endregion Public Methods
    }
}