using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using FastReport.Forms;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Export.Email
{
  /// <summary>
  /// Form for <see cref="EmailExport"/>.
  /// For internal use only.
  /// </summary>
  public partial class EmailExportForm : BaseDialogForm
  {
    private EmailExport export;
    private List<ExportBase> exports;

    private bool IsValidEmail(string strIn)
    {
      // Return true if strIn is in valid e-mail format.
      return Regex.IsMatch(strIn,
             @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
             @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
    }

    private void EmailExportForm_Shown(object sender, EventArgs e)
    {
      if (String.IsNullOrEmpty(tbAddressFrom.Text) || String.IsNullOrEmpty(tbHost.Text))
      {
        pageControl1.ActivePageIndex = 1;
        if (String.IsNullOrEmpty(tbAddressFrom.Text))
          tbAddressFrom.Focus();
        else
          tbHost.Focus();
      }
      else
      {
        cbxAddressTo.Focus();
        if (!export.Account.AllowUI)
        {
          pageControl1.Width -= pageControl1.SelectorWidth;
          Width -= pageControl1.SelectorWidth;
          pageControl1.SelectorWidth = 0;
        }  
      }  
    }

    private void cbxAttachment_SelectedIndexChanged(object sender, EventArgs e)
    {
      btnSettings.Enabled = cbxAttachment.SelectedIndex > 0;
    }

    private void btnSettings_Click(object sender, EventArgs e)
    {
      ExportBase export = exports[cbxAttachment.SelectedIndex];
      export.SetReport(this.export.Report);
      export.ShowDialog();
    }

    private void Init()
    {
      XmlItem xi = Config.Root.FindItem("EmailExport").FindItem("AccountSettings");
      
      // restore account info from the config
      if (String.IsNullOrEmpty(export.Account.Address))
      {
        export.Account.Address = xi.GetProp("Address");
        export.Account.Name = xi.GetProp("Name");
        export.Account.MessageTemplate = xi.GetProp("Template");
        export.Account.Host = xi.GetProp("Host");
        string port = xi.GetProp("Port");
        if (port != "")
          export.Account.Port = int.Parse(port);
        export.Account.UserName = xi.GetProp("UserName");
        export.Account.Password = xi.GetProp("Password");
        export.Account.EnableSSL = xi.GetProp("EnableSSL") == "1";
      }
      
      // fill account info
      tbAddressFrom.Text = export.Account.Address;
      tbName.Text = export.Account.Name;
      tbTemplate.Text = export.Account.MessageTemplate;
      tbHost.Text = export.Account.Host;
      udPort.Value = export.Account.Port;
      tbUserName.Text = export.Account.UserName;
      tbPassword.Text = export.Account.Password;
      cbEnableSSL.Checked = export.Account.EnableSSL;
      
      // fill email
      string[] addresses = xi.GetProp("RecentAddresses").Split(new char[] { '\r' });
      cbxAddressTo.Items.AddRange(addresses);
      if (!String.IsNullOrEmpty(export.Address))
        cbxAddressTo.Text = export.Address;
      else if (cbxAddressTo.Items.Count > 0)
        cbxAddressTo.SelectedIndex = 0;
      string[] cc = export.CC;
      if (cc != null)
      {
        for (int i = 0; i < cc.Length; i++)
          tbCC.Text += cc[i] + (i < cc.Length - 1 ? ";" : "");
      }

      string[] subjects = xi.GetProp("RecentSubjects").Split(new char[] { '\r' });
      cbxSubject.Items.AddRange(subjects);
      if (!String.IsNullOrEmpty(export.Subject))
        cbxSubject.Text = export.Subject;
      else if (cbxSubject.Items.Count > 0)
        cbxSubject.SelectedIndex = 0;

      if (!String.IsNullOrEmpty(export.MessageBody))
        tbMessage.Text = export.MessageBody;
      else
        tbMessage.Text = tbTemplate.Text;

      // fill exports
      exports = new List<ExportBase>();
      List<ObjectInfo> list = new List<ObjectInfo>();
      RegisteredObjects.Objects.EnumItems(list);

      int exportIndex = 0;
      cbxAttachment.Items.Add(Res.Get("Preview,SaveNative"));
      exports.Add(null);

      foreach (ObjectInfo info in list)
      {
        if (info.Object != null && info.Object.IsSubclassOf(typeof(ExportBase)))
        {
          cbxAttachment.Items.Add(Res.TryGet(info.Text));
          exports.Add(Activator.CreateInstance(info.Object) as ExportBase);
          if (export.Export != null && export.Export.GetType() == info.Object)
            exportIndex = exports.Count - 1;
        }
      }
      
      string recentExport = xi.GetProp("RecentExport");
      if (exportIndex != 0)
        cbxAttachment.SelectedIndex = exportIndex;
      else if (recentExport != "")
        cbxAttachment.SelectedIndex = int.Parse(recentExport);
      else
        cbxAttachment.SelectedIndex = 0;
    }

    private bool Done()
    {
      if (!IsValidEmail(tbAddressFrom.Text))
      {
        pageControl1.ActivePageIndex = 1;
        FRMessageBox.Error(Res.Get("Export,Email,AddressError"));
        tbAddressFrom.Focus();
        return false;
      }
      if (String.IsNullOrEmpty(tbHost.Text))
      {
        pageControl1.ActivePageIndex = 1;
        FRMessageBox.Error(Res.Get("Export,Email,HostError"));
        tbHost.Focus();
        return false;
      }
      if (!IsValidEmail(cbxAddressTo.Text))
      {
        pageControl1.ActivePageIndex = 0;
        FRMessageBox.Error(Res.Get("Export,Email,AddressError"));
        cbxAddressTo.Focus();
        return false;
      }

      XmlItem xi = Config.Root.FindItem("EmailExport").FindItem("AccountSettings");

      // get account info
      export.Account.Address = tbAddressFrom.Text;
      export.Account.Name = tbName.Text;
      export.Account.MessageTemplate = tbTemplate.Text;
      export.Account.Host = tbHost.Text;
      export.Account.Port = (int)udPort.Value;
      export.Account.UserName = tbUserName.Text;
      export.Account.Password = tbPassword.Text;
      export.Account.EnableSSL = cbEnableSSL.Checked;

      // save account info
      xi.SetProp("Address", export.Account.Address);
      xi.SetProp("Name", export.Account.Name);
      xi.SetProp("Template", export.Account.MessageTemplate);
      xi.SetProp("Host", export.Account.Host);
      xi.SetProp("Port", export.Account.Port.ToString());
      xi.SetProp("UserName", export.Account.UserName);
      xi.SetProp("Password", export.Account.Password);
      xi.SetProp("EnableSSL", export.Account.EnableSSL ? "1" : "0");

      // get email info
      export.Address = cbxAddressTo.Text.Trim();
      export.CC = tbCC.Text.Trim() == "" ? null : tbCC.Text.Trim().Split(new char[] { ';' });
      export.Subject = cbxSubject.Text;
      export.MessageBody = tbMessage.Text;
      export.Export = exports[cbxAttachment.SelectedIndex];
      
      // save email info
      string addresses = "\r" + cbxAddressTo.Text + "\r";
      foreach (object obj in cbxAddressTo.Items)
      {
        string address = obj.ToString();
        if (!addresses.Contains("\r" + address + "\r"))
          addresses += address + "\r";
      }
      
      addresses = addresses.Substring(1, addresses.Length - 2);
      xi.SetProp("RecentAddresses", addresses);

      string subjects = "\r" + cbxSubject.Text + "\r";
      foreach (object obj in cbxSubject.Items)
      {
        string subject = obj.ToString();
        if (!subjects.Contains("\r" + subject + "\r"))
          subjects += subject + "\r";
      }

      subjects = subjects.Substring(1, subjects.Length - 2);
      xi.SetProp("RecentSubjects", subjects);
      
      xi.SetProp("RecentExport", cbxAttachment.SelectedIndex.ToString());
      return true;
    }

    /// <summary>
    /// Hides attachment settings.
    /// For internal use only.
    /// </summary>
    public void HideAttachmentSettings()
    {
      lblAttachment.Visible = false;
      cbxAttachment.Visible = false;
      btnSettings.Visible = false;
    }

    /// <inheritdoc/>
    public override void Localize()
    {
      base.Localize();
      
      MyRes res = new MyRes("Export,Email");
      Text = res.Get("");
      pgEmail.Text = res.Get("Email");
      lblAddressTo.Text = res.Get("Address");
      lblCC.Text = res.Get("CC");
      lblSubject.Text = res.Get("Subject");
      lblMessage.Text = res.Get("Message");
      lblAttachment.Text= res.Get("Attachment");
      btnSettings.Text = res.Get("Settings");
      pgAccount.Text = res.Get("Account");
      lblAddressFrom.Text = res.Get("Address");
      lblName.Text = res.Get("Name");
      lblTemplate.Text = res.Get("Template");
      lblHost.Text = res.Get("Host");
      lblPort.Text = res.Get("Port");
      lblUserName.Text = res.Get("UserName");
      lblPassword.Text = res.Get("Password");
      cbEnableSSL.Text = res.Get("EnableSSL");
    }

    private void EmailExportForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (DialogResult == DialogResult.OK)
        if (!Done())
          e.Cancel = true;
    }

        /// <summary>
        /// While scale > 100, multiline textboxes has wrong height
        /// </summary>
        private void FixMultilineTextboxesBottom()
        {
            tbTemplate.Height = (int)(97 / DpiHelper.GetDpiMultiplier());
            tbMessage.Height = (int)(97 / DpiHelper.GetDpiMultiplier());
        }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailExportForm"/> class.
    /// </summary>
    public EmailExportForm(EmailExport export)
    {
            this.export = export;
      InitializeComponent();
      Localize();
      Init();
            FixMultilineTextboxesBottom();
            Scale();

            tbTemplate.Height = labelLine1.Top - tbTemplate.Top;
            tbMessage.Height = cbxAttachment.Top - tbMessage.Top - 10;

            btnCancel.Location = new Point(pageControl1.Right - btnCancel.Width, pageControl1.Bottom + DpiHelper.ConvertUnits(3));
            btnOk.Location = new Point(btnCancel.Left - btnOk.Width - DpiHelper.ConvertUnits(20), btnCancel.Location.Y);
    }
  }
}

