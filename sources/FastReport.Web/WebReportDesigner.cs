using FastReport.Data;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;

namespace FastReport.Web.Handlers
{
    /// <summary>
    /// Web handler class
    /// </summary>
    public partial class WebExport: IHttpHandler
    {
        private string CutRestricted(WebReport webReport, string xmlString)
        {
            using (MemoryStream xmlStream = new MemoryStream())
            {
                WebUtils.Write(xmlStream, xmlString);
                xmlStream.Position = 0;
                using (FastReport.Utils.XmlDocument xml = new FastReport.Utils.XmlDocument())
                {
                    xml.Load(xmlStream);

                    if (!webReport.DesignScriptCode)
                    {
                        xml.Root.SetProp("CodeRestricted", "true");
                        // cut script
                        FastReport.Utils.XmlItem scriptItem = xml.Root.FindItem("ScriptText");
                        if (scriptItem != null && !String.IsNullOrEmpty(scriptItem.Value))
                            scriptItem.Value = String.Empty;
                    }
                    // cut connection strings
                    FastReport.Utils.XmlItem dictionary = xml.Root.FindItem("Dictionary");
                    if (dictionary != null)
                    {
                        for (int i = 0; i < dictionary.Items.Count; i++)
                        {
                            FastReport.Utils.XmlItem item = dictionary.Items[i];
                            if (!String.IsNullOrEmpty(item.GetProp("ConnectionString")))
                                item.SetProp("ConnectionString", String.Empty);
                        }
                    }

                    // save prepared xml
                    using (MemoryStream secondXmlStream = new MemoryStream())
                    {
                        xml.Save(secondXmlStream);
                        secondXmlStream.Position = 0;
                        byte[] buff = new byte[secondXmlStream.Length];
                        secondXmlStream.Read(buff, 0, buff.Length);
                        xmlString = Encoding.UTF8.GetString(buff);
                    }
                }
            }
            return xmlString;
        }

        private string PasteRestricted(WebReport webReport, string xmlString)
        {
            using (MemoryStream xmlStream1 = new MemoryStream())
            using (MemoryStream xmlStream2 = new MemoryStream())
            {
                WebUtils.Write(xmlStream1, webReport.Report.SaveToString());
                WebUtils.Write(xmlStream2, xmlString);
                xmlStream1.Position = 0;
                xmlStream2.Position = 0;
                FastReport.Utils.XmlDocument xml1 = new FastReport.Utils.XmlDocument();
                FastReport.Utils.XmlDocument xml2 = new FastReport.Utils.XmlDocument();
                xml1.Load(xmlStream1);
                xml2.Load(xmlStream2);

                if (!webReport.DesignScriptCode)
                {
                    xml2.Root.SetProp("CodeRestricted", "");
                    // clean received script
                    FastReport.Utils.XmlItem scriptItem2 = xml2.Root.FindItem("ScriptText");
                    if (scriptItem2 != null)
                        scriptItem2.Value = "";
                    // paste old script
                    FastReport.Utils.XmlItem scriptItem1 = xml1.Root.FindItem("ScriptText");
                    if (scriptItem1 != null)
                    {
                        if (String.IsNullOrEmpty(scriptItem1.Value))
                        {
                            scriptItem2.Dispose();
                            scriptItem2 = null;
                        }
                        else
                        if (scriptItem2 != null)
                            scriptItem2.Value = scriptItem1.Value;
                        else
                            xml2.Root.AddItem(scriptItem1);
                    }

                }
                // paste saved connection strings
                FastReport.Utils.XmlItem dictionary1 = xml1.Root.FindItem("Dictionary");
                FastReport.Utils.XmlItem dictionary2 = xml2.Root.FindItem("Dictionary");
                if (dictionary1 != null && dictionary2 != null)
                {
                    for (int i = 0; i < dictionary1.Items.Count; i++)
                    {
                        FastReport.Utils.XmlItem item1 = dictionary1.Items[i];
                        string connectionString = item1.GetProp("ConnectionString");
                        if (!String.IsNullOrEmpty(connectionString))
                        {
                            FastReport.Utils.XmlItem item2 = dictionary2.FindItem(item1.Name);
                            if (item2 != null)
                                item2.SetProp("ConnectionString", connectionString);
                        }
                    }
                }

                // save prepared xml
                using (MemoryStream secondXmlStream = new MemoryStream())
                {
                    xml2.Save(secondXmlStream);
                    secondXmlStream.Position = 0;
                    byte[] buff = new byte[secondXmlStream.Length];
                    secondXmlStream.Read(buff, 0, buff.Length);
                    xmlString = Encoding.UTF8.GetString(buff);
                }
            }
            return xmlString;
        }

