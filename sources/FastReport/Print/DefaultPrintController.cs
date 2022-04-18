using System;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using FastReport.Utils;
using System.Drawing;

namespace FastReport.Print
{
    internal class DefaultPrintController : PrintControllerBase
    {
        public override void QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {
            Page = GetNextPage();
            if (Page != null)
            {
                SetPaperSize(Page, e);
                e.PageSettings.Landscape = Page.Landscape;
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
            if (Config.IsRunningOnMono)
            {
                // Point is the only right thing to use in mono. Pixel unit produces weird layout
                g.PageUnit = GraphicsUnit.Point;
                g.ResetTransform();
                g.TranslateTransform(-e.PageSettings.HardMarginX / 100f * 72f, -e.PageSettings.HardMarginY / 100f * 72f);

                // workaround different pango/cairo rendering behavior
                if (DrawUtils.GetMonoRendering(g) == MonoRendering.Pango)
                {
                    g.ScaleTransform(72f / 96f, 72f / 96f);
                    paintArgs = new FRPaintEventArgs(g, 1, 1, Report.GraphicCache);
                }
                else
                {
                    paintArgs = new FRPaintEventArgs(g, 72f / 96f, 72f / 96f, Report.GraphicCache);
                }
            }
            else
            {
                g.PageUnit = GraphicsUnit.Pixel;
                g.TranslateTransform(-e.PageSettings.HardMarginX / 100f * g.DpiX, -e.PageSettings.HardMarginY / 100f * g.DpiY);
                paintArgs = new FRPaintEventArgs(g, g.DpiX / 96, g.DpiY / 96, Report.GraphicCache);
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
            e.HasMorePages = HasMorePages();
        }

        public DefaultPrintController(Report report, PrintDocument doc, int curPage) : base(report, doc, curPage)
        {
        }
    }
}