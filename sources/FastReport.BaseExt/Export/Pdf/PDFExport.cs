using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using FastReport.Export.TTF;
using FastReport.Table;
using FastReport.Utils;
using System;
using System.Windows.Forms;

namespace FastReport.Export.Pdf
{
    /// <summary>
    /// PDF export (Adobe Acrobat)
    /// </summary>
    public partial class PDFExport : ExportBase
    {
        #region Private Constants

        const float PDF_DIVIDER = 0.75f;
        const float PDF_PAGE_DIVIDER = 2.8346400000000003f; // mm to point
        const int PDF_PRINTOPT = 3;
        readonly float pDF_TTF_DIVIDER = 1 / (750 * 96f / DrawUtils.ScreenDpi);

        #endregion

        /// <summary>
        /// Embedded File
        /// </summary>
        public class EmbeddedFile
        {
            private string name;
            private string description;
            private DateTime modDate;
            private EmbeddedRelation relation;
            private string mime;
            private Stream fileStream;
            private long xref;
            private ZUGFeRD_ConformanceLevel zUGFeRD_ConformanceLevel;

            /// <summary>
            /// Name of embedded file.
            /// </summary>
            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            /// <summary>
            /// Description of embedded file.
            /// </summary>
            public string Description
            {
                get { return description; }
                set { description = value; }
            }

            /// <summary>
            /// Modify Date of embedded file.
            /// </summary>
            public DateTime ModDate
            {
                get { return modDate; }
                set { modDate = value; }
            }

            /// <summary>
            /// Relationship between the embedded document and the PDF part.
            /// </summary>
            public EmbeddedRelation Relation
            {
                get { return relation; }
                set { relation = value; }
            }

            /// <summary>
            /// Valid MIME type. 
            /// </summary>
            public string MIME
            {
                get { return mime; }
                set { mime = value; }
            }

            /// <summary>
            /// Stream of embedded file.
            /// </summary>
            public Stream FileStream
            {
                get { return fileStream; }
                set { fileStream = value; }
            }

            /// <summary>
            /// File reference.
            /// </summary>
            public long Xref
            {
                get { return xref; }
                set { xref = value; }
            }

            /// <summary>
            /// ZUGFeRD Conformance Level.
            /// </summary>
            public ZUGFeRD_ConformanceLevel ZUGFeRDConformanceLevel
            {
                get { return zUGFeRD_ConformanceLevel; }
                set { zUGFeRD_ConformanceLevel = value; }
            }

            /// Initializes a new instance of the <see cref="EmbeddedFile"/> class.
            public EmbeddedFile()
            {
                modDate = SystemFake.DateTime.Now;
                relation = EmbeddedRelation.Alternative;
                zUGFeRD_ConformanceLevel = ZUGFeRD_ConformanceLevel.BASIC;
                mime = "text/xml";
                fileStream = null;
            }
        }

        #region Public Enums

        /// <summary>
        /// Default preview size.
        /// </summary>
        public enum MagnificationFactor
        {
            /// <summary>
            /// Actual size
            /// </summary>
            ActualSize = 0,

            /// <summary>
            /// Fit Page
            /// </summary>
            FitPage = 1,

            /// <summary>
            /// Fit Width
            /// </summary>
            FitWidth = 2,

            /// <summary>
            /// Default
            /// </summary>
            Default = 3,

            /// <summary>
            /// 10%
            /// </summary>
            Percent_10 = 4,

            /// <summary>
            /// 25%
            /// </summary>
            Percent_25 = 5,

            /// <summary>
            /// 50%
            /// </summary>
            Percent_50 = 6,

            /// <summary>
            /// 75%
            /// </summary>
            Percent_75 = 7,

            /// <summary>
            /// 100%
            /// </summary>
            Percent_100 = 8,

            /// <summary>
            /// 125%
            /// </summary>
            Percent_125 = 9,

            /// <summary>
            /// 150%
            /// </summary>
            Percent_150 = 10,

            /// <summary>
            /// 200%
            /// </summary>
            Percent_200 = 11,

            /// <summary>
            /// 400%
            /// </summary>
            Percent_400 = 12,

            /// <summary>
            /// 800%
            /// </summary>
            Percent_800 = 13,
        }

        /// <summary>
        /// Standard of PDF format.
        /// </summary>
        public enum PdfStandard
        {
            /// <summary>
            /// PDF 1.5
            /// </summary>
            None = 0,

            /// <summary>
            /// PDF/A-1a
            /// </summary>
            PdfA_1a = 1,

            /// <summary>
            /// PDF/A-2a
            /// </summary>
            PdfA_2a = 2,

            /// <summary>
            /// PDF/A-2b
            /// </summary>
            PdfA_2b = 3,

            /// <summary>
            /// PDF/A-2u
            /// </summary>
            PdfA_2u = 4,

            /// <summary>
            /// PDF/A-3a
            /// </summary>
            PdfA_3a = 5,

            /// <summary>
            /// PDF/A-3b
            /// </summary>
            PdfA_3b = 6,

            /// <summary>
            /// Pdf/X-3
            /// </summary>
            PdfX_3 = 7,

            /// <summary>
            /// Pdf/X-4
            /// </summary>
            PdfX_4 = 8
        }

        /// <summary>
        /// Color Space.
        /// </summary>
        public enum PdfColorSpace
        {
            /// <summary>
            /// RGB color space
            /// </summary>
            RGB = 0,

            /// <summary>
            /// CMYK color space
            /// </summary>
            CMYK = 1,
        }

        /// <summary>
        /// Types of pdf export.
        /// </summary>
        public enum ExportType
        {
            /// <summary>
            /// Simple export
            /// </summary>
            Export,
            /// <summary>
            /// Web print mode
            /// </summary>
            WebPrint
        }

        /// <summary>
        /// Relationship between the embedded document and the PDF part.
        /// </summary>
        public enum EmbeddedRelation
        {
            /// <summary>
            /// The embedded file contains data which is used for the visual representation.
            /// </summary>
            Data,
            /// <summary>
            /// The embedded file contains the source data for the visual representation derived therefrom in the PDF part.
            /// </summary>
            Source,
            /// <summary>
            /// This data relationship should be used if the embedded data are an alternative representation of the PDF contents.
            /// </summary>
            Alternative,
            /// <summary>
            /// This data relationship is used if the embedded file serves neither as the source nor as the alternative representation, but the file contains additional information.
            /// </summary>
            Supplement,
            /// <summary>
            /// If none of the data relationships above apply or there is an unknown data relationship, this data relationship is used.
            /// </summary>
            Unspecified
        }

        /// <summary>
        /// ZUGFeRD Conformance Level.
        /// </summary>
        public enum ZUGFeRD_ConformanceLevel
        {
            /// <summary>
            /// Basic level.
            /// </summary>
            BASIC,
            /// <summary>
            /// Comfort level.
            /// </summary>
            COMFORT,
            /// <summary>
            /// Extended level.
            /// </summary>
            EXTENDED
        }
        #endregion

        #region Private Fields

        // Options
        private PdfStandard pdfCompliance = PdfStandard.None;
        private bool embeddingFonts = true;
        private bool background = true;
        private bool textInCurves = false;
        private PdfColorSpace colorSpace = PdfColorSpace.RGB;
        private bool imagesOriginalResolution = false;
        private bool printOptimized = true;
        private bool jpegCompression = false;
        private int jpegQuality = 95;
        private bool interactiveForms = false;
        private string interactiveFormsFontSetPattern = string.Empty;
        // end

        // Document Information
        private string title = "";
        private string author = "";
        private string subject = "";
        private string keywords = "";
        private string creator = "FastReport";
        private string producer = "FastReport.NET";
        // end

        // Security
        private string ownerPassword = "";
        private string userPassword = "";
        private bool allowPrint = true;
        private bool allowModify = true;
        private bool allowCopy = true;
        private bool allowAnnotate = true;
        // end

        // Viewer
        private bool showPrintDialog = false;
        private bool hideToolbar = false;
        private bool hideMenubar = false;
        private bool hideWindowUI = false;
        private bool fitWindow = false;
        private bool centerWindow = true;
        private bool printScaling = false;
        private bool outline = true;
        private MagnificationFactor defaultZoom = MagnificationFactor.ActualSize;
        // end

        // Other options
        private bool displayDocTitle = true;
        private bool encrypted = false;
        private bool compressed = true;
        private bool transparentImages = true;
        private int richTextQuality = 95;
        private int defaultPage = 1;
        private float dpiFX = 96f / DrawUtils.ScreenDpi;
        private bool buffered = false;
        private byte[] colorProfile = null;
        private List<EmbeddedFile> embeddedFiles;
        // end

        // Internal fields
        private string fileID;
        private long rootNumber;
        private long pagesNumber;
        private long outlineNumber;
        private long infoNumber;
        private long startXRef;
        private long actionDict;
        private long printDict;
        private List<long> xRef;
        private List<long> pagesRef;
        private List<string> trasparentStroke;
        private List<string> trasparentFill;
        private List<float> pagesHeights;
        private List<float> pagesTopMargins;
        private float marginLeft;
        private float marginWoBottom;
        private Stream pdf;
        private NumberFormatInfo numberFormatInfo;
        private float paperWidth;
        private float paperHeight;
        private long metaFileId;
        private long structId;
        private long colorProfileId;
        private long attachmentsNamesId;
        private long attachmentsListId;
        private IGraphics graphics;
        private StringBuilder contentBuilder;
        private long contentsPos;
        private ExportType exportMode;
        private string zUGFeRDDescription;
        private List<long> acroFormsRefs;
        //private List<long> FAcroFormsAnnotsRefs;
        private List<int> acroFormsFonts;
        // end

        // for signature purposes
        private bool isDigitalSignEnable;
        private bool isDigitalSignatureInvisible;

        private DateTime digitalSignCreationDate;
        private SignatureDictIndicies signatureDictIndicies;
        private long[] digitalSignByteRange;
        private System.Security.Cryptography.X509Certificates.X509Certificate2 digitalSignCertificate;

        private string digitalSignLocation;
        private string digitalSignReason;
        private string digitalSignContactInfo;

        private bool haveToSaveDigitalSignCertificate;
        private string digitalSignCertificatePath;
        private string digitalSignCertificatePassword;
        //end

        #endregion

        #region Public Properties

        #region Options

        /// <summary>
        /// Gets or sets PDF Compliance standard.
        /// After set, do not change other settings, it may lead to fail compliance test.
        /// </summary>
        public PdfStandard PdfCompliance
        {
            get
            {
                return pdfCompliance;
            }
            set
            {
                pdfCompliance = value;
                switch (pdfCompliance)
                {
                    case PdfStandard.None:
                        break;
                    case PdfStandard.PdfA_1a:
                    case PdfStandard.PdfA_2a:
                    case PdfStandard.PdfA_2b:
                    case PdfStandard.PdfA_2u:
                    case PdfStandard.PdfA_3a:
                    case PdfStandard.PdfA_3b:
                        EmbeddingFonts = true;
                        TextInCurves = false;
                        OwnerPassword = "";
                        UserPassword = "";
                        encrypted = false;
                        break;
                    case PdfStandard.PdfX_3:
                    case PdfStandard.PdfX_4:
                        OwnerPassword = "";
                        UserPassword = "";
                        encrypted = false;
                        break;

                }
            }
        }

        /// <summary>
        /// Enable or disable of embedding the TrueType fonts.
        /// </summary>
        public bool EmbeddingFonts
        {
            get { return embeddingFonts; }
            set
            {
                embeddingFonts = value;
                if (embeddingFonts)
                    textInCurves = false;
            }
        }

        /// <summary>
        /// Enable or disable of exporting the background.
        /// </summary>
        public bool Background
        {
            get { return background; }
            set { background = value; }
        }

        /// <summary>
        /// Enable or disable export text in curves
        /// </summary>
        public bool TextInCurves
        {
            get { return textInCurves; }
            set
            {
                textInCurves = value;
                if (textInCurves)
                    embeddingFonts = false;
            }
        }

        /// <summary>
        /// Gets or sets PDF color space
        /// </summary>
        public PdfColorSpace ColorSpace
        {
            get { return colorSpace; }
            set { colorSpace = value; }
        }

        /// <summary>
        /// Enables or disables saving images in their original resolution
        /// </summary>
        public bool ImagesOriginalResolution
        {
            get { return imagesOriginalResolution; }
            set { imagesOriginalResolution = value; }
        }

        /// <summary>
        /// Enables or disables optimization of images for printing
        /// </summary>
        public bool PrintOptimized
        {
            get { return printOptimized; }
            set { printOptimized = value; }
        }

        /// <summary>
        /// Enable or disable image jpeg compression
        /// </summary>
        public bool JpegCompression
        {
            get { return jpegCompression; }
            set { jpegCompression = value; }
        }

        /// <summary>
        /// Sets the quality of images in the PDF
        /// </summary>
        public int JpegQuality
        {
            get { return jpegQuality; }
            set
            {
                jpegQuality = value;
                RichTextQuality = value;
            }
        }

        #endregion

        #region Document Information

        /// <summary>
        /// Title of the document.
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// Author of the document.
        /// </summary>
        public string Author
        {
            get { return author; }
            set { author = value; }
        }

        /// <summary>
        /// Subject of the document.
        /// </summary>
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        /// <summary>
        /// Keywords of the document.
        /// </summary>
        public string Keywords
        {
            get { return keywords; }
            set { keywords = value; }
        }

        /// <summary>
        /// Creator of the document.
        /// </summary>
        public string Creator
        {
            get { return creator; }
            set { creator = value; }
        }

