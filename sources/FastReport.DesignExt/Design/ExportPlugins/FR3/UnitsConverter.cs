using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using FastReport.Barcode;
using FastReport.Map;
using FastReport.Data;

namespace FastReport.Design.ExportPlugins.FR3
{
    /// <summary>
    /// The FR3 units converter.
    /// </summary>
    public static class UnitsConverter
    {
        #region Public Methods

        /// <summary>
        /// Converts Color to TColor.
        /// </summary>
        /// <param name="color">Color value.</param>
        /// <returns>String that contains TColor value.</returns>
        public static string ColorToTColor(Color color)
        {
            return (color.B << 16 | color.G << 8 | color.R).ToString();
        }

        /// <summary>
        /// Converts font style.
        /// </summary>
        /// <param name="fontStyle">FontStyle value.</param>
        /// <returns>String that contains converted value.</returns>
        public static string ConvertFontStyle(FontStyle fontStyle)
        {
            int fs = 0;
            if (FontStyle.Bold == (fontStyle & FontStyle.Bold))
            {
                fs = 1;
            }
            if (FontStyle.Italic == (fontStyle & FontStyle.Italic))
            {
                fs = fs | 2;
            }
            if (FontStyle.Underline == (fontStyle & FontStyle.Underline))
            {
                fs = fs | 4;
            }
            if (FontStyle.Strikeout == (fontStyle & FontStyle.Strikeout))
            {
                fs = fs | 8;
            }
            return fs.ToString();
        }

        /// <summary>
        /// Converts horizontal alignment of text.
        /// </summary>
        /// <param name="ha">HorzAlign value.</param>
        /// <returns>String that contains converted value.</returns>
        public static string ConvertHorzAlign(HorzAlign ha)
        {
            string result = "";
            switch (ha)
            {
                case HorzAlign.Left:
                    result = "haLeft";
                    break;
                case HorzAlign.Center:
                    result = "haCenter";
                    break;
                case HorzAlign.Right:
                    result = "haRight";
                    break;
                case HorzAlign.Justify:
                    result = "haBlock";
                    break;
                default:
                    result = "haLeft";
                    break;
            }
            return result;
        }

        /// <summary>
        /// Converts vertical alignment of text.
        /// </summary>
        /// <param name="va">VertAlign value.</param>
        /// <returns>String that contains coverted value.</returns>
        public static string ConvertVertAlign(VertAlign va)
        {
            string result = "";
            switch (va)
            {
                case VertAlign.Top:
                    result = "vaTop";
                    break;
                case VertAlign.Center:
                    result = "vaCenter";
                    break;
                case VertAlign.Bottom:
                    result = "vaBottom";
                    break;
                default:
                    result = "vaTop";
                    break;
            }
            return result;
        }

        /// <summary>
        /// Converts font size to delphi font height.
        /// </summary>
        /// <param name="size">Font size value.</param>
        /// <returns>String that contains font height value.</returns>
        public static string ConvertFontSize(float size)
        {
            return ((int)(-Math.Round(size * 96 / 72, 0))).ToString();
        }

        /// <summary>
        /// Convert line style to frame style.
        /// </summary>
        /// <param name="style">Line style value.</param>
        /// <returns>String that contains converted value.</returns>
        public static string ConvertLineStyle(LineStyle style)
        {
            string result = "";
            switch (style)
            {
                case LineStyle.Solid:
                    result = "fsSolid";
                    break;
                case LineStyle.Dash:
                    result = "fsDash";
                    break;
                case LineStyle.DashDot:
                    result = "fsDashDot";
                    break;
                case LineStyle.DashDotDot:
                    result = "fsDashDotDot";
                    break;
                case LineStyle.Dot:
                    result = "fsDot";
                    break;
                case LineStyle.Double:
                    result = "fsDouble";
                    break;
                default:
                    result = "fsSolid";
                    break;
            }
            return result;
        }

