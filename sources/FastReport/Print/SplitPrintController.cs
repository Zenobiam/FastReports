using System;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using FastReport.Utils;
using System.Drawing;

namespace FastReport.Print
{
    internal class SplitPrintController : PrintControllerBase
    {
        #region Fields
        private List<SplittedPage> splittedPages;
        private float addX = 5;
        private float addY = 5;
        private float offsetX;
        private float offsetY;
        private bool landscape;
        #endregion

        #region Private methods
        private ReportPage GetNextSplittedPage()
        {
            if (splittedPages.Count == 0)
                return null;

            PageNo = splittedPages[0].pageNo;
            offsetX = splittedPages[0].offsetX;
            offsetY = splittedPages[0].offsetY;
            landscape = splittedPages[0].landscape;
            splittedPages.RemoveAt(0);

            return Report.PreparedPages.GetPage(PageNo);
        }

        private bool HasMoreSplittedPages()
        {
            return splittedPages.Count > 0;
        }

        private void TrySplit(float a, float b, float c, float d, out int x, out int y)
        {
            x = Math.Abs(Math.Truncate(a / c) * c - a) < 11 ?
              (int)Math.Round(a / c) : (int)Math.Truncate(a / c) + 1;

            y = Math.Abs(Math.Truncate(b / d) * d - b) < 11 ?
              (int)Math.Round(b / d) : (int)Math.Truncate(b / d) + 1;
        }

        private void SplitPage(float a, float b, float c, float d, out int x, out int y, out bool needRotate)
        {
            needRotate = false;

            TrySplit(a, b, c, d, out x, out y);

            int tempX = x;
            int tempY = y;

            float tempC = c;
            c = d;
            d = tempC;

            TrySplit(a, b, c, d, out x, out y);

            if (x * y >= tempX * tempY)
            {
                x = tempX;
                y = tempY;
            }
            else
                needRotate = true;
        }
        #endregion

        #region Public methods
        public override void QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {
            Page = GetNextSplittedPage();
            if (Page != null)
            {
                SetPaperSize(Report.PrintSettings.PrintOnSheetWidth, Report.PrintSettings.PrintOnSheetHeight,
                  Report.PrintSettings.PrintOnSheetRawPaperSize, e);
                e.PageSettings.Landscape = landscape;
                SetPaperSource(Page, e);
                Duplex duplex = Page.Duplex;
                if (duplex != Duplex.Default)
                    e.PageSettings.PrinterSettings.Duplex = duplex;
            }
        }

        public override void PrintPage(object sender, PrintPageEventArgs e)
        {
            StartPage(e);

            Graphics g = e.Graphics;
            FRPaintEventArgs paintArgs;
            IGraphics gr = new GdiGraphics(g, false);
            
            if (Config.IsRunningOnMono)
            {
                // Point is the only right thing to use in mono. Pixel unit produces weird layout
                g.PageUnit = GraphicsUnit.Point;
                g.ResetTransform();
                g.TranslateTransform(offsetX * Units.Millimeters * 72f / 96f, offsetY * Units.Millimeters * 72f / 96f);

                // workaround different pango/cairo rendering behavior
                if (DrawUtils.GetMonoRendering(g) == MonoRendering.Pango)
                {
                    g.ScaleTransform(72f / 96f, 72f / 96f);
                    paintArgs = new FRPaintEventArgs(gr, 1, 1, Report.GraphicCache);
                }
                else
                {
                    paintArgs = new FRPaintEventArgs(gr, 72f / 96f, 72f / 96f, Report.GraphicCache);
                }
            }
            else
            {
                g.PageUnit = GraphicsUnit.Pixel;
                g.TranslateTransform(offsetX * Units.Millimeters * g.DpiX / 96, offsetY * Units.Millimeters * g.DpiY / 96);
                paintArgs = new FRPaintEventArgs(gr, g.DpiX / 96, g.DpiY / 96, Report.GraphicCache);
            }

            Page.Print(paintArgs);

#if Demo
      using (Watermark trialWatermark = new Watermark())
      {
        trialWatermark.Text = typeof(Duplex).Name[0].ToString() + typeof(Exception).Name[0].ToString() +
          typeof(Margins).Name[0].ToString() + typeof(Object).Name[0].ToString() + " " +
          typeof(ValueType).Name[0].ToString() + typeof(Exception).Name[0].ToString() +
          typeof(Rectangle).Name[0].ToString() + typeof(ShapeKind).Name[0].ToString() +
          typeof(ICloneable).Name[0].ToString() + typeof(Object).Name[0].ToString() +
          typeof(NonSerializedAttribute).Name[0].ToString();
        trialWatermark.DrawText(paintArgs, new RectangleF(0, 0, 
          Page.PaperWidth * Units.Millimeters, Page.PaperHeight * Units.Millimeters), Report, true);
      }
#endif
#if Academic
      using (Watermark trialWatermark = new Watermark())
      {
        trialWatermark.Text = typeof(Array).Name[0].ToString() + typeof(Char).Name[0].ToString() +
          typeof(Array).Name[0].ToString() + typeof(DateTime).Name[0].ToString() +
          typeof(Enum).Name[0].ToString() + typeof(Margins).Name[0].ToString() +
          typeof(IEnumerable).Name[0].ToString() + typeof(Char).Name[0].ToString() +
          " " +
          typeof(LineObject).Name[0].ToString() + typeof(IEnumerable).Name[0].ToString() +
          typeof(Char).Name[0].ToString() + typeof(Enum).Name[0].ToString() +
          typeof(Nullable).Name[0].ToString() + typeof(ShapeObject).Name[0].ToString() +
          typeof(Enum).Name[0].ToString();
        trialWatermark.DrawText(paintArgs, new RectangleF(0, 0, 
          Page.PaperWidth * Units.Millimeters, Page.PaperHeight * Units.Millimeters), Report, true);
      }
#endif

            Page.Dispose();

            FinishPage(e);
            e.HasMorePages = HasMoreSplittedPages();
        }
        #endregion

