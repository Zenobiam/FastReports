using FastReport.Data;
using FastReport.Export.Html;
using FastReport.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;

namespace FastReport.Web
{
    /// <summary>
    /// Represents the Web Report.
    /// </summary>
    [Designer("FastReport.VSDesign.WebReportComponentDesigner, FastReport.VSDesign, Version=1.0.0.0, Culture=neutral, PublicKeyToken=db7e5ce63278458c, processorArchitecture=MSIL")]
    [ToolboxBitmap(typeof(WebReport), "Resources.Report.bmp")]
    public partial class WebReport : WebControl, INamingContainer
    {
        #region Private fields

        private string fReportGuid;
        private WebToolbar fToolbar;
        private int currentReportIndex = -1;
        private List<ReportTab> tabs;
        private bool fPreviewMode = false;
        private WebReportCache cache;
        private WebRes reportRes = new WebRes();

        #endregion Private fields

        /// <inheritdoc/>
        public override Unit Width
        {
            get {
                return base.Width;
            }
            set { 
                Prop.WidthString = value.ToString();
                base.Width = value;
            }
        }

        /// <inheritdoc/>
        public override Unit Height
        {
            get {
                return base.Height;
            }
            set { 
                Prop.HeightString = value.ToString();
                base.Height = value;
            }
        }

        internal WebRes ReportRes
        {
            get { return reportRes; }
        }

        #region Public properties

        #region Layout

        /// <summary>
        /// Used only if layers mode is off
        /// </summary>
        [DefaultValue(false)]
        [Category("Layout")]
        [Browsable(true)]
        public bool AutoWidth
        {
            get { return Prop.AutoWidth; }
            set { Prop.AutoWidth = value; }
        }

        /// <summary>
        /// Used only if layers mode is off
        /// </summary>
        [DefaultValue(false)]
        [Category("Layout")]
        [Browsable(true)]
        public bool AutoHeight
        {
            get { return Prop.AutoHeight; }
            set { Prop.AutoHeight = value; }
        }

        /// <summary>
        /// Enable or disable inline object registration
        /// </summary>
        [DefaultValue(false)]
        [Category("Layout")]
        [Browsable(true)]
        public bool InlineRegistration
        {
            get { return Prop.InlineRegistration; }
            set { Prop.InlineRegistration = value; }
        }

        /// <summary>
        ///
        /// </summary>
        [DefaultValue(false)]
        [Category("Layout")]
        [Browsable(true)]
        public bool EnableMargins
        {
            get { return Prop.EnableMargins; }
            set { Prop.EnableMargins = value; }
        }

        /// <summary>
        /// Enable or disable using of external jQuery library
        /// </summary>
        [DefaultValue(false)]
        [Category("Layout")]
        [Browsable(true)]
        public bool ExternalJquery
        {
            get { return Prop.ExternalJquery; }
            set { Prop.ExternalJquery = value; }
        }

        /// <summary>
        /// Enable or disable layers mode visualisation
        /// </summary>
        [DefaultValue(true)]
        [Category("Layout")]
        [Browsable(true)]
        public bool Layers
        {
            get { return Prop.Layers; }
            set { Prop.Layers = value; }
        }

        /// <summary>
        /// Gets or sets Padding of Report section
        /// </summary>
        [Category("Layout")]
        [Browsable(true)]
        public System.Windows.Forms.Padding Padding
        {
            get { return Prop.Padding; }
            set { Prop.Padding = value; }
        }

        #endregion Layout

        #region Network

        /// <summary>
        /// Timeout in seconds for automatic refresh of report. Zero value disable auto-refresh.
        /// </summary>
        [Category("Network")]
        [DefaultValue(0)]
        [Browsable(true)]
        public int RefreshTimeout
        {
            get { return Prop.RefreshTimeout; }
            set { Prop.RefreshTimeout = value; }
        }

        /// <summary>
        /// Delay in cache in minutes
        /// </summary>
        [Category("Network")]
        [DefaultValue(60)]
        [Browsable(true)]
        public int CacheDelay
        {
            get { return Prop.CacheDelay; }
            set { Prop.CacheDelay = value; }
        }

        /// <summary>
        /// Priority of items in cache
        /// </summary>
        [Category("Network")]
        [DefaultValue(CacheItemPriority.Normal)]
        [Browsable(true)]
        public CacheItemPriority CachePriority
        {
            get { return Prop.CachePriority; }
            set { Prop.CachePriority = value; }
        }

        /// <summary>
        /// Enable or disable the multiple instances environment
        /// </summary>
        [Category("Network")]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool CloudEnvironmet
        {
            get { return Prop.CloudEnvironmet; }
            set { Prop.CloudEnvironmet = value; }
        }

        internal static ScriptSecurity ScriptSecurity = null;

        #endregion Network

        #region Designer

        /// <summary>
        /// Enable the Report Designer
        /// </summary>
        [DefaultValue(false)]
        [Category("Designer")]
        [Browsable(true)]
        public bool DesignReport
        {
            get { return Prop.DesignReport; }
            set { Prop.DesignReport = value; }
        }

        /// <summary>
        /// Enable code editor in the Report Designer
        /// </summary>
        [DefaultValue(false)]
        [Category("Designer")]
        [Browsable(true)]
        public bool DesignScriptCode
        {
            get { return Prop.DesignScriptCode; }
            set { Prop.DesignScriptCode = value; }
        }

        /// <summary>
        /// Gets or sets path to the Report Designer
        /// </summary>
        [DefaultValue("~/WebReportDesigner/index.html")]
        [Category("Designer")]
        [Browsable(true)]
        public string DesignerPath
        {
            get { return Prop.DesignerPath; }
            set { Prop.DesignerPath = value; }
        }

        /// <summary>
        /// Gets or sets path to a folder for save designed reports
        /// If value is empty then designer posts saved report in variable ReportFile on call the DesignerSaveCallBack
        /// </summary>
        [DefaultValue("")]
        [Category("Designer")]
        [Browsable(true)]
        public string DesignerSavePath
        {
            get { return Prop.DesignerSavePath; }
            set { Prop.DesignerSavePath = value; }
        }

