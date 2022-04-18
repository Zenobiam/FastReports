using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace FastReport.RichTextParser
{
    partial class RTF_View
    {
        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            VisaulHelpers state = this.HelpersMode;
            Graphics g = e.Graphics;

            HelpersMode = VisaulHelpers.DoNotClearPageBackground;
            g.PageUnit = GraphicsUnit.Millimeter;  //.Pixel;
            //g.TranslateTransform(-e.PageSettings.HardMarginX / 100f * g.DpiX, -e.PageSettings.HardMarginY / 100f * g.DpiY);

            PaintEventArgs pea = new PaintEventArgs(g, e.MarginBounds);
            OnPaint(pea);

            e.HasMorePages = false; // HasMorePages();
            this.HelpersMode = state;
        }

        public void PrintInternal(int curPage)
        {
            using (PrintDocument doc = new PrintDocument())
            {
 
                //PrintControllerBase controller = null;
                //switch (report.PrintSettings.PrintMode)
                //{
                //    case PrintMode.Default:
                //        controller = new DefaultPrintController(report, doc, curPage);
                //        break;

                //    case PrintMode.Split:
                //        controller = new SplitPrintController(report, doc, curPage);
                //        break;

                //    case PrintMode.Scale:
                //        controller = new ScalePrintController(report, doc, curPage);
                //        break;
                //}

                doc.PrintController = new StandardPrintController();
                doc.PrintPage += new PrintPageEventHandler(PrintPage);
                //doc.QueryPageSettings += new QueryPageSettingsEventHandler(controller.QueryPageSettings);

                //Duplex duplex = report.PrintSettings.Duplex;
                //if (duplex != Duplex.Default)
                //    doc.PrinterSettings.Duplex = duplex;

                try
                {
                    doc.Print();
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message, e.Source);
                }

            }
        }
    }

}
