using FastReport.Dialog;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FastReport.Web
{
    public partial class WebReport : WebControl, INamingContainer
    {
        private void TextBoxChange(TextBoxControl tb, string data)
        {
            tb.Text = data;
            tb.FilterData();
            tb.OnTextChanged(null);
        }

        private string GetValHook()
        {
            return
                "<script>$.valHooks.textarea = {" +
                "get: function(elem) {" +
                "return elem.value.replace(/\\r?\\n/g, \'\\r\\n\');" +
                "}};</script>";
        }

        private string GetTextBoxHtml(TextBoxControl control)
        {
            string id = Prop.ControlID + control.Name;
            if (control.Multiline)
            {
                return
                    GetValHook() +
                    string.Format("<textarea class=\"{0}\" style=\"{1}\" type=\"text\" name=\"{2}\" onchange=\"{4}\" id=\"{5}\">{3}</textarea>",
                    // class
                    "", //0
                    // style
                    GetTextBoxStyle(control), //1
                    // name
                    control.Name, //2
                    // value
                    control.Text, //3
                    // onclick
                    GetEvent("onchange", control, string.Format("$('#{0}').val()", id)), //4
                    //GetEvent("onchange", control, string.Format("document.getElementById('{0}').value", id)), //4
                    // title
                    id //5
                    );
            }
            else
            {
                return string.Format("<input class=\"{0}\" style=\"{1}\" type=\"text\" name=\"{2}\" value=\"{3}\" onchange=\"{4}\" id=\"{5}\"/>",
                    // class
                    "",
                    // style
                    GetTextBoxStyle(control),
                    // name
                    control.Name,
                    // value
                    control.Text,
                    // onclick
                    GetEvent("onchange", control, string.Format("document.getElementById('{0}').value", id)),
                    // title
                    id
                    );
            }
        }

        private string GetTextBoxStyle(TextBoxControl control)
        {
            return string.Format("{0}{1}{2}", GetControlPosition(control), GetControlFont(control), GetControlAlign(control));
        }
    }
}