        /// <summary>
        /// Gets or sets path to callback page after Save from Designer
        /// </summary>
        [DefaultValue("")]
        [Category("Designer")]
        [Browsable(true)]
        public string DesignerSaveCallBack
        {
            get { return Prop.DesignerSaveCallBack; }
            set { Prop.DesignerSaveCallBack = value; }
        }

        /// <summary>
        /// Gets or sets the locale of Designer
        /// </summary>
        [DefaultValue("")]
        [Category("Designer")]
        [Browsable(true)]
        public string DesignerLocale
        {
            get { return Prop.DesignerLocale; }
            set { Prop.DesignerLocale = value; }
        }

        /// <summary>
        /// Gets or sets the text of configuration of Online Designer
        /// </summary>
        [DefaultValue("")]
        [Category("Designer")]
        [Browsable(true)]
        public string DesignerConfig
        {
            get { return Prop.DesignerConfig; }
            set { Prop.DesignerConfig = value; }
        }

        #endregion Designer

        #region Report

        /// <summary>
        /// Report Resource String. 
        /// </summary>
        [DefaultValue("")]
        [Category("Report")]
        [Browsable(true)]
        public string ReportResourceString
        {
            get { return Prop.ReportResourceString; }
            set { Prop.ReportResourceString = value; }
        }

        /// <summary>
        /// Gets or sets report data source(s).
        /// </summary>
        /// <remarks>
        /// To pass several datasources, use ';' delimiter, for example:
        /// "sqlDataSource1;sqlDataSource2"
        /// </remarks>
        [DefaultValue("")]
        [Category("Report")]
        [Browsable(true)]
        public string ReportDataSources
        {
            get { return Prop.ReportDataSources; }
            set { Prop.ReportDataSources = value; }
        }

        /// <summary>
        /// Switches the pictures visibility in report
        /// </summary>
        [DefaultValue(true)]
        [Category("Report")]
        [Browsable(true)]
        public bool Pictures
        {
            get { return Prop.Pictures; }
            set { Prop.Pictures = value; }
        }

        /// <summary>
        /// Enables or disables embedding pictures in HTML (inline HTML5 images).
        /// </summary>
        [Category("Report")]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool EmbedPictures
        {
            get { return Prop.EmbedPictures; }
            set { Prop.EmbedPictures = value; }
        }

        /// <summary>
        /// Switches the pages layout between multiple pages (false) and single page (true).
        /// </summary>
        [DefaultValue(false)]
        [Category("Report")]
        [Browsable(true)]
        public bool SinglePage
        {
            get { return Prop.SinglePage; }
            set { Prop.SinglePage = value; }
        }

        /// <summary>
        /// Gets or sets the name of report file.
        /// </summary>
        [DefaultValue("")]
        [Category("Report")]
        [Browsable(true)]
        [Editor("FastReport.VSDesign.ReportFileEditor, FastReport.VSDesign, Version=1.0.0.0, Culture=neutral, PublicKeyToken=db7e5ce63278458c, processorArchitecture=MSIL", typeof(UITypeEditor))]
        public string ReportFile
        {
            get { return Prop.ReportFile; }
            set { Prop.ReportFile = value; }
        }

        /// <summary>
        /// Gets or sets the default path for reports (including inherited).
        /// </summary>
        public string ReportPath
        {
            get { return Prop.ReportPath; }
            set { Prop.ReportPath = value; }
        }

        /// <summary>
        /// Gets or sets the name of localization file.
        /// </summary>
        [DefaultValue("")]
        [Category("Report")]
        [Browsable(true)]
        [Editor("FastReport.VSDesign.LocalizationFileEditor, FastReport.VSDesign, Version=1.0.0.0, Culture=neutral, PublicKeyToken=db7e5ce63278458c, processorArchitecture=MSIL", typeof(UITypeEditor))]
        public string LocalizationFile
        {
            get { return Prop.LocalizationFile; }
            set
            {
                Prop.LocalizationFile = value;
            }
        }

        /// <summary>
        /// Sets the zoom scale factor of previewed page between 0..1f.
        /// </summary>
        [DefaultValue(1f)]
        [Category("Report")]
        [Browsable(true)]
        public float Zoom
        {
            get { return Prop.Zoom; }
            set { Prop.Zoom = value; }
        }

        /// <summary>
        /// Sets the zoom mode of previewed page.
        /// </summary>
        [DefaultValue(ZoomMode.Scale)]
        [Category("Report")]
        [Browsable(true)]
        public ZoomMode ZoomMode
        {
            get { return Prop.ZoomMode; }
            set { Prop.ZoomMode = value; }
        }

        /// <summary>
        /// Enables or disables showing the report after call Prepare.
        /// </summary>
        [DefaultValue(false)]
        [Category("Report")]
        [Browsable(true)]
        public bool ShowAfterPrepare
        {
            get { return Prop.ShowAfterPrepare; }
            set { Prop.ShowAfterPrepare = value; }
        }

        /// <summary>
        /// Enables or disables showing any debug information on errors.
        /// </summary>
        [DefaultValue(false)]
        [Category("Report")]
        [Browsable(true)]
        public bool Debug
        {
            get { return Prop.Debug; }
            set { Prop.Debug = value; }
        }

        /// <summary>
        /// Gets or sets the log file path. 
        /// </summary>
        [DefaultValue("")]
        [Category("Report")]
        [Browsable(true)]
        public string LogFile
        {
            get { return Prop.LogFile; }
            set { Prop.LogFile = value; }
        }

        /// <summary>
        /// Enables or disables unlimited width of report pages. This options overrides the report page property value 
        /// when it set to true.
        /// </summary>
        [DefaultValue(false)]
        [Category("Report")]
        [Browsable(true)]
        public bool UnlimitedWidth
        {
            get { return Prop.UnlimitedWidth; }
            set { Prop.UnlimitedWidth = value; }
        }

        /// <summary>
        /// Enables or disables unlimited height of report pages. This options overrides the report page property value
        /// when it set to true.
        /// </summary>
        [DefaultValue(false)]
        [Category("Report")]
        [Browsable(true)]
        public bool UnlimitedHeight
        {
            get { return Prop.UnlimitedHeight; }
            set { Prop.UnlimitedHeight = value; }
        }