        // save report
        private void SetReportTemplate(HttpContext context)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
            string guid = context.Request.Params["putReport"];
            SetUpWebReport(guid, context);
            if (WebUtils.SetupResponse(webReport, context))
            {

                if ((webReport != null))
                {
                    string reportString = GetPOSTReport(context);
                    try
                    {
                        // paste restricted back in report before save
                        webReport.Report.LoadFromString(PasteRestricted(webReport, reportString));

                        SaveDesignedReportEventArgs e = new SaveDesignedReportEventArgs();
                        e.Stream = new MemoryStream();
                        webReport.Report.Save(e.Stream);
                        e.Stream.Position = 0;
                        webReport.OnSaveDesignedReport(e);

                        if (!String.IsNullOrEmpty(webReport.DesignerSaveCallBack))
                        {
                            string report = webReport.Report.SaveToString();

                            string reportFileName = String.Concat(webReport.ReportGuid, ".frx");

                            UriBuilder uri = new UriBuilder();
                            uri.Scheme = context.Request.Url.Scheme;
                            uri.Host = context.Request.Url.Host;
                            if (!webReport.CloudEnvironmet)
                                uri.Port = context.Request.Url.Port;
                            uri.Path = webReport.ResolveUrl(webReport.DesignerSaveCallBack);
                            string queryToAppend = String.Format(
                                "reportID={0}&reportUUID={1}", webReport.ID != null ? webReport.ID : "", reportFileName);
                            if (uri.Query != null && uri.Query.Length > 1)
                                uri.Query = uri.Query.Substring(1) + "&" + queryToAppend;
                            else
                                uri.Query = queryToAppend;
                            string callBackURL = uri.ToString();

                            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;

                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(callBackURL);

                            if (request != null)
                            {
                                // set up the custom headers
                                if (webReport.RequestHeaders != null)
                                    request.Headers = webReport.RequestHeaders;

                                WebUtils.CopyCookies(request, context);

                                // if save report in reports folder
                                if (!String.IsNullOrEmpty(webReport.DesignerSavePath))
                                {
                                    string savePath = context.Request.MapPath(webReport.DesignerSavePath);
                                    if (Directory.Exists(savePath))
                                    {
                                        File.WriteAllText(Path.Combine(savePath, reportFileName), report, Encoding.UTF8);
                                    }
                                    else
                                    {
                                        context.Response.Write("DesignerSavePath does not exists");
                                        context.Response.StatusCode = 404;
                                    }

                                    request.Method = "GET";
                                }
                                else
                                // send report directly in POST
                                {
                                    request.Method = "POST";
                                    request.ContentType = "text/xml";
                                    byte[] postData = Encoding.UTF8.GetBytes(report);
                                    request.ContentLength = postData.Length;
                                    Stream reqStream = request.GetRequestStream();
                                    reqStream.Write(postData, 0, postData.Length);
                                    postData = null;
                                    reqStream.Close();
                                }
                                // Request call-back
                                //HttpWebResponse resp;
                                try
                                {
                                    request.BeginGetResponse(new AsyncCallback(FinishWebRequest), request);
                                    // resp.AsyncWaitHandle.WaitOne();

                                    //resp = request.GetResponse() as HttpWebResponse;
                                    context.Response.StatusCode = 202;
                                    //context.Response.Write(resp.StatusDescription);

                                }
                                catch (WebException err)
                                {
                                    context.Response.StatusCode = 500;
                                    if (webReport.Debug)
                                        using (Stream data = err.Response.GetResponseStream())
                                        using (StreamReader reader = new StreamReader(data))
                                        {
                                            string text = reader.ReadToEnd();
                                            if (!String.IsNullOrEmpty(text))
                                            {
                                                int startExceptionText = text.IndexOf("<!--");
                                                int endExceptionText = text.LastIndexOf("-->");
                                                if (startExceptionText != -1)
                                                    text = text.Substring(startExceptionText + 6, endExceptionText - startExceptionText - 6);
                                                context.Response.Write(text);

                                                context.Response.StatusCode = (int)(err.Response as HttpWebResponse).StatusCode;
                                            }
                                        }
                                    else
                                        context.Response.Write(err.Message);
                                }

                            }
                            request = null;
                        }
                    }
                    catch (Exception e)
                    {
                        if (webReport.Debug)
                            context.Response.Write(e.Message);
                        context.Response.StatusCode = 500;
                    }
                }
                else
                    context.Response.StatusCode = 404;
            }
            Finalize(context);
        }

