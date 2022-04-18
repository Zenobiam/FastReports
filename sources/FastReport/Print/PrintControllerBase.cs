using System;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using FastReport.Utils;

namespace FastReport.Print
{
  internal abstract class PrintControllerBase
  {
    #region Fields
    private Report report;
    private PrintDocument doc;
    private PageNumbersParser pages;
    private ReportPage page;
    private int pageNo;
    private int printedPageCount;
    #endregion

    #region Properties
    public Report Report
    {
      get { return report; }
    }

    public PrintDocument Doc
    {
      get { return doc; }
    }

    public PageNumbersParser Pages
    {
      get { return pages; }
    }

    public ReportPage Page
    {
      get { return page; }
      set { page = value; }
    }

    public int PageNo
    {
      get { return pageNo; }
      set { pageNo = value; }
    }
    #endregion

    #region Private Methods
    private PaperSize FindPaperSize(PrinterSettings.PaperSizeCollection paperSizes,
      float paperWidth, float paperHeight, int rawKind)
    {
      if (paperSizes == null)
        return null;
      foreach (PaperSize ps in paperSizes)
      {
        // convert hundreds of inches to mm
        float psWidth = ps.Width / 100f * 25.4f;
        float psHeight = ps.Height / 100f * 25.4f;
        // check if page has the same kind and size
        bool sizeEqual = Math.Abs(paperWidth - psWidth) < 5 && Math.Abs(paperHeight - psHeight) < 5;
        if (sizeEqual)
        {
          if (rawKind == 0 || ps.RawKind == rawKind)
            return ps;
        }
      }

      return null;
    }
    #endregion

    #region Protected methods
    protected ReportPage GetNextPage()
    {
      pageNo = 0;
      if (Pages.GetPage(ref pageNo))
        return Report.PreparedPages.GetPage(pageNo);
      return null;
    }

    protected bool HasMorePages()
    {
      return Pages.Count > 0; 
    }

    protected void SetPaperSize(ReportPage page, QueryPageSettingsEventArgs e)
    {
      float width = page.PaperWidth;
      float height = page.PaperHeight;
      if (page.Landscape)
      {
        width = page.PaperHeight;
        height = page.PaperWidth;
      }
      SetPaperSize(width, height, page.RawPaperSize, e);
    }

    protected void SetPaperSize(float paperWidth, float paperHeight, int rawKind, QueryPageSettingsEventArgs e)
    {
      PaperSize ps = null;

      // check PaperWidth, PaperHeight, RawKind
      if (rawKind != 0)
        ps = FindPaperSize(e.PageSettings.PrinterSettings.PaperSizes, paperWidth, paperHeight, rawKind);

      // check PaperWidth, PaperHeight only
      if (ps == null)
        ps = FindPaperSize(e.PageSettings.PrinterSettings.PaperSizes, paperWidth, paperHeight, 0);

      // paper size not found, create custom one
      if (ps == null)
      {
        ps = new PaperSize();
        ps.Width = (int)Math.Round(paperWidth / 25.4f * 100);
        ps.Height = (int)Math.Round(paperHeight / 25.4f * 100);
      }

      e.PageSettings.PaperSize = ps;
    }

    protected void SetPaperSource(ReportPage page, QueryPageSettingsEventArgs e)
    {
      int rawKind = Report.PrintSettings.PaperSource;
      // it's set to Automatic, try page.PaperSource
      if (rawKind == 7)
      {
        if (PageNo == 0)
        {
            rawKind = page.FirstPageSource;
        }
        else if (PageNo == Report.PreparedPages.Count - 1)
        {
            rawKind = page.LastPageSource;
        }
        else
        {
            rawKind = page.OtherPagesSource;
        }
      }
      // do not change paper source if it is AutomaticFeed
      if (rawKind == 7)
        return;

      foreach (PaperSource ps in e.PageSettings.PrinterSettings.PaperSources)
      {
        if (ps.RawKind == rawKind)
        {
          e.PageSettings.PaperSource = ps;
          return;
        }
      }
    }

    protected void StartPage(PrintPageEventArgs e)
    {
      printedPageCount++;
      Config.ReportSettings.OnProgress(Report, 
        String.Format(Res.Get("Messages,PrintingPage"), printedPageCount), printedPageCount, 0);
    }

    protected void FinishPage(PrintPageEventArgs e)
    {
      if (Report.Aborted)
        e.Cancel = true;
    }
    #endregion

    #region Public methods
    public abstract void QueryPageSettings(object sender, QueryPageSettingsEventArgs e);
    public abstract void PrintPage(object sender, PrintPageEventArgs e);
    #endregion

    public PrintControllerBase(Report report, PrintDocument doc, int curPage)
    {
        this.report = report;
        this.doc = doc;
        pages = new PageNumbersParser(report, curPage);

        // select the printer
        if (!String.IsNullOrEmpty(report.PrintSettings.Printer))
            this.doc.PrinterSettings.PrinterName = report.PrintSettings.Printer;
        
        // print to file
        if (report.PrintSettings.PrintToFile)
        {
            this.doc.PrinterSettings.PrintFileName = report.PrintSettings.PrintToFileName;
            this.doc.PrinterSettings.PrintToFile = true;
        }
        
        // set job name
        if (!String.IsNullOrEmpty(report.ReportInfo.Name))
            this.doc.DocumentName = report.ReportInfo.Name;
        else
            this.doc.DocumentName = report.FileName;

        // set copies and collation
        if (report.PrintSettings.Copies < 1)
            report.PrintSettings.Copies = 1;
        this.doc.PrinterSettings.Copies = (short)report.PrintSettings.Copies;
        this.doc.PrinterSettings.Collate = report.PrintSettings.Collate;
    }
  }
}
