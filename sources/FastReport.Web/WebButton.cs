using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FastReport.Dialog;
using System.Windows.Forms;

namespace FastReport.Web
{
    public partial class WebReport : WebControl, INamingContainer
    {
        private void ButtonClick(ButtonControl button)
        {
            if (button.DialogResult == DialogResult.OK)
            {
                FormClose(button, CloseReason.None);
                Prop.CurrentForm++;
            }
            else if (button.DialogResult == DialogResult.Cancel)
            {
                FormClose(button, CloseReason.UserClosing);
                Prop.State = ReportState.Canceled;
            }
            button.OnClick(null);
        }

        private void FormClose(ButtonControl button, CloseReason reason)
        {
            DialogPage dialog = button.Report.Pages[Prop.CurrentForm] as DialogPage;
            dialog.Form.DialogResult = button.DialogResult;
            FormClosingEventArgs closingArgs = new FormClosingEventArgs(reason, false);
            dialog.OnFormClosing(closingArgs);
            FormClosedEventArgs closedArgs = new FormClosedEventArgs(reason);
            dialog.OnFormClosed(closedArgs);
            dialog.ActiveInWeb = false;
        }

        private string GetButtonHtml(ButtonControl control)
        {
            return string.Format("<input class=\"{0}\" style=\"{1}\" type=\"button\" name=\"{2}\" value=\"{3}\" onclick=\"{4}\" title=\"{5}\"/>",
                // class
                "",
                // style
                GetButtonStyle(control),
                // name
                control.Name,
                // value
                control.Text,
                // onclick
                GetEvent("onclick", control, ""),
                // title
                control.Text
                );
        }

        private string GetButtonStyle(ButtonControl control)
        {
            return string.Format("{0}{1}{2}padding:0;margin:0;", GetControlPosition(control), GetControlFont(control), GetControlAlign(control));
        }

    }
}
