using FastReport.Dialog;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FastReport.Web
{
    public partial class WebReport : WebControl, INamingContainer
    {
        private void ComboBoxChange(ComboBoxControl cb, int index)
        {
            cb.SelectedIndex = index;
            ControlFilterRefresh(cb);
            cb.OnSelectedIndexChanged(null);
        }

        private string GetComboBoxHtml(ComboBoxControl control)
        {
            if (control.Items.Count == 0)
            {
                control.FillData();
                ControlFilterRefresh(control);
            }
            string id = Prop.ControlID + control.Name;
            string html = string.Format("<select class=\"{0}\" style=\"{1}\" name=\"{2}\" onchange=\"{3}\" id=\"{4}\">{5}</select>",
                // class
                "",
                // style
                GetComboBoxStyle(control),
                // name
                control.Name,
                // onclick
                GetEvent("onchange", control, string.Format("document.getElementById('{0}').selectedIndex", id)),
                // title
                id,
                GetComboBoxItems(control)//control.Text
                );
            control.FilterData();
            return html;
        }

        private string GetComboBoxItems(ComboBoxControl control)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < control.Items.Count; i++)
            {
                sb.Append(string.Format("<option {0} value=\"{1}\">{2}</option>",
                    i == control.SelectedIndex ? "selected" : "",
                    control.Items[i],
                    control.Items[i]));
            }
            return sb.ToString();
        }

        private string GetComboBoxStyle(ComboBoxControl control)
        {
            return string.Format("{0}{1}", GetControlPosition(control), GetControlFont(control));
        }        

    }
}
