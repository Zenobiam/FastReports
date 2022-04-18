using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Data;

namespace FastReport.Forms
{
  internal partial class GroupBandEditorForm : BaseDialogForm
  {
    private GroupHeaderBand band;

    private void GroupBandEditorForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      Done();
    }
    
    private void Init()
    {
      cbxCondition.Report = band.Report;
      cbxCondition.Text = band.Condition;
      if (band.GroupDataBand != null)
        cbxCondition.DataSource = band.GroupDataBand.DataSource;
      cbxSort.SelectedIndex = (int)band.SortOrder;
    }
    
    private void Done()
    {
      if (DialogResult == DialogResult.OK)
      {
        band.Condition = cbxCondition.Text;
        band.SortOrder = (SortOrder)cbxSort.SelectedIndex;
      }
    }

    public override void Localize()
    {
      base.Localize();
      MyRes res = new MyRes("Forms,GroupBandEditor");
      Text = res.Get("");
      gbCondition.Text = res.Get("Condition");
      gbSettings.Text = res.Get("Settings");
      lblHint.Text = res.Get("Hint");
      lblSort.Text = res.Get("Sort");
      cbxSort.Items.Add(res.Get("NoSort"));
      cbxSort.Items.Add(res.Get("Ascending"));
      cbxSort.Items.Add(res.Get("Descending"));
    }
    
    public GroupBandEditorForm(GroupHeaderBand band)
    {
            this.band = band;
      InitializeComponent();
      Localize();
      Init();
            Scale();
    }
  }
}

