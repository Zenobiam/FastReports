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
using FastReport.Design.Toolbars;
using FastReport.Design.ImportPlugins.RTF;

namespace FastReport.Design.StandardDesigner
{
  /// <summary>
  /// Represents the designer's main menu.
  /// </summary>
  /// <remarks>
  /// To get this menu, use the following code:
  /// <code>
  /// Designer designer;
  /// DesignerMenu menu = designer.Plugins.FindType("DesignerMenu") as DesignerMenu;
  /// </code>
  /// </remarks>
  [ToolboxItem(false)]
  public class DesignerMenu : MenuStrip, IDesignerPlugin
  {
    #region Fields
    private Designer FDesigner;

    /// <summary>
    /// The "File" menu.
    /// </summary>
    public ToolStripMenuItem miFile;

    /// <summary>
    /// The "File|New..." menu.
    /// </summary>
    public ToolStripMenuItem miFileNew;

    /// <summary>
    /// The "File|Open..." menu.
    /// </summary>
    public ToolStripMenuItem miFileOpen;

    /// <summary>
    /// The "File|Import..." menu.
    /// </summary>
    public ToolStripMenuItem miFileImport;

    /// <summary>
    /// The "File|Close" menu.
    /// </summary>
    public ToolStripMenuItem miFileClose;

    /// <summary>
    /// The "File|Save" menu.
    /// </summary>
    public ToolStripMenuItem miFileSave;

    /// <summary>
    /// The "File|Save as..." menu.
    /// </summary>
    public ToolStripMenuItem miFileSaveAs;

    /// <summary>
    /// The "File|Save All" menu.
    /// </summary>
    public ToolStripMenuItem miFileSaveAll;

    /// <summary>
    /// The "File|Page Setup..." menu.
    /// </summary>
    public ToolStripMenuItem miFilePageSetup;

    /// <summary>
    /// The "File|Printer Setup..." menu.
    /// </summary>
    public ToolStripMenuItem miFilePrinterSetup;

    /// <summary>
    /// The "File|Preview..." menu.
    /// </summary>
    public ToolStripMenuItem miFilePreview;

    /// <summary>
    /// The "File|Select Language" menu.
    /// </summary>
    public ToolStripMenuItem miFileSelectLanguage;

    /// <summary>
    /// The "File|Exit" menu.
    /// </summary>
    public ToolStripMenuItem miFileExit;

    /// <summary>
    /// The "Edit" menu.
    /// </summary>
    public ToolStripMenuItem miEdit;

    /// <summary>
    /// The "Edit|Undo" menu.
    /// </summary>
    public ToolStripMenuItem miEditUndo;

    /// <summary>
    /// The "Edit|Redo" menu.
    /// </summary>
    public ToolStripMenuItem miEditRedo;

    /// <summary>
    /// The "Edit|Cut" menu.
    /// </summary>
    public ToolStripMenuItem miEditCut;

    /// <summary>
    /// The "Edit|Copy" menu.
    /// </summary>
    public ToolStripMenuItem miEditCopy;

    /// <summary>
    /// The "Edit|Paste" menu.
    /// </summary>
    public ToolStripMenuItem miEditPaste;

    /// <summary>
    /// The "Edit|Delete" menu.
    /// </summary>
    public ToolStripMenuItem miEditDelete;

    /// <summary>
    /// The "Edit|Delete Page" menu.
    /// </summary>
    public ToolStripMenuItem miEditDeletePage;

    /// <summary>
    /// The "Edit|Select All" menu.
    /// </summary>
    public ToolStripMenuItem miEditSelectAll;

    /// <summary>
    /// The "Edit|Group" menu.
    /// </summary>
    public ToolStripMenuItem miEditGroup;

    /// <summary>
    /// The "Edit|Ungroup" menu.
    /// </summary>
    public ToolStripMenuItem miEditUngroup;

    /// <summary>
    /// The "Edit|Find..." menu.
    /// </summary>
    public ToolStripMenuItem miEditFind;

    /// <summary>
    /// The "Edit|Replace..." menu.
    /// </summary>
    public ToolStripMenuItem miEditReplace;


    /// <summary>
    /// The "View" menu.
    /// </summary>
    public ToolStripMenuItem miView;