        private void FinishWebRequest(IAsyncResult result)
        {
            HttpWebResponse response = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;
        }

        // send report to the designer
        private void SendReportTemplate(HttpContext context)
        {
            string guid = context.Request.Params["getReport"];
            SetUpWebReport(guid, context);
            if (WebUtils.SetupResponse(webReport, context))
            {
                if (webReport != null)
                {
                    string reportString = webReport.Report.SaveToString();
                    string report = CutRestricted(webReport, reportString);

                    if (report.IndexOf("inherited") != -1)
                    {
                        List<string> reportInheritance = new List<string>();
                        string baseReport = report;

                        while (!String.IsNullOrEmpty(baseReport))
                        {
                            reportInheritance.Add(baseReport);
                            using (MemoryStream xmlStream = new MemoryStream())
                            {
                                WebUtils.Write(xmlStream, baseReport);
                                xmlStream.Position = 0;
                                using (FastReport.Utils.XmlDocument xml = new Utils.XmlDocument())
                                {
                                    xml.Load(xmlStream);
                                    string baseReportFile = xml.Root.GetProp("BaseReport");
                                    //context.Request.MapPath(baseReportFile, webReport.Report.FileName, true);
                                    string fileName = Path.GetFullPath(Path.GetDirectoryName(Report.FileName) + Path.DirectorySeparatorChar + baseReportFile);
                                    if (File.Exists(fileName))
                                    {
                                        baseReport = File.ReadAllText(fileName, Encoding.UTF8);
                                    }
                                    else
                                        baseReport = String.Empty;
                                }
                            }
                        }
                        StringBuilder responseBuilder = new StringBuilder();
                        responseBuilder.Append("{\"reports\":[");
                        for (int i = reportInheritance.Count - 1; i >= 0; i--)
                        {
                            string s = reportInheritance[i];
                            responseBuilder.Append("\"");
                            responseBuilder.Append(s.Replace("\r\n", "").Replace("\"", "\\\""));
                            if (i > 0)
                                responseBuilder.Append("\",");
                            else
                                responseBuilder.Append("\"");
                        }
                        responseBuilder.Append("]}");
                        context.Response.Write(responseBuilder.ToString());
                    }
                    else
                        context.Response.Write(report);
                }
                else
                    context.Response.StatusCode = 404;
            }
            Finalize(context);
        }

