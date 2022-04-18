using System;
using System.Drawing;
using System.Text;
using System.Web;
using FastReport.Dialog;
using System.Windows.Forms;

namespace FastReport.Web
{
    public partial class WebReport
    {

        private void ProcessDialogs(StringBuilder sb, HttpContext context)
        {
            if (Prop.Dialogs)
            {
                Report report = this.Report;
                while (Prop.CurrentForm < report.Pages.Count && !(report.Pages[Prop.CurrentForm] is DialogPage && report.Pages[Prop.CurrentForm].Visible == true))
                    Prop.CurrentForm++;

                if (Prop.CurrentForm < report.Pages.Count)
                {
                    Prop.State = ReportState.Forms;
                    DialogPage dialog = report.Pages[Prop.CurrentForm] as DialogPage;
                    if (!dialog.ActiveInWeb)
                    {
                        dialog.ActiveInWeb = true;
                        dialog.OnLoad(EventArgs.Empty);
                        dialog.OnShown(EventArgs.Empty);
                    }
                    GetDialogHtml(sb, dialog, context);
                }
                else
                    Prop.State = ReportState.Report;
            }
            else
                Prop.State = ReportState.Report;
        }

        internal void SetUpDialogs(HttpContext context)
        {
            string dialogN = context.Request.Params["dialog"];
            string controlName = context.Request.Params["control"];
            string eventName = context.Request.Params["event"];
            string data = context.Request.Params["data"];

            if (!string.IsNullOrEmpty(dialogN))
            {
                int dialogIndex = Convert.ToInt16(dialogN);
                if (dialogIndex >= 0 && dialogIndex < Report.Pages.Count)
                {
                    DialogPage dialog = Report.Pages[dialogIndex] as DialogPage;                    

                    DialogControl control = dialog.FindObject(controlName) as DialogControl;
                    if (control != null)
                    {
                        if (eventName == "onchange")
                        {
                            if (!string.IsNullOrEmpty(data))
                            {
                                if (control is TextBoxControl)
                                    TextBoxChange(control as TextBoxControl, data);
                                else if (control is ComboBoxControl)
                                    ComboBoxChange(control as ComboBoxControl, Convert.ToInt16(data));
                                else if (control is ListBoxControl)
                                    ListBoxChange(control as ListBoxControl, Convert.ToInt16(data));
                                else if (control is CheckedListBoxControl)
                                    CheckedListBoxChange(control as CheckedListBoxControl, data);
                                else if (control is DateTimePickerControl)
                                    DateTimePickerChange(control as DateTimePickerControl, data);
                                else if (control is MonthCalendarControl)
                                    MonthCalendarChange(control as MonthCalendarControl, data);
                            }
                        }
                        else if (eventName == "onclick")
                        {
                            if (control is ButtonControl)
                                ButtonClick(control as ButtonControl);
                            else if (control is CheckBoxControl)
                                CheckBoxClick(control as CheckBoxControl, data);
                            else if (control is RadioButtonControl)
                                RadioButtonClick(control as RadioButtonControl, data);
                        }
                    }
                }
            }
        }

        private void ControlFilterRefresh(DataFilterBaseControl control)
        {            
            control.FilterData();            
            if (control.DetailControl != null)
            {
                control.DetailControl.ResetFilter();
                control.DetailControl.FillData(control);
            }
        }

        private string GetDialogID()
        {
            return String.Concat(Prop.ControlID, "Dialog");
        }

        private void GetDialogHtml(StringBuilder sb, DialogPage dialog, HttpContext context)
        {
            string s = String.Format("<div style=\"min-width:{0}px! important;min-height:{1}px !important\">",
                dialog.Width.ToString(),
                dialog.Height.ToString()
                );
            sb.Append(s);

            
            sb.Append(String.Format("<div id=\"{0}\" style=\"position:relative;\" title=\"{1}\">", 
                    GetDialogID(), 
                    dialog.Text));
            GetComponentHtml(sb, dialog.Controls);
            sb.Append("</div></div>");
        }

