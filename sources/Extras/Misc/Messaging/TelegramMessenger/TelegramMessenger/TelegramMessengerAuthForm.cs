using System;
using System.Windows.Forms;

namespace FastReport.Messaging
{
    public partial class TelegramMessengerAuthForm : Form
    {
        TelegramMessenger telegram;
        Report report;
        public TelegramMessengerAuthForm(TelegramMessenger telegram, Report report)
        {
            this.telegram = telegram;
            this.report = report;
            InitializeComponent();
        }

        private async void okBtn_Click_Async(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(codeTextBox.Text))
                throw new Exception("Enter code, please!");
            try
            {
                Hide();
                Cursor = Cursors.WaitCursor;
                await telegram.AuthorizeAsync(codeTextBox.Text, passTextBox.Text);
                TelegramMessengerForm form = new TelegramMessengerForm(report);
                //form.Closed += (s, args) => this.Close();
                form.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