    /// <summary>
    /// The "View|Toolbars" menu.
    /// </summary>
    public ToolStripMenuItem miViewToolbars;

    /// <summary>
    /// The "View|Start Page" menu.
    /// </summary>
    public ToolStripMenuItem miViewStartPage;

    /// <summary>
    /// The "View|Messages" menu.
    /// </summary>
    public ToolStripMenuItem miViewMessages;

	/// <summary>
	/// The "View|Options..." menu.
	/// </summary>
	public ToolStripMenuItem miViewOptions;

    /// <summary>
    /// The "Insert" menu.
    /// </summary>
    public ToolStripMenuItem miInsert;


    /// <summary>
    /// The "Report" menu.
    /// </summary>
    public ToolStripMenuItem miReport;

    /// <summary>
    /// The "Report|Options..." menu.
    /// </summary>
    public ToolStripMenuItem miReportOptions;


    /// <summary>
    /// The "Data" menu.
    /// </summary>
    public ToolStripMenuItem miData;

    /// <summary>
    /// The "Data|Choose Report Data..." menu.
    /// </summary>
    public ToolStripMenuItem miDataChoose;

    /// <summary>
    /// The "Data|Add Data Source..." menu.
    /// </summary>
    public ToolStripMenuItem miDataAdd;

    /// <summary>
    /// The "Data|Show Data Dictionary" menu.
    /// </summary>
    public ToolStripMenuItem miDataShowData;


    /// <summary>
    /// The "Window" menu.
    /// </summary>
    public ToolStripMenuItem miWindow;

    /// <summary>
    /// The "Window|Close All" menu.
    /// </summary>
    public ToolStripMenuItem miWindowCloseAll;


    /// <summary>
    /// The "Help" menu.
    /// </summary>
    public ToolStripMenuItem miHelp;

    /// <summary>
    /// The "Help|Help Contents..." menu.
    /// </summary>
    public ToolStripMenuItem miHelpContents;

    /// <summary>
    /// The "Help|About..." menu.
    /// </summary>
    public ToolStripMenuItem miHelpAbout;
    #endregion

    #region Properties
    internal Designer Designer
    {
      get { return FDesigner; }
    }

    /// <inheritdoc/>
    public string PluginName
    {
      get { return Name; }
    }
    #endregion

    #region Private Methods
    private void UpdateControls()
    {
      miFileNew.Enabled = Designer.cmdNew.Enabled;
      miFileOpen.Enabled = Designer.cmdOpen.Enabled;
      miFileClose.Enabled = Designer.cmdClose.Enabled;
      miFileClose.Visible = Designer.MdiMode;
      miFileSave.Enabled = Designer.cmdSave.Enabled;
      miFileSaveAs.Enabled = Designer.cmdSaveAs.Enabled;
      miFileSaveAll.Visible = Designer.MdiMode;
      miFileSaveAll.Enabled = Designer.cmdSaveAll.Enabled;
      miFilePageSetup.Enabled = Designer.cmdPageSetup.Enabled;
      miFilePrinterSetup.Enabled = Designer.cmdPrinterSetup.Enabled;
      miFilePreview.Enabled = Designer.cmdPreview.Enabled;
      miEditUndo.Enabled = Designer.cmdUndo.Enabled;
      miEditRedo.Enabled = Designer.cmdRedo.Enabled;
      miEditCut.Enabled = Designer.cmdCut.Enabled;
      miEditCopy.Enabled = Designer.cmdCopy.Enabled;
      miEditDeletePage.Enabled = Designer.cmdDeletePage.Enabled;
      miEditDelete.Enabled = Designer.cmdDelete.Enabled;
      miEditSelectAll.Enabled = Designer.cmdSelectAll.Enabled;
      miEditGroup.Enabled = Designer.cmdGroup.Enabled;
      miEditUngroup.Enabled = Designer.cmdUngroup.Enabled;
      miEditFind.Enabled = Designer.cmdFind.Enabled;
      miEditReplace.Enabled = Designer.cmdReplace.Enabled;
      miInsert.Visible = Designer.cmdInsert.Enabled;
      miDataChoose.Enabled = Designer.cmdChooseData.Enabled;
      miDataAdd.Enabled = Designer.cmdAddData.Enabled;
#if Basic
      miDataAdd.Visible = false;
#endif      
      miReportOptions.Enabled = Designer.cmdReportSettings.Enabled;
      miViewStartPage.Visible = Designer.MdiMode;
      miWindow.Visible = Designer.MdiMode;
      miHelpContents.Enabled = Designer.cmdHelpContents.Enabled;
      
      Refresh();
    }

