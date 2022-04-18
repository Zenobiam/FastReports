using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FastReport.Export.Odf
{
    /// <summary>
    /// Base class for any ODF exports.
    /// </summary>
    public partial class ODFExport : ExportBase
    {
        #region Constants

        private const float odfDivider = 37.82f;
        private const float odfMargDiv = 10f;
        private const float odfPageDiv = 10f;
        #endregion Constants

        /// <summary>
        /// Enum of OpenOffice formats.
        /// </summary>
        public enum OpenOfficeFormat
        {
            /// <summary>
            /// OpenOffice Spreadsheet format.
            /// </summary>
            Spreadsheet,

            /// <summary>
            /// OpenOffice Writer format.
            /// </summary>
            Writer
        }

        /// <summary>
        /// Standard of ODF format.
        /// </summary>
        public enum OdfStandard
        {
            /// <summary>
            /// ODF 1.0/1.1
            /// </summary>
            None = 0,

            /// <summary>
            /// ODF 1.2
            /// </summary>
            Odf1_2 = 1,

            /// <summary>
            /// XODF 1.0/1.1
            /// </summary>
            Xodf1_1 = 2,

            /// <summary>
            /// XODF 1.2
            /// </summary>
            Xodf1_2 = 3
        }

        #region Private fields

        private string creator;
        private OpenOfficeFormat exportType;
        private bool firstPage;
        private ExportMatrix matrix;
        private float pageBottom;
        private bool pageBreaks;
        private float pageHeight;
        private bool pageLandscape;
        private float pageLeft;
        private float pageRight;
        private float pageTop;
        private float pageWidth;
        private bool wysiwyg;

        private OdfStandard odfCompliance = OdfStandard.None;
        private bool isXODT = false;
        private bool useTagText = false;
        private bool writeVersion = false;
        private bool extendedStyles = false;
        private bool writeRdf = false;
        #endregion Private fields

        #region Properties

        /// <summary>
        /// Creator of the document
        /// </summary>
        public string Creator
        {
            get { return creator; }
            set { creator = value; }
        }

        /// <summary>
        /// Is XODT format
        /// </summary>
        public bool IsXOTD
        {
            get { return isXODT; }
            set { isXODT = value; }
        }

        /// <summary>
        /// Switch of page breaks
        /// </summary>
        public bool PageBreaks
        {
            get { return pageBreaks; }
            set { pageBreaks = value; }
        }

        /// <summary>
        /// Wysiwyg mode, set for better results
        /// </summary>
        public bool Wysiwyg
        {
            get { return wysiwyg; }
            set { wysiwyg = value; }
        }

        internal OpenOfficeFormat ExportType
        {
            get { return exportType; }
            set { exportType = value; }
        }

        /// <summary>
        /// Gets or sets ODF Compliance standard.
        /// </summary>
        public OdfStandard OdfCompliance
        {
            get
            {
                return odfCompliance;
            }
            set
            {
                odfCompliance = value;
                switch (odfCompliance)
                {
                    case OdfStandard.Xodf1_1:
                        useTagText = true;
                        isXODT = true;
                        writeVersion = false;
                        extendedStyles = false;
                        writeRdf = false;
                        break;
                    case OdfStandard.None:
                        writeVersion = false;
                        extendedStyles = false;
                        writeRdf = false;
                        break;
                    case OdfStandard.Xodf1_2:
                        useTagText = true;
                        isXODT = true;
                        writeVersion = true;
                        extendedStyles = true;
                        writeRdf = true;
                        break;
                    case OdfStandard.Odf1_2:
                        writeVersion = true;
                        extendedStyles = true;
                        writeRdf = true;
                        break;

                }
            }
        }
        #endregion Properties

        #region Private Methods

        private string GetOdfVersion()
        {
            switch (OdfCompliance)
            {
                case OdfStandard.Xodf1_2:
                case OdfStandard.Odf1_2:
                    return "1.2";
                case OdfStandard.Xodf1_1:
                case OdfStandard.None:
                default:
                    return "1.0";
            }
        }

        private int ExportCell(Stream file, ZipArchive zip, ExportIEMObject obj, int dx, int dy, ref int picCount)
        {
            Write(file, String.Format("<table:table-cell table:style-name=\"ce{0}\" office:value-type=\"string\" ",
                obj.StyleIndex.ToString()));
            if (dx > 1 || dy > 1)
            {
                Write(file, String.Format("table:number-columns-spanned=\"{0}\" ", dx.ToString()));
                Write(file, String.Format("table:number-rows-spanned=\"{0}\" ", dy.ToString()));
            }
            WriteLine(file, ">");
            if (obj.IsText)
            {
                // text
                ExportText(file, obj);
            }
            else if (obj.Width > 0)
            {
                // picture
                ExportPicture(file, zip, obj, ++picCount);
            }

            WriteLine(file, "</table:table-cell>");
            return picCount;
        }

        private void ExportODF(Stream stream)
        {
            ZipArchive zip = new ZipArchive();

            string ExportMime = exportType == OpenOfficeFormat.Spreadsheet ? "spreadsheet" : "text";
            OdfCreateMime(zip, "mimetype", ExportMime);
            OdfMakeDocStyles(zip, "styles.xml");

            #region Content.xml

            MemoryStream file = new MemoryStream();
            WriteLine(file, "<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            Write(file, "<office:document-content ");
            Write(file, OdfMakeXmlHeader());
            if (writeVersion) Write(file, $" office:version=\"{GetOdfVersion()}\"");
            WriteLine(file, ">");
            WriteLine(file, "<office:scripts/>");
            OdfFontFaceDecals(file);

            OdfAutomaticStyles(file);

            // body
            WriteLine(file, "<office:body>");
            if(!useTagText)
                WriteLine(file, "<office:spreadsheet>");
            else
                WriteLine(file, "<office:text>");

            TableBegin(file, 1);

            // rows
            int picCount = OdfRows(file, zip);

            TableEnd(file);

            if (!useTagText)
                WriteLine(file, "</office:spreadsheet>");
            else
                WriteLine(file, "</office:text>");

            WriteLine(file, "</office:body>");
            WriteLine(file, "</office:document-content>");
            zip.AddStream("content.xml", file);

            #endregion Content.xml

            OdfCreateManifest(zip, "META-INF/manifest.xml", picCount, ExportMime);
            OdfCreateMeta(zip, "meta.xml", Creator);
            if (writeRdf) OdfCreateRDF(zip, "manifest.rdf");

            zip.SaveToStream(Stream);
            zip.Clear();
        }

        private void ExportPicture(Stream file, ZipArchive zip, ExportIEMObject obj, int picCount)
        {
            string picName = picCount.ToString() + ".png";
            zip.AddStream("Pictures/Pic" + picName, obj.PictureStream);
            if (exportType == OpenOfficeFormat.Writer)
                Write(file, "<text:p>");

            Write(file,
                String.Format("<draw:frame text:anchor-type=\"frame\" draw:z-index=\"{0}\" draw:name=\"Pictures{1}\" draw:style-name=\"gr1\" svg:width=\"{2}cm\" svg:height=\"{3}cm\" svg:x=\"0cm\" svg:y=\"0cm\">",
                (picCount - 1).ToString(),
                picCount.ToString(),
                GetStringValue(obj.Width),
                GetStringValue(obj.Height)
                ));

            Write(file,
                String.Format("<draw:image xlink:href=\"Pictures/Pic{0}\" xlink:type=\"simple\" xlink:show=\"embed\" xlink:actuate=\"onLoad\"/>",
                picName
                ));

            Write(file, "</draw:frame>");
            if (exportType == OpenOfficeFormat.Writer)
                WriteLine(file, "</text:p>");
        }

        private void ExportText(Stream file, ExportIEMObject obj)
        {
            Write(file, "<text:p");
            if (exportType == OpenOfficeFormat.Writer)
                Write(file,
                String.Format(" text:style-name=\"P{0}\"", obj.StyleIndex.ToString()));
            Write(file, ">");
            WriteLine(file,
            String.Format("{0}</text:p>",
            ExportUtils.OdtString(obj.Text, obj.TextRenderType)));

        }

        private string GetBorderLineStyle(BorderLine line, string name)
        {
            return String.Format("fo:border-{0}=\"{1}cm {2} {3}\" ",
                name,
                GetStringValue(line.Width),
                OdfGetFrameName(line.Style),
                ExportUtils.HTMLColorCode(line.Color));
        }

        private string GetStringValue(float value)
        {
            return ExportUtils.FloatToString(value / odfDivider);
        }

        private void HorizAlignStyle(Stream file, HorzAlign hAlign)
        {
            if (hAlign == HorzAlign.Left)
                Write(file, "fo:text-align=\"start\" ");
            else if (hAlign == HorzAlign.Center)
                Write(file, "fo:text-align=\"center\" ");
            else if (hAlign == HorzAlign.Right)
                Write(file, "fo:text-align=\"end\" ");
            else if (hAlign == HorzAlign.Justify)
                Write(file, "fo:text-align=\"justify\" ");
        }

        private void MarginStyle(Stream file, Padding padding)
        {
            if (padding.Left > 0)
                Write(file,
                    String.Format("fo:margin-left=\"{0}cm\" ",
                    GetStringValue(padding.Left)));
            if (padding.Right > 0)
                Write(file,
                    String.Format("fo:margin-right=\"{0}cm\" ",
                    GetStringValue(padding.Right)));
            if (padding.Top > 0)
                Write(file,
                    String.Format("fo:margin-top=\"{0}cm\" ",
                    GetStringValue(padding.Top)));
            if (padding.Bottom > 0)
                Write(file,
                    String.Format("fo:margin-bottom=\"{0}cm\" ",
                    GetStringValue(padding.Bottom)));
        }

        private void OdfAutomaticStyles(Stream file)
        {
            WriteLine(file, "<office:automatic-styles>");
            OdfColumnStyles(file);
            OdfRowStyles(file);
            OdfStyles(file);

            if (exportType == OpenOfficeFormat.Writer)
            {
                WriterStyles(file);
            }

            WriteLine(file, "<style:style style:name=\"gr1\" style:family=\"graphic\">");
            Write(file, "<style:graphic-properties draw:stroke=\"none\" " +
                "draw:fill=\"none\" draw:textarea-horizontal-align=\"left\" " +
                "draw:textarea-vertical-align=\"top\" draw:color-mode=\"standard\" " +
                "draw:luminance=\"0%\" draw:contrast=\"0%\" draw:gamma=\"100%\" " +
                "draw:red=\"0%\" draw:green=\"0%\" draw:blue=\"0%\" " +
                "fo:clip=\"rect(0cm, 0cm, 0cm, 0cm)\" draw:image-opacity=\"100%\" " +
                "style:mirror=\"none\"");
            if (extendedStyles) Write(file, " style:run-through=\"background\" style:vertical-pos=\"from-top\" " +
                 "style:horizontal-pos=\"from-left\" style:horizontal-rel=\"paragraph\" " +
                 "draw:wrap-influence-on-position=\"once-concurrent\" " +
                 "style:flow-with-text=\"false\"");
            WriteLine(file, "/>");
                
            WriteLine(file, "</style:style>");

            WriteLine(file, "</office:automatic-styles>");
        }

        private void OdfColumns(Stream file)
        {
            for (int x = 1; x < matrix.Width; x++)
                WriteLine(file,
                    String.Format("<table:table-column table:style-name=\"co{0}\"/>",
                    GetStringValue((matrix.XPosById(x) - matrix.XPosById(x - 1)))
                    ));
        }

        private void OdfColumnStyles(Stream file)
        {
            List<string> fList = new List<string>();
            for (int i = 1; i < matrix.Width; i++)
            {
                string s = GetStringValue((matrix.XPosById(i) - matrix.XPosById(i - 1)));
                if (fList.IndexOf(s) == -1)
                    fList.Add(s);
            }
            fList.Sort();
            for (int i = 0; i < fList.Count; i++)
            {
                WriteLine(file,
                    String.Format(
                        "<style:style style:name=\"co{0}\" style:family=\"table-column\">",
                        fList[i]
                    ));
                WriteLine(file,
                    String.Format(
                        "<style:table-column-properties fo:break-before=\"auto\" style:column-width=\"{0}cm\"/></style:style>",
                        fList[i]));
            }
        }

        private void OdfCreateManifest(ZipArchive zip, string fileName, int PicCount, string MValue)
        {
            MemoryStream file = new MemoryStream();
            WriteLine(file, "<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            Write(file, "<manifest:manifest xmlns:manifest=\"urn:oasis:names:tc:opendocument:xmlns:manifest:1.0\"");
            if (writeVersion) Write(file, $" manifest:version=\"{GetOdfVersion()}\"");
            WriteLine(file, ">");
            WriteLine(file, String.Format(" <manifest:file-entry manifest:media-type=\"application/vnd.oasis.opendocument.{0}\" manifest:full-path=\"/\"/>", MValue));
            WriteLine(file, " <manifest:file-entry manifest:media-type=\"text/xml\" manifest:full-path=\"content.xml\"/>");
            WriteLine(file, " <manifest:file-entry manifest:media-type=\"text/xml\" manifest:full-path=\"styles.xml\"/>");
            if(writeRdf) WriteLine(file, " <manifest:file-entry manifest:media-type=\"application/rdf+xml\" manifest:full-path=\"manifest.rdf\"/>");
            WriteLine(file, " <manifest:file-entry manifest:media-type=\"text/xml\" manifest:full-path=\"meta.xml\"/>");
            for (int i = 1; i <= PicCount; i++)
                WriteLine(file, String.Format(" <manifest:file-entry manifest:media-type=\"image/png\" manifest:full-path=\"Pictures/Pic{0}.png\"/>", i.ToString()));
            WriteLine(file, "</manifest:manifest>");
            zip.AddStream(fileName, file);
        }

        private void OdfCreateMeta(ZipArchive zip, string fileName, string Creator)
        {
            StringBuilder sb = new StringBuilder(570);
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.Append("<office:document-meta xmlns:office=\"urn:oasis:names:tc:opendocument:xmlns:office:1.0\" ").
                Append("xmlns:xlink=\"http://www.w3.org/1999/xlink\" ").
                Append("xmlns:dc=\"http://purl.org/dc/elements/1.1/\" ").
                Append("xmlns:meta=\"urn:oasis:names:tc:opendocument:xmlns:meta:1.0\"");
            if (writeVersion) sb.Append($" office:version=\"{GetOdfVersion()}\"");
            sb.AppendLine(">");
            sb.AppendLine("  <office:meta>");
            sb.Append("    <meta:generator>fast-report.com/Fast Report.NET/build:").Append(Config.Version).AppendLine("</meta:generator>");
            sb.Append("    <meta:initial-creator>").Append(ExportUtils.XmlString(Creator, TextRenderType.Default)).AppendLine("</meta:initial-creator>");
            sb.Append("    <meta:creation-date>").Append(SystemFake.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")).AppendLine("</meta:creation-date>");
            sb.AppendLine("  </office:meta>");
            sb.AppendLine("</office:document-meta>");
            MemoryStream file = new MemoryStream();
            Write(file, sb.ToString());
            zip.AddStream(fileName, file);
        }

        private void OdfCreateRDF(ZipArchive zip, string fileName)
        {
            MemoryStream file = new MemoryStream();
            WriteLine(file, "<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            WriteLine(file, "<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">");
            WriteLine(file, "  <rdf:Description rdf:about=\"styles.xml\">");
            WriteLine(file, "    <rdf:type rdf:resource=\"http://docs.oasis-open.org/ns/office/1.2/meta/odf#StylesFile\"/>");
            WriteLine(file, "  </rdf:Description>");

            WriteLine(file, "  <rdf:Description rdf:about=\"\">");
            WriteLine(file, "    <ns0:hasPart xmlns:ns0=\"http://docs.oasis-open.org/ns/office/1.2/meta/pkg#\" rdf:resource=\"styles.xml\"/>");
            WriteLine(file, "  </rdf:Description>");

            WriteLine(file, "  <rdf:Description rdf:about=\"content.xml\">");
            WriteLine(file, "    <rdf:type rdf:resource=\"http://docs.oasis-open.org/ns/office/1.2/meta/odf#ContentFile\"/>");
            WriteLine(file, "  </rdf:Description>");

            WriteLine(file, "  <rdf:Description rdf:about=\"\">");
            WriteLine(file, "    <ns0:hasPart xmlns:ns0=\"http://docs.oasis-open.org/ns/office/1.2/meta/pkg#\" rdf:resource=\"content.xml\"/>");
            WriteLine(file, "  </rdf:Description>");

            WriteLine(file, "  <rdf:Description rdf:about=\"\">");
            WriteLine(file, "    <rdf:type rdf:resource=\"http://docs.oasis-open.org/ns/office/1.2/meta/pkg#Document\"/>");
            WriteLine(file, "  </rdf:Description>");
            WriteLine(file, "</rdf:RDF>");
            zip.AddStream(fileName, file);
        }

        private void OdfCreateMime(ZipArchive zip, string fileName, string MValue)
        {
            MemoryStream file = new MemoryStream();
            Write(file, String.Concat("application/vnd.oasis.opendocument.", MValue));
            zip.AddStream(fileName, file);
        }

        private void OdfFontFaceDecals(Stream file)
        {
            List<string> fList = new List<string>();
            for (int i = 0; i < matrix.StylesCount; i++)
            {
                ExportIEMStyle style = matrix.StyleById(i);
                if ((style.Font != null) && (fList.IndexOf(style.Font.Name) == -1))
                    fList.Add(style.Font.Name);
            }
            WriteLine(file, "<office:font-face-decls>");
            fList.Sort();
            for (int i = 0; i < fList.Count; i++)
            {
                WriteLine(file,
                    String.Format(
                        "<style:font-face style:name=\"{0}\" svg:font-family=\"&apos;{0}&apos;\" style:font-pitch=\"variable\"/>",
                        fList[i]
                    ));
            }
            WriteLine(file, "</office:font-face-decls>");
        }

        private string OdfGetFrameName(LineStyle style)
        {
            switch (style)
            {
                case LineStyle.Dash:
                case LineStyle.DashDot:
                case LineStyle.DashDotDot:
                case LineStyle.Dot:
                    return "solid";

                case LineStyle.Double:
                    return "double";

                default:
                    return "solid";
            }
        }

        private void OdfMakeDocStyles(ZipArchive zip, string fileName)
        {
            MemoryStream file = new MemoryStream();
            WriteLine(file, "<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            Write(file, "<office:document-styles ");
            Write(file, OdfMakeXmlHeader());
            if (writeVersion) Write(file, $" office:version=\"{GetOdfVersion()}\"");
            WriteLine(file, ">");
            WriteLine(file, "<office:automatic-styles>");
            WriteLine(file, "<style:page-layout style:name=\"pm1\">");
            WriteLine(file,
                String.Format("<style:page-layout-properties fo:page-width=\"{0}cm\" fo:page-height=\"{1}cm\" fo:margin-top=\"{2}cm\" fo:margin-bottom=\"{3}cm\" fo:margin-left=\"{4}cm\" fo:margin-right=\"{5}cm\"/>",
                ExportUtils.FloatToString(pageWidth / odfPageDiv),
                ExportUtils.FloatToString(pageHeight / odfPageDiv),
                ExportUtils.FloatToString(pageTop / odfMargDiv),
                ExportUtils.FloatToString(pageBottom / odfMargDiv),
                ExportUtils.FloatToString(pageLeft / odfMargDiv),
                ExportUtils.FloatToString(pageRight / odfMargDiv)
                ));
            WriteLine(file, "</style:page-layout>");
            WriteLine(file, "</office:automatic-styles>");
            WriteLine(file, "<office:master-styles>");
            WriteLine(file, "<style:master-page style:name=\"PageDef\" style:page-layout-name=\"pm1\">");
            WriteLine(file, "</style:master-page>");
            WriteLine(file, "</office:master-styles>");
            WriteLine(file, "</office:document-styles>");
            zip.AddStream(fileName, file);
        }

        private string OdfMakeXmlHeader()
        {
            return " xmlns:office=\"urn:oasis:names:tc:opendocument:xmlns:office:1.0\"" +
                " xmlns:style=\"urn:oasis:names:tc:opendocument:xmlns:style:1.0\"" +
                " xmlns:text=\"urn:oasis:names:tc:opendocument:xmlns:text:1.0\"" +
                " xmlns:table=\"urn:oasis:names:tc:opendocument:xmlns:table:1.0\"" +
                " xmlns:draw=\"urn:oasis:names:tc:opendocument:xmlns:drawing:1.0\"" +
                " xmlns:fo=\"urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0\"" +
                " xmlns:xlink=\"http://www.w3.org/1999/xlink\"" +
                " xmlns:dc=\"http://purl.org/dc/elements/1.1/\"" +
                " xmlns:meta=\"urn:oasis:names:tc:opendocument:xmlns:meta:1.0\"" +
                " xmlns:number=\"urn:oasis:names:tc:opendocument:xmlns:datastyle:1.0\"" +
                " xmlns:svg=\"urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0\"" +
                " xmlns:chart=\"urn:oasis:names:tc:opendocument:xmlns:chart:1.0\"" +
                " xmlns:dr3d=\"urn:oasis:names:tc:opendocument:xmlns:dr3d:1.0\"" +
                " xmlns:math=\"http://www.w3.org/1998/Math/MathML\"" +
                " xmlns:form=\"urn:oasis:names:tc:opendocument:xmlns:form:1.0\"" +
                " xmlns:script=\"urn:oasis:names:tc:opendocument:xmlns:script:1.0\"" +
                " xmlns:dom=\"http://www.w3.org/2001/xml-events\"" +
                " xmlns:xforms=\"http://www.w3.org/2002/xforms\"" +
                " xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"" +
                " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"";
        }

        private int OdfRows(Stream file, ZipArchive zip)
        {
            int picCount = 0;
            int page = 0;

            for (int y = 0; y < matrix.Height - 1; y++)
            {
                string pageB = String.Empty;
                if ((pageBreaks) && (matrix.YPosById(y) >= matrix.PageBreak(page)))
                {
                    page++;
                    pageB = "pb";
                    TableEnd(file);
                    TableBegin(file, page + 1);
                }

                WriteLine(file,
                    String.Format("<table:table-row table:style-name=\"ro{0}{1}\">",
                    GetStringValue((matrix.YPosById(y + 1) - matrix.YPosById(y))),
                    pageB));

                for (int x = 0; x < matrix.Width - 1; x++)
                {
                    int i = matrix.Cell(x, y);
                    if (i != -1)
                    {
                        ExportIEMObject obj = matrix.ObjectById(i);
                        if (obj.Counter == 0)
                        {
                            obj.Counter = 1;
                            int fx, fy, dx, dy;
                            matrix.ObjectPos(i, out fx, out fy, out dx, out dy);
                            ExportCell(file, zip, obj, dx, dy, ref picCount);
                        }
                        else
                        {
                            Write(file, "<table:covered-table-cell/>");
                        }
                    }
                    else
                    {
                        Write(file, "<table:table-cell");
                        if (exportType == OpenOfficeFormat.Writer)
                            WriteLine(file, "><text:p text:style-name=\"PB\"/></table:table-cell>");
                        else
                            WriteLine(file, "/>");
                    }
                }
                WriteLine(file, "</table:table-row>");
            }
            return picCount;
        }

        private void OdfRowStyles(Stream file)
        {
            List<KeyValuePair<float, bool>> positions = new List<KeyValuePair<float, bool>>();

            int page = 0;
            for (int i = 0; i < matrix.Height - 1; i++)
            {
                bool breakValue = false;
                if ((pageBreaks) && (matrix.YPosById(i) >= matrix.PageBreak(page)))
                {
                    page++;
                    breakValue = true;
                }
                KeyValuePair<float, bool> value =
                    new KeyValuePair<float, bool>(matrix.YPosById(i + 1) - matrix.YPosById(i), breakValue);
                if (positions.IndexOf(value) == -1)
                {
                    positions.Add(value);
                }
            }

            for (int i = 0; i < positions.Count; i++)
            {
                string rowPos = GetStringValue(positions[i].Key);
                string pageB = positions[i].Value ? "pb" : String.Empty;
                WriteLine(file,
                    String.Format(
                        "<style:style style:name=\"ro{0}{1}\" style:family=\"table-row\">",
                        rowPos,
                        pageB));
                string pageAuto = positions[i].Value ? "page" : "auto";
                WriteLine(file,
                    String.Format(
                        "<style:table-row-properties fo:break-before=\"{0}\" style:row-height=\"{1}cm\"/>",
                        pageAuto,
                        rowPos));
                WriteLine(file, "</style:style>");
            }

            WriteLine(file, "<style:style style:name=\"ta1\" style:family=\"table\" style:master-page-name=\"PageDef\">");
            WriteLine(file, "<style:table-properties table:display=\"true\" style:writing-mode=\"lr-tb\"/>");
            WriteLine(file, "</style:style>");
            WriteLine(file, "<style:style style:name=\"ceb\" style:family=\"table-cell\" />");
        }

        private void OdfStyles(Stream file)
        {
            for (int i = 0; i < matrix.StylesCount; i++)
            {
                ExportIEMStyle style = matrix.StyleById(i);
                WriteLine(file,
                    String.Format("<style:style style:name=\"ce{0}\" style:family=\"table-cell\" >",
                    i.ToString()));

                OdfTableCellStyles(file, style);
                if (exportType == OpenOfficeFormat.Spreadsheet)
                {
                    ParagraphStyle(file, style);
                    TextPropertiesStyle(file, style);
                }

                WriteLine(file, "</style:style>");
            }
        }

        private void OdfTableCellStyles(Stream file, ExportIEMStyle style)
        {
            Write(file,
                String.Format("<style:table-cell-properties fo:background-color=\"{0}\" style:repeat-content=\"false\" fo:wrap-option=\"wrap\" ",
                ExportUtils.HTMLColorCode(style.FillColor)));

            if (style.Angle > 0)
            {
                Write(file,
                    String.Format("style:rotation-angle=\"{0}\" style:rotation-align=\"none\" ",
                    (360 - style.Angle).ToString()));
            }

            if (style.VAlign == VertAlign.Center)
                Write(file, "style:vertical-align=\"middle\" ");
            if (style.VAlign == VertAlign.Top)
                Write(file, "style:vertical-align=\"top\" ");
            if (style.VAlign == VertAlign.Bottom)
                Write(file, "style:vertical-align=\"bottom\" ");

            if ((style.Border.Lines & BorderLines.Left) > 0)
                Write(file, GetBorderLineStyle(style.Border.LeftLine, "left"));
            if ((style.Border.Lines & BorderLines.Right) > 0)
                Write(file, GetBorderLineStyle(style.Border.RightLine, "right"));
            if ((style.Border.Lines & BorderLines.Top) > 0)
                Write(file, GetBorderLineStyle(style.Border.TopLine, "top"));
            if ((style.Border.Lines & BorderLines.Bottom) > 0)
                Write(file, GetBorderLineStyle(style.Border.BottomLine, "bottom"));

            Write(file, "/>");
        }

        private void ParagraphStyle(Stream file, ExportIEMStyle style)
        {
            Write(file, "<style:paragraph-properties ");
            HorizAlignStyle(file, style.HAlign);
            MarginStyle(file, style.Padding);
            WriteLine(file, "/>");
        }

        private void TableBegin(Stream file, int tableNumber)
        {
            // table
            WriteLine(file,
                String.Format("<table:table table:name=\"Table{0}\" table:style-name=\"ta1\" table:print=\"false\">", tableNumber));

            // columns
            OdfColumns(file);
        }

        private void TableEnd(Stream file)
        {
            WriteLine(file, "</table:table>");
        }

        private void TextPropertiesStyle(Stream file, ExportIEMStyle style)
        {
            Write(file,
                String.Format("<style:text-properties style:font-name=\"{0}\" fo:font-size=\"{1}pt\" ",
                style.Font.Name, ExportUtils.FloatToString(style.Font.Size)));

            if ((style.Font.Style & FontStyle.Underline) > 0)
                Write(file, " style:text-underline-style=\"solid\" " +
                    "style:text-underline-width=\"auto\" " +
                    "style:text-underline-color=\"font-color\" ");
            if ((style.Font.Style & FontStyle.Italic) > 0)
                Write(file, " style:font-style=\"italic\" ");
            if ((style.Font.Style & FontStyle.Bold) > 0)
                Write(file,
                    " fo:font-weight=\"bold\" style:font-weight-asian=\"bold\" style:font-weight-complex=\"bold\"");

            WriteLine(file, String.Format(" fo:color=\"{0}\"/>",
                ExportUtils.HTMLColorCode(style.TextColor)));
        }

        private void Write(Stream stream, string value)
        {
            byte[] buf = Encoding.UTF8.GetBytes(value);
            stream.Write(buf, 0, buf.Length);
        }

        private void WriteLine(Stream stream, string value)
        {
            byte[] buf = Encoding.UTF8.GetBytes(value);
            stream.Write(buf, 0, buf.Length);
            stream.WriteByte(13);
            stream.WriteByte(10);
        }
        private void WriterStyles(Stream file)
        {
            for (int i = 0; i < matrix.StylesCount; i++)
            {
                ExportIEMStyle style = matrix.StyleById(i);
                WriteLine(file,
                    String.Format("<style:style style:name=\"P{0}\" style:family=\"paragraph\" >",
                    i.ToString()));

                ParagraphStyle(file, style);
                TextPropertiesStyle(file, style);

                WriteLine(file, "</style:style>");
            }
        }
        #endregion Private Methods

        #region Protected Methods

        /// <inheritdoc/>
        protected override void ExportBand(Base band)
        {
            base.ExportBand(band);
            matrix.AddBand(band, this);
        }

        /// <inheritdoc/>
        protected override void ExportPageBegin(ReportPage page)
        {
            base.ExportPageBegin(page);
            matrix.AddPageBegin(page);
        }

        /// <inheritdoc/>
        protected override void ExportPageEnd(ReportPage page)
        {
            matrix.AddPageEnd(page);
            if (firstPage)
            {
                pageBottom = page.BottomMargin;
                pageLeft = page.LeftMargin;
                pageRight = page.RightMargin;
                pageTop = page.TopMargin;
                pageWidth = ExportUtils.GetPageWidth(page);
                pageHeight = ExportUtils.GetPageHeight(page);
                pageLandscape = page.Landscape;
                firstPage = false;
            }
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            matrix.Prepare();
            ExportODF(Stream);
        }

        /// <inheritdoc/>
        protected override void Start()
        {
            base.Start();
            matrix = new ExportMatrix();
            if (wysiwyg)
                matrix.Inaccuracy = 0.5f;
            else
                matrix.Inaccuracy = 10;
            matrix.RotatedAsImage = true;
            matrix.PlainRich = true;
            matrix.AreaFill = true;
            matrix.Report = Report;
            matrix.MaxCellHeight = 400;
            matrix.Images = true;
            matrix.ImageFormat = ImageFormat.Png;
            matrix.ShowProgress = ShowProgress;
            firstPage = true;
        }
        #endregion Protected Methods

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ODFExport"/> class.
        /// </summary>
        public ODFExport()
        {
            exportType = OpenOfficeFormat.Spreadsheet;
            pageBreaks = true;
            wysiwyg = true;
            creator = "FastReport .NET";
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            base.Serialize(writer);
            writer.WriteBool("Wysiwyg", Wysiwyg);
            writer.WriteBool("PageBreaks", PageBreaks);
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Open Document Spreadsheet export (Open Office Calc).
    /// </summary>
    public class ODSExport : ODFExport
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ODSExport"/> class.
        /// </summary>
        public ODSExport()
        {
            ExportType = OpenOfficeFormat.Spreadsheet;
        }

        #endregion Public Constructors

        #region Protected Methods

        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            if (IsXOTD)
                return new MyRes("FileFilters").Get("XodsFile");
            return new MyRes("FileFilters").Get("OdsFile");
        }

        #endregion Protected Methods
    }

    /// <summary>
    /// Open Document Text export (Open Office Writer).
    /// </summary>
    public class ODTExport : ODFExport
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ODTExport"/> class.
        /// </summary>
        public ODTExport()
        {
            ExportType = OpenOfficeFormat.Writer;
        }

        #endregion Public Constructors

        #region Protected Methods

        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            if (IsXOTD)
                return new MyRes("FileFilters").Get("XodtFile");
            return new MyRes("FileFilters").Get("OdtFile");
        }

        #endregion Protected Methods
    }
}