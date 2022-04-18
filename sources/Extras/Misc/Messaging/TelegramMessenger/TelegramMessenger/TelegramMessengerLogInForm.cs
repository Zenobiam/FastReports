using FastReport.Utils;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FastReport.Messaging
{
    public partial class TelegramMessengerLogInForm : Form
    {
        private TelegramMessenger telegram;
        private Report report;
        private string senderPhone;

        #region Constructors

        public TelegramMessengerLogInForm()
        {
            InitializeComponent();
            senderPhone = senderPhoneTextBox.Text;
            telegram = new TelegramMessenger();
        }

        public TelegramMessengerLogInForm(Report report)
        {
            InitializeComponent();
            this.report = report;
            telegram = new TelegramMessenger();
            senderPhone = senderPhoneTextBox.Text;
        }

        #endregion Constructors //Constructors

        protected void Init()
        {
            if (telegram == null)
                telegram = new TelegramMessenger();

            XmlItem xi = Config.Root.FindItem("TelegramMessenger").FindItem("MessengerSettings");
            senderPhone = xi.GetProp("SenderPhone");

            if (!string.IsNullOrEmpty(senderPhone))
            {
                senderPhoneTextBox.Text = senderPhone;
                telegram.SenderPhone = senderPhone;
            }
        }

        private async void okBtn_ClickAsync(object sender, EventArgs e)
        {
            if(Done())
            {
                telegram.SenderPhone = senderPhone;
                await telegram.ConnectAsync();
                if (telegram.Connected)
                {
                    if (!telegram.Authorized)
                    {
                        Hide();
                        Cursor = Cursors.WaitCursor;
                        await telegram.RequestCodeAsync();
                        TelegramMessengerAuthForm authForm = new TelegramMessengerAuthForm(telegram, report);
                        authForm.ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        Hide();
                        TelegramMessengerForm form = new TelegramMessengerForm(telegram, report);
                        form.Show();
                        this.Close();
                    }
                }
                else
                    throw new Exception("Can't connect to Telegram servers!");
            }
        }

        /// <summary>
        /// Finishes the form work.
        /// </summary>
        /// <returns>Returns true if work has been successfully finished, otherwise false.</returns>
        protected bool Done()
        {
            if (!String.IsNullOrEmpty(senderPhoneTextBox.Text))
            {
                XmlItem xi = Config.Root.FindItem("TelegramMessenger").FindItem("MessengerSettings");
                xi.SetProp("SenderPhone", senderPhone);
                if (!TelegramUtils.IsPhoneNumber(senderPhone))
                {
                    FRMessageBox.Error("Wrong phone number format!");
                    senderPhoneTextBox.Focus();
                    return false;
                }
                return true;
            }
            return false;
        }

        private void senderPhoneTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(senderPhoneTextBox.Text))
                senderPhone = senderPhoneTextBox.Text.Replace(" ", "").Replace("+", "");
        }

        private void TelegramMessengerInfoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                if (!Done())
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
