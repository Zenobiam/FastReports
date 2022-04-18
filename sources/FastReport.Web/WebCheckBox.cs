using FastReport.Dialog;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FastReport.Web
{
    public partial class WebReport : WebControl, INamingContainer
    {
        private void CheckBoxClick(CheckBoxControl cb, string data)
        {
            bool oldValue = cb.Checked;
            cb.Checked = data == "true";
            cb.FilterData();
            cb.OnClick(null);
            if (oldValue != cb.Checked)
                cb.OnCheckedChanged(EventArgs.Empty);
        }

        private string GetCheckBoxHtml(CheckBoxControl control)
        {
            string id = Prop.ControlID + control.Name;
            return string.Format("<span class=\"{0}\" style=\"{1}\"><input style=\"vertical-align:middle;padding:0;margin:0 5px 0 0;\" type=\"checkbox\" name=\"{2}\" value=\"{3}\" onclick=\"{4}\" id=\"{5}\" {6}/><label style=\"{9}\" for=\"{7}\">{8}</label></span>",
                // class
                "",
                // style
                GetCheckBoxStyle(control),
                // name
                control.Name,
                // value
                control.Text,
                // onclick
                GetEvent("onclick", control, string.Format("document.getElementById('{0}').checked", id)),
                // title
                id,
                control.Checked ? "checked" : "",
                id,
                control.Text,
                GetControlFont(control)
                );
        }

        private string GetCheckBoxStyle(CheckBoxControl control)
        {
            return string.Format("{0}{1}", GetControlPosition(control), GetControlFont(control));
        }
    }
}
