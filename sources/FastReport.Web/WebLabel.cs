using FastReport.Dialog;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FastReport.Web
{
    public partial class WebReport : WebControl, INamingContainer
    {
        private string GetLabelHtml(LabelControl control)
        {
            return string.Format("<div style=\"{0}\">{1}</div>",
                GetLabelStyle(control),
                control.Text
                );
        }

        private string GetLabelStyle(LabelControl control)
        {
            return string.Format("{0}{1}{2}", GetControlPosition(control), GetControlFont(control), GetControlAlign(control));
        }

    }
}
