using FastReport.Export;
using FastReport.Export.OoXML;
using FastReport.Export.Pdf;
using FastReport.Export.RichText;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.UI.WebControls;
using System.Xml.Serialization;

namespace FastReport.Web
{
    /// <summary>
    /// Tab position enum.
    /// </summary>
    public enum TabPosition
    {
        /// <summary>
        /// Tabs inside toolbar.
        /// </summary>
        InsideToolbar,

        /// <summary>
        /// Tabs under toolbar.
        /// </summary>
        UnderToolbar,

        /// <summary>
        /// Tabs are hidden.
        /// </summary>
        Hidden
    }

    /// <summary>
    /// Event arguments for custom auth.
    /// </summary>
    public class CustomAuthEventArgs : EventArgs
    {
        private bool authPassed = true;
        private HttpContext context;

        /// <summary>
        /// Gets or sets the HttpContext value.
        /// </summary>
        public HttpContext Context
        {
            get { return context; }
            set { context = value; }
        }

        /// <summary>
        /// Gets or sets the auth passed value.
        /// </summary>
        public bool AuthPassed
        {
            get { return authPassed; }
            set { authPassed = value; }
        }
    }

    /// <summary>
    /// Event arguments for Save report from Designer.
    /// </summary>
    public class SaveDesignedReportEventArgs : EventArgs
    {
        private Stream stream;

        /// <summary>
        /// Contain the stream with designed report.
        /// </summary>
        public Stream Stream
        {
            get { return stream; }
            set { stream = value; }
        }
    }



    /// <summary>
    /// Zoom mode enum
    /// </summary>
    public enum ZoomMode
    {
        /// <summary>
        ///
        /// </summary>
        Scale,

        /// <summary>
        ///
        /// </summary>
        Width,

        /// <summary>
        ///
        /// </summary>
        Page
    }

    /// <summary>
    /// Report states enum.
    /// </summary>
    public enum ReportState
    {
        /// <summary>
        ///
        /// </summary>
        Empty,

        /// <summary>
        ///
        /// </summary>
        Forms,

        /// <summary>
        ///
        /// </summary>
        Report,

        /// <summary>
        ///
        /// </summary>
        Done,

        /// <summary>
        ///
        /// </summary>
        Canceled
    }

    /// <summary>
    /// WebReport Properies class.
    /// </summary>
    [Serializable]
    public class WebReportProperties
    {
        [XmlIgnore]
        private Unit fWidth;

        [XmlIgnore]
        private Unit fHeight;

        [XmlElement("Width")]
        internal string WidthString
        {
            get { return fWidth.ToString(); }
            set { fWidth = Unit.Parse(value); }
        }

        [XmlElement("Height")]
        internal string HeightString
        {
            get { return fHeight.ToString(); }
            set { fHeight = Unit.Parse(value); }
        }


        #region Private

        #region Layout

        private bool autoWidth = false;
        private bool autoHeight = false;
        private bool layers = true;
        private System.Windows.Forms.Padding fPadding = new System.Windows.Forms.Padding(0, 0, 0, 0);
        private int toolbarHeight = 50;
        private ToolbarStyle toolbarStyle = ToolbarStyle.Large;
        private ToolbarIconsStyle toolbarIconsStyle = ToolbarIconsStyle.Red;
        private ToolbarBackgroundStyle toolbarBackgroundStyle = ToolbarBackgroundStyle.Light;
        private bool externalJquery = false;
        private bool inlineRegistration = false;
        private bool enableMargins = true;

        #endregion Layout

        #region Designer

        private bool fDesignReport = false;
        private bool fDesignScriptCode = false;
        private string fDesignerPath = "~/WebReportDesigner/index.html";
        private string fDesignerSavePath = String.Empty;
        private string fDesignerSaveCallBack = String.Empty;
        private string fDesignerLocale = String.Empty;
        private string fDesignerConfig = String.Empty;

        #endregion Designer

        #region Report