        /// <summary>
        /// Producer of the document.
        /// </summary>
        public string Producer
        {
            get { return producer; }
            set { producer = value; }
        }

        #endregion

        #region Security

        /// <summary>
        /// Sets the owner password.
        /// </summary>
        public string OwnerPassword
        {
            get { return ownerPassword; }
            set { ownerPassword = value; }
        }

        /// <summary>
        /// Sets the user password.
        /// </summary>
        public string UserPassword
        {
            get { return userPassword; }
            set { userPassword = value; }
        }

        /// <summary>
        /// Enable or disable printing in protected document.
        /// </summary>
        public bool AllowPrint
        {
            get { return allowPrint; }
            set { allowPrint = value; }
        }

        /// <summary>
        /// Enable or disable modifying in protected document.
        /// </summary>
        public bool AllowModify
        {
            get { return allowModify; }
            set { allowModify = value; }
        }

        /// <summary>
        /// Enable or disable copying in protected document.
        /// </summary>
        public bool AllowCopy
        {
            get { return allowCopy; }
            set { allowCopy = value; }
        }

        /// <summary>
        /// Enable or disable annotating in protected document.
        /// </summary>
        public bool AllowAnnotate
        {
            get { return allowAnnotate; }
            set { allowAnnotate = value; }
        }

        #endregion

        #region Viewer

        /// <summary>
        /// Enable or disable the print dialog window after opening
        /// </summary>
        public bool ShowPrintDialog
        {
            get { return showPrintDialog; }
            set { showPrintDialog = value; }
        }

        /// <summary>
        /// Enable or disable hide the toolbar.
        /// </summary>
        public bool HideToolbar
        {
            get { return hideToolbar; }
            set { hideToolbar = value; }
        }

        /// <summary>
        /// Enable or disable hide the menu's bar.
        /// </summary>
        public bool HideMenubar
        {
            get { return hideMenubar; }
            set { hideMenubar = value; }
        }

        /// <summary>
        /// Enable or disable hide the Windows UI.
        /// </summary>
        public bool HideWindowUI
        {
            get { return hideWindowUI; }
            set { hideWindowUI = value; }
        }

        /// <summary>
        /// Enable or disable of fitting the window
        /// </summary>
        public bool FitWindow
        {
            get { return fitWindow; }
            set { fitWindow = value; }
        }

        /// <summary>
        /// Enable or disable of centering the window.
        /// </summary>
        public bool CenterWindow
        {
            get { return centerWindow; }
            set { centerWindow = value; }
        }

        /// <summary>
        /// Enable or disable of scaling the page for shrink to printable area.
        /// </summary>
        public bool PrintScaling
        {
            get { return printScaling; }
            set { printScaling = value; }
        }

        /// <summary>
        /// Enable or disable of document's Outline.
        /// </summary>
        public bool Outline
        {
            get { return outline; }
            set { outline = value; }
        }

        /// <summary>
        /// Set default zoom on open document
        /// </summary>
        public MagnificationFactor DefaultZoom
        {
            get { return defaultZoom; }
            set { defaultZoom = value; }
        }

        #endregion

        #region Other

        /// <summary>
        /// Sets the quality of RichText objects in the PDF
        /// </summary>
        public int RichTextQuality
        {
            get { return richTextQuality; }
            set { richTextQuality = value; }
        }

        /// <summary>
        /// Enable or disable the compression in PDF document.
        /// </summary>
        public bool Compressed
        {
            get { return compressed; }
            set { compressed = value; }
        }

        /// <summary>
        /// Enable or disable of images transparency.
        /// </summary>
        public bool TransparentImages
        {
            get { return transparentImages; }
            set { transparentImages = value; }
        }

        /// <summary>
        /// Enable or disable of displaying document's title.
        /// </summary>
        public bool DisplayDocTitle
        {
            get { return displayDocTitle; }
            set { displayDocTitle = value; }
        }

        /// <summary>
        /// Set default page on open document
        /// </summary>
        public int DefaultPage
        {
            get { return defaultPage; }
            set { defaultPage = value; }
        }

        /// <summary>
        /// Color Profile (ICC file).
        /// If "null" then default profile will be used
        /// </summary>
        public byte[] ColorProfile
        {
            get { return colorProfile; }
            set { colorProfile = value; }
        }

        /// <summary>
        /// Gets or sets pdf export mode
        /// </summary>
        public ExportType ExportMode
        {
            get { return exportMode; }
            set { exportMode = value; }
        }

        /// <summary>
        /// Gets pdf AcroForms compatibility, if set then EmbeddingFonts = false and PdfCompliance = PdfStandard.None
        /// </summary>
        public bool InteractiveForms
        {
            get
            {
                return interactiveForms && PdfCompliance == PdfStandard.None;
            }
            set
            {
                if (value)
                    PdfCompliance = PdfStandard.None;
                interactiveForms = value;
            }
        }

        /// <summary>
        /// Set pattern for selection of embedding glyphs for Interactive Forms
        /// </summary>
        public string InteractiveFormsFontSetPattern
        {
            get
            {
                return interactiveFormsFontSetPattern;
            }
            set
            {
                interactiveFormsFontSetPattern = value;
            }
        }
        #endregion

        #region DigitalSign

        /// <summary>
        /// Enable or disable digital sign for pdf document
        /// </summary>
        /// <remarks>
        /// Be sure to specify a valid certificate for signing using the DigitalSignCertificate property.
        /// Or using the DigitalSignCertificatePath and DigitalSignCertificatePassword properties.
        /// </remarks>
        public bool IsDigitalSignEnable
        {
            get { return isDigitalSignEnable; }
            set { isDigitalSignEnable = value; }
        }

        /// <summary>
        /// Should save and serialize password for digital sign certificate.
        /// Do not save password unless absolutely necessary!!!
        /// </summary>
        public bool SaveDigitalSignCertificatePassword
        {
            get { return haveToSaveDigitalSignCertificate; }
            set { haveToSaveDigitalSignCertificate = value; }
        }

        /// <summary>
        /// Manualy sets digital sign certificate for exported documents.
        /// </summary>
        /// <remarks>
        /// This property is in priority, i.e. if a certificate is specified,
        /// the DigitalSignCertificatePath and DigitalSignCertificatePassword properties will not be used.
        /// </remarks>
        public System.Security.Cryptography.X509Certificates.X509Certificate2 DigitalSignCertificate
        {
            set { digitalSignCertificate = value; }
        }

        /// <summary>
        /// The path for load digital sign certificate.
        /// </summary>
        public string DigitalSignCertificatePath
        {
            get { return digitalSignCertificatePath; }
            set { digitalSignCertificatePath = value; }
        }

        /// <summary>
        /// Sets digital sign certificate password.
        /// </summary>
        public string DigitalSignCertificatePassword
        {
            set { digitalSignCertificatePassword = value; }
        }

        /// <summary>
        /// Gets or sets the cpu host name or physical location of the signing
        /// </summary>
        public string DigitalSignLocation
        {
            get { return digitalSignLocation; }
            set { digitalSignLocation = value; }
        }

        /// <summary>
        /// The reason for the signing, such as (I agree ...)
        /// </summary>
        public string DigitalSignReason
        {
            get { return digitalSignReason; }
            set { digitalSignReason = value; }
        }

        /// <summary>
        /// The information to enable the recipient to contact the signer to verify the signature
        /// </summary>
        public string DigitalSignContactInfo
        {
            get { return digitalSignContactInfo; }
            set { digitalSignContactInfo = value; }
        }
        #endregion

        #endregion

        #region Private Methods

        private string getPdfVersion()
        {
            switch (PdfCompliance)
            {
                case PdfStandard.PdfA_2a:
                case PdfStandard.PdfA_2b:
                case PdfStandard.PdfA_2u:
                case PdfStandard.PdfA_3a:
                case PdfStandard.PdfA_3b:
                    return "1.7";
                case PdfStandard.PdfA_1a:
                case PdfStandard.PdfX_3: // PDF/X-3:2003
                    return "1.4";
                case PdfStandard.PdfX_4:
                    return "1.6";
                case PdfStandard.None:
                default:
                    return "1.5";
            }
        }

        private bool isPdfX()
        {
            switch (PdfCompliance)
            {
                case PdfStandard.PdfX_3:
                case PdfStandard.PdfX_4:
                    return true;
                default:
                    return false;
            }
        }

        private bool isPdfA()
        {
            switch (PdfCompliance)
            {
                case PdfStandard.PdfA_1a:
                case PdfStandard.PdfA_2a:
                case PdfStandard.PdfA_2b:
                case PdfStandard.PdfA_2u:
                case PdfStandard.PdfA_3a:
                case PdfStandard.PdfA_3b:
                    return true;
                default:
                    return false;
            }
        }

        private void AddPDFHeader()
        {
            WriteLn(pdf, "%PDF-" + getPdfVersion());
            byte[] signature = { 0x25, 0xE2, 0xE3, 0xCF, 0xD3, 0x0D, 0x0A };
            pdf.Write(signature, 0, signature.Length);
            // reserve object for pages
            UpdateXRef();
        }

        //private void AddPage(ReportPage page)
        //{
        //    pageFonts = new List<ExportTTFFont>();
        //    trasparentStroke = new List<string>();
        //    trasparentFill = new List<string>();
        //    picResList = new List<long>();

        //    paperWidth = ExportUtils.GetPageWidth(page) * Units.Millimeters;
        //    paperHeight = ExportUtils.GetPageHeight(page) * Units.Millimeters;

        //    marginWoBottom = (ExportUtils.GetPageHeight(page) - page.TopMargin) * PDF_PAGE_DIVIDER;
        //    marginLeft = page.LeftMargin * PDF_PAGE_DIVIDER;

        //    pagesHeights.Add(marginWoBottom);
        //    pagesTopMargins.Add(page.TopMargin * PDF_PAGE_DIVIDER);

        //    long FContentsPos = 0;
        //    StringBuilder contentBuilder = new StringBuilder(65535);

        //    // page fill
        //    if (background)
        //        using (TextObject pageFill = new TextObject())
        //        {
        //            pageFill.Fill = page.Fill;
        //            pageFill.Left = -marginLeft / PDF_DIVIDER;
        //            pageFill.Top = -page.TopMargin * PDF_PAGE_DIVIDER / PDF_DIVIDER;
        //            pageFill.Width = ExportUtils.GetPageWidth(page) * PDF_PAGE_DIVIDER / PDF_DIVIDER;
        //            pageFill.Height = ExportUtils.GetPageHeight(page) * PDF_PAGE_DIVIDER / PDF_DIVIDER;
        //            AddTextObject(pageFill, false, contentBuilder);
        //        }

        //    // bitmap watermark on bottom
        //    if (page.Watermark.Enabled && !page.Watermark.ShowImageOnTop)
        //        AddBitmapWatermark(page, contentBuilder);

        //    // text watermark on bottom
        //    if (page.Watermark.Enabled && !page.Watermark.ShowTextOnTop)
        //        AddTextWatermark(page, contentBuilder);

        //    // page borders
        //    if (page.Border.Lines != BorderLines.None)
        //    {
        //        using (TextObject pageBorder = new TextObject())
        //        {
        //            pageBorder.Border = page.Border;
        //            pageBorder.Left = 0;
        //            pageBorder.Top = 0;
        //            pageBorder.Width = (ExportUtils.GetPageWidth(page) - page.LeftMargin - page.RightMargin) * PDF_PAGE_DIVIDER / PDF_DIVIDER;
        //            pageBorder.Height = (ExportUtils.GetPageHeight(page) - page.TopMargin - page.BottomMargin) * PDF_PAGE_DIVIDER / PDF_DIVIDER;
        //            AddTextObject(pageBorder, true, contentBuilder);
        //        }
        //    }

        //    foreach (Base c in page.AllObjects)
        //    {
        //        ExportObj(c);
        //    }

        //    // bitmap watermark on top
        //    if (page.Watermark.Enabled && page.Watermark.ShowImageOnTop)
        //        AddBitmapWatermark(page, contentBuilder);

        //    // text watermark on top
        //    if (page.Watermark.Enabled && page.Watermark.ShowTextOnTop)
        //        AddTextWatermark(page, contentBuilder);

        //    // write page
        //    FContentsPos = UpdateXRef();
        //    WriteLn(pdf, ObjNumber(FContentsPos));

        //    using (MemoryStream tempContentStream = new MemoryStream())
        //    {
        //        Write(tempContentStream, contentBuilder.ToString());
        //        tempContentStream.Position = 0;
        //        WritePDFStream(pdf, tempContentStream, FContentsPos, compressed, encrypted, true, true);
        //    }

        //    if (!textInCurves)
        //        if (pageFonts.Count > 0)
        //            for (int i = 0; i < pageFonts.Count; i++)
        //                if (!pageFonts[i].Saved)
        //                {
        //                    pageFonts[i].Reference = UpdateXRef();
        //                    pageFonts[i].Saved = true;
        //                }

        //    long PageNumber = UpdateXRef();
        //    pagesRef.Add(PageNumber);
        //    WriteLn(pdf, ObjNumber(PageNumber));
        //    StringBuilder sb = new StringBuilder(512);
        //    sb.AppendLine("<<").AppendLine("/Type /Page");
        //    sb.Append("/MediaBox [0 0 ").Append(FloatToString(ExportUtils.GetPageWidth(page) * PDF_PAGE_DIVIDER)).Append(" ");
        //    sb.Append(FloatToString(ExportUtils.GetPageHeight(page) * PDF_PAGE_DIVIDER)).AppendLine(" ]");
        //    //margins
        //    if (isPdfX())
        //        sb.Append("/TrimBox [")
        //            .Append(FloatToString(page.LeftMargin * PDF_PAGE_DIVIDER)).Append(" ")
        //            .Append(FloatToString(page.TopMargin * PDF_PAGE_DIVIDER)).Append(" ")
        //            .Append(FloatToString(page.RightMargin * PDF_PAGE_DIVIDER)).Append(" ")
        //            .Append(FloatToString(page.BottomMargin * PDF_PAGE_DIVIDER)).Append("]");

