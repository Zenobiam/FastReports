using FastReport.Dialog;
using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FastReport.Web
{
    public partial class WebReport : WebControl, INamingContainer
    {
        private void CheckedListBoxChange(CheckedListBoxControl cb, string index)
        {
            int i = index.IndexOf("_");
            if (i != -1)
            {
                string item = index.Substring(0, i);
                string state = index.Substring(i + 1);
                int checkedIndex = -1;
                if (Int32.TryParse(item, out checkedIndex))
                {
                    cb.CheckedListBox.SetItemChecked(checkedIndex, state == "true");
                    ControlFilterRefresh(cb);
                    cb.OnSelectedIndexChanged(null);                    
                }
            }
        }

        private string GetCheckedListBoxHtml(CheckedListBoxControl control)
        {
            if (control.Items.Count == 0)
            {
                control.FillData();
                ControlFilterRefresh(control);
            }
            string id = Prop.ControlID + control.Name;
            string html = string.Format("<span class=\"{0}\" style=\"{1}\" name=\"{2}\" size=\"{3}\" id=\"{4}\">{5}</span>",
                // class
                "",
                // style
                GetCheckedListBoxStyle(control),
                // name
                control.Name,
                // size
                control.Items.Count.ToString(),
                // title
                id,
                GetCheckedListBoxItems(control)
                );
            control.FilterData();
            return html;
        }

        private string GetCheckedListBoxItems(CheckedListBoxControl control)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < control.Items.Count; i++)
            {
                string id = Prop.ControlID + control.Name + i.ToString();
                sb.Append(string.Format("<input {0} type=\"checkbox\" onchange=\"{1}\" id=\"{2}\" /> {3}<br />",
                    control.CheckedIndices.Contains(i) ? "checked" : "",
                    // onchange
                    GetEvent("onchange", control, i.ToString() + " + '_' + " + String.Format("document.getElementById('{0}').checked", id)),
                    id,
                    control.Items[i]
                    ));
            }
            return sb.ToString();
        }

        private string GetCheckedListBoxStyle(CheckedListBoxControl control)
        {
            return string.Format("overflow-y:scroll;{0}{1}", GetControlPosition(control), GetControlFont(control));
        }        

    }
}
