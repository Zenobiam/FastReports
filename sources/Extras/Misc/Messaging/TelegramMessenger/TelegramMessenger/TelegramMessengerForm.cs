using FastReport.Utils;
using System.IO;
using System.Windows.Forms;

namespace FastReport.Messaging
{
    public partial class TelegramMessengerForm : MessengerForm
    {
        #region Fields

        private TelegramMessenger telegram;
        string recipientPhone;
        string senderPhone;
        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmppMessengerForm"/> class.
        /// </summary>
        public TelegramMessengerForm() : base()
        {
            telegram = new TelegramMessenger();
            InitializeComponent();
            Localize();
            Init();
        }
        public TelegramMessengerForm(Report report) : base(report)
        {
            telegram = new TelegramMessenger();
            InitializeComponent();
            Localize();
            Init();
        }

        public TelegramMessengerForm(TelegramMessenger telegram, Report report) : base(report)
        {
            this.telegram = telegram;
            InitializeComponent();
            Localize();
            Init();
        }

        #endregion //Constructors

        #region Protected Methods

        /// <inheritdoc/>
        protected override void Init()
        {
            if (telegram == null)
                telegram = new TelegramMessenger();
            base.Init();
            XmlItem xi = Config.Root.FindItem("TelegramMessenger").FindItem("MessengerSettings");
            senderPhone = xi.GetProp("SenderPhone");
            recipientPhone = xi.GetProp("RecipientPhone");

            if (!string.IsNullOrEmpty(senderPhone) && !string.IsNullOrEmpty(recipientPhone))
            {
                telegram.SenderPhone = senderPhone;
                telegram.RecipientPhone = recipientPhone;
                if(recipientPhoneTextBox != null)
                recipientPhoneTextBox.Text = recipientPhone;
            }

            tbProxyServer.Text = xi.GetProp("ProxyServer");
            tbProxyPort.Text = xi.GetProp("ProxyPort");
        }

        /// <summary>
        /// Finishes the form work.
        /// </summary>
        /// <returns>Returns true if work has been successfully finished, otherwise false.</returns>
        protected override bool Done()
        {
            if(base.Done())
            if (!string.IsNullOrEmpty(recipientPhone))
            {
                XmlItem xi = Config.Root.FindItem("TelegramMessenger").FindItem("MessengerSettings");
                xi.SetProp("RecipientPhone", recipientPhone);
                if (!TelegramUtils.IsPhoneNumber(recipientPhone))
                {
                    FRMessageBox.Error(Res.Get("Messaging,MessengerForm,PortError"));
                    recipientPhoneTextBox.Focus();
                    return false;
                }
                return true;
            }
            return false;
        }

        public override void Localize()
        {
            base.Localize();

            MyRes res = new MyRes("Messaging,Telegram");
            this.Text = res.Get("");
            if(recipientPhoneLbl != null)
            recipientPhoneLbl.Text = res.Get("RecipientPhone") + ":";
            if (signOutBtn != null)
                signOutBtn.Text = res.Get("SignOut");
        }
        #endregion // Events Handlers
        /// <inheritdoc/>
        private void btnOk_Click_1(object sender, System.EventArgs e)
        {
            if(Done())
            {
                this.Hide();
                Cursor = Cursors.WaitCursor;
                if (!telegram.Connected)
                    telegram.Connect();
                var export = Exports[cbFileType.SelectedIndex];
                telegram.RecipientPhone = recipientPhone;
                
                telegram.SendReport(Report, export);
                Close();
            }
        }

        private void signOutBtn_Click_1(object sender, System.EventArgs e)
        {
            if (File.Exists("session.dat"))
            {
                File.Delete("session.dat");
            }
            telegram.Dispose();
            this.Hide();
            TelegramMessengerLogInForm logInForm = new TelegramMessengerLogInForm(Report);
            logInForm.ShowDialog();
            this.Close();
        }

        private void recipientPhoneTextBox_TextChanged(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(recipientPhoneTextBox.Text))
                recipientPhone = recipientPhoneTextBox.Text.Replace(" ", "").Replace("+", "");
        }
    }
}
