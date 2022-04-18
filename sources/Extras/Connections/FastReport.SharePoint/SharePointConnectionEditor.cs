
using FastReport.Data.ConnectionEditors;
using FastReport.Utils;

namespace FastReport.Data
{
    public partial class SharePointConnectionEditor : ConnectionEditorBase
    {
        private string FConnectionString;

        public SharePointConnectionEditor()
        {
            InitializeComponent();
            Localize();
        }

        private void Localize()
        {
            MyRes res = new MyRes("ConnectionEditors,Common");

            gbServer.Text = res.Get("ServerLogon");
            lblServer.Text = res.Get("Server");
            lblUserName.Text = res.Get("UserName");
            lblPassword.Text = res.Get("Password");
        }

        protected override void SetConnectionString(string value)
        {
            FConnectionString = value;

            SharePointConnectionStringBuilder builder = new SharePointConnectionStringBuilder(value);

            tbServer.Text = builder.Url;
            tbUserName.Text = builder.UserName;
            tbPassword.Text = builder.Password;
            cbOffice.Checked = builder.Office365;
        }

        protected override string GetConnectionString()
        {
            if (FConnectionString == null) FConnectionString = "";
            SharePointConnectionStringBuilder builder = new SharePointConnectionStringBuilder(FConnectionString);

            builder.Url = tbServer.Text;
            builder.UserName = tbUserName.Text;
            builder.Password = tbPassword.Text;
            builder.Office365 = cbOffice.Checked;

            return builder.ToString();
        }
    }
}
