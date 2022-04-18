using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Design;
using FastReport.Code;
using FastReport.Data;

namespace FastReport.Forms
{
  internal partial class TotalEditorForm : BaseDialogForm
  {
    private ReportPage page;
    private Report report;
    private Total total;
    
    public Total Total
    {
      get { return total; }
      set 
      { 
        total = value; 
        UpdateControls();
      }
    }
    
    private void UpdateControls()
    {
      tbTotalName.Text = total.Name;
      cbxFunction.SelectedIndex = (int)total.TotalType;
      
      if (total.TotalType != TotalType.Count)
        cbxDataColumn.Text = total.Expression;

      if (total.Evaluator != null && cbxDataBand.Items.IndexOf(total.Evaluator) != -1)
        cbxDataBand.SelectedItem = total.Evaluator;
      
      cbInvisibleRows.Checked = total.IncludeInvisibleRows;
      tbEvaluateCondition.Text = total.EvaluateCondition;

      if (total.PrintOn != null && cbxPrintOn.Items.IndexOf(total.PrintOn) != -1)
        cbxPrintOn.SelectedItem = total.PrintOn;
      
      cbResetAfterPrint.Checked = total.ResetAfterPrint;
      cbResetRepeated.Checked = total.ResetOnReprint;  
    }

    private void tbEvaluateCondition_ButtonClick(object sender, EventArgs e)
    {
      using (ExpressionEditorForm form = new ExpressionEditorForm(report))
      {
        form.ExpressionText = tbEvaluateCondition.Text;
        if (form.ShowDialog() == DialogResult.OK)
          tbEvaluateCondition.Text = form.ExpressionText;
      }
    }

    private void cbxDataBand_SelectedIndexChanged(object sender, EventArgs e)
    {
      cbxPrintOn.Items.Clear();
      cbxPrintOn.Items.Add(0);
      
      if (cbxDataBand.SelectedIndex != -1)
      {
        DataBand dataBand = cbxDataBand.SelectedItem as DataBand;
        ObjectCollection list = new ObjectCollection();
        ReportPage page = dataBand.Page as ReportPage;
        while (page != null)
        {
          list.AddRange(page.AllObjects);
          if (page.Subreport != null)
            page = page.Subreport.Page as ReportPage;
          else
            break;  
        }
        
        foreach (Base c in list)
        {
          Base band = c as BandBase;
          if (c is ChildBand)
            band = (c as ChildBand).GetTopParentBand;
          if (band is DataFooterBand || band is GroupFooterBand || 
            band is ColumnFooterBand || band is PageFooterBand || band is ReportSummaryBand)
            cbxPrintOn.Items.Add(c);
        }
      }

      cbxPrintOn.SelectedIndex = 0;
    }

    private void cbxFunction_SelectedIndexChanged(object sender, EventArgs e)
    {
      cbxDataColumn.Enabled = cbxFunction.SelectedIndex != 4;
    }

    private void TotalEditorForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      if (DialogResult == DialogResult.OK)
        Done();
    }

    private void Init()
    {
      cbxDataColumn.Report = report;
      cbxFunction.SelectedIndex = 0;
      
      foreach (Base c in report.AllObjects)
      {
        if (c is DataBand)
          cbxDataBand.Items.Add(c);
      }
      if (cbxDataBand.Items.Count > 0)
        cbxDataBand.SelectedIndex = 0;
        
      Total total = new Total();
      total.Name = report.Dictionary.CreateUniqueName("Total");
      Total = total;
    }

    private void Done()
    {
      total.Name = tbTotalName.Text;
      total.TotalType = (TotalType)cbxFunction.SelectedIndex;
      
      total.Expression = cbxDataColumn.Text;

      if (cbxDataBand.SelectedIndex != -1)
        total.Evaluator = cbxDataBand.SelectedItem as DataBand;
      else
        total.Evaluator = null;
      
      total.IncludeInvisibleRows = cbInvisibleRows.Checked;
      total.EvaluateCondition = tbEvaluateCondition.Text;

      if (cbxPrintOn.SelectedIndex != 0)
        total.PrintOn = cbxPrintOn.SelectedItem as BandBase;
      else
        total.PrintOn = null;
      total.ResetAfterPrint = cbResetAfterPrint.Checked;
      total.ResetOnReprint = cbResetRepeated.Checked;  
    }

    public override void Localize()
    {
      base.Localize();
      MyRes res = new MyRes("Forms,TotalEditor");
      Text = res.Get("");
      gbTotal.Text = res.Get("Total");
      lblTotalName.Text = res.Get("TotalName");
      lblFunction.Text = res.Get("Function");
      lblDataColumnOrExpression.Text = res.Get("DataColumnOrExpression");
      lblDataBand.Text = res.Get("DataBand");
      lblEvaluateCondition.Text = res.Get("EvaluateCondition");
      lblPrintOn.Text = res.Get("PrintOn");
      gbOptions.Text = res.Get("Options");
      cbResetAfterPrint.Text = res.Get("ResetAfterPrint");
      cbResetRepeated.Text = res.Get("ResetRepeated");
      cbInvisibleRows.Text = res.Get("InvisibleRows");
      tbEvaluateCondition.ImageIndex = 52;
      cbxFunction.Items.AddRange(new object[] { 
        res.Get("Sum"), res.Get("Min"), res.Get("Max"), res.Get("Avg"), res.Get("Count"), res.Get("CountDistinct") });
    }

    public TotalEditorForm(Designer designer)
    {
      report = designer.ActiveReport;
      page = designer.ActiveReportTab.ActivePage as ReportPage;
      InitializeComponent();
      Localize();
      Init();
            Scale();
    }
  }


}