        /// <summary>
        /// Enables or disables showing the report dialogs.
        /// </summary>
        [DefaultValue(true)]
        [Category("Report")]
        [Browsable(true)]
        public bool Dialogs
        {
            get { return Prop.Dialogs; }
            set { Prop.Dialogs = value; }
        }

        #endregion Report

        #region Toolbar

        /// <summary>
        /// Sets the toolbar style - small or large.
        /// </summary>
        [DefaultValue(ToolbarStyle.Large)]
        [Category("Toolbar")]
        [Browsable(true)]
        public ToolbarStyle ToolbarStyle
        {
            get { return Prop.ToolbarStyle; }
            set { Prop.ToolbarStyle = value; }
        }

        /// <summary>
        /// Sets the toolbar icons style.
        /// </summary>
        [DefaultValue(ToolbarIconsStyle.Red)]
        [Category("Toolbar")]
        [Browsable(true)]
        public ToolbarIconsStyle ToolbarIconsStyle
        {
            get { return Prop.ToolbarIconsStyle; }
            set { Prop.ToolbarIconsStyle = value; }
        }

        /// <summary>
        /// Sets the toolbar background style.
        /// </summary>
        [DefaultValue(ToolbarBackgroundStyle.Light)]
        [Category("Toolbar")]
        [Browsable(true)]
        public ToolbarBackgroundStyle ToolbarBackgroundStyle
        {
            get { return Prop.ToolbarBackgroundStyle; }
            set { Prop.ToolbarBackgroundStyle = value; }
        }

        /// <summary>
        /// Switches the toolbar visibility.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowToolbar
        {
            get { return Prop.ShowToolbar; }
            set { Prop.ShowToolbar = value; }
        }

