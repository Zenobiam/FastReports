using System;
using System.Diagnostics;
using System.Windows.Forms;
using FastReport.Forms;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Cloud.StorageClient.GoogleDrive
{
    /// <summary>
    /// Represents the Client Info diabolg form.
    /// </summary>
    public partial class ClientInfoForm : BaseDialogForm
    {
#region Fields

#pragma warning disable FR0001 // Field names must be longer than 2 characters.
        private string id;
#pragma warning restore FR0001 // Field names must be longer than 2 characters.
        private string secret;
        private string auth;
        private bool isBtnOkClicked = false;
        GoogleDriveStorageClient client;

#endregion // Fields

#region Properties

        /// <summary>
        /// Gets the client ID.
        /// </summary>
        public string Id
        {
            get { return id; }
        }

        /// <summary>
        /// Gets the client secret.
        /// </summary>
        public string Secret
        {
            get { return secret; }
        }

        /// <summary>
        /// Gets the client Auth key.
        /// </summary>
        public string AuthKey
        {
            get { return auth; }
        }

#endregion // Properties

#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientInfoForm"/> class.
        /// </summary>
        public ClientInfoForm()
        {
            this.id = "";
            this.secret = "";
            InitializeComponent();
            Localize();

            client = new GoogleDriveStorageClient();
            Scale();

            // apply Right to Left layout
            if (Config.RightToLeft)
            {
                RightToLeft = RightToLeft.Yes;

                // move components to other side
                labelClientId.Left = ClientSize.Width - labelClientId.Left - labelClientId.Width;
                labelClientId.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                tbClientId.Left = ClientSize.Width - tbClientId.Left - tbClientId.Width;
                tbClientId.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                labelClientSecret.Left = ClientSize.Width - labelClientSecret.Left - labelClientSecret.Width;
                labelClientSecret.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                tbClientSecret.Left = ClientSize.Width - tbClientSecret.Left - tbClientSecret.Width;
                tbClientSecret.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

                // move parent components from rigth to left
                btnOk.Left = ClientSize.Width - btnOk.Left - btnOk.Width;
                btnCancel.Left = ClientSize.Width - btnCancel.Left - btnCancel.Width;
            }
        }

#endregion // Constructors

#region Public Methods

        /// <inheritdoc/>
        public override void Localize()
        {
            base.Localize();

            MyRes res = new MyRes("Cloud,SkyDrive");
            this.Text = res.Get("ClientInfoDialog");
            labelClientId.Text = res.Get("ClientId");
            labelClientSecret.Text = res.Get("ClientSecret");
        }

#endregion // Public Methods

#region Events Handlers

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbClientKey.Text))
            {
                id = tbClientId.Text;
                secret = tbClientSecret.Text;
                client.ClientInfo = new SkyDrive.ClientInfo("", Id, Secret);
                OpenUrl(client.GetAuthorizationUrl());
                if (!isBtnOkClicked)
                    Height += DpiHelper.ConvertUnits(30);
                tbClientId.Enabled = false;
                tbClientSecret.Enabled = false;
                tbClientKey.Visible = true;
                labelKey.Visible = true;
                isBtnOkClicked = true;
            }
            else
            {
                client.AuthCode = tbClientKey.Text;
                string token = client.GetAccessToken();
                client.IsUserAuthorized = true;
                XmlItem xi = Config.Root.FindItem("GoogleDriveCloud").FindItem("StorageSettings");
                xi.SetProp("ClientId", client.ClientInfo.Id);
                xi.SetProp("ClientSecret", client.ClientInfo.Secret);
                xi.SetProp("AuthCode", client.AuthCode);
                xi.SetProp("IsUserAuthorized", client.IsUserAuthorized.ToString());
                xi.SetProp("AccessToken", client.AccessToken);
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

#endregion // Events Handlers

#region private methods
        private void OpenUrl(string url)
        {
            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.FileName = url;
            System.Diagnostics.Process.Start(psi);
        }
#endregion
    }
}