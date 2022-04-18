using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Editor;
using FastReport.Editor.Common;
using FastReport.Editor.Syntax;
using FastReport.Utils;
using FastReport.Forms;
using FastReport.Design.ToolWindows;
using FastReport.Data;
using FastReport.Code;
using System.Reflection;
using FastReport.DevComponents;

namespace FastReport.Design.PageDesigners.Code
{
  internal class CodePageDesigner : PageDesignerBase
  {
    #region Fields
    private SyntaxEdit edit;
    private TextSource source;
    private bool editInitialized;
    private string script;

    private int defaultHintDelay;
    private int defaultCompletionDelay;
        Font defaultFont;
    #endregion

    #region Properties
    public SyntaxEdit Edit
    {
      get
      {
        if (!editInitialized)
          CreateEdit();
        return edit;
      }
    }

    public TextSource Source
    {
      get
      {
        if (!editInitialized)
          CreateEdit();
        return source;
      }
    }
    
    public string Script
    {
      get 
      {
        return editInitialized ? Edit.Source.Text : script; 
      }  
      set 
      { 
        script = value; 
        if (editInitialized)
          SetScriptText();
      }
    }
    #endregion

    #region Private Methods
    private void CreateEdit()
    {
      defaultHintDelay = SyntaxConsts.DefaultHintDelay;
      defaultCompletionDelay = SyntaxConsts.DefaultCompletionDelay;

      editInitialized = true;
      source = new TextSource();
      edit = new SyntaxEdit();
            defaultFont = edit.Font;
      edit.Dock = DockStyle.Fill;
      edit.BorderStyle = EditBorderStyle.None;
      edit.Source = source;
      edit.AllowDrop = true;
      edit.DragOver += new DragEventHandler(Edit_DragOver);
      edit.DragDrop += new DragEventHandler(Edit_DragDrop);
      Controls.Add(edit);

      // do this after controls.add!
      edit.IndentOptions = IndentOptions.AutoIndent | IndentOptions.SmartIndent;
      edit.NavigateOptions = NavigateOptions.BeyondEol;
      edit.Braces.BracesOptions = BracesOptions.Highlight | BracesOptions.HighlightBounds;
      edit.Braces.BackColor = Color.LightGray;
      edit.Braces.Style = FontStyle.Regular;
      edit.Scroll.ScrollBars = RichTextBoxScrollBars.Both;
      edit.Scroll.Options = ScrollingOptions.AllowSplitHorz | ScrollingOptions.AllowSplitVert | ScrollingOptions.SmoothScroll;
      edit.Outlining.AllowOutlining = true;
      edit.DisplayLines.UseSpaces = true;
      edit.SplitHorz += new EventHandler(Edit_SplitHorz);
      edit.SplitVert += new EventHandler(Edit_SplitVert);
      edit.TextChanged += new EventHandler(Edit_TextChanged);
      edit.ImeMode = ImeMode.On;
            if (Config.Root.FindItem("Designer").FindItem("Fonts").Find("CodePageDesigner") != -1)
            {
                XmlItem item = Config.Root.FindItem("Designer").FindItem("Fonts").FindItem("CodePageDesigner").FindItem("CodePage");
                edit.Font = new Font(item.GetProp("font-name"), DpiHelper.ParseFontSize(int.Parse(item.GetProp("font-size"))), (item.GetProp("font-italic") == "1" ? FontStyle.Italic : FontStyle.Regular) | (item.GetProp("font-bold") == "1" ? FontStyle.Bold : FontStyle.Regular));
            }
            else
            {
                Edit.Font = new Font(defaultFont.Name, DpiHelper.ParseFontSize(defaultFont.Size), defaultFont.Style);
            }

            UpdateOptions();
      UpdateEditColors();
      LocalizeEditMenu();
      SetScriptText();
    }

    private void SetScriptText()
    {
      Edit.Source.Text = script;
    }

    private void UpdateEditColors()
    {
      edit.Gutter.BrushColor = UIStyleUtils.GetControlColor(Designer.UIStyle);
      edit.Gutter.PenColor = edit.Gutter.BrushColor;
    }

    private void Edit_SplitHorz(object sender, EventArgs e)
    {
      Edit.HorzSplitEdit.BorderStyle = Edit.BorderStyle;
    }

    private void Edit_SplitVert(object sender, EventArgs e)
    {
      Edit.VertSplitEdit.BorderStyle = Edit.BorderStyle;
    }

    private void Edit_DragOver(object sender, DragEventArgs e)
    {
      DictionaryWindow.DraggedItem item = DictionaryWindow.DragUtils.GetOne(e);
      if (item != null)
      {
        Point pt = Edit.PointToClient(new Point(e.X, e.Y));
        HitTestInfo hit = new HitTestInfo();
        Edit.GetHitTest(pt, ref hit);
        if (hit.Pos != -1 && hit.Line != -1)
          Edit.Position = new Point(hit.Pos, hit.Line);
        e.Effect = e.AllowedEffect;
      }
    }