        /// <summary>
        /// Switches the bottom toolbar visibility.
        /// </summary>
        [DefaultValue(false)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowBottomToolbar
        {
            get { return Prop.ShowBottomToolbar; }
            set { Prop.ShowBottomToolbar = value; }
        }

        /// <summary>
        /// Sets the path to the custom buttons on site.
        /// </summary>
        /// <remarks>
        /// Pictures should be named:
        /// Checkbox.gif, Progress.gif, toolbar.png, toolbar_background.png, toolbar_big.png, toolbar_disabled.png, toolbar_disabled.png
        /// </remarks>
        [DefaultValue("")]
        [Category("Toolbar")]
        [Browsable(true)]
        //[UrlProperty]
        [Editor("FastReport.VSDesign.ReportFileEditor, FastReport.VSDesign, Version=1.0.0.0, Culture=neutral, PublicKeyToken=db7e5ce63278458c, processorArchitecture=MSIL", typeof(UITypeEditor))]
        public string ButtonsPath
        {
            get { return Prop.ButtonsPath; }
            set { Prop.ButtonsPath = value; }
        }

        /// <summary>
        /// Switches the visibility of Exports in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowExports
        {
            get { return Prop.ShowExports; }
            set { Prop.ShowExports = value; }
        }

        /// <summary>
        /// Switches the visibility of Print button in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowPrint
        {
            get { return Prop.ShowPrint; }
            set { Prop.ShowPrint = value; }
        }

        /// <summary>
        /// Switches the visibility of First Button in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowFirstButton
        {
            get { return Prop.ShowFirstButton; }
            set { Prop.ShowFirstButton = value; }
        }

        /// <summary>
        /// Switches the visibility of Previous Button in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowPrevButton
        {
            get { return Prop.ShowPrevButton; }
            set { Prop.ShowPrevButton = value; }
        }

        /// <summary>
        /// Switches the visibility of Next Button in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowNextButton
        {
            get { return Prop.ShowNextButton; }
            set { Prop.ShowNextButton = value; }
        }

        /// <summary>
        /// Switches the visibility of Last Button in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowLastButton
        {
            get { return Prop.ShowLastButton; }
            set { Prop.ShowLastButton = value; }
        }

        /// <summary>
        /// Switches the visibility of Zoom in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowZoomButton
        {
            get { return Prop.ShowZoomButton; }
            set { Prop.ShowZoomButton = value; }
        }

        /// <summary>
        /// Switches the visibility of Refresh in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowRefreshButton
        {
            get { return Prop.ShowRefreshButton; }
            set { Prop.ShowRefreshButton = value; }
        }

        /// <summary>
        /// Switches the visibility of Page Number in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowPageNumber
        {
            get { return Prop.ShowPageNumber; }
            set { Prop.ShowPageNumber = value; }
        }

        /// <summary>
        /// Switches the visibility of closing buttons for Tabs.
        /// </summary>
        [DefaultValue(false)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowTabCloseButton
        {
            get { return Prop.ShowTabCloseButton; }
            set { Prop.ShowTabCloseButton = value; }
        }

        /// <summary>
        /// Switches the visibility of Back buttons.
        /// </summary>
        [DefaultValue(false)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowBackButton
        {
            get { return Prop.ShowBackButton; }
            set { Prop.ShowBackButton = value; }
        }

        /// <summary>
        /// Selects the position of tabs.
        /// </summary>
        [DefaultValue(TabPosition.InsideToolbar)]
        [Category("Toolbar")]
        [Browsable(true)]
        public TabPosition TabPosition
        {
            get { return Prop.TabPosition; }
            set { Prop.TabPosition = value; }
        }

        /// <summary>
        /// Sets the Toolbar background color.
        /// </summary>
        [DefaultValue(0xECECEC)]
        [Category("Toolbar")]
        [Browsable(true)]
        public System.Drawing.Color ToolbarColor
        {
            get { return Prop.ToolbarColor; }
            set { Prop.ToolbarColor = value; }
        }

        /// <summary>
        /// Switches the outline visibility.
        /// </summary>
        [DefaultValue(false)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowOutline
        {
            get { return Prop.ShowOutline; }
            set { Prop.ShowOutline = value; }
        }

        #endregion Toolbar

        #region Print

        /// <summary>
        /// Enable or disable print in PDF.
        /// </summary>
        [DefaultValue(true)]
        [Category("Print")]
        [Browsable(true)]
        public bool PrintInPdf
        {
            get { return Prop.PrintInPdf; }
            set { Prop.PrintInPdf = value; }
        }

        /// <summary>
        /// Enable or disable print in browser.
        /// </summary>
        [DefaultValue(true)]
        [Category("Print")]
        [Browsable(true)]
        public bool PrintInBrowser
        {
            get { return Prop.PrintInBrowser; }
            set { Prop.PrintInBrowser = value; }
        }

        /// <summary>
        /// Sets the width of print window.
        /// </summary>
        [DefaultValue("700px")]
        [Category("Print")]
        [Browsable(true)]
        public string PrintWindowWidth
        {
            get { return Prop.PrintWindowWidth; }
            set { Prop.PrintWindowWidth = value; }
        }

        /// <summary>
        /// Sets the height of print window.
        /// </summary>
        [DefaultValue("500px")]
        [Category("Print")]
        [Browsable(true)]
        public string PrintWindowHeight
        {
            get { return Prop.PrintWindowHeight; }
            set { Prop.PrintWindowWidth = value; }
        }

        #endregion Print

        #region Hidden Properties

        [Browsable(false)]
        internal bool ControlVisible
        {
            get { return Prop.ControlVisible; }
            set { Prop.ControlVisible = value; }
        }

        [Browsable(false)]
        internal List<ReportTab> ReportTabs
        {
            get
            {
                InitTabs();
                return tabs;
            }
            set
            {
                tabs = value;
                InitTabs();
            }
        }

        /// <summary>
        /// Direct access to the Properties of report object.
        /// </summary>
        [Browsable(false)]
        internal WebReportProperties Prop
        {
            get
            {
                return ReportTabs[currentReportIndex].Properties;
            }
            set
            {
                ReportTabs[currentReportIndex].Properties = value;
            }
        }

        /// <summary>
        /// Direct access to the Tabs.
        /// </summary>
        [Browsable(false)]
        public List<ReportTab> Tabs
        {
            get
            {
                return ReportTabs;
            }
        }

        /// <summary>
        /// Gets or sets the current tab index.
        /// </summary>
        [Browsable(false)]
        public int CurrentTabIndex
        {
            get
            {
                return currentReportIndex;
            }

            set
            {
                currentReportIndex = value;
                Toolbar.CurrentTabIndex = currentReportIndex;
            }
        }

        /// <summary>
        /// Gets the current tab.
        /// </summary>
        [Browsable(false)]
        public ReportTab CurrentTab
        {
            get
            {
                return ReportTabs[currentReportIndex];
            }
        }

        /// <summary>
        ///
        /// </summary>
        [Browsable(false)]
        internal WebToolbar Toolbar
        {
            get
            {
                if (fToolbar == null)
                {
                    fToolbar = new WebToolbar(
                        ReportGuid,
                        ReportTabs,
                        (this.Width.Type == UnitType.Pixel) && (this.Height.Type == UnitType.Pixel),
                        reportRes);
                    fToolbar.BackgroundColor = WebUtils.RGBAColor(this.BackColor);
                }
                return fToolbar;
            }
            set
            {
                fToolbar = value;
            }
        }

        /// <summary>
        /// Enables the preview mode for working together the Online Designer.
        /// </summary>
        [Browsable(false)]
        public bool PreviewMode
        {
            get { return fPreviewMode; }
            set { fPreviewMode = value; }
        }

        /// <summary>
        /// Direct access to Report object.
        /// </summary>
        [Browsable(false)]
        public Report Report
        {
            get
            {
                return ReportTabs[currentReportIndex].Report;
            }
            set
            {
                ReportTabs[currentReportIndex].Report = value;
            }
        }

        /// <summary>
        /// Gets total pages of current report.
        /// </summary>
        [Browsable(false)]
        public int TotalPages
        {
            get
            {
                if (Report.PreparedPages != null)
                    return Report.PreparedPages.Count;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Gets or sets current state of report.
        /// </summary>
        [Browsable(false)]
        public ReportState State
        {
            get { return Prop.State; }
            set { Prop.State = value; }
        }

        /// <summary>
        /// Returns true when report done. This property may be set in true for forcing load the prepared report.
        /// </summary>
        [Browsable(false)]
        public bool ReportDone
        {
            get { return Prop.ReportDone; }
            set { Prop.ReportDone = value; }
        }

        /// <summary>
        /// Gets or sets number of current page.
        /// </summary>
        [Browsable(false)]
        public int CurrentPage
        {
            get { return Prop.CurrentPage; }
            set { Prop.CurrentPage = value; }
        }

        /// <summary>
        /// Gets or sets guid of report.
        /// </summary>
        [Browsable(false)]
        public string ReportGuid
        {
            get
            {
                if (String.IsNullOrEmpty(fReportGuid))
                {
                    fReportGuid = TryGetGUID();
                }
                return fReportGuid;
            }
            set { fReportGuid = value; }
        }

        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        [Browsable(false)]
        public WebHeaderCollection RequestHeaders
        {
            get { return Prop.RequestHeaders; }
            set { Prop.RequestHeaders = value; }
        }

        /// <summary>
        /// Gets or sets the response headers.
        /// </summary>
        [Browsable(false)]
        public WebHeaderCollection ResponseHeaders
        {
            get { return Prop.ResponseHeaders; }
            set { Prop.ResponseHeaders = value; }
        }

        private string TryGetGUID()
        {
            string guid = String.Empty;
            if (this.Context != null && !String.IsNullOrEmpty(this.ID))
            {
                if (this.Context.Session != null && this.Context.Session[this.ID] != null)
                    guid = this.Context.Session[this.ID] as string;
            }
            if (String.IsNullOrEmpty(guid))
                guid = WebUtils.GetGUID();
            return guid;
        }

        #endregion Hidden Properties

        #endregion Public properties

        #region Events

        /// <summary>
        /// Occurs when report execution is started.
        /// </summary>
        [Browsable(true)]
        public event EventHandler StartReport;

        /// <summary>
        /// Occurs when designed report saving is started.
        /// </summary>
        [Browsable(true)]
        public event EventHandler<SaveDesignedReportEventArgs> SaveDesignedReport;

        /// <summary>
        /// Occurs when report drawing is started.
        /// </summary>
        [Browsable(true)]
        public event EventHandler<CustomDrawEventArgs> CustomDraw;

        /// <summary>
        /// Occurs when report auth is started.
        /// </summary>
        [Browsable(true)]
        public event EventHandler<CustomAuthEventArgs> CustomAuth;

        /// <summary>
        /// Runs the OnCustomDraw event.
        /// </summary>
        /// <param name="e">CustomDrawEventArgs object.</param>
        public void OnCustomDraw(CustomDrawEventArgs e)
        {
            if (CustomDraw != null)
            {
                CustomDraw(this, e);
            }
        }

        /// <summary>
        /// Runs the OnCustomAuth event.
        /// </summary>
        /// <param name="e">CustomAuthEventArgs object.</param>
        public void OnCustomAuth(CustomAuthEventArgs e)
        {
            if (CustomAuth != null)
            {
                CustomAuth(this, e);
            }
        }

        #endregion Events

        /// <summary>
        /// Runs the StartReport event.
        /// </summary>
        /// <param name="e">EventArgs object.</param>
        public void OnStartReport(EventArgs e)
        {
            if (StartReport != null)
            {
                StartReport(this, e);
            }
        }

        /// <summary>
        /// Runs the SaveDesignedReport event.
        /// </summary>
        /// <param name="e">SaveDesignedReportEventArgs object.</param>
        public void OnSaveDesignedReport(SaveDesignedReportEventArgs e)
        {
            if (SaveDesignedReport != null)
            {
                SaveDesignedReport(this, e);
            }
        }

        #region Navigation

        /// <summary>
        /// Forces going to the next report page.
        /// </summary>
        public void NextPage()
        {
            if (CurrentPage < TotalPages - 1)
                CurrentPage++;
        }

        /// <summary>
        /// Forces going to the previous report page.
        /// </summary>
        public void PrevPage()
        {
            if (CurrentPage > 0)
                CurrentPage--;
        }

        /// <summary>
        /// Forces going to the first report page.
        /// </summary>
        public void FirstPage()
        {
            CurrentPage = 0;
        }

        /// <summary>
        /// Forces going to the last report page.
        /// </summary>
        public void LastPage()
        {
            CurrentPage = TotalPages - 1;
        }

        /// <summary>
        /// Forces going to the "value" report page.
        /// </summary>
        public void SetPage(int value)
        {
            if (value >= 0 && value < TotalPages)
                CurrentPage = value;
        }

        #endregion Navigation

        /// <summary>
        /// Prepares the report.
        /// </summary>
        public void Prepare()
        {
            //this.Controls.Clear();
            Refresh();
        }

        /// <summary>
        /// Forces refreshing of report.
        /// </summary>
        public void Refresh()
        {
            CurrentPage = 0;
            Prop.CurrentForm = 0;
            State = ReportState.Empty;
            ControlVisible = true;
            ShowAfterPrepare = false;
        }

        /// <summary>
        /// Adds the new report tab.
        /// </summary>
        /// <param name="report"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ReportTab AddTab(Report report, string name)
        {
            return AddTab(report, name, false);
        }

        /// <summary>
        /// Adds the new report tab.
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public ReportTab AddTab(Report report)
        {
            return AddTab(report, String.Empty);
        }

        /// <summary>
        /// Adds the new report tab.
        /// </summary>
        /// <param name="report"></param>
        /// <param name="name"></param>
        /// <param name="reportDone"></param>
        /// <returns></returns>
        public ReportTab AddTab(Report report, string name, bool reportDone)
        {
            return AddTab(report, name, reportDone, false);
        }

        /// <summary>
        /// Adds the new report tab.
        /// </summary>
        /// <param name="report"></param>
        /// <param name="name"></param>
        /// <param name="reportDone"></param>
        /// <param name="needParent"></param>
        /// <returns></returns>
        public ReportTab AddTab(Report report, string name, bool reportDone, bool needParent)
        {
            ReportTab tab = new ReportTab(report);
            tab.Properties.Assign(tabs[0].Properties);
            tab.Properties.ControlID = tabs[0].Properties.ControlID;
            tab.Properties.HandlerURL = WebUtils.GetBasePath(this.Context) + WebUtils.HandlerFileName;
            tab.Properties.ReportDone = reportDone;
            tab.Properties.ShowRefreshButton = !reportDone;
            tab.NeedParent = needParent;
            if (!String.IsNullOrEmpty(name))
                tab.Name = name;
            ReportTabs.Add(tab);
            return tab;
        }

        #region Protected methods

        private Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
                return root;

            foreach (Control ctl in root.Controls)
            {
                Control foundCtl = FindControlRecursive(ctl, id);
                if (foundCtl != null)
                    return foundCtl;
            }

            return null;
        }

        internal void RegisterData(Report report)
        {
            if (Page != null)
            {
                string[] dataSources = ReportDataSources.Split(new char[] { ';' });
                foreach (string dataSource in dataSources)
                {
                    IDataSource ds = FindControlRecursive(Page, dataSource) as IDataSource;
                    if (ds == null)
                        continue;
                    string dataName = (ds as Control).ID;
                    RegisterDataAsp(report, ds, dataName);
                }
            }
        }

        /// <summary>
        /// Registers the ASP.NET application data to use it in the report.
        /// </summary>
        /// <param name="report">The <b>Report</b> object.</param>
        /// <param name="data">The application data.</param>
        /// <param name="name">The name of the data.</param>
        public void RegisterDataAsp(Report report, IDataSource data, string name)
        {
            aspDataName = name;
            this.report = report;
            DataSourceView view = data.GetView("");
            if (view != null)
                view.Select(DataSourceSelectArguments.Empty, new DataSourceViewSelectCallback(RegisterDataAsp));
        }

        private string aspDataName;
        private Report report;

        private void RegisterDataAsp(IEnumerable data)
        {
            if (data != null)
                RegisterDataAsp(report, data, aspDataName);
        }

        /// <summary>
        /// Registers the ASP.NET application data to use it in the report.
        /// </summary>
        /// <param name="report">The <b>Report</b> object.</param>
        /// <param name="data">The application data.</param>
        /// <param name="name">The name of the data.</param>
        public void RegisterDataAsp(Report report, IEnumerable data, string name)
        {
            DataComponentBase existingDs = report.Dictionary.FindDataComponent(name);
            if (existingDs is ViewDataSource && data is DataView)
            {
                // compatibility with old FR versions (earlier than 1.1.45)
                report.Dictionary.RegisterDataView(data as DataView, name, true);
            }
            else
            {
                // in a new versions, always register the business object
                report.Dictionary.RegisterBusinessObject(data, name, 1, true);
            }
        }

        /// <inheritdoc/>
        protected override void OnLoad(EventArgs e)
        {
            this.EnsureChildControls();
            if (this.Context != null)
            {
                this.Context.Session[this.ID] = ReportGuid;
            }
            base.OnLoad(e);
        }

        /// <inheritdoc/>
        protected override void OnUnload(EventArgs e)
        {
            cache.CleanFileStorage();
            base.OnUnload(e);
        }

        private void InitTabs()
        {
            if (tabs == null)
                tabs = new List<ReportTab>();
            if (tabs.Count == 0)
            {
                ReportTab item = new ReportTab();
                item.Properties.ControlID = GetControlID();
                item.Properties.HandlerURL = WebUtils.GetBasePath(this.Context) + WebUtils.HandlerFileName;
                tabs.Add(item);
                currentReportIndex = 0;
            }
        }

        private string GetControlID()
        {
            return String.Concat(ReportGuid, DateTime.Now.Ticks.ToString());
        }

        private void InitReport()
        {
            if (!DesignMode)
                Config.WebMode = true;

            cache = new WebReportCache(Context);
            Prop.ControlID = GetControlID();

            if (!fPreviewMode)
            {
                // load report
                WebReport oldweb = (WebReport)cache.GetObject(ReportGuid, this);
                if (oldweb != null)
                {
                    Report = oldweb.Report;
                    WebReportProperties storedProp = oldweb.Prop;
                    if (storedProp != null)
                        Prop = storedProp;
                }
            }
            else
            {
                this.Width = Unit.Percentage(100);
                this.Height = Unit.Percentage(100);
                this.Toolbar.EnableFit = true;
            }

            if (ShowAfterPrepare)
                ControlVisible = false;

            cache.Delay = CacheDelay;
            cache.Priority = CachePriority;

            if (this.ID == null)
                this.ID = ReportGuid;
        }

        /// <inheritdoc/>
        protected override void OnInit(EventArgs e)
        {
            if (HttpContext.Current != null)
                InitReport();
            base.OnInit(e);
        }

        internal void Localize()
        {
            if (!String.IsNullOrEmpty(LocalizationFile))
            {
                string fileName = LocalizationFile;
                if (!WebUtils.IsAbsolutePhysicalPath(fileName))
                    fileName = this.Context.Request.MapPath(fileName, base.AppRelativeTemplateSourceDirectory, true);
                reportRes.LoadLocale(fileName);
            }
        }

        /// <inheritdoc/>
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            // remove width & height styles from html element
            Unit width = Width;
            Unit height = Height;

            if (!DesignReport)
            {
                Width = new Unit();
                Height = new Unit();
            }

            Style.Add("display", "inline-block");
            base.AddAttributesToRender(writer);

            if (!DesignReport)
            {
                Width = width;
                Height = height;
            }
        }

        /// <inheritdoc/>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (HttpContext.Current != null && !InlineRegistration && Page != null)
            {
                Toolbar.RegisterGlobals(Prop.ControlID, Page.ClientScript, this.GetType(), Prop.ExternalJquery);
                if (!Page.ClientScript.IsClientScriptIncludeRegistered("fr_util"))
                    this.InlineRegistration = true;
            }

            // check for design time
            if (HttpContext.Current == null)
                RenderDesignModeNavigatorControls(writer);
            else
            {  // Run-time
                Prop.HandlerURL = WebUtils.GetBasePath(this.Context) + WebUtils.HandlerFileName;
                if (Config.FullTrust)
                {
                    WebUtils.CheckHandlersRuntime();
                }

                StringBuilder container = new StringBuilder();

                if (Page == null || PreviewMode) // Razor
                {
                    InitReport();
                }

                if (ControlVisible)
                {
                    if (InlineRegistration)
                    {
                        container.AppendLine(Toolbar.GetInlineRegistration(Prop.ExternalJquery));
                    }

                    //Localize();

                    container.AppendLine(Toolbar.GetHtmlProgress(Prop.HandlerURL, Prop.ControlID, !DesignReport, this.Width, this.Height));

                    writer.WriteLine(container.ToString());
                    base.RenderContents(writer);

                    if (cache.WebFarmMode)
                    {
                        StringBuilder sb = new StringBuilder();
                        ReportProcess(sb, this.Context);
                    }

                    // save all objects

                    //                    cache.PutObject(ReportGuid, this);
                    cache.PutObject(Prop.ControlID, this);
                }
            }
        }

        #endregion Protected methods

        #region DotNet 4 specific

#if DOTNET_4

        /// <summary>
        /// Returns the HTML code of report preview control.
        /// </summary>
        /// <returns>HtmlString object.</returns>
        public HtmlString GetHtml()
        {
            StringBuilder sb = new StringBuilder();
            HtmlTextWriter writer = new HtmlTextWriter(new StringWriter(sb, System.Globalization.CultureInfo.InvariantCulture));
            this.RenderControl(writer);
            return new HtmlString(Toolbar.GetCss() + sb.ToString());
        }

        /// <summary>
        /// Returns the code "<script>...</script>" for registration the necessary js libraries.
        /// </summary>
        /// <returns>HtmlString object.</returns>
        public HtmlString Scripts()
        {
            return WebReportGlobals.Scripts();
        }

        /// <summary>
        /// Returns the code for registration the necessary CSS.
        /// </summary>
        /// <returns>HtmlString object.</returns>
        public HtmlString Styles()
        {
            return WebReportGlobals.Styles();
        }

#endif

        #endregion DotNet 4 specific

        /// <summary>
        /// Sets the size of WebReport object.
        /// </summary>
        /// <param name="width">Width of WebReport.</param>
        /// <param name="height">Height of WebReport.</param>
        /// <returns>WebReport object.</returns>
        public WebReport SetSize(Unit width, Unit height)
        {
            this.Width = width;
            this.Height = height;
            return this;
        }

        /// <summary>
        /// Sets the StartReport event handler.
        /// </summary>
        /// <param name="start">Event handler.</param>
        /// <returns>WebReport object.</returns>
        public WebReport SetStartEvent(EventHandler start)
        {
            StartReport += start;
            return this;
        }

        /// <summary>
        /// Sets the CustomDraw event handler.
        /// </summary>
        /// <param name="draw">Event handler.</param>
        /// <returns>WebReport object.</returns>
        public WebReport SetCustomDrawEvent(EventHandler<CustomDrawEventArgs> draw)
        {
            CustomDraw += draw;
            return this;
        }

        /// <summary>
        /// Sets the CustomAuth event handler.
        /// </summary>
        /// <param name="auth">Event handler.</param>
        /// <returns>WebReport object.</returns>
        public WebReport SetCustomAuthEvent(EventHandler<CustomAuthEventArgs> auth)
        {
            CustomAuth += auth;
            return this;
        }

        #region Register Data

        /// <summary>
        /// Register the DataSet in report dictionary.
        /// </summary>
        /// <param name="data">DataSet object.</param>
        /// <param name="name">Name for the registered data.</param>
        /// <returns>WebReport object.</returns>
        public WebReport RegisterData(DataSet data, string name)
        {
            this.Report.Dictionary.RegisterData(data, name, true);
            return this;
        }

        /// <summary>
        /// Registers the DataRelation in report dictionary.
        /// </summary>
        /// <param name="data">DataRelation object.</param>
        /// <param name="name">Name for the registered data.</param>
        /// <returns>WebReport object.</returns>
        public WebReport RegisterData(DataRelation data, string name)
        {
            this.Report.Dictionary.RegisterData(data, name, true);
            return this;
        }

        /// <summary>
        /// Register the DataSet in report dictionary.
        /// </summary>
        /// <param name="data">DataSet object.</param>
        /// <returns>WebReport object.</returns>
        public WebReport RegisterData(DataSet data)
        {
            this.Report.Dictionary.RegisterData(data, "Data", true);
            return this;
        }

        /// <summary>
        /// Registers the DataTable in report dictionary.
        /// </summary>
        /// <param name="data">DataTable object.</param>
        /// <param name="name">Name for the registered data.</param>
        /// <returns>WebReport object.</returns>
        public WebReport RegisterData(DataTable data, string name)
        {
            this.Report.Dictionary.RegisterData(data, name, true);
            return this;
        }

        /// <summary>
        /// Registers the DataView in report dictionary.
        /// </summary>
        /// <param name="data">DataView object.</param>
        /// <param name="name">Name for the registered data.</param>
        /// <returns>WebReport object.</returns>
        public WebReport RegisterData(DataView data, string name)
        {
            this.Report.Dictionary.RegisterData(data, name, true);
            return this;
        }

        /// <summary>
        /// Registers the IEnumerable in report dictionary.
        /// </summary>
        /// <param name="data">IEnumerable data.</param>
        /// <param name="name">Name for the registered data.</param>
        /// <returns>WebReport object.</returns>
        public WebReport RegisterData(IEnumerable data, string name)
        {
            this.Report.Dictionary.RegisterData(data, name, true);
            return this;
        }

        /// <summary>
        /// Loads the report from file.
        /// </summary>
        /// <param name="fileName">File Name.</param>
        /// <returns>WebReport object.</returns>
        public WebReport LoadFromFile(string fileName)
        {
            this.Report.Load(fileName);
            return this;
        }

        /// <summary>
        /// Loads the report from stream.
        /// </summary>
        /// <param name="stream">Stream object.</param>
        /// <returns>WebReport object.</returns>
        public WebReport LoadFromStream(Stream stream)
        {
            this.Report.Load(stream);
            return this;
        }

        /// <summary>
        /// Loads the prepared report from file. Also the property <see cref="WebReport.ReportDone"/> will be enabled and
        /// <see cref="WebReport.ShowRefreshButton"/> will be disabled.
        /// </summary>
        /// <param name="fileName">File Name.</param>
        /// <returns>WebReport object.</returns>
        public WebReport LoadPrepared(string fileName)
        {
            this.Report.LoadPrepared(fileName);
            this.ReportDone = true;
            this.ShowRefreshButton = false;
            return this;
        }

        /// <summary>
        /// Loads the prepared report from stream. Also the property <see cref="WebReport.ReportDone"/> will be enabled and
        /// <see cref="WebReport.ShowRefreshButton"/> will be disabled.
        /// </summary>
        /// <param name="stream">Stream object.</param>
        /// <returns>WebReport object.</returns>
        public WebReport LoadPrepared(Stream stream)
        {
            this.Report.LoadPrepared(stream);
            this.ReportDone = true;
            this.ShowRefreshButton = false;
            return this;
        }

        #endregion Register Data

        private void WebReportInit(bool inlineRegistration, bool stretched)
        {
            this.Width = Unit.Pixel(550);
            this.Height = Unit.Pixel(500);
            this.ForeColor = Color.Black;
            this.BackColor = Color.White;
            this.InlineRegistration = inlineRegistration;
            if (stretched)
            {
                this.Width = Unit.Percentage(100);
                this.Height = Unit.Percentage(100);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebReport"/> class.
        /// </summary>
        public WebReport()
        {
            WebReportInit(false, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebReport"/> class.
        /// </summary>
        /// <param name="inlineRegistration">Enable registration of JS/CSS footprint inline.</param>
        public WebReport(bool inlineRegistration)
        {
            WebReportInit(inlineRegistration, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebReport"/> class.
        /// </summary>
        /// <param name="inlineRegistration">Enable registration of JS/CSS footprint inline.</param>
        /// <param name="stretched">Creates the control with 100% width and height.</param>
        public WebReport(bool inlineRegistration, bool stretched)
        {
            WebReportInit(inlineRegistration, stretched);
        }

        static WebReport()
        {
            ScriptSecurity = new ScriptSecurity(new ScriptChecker());
        }

        #region Serialization

        /// <summary>
        /// Gets the stream with serialized properties data.
        /// </summary>
        /// <returns>Stream object.</returns>
        public Stream GetPropData()
        {
            XmlSerializer propSerializer = new XmlSerializer(typeof(WebReportProperties));
            MemoryStream dataStream = new MemoryStream();
            propSerializer.Serialize(dataStream, Prop);
            dataStream.Position = 0;
            return dataStream;
        }

        /// <summary>
        /// Gets the stream with serialized report data.
        /// </summary>
        /// <returns>Stream object.</returns>
        public Stream GetReportData()
        {
            MemoryStream dataStream = new MemoryStream();
            Report.Save(dataStream);
            dataStream.Position = 0;
            return dataStream;
        }

        /// <summary>
        /// Gets the stream with serialized prepared report data.
        /// </summary>
        /// <returns>Stream object.</returns>
        public Stream GetPreparedData()
        {
            MemoryStream dataStream = new MemoryStream();
            Report.SavePrepared(dataStream);
            dataStream.Position = 0;
            return dataStream;
        }

        #endregion Serialization

        #region Deserialization

        /// <summary>
        /// Loads the serialized properties data from stream.
        /// </summary>
        /// <param name="propStream">Stream object.</param>
        public void SetPropData(Stream propStream)
        {
            if (propStream != null && propStream.Length > 0)
            {
                XmlSerializer propSerializer = new XmlSerializer(typeof(WebReportProperties));
                Prop = propSerializer.Deserialize(propStream) as WebReportProperties;
            }
        }

        /// <summary>
        /// Loads the serialized report data from stream.
        /// </summary>
        /// <param name="reportStream">Stream object.</param>
        public void SetReportData(Stream reportStream)
        {
            if (reportStream != null && reportStream.Length > 0)
            {
                Report.Load(reportStream);
            }
        }

        /// <summary>
        /// Loads the serialized prepared report data from stream.
        /// </summary>
        /// <param name="reportStream">Stream object.</param>
        public void SetPreparedData(Stream reportStream)
        {
            if (reportStream != null && reportStream.Length > 0)
            {
                Report.LoadPrepared(reportStream);
            }
        }

        #endregion Deserialization
    }

    /// <summary>
    /// Represents the static class WebReportGlobals with necessary methods for using in MVC environment.
    /// </summary>
    public static class WebReportGlobals
    {
#if DOTNET_4

        /// <summary>
        /// Returns the code for registration the necessary scripts with jQuery.
        /// </summary>
        /// <returns>HtmlString object.</returns>
        public static HtmlString Scripts()
        {
            return new HtmlString(ScriptsAsString());
        }

        /// <summary>
        /// Returns the code for registration the necessary scripts without jQuery.
        /// </summary>
        /// <returns>HtmlString object.</returns>
        public static HtmlString ScriptsWOjQuery()
        {
            return new HtmlString(ScriptsWOjQueryAsString());
        }

        /// <summary>
        /// Returns the code for registration the necessary CSS.
        /// </summary>
        /// <returns>HtmlString object.</returns>
        public static HtmlString Styles()
        {
            return new HtmlString(StylesAsString());
        }

        /// <summary>
        /// Returns the code for registration the necessary CSS without jQuery styles.
        /// </summary>
        /// <returns>HtmlString object.</returns>
        public static HtmlString StylesWOjQuery()
        {
            // reserved for future
            return new HtmlString(String.Empty);
        }

#endif

        /// <summary>
        /// Returns the string with code for registration the necessary scripts with jQuery.
        /// </summary>
        /// <returns>String object.</returns>
        public static string ScriptsAsString()
        {
            StringBuilder raw = new StringBuilder();
            raw.Append("<script src=\"").Append(GetResourceTemplateUrl("fr_util.js")).AppendLine("\" type=\"text/javascript\"></script>");
            raw.Append("<script src=\"").Append(GetResourceJqueryUrl("jquery.min.js")).AppendLine("\" type=\"text/javascript\"></script>");
            raw.Append("<script src=\"").Append(GetResourceJqueryUrl("jquery-ui.custom.min.js")).AppendLine("\" type=\"text/javascript\"></script>");
            return raw.ToString();
        }

        /// <summary>
        ///Returns the string with code for registration the necessary scripts without jQuery.
        /// </summary>
        /// <returns>String object.</returns>
        public static string ScriptsWOjQueryAsString()
        {
            StringBuilder raw = new StringBuilder();
            raw.Append("<script src=\"").Append(GetResourceTemplateUrl("fr_util.js")).AppendLine("\" type=\"text/javascript\"></script>");
            return raw.ToString();
        }

        /// <summary>
        /// Returns the string with code for registration the necessary CSS with jQuery.
        /// </summary>
        /// <returns>String object.</returns>
        public static string StylesAsString()
        {
            StringBuilder raw = new StringBuilder();
            raw.Append("<link rel=\"stylesheet\" href=\"").Append(GetResourceJqueryUrl("jquery-ui.min.css")).AppendLine("\">");
            return raw.ToString();
        }

        /// <summary>
        /// Returns the string with code for registration the necessary CSS without jQuery.
        /// </summary>
        /// <returns>String object.</returns>
        public static string StylesWOjQueryAsString()
        {
            // reserved for future
            return String.Empty;
        }

        private static string GetResourceJqueryUrl(string resName)
        {
            return new Page().ClientScript.GetWebResourceUrl(typeof(WebReport), string.Format("FastReport.Web.Resources.jquery.{0}", resName));
        }

        private static string GetResourceTemplateUrl(string resName)
        {
            return new Page().ClientScript.GetWebResourceUrl(typeof(WebReport), string.Format("FastReport.Web.Resources.Templates.{0}", resName));
        }
    }
}