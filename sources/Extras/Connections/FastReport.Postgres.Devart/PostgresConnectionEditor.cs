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
using Devart.Data;
using Devart.Data.PostgreSql;

namespace FastReport.Data
{
  public partial class PostgresConnectionEditor : ConnectionEditorBase
  {
    private string FConnectionString;

    private void cbxDatabase_DropDown(object sender, EventArgs e)
    {
      cbxDatabase.Items.Clear();
      
      try
      {
        PgSqlConnectionStringBuilder builder = new PgSqlConnectionStringBuilder(ConnectionString);
        builder.Database = "template1";
        using (PgSqlConnection connection = new PgSqlConnection(builder.ToString()))
        {
          connection.Open();
          PgSqlDataReader dr = new PgSqlCommand("SELECT datname FROM pg_database WHERE datallowconn = 't'", connection).ExecuteReader();
          while (dr.Read())
          {
            cbxDatabase.Items.Add(dr["datname"]);
          }
        }
      }
      catch
      {
      }  
    }

    private void btnAdvanced_Click(object sender, EventArgs e)
    {
      using (AdvancedConnectionPropertiesForm form = new AdvancedConnectionPropertiesForm())
      {
        PgSqlConnectionStringBuilder builder = new PgSqlConnectionStringBuilder(ConnectionString);
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
      PgSqlConnectionStringBuilder builder = new PgSqlConnectionStringBuilder(FConnectionString);
      
      builder.Host = tbServer.Text;
      builder.UserId = tbUserName.Text;
      builder.Password = tbPassword.Text;
      builder.Database = cbxDatabase.Text;
      
      return builder.ToString();
    }

    protected override void SetConnectionString(string value)
    {
      FConnectionString = value;
      PgSqlConnectionStringBuilder builder = new PgSqlConnectionStringBuilder(value);
      
      tbServer.Text = builder.Host;
      tbUserName.Text = builder.UserId;
      tbPassword.Text = builder.Password;
      cbxDatabase.Text = builder.Database;
    }

    public PostgresConnectionEditor()
    {
      InitializeComponent();
      Localize();
    }
  }
}
