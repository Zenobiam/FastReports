using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace FastReport.SharePoint
{
    internal class SimpleRESTClient
    {
        #region StaticRegion
        private static Uri CreateRequest(string siteUrl)
        {
            int index1 = siteUrl.IndexOf(@"://");
            if (index1 >= 0) index1 += 3; else index1 = 0;
            if(index1 == 0)
            {
                siteUrl = @"http://" + siteUrl;
            }
            return new Uri(siteUrl);
        }

        private static Uri CreateRequestLists(Uri site)
        {
            return new Uri(site, @"_api/web/lists");
        }

        private static Uri CreateRequestList(Uri site, string title)
        {
            return new Uri(site, String.Format(@"_api/web/lists/getbytitle('{0}')",title));
        }

        private static Uri CreateRequestFields(Uri site, string title)
        {
            return new Uri(site, String.Format(@"_api/web/lists/getbytitle('{0}')/Fields", title));
        }

        private static Uri CreateRequestItems(Uri site, string title)
        {
            return new Uri(site, String.Format(@"_api/web/lists/getbytitle('{0}')/items", title));
        }

        private static XmlDocument MakeRequest(WebRequest request)
        {
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(response.GetResponseStream());
                    return (xmlDoc);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static void testConnectionBasic(WebRequest request)
        {
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                //return (xmlDoc);
            }
        }

        public static string[] getLists(WebRequest request)
        {
            XmlDocument doc = MakeRequest(request);
            XmlNodeList entries = doc.GetElementsByTagName("entry");
            string[] strs = new string[entries.Count];
            int i = 0;
            foreach (XmlNode entry in entries)
            {
                XmlElement node;
                node = entry["id"]; if (node == null) continue;
                string url = node.InnerText;
                node = entry["content"]; if (node == null) continue;
                node = node["m:properties"]; if (node == null) continue;
                node = node["d:Title"]; if (node == null) continue;
                string title = node.InnerText;
                strs[i] = title;
                i++;
            }
            return strs;
        }
        public static void fillFields(DataTable table, WebRequest request)
        {
            XmlDocument doc = MakeRequest(request);
            XmlNodeList entries = doc.GetElementsByTagName("entry");
            //DataColumn[] columns = new DataColumn[entries.Count];
            int i = 0;
            foreach (XmlNode entry in entries)
            {
                XmlElement node;
                //temp = entry["id"]; if (temp == null) continue;
                //string url = temp.InnerText;
                node = entry["content"]; if (node == null) continue;
                node = node["m:properties"]; if (node == null) continue;
                XmlElement tmp = node["d:EntityPropertyName"]; if (tmp == null) continue;
                string columnid = tmp.InnerText;
                DataColumn column = new DataColumn(columnid);
                tmp = node["d:Title"]; if (tmp == null) continue;
                bool flag = true;
                int num = -1;
                while(flag)
                {
                    flag = false;
                    num++;
                    string caption = RemoveSpecialCharacters(tmp.InnerText);
                    if (num > 0)
                        caption = caption + num.ToString();
                    foreach (DataColumn c in table.Columns)
                    {
                        if(c.Caption == caption)
                        {
                            flag = true;
                        }
                    }
                    if(flag == false)
                    {
                        column.Caption = caption;
                    }
                }
                column.AllowDBNull = true;
                if (table.Columns.IndexOf(column) < 0)
                    table.Columns.Add(column);
                //columns[i] = new DataColumn(titled);
                //strs[i] = titled;
                i++;
            }
        }

        internal static void fillTable(DataTable table,WebRequest request)
        {
            XmlDocument doc = MakeRequest(request);
            XmlNodeList entries = doc.GetElementsByTagName("entry");
            int i = 0;
            foreach (XmlNode entry in entries)
            {
                DataRow row = table.NewRow();
                XmlElement node;
                node = entry["content"]; if (node == null) continue;
                node = node["m:properties"]; if (node == null) continue;
                foreach (DataColumn column in table.Columns)
                {
                    row[column] = null;
                    XmlElement tmp = node["d:" + column.ColumnName]; if (tmp == null) continue;
                    row[column] = tmp.InnerText;
                }
                //string titled = node.InnerText;

                table.Rows.Add(row);
                i++;
            }
        }

        private static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (Char.IsSeparator(c) || Char.IsPunctuation(c)) continue;
                sb.Append(c);
            }
            return sb.ToString();
        }
        #endregion StaticRegion

        string url;
        string user;
        string pass;
        bool office;
        bool needUpdate;
        string rtFa;
        string FedAuth;

        internal void testConnection()
        {
            if (needUpdate)
                updateCookie();
            
            testConnectionBasic(CreateRequest(CreateRequestLists(CreateRequest(url))));
        }

        internal WebRequest CreateRequest(Uri site)
        {
            
            HttpWebRequest request = WebRequest.Create(site) as HttpWebRequest;
            request.Timeout = 5000;
            if (office)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(site,new Cookie("rtFa", rtFa));
                request.CookieContainer.Add(site,new Cookie("FedAuth", FedAuth));
            }
            else
            {
                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
                {
                    if (user.IndexOf("@") > 0)
                    {
                        string[] strs = user.Split('@');
                        request.Credentials = new NetworkCredential(strs[0], pass, strs[1]);
                    }
                    else
                        request.Credentials = new NetworkCredential(user, pass);
                }
            }
            return request;
        }

        private void updateCookie()
        {
            if(office)
            {
                UTF8Encoding encoding = new UTF8Encoding();

                HttpWebRequest web = WebRequest.Create("https://login.microsoftonline.com/extSTS.srf") as HttpWebRequest;
                web.Method = "POST";
                web.ContentType = "text/xml";
                Uri uri = CreateRequest(url);
                using (Stream webStream = web.GetRequestStream())
                {
                    byte[] bytes = encoding.GetBytes(String.Format(Requests.request1, user, pass, uri.AbsoluteUri));
                    //web.ContentLength = bytes.Length;
                    webStream.Write(bytes, 0, bytes.Length);
                }
                string payload = null;
                using (WebResponse response = web.GetResponse())
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(response.GetResponseStream());
                    //xmlDoc.Save(@"c:\users\wital\downloads\test.xml");
                    XmlNodeList list = xmlDoc.GetElementsByTagName(@"wsse:BinarySecurityToken");
                    if (list.Count <= 0) return;

                    payload = list.Item(0).InnerText;
                }

                Uri forms = new Uri(uri, @"/_forms/default.aspx?wa=wsignin1.0");
                //Uri forms = new Uri(uri, @"/?wa=wsignin1.0&" + payload); 

                web = WebRequest.Create(forms) as HttpWebRequest;
                //web.Accept = "*/*";
                web.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0)";
                //web.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
                web.Method = "POST";
                web.ContentType = @"application/x-www-form-urlencoded";
                web.AllowAutoRedirect = false;

                //web.ContentLength = bytes.Length;
                using (Stream webStream = web.GetRequestStream())
                {
                    byte[] bytes = encoding.GetBytes(payload);
                    webStream.Write(bytes, 0, bytes.Length);
                }
                using (WebResponse response = web.GetResponse())
                    try
                    {
                        string sCookie = response.Headers[HttpResponseHeader.SetCookie];
                        int start_rtFa = sCookie.IndexOf("rtFa");
                        start_rtFa = sCookie.IndexOf("=", start_rtFa) + 1;
                        int end_rtFa = sCookie.IndexOf(";", start_rtFa);
                        rtFa = sCookie.Substring(start_rtFa, end_rtFa - start_rtFa);

                        int start_FedAuth = sCookie.IndexOf("FedAuth");
                        start_FedAuth = sCookie.IndexOf("=", start_FedAuth) + 1;
                        int end_FedAuth = sCookie.IndexOf(";", start_FedAuth);
                        FedAuth = sCookie.Substring(start_FedAuth, end_FedAuth - start_FedAuth);
                    }
                    catch
                    {
                        rtFa = "";
                        FedAuth = "";
                    }
                needUpdate = false;
            }
        }

        internal string[] getLists()
        {
            if (needUpdate)
                updateCookie();
                return getLists(CreateRequest(CreateRequestLists(CreateRequest(url))));
        }

        internal void check(string server, string userName, string password, bool office365)
        {
            if (this.url != server) { needUpdate = true; this.url = server; }
            if (this.user != userName) { needUpdate = true; this.user = userName; }
            if (this.pass != password) { needUpdate = true; this.pass = password; }
            if (this.office != office365) { needUpdate = true; this.office = office365; }
        }



        internal void fillTable(DataTable table)
        {
            if (needUpdate)
                updateCookie();
            fillTable(table, CreateRequest(CreateRequestItems(CreateRequest(url), table.TableName)));
        }

        internal void fillFields(DataTable table)
        {
            if (needUpdate)
                updateCookie();
            fillFields(table, CreateRequest(CreateRequestFields(CreateRequest(url), table.TableName)));
        }

        
    }
}
