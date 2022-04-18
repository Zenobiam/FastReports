using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Data;
using FastReport.FastQueryBuilder;
using FastReport.Design;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Forms
{
  internal partial class QueryWizardForm : BaseWizardForm
  {
    private TableDataSource table;

    public override int VisiblePanelIndex
    {
      get { return base.VisiblePanelIndex; }
      set
      {
        // disable page 2,3 in case of non-sql datasource
        if (table.Connection != null && !table.Connection.IsSqlBased)
        {
          if (value == 1)
            value = 3;
          if (value == 2)
            value = 0;
        }
        
        if (value == 2)
          UpdateParamTree(null);
        if (value == 3)
        {
          table.Alias = tbName.Text;
          table.SelectCommand = tbSql.Text;
          try
          {
            table.RefreshTable();
            UpdateColumnTree(null);
          }
          catch (Exception e)
          {
            FRMessageBox.Error(e.Message);
          }  
        }
        base.VisiblePanelIndex = value;
      }
    }

    private void tbSql_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyData == (Keys.A | Keys.Control))
            tbSql.SelectAll();
    }

    private void btnQueryBuilder_Click(object sender, EventArgs e)
    {
      if (table.Connection != null)
        using (DataConnectionBase conn = Activator.CreateInstance(table.Connection.GetType()) as DataConnectionBase)
        {
          conn.Assign(table.Connection);
          if (Config.DesignerSettings.ApplicationConnection != null)
            conn.ConnectionString = Config.DesignerSettings.ApplicationConnection.ConnectionString;

          CustomQueryBuilderEventArgs args = new CustomQueryBuilderEventArgs(conn, tbSql.Text, table.Parameters);
          Config.DesignerSettings.OnCustomQueryBuilder(this, args);
          tbSql.Text = args.SQL;
        }  
    }

    private void btnAddParameter_Click(object sender, EventArgs e)
    {
      CommandParameter c = new CommandParameter();
      c.Name = table.Parameters.CreateUniqueName("Parameter");
      c.DataType = table.Connection.GetDefaultParameterType();
      table.Parameters.Add(c);
      UpdateParamTree(c);
    }

    private void btnDeleteParameter_Click(object sender, EventArgs e)
    {
      if (tvParameters.SelectedNode == null)
        return;
      Base c = tvParameters.SelectedNode.Tag as Base;
      if (c == null)
        return;

      pgParamProperties.SelectedObject = null;
      c.Dispose();
      UpdateParamTree(null);
    }

    private void btnParameterUp_Click(object sender, EventArgs e)
    {
      if (tvParameters.SelectedNode == null)
        return;
      Base c = tvParameters.SelectedNode.Tag as Base;
      if (c == null)
        return;

      int index = table.Parameters.IndexOf(c);
      table.Parameters.RemoveAt(index);
      table.Parameters.Insert(index - 1, c);

      UpdateParamTree(c);
    }

    private void btnParameterDown_Click(object sender, EventArgs e)
    {
      if (tvParameters.SelectedNode == null)
        return;
      Base c = tvParameters.SelectedNode.Tag as Base;
      if (c == null)
        return;

      int index = table.Parameters.IndexOf(c);
      table.Parameters.RemoveAt(index);
      table.Parameters.Insert(index + 1, c);

      UpdateParamTree(c);
    }

    private void tvParameters_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Delete)
        btnDeleteParameter_Click(this, EventArgs.Empty);
    }

    private void btnRefreshColumns_Click(object sender, EventArgs e)
    {
      table.RefreshColumns(true);
      UpdateColumnTree(null);
    }

    private void btnAddColumn_Click(object sender, EventArgs e)
    {
      Column c = new Column();
      c.Name = table.Columns.CreateUniqueName("Column");
      c.Alias = table.Columns.CreateUniqueAlias(c.Alias);
      c.Calculated = true;
      table.Columns.Add(c);
      UpdateColumnTree(c);
    }

    private void btnDeleteColumn_Click(object sender, EventArgs e)
    {
      if (tvColumns.SelectedNode == null)
        return;
      Base c = tvColumns.SelectedNode.Tag as Base;
      if (c == null)
        return;

      pgColumnProperties.SelectedObject = null;
      c.Dispose();
      UpdateColumnTree(null);
    }

    private void tvColumns_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Delete)
        btnDeleteColumn_Click(this, EventArgs.Empty);
    }

    private void tvParameters_AfterSelect(object sender, TreeViewEventArgs e)
    {
      UpdateParamSelection();
    }

    private void UpdateParamSelection()
    {
      if (tvParameters.SelectedNode == null)
      {
        pgParamProperties.SelectedObject = null;
        btnDeleteParameter.Enabled = false;
        btnParameterUp.Enabled = false;
        btnParameterDown.Enabled = false;
        return;
      }
      pgParamProperties.SelectedObject = tvParameters.SelectedNode.Tag;
      btnDeleteParameter.Enabled = true;
      btnParameterUp.Enabled = tvParameters.SelectedNode.Index > 0;
      btnParameterDown.Enabled = tvParameters.SelectedNode.Index < tvParameters.Nodes.Count - 1;
    }

    private void tvColumns_AfterSelect(object sender, TreeViewEventArgs e)
    {
      pgColumnProperties.SelectedObject = tvColumns.SelectedNode.Tag;
    }

    private void pgParamProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
      if (tvParameters.SelectedNode != null && tvParameters.SelectedNode.Tag is CommandParameter)
        tvParameters.SelectedNode.Text = (tvParameters.SelectedNode.Tag as CommandParameter).Name;
    }

    private void pgColumnProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
      if (tvColumns.SelectedNode != null && tvColumns.SelectedNode.Tag is Column)
        tvColumns.SelectedNode.Text = (tvColumns.SelectedNode.Tag as Column).Alias;
    }

    private void UpdateParamTree(Base focusObj)
    {
      tvParameters.BeginUpdate();
      tvParameters.Nodes.Clear();

      foreach (CommandParameter c in table.Parameters)
      {
        TreeNode node = tvParameters.Nodes.Add(c.Name);
        node.Tag = c;
        node.ImageIndex = 231;
        node.SelectedImageIndex = node.ImageIndex;
        if (c == focusObj)
          tvParameters.SelectedNode = node;
      }

      if (focusObj == null && tvParameters.Nodes.Count > 0)
        tvParameters.SelectedNode = tvParameters.Nodes[0];
      tvParameters.EndUpdate();
      UpdateParamSelection();
    }

    private void UpdateColumnTree(Base focusObj)
    {
      tvColumns.BeginUpdate();
      tvColumns.Nodes.Clear();
      
      table.InitSchema();
      
      foreach (Column c in table.Columns)
      {
        TreeNode node = tvColumns.Nodes.Add(c.Alias);
        node.Tag = c;
        node.ImageIndex = c.GetImageIndex();
        node.SelectedImageIndex = node.ImageIndex;
        if (c == focusObj)
          tvColumns.SelectedNode = node;
      }

      if (focusObj == null && tvColumns.Nodes.Count > 0)
        tvColumns.SelectedNode = tvColumns.Nodes[0];
      tvColumns.EndUpdate();
    }

    private void Init()
    {
       VisiblePanelIndex = 0;
       picIcon.Image = ResourceLoader.GetBitmap("querywizard.png");
       tvParameters.ImageList = Res.GetImages();
       tvColumns.ImageList = Res.GetImages();
       btnAddParameter.Image = Res.GetImage(56);
       btnDeleteParameter.Image = Res.GetImage(51);
       btnParameterUp.Image = Res.GetImage(208);
       btnParameterDown.Image = Res.GetImage(209);
       btnRefreshColumns.Image = Res.GetImage(232);
       btnAddColumn.Image = Res.GetImage(55);
       btnDeleteColumn.Image = Res.GetImage(51);

       tsParameters.Renderer = Config.DesignerSettings.ToolStripRenderer;
       tsColumns.Renderer = Config.DesignerSettings.ToolStripRenderer;

       tbName.Text = table.Alias;
       tbSql.Text = table.SelectCommand;

       tbSql.Font = GetConfigFont("QueryWizardForm", "QueryWindow");
     }

    //For transfer to Utils
    private Font GetConfigFont(String formName, String formElement)
    {
       XmlItem xi = Config.Root.FindItem("Designer").FindItem("Fonts").FindItem(formName).FindItem(formElement);

       string QueryFontName = xi.GetProp("font-name");
       string QueryFontSize = xi.GetProp("font-size");
       bool bold = xi.GetProp("font-bold") == "1";
       bool italic = xi.GetProp("font-italic") == "1";

       return new Font(
           QueryFontName == "" ? "Tahoma" : QueryFontName, 
           QueryFontSize == "" ? 8 : float.Parse(QueryFontSize),
           (italic ? FontStyle.Italic : 0) | (bold ? FontStyle.Bold : 0)
           );
    }

    private void TableWizardForm_Shown(object sender, EventArgs e)
    {
      // needed for 120dpi mode
      tbSql.Height = btnQueryBuilder.Top - tbSql.Top - 12;
    }
    
    private void TableWizardForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      Config.SaveFormState(this);
    }

    public override void Localize()
    {
      base.Localize();
      MyRes res = new MyRes("Forms,QueryWizard");
      Text = res.Get("");
      pnName.Text = res.Get("Page1");
      pnSql.Text = res.Get("Page2");
      pnParameters.Text = res.Get("Page3");
      pnColumns.Text = res.Get("Page4");
      lblSetName.Text = res.Get("SetName");
      lblNameHint.Text = res.Get("NameHint");
      lblWhatData.Text = res.Get("WhatData");
      lblTypeSql.Text = res.Get("TypeSql");
      btnQueryBuilder.Text = res.Get("QueryBuilder");
      btnAddParameter.Text = res.Get("AddParameter");
      btnDeleteParameter.Text = res.Get("Delete");
      btnRefreshColumns.Text = res.Get("Refresh");
      btnAddColumn.Text = res.Get("AddColumn");
      btnDeleteColumn.Text = res.Get("Delete");
    }
    
    
    public QueryWizardForm(TableDataSource table)
    {
      this.table = table;
      InitializeComponent();
      Localize();
      Init();
      Config.RestoreFormState(this);
            Scale();
    }
        protected override void Scale()
        {
            base.Scale();
            btnCancel1.Location = new Point(tbName.Right - btnCancel1.Width, btnCancel1.Location.Y);
            btnFinish.Left = btnCancel1.Left - btnFinish.Width - DpiHelper.ConvertUnits(7);
            btnNext.Left = btnFinish.Left - btnNext.Width - DpiHelper.ConvertUnits(7);
            btnPrevious.Left = btnNext.Left - btnPrevious.Width - DpiHelper.ConvertUnits(7);
            this.pgColumnProperties.Font = tsColumns.Font;
            pgParamProperties.Font = tsColumns.Font;
            btnQueryBuilder.Left = tbSql.Right - btnQueryBuilder.Width;
        }
    }
}

