using FastReport.DataVisualization.Charting;
using CrystalDecisions.Shared;

namespace FastReport.Design.ImportPlugins.RPT
{
    /// <summary>
    /// The RPT units converter.
    /// </summary>
    public static class UnitsConverter
    {
        #region Public Methods

        /// <summary>
        /// Converts string.
        /// </summary>
        /// <param name="str">The string value.</param>
        /// <returns>The result string value.</returns>
        public static string ConvertString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            return str;
        }

        /// <summary>
        /// Converts twips to pixels.
        /// </summary>
        /// <param name="twips">The value in twips.</param>
        /// <returns>The value in pixels.</returns>
        public static float TwipsToPixels(int twips)
        {
            return twips * 96 / 1440;
        }

        /// <summary>
        /// Converts twips to millimeters.
        /// </summary>
        /// <param name="twips">The value in twips.</param>
        /// <returns>The value in millimeters.</returns>
        public static float TwipsToMillimeters(int twips)
        {
            return twips * 10 / 567;
        }

        /// <summary>
        /// Converts the Crystal Reports PrinterDuplex value to the Duplex value.
        /// </summary>
        /// <param name="pd">The Crystal Report PrinterDuplex value.</param>
        /// <returns>The Duplex value.</returns>
        public static System.Drawing.Printing.Duplex ConvertPrinterDuplex(CrystalDecisions.Shared.PrinterDuplex pd)
        {
            if (pd == CrystalDecisions.Shared.PrinterDuplex.Simplex)
            {
                return System.Drawing.Printing.Duplex.Simplex;
            }
            else if (pd == CrystalDecisions.Shared.PrinterDuplex.Horizontal)
            {
                return System.Drawing.Printing.Duplex.Horizontal;
            }
            else if (pd == CrystalDecisions.Shared.PrinterDuplex.Vertical)
            {
                return System.Drawing.Printing.Duplex.Vertical;
            }
            return System.Drawing.Printing.Duplex.Default;
        }

        /// <summary>
        /// Converts the Crystal Reports PaperSize to width and height values of paper size in millimeters.
        /// </summary>
        /// <param name="paperSize">The Crystal Reports PaperSize value.</param>
        /// <param name="paperOrientation">The Crystal Reports PaperOrieantation value.</param>
        /// <param name="page">The ReportPage instance.</param>
        public static void ConvertPaperSize(CrystalDecisions.Shared.PaperSize paperSize, CrystalDecisions.Shared.PaperOrientation paperOrientation, FastReport.ReportPage page)
        {
            if (page == null)
                return;

            float width = 0.0f;
            float height = 0.0f;
            bool landscape = false;
            if (paperSize == CrystalDecisions.Shared.PaperSize.PaperA4)
            {
                width = 210;
                height = 297;
            }
            else if (paperSize == CrystalDecisions.Shared.PaperSize.Paper10x14)
            {
                width = 254;
                height = 356;
            }
            else if (paperSize == CrystalDecisions.Shared.PaperSize.Paper11x17)
            {
                width = 279;
                height = 432;
            }
            else if (paperSize == CrystalDecisions.Shared.PaperSize.PaperA3)
            {
                width = 297;
                height = 420;
            }
            else if (paperSize == CrystalDecisions.Shared.PaperSize.PaperA5)
            {
                width = 148;
                height = 210;
            }
            else if (paperSize == CrystalDecisions.Shared.PaperSize.PaperB4)
            {
                width = 257;
                height = 364;
            }
            else if (paperSize == CrystalDecisions.Shared.PaperSize.PaperB5)
            {
                width = 182;
                height = 257;
            }
            else if (paperSize == CrystalDecisions.Shared.PaperSize.PaperFolio)
            {
                width = 216;
                height = 330;
            }
            else if (paperSize == CrystalDecisions.Shared.PaperSize.PaperLedger)
            {
                width = 432;
                height = 279;
            }
            else if (paperSize == CrystalDecisions.Shared.PaperSize.PaperLegal)
            {
                width = 216;
                height = 356;
            }
            else if (paperSize == CrystalDecisions.Shared.PaperSize.PaperLetter)
            {
                width = 216;
                height = 279;
            }
            else
            {
                width = 210;
                height = 297;
            }
            if (paperOrientation == CrystalDecisions.Shared.PaperOrientation.Landscape)
            {
                landscape = true;
            }
            page.PaperWidth = width;
            page.PaperHeight = height;
            page.Landscape = landscape;
        }