        private string fControlID;
        private string fReportResourceString = String.Empty;
        private string fReportDataSources = String.Empty;
        private bool fPictures = true;
        private bool fSinglePage = false;
        private string fReportFile = String.Empty;
        private string fReportPath = String.Empty;
        private string fLocalizationFile = String.Empty;
        private float fZoom = 1f;
        private ZoomMode fZoomMode = ZoomMode.Scale;
        private int fCacheDelay = 60;
        private CacheItemPriority fCachePriority = CacheItemPriority.Normal;
        private string handlerUrl;
        private int fTotalPages;
        private bool fShowAfterPrepare = false;
        private bool fDebug = false;
        private bool fCloudEnvironmet = false;
        private bool fEmbedPictures = false;
        private int fRefreshTimeout = 0;
        private string fLogFile;
        private bool fUnlimitedWidth = false;
        private bool fUnlimitedHeight = false;
        private bool fDialogs = true;

        #endregion Report

        #region Toolbar

        private bool fShowToolbar = true;
        private bool fShowBottomToolbar = false;
        private string fButtonsPath;
        private bool fShowExports = true;
        private bool fShowPrint = true;
        private bool fShowFirstButton = true;
        private bool fShowPrevButton = true;
        private bool fShowNextButton = true;
        private bool fShowLastButton = true;
        private bool fShowZoomButton = true;
        private bool fShowRefreshButton = true;
        private bool fShowPageNumber = true;
        private bool fShowTabCloseButton = false;
        private bool fShowBackButton = false;
        private bool fShowOutline = false;
        private System.Drawing.Color fToolbarColor = Color.FromArgb(0xECE9D8);
        private TabPosition fTabPosition = TabPosition.InsideToolbar;

        #endregion Toolbar

        #region RTF

        private bool fShowRtfExport = true;
        private int fRtfJpegQuality = 90;
        private RTFImageFormat fRtfImageFormat = RTFImageFormat.Metafile;
        private bool fRtfPictures = true;
        private bool fRtfPageBreaks = true;
        private bool fRtfWysiwyg = true;
        private string fRtfCreator = WebUtils.DefaultCreator;
        private bool fRtfAutoSize = false;

        #endregion RTF

        #region MHT

        private bool fShowMhtExport = true;
        private bool fMhtPictures = true;
        private bool fMhtWysiwyg = true;

        #endregion MHT

        #region ODS

        private bool fShowOdsExport = true;
        private bool fOdsPageBreaks = true;
        private bool fOdsWysiwyg = true;
        private string fOdsCreator = WebUtils.DefaultCreator;

        #endregion ODS

        #region ODT

        private bool fShowOdtExport = true;
        private bool fOdtPageBreaks = true;
        private bool fOdtWysiwyg = true;
        private string fOdtCreator = WebUtils.DefaultCreator;

        #endregion ODT

        #region XPS

        private bool fShowXpsExport = true;

        #endregion XPS

        #region DBF

        private bool fShowDbfExport = true;

        #endregion DBF

        #region Word2007

        private bool fShowWord2007Export = true;
        private bool fDocxMatrixBased = true;
        private string fDocxRowHeightIs;
        private bool fDocxParagraphBased = false;
        private bool fDocxWysiwyg = true;
        private bool fDocxPrintOptimized = false;

        #endregion Word2007

        #region Excel2007 format

        private bool showExcel2007Export = true;
        private bool xlsxPageBreaks = false;
        private bool xlsxSeamless = false;
        private bool xlsxWysiwyg = true;
        private bool xlsxDataOnly = false;
        private bool xlsxPrintOptimized = false;
        private bool xlsxPrintFitPage = false;

        #endregion Excel2007 format

        #region PowerPoint2007 format

        private bool fShowPowerPoint2007Export = true;
        private PptImageFormat fPptxImageFormat = PptImageFormat.Png;

        #endregion PowerPoint2007 format

        #region XML format

        private bool fShowXmlExcelExport = true;
        private bool fXmlExcelPageBreaks = true;
        private bool fXmlExcelWysiwyg = true;
        private bool fXmlExcelDataOnly = false;

        #endregion XML format

        #region PDF format

