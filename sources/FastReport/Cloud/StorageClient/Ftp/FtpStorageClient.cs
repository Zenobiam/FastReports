using System;
using System.IO;
using System.Net;
using FastReport.Export;
using FastReport.Utils;

namespace FastReport.Cloud.StorageClient.Ftp
{
    /// <summary>
    /// FTP storage client.
    /// </summary>
    public class FtpStorageClient : CloudStorageClient
    {
        #region Fields

        private string server;
        private string username;
        private string password;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets or sets the FTP server.
        /// </summary>
        public string Server
        {
            get { return server; }
            set { server = value; }
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpStorageClient"/> class.
        /// </summary>
        public FtpStorageClient() : base()
        {
            server = "";
            username = "";
            password = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpStorageClient"/> class.
        /// </summary>
        /// <param name="server">The FTP server.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public FtpStorageClient(string server, string username, string password) : base()
        {
            this.server = server;
            this.username = username;
            this.password = password;
        }

        #endregion // Contructors

        #region Protected Methods

        /// <inheritdoc/>
        protected override void SaveMemoryStream(MemoryStream ms)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                string uri = server.StartsWith("ftp://") ? server : "ftp://" + server;
                uri += "/" + Path.GetFileName(Filename);
                FtpWebRequest request = WebRequest.Create(uri) as FtpWebRequest;
                request.Method = WebRequestMethods.Ftp.UploadFile;
                RequestUtils.SetProxySettings(request, ProxySettings);
                request.Credentials = new NetworkCredential(username, password);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                int length = Convert.ToInt32(ms.Length);
                byte[] buffer = new byte[length];
                ms.Read(buffer, 0, length);
                request.ContentLength = buffer.Length;
                using (Stream rs = request.GetRequestStream())
                {
                    rs.Write(buffer, 0, length);
                }

                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
            }
            catch (WebException ex)
            {
                MyRes res = new MyRes("Cloud,Ftp");
                if (ex.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    throw new CloudStorageException(res.Get("ServerNotFoundError"), ex);
                }
                else if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    throw new CloudStorageException(res.Get("AccessDeniedError"), ex);
                }
                else
                {
                    throw new CloudStorageException(res.Get("ServerConnectionFiledError"), ex);
                }
            }
            catch (Exception ex)
            {
                throw new CloudStorageException(ex.Message, ex);
            }
        }

        #endregion // Protected Methods
    }
}
