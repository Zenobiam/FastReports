using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Print;
using System.Drawing.Printing;

namespace FastReport.Preview
{
    partial class PreparedPages
    {
        #region Public Methods

        internal bool Print()
        {
            return Print(1);
        }

        internal bool Print(int curPage)
        {
            PrinterSettings printerSettings = null;
            if (Report.PrintSettings.ShowDialog)
            {
                bool ok = Report.ShowPrintDialog(out printerSettings);
                if (!ok)
                    return false;
            }

            Print(printerSettings, curPage);
            return true;
        }

        internal void Print(PrinterSettings printerSettings, int curPage)
        {
            Printer printer = new Printer(Report);
            printer.Print(printerSettings, curPage);
        }

        #endregion Public Methods
    }
}
