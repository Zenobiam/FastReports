using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Forms;
using FastReport.Design.ToolWindows;
using FastReport.Data;
using FastReport.Code;
using System.Reflection;

namespace FastReport.Design.PageDesigners.Code
{
  internal class CodePageDesigner : PageDesignerBase
  {
    #region Fields
    private TextBox FEdit;
    #endregion

    #region Properties
    public TextBox Edit
    {
      get
      {
        return FEdit;
      }
    }

    public string Script
    {
      get 
      {
        return Edit.Text; 
      }  
      set 
      {
        Edit.Text = value;
      }
    }
    #endregion

    #region Private Methods
    private void CreateEdit()
    {
      FEdit = new TextBox();
      FEdit.Multiline = true;
      FEdit.AcceptsReturn = true;
      FEdit.AcceptsTab = true;
      FEdit.Font = DrawUtils.FixedFont;
      FEdit.Dock = DockStyle.Fill;
      FEdit.BorderStyle = BorderStyle.None;
      FEdit.AllowDrop = true;
      FEdit.DragOver += new DragEventHandler(Edit_DragOver);
      FEdit.DragDrop += new DragEventHandler(Edit_DragDrop);
      Controls.Add(FEdit);

      FEdit.ScrollBars = ScrollBars.Both;
      FEdit.TextChanged += new EventHandler(Edit_TextChanged);
      FEdit.ImeMode = ImeMode.On;
    }

    private void Edit_DragOver(object sender, DragEventArgs e)
    {
      int index = Edit.GetCharIndexFromPosition(Edit.PointToClient(new Point(e.X, e.Y)));
      Edit.Select(index, 0);
      e.Effect = e.AllowedEffect;
    }

    private void Edit_DragDrop(object sender, DragEventArgs e)
    {
        DictionaryWindow.DraggedItem item = DictionaryWindow.DragUtils.GetOne(e);
      
      CodeHelperBase codeHelper = Designer.ActiveReport.Report.CodeHelper;
      string text = "";
      if (item.obj is Column)
        text = codeHelper.ReplaceColumnName(item.text, (item.obj as Column).DataType);
      else if (item.obj is SystemVariable)
        text = codeHelper.ReplaceVariableName(item.obj as Parameter);
      else if (item.obj is Parameter)
        text = codeHelper.ReplaceParameterName(item.obj as Parameter);
      else if (item.obj is Total)
        text = codeHelper.ReplaceTotalName(item.text);
      else if (item.obj is MethodInfo)
        text = item.text;
      else
        text = "Report.Calc(\"" + item.text + "\")";
      
      Edit.SelectedText = text;
      Edit.Focus();
    }

    private void Edit_TextChanged(object sender, EventArgs e)
    {
      IDesignerPlugin stdToolbar = Designer.Plugins.Find("StandardToolbar");
      if (stdToolbar != null)
        stdToolbar.UpdateContent();
      IDesignerPlugin menu = Designer.Plugins.Find("MainMenu");
      if (menu != null)
        menu.UpdateContent();
    }
    
    private void CommitChanges()
    {
      if (Edit.Modified)
      {
        Edit.Modified = false;
        Designer.SetModified(this, "Script");
      }
    }

    private void LocalizeEditMenu()
    {
      //MyRes res = new MyRes("Designer,Menu,Edit");
    }
    #endregion
    
    #region Public Methods
    public void UpdateLanguage()
    {
        // do nothing
    }

    public void Copy()
    {
      Edit.Copy();
    }
    
    public void Cut()
    {
      Edit.Cut();
    }
    
    public void Paste()
    {
      Edit.Paste();
    }

    public bool CanUndo()
    {
      return Edit.CanUndo;
    }

    public bool CanRedo()
    {
      return false;
    }

    public override void FillObjects(bool resetSelection)
    {
      // do nothing
    }

    public override void PageDeactivated()
    {
      base.PageDeactivated();
      CommitChanges();
    }
    #endregion

    #region IDesignerPlugin
    public override void SaveState()
    {
      CodePageSettings.SaveState();
    }

    public override void RestoreState()
    {
    }

    public override DesignerOptionsPage GetOptionsPage()
    {
      return new CodePageOptions(this);
    }
    #endregion

    public CodePageDesigner(Designer designer) : base(designer)
    {
      Name = "Code";
      CreateEdit();
    }
  }

}