        /// <summary>
        /// Converts barcode type.
        /// </summary>
        /// <param name="barcode">BarcodeBase instance.</param>
        /// <returns>String that contains converted value.</returns>
        public static string ConvertBarcodeType(BarcodeBase barcode)
        {
            string result = "bcCode128";
            if (barcode is Barcode128)
            {
                result = "bcCode128";
            }
            else if (barcode is Barcode2of5Industrial)
            {
                result = "bcCode_2_5_industrial";
            }
            else if (barcode is Barcode2of5Interleaved)
            {
                result = "bcCode_2_5_interleaved";
            }
            else if (barcode is Barcode2of5Matrix)
            {
                result = "bcCode_2_5_matrix";
            }
            else if (barcode is Barcode39)
            {
                result = "bcCode39";
            }
            else if (barcode is Barcode39Extended)
            {
                result = "bcCode39Extended";
            }
            else if (barcode is Barcode93)
            {
                result = "bcCode93";
            }
            else if (barcode is Barcode93Extended)
            {
                result = "bcCode93Extended";
            }
            else if (barcode is BarcodeAztec)
            {
                result = "bcCodeAztec";
            }
            else if (barcode is BarcodeCodabar)
            {
                result = "bcCodeCodabar";
            }
            else if (barcode is BarcodeDatamatrix)
            {
                result = "bcCodeDataMatrix";
            }
            //else if (barcode is BarcodeEAN)
            //{
            //    result = "";
            //}
            else if (barcode is BarcodeEAN128)
            {
                result = "bcCodeEAN128";
            }
            else if (barcode is BarcodeEAN13)
            {
                result = "bcCodeEAN13";
            }
            else if (barcode is BarcodeEAN8)
            {
                result = "bcCodeEAN8";
            }
            else if (barcode is BarcodeIntelligentMail)
            {
                result = "bcCodeUSPSIntelligentMail";
            }
            else if (barcode is BarcodeMaxiCode)
            {
                result = "bcCodeMaxiCode";
            }
            else if (barcode is BarcodeMSI)
            {
                result = "bcCodeMSI";
            }
            else if (barcode is BarcodePDF417)
            {
                result = "bcCodePDF417";
            }
            //else if (barcode is BarcodePharmacode)
            //{
            //    result = "";
            //}
            //else if (barcode is BarcodePlessey)
            //{
            //    result = "";
            //}
            else if (barcode is BarcodePostNet)
            {
                result = "bcCodePostNet";
            }
            else if (barcode is BarcodeQR)
            {
                result = "bcCodeQR";
            }
            else if (barcode is BarcodeSupplement2)
            {
                result = "bcCodeUPC_Supp2";
            }
            else if (barcode is BarcodeSupplement5)
            {
                result = "bcCodeUPC_Supp5";
            }
            else if (barcode is BarcodeUPC_A)
            {
                result = "bcCodeUPC_A";
            }
            else if (barcode is BarcodeUPC_E0)
            {
                result = "bcCodeUPC_E0";
            }
            else if (barcode is BarcodeUPC_E1)
            {
                result = "bcCodeUPC_E1";
            }
            return result;
        }

        /// <summary>
        /// Converts BorderLines value.
        /// </summary>
        /// <param name="lines">BorderLines instance.</param>
        /// <returns>String that contains converted value.</returns>
        public static string ConvertBorderLines(BorderLines lines)
        {
            return ((int)lines).ToString();
        }

        /// <summary>
        /// Converts CheckedSymbol value.
        /// </summary>
        /// <param name="symbol">CheckeSymbol instance.</param>
        /// <returns>String that contains converted value.</returns>
        public static string ConvertCheckedSymbol(CheckedSymbol symbol)
        {
            string result = "csCheck";
            if (symbol == CheckedSymbol.Cross)
            {
                result = "csCross";
            }
            else if (symbol == CheckedSymbol.Plus)
            {
                result = "csPlus";
            }
            return result;
        }

        /// <summary>
        /// Converts ScaleDock value.
        /// </summary>
        /// <param name="dock">ScaleDock instance.</param>
        /// <returns>String that contains converted value.</returns>
        public static string ConvertScaleDock(ScaleDock dock)
        {
            string result = "sdTopLeft";
            switch (dock)
            {
                case ScaleDock.BottomCenter:
                    result = "sdBottomCenter";
                    break;
                case ScaleDock.BottomLeft:
                    result = "sdBottomLeft";
                    break;
                case ScaleDock.BottomRight:
                    result = "sdBottomRight";
                    break;
                case ScaleDock.MiddleLeft:
                    result = "sdMiddleLeft";
                    break;
                case ScaleDock.MiddleRight:
                    result = "sdMiddleRight";
                    break;
                case ScaleDock.TopCenter:
                    result = "sdTopCenter";
                    break;
                case ScaleDock.TopLeft:
                    result = "sdTopLeft";
                    break;
                case ScaleDock.TopRight:
                    result = "sdTopRight";
                    break;
                default:
                    result = "sdTopLeft";
                    break;
            }
            return result;
        }

