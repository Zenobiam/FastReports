using System;
using System.Windows.Forms;
using FastReport.Data.ConnectionEditors;
using FastReport.Forms;
using Sap.Data.SQLAnywhere;
using FastReport.Utils;

namespace FastReport.SqlAnywhere
{
    public partial class SqlAnywhereConnectionEditor : ConnectionEditorBase
    {
        private string FConnectionString;
        public SqlAnywhereConnectionEditor()
        {
            InitializeComponent();
            Localize();
        }

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            using (AdvancedConnectionPropertiesForm form = new AdvancedConnectionPropertiesForm())
            {
                SAConnectionStringBuilder builder = new SAConnectionStringBuilder(ConnectionString);
                form.AdvancedProperties = builder;
                if (form.ShowDialog() == DialogResult.OK)
                    ConnectionString = form.AdvancedProperties.ToString();
            }
        }
        private void Localize()
        {
            MyRes res = new MyRes("ConnectionEditors,Common");

            gbServer.Text = res.Get("ServerLogon");
            lblServer.Text = res.Get("Server");
            lblUserName.Text = res.Get("UserName");
            lblPassword.Text = res.Get("Password");

            gbDatabase.Text = res.Get("Database");
            lblDatabase.Text = res.Get("DatabaseName");
            btnAdvanced.Text = Res.Get("Buttons,Advanced");
        }
        protected override string GetConnectionString()
        {
            SAConnectionStringBuilder builder = new SAConnectionStringBuilder(FConnectionString);

            builder.ServerName = tbServer.Text;
            builder.UserID = tbUserName.Text;
            builder.Password = tbPassword.Text;
            builder.DatabaseName = tbDatabase.Text;

            return builder.ToString();
        }
        protected override void SetConnectionString(string value)
        {
            FConnectionString = value;
            SAConnectionStringBuilder builder = new SAConnectionStringBuilder(value);

            tbServer.Text = builder.ServerName;
            tbUserName.Text = builder.UserID;
            tbPassword.Text = builder.Password;
            tbDatabase.Text = builder.DatabaseName;
        }
    }
}
