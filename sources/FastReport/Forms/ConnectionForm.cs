using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Data;
using FastReport.Data.ConnectionEditors;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Forms
{
    internal partial class ConnectionForm : BaseDialogForm
  {
    private DataConnectionBase connection;
    private ConnectionEditorBase connectionEditor;
    private bool editMode;
    
    public DataConnectionBase Connection
    {
      get { return connection; }
      set { connection = value; }
    }
    
    public bool EditMode
    {
      get { return editMode; }
      set
      {
        editMode = value;
        if (value)
        {
          cbxConnections.Items.Clear();
          cbxConnections.Items.Add(RegisteredObjects.FindObject(Connection));
          cbxConnections.SelectedIndex = 0;
          cbxConnections.Enabled = false;
          cbAlwaysUse.Enabled = false;
        }
      }
    }
    
    private void EnumConnections()
    {
      List<ObjectInfo> registeredObjects = new List<ObjectInfo>();
      RegisteredObjects.Objects.EnumItems(registeredObjects);
      string lastUsed = Config.Root.FindItem("Forms").FindItem(Name).GetProp("ConnectionType");

      foreach (ObjectInfo info in registeredObjects)
      {
        if (info.Object != null && info.Object.IsSubclassOf(typeof(DataConnectionBase)))
        {
          cbxConnections.Items.Add(info);
          if (info.Object.Name == lastUsed)
          {
            cbxConnections.SelectedIndex = cbxConnections.Items.Count - 1;
            cbAlwaysUse.Checked = true;
          }  
        }  
      }

      if (cbxConnections.SelectedIndex == -1)
        cbxConnections.SelectedIndex = 0;
    }

    private bool TestConnection(bool showOkMessage)
    {
      string saveConnectionString = connection.ConnectionString;
      connection.ConnectionString = connectionEditor.ConnectionString;
      bool successful = true;
      string errorMsg = "";
      try
      {
        connection.TestConnection();
      }
      catch (Exception e)
      {
        successful = false;
        errorMsg = e.Message;
      }
      connection.ConnectionString = saveConnectionString;

      if (successful && showOkMessage)
        FRMessageBox.Information(Res.Get("Forms,Connection,TestSuccesful"));
      else if (!successful)
        FRMessageBox.Error(errorMsg);
      return successful;  
    }

    private void cbxConnections_DrawItem(object sender, DrawItemEventArgs e)
    {
      e.DrawBackground();
      if (e.Index >= 0)
      {
        ObjectInfo info = cbxConnections.Items[e.Index] as ObjectInfo;
        TextRenderer.DrawText(e.Graphics, Res.TryGet(info.Text), e.Font, e.Bounds.Location, e.ForeColor);
      }
    }

    private void ConnectionForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      if (cbAlwaysUse.Checked)
      {
        Config.Root.FindItem("Forms").FindItem(Name).SetProp("ConnectionType", 
          (cbxConnections.SelectedItem as ObjectInfo).Object.Name);
      }    
    }

    private void ConnectionForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (DialogResult == DialogResult.OK && connection != null && connectionEditor != null)
      {
        connection.LoginPrompt = cbLoginPrompt.Checked;
        if (connection.LoginPrompt || TestConnection(false))
          connection.ConnectionString = connectionEditor.ConnectionString;
        else
          e.Cancel = true;
      }  
    }

    private void cbxConnections_SelectedIndexChanged(object sender, EventArgs e)
    {
      cbAlwaysUse.Checked = false;
      SuspendLayout();
      if (connectionEditor != null)
        connectionEditor.Dispose();
      if (!editMode)
      {
        if (connection != null)
          connection.Dispose();
        connection = null;  
      
        Type connectionType = (cbxConnections.SelectedItem as ObjectInfo).Object;
        connection = Activator.CreateInstance(connectionType) as DataConnectionBase;
      }    
      connectionEditor = connection.GetEditor();

      if (connectionEditor != null)
      {
#if !MONO
                DpiHelper.RescaleWithNewDpi(()=>
                {
                    SpecificAnchors = new Dictionary<Control, AnchorStyles>();
                    DisableAnchors(connectionEditor.Controls);
                    ControlScalingBegin += ConnectionForm_ControlScalingBegin;
                    ScaleControl(connectionEditor);
                    ControlScalingBegin -= ConnectionForm_ControlScalingBegin;
                    EnableAnchors();
                }, FormRatio);
#endif
        connectionEditor.CheckRTL();
        connectionEditor.Parent = this;
        connectionEditor.Location = new Point(0, gbSelect.Bottom);
        //connectionEditor.Font = DrawUtils.DefaultFont;
        PerformAutoScale();
        connectionEditor.UpdateLayout();
        ClientSize = new Size(ClientSize.Width, 
          connectionEditor.Bottom + cbLoginPrompt.Height + btnOk.Height + gbSelect.Top * 5);
      }
      else
        ClientSize = new Size(ClientSize.Width, 
        gbSelect.Bottom + cbLoginPrompt.Height + btnOk.Height + gbSelect.Top * 5);

      ResumeLayout();
      Refresh();
      if (connection != null && connectionEditor != null)
      {
        connectionEditor.ConnectionString = connection.ConnectionString;
        cbLoginPrompt.Checked = connection.LoginPrompt;
      }  
      btnTest.Enabled = connection != null;
    }

    private void btnTest_Click(object sender, EventArgs e)
    {
      TestConnection(true);
    }

    public override void Localize()
    {
      base.Localize();
      MyRes res = new MyRes("Forms,Connection");
      Text = res.Get("");
      gbSelect.Text = res.Get("Select");
      cbAlwaysUse.Text = res.Get("AlwaysUse");
      cbLoginPrompt.Text = res.Get("LoginPrompt");
      btnTest.Text = res.Get("Test");
    }
    
    public ConnectionForm()
    {
      InitializeComponent();
      Localize();

      ControlScalingBegin += ConnectionForm_ControlScalingBegin;
      Scale();
      CheckRtl();
      ControlScalingBegin -= ConnectionForm_ControlScalingBegin;
      EnumConnections();
    }
        public override void CheckRtl()
        {
            base.CheckRtl();

            // apply Right to Left layout
            if (Config.RightToLeft)
            {
                RightToLeft = RightToLeft.Yes;

                // move components to other side
                cbAlwaysUse.Left = gbSelect.Width - cbAlwaysUse.Left - cbAlwaysUse.Width;
                cbAlwaysUse.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                cbLoginPrompt.Left = ClientSize.Width - cbLoginPrompt.Left - cbLoginPrompt.Width;
                cbLoginPrompt.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
                btnTest.Left = ClientSize.Width - btnTest.Left - btnTest.Width;
                btnOk.Left = ClientSize.Width - btnOk.Left - btnOk.Width;
                btnCancel.Left = ClientSize.Width - btnCancel.Left - btnCancel.Width;
            }
        }
        protected override void Scale()
        {
            base.Scale();
            cbLoginPrompt.Location = new Point(btnTest.Left, btnTest.Top - cbLoginPrompt.Height - DpiHelper.ConvertUnits(5));
        }
        private void ConnectionForm_ControlScalingBegin(object sender, Bools boolArgs)
        {
            //if(sender is ConnectionEditorBase)
            //{
            //    boolArgs.NeedCheckControl = false;
            //    boolArgs.NeedScaleThisControl = false;
            //    boolArgs.NeedSpecificConside = false;
            //}
            if(sender is GroupBox && sender != gbSelect)
            {
                ScaleFont(sender as GroupBox);
            }
            else if((sender as Control).Name == "btnAdvanced")
            {
                Button btn = sender as Button;
                ScaleFont(sender as Control);
                ScaleLocation(btn);
                ScaleSize(btn);

                btn.Padding = DpiHelper.ConvertUnits(btn.Padding);
                btn.Margin = DpiHelper.ConvertUnits(btn.Margin);
                boolArgs.NeedScaleThisControl = false;
                btn.Left = btn.Parent.Width - btn.Width - DpiHelper.ConvertUnits(7);
            }
            else if(sender == btnTest)
            {
                Button test = sender as Button;
                test.Height = btnOk.Height;
                test.Location = new Point(test.Location.X, btnOk.Top);
                boolArgs.NeedScaleThisControl = false;
                ScaleLocation(sender as Control);
                ScaleSize(sender as Control);
                ScaleFont(sender as Control);
            }
        }
    }
}

