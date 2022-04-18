using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Data;
using FastReport.Controls;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Forms
{
  internal partial class DataBandEditorForm : BaseDialogForm
  {
    private DataBand band;
    private bool firstRun;
    
    private Report Report
    {
      get { return band.Report; }
    }
    
    private DataColumnComboBox[] ComboArray
    {
      get { return new DataColumnComboBox[] { cbxSort1, cbxSort2, cbxSort3 }; }
    }
    
    private RadioButton[] AscArray
    {
      get { return new RadioButton[] { rbSortAsc1, rbSortAsc2, rbSortAsc3 }; }
    }

    private RadioButton[] DescArray
    {
      get { return new RadioButton[] { rbSortDesc1, rbSortDesc2, rbSortDesc3 }; }
    }

    private void FillSort(DataSourceBase dataSource, bool reset)
    {
      SortCollection sort = band.Sort;
      for (int i = 0; i < ComboArray.Length; i++)
      {
        ComboArray[i].Report = Report;
        ComboArray[i].DataSource = dataSource;
        
        if (i >= sort.Count || reset)
        {
          ComboArray[i].Text = "";
          AscArray[i].Checked = true;
          DescArray[i].Checked = false;
        }
        else
        {
          ComboArray[i].Text = sort[i].Expression;
          AscArray[i].Checked = !sort[i].Descending;
          DescArray[i].Checked = sort[i].Descending;
        }
      }
    }
    
    private void GetSort()
    {
      band.Sort.Clear();
      
      for (int i = 0; i < ComboArray.Length; i++)
      {
        if (!String.IsNullOrEmpty(ComboArray[i].Text))
          band.Sort.Add(new Sort(ComboArray[i].Text, DescArray[i].Checked));
      }
    }
    
    private void tvDataSource_AfterSelect(object sender, TreeViewEventArgs e)
    {
      if (!firstRun)
      {
        DataSourceBase data = tvDataSource.SelectedNode.Tag as DataSourceBase;
        FillSort(data, true);
      }
      firstRun = false;
    }

    private void tvDataSource_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      DialogResult = DialogResult.OK;
    }

    private void tbFilter_ButtonClick(object sender, EventArgs e)
    {
      tbFilter.Text = Editors.EditExpression(Report, tbFilter.Text);
    }

    private void DataBandEditorForm_Shown(object sender, EventArgs e)
    {
      tvDataSource.Focus();
    }
    
    private void DataBandEditorForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      Done();
    }
    
    private void Init()
    {
      firstRun = true;
      tbFilter.ImageIndex = 52;
      tvDataSource.CreateNodes(band.Report.Dictionary);
      tvDataSource.SelectedItem = band.DataSource == null ? "" : band.DataSource.FullName;
      tvDataSource.Visible = tvDataSource.Nodes.Count > 1;
      lblNoData.Visible = !tvDataSource.Visible;
      FillSort(band.DataSource, false);
      tbFilter.Text = band.Filter;
      StartPosition = FormStartPosition.Manual;
      Config.RestoreFormState(this);
    }
    
    private void Done()
    {
      if (DialogResult == DialogResult.OK)
      {
        band.DataSource = tvDataSource.SelectedNode == null ? null : tvDataSource.SelectedNode.Tag as DataSourceBase;
        GetSort();
        band.Filter = tbFilter.Text;
      }
      Config.SaveFormState(this);
    }
    
    public override void Localize()
    {
      base.Localize();
      MyRes res = new MyRes("Forms,DataBandEditor");
      Text = res.Get("");
      pnDataSource.Text = res.Get("DataSource");
      lblNoData.Text = res.Get("NoData");
      pnSort.Text = res.Get("Sort");
      lblSort1.Text = res.Get("SortBy");
      lblSort2.Text = res.Get("ThenBy");
      lblSort3.Text = res.Get("ThenBy");
      for (int i = 0; i < AscArray.Length; i++)
      {
        AscArray[i].Text = res.Get("Ascending");
        DescArray[i].Text = res.Get("Descending");
      }
      
      pnFilter.Text = res.Get("Filter");
      lblFilter.Text = res.Get("Expression");
    }
    
    public DataBandEditorForm(DataBand band)
    {
            this.band = band;
      InitializeComponent();
      Localize();
      Init();
            Scale();
    }

        protected override void Scale()
        {
            base.Scale();
            rbSortAsc1.Location = new Point(cbxSort1.Right + DpiHelper.ConvertUnits(10), lblSort1.Bottom);
            rbSortDesc1.Location = new Point(cbxSort1.Right + DpiHelper.ConvertUnits(10), rbSortAsc1.Bottom + DpiHelper.ConvertUnits(2));

            rbSortAsc2.Location = new Point(cbxSort2.Right + DpiHelper.ConvertUnits(10), lblSort2.Bottom);
            rbSortDesc2.Location = new Point(cbxSort2.Right + DpiHelper.ConvertUnits(10), rbSortAsc2.Bottom + DpiHelper.ConvertUnits(2));

            rbSortAsc3.Location = new Point(cbxSort3.Right + DpiHelper.ConvertUnits(10), lblSort3.Bottom);
            rbSortDesc3.Location = new Point(cbxSort3.Right + DpiHelper.ConvertUnits(10), rbSortAsc3.Bottom + DpiHelper.ConvertUnits(2));
        }

    }
}