        public SplitPrintController(Report report, PrintDocument doc, int curPage) : base(report, doc, curPage)
        {
            splittedPages = new List<SplittedPage>();

            // get hard margins
            float leftMargin = doc.PrinterSettings.DefaultPageSettings.HardMarginX / 100f * 25.4f;
            float topMargin = doc.PrinterSettings.DefaultPageSettings.HardMarginY / 100f * 25.4f;
            float rightMargin = leftMargin;
            float bottomMargin = topMargin;

            int countX;
            int countY;
            bool needChangeOrientation;

            while (true)
            {
                Page = GetNextPage();
                if (Page == null)
                    break;

                if (!Page.UnlimitedHeight && !Page.UnlimitedWidth)
                {
                    SplitPage(Page.PaperWidth, Page.PaperHeight,
                        Report.PrintSettings.PrintOnSheetWidth, Report.PrintSettings.PrintOnSheetHeight,
                        out countX, out countY, out needChangeOrientation);
                }
                else
                {
                    SplitPage(Page.WidthInPixels / Units.Millimeters, Page.HeightInPixels / Units.Millimeters,
                        Report.PrintSettings.PrintOnSheetWidth, Report.PrintSettings.PrintOnSheetHeight,
                        out countX, out countY, out needChangeOrientation);
                }

                bool landscape = false;
                if (needChangeOrientation)
                    landscape = true;

                float pieceX = landscape ? Report.PrintSettings.PrintOnSheetHeight : Report.PrintSettings.PrintOnSheetWidth;
                float pieceY = landscape ? Report.PrintSettings.PrintOnSheetWidth : Report.PrintSettings.PrintOnSheetHeight;

                float marginY = 0;
                float printedY = 0;
                float offsY = -topMargin;

                for (int y = 1; y <= countY; y++)
                {
                    float marginX = 0;
                    float printedX = 0;
                    float offsX = -leftMargin;

                    for (int x = 1; x <= countX; x++)
                    {
                        splittedPages.Add(new SplittedPage(PageNo, offsX, offsY, landscape));

                        printedX += (pieceX - marginX - rightMargin) - addX;
                        offsX = -printedX;
                        marginX = leftMargin;
                    }

                    printedY += (pieceY - marginY - bottomMargin) - addY;
                    offsY = -printedY;
                    marginY = topMargin;
                }

                Page.Dispose();
            }
        }


        private class SplittedPage
        {
            // physical pageno
            public int pageNo;
            // offsetx, in mm
            public float offsetX;
            // offsety, in mm
            public float offsetY;
            // determines if we should rotate the output page
            public bool landscape;

            public SplittedPage(int pageNo, float ofsX, float ofsY, bool landscape)
            {
                this.pageNo = pageNo;
                offsetX = ofsX;
                offsetY = ofsY;
                this.landscape = landscape;
            }
        }
    }
}