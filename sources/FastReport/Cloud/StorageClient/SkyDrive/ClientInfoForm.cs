using System;
using System.Windows.Forms;
using FastReport.Forms;
using FastReport.Utils;

namespace FastReport.Cloud.StorageClient.SkyDrive
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
        }

        #endregion // Public Methods

        #region Events Handlers

        private void btnOk_Click(object sender, EventArgs e)
        {
            id = tbClientId.Text;
            this.Close();
        }

        #endregion // Events Handlers
    }
}