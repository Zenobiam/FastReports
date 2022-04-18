using FastReport.Preview;
using FastReport.Table;
using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace FastReport.Web.Handlers
{
    /// <summary>
    /// Web handler class
    /// </summary>
    public partial class WebExport : IHttpHandler, IRequiresSessionState
    {
        /// <summary>
        /// You will need to configure this handler in the web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        private WebReportCache cache;
        private WebReport webReport;
        private WebLog log;
        private delegate void ActionNet2<T1, T2>(T1 arg1, T2 arg2);

        private Report Report
        {
            get { return webReport.Report; }
        }

        private WebReportProperties Properties
        {
            get { return webReport.Prop; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }

        /// <summary>
        /// Process Request
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {                     
            // init of cache
            cache = new WebReportCache(context);
            // pictures
            if (context.Request.Params[WebUtils.PicsPrefix] != null)
                SendPicture(context);
            // export
            else if (context.Request.QueryString[WebUtils.ConstID] != null)
                SendExport(context);
            // print
            else if (context.Request.Params[WebUtils.PrintPrefix] != null &&
                context.Request.Params[WebUtils.ReportPrefix] != null)
                SendPrint(context);
            // report
            else if (context.Request.Params["object"] != null)
                SendObjectResponse(context);
            else if (context.Request.Params["previewobject"] != null)
                SendPreviewObjectResponse(context);
            else if (context.Request.Params["form"] != null)
                SendForm(context);
            else if (context.Request.Params["respic"] != null)
                SendResourcePic(context);
            else if (context.Request.Params["getReport"] != null)
                SendReportTemplate(context);
            else if (context.Request.Params["putReport"] != null)
                SetReportTemplate(context);
            else if (context.Request.Params["getPreview"] != null)
                SendReportPreview(context);
            else if (context.Request.Params["makePreview"] != null)
                MakeReportPreview(context);
            else if (context.Request.Params["getFunctions"] != null)
                MakeFunctionsList(context);
            else if (context.Request.Params["getDesignerConfig"] != null)
                MakeDesignerConfig(context);
            else if (context.Request.Params["getConnectionTypes"] != null)
                MakeConnectionTypes(context);
            else if (context.Request.Params["getConnectionTables"] != null)
                MakeConnectionTables(context);
            else if (context.Request.Params["getConnectionStringProperties"] != null)
                GetConnectionStringProperties(context);
            else if (context.Request.Params["makeConnectionString"] != null)
                MakeConnectionString(context);
            else
                SendDebug(context);
        }

        private void Finalize(HttpContext context)
        {
            // clean for objects in file cache
            if (cache.WebFarmMode)
            {
                if (webReport != null)
                {
                    if (webReport.Report != null)
                    {
                        if (webReport.Report.PreparedPages != null)
                        {
                            webReport.Report.PreparedPages.Clear();
                        }
                        webReport.Report.Dispose();
                        webReport.Report = null;
                    }
                    webReport.Dispose();
                    webReport = null;
                }
            }

            // see https://stackoverflow.com/questions/20988445/how-to-avoid-response-end-thread-was-being-aborted-exception-during-the-exce
            context.Response.Flush();
            context.Response.SuppressContent = true;
            context.ApplicationInstance.CompleteRequest();
            //context.Response.End();
        }

        private string GetResourceTemplateUrl(HttpContext context, string resName)
        {
            return new System.Web.UI.Page().ClientScript.
                GetWebResourceUrl(this.GetType(), string.Format("FastReport.Web.Resources.Templates.{0}", resName));
        }

        private void SendReportPreview(HttpContext context)
        {
            string guid = context.Request.Params["getPreview"];
            WebUtils.SetupResponse(webReport, context);
            context.Response.StatusCode = 501;
            Finalize(context);
        }

        private bool CertificateValidationCallBack(object sender,
            X509Certificate certificate,
            X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            //Return True to force the certificate to be accepted.
            return true;
        }

        private void SendResourcePic(HttpContext context)
        {
            if (WebUtils.SetupResponse(null, context))
            {
                string file = context.Request.Params["respic"];
                context.Response.AddHeader("Content-Type", string.Concat("image/", Path.GetExtension(file)));
                string resname = file.Replace('/', '.');
                using (Stream stream = this.GetType().Assembly.GetManifestResourceStream(string.Concat("FastReport.Web.Resources.jquery.", resname)))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    byte[] res = reader.ReadBytes((int)stream.Length);
                    context.Response.OutputStream.Write(res, 0, res.Length);
                    res = null;
                }
            }
            Finalize(context);
        }

        private void SendObjectResponse(HttpContext context)
        {
            string uuid = context.Request.Params["object"];
            SetUpWebReport(uuid, context);
            if (WebUtils.SetupResponse(webReport, context))
            {

                if (!NeedExport(context) && !NeedPrint(context))
                {
                    if (webReport.DesignReport)
                    {
                        webReport.ReportLoad(context);
                        webReport.ReportRegisterDataAndRunEvent();
                        SendDesigner(context, uuid);
                    }
                    else
                    {
                        SendReport(context);
                    }
                }
                cache.PutObject(uuid, webReport);
            }
            Finalize(context);
        }

        private void SendReport(HttpContext context)
        {
            StringBuilder sb = new StringBuilder();
            if (context.Request.Params["settab"] != null)
            {
                int i = 0;
                if (int.TryParse(context.Request.Params["settab"], out i))
                    if (i >= 0 && i < webReport.Tabs.Count)
                        webReport.CurrentTabIndex = i;
            }
            else if (context.Request.Params["closetab"] != null)
            {
                int i = 0;
                if (int.TryParse(context.Request.Params["closetab"], out i))
                    if (i >= 0 && i < webReport.Tabs.Count)
                    {
                        webReport.Tabs[i].Report.Dispose();
                        webReport.Tabs.RemoveAt(i);
                        if (i < webReport.Tabs.Count)
                            webReport.CurrentTabIndex = i;
                        else
                            webReport.CurrentTabIndex = i - 1;
                    }
            }

            Properties.ControlID = context.Request.Params["object"];

            if (String.IsNullOrEmpty(Properties.ControlID))
                Properties.ControlID = context.Request.Params["previewobject"];

            context.Response.AddHeader("Content-Type", "html/text");
            try
            {
                webReport.Localize();
                if (context.Request.Params["refresh"] != null)
                {
                    webReport.Refresh();
                    sb.Append(
                        webReport.Toolbar.GetHtmlProgress(
                            context.Response.ApplyAppPathModifier(WebUtils.HandlerFileName), Properties.ControlID, false, webReport.Width, webReport.Height));
                }
                else
                {
                    if (context.Request.Params["reload"] != null)
                        webReport.Refresh();

                    if (context.Request.Params["dialog"] != null)
                        webReport.SetUpDialogs(context);

                    SetReportPage(context);

                    if (Properties.RefreshTimeout > 0)
                        sb.Append(
                            webReport.Toolbar.GetAutoRefresh(
                                context.Response.ApplyAppPathModifier(
                                    WebUtils.HandlerFileName), Properties.ControlID, Properties.RefreshTimeout * 1000));

                    webReport.BeginReport(sb, context);
                    StringBuilder sb_rep = new StringBuilder();
                    webReport.GetReportHTML(sb_rep, context);
                    webReport.GetReportTopToolbar(sb);
                    sb.Append(sb_rep);
                    webReport.GetReportBottomToolbar(sb);
                    webReport.EndReport(sb);
                }
            }
            catch (Exception e)
            {
                log.AddError(e);
            }

            if (log.Text.Length > 0)
            {
                context.Response.Write(log.Text);
                log.Flush();
                log.Clear();
            }

            SetContainer(context, Properties.ControlID);
            context.Response.Write(sb.ToString());
        }

        private void SetContainer(HttpContext context, string p)
        {
            context.Response.AddHeader("FastReport-container", p);
        }

        private bool NeedPrint(HttpContext context)
        {
            if (context.Request.Params["print_browser"] != null)
                webReport.ExportHtml(context, true, true);
            else if (context.Request.Params["print_pdf"] != null)
                webReport.ExportPdf(context, true, true, true);
            else
                return false;
            return true;
        }

        private bool NeedExport(HttpContext context)
        {
            bool result = true;
            if (context.Request.Params["export_pdf"] != null)
                webReport.ExportPdf(context);
            else if (context.Request.Params["export_excel2007"] != null)
                webReport.ExportExcel2007(context);
            else if (context.Request.Params["export_word2007"] != null)
                webReport.ExportWord2007(context);
            else if (context.Request.Params["export_pp2007"] != null)
                webReport.ExportPowerPoint2007(context);
            else if (context.Request.Params["export_text"] != null)
                webReport.ExportText(context);
            else if (context.Request.Params["export_rtf"] != null)
                webReport.ExportRtf(context);
            else if (context.Request.Params["export_xps"] != null)
                webReport.ExportXps(context);
            else if (context.Request.Params["export_ods"] != null)
                webReport.ExportOds(context);
            else if (context.Request.Params["export_odt"] != null)
                webReport.ExportOdt(context);
            else if (context.Request.Params["export_mht"] != null)
                webReport.ExportMht(context);
            else if (context.Request.Params["export_xml"] != null)
                webReport.ExportXmlExcel(context);
            else if (context.Request.Params["export_dbf"] != null)
                webReport.ExportDbf(context);
            else if (context.Request.Params["export_csv"] != null)
                webReport.ExportCsv(context);
            else if (context.Request.Params["export_fpx"] != null)
                webReport.ExportPrepared(context);
            else
                result = false;
            return result;
        }

        private void SetReportPage(HttpContext context)
        {
            if (context.Request.Params["next"] != null)
                webReport.NextPage();
            else if (context.Request.Params["prev"] != null)
                webReport.PrevPage();
            else if (context.Request.Params["first"] != null)
                webReport.FirstPage();
            else if (context.Request.Params["last"] != null)
                webReport.LastPage();
            else if (context.Request.Params["goto"] != null)
            {
                int i = 0;
                if (int.TryParse(context.Request.Params["goto"], out i))
                    webReport.SetPage(i - 1);
            }
            else if (context.Request.Params["bookmark"] != null)
            {
                int i = PageNByBookmark(context.Server.UrlDecode(context.Request.Params["bookmark"]));
                if (i != -1)
                    webReport.SetPage(i);
            }
            else if (context.Request.Params["click"] != null)
            {
                string[] clickParams = context.Request.Params["click"].Split(',');
                if (clickParams.Length == 4)
                {
                    float left = 0;
                    float top = 0;
                    int pageN = 0;
                    if (int.TryParse(clickParams[1], out pageN) &&
                        float.TryParse(clickParams[2], out left) &&
                        float.TryParse(clickParams[3], out top))
                    {
                        DoClickObjectByParamID(clickParams[0], pageN, left, top);
                    }
                }
            }
            else if (context.Request.Params["detailed_report"] != null)
            {
                string[] detailParams = context.Server.UrlDecode(context.Request.Params["detailed_report"]).Split(',');
                if (detailParams.Length == 3)
                {
                    if (!String.IsNullOrEmpty(detailParams[0]) &&
                        !String.IsNullOrEmpty(detailParams[1]) &&
                        !String.IsNullOrEmpty(detailParams[2])
                        )
                    {
                        DoDetailedReport(detailParams[0], detailParams[1], detailParams[2]);
                    }
                }
            }
            else if (context.Request.Params["detailed_page"] != null)
            {
                string[] detailParams = context.Server.UrlDecode(context.Request.Params["detailed_page"]).Split(',');
                if (detailParams.Length == 3)
                {
                    if (!String.IsNullOrEmpty(detailParams[0]) &&
                        !String.IsNullOrEmpty(detailParams[1]) &&
                        !String.IsNullOrEmpty(detailParams[2])
                        )
                    {
                        DoDetailedPage(detailParams[0], detailParams[1], detailParams[2]);
                    }
                }
            }
            else if (context.Request.Params["zoom_width"] != null)
                webReport.Prop.ZoomMode = ZoomMode.Width;
            else if (context.Request.Params["zoom_page"] != null)
                webReport.Prop.ZoomMode = ZoomMode.Page;
            else if (context.Request.Params["zoom_300"] != null)
                webReport.Prop.Zoom = 3;
            else if (context.Request.Params["zoom_200"] != null)
                webReport.Prop.Zoom = 2;
            else if (context.Request.Params["zoom_150"] != null)
                webReport.Prop.Zoom = 1.5f;
            else if (context.Request.Params["zoom_100"] != null)
                webReport.Prop.Zoom = 1;
            else if (context.Request.Params["zoom_90"] != null)
                webReport.Prop.Zoom = 0.9f;
            else if (context.Request.Params["zoom_75"] != null)
                webReport.Prop.Zoom = 0.75f;
            else if (context.Request.Params["zoom_50"] != null)
                webReport.Prop.Zoom = 0.5f;
            else if (context.Request.Params["zoom_25"] != null)
                webReport.Prop.Zoom = 0.25f;
            else if (context.Request.Params["checkbox_click"] != null)
            {
                string[] clickParams = context.Request.Params["checkbox_click"].Split(',');
                if (clickParams.Length == 4)
                {
                    float left = 0;
                    float top = 0;
                    int pageN = 0;
                    if (int.TryParse(clickParams[1], out pageN) &&
                        float.TryParse(clickParams[2], out left) &&
                        float.TryParse(clickParams[3], out top))
                    {
                        DoCheckboxClick(clickParams[0], pageN, left, top);
                    }
                }
            }
            else if (context.Request.Params["text_edit"] != null)
            {
                string[] clickParams = context.Request.Params["text_edit"].Split(',');
                string text = context.Request.Form["text"];
                if (clickParams.Length == 4 && text != null)
                {
                    float left = 0;
                    float top = 0;
                    int pageN = 0;
                    if (int.TryParse(clickParams[1], out pageN) &&
                        float.TryParse(clickParams[2], out left) &&
                        float.TryParse(clickParams[3], out top))
                    {
                        string decodedText = HttpUtility.HtmlDecode(text);
                        // we need to normalize line endings because they are different in .net and browsers 
                        string normalizedText = System.Text.RegularExpressions.Regex.Replace(decodedText, @"\r\n|\n\r|\n|\r", "\r\n");
                        DoTextEdit(clickParams[0], pageN, left, top, normalizedText);
                    }
                }
            }
        }

        private void DoCheckboxClick(string objectName, int pageN, float left, float top)
        {
            if (Report.PreparedPages == null)
                return;

            bool found = false;
            while (pageN < Report.PreparedPages.Count && !found)
            {
                ReportPage page = Report.PreparedPages.GetPage(pageN);
                if (page != null)
                {
                    ObjectCollection allObjects = page.AllObjects;
                    System.Drawing.PointF point = new System.Drawing.PointF(left + 1, top + 1);
                    foreach (Base obj in allObjects)
                    {
                        CheckBoxObject c = obj as CheckBoxObject;
                        if (c != null &&
                            c.Name == objectName &&
                            c.AbsBounds.Contains(point))
                        {
                            c.Checked = !c.Checked;

                            if (webReport.Report.NeedRefresh)
                                webReport.Report.InteractiveRefresh();
                            else
                                webReport.Report.PreparedPages.ModifyPage(pageN, page);

                            found = true;
                            break;
                        }
                    }
                    page.Dispose();
                    pageN++;
                }
            }
        }

        private void DoTextEdit(string objectName, int pageN, float left, float top, string text)
        {
            if (Report.PreparedPages == null)
                return;
            ActionNet2<TextObject, ReportPage> AssignText = delegate(TextObject t, ReportPage page)
            {
                t.Text = text;
                if (webReport.Report.NeedRefresh)
                    webReport.Report.InteractiveRefresh();
                else
                    webReport.Report.PreparedPages.ModifyPage(pageN, page as ReportPage);
            };
            ProcessClick(objectName, pageN, left, top, null, AssignText, null);
        }

        private void DoDetailedReport(string objectName, string paramName, string paramValue)
        {
            if (!String.IsNullOrEmpty(objectName))
            {
                ReportComponentBase obj = Report.FindObject(objectName) as ReportComponentBase;
                if (obj != null && obj.Hyperlink.Kind == HyperlinkKind.DetailReport)
                {
                    Report tabReport = new Report();
                    string fileName = obj.Hyperlink.DetailReportName;
                    if (File.Exists(fileName))
                    {
                        tabReport.Load(fileName);

                        Data.Parameter param = tabReport.Parameters.FindByName(paramName);
                        if (param != null && param.ChildObjects.Count > 0)
                        {
                            string[] paramValues = paramValue.Split(obj.Hyperlink.ValuesSeparator[0]);
                            if (paramValues.Length > 0)
                            {
                                int i = 0;
                                foreach (Data.Parameter childParam in param.ChildObjects)
                                {
                                    childParam.Value = paramValues[i++];
                                    if (i >= paramValues.Length)
                                        break;
                                }
                            }
                        }
                        else
                            tabReport.SetParameterValue(paramName, paramValue);
                        Report.Dictionary.ReRegisterData(tabReport.Dictionary);
                        webReport.AddTab(tabReport, paramValue, false);
                        int prevTab = webReport.CurrentTabIndex;
                        webReport.CurrentTabIndex = webReport.Tabs.Count - 1;
                        webReport.Prop.PreviousTab = prevTab;
                    }
                }
            }
        }

        private void DoDetailedPage(string objectName, string paramName, string paramValue)
        {
            if (!String.IsNullOrEmpty(objectName))
            {
                Report currentReport = webReport.CurrentTab.NeedParent ? webReport.ReportTabs[0].Report : Report;
                ReportComponentBase obj = currentReport.FindObject(objectName) as ReportComponentBase;
                if (obj != null && obj.Hyperlink.Kind == HyperlinkKind.DetailPage)
                {
                    ReportPage reportPage = currentReport.FindObject(obj.Hyperlink.DetailPageName) as ReportPage;
                    if (reportPage != null)
                    {
                        Data.Parameter param = currentReport.Parameters.FindByName(paramName);
                        if (param != null && param.ChildObjects.Count > 0)
                        {
                            string[] paramValues = paramValue.Split(obj.Hyperlink.ValuesSeparator[0]);
                            if (paramValues.Length > 0)
                            {
                                int i = 0;
                                foreach (Data.Parameter childParam in param.ChildObjects)
                                {
                                    childParam.Value = paramValues[i++];
                                    if (i >= paramValues.Length)
                                        break;
                                }
                            }
                        }
                        else
                        {
                            currentReport.SetParameterValue(paramName, paramValue);
                        }
                        PreparedPages oldPreparedPages = currentReport.PreparedPages;
                        PreparedPages pages = new PreparedPages(currentReport);
                        Report tabReport = new Report();
                        currentReport.SetPreparedPages(pages);
                        currentReport.PreparePage(reportPage);
                        tabReport.SetPreparedPages(currentReport.PreparedPages);
                        webReport.AddTab(tabReport, paramValue, true, true);
                        int prevTab = webReport.CurrentTabIndex;
                        currentReport.SetPreparedPages(oldPreparedPages);
                        webReport.CurrentTabIndex = webReport.Tabs.Count - 1;
                        webReport.Prop.PreviousTab = prevTab;
                    }
                }
            }
        }

        private void DoClickObjectByParamID(string objectName, int pageN, float left, float top)
        {
            if (webReport.Report.PreparedPages != null)
            {
                bool found = false;
                while (pageN < webReport.Report.PreparedPages.Count && !found)
                {
                    ReportPage page = webReport.Report.PreparedPages.GetPage(pageN);
                    if (page != null)
                    {
                        ObjectCollection allObjects = page.AllObjects;
                        System.Drawing.PointF point = new System.Drawing.PointF(left + 1, top + 1);
                        foreach (Base obj in allObjects)
                        {
                            if (obj is ReportComponentBase)
                            {
                                ReportComponentBase c = obj as ReportComponentBase;
                                if (c is TableBase)
                                {
                                    TableBase table = c as TableBase;
                                    for (int i = 0; i < table.RowCount; i++)
                                    {
                                        for (int j = 0; j < table.ColumnCount; j++)
                                        {
                                            TableCell textcell = table[j, i];
                                            if (textcell.Name == objectName)
                                            {
                                                System.Drawing.RectangleF rect =
                                                    new System.Drawing.RectangleF(table.Columns[j].AbsLeft,
                                                    table.Rows[i].AbsTop,
                                                    textcell.Width,
                                                    textcell.Height);
                                                if (rect.Contains(point))
                                                {
                                                    Click(textcell, pageN, page);
                                                    found = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (found)
                                            break;
                                    }
                                }
                                else
                                if (c.Name == objectName &&
#if FRCORE
                                  c.AbsBounds.Contains(point))
#else
                                  c.PointInObject(point))
#endif
                                {
                                    Click(c, pageN, page);
                                    found = true;
                                    break;
                                }
                            }
                        }
                        page.Dispose();
                        pageN++;
                    }
                }
            }
        }

        private void Click(ReportComponentBase c, int pageN, ReportPage page)
        {
            c.OnClick(null);
            if (webReport.Report.NeedRefresh)
                webReport.Report.InteractiveRefresh();
            else
                webReport.Report.PreparedPages.ModifyPage(pageN, page);
        }

        private int PageNByBookmark(string bookmark)
        {
            int pageN = -1;
            if (webReport.Report.PreparedPages != null)
            {
                for (int i = 0; i < webReport.Report.PreparedPages.Count; i++)
                {
                    ReportPage page = webReport.Report.PreparedPages.GetPage(i);
                    if (page != null)
                    {
                        ObjectCollection allObjects = page.AllObjects;
                        for (int j = 0; j < allObjects.Count; j++)
                        {
                            ReportComponentBase c = allObjects[j] as ReportComponentBase;
                            if (c.Bookmark == bookmark)
                            {
                                pageN = i;
                                break;
                            }
                        }
                        page.Dispose();
                        if (pageN != -1)
                            break;
                    }
                }
            }
            return pageN;
        }

        private void SendDebug(HttpContext context)
        {
            if (WebUtils.SetupResponse(null, context))
            {
                int count = cache.CleanFileStorage();
                context.Response.ContentType = "text/html";
                context.Response.Write("FastReport.Web.WebExport handler: FastReport.NET");
                context.Response.Write(String.Concat("<b>v", FastReport.Utils.Config.Version, "</b><br/>",
                    "Current server time: <b>", DateTime.Now.ToString(), "</b><br/>",
                    "Cluster mode: <b>", (cache.WebFarmMode ? "ON" : "OFF"), "</b><br/>",
                    "Files in storage: <b>", count.ToString(), "</b>"));
            }
            Finalize(context);
        }

        private void SendPrint(HttpContext context)
        {
            try
            {
                string guid = context.Request.Params[WebUtils.ReportPrefix];
                SetUpWebReport(guid, context);
                if (WebUtils.SetupResponse(webReport, context))
                {
                    if (context.Request.Params[WebUtils.PrintPrefix] == "pdf")
                        webReport.PrintPdf(context);
                    else
                        webReport.PrintHtml(context);
                }
            }
            catch (Exception e)
            {
                log.AddError(e);
            }
            if (log.Text.Length > 0)
            {
                context.Response.Write(log.Text);
                log.Flush();
                log.Clear();
            }
            Finalize(context);
        }

        private void SendExport(HttpContext context)
        {
            log = new WebLog(false);
            string cacheKeyName = context.Request.QueryString[WebUtils.ConstID];
            WebExportItem exportItem = cache.GetObject(cacheKeyName) as WebExportItem;
            SetUpWebReport(exportItem.ReportID, context);
            if (WebUtils.SetupResponse(webReport, context))
            {
                if (exportItem != null)
                {
                    try
                    {
                        bool isIE8 = WebUtils.IsIE8(context);
                        //cache.Remove(cacheKeyName);
                        context.Response.ClearContent();
                        context.Response.ClearHeaders();
                        if (string.IsNullOrEmpty(exportItem.ContentType))
                            context.Response.ContentType = "application/unknown";
                        else
                            context.Response.ContentType = exportItem.ContentType;
                        context.Response.AddHeader("Content-Type", context.Response.ContentType);

                        if (!isIE8)
                            WebUtils.AddNoCacheHeaders(context);

                        string disposition = "attachment";
                        if (context.Request.QueryString["displayinline"].Equals("True", StringComparison.OrdinalIgnoreCase))
                            disposition = "inline";

                        string fileName = context.Server.UrlPathEncode(exportItem.FileName).Replace(",", "");
                        string s = String.Concat(isIE8 ? "=" : "*=UTF-8''", fileName);
                        string contentDisposition = string.Format("{0}; filename{1}", disposition, s);
                        context.Response.AddHeader("Content-Disposition", contentDisposition);
                        context.Response.AddHeader("Cache-Control", "private");
                        context.Response.Flush();

                        WebUtils.ResponseChunked(context.Response, exportItem.File);

                        //exportItem.File = null;
                        //exportItem = null;
                    }
                    catch (Exception e)
                    {
                        log.AddError(e);
                    }

                    if (log.Text.Length > 0)
                    {
                        context.Response.Write(log.Text);
                        log.Flush();
                        log.Clear();
                    }
                }
            }
            Finalize(context);
        }

        private void SendPicture(HttpContext context)
        {
            if (WebUtils.SetupResponse(null, context))
            {
                try
                {
                    string imageKeyName = Convert.ToString(context.Request.Params[WebUtils.PicsPrefix]);
                    byte[] image = cache.GetObject(imageKeyName) as byte[];

                    context.Response.ContentType = WebUtils.IsPng(image) ? "image/png" : "image/svg+xml";
                    context.Response.Flush();
                    try
                    {
                        if (image != null)
                            WebUtils.ResponseChunked(context.Response, image);
                    }
                    finally
                    {
                        image = null;
                    }
                }
                catch
                {
                    // nothing
                }
            }
            Finalize(context);
        }

        private void SendForm(HttpContext context)
        {
            context.Response.AddHeader("Content-Type", "text/html");
            string uuid = context.Request.Params["form"];
            SetUpWebReport(uuid, context);
            if (WebUtils.SetupResponse(webReport, context))
            {
                string formName = context.Request.Params["formName"];
                if (webReport != null && formName == "text_edit")
                {
                    string click = context.Request.Params["formClick"].ToString();
                    if (!String.IsNullOrEmpty(click))
                    {
                        string[] clickParams = click.Split(',');
                        if (clickParams.Length == 4)
                        {
                            int pageN;
                            float left;
                            float top;
                            if (int.TryParse(clickParams[1], out pageN) &&
                                float.TryParse(clickParams[2], out left) &&
                                float.TryParse(clickParams[3], out top))
                            {
                                WriteTextEditForm(context.Response, clickParams[0], pageN, left, top);
                            }
                        }
                    }
                }
            }
            Finalize(context);
        }

        private void WriteTextEditForm(HttpResponse response, string objectName, int pageN, float left, float top)
        {
            if (Report.PreparedPages == null)
                return;
            ProcessClick(objectName, pageN, left, top, EditClick, null, response);
        }

        private void EditClick(TextObject c, HttpResponse response)
        {
            string encodedText = HttpUtility.HtmlEncode((c as TextObject).Text);
            WebTemplate template = new WebTemplate(GetResourceTemplate("textedit_form.html"), WebTemplateMode.HTML, true);
            template.SetVariable("text", encodedText);
            template.SetVariable("cancel-button", GetCancelButton());
            template.SetVariable("ok-button", GetOkButton());
            response.Write(template.Prepare());
        }

        private void ProcessClick(string objectName, int pageN, float left, float top, ActionNet2<TextObject, HttpResponse> HandleClickFront, ActionNet2<TextObject,
            ReportPage> HandleClickBack, HttpResponse response)
        {
            bool found = false;
            while (pageN < Report.PreparedPages.Count && !found)
            {
                ReportPage page = Report.PreparedPages.GetPage(pageN);
                if (page != null)
                {
                    ObjectCollection allObjects = page.AllObjects;
                    System.Drawing.PointF point = new System.Drawing.PointF(left + 1, top + 1);
                    foreach (Base obj in allObjects)
                    {
                        if (obj is ReportComponentBase)
                        {
                            ReportComponentBase c = obj as ReportComponentBase;
                            if (c is TableBase)
                            {
                                TableBase table = c as TableBase;
                                for (int i = 0; i < table.RowCount; i++)
                                {
                                    for (int j = 0; j < table.ColumnCount; j++)
                                    {
                                        TableCell textcell = table[j, i];
                                        if (textcell.Name == objectName)
                                        {
                                            RectangleF rect = new RectangleF(table.Columns[j].AbsLeft,
                                                table.Rows[i].AbsTop,
                                                textcell.Width,
                                                textcell.Height);
                                            if (rect.Contains(point))
                                            {
                                                if (HandleClickFront != null)
                                                    HandleClickFront(textcell as TextObject, response);
                                                if (HandleClickBack != null)
                                                    HandleClickBack(textcell as TextObject, page);
                                                found = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (found)
                                        break;
                                }
                            }
                            else if (c is TextObject)
                            {
                                if (c != null && c.Name == objectName && c.AbsBounds.Contains(point))
                                {
                                    if (HandleClickFront != null)
                                        HandleClickFront(c as TextObject, response);
                                    if (HandleClickBack != null)
                                        HandleClickBack(c as TextObject, page);
                                    found = true;
                                    break;
                                }
                            }
                            if (found)
                                break;
                        }
                    }
                    page.Dispose();
                    pageN++;
                }
            }
        }
        private string GetCancelButton()
        {
            StringBuilder sb = new StringBuilder();
            WebRes res = webReport.ReportRes;
            res.Root("Buttons");
            sb.Append(string.Format("<button onclick=\"window.close();\">{0}</button>", res.Get("Cancel")));
            return sb.ToString();
        }

        private string GetOkButton()
        {
            StringBuilder sb = new StringBuilder();
            WebRes res = webReport.ReportRes;
            res.Root("Buttons");
            sb.Append(string.Format("<button onclick=\"window.postMessage('submit', '*'); \">{0}</button>", res.Get("Ok")));
            return sb.ToString();
        }

        private void SetUpWebReport(string ID, HttpContext context)
        {
            if (webReport == null)
            {
                webReport = new WebReport();
                webReport.ReportGuid = ID;
            }
            webReport = cache.GetObject(ID, webReport) as WebReport;
            webReport.Prop.HandlerURL = WebUtils.GetBasePath(context) + WebUtils.HandlerFileName;
            cache.Priority = webReport.CachePriority;
            cache.Delay = webReport.CacheDelay;
            log = new WebLog(webReport.Debug);
            if (!String.IsNullOrEmpty(webReport.LogFile))
            {
                log.LogFile = context.Server.MapPath(webReport.LogFile);
            }
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
    }
}