        private bool showPdfExport = true;
        private bool pdfInteractiveForms = false;
        private bool pdfEmbeddingFonts = true;
        private bool pdfBackground = true;
        private bool pdfPrintOptimized = true;
        private bool pdfOutline = true;
        private bool pdfDisplayDocTitle = true;
        private bool pdfHideToolbar = false;
        private bool pdfHideMenubar = false;
        private bool pdfHideWindowUI = false;
        private bool pdfFitWindow = false;
        private bool pdfCenterWindow = false;
        private bool pdfPrintScaling = true;
        private string pdfTitle = String.Empty;
        private string pdfAuthor = String.Empty;
        private string pdfSubject = String.Empty;
        private string pdfKeywords = String.Empty;
        private string pdfCreator = WebUtils.DefaultCreator;
        private string pdfProducer = WebUtils.DefaultProducer;
        private string pdfUserPassword = String.Empty;
        private string pdfOwnerPassword = String.Empty;
        private bool pdfAllowPrint = true;
        private bool pdfAllowModify = true;
        private bool pdfAllowCopy = true;
        private bool pdfAllowAnnotate = true;
        private bool pdfA = false;
        private bool pdfShowPrintDialog = false;
        private bool pdfTextInCurves = false;

        private bool pdfImagesOriginalResolution = false;
        private bool pdfJpegCompression = false;
        private PDFExport.PdfColorSpace pdfColorSpace = PDFExport.PdfColorSpace.RGB;

        #endregion PDF format

        #region CSV format

        private bool fShowCsvExport = true;
        private string fCsvSeparator = ";";
        private bool fCsvDataOnly = false;

        #endregion CSV format

        #region Text format

        private bool fShowTextExport = true;
        private bool fTextDataOnly = false;
        private bool fTextPageBreaks = true;
        private bool fTextAllowFrames = true;
        private bool fTextSimpleFrames = true;
        private bool fTextEmptyLines = false;

        #endregion Text format

        #region FPX (prepared) format

        private bool fShowPreparedReport = true;

        #endregion FPX (prepared) format

        #region Print

        private bool fPrintInPdf = true;
        private bool fPrintInBrowser = true;
        private string fPrintWindowWidth = "700px";
        private string fPrintWindowHeight = "500px";

        #endregion Print

        #region Hidden Properties

        private int fCurrentPage = 0;
        private int fCurrentForm = 0;
        private ReportState fReportState = ReportState.Empty;

        //private bool fHTMLDone = false;
        private float fCurrentWidth = 0;

        private float fCurrentHeight = 0;
        private bool fControlVisible = true;
        private int fPreviousTab = 0;
        private WebHeaderCollection fRequestHeaders = new WebHeaderCollection();
        private WebHeaderCollection fResponseHeaders = new WebHeaderCollection();

        #endregion Hidden Properties

        #region Extra

        /// <summary>
        /// "mm/dd/yy"
        /// </summary>
        internal const string DEFAULT_DATE_TIME_PICKER_FORMAT = "mm/dd/yy";

        private string dateTimePickerFormat = DEFAULT_DATE_TIME_PICKER_FORMAT;

        #endregion Extra

        #endregion Private

        #region Public properties

        #region Layout

