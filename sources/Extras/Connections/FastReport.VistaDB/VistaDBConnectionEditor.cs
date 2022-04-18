using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FastReport.Data.ConnectionEditors;
using FastReport.Forms;
using VistaDB.Provider;
using FastReport.Utils;

namespace FastReport.Data
{
  public partial class VistaDBConnectionEditor : ConnectionEditorBase
  {
    private string FConnectionString;

    private void tbDatabase_ButtonClick(object sender, EventArgs e)
    {
      using (OpenFileDialog dialog = new OpenFileDialog())
      {
        dialog.Filter = Res.Get("FileFilters,Vdb3File");
        if (dialog.ShowDialog() == DialogResult.OK)
          tbDatabase.Text = dialog.FileName;
      }
    }

    private void btnAdvanced_Click(object sender, EventArgs e)
    {
      using (AdvancedConnectionPropertiesForm form = new AdvancedConnectionPropertiesForm())
      {
        VistaDBConnectionStringBuilder builder = new VistaDBConnectionStringBuilder(ConnectionString);
        form.AdvancedProperties = builder;
        if (form.ShowDialog() == DialogResult.OK)
          ConnectionString = form.AdvancedProperties.ToString();
      }
    }

    private void Localize()
    {
      MyRes res = new MyRes("ConnectionEditors,Common");
      gbDatabase.Text = res.Get("Database");
      lblDataFile.Text = res.Get("DatabaseFile");
      lblPassword.Text = res.Get("Password");
      btnAdvanced.Text = Res.Get("Buttons,Advanced");
      tbDatabase.Image = Res.GetImage(1);
    }

    protected override string GetConnectionString()
    {
      VistaDBConnectionStringBuilder builder = new VistaDBConnectionStringBuilder(FConnectionString);
      
      builder.DataSource = tbDatabase.Text;
      builder.Password = tbPassword.Text;
      
      return builder.ToString();
    }

    protected override void SetConnectionString(string value)
    {
      FConnectionString = value;
      VistaDBConnectionStringBuilder builder = new VistaDBConnectionStringBuilder(value);
      try
      {
        tbDatabase.Text = builder.DataSource;
      }
      catch
      {
        tbDatabase.Text = "";
      }
      try
      {
        tbPassword.Text = builder.Password;
      }
      catch
      {
        tbPassword.Text = "";
      }
    }

    public VistaDBConnectionEditor()
    {
      InitializeComponent();
      Localize();
    }
  }
}
