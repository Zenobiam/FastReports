using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FastReport.Forms;
using FastReport.Export;
using FastReport.Utils;

namespace FastReport.Cloud.StorageClient.Dropbox
{
    /// <summary>
    /// Represents form of Dropbox storage client.
    /// </summary>
    public partial class DropboxStorageClientForm : BaseDialogForm
    {
        #region Fields

        private DropboxStorageClient dropboxClient;
        private List<ExportBase> exports;
        private Report report;

        #endregion // Fields

        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DropboxStorageClientForm"/> class.
        /// </summary>
        /// <param name="accessToken">The Dropbox access token.</param>
        /// <param name="report">The report template.</param>
        public DropboxStorageClientForm(string accessToken, Report report)
        {
            dropboxClient = new DropboxStorageClient(accessToken);
            this.report = report;
            InitializeComponent();
            Localize();
            Init();
            Scale();

            // apply Right to Left layout
            if (Config.RightToLeft)
            {
                RightToLeft = RightToLeft.Yes;

                // move components to other side
                labelFileType.Left = pgFile.Width - labelFileType.Left - labelFileType.Width;
                labelFileType.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                cbFileType.Left = pgFile.Width - cbFileType.Left - cbFileType.Width;
                cbFileType.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                buttonSettings.Left = pgFile.Width - buttonSettings.Left - buttonSettings.Width;
                buttonSettings.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                labelServer.Left = pgProxy.Width - labelServer.Left - labelServer.Width;
                labelServer.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                tbServer.Left = pgProxy.Width - tbServer.Left - tbServer.Width;
                tbServer.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                labelColon.Left = pgProxy.Width - labelColon.Left - labelColon.Width;
                labelColon.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                tbPort.Left = pgProxy.Width - tbPort.Left - tbPort.Width;
                tbPort.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                labelUsername.Left = pgProxy.Width - labelUsername.Left - labelUsername.Width;
                labelUsername.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                tbUsername.Left = pgProxy.Width - tbUsername.Left - tbUsername.Width;
                tbUsername.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                labelPassword.Left = pgProxy.Width - labelPassword.Left - labelPassword.Width;
                labelPassword.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                tbPassword.Left = pgProxy.Width - tbPassword.Left - tbPassword.Width;
                tbPassword.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

                // move parent components from rigth to left
                btnOk.Left = ClientSize.Width - btnOk.Left - btnOk.Width;
                btnCancel.Left = ClientSize.Width - btnCancel.Left - btnCancel.Width;
            }
        }

        #endregion // Constructors

        #region Private Methods

        private void Init()
        {
            exports = new List<ExportBase>();
            List<ObjectInfo> list = new List<ObjectInfo>();
            RegisteredObjects.Objects.EnumItems(list);
            cbFileType.Items.Add(Res.Get("Preview,SaveNative"));
            exports.Add(null);
            foreach (ObjectInfo info in list)
            {
                if (info.Object != null && info.Object.IsSubclassOf(typeof(ExportBase)))
                {
                    cbFileType.Items.Add(Res.TryGet(info.Text));
                    exports.Add(Activator.CreateInstance(info.Object) as ExportBase);
                }
            }
            cbFileType.SelectedIndex = 0;

            XmlItem xi = Config.Root.FindItem("DropboxCloud").FindItem("StorageSettings");
            string accessToken = xi.GetProp("AccessToken");
            if (!String.IsNullOrEmpty(accessToken))
            {
                dropboxClient.AccessToken = accessToken;
            }

            tbServer.Text = xi.GetProp("Server");
            tbPort.Text = xi.GetProp("Port");
            tbUsername.Text = xi.GetProp("Username");
            tbPassword.Text = xi.GetProp("Password");
        }

        private bool IsNumeric(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                try
                {
                    Convert.ToInt32(str);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        private bool Done()
        {
            if (!String.IsNullOrEmpty(tbPort.Text))
            {
                if (!IsNumeric(tbPort.Text))
                {
                    FRMessageBox.Error(Res.Get("Cloud,Dropbox,PortError"));
                    tbPort.Focus();
                    return false;
                }
            }

            XmlItem xi = Config.Root.FindItem("DropboxCloud").FindItem("StorageSettings");

            xi.SetProp("AccessToken", dropboxClient.AccessToken);

            xi.SetProp("Server", tbServer.Text);
            xi.SetProp("Port", tbPort.Text);
            xi.SetProp("Username", tbUsername.Text);
            xi.SetProp("Password", tbPassword.Text);

            return true;
        }

        #endregion // Private Methods

        #region Public Methods

        /// <inheritdoc/>
        public override void Localize()
        {
            base.Localize();

            MyRes res = new MyRes("Cloud,Dropbox");
            this.Text = res.Get("");
            pgFile.Text = res.Get("File");
            pgProxy.Text = res.Get("Proxy");
            labelFileType.Text = res.Get("FileType");
            buttonSettings.Text = res.Get("Settings");
            labelServer.Text = res.Get("Server");
            labelUsername.Text = res.Get("Username");
            labelPassword.Text = res.Get("Password");
        }

        #endregion // Public Methods

        #region Events Handlers

        private void cbFileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonSettings.Enabled = cbFileType.SelectedIndex != 0;
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            ExportBase export = exports[cbFileType.SelectedIndex];
            export.SetReport(report);
            export.ShowDialog();
        }

        private void DropboxStorageClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                if (!Done())
                {
                    e.Cancel = true;
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            if (String.IsNullOrEmpty(dropboxClient.AccessToken))
            {
                if (!String.IsNullOrEmpty(tbServer.Text))
                {
                    int port = 0;
                    if (!IsNumeric(tbPort.Text))
                    {
                        FRMessageBox.Error(Res.Get("Cloud,Dropbox,PortError"));
                        tbPort.Focus();
                    }
                    else
                    {
                        port = Convert.ToInt32(tbPort.Text);
                    }
                    dropboxClient.ProxySettings = new CloudProxySettings(ProxyType.Http, tbServer.Text, port, tbUsername.Text, tbPassword.Text);
                }

                ApplicationInfoForm appInfoDialog = new ApplicationInfoForm();
                appInfoDialog.ShowDialog();
                dropboxClient.AccessToken = appInfoDialog.AccessToken;
            }

            bool saved = false;
            try
            {
                dropboxClient.SaveReport(report, exports[cbFileType.SelectedIndex]);
                saved = true;
            } catch{ }

            if(!saved)
            {
                ApplicationInfoForm appInfoDialog = new ApplicationInfoForm();
                appInfoDialog.ShowDialog();
                dropboxClient.AccessToken = appInfoDialog.AccessToken;
                try
                {
                    dropboxClient.SaveReport(report, exports[cbFileType.SelectedIndex]);
                }
                catch (CloudStorageException ex)
                {
                    MessageBox.Show(ex.Message, Res.Get("Messages,Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        #endregion // Events Handlers
    }
}