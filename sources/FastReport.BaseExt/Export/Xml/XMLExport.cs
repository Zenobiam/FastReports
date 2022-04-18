using System;
using System.Text;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using FastReport.Utils;
using System.Globalization;
using FastReport.Format;


namespace FastReport.Export.Xml
{
    /// <summary>
    /// Represents the Excel 2003 XML export filter.
    /// </summary>
    public partial class XMLExport : ExportBase
    {

        #region Constants
        private const int xLMaxHeight = 409;
        private const float xdivider = 1.376f;
        private const float ydivider = 1.333f;
        #endregion


        #region Private fields
        private bool pageBreaks;
        private string creator;
        private bool dataOnly;
        private bool wysiwyg;
        private Dictionary<string, XMLSheet> sheets;
        private ExportMatrix matrix;
        private XMLSheet sh;
        private bool splitPages;
        private List<ExportIEMStyle> commonStyles;
        #endregion

        private class XMLSheet
        {
            #region Constants
            private const float margDiv = 25.4F;
            #endregion

            #region Private fields
            private bool pageBreaks;
            private readonly string name;
            #endregion


            internal readonly ExportMatrix matrix;

            public void ExportSheet(Stream stream, StringBuilder builder)
            {
                int i, x, fx, fy, dx, dy;
                ExportIEMObject Obj;

                builder.AppendLine("<Worksheet ss:Name=\"" + name + "\">");

                // add table
                builder.Append("<Table ss:ExpandedColumnCount=\"").Append(matrix.Width.ToString()).Append("\"").
                    Append(" ss:ExpandedRowCount=\"").Append(matrix.Height.ToString()).AppendLine("\" x:FullColumns=\"1\" x:FullRows=\"1\">");
                for (x = 1; x < matrix.Width; x++)
                    builder.Append("<Column ss:AutoFitWidth=\"0\" ss:Width=\"").
                        Append(ExportUtils.FloatToString((matrix.XPosById(x) - matrix.XPosById(x - 1)) / xdivider)).
                        AppendLine("\"/>");
                WriteBuf(stream, builder);

                for (int y = 0; y < matrix.Height - 1; y++)
                {
                    builder.Append("<Row ss:Height=\"").
                        Append(ExportUtils.FloatToString((matrix.YPosById(y + 1) - matrix.YPosById(y)) / ydivider)).
                        AppendLine("\">");
                    for (x = 0; x < matrix.Width; x++)
                    {
                        i = matrix.Cell(x, y);
                        if (i != -1)
                        {
                            Obj = matrix.ObjectById(i);
                            if (Obj.Counter == 0)
                            {
                                builder.Append("<Cell ss:Index=\"").
                                    Append(Convert.ToString(x + 1) + "\" ");
                                matrix.ObjectPos(i, out fx, out fy, out dx, out dy);
                                Obj.Counter = 1;
                                if (Obj.IsText)
                                {
                                    if (dx > 1)
                                        builder.Append("ss:MergeAcross=\"").Append(Convert.ToString(dx++ - 1)).Append("\" ");
                                    if (dy > 1)
                                        builder.Append("ss:MergeDown=\"").Append(Convert.ToString(dy++ - 1)).Append("\" ");
                                    builder.Append("ss:StyleID=\"s").Append(Obj.StyleIndex.ToString()).AppendLine("\">");

                                    decimal value = 0;
                                    bool isNumeric = ExportUtils.ParseTextToDecimal(Obj.Text, Obj.Style.Format, out value);
                                    string type = isNumeric ? "ss:Type=\"Number\"" : "ss:Type=\"String\"";
                                    string data = Obj.HtmlTags ? "ss:Data" : "Data";
                                    string xmlns = Obj.HtmlTags ? " xmlns=\"http://www.w3.org/TR/REC-html40\"" : String.Empty;
                                    string strValue = isNumeric ?
                                        Convert.ToString(value, CultureInfo.InvariantCulture.NumberFormat) :
                                        ExportUtils.XmlString(Obj.Text, Obj.TextRenderType);
                                    builder.Append("<").Append(data).Append(" ").Append(type).Append(xmlns).
                                        Append(">").Append(strValue).Append("</").Append(data).Append("></Cell>");
                                }
                            }
                        }
                        else
                            builder.Append("<Cell ss:Index=\"").Append(Convert.ToString(x + 1)).Append("\"/>");
                    }
                    builder.AppendLine("</Row>");
                    if (builder.Length > 8192)
                        WriteBuf(stream, builder);
                }
                builder.AppendLine("</Table>");
                builder.AppendLine("<WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\">");
                if (matrix.PagesCount > 0)
                {
                    builder.AppendLine("<PageSetup>");
                    if (matrix.Landscape(0))
                        builder.AppendLine("<Layout x:Orientation=\"Landscape\"/>");

                    builder.AppendLine(string.Format(CultureInfo.InvariantCulture, "<PageMargins x:Bottom=\"{0:F2}\"" +
                                                      " x:Left=\"{1:F2}\"" +
                                                      " x:Right=\"{2:F2}\"" +
                                                      " x:Top=\"{3:F2}\"/>",
                                                      matrix.PageBMargin(0) / margDiv,
                                                      matrix.PageLMargin(0) / margDiv,
                                                      matrix.PageRMargin(0) / margDiv,
                                                      matrix.PageTMargin(0) / margDiv));
                    builder.AppendLine("</PageSetup>");
                    builder.AppendLine("<Print>");
                    builder.AppendLine("<ValidPrinterInfo/>");
                    builder.AppendLine("<PaperSizeIndex>" + matrix.RawPaperSize(0).ToString() + "</PaperSizeIndex>");
                    builder.AppendLine("</Print>");
                }
                builder.AppendLine("</WorksheetOptions>");
                // add page breaks
                if (pageBreaks)
                {
                    builder.AppendLine("<PageBreaks xmlns=\"urn:schemas-microsoft-com:office:excel\">");
                    builder.AppendLine("<RowBreaks>");
                    int page = 0;
                    for (i = 0; i <= matrix.Height - 1; i++)
                    {
                        if (matrix.YPosById(i) >= matrix.PageBreak(page))
                        {
                            builder.AppendLine("<RowBreak>");
                            builder.AppendLine(string.Format("<Row>{0}</Row>", i));
                            builder.AppendLine("</RowBreak>");
                            page++;
                        }
                    }
                    builder.AppendLine("</RowBreaks>");
                    builder.AppendLine("</PageBreaks>");
                }
                builder.AppendLine("</Worksheet>");
                WriteBuf(stream, builder);
            }


