using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using FastReport.Controls;
using FastReport.Code;
using FastReport.Utils;
using FastReport.Dialog;
using FastReport.Design.PageDesigners;
using FastReport.Design.PageDesigners.Code;
using FastReport.Forms;
using FastReport.Preview;
using FastReport.Data;
#if !MONO
using FastReport.DevComponents.DotNetBar;
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Design
{
    internal class ReportTab : DocumentWindow
    {
        #region Const
        private const string newPageButtonName = "+";
        #endregion

        #region Fields
        private bool disposed;
        private PageBase activePage;
#if !MONO
        private readonly TabStrip tabs;
#else
        private readonly FRTabStrip tabs;
#endif
        private bool tabMoved;
        private bool updatingTabs;
        #endregion

        #region Properties
        public Report Report { get; }

        public Designer Designer { get; }

        public BlobStore BlobStore { get; }

        public PageBase ActivePage
        {
            get { return activePage; }
            set
            {
                if (activePage != value)
                {
                    PageDesignerBase oldPd = GetPageDesigner(activePage);
                    PageDesignerBase newPd = GetPageDesigner(value);
                    oldPd.CancelPaste();
                    if (oldPd != newPd)
                        oldPd.PageDeactivated();
                    activePage = value;
#if !MONO
                    foreach (TabItem tab in tabs.Tabs)
#else
                    foreach (Tab tab in tabs.Tabs)
#endif
                    {
                        if (tab.Tag == activePage)
                        {
                            tabs.SelectedTab = tab;
                            break;
                        }
                    }
#if MONO
                    if (oldPd != newPd)
                    {
                        oldPd.Visible = false;
                        newPd.Visible = true;
                    }
#endif
                    newPd.Page = value;
                    newPd.FillObjects(true);
                    newPd.BringToFront();
                    if (oldPd != newPd)
                        newPd.PageActivated();

                    Designer.SelectionChanged(null);
                }
            }
        }

        public PageDesignerBase ActivePageDesigner
        {
            get { return GetPageDesigner(ActivePage); }
        }

        public int ActivePageIndex
        {
            get { return tabs.SelectedTabIndex; }
            set
            {
                tabs.SelectedTabIndex = value;
                ActivePage = tabs.SelectedTab.Tag as PageBase;
            }
        }

        public bool Modified { get; private set; }

        public CodePageDesigner Editor
        {
            get { return GetPageDesigner(null) as CodePageDesigner; }
        }

        public string Script
        {
            get { return Editor.Script; }
            set { Editor.Script = value; }
        }

        public string ReportName
        {
            get
            {
                if (String.IsNullOrEmpty(Report.FileName))
                    return Res.Get("Designer,Untitled");
                return Report.FileName;
            }
        }

        internal UndoRedo UndoRedo { get; }

        internal bool CanUndo
        {
            get
            {
                int i = ActivePageIndex;
                if (i == 0)
                    return Editor.CanUndo();
                return UndoRedo.UndoCount > 1;
            }
        }

        internal bool CanRedo
        {
            get
            {
                int i = ActivePageIndex;
                if (i == 0)
                    return Editor.CanRedo();
                return UndoRedo.RedoCount > 0;
            }
        }

        public PluginCollection Plugins { get; }
        #endregion

        #region Private Methods
        // (Deprecated) for better appearance, play with ui controls offset
        private Point UIOffset
        {
            get
            {
                return new Point();
            }
        }

        private PageDesignerBase GetPageDesigner(PageBase page)
        {
            Type pdType = page == null ? typeof(CodePageDesigner) : page.GetPageDesignerType();
            // try to find existing page designer
            foreach (IDesignerPlugin plugin in Plugins)
            {
                if (plugin.GetType() == pdType)
                    return plugin as PageDesignerBase;
            }
            // not found, create new one
            PageDesignerBase pd = Activator.CreateInstance(pdType, new object[] { Designer }) as PageDesignerBase;
#if !MONO
            pd.Location = new Point(UIOffset.X, 0);
            pd.Size = new Size(ParentControl.Width - UIOffset.X, ParentControl.Height - tabs.Height - UIOffset.Y);
            ParentControl.Controls.Add(pd);
#else
            pd.Location = new Point(0, 0);
            pd.Size = new Size(Width, Height - tabs.Height);
            Controls.Add(pd);
#endif
            Plugins.Add(pd);
            pd.RestoreState();
            pd.UpdateUIStyle();
            return pd;
        }

#if !MONO
        private void FTabs_TabItemClose(object sender, TabStripActionEventArgs e)
        {
            // do not allow to close the "Code" tab
            if (tabs.SelectedTab.Tag == null)
                e.Cancel = true;
            else
                Designer.cmdDeletePage.Invoke();
        }

        private void Tabs_DoubleClick(object sender, EventArgs e)
        {
            TabStrip ts = sender as TabStrip;
            if (!ts.SelectedTab.IsMouseOver && !ts._TabSystemBox.IsMouseOverArrowControls)
                NewReportPage();
        }

        private void FTabs_TabMoved(object sender, TabStripTabMovedEventArgs e)
#else
        private void FTabs_TabMoved(object sender, TabMovedEventArgs e)
#endif
        {
            // do not allow to move the "Code" tab
            if (e.OldIndex == 0 || e.NewIndex == 0)
#if !MONO
                e.Cancel = true;
#else
                ;
#endif
            else
                tabMoved = true;
        }

        private void FTabs_MouseUp(object sender, MouseEventArgs e)
        {
            if (tabs.SelectedTab.Text == newPageButtonName)
                NewReportPage();

            if (tabMoved)
            {
                // clear pages. Do not call Clear because pages will be disposed then
                while (Report.Pages.Count > 0)
                {
                    Report.Pages.RemoveAt(0);
                }
                // add pages in new order
#if !MONO
                foreach (TabItem tab in tabs.Tabs)
#else
                foreach (Tab tab in tabs.Tabs)
#endif
                {
                    if (tab.Tag is PageBase)
                        Report.Pages.Add(tab.Tag as PageBase);
                }
                Designer.SetModified(null, "ChangePagesOrder");
            }
            tabMoved = false;
        }

#if !MONO
        private void FTabs_SelectedTabChanged(object sender, TabStripTabChangedEventArgs e)
#else
        private void FTabs_SelectedTabChanged(object sender, EventArgs e)
#endif
        {
            if (updatingTabs)
                return;

            if (tabs.SelectedTab.Text != newPageButtonName)
            {
#if !MONO
                if(e.NewTab == e.OldTab)
                {
                    PageDesignerBase page = GetPageDesigner(activePage);
                    page.FillObjects(true);
                    Designer.SelectionChanged(null);
                }
                else
#endif
                    ActivePage = tabs.SelectedTab.Tag as PageBase;
            }
        }

#if !MONO
        private void ParentControl_Resize(object sender, EventArgs e)
        {
            foreach (Control c in ParentControl.Controls)
            {
                if (c is PageDesignerBase)
                    c.Size = new Size(ParentControl.Width - UIOffset.X, ParentControl.Height - tabs.Height - UIOffset.Y);
            }
        }
#else
        protected override void OnResize(EventArgs eventargs)
        {
          base.OnResize(eventargs);
          foreach (Control c in Controls)
          {
            if (c is PageDesignerBase)
              c.Size = new Size(Width, Height - tabs.Height);
          }
        }
#endif

#endregion

#region Protected Methods
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposed)
            {
                for (int i = 0; i < Plugins.Count; i++)
                {
                    IDesignerPlugin plugin = Plugins[i];
                    if (plugin is IDisposable)
                        (plugin as IDisposable).Dispose();
                }
                if (Designer.MdiMode && Report != null)
                    Report.Dispose();
                UndoRedo.Dispose();
                if (BlobStore != null)
                    BlobStore.Dispose();
                disposed = true;
            }
        }

