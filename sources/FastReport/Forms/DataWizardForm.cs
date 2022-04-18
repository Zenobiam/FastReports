using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Data;
using FastReport.Design;
using FastReport.Controls;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Forms
{
  internal partial class DataWizardForm : BaseWizardForm
  {
    private Report report;
    private LastConnections connections;
    private DataConnectionBase connection;
    private bool updating;
    private bool editMode;

#region Properties
    public DataConnectionBase Connection
    {
      get { return connection; }
      set
      {
        if (value != connection)
        {
          if (connection != null)
          {
            report.Dictionary.Connections.Remove(connection);
            connection.Clear();
          }  
          connection = value;
          if (value != null)
          {
            connections.Add(connection);
            report.Dictionary.Connections.Add(connection);
            UpdateConnectionsCombo();
          }
        }
        if (value != null)
          tbConnString.Text = value.ConnectionString;
        UpdateButtons();
      }
    }
    
    public bool EditMode
    {
      get { return editMode; }
      set
      {
        editMode = value;
        if (value)
        {
          cbxConnections.Enabled = false;
          btnNewConnection.Enabled = false;
          tbConnName.Text = Connection.Name;
        }
      }
    }

    public override int VisiblePanelIndex
    {
      get { return base.VisiblePanelIndex; }
      set
      {
        base.VisiblePanelIndex = value;
        if (value == 1)
          UpdateTree();
      }
    }

    private bool ApplicationConnectionMode
    {
      get { return Config.DesignerSettings.ApplicationConnection != null; }
    }
#endregion

#region First page
    private void cbxConnections_DrawItem(object sender, DrawItemEventArgs e)
    {
      e.DrawBackground();
      if (e.Index >= 0)
      {
        try
        {
          DataConnectionBase connection = cbxConnections.Items[e.Index] as DataConnectionBase;
          TextRenderer.DrawText(e.Graphics, connection.GetConnectionId(), e.Font, e.Bounds.Location, e.ForeColor);
        }
        catch
        {
        }
      }
    }
    
    private void btnConnString_Click(object sender, EventArgs e)
    {
      if (!tbConnString.Visible)
      {
        btnConnString.Image = Res.GetImage(184);
        tbConnString.Visible = true;
      }
      else
      {
        btnConnString.Image = Res.GetImage(183);
        tbConnString.Visible = false;
      }
    }

    private void UpdateConnectionsCombo()
    {
      updating = true;
      cbxConnections.Items.Clear();
      cbxConnections.Items.AddRange(connections.ToArray());
      if (cbxConnections.Items.Count > 0)
        cbxConnections.SelectedIndex = 0;
      updating = false;  
    }

    private void cbxConnections_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (updating)
        return;
      updating = true;
      Connection = cbxConnections.SelectedItem as DataConnectionBase;
      updating = false;  
    }

    private void btnNewConnection_Click(object sender, EventArgs e)
    {
      using (ConnectionForm form = new ConnectionForm())
      {
        if (form.ShowDialog() == DialogResult.OK)
          Connection = form.Connection;
      }
    }

    private void btnEditConnection_Click(object sender, EventArgs e)
    {
      using (ConnectionForm form = new ConnectionForm())
      {
        form.Connection = Connection;
        form.EditMode = true;
        if (form.ShowDialog() == DialogResult.OK)
          Connection = form.Connection;
      }
    }
#endregion

#region Second page
    private void CheckTableSchema(TreeNode node)
    {
      TableDataSource data = node.Tag as TableDataSource;
      if (data != null && data.Columns.Count == 0)
      {
        try
        {
          data.InitSchema();
        }
        catch (Exception ex)
        {
          FRMessageBox.Error(ex.Message);
        }
        finally
        {
          node.Nodes.Clear();
          DataTreeHelper.AddColumns(node.Nodes, data.Columns, false, true);
        }
      }
    }
    
    private void tvTables_BeforeExpand(object sender, TreeViewCancelEventArgs e)
    {
      CheckTableSchema(e.Node);
    }

    private void tvTables_AfterCheck(object sender, TreeViewEventArgs e)
    {
      if (updating)
        return;
      CheckTableSchema(e.Node);
    }

    private void btnAddTable_Click(object sender, EventArgs e)
    {
      TableDataSource table = new TableDataSource();
      table.Name = report.Dictionary.CreateUniqueName("Table");
      table.Alias = report.Dictionary.CreateUniqueAlias(table.Alias);
      table.Connection = Connection;
      using (QueryWizardForm form = new QueryWizardForm(table))
      {
        if (form.ShowDialog() != DialogResult.OK)
          table.Dispose();
        else
          UpdateTree();
      }
    }

    private void UpdateTree()
    {
      updating = true;
      tvTables.BeginUpdate();
      tvTables.Nodes.Clear();
      tvTables.Sorted = false;

      lblWait.Location = new Point(tvTables.Left + 1, tvTables.Top + tvTables.Height / 2 - 32);
      lblWait.Size = new Size(tvTables.Width - 2, 32);
      lblWait.Visible = true;
      lblWait.Refresh();
      try
      {
        connection.CreateAllTables(false);
      }
      catch
      {
      }
      lblWait.Visible = false;
      btnAddQuery.Enabled = Connection.IsSqlBased;

      DataTreeHelper.CreateDataTree(report.Dictionary, Connection, tvTables.Nodes);
      tvTables.EndUpdate();
      updating = false;
    }

    private void cbxCheckAll_CheckedChanged(object sender, EventArgs e)
    {
        foreach (TreeNode node in tvTables.Nodes)
        {
            node.Checked = cbxCheckAll.Checked;
        }
    }

#endregion

    private void DataWizardForm_Shown(object sender, EventArgs e)
    {
      // needed for 120dpi mode
      tbConnString.Height = pnDatabase.Height - tbConnString.Top - 12;
    }

    private void DataWizardForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      Done();
      Config.SaveFormState(this);
    }

    private void UpdateButtons()
    {
      bool enabled = Connection != null;
      btnEditConnection.Enabled = enabled;
      btnNext.Enabled = enabled;
    }

    private void ReadConnections()
    {
      if (Config.DesignerSettings.CustomConnections.Count > 0)
      {
        connections.Deserialize(Config.DesignerSettings.CustomConnections);
      }
      else
      {
        XmlItem root = Config.Root.FindItem("Designer").FindItem("LastConnections");

        // read last connections
        using (FRReader reader = new FRReader(null, root))
        {
          reader.Read(connections);
        }

        // write successfully readed connections back
        using (FRWriter writer = new FRWriter(root))
        {
          root.Clear();
          writer.Write(connections);
        }
      }  

      if (connections.Count > 0)
        Connection = connections[0];
    }
    
    private void Init()
    {
      picIcon.Image = ResourceLoader.GetBitmap("datawizard.png");
      btnConnString.Image = Res.GetImage(183);
      tvTables.ImageList = Res.GetImages();

      if (ApplicationConnectionMode)
      {
        if (report.Dictionary.Connections.Count == 0)
        {
          Connection = Activator.CreateInstance(Config.DesignerSettings.ApplicationConnectionType) as DataConnectionBase;
          Connection.Name = report.Dictionary.CreateUniqueName("Connection");
        }
        else
          Connection = report.Dictionary.Connections[0];

        Connection.ConnectionString = Config.DesignerSettings.ApplicationConnection.ConnectionString;
        EditMode = true;
        VisiblePanelIndex = 1;
        btnPrevious.Visible = false;
        btnNext.Visible = false;
      }
      else
      {
        VisiblePanelIndex = 0;
        tbConnName.Text = report.Dictionary.CreateUniqueName("Connection");
        ReadConnections();
      }

      UpdateButtons();
    }
    
    private void Done()
    {
      if (DialogResult == DialogResult.OK)
      {
        Connection.Name = tbConnName.Text;
        
        // enable items that we have checked in the table tree
        foreach (TreeNode tableNode in tvTables.Nodes)
        {
          TableDataSource table = tableNode.Tag as TableDataSource;
          if (table != null)
          {
            table.Enabled = tableNode.Checked;

            foreach (TreeNode colNode in tableNode.Nodes)
            {
              Column column = colNode.Tag as Column;
              if (column != null)
                column.Enabled = colNode.Checked;
            }
          }
        }

        // create relations if any
        Connection.CreateRelations();
        
        XmlItem root = Config.Root.FindItem("Designer").FindItem("LastConnections");
        using (FRWriter writer = new FRWriter(root))
        {
          root.Clear();
          writer.Write(connections);
        }
      }

      if (ApplicationConnectionMode)
      {
        // in this mode, we don't have to store connection string in the report
        Connection.ConnectionString = "";
      }
      if (DialogResult == DialogResult.OK || EditMode)
        connections.Remove(Connection);
      connections.Dispose();
    }

    public override void Localize()
    {
      base.Localize();
      MyRes res = new MyRes("Forms,DataWizard");
      Text = res.Get("");
      pnDatabase.Text = res.Get("Page1");
      pnTables.Text = res.Get("Page2");
      lblWhichData.Text = res.Get("WhichData");
      lblHint.Text = res.Get("Hint");
      btnNewConnection.Text = res.Get("NewConnection");
      btnEditConnection.Text = res.Get("EditConnection");
      lblConnName.Text = res.Get("ConnectionName");
      lblConnString.Text = res.Get("ConnectionString");
      lblWhichTables.Text = res.Get("WhichTables");
      lblWait.Text = res.Get("Wait");
      btnAddQuery.Text = res.Get("AddQuery");
      cbxCheckAll.Text = res.Get("CheckAll");
    }
    
    public DataWizardForm(Report report)
    {
            this.report = report;
        connections = new LastConnections();
        InitializeComponent();
        Localize();
        Init();
        Config.RestoreFormState(this);
        lblWhichData.Font = new Font(DrawUtils.DefaultFont, FontStyle.Bold);
        lblWhichTables.Font = lblWhichData.Font;
            
            ControlScalingBegin += DataWizardForm_ControlScalingBegin;
            Scale();
            CheckRtl();
    }

        public override void CheckRtl()
        {
            base.CheckRtl();
            // apply Right to Left layout
            if (Config.RightToLeft)
            {
                RightToLeft = RightToLeft.Yes;

                // move components to other side
                lblCaption.Left = ClientSize.Width - lblCaption.Left - lblCaption.Width;
                lblCaption.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                picIcon.Left = ClientSize.Width - picIcon.Left - picIcon.Width;
                picIcon.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                lblHint.Left = ClientSize.Width - lblHint.Left - lblHint.Width;
                lblHint.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                cbxConnections.Left = ClientSize.Width - cbxConnections.Left - cbxConnections.Width;
                cbxConnections.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                btnNewConnection.Left = ClientSize.Width - btnNewConnection.Left - btnNewConnection.Width;
                lblConnName.Left = ClientSize.Width - lblConnName.Left - lblConnName.Width;
                lblConnName.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                tbConnName.Left = ClientSize.Width - tbConnName.Left - tbConnName.Width;
                tbConnName.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                btnEditConnection.Left = ClientSize.Width - btnEditConnection.Left - btnEditConnection.Width;
                btnPrevious.Left = ClientSize.Width - btnPrevious.Left - btnPrevious.Width;
                btnNext.Left = ClientSize.Width - btnNext.Left - btnNext.Width;
                btnFinish.Left = ClientSize.Width - btnFinish.Left - btnFinish.Width;
                btnCancel1.Left = ClientSize.Width - btnCancel1.Left - btnCancel1.Width;

                lblWhichTables.Left = pnTables.Right - lblWhichTables.Width;
                lblWhichTables.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                cbxCheckAll.Left = pnTables.Right - cbxCheckAll.Width - (int)(10 * FormRatio);
                cbxCheckAll.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
                tvTables.RightToLeftLayout = true;
                btnAddQuery.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                btnAddQuery.Left = pnTables.Left + (int)(10 * FormRatio);

                btnOk.Left = ClientSize.Width - btnOk.Left - btnOk.Width;
                btnCancel.Left = ClientSize.Width - btnCancel.Left - btnCancel.Width;
            }
        }

        protected override void Scale()
        {
            base.Scale();
            btnAddQuery.Location = new Point(tvTables.Right - btnAddQuery.Width, btnAddQuery.Location.Y);
            cbxCheckAll.AutoSize = true;
            cbxCheckAll.Top = btnAddQuery.Top;
            cbxConnections.Font = DpiHelper.GetFontForTextBoxHeight(btnNewConnection.Height - 3, cbxConnections.Font);
            cbxConnections.Location = new Point(cbxConnections.Location.X, btnNewConnection.Location.Y + 1);
            tbConnName.Font = DpiHelper.GetFontForTextBoxHeight(btnEditConnection.Height - 2, tbConnName.Font);
            tbConnName.Location = new Point(tbConnName.Location.X, btnEditConnection.Location.Y + 1);
            picIcon.Location = new Point(btnNewConnection.Right - picIcon.Width, pnTop.Height / 2 - picIcon.Height / 2);
            lblWhichData.Width = lblHint.Width;
        }

        private void DataWizardForm_ControlScalingBegin(object sender, Bools boolArgs)
        {
            if(sender is Label)
            {

                if((sender as Label).Name == "lblHint")
                {
                    boolArgs.NeedScaleThisControl = false;
                    boolArgs.NeedSpecificConside = false;
                    Label lb = sender as Label;
                    lb.Location = DpiHelper.ConvertUnits(lb.Location);
                    lb.Size = DpiHelper.ConvertUnits(lb.Size);
                    lb.Width = lb.Parent.Width - DpiHelper.ConvertUnits(27);
                }
                else if(sender == lblWhichData)
                {
                    Label lbl = sender as Label;
                    ScaleFont(lbl);
                    lbl.Height = (int)CreateGraphics().MeasureString(lbl.Text, lbl.Font, lbl.Width).Height;
                }
            }
        }

    private class LastConnections : IDisposable, IFRSerializable
    {
      private List<DataConnectionBase> connections;

      public DataConnectionBase this[int index]
      {
        get { return connections[index]; }
      }
      
      public int Count
      {
        get { return connections.Count; }
      }
      
      public void Serialize(FRWriter writer)
      {
        List<string> savedConnections = new List<string>();
        writer.ItemName = "LastConnections";
        foreach (DataConnectionBase connection in connections)
        {
          if (savedConnections.IndexOf(connection.GetConnectionId()) == -1)
          {
            writer.SaveChildren = false;
            writer.Write(connection);
            savedConnections.Add(connection.GetConnectionId());  
          }  
        }
      }

      public void Deserialize(FRReader reader)
      {
        while (reader.NextItem())
        {
          try
          {
            DataConnectionBase connection = reader.Read() as DataConnectionBase;
            if (connection != null)
              connections.Add(connection);
          }
          catch
          {
          }    
        }
      }
      
      public void Deserialize(List<ConnectionEntry> connections)
      {
        foreach (ConnectionEntry ce in connections)
        {
          try
          {
            DataConnectionBase connection = 
              Activator.CreateInstance(ce.type) as DataConnectionBase;
            connection.ConnectionString = ce.connectionString;
                        this.connections.Add(connection);
          }
          catch
          {
          }
        }
      }
      
      public void Add(DataConnectionBase connection)
      {
        for (int i = 0; i < connections.Count; i++)
        {
          try
          {
            if (connections[i].GetConnectionId() == connection.GetConnectionId())
            {
              connections.RemoveAt(i);
              break;
            }
          }
          catch
          {
          }
        }
        connections.Insert(0, connection);
      }
      
      public void Remove(DataConnectionBase connection)
      {
        if (connections.Contains(connection))
          connections.Remove(connection);
      }
      
      public DataConnectionBase[] ToArray()
      {
        return connections.ToArray();
      }

      public void Dispose()
      {
        foreach (DataConnectionBase connection in connections)
        {
          connection.Dispose();
        }
        connections.Clear();
      }

      public LastConnections()
      {
        connections = new List<DataConnectionBase>();
      }
    }
  }
}

