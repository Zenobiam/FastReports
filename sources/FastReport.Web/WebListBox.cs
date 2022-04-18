using FastReport.Dialog;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FastReport.Web
{
    public partial class WebReport : WebControl, INamingContainer
    {
        private void ListBoxChange(ListBoxControl cb, int index)
        {
            cb.SelectedIndex = index;
            ControlFilterRefresh(cb);
            cb.OnSelectedIndexChanged(null);
        }

        private string GetListBoxHtml(ListBoxControl control)
        {
            if (control.Items.Count == 0)
            {
                control.FillData();
                ControlFilterRefresh(control);
            }
            string id = Prop.ControlID + control.Name;
            string html = string.Format("<select class=\"{0}\" style=\"{1}\" name=\"{2}\" size=\"{3}\" onchange=\"{4}\" id=\"{5}\">{6}</select>",
                // class
                "",
                // style
                GetListBoxStyle(control),
                // name
                control.Name,
                // size
                control.Items.Count.ToString(),
                // onclick
                GetEvent("onchange", control, string.Format("document.getElementById('{0}').selectedIndex", id)),
                // title
                id,
                GetListBoxItems(control)//control.Text
                );
            control.FilterData();
            return html;
        }

        private string GetListBoxItems(ListBoxControl control)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < control.Items.Count; i++)
            {
                sb.Append(string.Format("<option {0}>{1}</option>",
                    i == control.SelectedIndex ? "selected" : "",
                    control.Items[i]));
            }
            return sb.ToString();
        }

        private string GetListBoxStyle(ListBoxControl control)
        {
            return string.Format("{0}{1}", GetControlPosition(control), GetControlFont(control));
        }        

    }
}