#endregion

#region Public Methods
        public bool CanClose()
        {
            ActivePageDesigner.PageDeactivated();
            if (Designer.AskSave && !SaveCurrentFile())
            {
                ActivePageDesigner.PageActivated();
                return false;
            }
            return true;
        }

        internal void ReportActivated()
        {
            ActivePageDesigner.FillObjects(true);
            ActivePageDesigner.PageActivated();
            UpdateCaption();
        }

        internal void ReportDeactivated()
        {
            ActivePageDesigner.PageDeactivated();
        }

        internal void SetModified()
        {
            Modified = true;
            UpdateCaption();
        }

        internal void SetModified(object sender, string action, string objName)
        {
            // cancel insertion if user has changed some properties (#2573)
            if (ActivePageDesigner.IsInsertingObject && action == "Change")
            {
                ActivePageDesigner.CancelPaste();
                Designer.SelectedObjects.Clear();
                Designer.SelectedObjects.Add(ActivePageDesigner.Page);
                Designer.SelectionChanged(null);
            }

            Modified = true;
            Report.ScriptText = Script;
            if (action != "Script-no-undo")
            {
                UndoRedo.AddUndo(action, objName);
                UndoRedo.ClearRedo();
            }
            bool resetSelection = action == "Delete" ? true : false;
            ActivePageDesigner.FillObjects(resetSelection);
            InitPages(ActivePageIndex);
            UpdateCaption();
        }

        internal void ResetModified()
        {
            Modified = false;
            UndoRedo.ClearUndo();
            UndoRedo.ClearRedo();
            UndoRedo.AddUndo("Load", null);
            Editor.Edit.Modified = false;
            UpdateCaption();
        }

        public void SwitchToCode()
        {
            ActivePage = null;
        }

        internal void UpdateCaption()
        {
            Text = ReportName;
            string titleText = "";
            if (String.IsNullOrEmpty(Config.DesignerSettings.Text))
                titleText = "FastReport - " + ReportName;
            else
                titleText = Config.DesignerSettings.Text + ReportName;

            if (Designer.cmdSave.Enabled)
                titleText += "*";

            Form form = Designer.FindForm();
            if (form != null && (form.GetType().Name.EndsWith("DesignerForm") || form.GetType().Name.EndsWith("DesignerRibbonForm")))
                form.Text = titleText;
        }

        public void InitReport()
        {
            // check if report has a reportpage
            bool reportPageExists = false;
            foreach (PageBase page in Report.Pages)
            {
                if (page is ReportPage)
                    reportPageExists = true;
            }

            // if it has not, create the page 
            if (!reportPageExists)
            {
                ReportPage reportPage = new ReportPage();
                Report.Pages.Add(reportPage);
                reportPage.CreateUniqueName();
                reportPage.SetDefaults();
            }

            Script = Report.ScriptText;
            InitPages(1);
            UpdateCaption();
            ResetModified();
            Plugins.Unlock();
        }

        public void InitPages(int index)
        {
            updatingTabs = true;

            // add tabs
            tabs.Tabs.Clear();

            // code tab
#if !MONO
            TabItem codeTab = new TabItem();
#else
            Tab codeTab = new Tab();
            codeTab.AllowMove = false;
            codeTab.CloseButton = false;
#endif
            codeTab.Text = Res.Get("Designer,Workspace,Code");
            codeTab.Image = Res.GetImage(61);
#if !MONO
            codeTab.ImageIndex = 61;
#endif
            tabs.Tabs.Add(codeTab);

            if (Designer.Restrictions.DontEditCode)
                codeTab.Visible = false;

            // page tabs
            foreach (PageBase page in Report.Pages)
            {
#if !MONO
                TabItem pageTab = new TabItem();
#else
                Tab pageTab = new Tab();
                pageTab.CloseButton = false;
#endif
                pageTab.Tag = page;
                pageTab.Text = page.PageName;
                ObjectInfo info = RegisteredObjects.FindObject(page);
#if !MONO
                pageTab.ImageIndex = info.ImageIndex;
#endif
                pageTab.Image = info.Image;
                tabs.Tabs.Add(pageTab);
            }

            // "+" button
#if !MONO
            TabItem newPage = new TabItem();
#else
            Tab newPage = new Tab();
            newPage.CloseButton = false;
#endif
            newPage.Text = newPageButtonName;
            tabs.Tabs.Add(newPage);

            updatingTabs = false;
            ActivePageIndex = index;
            tabs.Refresh();
        }

        public void Localize()
        {
            Plugins.Localize();
            InitPages(ActivePageIndex);
        }

        public void UpdateUIStyle()
        {
#if !MONO
            tabs.Style = UIStyleUtils.GetTabStripStyle1(Designer.UIStyle);

            //HACK
            if (StyleManager.Style == eStyle.VisualStudio2012Light)
            {
                tabs.ColorScheme.TabItemHotText = Color.White;
                tabs.ColorScheme.TabItemSelectedText = Color.White;
            }
            else
            {
                tabs.ColorScheme = new TabColorScheme(tabs.Style);
            }

            Plugins.UpdateUIStyle();
            ParentControl_Resize(this, EventArgs.Empty);
#else
            tabs.Style = Designer.UIStyle;
            Plugins.UpdateUIStyle();
#endif
        }

