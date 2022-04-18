using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastReport.Forms;
using FastReport.Utils;

namespace FastReport.Cloud.StorageClient.GoogleDrive
{
    /// <summary>
    /// Represents form of the web browser.
    /// </summary>
    public partial class WebBrowserForm : BaseDialogForm
    {
        #region Fields

        private string url;
        private string authCode;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets obtained authorization code.
        /// </summary>
        public string AuthCode
        {
            get { return authCode; }
        }

        #endregion // Properties

        #region Constructors

        /// <inheritdoc/>
        public WebBrowserForm(string url)
        {
            InitializeComponent();
            this.url = url;
            authCode = "";
            wbBrowser.Navigated += new WebBrowserNavigatedEventHandler(wbBrowser_Navigated);
            Scale();

            // apply Right to Left layout
            if (Config.RightToLeft)
            {
                RightToLeft = RightToLeft.Yes;

                // move parent components from rigth to left
                btnOk.Left = ClientSize.Width - btnOk.Left - btnOk.Width;
                btnCancel.Left = ClientSize.Width - btnCancel.Left - btnCancel.Width;
            }
        }

        #endregion // Constructors

        #region Events Handlers

        private void WebBrowserForm_Shown(object sender, EventArgs e)
        {
            wbBrowser.Navigate(url);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            authCode = Regex.Split(Regex.Split(wbBrowser.DocumentText, "code=")[1], "<")[0];
        }

        private void wbBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (wbBrowser.DocumentText.Contains("code="))
            {
                authCode = Regex.Split(Regex.Split(wbBrowser.DocumentText, "code=")[1], "<")[0];
                this.Close();
            }
        }

        #endregion // Events Handlers
    }
}