using System.Windows.Forms;
using FastReport.Data.ConnectionEditors;
using FastReport.Forms;
using FastReport.Utils;
using Alachisoft.NosDB.Common;
using System;
using System.Data.Common;

namespace FastReport.Data
{
    public partial class NosDbConnectionEditor : ConnectionEditorBase
    {
        private string FConnectionString;
        private static string[] collectionsName;
         
        public static string[] CollectionNames
        {
            get { return collectionsName; }
        }

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            using (AdvancedConnectionPropertiesForm form = new AdvancedConnectionPropertiesForm())
            {
                DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
                builder.ConnectionString = ConnectionString;
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
            ConnectionStringBuilder builder = new ConnectionStringBuilder();
            if (!string.IsNullOrEmpty(FConnectionString))
                builder = new ConnectionStringBuilder(FConnectionString);
            builder.DataSource = new string[] { tbServer.Text };
            builder.UserId = tbUserName.Text;
            builder.Password = tbPassword.Text;
            builder.Database = tbDatabase.Text;
            collectionsName = tbCollections.Text.Split(new char[] { ',', ' ', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            return builder.ToString();
        }

        protected override void SetConnectionString(string value)
        {
            FConnectionString = value;
            if(!string.IsNullOrEmpty(value))
            {
                ConnectionStringBuilder builder = new ConnectionStringBuilder(value);

                tbServer.Text = builder.DataSource[0];
                tbUserName.Text = builder.UserId;
                tbPassword.Text = builder.Password;
                tbDatabase.Text = builder.Database;
            }
        }

        public NosDbConnectionEditor()
        {
            InitializeComponent();
            Localize();
        }
    }
}
