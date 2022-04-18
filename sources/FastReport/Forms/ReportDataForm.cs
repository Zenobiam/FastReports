using System;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Data;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif


namespace FastReport.Forms
{
  internal partial class ReportDataForm : BaseDialogForm
  {
    private Report report;

    private bool IsBusinessObjectNode(TreeNode node)
    {
      while (node != null)
      {
        if (node.Tag is BusinessObjectDataSource)
          return true;
        node = node.Parent;
      }
      return false;
    }
    
    private void UpdateNames(TreeNodeCollection root)
    {
      foreach (TreeNode node in root)
      {
        DataComponentBase data = node.Tag as DataComponentBase;
        if (data != null && !(data is DataConnectionBase))
          node.Text = cbAliases.Checked ? data.Alias : data.Name;
        UpdateNames(node.Nodes);
      }
    }

    private void UpdateNames()
    {
      tvData.BeginUpdate();
      UpdateNames(tvData.Nodes);
      tvData.EndUpdate();
    }

    private void CheckEnabled(TreeNodeCollection root)
    {
      foreach (TreeNode node in root)
      {
        DataComponentBase data = node.Tag as DataComponentBase;
        if (data != null && !(data is DataConnectionBase))
          data.Enabled = node.Checked;
        
        // do not check relation columns - they should be handled by its original datasources
        if (!(data is Relation))
          CheckEnabled(node.Nodes);  
      }
    }

    private void EnableItem(TreeNodeCollection root, DataComponentBase item)
    {
      foreach (TreeNode node in root)
      {
        DataComponentBase data = node.Tag as DataComponentBase;
        if (data == item)
        {
          node.Checked = true;
          break;
        }
        CheckEnabled(node.Nodes);
      }
    }

    private void ReportDataForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      Config.SaveFormState(this);
      if (DialogResult == DialogResult.OK)
        Done();
    }

    private void tvData_BeforeExpand(object sender, TreeViewCancelEventArgs e)
    {
      TreeNode node = e.Node;
      if (!node.Checked && IsBusinessObjectNode(node))
      {
        node.Checked = true;
      }
    }

    private void tvData_AfterCheck(object sender, TreeViewEventArgs e)
    {
      TreeNode node = e.Node;

      if (IsBusinessObjectNode(node))
      {
        BusinessObjectConverter conv = new BusinessObjectConverter(report.Dictionary);
        conv.CheckNode(node);
      }
      else
      {
        DataComponentBase data = node.Tag as DataComponentBase;
        if (node.Checked && data is Relation)
          EnableItem(tvData.Nodes, (data as Relation).ParentDataSource);
      }
    }

    private void cbAliases_CheckedChanged(object sender, EventArgs e)
    {
      UpdateNames();
    }

    private void Init()
    {
      tvData.CreateNodes(report.Dictionary);
        StartPosition = FormStartPosition.Manual;
      // remove existing business objects nodes
      for (int i = 0; i < tvData.Nodes.Count; i++)
      {
        if (tvData.Nodes[i].Tag is BusinessObjectDataSource)
        {
          tvData.Nodes.RemoveAt(i);
          i--;
        }
      }

      // create nodes using BOConverter
      BusinessObjectConverter conv = new BusinessObjectConverter(report.Dictionary);
      foreach (DataSourceBase data in report.Dictionary.DataSources)
      {
        if (data is BusinessObjectDataSource)
          conv.CreateTree(tvData.Nodes, data);
      }
    }
    
    private void Done()
    {
      CheckEnabled(tvData.Nodes);
      report.Dictionary.UpdateRelations();

      // create business objects based on tree
      BusinessObjectConverter conv = new BusinessObjectConverter(report.Dictionary);
      foreach (TreeNode node in tvData.Nodes)
      {
        if (node.Tag is BusinessObjectDataSource)
        {
          conv.CreateDataSource(node);
        }
      }
    }
    
    public override void Localize()
    {
      base.Localize();
      Text = Res.Get("Forms,ReportData");
      lblHint.Text = Res.Get("Forms,ReportData,Hint");
      cbAliases.Text = Res.Get("Forms,ReportData,Aliases");
    }

        public override void CheckRtl()
        {
            base.CheckRtl();

            // apply Right to Left layout
            if (Config.RightToLeft)
            {
                RightToLeft = RightToLeft.Yes;

                // move components to other side
                lblHint.Left = ClientSize.Width - lblHint.Left - lblHint.Width;
                lblHint.Anchor = (AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left);
                tvData.RightToLeft = RightToLeft.Yes;
                tvData.RightToLeftLayout = true;
                cbAliases.Left = ClientSize.Width - cbAliases.Left - cbAliases.Width;
                cbAliases.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
                btnOk.Left = ClientSize.Width - btnOk.Left - btnOk.Width;
                btnOk.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                btnCancel.Left = ClientSize.Width - btnCancel.Left - btnCancel.Width;
                btnCancel.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            }
        }

    public ReportDataForm(Report report)
    {
            this.report = report;
        InitializeComponent();
        Localize();
        Init();
        Config.RestoreFormState(this);
        Scale();
        CheckRtl();
    }
#if !MONO
        protected override void Scale()
        {
            //ControlScalingBegin += ReportDataForm_ControlScalingBegin;
            base.Scale();
            cbAliases.Location = new System.Drawing.Point(cbAliases.Location.X, tvData.Bottom + DpiHelper.ConvertUnits(10));
            //ScaleAutosizeLabel(lblHint);
            lblHint.Width = Width - DpiHelper.ConvertUnits(30);
            lblHint.Height = tvData.Top - lblHint.Height;
        }
#endif
    }
}