        // preview for Designer
        private void MakeReportPreview(HttpContext context)
        {
            string guid = context.Request.Params["makePreview"];
            SetUpWebReport(guid, context);
            if (WebUtils.SetupResponse(webReport, context))
            {
                if (webReport != null)
                {
                    string receivedReportString = GetPOSTReport(context);
                    try
                    {
                        WebReport previewReport = new WebReport();
                        previewReport.ID = webReport.ID + "-preview";
                        previewReport.Report = webReport.Report;
                        previewReport.Prop.Assign(webReport.Prop);
                        //previewReport.LocalizationFile = webReport.LocalizationFile;
                        previewReport.Width = System.Web.UI.WebControls.Unit.Pixel(880);
                        previewReport.Height = System.Web.UI.WebControls.Unit.Pixel(770);
                        previewReport.Toolbar.EnableFit = true;
                        previewReport.Layers = true;
                        string reportString = PasteRestricted(webReport, receivedReportString);
                        previewReport.Report.ReportResourceString = reportString;
                        previewReport.ReportFile = String.Empty;
                        previewReport.ReportResourceString = reportString;
                        previewReport.Prepare();
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<script src=\"").Append(GetResourceTemplateUrl(context, "fr_util.js")).AppendLine("\" type=\"text/javascript\"></script>");
                        HtmlTextWriter writer = new HtmlTextWriter(new StringWriter(sb, System.Globalization.CultureInfo.InvariantCulture));
                        previewReport.ShowZoomButton = false;
                        previewReport.PreviewMode = true;
                        previewReport.DesignReport = false;
                        previewReport.Page = null;
                        previewReport.RenderControl(writer);
                        string responseText = WebReportGlobals.ScriptsAsString() + previewReport.Toolbar.GetCss() + sb.ToString();
                        context.Response.Write(responseText);
                    }
                    catch (Exception e)
                    {
                        if (webReport.Debug)
                            context.Response.Write(e.Message);
                        context.Response.StatusCode = 500;
                    }
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
            }
            Finalize(context);
        }

        private string GetPOSTReport(HttpContext context)
        {
            string requestString = "";
            using (TextReader textReader = new StreamReader(context.Request.InputStream))
                requestString = textReader.ReadToEnd();

            string xmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            StringBuilder result = new StringBuilder(xmlHeader.Length + requestString.Length + 100);
            result.Append(xmlHeader);
            result.Append(requestString.
                    Replace("&gt;", ">").
                    Replace("&lt;", "<").
                    Replace("&quot;", "\"").
                    Replace("&amp;#10;", "&#10;").
                    Replace("&amp;#13;", "&#13;").
                    Replace("&amp;#xA;", "&#10;").
                    Replace("&amp;#xD;", "&#13;").
                    Replace("&amp;quot;", "&quot;").
                    Replace("&amp;amp;", "&").
                    Replace("&amp;lt;", "&lt;").
                    Replace("&amp;gt;", "&gt;"));
            return result.ToString();
        }

        private void SendPreviewObjectResponse(HttpContext context)
        {
            string uuid = context.Request.Params["previewobject"];
            SetUpWebReport(uuid, context);
            if (WebUtils.SetupResponse(webReport, context))
            {

                if (!NeedExport(context) && !NeedPrint(context))
                    SendReport(context);

                cache.PutObject(uuid, webReport);
            }
            Finalize(context);
        }

        // On-line Designer
        private void SendDesigner(HttpContext context, string uuid)
        {
            if (WebUtils.SetupResponse(webReport, context))
            {
                StringBuilder sb = new StringBuilder();
                context.Response.AddHeader("Content-Type", "html/text");
                try
                {
                    string designerPath = WebUtils.GetAppRoot(context, webReport.DesignerPath);
                    string designerLocale = String.IsNullOrEmpty(webReport.DesignerLocale) ? "" : "&lang=" + webReport.DesignerLocale;
                    sb.Append(String.Format("<iframe src=\"{0}?uuid={1}{2}{3}\" style=\"border:none;\" width=\"{4}\" height=\"{5}\" allowfullscreen=\"true\" webkitallowfullscreen=\"true\" mozallowfullscreen=\"true\" oallowfullscreen=\"true\" msallowfullscreen=\"true\">",
                        designerPath, //0
                        uuid, //1
                        WebUtils.GetARRAffinity(), //2
                        designerLocale, //3
                        webReport.Width.ToString(), //4 
                        webReport.Height.ToString() //5
                        ));
                    sb.Append("<p style=\"color:red\">ERROR: Browser does not support IFRAME!</p>");
                    sb.AppendLine("</iframe>");

                    // add resize here
                    if (webReport.Height == System.Web.UI.WebControls.Unit.Percentage(100))
                        sb.Append(GetFitScript(uuid));
                }
                catch (Exception e)
                {
                    log.AddError(e);
                }

                if (log.Text.Length > 0)
                {
                    context.Response.Write(log.Text);
                    log.Clear();
                }

                SetContainer(context, Properties.ControlID);
                context.Response.Write(sb.ToString());
            }
        }

