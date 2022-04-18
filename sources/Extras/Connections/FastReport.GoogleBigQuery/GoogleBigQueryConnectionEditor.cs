using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FastReport.Data.ConnectionEditors;
using FastReport.Forms;
using FastReport.Utils;
using GoogleBigQuery;

namespace FastReport.Data
{
    public partial class GoogleBigQueryConnectionEditor : ConnectionEditorBase
    {
        private string FConnectionString;

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            using (AdvancedConnectionPropertiesForm form = new AdvancedConnectionPropertiesForm())
            {
                GbqConnectionStringBuilder builder = new GbqConnectionStringBuilder(ConnectionString);
                form.AdvancedProperties = builder;
                if (form.ShowDialog() == DialogResult.OK)
                    ConnectionString = form.AdvancedProperties.ToString();
            }
        }

        private void Localize()
        {
/*            MyRes res = new MyRes("ConnectionEditors,Common");
            gbDatabase.Text = res.Get("Database");
            lblDataFile.Text = res.Get("DatabaseFile");
            lblUserName.Text = res.Get("UserName");
            lblPassword.Text = res.Get("Password");
            btnAdvanced.Text = Res.Get("Buttons,Advanced");
            tbDatabase.Image = Res.GetImage(1);
 * */
        }

        protected override string GetConnectionString()
        {
            GbqConnectionStringBuilder builder = new GbqConnectionStringBuilder(FConnectionString);

            builder.ProjectID = tbGoogleProjectID.Text;
            builder.ClientID = tbClientID.Text;
            builder.ClientSecret = tbClientSecret.Text;

            return builder.ToString();
        }

        protected override void SetConnectionString(string value)
        {            
            FConnectionString = value;
            GbqConnectionStringBuilder builder = new GbqConnectionStringBuilder(value);
            try
            {
                tbGoogleProjectID.Text = builder.ProjectID;
            }
            catch
            {
                tbGoogleProjectID.Text = "";
            }
            try
            {
                tbClientID.Text = builder.ClientID;
            }
            catch
            {
                tbClientID.Text = "";
            }
            try
            {
                tbClientSecret.Text = builder.ClientSecret;
            }
            catch
            {
                tbClientSecret.Text = "";
            }
        }

        public GoogleBigQueryConnectionEditor()
        {
            InitializeComponent();
            Localize();
        }
    }
}
