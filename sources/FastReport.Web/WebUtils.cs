using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;

namespace FastReport.Web
{
    /// <summary>
    /// 
    /// </summary>
    public static class WebUtils
    {
        /// <summary>
        /// Contain the filename of httphandler
        /// </summary>
        public const string HandlerFileName = "FastReport.Export.axd";
        internal const string PicsPrefix = "frximg";
        internal const string PrintPrefix = "frxprint";
        internal const string ReportPrefix = "frxreport";
        internal const string StartupScriptName = "FrxStartup";
        internal const string ConstID = "ID";
        internal const string DefaultCreator = "FastReport";
        internal const string DefaultProducer = "FastReport .NET";
        internal const string HiddenIDSuffix = "FRID";

        /// <summary>
        /// Determines whether the path is an absolute physical path.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><b>true</b> if the path is absolute physical path.</returns>
        public static bool IsAbsolutePhysicalPath(string path)
        {
            if ((path == null) || (path.Length < 3))
            {
                return false;
            }
            return Path.IsPathRooted(path); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string ToHtmlString(Unit unit)
        {
            switch (unit.Type)
            {
                case UnitType.Pixel:
                    return unit.Value + "px";
                case UnitType.Percentage:
                    return unit.Value + "%";
                default:
                    return unit.Value.ToString();
            }
        }

        /// <summary>
        /// Returns the HTML color representation;
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string HTMLColor(Color color)
        {
            if (color == Color.Transparent)
                return "transparent";
#if DOTNET_4
            return ColorTranslator.ToHtml(color);
#else
            return String.Join(String.Empty, new String[] { 
                "#",
                color.R.ToString("X2"),
                color.G.ToString("X2"),
                color.B.ToString("X2") 
            });
#endif
        }

        private static bool CheckNewHandlerInLocationTags(XmlNode element)
        {
            foreach (XmlNode locationNode in element.SelectNodes("location"))
                if (CheckNewHandler(locationNode))
                    return true;

            return false;
        }

        private static bool CheckNewHandler(XmlNode element)
        {
            bool found = false;
            XmlNode node = element.SelectSingleNode("system.webServer");
            if (node != null)
            {
                XmlNode node2 = node.SelectSingleNode("handlers");
                if (node2 != null)
                {
                    XmlNode node3 = node2.SelectSingleNode(String.Format("add[@path=\"{0}\"]", HandlerFileName));
                    found = (node3 != null);
                }
            }
            return found;
        }

        private static bool CheckOldHandler(XmlNode element)
        {
            bool found = false;
            XmlNode node = element.SelectSingleNode("system.web");
            if (node != null)
            {
                XmlNode node2 = node.SelectSingleNode("httpHandlers");
                if (node2 != null)
                {
                    XmlNode node3 = node2.SelectSingleNode(String.Format("add[@path=\"{0}\"]", HandlerFileName));
                    found = (node3 != null);
                }
            }
            return found;
        }

        /// <summary>
        /// Check http handlers in web.config
        /// </summary>
        /// <returns></returns>
        public static bool CheckHandlers()
        {
            string webConfigFile = HttpContext.Current.Server.MapPath("~/web.config");
            if (!File.Exists(webConfigFile))
                webConfigFile = HttpContext.Current.Server.MapPath("~/Web.config");
            bool found1 = false;
            bool found2 = false;
            bool found3 = false;
            if (File.Exists(webConfigFile))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(webConfigFile);
                XmlElement element = xml.DocumentElement;
                found1 = CheckOldHandler(element);
                found2 = CheckNewHandler(element);
                found3 = CheckNewHandlerInLocationTags(element);
            }
            return found1 || found2 || found3;
        }