        /// <summary>
        /// Converts the Crystal Reports Border value to FastReport BorderLines value.
        /// </summary>
        /// <param name="border">The Crystal Reports value.</param>
        /// <returns>The FastReport BorderLines value.</returns>
        public static FastReport.BorderLines ConvertBorderToBorderLines(CrystalDecisions.CrystalReports.Engine.Border border)
        {
            FastReport.BorderLines lines = FastReport.BorderLines.None;
            if (border != null)
            {
                if (border.TopLineStyle != CrystalDecisions.Shared.LineStyle.NoLine)
                {
                    lines |= FastReport.BorderLines.Top;
                }
                if (border.LeftLineStyle != CrystalDecisions.Shared.LineStyle.NoLine)
                {
                    lines |= FastReport.BorderLines.Left;
                }
                if (border.RightLineStyle != CrystalDecisions.Shared.LineStyle.NoLine)
                {
                    lines |= FastReport.BorderLines.Right;
                }
                if (border.BottomLineStyle != CrystalDecisions.Shared.LineStyle.NoLine)
                {
                    lines |= FastReport.BorderLines.Bottom;
                }
            }
            return lines;
        }

        /// <summary>
        /// Converts the Crystal Reports LineStyle value to FastReport LineStyle value.
        /// </summary>
        /// <param name="style">The Crystal Reports LineStyle value.</param>
        /// <returns>The FastReport LineStyle value.</returns>
        public static FastReport.LineStyle ConvertLineStyle(CrystalDecisions.Shared.LineStyle style)
        {
            if (style == CrystalDecisions.Shared.LineStyle.SingleLine)
            {
                return FastReport.LineStyle.Solid;
            }
            else if (style == CrystalDecisions.Shared.LineStyle.DashLine)
            {
                return FastReport.LineStyle.Dash;
            }
            else if (style == CrystalDecisions.Shared.LineStyle.DotLine)
            {
                return FastReport.LineStyle.Dot;
            }
            else if (style == CrystalDecisions.Shared.LineStyle.DoubleLine)
            {
                return FastReport.LineStyle.Double;
            }
            return FastReport.LineStyle.Solid;
        }

        /// <summary>
        /// Converts the Crystal Reports LineStyle value to ChartDashStyle value.
        /// </summary>
        /// <param name="style">The Crystal Reports LineStyle value.</param>
        /// <returns>The ChartDashStyle value.</returns>
        public static ChartDashStyle ConvertLineStyleToChartDashStyle(CrystalDecisions.Shared.LineStyle style)
        {
            if (style == CrystalDecisions.Shared.LineStyle.SingleLine)
            {
                return ChartDashStyle.Solid;
            }
            else if (style == CrystalDecisions.Shared.LineStyle.DashLine)
            {
                return ChartDashStyle.Dash;
            }
            else if (style == CrystalDecisions.Shared.LineStyle.DotLine)
            {
                return ChartDashStyle.Dot;
            }
            else if (style == CrystalDecisions.Shared.LineStyle.NoLine)
            {
                return ChartDashStyle.NotSet;
            }
            return ChartDashStyle.Solid;
        }

        /// <summary>
        /// Converts the Crystal Reports Alignment value to FastReport HorzAlign value.
        /// </summary>
        /// <param name="alignment">The Crystal Reports Alignment value.</param>
        /// <returns>The FastReport HorzAlign value.</returns>
        public static FastReport.HorzAlign ConvertAlignment(CrystalDecisions.Shared.Alignment alignment)
        {
            if (alignment == CrystalDecisions.Shared.Alignment.LeftAlign)
            {
                return FastReport.HorzAlign.Left;
            }
            else if (alignment == CrystalDecisions.Shared.Alignment.RightAlign)
            {
                return FastReport.HorzAlign.Right;
            }
            else if (alignment == CrystalDecisions.Shared.Alignment.HorizontalCenterAlign)
            {
                return FastReport.HorzAlign.Center;
            }
            else if (alignment == CrystalDecisions.Shared.Alignment.Justified)
            {
                return FastReport.HorzAlign.Justify;
            }
            return FastReport.HorzAlign.Left;
        }

        #endregion // Public Methods
    }
}
