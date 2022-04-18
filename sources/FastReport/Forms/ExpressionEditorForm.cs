using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Data;
using FastReport.Code;
using System.Reflection;
using FastReport.Controls;

namespace FastReport.Forms
{
  internal partial class ExpressionEditorForm : BaseDialogForm
  {
    private Report report;
    private static List<string> FExpandedNodes;
    
    public string ExpressionText
    {
      get { return tbText.Text; }
      set { tbText.Text = value; }
    }
    
    private string GetTextWithBrackets()
    {
      if (tvData.SelectedItemType == DataTreeSelectedItemType.Function ||
        tvData.SelectedItemType == DataTreeSelectedItemType.DialogControl)
        return tvData.SelectedItem;

      return "[" + tvData.SelectedItem + "]";
    }
    
    private void tbText_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter && e.Control)
        DialogResult = DialogResult.OK;
      else if (e.KeyData == (Keys.A | Keys.Control))
        tbText.SelectAll();
    }

    private void tvData_AfterSelect(object sender, TreeViewEventArgs e)
    {
      bool descrVisible = tvData.SelectedNode != null &&
        (tvData.SelectedNode.Tag is MethodInfo || tvData.SelectedNode.Tag is SystemVariable);
      expandableSplitter1.Visible = descrVisible;
      lblDescription.Visible = descrVisible;
      
      if (descrVisible)
        lblDescription.ShowDescription(report, tvData.SelectedNode.Tag);
    }

    private void tvData_ItemDrag(object sender, ItemDragEventArgs e)
    {
      tvData.SelectedNode = e.Item as TreeNode;
      if (tvData.SelectedItem != "")
        tvData.DoDragDrop(e.Item, DragDropEffects.Move);
      else
        tvData.DoDragDrop(e.Item, DragDropEffects.None);
    }

    private void tbText_DragOver(object sender, DragEventArgs e)
    {
      int index = tbText.GetCharIndexFromPosition(tbText.PointToClient(new Point(e.X, e.Y)));
      if (index == tbText.Text.Length - 1)
        index++;
      tbText.Focus();
      tbText.Select(index, 0);
      e.Effect = e.AllowedEffect;
    }

    private void tbText_DragDrop(object sender, DragEventArgs e)
    {
      tbText.SelectedText = GetTextWithBrackets();
      tbText.Focus();
    }

    private void tvData_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      if (tvData.SelectedItem != "")
      {
        tbText.SelectedText = GetTextWithBrackets();
        tbText.Focus();
      }
    }

    private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
    {
      tbText.Focus();
    }

    private void TextEditorForm_Shown(object sender, EventArgs e)
    {
      tbText.Focus();
      tbText.Select(0, 0);
    }

    private void TextEditorForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      Done();
    }
    
    private void Init()
    {
            if (Config.Root.FindItem("Designer").FindItem("Fonts").Find("ExpressionEditorForm") != -1)
            {
                XmlItem item = Config.Root.FindItem("Designer").FindItem("Fonts").FindItem("ExpressionEditorForm").FindItem("FormulaEditor");
                tbText.Font = new Font(item.GetProp("font-name"), int.Parse(item.GetProp("font-size")), (item.GetProp("font-italic") == "1" ? FontStyle.Italic : FontStyle.Regular) | (item.GetProp("font-bold") == "1" ? FontStyle.Bold : FontStyle.Regular));
            }
            else
                tbText.Font = DrawUtils.FixedFont;
      tvData.CreateNodes(report.Dictionary);
      if (FExpandedNodes != null)
        tvData.ExpandedNodes = FExpandedNodes;
      StartPosition = FormStartPosition.Manual;
      Config.RestoreFormState(this);
      XmlItem xi = Config.Root.FindItem("Forms").FindItem("ExpressionEditorForm");
      string s = xi.GetProp("Splitter");
      if (s != "")
        splitContainer1.SplitterDistance = int.Parse(s);
      s = xi.GetProp("DescriptionHeight");
      if (s != "")
        lblDescription.Height = int.Parse(s);
    }
    
    private void Done()
    {
      FExpandedNodes = tvData.ExpandedNodes;
      Config.SaveFormState(this);
      XmlItem xi = Config.Root.FindItem("Forms").FindItem("ExpressionEditorForm");
      xi.SetProp("Splitter", splitContainer1.SplitterDistance.ToString());
      xi.SetProp("DescriptionHeight", lblDescription.Height.ToString());
    }

    public override void Localize()
    {
      base.Localize();
      Text = Res.Get("Forms,ExpressionEditor");
    }
    
    public ExpressionEditorForm(Report report)
    {
            this.report = report;
      InitializeComponent();
      Localize();
      Init();
            Scale();
    }
  }
}