#if !MONO
        public void UpdateDpiDependencies()
        {
            tabs.Height = DpiHelper.ConvertUnits(30);
            tabs.Margin = DpiHelper.ConvertUnits(tabs.Margin);
            tabs.Font = DpiHelper.ConvertUnits(DrawUtils.DefaultFont, true);
            foreach(TabItem tab in tabs.Tabs)
            {
                if(tab.Image != null)
                    tab.Image = Res.GetImage(tab.ImageIndex);
            }
            tabs.RecalcSize();
        }
#endif
#endregion

#region Designer Commands
        internal bool SaveCurrentFile()
        {
            if (Modified)
            {
                DialogResult res = AskSave();
                if (res == DialogResult.Cancel)
                    return false;
                if (Designer.IsPreviewPageDesigner)
                {
                    if (res == DialogResult.No)
                        Designer.Modified = false;
                }
                else
                {
                    if (res == DialogResult.Yes)
                        return SaveFile(false);
                    else if (res == DialogResult.No)
                        Designer.Modified = false;
                }
            }
            return true;
        }

        internal bool EmptyReport(bool askSave)
        {
            if (askSave)
            {
                if (!SaveCurrentFile())
                    return false;
            }
            Designer.Lock();
            try
            {
                Report.FileName = "";
                Report.Clear();
                Report.Dictionary.ReRegisterData();
                Config.DesignerSettings.OnReportLoaded(this, new ReportLoadedEventArgs(Report));
            }
            finally
            {
                InitReport();
                Designer.Unlock();
            }
            return true;
        }

        private DialogResult AskSave()
        {
            string text = Designer.IsPreviewPageDesigner ?
              Res.Get("Messages,SaveChangesToPreviewPage") : String.Format(Res.Get("Messages,SaveChanges"), ReportName);
            return FRMessageBox.Confirm(text, MessageBoxButtons.YesNoCancel);
        }

        public bool LoadFile(string fileName)
        {
            OpenSaveDialogEventArgs e = new OpenSaveDialogEventArgs(Designer);

            // empty fileName: user pressed "Open" button, show open dialog.
            // non-empty fileName: user choosed a report from recent file list, just load the specified report.
            if (String.IsNullOrEmpty(fileName))
            {
                Config.DesignerSettings.OnCustomOpenDialog(Designer, e);
                if (e.Cancel)
                    return false;
                fileName = e.FileName;
            }

            bool result = SaveCurrentFile();
            if (result)
            {
                try
                {
                    Designer.Lock();
                    OpenSaveReportEventArgs e1 = new OpenSaveReportEventArgs(Report, fileName, e.Data, false);
                    Config.DesignerSettings.OnCustomOpenReport(Designer, e1);
                    Report.FileName = fileName;

                    if (Path.GetExtension(fileName).ToLower() == ".frx")
                    {
                        Designer.cmdRecentFiles.Update(fileName);
                    }
                    else
                    {
                        Report.FileName = Path.ChangeExtension(fileName, ".frx");
                    }

                    Config.DesignerSettings.OnReportLoaded(this, new ReportLoadedEventArgs(Report));
                    result = true;
                }
#if !DEBUG
                catch (Exception ex)
                {
                  EmptyReport(false);
                  FRMessageBox.Error(String.Format(Res.Get("Messages,CantLoadReport") + "\r\n" + ex.Message, fileName));
                  result = false;
                }
#endif
                finally
                {
                    InitReport();
                    Designer.Unlock();
                }
            }

            return result;
        }

        public bool LoadAutoSaveFile(string filePath)
        {
            OpenSaveDialogEventArgs e = new OpenSaveDialogEventArgs(Designer);

            string fileName = Config.AutoSaveFile;

            bool result = SaveCurrentFile();
            if (result)
            {
                try
                {
                    Designer.Lock();
                    OpenSaveReportEventArgs e1 = new OpenSaveReportEventArgs(Report, fileName, e.Data, false);
                    Config.DesignerSettings.OnCustomOpenReport(Designer, e1);
                    Report.FileName = filePath;
                    Config.DesignerSettings.OnReportLoaded(this, new ReportLoadedEventArgs(Report));
                    result = true;
                }
#if !DEBUG
                catch (Exception ex)
                {
                  EmptyReport(false);
                  FRMessageBox.Error(String.Format(Res.Get("Messages,CantLoadReport") + "\r\n" + ex.Message, fileName));
                  result = false;
                }
#endif
                finally
                {
                    InitReport();
                    Designer.Unlock();
                }
            }

            return result;
        }

        public bool SaveFile(bool saveAs)
        {
            if (File.Exists(Config.AutoSaveFileName))
                File.Delete(Config.AutoSaveFileName);
            if (File.Exists(Config.AutoSaveFile))
                File.Delete(Config.AutoSaveFile);

            // update report's script
            Report.ScriptText = Script;

            while (true)
            {
                OpenSaveDialogEventArgs e = new OpenSaveDialogEventArgs(Designer);
                string fileName = Report.FileName;

                // show save dialog in case of untitled report or "save as"
                if (saveAs || String.IsNullOrEmpty(fileName))
                {
                    if (String.IsNullOrEmpty(fileName))
                        fileName = Res.Get("Designer,Untitled");
                    else
                        fileName = Path.GetFileName(fileName);

                    e.FileName = fileName;
                    Config.DesignerSettings.OnCustomSaveDialog(Designer, e);
                    if (e.Cancel)
                        return false;

                    fileName = e.FileName;
                }

                OpenSaveReportEventArgs e1 = new OpenSaveReportEventArgs(Report, fileName, e.Data, e.IsPlugin);

                try
                {
                    Config.DesignerSettings.OnCustomSaveReport(Designer, e1);
                    // don't change the report name if plugin was used
                    if (e.IsPlugin)
                        fileName = Report.FileName;
                    Report.FileName = fileName;
                    Modified = false;
                    Designer.UpdatePlugins(null);
                    if (!e.IsPlugin)
                        Designer.cmdRecentFiles.Update(fileName);
                    UpdateCaption();
                    return true;
                }
                catch
                {
                    // something goes wrong, suggest to save to another place
                    FRMessageBox.Error(Res.Get("Messages,CantSaveReport"));
                    saveAs = true;
                }
            }
        }

        public bool AutoSaveFile()
        {
            bool result = false;

            Report.ScriptText = Script;

            OpenSaveDialogEventArgs e = new OpenSaveDialogEventArgs(Designer);
            OpenSaveReportEventArgs e1 = new OpenSaveReportEventArgs(Report, Config.AutoSaveFile, e.Data, e.IsPlugin);

            try
            {
                Designer.Lock();
                Designer.PropertiesWindow.Saving = true;
                Directory.CreateDirectory(Config.AutoSaveFolder);

                using (FileStream f = new FileStream(Config.AutoSaveFile, FileMode.Create))
                    Report.Save(f);

                File.WriteAllText(Config.AutoSaveFileName, Report.FileName);

                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                Designer.Unlock();
                Designer.PropertiesWindow.Saving = false;
            }

            return result;
        }

        public void Preview()
        {
            ActivePageDesigner.CancelPaste();
            int i = ActivePageIndex;
            Report.ScriptText = Script;
            Designer.MessagesWindow.ClearMessages();
            UndoRedo.AddUndo("Preview", "Report");
            Designer.Lock();

            Designer.PropertiesWindow.SaveState();
            bool saveProgress = Config.ReportSettings.ShowProgress;
            Config.ReportSettings.ShowProgress = true;

            try
            {
                if (Report.Prepare())
                    Config.DesignerSettings.OnCustomPreviewReport(Report, EventArgs.Empty);
            }
#if !DEBUG
            catch (Exception e)
            {
                if (!(e is CompilerException))
                {
                    FRMessageBox.Error(e.Message);
                    //using (ExceptionForm form = new ExceptionForm(e))
                    //{            
                    //  form.ShowDialog();
                    //}
                }
                else
                    Designer.MessagesWindow.Show();
            }
#endif
            finally
            {
                Config.ReportSettings.ShowProgress = saveProgress;

                Image previewPicture = null;
                if (Report.ReportInfo.SavePreviewPicture)
                {
                    previewPicture = Report.ReportInfo.Picture;
                    Report.ReportInfo.Picture = null;
                }
                UndoRedo.GetUndo(1);
                UndoRedo.ClearRedo();
                if (Report.ReportInfo.SavePreviewPicture)
                    Report.ReportInfo.Picture = previewPicture;
                // clear dead objects
                Designer.Objects.Clear();
                Designer.SelectedObjects.Clear();

                InitPages(i);
                Designer.PropertiesWindow.RestoreState();
                Designer.Unlock();
            }
        }

        public void Undo(int actionsCount)
        {
            int i = ActivePageIndex;
            if (i == 0)
            {
#if !MONO
                Editor.Source.Undo();
#else
                Editor.Edit.Undo();
#endif
                return;
            }

            Designer.Lock();
            Report.ScriptText = Script;
            UndoRedo.GetUndo(actionsCount);
            Script = Report.ScriptText;
            InitPages(i);
            Designer.Unlock();
        }

        public void Redo(int actionsCount)
        {
            int i = ActivePageIndex;
            if (i == 0)
            {
#if !MONO
                Editor.Source.Redo();
#endif
                return;
            }

            Designer.Lock();
            UndoRedo.GetRedo(actionsCount);
            Script = Report.ScriptText;
            InitPages(i);
            Designer.Unlock();
        }

        public void NewReportPage()
        {
            ReportPage page = new ReportPage();
            Report.Pages.Add(page);
            page.SetDefaults();
            page.CreateUniqueName();
            InitPages(Report.Pages.Count);
            Designer.SetModified(this, "AddPage");
        }

        public void NewDialog()
        {
            DialogPage page = new DialogPage();
            Report.Pages.Add(page);
            page.SetDefaults();
            page.CreateUniqueName();
            InitPages(Report.Pages.Count);
            Designer.SetModified(this, "AddPage");
        }

        public void DeletePage()
        {
            Designer.Lock();
            Report.Pages.Remove(ActivePage);
            foreach (Base obj in ActivePage.AllObjects)
            {
                if (obj is SubreportObject)
                    obj.Delete();
            }
            ActivePage.Delete();
            InitPages(Report.Pages.Count);
            Designer.Unlock();
            Designer.SetModified(this, "DeletePage");
        }

        public void CopyPage()
        {
            Designer.Lock();
            ReportPage newPage = new ReportPage();
            newPage.AssignAll(ActivePage);
            Report.Pages.Add(newPage);
            newPage.CreateUniqueName();
            foreach (object obj in newPage.AllObjects)
            {
                if (obj is Base)
                {
                    (obj as Base).CreateUniqueName();
                }
            }
            InitPages(Report.Pages.Count);
            Designer.Unlock();
            Designer.SetModified(this, "CopyPage");
        }

