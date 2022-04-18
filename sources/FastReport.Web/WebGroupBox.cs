using FastReport.Dialog;
using System.Text;

namespace FastReport.Web
{
    public partial class WebReport
    {
        private string GetGroupBoxHtml(GroupBoxControl groupBox)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(string.Format("<div style=\"{0}\">", GetControlPosition(groupBox)));

            sb.Append(string.Format("<div id=\"{0}\" style=\"position:relative;\">", groupBox.Name));

            GetComponentHtml(sb, groupBox.Controls);
            sb.Append("</div></div>");
            return sb.ToString();
        }

    }
}
