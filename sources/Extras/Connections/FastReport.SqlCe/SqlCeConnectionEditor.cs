using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using FastReport.Data.ConnectionEditors;
using FastReport.Forms;
using FastReport.Utils;

namespace FastReport.Data
{
  public partial class SqlCeConnectionEditor : ConnectionEditorBase
  {
    private string FConnectionString;

    private void tbDatabase_ButtonClick(object sender, EventArgs e)
    {
      using (OpenFileDialog dialog = new OpenFileDialog())
      {
        dialog.Filter = Res.Get("FileFilters,SdfFile");
        if (dialog.ShowDialog() == DialogResult.OK)
          tbDatabase.Text = dialog.FileName;
      }
    }

    private void Localize()
    {
      MyRes res = new MyRes("ConnectionEditors,Common");
      gbDatabase.Text = res.Get("Database");
      lblDataFile.Text = res.Get("DatabaseFile");
      lblPassword.Text = res.Get("Password");
      tbDatabase.Image = Res.GetImage(1);
    }

    protected override string GetConnectionString()
    {
      SqlCeConnectionStringBuilder builder = new SqlCeConnectionStringBuilder(FConnectionString);

      builder.DataSource = tbDatabase.Text;
      builder.Password = tbPassword.Text;

      return builder.ToString();
    }

    protected override void SetConnectionString(string value)
    {
      FConnectionString = value;
      SqlCeConnectionStringBuilder builder = new SqlCeConnectionStringBuilder(value);

      tbDatabase.Text = builder.DataSource;
      tbPassword.Text = builder.Password;
    }

    public SqlCeConnectionEditor()
    {
      InitializeComponent();
      Localize();
    }
  }
}
