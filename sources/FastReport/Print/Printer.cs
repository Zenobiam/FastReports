using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Printing;
using FastReport.Preview;
using FastReport.Forms;
using FastReport.Utils;

namespace FastReport.Print
{
  internal class Printer
  {
    private Report report;

    public void Print(PrinterSettings printerSettings, int curPage)
    { 
      if (report.PrintSettings.CopyNames.Length > 0)
      {
        // copy names are set, handle copies in code
        int copies = report.PrintSettings.Copies;

        try
        {
          report.PrintSettings.Copies = 1;
          for (int copyIndex = 1; copyIndex <= copies; copyIndex++)
          {
            report.PreparedPages.MacroValues["Copy#"] = copyIndex;
            PrintInternal(printerSettings, curPage);
          }
        }
        finally
        {
          report.PrintSettings.Copies = copies;
          report.PreparedPages.MacroValues.Remove("Copy#");
        }
      }
      else
      {
        // just print
        PrintInternal(printerSettings, curPage);
      }
    }

    private void PrintInternal(PrinterSettings printerSettings, int curPage)
    {
      using (PrintDocument doc = new PrintDocument())
      {
        if (printerSettings != null)
          doc.PrinterSettings = printerSettings;

        PrintControllerBase controller = null;
        switch (report.PrintSettings.PrintMode)
        {
          case PrintMode.Default:
            controller = new DefaultPrintController(report, doc, curPage);
            break;

          case PrintMode.Split:
            controller = new SplitPrintController(report, doc, curPage);
            break;

          case PrintMode.Scale:
            controller = new ScalePrintController(report, doc, curPage);
            break;
        }

        doc.PrintController = new StandardPrintController();
        doc.PrintPage += new PrintPageEventHandler(controller.PrintPage);
        doc.QueryPageSettings += new QueryPageSettingsEventHandler(controller.QueryPageSettings);
        Duplex duplex = report.PrintSettings.Duplex;
        if (duplex != Duplex.Default)
            doc.PrinterSettings.Duplex = duplex;

        try
        {
            report.SetOperation(ReportOperation.Printing);
            Config.ReportSettings.OnStartProgress(report);
            doc.Print();
            Config.ReportSettings.OnReportPrinted(report);
        }
        finally
        {
            Config.ReportSettings.OnFinishProgress(report);
            report.SetOperation(ReportOperation.None);
        }
      }
    }

    public Printer(Report report) 
    {
        this.report = report;
    }
  }
}