#if !MONO
        public void CtrlTab()
        {
            if (!tabs.SelectNextTab()) tabs.SelectedTabIndex = 0;
        }

        public void CtrlShiftTab()
        {
            tabs.SelectPreviousTab();
        }
#endif

#endregion

        public ReportTab(Designer designer, Report report)
            : base()
        {
            this.Designer = designer;
            this.Report = report;
            Plugins = new PluginCollection(this.Designer);
            UndoRedo = new UndoRedo(this);
            if (!designer.IsPreviewPageDesigner)
                BlobStore = new BlobStore(false);

#if !MONO
            tabs = new TabStrip();
            tabs.Height = DpiHelper.ConvertUnits(30);
            tabs.Dock = DockStyle.Bottom;
            tabs.TabAlignment = eTabStripAlignment.Bottom;
            tabs.TabLayoutType = eTabLayoutType.FixedWithNavigationBox;
            tabs.AutoHideSystemBox = true;
            tabs.ShowFocusRectangle = false;
            tabs.CloseButtonVisible = false;
            tabs.Margin = DpiHelper.ConvertUnits(tabs.Margin);

            tabs.TabItemClose += new TabStrip.UserActionEventHandler(FTabs_TabItemClose);
            tabs.TabMoved += new TabStrip.TabMovedEventHandler(FTabs_TabMoved);
            tabs.MouseUp += new MouseEventHandler(FTabs_MouseUp);
            tabs.SelectedTabChanged += new TabStrip.SelectedTabChangedEventHandler(FTabs_SelectedTabChanged);
            tabs.DoubleClick += Tabs_DoubleClick;

            ParentControl.Controls.Add(tabs);
            ParentControl.Resize += new EventHandler(ParentControl_Resize);
#else
            tabs = new FRTabStrip();
            tabs.Dock = DockStyle.Bottom;
            tabs.TabOrientation = TabOrientation.Bottom;
            tabs.Height = 24;
            tabs.TabMoved += FTabs_TabMoved;
            tabs.MouseUp += FTabs_MouseUp;
            tabs.SelectedTabChanged += FTabs_SelectedTabChanged;
      
            Controls.Add(tabs);
#endif

            InitReport();
        }
    }
}