        private string GetFitScript(string ID)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script>");
            sb.AppendLine("(function() {");
            sb.AppendLine(String.Format("var div = document.querySelector('#{0}'),", ID));
            sb.AppendLine("iframe,");
            sb.AppendLine("rect,");
            sb.AppendLine("e = document.documentElement,");
            sb.AppendLine("g = document.getElementsByTagName('body')[0],");
            //sb.AppendLine("x = window.innerWidth || e.clientWidth || g.clientWidth,");
            sb.AppendLine("y = window.innerHeight|| e.clientHeight|| g.clientHeight;");
            sb.AppendLine("if (div) {");
            sb.AppendLine("iframe = div.querySelector('iframe');");
            sb.AppendLine("if (iframe) {");
            sb.AppendLine("rect = iframe.getBoundingClientRect();");
            //sb.AppendLine("iframe.setAttribute('width', x - rect.left);");
            sb.AppendLine("iframe.setAttribute('height', y - rect.top - 11);");
            sb.AppendLine("}}}());");
            sb.AppendLine("</script>");
            return sb.ToString();
        }

        private void MakeDesignerConfig(HttpContext context)
        {
            context.Response.AddHeader("Content-Type", "application/json");
            string uuid = context.Request.Params["getDesignerConfig"];
            SetUpWebReport(uuid, context);
            if (WebUtils.SetupResponse(webReport, context))
            {
                if (webReport != null)
                {
                    context.Response.Write(webReport.DesignerConfig);
                }
            }
            Finalize(context);
        }

        private void MakeConnectionTypes(HttpContext context)
        {
            context.Response.AddHeader("Content-Type", "application/json");
            string uuid = context.Request.Params["getConnectionTypes"];
            SetUpWebReport(uuid, context);
            if (WebUtils.SetupResponse(webReport, context))
            {

                List<string> names = new List<string>();
                List<ObjectInfo> objects = new List<ObjectInfo>();
                RegisteredObjects.Objects.EnumItems(objects);

                foreach (ObjectInfo info in objects)
                    if (info.Object != null && info.Object.IsSubclassOf(typeof(Data.DataConnectionBase)))
                        names.Add("\"" + info.Object.FullName + "\":\"" + Res.TryGetBuiltin(info.Text) + "\"");

                context.Response.Write("{" + String.Join(",", names.ToArray()) + "}");
            }
            Finalize(context);
        }

        private void MakeConnectionTables(HttpContext context)
        {
            string uuid = context.Request.Params["getConnectionTables"];
            SetUpWebReport(uuid, context);
            if (WebUtils.SetupResponse(webReport, context))
            {

                string connectionType = context.Request.Params["connectionType"];
                string connectionString = context.Request.Params["connectionString"];

                List<ObjectInfo> objects = new List<ObjectInfo>();
                RegisteredObjects.Objects.EnumItems(objects);
                Type connType = null;

                foreach (ObjectInfo info in objects)
                    if (info.Object != null &&
                        info.Object.IsSubclassOf(typeof(DataConnectionBase)) &&
                        info.Object.FullName == connectionType)
                    {
                        connType = info.Object;
                        break;
                    }

                if (connType != null)
                {
                    try
                    {
                        using (DataConnectionBase conn = (DataConnectionBase)Activator.CreateInstance(connType))
                        using (FRWriter writer = new FRWriter())
                        {
                            conn.ConnectionString = connectionString;
                            conn.CreateAllTables(true);

                            foreach (TableDataSource c in conn.Tables)
                                c.Enabled = true;

                            writer.SaveChildren = true;
                            writer.WriteHeader = false;
                            writer.Write(conn);
                            writer.Save(context.Response.OutputStream);
                            context.Response.ContentType = "application/xml";
                        }
                    }
                    catch (Exception ex)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.TrySkipIisCustomErrors = true;
                        context.Response.ContentType = "text/plain";
                        context.Response.Write(ex.ToString());
                    }
                }
                else
                {

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.TrySkipIisCustomErrors = true;
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("Connection type not found");
                }
            }
            Finalize(context);
        }

        private void GetConnectionStringProperties(HttpContext context)
        {
            string uuid = context.Request.Params["getConnectionStringProperties"];
            SetUpWebReport(uuid, context);
            if (WebUtils.SetupResponse(webReport, context))
            {

                string connectionType = context.Request.Params["connectionType"];
                string connectionString = context.Request.Params["connectionString"];

                List<ObjectInfo> objects = new List<ObjectInfo>();
                RegisteredObjects.Objects.EnumItems(objects);
                Type connType = null;

                foreach (ObjectInfo info in objects)
                    if (info.Object != null &&
                        info.Object.IsSubclassOf(typeof(DataConnectionBase)) &&
                        info.Object.FullName == connectionType)
                    {
                        connType = info.Object;
                        break;
                    }

                if (connType == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.TrySkipIisCustomErrors = true;
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("Connection type not found");
                    Finalize(context);
                    return;
                }

                StringBuilder data = new StringBuilder();

                // this piece of code mimics functionality of PropertyGrid: finds available properties
                try
                {
                    using (DataConnectionBase conn = (DataConnectionBase)Activator.CreateInstance(connType))
                    {
                        conn.ConnectionString = connectionString;
                        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(conn);

                        foreach (PropertyDescriptor pd in props)
                        {
                            if (!pd.IsBrowsable || pd.IsReadOnly)
                                continue;

                            if (pd.Name == "Name" ||
                                pd.Name == "ConnectionString" ||
                                pd.Name == "ConnectionStringExpression" ||
                                pd.Name == "LoginPrompt" ||
                                pd.Name == "CommandTimeout" ||
                                pd.Name == "Alias" ||
                                pd.Name == "Description" ||
                                pd.Name == "Restrictions")
                                continue;

                            object value = null;

                            try
                            {
                                object owner = conn;
                                if (conn is ICustomTypeDescriptor)
                                    owner = ((ICustomTypeDescriptor)conn).GetPropertyOwner(pd);
                                value = pd.GetValue(owner);
                            }
                            catch { }

                            data.Append("{");
                            data.Append("\"name\":\"" + WebUtils.JavaScriptStringEncode(pd.Name) + "\",");
                            data.Append("\"displayName\":\"" + WebUtils.JavaScriptStringEncode(pd.DisplayName) + "\",");
                            data.Append("\"description\":\"" + WebUtils.JavaScriptStringEncode(pd.Description) + "\",");
                            data.Append("\"value\":\"" + WebUtils.JavaScriptStringEncode(value == null ? "" : value.ToString()) + "\",");
                            data.Append("\"propertyType\":\"" + WebUtils.JavaScriptStringEncode(pd.PropertyType.FullName) + "\"");
                            data.Append("},");
                        }
                    }
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.TrySkipIisCustomErrors = true;
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(ex.ToString());
                    Finalize(context);
                    return;
                }

                context.Response.ContentType = "application/json";
                context.Response.Write("{\"properties\":[" + data.ToString().TrimEnd(',') + "]}");
            }
            Finalize(context);
        }

        private void MakeConnectionString(HttpContext context)
        {
            string uuid = context.Request.Params["makeConnectionString"];
            SetUpWebReport(uuid, context);
            if (WebUtils.SetupResponse(webReport, context))
            {

                string connectionType = context.Request.Params["connectionType"];

                List<ObjectInfo> objects = new List<ObjectInfo>();
                RegisteredObjects.Objects.EnumItems(objects);
                Type connType = null;

                foreach (ObjectInfo info in objects)
                    if (info.Object != null &&
                        info.Object.IsSubclassOf(typeof(DataConnectionBase)) &&
                        info.Object.FullName == connectionType)
                    {
                        connType = info.Object;
                        break;
                    }

                if (connType == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.TrySkipIisCustomErrors = true;
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("Connection type not found");
                    Finalize(context);
                    return;
                }

                try
                {
                    using (DataConnectionBase conn = (DataConnectionBase)Activator.CreateInstance(connType))
                    {
                        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(conn);

                        foreach (PropertyDescriptor pd in props)
                        {
                            if (!pd.IsBrowsable || pd.IsReadOnly)
                                continue;

                            if (pd.Name == "Name" ||
                                pd.Name == "ConnectionString" ||
                                pd.Name == "ConnectionStringExpression" ||
                                pd.Name == "LoginPrompt" ||
                                pd.Name == "CommandTimeout" ||
                                pd.Name == "Alias" ||
                                pd.Name == "Description" ||
                                pd.Name == "Restrictions")
                                continue;

                            try
                            {
                                string propertyValue = context.Request.Form[pd.Name];
                                TypeConverter typeConverter = TypeDescriptor.GetConverter(pd.PropertyType);
                                object value = typeConverter.ConvertFromString(propertyValue);

                                object owner = conn;
                                if (conn is ICustomTypeDescriptor)
                                    owner = ((ICustomTypeDescriptor)conn).GetPropertyOwner(pd);
                                pd.SetValue(owner, value);
                            }
                            catch (Exception ex)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                context.Response.TrySkipIisCustomErrors = true;
                                context.Response.ContentType = "text/plain";
                                context.Response.Write(ex.ToString());
                                Finalize(context);
                                return;
                            }
                        }

                        context.Response.ContentType = "application/json";
                        context.Response.Write("{\"connectionString\":\"" + WebUtils.JavaScriptStringEncode(conn.ConnectionString) + "\"}");
                        Finalize(context);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.TrySkipIisCustomErrors = true;
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(ex.ToString());
                    Finalize(context);
                    return;
                }
            }
            Finalize(context);
        }

        private void MakeFunctionsList(HttpContext context)
        {
            context.Response.AddHeader("Content-Type", "application/xml");
            string uuid = context.Request.Params["getFunctions"];
            SetUpWebReport(uuid, context);
            if (WebUtils.SetupResponse(webReport, context))
            {
                FastReport.Utils.XmlDocument xml = new FastReport.Utils.XmlDocument();
                xml.AutoIndent = true;
                List<ObjectInfo> list = new List<ObjectInfo>();
                RegisteredObjects.Objects.EnumItems(list);
                ObjectInfo rootFunctions = null;
                foreach (ObjectInfo item in list)
                {
                    if (item.Name == "Functions")
                    {
                        rootFunctions = item;
                        break;
                    }
                }
                xml.Root.Name = "ReportFunctions";
#if !FRCORE
                if (rootFunctions != null)
                    RegisteredObjects.CreateFunctionsTree(Report, rootFunctions, xml.Root);
#endif
                using (MemoryStream stream = new MemoryStream())
                {
                    xml.Save(stream);
                    stream.Position = 0;
                    byte[] buff = new byte[stream.Length];
                    stream.Read(buff, 0, buff.Length);
                    string answer = Encoding.UTF8.GetString(buff);
                    context.Response.Write(answer);
                }
            }
            Finalize(context);
        }
    }
}
 