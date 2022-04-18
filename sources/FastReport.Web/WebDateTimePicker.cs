using FastReport.Dialog;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FastReport.Web
{
    public partial class WebReport : WebControl, INamingContainer
    {

        /// <summary>
        /// Gets or sets date time format in jqueryui datepicker style
        /// </summary>
        [DefaultValue(WebReportProperties.DEFAULT_DATE_TIME_PICKER_FORMAT)]
        [Category("Data")]
        [Browsable(true)]
        public string DateTimePickerFormat
        {
            get { return Prop.DateTimePickerFormat; }
            set { Prop.DateTimePickerFormat = value; }
        }

        

        private void DateTimePickerChange(DateTimePickerControl dp, string value)
        {
            dp.Value = DateTime.ParseExact(value, "d", CultureInfo.InvariantCulture); 
        }  

        private string GetDateTimePickerHtml(DateTimePickerControl control)
        {
            control.FillData();
            ControlFilterRefresh(control);
            string id = Prop.ControlID + control.Name;
            StringBuilder html = new StringBuilder();

            if(DateTimePickerFormat == null || DateTimePickerFormat == "" || DateTimePickerFormat == WebReportProperties.DEFAULT_DATE_TIME_PICKER_FORMAT)
            {
                string s = control.Value.Month.ToString() + "/" + control.Value.Day.ToString() + "/" + control.Value.Year.ToString();
                string ev = GetEvent("onchange", control, string.Format("document.getElementById('{0}').value", id));
                html.Append(String.Format("<input type=\"text\" value=\"{4}\" class=\"{0}\" style=\"{1}\" onchange=\"{2}\" id=\"{3}\"/>",
                    "",
                    GetDateTimePickerStyle(control),
                    ev,
                    id,
                    s
                    ));
                html.Append("<script>$(function() {$( \"#").Append(id).Append("\" ).datepicker();");
                html.Append("$( \"#").Append(id).Append("\" ).datepicker( \"option\", \"dateFormat\", \"").
                    Append(WebReportProperties.DEFAULT_DATE_TIME_PICKER_FORMAT).Append("\" );");
                html.Append("});</script>");
            }
            else
            {
                string value = "(function(){{ var tStr = function(k){{ if( k < 10) return '0' + k; return k; }}; var dateTime=$('#{0}').datepicker('getDate'); if(dateTime) return tStr(dateTime.getMonth() + 1 ) + '/' + tStr(dateTime.getDate()) + '/' + tStr(dateTime.getFullYear()); return '01/01/2019';}})()";
                string ev = GetEvent("onchange", control, string.Format(value, id));
                html.Append(String.Format("<input type=\"text\" class=\"{0}\" style=\"{1}\" onchange=\"{2}\" id=\"{3}\"/>",
                    "",
                    GetDateTimePickerStyle(control),
                    ev,
                    id
                    ));
                html.Append("<script>$(function() {$( \"#").Append(id).Append("\" ).datepicker();");
                html.Append("$( \"#").Append(id).Append("\" ).datepicker( \"option\", \"dateFormat\", \"").
                    Append(DateTimePickerFormat).Append("\" );");
                
                html.Append("$( \"#").Append(id).Append("\" ).datepicker( \"setDate\", new Date( ")
                    .Append(control.Value.Year).Append(",").Append(control.Value.Month - 1).Append(",").Append(control.Value.Day)
                    .Append("));");
                html.Append("});</script>");
            }

                       
            
            //control.FilterData();
            return html.ToString();
        }

        private string GetDateTimePickerStyle(DateTimePickerControl control)
        {
            return string.Format("{0}{1}", GetControlPosition(control), GetControlFont(control));
        }        

    }
}
