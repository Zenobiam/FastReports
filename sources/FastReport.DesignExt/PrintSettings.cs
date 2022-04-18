using System;
using System.ComponentModel;
using System.Drawing.Printing;
using FastReport.Utils;
using System.Drawing;

namespace FastReport
{
  /// <summary>
  /// Specifies the report printing mode.
  /// </summary>
  public enum PrintMode 
  { 
    /// <summary>
    /// Specifies the default printing mode. One report page produces 
    /// one printed paper sheet of the same size.
    /// </summary>
    Default, 
    
    /// <summary>
    /// Specifies the split mode. Big report page produces several smaller paper sheets.
    /// Use this mode to print A3 report on A4 printer.
    /// </summary>
    Split, 
    
    /// <summary>
    /// Specifies the scale mode. One or several report pages produce one bigger paper sheet.
    /// Use this mode to print A5 report on A4 printer. 
    /// </summary>
    Scale 
  }

  /// <summary>
  /// Specifies the number of report pages printed on one paper sheet.
  /// </summary>
  public enum PagesOnSheet 
  { 
    /// <summary>
    /// Specifies one report page per sheet.
    /// </summary>
    One,

    /// <summary>
    /// Specifies two report pages per sheet.
    /// </summary>
    Two,

    /// <summary>
    /// Specifies four report pages per sheet.
    /// </summary>
    Four,

    /// <summary>
    /// Specifies eight report pages per sheet.
    /// </summary>
    Eight
  }
  
  /// <summary>
  /// Specifies the pages to print.
  /// </summary>
  public enum PrintPages 
  { 
    /// <summary>
    /// Print all report pages.
    /// </summary>
    All, 
    
    /// <summary>
    /// Print odd pages only.
    /// </summary>
    Odd, 
    
    /// <summary>
    /// Print even pages only.
    /// </summary>
    Even
  }
  
  /// <summary>
  /// This class contains the printer settings. 
  /// It is used in the <see cref="Report.PrintSettings"/> property.
  /// </summary>
  /// <remarks>
  /// Typical use of this class is to setup a printer properties without using the "Print"
  /// dialog. In this case, setup necessary properties and turn off the dialog via the 
  /// <see cref="ShowDialog"/> property.
  /// </remarks>
  [TypeConverter(typeof(FastReport.TypeConverters.FRExpandableObjectConverter))]
  public class PrintSettings : IDisposable
  {
    #region Fields
    private string printer;
    private bool savePrinterWithReport;
    private bool printToFile;
    private string printToFileName;
    private PageRange pageRange;
    private string pageNumbers;
    private int copies;
    private bool collate;
    private PrintPages printPages;
    private bool reverse;
    private Duplex duplex;
    private int paperSource;
    private PrintMode printMode;
    private float printOnSheetWidth;
    private float printOnSheetHeight;
    private int printOnSheetRawPaperSize;
    private PagesOnSheet pagesOnSheet;
    private string[] copyNames;
    private bool showDialog;
    private IGraphics measureGraphics;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the printer name.
    /// </summary>
    [TypeConverterAttribute(typeof(PrinterConverter))]
    public string Printer
    {
      get { return printer; }
      set 
      { 
        printer = value;
        DisposeMeasureGraphics();
      }
    }