        //    sb.AppendLine("/Parent 1 0 R");
        //    if (!isPdfX())
        //    {
        //        if (ColorSpace == PdfColorSpace.RGB)
        //            sb.AppendLine("/Group << /Type /Group /S /Transparency /CS /DeviceRGB >>");
        //        else if (ColorSpace == PdfColorSpace.CMYK)
        //            sb.AppendLine("/Group << /Type /Group /S /Transparency /CS /DeviceCMYK >>");
        //    }
        //    sb.AppendLine("/Resources << ");

        //    if (pageFonts.Count > 0)
        //    {
        //        sb.Append("/Font << ");
        //        foreach (ExportTTFFont font in pageFonts)
        //            sb.Append(font.Name).Append(" ").Append(ObjNumberRef(font.Reference)).Append(" ");
        //        sb.AppendLine(" >>");
        //    }

        //    if (isPdfX())
        //    {
        //        sb.AppendLine("/ExtGState <<");
        //        for (int i = 0; i < trasparentStroke.Count; i++)
        //            sb.Append("/GS").Append(i.ToString()).Append("S << /Type /ExtGState /ca ").Append(1).AppendLine(" >>");
        //        for (int i = 0; i < trasparentFill.Count; i++)
        //            sb.Append("/GS").Append(i.ToString()).Append("F << /Type /ExtGState /CA ").Append(1).AppendLine(" >>");
        //        sb.AppendLine(">>");
        //    }
        //    else
        //    {
        //        sb.AppendLine("/ExtGState <<");
        //        for (int i = 0; i < trasparentStroke.Count; i++)
        //            sb.Append("/GS").Append(i.ToString()).Append("S << /Type /ExtGState /ca ").Append(trasparentStroke[i]).AppendLine(" >>");
        //        for (int i = 0; i < trasparentFill.Count; i++)
        //            sb.Append("/GS").Append(i.ToString()).Append("F << /Type /ExtGState /CA ").Append(trasparentFill[i]).AppendLine(" >>");
        //        sb.AppendLine(">>");
        //    }

        //    if (picResList.Count > 0)
        //    {
        //        sb.Append("/XObject << ");
        //        foreach (long resIndex in picResList)
        //            sb.Append("/Im").Append(resIndex.ToString()).Append(" ").Append(ObjNumberRef(resIndex)).Append(" ");
        //        sb.AppendLine(" >>");
        //    }

        //    sb.AppendLine("/ProcSet [/PDF /Text /ImageC ]");
        //    sb.AppendLine(">>");

        //    sb.Append("/Contents ").AppendLine(ObjNumberRef(FContentsPos));
        //    if (pageAnnots.Length > 0)
        //    {
        //        sb.AppendLine(GetPageAnnots());
        //        pageAnnots.Length = 0;
        //    }

        //    sb.AppendLine(">>");
        //    sb.AppendLine("endobj");
        //    Write(pdf, sb.ToString());
        //}

        private void AddBitmapWatermark(ReportPage page, StringBuilder sb)
        {
            if (page.Watermark.Image != null)
            {
                using (PictureObject pictureWatermark = new PictureObject())
                {
                    pictureWatermark.Left = -marginLeft / PDF_DIVIDER;
                    pictureWatermark.Top = -page.TopMargin * PDF_PAGE_DIVIDER / PDF_DIVIDER;
                    pictureWatermark.Width = ExportUtils.GetPageWidth(page) * PDF_PAGE_DIVIDER / PDF_DIVIDER;
                    pictureWatermark.Height = ExportUtils.GetPageHeight(page) * PDF_PAGE_DIVIDER / PDF_DIVIDER;

                    pictureWatermark.SizeMode = PictureBoxSizeMode.Normal;
                    pictureWatermark.Image = new Bitmap((int)pictureWatermark.Width, (int)pictureWatermark.Height);
                    float transparency = page.Watermark.ImageTransparency;
                    page.Watermark.ImageTransparency = 0;
                    using (Graphics g = Graphics.FromImage(pictureWatermark.Image))
                    {
                        g.Clear(Color.Transparent);
                        page.Watermark.DrawImage(new FRPaintEventArgs(g, 1, 1, Report.GraphicCache),
                            new RectangleF(0, 0, pictureWatermark.Width, pictureWatermark.Height), Report, true);
                    }
                    pictureWatermark.Transparency = page.Watermark.ImageTransparency = transparency;
                    pictureWatermark.Fill = new SolidFill(Color.Transparent);
                    pictureWatermark.FillColor = Color.Transparent;
                    AddPictureObject(pictureWatermark, false, jpegQuality, sb);
                }
            }
        }

        private void AddTextWatermark(ReportPage page, StringBuilder sb)
        {
            if (!String.IsNullOrEmpty(page.Watermark.Text))
                using (TextObject textWatermark = new TextObject())
                {
                    textWatermark.HorzAlign = HorzAlign.Center;
                    textWatermark.VertAlign = VertAlign.Center;
                    textWatermark.Left = -marginLeft / PDF_DIVIDER;
                    textWatermark.Top = -page.TopMargin * PDF_PAGE_DIVIDER / PDF_DIVIDER;
                    textWatermark.Width = ExportUtils.GetPageWidth(page) * PDF_PAGE_DIVIDER / PDF_DIVIDER;
                    textWatermark.Height = ExportUtils.GetPageHeight(page) * PDF_PAGE_DIVIDER / PDF_DIVIDER;
                    textWatermark.Text = page.Watermark.Text;
                    textWatermark.TextFill = page.Watermark.TextFill;
                    if (page.Watermark.TextRotation == WatermarkTextRotation.Vertical)
                        textWatermark.Angle = 270;
                    else if (page.Watermark.TextRotation == WatermarkTextRotation.ForwardDiagonal)
                        textWatermark.Angle = 360 - (int)(Math.Atan(textWatermark.Height / textWatermark.Width) * (180 / Math.PI));
                    else if (page.Watermark.TextRotation == WatermarkTextRotation.BackwardDiagonal)
                        textWatermark.Angle = (int)(Math.Atan(textWatermark.Height / textWatermark.Width) * (180 / Math.PI));
                    textWatermark.Font = page.Watermark.Font;
                    if (page.Watermark.TextFill is SolidFill)
                        textWatermark.TextColor = (page.Watermark.TextFill as SolidFill).Color;
                    textWatermark.Fill = new SolidFill(Color.Transparent);
                    textWatermark.FillColor = Color.Transparent;
                    AddTextObject(textWatermark, false, sb);
                }
        }

        private void AddTable(TableBase table, bool drawCells, StringBuilder sb_in)
        {
            float y = 0;
            StringBuilder sb = new StringBuilder(1024);
            for (int i = 0; i < table.RowCount; i++)
            {
                float x = 0;
                for (int j = 0; j < table.ColumnCount; j++)
                {
                    if (!table.IsInsideSpan(table[j, i]))
                    {
                        table[j, i].Left = x;
                        table[j, i].Top = y;

                        if (drawCells)
                        {
                            Border oldBorder = table[j, i].Border.Clone();
                            table[j, i].Border.Lines = BorderLines.None;
                            if (table[j, i] is TextObject)
                                AddTextObject(table[j, i] as TextObject, false, sb_in);
                            else
                                AddPictureObject(table[j, i] as ReportComponentBase, false, jpegQuality, sb_in);
                            table[j, i].Border = null;
                            table[j, i].Border = oldBorder;
                        }
                        else
                            DrawPDFBorder(table[j, i].Border, table[j, i].AbsLeft, table[j, i].AbsTop, table[j, i].Width, table[j, i].Height, sb);
                    }
                    x += (table.Columns[j]).Width;
                }
                y += (table.Rows[i]).Height;
            }
            sb_in.Append(sb);
        }

        private void AddShape(ShapeObject shapeObject, StringBuilder sb)
        {
            if (shapeObject.Shape != ShapeKind.RoundRectangle && shapeObject.Fill is SolidFill)
            {
                if (shapeObject.Shape == ShapeKind.Rectangle)
                {
                    DrawPDFFillRect(
                        GetLeft(shapeObject.AbsLeft), GetTop(shapeObject.AbsTop),
                        shapeObject.Width * PDF_DIVIDER, shapeObject.Height * PDF_DIVIDER,
                        shapeObject.Fill, sb);
                    DrawPDFRect(
                        GetLeft(shapeObject.AbsLeft),
                        GetTop(shapeObject.AbsTop),
                        GetLeft(shapeObject.AbsLeft + shapeObject.Width),
                        GetTop(shapeObject.AbsTop + shapeObject.Height),
                        shapeObject.Border.Color, shapeObject.Border.Width * PDF_DIVIDER, shapeObject.Border.Style, sb);
                }
                else if (shapeObject.Shape == ShapeKind.Triangle)
                    DrawPDFTriangle(GetLeft(shapeObject.AbsLeft), GetTop(shapeObject.AbsTop),
                        shapeObject.Width * PDF_DIVIDER, shapeObject.Height * PDF_DIVIDER,
                        shapeObject.FillColor, shapeObject.Border.Color, shapeObject.Border.Width * PDF_DIVIDER, shapeObject.Border.Style, sb);
                else if (shapeObject.Shape == ShapeKind.Diamond)
                    DrawPDFDiamond(GetLeft(shapeObject.AbsLeft), GetTop(shapeObject.AbsTop),
                        shapeObject.Width * PDF_DIVIDER, shapeObject.Height * PDF_DIVIDER,
                        shapeObject.FillColor, shapeObject.Border.Color, shapeObject.Border.Width * PDF_DIVIDER, shapeObject.Border.Style, sb);
                else if (shapeObject.Shape == ShapeKind.Ellipse)
                    DrawPDFEllipse(GetLeft(shapeObject.AbsLeft), GetTop(shapeObject.AbsTop),
                        shapeObject.Width * PDF_DIVIDER, shapeObject.Height * PDF_DIVIDER,
                        shapeObject.FillColor, shapeObject.Border.Color, shapeObject.Border.Width * PDF_DIVIDER, shapeObject.Border.Style, sb);

                if (!isPdfX())
                    AddAnnot(shapeObject);
            }
            else
                AddPictureObject(shapeObject, true, jpegQuality, sb);
        }

        //private void AddPolyLine(PolyLineObject obj, StringBuilder sb)
        //{
        //    int len = obj.PointsArray.Length;
        //    if (len == 0 || len == 1)
        //    {
        //        float localX = GetLeft(obj.AbsLeft);
        //        float localY = GetTop(obj.AbsTop);
        //        DrawPDFLine(
        //            localX, localY + 6 * PDF_DIVIDER,
        //            localX, localY - 6 * PDF_DIVIDER,
        //            obj.Border.Color, obj.Border.Width * PDF_DIVIDER, obj.Border.Style, null, null, sb);
        //        DrawPDFLine(
        //            localX - 6 * PDF_DIVIDER, localY,
        //            localX + 6 * PDF_DIVIDER, localY,
        //            obj.Border.Color, obj.Border.Width * PDF_DIVIDER, obj.Border.Style, null, null, sb);
        //    }
        //    else if (len == 2)
        //    {
        //        DrawPDFLine(
        //            GetLeft(obj.AbsLeft) + (obj.PointsArray[0].X + obj.CenterX) * PDF_DIVIDER,
        //            GetTop(obj.AbsTop) - (obj.PointsArray[0].Y + obj.CenterY) * PDF_DIVIDER,
        //            GetLeft(obj.AbsLeft) + (obj.PointsArray[1].X + obj.CenterX) * PDF_DIVIDER,
        //            GetTop(obj.AbsTop) - (obj.PointsArray[1].Y + obj.CenterY) * PDF_DIVIDER, obj.Border.Color, obj.Border.Width * PDF_DIVIDER, obj.Border.Style, null, null, sb);
        //    }
        //    else
        //    {
        //        if (obj is PolygonObject)
        //        {
        //            if (obj.Fill is SolidFill)
        //                DrawPDFPolygon(GetLeft(obj.AbsLeft),
        //                GetTop(obj.AbsTop), GetLeft(obj.AbsLeft + obj.Width), GetTop(obj.AbsTop + obj.Height),
        //                obj.CenterX, obj.CenterY, obj.PointsArray, obj.FillColor,
        //                obj.Border.Color, obj.Border.Width * PDF_DIVIDER, obj.Border.Style, sb);
        //            //else if (obj.Fill is LinearGradientFill || obj.Fill is PathGradientFill)
        //            //    FillPDFGraphicsPath(GetLeft(obj.AbsLeft),
        //            //    GetTop(obj.AbsTop), obj.Width * PDF_DIVIDER, obj.Height * PDF_DIVIDER,
        //            //    obj.GetPath(null, 0,0,0,0, PDF_DIVIDER, PDF_DIVIDER),
        //            //    obj.Fill.CreateBrush(new RectangleF(0, 0, obj.Width, obj.Height)), sb);
        //            else
        //                AddPictureObject(obj, true, jpegQuality, sb);
        //        }
        //        else
        //            DrawPDFPolyLine(GetLeft(obj.AbsLeft),
        //                GetTop(obj.AbsTop), GetLeft(obj.AbsLeft + obj.Width), GetTop(obj.AbsTop + obj.Height),
        //                obj.CenterX, obj.CenterY, obj.PointsArray, false,
        //                obj.Border.Color, obj.Border.Width * PDF_DIVIDER, obj.Border.Style, sb);
        //    }
        //}



