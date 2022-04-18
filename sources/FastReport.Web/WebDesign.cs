using FastReport.Utils;
using System.Web.UI;

namespace FastReport.Web
{    
    public partial class WebReport
    {       
        private void RenderDesignModeNavigatorControls(HtmlTextWriter writer)
        {
            writer.WriteLine("<span id=\"" + ID + "\" style=\"display:inline-block;border-color:" + 
                this.BorderColor.ToString() + ";border-width:" + this.BorderWidth.Value.ToString() + "px;border-style:" + 
                this.BorderStyle.ToString() + ";height:" + this.Height.ToString() + "px;width:" + this.Width.ToString() + 
                "px;vertical-align:top;\">");

            writer.WriteLine("<div style=\"text-align:center; vertical-align: middle; " +
                "padding-left: " + Padding.Left.ToString() + "px; padding-right: " + Padding.Right.ToString() +
                "px; padding-top: " + Padding.Top.ToString() + "px; padding-bottom: " + Padding.Bottom + "px; " +
                "font-weight: bold; font-family: Tahoma; font-size: 22px; color: #CCCCCC;\">" +
                "FastReport .NET<br>ver." + Config.Version + "</div>");

            writer.WriteLine("</span>");
        }
    }
}