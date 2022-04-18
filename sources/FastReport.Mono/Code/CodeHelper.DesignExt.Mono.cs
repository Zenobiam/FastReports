using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using FastReport.Data;
using FastReport.Engine;
using System.IO;
using FastReport.Utils;
using FastReport.Design.PageDesigners.Code;

namespace FastReport.Code
{
  partial class CodeHelperBase
  {
    #region Fields
    #endregion

    #region Properties
    public TextBox Editor
    {
      get { return Report.Designer.Editor.Edit; }
    }
    #endregion

    #region Protected Methods
    protected string Indent(int num)
    {
      return "".PadLeft(num * CodePageSettings.TabSize, ' ');
    }
    #endregion

    #region Public Methods
    public abstract bool AddHandler(Type eventType, string eventName);
    public abstract void LocateHandler(string eventName);
    public abstract List<string> GetEvents(Type eventType);
    
    public void Locate(int line, int column)
    {
      Editor.Focus();
      //Editor.Position = new Point(column - 1, line - 1);
    }
    #endregion
  }

}