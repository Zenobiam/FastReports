using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Text;
using FastReport.Forms;
using FastReport.Utils;
using FastReport.Controls;
using FastReport.Design.ToolWindows;
using FastReport.Design.PageDesigners.Code;
#if !MONO
using FastReport.DevComponents.DotNetBar;
using FastReport.DevComponents;
#endif

namespace FastReport.Design
{
    /// <summary>
    /// Represents the report's designer control.
    /// </summary>
    /// <remarks>
    /// Usually you don't need to create an instance of this class. The designer can be called
    /// using the <see cref="FastReport.Report.Design()"/> method of 
    /// the <see cref="FastReport.Report"/> instance.
    /// <para/>This control represents pure designer surface + Objects toolbar. If you need
    /// standard menu, statusbar, toolbars and tool windows, use the 
    /// <see cref="FastReport.Design.StandardDesigner.DesignerControl"/> control instead. Also you may 
    /// decide to use a designer's form (<see cref="FastReport.Design.StandardDesigner.DesignerForm"/>)
    /// instead of a control.
    /// <para/>To run a designer, you need to attach a Report instance to it. This can be done via
    /// the <see cref="Report"/> property.
    /// <para/>To call the designer in MDI (Multi-Document Interface) mode, use the 
    /// <see cref="MdiMode"/> property.
    /// <para/>To set up some global properties, use the <see cref="Config"/> static class
    /// or <see cref="EnvironmentSettings"/> component that you can use in the Visual Studio IDE.
    /// </remarks>
    [ToolboxItem(false)]
    [Designer("FastReport.VSDesign.DesignerControlLayoutDesigner, FastReport.VSDesign, Version=1.0.0.0, Culture=neutral, PublicKeyToken=db7e5ce63278458c, processorArchitecture=MSIL")]
    public partial class Designer : UserControl, ISupportInitialize
    {
        #region Fields
        private Report report;
        private ReportTab activeReportTab;
        private PluginCollection plugins;
        private List<string> recentFiles;
        private DesignerClipboard clipboard;
        private bool mdiMode;
        private List<DocumentWindow> documents;
        private StartPageTab startPage;
        private ObjectCollection objects;
        private SelectedObjectCollection selectedObjects;
        private SelectedObjectCollection previouslySelectedObjects;
        private SelectedComponents selectedComponents;
        private SelectedReportComponents selectedReportComponents;
        private SelectedTextObjects selectedTextObjects;
        private LastFormatting lastFormatting;
        private bool modified;
        private bool formatPainter;
        private ReportComponentBase formatPainterPattern;
        private DesignerRestrictions restrictions;
        private bool isPreviewPageDesigner;
        private bool askSave;
        private string layoutState;
        private bool initFlag;
        private bool layoutNeeded;
        private UIStyle uIStyle;

        // docking
#if !MONO
        private DotNetBarManager dotNetBarManager;
        private DockSite bottomDockSite;
        private DockSite leftDockSite;
        private DockSite rightDockSite;
        private DockSite topDockSite;
        private DockSite tbLeftDockSite;
        private DockSite tbRightDockSite;
        private DockSite tbTopDockSite;
        private DockSite tbBottomDockSite;
        private FastReport.DevComponents.DotNetBar.TabControl tabs;
#else
        private FRTabControl tabs;
        private Splitter splitter;
        private SplitContainer FSplitContainer;
        private FRSideControl FSideControl;
#endif

        // tools
        private ObjectsToolbar objectsToolbar;
        private DictionaryWindow dataWindow;
        private PropertiesWindow propertiesWindow;
        private ReportTreeWindow reportTreeWindow;
        private MessagesWindow messagesWindow;

        // commands
        private NewCommand fcmdNew;
        private NewPageCommand fcmdNewPage;
        private NewDialogCommand fcmdNewDialog;
        private OpenCommand fcmdOpen;
        private SaveCommand fcmdSave;
        private SaveAsCommand fcmdSaveAs;
        private SaveAllCommand fcmdSaveAll;
        private CloseCommand fcmdClose;
        private CloseAllCommand fcmdCloseAll;
        private PreviewCommand fcmdPreview;
        private PrinterSettingsCommand fcmdPrinterSetup;
        private PageSettingsCommand fcmdPageSetup;
        private AddDataCommand fcmdAddData;
        private SortDataSourcesCommand fcmdSortDataSources;
        private ChooseDataCommand fcmdChooseData;
        private UndoCommand fcmdUndo;
        private RedoCommand fcmdRedo;
        private CutCommand fcmdCut;
        private CopyCommand fcmdCopy;
        private PasteCommand fcmdPaste;
        private FormatPainterCommand fcmdFormatPainter;
        private DeleteCommand fcmdDelete;
        private CopyPageCommand copyPageCommand;
        private DeletePageCommand fcmdDeletePage;
        private SelectAllCommand fcmdSelectAll;
        private GroupCommand fcmdGroup;
        private UngroupCommand fcmdUngroup;
        private EditCommand fcmdEdit;
        private FindCommand fcmdFind;
        private ReplaceCommand fcmdReplace;
        private BringToFrontCommand fcmdBringToFront;
        private SendToBackCommand fcmdSendToBack;
        private InsertCommand fcmdInsert;
        private InsertBandCommand fcmdInsertBand;
        private RecentFilesCommand fcmdRecentFiles;
        private SelectLanguageCommand fcmdSelectLanguage;
        private ViewStartPageCommand fcmdViewStartPage;
        private ReportSettingsCommand fcmdReportSettings;
        private OptionsCommand fcmdOptions;
        private ReportStylesCommand fcmdReportStyles;
        private HelpContentsCommand fcmdHelpContents;
        private AboutCommand fcmdAbout;
        private WelcomeCommand fcmdWelcome;
        private Timer autoSaveTimer;
        // polygon
        private PolygonSelectModeCommand fcmdPolySelectMove;
        private PolygonSelectModeCommand fcmdPolySelectPointer;
        private PolygonSelectModeCommand fcmdPolySelectAddPoint;
        private PolygonSelectModeCommand fcmdPolySelectBezier;
        private PolygonSelectModeCommand fcmdPolySelectRemovePoint;
        #endregion

        #region Properties

        /// <summary>
        /// Occurs when designer's UI state changed.
        /// </summary>
        public event EventHandler UIStateChanged;

        /// <summary>
        /// Gets or sets the edited report.
        /// </summary>
        /// <remarks>
        /// To initialize the designer, you need to pass a Report instance to this property.
        /// This will create the designer's surface associated with the report.
        /// <code>
        /// Designer designer = new Designer();
        /// designer.Parent = form1;
        /// designer.Report = report1;
        /// </code>
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Report Report
        {
            get { return report; }
            set
            {
                report = value;
                if (report != null)
                    report.Designer = this;
                InitTabs();
            }
        }