        /// <summary>
        /// Converts DashStyle value.
        /// </summary>
        /// <param name="ds">DashStyle instance.</param>
        /// <returns>String that contains converted value.</returns>
        public static string ConvertDashStyle(DashStyle ds)
        {
            string result = "psSolid";
            switch (ds)
            {
                case DashStyle.Solid:
                    result = "psSolid";
                    break;
                case DashStyle.Dash:
                    result = "psDash";
                    break;
                case DashStyle.DashDot:
                    result = "psDashDot";
                    break;
                case DashStyle.DashDotDot:
                    result = "psDashDotDot";
                    break;
                case DashStyle.Dot:
                    result = "psDot";
                    break;
                default:
                    result = "psSolid";
                    break;
            }
            return result;
        }

        /// <summary>
        /// Converts TotalType value.
        /// </summary>
        /// <param name="tt">TotalType instance.</param>
        /// <returns>String that contains converted value.</returns>
        public static string ConvertTotalType(TotalType tt)
        {
            string result = "opSum";
            switch (tt)
            {
                case TotalType.Avg:
                    result = "opAverage";
                    break;
                case TotalType.Count:
                    result = "opCount";
                    break;
                case TotalType.Max:
                    result = "opMax";
                    break;
                case TotalType.Min:
                    result = "opMin";
                    break;
                case TotalType.Sum:
                    result = "opSum";
                    break;
                default:
                    result = "opSum";
                    break;
            }
            return result;
        }

        /// <summary>
        /// Converts MapLabelKind value.
        /// </summary>
        /// <param name="kind">MapLabelKind instance.</param>
        /// <returns>String that contains converted value.</returns>
        public static string ConvertMapLabelKind(MapLabelKind kind)
        {
            string result = "mlName";
            switch (kind)
            {
                case MapLabelKind.Name:
                    result = "mlName";
                    break;
                case MapLabelKind.NameAndValue:
                    result = "mlNameAndValue";
                    break;
                case MapLabelKind.None:
                    result = "mlNone";
                    break;
                case MapLabelKind.Value:
                    result = "mlValue";
                    break;
                default:
                    result = "mlName";
                    break;
            }
            return result;
        }

        /// <summary>
        /// Converts MapPalette value.
        /// </summary>
        /// <param name="palette">MapPalette instance.</param>
        /// <returns>String that contains converted value.</returns>
        public static string ConvertMapPalette(MapPalette palette)
        {
            string result = "mpBrightPastel";
            switch (palette)
            {
                case MapPalette.BrightPastel:
                    result = "mpBrightPastel";
                    break;
                case MapPalette.Earth:
                    result = "mpEarth";
                    break;
                case MapPalette.Grayscale:
                    result = "mpGrayScale";
                    break;
                case MapPalette.Light:
                    result = "mpLight";
                    break;
                case MapPalette.None:
                    result = "mpNone";
                    break;
                case MapPalette.Pastel:
                    result = "mpPastel";
                    break;
                case MapPalette.Sea:
                    result = "mpSea";
                    break;
                default:
                    result = "mpBrightPastel";
                    break;
            }
            return result;
        }

        /// <summary>
        /// Converts ShapeKind value.
        /// </summary>
        /// <param name="sk">ShapeKind instance.</param>
        /// <returns>String that contains coverted value.</returns>
        public static string ConvertShapeKind(ShapeKind sk)
        {
            string result = "skRectangle";
            switch (sk)
            {
                case ShapeKind.Rectangle:
                    result = "skRectangle";
                    break;
                case ShapeKind.Diamond:
                    result = "skDiamond";
                    break;
                case ShapeKind.Ellipse:
                    result = "skEllipse";
                    break;
                case ShapeKind.RoundRectangle:
                    result = "skRoundRectangle";
                    break;
                case ShapeKind.Triangle:
                    result = "skTriangle";
                    break;
                default:
                    result = "skRectangle";
                    break;
            }
            return result;
        }

        #endregion // Public Methods
    }
}