    private void InsertMenuCreateMenus(ObjectInfo rootItem, ToolStripItemCollection rootMenu)
    {
      foreach (ObjectInfo item in rootItem.Items)
      {
        ToolStripMenuItem menuItem = new ToolStripMenuItem();
        menuItem.Text = Res.TryGet(item.Text);
        menuItem.Tag = item;
        rootMenu.Add(menuItem);

        if (item.Items.Count > 0)
        {
          // it's a category
          InsertMenuCreateMenus(item, menuItem.DropDownItems);
        }
        else
        {
          menuItem.Image = item.Image;
          menuItem.Click += insertMenu_Click;
        }
      }
    }

    private void CreateInsertMenu()
    {
      if (Designer.ActiveReportTab != null && Designer.ActiveReportTab.ActivePage != null)
      {
        ObjectInfo pageItem = new ObjectInfo();
        ObjectInfo info = RegisteredObjects.FindObject(Designer.ActiveReportTab.ActivePage);
            bool contain = true;
            if (info != null)
            {
                for (int item = 0; item < info.Items.Count; item++)
                {
                    contain = true;
                    if (pageItem.Items.Count != 0)
                    {
                        for (int l = 0; l < pageItem.Items.Count; l++)
                        {
                            if (info.Items[item].Text == pageItem.Items[l].Text)
                            {
                                    contain = false;
                                    break;
                            }
                        }
                    }
                    if (contain)
                    {
                        pageItem.Items.Add(info.Items[item]);
                    }
                }

                    InsertMenuCreateMenus(pageItem, miInsert.DropDownItems);
                
            }
      }
    }

    private void miFile_DropDownOpening(object sender, EventArgs e)
    {
      // clear existing recent items
      int i = miFile.DropDownItems.IndexOf(miFileSelectLanguage) + 2;
      miFileImport.DropDownItems.Clear();
      miFileImport.BackColor = Color.White;
      ToolStripMenuItem menuItem1 = new ToolStripMenuItem();
      menuItem1.Text = "RTF | FRX";
      menuItem1.BackColor = Color.White;
      menuItem1.Click += MenuItem1_Click;
      miFileImport.DropDownItems.Add(menuItem1);       
      while (miFile.DropDownItems[i] != miFileExit)
      {
        miFile.DropDownItems[i].Dispose();
      }  
      // add new items
      if (Designer.cmdRecentFiles.Enabled && Designer.RecentFiles.Count > 0)
      {
        foreach (string s in Designer.RecentFiles)
        {
          ToolStripMenuItem menuItem = new ToolStripMenuItem();
          
          menuItem.Text = s;
          menuItem.Click += recentFile_Click;
          menuItem.BackColor = Color.White;
          miFile.DropDownItems.Insert(i, menuItem);
        }
        // make the separator
        i = miFile.DropDownItems.IndexOf(miFileExit);
        miFile.DropDownItems.Insert(i, new ToolStripSeparator { BackColor = Color.White });
      }
    }

     private void MenuItem1_Click(object sender, EventArgs e)
     {
            string source_name = string.Empty;
            try
            { 
            if (source_name.Length > 0)
            {
                source_name = string.Empty;
                if (!File.Exists(source_name))
                {
                    if (Directory.Exists(source_name))
                    {
                        Console.WriteLine("Starting batch conversion in '" + source_name + "'");
                        TraverseDirectory(source_name);
                        return;
                    }
                    Console.WriteLine("File '" + source_name + "' not found");
                    return;
                }
            }
            else
            {
                source_name = SelectFile();
            }
            ImportRtf rtf_convertor = new ImportRtf(source_name);
            rtf_convertor.ResetProperties();
            Report report = rtf_convertor.CreateReport();

            report.FileName = source_name + ".frx";
            report.Save(report.FileName);
            Designer.cmdOpen.LoadFile(report.FileName);
            }
            catch (Exception exc)
            {
                DialogResult dr = MessageBox.Show(exc.Message + "\n\nWould you like to comnvert another file?",
                    "Unable convert " + Path.GetFileName(source_name), MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    source_name = SelectFile();
                    if (source_name != null);
                }
              
            }
        }