    /// <summary>
    /// Gets or sets a value indicating that the printer name should be saved in a report file.
    /// </summary>
    /// <remarks>
    /// If this property is set to <b>true</b>, the printer name will be saved in a report file.
    /// Next time when you open the report, the printer will be automatically selected.
    /// </remarks>
    [DefaultValue(false)]
    public bool SavePrinterWithReport
    {
      get { return savePrinterWithReport; }
      set { savePrinterWithReport = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating that the printing output should be send 
    /// to a file instead of a printer.
    /// </summary>
    /// <remarks>
    /// Also set the <see cref="PrintToFileName"/> property.
    /// </remarks>
    [DefaultValue(false)]
    public bool PrintToFile
    {
      get { return printToFile; }
      set { printToFile = value; }
    }

    /// <summary>
    /// The name of a file to print the report to.
    /// </summary>
    /// <remarks>
    /// This property is used if <see cref="PrintToFile"/> property is <b>true</b>.
    /// </remarks>
    public string PrintToFileName
    {
      get { return printToFileName; }
      set { printToFileName = value; }
    }

    /// <summary>
    /// Gets or sets a value specifies the page range to print.
    /// </summary>
    [DefaultValue(PageRange.All)]
    public PageRange PageRange
    {
      get { return pageRange; }
      set { pageRange = value; }
    }

    /// <summary>
    /// Gets or sets the page number(s) to print.
    /// </summary>
    /// <remarks>
    /// This property is used if <see cref="PageRange"/> property is set to <b>PageNumbers</b>.
    /// You can specify the page numbers, separated by commas, or the page ranges.
    /// For example: "1,3,5-12".
    /// </remarks>
    public string PageNumbers
    {
      get { return pageNumbers; }
      set { pageNumbers = value; }
    }

    /// <summary>
    /// Gets or sets the number of copies to print.
    /// </summary>
    [DefaultValue(1)]
    public int Copies
    {
      get { return copies; }
      set { copies = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the printed document should be collated.
    /// </summary>
    [DefaultValue(true)]
    public bool Collate
    {
      get { return collate; }
      set { collate = value; }
    }

    /// <summary>
    /// Gets or sets a value specifies the pages to print.
    /// </summary>
    [DefaultValue(PrintPages.All)]
    public PrintPages PrintPages
    {
      get { return printPages; }
      set { printPages = value; }
    }

    /// <summary>
    /// Gets or sets a value determines whether to print pages in reverse order.
    /// </summary>
    [DefaultValue(false)]
    public bool Reverse
    {
      get { return reverse; }
      set { reverse = value; }
    }

    /// <summary>
    /// Gets or sets the duplex mode.
    /// </summary>
    [DefaultValue(Duplex.Default)]
    public Duplex Duplex
    {
      get { return duplex; }
      set { duplex = value; }
    }

    /// <summary>
    /// Gets or sets the paper source.
    /// </summary>
    /// <remarks>
    /// This property corresponds to the RAW source number. Default value is 7 which 
    /// corresponds to DMBIN_AUTO.
    /// </remarks>
    [DefaultValue(7)]
    public int PaperSource
    {
      get { return paperSource; }
      set { paperSource = value; }
    }

    /// <summary>
    /// Gets or sets the print mode.
    /// </summary>
    /// <remarks>
    /// See the <see cref="FastReport.PrintMode"/> enumeration for details. If you use 
    /// the mode other than <b>Default</b>, you must specify the sheet size in the
    /// <see cref="PrintOnSheetWidth"/>, <see cref="PrintOnSheetHeight"/> properties.
    /// </remarks>
    [DefaultValue(PrintMode.Default)]
    public PrintMode PrintMode
    {
      get { return printMode; }
      set { printMode = value; }
    }

    /// <summary>
    /// Gets or sets the width of the paper sheet to print on.
    /// </summary>
    /// <remarks>
    /// This property is used if the <see cref="PrintMode"/> property is not <b>Default</b>.
    /// Specify the paper width in millimeters.
    /// </remarks>
    public float PrintOnSheetWidth
    {
      get { return printOnSheetWidth; }
      set { printOnSheetWidth = value; }
    }

    /// <summary>
    /// Gets or sets the height of the paper sheet to print on.
    /// </summary>
    /// <remarks>
    /// This property is used if the <see cref="PrintMode"/> property is not <b>Default</b>.
    /// Specify the paper height in millimeters.
    /// </remarks>
    public float PrintOnSheetHeight
    {
      get { return printOnSheetHeight; }
      set { printOnSheetHeight = value; }
    }

    /// <summary>
    /// Gets or sets the raw index of a paper size.
    /// </summary>
    [DefaultValue(0)]
    public int PrintOnSheetRawPaperSize
    {
      get { return printOnSheetRawPaperSize; }
      set { printOnSheetRawPaperSize = value; }
    }

    /// <summary>
    /// Gets or sets the number of pages per printed sheet.
    /// </summary>
    /// <remarks>
    /// This property is used if the <see cref="PrintMode"/> property is set to <b>Scale</b>.
    /// </remarks>
    [DefaultValue(PagesOnSheet.One)]
    public PagesOnSheet PagesOnSheet
    {
      get { return pagesOnSheet; }
      set { pagesOnSheet = value; }
    }

    /// <summary>
    /// Gets or sets an array of printed copy names, such as "Original", "Copy", etc.
    /// </summary>
    public string[] CopyNames
    {
      get { return copyNames; }
      set { copyNames = value; }
    }

    /// <summary>
    /// Specifies whether to display the "Print" dialog.
    /// </summary>
    [DefaultValue(true)]
    public bool ShowDialog
    {
      get { return showDialog; }
      set { showDialog = value; }
    }

    internal IGraphics MeasureGraphics
    {
      get
      {
        if (measureGraphics == null)
        {
          PrinterSettings printer = new PrinterSettings();
          try
          {
            if (!String.IsNullOrEmpty(Printer))
              printer.PrinterName = Printer;
          }
          catch
          {
          }  
          
          try
          {
            measureGraphics = new GdiGraphics(printer.CreateMeasurementGraphics(), false);
          }
          catch
          {
            measureGraphics = null;
          }  
        }
        return measureGraphics;
      }
    }
    #endregion
    
    #region Private Methods
    private void DisposeMeasureGraphics()
    {
      if (measureGraphics != null)
        measureGraphics.Dispose();
      measureGraphics = null;
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public void Dispose()
    {
      DisposeMeasureGraphics();
    }

    /// <summary>
    /// Assigns values from another source.
    /// </summary>
    /// <param name="source">Source to assign from.</param>
    public void Assign(PrintSettings source)
    {
      Printer = source.Printer;
      SavePrinterWithReport = source.SavePrinterWithReport;
      PrintToFile = source.PrintToFile;
      PrintToFileName = source.PrintToFileName;
      PageRange = source.PageRange;
      PageNumbers = source.PageNumbers;
      Copies = source.Copies;
      Collate = source.Collate;
      PrintPages = source.PrintPages;
      Reverse = source.Reverse;
      Duplex = source.Duplex;
      PaperSource = source.PaperSource;
      PrintMode = source.PrintMode;
      PrintOnSheetWidth = source.PrintOnSheetWidth;
      PrintOnSheetHeight = source.PrintOnSheetHeight;
      PrintOnSheetRawPaperSize = source.PrintOnSheetRawPaperSize;
      PagesOnSheet = source.PagesOnSheet;
      source.CopyNames.CopyTo(CopyNames, 0);
      ShowDialog = source.ShowDialog;
    }
    
    /// <summary>
    /// Resets all settings to its default values.
    /// </summary>
    public void Clear()
    {
      printer = "";
      savePrinterWithReport = false;
      printToFile = false;
      printToFileName = "";
      pageRange = PageRange.All;
      pageNumbers = "";
      copies = 1;
      collate = true;
      printPages = PrintPages.All;
      reverse = false;
      duplex = Duplex.Default;
      paperSource = 7;
      printMode = PrintMode.Default;
      printOnSheetWidth = 210;
      printOnSheetHeight = 297;
      printOnSheetRawPaperSize = 0;
      pagesOnSheet = PagesOnSheet.One;
      copyNames = new string[0];
      showDialog = true;
      DisposeMeasureGraphics();
    }

    internal void Serialize(FRWriter writer, PrintSettings c)
    {
      if (SavePrinterWithReport && Printer != c.Printer)
        writer.WriteStr("PrintSettings.Printer", Printer);
      if (SavePrinterWithReport != c.SavePrinterWithReport)
        writer.WriteBool("PrintSettings.SavePrinterWithReport", SavePrinterWithReport);
      if (PrintToFile != c.PrintToFile)
        writer.WriteBool("PrintSettings.PrintToFile", PrintToFile);
      if (PrintToFileName != c.PrintToFileName)
        writer.WriteStr("PrintSettings.PrintToFileName", PrintToFileName);
      if (PageRange != c.PageRange)
        writer.WriteValue("PrintSettings.PageRange", PageRange);
      if (PageNumbers != c.PageNumbers)
        writer.WriteStr("PrintSettings.PageNumbers", PageNumbers);
      if (Copies != c.Copies)
        writer.WriteInt("PrintSettings.Copies", Copies);
      if (Collate != c.Collate)
        writer.WriteBool("PrintSettings.Collate", Collate);
      if (PrintPages != c.PrintPages)
        writer.WriteValue("PrintSettings.PrintPages", PrintPages);
      if (Reverse != c.Reverse)
        writer.WriteBool("PrintSettings.Reverse", Reverse);
      if (Duplex != c.Duplex)
        writer.WriteValue("PrintSettings.Duplex", Duplex);
      if (PaperSource != c.PaperSource)
        writer.WriteInt("PrintSettings.PaperSource", PaperSource);
      if (PrintMode != c.PrintMode)
        writer.WriteValue("PrintSettings.PrintMode", PrintMode);
      if (PrintOnSheetWidth != c.PrintOnSheetWidth)
        writer.WriteFloat("PrintSettings.PrintOnSheetWidth", PrintOnSheetWidth);
      if (PrintOnSheetHeight != c.PrintOnSheetHeight)
        writer.WriteFloat("PrintSettings.PrintOnSheetHeight", PrintOnSheetHeight);
      if (PrintOnSheetRawPaperSize != c.PrintOnSheetRawPaperSize)
        writer.WriteInt("PrintSettings.PrintOnSheetRawPaperSize", PrintOnSheetRawPaperSize);
      if (PagesOnSheet != c.PagesOnSheet)
        writer.WriteValue("PrintSettings.PagesOnSheet", PagesOnSheet);
      if (!writer.AreEqual(CopyNames, c.CopyNames))
        writer.WriteValue("PrintSettings.CopyNames", CopyNames);
      if (ShowDialog != c.ShowDialog)
        writer.WriteBool("PrintSettings.ShowDialog", ShowDialog);
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="PrintSettings"/> class with default settings.
    /// </summary>
    public PrintSettings() 
    {
      Clear();
    }
  }
}