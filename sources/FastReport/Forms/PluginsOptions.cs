using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Design;
#if !MONO
using FastReport.Design.Toolbars;
using FastReport.DevComponents.DotNetBar;
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Forms
{
  internal partial class PluginsOptions : DesignerOptionsPage
  {
    private Designer designer;
    
    private void Localize()
    {
      MyRes res = new MyRes("Forms,UIOptions");
      tab1.Text = res.Get("");
      lblUIStyle.Text = res.Get("Appearance");
      cbxUIStyle.Items.AddRange(UIStyleUtils.UIStyleNames);
      cbxRibbon.Text = res.Get("Ribbon");
      cbxWelcome.Text = res.Get("Welcome");
      cbxDisableHotkeys.Text = res.Get("DisableHotkeys");
      cbxDisableHDPI.Text = res.Get("DisableHDPI");
      res = new MyRes("Forms,PluginsOptions");
      tab2.Text = res.Get("");
      lblHint.Text = res.Get("Hint");
      btnAdd.Text = res.Get("Add");
      btnRemove.Text = res.Get("Remove");
      lblNote.Text = res.Get("Note");
      fontsButton.Text = res.Get("Fonts");
      btnExportsMenuEdit.Text = res.Get("ExportsEditor");

      res = new MyRes("Forms,UIOptions,RightToLeft");
      lblRightToLeft.Text = res.Get("");
      cbxRightToLeft.Items.Add(res.Get("Auto"));
      cbxRightToLeft.Items.Add(res.Get("No"));
      cbxRightToLeft.Items.Add(res.Get("Yes"));
      cbxRightToLeft.SelectedIndex = 0;
    }
    
    private void AddPlugin(string name)
    {
      lbPlugins.Items.Add(name);
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      using (OpenFileDialog dialog = new OpenFileDialog())
      {
        dialog.Filter = Res.Get("FileFilters,Assembly");
        if (dialog.ShowDialog() == DialogResult.OK)
        {
          AddPlugin(dialog.FileName);
        }
      }
    }

    private void btnRemove_Click(object sender, EventArgs e)
    {
      lbPlugins.Items.Remove(lbPlugins.SelectedItem);
    }

    private void btnUp_Click(object sender, EventArgs e)
    {
      int index = lbPlugins.SelectedIndex;
      if (index > 0)
      {
        Object item = lbPlugins.SelectedItem;
        lbPlugins.Items.Remove(item);
        lbPlugins.Items.Insert(index - 1, item);
        lbPlugins.SelectedItem = item;
      }
    }

    private void btnDown_Click(object sender, EventArgs e)
    {
      int index = lbPlugins.SelectedIndex;
      if (index < lbPlugins.Items.Count - 1)
      {
        Object item = lbPlugins.SelectedItem;
        lbPlugins.Items.Remove(item);
        lbPlugins.Items.Insert(index + 1, item);
        lbPlugins.SelectedItem = item;
      }
    }

    private void lbPlugins_SelectedIndexChanged(object sender, EventArgs e)
    {
      bool enabled = lbPlugins.SelectedItems.Count != 0;
      btnRemove.Enabled = enabled;
      btnUp.Enabled = enabled;
      btnDown.Enabled = enabled;
    }

    private void lbPlugins_DrawItem(object sender, DrawItemEventArgs e)
    {
      e.DrawBackground();
      if (e.Index >= 0)
      {
        e.Graphics.DrawImage(Res.GetImage(89), e.Bounds.Left, e.Bounds.Top + DpiHelper.ConvertUnits(1));
        TextRenderer.DrawText(e.Graphics, lbPlugins.Items[e.Index].ToString(), e.Font,
          new Rectangle(e.Bounds.Left + DpiHelper.ConvertUnits(20), e.Bounds.Top + DpiHelper.ConvertUnits(2), e.Bounds.Width - DpiHelper.ConvertUnits(20), e.Bounds.Height), e.ForeColor,
          TextFormatFlags.PathEllipsis);
      }  
    }

    private void SetConfigRightToLeft()
    {
        switch (cbxRightToLeft.SelectedIndex)
        {
            case 0:
                Config.RightToLeft = CultureInfo.CurrentCulture.TextInfo.IsRightToLeft;
                break;
            case 1:
                Config.RightToLeft = false;
                break;
            case 2:
                Config.RightToLeft = true;
                break;
            default:
                Config.RightToLeft = false;
                break;
        }
    }

    private void LoadRightToLeft()
    {
        XmlItem uiOptions = Config.Root.FindItem("UIOptions");
        string rtl = uiOptions.GetProp("RightToLeft");

        if (!String.IsNullOrEmpty(rtl))
        {
            switch (rtl)
            {
                case "Auto":
                    cbxRightToLeft.SelectedIndex = 0;
                    break;
                case "No":
                    cbxRightToLeft.SelectedIndex = 1;
                    break;
                case "Yes":
                    cbxRightToLeft.SelectedIndex = 2;
                    break;
                default:
                    cbxRightToLeft.SelectedIndex = 1;
                    break;
            }
            SetConfigRightToLeft();
        }
    }

    private void SaveRightToLeft()
    {
        XmlItem uiOptions = Config.Root.FindItem("UIOptions");

        switch (cbxRightToLeft.SelectedIndex)
        {
            case 0:
                uiOptions.SetProp("RightToLeft", "Auto");
                break;
            case 1:
                uiOptions.SetProp("RightToLeft", "No");
                break;
            case 2:
                uiOptions.SetProp("RightToLeft", "Yes");
                break;
            default:
                uiOptions.SetProp("RightToLeft", "No");
                break;
        }
        SetConfigRightToLeft();
    }

    public override void Init()
    {
      cbxUIStyle.SelectedIndex = (int)designer.UIStyle;
#if COMMUNITY
            btnExportsMenuEdit.Visible = false;
            cbxRibbon.Visible = false;
#endif
            cbxDisableHDPI.Visible = false;
#if MONO
      cbxRibbon.Enabled = false;
      cbxWelcome.Enabled = false;
      cbxWelcome.Enabled = false;
      cbxDisableHotkeys.Enabled = false;
            //cbxDisableHDPI.Enabled = false;
//      cbxDisableBacklight.Enabled = false;
#else
            //cbxDisableHDPI.Checked = Config.DisableHighDpi;
            cbxRibbon.Checked = Config.UseRibbon;
      cbxWelcome.Checked = Config.WelcomeShowOnStartup;
      cbxWelcome.Visible = Config.WelcomeEnabled;
      cbxDisableHotkeys.Checked = Config.DisableHotkeys;
#endif      
      btnUp.Image = Res.GetImage(208);
      btnDown.Image = Res.GetImage(209);

      XmlItem pluginsItem = Config.Root.FindItem("Plugins");
      for (int i = 0; i < pluginsItem.Count; i++)
      {
        XmlItem xi = pluginsItem[i];
        AddPlugin(xi.GetProp("Name"));
      }
      
      lblNote.Width = tab2.Width - lblNote.Left * 2;
      lblNote.Height = tab2.Height - lblNote.Top;
      lbPlugins_SelectedIndexChanged(this, EventArgs.Empty);
      
      LoadRightToLeft(); // load and apply Right to Left option
    }

    public override void Done(DialogResult result)
    {
        if (result == DialogResult.OK)
        {
#if !MONO
            //HACK
            if (cbxRibbon.Checked == true && Config.UseRibbon == false)
            {
                foreach (Bar bar in designer.DotNetBarManager.Bars)
                    if (bar is ToolbarBase)
                        bar.Hide();
            }
            else if (cbxRibbon.Checked == false && Config.UseRibbon == true)
            {
                foreach (Bar bar in designer.DotNetBarManager.Bars)
                    if (bar is ToolbarBase)
                        bar.Show();
            }
        
            Config.WelcomeShowOnStartup = cbxWelcome.Checked;
            Config.UseRibbon = cbxRibbon.Checked;
            Config.DisableHotkeys = cbxDisableHotkeys.Checked;
                //Config.DisableHighDpi = cbxDisableHDPI.Checked;
#endif
            Config.UIStyle = (UIStyle)cbxUIStyle.SelectedIndex;
            designer.UIStyle = (UIStyle)cbxUIStyle.SelectedIndex;
            //(FDesigner.FindForm() as DesignerRibbonForm).UpdateUIStyle();
        
            XmlItem pluginsItem = Config.Root.FindItem("Plugins");
            pluginsItem.Clear();
        
            foreach (object item in lbPlugins.Items)
            {
                XmlItem xi = pluginsItem.Add();
                xi.Name = "Plugin";
                xi.SetProp("Name", item.ToString());
            }

            SaveRightToLeft(); // save and apply Right to Left option
        }
    }

    public PluginsOptions(Designer designer) : base()
    {
      this.designer = designer;
      InitializeComponent();
      Localize();
            Scale();
    }

    private void fontsButton_Click(object sender, EventArgs e)
    {
        FormsFonts fontsForm = new FormsFonts();
        if (fontsForm.ShowDialog(this) == DialogResult.OK)
        {
#if !MONO
            designer.ActiveReportTab.Editor.UpdateFont();
#endif
        }
    }

        
    private void BtnExportsMenuEdit_Click(object sender, EventArgs e)
    {
#if !COMMUNITY
      using (ExportsOptionsEditorForm exportsEditorForm = new ExportsOptionsEditorForm())
      {
        exportsEditorForm.ShowDialog();
      }
#endif
        }

    }
}