        static void TraverseDirectory(string dirname)
        {
            foreach (string dir in Directory.GetDirectories(dirname))
                TraverseDirectory(dir);
            foreach (string file in Directory.GetFiles(dirname))
                if (file.ToLower().EndsWith(".rtf"))
                    ConvertFile(file);
        }
        static string SelectFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "RichText files (*.rtf)|*.rtf";
            dialog.Title = "Select a file for conversion to FRX format";

            if (dialog.ShowDialog() != DialogResult.OK)
                return null;
            return dialog.FileName;
        }

        static void ConvertFile(string filename)
        {
            try
            {
                ImportRtf rtf_convertor = new ImportRtf(filename);
                rtf_convertor.ResetProperties();
                Report report = rtf_convertor.CreateReport();
                report.FileName = filename + ".frx";
                report.Save(report.FileName);
            }
            catch (Exception excp)
            {
                Console.WriteLine("Processing file: " + filename);
                Console.WriteLine(excp.Message);
            }
        } 
      
    private void miEdit_DropDownOpening(object sender, EventArgs e)
    {
      miEditPaste.Enabled = Designer.cmdPaste.Enabled;
    }

    private void miInsert_DropDownOpening(object sender, EventArgs e)
    {
      miInsert.DropDownItems.Clear();
      CreateInsertMenu();
    }

    private void miDataShowDataSources_Click(object sender, EventArgs e)
    {
      ToolWindowBase window = Designer.Plugins.Find("DictionaryWindow") as ToolWindowBase;
      window.Show();
    }

    private void miView_DropDownOpening(object sender, EventArgs e)
    {
      // delete list of toolbars
      miViewToolbars.DropDownItems.Clear();

      // create list of toolbars
      foreach (IDesignerPlugin plugin in Designer.Plugins)
      {
        if (plugin is ToolbarBase)
        {
          ToolStripMenuItem menuItem = new ToolStripMenuItem();
          menuItem.Text = (plugin as ToolbarBase).Text;
          menuItem.Tag = plugin;
          menuItem.BackColor = Color.White;
          menuItem.Checked = (plugin as ToolbarBase).Visible;
          menuItem.Click += toolbar_Click;
          miViewToolbars.DropDownItems.Add(menuItem);
        }
      }

      miViewMessages.Checked = Designer.MessageWindowEnabled;
    }

    private void miWindow_DropDownOpening(object sender, EventArgs e)
    {
      // delete list of windows
      while (!(miWindow.DropDownItems[0] is ToolStripSeparator))
        miWindow.DropDownItems[0].Dispose();
      
      // create list of windows
      int i = 0;
      foreach (DocumentWindow c in Designer.Documents)
      {
        ToolStripMenuItem menuItem = new ToolStripMenuItem();
        menuItem.Text = (i + 1).ToString() + " " + c.Text;
        menuItem.Tag = c;
        menuItem.Checked = c == Designer.ActiveReportTab;
        menuItem.Click += window_Click;
        miWindow.DropDownItems.Insert(i, menuItem);
        i++;
      }
    }
    
	private void miViewMessages_Click(object sender, EventArgs e)
	{
        Designer.MessageWindowEnabled = miViewMessages.Checked;
	}

    private void toolbar_Click(object sender, EventArgs e)
    {
      ToolbarBase toolbar = (sender as ToolStripMenuItem).Tag as ToolbarBase;
      toolbar.Visible = !toolbar.Visible;
    }

    private void recentFile_Click(object sender, EventArgs e)
    {
      Designer.UpdatePlugins(null);
      Designer.cmdOpen.LoadFile((sender as ToolStripMenuItem).Text);
    }

    private void window_Click(object sender, EventArgs e)
    {
      DocumentWindow window = (sender as ToolStripMenuItem).Tag as DocumentWindow;
      window.Activate();
    }

    private void insertMenu_Click(object sender, EventArgs e)
    {
      ObjectInfo info = (sender as ToolStripMenuItem).Tag as ObjectInfo;
      Designer.InsertObject(info, InsertFrom.NewObject);
    }
    #endregion

    #region IDesignerPlugin
    /// <inheritdoc/>
    public void SaveState()
    {
    }
        
