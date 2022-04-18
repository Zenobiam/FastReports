using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FastReport.Web
{

    #region Toolbar enums
    /// <summary>
    /// Describes sizes of Toolbar enum 
    /// </summary>
    public enum ToolbarStyle
    {
        /// <summary>
        /// Small size toolbar
        /// </summary>
        Small,
        /// <summary>
        /// Big size toolbar
        /// </summary>
        Large
    }

    /// <summary>
    /// Toolbar Icons Styles
    /// </summary>
    public enum ToolbarIconsStyle
    {
        /// <summary>
        /// Red Icons
        /// </summary>
        Red,
        /// <summary>
        /// Green Icons
        /// </summary>
        Green,
        /// <summary>
        /// Blue Icons
        /// </summary>
        Blue,
        /// <summary>
        /// Black Icons
        /// </summary>
        Black,
        /// <summary>
        /// Custom Icons
        /// </summary>
        Custom
    }

    /// <summary>
    /// Toolbar Background Styles.
    /// </summary>
    public enum ToolbarBackgroundStyle
    {
        /// <summary>
        /// Transparent background.
        /// </summary>
        None,
        /// <summary>
        /// Light background.
        /// </summary>
        Light,
        /// <summary>
        /// Medium dark background.
        /// </summary>
        Medium,
        /// <summary>
        /// Dark background.
        /// </summary>
        Dark,
        /// <summary>
        /// Custom background.
        /// </summary>
        Custom
    }
    #endregion

    /// <summary>
    /// Web Toolbar
    /// </summary>
    internal class WebToolbar
    {
        #region Private variables
        private Page page;
        private bool enableFit = true;
        private List<ReportTab> fTabs;
        private int currentTab = 0;
        private WebRes res;
        private string backgroundColor = "white";
        #endregion 

        #region Public properties

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public string BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }

        /// <summary>
        /// Gets or sets current tab index
        /// </summary>
        public int CurrentTabIndex
        {
            get
            {
                return currentTab;
            }
            set
            {
                currentTab = value;
            }
        }

        /// <summary>
        /// Enable or disable fitting
        /// </summary>
        public bool EnableFit
        {
            get { return enableFit; }
            set { enableFit = value; }
        }

        /// <summary>
        /// Report properties 
        /// </summary>
        public WebReportProperties ReportProperties
        {
            get { return fTabs[currentTab].Properties; }
            set { fTabs[currentTab].Properties = value; }
        }

        /// <summary>
        /// Current report 
        /// </summary>
        public Report Report
        {
            get { return fTabs[currentTab].Report; }
            set { fTabs[currentTab].Report = value; }
        }
        #endregion Public properties

        #region Private methods
        private string GetReportId()
        {
            return string.Format("<input type=\"hidden\" name=\"object\" value=\"{0}\"/>", ReportProperties.ControlID);
        }

        private string GetNavigation()
        {
            StringBuilder sb = new StringBuilder();
            if (ReportProperties.TotalPages > 1 && !ReportProperties.SinglePage)
            {
                res.Root("Web");
                sb.Append("<div class=\"td divider\">&nbsp;</div>");
                if (ReportProperties.ShowFirstButton)
                    sb.Append(string.Format("<input class=\"td nav first_button\" type=\"button\" name=\"first\" value=\"\" {0} title=\"{1}\" onclick=\"{2}\"/>",
                        (ReportProperties.CurrentPage == 0) ? "disabled=\"disabled\"" : "",
                        res.Get("First"),
                        GetNavRequest("first", "1")));

                if (ReportProperties.ShowPrevButton)
                    sb.Append(string.Format("<input class=\"td nav prev_button\" type=\"button\" name=\"prev\" value=\"\" {0} title=\"{1}\" onclick=\"{2}\"/>",
                        (ReportProperties.CurrentPage == 0) ? "disabled=\"disabled\"" : "",
                        res.Get("Prev"),
                        GetNavRequest("prev", "1")));

                sb.Append(GetPageNumbers());

                if (ReportProperties.ShowNextButton)
                    sb.Append(string.Format("<input class=\"td nav next_button\" type=\"button\" name=\"next\" value=\"\" {0} title=\"{1}\" onclick=\"{2}\"/>",
                         (ReportProperties.CurrentPage < ReportProperties.TotalPages - 1) ? "" : "disabled=\"disabled\"",
                         res.Get("Next"),
                         GetNavRequest("next", "1")));

                if (ReportProperties.ShowLastButton)
                {
                    sb.Append(string.Format("<input class=\"td nav last_button\" type=\"button\" name=\"last\" value=\"\" {0} title=\"{1}\" onclick=\"{2}\"/>",
                         (ReportProperties.CurrentPage < ReportProperties.TotalPages - 1) ? "" : "disabled=\"disabled\"",
                         res.Get("Last"),
                         GetNavRequest("last", "1")));
                }
            }
            sb.Append("<div class=\"td divider\">&nbsp;</div>");
            return sb.ToString();
        }

        private string GetPageNumbers()
        {
            StringBuilder sb = new StringBuilder();
            if (ReportProperties.ShowPageNumber)
            {
                res.Root("Web");
                string id = ReportProperties.ControlID + "PageN";
                string s = string.Format("<input class=\"td input center\" type=\"text\" name=\"page\" value=\"{0}\" size=\"4\" onchange=\"{1}\" title=\"{3}\" id=\"{2}\"/>",
                    (ReportProperties.TotalPages > 0 ? ReportProperties.CurrentPage + 1 : 0).ToString(),
                    GetNavRequest("goto", "' + document.getElementById('" + id + "').value + '"), id, res.Get("EnterPage"));
                sb.Append(s);
                sb.Append("<div class=\"td delim\">/</div>");
                sb.Append(string.Format("<input class=\"td input center\" type=\"text\" value=\"{0}\" size=\"4\" readonly=\"readonly\" title=\"{1}\"/>", ReportProperties.TotalPages.ToString(), res.Get("TotalPages")));
            }
            return sb.ToString();
        }

        private string GetRefresh()
        {
            StringBuilder sb = new StringBuilder();
            if (ReportProperties.ShowRefreshButton)
            {
                res.Root("Web");
                sb.Append("<div class=\"td divider\">&nbsp;</div>");
                sb.Append(string.Format("<input class=\"td nav refresh_button\" type=\"button\" name=\"refresh\" value=\"\" title=\"{0}\" onclick=\"{1}\"/>",
                    res.Get("Refresh"), GetNavRequest("refresh", "1")));
            }
            return sb.ToString();
        }

        private string GetBackButton()
        {
            StringBuilder sb = new StringBuilder();
            if (ReportProperties.ShowBackButton && currentTab > 0)
            {
                res.Root("Web");
                sb.Append("<div class=\"td divider\">&nbsp;</div>");
                sb.Append(string.Format("<input class=\"td nav back_button\" type=\"button\" name=\"back\" value=\"\" title=\"{0}\" onclick=\"{1}\"/>",
                    res.Get("Back"), GetNavRequest("settab", ReportProperties.PreviousTab.ToString())));                
            }
            return sb.ToString();
        }

        private string GetZoomMenu()
        {
            StringBuilder sb = new StringBuilder();
            if (ReportProperties.ShowZoomButton)
            {
                res.Root("Web");
                sb.Append("<div class=\"td divider\">&nbsp;</div>");
      	        sb.Append("<ul class=\"td nav\">");
        	    sb.Append("<li><input class=\"nav zoom_button\" type=\"button\" value=\"\"/>");
                sb.Append("<ul class=\"round checkboxes\">");
                if (enableFit)
                {
                    sb.Append(GetZoomItem("zoom_width", res.Get("FitWidth")));
                    sb.Append(GetZoomItem("zoom_page", res.Get("FitPage")));
                }
                sb.Append(GetZoomItem("zoom_300", "300%"));
                sb.Append(GetZoomItem("zoom_200", "200%"));
                sb.Append(GetZoomItem("zoom_150", "150%"));
                sb.Append(GetZoomItem("zoom_100", "100%"));
                sb.Append(GetZoomItem("zoom_90", "90%"));
                sb.Append(GetZoomItem("zoom_75", "75%"));
                sb.Append(GetZoomItem("zoom_50", "50%"));
                sb.Append(GetZoomItem("zoom_25", "25%"));
                sb.Append("</ul></li></ul>");
            }
            return sb.ToString();
        }

        private string GetZoomItem(string actionName, string caption)
        {
            return string.Format("<li class=\"radioitem\"><input class=\"menutext padleft {0}\" type=\"button\" name=\"{1}\" value=\"{2}\" onclick=\"{3}\"/></li>",
                 CheckCurrentZoom(actionName) ? "checked" : "", actionName, caption,
                    GetNavRequest(actionName, "1"));
        }

        private bool CheckCurrentZoom(string name)
        {
            string s = name.Substring(5);
            if (s == "width" && ReportProperties.ZoomMode == ZoomMode.Width)
                return true;
            else if (s == "page" && ReportProperties.ZoomMode == ZoomMode.Page)
                return true;
            else if (ReportProperties.ZoomMode == ZoomMode.Scale && Math.Round(ReportProperties.Zoom * 100).ToString() == s)
                return true;
            else
                return false;
        }

        private string GetTabs()
        {
            StringBuilder sb = new StringBuilder();
            if (fTabs.Count > 1)
            {
                for (int i = 0; i < fTabs.Count; i++)
                {
                    sb.Append("<div class=\"tabselector\">");
                    sb.Append(string.Format("<input class=\"td tab {2}\" type=\"button\" name=\"tab1\" value=\"{0}\" title=\"{3}\" onclick=\"{1}\"/>",
                        GetTabName(i), GetNavRequest("settab", i.ToString()), i == CurrentTabIndex ? "tabselected":"", fTabs[i].Name));
                    if (ReportProperties.ShowTabCloseButton)
                    {
                        sb.Append(string.Format("<input class=\"td tabclose {2}\" type=\"button\" name=\"tab1c\" value=\"{0}\" title=\"{3}\" onclick=\"{1}\"/>",
                            "X", GetNavRequest("closetab", i.ToString()), i == CurrentTabIndex ? "tabselected" : "", "X"));
                    }
                    sb.Append("</div>");
                }
                if (ReportProperties.TabPosition == TabPosition.UnderToolbar)
                    sb.Append("<br /><br />");
            }
            return sb.ToString();
        }

        private string GetTabName(int i)
        {
            if (String.IsNullOrEmpty(fTabs[i].Name))
            {
                string s = fTabs[i].Report.ReportInfo.Name;
                if (String.IsNullOrEmpty(s))
                    s = Path.GetFileNameWithoutExtension(fTabs[i].Report.FileName);
                if (String.IsNullOrEmpty(s))
                    s = (i + 1).ToString();
                return s;
            }
            else
                return fTabs[i].Name;
        }

        private string GetPrintMenu()
        {            
            StringBuilder sb = new StringBuilder();
            if (ReportProperties.ShowPrint)
            {
                res.Root("Web");
                sb.Append("<div class=\"td divider\">&nbsp;</div>");
                sb.Append("<ul class=\"td nav\">");
                sb.Append(String.Format("<li><input class=\"nav print_button\" type=\"button\" value=\"\" title=\"{0}\" onclick=\"{1}\"/>",
                    GetPrintRes(),
                    GetPrintReq()
                    ));
                if (ReportProperties.PrintInPdf && ReportProperties.PrintInBrowser)
                {
                    sb.Append("<ul class=\"round\">");
                    sb.Append(GetActionButton("print_browser", res.Get("PrintFromBrowser")));
                    sb.Append(GetActionButton("print_pdf", res.Get("PrintFromAcrobat")));
                    sb.Append("</ul>");
                }
                sb.Append("</li></ul>");
            }
            return sb.ToString();
        }

        private string GetPrintReq()
        {
            string result = "";
            if (ReportProperties.PrintInPdf && !ReportProperties.PrintInBrowser)
                result = GetRequest("print_pdf", "1");
            else if (!ReportProperties.PrintInPdf && ReportProperties.PrintInBrowser)
                result = GetRequest("print_browser", "1");
            return result;
        }

        private string GetPrintRes()
        {
            string result = "";
            res.Root("Web");
            if (ReportProperties.PrintInPdf && !ReportProperties.PrintInBrowser)
                result = res.Get("PrintFromAcrobat");
            else if (!ReportProperties.PrintInPdf && ReportProperties.PrintInBrowser)
                result = res.Get("PrintFromBrowser");
            return result;
        }

        private string GetExportMenu()
        {
            StringBuilder sb = new StringBuilder();
            if (ReportProperties.ShowExports)
            {
                sb.Append("<div class=\"td divider\">&nbsp;</div>");
                sb.Append("<ul class=\"td nav\">");
                sb.Append("<li><input class=\"nav export_button\" type=\"button\" value=\"\"/>");
                sb.Append("<ul class=\"round\">");

                res.Root("Preview");
                if (ReportProperties.ShowPreparedReport)
                    sb.Append(GetActionButton("export_fpx", res.Get("SaveNative")));
                res.Root("Export");
                if (ReportProperties.ShowPdfExport)
                    sb.Append(GetActionButton("export_pdf", res.Get("Pdf,File")));
                if (ReportProperties.ShowExcel2007Export)
                    sb.Append(GetActionButton("export_excel2007", res.Get("Xlsx,File")));
                if (ReportProperties.ShowWord2007Export)
                    sb.Append(GetActionButton("export_word2007", res.Get("Docx,File")));
                if (ReportProperties.ShowPowerPoint2007Export)
                    sb.Append(GetActionButton("export_pp2007", res.Get("Pptx,File")));
                if (ReportProperties.ShowTextExport)
                    sb.Append(GetActionButton("export_text", res.Get("Text,File")));
                if (ReportProperties.ShowRtfExport)
                    sb.Append(GetActionButton("export_rtf", res.Get("RichText,File")));
                if (ReportProperties.ShowXpsExport)
                    sb.Append(GetActionButton("export_xps", res.Get("Xps,File")));
                if (ReportProperties.ShowOdsExport)
                    sb.Append(GetActionButton("export_ods", res.Get("Ods,File")));
                if (ReportProperties.ShowOdtExport)
                    sb.Append(GetActionButton("export_odt", res.Get("Odt,File")));
                if (ReportProperties.ShowMhtExport)
                    sb.Append(GetActionButton("export_mht", res.Get("Html,MHTFile")));
                if (ReportProperties.ShowXmlExcelExport)
                    sb.Append(GetActionButton("export_xml", res.Get("Xml,File")));
                if (ReportProperties.ShowDbfExport)
                    sb.Append(GetActionButton("export_dbf", res.Get("Dbf,File")));
                if (ReportProperties.ShowCsvExport)
                    sb.Append(GetActionButton("export_csv", res.Get("Csv,File")));
                sb.Append("</ul></li></ul>");
            }

            return sb.ToString();
        }

        private string GetActionButton(string actionCode, string actionName)
        {
            return string.Format("<li class=\"menuitem\"><input class=\"menutext\" type=\"button\" name=\"{0}\"  value=\"{1}\" onclick=\"{2}\"/></li>",
                actionCode, actionName, GetRequest(actionCode, "1"));
        }

        private string GetRequest(string requestName, string value)
        {
            return string.Format("window.open('{0}?previewobject={1}&{2}={3}{4}')",
                ReportProperties.HandlerURL, ReportProperties.ControlID,
                requestName, value, WebUtils.GetSalt());
        }

        private string GetNavRequest(string requestName, string value)
        {
            return string.Format("frRequestServer('{0}?previewobject={1}&{2}={3}{4}')",
                ReportProperties.HandlerURL, ReportProperties.ControlID,
                requestName, value, WebUtils.GetSalt());
        }

        private string GetCheckBoxImageURL()
        {
            if (String.IsNullOrEmpty(ReportProperties.ButtonsPath))
                return GetResourceImageUrl("Checkbox.gif");
            else
                return ReportProperties.ButtonsPath + "Checkbox.gif";
        }

        private string GetDisabledImagesURL()
        {
            if (ReportProperties.ToolbarStyle == ToolbarStyle.Large)
                return (ReportProperties.ToolbarIconsStyle == ToolbarIconsStyle.Custom) ?
                    ReportProperties.ButtonsPath + "toolbar_disabled_big.png" :
                    GetResourceImageUrl("toolbar_disabled_32.png");
            else
                return (ReportProperties.ToolbarIconsStyle == ToolbarIconsStyle.Custom) ?
                    ReportProperties.ButtonsPath + "toolbar_disabled.png" :
                    GetResourceImageUrl("toolbar_disabled.png");
        }

        private string GetToolBarIconsURL()
        {
            string backExt = (ReportProperties.ToolbarStyle == ToolbarStyle.Large) ? "_32.png" : ".png";
            if (ReportProperties.ToolbarIconsStyle == ToolbarIconsStyle.Red)
                return GetResourceImageUrl("toolbar_red" + backExt);
            else if (ReportProperties.ToolbarIconsStyle == ToolbarIconsStyle.Green)
                return GetResourceImageUrl("toolbar_green" + backExt);
            else if (ReportProperties.ToolbarIconsStyle == ToolbarIconsStyle.Blue)
                return GetResourceImageUrl("toolbar_blue" + backExt);
            else if (ReportProperties.ToolbarIconsStyle == ToolbarIconsStyle.Black)
                return GetResourceImageUrl("toolbar_black" + backExt);
            else
            {
                // custom
                return (ReportProperties.ToolbarStyle == ToolbarStyle.Large) ?
                    ReportProperties.ButtonsPath + "toolbar_big.png" :
                    ReportProperties.ButtonsPath + "toolbar.png";
            }
        }

        private string GetToolBarBackgroundURL()
        {
            string backExt = (ReportProperties.ToolbarStyle == ToolbarStyle.Large) ? "_32.png" : ".png";
            if (ReportProperties.ToolbarBackgroundStyle == ToolbarBackgroundStyle.Light)
                return GetResourceImageUrl("toolbar_background_light" + backExt);
            else if (ReportProperties.ToolbarBackgroundStyle == ToolbarBackgroundStyle.Medium)
                return GetResourceImageUrl("toolbar_background_medium" + backExt);
            else if (ReportProperties.ToolbarBackgroundStyle == ToolbarBackgroundStyle.Dark)
                return GetResourceImageUrl("toolbar_background_dark" + backExt);
            else
            {
                // custom background
                return (ReportProperties.ToolbarStyle == ToolbarStyle.Large) ?
                    ReportProperties.ButtonsPath + "toolbar_background_big.png" :
                    ReportProperties.ButtonsPath + "toolbar_background.png";
            }
        }
        private string GetResourceImageUrl(string resName)
        {
            return page.ClientScript.GetWebResourceUrl(this.GetType(), string.Format("FastReport.Web.Resources.Images.{0}", resName));
        }

        private string GetResourceTemplateUrl(string resName)
        {
            return page.ClientScript.GetWebResourceUrl(this.GetType(), string.Format("FastReport.Web.Resources.Templates.{0}", resName));
        }

        private string GetResourceJqueryUrl(string resName)
        {
            return page.ClientScript.GetWebResourceUrl(this.GetType(), string.Format("FastReport.Web.Resources.jquery.{0}", resName));
        }

        private string GetResourceButtonUrl(string resName)
        {
            return page.ClientScript.GetWebResourceUrl(this.GetType(), string.Format("FastReport.Web.Resources.Buttons.{0}", resName));
        }

        private string GetResourceTemplate(string name)
        {
            string result;
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream(string.Format("FastReport.Web.Resources.Templates.{0}", name)))
            using (TextReader reader = new StreamReader(stream))
                result = reader.ReadToEnd();
            return result;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Registration of scripts and styles in ClientScript
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ClientScript"></param>
        /// <param name="t"></param>
        /// <param name="extJquery"></param>
        public void RegisterGlobals(string ID, ClientScriptManager ClientScript, Type t, bool extJquery)
        {
            ClientScript.RegisterClientScriptBlock(t, ID, GetCss(), false);
            ClientScript.RegisterClientScriptInclude(t, "fr_util", GetResourceTemplateUrl("fr_util.js"));
            if (!extJquery)
            {
                ClientScript.RegisterClientScriptInclude(t, "jquery-fr", GetResourceJqueryUrl("jquery.min.js"));
                ClientScript.RegisterClientScriptInclude(t, "jquery-ui-fr", GetResourceJqueryUrl("jquery-ui.custom.min.js"));
                ClientScript.RegisterClientScriptBlock(t, "jquery-css-fr",
                    String.Format("<link rel=\"stylesheet\" href=\"{0}\">",
                    GetResourceJqueryUrl("jquery-ui.min.css")), false);
            }
        }

        /// <summary>
        /// Gets Inline Registration as string
        /// </summary>
        /// <returns></returns>
        public string GetInlineRegistration(bool extJquery)
        {
            StringBuilder reg = new StringBuilder();
            reg.AppendLine(GetCss());

            if (!extJquery)
            {
                reg.AppendLine(WebReportGlobals.StylesAsString());
                reg.AppendLine(WebReportGlobals.ScriptsAsString());
            }
            else
            {
                reg.AppendLine(WebReportGlobals.StylesWOjQueryAsString());
                reg.AppendLine(WebReportGlobals.ScriptsWOjQueryAsString());
            }

            return reg.ToString();
        }

        /// <summary>
        /// Gets CSS of toolbar
        /// </summary>
        /// <returns></returns>
        public string GetCss()
        {
            StringBuilder sb = new StringBuilder();
            string s = GetResourceTemplate(ReportProperties.ToolbarStyle == ToolbarStyle.Large ? "styles_big.css" : "styles.css");
            sb.AppendLine("<style type=\"text/css\"><!--");
            s = s.Replace("frreport", ReportProperties.ControlID);
            WebTemplate template = new WebTemplate(s, WebTemplateMode.CSS, false);
            template.SetVariable("main-background-color", backgroundColor);
            template.SetVariable("body-height", "calc(100% - " + ReportProperties.ToolbarHeight.ToString() + "px)");
            template.SetVariable("toolbar-height", ReportProperties.ToolbarHeight.ToString() + "px");
            template.SetVariable("toolbar-background-color", WebUtils.HTMLColor(ReportProperties.ToolbarColor));
            template.SetVariable("toolbar-background-url", 
                ReportProperties.ToolbarBackgroundStyle == ToolbarBackgroundStyle.None ? "none" : 
                string.Format("url({0})", GetToolBarBackgroundURL()));
            template.SetVariable("toolbar-checkbox-url", string.Format("url({0})", GetCheckBoxImageURL()));
            template.SetVariable("toolbar-image-url", string.Format("url({0})", GetToolBarIconsURL()));
            template.SetVariable("toolbar-image-disabled-url", string.Format("url({0})", GetDisabledImagesURL()));
            s = template.Prepare();
            sb.AppendLine(s).Append("--></style>");
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetHtmlBody()
        {
            WebTemplate template = new WebTemplate(GetResourceTemplate("toolbar.html"), WebTemplateMode.HTML, false);
            if (ReportProperties.State == ReportState.Done)
            {
                template.SetVariable("back-button", GetBackButton());
                template.SetVariable("export-menu", GetExportMenu());
                template.SetVariable("print-menu", GetPrintMenu());
                template.SetVariable("zoom-menu", GetZoomMenu());
                template.SetVariable("navigation", GetNavigation());
            }
            template.SetVariable("refresh-button", GetRefresh());
            if (ReportProperties.TabPosition == TabPosition.InsideToolbar)
                template.SetVariable("tabsinside", GetTabs());
            else if (ReportProperties.TabPosition == TabPosition.UnderToolbar)
                template.SetVariable("tabsunder", GetTabs());
            template.SetVariable("reportid", GetReportId());
            return template.Prepare();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerPath"></param>
        /// <param name="ID"></param>
        /// <param name="preview"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public string GetHtmlProgress(string handlerPath, string ID, bool preview, Unit width, Unit height)
        {
            StringBuilder sb = new StringBuilder();
            string progress_path = String.IsNullOrEmpty(ReportProperties.ButtonsPath) ? 
                GetResourceImageUrl("Progress.gif") :
                ReportProperties.ButtonsPath + "Progress.gif";
            sb.Append("<div id=\"").Append(ID).Append("\"><noscript><span style='color:red'>ERROR: JavaScript disabled</span></noscript>");
            sb.Append("<div style=\"background:url(").Append(progress_path).Append(") no-repeat center center").
                Append(";width:100%;height:100%;min-width:110px;min-height:110px;\"></div>").
                Append("<script>frRequestServer(").
                Append("'").Append(handlerPath).Append("?").Append(preview ? "previewobject=" : "object=").Append(ID).
                Append("'").Append(");</script></div>");
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerPath"></param>
        /// <param name="ID"></param>
        /// <param name="refreshTimeOut"></param>
        /// <returns></returns>
        public string GetAutoRefresh(string handlerPath, string ID, int refreshTimeOut)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<script>setTimeout(function(){frRequestServer(").
                Append("'").Append(handlerPath).Append("?reload=true&object=").Append(ID).
                Append("'").
                Append(");},").Append(refreshTimeOut.ToString()).Append(");</script>");
            return sb.ToString();
        }
        #endregion

        /// <summary>
        /// Constructor of WebToolbar
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="tabs"></param>
        /// <param name="fit"></param>
        /// <param name="Localization"></param>
        public WebToolbar(string guid, List<ReportTab> tabs, bool fit, WebRes Localization)
        {
            page = new Page();
            fTabs = tabs;
            res = Localization;
            enableFit = fit;
        }

    }
}