        /// <summary>
        /// Gets active report object. 
        /// </summary>
        /// <remarks>
        /// May be <b>null</b> if Start Page selected, or no reports opened.
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Report ActiveReport
        {
            get { return activeReportTab == null ? null : activeReportTab.Report; }
            set
            {
                foreach (DocumentWindow c in Documents)
                {
                    if (c is ReportTab && (c as ReportTab).Report == value)
                    {
                        c.Activate();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a collection of global plugins such as menu, properties window, etc. 
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PluginCollection Plugins
        {
            get { return plugins; }
        }

        /// <summary>
        /// Gets a collection of objects on the active page of the active report.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObjectCollection Objects
        {
            get { return objects; }
        }

        /// <summary>
        /// Gets a collection of selected objects on the active page of the active report.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SelectedObjectCollection SelectedObjects
        {
            get { return selectedObjects; }
        }

        /// <summary>
        /// Gets a collection of selected objects of the <b>ComponentBase</b> type.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SelectedComponents SelectedComponents
        {
            get { return selectedComponents; }
        }

        /// <summary>
        /// Gets a collection of selected objects of the <b>ReportComponentBase</b> type.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SelectedReportComponents SelectedReportComponents
        {
            get { return selectedReportComponents; }
        }

        /// <summary>
        /// Gets a collection of selected objects of the <b>TextObject</b> type.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SelectedTextObjects SelectedTextObjects
        {
            get { return selectedTextObjects; }
        }

        /// <summary>
        /// Gets or sets a value indicating that the report was modified.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Modified
        {
            get { return modified; }
            set { modified = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether to ask user to save changes when closing the designer.
        /// </summary>
        public bool AskSave
        {
            get { return askSave; }
            set { askSave = value; }
        }

        /// <summary>
        /// Gets the designer restrictions.
        /// </summary>
        public DesignerRestrictions Restrictions
        {
            get { return restrictions; }
        }

        /// <summary>
        /// Gets or sets a value indicating that designer is run in MDI mode.
        /// </summary>
        /// <remarks>
        /// <para/>To call the designer in MDI (Multi-Document Interface) mode, use the following code:
        /// <code>
        /// DesignerControl designer = new DesignerControl();
        /// designer.MdiMode = true;
        /// designer.ShowDialog();
        /// </code>
        /// </remarks>
        [DefaultValue(false)]
        public bool MdiMode
        {
            get { return mdiMode; }
            set
            {
                mdiMode = value;
                UpdateMdiMode();
            }
        }

        /// <summary>
        /// Gets or sets the visual style.
        /// </summary>
        public UIStyle UIStyle
        {
            get { return uIStyle; }
            set
            {
                uIStyle = value;
                UpdateUIStyle();
            }
        }

        /// <summary>
        /// Gets a value indicating that designer is used to edit a preview page.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPreviewPageDesigner
        {
            get { return isPreviewPageDesigner; }
            set { isPreviewPageDesigner = value; }
        }

#if MONO
		public bool MessageWindowEnabled
		{
            get 
            { 
                return !this.FSplitContainer.Panel2Collapsed; 
            }
            set
			{
				this.FSplitContainer.Panel2Collapsed = !value;
			}
		}
#endif

        /// <summary>
        /// The "File|New" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NewCommand cmdNew
        {
            get { return fcmdNew; }
        }

        /// <summary>
        /// The "New Page" toolbar command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NewPageCommand cmdNewPage
        {
            get { return fcmdNewPage; }
        }

        /// <summary>
        /// The "New Dialog" toolbar command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NewDialogCommand cmdNewDialog
        {
            get { return fcmdNewDialog; }
        }

        /// <summary>
        /// The "File|Open..." command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OpenCommand cmdOpen
        {
            get { return fcmdOpen; }
        }

        /// <summary>
        /// The "File|Save" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SaveCommand cmdSave
        {
            get { return fcmdSave; }
        }

        /// <summary>
        /// The "File|Save As..." command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SaveAsCommand cmdSaveAs
        {
            get { return fcmdSaveAs; }
        }

        /// <summary>
        /// The "File|Save All" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SaveAllCommand cmdSaveAll
        {
            get { return fcmdSaveAll; }
        }

        /// <summary>
        /// The "File|Close" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CloseCommand cmdClose
        {
            get { return fcmdClose; }
        }

        /// <summary>
        /// The "Window|Close All" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CloseAllCommand cmdCloseAll
        {
            get { return fcmdCloseAll; }
        }

        /// <summary>
        /// The "File|Preview..." command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PreviewCommand cmdPreview
        {
            get { return fcmdPreview; }
        }

        /// <summary>
        /// The "File|Printer Setup..." command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PrinterSettingsCommand cmdPrinterSetup
        {
            get { return fcmdPrinterSetup; }
        }

        /// <summary>
        /// The "File|Page Setup..." command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PageSettingsCommand cmdPageSetup
        {
            get { return fcmdPageSetup; }
        }

        /// <summary>
        /// The "Data|Add New Data Source..." command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AddDataCommand cmdAddData
        {
            get { return fcmdAddData; }
        }

        /// <summary>
        /// The "Data|Sort Data Sources" command.
        /// </summary>
        public SortDataSourcesCommand cmdSortDataSources
        {
            get { return fcmdSortDataSources; }
        }

        /// <summary>
        /// The "Data|Choose Report Data..." command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChooseDataCommand cmdChooseData
        {
            get { return fcmdChooseData; }
        }

        /// <summary>
        /// The "Edit|Undo" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UndoCommand cmdUndo
        {
            get { return fcmdUndo; }
        }

        /// <summary>
        /// The "Edit|Redo" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RedoCommand cmdRedo
        {
            get { return fcmdRedo; }
        }

        /// <summary>
        /// The "Edit|Cut" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CutCommand cmdCut
        {
            get { return fcmdCut; }
        }

        /// <summary>
        /// The "Edit|Copy" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CopyCommand cmdCopy
        {
            get { return fcmdCopy; }
        }

        /// <summary>
        /// The "Edit|Paste" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PasteCommand cmdPaste
        {
            get { return fcmdPaste; }
        }

        /// <summary>
        /// The "Format Painter" toolbar command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FormatPainterCommand cmdFormatPainter
        {
            get { return fcmdFormatPainter; }
        }

        /// <summary>
        /// The "Edit|Delete" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DeleteCommand cmdDelete
        {
            get { return fcmdDelete; }
        }

        /// <summary>
        /// The "Edit|Copy Page" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CopyPageCommand cmdCopyPage
        {
            get { return copyPageCommand; }
        }

        /// <summary>
        /// The "Edit|Delete Page" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DeletePageCommand cmdDeletePage
        {
            get { return fcmdDeletePage; }
        }

        /// <summary>
        /// The "Edit|Select All" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SelectAllCommand cmdSelectAll
        {
            get { return fcmdSelectAll; }
        }

        /// <summary>
        /// The "Edit|Group" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GroupCommand cmdGroup
        {
            get { return fcmdGroup; }
        }

        /// <summary>
        /// The "Edit|Ungroup" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UngroupCommand cmdUngroup
        {
            get { return fcmdUngroup; }
        }

        /// <summary>
        /// The "Edit" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EditCommand cmdEdit
        {
            get { return fcmdEdit; }
        }

        /// <summary>
        /// The "Edit|Find..." command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FindCommand cmdFind
        {
            get { return fcmdFind; }
        }


        /// <summary>
        /// The "Polygon move command" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PolygonSelectModeCommand CmdPolySelectMove
        {
            get { return fcmdPolySelectMove; }
        }
        /// <summary>
        /// The "Polygon point move" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PolygonSelectModeCommand CmdPolySelectPointer
        {
            get { return fcmdPolySelectPointer; }
        }
        /// <summary>
        /// The "Polygon add new point" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PolygonSelectModeCommand CmdPolySelectAddPoint
        {
            get { return fcmdPolySelectAddPoint; }
        }
        /// <summary>
        /// The "Polygon berier" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PolygonSelectModeCommand CmdPolySelectBezier
        {
            get { return fcmdPolySelectBezier; }
        }
        /// <summary>
        /// The "Polygon remove point" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PolygonSelectModeCommand CmdPolySelectRemovePoint
        {
            get { return fcmdPolySelectRemovePoint; }
        }

        /// <summary>
        /// The "Edit|Replace..." command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ReplaceCommand cmdReplace
        {
            get { return fcmdReplace; }
        }

        /// <summary>
        /// The "Bring To Front" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BringToFrontCommand cmdBringToFront
        {
            get { return fcmdBringToFront; }
        }

        /// <summary>
        /// The "Send To Back" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SendToBackCommand cmdSendToBack
        {
            get { return fcmdSendToBack; }
        }

        /// <summary>
        /// The "Insert" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InsertCommand cmdInsert
        {
            get { return fcmdInsert; }
        }

        /// <summary>
        /// The "Insert Band" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InsertBandCommand cmdInsertBand
        {
            get { return fcmdInsertBand; }
        }

        /// <summary>
        /// The "Recent Files" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RecentFilesCommand cmdRecentFiles
        {
            get { return fcmdRecentFiles; }
        }

        /// <summary>
        /// The "File|Select Language..." command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SelectLanguageCommand cmdSelectLanguage
        {
            get { return fcmdSelectLanguage; }
        }

        /// <summary>
        /// The "View|Start Page" command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ViewStartPageCommand cmdViewStartPage
        {
            get { return fcmdViewStartPage; }
        }

        /// <summary>
        /// The "Report|Options..." command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ReportSettingsCommand cmdReportSettings
        {
            get { return fcmdReportSettings; }
        }

        /// <summary>
        /// The "View|Options..." command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OptionsCommand cmdOptions
        {
            get { return fcmdOptions; }
        }

        /// <summary>
        /// The "Report|Styles..." command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ReportStylesCommand cmdReportStyles
        {
            get { return fcmdReportStyles; }
        }

        /// <summary>
        /// The "Help|Help Contents..." command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HelpContentsCommand cmdHelpContents
        {
            get { return fcmdHelpContents; }
        }

        /// <summary>
        /// The "Help|About..." command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AboutCommand cmdAbout
        {
            get { return fcmdAbout; }
        }

        /// <summary>
        /// The "Show welcome window..." command.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WelcomeCommand cmdWelcome
        {
            get { return fcmdWelcome; }
        }

        /// <summary>
        /// Gets or sets the layout state of the designer.
        /// </summary>
        /// <remarks>
        /// This property is used to store layout in Visual Studio design time. You may also use
        /// it to save and restore the designer's layout in your code. However, consider using the
        /// <see cref="SaveConfig()"/> and <see cref="RestoreConfig()"/> methods that use FastReport 
        /// configuration file.
        /// </remarks>
        [Browsable(false)]
        public string LayoutState
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.Root.Name = "Config";
                SaveDockState(doc.Root);
                using (MemoryStream stream = new MemoryStream())
                {
                    doc.Save(stream);
                    UTF8Encoding encoding = new UTF8Encoding();
                    return encoding.GetString(stream.ToArray());
                }
            }
            set
            {
                layoutState = value;
                if (initFlag)
                    layoutNeeded = true;
                else
                    RestoreLayout(value);
            }
        }

        /// <summary>
        /// Fires when the layout is changed.
        /// </summary>
        /// <remarks>
        /// This event is for internal use only.
        /// </remarks>
        public event EventHandler LayoutChangedEvent;

        internal StartPageTab StartPage
        {
            get { return startPage; }
        }

        // active report tab. May be null if Start Page selected, or no reports opened
        internal ReportTab ActiveReportTab
        {
            get { return activeReportTab; }
            set
            {
                if (activeReportTab != value)
                {
                    if (activeReportTab != null)
                        activeReportTab.ReportDeactivated();
                    activeReportTab = value;
                    if (value != null)
                        value.ReportActivated();
                    else
                        ClearSelection();
                    UpdatePlugins(null);
                }
            }
        }

        // list of recent opened files
        internal List<string> RecentFiles
        {
            get { return recentFiles; }
        }

        internal List<DocumentWindow> Documents
        {
            get { return documents; }
        }

        internal DesignerClipboard Clipboard
        {
            get { return clipboard; }
        }

        internal DictionaryWindow DataWindow
        {
            get { return dataWindow; }
        }

        internal PropertiesWindow PropertiesWindow
        {
            get { return propertiesWindow; }
        }

        internal ReportTreeWindow ReportTreeWindow
        {
            get { return reportTreeWindow; }
        }

        internal MessagesWindow MessagesWindow
        {
            get { return messagesWindow; }
        }

        internal CodePageDesigner Editor
        {
            get { return ActiveReportTab.Editor; }
        }

        internal LastFormatting LastFormatting
        {
            get { return lastFormatting; }
        }

        internal bool FormatPainter
        {
            get { return formatPainter; }
            set
            {
                if (formatPainter != value)
                {
                    formatPainter = value;
                    formatPainterPattern = value ? SelectedReportComponents.First : null;
                    UpdatePlugins(null);
                }
            }
        }

#if !MONO
        internal DotNetBarManager DotNetBarManager
        {
            get { return dotNetBarManager; }
        }
#endif

        internal bool IsVSDesignMode
        {
            get { return DesignMode; }
        }

        internal Timer AutoSaveTimer
        {
            get { return autoSaveTimer; }
        }
        #endregion

        #region Private Methods
        private void SetupAutoSave()
        {
            autoSaveTimer = new Timer();
            int minutes = 0;
            if (!int.TryParse(Config.Root.FindItem("Designer").FindItem("Saving").GetProp("AutoSaveMinutes"), out minutes))
                minutes = 5;
            autoSaveTimer.Interval = minutes * 60000;
            autoSaveTimer.Tick += delegate(object s1, EventArgs e1)
            {
                if (this != null && !IsPreviewPageDesigner)
                {
                    if (cmdSave.Enabled)
                    {
                        ActiveReportTab.AutoSaveFile();
                    }
                    else
                    {
                        if (File.Exists(Config.AutoSaveFileName))
                            File.Delete(Config.AutoSaveFileName);
                        if (File.Exists(Config.AutoSaveFile))
                            File.Delete(Config.AutoSaveFile);
                    }
                }
            };
        }

        public void UpdateUIStyle()
        {
#if !MONO
            switch (UIStyle)
            {
                case UIStyle.Office2003:
                case UIStyle.Office2007Blue:
                case UIStyle.Office2010Blue:
                    StyleManager.ChangeStyle(eStyle.Office2010Blue, Color.Empty);
                    break;
                case UIStyle.Office2007Silver:
                case UIStyle.Office2010Silver:
                    StyleManager.ChangeStyle(eStyle.Office2010Silver, Color.Empty);
                    break;
                case UIStyle.Office2007Black:
                case UIStyle.Office2010Black:
                    StyleManager.ChangeStyle(eStyle.Office2010Black, Color.Empty);
                    break;
                case UIStyle.Office2013:
                    StyleManager.ChangeStyle(eStyle.Office2013, Color.Empty);
                    break;
                case UIStyle.VisualStudio2005:
                case UIStyle.VisualStudio2010:
                    StyleManager.ChangeStyle(eStyle.VisualStudio2010Blue, Color.Empty);
                    break;
                case UIStyle.VisualStudio2012Light:
                    StyleManager.ChangeStyle(eStyle.VisualStudio2012Light, Color.Empty);
                    break;
                case UIStyle.VistaGlass:
                    StyleManager.ChangeStyle(eStyle.Windows7Blue, Color.Empty);
                    break;
            }

            dotNetBarManager.Style = UIStyleUtils.GetDotNetBarStyle(UIStyle);
#else
            tabs.Style = UIStyle;
            FSideControl.Style = UIStyle;
            splitter.BackColor = UIStyleUtils.GetColorTable(UIStyle).ControlBackColor;
#endif

            foreach (DocumentWindow document in Documents)
            {
                if (document is ReportTab)
                    (document as ReportTab).UpdateUIStyle();
            }
            plugins.UpdateUIStyle();
        }

        // init global designer plugins
        private void InitPluginsInternal()
        {
            SuspendLayout();
            InitPlugins();
#if !MONO
            foreach (Type pluginType in DesignerPlugins.Plugins)
            {
                IDesignerPlugin plugin = plugins.Add(pluginType);
                if (plugin is ToolWindowBase)
                    (plugin as ToolWindowBase).DoDefaultDock();
            }
#endif
            ResumeLayout();
        }

        private void UpdateMdiMode()
        {
#if !MONO
            tabs.TabsVisible = mdiMode;
#else
            tabs.ShowTabs = mdiMode;
#endif
            if (mdiMode)
                AddStartPageTab();
            else if (StartPage != null)
                StartPage.Close();
        }

        // create initial pages (Start Page, default report)
        private void InitTabs()
        {
            if (mdiMode)
                AddStartPageTab();
            if (report != null)
                AddReportTab(CreateReportTab(report));
        }

#if !MONO
        private void FTabs_SelectedTabChanged(object sender, TabStripTabChangedEventArgs e)
        {
            if (e.NewTab is DocumentWindow)
            {
                if (e.NewTab is StartPageTab)
                    ActiveReportTab = null;
                else
                    ActiveReportTab = e.NewTab as ReportTab;
            }
        }
#else
        private void FTabs_SelectedTabChanged(object sender, EventArgs e)
        {
            if (tabs.SelectedTab is DocumentWindow)
            {
                if (tabs.SelectedTab is StartPageTab)
                    ActiveReportTab = null;
                else
                    ActiveReportTab = tabs.SelectedTab as ReportTab;
            }
        }
#endif

#if !MONO
        private void FTabs_TabItemClose(object sender, TabStripActionEventArgs e)
#else
        private void FTabs_TabClosed(object sender, EventArgs e)
#endif
        {
            if (tabs.SelectedTab is ReportTab)
            {
                ReportTab tab = tabs.SelectedTab as ReportTab;
                if (!CloseDocument(tab))
                {
                    return;
                }
            }
            else if (tabs.SelectedTab is StartPageTab)
            {
                StartPageTab tab = tabs.SelectedTab as StartPageTab;
                if (!CloseDocument(tab))
                {
                    return;
                }
            }

            if (Documents.Count > 0)
            {
                Documents[Documents.Count - 1].Activate();
            }
            else
            {
                ClearSelection();
            }
        }

        private void AddDocument(DocumentWindow doc)
        {
            Documents.Add(doc);
            doc.AddToTabControl(tabs);
            doc.Activate();
        }

        private void RemoveDocument(DocumentWindow doc)
        {
            if (doc is StartPageTab)
                startPage = null;
            Documents.Remove(doc);
            doc.Close();
        }

        internal bool CloseDocument(DocumentWindow doc)
        {
            if (doc is ReportTab)
            {
                if (!(doc as ReportTab).CanClose())
                    return false;
            }

            RemoveDocument(doc);
            return true;
        }

        private void ClearSelection()
        {
            activeReportTab = null;
            Objects.Clear();
            SelectedObjects.Clear();
            SelectionChanged(null);
            UpdatePlugins(null);
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Initializes designer plugins such as toolbars and toolwindows.
        /// </summary>
        protected virtual void InitPlugins()
        {
            // make default layout
#if !MONO
            objectsToolbar = new ObjectsToolbar(this);
            dataWindow = new DictionaryWindow(this);
            propertiesWindow = new PropertiesWindow(this);
            reportTreeWindow = new ReportTreeWindow(this);
            messagesWindow = new MessagesWindow(this);

            dataWindow.DockTo(rightDockSite);
            reportTreeWindow.DockTo(dataWindow);
            propertiesWindow.DockTo(rightDockSite, dataWindow, eDockSide.Bottom);
            messagesWindow.DockTo(bottomDockSite);
            messagesWindow.Bar.AutoHide = true;
            messagesWindow.Hide();
            rightDockSite.Width = DpiHelper.ConvertUnits(250);

            leftDockSite.MouseUp += new MouseEventHandler(dockSite_MouseUp);
            rightDockSite.MouseUp += new MouseEventHandler(dockSite_MouseUp);
            topDockSite.MouseUp += new MouseEventHandler(dockSite_MouseUp);
            bottomDockSite.MouseUp += new MouseEventHandler(dockSite_MouseUp);
            dotNetBarManager.AutoHideChanged += new EventHandler(LayoutChanged);
            dotNetBarManager.BarDock += new EventHandler(LayoutChanged);

            dataWindow.VisibleChanged += new EventHandler(LayoutChanged);
            propertiesWindow.VisibleChanged += new EventHandler(LayoutChanged);
            reportTreeWindow.VisibleChanged += new EventHandler(LayoutChanged);
            messagesWindow.VisibleChanged += new EventHandler(LayoutChanged);
#else
            FSplitContainer = new SplitContainer();
            FSplitContainer.Orientation = Orientation.Horizontal;
            FSplitContainer.Dock = DockStyle.Fill;
            FSplitContainer.Panel2MinSize = 100;
            Controls.Add(FSplitContainer);

            tabs = new FRTabControl();
            tabs.Dock = DockStyle.Fill;
            tabs.ShowCaption = false;
            tabs.SelectedTabChanged += FTabs_SelectedTabChanged;
            tabs.TabClosed += FTabs_TabClosed;
            //Controls.Add(FTabs);
            FSplitContainer.Panel1.Controls.Add(tabs);

            splitter = new Splitter();
            splitter.Dock = DockStyle.Right;
            Controls.Add(splitter);

            objectsToolbar = new ObjectsToolbar(this);
            dataWindow = new DictionaryWindow(this);
            propertiesWindow = new PropertiesWindow(this);
            reportTreeWindow = new ReportTreeWindow(this);
            messagesWindow = new MessagesWindow(this);

            messagesWindow.Parent = FSplitContainer.Panel2;
            messagesWindow.Dock = DockStyle.Fill;

            FSideControl = new FRSideControl();
            FSideControl.Dock = DockStyle.Right;
            FSideControl.Width = 250;
            FSideControl.TopPanel.Height = 350;
            FSideControl.AddToTopPanel(dataWindow);
            FSideControl.AddToTopPanel(reportTreeWindow);
            FSideControl.AddToBottomPanel(propertiesWindow);
            // FSideControl.AddToBottomPanel(FMessagesWindow);
            FSideControl.RefreshLayout();
            Controls.Add(FSideControl);
#endif

            plugins.AddRange(new IDesignerPlugin[] {
        objectsToolbar, dataWindow, propertiesWindow, 
        reportTreeWindow, messagesWindow });

            // add export plugins
            plugins.Add(typeof(ExportPlugins.FR3.FR3ExportPlugin));
#if MSCHART
            plugins.Add(typeof(ExportPlugins.RDL.RDLExportPlugin));
#endif

            // add import plugins
#if MSCHART
            plugins.Add(typeof(ImportPlugins.RDL.RDLImportPlugin));
#endif
            plugins.Add(typeof(ImportPlugins.ListAndLabel.ListAndLabelImportPlugin));
            plugins.Add(typeof(ImportPlugins.DevExpress.DevExpressImportPlugin));
            plugins.Add(typeof(ImportPlugins.RTF.RTFImportPlugin));
        }

#if !MONO
        /// <inheritdoc/>
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            dotNetBarManager.ParentForm = FindForm();

            if (DesignMode)
                DisableFloatingBars();
        }
#endif
        #endregion

        #region Public Methods
        /// <summary>
        /// Cancels paste mode.
        /// </summary>
        public void CancelPaste()
        {
            ActiveReportTab.ActivePageDesigner.CancelPaste();
        }

        /// <summary>
        /// AutoSave system initialization.
        /// </summary>
        public void StartAutoSave()
        {
            if (IsPreviewPageDesigner)
                return;

            if (File.Exists(Config.AutoSaveFile) &&
                File.Exists(Config.AutoSaveFileName) &&
                Config.Root.FindItem("Designer").FindItem("Saving").GetProp("EnableAutoSave") != "0")
            {
                string filepath = File.ReadAllText(Config.AutoSaveFileName);

                using (LoadAutoSaveForm f = new LoadAutoSaveForm(filepath))
                {
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        ActiveReportTab.LoadAutoSaveFile(filepath);
                        SetModified();
                    }
                }
            }

            if (File.Exists(Config.AutoSaveFileName))
                File.Delete(Config.AutoSaveFileName);
            if (File.Exists(Config.AutoSaveFile))
                File.Delete(Config.AutoSaveFile);

            autoSaveTimer.Enabled = Config.Root.FindItem("Designer").FindItem("Saving").GetProp("EnableAutoSave") != "0";
        }

        /// <summary>
        /// Stops the AutoSave system.
        /// </summary>
        public void StopAutoSave()
        {
            if (IsPreviewPageDesigner)
                return;

            autoSaveTimer.Stop();

            if (File.Exists(Config.AutoSaveFileName))
                File.Delete(Config.AutoSaveFileName);
            if (File.Exists(Config.AutoSaveFile))
                File.Delete(Config.AutoSaveFile);
        }

        /// <summary>
        /// Call this method if you change something in the report. 
        /// </summary>
        /// <remarks>
        /// This method adds the current report state to the undo buffer and updates all plugins.
        /// </remarks>
        public void SetModified()
        {
            SetModified(null, "Change");
        }

        /// <summary>
        /// Call this method if you change something in the report. 
        /// </summary>
        /// <param name="sender">The object that was modified.</param>
        /// <param name="action">The undo action name.</param>
        /// <remarks>
        /// This method adds the current report state to the undo buffer and updates all plugins.
        /// </remarks>
        public void SetModified(object sender, string action)
        {
            SetModified(sender, action, null);
        }

        /// <summary>
        /// Call this method if you change something in the report.
        /// </summary>
        /// <param name="sender">The object that was modified.</param>
        /// <param name="action">The undo action name.</param>
        /// <param name="objName">The name of modified object.</param>
        public void SetModified(object sender, string action, string objName)
        {
            modified = true;
            if (ActiveReportTab != null)
                ActiveReportTab.SetModified(sender, action, objName);
            UpdatePlugins(sender);
        }

        /// <summary>
        /// Call this method to tell the designer that current selection is changed.
        /// </summary>
        /// <param name="sender">The plugin that changes the selection (may be <b>null</b>).</param>
        public void SelectionChanged(object sender)
        {
            // check groups
            ObjectCollection selectedObjects = new ObjectCollection();
            SelectedObjects.CopyTo(selectedObjects);
            foreach (Base c in selectedObjects)
            {
                if (c is ComponentBase && (c as ComponentBase).GroupIndex != 0)
                {
                    int groupIndex = (c as ComponentBase).GroupIndex;
                    foreach (Base c1 in Objects)
                    {
                        if (c1 is ComponentBase && (c1 as ComponentBase).GroupIndex == groupIndex)
                        {
                            if (SelectedObjects.IndexOf(c1) == -1)
                                SelectedObjects.Add(c1);
                        }
                    }
                }
            }

            foreach (Base c in previouslySelectedObjects)
            {
                c.SelectionChanged();
            }

            if (formatPainter && formatPainterPattern != null)
            {
                foreach (Base c in selectedObjects)
                {
                    c.SelectionChanged();
                    if (c is ReportComponentBase)
                        (c as ReportComponentBase).AssignFormat(formatPainterPattern);
                }
            }

            SelectedComponents.Update();
            SelectedReportComponents.Update();
            SelectedTextObjects.Update();
            if (ActiveReportTab != null)
                ActiveReportTab.Plugins.SelectionChanged(sender);
            plugins.SelectionChanged(sender);

            SelectedObjects.CopyTo(previouslySelectedObjects);

            if (formatPainter && formatPainterPattern != null)
            {
                if (sender != null)
                    SetModified();
                else
                {
                    SetModified(null, "Script-no-undo");
                    FormatPainter = false;
                }
            }
            OnUIStateChanged();
        }

        /// <summary>
        /// Locks all plugins. 
        /// </summary>
        /// <remarks>
        /// This method is usually called when we destroy the report to prevent unexpected
        /// errors - such as trying to draw destroyed objects.
        /// </remarks>
        public void Lock()
        {
            if (ActiveReportTab != null)
                ActiveReportTab.Plugins.Lock();
            plugins.Lock();
        }

        /// <summary>
        /// Unlocks all plugins. 
        /// </summary>
        /// <remarks>
        /// Call this method after the <b>Lock</b>.
        /// </remarks>
        public void Unlock()
        {
            if (ActiveReportTab != null)
                ActiveReportTab.Plugins.Unlock();
            plugins.Unlock();
        }

        /// <summary>
        /// Call this method to refresh all plugins' content.
        /// </summary>
        /// <param name="sender">The plugin that we don't need to refresh.</param>
        public void UpdatePlugins(object sender)
        {
            if (ActiveReportTab != null)
                ActiveReportTab.Plugins.Update(sender);
            plugins.Update(sender);
            OnUIStateChanged();
        }

        /// <summary>
        /// Scales the plugin based on the current Dpi.
        /// </summary>
        /// <remarks>
        /// This method is called by the dpi or screen resolution changed.
        /// </remarks>
        public void ReinitDpiSize()
        {
#if !MONO
            Plugins.ReinitDpiSize();
            ActiveReportTab.ActivePageDesigner.PageActivated();
            activeReportTab.GetTabFont();
            activeReportTab.UpdateDpiDependencies();
            Update();
#endif
        }

        // adds the Start Page tab
        internal void AddStartPageTab()
        {
            if (StartPage != null)
                return;

            startPage = new StartPageTab(this);
            AddDocument(startPage);
            UpdatePlugins(null);
        }

        // adds the report tab
        internal ReportTab CreateReportTab(Report report)
        {
            return new ReportTab(this, report);
        }

        internal void AddReportTab(ReportTab tab)
        {
            AddDocument(tab);
            tab.UpdateUIStyle();
            ActiveReportTab = tab;
        }

        private void OnUIStateChanged()
        {
            if (UIStateChanged != null)
                UIStateChanged(this, EventArgs.Empty);
        }
#endregion

#region Layout
#if !MONO
        private void CreateLayout()
        {
            tabs = new FastReport.DevComponents.DotNetBar.TabControl();
            tabs.Dock = DockStyle.Fill;
            tabs.TabLayoutType = eTabLayoutType.FixedWithNavigationBox;
            tabs.AutoHideSystemBox = false;
            tabs.CloseButtonOnTabsVisible = true;
            tabs.ControlTabNavigationEnabled = false;
            tabs.SelectedTabChanged += new TabStrip.SelectedTabChangedEventHandler(FTabs_SelectedTabChanged);
            tabs.TabItemClose += new TabStrip.UserActionEventHandler(FTabs_TabItemClose);

            Controls.Add(tabs);

            dotNetBarManager = new DotNetBarManager(this.components);
            leftDockSite = new DockSite();
            rightDockSite = new DockSite();
            topDockSite = new DockSite();
            bottomDockSite = new DockSite();
            tbLeftDockSite = new DockSite();
            tbRightDockSite = new DockSite();
            tbTopDockSite = new DockSite();
            tbBottomDockSite = new DockSite();
            // 
            // dotNetBarManager
            // 
            dotNetBarManager.AutoDispatchShortcuts.Add(eShortcut.F1);
            dotNetBarManager.AutoDispatchShortcuts.Add(eShortcut.CtrlC);
            dotNetBarManager.AutoDispatchShortcuts.Add(eShortcut.CtrlA);
            dotNetBarManager.AutoDispatchShortcuts.Add(eShortcut.CtrlV);
            dotNetBarManager.AutoDispatchShortcuts.Add(eShortcut.CtrlX);
            dotNetBarManager.AutoDispatchShortcuts.Add(eShortcut.CtrlZ);
            dotNetBarManager.AutoDispatchShortcuts.Add(eShortcut.CtrlY);
            dotNetBarManager.AutoDispatchShortcuts.Add(eShortcut.Del);
            dotNetBarManager.AutoDispatchShortcuts.Add(eShortcut.Ins);

            dotNetBarManager.DefinitionName = "";
            dotNetBarManager.EnableFullSizeDock = false;
            dotNetBarManager.ShowCustomizeContextMenu = false;

            dotNetBarManager.LeftDockSite = leftDockSite;
            dotNetBarManager.RightDockSite = rightDockSite;
            dotNetBarManager.TopDockSite = topDockSite;
            dotNetBarManager.BottomDockSite = bottomDockSite;
            dotNetBarManager.ToolbarLeftDockSite = tbLeftDockSite;
            dotNetBarManager.ToolbarRightDockSite = tbRightDockSite;
            dotNetBarManager.ToolbarTopDockSite = tbTopDockSite;
            dotNetBarManager.ToolbarBottomDockSite = tbBottomDockSite;
            // 
            // leftDockSite
            // 
            leftDockSite.Dock = DockStyle.Left;
            leftDockSite.DocumentDockContainer = new DocumentDockContainer();
            leftDockSite.Name = "leftDockSite";
            // 
            // rightDockSite
            // 
            rightDockSite.Dock = DockStyle.Right;
            rightDockSite.DocumentDockContainer = new DocumentDockContainer();
            rightDockSite.Name = "rightDockSite";
            // 
            // topDockSite
            // 
            topDockSite.Dock = DockStyle.Top;
            topDockSite.DocumentDockContainer = new DocumentDockContainer();
            topDockSite.Name = "topDockSite";
            // 
            // bottomDockSite
            // 
            bottomDockSite.Dock = DockStyle.Bottom;
            bottomDockSite.DocumentDockContainer = new DocumentDockContainer();
            bottomDockSite.Name = "bottomDockSite";
            // 
            // tbLeftDockSite
            // 
            tbLeftDockSite.Dock = DockStyle.Left;
            tbLeftDockSite.Name = "tbLeftDockSite";
            // 
            // tbRightDockSite
            // 
            tbRightDockSite.Dock = DockStyle.Right;
            tbRightDockSite.Name = "tbRightDockSite";
            // 
            // tbTopDockSite
            // 
            tbTopDockSite.Dock = DockStyle.Top;
            tbTopDockSite.Name = "tbTopDockSite";
            // 
            // tbBottomDockSite
            // 
            tbBottomDockSite.Dock = DockStyle.Bottom;
            tbBottomDockSite.Name = "tbBottomDockSite";

            Controls.Add(leftDockSite);
            Controls.Add(rightDockSite);
            Controls.Add(topDockSite);
            Controls.Add(bottomDockSite);
            Controls.Add(tbLeftDockSite);
            Controls.Add(tbRightDockSite);
            Controls.Add(tbTopDockSite);
            Controls.Add(tbBottomDockSite);
        }
#endif

        private void SaveDockState(XmlItem root)
        {
#if !MONO
            XmlItem xi = root.FindItem("Designer").FindItem("DockRibbon");
            xi.SetProp("Text", dotNetBarManager.LayoutDefinition);
#else
            FSideControl.SaveState(root);
            XmlItem xi = root.FindItem("Designer").FindItem("MessagesWindow");
            xi.SetProp("Height", FSplitContainer.SplitterDistance.ToString());
            xi.SetProp("Visible", FSplitContainer.Panel2Collapsed ? "False" : "True");
#endif
        }

        private void RestoreDockState(XmlItem root)
        {
#if !MONO
            XmlItem xi = root.FindItem("Designer").FindItem("DockRibbon");
            string s = xi.GetProp("Text");
            if (s != "")
            {
                // clear toolbar's DockLine property to restore position correctly
                foreach (Bar bar in dotNetBarManager.Bars)
                {
                    if (bar.BarType == eBarType.Toolbar)
                        bar.DockLine = 0;
                }

                dotNetBarManager.LayoutDefinition = s;
            }
#else
            XmlItem xi = root.FindItem("Designer").FindItem("MessagesWindow");
            string height = xi.GetProp("Height");
            if (height != "")
                FSplitContainer.SplitterDistance = int.Parse(height);
            else
                FSplitContainer.SplitterDistance = 1000;
            string visible = xi.GetProp("Visible");
            if (visible == "False")
                FSplitContainer.Panel2Collapsed = true;

            FSideControl.RestoreState(root);
#endif
        }

        private void SaveRecentFiles()
        {
            XmlItem xi = Config.Root.FindItem("Designer");
            string files = "";
            foreach (string s in RecentFiles)
            {
                files += s + '\r';
            }
            xi.SetProp("RecentFiles", files);
        }

        private void RestoreRecentFiles()
        {
            XmlItem xi = Config.Root.FindItem("Designer");
            string[] files = xi.GetProp("RecentFiles").Split(new char[] { '\r' });
            RecentFiles.Clear();
            foreach (string s in files)
            {
                if (!String.IsNullOrEmpty(s))
                    RecentFiles.Add(s);
            }
        }

        /// <summary>
        /// Saves config to a FastReport configuration file.
        /// </summary>
        public void SaveConfig()
        {
            SaveRecentFiles();
            plugins.SaveState();
            SaveDockState(Config.Root);
        }

        /// <summary>
        /// Restores config from a FastReport configuration file.
        /// </summary>
        /// <remarks>
        /// Call this method to restore the designer's layout. You need to do this after the
        /// designer's control is placed on a form.
        /// </remarks>
        public void RestoreConfig()
        {
            SuspendLayout();

            RestoreRecentFiles();
            RestoreDockState(Config.Root);
            plugins.RestoreState();

            ResumeLayout();
        }

        /// <summary>
        /// Refresh the designer's toolbars and toolwindows layout.
        /// </summary>
        /// <remarks>
        /// Call this method if you use 
        /// <see cref="FastReport.Design.StandardDesigner.DesignerControl">DesignerControl</see>. To restore
        /// the layout that you've created in VS design time, you need to call this method in the form's
        /// <b>Load</b> event handler. If you don't do this, tool windows like Properties, Data, Report Tree
        /// will not be available.
        /// </remarks>
        public void RefreshLayout()
        {
            RestoreLayout(layoutState);
        }

        private void RestoreLayout(string value)
        {
            XmlDocument doc = new XmlDocument();
            if (value == null)
                value = "";
            int startIndex = value.IndexOf("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            if (startIndex != -1)
            {
                UTF8Encoding encoding = new UTF8Encoding();
                using (MemoryStream stream = new MemoryStream(encoding.GetBytes(value.Substring(startIndex))))
                {
                    doc.Load(stream);
                }
            }

            SuspendLayout();
            RestoreDockState(doc.Root);
            ResumeLayout();
        }

        /// <inheritdoc/>
        public void BeginInit()
        {
            initFlag = true;
        }

        /// <inheritdoc/>
        public void EndInit()
        {
            if (initFlag)
            {
                if (layoutNeeded)
                    RestoreLayout(layoutState);
                initFlag = false;
                layoutNeeded = false;

#if !MONO
                if (DesignMode)
                    DisableFloatingBars();
#endif
            }
        }

#if !MONO
        private void DisableFloatingBars()
        {
            foreach (Bar bar in dotNetBarManager.Bars)
            {
                bar.CanUndock = false;
            }
        }

        private void dockSite_MouseUp(object sender, MouseEventArgs e)
        {
            LayoutChanged(sender, EventArgs.Empty);
        }

        internal void LayoutChanged(object sender, EventArgs e)
        {
            if (DesignMode && !initFlag)
            {
                if (LayoutChangedEvent != null)
                    LayoutChangedEvent(this, EventArgs.Empty);
            }
        }
#endif
#endregion

#region Commands
        private void InitCommands()
        {
            fcmdNew = new NewCommand(this);
            fcmdNewPage = new NewPageCommand(this);
            fcmdNewDialog = new NewDialogCommand(this);
            fcmdOpen = new OpenCommand(this);
            fcmdSave = new SaveCommand(this);
            fcmdSaveAs = new SaveAsCommand(this);
            fcmdSaveAll = new SaveAllCommand(this);
            fcmdClose = new CloseCommand(this);
            fcmdCloseAll = new CloseAllCommand(this);
            fcmdPreview = new PreviewCommand(this);
            fcmdPrinterSetup = new PrinterSettingsCommand(this);
            fcmdPageSetup = new PageSettingsCommand(this);
            fcmdAddData = new AddDataCommand(this);
            fcmdSortDataSources = new SortDataSourcesCommand(this);
            fcmdChooseData = new ChooseDataCommand(this);
            fcmdUndo = new UndoCommand(this);
            fcmdRedo = new RedoCommand(this);
            fcmdCut = new CutCommand(this);
            fcmdCopy = new CopyCommand(this);
            fcmdPaste = new PasteCommand(this);
            fcmdFormatPainter = new FormatPainterCommand(this);
            fcmdDelete = new DeleteCommand(this);
            copyPageCommand = new CopyPageCommand(this);
            fcmdDeletePage = new DeletePageCommand(this);
            fcmdSelectAll = new SelectAllCommand(this);
            fcmdGroup = new GroupCommand(this);
            fcmdUngroup = new UngroupCommand(this);
            fcmdEdit = new EditCommand(this);
            fcmdFind = new FindCommand(this);
            fcmdReplace = new ReplaceCommand(this);
            fcmdBringToFront = new BringToFrontCommand(this);
            fcmdSendToBack = new SendToBackCommand(this);
            fcmdInsert = new InsertCommand(this);
            fcmdInsertBand = new InsertBandCommand(this);
            fcmdRecentFiles = new RecentFilesCommand(this);
            fcmdSelectLanguage = new SelectLanguageCommand(this);
            fcmdViewStartPage = new ViewStartPageCommand(this);
            fcmdReportSettings = new ReportSettingsCommand(this);
            fcmdOptions = new OptionsCommand(this);
            fcmdReportStyles = new ReportStylesCommand(this);
            fcmdHelpContents = new HelpContentsCommand(this);
            fcmdAbout = new AboutCommand(this);
            fcmdWelcome = new WelcomeCommand(this);
            fcmdPolySelectAddPoint = new PolygonSelectModeCommand(this, PolyLineObject.PolygonSelectionMode.AddToLine);
            fcmdPolySelectBezier = new PolygonSelectModeCommand(this, PolyLineObject.PolygonSelectionMode.AddBezier);
            fcmdPolySelectMove = new PolygonSelectModeCommand(this, PolyLineObject.PolygonSelectionMode.MoveAndScale);
            fcmdPolySelectPointer = new PolygonSelectModeCommand(this, PolyLineObject.PolygonSelectionMode.Normal);
            fcmdPolySelectRemovePoint = new PolygonSelectModeCommand(this, PolyLineObject.PolygonSelectionMode.Delete);

        }

        internal void InitPages(int pageNo)
        {
            ActiveReportTab.InitPages(pageNo);
        }

        /// <summary>
        /// Initializes the workspace after the new report is loaded.
        /// </summary>
        public void InitReport()
        {
            ActiveReportTab.InitReport();
            ActiveReportTab.SetModified();
        }

        internal void InsertObject(ObjectInfo[] infos, InsertFrom source)
        {
            ObjectCollection list = new ObjectCollection();
            CancelPaste();

            foreach (ObjectInfo info in infos)
            {
                Base c = Activator.CreateInstance(info.Object) as Base;
                LastFormatting.SetFormatting(c as ReportComponentBase);
                c.OnBeforeInsert(info.Flags);

                list.Add(c);
                //c.Parent = ActiveReportTab.ActivePageDesigner.GetParentForPastedObjects();

                Config.DesignerSettings.OnObjectInserted(this, new ObjectInsertedEventArgs(c, source));
            }

            ActiveReportTab.ActivePageDesigner.Paste(list, source);
        }

        internal void InsertObject(ObjectInfo info, InsertFrom source)
        {
            InsertObject(new ObjectInfo[] { info }, source);
        }

        internal void ResetObjectsToolbar(bool ignoreMultiInsert)
        {
            objectsToolbar.ClickSelectButton(ignoreMultiInsert);
            FormatPainter = false;
        }

        internal void Exit(object sender, EventArgs e)
        {
            Form form = FindForm();
            if (form != null)
                form.Close();
        }

        /// <summary>
        /// Tries to create a new empty report.
        /// </summary>
        /// <returns><b>true</b> if report was created successfully; <b>false</b> if user cancels the action.</returns>
        public bool CreateEmptyReport()
        {
            bool result = false;
            if (MdiMode)
            {
                Report report = new Report();
                report.Designer = this;
                Config.DesignerSettings.OnReportLoaded(this, new ReportLoadedEventArgs(report));
                AddReportTab(CreateReportTab(report));
                result = true;
            }
            else
                result = ActiveReportTab.EmptyReport(true);

            if (result)
            {
                ActiveReportTab.SetModified();
                UpdatePlugins(null);
            }

            return result;
        }

        internal void ErrorMsg(string msg, string objName)
        {
            MessagesWindow.AddMessage(msg, objName);
        }

        internal void ErrorMsg(string msg, int line, int column)
        {
            MessagesWindow.AddMessage(msg, line, column);
        }

        /// <summary>
        /// Displays a message in the "Messages" window.
        /// </summary>
        /// <param name="msg">Message text.</param>
        public void ShowMessage(string msg)
        {
            MessagesWindow.AddMessage(msg, 1, 1);
        }

        /// <summary>
        /// Clears the "Messages" window.
        /// </summary>
        public void ClearMessages()
        {
            MessagesWindow.ClearMessages();
        }

        /// <summary>
        /// Shows the selected object's information in the designer's statusbar.
        /// </summary>
        /// <param name="location">Object's location.</param>
        /// <param name="size">Object's size.</param>
        /// <param name="text">Textual information about the selected object.</param>
        public virtual void ShowStatus(string location, string size, string text)
        {
        }

        /// <summary>
        /// Close all opened reports, ask to save changes.
        /// </summary>
        /// <returns><b>true</b> if all tabs closed succesfully.</returns>
        /// <remarks>
        /// Use this method to close all opened documents and save changes when you closing the main form
        /// that contains the designer control. To do this, create an event handler for your form's FormClosing 
        /// event and call this method inside the handler. If it returns false, set e.Cancel to <b>true</b>.
        /// </remarks>
        public bool CloseAll()
        {
            // close all tabs from the last tab to first.
            while (Documents.Count > 0)
            {
                DocumentWindow c = Documents[Documents.Count - 1];
                if (!CloseDocument(c))
                {
                    // tab cannot be closed. Activate it and exit. Do not allow to close the designer.
                    c.Activate();
                    return false;
                }
            }

            return true;
        }

#endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Designer"/> class with default settings.
        /// </summary>
        public Designer()
        {
            Config.MainForm = this;
            Report.EnsureInit();

            plugins = new PluginCollection(this);
            recentFiles = new List<string>();
            clipboard = new DesignerClipboard(this);
            documents = new List<DocumentWindow>();
            objects = new ObjectCollection();
            selectedObjects = new SelectedObjectCollection();
            previouslySelectedObjects = new SelectedObjectCollection();
            selectedComponents = new SelectedComponents(this);
            selectedReportComponents = new SelectedReportComponents(this);
            selectedTextObjects = new SelectedTextObjects(this);
            restrictions = Config.DesignerSettings.Restrictions.Clone();
            askSave = true;
            lastFormatting = new LastFormatting();

            InitCommands();
#if !MONO
            BarUtilities.UseTextRenderer = true;
            CreateLayout();
#endif
            InitPluginsInternal();
            UpdatePlugins(null);
            UpdateMdiMode();
            UIStyle = Config.UIStyle;
            SetupAutoSave();

            // Right to Left layout
            RightToLeft = Config.RightToLeft ? RightToLeft.Yes : RightToLeft.No;
#if !MONO
            objectsToolbar.DockSide = Config.RightToLeft ? eDockSide.Right : eDockSide.Left;
#endif
            //SaveConfig();
        }
    }
}