        /// <inheritdoc/>
        public void ReinitDpiSize()
        {            
        }

    /// <inheritdoc/>
    public void RestoreState()
    {
    }

    /// <inheritdoc/>
    public void SelectionChanged()
    {
      UpdateControls();
    }

    /// <inheritdoc/>
    public void UpdateContent()
    {
      UpdateControls();
    }

    /// <inheritdoc/>
    public void Lock()
    {
    }

    /// <inheritdoc/>
    public void Unlock()
    {
      UpdateContent();
    }

    /// <inheritdoc/>
    public void Localize()
    {
      MyRes res = new MyRes("Designer,Menu");

      miFile.Text = res.Get("File");
      miFileNew.Text = res.Get("File,New");
      miFileOpen.Text = res.Get("File,Open");
      miFileImport.Text = res.Get("File,Import");
      miFileClose.Text = res.Get("File,Close");
      miFileSave.Text = res.Get("File,Save");
      miFileSaveAs.Text = res.Get("File,SaveAs");
      miFileSaveAll.Text = res.Get("File,SaveAll");
      miFilePageSetup.Text = res.Get("File,PageSetup");
      miFilePrinterSetup.Text = res.Get("File,PrinterSetup");
      miFilePreview.Text = res.Get("File,Preview");
      miFileSelectLanguage.Text = res.Get("File,SelectLanguage");
      miFileExit.Text = res.Get("File,Exit");

      miEdit.Text = res.Get("Edit");
      miEditUndo.Text = res.Get("Edit,Undo");
      miEditRedo.Text = res.Get("Edit,Redo");
      miEditCut.Text = res.Get("Edit,Cut");
      miEditCopy.Text = res.Get("Edit,Copy");
      miEditPaste.Text = res.Get("Edit,Paste");
      miEditDelete.Text = res.Get("Edit,Delete");
      miEditDeletePage.Text = res.Get("Edit,DeletePage");
      miEditSelectAll.Text = res.Get("Edit,SelectAll");
      miEditGroup.Text = res.Get("Edit,Group");
      miEditUngroup.Text = res.Get("Edit,Ungroup");
      miEditFind.Text = res.Get("Edit,Find");
      miEditReplace.Text = res.Get("Edit,Replace");

      miInsert.Text = res.Get("Insert");

      miReport.Text = res.Get("Report");
      miReportOptions.Text = res.Get("Report,Options");

      miData.Text = res.Get("Data");
      miDataChoose.Text = res.Get("Data,Choose");
      miDataAdd.Text = res.Get("Data,Add");
      miDataShowData.Text = res.Get("Data,ShowData");

      miView.Text = res.Get("View");
      miViewToolbars.Text = res.Get("View,Toolbars");
      miViewStartPage.Text = res.Get("View,StartPage");
			miViewMessages.Text = "Messages"; //res.Get ("Designer,ToolWindow,Messages");
      miViewOptions.Text = res.Get("View,Options");

      miWindow.Text = res.Get("Window");
      miWindowCloseAll.Text = res.Get("Window,CloseAll");

      miHelp.Text = res.Get("Help");
      miHelpContents.Text = res.Get("Help,Contents");
      miHelpAbout.Text = res.Get("Help,About");
    }

    /// <inheritdoc/>
    public virtual DesignerOptionsPage GetOptionsPage()
    {
      return null;
    }

