using System;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using FastReport.Utils;
using System.Drawing;

namespace FastReport.Print
{
    internal class ScalePrintController : PrintControllerBase
    {
        #region Fields
        private bool firstTime = true;
        #endregion

        #region Private Methods
        private float CorrectScaleForUnlimitedPage(float scale, bool sourceIsLandscape,
            float sheetWidth, float sheetHeight, float paperWidth, float paperHeight)
        {
            if (Page.UnlimitedWidth || Page.UnlimitedHeight)
            {
                switch (Report.PrintSettings.PagesOnSheet)
                {
                    case PagesOnSheet.Two:
                        if (sourceIsLandscape)
                        {
                            float twoPagesHeight = paperHeight * scale * 2;
                            if (twoPagesHeight > sheetHeight)
                            {
                                scale *= twoPagesHeight / sheetHeight / 2;
                            }
                        }
                        break;
                    case PagesOnSheet.Eight:
                        if (sourceIsLandscape)
                        {
                            float fourPagesHeight = paperHeight * scale * 4;
                            if (fourPagesHeight > sheetHeight)
                            {
                                scale *= fourPagesHeight / sheetHeight / 2;
                            }
                        }
                        break;
                }
            }
            return scale;
        }

        #endregion // Private Methods

        public override void QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {
            if (firstTime)
                Page = GetNextPage();
            firstTime = false;

            if (Page != null)
            {
                SetPaperSize(Report.PrintSettings.PrintOnSheetWidth, Report.PrintSettings.PrintOnSheetHeight,
                  Report.PrintSettings.PrintOnSheetRawPaperSize, e);

                float paperWidth = Page.UnlimitedWidth ? Page.UnlimitedWidthValue / Units.Millimeters : Page.PaperWidth;
                float paperHeight = Page.UnlimitedHeight ? Page.UnlimitedHeightValue / Units.Millimeters : Page.PaperHeight;

                // rotate page if print 2 or 8 pages per sheet
                bool rotate = Report.PrintSettings.PagesOnSheet == PagesOnSheet.Two ||
                  Report.PrintSettings.PagesOnSheet == PagesOnSheet.Eight;
                bool sourceIsLandscape = paperWidth > paperHeight;
                e.PageSettings.Landscape = rotate ? !sourceIsLandscape : sourceIsLandscape;

                SetPaperSource(Page, e);
                Duplex duplex = Page.Duplex;
                if (duplex != Duplex.Default)
                    e.PageSettings.PrinterSettings.Duplex = duplex;
            }
        }

        public override void PrintPage(object sender, PrintPageEventArgs e)
        {
            StartPage(e);

            if (Page != null)
            {
                int countX = 0;
                int countY = 0;
                float scale = 0;

                float paperWidth = Page.UnlimitedWidth ? Page.UnlimitedWidthValue / Units.Millimeters : Page.PaperWidth;
                float paperHeight = Page.UnlimitedHeight ? Page.UnlimitedHeightValue / Units.Millimeters : Page.PaperHeight;

                // switch dimensions because FSheetWidth, FSheetHeight is a portrait dimensions
                float sheetWidth = Report.PrintSettings.PrintOnSheetWidth;
                float sheetHeight = Report.PrintSettings.PrintOnSheetHeight;
                bool sourceIsLandscape = paperWidth > paperHeight;
                if (sourceIsLandscape)
                {
                    sheetWidth = Report.PrintSettings.PrintOnSheetHeight;
                    sheetHeight = Report.PrintSettings.PrintOnSheetWidth;
                }

                switch (Report.PrintSettings.PagesOnSheet)
                {
                    case PagesOnSheet.One:
                        countX = 1;
                        countY = 1;
                        scale = Math.Min(sheetWidth / paperWidth, sheetHeight / paperHeight);
                        break;

                    case PagesOnSheet.Two:
                        countX = 2;
                        countY = 1;
                        if (sourceIsLandscape)
                        {
                            countX = 1;
                            countY = 2;
                        }
                        scale = Math.Min(sheetHeight / paperWidth, sheetWidth / paperHeight);
                        scale = CorrectScaleForUnlimitedPage(scale, sourceIsLandscape, sheetWidth, sheetHeight, paperWidth, paperHeight);
                        break;

                    case PagesOnSheet.Four:
                        countX = 2;
                        countY = 2;
                        scale = Math.Min(sheetWidth / paperWidth, sheetHeight / paperHeight) / 2;
                        break;

                    case PagesOnSheet.Eight:
                        countX = 4;
                        countY = 2;
                        if (sourceIsLandscape)
                        {
                            countX = 2;
                            countY = 4;
                        }
                        scale = Math.Min(sheetHeight / paperWidth, sheetWidth / paperHeight) / 2;
                        scale = CorrectScaleForUnlimitedPage(scale, sourceIsLandscape, sheetWidth, sheetHeight, paperWidth, paperHeight);
                        break;
                }

                float pieceX = paperWidth * scale;
                float pieceY = paperHeight * scale;

                float leftMargin = e.PageSettings.HardMarginX / 100f * 25.4f;
                float topMargin = e.PageSettings.HardMarginY / 100f * 25.4f;

                float offsY = -topMargin;

                for (int y = 0; y < countY; y++)
                {
                    float offsX = -leftMargin;

                    for (int x = 0; x < countX; x++)
                    {
                        Graphics g = e.Graphics;
                        FRPaintEventArgs paintArgs;
                        GraphicsState state = g.Save();
                        IGraphics gr = new GdiGraphics(g, false);

                        try
                        {
                            if (Config.IsRunningOnMono)
                            {
                                // Point is the only right thing to use in mono. Pixel unit produces weird layout
                                g.PageUnit = GraphicsUnit.Point;
                                g.TranslateTransform(offsX * Units.Millimeters * 72f / 96f, offsY * Units.Millimeters * 72f / 96f);

                                // workaround different pango/cairo rendering behavior
                                if (DrawUtils.GetMonoRendering(g) == MonoRendering.Pango)
                                {
                                    g.ScaleTransform(scale * 72f / 96f, scale * 72f / 96f);
                                    paintArgs = new FRPaintEventArgs(gr, 1, 1, Report.GraphicCache);
                                }
                                else
                                {
                                    g.ScaleTransform(scale, scale);
                                    paintArgs = new FRPaintEventArgs(gr, 72f / 96f, 72f / 96f, Report.GraphicCache);
                                }
                            }
                            else
                            {
                                g.PageUnit = GraphicsUnit.Pixel;
                                g.TranslateTransform(offsX * Units.Millimeters * g.DpiX / 96, offsY * Units.Millimeters * g.DpiY / 96);
                                g.ScaleTransform(scale, scale);
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

                        }
                        finally
                        {
                            g.Restore(state);
                        }

                        offsX += pieceX;
                        Page.Dispose();
                        Page = GetNextPage();
                        if (Page == null)
                            break;
                    }

                    if (Page == null)
                        break;
                    offsY += pieceY;
                }
            }

            FinishPage(e);
            e.HasMorePages = Page != null;
        }

        public ScalePrintController(Report report, PrintDocument doc, int curPage) : base(report, doc, curPage)
        {
        }
    }
}