        private void AddLine(LineObject l, StringBuilder sb)
        {
            DrawPDFLine(GetLeft(l.AbsLeft),
                GetTop(l.AbsTop), GetLeft(l.AbsLeft + l.Width), GetTop(l.AbsTop + l.Height),
                l.Border.Color, l.Border.Width * PDF_DIVIDER, l.Border.Style, l.StartCap, l.EndCap, sb);
        }

        private void AddBandObject(BandBase band, StringBuilder sb)
        {
            using (TextObject newObj = new TextObject())
            {
                newObj.Left = band.AbsLeft;
                newObj.Top = band.AbsTop;
                newObj.Width = band.Width;
                newObj.Height = band.Height;
                newObj.Fill = band.Fill;
                newObj.Border = band.Border;
                AddTextObject(newObj, true, sb);
            }
        }

        private void AddTextObject(TextObject obj, bool drawBorder, StringBuilder sb_in)
        {
            string Width = FloatToString(obj.Width * PDF_DIVIDER);
            string Height = FloatToString(obj.Height * PDF_DIVIDER);

            if (!isPdfX())
                AddAnnot(obj);

            StringBuilder sb = new StringBuilder(256);
            StringBuilder image_sb = null;

            sb.AppendLine("q");
            sb.Append(FloatToString(GetLeft(obj.AbsLeft))).Append(" ");
            sb.Append(FloatToString(GetTop(obj.AbsTop + obj.Height))).Append(" ");
            sb.Append(FloatToString((obj.Width) * PDF_DIVIDER)).Append(" ");
            sb.Append(FloatToString((obj.Height) * PDF_DIVIDER)).AppendLine(" re");
            if (obj.Clip)
                sb.AppendLine("W");
            sb.AppendLine("n");

            // draw background
            if (obj.Fill is SolidFill || (obj.Fill is GlassFill && !(obj.Fill as GlassFill).Hatch) || IsFillableGradientGrid(obj.Fill))
                DrawPDFFillRect(GetLeft(obj.AbsLeft), GetTop(obj.AbsTop),
                    obj.Width * PDF_DIVIDER, obj.Height * PDF_DIVIDER, obj.Fill, sb);
            else if (obj.Width > 0 && obj.Height > 0)
            {
                using (PictureObject backgroundPicture = new PictureObject())
                {
                    backgroundPicture.Left = obj.AbsLeft;
                    backgroundPicture.Top = obj.AbsTop;
                    backgroundPicture.Width = obj.Width;
                    backgroundPicture.Height = obj.Height;
                    backgroundPicture.Image = new Bitmap((int)backgroundPicture.Width, (int)backgroundPicture.Height);
                    using (Graphics g = Graphics.FromImage(backgroundPicture.Image))
                    {
                        g.Clear(Color.Transparent);
                        g.TranslateTransform(-obj.AbsLeft, -obj.AbsTop);
                        BorderLines oldLines = obj.Border.Lines;
                        obj.Border.Lines = BorderLines.None;
                        string oldText = obj.Text;
                        obj.Text = String.Empty;
                        obj.Draw(new FRPaintEventArgs(g, 1, 1, Report.GraphicCache));
                        obj.Text = oldText;
                        obj.Border.Lines = oldLines;
                    }
                    AddPictureObject(backgroundPicture, false, jpegQuality, sb_in);
                }
            }
            switch (obj.TextRenderType)
            {
                case TextRenderType.HtmlParagraph:
                    {
                        RectangleF textRect = new RectangleF(
                             obj.AbsLeft + obj.Padding.Left,
                             obj.AbsTop + obj.Padding.Top,
                             obj.Width - obj.Padding.Horizontal,
                             obj.Height - obj.Padding.Vertical);
                        AddTextObjectHtmlInternal(obj, textRect, sb);
                    }
                    break;
                default:
                    if (obj.Underlines)
                        AppendUnderlines(sb, obj);

                    if (obj.Editable && InteractiveForms)
                    {
                        long xref = AddTextDefaultValueForEditable(obj);
                        AddTextField(obj, xref);
                    }
                    else if (!String.IsNullOrEmpty(obj.Text))
                    {
                        int ObjectFontNumber = GetObjFontNumber(obj.Font);
                        // obj with HtmlTags uses own font/color for each word/run
                        if (!obj.HasHtmlTags)
                            AppendFont(sb, ObjectFontNumber, obj.Font.Size, obj.TextColor);

                        using (Font f = new Font(obj.Font.Name, obj.Font.Size * dpiFX, obj.Font.Style))
                        {
                            RectangleF textRect = new RectangleF(
                              obj.AbsLeft + obj.Padding.Left,
                              obj.AbsTop + obj.Padding.Top,
                              obj.Width - obj.Padding.Horizontal,
                              obj.Height - obj.Padding.Vertical);

                            bool transformNeeded = obj.Angle != 0 || obj.FontWidthRatio != 1;

                            // transform, rotate and scale pdf coordinates if needed
                            if (transformNeeded)
                            {
                                textRect.X = -textRect.Width / 2;
                                textRect.Y = -textRect.Height / 2;

                                float angle = (float)((360 - obj.Angle) * Math.PI / 180);
                                float sin = (float)Math.Sin(angle);
                                float cos = (float)Math.Cos(angle);
                                float x = GetLeft(obj.AbsLeft + obj.Width / 2);
                                float y = GetTop(obj.AbsTop + obj.Height / 2);
                                // offset the origin to the middle of bounding rectangle, then rotate
                                sb.Append(FloatToString(cos)).Append(" ").
                                    Append(FloatToString(sin)).Append(" ").
                                    Append(FloatToString(-sin)).Append(" ").
                                    Append(FloatToString(cos)).Append(" ").
                                    Append(FloatToString(x)).Append(" ").
                                    Append(FloatToString(y)).AppendLine(" cm");

                                // apply additional matrix to scale x coordinate
                                if (obj.FontWidthRatio != 1)
                                    sb.Append(FloatToString(obj.FontWidthRatio)).AppendLine(" 0 0 1 0 0 cm");
                            }

                            image_sb = AddTextObjectInternal(obj, textRect, transformNeeded, ObjectFontNumber, sb, f, false);
                        }
                    }
                    break;
            }


            sb.AppendLine("Q");
            if (drawBorder)
                DrawPDFBorder(obj.Border, obj.AbsLeft, obj.AbsTop, obj.Width, obj.Height, sb);
            sb_in.Append(sb);
            if (image_sb != null)
                sb_in.Append(image_sb);
        }

        private void AddTextObjectHtmlInternal(TextObject obj, RectangleF textRect, StringBuilder sb)
        {
            StringFormat format = obj.GetStringFormat(Report.GraphicCache /*cache*/, 0);
            Color color = Color.Black;
            if (obj.TextFill is SolidFill) color = (obj.TextFill as SolidFill).Color;
            using (HtmlTextRenderer renderer = new HtmlTextRenderer(obj.Text, graphics, obj.Font.Name, obj.Font.Size, obj.Font.Style, color,
                  obj.TextOutline.Color, textRect, obj.Underlines,
                  format, obj.HorzAlign, obj.VertAlign, obj.ParagraphFormat.MultipleScale(dpiFX), obj.ForceJustify,
                  dpiFX, dpiFX, obj.InlineImageCache))
            {
                foreach (HtmlTextRenderer.RectangleFColor rect in renderer.Backgrounds)
                    DrawPDFFillRect(GetLeft(rect.Left), GetTop(rect.Top),
                           rect.Width * PDF_DIVIDER, rect.Height * PDF_DIVIDER, new SolidFill(rect.Color), sb);
                List<PictureObject> pictures = new List<PictureObject>();
                if (obj.RightToLeft)
                {
                    foreach (HtmlTextRenderer.Paragraph paragraph in renderer.Paragraphs)
                        foreach (HtmlTextRenderer.Line line in paragraph.Lines)
                            foreach (HtmlTextRenderer.Word word in line.Words)
                                if (word.Type == HtmlTextRenderer.WordType.Normal)
                                    foreach (HtmlTextRenderer.Run run in word.Runs)
                                    {
                                        if (run is HtmlTextRenderer.RunText)
                                        {
                                            HtmlTextRenderer.RunText runText = run as HtmlTextRenderer.RunText;
                                            using (Font fnt = runText.Style.GetFont())
                                            {
                                                int ObjectFontNumber = GetObjFontNumber(fnt);
                                                AppendFont(sb, ObjectFontNumber, fnt.Size / dpiFX, run.Style.Color);
                                                AppendText(sb, ObjectFontNumber, fnt, run.Left - run.Width + fnt.Size * 0.2f, run.Top, 0, runText.Text,
                                                    obj.RightToLeft, false, obj.TextOutline, run.Style.Color, false, 0);
                                            }
                                        }
                                        else if (run is HtmlTextRenderer.RunImage)
                                        {
                                            HtmlTextRenderer.RunImage runImage = run as HtmlTextRenderer.RunImage;
                                            PictureObject picture = new PictureObject();
                                            picture.Left = (runImage.Left - runImage.Width) / renderer.Scale;
                                            picture.Top = runImage.Top / renderer.Scale;
                                            picture.Width = runImage.Width / renderer.Scale;
                                            picture.Height = runImage.Height / renderer.Scale;
                                            picture.SizeMode = PictureBoxSizeMode.StretchImage;
                                            picture.Image = runImage.Image;
                                            picture.SetReport(obj.Report);
                                            pictures.Add(picture);
                                        }
                                    }
                }
                else
                {
                    foreach (HtmlTextRenderer.Paragraph paragraph in renderer.Paragraphs)
                        foreach (HtmlTextRenderer.Line line in paragraph.Lines)
                            foreach (HtmlTextRenderer.Word word in line.Words)
                                if (word.Type == HtmlTextRenderer.WordType.Normal)
                                    foreach (HtmlTextRenderer.Run run in word.Runs)
                                    {
                                        if (run is HtmlTextRenderer.RunText)
                                        {
                                            HtmlTextRenderer.RunText runText = run as HtmlTextRenderer.RunText;
                                            using (Font fnt = runText.Style.GetFont())
                                            {
                                                int ObjectFontNumber = GetObjFontNumber(fnt);
                                                AppendFont(sb, ObjectFontNumber, fnt.Size / dpiFX, run.Style.Color);
                                                AppendText(sb, ObjectFontNumber, fnt, run.Left, run.Top, 0, runText.Text,
                                                    obj.RightToLeft, false, obj.TextOutline, run.Style.Color, false, 0);
                                            }
                                        }
                                        else if (run is HtmlTextRenderer.RunImage)
                                        {
                                            HtmlTextRenderer.RunImage runImage = run as HtmlTextRenderer.RunImage;
                                            PictureObject picture = new PictureObject();
                                            picture.Left = runImage.Left / renderer.Scale;
                                            picture.Top = runImage.Top / renderer.Scale;
                                            picture.Width = runImage.Width / renderer.Scale;
                                            picture.Height = runImage.Height / renderer.Scale;
                                            picture.SizeMode = PictureBoxSizeMode.StretchImage;
                                            picture.Image = runImage.Image;
                                            picture.SetReport(obj.Report);
                                            pictures.Add(picture);
                                        }
                                    }
                }

                foreach (PictureObject pobj in pictures)
                {
                    try
                    {
                        AddPictureObject(pobj, false, jpegQuality, sb);
                    }
                    finally
                    {
                        pobj.Dispose();
                    }
                }

                foreach (HtmlTextRenderer.LineFColor line in renderer.Underlines)
                    DrawPDFLine(GetLeft(line.Left), GetTop(line.Top), GetLeft(line.Right), GetTop(line.Top),
                        line.Color, line.Width * PDF_DIVIDER, LineStyle.Solid, null, null, sb);

                foreach (HtmlTextRenderer.LineFColor line in renderer.Stikeouts)
                    DrawPDFLine(GetLeft(line.Left), GetTop(line.Top), GetLeft(line.Right), GetTop(line.Top),
                        line.Color, line.Width * PDF_DIVIDER, LineStyle.Solid, null, null, sb);
            }
        }