        /// <summary>
        ///
        /// </summary>
        public ToolbarStyle ToolbarStyle
        {
            get { return toolbarStyle; }
            set
            {
                toolbarStyle = value;
                if (value == ToolbarStyle.Large)
                    toolbarHeight = 50;
                else
                    toolbarHeight = 40;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public ToolbarIconsStyle ToolbarIconsStyle
        {
            get { return toolbarIconsStyle; }
            set { toolbarIconsStyle = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public ToolbarBackgroundStyle ToolbarBackgroundStyle
        {
            get { return toolbarBackgroundStyle; }
            set { toolbarBackgroundStyle = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool AutoWidth
        {
            get { return autoWidth; }
            set { autoWidth = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool AutoHeight
        {
            get { return autoHeight; }
            set { autoHeight = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool Layers
        {
            get { return layers; }
            set { layers = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public System.Windows.Forms.Padding Padding
        {
            get { return fPadding; }
            set { fPadding = value; }
        }

        #endregion Layout

        #region Designer

        /// <summary>
        ///
        /// </summary>
        public bool DesignReport
        {
            get { return fDesignReport; }
            set { fDesignReport = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool DesignScriptCode
        {
            get { return fDesignScriptCode; }
            set { fDesignScriptCode = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string DesignerPath
        {
            get { return fDesignerPath; }
            set { fDesignerPath = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string DesignerSavePath
        {
            get { return fDesignerSavePath; }
            set { fDesignerSavePath = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string DesignerSaveCallBack
        {
            get { return fDesignerSaveCallBack; }
            set { fDesignerSaveCallBack = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string DesignerLocale
        {
            get { return fDesignerLocale; }
            set { fDesignerLocale = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string DesignerConfig
        {
            get { return fDesignerConfig; }
            set { fDesignerConfig = value; }
        }

        #endregion Designer

        #region Report

        /// <summary>
        ///
        /// </summary>
        public string ReportResourceString
        {
            get { return fReportResourceString; }
            set { fReportResourceString = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string ReportDataSources
        {
            get { return fReportDataSources; }
            set { fReportDataSources = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool Pictures
        {
            get { return fPictures; }
            set { fPictures = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool SinglePage
        {
            get { return fSinglePage; }
            set { fSinglePage = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool InlineRegistration
        {
            get { return inlineRegistration; }
            set { inlineRegistration = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool EnableMargins
        {
            get { return enableMargins; }
            set { enableMargins = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ExternalJquery
        {
            get { return externalJquery; }
            set { externalJquery = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string ReportFile
        {
            get { return fReportFile; }
            set { fReportFile = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string ReportPath
        {
            get { return fReportPath; }
            set { fReportPath = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string LocalizationFile
        {
            get { return fLocalizationFile; }
            set { fLocalizationFile = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public float Zoom
        {
            get { return fZoom; }
            set
            {
                fZoom = value;
                fZoomMode = ZoomMode.Scale;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public ZoomMode ZoomMode
        {
            get { return fZoomMode; }
            set { fZoomMode = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public int RefreshTimeout
        {
            get { return fRefreshTimeout; }
            set { fRefreshTimeout = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public int CacheDelay
        {
            get { return fCacheDelay; }
            set { fCacheDelay = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public CacheItemPriority CachePriority
        {
            get { return fCachePriority; }
            set { fCachePriority = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool CloudEnvironmet
        {
            get { return fCloudEnvironmet; }
            set { fCloudEnvironmet = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool EmbedPictures
        {
            get { return fEmbedPictures; }
            set { fEmbedPictures = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ShowAfterPrepare
        {
            get { return fShowAfterPrepare; }
            set { fShowAfterPrepare = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool Debug
        {
            get { return fDebug; }
            set { fDebug = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool UnlimitedWidth
        {
            get { return fUnlimitedWidth; }
            set { fUnlimitedWidth = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool UnlimitedHeight
        {
            get { return fUnlimitedHeight; }
            set { fUnlimitedHeight = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool Dialogs
        {
            get { return fDialogs; }
            set { fDialogs = value; }
        }

        #endregion Report

        #region Toolbar

        /// <summary>
        ///
        /// </summary>
        public bool ShowToolbar
        {
            get { return fShowToolbar; }
            set { fShowToolbar = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ShowBottomToolbar
        {
            get { return fShowBottomToolbar; }
            set { fShowBottomToolbar = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string ButtonsPath
        {
            get { return fButtonsPath; }
            set { fButtonsPath = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ShowExports
        {
            get { return fShowExports; }
            set { fShowExports = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ShowPrint
        {
            get { return fShowPrint; }
            set { fShowPrint = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ShowFirstButton
        {
            get { return fShowFirstButton; }
            set { fShowFirstButton = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ShowPrevButton
        {
            get { return fShowPrevButton; }
            set { fShowPrevButton = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ShowNextButton
        {
            get { return fShowNextButton; }
            set { fShowNextButton = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ShowLastButton
        {
            get { return fShowLastButton; }
            set { fShowLastButton = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ShowZoomButton
        {
            get { return fShowZoomButton; }
            set { fShowZoomButton = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ShowRefreshButton
        {
            get { return fShowRefreshButton; }
            set { fShowRefreshButton = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ShowPageNumber
        {
            get { return fShowPageNumber; }
            set { fShowPageNumber = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ShowTabCloseButton
        {
            get { return fShowTabCloseButton; }
            set { fShowTabCloseButton = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ShowBackButton
        {
            get { return fShowBackButton; }
            set { fShowBackButton = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public TabPosition TabPosition
        {
            get { return fTabPosition; }
            set { fTabPosition = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public System.Drawing.Color ToolbarColor
        {
            get { return fToolbarColor; }
            set { fToolbarColor = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ShowOutline
        {
            get { return fShowOutline; }
            set { fShowOutline = value; }
        }

        #endregion Toolbar

        #region RTF

        /// <summary>
        ///
        /// </summary>
        public bool ShowRtfExport
        {
            get { return fShowRtfExport; }
            set { fShowRtfExport = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public int RtfJpegQuality
        {
            get { return fRtfJpegQuality; }
            set { fRtfJpegQuality = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public RTFImageFormat RtfImageFormat
        {
            get { return fRtfImageFormat; }
            set { fRtfImageFormat = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool RtfPictures
        {
            get { return fRtfPictures; }
            set { fRtfPictures = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool RtfPageBreaks
        {
            get { return fRtfPageBreaks; }
            set { fRtfPageBreaks = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool RtfWysiwyg
        {
            get { return fRtfWysiwyg; }
            set { fRtfWysiwyg = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string RtfCreator
        {
            get { return fRtfCreator; }
            set { fRtfCreator = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool RtfAutoSize
        {
            get { return fRtfAutoSize; }
            set { fRtfAutoSize = value; }
        }

        #endregion RTF

        #region MHT

        /// <summary>
        ///
        /// </summary>
        public bool ShowMhtExport
        {
            get { return fShowMhtExport; }
            set { fShowMhtExport = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool MhtPictures
        {
            get { return fMhtPictures; }
            set { fMhtPictures = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool MhtWysiwyg
        {
            get { return fMhtWysiwyg; }
            set { fMhtWysiwyg = value; }
        }

        #endregion MHT

        #region ODS

        /// <summary>
        ///
        /// </summary>
        public bool ShowOdsExport
        {
            get { return fShowOdsExport; }
            set { fShowOdsExport = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool OdsPageBreaks
        {
            get { return fOdsPageBreaks; }
            set { fOdsPageBreaks = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool OdsWysiwyg
        {
            get { return fOdsWysiwyg; }
            set { fOdsWysiwyg = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string OdsCreator
        {
            get { return fOdsCreator; }
            set { fOdsCreator = value; }
        }

        #endregion ODS

        #region ODT

        /// <summary>
        ///
        /// </summary>
        public bool ShowOdtExport
        {
            get { return fShowOdtExport; }
            set { fShowOdtExport = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool OdtPageBreaks
        {
            get { return fOdtPageBreaks; }
            set { fOdtPageBreaks = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool OdtWysiwyg
        {
            get { return fOdtWysiwyg; }
            set { fOdtWysiwyg = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string OdtCreator
        {
            get { return fOdtCreator; }
            set { fOdtCreator = value; }
        }

        #endregion ODT

        #region XPS format

        /// <summary>
        ///
        /// </summary>
        public bool ShowXpsExport
        {
            get { return fShowXpsExport; }
            set { fShowXpsExport = value; }
        }

        #endregion XPS format

        #region DBF format

        /// <summary>
        ///
        /// </summary>
        public bool ShowDbfExport
        {
            get { return fShowDbfExport; }
            set { fShowDbfExport = value; }
        }

        #endregion DBF format

        #region Word2007 format

        /// <summary>
        ///
        /// </summary>
        public bool ShowWord2007Export
        {
            get { return fShowWord2007Export; }
            set { fShowWord2007Export = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool DocxParagraphBased
        {
            get { return fDocxParagraphBased; }
            set { fDocxParagraphBased = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool DocxMatrixBased
        {
            get { return fDocxMatrixBased; }
            set { fDocxMatrixBased = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string DocxRowHeightIs
        {
            get { return fDocxRowHeightIs; }
            set { fDocxRowHeightIs = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool DocxWysiwyg
        {
            get { return fDocxWysiwyg; }
            set { fDocxWysiwyg = true; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool DocxPrintOptimized
        {
            get { return fDocxPrintOptimized; }
            set { fDocxPrintOptimized = value; }
        }

        #endregion Word2007 format

        #region Excel2007 format

        /// <summary>
        ///
        /// </summary>
        public bool ShowExcel2007Export
        {
            get { return showExcel2007Export; }
            set { showExcel2007Export = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool XlsxPrintFitPage
        {
            get { return xlsxPrintFitPage; }
            set { xlsxPrintFitPage = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool XlsxPageBreaks
        {
            get { return xlsxPageBreaks; }
            set { xlsxPageBreaks = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool XlsxSeamless
        {
            get { return xlsxSeamless; }
            set { xlsxSeamless = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool XlsxWysiwyg
        {
            get { return xlsxWysiwyg; }
            set { xlsxWysiwyg = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool XlsxDataOnly
        {
            get { return xlsxDataOnly; }
            set { xlsxDataOnly = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool XlsxPrintOptimized
        {
            get { return xlsxPrintOptimized; }
            set { xlsxPrintOptimized = value; }
        }

        #endregion Excel2007 format

        #region PowerPoint2007 format

        /// <summary>
        ///
        /// </summary>
        public bool ShowPowerPoint2007Export
        {
            get { return fShowPowerPoint2007Export; }
            set { fShowPowerPoint2007Export = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public PptImageFormat PptxImageFormat
        {
            get { return fPptxImageFormat; }
            set { fPptxImageFormat = value; }
        }

        #endregion PowerPoint2007 format

        #region XML format

        /// <summary>
        ///
        /// </summary>
        public bool ShowXmlExcelExport
        {
            get { return fShowXmlExcelExport; }
            set { fShowXmlExcelExport = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool XmlExcelPageBreaks
        {
            get { return fXmlExcelPageBreaks; }
            set { fXmlExcelPageBreaks = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool XmlExcelWysiwyg
        {
            get { return fXmlExcelWysiwyg; }
            set { fXmlExcelWysiwyg = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool XmlExcelDataOnly
        {
            get { return fXmlExcelDataOnly; }
            set { fXmlExcelDataOnly = value; }
        }

        #endregion XML format

        #region PDF format

        /// <summary>
        ///
        /// </summary>
        public bool ShowPdfExport
        {
            get { return showPdfExport; }
            set { showPdfExport = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfInteractiveForms
        {
            get { return pdfInteractiveForms; }
            set { pdfInteractiveForms = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfEmbeddingFonts
        {
            get { return pdfEmbeddingFonts; }
            set { pdfEmbeddingFonts = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfTextInCurves
        {
            get { return pdfTextInCurves; }
            set { pdfTextInCurves = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfBackground
        {
            get { return pdfBackground; }
            set { pdfBackground = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfPrintOptimized
        {
            get { return pdfPrintOptimized; }
            set { pdfPrintOptimized = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfOutline
        {
            get { return pdfOutline; }
            set { pdfOutline = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfDisplayDocTitle
        {
            get { return pdfDisplayDocTitle; }
            set { pdfDisplayDocTitle = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfHideToolbar
        {
            get { return pdfHideToolbar; }
            set { pdfHideToolbar = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfHideMenubar
        {
            get { return pdfHideMenubar; }
            set { pdfHideMenubar = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfHideWindowUI
        {
            get { return pdfHideWindowUI; }
            set { pdfHideWindowUI = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfFitWindow
        {
            get { return pdfFitWindow; }
            set { pdfFitWindow = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfCenterWindow
        {
            get { return pdfCenterWindow; }
            set { pdfCenterWindow = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfPrintScaling
        {
            get { return pdfPrintScaling; }
            set { pdfPrintScaling = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string PdfTitle
        {
            get { return pdfTitle; }
            set { pdfTitle = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string PdfAuthor
        {
            get { return pdfAuthor; }
            set { pdfAuthor = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string PdfSubject
        {
            get { return pdfSubject; }
            set { pdfSubject = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string PdfKeywords
        {
            get { return pdfKeywords; }
            set { pdfKeywords = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string PdfCreator
        {
            get { return pdfCreator; }
            set { pdfCreator = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string PdfProducer
        {
            get { return pdfProducer; }
            set { pdfProducer = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string PdfUserPassword
        {
            get { return pdfUserPassword; }
            set { pdfUserPassword = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string PdfOwnerPassword
        {
            get { return pdfOwnerPassword; }
            set { pdfOwnerPassword = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfAllowPrint
        {
            get { return pdfAllowPrint; }
            set { pdfAllowPrint = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfAllowModify
        {
            get { return pdfAllowModify; }
            set { pdfAllowModify = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfAllowCopy
        {
            get { return pdfAllowCopy; }
            set { pdfAllowCopy = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfAllowAnnotate
        {
            get { return pdfAllowAnnotate; }
            set { pdfAllowAnnotate = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfA
        {
            get { return pdfA; }
            set { pdfA = value; }
        }

        /// <summary>
        /// Enable or disable showing of Print Dialog
        /// </summary>
        public bool PdfShowPrintDialog
        {
            get { return pdfShowPrintDialog; }
            set { pdfShowPrintDialog = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfImagesOriginalResolution
        {
            get { return pdfImagesOriginalResolution; }
            set { pdfImagesOriginalResolution = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PdfJpegCompression
        {
            get { return pdfJpegCompression; }
            set { pdfJpegCompression = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public PDFExport.PdfColorSpace PdfColorSpace
        {
            get { return pdfColorSpace; }
            set { pdfColorSpace = value; }
        }

        #endregion PDF format

        #region CSV format

        /// <summary>
        ///
        /// </summary>
        public bool ShowCsvExport
        {
            get { return fShowCsvExport; }
            set { fShowCsvExport = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string CsvSeparator
        {
            get { return fCsvSeparator; }
            set { fCsvSeparator = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool CsvDataOnly
        {
            get { return fCsvDataOnly; }
            set { fCsvDataOnly = value; }
        }

        #endregion CSV format

        #region Prepared format

        /// <summary>
        ///
        /// </summary>
        public bool ShowPreparedReport
        {
            get { return fShowPreparedReport; }
            set { fShowPreparedReport = value; }
        }

        #endregion Prepared format

        #region Text format

        /// <summary>
        ///
        /// </summary>
        public bool ShowTextExport
        {
            get { return fShowTextExport; }
            set { fShowTextExport = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool TextDataOnly
        {
            get { return fTextDataOnly; }
            set { fTextDataOnly = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool TextPageBreaks
        {
            get { return fTextPageBreaks; }
            set { fTextPageBreaks = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool TextAllowFrames
        {
            get { return fTextAllowFrames; }
            set { fTextAllowFrames = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool TextSimpleFrames
        {
            get { return fTextSimpleFrames; }
            set { fTextSimpleFrames = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool TextEmptyLines
        {
            get { return fTextEmptyLines; }
            set { fTextEmptyLines = value; }
        }

        #endregion Text format

        #region Print

        /// <summary>
        ///
        /// </summary>
        public bool PrintInPdf
        {
            get { return fPrintInPdf; }
            set { fPrintInPdf = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool PrintInBrowser
        {
            get { return fPrintInBrowser; }
            set { fPrintInBrowser = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string PrintWindowWidth
        {
            get { return fPrintWindowWidth; }
            set { fPrintWindowWidth = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string PrintWindowHeight
        {
            get { return fPrintWindowHeight; }
            set { fPrintWindowHeight = value; }
        }

        #endregion Print

        #region Hidden Properties

        /// <summary>
        ///
        /// </summary>
        public bool ControlVisible
        {
            get { return fControlVisible; }
            set { fControlVisible = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool ReportDone
        {
            get { return State == ReportState.Done; }
            set { State = value ? ReportState.Done : ReportState.Empty; }
        }

        /// <summary>
        ///
        /// </summary>
        public int TotalPages
        {
            get { return fTotalPages; }
            set { fTotalPages = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string LogFile
        {
            get { return fLogFile; }
            set { fLogFile = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public WebHeaderCollection RequestHeaders
        {
            get { return fRequestHeaders; }
            set { fRequestHeaders = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public WebHeaderCollection ResponseHeaders
        {
            get { return fResponseHeaders; }
            set { fResponseHeaders = value; }
        }

        internal ReportState State
        {
            get { return fReportState; }
            set { fReportState = value; }
        }

        internal int CurrentForm
        {
            get { return fCurrentForm; }
            set { fCurrentForm = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public int CurrentPage
        {
            get { return fCurrentPage; }
            set { fCurrentPage = value; }
        }

        /// <summary>
        ///
        /// </summary>
        internal float CurrentWidth
        {
            get { return fCurrentWidth; }
            set { fCurrentWidth = value; }
        }

        /// <summary>
        ///
        /// </summary>
        internal float CurrentHeight
        {
            get { return fCurrentHeight; }
            set { fCurrentHeight = value; }
        }

        /// <summary>
        ///
        /// </summary>
        internal int ToolbarHeight
        {
            get { return toolbarHeight; }
        }

        /// <summary>
        ///
        /// </summary>
        internal string ControlID
        {
            get { return fControlID; }
            set { fControlID = value; }
        }

        /// <summary>
        ///
        /// </summary>
        internal string HandlerURL
        {
            get { return handlerUrl; }
            set { handlerUrl = value; }
        }

        internal int PreviousTab
        {
            get { return fPreviousTab; }
            set { fPreviousTab = value; }
        }

        #endregion Hidden Properties

        #region Extra

        /// <summary>
        ///
        /// </summary>
        public string DateTimePickerFormat
        {
            get { return dateTimePickerFormat; }
            set { dateTimePickerFormat = value; }
        }

        #endregion Extra

        #endregion Public properties

        /// <summary>
        /// Assigns another object values.
        /// </summary>
        /// <param name="properties"></param>
        public void Assign(WebReportProperties properties)
        {
            TabPosition = properties.TabPosition;
            ShowTabCloseButton = properties.ShowTabCloseButton;
            ToolbarStyle = properties.ToolbarStyle;
            ShowBottomToolbar = properties.ShowBottomToolbar;
            ShowToolbar = properties.ShowToolbar;
            ToolbarBackgroundStyle = properties.ToolbarBackgroundStyle;
            ToolbarColor = properties.ToolbarColor;
            ToolbarIconsStyle = properties.ToolbarIconsStyle;
            ToolbarStyle = properties.ToolbarStyle;
            ButtonsPath = properties.ButtonsPath;

            SinglePage = properties.SinglePage;
            LocalizationFile = properties.LocalizationFile;
            Zoom = properties.Zoom;
            ZoomMode = properties.ZoomMode;
            CacheDelay = properties.CacheDelay;
            CloudEnvironmet = properties.CloudEnvironmet;
            EmbedPictures = properties.EmbedPictures;

            ShowCsvExport = properties.ShowCsvExport;
            ShowDbfExport = properties.ShowDbfExport;
            ShowExcel2007Export = properties.ShowExcel2007Export;
            ShowExports = properties.ShowExports;
            ShowFirstButton = properties.ShowFirstButton;
            ShowLastButton = properties.ShowLastButton;
            ShowMhtExport = properties.ShowMhtExport;
            ShowNextButton = properties.ShowNextButton;
            ShowOdsExport = properties.ShowOdsExport;
            ShowOdtExport = properties.ShowOdtExport;
            ShowPageNumber = properties.ShowPageNumber;
            ShowPdfExport = properties.ShowPdfExport;
            ShowPowerPoint2007Export = properties.ShowPowerPoint2007Export;
            ShowPreparedReport = properties.ShowPreparedReport;
            ShowPrevButton = properties.ShowPrevButton;
            ShowPrint = properties.ShowPrint;
            ShowRefreshButton = properties.ShowRefreshButton;
            ShowRtfExport = properties.ShowRtfExport;
            ShowTextExport = properties.ShowTextExport;
            ShowWord2007Export = properties.ShowWord2007Export;
            ShowXmlExcelExport = properties.ShowXmlExcelExport;
            ShowXpsExport = properties.ShowXpsExport;
            ShowZoomButton = properties.ShowZoomButton;
            ShowBackButton = properties.ShowBackButton;
            PrintInBrowser = properties.PrintInBrowser;
            PrintInPdf = properties.PrintInPdf;

            ReportPath = properties.ReportPath;
            LogFile = properties.LogFile;
            EnableMargins = properties.EnableMargins;

            UnlimitedHeight = properties.UnlimitedHeight;
            UnlimitedWidth = properties.UnlimitedWidth;

            fDialogs = properties.Dialogs;
            fRequestHeaders = properties.RequestHeaders;
            fResponseHeaders = properties.ResponseHeaders;

            fDesignerConfig = properties.DesignerConfig;

            fDocxMatrixBased = properties.DocxMatrixBased;
            fDocxParagraphBased = properties.DocxParagraphBased;
            fDocxRowHeightIs = properties.DocxRowHeightIs;
            fDocxWysiwyg = properties.DocxWysiwyg;

            xlsxDataOnly = properties.XlsxDataOnly;
            xlsxPageBreaks = properties.XlsxPageBreaks;
            xlsxSeamless = properties.XlsxSeamless;
            xlsxWysiwyg = properties.XlsxWysiwyg;

            dateTimePickerFormat = properties.DateTimePickerFormat;

            HeightString = properties.HeightString;
            WidthString = properties.WidthString;

        }
    }
}