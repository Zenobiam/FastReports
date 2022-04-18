using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Forms;

namespace FastReport.Design.PageDesigners
{
  internal class PageDesignerBase : Panel, IDesignerPlugin
  {
    #region Fields

    private Designer designer;
    private PageBase page;
    private bool locked;
    private bool isInsertingObject;

    #endregion

    #region Properties

    public PageBase Page
    {
      get { return page; }
      set { page = value; }
    }

    public Designer Designer
    {
      get { return designer; }
    }

    public bool Locked
    {
      get { return locked; }
    }

    public bool IsInsertingObject
    {
        get { return isInsertingObject; }
        set { isInsertingObject = value; }
    }
    
    public Report Report
    {
      get { return Page.Report; }
    }

    public string PluginName
    {
      get { return Name; }
    }

    #endregion

    #region Public Methods
    // this method is called when the page becomes active. You may, for example, modify the main menu
    // in this method.
    public virtual void PageActivated()
    {
    }

    // this method is called when the page becomes inactive. You should remove items added to the main menu
    // in this method.
    public virtual void PageDeactivated()
    {
    }
    
    public virtual void FillObjects(bool resetSelection)
    {
      Designer.Objects.Clear();
      Designer.Objects.Add(Page.Report);
      Designer.Objects.Add(Page);
      foreach (Base b in Page.AllObjects)
      {
        Designer.Objects.Add(b);
      }

      if (resetSelection)
      {
        Designer.SelectedObjects.Clear();
        Designer.SelectedObjects.Add(Page);
      }
    }
    
    public virtual void Paste(ObjectCollection list, InsertFrom source)
    {
    }
    
    public virtual void CancelPaste()
    {
    }
    
    public virtual void SelectAll()
    {
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
        SaveState();
    }
    #endregion

    #region IDesignerPlugin

    public virtual void Localize()
    {
    }

    public virtual void SaveState()
    {
    }

    public virtual void RestoreState()
    {
    }

    public virtual void SelectionChanged()
    {
    }

    public virtual void UpdateContent()
    {
    }

    public virtual void Lock()
    {
      locked = true;
    }

    public virtual void Unlock()
    {
      locked = false;
      UpdateContent();
    }

        public virtual void ReinitDpiSize()
        {

        }

    public virtual DesignerOptionsPage GetOptionsPage()
    {
      return null;
    }
    
    public virtual void UpdateUIStyle()
    {
    }

    #endregion

    public PageDesignerBase(Designer designer) : base()
    {
        this.designer = designer;
        isInsertingObject = false;
    }
  }
}