        private StringBuilder AddTextObjectInternal(TextObject obj, RectangleF textRect, bool transformNeeded,
            int ObjectFontNumber, StringBuilder sb, Font f, bool bbox)
        {
            StringBuilder image_sb = null;
            // break the text to paragraphs, lines, words and runs
            StringFormat format = obj.GetStringFormat(Report.GraphicCache /*cache*/, 0);
            Brush textBrush = Report.GraphicCache.GetBrush(obj.TextColor);
            AdvancedTextRenderer renderer = new AdvancedTextRenderer(obj.Text, graphics, f, textBrush, null,
                textRect, format, obj.HorzAlign, obj.VertAlign, obj.LineHeight, obj.Angle, obj.FontWidthRatio,
                obj.ForceJustify, obj.Wysiwyg, obj.HasHtmlTags, true, dpiFX, dpiFX,
                obj.InlineImageCache);
            if (obj.HasHtmlTags == true)
            {
                image_sb = new StringBuilder();
                foreach (PictureObject pobj in obj.GetPictureFromHtmlText(renderer))
                {
                    // obj.ChildObjects.Add(pobj);
                    //pobj.Parent = obj;
                    //if (obj != null)
                    //pobj.Top = pobj.Top - obj.AbsTop;
                    pobj.SetReport(obj.Report);
                    AddPictureObject(pobj, false, jpegQuality, image_sb);
                }
            }
            float w = f.Height * 0.1f; // to match .net char X offset
                                       // invert offset in case of rtl
            if (obj.RightToLeft)
                w = -w;
            // we don't need this offset if text is centered
            if (obj.HorzAlign == HorzAlign.Center)
                w = 0;

            // render
            foreach (AdvancedTextRenderer.Paragraph paragraph in renderer.Paragraphs)
                foreach (AdvancedTextRenderer.Line line in paragraph.Lines)
                {
                    float lineOffset = 0;
                    float lineHeight = line.CalcHeight();
                    float objHeight = (obj.Angle == 0 || obj.Angle == 180) ?
                        obj.Height - obj.Padding.Vertical :
                        obj.Width - obj.Padding.Horizontal;
                    if (lineHeight > objHeight)
                    {
                        if (obj.VertAlign == VertAlign.Center)
                            lineOffset = -lineHeight / 2;
                        else if (obj.VertAlign == VertAlign.Bottom)
                            lineOffset = -lineHeight;
                    }
                    foreach (RectangleF rect in line.Underlines)
                        DrawPDFUnderline(ObjectFontNumber, f, rect.Left, rect.Top, rect.Width, w,
                            obj.TextColor, transformNeeded, sb);
                    foreach (RectangleF rect in line.Strikeouts)
                        DrawPDFStrikeout(ObjectFontNumber, f, rect.Left, rect.Top, rect.Width, w,
                            obj.TextColor, transformNeeded, sb);

                    foreach (AdvancedTextRenderer.Word word in line.Words)
                        if (renderer.HtmlTags)
                            foreach (AdvancedTextRenderer.Run run in word.Runs)
                                using (Font fnt = run.GetFont())
                                {
                                    ObjectFontNumber = GetObjFontNumber(fnt);
                                    AppendFont(sb, ObjectFontNumber, fnt.Size / dpiFX, run.Style.Color);
                                    AppendText(sb, ObjectFontNumber, fnt, run.Left, run.Top, w, run.Text,
                                        obj.RightToLeft, transformNeeded, obj.TextOutline, run.Style.Color, bbox, obj.Height - obj.Padding.Vertical);
                                }
                        else
                            AppendText(sb, ObjectFontNumber, f, word.Left, word.Top + lineOffset, w, word.Text,
                                obj.RightToLeft, transformNeeded, obj.TextOutline, obj.TextColor, bbox, obj.Height - obj.Padding.Vertical);
                }
            return image_sb;
        }

        private void AppendUnderlines(StringBuilder Result, TextObject obj)
        {
            float lineHeight = obj.LineHeight == 0 ? obj.Font.GetHeight() : obj.LineHeight;
            lineHeight *= dpiFX * PDF_DIVIDER;
            float curY = GetTop(obj.AbsTop) - lineHeight;
            float bottom = GetTop(obj.AbsBottom);
            float left = GetLeft(obj.AbsLeft);
            float right = GetLeft(obj.AbsRight);
            float width = obj.Border.Width * PDF_DIVIDER;
            while (curY > bottom)
            {
                DrawPDFLine(left, curY, right, curY, obj.Border.Color, width, LineStyle.Solid, null, null, Result);
                curY -= lineHeight;
            }
        }

        private void DrawText(StringBuilder Result, float x, float y, string s, ExportTTFFont font, Color fontColor)
        {
            bool SimulateItalic = font.NeedSimulateItalic && font.SourceFont.Italic;
            bool SimulateBold = font.NeedSimulateBold && font.SourceFont.Bold;

            Result.AppendLine("BT");
            Result.Append(FloatToString(x)).Append(" ").Append(FloatToString(y)).AppendLine(" Td");
            if (SimulateItalic)
                Result.Append("1 0 0.3333 1 ").Append(FloatToString(x)).Append(' ').Append(FloatToString(y)).AppendLine(" Tm");
            if (SimulateBold)
            {
                GetPDFStrokeColor(fontColor, Result);
                Result.Append("2 Tr ").Append(FloatToString(font.SourceFontBoldMultiplier)).Append(" w ");
            }
            else
                Result.AppendLine("0 Tr");
            Result.Append("<").Append(ExportUtils.StrToHex2(s)).AppendLine("> Tj");
            if (SimulateBold)
                Result.AppendLine("0 Tr");
            Result.AppendLine("ET");
        }

        private void DrawTextOutline(StringBuilder Result, float x, float y, string s, TextOutline objTextOutline)
        {
            Result.AppendLine();
            GetPDFStrokeColor(objTextOutline.Color, Result);
            Result.AppendLine("BT");
            Result.Append(FloatToString(x)).Append(" ").Append(FloatToString(y)).AppendLine(" Td");
            Result.AppendLine("1 Tr");
            Result.Append(FloatToString(objTextOutline.Width * PDF_DIVIDER)).AppendLine(" w");
            Result.Append("<").Append(ExportUtils.StrToHex2(s)).AppendLine("> Tj");
            Result.AppendLine("ET");
        }

        private void AppendText(StringBuilder Result, int fontNumber, Font font, float x, float y, float offsX, string text, bool rtl, bool transformNeeded, TextOutline objTextOutline)
        {
            AppendText(Result, fontNumber, font, x, y, offsX, text, rtl, transformNeeded, objTextOutline, Color.Black);
        }

        private void AppendText(StringBuilder Result, int fontNumber, Font font, float x, float y, float offsX, string text, bool rtl, bool transformNeeded, TextOutline objTextOutline, Color fontColor)
        {
            AppendText(Result, fontNumber, font, x, y, offsX, text, rtl, transformNeeded, objTextOutline, fontColor, false, 0);
        }


        private void AppendText(StringBuilder Result, int fontNumber, Font font, float x, float y, float offsX, string text, bool rtl, bool transformNeeded, TextOutline objTextOutline, Color fontColor, bool bbox, float bbox_height)
        {
            if (!textInCurves)
            {
                ExportTTFFont pdffont = pageFonts[fontNumber];
                // text with fonts
                if (!bbox)
                {
                    x = (transformNeeded ? x * PDF_DIVIDER : GetLeft(x)) + offsX;
                    y = transformNeeded ? -y * PDF_DIVIDER : GetTop(y);
                    y -= GetBaseline(font) * PDF_DIVIDER;
                }
                else
                {
                    x = x * PDF_DIVIDER + offsX;
                    y = (bbox_height - y - GetBaseline(font)) * PDF_DIVIDER;
                }

                string s = pdffont.RemapString(text, rtl);

                if (objTextOutline == null || !objTextOutline.Enabled)
                {
                    DrawText(Result, x, y, s, pdffont, fontColor);
                }
                else
                {
                    if (objTextOutline.DrawBehind)
                    {
                        DrawTextOutline(Result, x, y, s, objTextOutline);
                        DrawText(Result, x, y, s, pdffont, fontColor);
                    }
                    else
                    {
                        DrawText(Result, x, y, s, pdffont, fontColor);
                        DrawTextOutline(Result, x, y, s, objTextOutline);
                    }
                }
            }
            else
            {
                if (!bbox)
                {
                    x = (transformNeeded ? x * PDF_DIVIDER : GetLeft(x)) + offsX;
                    y = transformNeeded ? -y * PDF_DIVIDER : GetTop(y);
                    y -= GetBaseline(font) * PDF_DIVIDER;
                }
                else
                {
                    x = x * PDF_DIVIDER + offsX;
                    y = (bbox_height - y + GetBaseline(font)) * PDF_DIVIDER;
                }

                ExportTTFFont pdffont = pageFonts[fontNumber];

                float paddingX;
                float paddingY;
                ExportTTFFont.GlyphTTF[] txt = pdffont.getGlyphString(text, rtl, font.Size, out paddingX, out paddingY);
                if (!objTextOutline.Enabled)
                {
                    float shift = 0;
                    foreach (ExportTTFFont.GlyphTTF ch in txt)
                    {
                        //draw ch;
                        DrawPDFPolygonChar(ch.path, x + shift * PDF_DIVIDER, y, fontColor, Result);
                        shift += ch.width;
                    }
                }
                else
                {
                    if (objTextOutline.DrawBehind)
                    {
                        //outline
                        float shift = 0;
                        foreach (ExportTTFFont.GlyphTTF ch in txt)
                        {
                            //draw ch;
                            DrawPDFPolygonCharOutline(ch.path, x + shift * PDF_DIVIDER, y, objTextOutline.Color, objTextOutline.Width, Result);
                            shift += ch.width;
                        }

                        //fill
                        shift = 0;
                        foreach (ExportTTFFont.GlyphTTF ch in txt)
                        {
                            //draw ch;
                            DrawPDFPolygonChar(ch.path, x + shift * PDF_DIVIDER, y, fontColor, Result);
                            shift += ch.width;
                        }
                    }
                    else
                    {
                        //fill
                        float shift = 0;
                        foreach (ExportTTFFont.GlyphTTF ch in txt)
                        {
                            //draw ch;
                            DrawPDFPolygonChar(ch.path, x + shift * PDF_DIVIDER, y, fontColor, Result);
                            shift += ch.width;
                        }
                        //ouline
                        shift = 0;
                        foreach (ExportTTFFont.GlyphTTF ch in txt)
                        {
                            //draw ch;
                            DrawPDFPolygonCharOutline(ch.path, x + shift * PDF_DIVIDER, y, objTextOutline.Color, objTextOutline.Width, Result);
                            shift += ch.width;
                        }
                    }
                }
            }
        }

        private string GetZoomString(int page, float zoom)
        {
            return ExportUtils.StringFormat(" /XYZ 0 {0} {1}", Math.Round(pagesHeights[page - 1] + pagesTopMargins[page - 1]).ToString(), FloatToString(zoom));
        }

        private void SetMagnificationFactor(int PageNumber, MagnificationFactor factor)
        {
            if (factor == MagnificationFactor.Default)
                return;
            if (pagesRef.Count <= 0)
                return;

            string Magnificator = "";

            actionDict = UpdateXRef();
            WriteLn(pdf, ObjNumber(actionDict));
            WriteLn(pdf, "<<");
            WriteLn(pdf, "/S /GoTo");
            switch (factor)
            {
                case MagnificationFactor.ActualSize:
                    Magnificator = GetZoomString(PageNumber, 1f);
                    break;
                case MagnificationFactor.FitPage:
                    Magnificator = " /Fit";
                    break;
                case MagnificationFactor.FitWidth:
                    Magnificator = " /FitH 0";
                    break;
                case MagnificationFactor.Percent_10:
                    Magnificator = GetZoomString(PageNumber, 0.1f);
                    break;
                case MagnificationFactor.Percent_25:
                    Magnificator = GetZoomString(PageNumber, 0.25f);
                    break;
                case MagnificationFactor.Percent_50:
                    Magnificator = GetZoomString(PageNumber, 0.5f);
                    break;
                case MagnificationFactor.Percent_75:
                    Magnificator = GetZoomString(PageNumber, 0.75f);
                    break;
                case MagnificationFactor.Percent_100:
                    Magnificator = GetZoomString(PageNumber, 1f);
                    break;
                case MagnificationFactor.Percent_125:
                    Magnificator = GetZoomString(PageNumber, 1.25f);
                    break;
                case MagnificationFactor.Percent_150:
                    Magnificator = GetZoomString(PageNumber, 1.5f);
                    break;
                case MagnificationFactor.Percent_200:
                    Magnificator = GetZoomString(PageNumber, 2f);
                    break;
                case MagnificationFactor.Percent_400:
                    Magnificator = GetZoomString(PageNumber, 4f);
                    break;
                case MagnificationFactor.Percent_800:
                    Magnificator = GetZoomString(PageNumber, 8f);
                    break;
            }

            string targetPage = ObjNumberRef(pagesRef[PageNumber - 1]);
            WriteLn(pdf, ExportUtils.StringFormat("/D [{0}{1}]", targetPage, Magnificator));
            WriteLn(pdf, ">>");
            WriteLn(pdf, "endobj");
        }

