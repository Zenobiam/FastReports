using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using FastReport.Export;
using FastReport.Cloud.OAuth;
using FastReport.Cloud.StorageClient.SkyDrive;
using FastReport.Utils;

namespace FastReport.Cloud.StorageClient.Box
{
    /// <summary>
    /// Box cloud storage client.
    /// </summary>
    public class BoxStorageClient : CloudStorageClient
    {
        #region Fields

        private ClientInfo clientInfo;
        private string authCode;
        private string accessToken;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets or sets the client info.
        /// </summary>
        public ClientInfo ClientInfo
        {
            get { return clientInfo; }
            set { clientInfo = value; }
        }

        /// <summary>
        /// Gets or sets the authorization code.
        /// </summary>
        public string AuthCode
        {
            get { return authCode; }
            set { authCode = value; }
        }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        public string AccessToken
        {
            get { return accessToken; }
            set { accessToken = value; }
        }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxStorageClient"/> class.
        /// </summary>
        public BoxStorageClient() : base()
        {
            this.clientInfo = new ClientInfo("", "", "");
            authCode = "";
            accessToken = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxStorageClient"/> class.
        /// </summary>
        /// <param name="clientInfo">The storage client info.</param>
        public BoxStorageClient(ClientInfo clientInfo) : base()
        {
            this.clientInfo = clientInfo;
            authCode = "";
            accessToken = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxStorageClient"/> class.
        /// </summary>
        /// <param name="clientId">Client ID.</param>
        /// <param name="clientSecret">Client Secret.</param>
        public BoxStorageClient(string clientId, string clientSecret) : base()
        {
            this.clientInfo = new ClientInfo("", clientId, clientSecret);
            authCode = "";
            accessToken = "";
        }

        #endregion // Constructors

        #region Private Methods

        private byte[] BuildGetAccessTokenRequestContent()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("code", authCode);
            data.Add("client_id", clientInfo.Id);
            data.Add("client_secret", clientInfo.Secret);
            //data.Add("redirect_uri", @"urn:ietf:wg:oauth:2.0:oob");
            data.Add("grant_type", "authorization_code");
            return Encoding.UTF8.GetBytes(HttpUtils.UrlDataEncode(data));
        }

        private string GetUploadUri(string fileid = "")
        {
            if (String.IsNullOrEmpty(fileid))
                return @"https://upload.box.com/api/2.0/files/content";
            else
                return String.Format(@"https://upload.box.com/api/2.0/files/{0}/content", fileid);
        }

        private void UploadNewFile(MemoryStream ms, string fileid = "")
        {
            System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
            string uri = GetUploadUri(fileid);
            WebRequest request = WebRequest.Create(uri);
            request.Method = HttpMethod.Post;
            RequestUtils.SetProxySettings(request, ProxySettings);

            request.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken));
            request.ContentType = "multipart/form-data; boundary=\"foo_bar_baz\"";
            List<byte> content = new List<byte>();

            StringBuilder sb = new StringBuilder("--foo_bar_baz\r\n");
            sb.Append(String.Format("Content-Disposition: form-data; filename=\"{0}\"; name=\"filename\"\r\n", Filename));
            sb.Append("Content-Type: application/octet-stream\r\n");
            sb.Append("\r\n");
            content.AddRange(Encoding.UTF8.GetBytes(sb.ToString()));

            int msLength = Convert.ToInt32(ms.Length);
            byte[] msBuffer = new byte[msLength];
            ms.Read(msBuffer, 0, msLength);
            content.AddRange(msBuffer);

            sb = new StringBuilder("\r\n--foo_bar_baz\r\n");
            sb.Append("Content-Disposition: form-data; name=\"folder_id\"");
            sb.Append("\r\n\r\n0\r\n");
            sb.Append("--foo_bar_baz--");
            content.AddRange(Encoding.UTF8.GetBytes(sb.ToString()));

            int length = content.Count;
            byte[] buffer = new byte[length];
            buffer = content.ToArray();
            request.ContentLength = buffer.Length;
            using (Stream rs = request.GetRequestStream())
            {
                rs.Write(buffer, 0, buffer.Length);
            }

            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
        }

        private void UploadExistFile(MemoryStream ms)
        {
            UploadNewFile(ms, GetFileID());
        }

        private string GetFileID()
        {
            WebRequest request = WebRequest.Create(@"https://api.box.com/2.0/folders/0/items/");
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken));

            WebResponse response = request.GetResponse();

            string JSONString = String.Empty;

            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                JSONString += reader.ReadToEnd();
            }

            int indexFilename = JSONString.IndexOf(Filename);

            int firstBracketIndex = 0;
            int i = 1;
            bool file_version_bracketWas = false;
            while(true)
            {
                if (JSONString[indexFilename - i].ToString() == "{")
                {
                    if(file_version_bracketWas)
                    {
                        firstBracketIndex = indexFilename - i;
                        break;
                    }
                    else
                    {
                        file_version_bracketWas = true;
                    }
                }
                i++;
            }

            string fileJson = JSONString.Substring(firstBracketIndex, indexFilename - firstBracketIndex);
            int idIndex = fileJson.IndexOf("id");

            string idString = fileJson.Substring(idIndex + 4, fileJson.IndexOf(",", idIndex) - idIndex - 4);

            //string str = fileJson.Substring(idIndex, fileJson.Length - fileJson)

            return idString.Replace("\"", "");
        }

        #endregion // Private Methods

        #region Protected Methods

        /// <inheritdoc/>
        protected override void SaveMemoryStream(MemoryStream ms)
        {
            bool isExist = false;
            try
            {
                UploadNewFile(ms);
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("409"))
                    isExist = true;
                else
                    throw new CloudStorageException(ex.Message, ex);
            }

            try
            {
                if(isExist)
                    UploadExistFile(ms);
            }
            catch(Exception ex)
            {
                throw new CloudStorageException(ex.Message, ex);
            }
        }

        #endregion // Protected Methods

        #region Public Methods

        /// <summary>
        /// Gets the authorization URL.
        /// </summary>
        /// <returns>The authorization URL stirng.</returns>
        public string GetAuthorizationUrl()
        {
            return String.Format(@"https://www.box.com/api/oauth2/authorize?response_type=code&redirect_uri={0}&client_id={1}",
                "https://www.box.com", clientInfo.Id);
        }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <returns>The access token string.</returns>
        public string GetAccessToken()
        {
            System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
            string uri = String.Format(@"?client_id={0}&client_secret={1}&grant_type=authorization_code&code={2}",
                clientInfo.Id, clientInfo.Secret, authCode);
            WebRequest request = WebRequest.Create(@"https://www.box.com/api/oauth2/token");
            request.Method = HttpMethod.Post;
            RequestUtils.SetProxySettings(request, ProxySettings);

            request.ContentType = "application/x-www-form-urlencoded";
            byte[] content = BuildGetAccessTokenRequestContent();
            request.ContentLength = content.Length;
            using (Stream rs = request.GetRequestStream())
            {
                rs.Write(content, 0, content.Length);
            }

            WebResponse response = request.GetResponse();
            accessToken = Parser.ParseGoogleDriveToken(response.GetResponseStream());

            return accessToken;
        }

        #endregion // Public Methods
    }
}