        private void GetComponentHtml(StringBuilder sb, DialogComponentCollection collection)
        {
            foreach (DialogControl control in collection)
            {
                if (control.Visible)
                {
                    // button
                    if (control is ButtonControl)
                        sb.Append(GetButtonHtml(control as ButtonControl));
                    // label
                    else if (control is LabelControl)
                        sb.Append(GetLabelHtml(control as LabelControl));
                    // textbox
                    else if (control is TextBoxControl)
                        sb.Append(GetTextBoxHtml(control as TextBoxControl));
                    // checkbox
                    else if (control is CheckBoxControl)
                        sb.Append(GetCheckBoxHtml(control as CheckBoxControl));
                    // radio button
                    else if (control is RadioButtonControl)
                        sb.Append(GetRadioButtonHtml(control as RadioButtonControl));
                    // combo box
                    else if (control is ComboBoxControl)
                        sb.Append(GetComboBoxHtml(control as ComboBoxControl));
                    // list box
                    else if (control is ListBoxControl)
                        sb.Append(GetListBoxHtml(control as ListBoxControl));
                    // checked list box
                    else if (control is CheckedListBoxControl)
                        sb.Append(GetCheckedListBoxHtml(control as CheckedListBoxControl));
                    // datetime
                    else if (control is DateTimePickerControl)
                        sb.Append(GetDateTimePickerHtml(control as DateTimePickerControl));
                    // monthcalendar
                    else if (control is MonthCalendarControl)
                        sb.Append(GetMonthCalendarHtml(control as MonthCalendarControl));
                    // GroupBox
                    else if (control is GroupBoxControl)
                        sb.Append(GetGroupBoxHtml(control as GroupBoxControl));
                }
            }
        }


        private void GetDialogWindow(StringBuilder sb, DialogPage dialog, WebReport webReport)
        {
            sb.Append("<script>").
                Append("$(function() {").
                Append("$(\"#").Append(GetDialogID()).Append("\").dialog({").
                Append("width:").Append((int)dialog.Width + 5).Append(", ").
                Append("height:").Append((int)dialog.Height + 20).
                Append(" }); });").
                Append("</script>");
        }
        
        private string GetEvent(string eventName, DialogControl control, string data)
        {
            return string.Format("{0}frRequestServer('{1}?object={2}&dialog={3}&control={4}&event={5}&data=' + {6}){7}",
                "setTimeout(function(){",
                Prop.HandlerURL,
                Prop.ControlID,
                Prop.CurrentForm.ToString(),
                control.Name,
                eventName,
                string.IsNullOrEmpty(data) ? "''" : string.Format("{0}", data),
                "},250)"
                );
        }

        private string GetControlFont(DialogControl control)
        {
            return string.Format("font-size:{0}pt;font-weight:normal;display:inline-block;", control.Font.Size);
        }

        private string GetControlPosition(DialogControl control)
        {            
            return string.Format("position:absolute;left:{0}px;top:{1}px;width:{2}px;height:{3}px;padding:0px;margin:0px;", 
                control.Left, 
                control.Top, 
                control.Width, 
                control.Height);
        }

        private string GetControlAlign(DialogControl control)
        {
            if (control is LabelControl)
                return GetAlign((control as LabelControl).TextAlign);
            else if (control is ButtonControl)
                return GetAlign((control as ButtonControl).TextAlign);
            else if (control is TextBoxControl)
                return GetEditAlign((control as TextBoxControl).TextAlign);
            else
                return ""; 
        }

        private string GetEditAlign(HorizontalAlignment align)
        {
            if (align == HorizontalAlignment.Left)
                return "text-align:left;";
            else if (align == HorizontalAlignment.Center)
                return "text-align:center;";
            else if (align == HorizontalAlignment.Right)
                return "text-align:right;";
            else
                return "";
        }

        private string GetAlign(ContentAlignment align)
        {            
            if (align == ContentAlignment.TopLeft)
                return "text-align:left;vertical-align:top;";
            else if (align == ContentAlignment.TopCenter)
                return "text-align:center;vertical-align:top;";
            else if (align == ContentAlignment.TopRight)
                return "text-align:right;vertical-align:top;";
            else if (align == ContentAlignment.BottomLeft)
                return "text-align:left;vertical-align:bottom;";
            else if (align == ContentAlignment.BottomCenter)
                return "text-align:center;vertical-align:bottom;";
            else if (align == ContentAlignment.TopRight)
                return "text-align:right;vertical-align:bottom;";
            else if (align == ContentAlignment.MiddleLeft)
                return "text-align:left;vertical-align:middle;";
            else if (align == ContentAlignment.MiddleCenter)
                return "text-align:center;vertical-align:middle;";
            else if (align == ContentAlignment.MiddleRight)
                return "text-align:right;vertical-align:middle;";
            else
                return "";
        }
    }
}
