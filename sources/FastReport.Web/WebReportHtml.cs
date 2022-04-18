using FastReport.Export;
using FastReport.Export.Html;
using FastReport.Utils;
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FastReport.Web
{
    public partial class WebReport : WebControl, INamingContainer
    {
        float _pageWidth;
        float _pageHeight;

        internal void BeginReport(StringBuilder sb, HttpContext context)
        {
            sb.AppendLine(string.Format("<div class=\"{0}\" style=\"width:{1};height:{2}\">", Prop.ControlID, Width, Height));
        }

        internal void GetReportTopToolbar(StringBuilder sb)
        {
            if (Prop.ShowToolbar)
                GetToolbar(sb);
        }

        internal void GetReportBottomToolbar(StringBuilder sb)
        {
            if (Prop.ShowBottomToolbar)
                GetToolbar(sb);
        }

        private void GetToolbar(StringBuilder sb)
        {
            string s = Toolbar.GetHtmlBody();
            sb.AppendLine(s);
        }

        internal void EndReport(StringBuilder sb)
        {
            sb.AppendLine("</div>");
        }

        private void ReportProcess(StringBuilder sb, HttpContext context)
        {
            if (Prop.State == ReportState.Empty)
            {
                ReportLoad(context);
                if (Prop.UnlimitedHeight || Prop.UnlimitedWidth)
                {
                    foreach (Base obj in Report.AllObjects)
                    {
                        if (obj is ReportPage)
                        {
                            if (Prop.UnlimitedWidth)
                                (obj as ReportPage).UnlimitedWidth = true;
                            if (Prop.UnlimitedHeight)
                                (obj as ReportPage).UnlimitedHeight = true;
                        }
                    }
                }
                ReportRegisterDataAndRunEvent();
                if (!Prop.ReportDone)
                    ReportPrepare();
            }

            if (Prop.State == ReportState.Forms && sb != null)
                ProcessDialogs(sb, context);

            if (Prop.State == ReportState.Report)
            {
                Report.PreparePhase2();
                if (Report.PreparedPages != null)
                {
                    Prop.State = ReportState.Done;
                }
            }

            if (Prop.State == ReportState.Done)
            {
                Prop.TotalPages = Report.PreparedPages.Count;
            }
        }

        private void ReportPrepare()
        {
            Report.PreparePhase1();
            Prop.State = ReportState.Forms;
        }

        internal void ReportRegisterDataAndRunEvent()
        {
#if !FRCORE
            Config.ReportSettings.ShowProgress = false;
#endif
            this.RegisterData(Report);
            this.OnStartReport(EventArgs.Empty);
        }

        internal void ReportLoad(HttpContext context)
        {
            Config.WebMode = true;
            FastReport.Data.ParameterCollection preParams = new FastReport.Data.ParameterCollection(null);
            Report.Parameters.CopyTo(preParams);
            if (!String.IsNullOrEmpty(Prop.ReportFile))
            {
                string fileName = Prop.ReportFile;
                if (!WebUtils.IsAbsolutePhysicalPath(fileName))
                    fileName = context.Request.MapPath(fileName,
                        Path.GetDirectoryName(context.Request.CurrentExecutionFilePath), true);
                if (!File.Exists(fileName))
                    fileName = context.Request.MapPath(Prop.ReportFile,
                        Prop.ReportPath, true);
                Report.Load(fileName);
            }
            else if (!String.IsNullOrEmpty(Prop.ReportResourceString))
            {
                Report.ReportResourceString = Prop.ReportResourceString;
            }
            foreach (FastReport.Data.Parameter param in preParams)
            {
                if (param.Value != null)
                    Report.SetParameterValue(param.Name, param.Value);
            }
        }

        internal void GetReportHTML(StringBuilder sb, HttpContext context)
        {
            ReportProcess(sb, context);
            if (Prop.State == ReportState.Done)
                ReportInHtmlAndSave(sb, context);
        }

        private void ReportInHtmlAndSave(StringBuilder sb, HttpContext context)
        {
            HTMLExport html = new HTMLExport(true); // webPreview mode
            html.CustomDraw += this.CustomDraw;
            html.StylePrefix = Prop.ControlID.Substring(0, 6);
            html.Init_WebMode();
            html.Pictures = Prop.Pictures;
            html.EmbedPictures = EmbedPictures;
            html.EnableVectorObjects = !WebUtils.IsIE8(context); // don't draw svg barcodes for IE8
            html.OnClickTemplate =
                //"frRequestServer('" + context.Response.ApplyAppPathModifier(WebUtils.HandlerFileName) + "?previewobject={0}&amp;{1}={2}')";
                "frClick('" + context.Response.ApplyAppPathModifier(WebUtils.HandlerFileName) + "', '{0}', '{1}', '{2}')";
            html.ReportID = Prop.ControlID;
            html.EnableMargins = Prop.EnableMargins;

            // calc zoom
            CalcHtmlZoom(html);

            html.Layers = Layers;
            html.PageNumbers = SinglePage ? "" : (Prop.CurrentPage + 1).ToString();

            if (Prop.AutoWidth)
                html.WidthUnits = HtmlSizeUnits.Percent;
            if (Prop.AutoHeight)
                html.HeightUnits = HtmlSizeUnits.Percent;

            html.WebImagePrefix = String.Concat(context.Response.ApplyAppPathModifier(WebUtils.HandlerFileName), "?", WebUtils.PicsPrefix);
            html.SinglePage = SinglePage;
            html.CurPage = CurrentPage;
            html.Export(Report, (Stream)null);

            AddOutlineHtml(sb);

            sb.Append("<div class=\"frbody\" style =\"");
            if (Layers)
                sb.Append("position:relative;z-index:0;");
            sb.Append("\">");

            // container for html report body
            int pageWidth = (int)Math.Ceiling(GetReportPageWidthInPixels() * html.Zoom);
            int pageHeight = (int)Math.Ceiling(GetReportPageHeightInPixels() * html.Zoom);
            int paddingLeft = (int)Math.Ceiling(Padding.Left * html.Zoom);
            int paddingRight = (int)Math.Ceiling(Padding.Right * html.Zoom);
            int paddingTop = (int)Math.Ceiling(Padding.Top * html.Zoom);
            int paddingBottom = (int)Math.Ceiling(Padding.Bottom * html.Zoom);
            sb.Append("<div class=\"frcontainer\" style=\"width:" + pageWidth +
                "px;height:" + pageHeight +
                "px;padding-left:" + paddingLeft +
                "px;padding-right:" + paddingRight +
                "px;padding-top:" + paddingTop +
                "px;padding-bottom:" + paddingBottom + "px\">");

            if (html.Count > 0)
            {
                if (SinglePage)
                {
                    DoAllHtmlPages(sb, html);
                    Prop.CurrentPage = 0;
                }
                else
                    DoHtmlPage(sb, html, 0);
            }

            sb.Append("</div>"); // frcontainer
            sb.Append("</div>"); // frbody
        }

        private float GetReportPageWidthInPixels()
        {
            if(SinglePage)
            {
                foreach (ReportPage page in Report.Pages)
                {
                    // find maxWidth
                    if (page.WidthInPixels > _pageWidth)
                        _pageWidth = page.WidthInPixels;
                }
            }
            else
            {
                _pageWidth = Report.PreparedPages.GetPageSize(CurrentPage).Width;
            }

            return _pageWidth;
        }

        private float GetReportPageHeightInPixels()
        {
            _pageHeight = 0;
            if (SinglePage)
            {
                for(int i = 0; i < Report.PreparedPages.Count; i++)
                {
                    _pageHeight += Report.PreparedPages.GetPageSize(i).Height;
                }
            }
            else
            {
                _pageHeight = Report.PreparedPages.GetPageSize(CurrentPage).Height;
            }

            return _pageHeight;
        }

        private void CalcHtmlZoom(HTMLExport html)
        {
            float pageWidth = GetReportPageWidthInPixels() + Padding.Horizontal;
            float pageHeight = GetReportPageHeightInPixels() + Padding.Vertical;

            int scrollbarWidth = 20;

            if (Prop.ZoomMode == ZoomMode.Width)
                html.Zoom = (float)Math.Round((this.Width.Value - scrollbarWidth) / pageWidth, 2);
            else if (Prop.ZoomMode == ZoomMode.Page)
                html.Zoom = (float)Math.Round(Math.Min((float)(this.Width.Value - scrollbarWidth) / pageWidth,
                    (float)(this.Height.Value -
                    (Prop.ShowToolbar ? Prop.ToolbarHeight : 0) -
                    (Prop.ShowBottomToolbar ? Prop.ToolbarHeight : 0) - scrollbarWidth) / pageHeight), 2);
            else
                html.Zoom = Prop.Zoom;
        }

        private void DoHtmlPage(StringBuilder sb, HTMLExport html, int pageN)
        {
            if (html.PreparedPages[pageN].PageText == null)
            {
                html.PageNumbers = (pageN + 1).ToString();
                html.Export(Report, (Stream)null);
            }

            Prop.CurrentWidth = html.PreparedPages[pageN].Width;
            Prop.CurrentHeight = html.PreparedPages[pageN].Height;

            if (html.PreparedPages[pageN].CSSText != null
                && html.PreparedPages[pageN].PageText != null)
            {
                sb.Append(html.PreparedPages[pageN].CSSText);
                sb.Append(html.PreparedPages[pageN].PageText);
                StoreHtmlPictures(html, pageN);
            }
        }

        private void DoAllHtmlPages(StringBuilder sb, HTMLExport html)
        {
            Prop.CurrentWidth = 0;
            Prop.CurrentHeight = 0;
            for (int pageN = 0; pageN < html.PreparedPages.Count; pageN++)
            {
                if (html.PreparedPages[pageN].PageText == null)
                {
                    html.PageNumbers = (pageN + 1).ToString();
                    html.Export(Report, (Stream)null);
                    if (html.PreparedPages[pageN].Width > Prop.CurrentWidth)
                        Prop.CurrentWidth = html.PreparedPages[pageN].Width;
                    if (html.PreparedPages[pageN].Height > Prop.CurrentHeight)
                        Prop.CurrentHeight = html.PreparedPages[pageN].Height;
                }
                if (html.PreparedPages[pageN].CSSText != null
                    && html.PreparedPages[pageN].PageText != null)
                {
                    sb.Append(html.PreparedPages[pageN].CSSText);
                    sb.Append(html.PreparedPages[pageN].PageText);
                    StoreHtmlPictures(html, Layers ? 0 : pageN);
                }
            }
        }

        private void StoreHtmlPictures(HTMLExport html, int pageN)
        {
            WebReportCache cache = new WebReportCache(this.Context);
            for (int i = 0; i < html.PreparedPages[pageN].Pictures.Count; i++)
            {
                try
                {
                    Stream picStream = html.PreparedPages[pageN].Pictures[i];
                    byte[] image = new byte[picStream.Length];
                    picStream.Position = 0;
                    int n = picStream.Read(image, 0, (int)picStream.Length);
                    string guid = html.PreparedPages[pageN].Guids[i];
                    cache.PutObject(guid, image);
                }
                catch
                {
                    //Log.AppendFormat("Error with picture: {0}\n", i.ToString());
                }
            }
        }

        private void AddOutlineHtml(StringBuilder sb)
        {
            if (!ShowOutline)
                return;

            if (Report == null ||
                Report.Engine == null ||
                Report.Engine.OutlineXml == null ||
                Report.Engine.OutlineXml.Count == 0)
                return;

            StringBuilder outlineJson = new StringBuilder();
            outlineJson.Append("[");
            BuildOutline(outlineJson, Report.Engine.OutlineXml, true);
            outlineJson.Append("]");
            
            bool percent = Height.Type == UnitType.Percentage;
            string pageHeight = ((_pageHeight + Padding.Vertical) /* * (Height.Value / 100)*/ * Prop.Zoom)
                .ToString().Replace(',','.') + "px";
            string navRequest = string.Format("frRequestServer('{0}?previewobject={1}&goto=' + val + '{2}')",
                Prop.HandlerURL, // 0
                Prop.ControlID, // 1
                WebUtils.GetSalt()); // 2

            sb.Append("<div class=\"froutline\">")
              .Append(percent ? "<div class=\"froutline2\" style=\"max-height:" + pageHeight + "\">" : "")

              .Append("<style>")
              .Append(GetResource("jstree.style.min.css")
                .Replace("32px.png", GetResourceUrl("jstree.32px.png")) // replace pictures links
                .Replace("40px.png", GetResourceUrl("jstree.40px.png"))
                .Replace("throbber.gif", GetResourceUrl("jstree.throbber.gif")))
              .Append("</style>")

              .Append("<style>.gutter.gutter-horizontal { height:" + (percent ? pageHeight : ("calc(100% - " + Prop.ToolbarHeight.ToString() + "px)")) + "; }</style>")

              .Append("<script>")
              .Append("(function(){").Append(GetResource("jstree.jstree.min.js")).Append("})();")
              .Append("(function(){").Append(GetResource("split.split.min.js")).Append("}).call(frOutline);")
              .Append("frOutline(") // call frOutline function
                .Append(outlineJson) // 1st param of frOutline function
                .Append(",\"").Append(WebUtils.JavaScriptStringEncode(ReportGuid)).Append("\",") // 2nd param of frOutline function
                .Append("function(val){" + navRequest + ";});") // 3rd param of frOutline function
              .Append("</script>")

              .Append("<div class=\"froutlinecontainer\"></div>")

              .Append(percent ? "</div>" : "") // froutline2
              .Append("</div>"); // froutline
        }

        private void BuildOutline(StringBuilder sb, XmlItem xml, bool top)
        {
            for (int i = 0; i < xml.Count; i++)
            {
                sb.Append("{");

                XmlItem node = xml[i];
                string text = node.GetProp("Text");
                int page = 0;
                float offset = 0f;

                string s = node.GetProp("Page");
                if (s != "")
                {
                    page = int.Parse(s);
                    s = node.GetProp("Offset");
                    if (s != "")
                        offset = (float)Converter.FromString(typeof(float), s)/* * PDF_DIVIDER*/;
                }

                // id for saving tree state
                sb.Append("\"id\":\"").Append(WebUtils.JavaScriptStringEncode(ReportGuid + "_" + text + "_" + page + "_" + offset + "_" + sb.Length)).Append("\",");

                sb.Append("\"text\":\"").Append(WebUtils.JavaScriptStringEncode(text)).Append("\",");
                sb.Append("\"data\":{")
                    .Append("\"page\":").Append(page)
                    .Append(",\"offset\":").Append(offset.ToString().Replace(',', '.'))
                .Append("}");

                // open if there is only one node on top
                if (top && xml.Count == 1)
                    sb.Append(",\"state\":{\"opened\":true}");

                if (node.Count > 0)
                {
                    sb.Append(",\"children\": [");
                    BuildOutline(sb, node, false);
                    sb.Append("]");
                }

                sb.Append("}");

                // don't add comma if it is the last node
                if (i < xml.Count - 1)
                    sb.Append(",");
            }
        }

        private string GetResource(string resName)
        {
            string result;
            using (Stream stream = typeof(WebReport).Assembly.GetManifestResourceStream("FastReport.Web.Resources." + resName))
            using (TextReader reader = new StreamReader(stream))
                result = reader.ReadToEnd();
            return result;
        }

        private string GetResourceUrl(string resName)
        {
            return Page.ClientScript.GetWebResourceUrl(this.GetType(), "FastReport.Web.Resources." + resName);
        }
    }
}
 