    private void Edit_DragDrop(object sender, DragEventArgs e)
    {
      DictionaryWindow.DraggedItem item = DictionaryWindow.DragUtils.GetOne(e);
      if (item == null)
        return;
      
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
      
      Edit.Selection.InsertString(text);
      Edit.Focus();
    }

    private void Edit_TextChanged(object sender, EventArgs e)
    {
      Designer.SetModified(this, "Script-no-undo");
      
      IDesignerPlugin stdToolbar = Designer.Plugins.Find("StandardToolbar");
      if (stdToolbar != null)
        stdToolbar.UpdateContent();
      IDesignerPlugin menu = Designer.Plugins.Find("MainMenu");
      if (menu != null)
        menu.UpdateContent();
      
      if(!Edit.Focused)
        Edit.Focus();
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
      MyRes res = new MyRes("Designer,Menu,Edit");
      StringConsts.MenuUndoCaption = res.Get("Undo");
      StringConsts.MenuRedoCaption = res.Get("Redo");
      StringConsts.MenuCutCaption = res.Get("Cut");
      StringConsts.MenuCopyCaption = res.Get("Copy");
      StringConsts.MenuPasteCaption = res.Get("Paste");
      StringConsts.MenuDeleteCaption = res.Get("Delete");
      StringConsts.MenuSelectAllCaption = res.Get("SelectAll");
    }
        #endregion

        #region Public Methods

        public override void ReinitDpiSize()
        {
            base.ReinitDpiSize();
            UpdateFont();
        }
        public void UpdateLanguage()
    {
      SyntaxParser parser = Designer.ActiveReport.Report.CodeHelper.Parser;
      Edit.Lexer = parser;
      Source.Lexer = parser;
      Source.FormatText();
      Designer.ActiveReport.Report.CodeHelper.RegisterAssemblies();
      Edit.Refresh();
    }

    public void UpdateOptions()
    {
      SyntaxConsts.DefaultHintDelay = CodePageSettings.EnableCodeCompletion ? defaultHintDelay : int.MaxValue;
      SyntaxConsts.DefaultCompletionDelay = CodePageSettings.EnableCodeCompletion ? defaultCompletionDelay : int.MaxValue;
      
      Edit.NavigateOptions = CodePageSettings.EnableVirtualSpace ? NavigateOptions.BeyondEol : NavigateOptions.None;
      Edit.DisplayLines.UseSpaces = CodePageSettings.UseSpaces;
      Edit.Outlining.AllowOutlining = CodePageSettings.AllowOutlining;
      Edit.DisplayLines.TabStops = new int[] { CodePageSettings.TabSize };
      edit.Gutter.Options = CodePageSettings.LineNumbers ? GutterOptions.PaintLineNumbers : GutterOptions.None;
    }

        public void UpdateFont()
        {
            if (Config.Root.FindItem("Designer").FindItem("Fonts").Find("CodePageDesigner") != -1)
            {
                XmlItem editorFont = Config.Root.FindItem("Designer").FindItem("Fonts").FindItem("CodePageDesigner").FindItem("CodePage");
                Font font = new Font(editorFont.GetProp("font-name"), DpiHelper.ParseFontSize(int.Parse(editorFont.GetProp("font-size"))), (editorFont.GetProp("font-italic") == "1" ? FontStyle.Italic : FontStyle.Regular) | (editorFont.GetProp("font-bold") == "1" ? FontStyle.Bold : FontStyle.Regular));
                if (!Edit.Font.Equals(font))
                {
                    Edit.Font = font;
                }
            }
            else
            {
                Edit.Font = new Font(defaultFont.Name, DpiHelper.ParseFontSize(defaultFont.Size), defaultFont.Style);
            }
        }

    public void Copy()
    {
      Edit.Selection.Copy();
    }
    
    public void Cut()
    {
      Edit.Selection.Cut();
    }
    
    public void Paste()
    {
      Edit.Selection.Paste();
    }

    public bool CanUndo()
    {
      return Source.CanUndo();
    }

    public bool CanRedo()
    {
      return Source.CanRedo();
    }

    public override void FillObjects(bool resetSelection)
    {
      // do nothing
    }

    public override void PageActivated()
    {
      base.PageActivated();
      UpdateOptions();
      UpdateLanguage();
    }

    public override void PageDeactivated()
    {
      base.PageDeactivated();
      if (editInitialized)
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

    public override void UpdateUIStyle()
    {
      base.UpdateUIStyle();
      if (editInitialized)
        UpdateEditColors();
    }

    public override void Localize()
    {
      base.Localize();
      if (editInitialized)
        LocalizeEditMenu();
    }
    #endregion

    public CodePageDesigner(Designer designer) : base(designer)
    {
      Name = "Code";
      RightToLeft = Config.RightToLeft ? RightToLeft.Yes : RightToLeft.No;
      //FEdit.RightToLeft = Config.RightToLeft ? RightToLeft.Yes : RightToLeft.No;
    }
  }

}
