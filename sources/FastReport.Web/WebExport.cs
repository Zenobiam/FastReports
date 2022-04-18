using FastReport.Export.Csv;
using FastReport.Export.Dbf;
using FastReport.Export.Html;
using FastReport.Export.Mht;
using FastReport.Export.Odf;
using FastReport.Export.OoXML;
using FastReport.Export.Pdf;
using FastReport.Export.RichText;
using FastReport.Export.Text;
using FastReport.Export.Xml;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FastReport.Web
{
    public partial class WebReport : WebControl, INamingContainer
    {
        #region RTF format

        /// <summary>
        /// Switches a visibility of RTF export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowRtfExport
        {
            get { return Prop.ShowRtfExport; }
            set { Prop.ShowRtfExport = value; }
        }

        /// <summary>
        /// Gets or sets the quality of Jpeg images in RTF file.
        /// </summary>
        /// <remarks>
        /// Default value is 90. This property will be used if you select Jpeg
        /// in the <see cref="RtfImageFormat"/> property.
        /// </remarks>
        [DefaultValue(90)]
        [Category("Rtf Format")]
        [Browsable(true)]
        public int RtfJpegQuality
        {
            get { return Prop.RtfJpegQuality; }
            set { Prop.RtfJpegQuality = value; }
        }

        /// <summary>
        /// Gets or sets the image format that will be used to save pictures in RTF file.
        /// </summary>
        /// <remarks>
        /// Default value is <b>Metafile</b>. This format is better for exporting such objects as
        /// <b>MSChartObject</b> and <b>ShapeObject</b>.
        /// </remarks>
        [DefaultValue(RTFImageFormat.Metafile)]
        [Category("Rtf Format")]
        [Browsable(true)]
        public RTFImageFormat RtfImageFormat
        {
            get { return Prop.RtfImageFormat; }
            set { Prop.RtfImageFormat = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that pictures are enabled.
        /// </summary>
        [DefaultValue(true)]
        [Category("Rtf Format")]
        [Browsable(true)]
        public bool RtfPictures
        {
            get { return Prop.RtfPictures; }
            set { Prop.RtfPictures = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that page breaks are enabled.
        /// </summary>
        [DefaultValue(true)]
        [Category("Rtf Format")]
        [Browsable(true)]
        public bool RtfPageBreaks
        {
            get { return Prop.RtfPageBreaks; }
            set { Prop.RtfPageBreaks = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the wysiwyg mode should be used
        /// for better results.
        /// </summary>
        [DefaultValue(true)]
        [Category("Rtf Format")]
        [Browsable(true)]
        public bool RtfWysiwyg
        {
            get { return Prop.RtfWysiwyg; }
            set { Prop.RtfWysiwyg = value; }
        }

        /// <summary>
        /// Gets or sets the creator of the document.
        /// </summary>
        [DefaultValue("FastReport")]
        [Category("Rtf Format")]
        [Browsable(true)]
        public string RtfCreator
        {
            get { return Prop.RtfCreator; }
            set { Prop.RtfCreator = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the rows in the resulting table
        /// should calculate its height automatically.
        /// </summary>
        [DefaultValue(false)]
        [Category("Rtf Format")]
        [Browsable(true)]
        public bool RtfAutoSize
        {
            get { return Prop.RtfAutoSize; }
            set { Prop.RtfAutoSize = value; }
        }

        #endregion RTF format

        #region MHT format

        /// <summary>
        /// Switches a visibility of MHT (web-archive) export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowMhtExport
        {
            get { return Prop.ShowMhtExport; }
            set { Prop.ShowMhtExport = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that pictures are enabled.
        /// </summary>
        [DefaultValue(true)]
        [Category("Mht Format")]
        [Browsable(true)]
        public bool MhtPictures
        {
            get { return Prop.MhtPictures; }
            set { Prop.MhtPictures = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the wysiwyg mode should be used
        /// for better results.
        /// </summary>
        [DefaultValue(true)]
        [Category("Mht Format")]
        [Browsable(true)]
        public bool MhtWysiwyg
        {
            get { return Prop.MhtWysiwyg; }
            set { Prop.MhtWysiwyg = value; }
        }

        #endregion MHT format

        #region ODS format

        /// <summary>
        /// Switches a visibility of Open Office Spreadsheet (ODS) export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowOdsExport
        {
            get { return Prop.ShowOdsExport; }
            set { Prop.ShowOdsExport = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that page breaks are enabled.
        /// </summary>
        [DefaultValue(true)]
        [Category("Ods Format")]
        [Browsable(true)]
        public bool OdsPageBreaks
        {
            get { return Prop.OdsPageBreaks; }
            set { Prop.OdsPageBreaks = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the wysiwyg mode should be used
        /// for better results.
        /// </summary>
        [DefaultValue(true)]
        [Category("Ods Format")]
        [Browsable(true)]
        public bool OdsWysiwyg
        {
            get { return Prop.OdsWysiwyg; }
            set { Prop.OdsWysiwyg = value; }
        }

        /// <summary>
        /// Gets or sets the creator of the document.
        /// </summary>
        [DefaultValue("FastReport")]
        [Category("Ods Format")]
        [Browsable(true)]
        public string OdsCreator
        {
            get { return Prop.OdsCreator; }
            set { Prop.OdsCreator = value; }
        }

        #endregion ODS format

        #region ODT format

        /// <summary>
        /// Switches a visibility of Open Office Text (ODT) export in toolbar
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowOdtExport
        {
            get { return Prop.ShowOdtExport; }
            set { Prop.ShowOdtExport = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that page breaks are enabled.
        /// </summary>
        [DefaultValue(true)]
        [Category("Odt Format")]
        [Browsable(true)]
        public bool OdtPageBreaks
        {
            get { return Prop.OdtPageBreaks; }
            set { Prop.OdtPageBreaks = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the wysiwyg mode should be used
        /// for better results.
        /// </summary>
        [DefaultValue(true)]
        [Category("Odt Format")]
        [Browsable(true)]
        public bool OdtWysiwyg
        {
            get { return Prop.OdtWysiwyg; }
            set { Prop.OdtWysiwyg = value; }
        }

        /// <summary>
        /// Gets or sets the creator of the document.
        /// </summary>
        [DefaultValue("FastReport")]
        [Category("Odt Format")]
        [Browsable(true)]
        public string OdtCreator
        {
            get { return Prop.OdtCreator; }
            set { Prop.OdtCreator = value; }
        }

        #endregion ODT format

        #region XPS format

        /// <summary>
        /// Switches a visibility of XPS export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowXpsExport
        {
            get { return Prop.ShowXpsExport; }
            set { Prop.ShowXpsExport = value; }
        }

        #endregion XPS format

        #region DBF format

        /// <summary>
        /// Switches a visibility of DBF export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowDbfExport
        {
            get { return Prop.ShowDbfExport; }
            set { Prop.ShowDbfExport = value; }
        }

        #endregion DBF format

        #region Word2007 format

        /// <summary>
        /// Switches a visibility of Word 2007 export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowWord2007Export
        {
            get { return Prop.ShowWord2007Export; }
            set { Prop.ShowWord2007Export = value; }
        }

        /// <summary>
        /// Enable or disable a matrix view of Word 2007 document.
        /// </summary>
        [DefaultValue(true)]
        [Category("Word 2007 Format")]
        [Browsable(true)]
        public bool DocxMatrixBased
        {
            get { return Prop.DocxMatrixBased; }
            set { Prop.DocxMatrixBased = value; }
        }

        /// <summary>
        /// Enable or disable the WYSIWYG for Word 2007 document.
        /// </summary>
        [DefaultValue(true)]
        [Category("Word 2007 Format")]
        [Browsable(true)]
        public bool DocxWysiwyg
        {
            get { return Prop.DocxWysiwyg; }
            set { Prop.DocxWysiwyg = value; }
        }

        /// <summary>
        /// Enable or disable a paragraph view of Word 2007 document.
        /// </summary>
        [DefaultValue(false)]
        [Category("Word 2007 Format")]
        [Browsable(true)]
        public bool DocxParagraphBased
        {
            get { return Prop.DocxParagraphBased; }
            set { Prop.DocxParagraphBased = value; }
        }

        /// <summary>
        /// Enable or disable the print optimized images in Word 2007 document.
        /// </summary>
        [DefaultValue(false)]
        [Category("Word 2007 Format")]
        [Browsable(true)]
        public bool DocxPrintOptimized
        {
            get { return Prop.DocxPrintOptimized; }
            set { Prop.DocxPrintOptimized = value; }
        }

        /// <summary>
        /// Gets or sets a value of RowHeightIs Word 2007 document.
        /// </summary>
        [DefaultValue("")]
        [Category("Word 2007 Format")]
        [Browsable(true)]
        public string DocxRowHeightIs
        {
            get { return Prop.DocxRowHeightIs; }
            set { Prop.DocxRowHeightIs = value; }
        }

        #endregion Word2007 format

        #region Excel2007 format

        /// <summary>
        /// Switches a visibility of Excel 2007 export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowExcel2007Export
        {
            get { return Prop.ShowExcel2007Export; }
            set { Prop.ShowExcel2007Export = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that page breaks are enabled.
        /// </summary>
        [DefaultValue(false)]
        [Category("Excel 2007 Format")]
        [Browsable(true)]
        public bool XlsxPageBreaks
        {
            get { return Prop.XlsxPageBreaks; }
            set { Prop.XlsxPageBreaks = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that table without breaks are enabled.
        /// </summary>
        [DefaultValue(false)]
        [Category("Excel 2007 Format")]
        [Browsable(true)]
        public bool XlsxSeamless
        {
            get { return Prop.XlsxSeamless; }
            set { Prop.XlsxSeamless = value; }
        }

        /// <summary>
        /// Enable or disable the print optimized images in Excel 2007 document.
        /// </summary>
        [DefaultValue(false)]
        [Category("Excel 2007 Format")]
        [Browsable(true)]
        public bool XlsxPrintOptimized
        {
            get { return Prop.XlsxPrintOptimized; }
            set { Prop.XlsxPrintOptimized = value; }
        }

        /// <summary>
        /// Enable or disable the Print Fit in Excel 2007 document.
        /// </summary>
        [DefaultValue(false)]
        [Category("Excel 2007 Format")]
        [Browsable(true)]
        public bool XlsxPrintFitPage
        {
            get { return Prop.XlsxPrintFitPage; }
            set { Prop.XlsxPrintFitPage = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the wysiwyg mode should be used
        /// for better results.
        /// </summary>
        [DefaultValue(true)]
        [Category("Excel 2007 Format")]
        [Browsable(true)]
        public bool XlsxWysiwyg
        {
            get { return Prop.XlsxWysiwyg; }
            set { Prop.XlsxWysiwyg = value; }
        }

        /// <summary>
        /// Enable or disable an exporting data without any header/group bands.
        /// </summary>
        [DefaultValue(true)]
        [Category("Excel 2007 Format")]
        [Browsable(true)]
        public bool XlsxDataOnly
        {
            get { return Prop.XlsxDataOnly; }
            set { Prop.XlsxDataOnly = value; }
        }

        #endregion Excel2007 format

        #region PowerPoint2007 format

        /// <summary>
        /// Switches a visibility of PowerPoint 2007 export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowPowerPoint2007Export
        {
            get { return Prop.ShowPowerPoint2007Export; }
            set { Prop.ShowPowerPoint2007Export = value; }
        }

        /// <summary>
        /// Gets or sets an image format that will be used to save pictures in PowerPoint file.
        /// </summary>
        [DefaultValue(PptImageFormat.Png)]
        [Category("PowerPoint 2007 Format")]
        [Browsable(true)]
        public PptImageFormat PptxImageFormat
        {
            get { return Prop.PptxImageFormat; }
            set { Prop.PptxImageFormat = value; }
        }

        #endregion PowerPoint2007 format

        #region XML format

        /// <summary>
        /// Switches a visibility of XML (Excel) export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowXmlExcelExport
        {
            get { return Prop.ShowXmlExcelExport; }
            set { Prop.ShowXmlExcelExport = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that page breaks are enabled.
        /// </summary>
        [DefaultValue(true)]
        [Category("Xml Excel Format")]
        [Browsable(true)]
        public bool XmlExcelPageBreaks
        {
            get { return Prop.XmlExcelPageBreaks; }
            set { Prop.XmlExcelPageBreaks = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the wysiwyg mode should be used
        /// for better results.
        /// </summary>
        [DefaultValue(true)]
        [Category("Xml Excel Format")]
        [Browsable(true)]
        public bool XmlExcelWysiwyg
        {
            get { return Prop.XmlExcelWysiwyg; }
            set { Prop.XmlExcelWysiwyg = value; }
        }

        /// <summary>
        /// Enable or disable an exporting data without any header/group bands.
        /// </summary>
        [DefaultValue(false)]
        [Category("Xml Excel Format")]
        [Browsable(true)]
        public bool XmlExcelDataOnly
        {
            get { return Prop.XmlExcelDataOnly; }
            set { Prop.XmlExcelDataOnly = value; }
        }

        #endregion XML format

        #region PDF format

        /// <summary>
        /// Switches a visibility of PDF (Adobe Acrobat) export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowPdfExport
        {
            get { return Prop.ShowPdfExport; }
            set { Prop.ShowPdfExport = value; }
        }

        /// <summary>
        /// Enable or disable an embedding the TrueType fonts.
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfEmbeddingFonts
        {
            get { return Prop.PdfEmbeddingFonts; }
            set { Prop.PdfEmbeddingFonts = value; }
        }

        /// <summary>
        /// Enable or disable an export text in curves.
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfTextInCurves
        {
            get { return Prop.PdfTextInCurves; }
            set { Prop.PdfTextInCurves = value; }
        }

        /// <summary>
        /// Enable or disable an exporting of the background in PDF.
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfBackground
        {
            get { return Prop.PdfBackground; }
            set { Prop.PdfBackground = value; }
        }

        /// <summary>
        /// Enable or disable the Intercative Forms inside PDF.
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfInteractiveForms
        {
            get { return Prop.PdfInteractiveForms; }
            set { Prop.PdfInteractiveForms = value; }
        }

        /// <summary>
        /// Enable or disable an optimization the images for printing.
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfPrintOptimized
        {
            get { return Prop.PdfPrintOptimized; }
            set { Prop.PdfPrintOptimized = value; }
        }

        /// <summary>
        /// Enable or disable a document's Outline.
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfOutline
        {
            get { return Prop.PdfOutline; }
            set { Prop.PdfOutline = value; }
        }

        /// <summary>
        /// Enable or disable a displaying document's title.
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfDisplayDocTitle
        {
            get { return Prop.PdfDisplayDocTitle; }
            set { Prop.PdfDisplayDocTitle = value; }
        }

        /// <summary>
        /// Enable or disable a hiding the toolbar.
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfHideToolbar
        {
            get { return Prop.PdfHideToolbar; }
            set { Prop.PdfHideToolbar = value; }
        }

        /// <summary>
        /// Enable or disable a hiding the menu's bar.
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfHideMenubar
        {
            get { return Prop.PdfHideMenubar; }
            set { Prop.PdfHideMenubar = value; }
        }

        /// <summary>
        /// Enable or disable a hiding the Windows UI.
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfHideWindowUI
        {
            get { return Prop.PdfHideWindowUI; }
            set { Prop.PdfHideWindowUI = value; }
        }

        /// <summary>
        /// Enable or disable a fitting the window.
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfFitWindow
        {
            get { return Prop.PdfFitWindow; }
            set { Prop.PdfFitWindow = value; }
        }

        /// <summary>
        /// Enable or disable a centering the window.
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfCenterWindow
        {
            get { return Prop.PdfCenterWindow; }
            set { Prop.PdfCenterWindow = value; }
        }

        /// <summary>
        /// Enable or disable a scaling the page for shrink to printable area.
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfPrintScaling
        {
            get { return Prop.PdfPrintScaling; }
            set { Prop.PdfPrintScaling = value; }
        }

        /// <summary>
        /// Sets the Title of the document.
        /// </summary>
        [DefaultValue("")]
        [Category("Pdf Format")]
        [Browsable(true)]
        public string PdfTitle
        {
            get { return Prop.PdfTitle; }
            set { Prop.PdfTitle = value; }
        }

        /// <summary>
        /// Sets the Author of the document.
        /// </summary>
        [DefaultValue("")]
        [Category("Pdf Format")]
        [Browsable(true)]
        public string PdfAuthor
        {
            get { return Prop.PdfAuthor; }
            set { Prop.PdfAuthor = value; }
        }

        /// <summary>
        /// Sets the Subject of the document.
        /// </summary>
        [DefaultValue("")]
        [Category("Pdf Format")]
        [Browsable(true)]
        public string PdfSubject
        {
            get { return Prop.PdfSubject; }
            set { Prop.PdfSubject = value; }
        }

        /// <summary>
        /// Sets the Keywords of the document.
        /// </summary>
        [DefaultValue("")]
        [Category("Pdf Format")]
        [Browsable(true)]
        public string PdfKeywords
        {
            get { return Prop.PdfKeywords; }
            set { Prop.PdfKeywords = value; }
        }

        /// <summary>
        /// Sets the Creator of the document.
        /// </summary>
        [DefaultValue("FastReport")]
        [Category("Pdf Format")]
        [Browsable(true)]
        public string PdfCreator
        {
            get { return Prop.PdfCreator; }
            set { Prop.PdfCreator = value; }
        }

        /// <summary>
        /// Sets the Producer of the document.
        /// </summary>
        [DefaultValue("FastReport.NET")]
        [Category("Pdf Format")]
        [Browsable(true)]
        public string PdfProducer
        {
            get { return Prop.PdfProducer; }
            set { Prop.PdfProducer = value; }
        }

        /// <summary>
        /// Sets the users password.
        /// </summary>
        [DefaultValue("")]
        [Category("Pdf Format")]
        [Browsable(true)]
        public string PdfUserPassword
        {
            get { return Prop.PdfUserPassword; }
            set { Prop.PdfUserPassword = value; }
        }

        /// <summary>
        /// Sets the owners password.
        /// </summary>
        [DefaultValue("")]
        [Category("Pdf Format")]
        [Browsable(true)]
        public string PdfOwnerPassword
        {
            get { return Prop.PdfOwnerPassword; }
            set { Prop.PdfOwnerPassword = value; }
        }

        /// <summary>
        /// Enable or disable a printing in protected document.
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfAllowPrint
        {
            get { return Prop.PdfAllowPrint; }
            set { Prop.PdfAllowPrint = value; }
        }

        /// <summary>
        /// Enable or disable a modifying in protected document.
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfAllowModify
        {
            get { return Prop.PdfAllowModify; }
            set { Prop.PdfAllowModify = value; }
        }

        /// <summary>
        /// Enable or disable a copying in protected document.
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfAllowCopy
        {
            get { return Prop.PdfAllowCopy; }
            set { Prop.PdfAllowCopy = value; }
        }

        /// <summary>
        /// Enable or disable an annotating in protected document.
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfAllowAnnotate
        {
            get { return Prop.PdfAllowAnnotate; }
            set { Prop.PdfAllowAnnotate = value; }
        }

        /// <summary>
        /// Enable or disable the PDF/A document.
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfA
        {
            get { return Prop.PdfA; }
            set { Prop.PdfA = value; }
        }

        /// <summary>
        /// Enable or disable a showing of Print Dialog.
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfShowPrintDialog
        {
            get { return Prop.PdfShowPrintDialog; }
            set { Prop.PdfShowPrintDialog = value; }
        }

        /// <summary>
        /// Enable or disable the Images Original Resolution.
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfImagesOriginalResolution
        {
            get { return Prop.PdfImagesOriginalResolution; }
            set { Prop.PdfImagesOriginalResolution = value; }
        }


        /// <summary>
        /// Enable or disable the Jpeg Compression.
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfJpegCompression
        {
            get { return Prop.PdfJpegCompression; }
            set { Prop.PdfJpegCompression = value; }
        }


        /// <summary>
        /// Select a color space (RGB or CMYK)
        /// </summary>
        [DefaultValue(typeof(PDFExport.PdfColorSpace), "RGB")]
        [Category("Pdf Format")]
        [Browsable(true)]
        public PDFExport.PdfColorSpace PdfColorSpace
        {
            get { return Prop.PdfColorSpace; }
            set { Prop.PdfColorSpace = value; }
        }

        #endregion PDF format

        #region CSV format

        /// <summary>
        /// Switch visibility the CSV (comma separated values) export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowCsvExport
        {
            get { return Prop.ShowCsvExport; }
            set { Prop.ShowCsvExport = value; }
        }

        /// <summary>
        /// Gets or sets the cells separator.
        /// </summary>
        [DefaultValue(";")]
        [Category("Csv Format")]
        [Browsable(true)]
        public string CsvSeparator
        {
            get { return Prop.CsvSeparator; }
            set { Prop.CsvSeparator = value; }
        }

        /// <summary>
        /// Enable or disable an exporting data without any header/group bands.
        /// </summary>
        [DefaultValue(false)]
        [Category("Csv Format")]
        [Browsable(true)]
        public bool CsvDataOnly
        {
            get { return Prop.CsvDataOnly; }
            set { Prop.CsvDataOnly = value; }
        }

        #endregion CSV format

        #region Prepared report

        /// <summary>
        /// Switch a visibility of prepared report export in toolbar
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowPreparedReport
        {
            get { return Prop.ShowPreparedReport; }
            set { Prop.ShowPreparedReport = value; }
        }

        #endregion Prepared report

        #region Text format

        /// <summary>
        /// Switch a visibility of text (plain text) export in toolbar
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowTextExport
        {
            get { return Prop.ShowTextExport; }
            set { Prop.ShowTextExport = value; }
        }

        /// <summary>
        /// Enable or disable an exporting data without any header/group bands.
        /// </summary>
        [DefaultValue(false)]
        [Category("Text Format")]
        [Browsable(true)]
        public bool TextDataOnly
        {
            get { return Prop.TextDataOnly; }
            set { Prop.TextDataOnly = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that page breaks are enabled.
        /// </summary>
        [DefaultValue(true)]
        [Category("Text Format")]
        [Browsable(true)]
        public bool TextPageBreaks
        {
            get { return Prop.TextPageBreaks; }
            set { Prop.TextPageBreaks = value; }
        }

        /// <summary>
        /// Enable or disable the frames in text file.
        /// </summary>
        [DefaultValue(true)]
        [Category("Text Format")]
        [Browsable(true)]
        public bool TextAllowFrames
        {
            get { return Prop.TextAllowFrames; }
            set { Prop.TextAllowFrames = value; }
        }

        /// <summary>
        /// Enable or disable the simple (non graphic) frames in text file.
        /// </summary>
        [DefaultValue(true)]
        [Category("Text Format")]
        [Browsable(true)]
        public bool TextSimpleFrames
        {
            get { return Prop.TextSimpleFrames; }
            set { Prop.TextSimpleFrames = value; }
        }

        /// <summary>
        /// Enable or disable an empty lines in text file.
        /// </summary>
        [DefaultValue(false)]
        [Category("Text Format")]
        [Browsable(true)]
        public bool TextEmptyLines
        {
            get { return Prop.TextEmptyLines; }
            set { Prop.TextEmptyLines = value; }
        }

        #endregion Text format

        #region All exports

        private void DoProcessReport()
        {
            StringBuilder sb = new StringBuilder();
            ReportProcess(sb, this.Context);
        }

        private string GetExportFileName(string format)
        {
            string s = String.Concat(
                Path.GetFileNameWithoutExtension(
                Report.FileName.Length == 0 ? WebUtils.ReportPrefix : Report.FileName),
                ".",
                format);
            return s;
        }

        private void ResponseExport(WebExportItem exportItem, bool displayInline, HttpContext context)
        {
            WebReport webReport = new WebReport();
            WebReportCache exportcache = new WebReportCache(context);
            object webReportObject = exportcache.GetObject(exportItem.ReportID, webReport);
            if (webReportObject != null)
            {
                webReport = webReportObject as WebReport;
                if (WebUtils.SetupResponse(webReport, context))
                {
                    exportItem.FileName = GetExportFileName(exportItem.Format);
                    string guid = WebUtils.GetGUID();
                    exportcache.PutObject(guid, exportItem);
                    string url = string.Format("~/{0}?{1}={2}&displayinline={3}", WebUtils.HandlerFileName, WebUtils.ConstID, guid, displayInline);
                    context.Response.Redirect(url, false);
                }
            }
            else
            {
                // 500
            }
        }

        private void ResponseExport(WebExportItem ExportItem, HttpContext context)
        {
            ResponseExport(ExportItem, false, context);
        }

        /// <summary>
        /// Exports in CSV format.
        /// </summary>
        public void ExportCsv()
        {
            ExportCsv(this.Context);
        }

        /// <summary>
        /// Exports in CSV format.
        /// </summary>
        public void ExportCsv(HttpContext context)
        {
            if (State != ReportState.Done)
                DoProcessReport();

            if (State == ReportState.Done && TotalPages > 0)
            {
                WebExportItem exportItem = new WebExportItem();
                exportItem.ReportID = ReportGuid;
                CSVExport csvExport = new CSVExport();
                csvExport.OpenAfterExport = false;
                // set csv export properties
                csvExport.Separator = CsvSeparator;
                csvExport.DataOnly = CsvDataOnly;
                using (MemoryStream ms = new MemoryStream())
                {
                    csvExport.Export(Report, ms);
                    exportItem.File = ms.ToArray();
                }
                exportItem.Format = "csv";
                exportItem.ContentType = "text/x-csv";
                ResponseExport(exportItem, context);
            }
        }

        /// <summary>
        /// Exports in Text format.
        /// </summary>
        public void ExportText()
        {
            ExportText(this.Context);
        }

        /// <summary>
        /// Exports in Text format.
        /// </summary>
        public void ExportText(HttpContext context)
        {
            if (State != ReportState.Done)
                DoProcessReport();

            if (State == ReportState.Done && TotalPages > 0)
            {
                WebExportItem exportItem = new WebExportItem();
                exportItem.ReportID = ReportGuid;
                TextExport textExport = new TextExport();
                textExport.OpenAfterExport = false;
                // set text export properties
                textExport.AvoidDataLoss = true;
                textExport.DataOnly = TextDataOnly;
                textExport.PageBreaks = TextPageBreaks;
                textExport.Frames = TextAllowFrames;
                textExport.TextFrames = TextSimpleFrames;
                textExport.EmptyLines = TextEmptyLines;
                using (MemoryStream ms = new MemoryStream())
                {
                    textExport.Export(Report, ms);
                    exportItem.File = ms.ToArray();
                }
                exportItem.Format = "txt";
                exportItem.ContentType = "text/plain";
                ResponseExport(exportItem, context);
            }
        }

        /// <summary>
        /// Exports in DBF format.
        /// </summary>
        public void ExportDbf()
        {
            ExportDbf(this.Context);
        }

        /// <summary>
        /// Exports in DBF format.
        /// </summary>
        public void ExportDbf(HttpContext context)
        {
            if (State != ReportState.Done)
                DoProcessReport();

            if (State == ReportState.Done && TotalPages > 0)
            {
                WebExportItem exportItem = new WebExportItem();
                exportItem.ReportID = ReportGuid;
                DBFExport dbfExport = new DBFExport();
                dbfExport.OpenAfterExport = false;
                // set text export properties
                dbfExport.DataOnly = true;
                using (MemoryStream ms = new MemoryStream())
                {
                    dbfExport.Export(Report, ms);
                    exportItem.File = ms.ToArray();
                }
                exportItem.Format = "dbf";
                exportItem.ContentType = "application/dbf";
                ResponseExport(exportItem, context);
            }
        }

        /// <summary>
        /// Exports in PDF format.
        /// </summary>
        public void ExportPdf()
        {
            ExportPdf(this.Context, false, PdfShowPrintDialog, false);
        }

        /// <summary>
        /// Exports in PDF format.
        /// </summary>
        public void ExportPdf(bool displayInline)
        {
            ExportPdf(this.Context, displayInline, PdfShowPrintDialog, false);
        }

        /// <summary>
        /// Exports in PDF format.
        /// </summary>
        public void ExportPdf(HttpContext context)
        {
            ExportPdf(context, false, PdfShowPrintDialog, false);
        }

        /// <summary>
        /// Exports in PDF format inline.
        /// </summary>
        public void ExportPdf(HttpContext context, bool displayInline, bool showPrintDialog, bool print)
        {
            if (State != ReportState.Done)
                DoProcessReport();

            if (State == ReportState.Done)
            {
                WebExportItem exportItem = new WebExportItem();
                exportItem.ReportID = this.ReportGuid;
                PDFExport pdfExport = new PDFExport();
                pdfExport.OpenAfterExport = false;
                // set pdf export properties
                pdfExport.EmbeddingFonts = PdfEmbeddingFonts;
                pdfExport.TextInCurves = PdfTextInCurves;
                pdfExport.Background = PdfBackground;
                pdfExport.PrintOptimized = PdfPrintOptimized;
                pdfExport.Title = PdfTitle;
                pdfExport.Author = PdfAuthor;
                pdfExport.Subject = PdfSubject;
                pdfExport.Keywords = PdfKeywords;
                pdfExport.Creator = PdfCreator;
                pdfExport.Producer = PdfProducer;
                pdfExport.Outline = PdfOutline;
                pdfExport.DisplayDocTitle = PdfDisplayDocTitle;
                pdfExport.HideToolbar = PdfHideToolbar;
                pdfExport.HideMenubar = PdfHideMenubar;
                pdfExport.HideWindowUI = PdfHideWindowUI;
                pdfExport.FitWindow = PdfFitWindow;
                pdfExport.CenterWindow = PdfCenterWindow;
                pdfExport.PrintScaling = PdfPrintScaling;
                pdfExport.UserPassword = PdfUserPassword;
                pdfExport.OwnerPassword = PdfOwnerPassword;
                pdfExport.AllowPrint = PdfAllowPrint;
                pdfExport.AllowCopy = PdfAllowCopy;
                pdfExport.AllowModify = PdfAllowModify;
                pdfExport.AllowAnnotate = PdfAllowAnnotate;
                pdfExport.PdfCompliance = PdfA ? PDFExport.PdfStandard.PdfA_2a : PDFExport.PdfStandard.None;
                pdfExport.ShowPrintDialog = showPrintDialog;
                pdfExport.InteractiveForms = PdfInteractiveForms;
                pdfExport.ExportMode = print ? PDFExport.ExportType.WebPrint : PDFExport.ExportType.Export;
                pdfExport.ImagesOriginalResolution = PdfImagesOriginalResolution;
                pdfExport.JpegCompression = PdfJpegCompression;
                pdfExport.ColorSpace = PdfColorSpace;
                using (MemoryStream ms = new MemoryStream())
                {
                    pdfExport.Export(Report, ms);
                    exportItem.File = ms.ToArray();
                }
                exportItem.Format = "pdf";
                exportItem.ContentType = "application/pdf";
                ResponseExport(exportItem, displayInline, context);
            }
        }

        /// <summary>
        /// Exports in HTML format inline.
        /// </summary>
        public void ExportHtml(HttpContext context, bool displayInline, bool print)
        {
            if (State != ReportState.Done)
                DoProcessReport();

            if (State == ReportState.Done)
            {
                WebExportItem exportItem = new WebExportItem();
                exportItem.ReportID = ReportGuid;
                HTMLExport htmlExport = new HTMLExport();
                htmlExport.CustomDraw += CustomDraw;
                htmlExport.OpenAfterExport = false;
                // set html export properties
                htmlExport.Navigator = false;
                htmlExport.Layers = Layers;
                htmlExport.SinglePage = true;
                htmlExport.Pictures = Pictures;
                htmlExport.Print = print;
                htmlExport.Preview = true;
                htmlExport.SubFolder = false;
                htmlExport.EmbedPictures = EmbedPictures;
                htmlExport.EnableVectorObjects = !WebUtils.IsIE8(context); // don't draw svg barcodes for IE8
                htmlExport.WebImagePrefix = String.Concat(WebUtils.GetAppRoot(context, WebUtils.HandlerFileName), "?", WebUtils.PicsPrefix);
                htmlExport.ExportMode = htmlExport.Print ? HTMLExport.ExportType.WebPrint : HTMLExport.ExportType.Export;
                using (MemoryStream ms = new MemoryStream())
                {
                    htmlExport.Export(Report, ms);
                    exportItem.File = ms.ToArray();
                }

                if (htmlExport.PrintPageData != null)
                {
                    WebReportCache cache = new WebReportCache(this.Context);
                    // add all pictures in cache
                    for (int i = 0; i < htmlExport.PrintPageData.Pictures.Count; i++)
                    {
                        Stream stream = htmlExport.PrintPageData.Pictures[i];
                        byte[] image = new byte[stream.Length];
                        stream.Position = 0;
                        int n = stream.Read(image, 0, (int)stream.Length);
                        string picGuid = htmlExport.PrintPageData.Guids[i];
                        cache.PutObject(picGuid, image);
                    }

                    // cleanup
                    for (int i = 0; i < htmlExport.PrintPageData.Pictures.Count; i++)
                    {
                        Stream stream = htmlExport.PrintPageData.Pictures[i];
                        stream.Dispose();
                        stream = null;
                    }
                    htmlExport.PrintPageData.Pictures.Clear();
                    htmlExport.PrintPageData.Guids.Clear();
                }

                exportItem.Format = "html";
                exportItem.ContentType = "text/html";

                ResponseExport(exportItem, displayInline, context);
            }
        }

        /// <summary>
        /// Exports in RTF format.
        /// </summary>
        public void ExportRtf()
        {
            ExportRtf(this.Context);
        }

        /// <summary>
        /// Exports in RTF format.
        /// </summary>
        public void ExportRtf(HttpContext context)
        {
            if (State != ReportState.Done)
                DoProcessReport();

            if (State == ReportState.Done && TotalPages > 0)
            {
                WebExportItem exportItem = new WebExportItem();
                exportItem.ReportID = ReportGuid;
                RTFExport rtfExport = new RTFExport();
                rtfExport.OpenAfterExport = false;
                // set Rtf export properties
                rtfExport.JpegQuality = RtfJpegQuality;
                rtfExport.ImageFormat = RtfImageFormat;
                rtfExport.Pictures = RtfPictures;
                rtfExport.PageBreaks = RtfPageBreaks;
                rtfExport.Wysiwyg = RtfWysiwyg;
                rtfExport.Creator = RtfCreator;
                rtfExport.AutoSize = RtfAutoSize;
                using (MemoryStream ms = new MemoryStream())
                {
                    rtfExport.Export(Report, ms);
                    exportItem.File = ms.ToArray();
                }
                exportItem.Format = "rtf";
                exportItem.ContentType = "application/rtf";
                ResponseExport(exportItem, context);
            }
        }

        /// <summary>
        /// Exports in MHT format.
        /// </summary>
        public void ExportMht()
        {
            ExportMht(this.Context);
        }

        /// <summary>
        /// Exports in MHT format.
        /// </summary>
        public void ExportMht(HttpContext context)
        {
            if (State != ReportState.Done)
                DoProcessReport();

            if (State == ReportState.Done && TotalPages > 0)
            {
                WebExportItem exportItem = new WebExportItem();
                exportItem.ReportID = ReportGuid;
                MHTExport mhtExport = new MHTExport();
                mhtExport.OpenAfterExport = false;
                // set MHT export properties
                mhtExport.Pictures = MhtPictures;
                mhtExport.Wysiwyg = MhtWysiwyg;
                using (MemoryStream ms = new MemoryStream())
                {
                    mhtExport.Export(Report, ms);
                    exportItem.File = ms.ToArray();
                }
                exportItem.Format = "mht";
                exportItem.ContentType = "message/rfc822";
                ResponseExport(exportItem, context);
            }
        }

        /// <summary>
        /// Exports in XML (Excel 2003) format.
        /// </summary>
        public void ExportXmlExcel()
        {
            ExportXmlExcel(this.Context);
        }

        /// <summary>
        /// Exports in XML (Excel 2003) format.
        /// </summary>
        public void ExportXmlExcel(HttpContext context)
        {
            if (State != ReportState.Done)
                DoProcessReport();

            if (State == ReportState.Done && TotalPages > 0)
            {
                WebExportItem exportItem = new WebExportItem();
                exportItem.ReportID = ReportGuid;
                XMLExport xmlExport = new XMLExport();
                xmlExport.OpenAfterExport = false;
                // set xml export properties
                xmlExport.PageBreaks = XmlExcelPageBreaks;
                xmlExport.Wysiwyg = XmlExcelWysiwyg;
                xmlExport.DataOnly = XmlExcelDataOnly;
                using (MemoryStream ms = new MemoryStream())
                {
                    xmlExport.Export(Report, ms);
                    exportItem.File = ms.ToArray();
                }
                exportItem.Format = "xls";
                exportItem.ContentType = "application/vnd.ms-excel";
                ResponseExport(exportItem, context);
            }
        }

        /// <summary>
        /// Exports in Open Office Spreadsheet format.
        /// </summary>
        public void ExportOds()
        {
            ExportOds(this.Context);
        }

        /// <summary>
        /// Exports in Open Office Spreadsheet format.
        /// </summary>
        public void ExportOds(HttpContext context)
        {
            if (State != ReportState.Done)
                DoProcessReport();

            if (State == ReportState.Done && TotalPages > 0)
            {
                WebExportItem exportItem = new WebExportItem();
                exportItem.ReportID = ReportGuid;
                ODSExport odsExport = new ODSExport();
                odsExport.OpenAfterExport = false;
                // set ODS export properties
                odsExport.Creator = OdsCreator;
                odsExport.Wysiwyg = OdsWysiwyg;
                odsExport.PageBreaks = OdsPageBreaks;
                using (MemoryStream ms = new MemoryStream())
                {
                    odsExport.Export(Report, ms);
                    exportItem.File = ms.ToArray();
                }
                exportItem.Format = "ods";
                exportItem.ContentType = "application/x-oleobject";
                ResponseExport(exportItem, context);
            }
        }

        /// <summary>
        /// Exports in Open Office Text format.
        /// </summary>
        public void ExportOdt()
        {
            ExportOdt(this.Context);
        }

        /// <summary>
        /// Exports in Open Office Text format.
        /// </summary>
        public void ExportOdt(HttpContext context)
        {
            if (State != ReportState.Done)
                DoProcessReport();

            if (State == ReportState.Done && TotalPages > 0)
            {
                WebExportItem exportItem = new WebExportItem();
                exportItem.ReportID = ReportGuid;
                ODTExport odtExport = new ODTExport();
                odtExport.OpenAfterExport = false;
                // set ODT export properties
                odtExport.Creator = OdtCreator;
                odtExport.Wysiwyg = OdtWysiwyg;
                odtExport.PageBreaks = OdtPageBreaks;
                using (MemoryStream ms = new MemoryStream())
                {
                    odtExport.Export(Report, ms);
                    exportItem.File = ms.ToArray();
                }
                exportItem.Format = "odt";
                exportItem.ContentType = "application/x-oleobject";
                ResponseExport(exportItem, context);
            }
        }

        /// <summary>
        /// Exports in XPS format.
        /// </summary>
        public void ExportXps()
        {
            ExportXps(this.Context);
        }

        /// <summary>
        /// Exports in XPS format.
        /// </summary>
        public void ExportXps(HttpContext context)
        {
            if (State != ReportState.Done)
                DoProcessReport();

            if (State == ReportState.Done && TotalPages > 0)
            {
                WebExportItem exportItem = new WebExportItem();
                exportItem.ReportID = ReportGuid;
                XPSExport xpsExport = new XPSExport();
                xpsExport.OpenAfterExport = false;
                using (MemoryStream ms = new MemoryStream())
                {
                    xpsExport.Export(Report, ms);
                    exportItem.File = ms.ToArray();
                }
                exportItem.Format = "xps";
                exportItem.ContentType = "application/vnd.ms-xpsdocument";
                ResponseExport(exportItem, context);
            }
        }

        /// <summary>
        /// Exports in Excel 2007 format.
        /// </summary>
        public void ExportExcel2007()
        {
            ExportExcel2007(this.Context);
        }

        /// <summary>
        /// Exports in Excel 2007 format.
        /// </summary>
        public void ExportExcel2007(HttpContext context)
        {
            if (State != ReportState.Done)
                DoProcessReport();

            if (State == ReportState.Done && TotalPages > 0)
            {
                WebExportItem exportItem = new WebExportItem();
                exportItem.ReportID = ReportGuid;
                Excel2007Export xlsxExport = new Excel2007Export();
                xlsxExport.OpenAfterExport = false;
                // set Excel 2007 export properties
                xlsxExport.PageBreaks = XlsxPageBreaks;
                xlsxExport.Seamless = XlsxSeamless;
                xlsxExport.DataOnly = XlsxDataOnly;
                xlsxExport.PrintOptimized = XlsxPrintOptimized;
                xlsxExport.PrintFit = XlsxPrintFitPage ?
                    Excel2007Export.PrintFitMode.FitSheetOnOnePage : Excel2007Export.PrintFitMode.NoScaling;
                xlsxExport.Wysiwyg = XlsxWysiwyg;
                using (MemoryStream ms = new MemoryStream())
                {
                    xlsxExport.Export(Report, ms);
                    exportItem.File = ms.ToArray();
                }
                exportItem.Format = "xlsx";
                exportItem.ContentType = "application/vnd.ms-excel";
                ResponseExport(exportItem, context);
            }
        }

        /// <summary>
        /// Exports in Word 2007 format.
        /// </summary>
        public void ExportWord2007()
        {
            ExportWord2007(this.Context);
        }

        /// <summary>
        /// Exports in Word 2007 format.
        /// </summary>
        public void ExportWord2007(HttpContext context)
        {
            if (State != ReportState.Done)
                DoProcessReport();

            if (State == ReportState.Done && TotalPages > 0)
            {
                WebExportItem exportItem = new WebExportItem();
                exportItem.ReportID = ReportGuid;
                Word2007Export docxExport = new Word2007Export();
                docxExport.OpenAfterExport = false;
                docxExport.Wysiwyg = DocxWysiwyg;
                // set Word 2007 export properties
                docxExport.MatrixBased = DocxMatrixBased;
                docxExport.ParagraphBased = DocxParagraphBased;
                docxExport.PrintOptimized = DocxPrintOptimized;
                if (!String.IsNullOrEmpty(DocxRowHeightIs) && DocxRowHeightIs.ToLower() == "min")
                    docxExport.RowHeight = Word2007Export.RowHeightType.Minimum;
                else
                    docxExport.RowHeight = Word2007Export.RowHeightType.Exactly;
                using (MemoryStream ms = new MemoryStream())
                {
                    docxExport.Export(Report, ms);
                    exportItem.File = ms.ToArray();
                }
                exportItem.Format = "docx";
                exportItem.ContentType = "application/vnd.ms-word";
                ResponseExport(exportItem, context);
            }
        }

        /// <summary>
        /// Exports in PowerPoint 2007 format.
        /// </summary>
        public void ExportPowerPoint2007()
        {
            ExportPowerPoint2007(this.Context);
        }

        /// <summary>
        /// Exports in PowerPoint 2007 format.
        /// </summary>
        public void ExportPowerPoint2007(HttpContext context)
        {
            if (State != ReportState.Done)
                DoProcessReport();

            if (State == ReportState.Done && TotalPages > 0)
            {
                WebExportItem exportItem = new WebExportItem();
                exportItem.ReportID = ReportGuid;
                PowerPoint2007Export pptxExport = new PowerPoint2007Export();
                pptxExport.OpenAfterExport = false;
                // set Power Point 2007 properties
                pptxExport.ImageFormat = PptxImageFormat;
                using (MemoryStream ms = new MemoryStream())
                {
                    pptxExport.Export(Report, ms);
                    exportItem.File = ms.ToArray();
                }
                exportItem.Format = "pptx";
                exportItem.ContentType = "application/vnd.ms-powerpoint ";
                ResponseExport(exportItem, context);
            }
        }

        /// <summary>
        /// Exports in prepared report.
        /// </summary>
        public void ExportPrepared()
        {
            ExportPrepared(this.Context);
        }

        /// <summary>
        /// Exports in prepared report.
        /// </summary>
        public void ExportPrepared(HttpContext context)
        {
            if (State != ReportState.Done)
                DoProcessReport();

            if (State == ReportState.Done && TotalPages > 0)
            {
                WebExportItem exportItem = new WebExportItem();
                exportItem.ReportID = ReportGuid;
                using (MemoryStream ms = new MemoryStream())
                {
                    Report.SavePrepared(ms);
                    exportItem.File = ms.ToArray();
                }
                exportItem.Format = "fpx";
                exportItem.ContentType = "application/octet-stream";
                ResponseExport(exportItem, context);
            }
        }

        /// <summary>
        /// Prints in Adobe Acrobat.
        /// </summary>
        public void PrintPdf()
        {
            ExportPdf(this.Context, true, true, true);
        }

        /// <summary>
        /// Prints in Adobe Acrobat.
        /// </summary>
        public void PrintPdf(HttpContext context)
        {
            ExportPdf(context, true, true, true);
        }

        /// <summary>
        /// Prints in Adobe Acrobat.
        /// </summary>
        public void PrintPdf(bool showPrintDialog)
        {
            ExportPdf(this.Context, true, showPrintDialog, true);
        }

        /// <summary>
        /// Prints in Adobe Acrobat.
        /// </summary>
        public void PrintPdf(HttpContext context, bool showPrintDialog)
        {
            ExportPdf(context, true, showPrintDialog, true);
        }

        /// <summary>
        /// Prints in browser.
        /// </summary>
        public void PrintHtml()
        {
            ExportHtml(this.Context, true, true);
        }

        /// <summary>
        /// Prints in browser.
        /// </summary>
        public void PrintHtml(HttpContext context)
        {
            ExportHtml(context, true, true);
        }

        #endregion All exports
    }
}