        private void AddPDFFooter()
        {
            if (!textInCurves)
            {
                foreach (ExportTTFFont font in fonts)
                {
                    WriteFont(font);
                }
            }

            pagesNumber = 1;
            xRef[0] = pdf.Position;
            WriteLn(pdf, ObjNumber(pagesNumber));

            WriteLn(pdf, "<<");
            WriteLn(pdf, "/Type /Pages");
            Write(pdf, "/Kids [");
            foreach (long page in pagesRef)
                Write(pdf, ObjNumberRef(page) + " ");
            WriteLn(pdf, "]");
            WriteLn(pdf, "/Count " + pagesRef.Count.ToString());
            WriteLn(pdf, ">>");
            WriteLn(pdf, "endobj");

            if (outline)
            {
                FastReport.Preview.Outline outlineTree = Report.PreparedPages.Outline;
                outlineNumber = UpdateXRef();
                this.outlineTree = new PDFOutlineNode();
                this.outlineTree.number = outlineNumber;
                BuildOutline(this.outlineTree, outlineTree.Xml);
                WriteOutline(this.outlineTree);
            }

            if (!isPdfX() && defaultZoom != MagnificationFactor.Default && defaultPage < Report.PreparedPages.Count)
                SetMagnificationFactor(defaultPage, defaultZoom);

            if (!isPdfX())
                WriteAnnots();

            if (isPdfX())
            {
                AddMetaDataPdfX();
                if (ColorSpace == PdfColorSpace.RGB)
                    AddColorProfile("PDFX", "pdfxprofile");
                else if (ColorSpace == PdfColorSpace.CMYK)
                    AddColorProfile("PDFX", "pdfcmykprofile");
            }

            if (isPdfA())
            {
                AddAttachments();
                AddStructure();
                AddMetaDataPdfA();
                if (ColorSpace == PdfColorSpace.RGB)
                    AddColorProfile("PDFA1", "pdfaprofile");
                else if (ColorSpace == PdfColorSpace.CMYK)
                    AddColorProfile("PDFA1", "pdfcmykprofile");
            }

            infoNumber = UpdateXRef();
            StringBuilder sb = new StringBuilder(1024);
            sb.AppendLine(ObjNumber(infoNumber));
            sb.Append("<<");
            sb.Append("/Title ");

            if (isPdfX() && String.IsNullOrEmpty(title))
                title = "FastReport.Net";

            PrepareString(title, encKey, encrypted, infoNumber, sb);
            sb.Append("/Author ");
            PrepareString(author, encKey, encrypted, infoNumber, sb);
            sb.Append("/Subject ");
            PrepareString(subject, encKey, encrypted, infoNumber, sb);
            sb.Append("/Keywords ");
            PrepareString(keywords, encKey, encrypted, infoNumber, sb);
            sb.Append("/Creator ");
            PrepareString(creator, encKey, encrypted, infoNumber, sb);
            sb.Append("/Producer ");
            PrepareString(producer, encKey, encrypted, infoNumber, sb);

            string s = "D:" + digitalSignCreationDate.ToString("yyyyMMddHHmmssZ");

            if (encrypted)
            {
                sb.Append("/CreationDate ");
                PrepareString(s, encKey, encrypted, infoNumber, sb);
                sb.Append("/ModDate ");
                PrepareString(s, encKey, encrypted, infoNumber, sb);
            }
            else
            {
                sb.AppendLine("/CreationDate (" + s + ")");
                sb.AppendLine("/ModDate (" + s + ")");
            }

            if (PdfCompliance == PdfStandard.PdfX_3)
            {
                sb.AppendLine("/GTS_PDFXVersion(PDF/X-3:2003)");
                sb.AppendLine("/Trapped /False");
            }
            else if (PdfCompliance == PdfStandard.PdfX_4)
            {
                sb.AppendLine("/GTS_PDFXVersion(PDF/X-4)");
                sb.AppendLine("/Trapped /False");
            }

            sb.AppendLine(">>");
            sb.AppendLine("endobj");

            Write(pdf, sb.ToString());

            //acroform
            long acroform = AddAcroForm();
            //end_acroform

            rootNumber = UpdateXRef();
            WriteLn(pdf, ObjNumber(rootNumber));
            WriteLn(pdf, "<<");

            WriteLn(pdf, "/Type /Catalog");
            WriteLn(pdf, "/Version /" + getPdfVersion());
            WriteLn(pdf, "/MarkInfo << /Marked true >>");


            if (acroform > 0)
                WriteLn(pdf, "/AcroForm " + ObjNumberRef(acroform).ToString());

            WriteLn(pdf, "/Pages " + ObjNumberRef(pagesNumber));
            if (!isPdfX())
            {
                if (defaultZoom != MagnificationFactor.Default)
                    WriteLn(pdf, "/OpenAction " + ObjNumberRef(actionDict));
            }

            if (showPrintDialog)
            {
                WriteLn(pdf, "/Names <</JavaScript " + ObjNumberRef(printDict) + ">>");
            }

            Write(pdf, "/PageMode ");

            if (outline)
            {
                WriteLn(pdf, "/UseOutlines");
                WriteLn(pdf, "/Outlines " + ObjNumberRef(outlineNumber));
            }
            else
            {
                WriteLn(pdf, "/UseNone");
            }

            if (isPdfA())
            {
                WriteLn(pdf, "/Metadata " + ObjNumberRef(metaFileId));
                if (embeddedFiles.Count > 0)
                {
                    Write(pdf, "/AF " + ObjNumberRef(attachmentsListId));
                    WriteLn(pdf, " /Names << /EmbeddedFiles " + ObjNumberRef(attachmentsNamesId) + " >>");
                }
                WriteLn(pdf, "/OutputIntents [ " + ObjNumberRef(colorProfileId) + " ]");
                WriteLn(pdf, "/StructTreeRoot " + ObjNumberRef(structId));
            }

            if (isPdfX())
            {
                WriteLn(pdf, "/Metadata " + ObjNumberRef(metaFileId));
                WriteLn(pdf, "/OutputIntents [ " + ObjNumberRef(colorProfileId) + " ]");
            }

            WriteLn(pdf, "/ViewerPreferences <<");

            if (displayDocTitle && !String.IsNullOrEmpty(title))
                WriteLn(pdf, "/DisplayDocTitle true");
            if (hideToolbar)
                WriteLn(pdf, "/HideToolbar true");
            if (hideMenubar)
                WriteLn(pdf, "/HideMenubar true");
            if (hideWindowUI)
                WriteLn(pdf, "/HideWindowUI true");
            if (fitWindow)
                WriteLn(pdf, "/FitWindow true");
            if (centerWindow)
                WriteLn(pdf, "/CenterWindow true");
            if (!printScaling)
                WriteLn(pdf, "/PrintScaling false"); // /None

            WriteLn(pdf, ">>");
            WriteLn(pdf, ">>");
            WriteLn(pdf, "endobj");
            startXRef = pdf.Position;
            WriteLn(pdf, "xref");
            WriteLn(pdf, "0 " + (xRef.Count + 1).ToString());
            WriteLn(pdf, "0000000000 65535 f");
            foreach (long xref in xRef)
                WriteLn(pdf, PrepXRefPos(xref) + " 00000 n");
            WriteLn(pdf, "trailer");
            WriteLn(pdf, "<<");
            WriteLn(pdf, "/Size " + (xRef.Count + 1).ToString());
            WriteLn(pdf, "/Root " + ObjNumberRef(rootNumber));
            WriteLn(pdf, "/Info " + ObjNumberRef(infoNumber));
            WriteLn(pdf, "/ID [<" + fileID + "><" + fileID + ">]");
            if (encrypted)
            {
                WriteLn(pdf, GetEncryptionDescriptor());
            }
            WriteLn(pdf, ">>");
            WriteLn(pdf, "startxref");
            WriteLn(pdf, startXRef.ToString());
            WriteLn(pdf, "%%EOF");

            if (isDigitalSignEnable && digitalSignCertificate != null && signatureDictIndicies.byteRangeIndex != 0)
            {
                digitalSignByteRange = new long[4] { 0, signatureDictIndicies.contentsIndex - 1, signatureDictIndicies.contentsIndex + 16384 + 1, pdf.Length - signatureDictIndicies.contentsIndex - 16384 - 1 };

                string byteRangeStr = string.Format("{0} {1} {2} {3} ]", digitalSignByteRange[0], digitalSignByteRange[1], digitalSignByteRange[2], digitalSignByteRange[3]);
                if (byteRangeStr.Length > 81)
                    throw new OverflowException("ByteRange was bigger than 80 bytes");

                pdf.Flush();

                pdf.Seek(signatureDictIndicies.byteRangeIndex, SeekOrigin.Begin);
                byte[] arr = Encoding.ASCII.GetBytes(byteRangeStr);
                pdf.Write(arr, 0, byteRangeStr.Length);

                AddSignature(digitalSignCertificate);
            }
        }

        private void AddEmbeddedFileItem(EmbeddedFile file)
        {
            long fileRef = UpdateXRef();
            WriteLn(pdf, ObjNumber(fileRef));
            Write(pdf, "<< /Params << /ModDate (D:" + file.ModDate.ToString("yyyyMMddHHmmss") + ")");
            Write(pdf, " /Size " + file.FileStream.Length.ToString());
            WriteLn(pdf, " >>");
            WriteLn(pdf, "/Subtype /" + file.MIME.Replace("/", "#2f"));
            WriteLn(pdf, "/Type /EmbeddedFile");
            WritePDFStream(pdf, file.FileStream, fileRef, compressed, encrypted, false, true, true);
            long fileRel = UpdateXRef();
            file.Xref = fileRel;
            WriteLn(pdf, ObjNumber(fileRel));
            WriteLn(pdf, "<< /AFRelationship /" + file.Relation.ToString());
            StringBuilder desc = new StringBuilder();
            PrepareString(file.Description, encKey, encrypted, fileRel, desc);
            WriteLn(pdf, "/Desc " + desc.ToString());
            Write(pdf, "/EF <<");
            Write(pdf, " /F " + ObjNumberRef(fileRef));
            Write(pdf, " /UF " + ObjNumberRef(fileRef));
            WriteLn(pdf, " >>");
            WriteLn(pdf, "/F (" + file.Name + ")");
            WriteLn(pdf, "/Type /Filespec");
            StringBuilder uf = new StringBuilder();
            StrToUTF16(file.Name, uf);
            WriteLn(pdf, "/UF <" + uf.ToString() + ">");
            WriteLn(pdf, ">>");
            WriteLn(pdf, "endobj");

        }

        private void AddAttachments()
        {
            if (embeddedFiles.Count > 0)
            {
                foreach (EmbeddedFile file in embeddedFiles)
                    AddEmbeddedFileItem(file);

                attachmentsNamesId = UpdateXRef();
                WriteLn(pdf, ObjNumber(attachmentsNamesId));
                Write(pdf, "<< /Names [");
                foreach (EmbeddedFile file in embeddedFiles)
                {
                    Write(pdf, " (" + file.Name + ") ");
                    Write(pdf, ObjNumberRef(file.Xref));
                }
                WriteLn(pdf, " ] >>");
                WriteLn(pdf, "endobj");

                attachmentsListId = UpdateXRef();
                WriteLn(pdf, ObjNumber(attachmentsListId));
                Write(pdf, "[ ");
                foreach (EmbeddedFile file in embeddedFiles)
                    Write(pdf, ObjNumberRef(file.Xref) + " ");
                WriteLn(pdf, "]");
                WriteLn(pdf, "endobj");
            }
        }

        private void AddStructure()
        {
            long roleMaps = UpdateXRef();
            WriteLn(pdf, ObjNumber(roleMaps));
            WriteLn(pdf, "<<\n/Footnote /Note\n/Endnote /Note\n/Textbox /Sect\n/Header /Sect\n/Footer /Sect\n/InlineShape /Sect\n/Annotation /Sect\n/Artifact /Sect\n/Workbook /Document\n/Worksheet /Part\n/Macrosheet /Part\n/Chartsheet /Part\n/Dialogsheet /Part\n/Slide /Part\n/Chart /Sect\n/Diagram /Figure\n>>\nendobj");

            structId = UpdateXRef();
            WriteLn(pdf, ObjNumber(structId));
            WriteLn(pdf, "<<\n/Type /StructTreeRoot");
            WriteLn(pdf, "/RoleMap " + ObjNumberRef(roleMaps));
            // /ParentTree /K /ParentTreeNextKey
            WriteLn(pdf, ">>\nendobj");
        }

        private void AddColorProfile(string GTS, string ICC)
        {
            string profileName = "default";

            // color profile stream
            long FColorProfileStreamId = UpdateXRef();
            WriteLn(pdf, ObjNumber(FColorProfileStreamId));
            WriteLn(pdf, "<<");
            WriteLn(pdf, "/N 3");

            if (ColorProfile != null)
            {
                string pname = ParseICCFile(ColorProfile);
                if (pname != null)
                {
                    profileName = pname.Trim('\0');
                    using (MemoryStream profileStream = new MemoryStream(ColorProfile))
                    {
                        WritePDFStream(pdf, profileStream, FColorProfileStreamId, compressed, encrypted, false, true);
                    }
                }
            }

            if (profileName == "default")
            {
                Assembly a = Assembly.GetExecutingAssembly();
                using (Stream stream = a.GetManifestResourceStream("FastReport.Resources.Pdf." + ICC + ".icc"))
                {
                    byte[] buf = new byte[stream.Length];
                    stream.Read(buf, 0, (int)stream.Length);
                    string pname = ParseICCFile(buf);
                    if (pname != null) profileName = pname.Trim('\0');
                    using (MemoryStream profileStream = new MemoryStream(buf))
                    {
                        WritePDFStream(pdf, profileStream, FColorProfileStreamId, compressed, encrypted, false, true);
                    }
                }
            }

            // color profile intent
            colorProfileId = UpdateXRef();
            WriteLn(pdf, ObjNumber(colorProfileId));
            WriteLn(pdf, "<<");
            WriteLn(pdf, "/Type /OutputIntent");
            WriteLn(pdf, "/S /GTS_" + GTS);
            WriteLn(pdf, ExportUtils.StringFormat("/OutputCondition ({0})", profileName));
            WriteLn(pdf, ExportUtils.StringFormat("/OutputConditionIdentifier ({0})", profileName));
            WriteLn(pdf, ExportUtils.StringFormat("/Info ({0})", profileName));
            WriteLn(pdf, "/DestOutputProfile " + ObjNumberRef(FColorProfileStreamId));
            WriteLn(pdf, ">>");
            WriteLn(pdf, "endobj");
        }

