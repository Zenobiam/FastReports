using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using FastReport.Controls;
using FastReport.Utils;
using FastReport.TypeConverters;

namespace FastReport.Forms
{
  internal partial class DataBandColumnEditorForm : BaseDialogForm
  {
    private BandColumns columns;
    //private string FUnits;

    public BandColumns Columns
    {
      get { return columns; }
      set
      {
        columns = value;

        UpdateControls();
      }
    }
    
    public void UpdateControls()
    {
      tbColumnCount.Text = Columns.Count.ToString();
      tbColumnMinRowCount.Text = Columns.MinRowCount.ToString();
      tbWidth.Text = Converter.ToString(Columns.Width, typeof(UnitsConverter));
      cbxColumnLayout.SelectedItem = Columns.Layout;
    }
    
    public override void Localize()
    {
      base.Localize();
      MyRes res = new MyRes("Forms,DataBandColumnEditor");
      Text = res.Get("");
      gbColumns.Text = res.Get("Columns");
      lblColumnCount.Text = res.Get("Count");
      lblLayout.Text = res.Get("Layout");
      lblMinRowCount.Text = res.Get("MinRowCount");
      lblWidth.Text = res.Get("Width");
    }

    public DataBandColumnEditorForm(BandColumns columns)
    {
      InitializeComponent();
      Columns = columns;
      cbxColumnLayout.Items.Add(ColumnLayout.AcrossThenDown);
      cbxColumnLayout.Items.Add(ColumnLayout.DownThenAcross);
      Localize();
      UpdateControls();

      this.RightToLeft = Config.RightToLeft ? RightToLeft.Yes : RightToLeft.No;
            Scale();
    }

    private void tbColumnCount_TextChanged(object sender, EventArgs e)
    {
      int newColumnCount = 0;
      if (int.TryParse(tbColumnCount.Text, out newColumnCount))
      {
        Columns.Count = newColumnCount;
        UpdateControls();
      }
    }

    private void tbColumnMinRowCount_TextChanged(object sender, EventArgs e)
    {
      int newMinRowCount = 0;
      if (int.TryParse(tbColumnMinRowCount.Text, out newMinRowCount))
      {
        Columns.MinRowCount = newMinRowCount;
        UpdateControls();
      }
    }

    private void tbWidth_TextChanged(object sender, EventArgs e)
    {
      Columns.Width = (float)Converter.FromString(tbWidth.Text, typeof(UnitsConverter));
      UpdateControls();
    }

    private void cbxColumnLayout_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cbxColumnLayout.Text == ColumnLayout.AcrossThenDown.ToString())
      {
        Columns.Layout = ColumnLayout.AcrossThenDown;
      }
      else
      {
        Columns.Layout = ColumnLayout.DownThenAcross;
      }
      UpdateControls();
    }
  }
}