            public XMLSheet(ExportMatrix matrix, string name, bool pageBreaks)
            {
                this.matrix = matrix;
                this.name = name;
                this.pageBreaks = pageBreaks;
            }
        }


        #region Properties

        /// <summary>
        /// Gets or sets a value that determines whether to insert page breaks in the output file or not.
        /// </summary>
        /// 
        public bool PageBreaks {
            get { return pageBreaks; }
            set { pageBreaks = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the wysiwyg mode should be used 
        /// for better results.
        /// </summary>
        /// <remarks>
        /// Default value is <b>true</b>. In wysiwyg mode, the resulting Excel file will look
        /// as close as possible to the prepared report. On the other side, it may have a lot 
        /// of small rows/columns, which will make it less editable. If you set this property
        /// to <b>false</b>, the number of rows/columns in the resulting file will be decreased.
        /// You will get less wysiwyg, but more editable file.
        /// </remarks>
        public bool Wysiwyg {
            get { return wysiwyg; }
            set { wysiwyg = value; }
        }

        /// <summary>
        /// Gets or sets the name of document creator.
        /// </summary>
        public string Creator {
            get { return creator; }
            set { creator = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether to export the databand rows only.
        /// </summary>
        public bool DataOnly {
            get { return dataOnly; }
            set { dataOnly = value; }
        }

        /// <summary>
        /// Each report page is placed on a new Excel page.
        /// </summary>
        public bool SplitPages {
            get { return splitPages; }
            set { splitPages = value; }
        }

        #endregion

        #region Private Methods

        private static void WriteBuf(Stream stream, StringBuilder buf)
        {
            // write the resulting string to a stream
            byte[] bytes = Encoding.UTF8.GetBytes(buf.ToString());
            stream.Write(bytes, 0, bytes.Length);
            buf.Length = 0;
        }


        private void ExportHeader(StringBuilder builder)
        {
            builder.AppendLine("<?xml version=\"1.0\"?>");
            builder.AppendLine("<?mso-application progid=\"Excel.Sheet\"?>");
            builder.Append("<?fr-application created=\"").Append(creator).AppendLine("\"?>");
            builder.AppendLine("<?fr-application homesite=\"http://www.fast-report.com\"?>");
            builder.Append("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"");
            builder.Append(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
            builder.Append(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
            builder.Append(" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"");
            builder.AppendLine(" xmlns:html=\"http://www.w3.org/TR/REC-html40\">");
            builder.AppendLine("<DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">");
            builder.Append("<Title>").Append(Report.ReportInfo.Name).AppendLine("</Title>");
            builder.Append("<Author>").Append(Report.ReportInfo.Author).AppendLine("</Author>");
            builder.Append("<Created>").Append(SystemFake.DateTime.Now.Date.ToString()).Append("T").Append(SystemFake.DateTime.Now.TimeOfDay.ToString()).AppendLine("Z</Created>");
            builder.Append("<Version>").Append(Report.ReportInfo.Version).AppendLine("</Version>");
            builder.AppendLine("</DocumentProperties>");
            builder.AppendLine("<ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">");
            builder.AppendLine("<ProtectStructure>False</ProtectStructure>");
            builder.AppendLine("<ProtectWindows>False</ProtectWindows>");
            builder.AppendLine("</ExcelWorkbook>");
        }

        private void ExportSheets(Stream stream, StringBuilder builder)
        {
            foreach (XMLSheet sheet in sheets.Values)
                sheet.ExportSheet(stream, builder);

            builder.AppendLine("</Workbook>");
            WriteBuf(stream, builder);
        }

        private void ExportXML(Stream stream)
        {
            StringBuilder builder = new StringBuilder(8448);

            ExportHeader(builder);

            ExportStyles(builder);

            ExportSheets(stream, builder);
        }
        #endregion

        #region StyleMethods
        internal void ExportStyles(StringBuilder builder)
        {
            builder.AppendLine("<Styles>");

            ExportIEMStyle EStyle;
            for (int x = 0; x < commonStyles.Count; x++)
            {
                EStyle = commonStyles[x];
                builder.Append("<Style ss:ID=\"s").Append(x.ToString()).AppendLine("\">");
                builder.AppendLine(GetXMLFont(EStyle));
                builder.AppendLine(GetXMLInterior(EStyle));
                builder.AppendLine(GetXMLAlignment(EStyle));
                builder.AppendLine(GetXMLBorders(EStyle));
                builder.Append(GetXMLFormat(EStyle));
                builder.AppendLine("</Style>");
            }

            builder.AppendLine("</Styles>");
        }

        private string GetXMLWeight(float lineWeight)
        {
            float LineWeight = lineWeight * xdivider;
            return ((int)Math.Round(LineWeight > 3 ? 3 : LineWeight)).ToString();
        }

        private string XmlAlign(HorzAlign horzAlign, VertAlign vertAlign, int angle)
        {
            string Fh = "Left", Fv = "Top";
            if (angle == 0 || angle == 180)
            {
                if (horzAlign == HorzAlign.Left)
                    Fh = "Left";
                else if (horzAlign == HorzAlign.Right)
                    Fh = "Right";
                else if (horzAlign == HorzAlign.Center)
                    Fh = "Center";
                else if (horzAlign == HorzAlign.Justify)
                    Fh = "Justify";
                if (vertAlign == VertAlign.Top)
                    Fv = "Top";
                else if (vertAlign == VertAlign.Bottom)
                    Fv = "Bottom";
                else if (vertAlign == VertAlign.Center)
                    Fv = "Center";
            }
            else if (angle == 90)
            {
                if (horzAlign == HorzAlign.Left)
                    Fv = "Top";
                else if (horzAlign == HorzAlign.Right)
                    Fv = "Bottom";
                else if (horzAlign == HorzAlign.Center)
                    Fv = "Center";
                if (vertAlign == VertAlign.Top)
                    Fh = "Right";
                else if (vertAlign == VertAlign.Bottom)
                    Fh = "Left";
                else if (vertAlign == VertAlign.Center)
                    Fh = "Center";
            }
            else
            {
                if (horzAlign == HorzAlign.Left)
                    Fv = "Bottom";
                else if (horzAlign == HorzAlign.Right)
                    Fv = "Top";
                else if (horzAlign == HorzAlign.Center)
                    Fv = "Center";
                if (vertAlign == VertAlign.Top)
                    Fh = "Right";
                else if (vertAlign == VertAlign.Bottom)
                    Fh = "Left";
                else if (vertAlign == VertAlign.Center)
                    Fh = "Center";
            }
            return "ss:Horizontal=\"" + Fh + "\" ss:Vertical=\"" + Fv + "\"";
        }


        private string GetXMLBorders(ExportIEMStyle style)
        {
            StringBuilder result = new StringBuilder(128);
            result.AppendLine("<Borders>");
            if ((style.Border.Lines & BorderLines.Left) > 0)
                result.Append("<Border ss:Position=\"Left\" ").
                    Append("ss:LineStyle=\"").
                    AppendFormat(GetXMLLineStyle(style.Border.LeftLine.Style)).
                    Append("\" ").
                    Append("ss:Weight=\"").
                    Append(GetXMLWeight(style.Border.LeftLine.Width)).
                    Append("\" ").
                    Append("ss:Color=\"").
                    Append(ExportUtils.HTMLColorCode(style.Border.LeftLine.Color)).
                    AppendLine("\"/>");
            if ((style.Border.Lines & BorderLines.Top) > 0)
                result.Append("<Border ss:Position=\"Top\" ").
                    Append("ss:LineStyle=\"").
                    AppendFormat(GetXMLLineStyle(style.Border.TopLine.Style)).
                    Append("\" ").
                    Append("ss:Weight=\"").
                    Append(GetXMLWeight(style.Border.TopLine.Width)).
                    Append("\" ").
                    Append("ss:Color=\"").
                    Append(ExportUtils.HTMLColorCode(style.Border.TopLine.Color)).
                    AppendLine("\"/>");
            if ((style.Border.Lines & BorderLines.Bottom) > 0)
                result.AppendLine("<Border ss:Position=\"Bottom\" ").
                    Append("ss:LineStyle=\"").
                    AppendFormat(GetXMLLineStyle(style.Border.BottomLine.Style)).
                    Append("\" ").
                    Append("ss:Weight=\"").
                    Append(GetXMLWeight(style.Border.BottomLine.Width)).
                    Append("\" ").
                    Append("ss:Color=\"").
                    Append(ExportUtils.HTMLColorCode(style.Border.BottomLine.Color)).
                    AppendLine("\"/>");
            if ((style.Border.Lines & BorderLines.Right) > 0)
                result.AppendLine("<Border ss:Position=\"Right\" ").
                    Append("ss:LineStyle=\"").
                    AppendFormat(GetXMLLineStyle(style.Border.RightLine.Style)).
                    Append("\" ").
                    Append("ss:Weight=\"").
                    Append(GetXMLWeight(style.Border.RightLine.Width)).
                    Append("\" ").
                    Append("ss:Color=\"").
                    Append(ExportUtils.HTMLColorCode(style.Border.RightLine.Color)).
                    AppendLine("\"/>");
            result.Append("</Borders>");
            return result.ToString();
        }

        private string GetXMLFont(ExportIEMStyle style)
        {
            StringBuilder result = new StringBuilder(128);
            result.Append("<Font ss:FontName=\"").Append(style.Font.Name).Append("\" ss:Size=\"").
                Append(ExportUtils.FloatToString(style.Font.Size)).Append("\" ss:Color=\"").
                Append(ExportUtils.HTMLColorCode(style.TextColor)).Append("\" ").
                Append(((style.Font.Style & FontStyle.Bold) > 0 ? "ss:Bold=\"1\" " : String.Empty)).
                Append(((style.Font.Style & FontStyle.Italic) > 0 ? "ss:Italic=\"1\" " : String.Empty)).
                Append(((style.Font.Style & FontStyle.Underline) > 0 ? "ss:Underline=\"Single\" " : String.Empty)).
                Append("/>");
            return result.ToString();
        }

        private string GetXMLInterior(ExportIEMStyle style)
        {
            if (style.FillColor.A != 0)
                return "<Interior ss:Color=\"" +
                    ExportUtils.HTMLColorCode(style.FillColor) + "\" ss:Pattern=\"Solid\"/>";
            return String.Empty;
        }

        private string GetXMLAlignment(ExportIEMStyle style)
        {
            StringBuilder result = new StringBuilder(64);
            result.Append("<Alignment ").Append(XmlAlign(style.HAlign, style.VAlign, style.Angle)).
                Append(" ss:WrapText=\"1\" ").
                Append(((style.Angle > 0 && style.Angle <= 90) ? "ss:Rotate=\"" + (-style.Angle).ToString() + "\"" : String.Empty)).
                Append(((style.Angle > 90 && style.Angle <= 180) ? "ss:Rotate=\"" + (180 - style.Angle).ToString() + "\"" : String.Empty)).
                Append(((style.Angle > 180 && style.Angle < 270) ? "ss:Rotate=\"" + (270 - style.Angle).ToString() + "\"" : String.Empty)).
                Append(((style.Angle >= 270 && style.Angle < 360) ? "ss:Rotate=\"" + (360 - style.Angle).ToString() + "\"" : String.Empty)).
                Append("/>");
            return result.ToString();
        }

        private string GetXMLLineStyle(LineStyle style)
        {
            switch (style)
            {
                case LineStyle.Dash:
                    return "Dash";
                case LineStyle.DashDot:
                    return "DashDot";
                case LineStyle.DashDotDot:
                    return "DashDotDot";
                case LineStyle.Dot:
                    return "Dot";
                case LineStyle.Double:
                    return "Double";
                default:
                    return "Continuous";
            }
        }

        private string GetXMLFormat(ExportIEMStyle style)
        {
            if (style.Format is NumberFormat || style.Format is CurrencyFormat)
            {
                return "<NumberFormat ss:Format=\"" + ExportUtils.GetExcelFormatSpecifier(style.Format) + "\"/>\r\n";
            }
            return String.Empty;
        }

        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            return new MyRes("FileFilters").Get("XlsFile");
        }

        /// <inheritdoc/>
        protected override void Start()
        {
            base.Start();

            sheets = new Dictionary<string, XMLSheet>();
            commonStyles = new List<ExportIEMStyle>();
        }

        /// <inheritdoc/>
        protected override void ExportPageBegin(ReportPage page)
        {
            base.ExportPageBegin(page);
            if (sheets.ContainsKey(page.Name) && !SplitPages)
            {
                sh = sheets[page.Name] as XMLSheet;
                sh.matrix.AddPageBegin(page);
            }
            else
            {
                matrix = new ExportMatrix();
                if (Wysiwyg)
                    matrix.Inaccuracy = 0.5f;
                else
                    matrix.Inaccuracy = 10;
                matrix.RotatedAsImage = false;
                matrix.PlainRich = true;
                matrix.AreaFill = true;
                matrix.CropAreaFill = true;
                matrix.Report = Report;
                matrix.Images = false;
                matrix.DataOnly = DataOnly;
                matrix.ShowProgress = ShowProgress;
                matrix.MaxCellHeight = ydivider * xLMaxHeight;
                matrix.AddPageBegin(page);
                matrix.Styles = commonStyles;
            }
        }

        /// <inheritdoc/>
        protected override void ExportBand(Base band)
        {
            base.ExportBand(band);
            if (sheets.ContainsKey(band.Page.Name) && !SplitPages)
            {
                sh = sheets[band.Page.Name] as XMLSheet;
                sh.matrix.AddBand(band, this);
            }
            else
            {
                matrix.AddBand(band, this);
            }
        }

        /// <inheritdoc/>
        protected override void ExportPageEnd(ReportPage page)
        {
            if (sheets.ContainsKey(page.Name) && !SplitPages)
            {
                sh = sheets[page.Name] as XMLSheet;
                sh.matrix.AddPageEnd(page);
            }
            else
            {
                string new_page_name = page.Name;

                if (SplitPages)
                {
                    if (sheets.ContainsKey(page.Name + "-1"))
                    {
                        int repeats = 2;
                        while (sheets.ContainsKey(page.Name + "-" + repeats.ToString()))
                            repeats++;

                        new_page_name += "-" + repeats.ToString();
                    }
                    else if (Report.PreparedPages.GetPage(sheets.Count + 1) != null &&
                        Report.PreparedPages.GetPage(sheets.Count + 1).Name == page.Name)
                    {
                        new_page_name += "-1";
                    }
                }

                matrix.AddPageEnd(page);
                sh = new XMLSheet(matrix, new_page_name, pageBreaks);
                sheets[new_page_name] = sh;
            }
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            foreach (XMLSheet sheet in sheets.Values)
                sheet.matrix.Prepare();

            MyRes Res = new MyRes("Export,Misc");
            if (ShowProgress)
                Config.ReportSettings.OnProgress(Report, Res.Get("SaveFile"));
            ExportXML(Stream);
        }

        #endregion

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            base.Serialize(writer);
            writer.WriteBool("Wysiwyg", Wysiwyg);
            writer.WriteBool("PageBreaks", PageBreaks);
            writer.WriteBool("DataOnly", DataOnly);
            writer.WriteBool("SplitPages", SplitPages);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XMLExport"/> class.
        /// </summary>       
        public XMLExport()
        {
            pageBreaks = true;
            Wysiwyg = true;
            creator = "FastReport";
        }
    }
}