        private string ParseICCFile(byte[] file)
        {
            using (MemoryStream profileStream = new MemoryStream(file))
            {
                profileStream.Position = 128;
                byte[] temp = new byte[4];
                profileStream.Read(temp, 0, temp.Length);
                int count = temp[3];//limit to 255 tag count, for error avoid
                uint offset = 0;//if 0 then error
                uint size = 0;//size of desc
                byte[] desc = new byte[4] { 0x64, 0x65, 0x73, 0x63 };
                for (int i = 0; i < count; i++)
                {
                    //try to find desc tag
                    profileStream.Read(temp, 0, temp.Length);//read signature
                    bool flag_eq = true;
                    for (int j = 0; j < temp.Length; j++)
                        if (temp[j] != desc[j])
                            flag_eq = false;
                    profileStream.Read(temp, 0, temp.Length);//read offset
                    if (flag_eq)
                    {
                        offset = (uint)(temp[0] << 24) + (uint)(temp[1] << 16) + (uint)(temp[2] << 8) + (uint)(temp[3]);
                    }
                    profileStream.Read(temp, 0, temp.Length);//read lenght
                    if (flag_eq)
                    {
                        size = (uint)(temp[0] << 24) + (uint)(temp[1] << 16) + (uint)(temp[2] << 8) + (uint)(temp[3]);
                        break;
                    }
                }
                if (offset == 0)
                    return null;
                profileStream.Position = offset;
                profileStream.Read(temp, 0, temp.Length);//read signature
                for (int j = 0; j < temp.Length; j++)
                    if (temp[j] != desc[j])
                        return null;//test 2 for desc error
                profileStream.Read(temp, 0, temp.Length);//read 0
                profileStream.Read(temp, 0, temp.Length);//read lenght
                uint len = (uint)(temp[3]);
                byte[] result = new byte[len];

                profileStream.Read(result, 0, result.Length);
                return Encoding.ASCII.GetString(result);
            }
        }

        private void AddMetaDataPdfA()
        {
            PDFMetaData pmd = new PDFMetaData();
            pmd.Creator = Creator;
            pmd.Description = Subject;
            pmd.Keywords = Keywords;
            pmd.Title = Title;
            pmd.Producer = Producer;
            pmd.CreateDate = digitalSignCreationDate.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz");
            pmd.DocumentID = fileID;
            pmd.InstanceID = fileID;
            pmd.ZUGFeRD = zUGFeRDDescription;

            switch (PdfCompliance)
            {
                default:
                    pmd.Part = "2";
                    pmd.Conformance = "A";
                    break;
                case PdfStandard.PdfA_1a:
                    pmd.Part = "1";
                    pmd.Conformance = "A";
                    break;
                case PdfStandard.PdfA_2b:
                    pmd.Part = "2";
                    pmd.Conformance = "B";
                    break;
                case PdfStandard.PdfA_2u:
                    pmd.Part = "2";
                    pmd.Conformance = "U";
                    break;
                case PdfStandard.PdfA_3a:
                    pmd.Part = "3";
                    pmd.Conformance = "A";
                    break;
                case PdfStandard.PdfA_3b:
                    pmd.Part = "3";
                    pmd.Conformance = "B";
                    break;
            }

            if (PdfCompliance == PdfStandard.PdfA_1a)
            {
                Author = Creator;
            }

            metaFileId = UpdateXRef();
            WriteLn(pdf, ObjNumber(metaFileId));
            WriteLn(pdf, "<< /Type /Metadata /Subtype /XML ");
            using (MemoryStream metaStream = new MemoryStream())
            {
                ExportUtils.WriteLn(metaStream, pmd.MetaDataString);
                metaStream.Position = 0;
                WritePDFStream(pdf, metaStream, metaFileId, false, encrypted, false, true);
            }
        }

        private void AddMetaDataPdfX()
        {
            string metadata = null;

            if (PdfCompliance == PdfStandard.PdfX_3)
                metadata = "MetaDataX3";
            else if (PdfCompliance == PdfStandard.PdfX_4)
                metadata = "MetaDataX4";
            else
                throw new Exception("Error while adding metadata to PDF. Unknown PDF/X version: " + PdfCompliance.ToString());

            // to pass adobe acrobat compliance test
            if (PdfCompliance == PdfStandard.PdfX_4)
            {
                Author = Creator;
            }

            PDFMetaData pmd = new PDFMetaData(metadata);
            pmd.Creator = Creator;
            pmd.Description = Subject;
            pmd.Keywords = Keywords;
            pmd.Title = Title;
            pmd.Producer = Producer;
            pmd.CreateDate = SystemFake.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
            pmd.DocumentID = fileID;
            pmd.InstanceID = fileID;

            // to pass adobe acrobat compliance test
            if (PdfCompliance == PdfStandard.PdfX_4)
            {
                if (pmd.Title == null || pmd.Title.Trim() == "")
                    pmd.Title = "FastReport.Net";
            }

            metaFileId = UpdateXRef();
            WriteLn(pdf, ObjNumber(metaFileId));
            WriteLn(pdf, "<< /Type /Metadata /Subtype /XML ");
            using (MemoryStream metaStream = new MemoryStream())
            {
                ExportUtils.WriteLn(metaStream, pmd.MetaDataString);
                metaStream.Position = 0;
                WritePDFStream(pdf, metaStream, metaFileId, false, encrypted, false, true);
            }
        }

        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            return new MyRes("FileFilters").Get("PdfFile");
        }

        /// <inheritdoc/>
        protected override void Start()
        {
            base.Start();

            graphics = Report.MeasureGraphics;
            xRef = new List<long>();
            pagesRef = new List<long>();
            pagesTopMargins = new List<float>();
            pagesHeights = new List<float>();
            fonts = new List<ExportTTFFont>();
            hashList = new Dictionary<string, long>();
            pageAnnots = new StringBuilder();
            annots = new List<PDFExportAnnotation>();
            acroFormsRefs = new List<long>();
            acroFormsFonts = new List<int>();
            gradientHashSet = new Dictionary<HashableByteArray, long>();
            digitalSignCreationDate = SystemFake.DateTime.UtcNow;

            fileID = ExportUtils.GetID().Replace("-", "");
            if (!String.IsNullOrEmpty(ownerPassword) || !String.IsNullOrEmpty(userPassword))
            {
                encrypted = true;
                embeddingFonts = true;
                PrepareKeys();
            }

            if (isDigitalSignEnable)
            {
                if (digitalSignCertificate == null && !String.IsNullOrEmpty(digitalSignCertificatePath))
                {
                    if (String.IsNullOrEmpty(digitalSignCertificatePassword))
                        digitalSignCertificate = new System.Security.Cryptography.X509Certificates
                            .X509Certificate2(digitalSignCertificatePath);
                    else
                        digitalSignCertificate = new System.Security.Cryptography.X509Certificates
                            .X509Certificate2(digitalSignCertificatePath, digitalSignCertificatePassword);
                    isDigitalSignatureInvisible = true;
                }

                if (digitalSignCertificate != null && !digitalSignCertificate.HasPrivateKey)
                {
                    throw new InvalidOperationException("Certificate for signing is not valid, use another certificate or disable digital signature.");
                }

                if (digitalSignCertificate == null)
                {
                    //isDigitalSignEnable = false;
                    isDigitalSignatureInvisible = true;
                }
                else
                {
                    buffered = true;
                }
            }

            if (buffered)
                pdf = new MemoryStream();
            else
                pdf = Stream;

            if (Report.PreparedPages.Outline.Xml.Count == 0)
                outline = false;

            AddPDFHeader();

            if (showPrintDialog)
            {
                long FPrintDictJS = UpdateXRef();
                WriteLn(pdf, ObjNumber(FPrintDictJS));
                WriteLn(pdf, @"<</S/JavaScript/JS(this.print\({bUI:true,bSilent:false,bShrinkToFit:true}\);)>>");
                WriteLn(pdf, "endobj");
                printDict = UpdateXRef();
                WriteLn(pdf, ObjNumber(printDict));
                WriteLn(pdf, "<</Names[(0000000000000000) " + ObjNumberRef(FPrintDictJS) + "] >>");
                WriteLn(pdf, "endobj");
            }
        }

        /*/// <inheritdoc/>
        protected override void ExportPage(int pageNo)
        {
            using (ReportPage page = GetPage(pageNo))
            {
                AddPage(page);
                ObjectCollection allObjects = page.AllObjects;
                for (int i = 0; i < allObjects.Count; i++)
                {
                    ReportComponentBase c = allObjects[i] as ReportComponentBase;
                    if (c != null)
                    {
                        c.Dispose();
                        c = null;
                    }
                }
            }
            if (pageNo % 50 == 0)
                Application.DoEvents();

        }*/

        /// <summary>
        /// Begin exporting of page
        /// </summary>
        /// <param name="page"></param>
        protected override void ExportPageBegin(ReportPage page)
        {
            if (ExportMode == ExportType.Export)
                base.ExportPageBegin(page);

            //reset shadings
            pageShadings = new List<long>();
            pageAlphaShading = new List<long>();
            //end reset shadings

            pageFonts = new List<ExportTTFFont>();
            trasparentStroke = new List<string>();
            trasparentFill = new List<string>();
            picResList = new List<long>();
            //FAcroFormsAnnotsRefs = new List<long>();

            paperWidth = ExportUtils.GetPageWidth(page) * Units.Millimeters;
            paperHeight = ExportUtils.GetPageHeight(page) * Units.Millimeters;

            marginWoBottom = (ExportUtils.GetPageHeight(page) - page.TopMargin) * PDF_PAGE_DIVIDER;
            marginLeft = page.LeftMargin * PDF_PAGE_DIVIDER;

            pagesHeights.Add(marginWoBottom);
            pagesTopMargins.Add(page.TopMargin * PDF_PAGE_DIVIDER);

            contentsPos = 0;
            contentBuilder = new StringBuilder(65535);

            // page fill
            if (background)
                using (TextObject pageFill = new TextObject())
                {
                    pageFill.Fill = page.Fill;
                    pageFill.Left = -marginLeft / PDF_DIVIDER;
                    pageFill.Top = -page.TopMargin * PDF_PAGE_DIVIDER / PDF_DIVIDER;
                    pageFill.Width = ExportUtils.GetPageWidth(page) * PDF_PAGE_DIVIDER / PDF_DIVIDER;
                    pageFill.Height = ExportUtils.GetPageHeight(page) * PDF_PAGE_DIVIDER / PDF_DIVIDER;
                    AddTextObject(pageFill, false, contentBuilder);
                }

            // bitmap watermark on bottom
            if (page.Watermark.Enabled && !page.Watermark.ShowImageOnTop)
                AddBitmapWatermark(page, contentBuilder);

            // text watermark on bottom
            if (page.Watermark.Enabled && !page.Watermark.ShowTextOnTop)
                AddTextWatermark(page, contentBuilder);

            // page borders
            if (page.Border.Lines != BorderLines.None)
            {
                using (TextObject pageBorder = new TextObject())
                {
                    pageBorder.Border = page.Border;
                    pageBorder.Left = 0;
                    pageBorder.Top = 0;
                    pageBorder.Width = (ExportUtils.GetPageWidth(page) - page.LeftMargin - page.RightMargin) * PDF_PAGE_DIVIDER / PDF_DIVIDER;
                    pageBorder.Height = (ExportUtils.GetPageHeight(page) - page.TopMargin - page.BottomMargin) * PDF_PAGE_DIVIDER / PDF_DIVIDER;
                    AddTextObject(pageBorder, true, contentBuilder);
                }
            }
        }