        /// <summary>
        /// Add http handlers in web.config
        /// </summary>
        public static void AddHandlers(string webConfigFile)
        {
            if (File.Exists(webConfigFile))
            {
                bool modified = false;
                XmlDocument xml = new XmlDocument();
                xml.Load(webConfigFile);
                XmlElement element = xml.DocumentElement;

                // integrated style
                string s = "system.webServer";
                XmlNode node = element.SelectSingleNode(s);
                if (node == null)
                {
                    node = xml.CreateElement(s);
                    element.AppendChild(node);
                }

                XmlNode node2 = node.SelectSingleNode("validation[@validateIntegratedModeConfiguration=\"false\"]");
                if (node2 == null)
                {
                    node2 = xml.CreateElement("validation");
                    XmlAttribute a = xml.CreateAttribute("validateIntegratedModeConfiguration");
                    a.Value = "false";
                    node2.Attributes.Append(a);
                    node.AppendChild(node2);
                    modified = true;
                }

                s = "handlers";
                node2 = node.SelectSingleNode(s);
                if (node2 == null)
                {
                    node2 = xml.CreateElement(s);
                    node.AppendChild(node2);
                }
                XmlNode node3 = node2.SelectSingleNode(String.Format("add[@path=\"{0}\"]", HandlerFileName));
                if (node3 == null)
                {
                    node3 = xml.CreateElement("add");
                    XmlAttribute a = xml.CreateAttribute("name");
                    a.Value = "FastReportHandler";
                    node3.Attributes.Append(a);
                    a = xml.CreateAttribute("path");
                    a.Value = HandlerFileName;
                    node3.Attributes.Append(a);
                    a = xml.CreateAttribute("verb");
                    a.Value = "*";
                    node3.Attributes.Append(a);
                    a = xml.CreateAttribute("type");
                    a.Value = "FastReport.Web.Handlers.WebExport";
                    node3.Attributes.Append(a);
                    node2.AppendChild(node3);
                    modified = true;
                }

                // standard style
                s = "system.web";
                node = element.SelectSingleNode(s);
                if (node == null)
                {
                    node = xml.CreateElement(s);
                    element.AppendChild(node);
                }
                s = "httpHandlers";
                node2 = node.SelectSingleNode(s);
                if (node2 == null)
                {
                    node2 = xml.CreateElement(s);
                    node.AppendChild(node2);
                }
                node3 = node2.SelectSingleNode(String.Format("add[@path=\"{0}\"]", HandlerFileName));
                if (node3 == null)
                {
                    node3 = xml.CreateElement("add");
                    XmlAttribute a = xml.CreateAttribute("path");
                    a.Value = HandlerFileName;
                    node3.Attributes.Append(a);
                    a = xml.CreateAttribute("verb");
                    a.Value = "*";
                    node3.Attributes.Append(a);
                    a = xml.CreateAttribute("type");
                    a.Value = "FastReport.Web.Handlers.WebExport";
                    node3.Attributes.Append(a);
                    node2.AppendChild(node3);
                    modified = true;
                }
                // save config
                if (modified)
                    xml.Save(webConfigFile);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void CheckHandlersRuntime()
        {
            if (!CheckHandlers())
                throw new Exception(GetHandlerError());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetHandlerError()
        {
            StringBuilder e = new StringBuilder();
            e.AppendLine("FastReport handler not found or its extension has been changed . Please check your web.config:");
            e.AppendLine("IIS6");
            e.AppendLine("<system.web>");
            e.AppendLine("...");
            e.AppendLine("  <httpHandlers>");
            e.Append("    <add path=\"").Append(HandlerFileName).AppendLine("\" verb=\"*\" type=\"FastReport.Web.Handlers.WebExport\"/>");
            e.AppendLine("      ....");
            e.AppendLine("  </httpHandlers>");
            e.AppendLine("</system.web>");
            e.AppendLine("IIS7");
            e.AppendLine("<configuration>");
            e.AppendLine("...");
            e.AppendLine("  <system.webServer>");
            e.AppendLine("    <validation validateIntegratedModeConfiguration=\"false\"/>");
            e.AppendLine("...");
            e.AppendLine("    <handlers>");
            e.AppendLine("    ...");
            e.AppendLine("      <remove name=\"FastReportHandler\"/>");
            e.Append("      <add name=\"FastReportHandler\" path=\"").Append(HandlerFileName).AppendLine("\" verb=\"*\" type=\"FastReport.Web.Handlers.WebExport\" />");
            e.AppendLine("    </handlers>");
            e.AppendLine("  </system.webServer>");
            e.AppendLine("</configuration>");
            return e.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReverseString(string str)
        {
            StringBuilder result = new StringBuilder(str.Length);
            int i, j;
            if (!String.IsNullOrEmpty(str))
                for (j = 0, i = str.Length - 1; i >= 0; i--, j++)
                    result.Append(str[i]);
            return result.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetGUID(HttpContext context, string id)
        {
            string result = String.Empty;
            if (HttpContext.Current != null)
                result = context.Request[String.Concat(id, "$", WebUtils.HiddenIDSuffix)];

            if (String.IsNullOrEmpty(result))
                result = Guid.NewGuid().ToString().Replace("-", "");
            else
                result = WebUtils.ReverseString(result);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetGUID()
        {
            Guid guid = Guid.Empty;
            while (Guid.Empty == guid)
            {
                guid = Guid.NewGuid();
            }
            string guidStr = String.Concat("fr",
                Convert.ToBase64String(guid.ToByteArray()).Substring(0, 22).Replace("/", "_").Replace("+", "-"));
            return guidStr;
        }

        /// <summary>
        /// IE8 and older browsers detection.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsIE8(HttpContext context)
        {
            if (context != null)
                return (context.Request.Browser.Browser == "InternetExplorer" || 
                        context.Request.Browser.Browser == "IE")
                        && (context.Request.Browser.Version == "8.0" || 
                            context.Request.Browser.Version == "7.0" || 
                            context.Request.Browser.Version == "6.0");
            else
                return false;
        }

        /// <summary>
        /// Add NoCache haders in Context.Reponse
        /// </summary>
        /// <param name="context"></param>
        public static void AddNoCacheHeaders(HttpContext context)
        {
            context.Response.AddHeader("Expires", "May, 3 Jul 1997 05:00:00 GMT");
            context.Response.AddHeader("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0, max-age=0");
            context.Response.AddHeader("Pragma", "no-cache");
        }

        /*/// <summary>
        /// Setup the context. 
        /// </summary>
        /// <param name="context"></param>
        //public static void SetContext(HttpContext context)
        //{
        //    SetAzureCookies(context);
        //    AddNoCacheHeaders(context);
        //} */

        /// <summary>
        /// Converts Color in HTML format with transparency.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string RGBAColor(Color color)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberGroupSeparator = String.Empty;
            provider.NumberDecimalSeparator = ".";
            return String.Format("rgba({0}, {1}, {2}, {3})",
                color.R.ToString(""),
                color.G.ToString(""),
                color.B.ToString(""),
                Math.Round((float)color.A / 255).ToString(provider)
            );
        }

        internal static void Write(Stream stream, string value)
        {
            byte[] buf = Encoding.UTF8.GetBytes(value);
            stream.Write(buf, 0, buf.Length);
        }

        internal static void ResponseChunked(HttpResponse httpResponse, byte[] p)
        {
            int chunkSize = 2048;
            int position = 0;
            while (position < p.Length && httpResponse.IsClientConnected)
            {
                if (chunkSize > p.Length - position)
                    chunkSize = p.Length - position;
                httpResponse.OutputStream.Write(p, position, chunkSize);
                position += chunkSize;
                httpResponse.Flush();
            }
        }

        internal static string GetAppRoot(HttpContext context, string path)
        {
            if (path.IndexOf("://") != -1)
                return path;
            string s = String.Concat(context.Request.ApplicationPath == "/" ? "" : context.Request.ApplicationPath, path.IndexOf("/") == 0 ? "" : "/", path.Replace("~/", ""));
            return s;
        }

        internal static object GetSalt()
        {
            return string.Concat("&s=", new Random(DateTime.Now.Millisecond).Next(10000).ToString());
        }

        internal static string GetBasePath(HttpContext httpContext)
        {
            if (httpContext != null)
            {
                string s = httpContext.Request.ApplicationPath;
                if (s.EndsWith("/"))
                    return s;
                else
                    return s + "/";
            }
            else
                return string.Empty;
        }

        internal static bool SetupResponse(WebReport webReport, HttpContext context)
        {
            if (webReport != null)
            {
                CustomAuthEventArgs authArgs = new CustomAuthEventArgs();
                authArgs.Context = context;
                webReport.OnCustomAuth(authArgs);
                if (!authArgs.AuthPassed)
                {
                    context.Response.StatusCode = 401;
                    return false;
                }

                if (webReport.ResponseHeaders != null)
                {
                    foreach (string key in webReport.ResponseHeaders.AllKeys)
                        context.Response.Headers.Add(key, webReport.ResponseHeaders.Get(key));
                }
            }
            WebUtils.SetAzureCookies(context);
            WebUtils.AddNoCacheHeaders(context);
            return true;
        }

        internal static void SetAzureCookies(HttpContext context)
        {
            string ARRAffinity = GetWebsiteInstanceId();
            if (!String.IsNullOrEmpty(ARRAffinity))
            {
                HttpCookie cookie = new HttpCookie("ARRAffinity", ARRAffinity);
                cookie.Expires = DateTime.Now.AddMinutes(30);
                context.Response.Cookies.Add(cookie);
            }            
        }

        internal static void CopyCookies(System.Net.WebRequest request, HttpContext context)
        {
            CookieContainer cookieContainer = new CookieContainer();            
            UriBuilder uri = new UriBuilder();
            uri.Scheme = context.Request.Url.Scheme;
            uri.Host = context.Request.Url.Host;
            string ARRAffinity = GetWebsiteInstanceId();

            if (!String.IsNullOrEmpty(ARRAffinity))
                cookieContainer.Add(uri.Uri, new Cookie("ARRAffinity", ARRAffinity));

            foreach (string key in context.Request.Cookies.AllKeys)
                cookieContainer.Add(uri.Uri, new Cookie(key, context.Server.UrlEncode(context.Request.Cookies[key].Value)));

            HttpWebRequest req = (HttpWebRequest)request;
            req.CookieContainer = cookieContainer;
        }

        internal static string GetARRAffinity()
        {
            string id = GetWebsiteInstanceId();
            if (!String.IsNullOrEmpty(id))
                return String.Concat("&ARRAffinity=", id);
            else
                return String.Empty;
        }

        internal static string GetWebsiteInstanceId()
        {
            return Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");
        }

        internal static string JavaScriptStringEncode(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            bool needEncode = false;

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                if (c >= 0 && c <= 31 || c == 34 || c == 39 || c == 60 || c == 62 || c == 92)
                {
                    needEncode = true;
                    break;
                }
            }

            if (!needEncode)
                return s;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                if (c >= 0 && c <= 7 || c == 11 || c >= 14 && c <= 31 || c == 39 || c == 60 || c == 62)
                {
                    sb.AppendFormat("\\u{0:x4}", (int)c);
                }
                else
                {
                    switch ((int)c)
                    {
                        case 8:
                            sb.Append("\\b");
                            break;
                        case 9:
                            sb.Append("\\t");
                            break;
                        case 10:
                            sb.Append("\\n");
                            break;
                        case 12:
                            sb.Append("\\f");
                            break;
                        case 13:
                            sb.Append("\\r");
                            break;
                        case 34:
                            sb.Append("\\\"");
                            break;
                        case 92:
                            sb.Append("\\\\");
                            break;
                        default:
                            sb.Append(c);
                            break;
                    }
                }
            }

            return sb.ToString();
        }

        internal static bool IsPng(byte[] image)
        {
            byte[] pngHeader = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
            bool isPng = true;
            for (int i = 0; i < 8; i++)
                if (image[i] != pngHeader[i])
                {
                    isPng = false;
                    break;
                }
            return isPng;
        }
    }    
}
