using FastReport.Dialog;
using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FastReport.Web
{
    public partial class WebReport : WebControl, INamingContainer
    {

        private void MonthCalendarChange(MonthCalendarControl dp, string value)
        {
            dp.SelectionStart  = DateTime.ParseExact(value, "d", CultureInfo.InvariantCulture); 
        }

        private string GetMonthCalendarHtml(MonthCalendarControl control)
        {
            control.FillData();
            ControlFilterRefresh(control);
            string id = Prop.ControlID + control.Name;
            StringBuilder html = new StringBuilder();
            string selectedDate = control.SelectionStart.Month.ToString() + "/" + control.SelectionStart.Day.ToString() + "/" + control.SelectionStart.Year.ToString();
            string ev = GetEvent("onchange", control, string.Format("document.getElementById('{0}').value", id));
            html.Append(String.Format("<div class=\"{0}\" style=\"{1}\" onchange=\"{2}\" id=\"{3}\"></div>",
                "",
                GetMonthCalendarStyle(control),
                ev,
                id                
                ));
            html.Append("<script>$(function() {$( \"#").Append(id).AppendLine("\" ).datepicker();");                
            html.Append("$( \"#").Append(id).Append("\" ).datepicker( \"option\", \"dateFormat\", \"").
                Append(WebReportProperties.DEFAULT_DATE_TIME_PICKER_FORMAT).AppendLine("\" );");
            html.Append("$( \"#").Append(id).Append(String.Format("\" ).datepicker( \"setDate\", \"{0}\", \"", selectedDate)).
                Append(WebReportProperties.DEFAULT_DATE_TIME_PICKER_FORMAT).AppendLine("\" );");
            
            html.Append("});</script>");            
            
            //control.FilterData();
            return html.ToString();
        }

        private string GetMonthCalendarStyle(MonthCalendarControl control)
        {
            return string.Format("{0}{1}", GetControlPosition(control), GetControlFont(control));
        }        

    }
}