    /// <inheritdoc/>
    public void UpdateUIStyle()
    {
      Renderer = UIStyleUtils.GetToolStripRenderer(Designer.UIStyle);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Creates a new menu item.
    /// </summary>
    /// <returns>New menu item.</returns>
    public ToolStripMenuItem CreateMenuItem()
    {
      return CreateMenuItem(null);
    }

    /// <summary>
    /// Creates a new menu item.
    /// </summary>
    /// <param name="click">Click handler.</param>
    /// <returns>New menu item.</returns>
    public ToolStripMenuItem CreateMenuItem(EventHandler click)
    {
      return CreateMenuItem(null, "", click);
    }

    /// <summary>
    /// Creates a new menu item.
    /// </summary>
    /// <param name="image">Item's image.</param>
    /// <param name="click">Click handler.</param>
    /// <returns>New menu item.</returns>
    public ToolStripMenuItem CreateMenuItem(Image image, EventHandler click)
    {
      return CreateMenuItem(image, "", click);
    }

    /// <summary>
    /// Creates a new menu item.
    /// </summary>
    /// <param name="text">Item's text.</param>
    /// <param name="click">Click handler.</param>
    /// <returns>New menu item.</returns>
    public ToolStripMenuItem CreateMenuItem(string text, EventHandler click)
    {
      return CreateMenuItem(null, text, click);
    }

    /// <summary>
    /// Creates a new menu item.
    /// </summary>
    /// <param name="image">Item's image.</param>
    /// <param name="text">Item's text.</param>
    /// <param name="click">Click handler.</param>
    /// <returns>New menu item.</returns>
    public ToolStripMenuItem CreateMenuItem(Image image, string text, EventHandler click)
    {
      ToolStripMenuItem item = new ToolStripMenuItem();
      item.Image = image;
      item.Text = text;
      if (click != null) 
         { 
            item.BackColor = Color.White;
            item.Click += click;
         }
      return item;
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DesignerMenu"/> class with default settings.
    /// </summary>
    /// <param name="designer">The report designer.</param>
    public DesignerMenu(Designer designer) : base()
    {
      FDesigner = designer;

      Name = "MainMenu";
      Font = DrawUtils.DefaultFont;
      GripStyle = ToolStripGripStyle.Hidden;
      Dock = DockStyle.Top;

      // create menu items
      miFile = CreateMenuItem();
      miFileNew = CreateMenuItem(Designer.cmdNew.Invoke);
      miFileOpen = CreateMenuItem(Res.GetImage(1), Designer.cmdOpen.Invoke);
      miFileClose = CreateMenuItem(Designer.cmdClose.Invoke);
      miFileSave = CreateMenuItem(Res.GetImage(2), Designer.cmdSave.Invoke);
      miFileSaveAs = CreateMenuItem(Designer.cmdSaveAs.Invoke);
      miFileSaveAll = CreateMenuItem(Res.GetImage(178), Designer.cmdSaveAll.Invoke);
      miFilePageSetup = CreateMenuItem(Designer.cmdPageSetup.Invoke);
      miFilePrinterSetup = CreateMenuItem(Designer.cmdPrinterSetup.Invoke);
      miFilePreview = CreateMenuItem(Res.GetImage(3), Designer.cmdPreview.Invoke);
      miFileSelectLanguage = CreateMenuItem(Designer.cmdSelectLanguage.Invoke);
      miFileExit = CreateMenuItem(Designer.Exit);

      miEdit = CreateMenuItem();
      miEditUndo = CreateMenuItem(Res.GetImage(8), Designer.cmdUndo.Invoke);
      miFileImport = CreateMenuItem();
      miEditRedo = CreateMenuItem(Res.GetImage(9), Designer.cmdRedo.Invoke);
      miEditCut = CreateMenuItem(Res.GetImage(5), Designer.cmdCut.Invoke);
      miEditCopy = CreateMenuItem(Res.GetImage(6), Designer.cmdCopy.Invoke);
      miEditPaste = CreateMenuItem(Res.GetImage(7), Designer.cmdPaste.Invoke);
      miEditDelete = CreateMenuItem(Res.GetImage(51), Designer.cmdDelete.Invoke);
      miEditDeletePage = CreateMenuItem(Res.GetImage(12), Designer.cmdDeletePage.Invoke);
      miEditSelectAll = CreateMenuItem(Designer.cmdSelectAll.Invoke);
      miEditGroup = CreateMenuItem(Res.GetImage(17), Designer.cmdGroup.Invoke);
      miEditUngroup = CreateMenuItem(Res.GetImage(16), Designer.cmdUngroup.Invoke);
      miEditFind = CreateMenuItem(Res.GetImage(181), Designer.cmdFind.Invoke);
      miEditReplace = CreateMenuItem(Designer.cmdReplace.Invoke);

      miView = CreateMenuItem();
      miViewStartPage = CreateMenuItem(Res.GetImage(179), Designer.cmdViewStartPage.Invoke);
      miViewToolbars = CreateMenuItem();
      miViewToolbars.BackColor = Color.White;
	  miViewMessages = CreateMenuItem(miViewMessages_Click);
	  miViewMessages.CheckOnClick = true;
      miViewOptions = CreateMenuItem(Designer.cmdOptions.Invoke);

      miInsert = CreateMenuItem();

      miReport = CreateMenuItem();
      miReportOptions = CreateMenuItem(Designer.cmdReportSettings.Invoke);
      
      miData = CreateMenuItem();
      miDataChoose = CreateMenuItem(Designer.cmdChooseData.Invoke);
      miDataAdd = CreateMenuItem(Res.GetImage(137), Designer.cmdAddData.Invoke);
      miDataShowData = CreateMenuItem(Res.GetImage(72), miDataShowDataSources_Click);

      miWindow = CreateMenuItem();
      miWindowCloseAll = CreateMenuItem(Res.GetImage(202), Designer.cmdCloseAll.Invoke);

      miHelp = CreateMenuItem();
      miHelpContents = CreateMenuItem(Res.GetImage(90), Designer.cmdHelpContents.Invoke);
      miHelpAbout = CreateMenuItem(Designer.cmdAbout.Invoke);

      // create menu structure
      Items.AddRange(new ToolStripItem[] { 
        miFile, miEdit, miView, miInsert, miReport, miData, miWindow, miHelp });
      miFile.DropDownItems.AddRange(new ToolStripItem[] { 
        miFileNew, miFileOpen,miFileImport, miFileClose, new ToolStripSeparator { BackColor = Color.White },
        miFileSave, miFileSaveAs, miFileSaveAll, new ToolStripSeparator { BackColor = Color.White },
        miFilePageSetup, miFilePrinterSetup, miFilePreview, new ToolStripSeparator { BackColor = Color.White },
        miFileSelectLanguage,new ToolStripSeparator { BackColor = Color.White },
        miFileExit });
      miEdit.DropDownItems.AddRange(new ToolStripItem[] { 
        miEditUndo, miEditRedo, new ToolStripSeparator { BackColor = Color.White },
        miEditCut, miEditCopy, miEditPaste, miEditDelete, miEditDeletePage, miEditSelectAll, new ToolStripSeparator { BackColor = Color.White },
        miEditGroup, miEditUngroup, new ToolStripSeparator { BackColor = Color.White },
        miEditFind, miEditReplace });
      miView.DropDownItems.AddRange(new ToolStripItem[] { 
        miViewStartPage,
        miViewToolbars, 
		miViewMessages,
        new ToolStripSeparator { BackColor = Color.White },
        miViewOptions });
      miReport.DropDownItems.Add(miReportOptions);
      miData.DropDownItems.AddRange(new ToolStripItem[] {
        miDataChoose, miDataAdd, miDataShowData });
      miWindow.DropDownItems.Add(miWindowCloseAll);
      miHelp.DropDownItems.AddRange(new ToolStripItem[] { 
        miHelpContents, new ToolStripSeparator { BackColor = Color.White },
        miHelpAbout });

      // shortcuts
      miFileOpen.ShortcutKeys = Keys.Control | Keys.O;
      miFileSave.ShortcutKeys = Keys.Control | Keys.S;
      miFileSaveAll.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
      miFilePreview.ShortcutKeys = Keys.Control | Keys.P;
      miEditUndo.ShortcutKeys = Keys.Control | Keys.Z;
      miEditRedo.ShortcutKeys = Keys.Control | Keys.Y;
      miEditCut.ShortcutKeys = Keys.Control | Keys.X;
      miEditCopy.ShortcutKeys = Keys.Control | Keys.C;
      miEditPaste.ShortcutKeys = Keys.Control | Keys.V;
      miEditSelectAll.ShortcutKeys = Keys.Control | Keys.A;
      miEditFind.ShortcutKeys = Keys.Control | Keys.F;
      miEditReplace.ShortcutKeys = Keys.Control | Keys.H;
      miHelpContents.ShortcutKeys = Keys.F1;
      
      // events
      miFile.DropDownOpening += new EventHandler(miFile_DropDownOpening);
      miEdit.DropDownOpening += new EventHandler(miEdit_DropDownOpening);
      miInsert.DropDownOpening += new EventHandler(miInsert_DropDownOpening);
      miView.DropDownOpening += new EventHandler(miView_DropDownOpening);
      miWindow.DropDownOpening += new EventHandler(miWindow_DropDownOpening);

      Localize();
      Designer.Controls.Add(this);
    }
  }

}