        /// <summary>
        /// End exporting
        /// </summary>
        /// <param name="page"></param>
        protected override void ExportPageEnd(ReportPage page)
        {
            base.ExportPageEnd(page);

            // bitmap watermark on top
            if (page.Watermark.Enabled && page.Watermark.ShowImageOnTop)
                AddBitmapWatermark(page, contentBuilder);

            // text watermark on top
            if (page.Watermark.Enabled && page.Watermark.ShowTextOnTop)
                AddTextWatermark(page, contentBuilder);

            // write page
            contentsPos = UpdateXRef();
            WriteLn(pdf, ObjNumber(contentsPos));

            using (MemoryStream tempContentStream = new MemoryStream())
            {
                Write(tempContentStream, contentBuilder.ToString());
                tempContentStream.Position = 0;
                WritePDFStream(pdf, tempContentStream, contentsPos, compressed, encrypted, true, true);
            }

            if (!textInCurves)
            {
                if (pageFonts.Count > 0)
                    for (int i = 0; i < pageFonts.Count; i++)
                        if (!pageFonts[i].Saved)
                        {
                            pageFonts[i].Reference = UpdateXRef();
                            pageFonts[i].Saved = true;
                        }
                for (int i = 0; i < acroFormsFonts.Count; i++)
                {
                    if (!fonts[acroFormsFonts[i]].Saved)
                    {
                        fonts[acroFormsFonts[i]].Reference = UpdateXRef();
                        fonts[acroFormsFonts[i]].Saved = true;
                    }
                }
            }

            long PageNumber = UpdateXRef();

            if (isDigitalSignEnable && pagesRef.Count == Report.PreparedPages.Count - 1 && isDigitalSignatureInvisible)
            {
                signatureDictIndicies = AddSignatureDict(null);
                UpdateXRef(PageNumber);
            }

            pagesRef.Add(PageNumber);
            WriteLn(pdf, ObjNumber(PageNumber));
            StringBuilder sb = new StringBuilder(512);
            sb.AppendLine("<<").AppendLine("/Type /Page");
            sb.Append("/MediaBox [0 0 ").Append(FloatToString(ExportUtils.GetPageWidth(page) * PDF_PAGE_DIVIDER)).Append(" ");
            sb.Append(FloatToString(ExportUtils.GetPageHeight(page) * PDF_PAGE_DIVIDER)).AppendLine(" ]");
            //margins
            if (isPdfX())
                sb.Append("/TrimBox [")
                    .Append(FloatToString(page.LeftMargin * PDF_PAGE_DIVIDER)).Append(" ")
                    .Append(FloatToString(page.TopMargin * PDF_PAGE_DIVIDER)).Append(" ")
                    .Append(FloatToString(page.RightMargin * PDF_PAGE_DIVIDER)).Append(" ")
                    .Append(FloatToString(page.BottomMargin * PDF_PAGE_DIVIDER)).Append("]");

            sb.AppendLine("/Parent 1 0 R");
            if (!isPdfX() && (PdfCompliance != PdfStandard.PdfA_1a))
            {
                if (ColorSpace == PdfColorSpace.RGB)
                    sb.AppendLine("/Group << /Type /Group /S /Transparency /CS /DeviceRGB >>");
                else if (ColorSpace == PdfColorSpace.CMYK)
                    sb.AppendLine("/Group << /Type /Group /S /Transparency /CS /DeviceCMYK >>");
            }
            sb.AppendLine("/Resources << ");

            if (pageFonts.Count > 0)
            {
                sb.Append("/Font << ");
                foreach (ExportTTFFont font in pageFonts)
                    sb.Append(font.Name).Append(" ").Append(ObjNumberRef(font.Reference)).Append(" ");
                sb.AppendLine(" >>");
            }
            if (pageShadings != null && pageShadings.Count > 0)
            {
                sb.Append("/Shading <<");
                for (int i = 0; i < pageShadings.Count; i++)
                    sb.Append(" /sh").Append(i + 1).Append(" ").Append(ObjNumberRef(pageShadings[i]));
                //sb.Append(FPageShadings);
                sb.Append(" >>");
            }

            if (isPdfX())
            {
                sb.AppendLine("/ExtGState <<");
                for (int i = 0; i < trasparentStroke.Count; i++)
                    sb.Append("/GS").Append(i.ToString()).Append("S << /Type /ExtGState /ca ").Append(1).AppendLine(" >>");
                for (int i = 0; i < trasparentFill.Count; i++)
                    sb.Append("/GS").Append(i.ToString()).Append("F << /Type /ExtGState /CA ").Append(1).AppendLine(" >>");
                for (int i = 0; i < pageAlphaShading.Count; i++)
                    // /s6 << /ca 1 /Type /ExtGState /AIS false /SMask << /Type /Mask /G << >> /S	/Luminosity >> /CA 1 >>
                    sb.Append("/s").Append(i + 1).AppendLine(" << /ca 1 /Type /ExtGState /CA 1 >>");
                sb.AppendLine(">>");
            }
            else
            {
                sb.AppendLine("/ExtGState <<");
                for (int i = 0; i < trasparentStroke.Count; i++)
                    sb.Append("/GS").Append(i.ToString()).Append("S << /Type /ExtGState /ca ").Append(trasparentStroke[i]).AppendLine(" >>");
                for (int i = 0; i < trasparentFill.Count; i++)
                    sb.Append("/GS").Append(i.ToString()).Append("F << /Type /ExtGState /CA ").Append(trasparentFill[i]).AppendLine(" >>");
                for (int i = 0; i < pageAlphaShading.Count; i++)
                    // /s6 << /ca 1 /Type /ExtGState /AIS false /SMask << /Type /Mask /G << >> /S	/Luminosity >> /CA 1 >>
                    sb.Append("/s").Append(i + 1).Append(" << /ca 1 /Type /ExtGState /AIS false /SMask << /Type /Mask /G ")
                        .Append(ObjNumberRef(pageAlphaShading[i])).AppendLine(" /S /Luminosity >> /CA 1 >>");
                sb.AppendLine(">>");
            }

            if (picResList.Count > 0)
            {
                sb.Append("/XObject << ");
                foreach (long resIndex in picResList)
                    sb.Append("/Im").Append(resIndex.ToString()).Append(" ").Append(ObjNumberRef(resIndex)).Append(" ");
                sb.AppendLine(" >>");
            }

            sb.AppendLine("/ProcSet [/PDF /Text /ImageC ]");
            sb.AppendLine(">>");

            sb.Append("/Contents ").AppendLine(ObjNumberRef(contentsPos));
            if (pageAnnots.Length > 0)
            {
                sb.AppendLine(GetPageAnnots());
                pageAnnots.Length = 0;
            }

            sb.AppendLine(">>");
            sb.AppendLine("endobj");
            Write(pdf, sb.ToString());
        }

        /// <summary>
        /// Export of Band
        /// </summary>
        /// <param name="band"></param>
        protected override void ExportBand(Base band)
        {
            if (ExportMode == ExportType.Export)
                base.ExportBand(band);
            ExportObj(band);
            foreach (Base c in band.ForEachAllConvectedObjects(this))
            {
                ExportObj(c);
            }
        }

        private void ExportObj(Base c)
        {
            if (c is ReportComponentBase && (c as ReportComponentBase).Exportable)
            {
                ReportComponentBase obj = c as ReportComponentBase;
                if (obj is CellularTextObject)
                    obj = (obj as CellularTextObject).GetTable();
                if (obj is TableCell)
                    return;
#if DOTNET_4
                else if (PDFExportV4(obj)) { }
#endif
                else
                    if (obj is TableBase)
                {
                    //using (TableBase table = obj as TableBase)
                    {
                        if ((obj as TableBase).ColumnCount > 0 && (obj as TableBase).RowCount > 0)
                        {
                            StringBuilder tableBorder = new StringBuilder(64);
                            using (TextObject tableback = new TextObject())
                            {
                                tableback.Border = (obj as TableBase).Border;
                                tableback.Fill = (obj as TableBase).Fill;
                                tableback.FillColor = (obj as TableBase).FillColor;
                                tableback.Left = (obj as TableBase).AbsLeft;
                                tableback.Top = (obj as TableBase).AbsTop;
                                float tableWidth = 0;
                                float tableHeight = 0;
                                for (int i = 0; i < (obj as TableBase).ColumnCount; i++)
                                    tableWidth += (obj as TableBase).Columns[i].Width;// table[i, 0].Width;
                                for (int i = 0; i < (obj as TableBase).RowCount; i++)
                                    tableHeight += (obj as TableBase).Rows[i].Height;
                                tableback.Width = tableWidth;// (tableWidth < table.Width) ? tableWidth : table.Width;
                                tableback.Height = tableHeight;
                                AddTextObject(tableback, false, contentBuilder);
                                DrawPDFBorder(tableback.Border, tableback.AbsLeft, tableback.AbsTop, tableback.Width, tableback.Height, tableBorder);
                            }
                            // draw cells
                            AddTable((obj as TableBase), true, contentBuilder);
                            // draw cells border
                            AddTable((obj as TableBase), false, contentBuilder);
                            // draw table border
                            contentBuilder.Append(tableBorder);
                        }
                    }
                }
                else if (obj is TextObject)
                    AddTextObject(obj as TextObject, true, contentBuilder);
                else if (obj is BandBase)
                    AddBandObject(obj as BandBase, contentBuilder);
                else if (obj is LineObject)
                    AddLine(obj as LineObject, contentBuilder);
                else if (obj is ShapeObject)
                    AddShape(obj as ShapeObject, contentBuilder);
                else if (obj is RichObject)
                    AddPictureObject(obj, true, richTextQuality, contentBuilder);
                else if (obj is PolyLineObject)
                    AddPDFPolylineVector(obj as PolyLineObject, contentBuilder);
                //AddPolyLine(obj as PolyLineObject, contentBuilder);
                else if (obj is CheckBoxObject && (obj as CheckBoxObject).Editable && InteractiveForms)
                    AddCheckBoxField(obj as CheckBoxObject);
                else if (!(obj is HtmlObject))
                    AddPictureObject(obj, true, jpegQuality, contentBuilder);

                if (obj is DigitalSignatureObject && isDigitalSignEnable)
                {
                    AddSignatureDict(obj);
                    if (digitalSignCertificate == null)
                    {
                        isDigitalSignatureInvisible = false;
                    }
                }
            }
        }


        /// <inheritdoc/>
        protected override void Finish()
        {
            AddPDFFooter();
            foreach (ExportTTFFont fnt in fonts)
                fnt.Dispose();

            if (buffered)
                ((MemoryStream)pdf).WriteTo(Stream);
        }
        #endregion

        #region Public Methods



        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            base.Serialize(writer);

            // Options
            writer.WriteValue("PdfCompliance", PdfCompliance);
            writer.WriteBool("EmbeddingFonts", EmbeddingFonts);
            writer.WriteBool("Background", Background);
            writer.WriteBool("TextInCurves", TextInCurves);
            writer.WriteValue("ColorSpace", ColorSpace);
            writer.WriteBool("ImagesOriginalResolution", ImagesOriginalResolution);
            writer.WriteBool("PrintOptimized", PrintOptimized);
            writer.WriteBool("JpegCompression", JpegCompression);
            writer.WriteInt("JpegQuality", JpegQuality);
            writer.WriteBool("InteractiveForms", InteractiveForms);
            // end

            // Document Information
            writer.WriteStr("Title", Title);
            writer.WriteStr("Author", Author);
            writer.WriteStr("Subject", Subject);
            writer.WriteStr("Keywords", Keywords);
            writer.WriteStr("Creator", Creator);
            writer.WriteStr("Producer", Producer);
            // end

            // Security
            writer.WriteBool("AllowPrint", AllowPrint);
            writer.WriteBool("AllowModify", AllowModify);
            writer.WriteBool("AllowCopy", AllowCopy);
            writer.WriteBool("AllowAnnotate", AllowAnnotate);
            // end

            // Viewer
            writer.WriteBool("AutoPrint", ShowPrintDialog);
            writer.WriteBool("HideToolbar", HideToolbar);
            writer.WriteBool("HideMenubar", HideMenubar);
            writer.WriteBool("HideWindowUI", HideWindowUI);
            writer.WriteBool("FitWindow", FitWindow);
            writer.WriteBool("CenterWindow", CenterWindow);
            writer.WriteBool("PrintScaling", PrintScaling);
            writer.WriteBool("Outline", Outline);
            writer.WriteValue("DefaultZoom", DefaultZoom);
            // end

            // Curves
            writer.WriteValue("GradientInterpolationPoints", GradientInterpolationPoints);
            writer.WriteValue("GradientQuality", GradientQuality);
            writer.WriteValue("CurvesInterpolation", CurvesInterpolation);
            writer.WriteValue("CurvesInterpolationText", CurvesInterpolationText);
            writer.WriteBool("SvgAsPicture", SvgAsPicture);

            writer.WriteBool("SaveDigitalSignCertificatePassword", SaveDigitalSignCertificatePassword);
            writer.WriteBool("IsDigitalSignEnable", IsDigitalSignEnable);

            writer.WriteValue("DigitalSignCertificatePath", DigitalSignCertificatePath);
            writer.WriteValue("DigitalSignLocation", DigitalSignLocation);
            writer.WriteValue("DigitalSignContactInfo", DigitalSignContactInfo);
            writer.WriteValue("DigitalSignReason", DigitalSignReason);

            if (SaveDigitalSignCertificatePassword)
            {
                writer.WriteValue("DigitalSignCertificatePassword", digitalSignCertificatePassword);
            }
            else
            {
                writer.WriteValue("DigitalSignCertificatePassword", null);
            }

            // end
        }

        /// <summary>
        /// Add an embedded XML file (only for PDF/A-3 standard).
        /// </summary>
        /// <param name="name">File name</param>
        /// <param name="description">Description</param>
        /// <param name="modDate">Modification date</param>
        /// <param name="fileStream">File stream</param>
        public void AddEmbeddedXML(string name, string description, DateTime modDate, Stream fileStream)
        {
            AddEmbeddedXML(name, description, modDate, fileStream, ZUGFeRD_ConformanceLevel.BASIC);
        }

        /// <summary>
        /// Add an embedded XML file (only for PDF/A-3 standard).
        /// </summary>
        /// <param name="name">File name</param>
        /// <param name="description">Description</param>
        /// <param name="modDate">Modification date</param>
        /// <param name="fileStream">File stream</param>
        /// <param name="ZUGFeRDLevel">ZUGFeRD Conformance Level</param>
        public void AddEmbeddedXML(string name, string description, DateTime modDate, Stream fileStream, ZUGFeRD_ConformanceLevel ZUGFeRDLevel)
        {
            zUGFeRDDescription = ExportUtils.StringFormat("<rdf:Description xmlns:zf=\"urn:ferd:pdfa:CrossIndustryDocument:invoice:1p0#\" rdf:about=\"\" zf:ConformanceLevel=\"{0}\" zf:DocumentFileName=\"{1}\" zf:DocumentType=\"INVOICE\" zf:Version=\"1.0\"/>", ZUGFeRDLevel.ToString(), name);
            AddEmbeddedFile(name, description, modDate, EmbeddedRelation.Alternative, "text/xml", fileStream);
        }

        /// <summary>
        /// Add an embedded file (only for PDF/A-3 standard).
        /// </summary>
        /// <param name="name">File name</param>
        /// <param name="description">Description</param>
        /// <param name="modDate">Modification date</param>
        /// <param name="relation">Relation type</param>
        /// <param name="mime">MIME type</param>
        /// <param name="fileStream">File stream</param>
        public void AddEmbeddedFile(string name, string description, DateTime modDate, EmbeddedRelation relation, string mime, Stream fileStream)
        {
            EmbeddedFile file = new EmbeddedFile();
            file.Name = name;
            file.Description = description;
            file.ModDate = modDate;
            file.Relation = relation;
            file.MIME = mime;
            file.FileStream = fileStream;
            embeddedFiles.Add(file);
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PDFExport"/> class.
        /// </summary>
        public PDFExport()
        {
            numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberGroupSeparator = String.Empty;
            numberFormatInfo.NumberDecimalSeparator = ".";
            embeddedFiles = new List<EmbeddedFile>();
            exportMode = ExportType.Export;
        }

        #endregion
    }
}