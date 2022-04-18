using FastReport.Format;
using FastReport.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace FastReport.Export.OoXML
{
    /// <summary>
    /// Drawing class
    /// </summary>
    internal class OoXMLDrawing : OoXMLBase
    {
        #region Private properties

        private int picCount;
        private StringBuilder drawing_string;
        private StringBuilder picture_rels;
        private Excel2007Export ooXML;

        #endregion Private properties

        public int PicCount { get { return picCount; } }

        #region Class overrides

        public override string RelationType
        {
            get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing"; }
        }

        public override string ContentType
        {
            get { return "application/vnd.openxmlformats-officedocument.drawing+xml"; }
        }

        public override string FileName
        {
            get { return "xl/drawings/drawing1.xml"; }
        }

        #endregion Class overrides

        private void ExportBorder(ExportIEMStyle style)
        {
            if (style.Border.Lines == BorderLines.All)
            {
                drawing_string.AppendFormat("<a:ln w=\"{0}\">", style.Border.LeftLine.Width);
                drawing_string.Append("<a:solidFill>");
                drawing_string.AppendFormat("<a:srgbClr val=\"{0:X2}{1:X2}{2:X2}\"/>",
                    style.Border.LeftLine.Color.R,
                    style.Border.LeftLine.Color.G,
                    style.Border.LeftLine.Color.B);
                drawing_string.Append("</a:solidFill>");

                drawing_string.Append(GetDashStyle(style.Border.LeftLine.DashStyle));

                drawing_string.Append("</a:ln>");
            }
        }

        public void Append(ExportIEMObject Obj, int x, int y, int dx, int dy, int rowNum, int matrixWidth)
        {
            picCount++;
            string rid = "\"rId" + picCount.ToString() + "\"";

            string picFileName = "xl/media/" + Obj.Hash + ".png";

            if (!ooXML.Zip.FileExists(picFileName))
            {
                Obj.PictureStream.Position = 0;
                ooXML.Zip.AddStream(picFileName, Obj.PictureStream);
            }

            picture_rels.AppendLine("<Relationship Id=" + rid +
                " Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/image\" " +
                "Target=\"/" + picFileName + "\" />");

            drawing_string.Append("<xdr:twoCellAnchor>"); // editAs=\"absolute\"

            drawing_string.Append("<xdr:from>");
            drawing_string.Append("<xdr:col>").Append(x.ToString()).Append("</xdr:col>");
            drawing_string.Append("<xdr:colOff>0</xdr:colOff>");
            if (!ooXML.Seamless)
            {
                drawing_string.Append("<xdr:row>").Append(y.ToString()).Append("</xdr:row>");
            }
            else
            {
                drawing_string.Append("<xdr:row>").Append((rowNum - 1).ToString()).Append("</xdr:row>");
            }
            drawing_string.Append("<xdr:rowOff>0</xdr:rowOff>");
            drawing_string.Append("</xdr:from>");

            drawing_string.Append("<xdr:to>");
            int colTo = x + dx;
            //if (x == 0 && colTo > matrixWidth - 1)
            //    colTo = matrixWidth - 1;
            drawing_string.Append("<xdr:col>").Append(colTo.ToString()).Append("</xdr:col>");
            drawing_string.Append("<xdr:colOff>0</xdr:colOff>");
            if (!ooXML.Seamless)
            {
                drawing_string.Append("<xdr:row>").Append((y + dy).ToString()).Append("</xdr:row>");
            }
            else
            {
                drawing_string.Append("<xdr:row>").Append((rowNum - 1 + dy).ToString()).Append("</xdr:row>");
            }
            drawing_string.Append("<xdr:rowOff>0</xdr:rowOff>");
            drawing_string.Append("</xdr:to>");
            drawing_string.Append("<xdr:pic>");
            drawing_string.Append("<xdr:nvPicPr>");
            drawing_string.Append("<xdr:cNvPr id=\"").
                Append(picCount.ToString()).Append("\" name=\"image").
                Append(picCount.ToString()).Append(".png\"> ");
            if (Obj.URL != null &&  Obj.URL.Length > 0)
            {
                ++picCount;
                string rIdLink = "\"rId" + picCount.ToString() + "\"";
                picture_rels.AppendLine("<Relationship Id=" + rIdLink +
                    " Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink\" " +
                    "Target=\"" + Utils.Converter.ToXml(Obj.URL) + "\" TargetMode=\"External\" />");
                drawing_string.Append("<a:hlinkClick xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" r:id=" +
                    rIdLink + "/>");
            }
            drawing_string.Append("</xdr:cNvPr>");
            drawing_string.Append("<xdr:cNvPicPr>");
            drawing_string.Append("<a:picLocks noChangeAspect=\"1\"/>");
            drawing_string.Append("</xdr:cNvPicPr>");
            drawing_string.Append("</xdr:nvPicPr>");

            drawing_string.Append("<xdr:blipFill>");
            drawing_string.Append(
                "<a:blip xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" r:embed=").
                Append(rid).Append("/>");
            //Append(" state=\"hqprint\"/>"); // fix for Excel 2007

            drawing_string.Append("<a:stretch><a:fillRect/></a:stretch>");
            drawing_string.Append("</xdr:blipFill>");

            drawing_string.Append("<xdr:spPr>");
            drawing_string.Append("<a:prstGeom prst=\"rect\"><a:avLst/></a:prstGeom>");
            ExportBorder(Obj.Style);
            drawing_string.Append("</xdr:spPr>");

            drawing_string.Append("</xdr:pic>");
            //drawing_string.Append("<xdr:clientData fLocksWithSheet=\"1\"/>"); fix for Excel 2007
            drawing_string.Append("<xdr:clientData />");
            drawing_string.AppendLine("</xdr:twoCellAnchor>");
        }

        public void Start()
        {
            //=== johnny appended 2012/6/17
            drawing_string = new StringBuilder(8192);
            picture_rels = new StringBuilder(256);

            drawing_string.Append(xml_header);
            drawing_string.Append(
                "<xdr:wsDr xmlns:xdr=\"http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing\" ").
                AppendLine("xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">");
            picture_rels.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\" ?>");
            picture_rels.Append("<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");
        }

        //public void Stop()
        //=== johnny appended 2012/6/17
        public void Stop(string sSheetID)
        {
            drawing_string.Append("</xdr:wsDr>");
            picture_rels.Append("</Relationships>");

            if (picCount != 0)
            {
                MemoryStream file = new MemoryStream();

                ExportUtils.WriteLn(file, picture_rels.ToString());

                file.Position = 0;
                ooXML.Zip.AddStream("xl/drawings/_rels/drawing" + sSheetID + ".xml.rels", file);

                file = new MemoryStream();
                ExportUtils.WriteLn(file, drawing_string.ToString());

                file.Position = 0;
                ooXML.Zip.AddStream("xl/drawings/drawing" + sSheetID + ".xml", file);
                picCount = 0;
            }
        }

        public OoXMLDrawing(Excel2007Export OoXML)
        {
            ooXML = OoXML;
            picCount = 0;
            drawing_string = new StringBuilder(8192);
            picture_rels = new StringBuilder(1024);
        }
    }

    /// <summary>
    /// Share all strings in document
    /// </summary>
    internal class OoXMLSharedStringTable : OoXMLBase
    {
        #region Private properties

        private List<string> sharedStringList;

        #endregion Private properties

        #region Class overrides

        public override string RelationType
        {
            get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings"; }
        }

        public override string ContentType
        {
            get { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml"; }
        }

        public override string FileName { get { return "sharedStrings.xml"; } }

        #endregion Class overrides

        private struct CurrentStyle
        {
            public int Size;
            public bool Bold;
            public bool Italic;
            public bool Underline;
            public bool Strike;
            public bool Sub;
            public bool Sup;
            public Color Colour;
            public string LastTag;
        }

        private string ConvertHtmlItem(CurrentStyle style)
        {
            StringBuilder str = new StringBuilder(256);
            str.Append("<rPr>");
            if (style.Bold) str.Append("<b />");
            if (style.Italic) str.Append("<i />");
            if (style.Underline) str.Append("<u />");

            str.Append(string.Format(CultureInfo.InvariantCulture, "<sz val=\"{0}\" />", style.Size));
            str.Append(string.Format(CultureInfo.InvariantCulture,
                "<color rgb=\"{0:X2}{1:X2}{2:X2}{3:X2}\" /> ",
                style.Colour.A, style.Colour.R, style.Colour.G, style.Colour.B));
            str.Append("<rFont val=\"Calibri\" /> ");

            str.Append("<family val=\"2\" /> ");
            str.Append("<charset val=\"204\" /> ");
            str.Append("<scheme val=\"minor\" /> ");
            str.Append("</rPr>");
            str.AppendLine();

            return str.ToString();
        }

        private void ParseFont(string s, CurrentStyle style, out CurrentStyle result_style)
        {
            result_style = style;

            string[] font_items = s.Split('=');
            string Tag = font_items[0].ToUpper();
            switch (Tag)
            {
                case "COLOR":
                    string[] val = font_items[1].Split('\"');
                    result_style.Colour = Color.FromName(val[1]);
                    break;
                    //default:
                    //throw new Exception("Unsupported font item: " + Tag);
            }
        }

        private string ParseHtmlTags(string s)
        {
            int Index = 0;
            int Begin = 0;
            int End = 0;
            string Tag;
            string Text;
            CurrentStyle current_style = new CurrentStyle();
            CurrentStyle previos_style;

            current_style.Size = 10;
            current_style.Bold = false;
            current_style.Italic = false;
            current_style.Underline = false;
            current_style.Colour = Color.FromName("Black");
            current_style.Strike = false;
            current_style.Sub = false;
            current_style.Sup = false;

            StringBuilder result = new StringBuilder(s.Length + 7);

            Stack<CurrentStyle> style_stack = new Stack<CurrentStyle>();

            Begin = s.IndexOfAny(new char[1] { '<' }, Index);

            while (Begin != -1)
            {
                if (Begin != 0 && Index == 0)
                {
                    result.Append("<r><t xml:space=\"preserve\">").
                        Append(ExportUtils.Excel2007String(s.Substring(Index, Begin), TextRenderType.Default)).
                        Append("</t></r>");
                }

                End = s.IndexOfAny(new char[1] { '>' }, Begin + 1);

                if (End == -1)
                    break;

                Tag = s.Substring(Begin + 1, End - Begin - 1);

                bool CloseTag = Tag.StartsWith("/");

                if (CloseTag)
                    Tag = Tag.Remove(0, 1);

                string[] items = Tag.Split(' ');

                Tag = items[0].ToUpper();

                if (!CloseTag)
                {
                    current_style.LastTag = Tag;
                    style_stack.Push(current_style);

                    switch (Tag)
                    {
                        case "B": current_style.Bold = true; break;
                        case "I": current_style.Italic = true; break;
                        case "U": current_style.Underline = true; break;
                        case "STRIKE": current_style.Strike = true; break;
                        case "SUB": current_style.Sub = true; break;
                        case "SUP": current_style.Sup = true; break;
                        case "FONT":
                            /*current_style.Font = items[1];*/
                            ParseFont(items[1], current_style, out current_style);
                            break;
                            //default:
                            //    throw new Exception("Unsupported HTML TAG");
                    }
                }
                else
                {
                    if (style_stack.Count > 0)
                    {
                        previos_style = style_stack.Pop();
                        if (previos_style.LastTag != Tag)
                        {
                            //throw new Exception("Unaligned HTML TAGS");
                        }
                        current_style = previos_style;
                    }
                }

                Index = End + 1;
                Begin = s.IndexOfAny(new char[1] { '<' }, Index);

                if (Begin == -1)
                {
                    Text = s.Substring(Index);
                }
                else
                {
                    Text = s.Substring(Index, Begin - Index);
                }
                if (Text.Length > 0)
                    result.Append("<r>").Append(ConvertHtmlItem(current_style)).
                        Append("<t xml:space=\"preserve\">").Append(ExportUtils.Excel2007String(Text, TextRenderType.Default)).
                        Append("</t></r>");
            }

            return result.ToString();
        }

        private bool HaveSeqSpc(string s)
        {
            int len = s.Length;
            for (int i = 0; i < len; i++)
            {
                if (s[i] == ' ' && (s.Length == 1 ||
                         (i < (len - 1) && s[i + 1] == ' ') ||
                         (i > 0 && s[i - 1] == ' ')
                         || i == len - 1))
                    return true;
            }
            return false;
        }

        public int Add(string s, TextRenderType textRenderType)
        {
            int idx;
            string parsed_tags = String.Empty;
            switch (textRenderType)
            {
                case TextRenderType.HtmlTags:
                    parsed_tags = ParseHtmlTags(s);
                    break;

                case TextRenderType.HtmlParagraph:
                    //TODO DETRAV EXCEL2007
                    break;
            }
            if (!String.IsNullOrEmpty(parsed_tags))
                s = parsed_tags;
            else if (HaveSeqSpc(s))
                s = "<t xml:space=\"preserve\">" + s + "</t>";
            else
                s = "<t>" + s + "</t>";
            StringBuilder str = new StringBuilder();
            foreach (char c in s)
            {
                if ((int)c < 32 || ((int)c >= 127 && (int)c < 160))
                {
                    str.Append("_x" + ((int)c).ToString("X4") + "_");
                }
                else
                {
                    str.Append(c);
                }
            }
            s = str.ToString();

            if (sharedStringList.Count > 100)
                idx = sharedStringList.IndexOf(s, sharedStringList.Count - 100);
            else
                idx = sharedStringList.IndexOf(s);

            if (idx == -1)
            {
                sharedStringList.Add(s);
                idx = sharedStringList.Count - 1;
            }

            return idx;
        }

        private void WriteSharedStringsBuffer(StringBuilder strings, Stream stream)
        {
            ExportUtils.WriteLn(stream, strings.ToString());
            strings.Length = 0;
        }

        public void Export(Excel2007Export OoXML)
        {
            MemoryStream file = new MemoryStream();

            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<sst xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" count=\"" +
            sharedStringList.Count + "\" uniqueCount=\"" + sharedStringList.Count + "\">");

            int i = 0;
            StringBuilder sharedStrings = new StringBuilder(8192);
            foreach (string item in sharedStringList)
            {
                // Next block is a fix of bug with not escaped ampersands.
                //=================================================================================
                int start = -1;
                List<int> indexes = new List<int>();
                start = item.IndexOf("&");
                
                // Find index of each ampersand in the string.
                while (start > -1)
                {
                    indexes.Add(start);
                    start = item.IndexOf("&", start + 1);
                }

                // Remove index of each correct ampersand.
                for (int j = indexes.Count - 1; j >= 0; j--)
                {
                    if (item.IndexOf("&amp;", indexes[j]) == indexes[j])
                        indexes.RemoveAt(j);
                    else if (item.IndexOf("&#10", indexes[j]) == indexes[j])
                        indexes.RemoveAt(j);
                    else if (item.IndexOf("&quot;", indexes[j]) == indexes[j])
                        indexes.RemoveAt(j);
                    else if (item.IndexOf("&apos;", indexes[j]) == indexes[j])
                        indexes.RemoveAt(j);
                    else if (item.IndexOf("&lt;", indexes[j]) == indexes[j])
                        indexes.RemoveAt(j);
                    else if (item.IndexOf("&gt;", indexes[j]) == indexes[j])
                        indexes.RemoveAt(j);
                    else if (item.IndexOf("&#", indexes[j]) == indexes[j])
                        indexes.RemoveAt(j);
                }

                // Escape each incorrect ampersand.
                string correctItem = item;
                for (int k = indexes.Count - 1; k >= 0; k--)
                {
                    correctItem = correctItem.Remove(indexes[k], 1);
                    correctItem = correctItem.Insert(indexes[k], "&amp;");
                }
                //=================================================================================

                sharedStrings.Append("<si>").Append(correctItem).Append("</si>");
                if (i++ >= 100)
                {
                    WriteSharedStringsBuffer(sharedStrings, file);
                    i = 0;
                }
            }
            WriteSharedStringsBuffer(sharedStrings, file);

            ExportUtils.WriteLn(file, "</sst>");

            file.Position = 0;
            OoXML.Zip.AddStream("xl/" + FileName, file);
        }

        public OoXMLSharedStringTable()
        {
            sharedStringList = new List<string>();
        }
    }

    /// <summary>
    /// Share all URL in document
    /// </summary>
    internal class OoXMLSharedURLTable : OoXMLBase
    {
#if READONLY_STRUCTS
        private readonly struct Item
#else
        private struct Item
#endif
        {
            internal readonly int id;
            internal readonly int x;
            internal readonly int y;
            internal readonly string url;

            internal Item(int id, int x, int y, string url)
            {
                this.id = id;
                this.x = x;
                this.y = y;
                this.url = url;
            }
        }

        #region Private properties

        private List<Item> uRL = new List<Item>();

        #endregion Private properties

        #region Class overrides

        /* "http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings"; */
        public override string RelationType { get { throw new Exception("Not decalred"); } }
        /* "application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml";  */
        public override string ContentType { get { throw new Exception("Not decalred"); } }
        /* return "sharedStrings.xml"; */
        public override string FileName { get { throw new Exception("Not decalred"); } }

        #endregion Class overrides

        internal void Add(int x, int y, string url)
        {
            int id = uRL.Count + 2;
            uRL.Add(new Item(id, x, y, url));
        }

        internal int Count { get { return uRL.Count; } }

        internal StringBuilder ExportBody(Excel2007Export OoXML)
        {
            StringBuilder builder = new StringBuilder(512);
            builder.Append("<hyperlinks>");
            foreach (Item i in uRL)
            {
                builder.AppendFormat("<hyperlink ref=\"{0}\" r:id=\"{1}{2}\"/>",
                    ExportUtils.GetCellReference(i.x, i.y + 1),   // Quoted("A2")
                    "rId", i.id.ToString()           // Quoted("rId1")
                    );
            }
            builder.AppendLine("</hyperlinks>");

            return builder;
        }

        internal StringBuilder ExportRelationship()
        {
            StringBuilder builder = new StringBuilder(256);
            foreach (Item i in uRL)
            {
                builder.AppendFormat(
                    "<Relationship TargetMode=\"External\" Target={0} " +
                    "Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink\" Id={1}/>",
                    Quoted(ExportUtils.Excel2007String(i.url, TextRenderType.Default)),
                    Quoted("rId" + i.id.ToString())
                );
            }
            return builder;
        }
    }

    /// <summary>
    /// Document styles
    /// </summary>
    internal class OoXMLDocumentStyles : OoXMLBase
    {
        #region Private properties

        private List<ExportIEMStyle> styles;
        private int formatIndex;

        #endregion Private properties

        #region Class overrides

        public override string RelationType
        {
            get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles"; }
        }

        public override string ContentType
        {
            get { return "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"; }
        }

        public override string FileName { get { return "styles.xml"; } }

        #endregion Class overrides

        #region Helpers

        private string GetRGBString(Color c)
        {
            return "\"" +
                ExportUtils.ByteToHex(c.A) +
                ExportUtils.ByteToHex(c.R) +
                ExportUtils.ByteToHex(c.G) +
                ExportUtils.ByteToHex(c.B) +
                "\"";
        }

        private string HAlign2String(HorzAlign ha)
        {
            switch (ha)
            {
                case HorzAlign.Center: return "center";
                case HorzAlign.Left: return "left";
                case HorzAlign.Right: return "right";
                case HorzAlign.Justify: return "justify";
            }
            return "";
        }

        private string VAlign2String(VertAlign va)
        {
            switch (va)
            {
                case VertAlign.Bottom: return "bottom";
                case VertAlign.Center: return "center";
                case VertAlign.Top: return "top";
            }
            return "";
        }

        private string Styles2String(LineStyle style, float Width)
        {
            if (Width < 2) switch (style)
                {
                    case LineStyle.Solid: return "\"thin\"";
                    case LineStyle.Double: return "\"double\"";
                    case LineStyle.Dot: return "\"dotted\"";
                    case LineStyle.DashDotDot: return "\"dashDotDot\"";
                    case LineStyle.DashDot: return "\"dashDot\"";
                    case LineStyle.Dash: return "\"dashed\"";
                }
            else if (Width < 3.5) switch (style)
                {
                    case LineStyle.Solid: return "\"medium\"";
                    case LineStyle.Double: return "\"double\"";
                    case LineStyle.Dot: return "\"mediumDashed\"";  // Due no "mediumDotted" do not exist in spec
                    case LineStyle.DashDotDot: return "\"mediumDashDotDot\"";
                    case LineStyle.DashDot: return "\"mediumDashDot\"";
                    case LineStyle.Dash: return "\"mediumDashed\"";
                }
            else switch (style)
                {
                    case LineStyle.Solid: return "\"thick\"";
                    case LineStyle.Double: return "\"double\"";
                    case LineStyle.Dot: return "\"mediumDashed\""; // Due no "mediumDotted" do not exist in spec
                    case LineStyle.DashDotDot: return "\"mediumDashDotDot\"";
                    case LineStyle.DashDot: return "\"slantDashDot\"";
                    case LineStyle.Dash: return "\"mediumDashed\"";
                }
            return "";
        }

        private string ConvertRotationAngle(int Angle)
        {
            if (Angle != 0 && Angle <= 90)
            {
                Angle = 90 + Angle;
            }
            else if (Angle >= 270)
            {
                Angle = 360 - Angle;
            }
            else
            {
                Angle = 0;
            }
            return Angle.ToString();
        }

        #endregion Helpers

        private void ExportFormats(Stream file)
        {
            StringBuilder result = new StringBuilder();
            int res_count = 0;

            for (int i = 0; i < styles.Count; i++)
            {
                ExportIEMStyle EStyle = styles[i];
                FormatBase format_base = EStyle.Format;
                if (format_base is CurrencyFormat || format_base is NumberFormat || format_base is DateFormat
                    || format_base is PercentFormat)
                {
                    if (GetDefaultNumberCode(format_base) == -1)
                    {
                        ++res_count;
                        result.AppendFormat(CultureInfo.InvariantCulture, "<numFmt numFmtId=\"{0}\" formatCode=\"{1}\" />",
                            formatIndex + res_count,
                            ExportUtils.GetExcelFormatSpecifier(format_base));
                    }
                }
            }
            if (res_count > 0)
            {
                ExportUtils.WriteLn(file, "<numFmts count=\"" + res_count + "\">");
                ExportUtils.WriteLn(file, result.ToString());
                ExportUtils.WriteLn(file, "</numFmts>");
            }
        }

        private int GetDefaultNumberCode(FormatBase format_base)
        {
            //https://github.com/closedxml/closedxml/wiki/NumberFormatId-Lookup-Table
            if (format_base is NumberFormat)
            {
                NumberFormat nbr = (format_base as NumberFormat);
                if (nbr.DecimalDigits == 0 &&
                   (/*nbr.GroupSeparator == " " ||*/ nbr.GroupSeparator == ""/* || // redmine 2525
                    nbr.GroupSeparator.ToCharArray()[0] == 160*/) &&
                    nbr.NegativePattern == 1) // -n
                    return 1;
                else if (nbr.DecimalDigits == 2 &&
                  nbr.GroupSeparator == "" && //without group separator
                  nbr.NegativePattern == 1) // -n
                    return 2;
                else if (nbr.DecimalDigits == 0 &&
                    nbr.GroupSeparator == "," &&
                    nbr.NegativePattern == 1) // -n
                    return 3;
                else if (nbr.DecimalDigits == 2 &&
                    nbr.GroupSeparator == "," &&
                    nbr.NegativePattern == 1) // -n
                    return 4;
                else if (nbr.DecimalDigits == 0 &&
                  nbr.GroupSeparator == "," &&
                  nbr.NegativePattern == 0) // (n)
                    return 37;
                else if (nbr.DecimalDigits == 2 &&
                    nbr.GroupSeparator == "," &&
                    nbr.NegativePattern == 0) // (n)
                    return 39;
                else
                    return -1;
            }
            else
                return -1;
        }

        private int GetFormatCode(ExportIEMStyle LookingForStyle)
        {
            int res_count = 0;
            for (int i = 0; i < styles.Count; i++)
            {
                ExportIEMStyle EStyle = styles[i];
                FormatBase format_base = EStyle.Format;

                if (format_base is CurrencyFormat || format_base is NumberFormat || format_base is DateFormat
                    || format_base is PercentFormat)
                {
                    int code = GetDefaultNumberCode(format_base);
                    bool defaultCode = code != -1;

                    if (!defaultCode)
                        ++res_count;
                    if (LookingForStyle == styles[i])
                    {
                        if (defaultCode)
                            return code;
                        else
                            return formatIndex + res_count;
                    }
                }
            }
            return 0;
        }

        private void ExportFonts(Stream file)
        {
            int i;
            ExportIEMStyle EStyle;
            StringBuilder fonts = new StringBuilder(4096);

            fonts.Append("<fonts count=\"").Append(styles.Count.ToString()).AppendLine("\">");
            for (i = 0; i < styles.Count; i++)
            {
                EStyle = styles[i];

                Font font = EStyle.Font;
                fonts.Append("<font>");
                if (font.Bold)
                    fonts.Append("<b/>");
                if (font.Italic)
                    fonts.Append("<i/>");
                if (font.Underline)
                    fonts.Append("<u/>");
                if (font.Strikeout)
                    fonts.Append("<strike val=\"1\"/>");

                fonts.Append("<sz val=\"").
                    Append(font.Size.ToString(CultureInfo.InvariantCulture.NumberFormat)).
                    AppendLine("\" />");
                Color c = EStyle.TextColor;
                if (c.A == 0 && c.R == 255 && c.G == 255 && c.B == 255)
                {
                    c = Color.Black;
                }
                fonts.Append("<color rgb=").Append(GetRGBString(c)).AppendLine(" />");
                fonts.Append("<name val=\"").Append(font.Name).AppendLine("\" />");
                fonts.AppendLine("</font>");
            }
            fonts.AppendLine("</fonts>");
            ExportUtils.Write(file, fonts.ToString());
        }

        private void ExportFills(Stream file)
        {
            int i;
            ExportIEMStyle EStyle;

            StringBuilder fills = new StringBuilder(4096);

            fills.Append("<fills count=\"").Append((styles.Count + 2).ToString()).AppendLine("\">");
            fills.AppendLine("<fill><patternFill patternType=\"none\"/></fill>");
            fills.AppendLine("<fill><patternFill patternType=\"gray125\"/></fill>");
            for (i = 0; i < styles.Count; i++)
            {
                EStyle = styles[i];
                fills.AppendLine("<fill>");
                if (EStyle.Fill is LinearGradientFill)
                {
                    LinearGradientFill linear = EStyle.Fill as LinearGradientFill;
                    fills.Append("<gradientFill degree=\"").Append(linear.Angle.ToString()).AppendLine("\">");
                    fills.AppendLine("<stop position=\"0\">");
                    fills.Append("<color rgb=").Append(GetRGBString(linear.StartColor)).AppendLine(" /> ");
                    fills.AppendLine("</stop>");
                    fills.AppendLine("<stop position=\"1\">");
                    fills.Append("<color rgb=").Append(GetRGBString(linear.EndColor)).AppendLine(" /> ");
                    fills.AppendLine("</stop>");
                    fills.AppendLine("</gradientFill>");
                }
                else if (EStyle.Fill is HatchFill)
                {
                    bool swap_color = false;
                    string PatternType = "none";
                    HatchFill hatch = EStyle.Fill as HatchFill;
                    switch (hatch.Style)
                    {
                        case System.Drawing.Drawing2D.HatchStyle.DarkUpwardDiagonal:
                            PatternType = "lightUp";
                            swap_color = true;
                            break;

                        case System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal:
                            PatternType = "lightUp";
                            break;

                        //  case System.Drawing.Drawing2D.HatchStyle.Cross:
                        //  case System.Drawing.Drawing2D.HatchStyle.Max:
                        case System.Drawing.Drawing2D.HatchStyle.LargeGrid:
                        case System.Drawing.Drawing2D.HatchStyle.SmallGrid:
                            PatternType = "lightGrid";
                            break;

                        case System.Drawing.Drawing2D.HatchStyle.ForwardDiagonal:
                            swap_color = true;
                            PatternType = "darkDown";
                            break;

                        case System.Drawing.Drawing2D.HatchStyle.DarkDownwardDiagonal:
                            PatternType = "darkDown";
                            break;

                        case System.Drawing.Drawing2D.HatchStyle.DarkHorizontal:
                            PatternType = "darkHorizontal";
                            break;

                        case System.Drawing.Drawing2D.HatchStyle.LightHorizontal:
                            PatternType = "lightHorizontal";
                            break;

                        case System.Drawing.Drawing2D.HatchStyle.LightVertical:
                            PatternType = "lightVertical";
                            break;

                        case System.Drawing.Drawing2D.HatchStyle.DarkVertical:
                            PatternType = "darkVertical";
                            break;

                        case System.Drawing.Drawing2D.HatchStyle.Trellis:
                            PatternType = "darkTrellis";
                            break;

                        case System.Drawing.Drawing2D.HatchStyle.Percent05:
                            PatternType = "gray0625";
                            break;

                        //case System.Drawing.Drawing2D.HatchStyle.Min:
                        case System.Drawing.Drawing2D.HatchStyle.Horizontal:
                        case System.Drawing.Drawing2D.HatchStyle.Vertical:
                        case System.Drawing.Drawing2D.HatchStyle.DiagonalCross:
                        case System.Drawing.Drawing2D.HatchStyle.Percent10:
                        case System.Drawing.Drawing2D.HatchStyle.Percent20:
                        case System.Drawing.Drawing2D.HatchStyle.Percent25:
                        case System.Drawing.Drawing2D.HatchStyle.Percent30:
                        case System.Drawing.Drawing2D.HatchStyle.Percent40:
                        case System.Drawing.Drawing2D.HatchStyle.Percent50:
                        case System.Drawing.Drawing2D.HatchStyle.Percent60:
                        case System.Drawing.Drawing2D.HatchStyle.Percent70:
                        case System.Drawing.Drawing2D.HatchStyle.Percent75:
                        case System.Drawing.Drawing2D.HatchStyle.Percent80:
                        case System.Drawing.Drawing2D.HatchStyle.Percent90:
                        case System.Drawing.Drawing2D.HatchStyle.LightDownwardDiagonal:
                        case System.Drawing.Drawing2D.HatchStyle.LightUpwardDiagonal:
                        case System.Drawing.Drawing2D.HatchStyle.WideDownwardDiagonal:
                        case System.Drawing.Drawing2D.HatchStyle.WideUpwardDiagonal:
                        case System.Drawing.Drawing2D.HatchStyle.NarrowVertical:
                        case System.Drawing.Drawing2D.HatchStyle.NarrowHorizontal:
                        case System.Drawing.Drawing2D.HatchStyle.DashedDownwardDiagonal:
                        case System.Drawing.Drawing2D.HatchStyle.DashedUpwardDiagonal:
                        case System.Drawing.Drawing2D.HatchStyle.DashedHorizontal:
                        case System.Drawing.Drawing2D.HatchStyle.DashedVertical:
                        case System.Drawing.Drawing2D.HatchStyle.SmallConfetti:
                        case System.Drawing.Drawing2D.HatchStyle.LargeConfetti:
                        case System.Drawing.Drawing2D.HatchStyle.ZigZag:
                        case System.Drawing.Drawing2D.HatchStyle.Wave:
                        case System.Drawing.Drawing2D.HatchStyle.DiagonalBrick:
                        case System.Drawing.Drawing2D.HatchStyle.HorizontalBrick:
                        case System.Drawing.Drawing2D.HatchStyle.Weave:
                        case System.Drawing.Drawing2D.HatchStyle.Plaid:
                        case System.Drawing.Drawing2D.HatchStyle.Divot:
                        case System.Drawing.Drawing2D.HatchStyle.DottedGrid:
                        case System.Drawing.Drawing2D.HatchStyle.DottedDiamond:
                        case System.Drawing.Drawing2D.HatchStyle.Shingle:
                        case System.Drawing.Drawing2D.HatchStyle.Sphere:
                        case System.Drawing.Drawing2D.HatchStyle.SmallCheckerBoard:
                        case System.Drawing.Drawing2D.HatchStyle.LargeCheckerBoard:
                        case System.Drawing.Drawing2D.HatchStyle.OutlinedDiamond:
                        case System.Drawing.Drawing2D.HatchStyle.SolidDiamond:
                            break;
                    }
                    fills.Append("<patternFill patternType=").Append(Quoted(PatternType)).AppendLine(" >");
                    if (!swap_color)
                    {
                        fills.Append("<fgColor rgb=").Append(GetRGBString(hatch.ForeColor)).AppendLine("/>");
                        fills.Append("<bgColor rgb=").Append(GetRGBString(hatch.BackColor)).Append("/>");
                    }
                    else
                    {
                        fills.Append("<fgColor rgb=").Append(GetRGBString(hatch.BackColor)).AppendLine("/>");
                        fills.Append("<bgColor rgb=").Append(GetRGBString(hatch.ForeColor)).AppendLine("/>");
                    }
                    fills.AppendLine("</patternFill>");
                }
                else
                {
                    if (EStyle.FillColor == Color.Transparent)
                        fills.AppendLine("<patternFill patternType=\"none\"/>");
                    else
                    {
                        fills.AppendLine("<patternFill patternType=\"solid\">");
                        fills.Append("<fgColor rgb=").Append(GetRGBString(EStyle.FillColor)).AppendLine("/>");
                        fills.AppendLine("</patternFill>");
                    }
                }
                fills.AppendLine("</fill>");
            }
            fills.AppendLine("</fills>");
            ExportUtils.WriteLn(file, fills.ToString());
        }

        private void ExportBorders(Stream file)
        {
            StringBuilder borderResult = new StringBuilder(256);
            borderResult.Append("<borders count=\"").Append(styles.Count.ToString()).AppendLine("\">");
            for (int i = 0; i < styles.Count; i++)
            {
                Border border = styles[i].Border;

                borderResult.AppendLine("<border>");

                if ((border.Lines & BorderLines.Left) != 0)
                    borderResult.AppendLine(
                        "<left style=" +
                        Styles2String(border.LeftLine.Style, border.LeftLine.Width) +
                        "><color rgb=" +
                        GetRGBString(border.LeftLine.Color) +
                        " /></left>");
                else
                    borderResult.AppendLine("<left />");

                if ((border.Lines & BorderLines.Right) != 0)
                    borderResult.AppendLine(
                        "<right style=" +
                        Styles2String(border.RightLine.Style, border.RightLine.Width) +
                        "><color rgb=" +
                        GetRGBString(border.RightLine.Color) +
                        " /></right>");
                else
                    borderResult.AppendLine("<right />");

                if ((border.Lines & BorderLines.Top) != 0)
                    borderResult.AppendLine(
                        "<top style=" +
                        Styles2String(border.TopLine.Style, border.TopLine.Width) +
                        "><color rgb=" +
                        GetRGBString(border.TopLine.Color) +
                        " /></top>");
                else
                    borderResult.AppendLine("<top />");

                if ((border.Lines & BorderLines.Bottom) != 0)
                    borderResult.AppendLine(
                        "<bottom style=" +
                        Styles2String(border.BottomLine.Style, border.BottomLine.Width) +
                        "><color rgb=" +
                        GetRGBString(border.BottomLine.Color) +
                        " /></bottom>");
                else
                    borderResult.AppendLine("<bottom />");

                borderResult.AppendLine("<diagonal />");
                borderResult.AppendLine("</border>");
            }
            borderResult.AppendLine("</borders>");
            ExportUtils.WriteLn(file, borderResult.ToString());
        }

        private VertAlign TranslateVAligment270(HorzAlign ha)
        {
            switch (ha)
            {
                case HorzAlign.Left:
                    return VertAlign.Bottom;

                case HorzAlign.Right:
                    return VertAlign.Top;
            }
            return VertAlign.Center;
        }

        private HorzAlign TranslateHAligment270(VertAlign va)
        {
            switch (va)
            {
                case VertAlign.Top: return HorzAlign.Left;
                case VertAlign.Bottom: return HorzAlign.Right;
            }
            return HorzAlign.Center;
        }

        private VertAlign TranslateVAligment90(HorzAlign ha)
        {
            switch (ha)
            {
                case HorzAlign.Left:
                    return VertAlign.Top;

                case HorzAlign.Right:
                    return VertAlign.Bottom;
            }
            return VertAlign.Center;
        }

        private HorzAlign TranslateHAligment90(VertAlign va)
        {
            switch (va)
            {
                case VertAlign.Top: return HorzAlign.Right;
                case VertAlign.Bottom: return HorzAlign.Left;
            }
            return HorzAlign.Center;
        }

        private VertAlign TranslateVAligment180(VertAlign va)
        {
            switch (va)
            {
                case VertAlign.Top: return VertAlign.Bottom;
                case VertAlign.Bottom: return VertAlign.Top;
            }
            return VertAlign.Center;
        }

        private HorzAlign TranslateHAligment180(HorzAlign ha)
        {
            switch (ha)
            {
                case HorzAlign.Left: return HorzAlign.Right;
                case HorzAlign.Right: return HorzAlign.Left;
            }
            return HorzAlign.Center;
        }

        private void ExportAlgnment(StringBuilder file, ExportIEMStyle EStyle)
        {
            HorzAlign halign = EStyle.HAlign;
            VertAlign valign = EStyle.VAlign;

            if (EStyle.Angle != 0)
            {
                if (EStyle.Angle == 90)
                {
                    valign = TranslateVAligment90(EStyle.HAlign);
                    halign = TranslateHAligment90(EStyle.VAlign);
                }
                else if (EStyle.Angle == 180)
                {
                    valign = TranslateVAligment180(EStyle.VAlign);
                    halign = TranslateHAligment180(EStyle.HAlign);
                }
                else if (EStyle.Angle == 270)
                {
                    valign = TranslateVAligment270(EStyle.HAlign); // VertAlign.Bottom;
                    halign = TranslateHAligment270(EStyle.VAlign); // HorzAlign.Right;
                }
                else if (EStyle.Angle < 90)
                {
                    halign = HorzAlign.Center;
                    valign = VertAlign.Center;
                }
                else if (EStyle.Angle > 270)
                {
                    halign = HorzAlign.Center;
                    valign = VertAlign.Center;
                }
            }

            const double paddingScale = 10.0;
            int padding = 0;

            if (halign == HorzAlign.Left)
                padding = EStyle.Padding.Left;
            else if (halign == HorzAlign.Right)
                padding = EStyle.Padding.Right;

            file.Append("<alignment horizontal=\"").Append(HAlign2String(halign)).
                Append("\" vertical=\"").Append(VAlign2String(valign)).
                Append("\" textRotation=\"").Append(ConvertRotationAngle(EStyle.Angle)).
                Append("\" wrapText=\"1").Append("\" indent=\"").
                Append((int)Math.Round(padding / paddingScale)).AppendLine("\" />");
        }

        private void ExportCellStyles(Stream file)
        {
            StringBuilder styles = new StringBuilder(4096);

            styles.Append("<cellXfs count=\"").Append(this.styles.Count.ToString()).AppendLine("\">");
            for (int i = 0; i < this.styles.Count; i++)
            {
                ExportIEMStyle EStyle = this.styles[i];
                styles.Append("<xf numFmtId=\"").Append(GetFormatCode(EStyle)).AppendLine("\"");
                styles.Append("fontId=\"").Append(i.ToString()).AppendLine("\" ");
                styles.Append("fillId=\"").Append((i + 2).ToString()).Append("\" ");
                styles.Append("borderId=\"").Append(i.ToString()).AppendLine("\" ");
                styles.AppendLine("xfId=\"0\" ");
                styles.AppendLine("applyAlignment=\"1\">");
                ExportAlgnment(styles, EStyle);
                styles.AppendLine("</xf>");
            }
            styles.AppendLine("</cellXfs>");

            ExportUtils.Write(file, styles.ToString());
        }

        public void Export(Excel2007Export OoXML)
        {
            MemoryStream file = new MemoryStream();

            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<styleSheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">");

            ExportFormats(file);
            ExportFonts(file);
            ExportFills(file);
            ExportBorders(file);
            // Cell styles
            ExportCellStyles(file);
            ExportUtils.WriteLn(file, "</styleSheet>");

            file.Position = 0;
            OoXML.Zip.AddStream("xl/" + FileName, file);
        }

        public OoXMLDocumentStyles()
        {
            styles = new List<ExportIEMStyle>();
            formatIndex = 164;
        }

        internal int Add(OoXMLSheet sh)
        {
            ExportIEMStyle style = new ExportIEMStyle();
            style.Border.Color = Color.Red;
            style.Border.Lines = BorderLines.All;
            //style.Font. = Color.Blue;
            styles.Add(style);
            styles.AddRange(sh.Matrix.Styles);
            return styles.Count;
        }
    }

    /// <summary>
    /// Workbook
    /// </summary>
    internal class OoXMLWorkbook : OoXMLBase
    {
        #region Class overrides

        public override string RelationType
        {
            get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"; }
        }

        public override string ContentType
        {
            get { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"; }
        }

        public override string FileName
        {
            get { return "xl/workbook.xml"; }
        }

        #endregion Class overrides

        #region Private properties

        private List<OoXMLSheet> sheetList = new List<OoXMLSheet>();

        #endregion Private properties

        #region Internal properties

        internal OoXMLSheet[] SheetList { get { return sheetList.ToArray(); } }

        #endregion Internal properties

        internal void Export(Excel2007Export OoXML/*, ExportMatrix FMatrix*/)
        {
            MemoryStream file = new MemoryStream();
            StringBuilder sheets = new StringBuilder(4096);
            sheets.AppendLine(xml_header);
            sheets.AppendLine(
                "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" " +
                "xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">");
            sheets.AppendLine("<fileVersion appName=\"xl\" lastEdited=\"4\" lowestEdited=\"4\" rupBuild=\"4505\" />");
            sheets.AppendLine("<workbookPr defaultThemeVersion=\"124226\" />");
            sheets.AppendLine("<bookViews>");
            sheets.AppendLine("<workbookView xWindow=\"0\" yWindow=\"0\" windowWidth=\"8610\" windowHeight=\"6225\" />");
            sheets.AppendLine("</bookViews>");
            sheets.AppendLine("<sheets>");
            int page_name_idx = 0;

            foreach (OoXMLSheet sh in this.sheetList)
            {
                StringBuilder page_name_sb = new StringBuilder(sh.Name);

                if (page_name_sb.Length == 0)
                {
                    page_name_idx++;
                    page_name_sb.Append("Page_" + page_name_idx.ToString());
                }
                else
                {
                    page_name_sb.Replace("&", "&amp;");
                    page_name_sb.Replace("<", "&lt;");
                    page_name_sb.Replace(">", "&gt;");

                    //forbidden character in name page title Excel2007 

                    page_name_sb.Replace("?", string.Empty);
                }
                sheets.Append("<sheet name=").Append(Quoted(page_name_sb.ToString())).Append(" sheetId=").Append(Quoted(sh.SheetID)).
                    Append(" r:id=").Append(Quoted(sh.rId)).AppendLine(" />");
            }
            sheets.AppendLine("</sheets>");
            sheets.AppendLine("<calcPr calcId=\"124519\" />");
            sheets.AppendLine("</workbook>");
            ExportUtils.Write(file, sheets.ToString());
            file.Position = 0;
            OoXML.Zip.AddStream(FileName, file);
        }

        internal void AddSheet(OoXMLSheet sh)
        {
            sheetList.Add(sh);
            this.AddRelation(sheetList.Count + 20, sh);
        }
    }

    /// <summary>
    /// OoXMLSheet class
    /// </summary>
    internal class OoXMLSheet : OoXMLBase
    {
        #region Private properties

        private const float MARG_DIV = 25.4F;

        private NumberFormatInfo formatInfo;
        private ExportMatrix matrix;
        private string name;
        private int internal_index;

        #endregion Private properties

        private string FloatToString(double value)
        {
            return Convert.ToString(Math.Round(value, 2), formatInfo);
        }

        #region Internal properties

        internal ExportMatrix Matrix { get { return matrix; } }
        internal string Name { get { return name; } }
        internal string SheetID { get { return internal_index.ToString(); } }
        internal int ID { set { internal_index = value; } }

        #endregion Internal properties

        #region Class overrides

        public override string RelationType
        {
            get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"; }
        }

        public override string ContentType
        {
            get { return "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"; }
        }

        public override string FileName
        {
            get { return "worksheets/sheet" + internal_index.ToString() + ".xml"; }
        }

        #endregion Class overrides

        private string ExportCell(Excel2007Export OoXML, ExportIEMObject Obj, int x, int y, int rowNum, int FirstStyleIndex)
        {
            StringBuilder cell = new StringBuilder(Obj.Text.Length + 30);
            if (Obj.IsNumeric || Obj.IsDateTime || Obj.IsPercent)
            {
                cell.Append("<c r=\"").Append(ExportUtils.GetCellReference(x, rowNum)).
                    Append("\" s=\"").Append((Obj.StyleIndex + FirstStyleIndex + 1).ToString()).Append("\"><v>");
                if (Obj.IsDateTime)
                    cell.Append(Convert.ToString(Convert.ToDateTime(Obj.Value).ToOADate(),
                        CultureInfo.InvariantCulture.NumberFormat)).Append("</v></c>");
                else if (Obj.IsPercent)
                    cell.Append(Convert.ToString(Obj.Value, CultureInfo.InvariantCulture.NumberFormat)).Append("</v></c>");
                else
                    cell.Append(Convert.ToString(Obj.Value, CultureInfo.InvariantCulture.NumberFormat)).Append("</v></c>");
            }
            else
            {
                String s = ExportUtils.Excel2007String(Obj.Text, Obj.TextRenderType);
                if (!String.IsNullOrEmpty(Obj.URL))
                    OoXML.URLTable.Add(x, y, Obj.URL);

                cell.Append("<c r=\"").Append(ExportUtils.GetCellReference(x, rowNum)).Append("\" s=\"").
                        Append((Obj.StyleIndex + FirstStyleIndex + 1).ToString());
                if (s!= string.Empty)
                {
                    int idx = OoXML.StringTable.Add(s, Obj.TextRenderType);
                    cell.Append("\" t=\"s\"><v>").Append(idx.ToString()).Append("</v></c>");
                }
                else
                    cell.Append("\" t=\"s\"></c>");
            }
            //cell.AppendLine();
            return cell.ToString();
        }

        internal string GetRangeReference(int i)
        {
            int fx, fy, dx, dy;
            matrix.ObjectPos(i, out fx, out fy, out dx, out dy);
            return "\"" +
                ExportUtils.GetCellReference(fx, fy + 1) + ":" +
                ExportUtils.GetCellReference(fx + dx - 1, fy + dy) +
                "\"";
        }

        internal string GetRangeReference(int i, int rowNum)
        {
            int fx, fy, dx, dy;
            matrix.ObjectPos(i, out fx, out fy, out dx, out dy);
            return "\"" +
                ExportUtils.GetCellReference(fx, rowNum - dy + 1) + ":" +
                ExportUtils.GetCellReference(fx + dx - 1, rowNum) +
                "\"";
        }

        internal void Export(Excel2007Export OoXML, int FirstStyleIndex, bool PageBreaks)
        {
            int merged_cells_count = 0;

            MemoryStream file = new MemoryStream();

            StringBuilder builder = new StringBuilder(8192);
            StringBuilder merged = new StringBuilder(256);

            builder.Append(xml_header);
            builder.AppendLine("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" " +
                "xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">");

            builder.AppendLine(OoXML.GetPageSetUpPr());

            builder.AppendLine(OoXML.GetMatrxDimension(matrix));

            builder.Append("<sheetViews>");
            builder.Append("<sheetView tabSelected=\"1\" workbookViewId=\"0\" />");
            builder.AppendLine("</sheetViews>");
            builder.AppendLine("<sheetFormatPr defaultRowHeight=\"15\" />");

            // sheet columns
            if (matrix.ObjectsCount > 0)
            {
                builder.Append("<cols>");
                for (int x = 1; x < matrix.Width; x++)
                {
                    builder.Append("<col min=\"").Append(x.ToString()).Append("\" max=\"").Append(x).Append("\" width=\"").
                        Append(ExportUtils.FloatToString((matrix.XPosById(x) - matrix.XPosById(x - 1)) / OoXML.XDivider)).
                        Append("\" customWidth=\"1\" />");
                }
                builder.AppendLine("</cols>");
            }

            //if empty sheet
            if (matrix.Height == 0 && matrix.Width == 0)
            {
                builder.AppendLine("<sheetData/>");
            }
            else
            {
                builder.AppendLine("<sheetData>");

                ExportUtils.Write(file, builder.ToString());
                builder.Length = 0;

                for (int y = 0, rowNum = 1; y < matrix.Height - 1; y++)
                {
                    StringBuilder line = new StringBuilder(128);

                    float ht = 0;
                    if (!matrix.IsYPosByIdNull(y) && !matrix.IsYPosByIdNull(y + 1))
                    {
                        ht = (matrix.YPosById(y + 1) - matrix.YPosById(y)) / OoXML.YDivider;
                    }
                    else
                    {
                        ht = 14.5f; // Set default row height in Excel with font size equal 11.
                    }
                    line.Append("<row r=\"").Append((rowNum).ToString()).Append("\" ht=\"").Append(ExportUtils.FloatToString(ht)).
                         Append("\" customHeight=\"1\">");

                    int nonEmptyCells = 0;

                    for (int x = 0; x < matrix.Width; x++)
                    {
                        int i = matrix.Cell(x, y);
                        if (i != -1)
                        {
                            int fx, fy, dx, dy;
                            matrix.ObjectPos(i, out fx, out fy, out dx, out dy);
                            ExportIEMObject Obj = matrix.ObjectById(i);

                            if (Obj.Counter < dy)
                            {
                                if ((dy == 1) || Obj.Counter == 0)
                                {
                                    line.Append(ExportCell(OoXML, Obj, x, y, rowNum, FirstStyleIndex));
                                }
                                else
                                {
                                    line.Append("<c r=\"").Append(ExportUtils.GetCellReference(x, rowNum)).Append("\" s=").
                                        Append(Quoted(Obj.StyleIndex + FirstStyleIndex + 1)).Append(" />");
                                }

                                for (int j = 1; j < dx; j++)
                                {
                                    x++;
                                    line.Append("<c r=\"").Append(ExportUtils.GetCellReference(x, rowNum)).Append("\" s=").
                                        Append(Quoted(Obj.StyleIndex + FirstStyleIndex + 1)).Append(" />");
                                }

                                if (!String.IsNullOrEmpty(Obj.Text) ||
                                        (Obj.Style != null &&
                                            (Obj.Style.Border.Lines != BorderLines.None ||
                                            Obj.Style.FillColor != Color.White ||
                                            Obj.Style.FillColor.A != 0)
                                        )
                                   )
                                    nonEmptyCells++;

                                if (!Obj.IsText)
                                {
                                    if (Obj.Width > 0 && Obj.Counter == 0)
                                    {
                                        OoXML.Drawing.Append(Obj, fx, fy, dx, dy, rowNum, matrix.Width);
                                        nonEmptyCells++;
                                    }
                                }

                                Obj.Counter++;
                                if ((Obj.Counter == dy) && (dx > 1 || dy > 1))
                                {
                                    merged_cells_count++;
                                    if (!OoXML.Seamless)
                                    {
                                        merged.Append("<mergeCell ref=").Append(GetRangeReference(i)).Append("/>");
                                    }
                                    else
                                    {
                                        merged.Append("<mergeCell ref=").Append(GetRangeReference(i, rowNum)).Append("/>");
                                    }
                                }
                            }
                        }
                    }
                    line.AppendLine("</row>");

                    if (!(OoXML.Seamless && nonEmptyCells == 0))
                    {
                        ExportUtils.Write(file, line.ToString());
                        rowNum++;
                    }

                    if (y % 1000 == 0)
                        Config.DoEvent();
                }

                builder.AppendLine("</sheetData>");
            }

            // merge cells
            if (merged_cells_count != 0)
            {
                builder.Append("<mergeCells count=\"").Append(merged_cells_count).Append("\">");
                builder.Append(merged);
                builder.Append("</mergeCells>");
            }

            // url list
            if (OoXML.URLTable.Count != 0)
            {
                builder.Append(OoXML.URLTable.ExportBody(OoXML));
            }

            builder.Append("<pageMargins left=\"").Append(FloatToString(matrix.PageLMargin(0) / MARG_DIV)).
                Append("\" right=\"").Append(FloatToString(matrix.PageRMargin(0) / MARG_DIV)).
                Append("\" top=\"").Append(FloatToString(matrix.PageTMargin(0) / MARG_DIV)).
                Append("\" bottom=\"").Append(FloatToString(matrix.PageBMargin(0) / MARG_DIV)).
                AppendLine("\" header=\"0\" footer=\"0\" />");

            /* page setup */
            builder.Append("<pageSetup paperSize=").
                Append(Quoted(matrix.RawPaperSize(0))).
                Append(" orientation=").Append(Quoted(matrix.Landscape(0) ? "landscape" : "portrait")).
                Append(GetPrintFitProp(OoXML)).
                Append(" horizontalDpi=").Append(Quoted(300)).
                Append(" verticalDpi=").Append(Quoted(300)).AppendLine(" />");

            /* row breaks */
            if (PageBreaks)
            {
                int pages = 0;
                List<string> pageBreaks = new List<string>();

                for (int i = 0; i <= matrix.Height - 1 && pages <= 1024; i++)
                {
                    if (!matrix.IsYPosByIdNull(i) && (matrix.YPosById(i) >= matrix.PageBreak(pages)))
                    {
                        pageBreaks.Add(string.Format("<brk id=\"{0}\" max=\"16383\" man=\"1\"/>", i));
                        pages++;
                    }
                }

                if (pages > 0)
                {
                    builder.AppendFormat("<rowBreaks count=\"{0}\" manualBreakCount=\"{1}\">", pages - 1, pages - 1);
                    builder.AppendLine();
                    for (int i = 0; i < pageBreaks.Count; i++)
                    {
                        builder.AppendLine(pageBreaks[i]);
                    }
                    builder.AppendLine("</rowBreaks>");
                }
            }

            /* drawing */
            if (OoXML.Drawing.PicCount != 0)
            {
                builder.Append("<drawing r:id=\"rId1\" />");
            }

            /* Relations present in documents */
            if (OoXML.URLTable.Count != 0 || OoXML.Drawing.PicCount != 0)
            {
                MemoryStream rel_file = new MemoryStream();
                StreamWriter rel_out = new StreamWriter(rel_file);
                rel_out.WriteLine(xml_header);
                rel_out.WriteLine("<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");
                if (OoXML.Drawing.PicCount != 0)
                {
                    rel_out.WriteLine(
                        "<Relationship Id=\"rId1\" " +
                        "Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing\" " +
                        "Target=\"../drawings/drawing" + SheetID + ".xml\"/>");
                    OoXML.SheetIDsWithPics.Add(SheetID);
                }
                if (OoXML.URLTable.Count != 0)
                {
                    rel_out.WriteLine(OoXML.URLTable.ExportRelationship());
                }
                rel_out.WriteLine("</Relationships>");
                rel_out.Flush();
                rel_file.Position = 0;
                OoXML.Zip.AddStream("xl/worksheets/_rels/sheet" + SheetID + ".xml.rels", rel_file);
            }

            builder.Append("</worksheet>");
            ExportUtils.Write(file, builder.ToString());
            file.Position = 0;
            OoXML.Zip.AddStream("xl/worksheets/sheet" + SheetID + ".xml", file);
        }

        private string GetPrintFitProp(Excel2007Export OoXML)
        {
            switch (OoXML.PrintFit)
            {
                case Excel2007Export.PrintFitMode.FitAllColumsOnOnePage:
                    return " fitToHeight=\"0\"";

                case Excel2007Export.PrintFitMode.FitAllRowsOnOnePage:
                    return " fitToWidth=\"0\"";

                default:
                    return String.Empty;
            }
        }

        //
        public OoXMLSheet(ExportMatrix matrix, string Name, int Index)
        {
            formatInfo = new NumberFormatInfo();
            formatInfo.NumberGroupSeparator = String.Empty;
            formatInfo.NumberDecimalSeparator = ".";
            this.matrix = matrix;
            name = Name;
            internal_index = Index;
        }
    }

    /// <summary>
    /// Excel 2007 export class
    /// </summary>
    public partial class Excel2007Export : OOExportBase
    {
        #region Enums

        /// <summary>
        /// Defines Print Scaling Mode.
        /// </summary>
        public enum PrintFitMode
        {
            /// <summary>
            /// Print sheets at thier actual size.
            /// </summary>
            NoScaling,

            /// <summary>
            /// Shrink the printout so that it fits on one page.
            /// </summary>
            FitSheetOnOnePage,

            /// <summary>
            /// Shrink the printout so that it is one page wide.
            /// </summary>
            FitAllColumsOnOnePage,

            /// <summary>
            /// Shrink the printout so that it is one page high.
            /// </summary>
            FitAllRowsOnOnePage
        }

        #endregion Enums

        #region Constants

        private const float oxmlXDivider = 6f;
#if MONO
        private const float oxmlYDivider = 1.378f;
#else
        private const float oxmlYDivider = 1.333f;
#endif		

        private float fontScale = 1f;

        #endregion Constants

        #region Private fields

        private MyRes res;
        private bool wysiwyg;
        private bool pageBreaks;
        private bool dataOnly;
        private bool seamless;
        private PrintFitMode printFitMode;

        private OoXMLSharedStringTable stringTable;
        private OoXMLSharedURLTable uRLTable;
        internal OoXMLDrawing drawing;
        private OoXMLCoreDocumentProperties coreDocProp;
        private OoXMLApplicationProperties appProp;
        private OoXMLDocumentStyles docStyles;
        private OoXMLWorkbook workBook;
        private Hashtable sheets;
        private ExportMatrix matrix;
        private OoXMLSheet sh;
        private bool printOptimized = false;
        private bool splitPages;

        private List<string> sheetIDsWithPics;

        #endregion Private fields

        #region Properties

        internal OoXMLDrawing Drawing { get { return drawing; } }
        internal OoXMLSharedStringTable StringTable { get { return stringTable; } }
        internal OoXMLSharedURLTable URLTable { get { return uRLTable; } }
        internal float YDivider { get { return oxmlYDivider; } }
        internal float XDivider { get { return oxmlXDivider; } }
        
        internal List<string> SheetIDsWithPics
        {
            get { return sheetIDsWithPics; }
            set { sheetIDsWithPics = value; }
        }

        /// <summary>
        /// Gets or sets the Print Fit Mode.
        /// </summary>
        public PrintFitMode PrintFit
        {
            get { return printFitMode; }
            set { printFitMode = value; }
        }

        /// <summary>
        /// Gets or sets the font scale value.
        /// <remarks> Default value is <b>1</b></remarks>
        /// </summary>
        public float FontScale
        {
            get { return fontScale; }
            set { fontScale = value != 0 ? value : 0.01f; }
        }

        /// <summary>
        /// Enable or disable a resolution optimization.
        /// </summary>
        public bool PrintOptimized
        {
            get { return printOptimized; }
            set { printOptimized = value; }
        }

        /// <summary>
        /// Each report page is placed on a new Excel page.
        /// </summary>
        public bool SplitPages 
        {
            get { return splitPages; }
            set { splitPages = value; }
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
        public bool Wysiwyg
        {
            get { return wysiwyg; }
            set { wysiwyg = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether to insert page breaks in the output file or not.
        /// </summary>
        public bool PageBreaks
        {
            get { return pageBreaks; }
            set { pageBreaks = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether to export the databand rows only.
        /// </summary>
        public bool DataOnly
        {
            get { return dataOnly; }
            set { dataOnly = value; }
        }

        /// <summary>
        /// Enable or disable export of page footers and next page headers without table breaks.
        /// </summary>
        public bool Seamless
        {
            get { return seamless; }
            set { seamless = value; }
        }

        #endregion Properties

        #region Private Methods

        private void CreateThemes()
        {
            // get a reference to the current assembly
            Assembly a = Assembly.GetExecutingAssembly();

            // get a list of resource names from the manifest
            //string[] resNames = a.GetManifestResourceNames();

            using (Stream outStream = a.GetManifestResourceStream("FastReport.Resources.OoXML.theme1.xml"))
            {
                MemoryStream fs = new MemoryStream();
                const int BUFFER_SIZE = 4096;
                outStream.CopyTo(fs, BUFFER_SIZE);

                fs.Position = 0;
                Zip.AddStream("xl/theme/theme1.xml", fs);
            }
        }

        private void CreateContentTypes()
        {
            MemoryStream file = new MemoryStream();
            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.Write(file, "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">");

            CreateThemes();
            ExportUtils.Write(file,
                "<Override PartName=\"/xl/theme/theme1.xml\" " +
                "ContentType=\"application/vnd.openxmlformats-officedocument.theme+xml\" />");
            ExportUtils.Write(file, "<Override PartName=" + QuotedRoot("xl/" + docStyles.FileName) +
                " ContentType=" + Quoted(docStyles.ContentType) + "/>");
            ExportUtils.Write(file, "<Default Extension=\"rels\" " +
                "ContentType=\"application/vnd.openxmlformats-package.relationships+xml\" />");
            ExportUtils.Write(file, "<Default Extension=\"xml\" ContentType=\"application/xml\" />");
            ExportUtils.Write(file, "<Default Extension=\"png\" ContentType=\"image/png\"/>");

            ExportUtils.Write(file, "<Override PartName=" + QuotedRoot(workBook.FileName) +
                " ContentType=" + Quoted(workBook.ContentType) + "/>");
            ExportUtils.Write(file, "<Override PartName=" + QuotedRoot(appProp.FileName) +
                " ContentType=" + Quoted(appProp.ContentType) + "/>");

            CreateWorkbookRelations();

            foreach (OoXMLSheet sh in workBook.SheetList)
            {
                ExportUtils.Write(file, "<Override PartName=" + QuotedRoot("xl/" + sh.FileName) +
                    " ContentType=" + Quoted(sh.ContentType) + " />");
                //===johnny appended 2012/6/17
                if (SheetIDsWithPics.Contains(sh.SheetID))
                {
                    ExportUtils.Write(file, "<Override PartName=" + QuotedRoot("xl/drawings/drawing" + sh.SheetID + ".xml") +
                        " ContentType=" + Quoted(drawing.ContentType) + "/>");
                }
            }

            ExportUtils.Write(file, "<Override PartName=" + QuotedRoot("xl/" + stringTable.FileName) +
                " ContentType=" + Quoted(stringTable.ContentType) + "/>");
            ExportUtils.Write(file, "<Override PartName=" + QuotedRoot(coreDocProp.FileName) + " ContentType=" +
                Quoted(coreDocProp.ContentType) + "/>");

            ExportUtils.Write(file, "</Types>");

            file.Position = 0;
            Zip.AddStream("[Content_Types].xml", file);
        }

        private void CreateRelations()
        {
            MemoryStream file = new MemoryStream();
            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file,
                "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");
            ExportUtils.WriteLn(file, "<Relationship Id=\"rId3\" Type=" + Quoted(appProp.RelationType) +
                " Target=" + Quoted(appProp.FileName) + " />");
            ExportUtils.WriteLn(file, "<Relationship Id=\"rId2\" Type=" + Quoted(coreDocProp.RelationType) +
                " Target=" + Quoted(coreDocProp.FileName) + " />");
            ExportUtils.WriteLn(file, "<Relationship Id=\"rId1\" Type=" + Quoted(workBook.RelationType) +
                " Target=" + Quoted(workBook.FileName) + " />");
            ExportUtils.WriteLn(file, "</Relationships>");
            file.Position = 0;
            Zip.AddStream("_rels/.rels", file);
        }

        private void CreateWorkbookRelations()
        {
            MemoryStream file = new MemoryStream();
            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file,
                "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");
            ExportUtils.WriteLn(file, "<Relationship Id=\"rId3\" Type=" + Quoted(docStyles.RelationType) +
                " Target=" + Quoted(docStyles.FileName) + " />");
            ExportUtils.WriteLn(file, "<Relationship Id=\"rId2\" Type=" +
                Quoted("http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme") +
                " Target=" + Quoted("theme/theme1.xml") + " />");
            foreach (OoXMLSheet sh in workBook.SheetList)
            {
                ExportUtils.WriteLn(file, "<Relationship Id=" + Quoted(sh.rId) + " Type=" + Quoted(sh.RelationType) +
                    " Target=" + Quoted(sh.FileName) + " />");
            }
            ExportUtils.WriteLn(file, "<Relationship Id=\"rId6\" Type=" + Quoted(stringTable.RelationType) +
                " Target=" + Quoted(stringTable.FileName) + " />");
            ExportUtils.WriteLn(file, "</Relationships>");
            file.Position = 0;
            Zip.AddStream("xl/_rels/workbook.xml.rels", file);
        }

        internal string GetMatrxDimension(ExportMatrix Matrix)
        {
            if (Matrix.Width == 0 && Matrix.Height == 0)
                return "<dimension ref=\"A1\"/>";
            else
                return "<dimension ref=\"A1:" + ExportUtils.GetCellReference(Matrix.Width - 2, Matrix.Height) + "\"/>";
        }

        internal string GetPageSetUpPr()
        {
            if (PrintFit != PrintFitMode.NoScaling)
                return "<sheetPr><pageSetUpPr fitToPage=\"1\"/></sheetPr>";
            else
                return String.Empty;
        }

        private void ExportOOXML(Stream Stream)
        {
            CreateRelations();

            int current_style_set = 0;

            foreach (OoXMLSheet sh in workBook.SheetList)
            {
                sh.Matrix.Prepare();
                Drawing.Start();
                int styles_count = 0;

                if (FontScale != 1)
                    for (int i = 0; i < sh.Matrix.Styles.Count; i++)
                    {
                        Font font = sh.Matrix.Styles[i].Font;
                        float scaledSize = (float)Math.Round(font.Size * FontScale);
                        float newSize = scaledSize == 0 ? 1 : scaledSize;
                        sh.Matrix.Styles[i].Font = new Font(font.FontFamily, newSize, font.Style);
                    }

                styles_count = docStyles.Add(sh);
                sh.Export(this, current_style_set, pageBreaks);
                current_style_set = styles_count;

                Drawing.Stop(sh.SheetID);
            }

            docStyles.Export(this);
            appProp.Export(this);
            stringTable.Export(this);
            coreDocProp.Export(this);
            workBook.Export(this);

            CreateContentTypes();
        }

        #endregion Private Methods

        #region Protected Methods

        /// <inheritdoc/>
        protected override void Start()
        {
            base.Start();

            Zip = new ZipArchive();
            stringTable = new OoXMLSharedStringTable();
            uRLTable = new OoXMLSharedURLTable();
            drawing = new OoXMLDrawing(this);
            coreDocProp = new OoXMLCoreDocumentProperties();
            appProp = new OoXMLApplicationProperties();
            docStyles = new OoXMLDocumentStyles();
            workBook = new OoXMLWorkbook();
            sheets = new Hashtable();
        }

        /// <inheritdoc/>
        protected override void ExportPageBegin(ReportPage page)
        {
            base.ExportPageBegin(page);
            if (sheets.ContainsKey(page.Name) && !SplitPages)
            {
                sh = sheets[page.Name] as OoXMLSheet;
                sh.Matrix.AddPageBegin(page);
            }
            else
            {
                matrix = new ExportMatrix();
                if (wysiwyg)
                    matrix.Inaccuracy = 0.05f;
                else
                    matrix.Inaccuracy = 5;

                matrix.ShowProgress = ShowProgress;
                matrix.PlainRich = true;
                matrix.AreaFill = false;
                matrix.CropAreaFill = true;
                matrix.Report = Report;
                matrix.Images = true;
                if (printOptimized)
                    matrix.ImageResolution = 300;
                else
                    matrix.ImageResolution = 150;
                matrix.WrapText = false;
                matrix.FullTrust = false;
                matrix.DataOnly = DataOnly;
                matrix.MaxCellHeight = 409 * oxmlYDivider;
                matrix.Seamless = seamless;
                matrix.AddPageBegin(page);
            }
        }

        /// <inheritdoc/>
        protected override void ExportBand(Base band)
        {
            base.ExportBand(band);
            if (sheets.ContainsKey(band.Page.Name) && !SplitPages)
            {
                sh = sheets[band.Page.Name] as OoXMLSheet;
                sh.Matrix.AddBand(band, this);
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
                sh = sheets[page.Name] as OoXMLSheet;
                sh.Matrix.AddPageEnd(page);
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
                sh = new OoXMLSheet(matrix, String.IsNullOrEmpty(page.ExportAlias)? page.Name : page.ExportAlias, sheets.Count + 1);
                sheets[new_page_name] = sh;
                workBook.AddSheet(sh);
            }
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            ExportOOXML(Stream);
            Zip.SaveToStream(Stream);
            Zip.Clear();

            foreach (OoXMLSheet sheet in workBook.SheetList)
                sheet.Matrix.Dispose();
            GC.Collect(2);
        }

        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            return new MyRes("FileFilters").Get("XlsxFile");
        }

        #endregion Protected Methods


        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            base.Serialize(writer);
            writer.WriteBool("Wysiwyg", Wysiwyg);
            writer.WriteBool("PageBreaks", PageBreaks);
            writer.WriteBool("DataOnly", DataOnly);
            writer.WriteBool("Seamless", Seamless);
            writer.WriteBool("PrintOptimized", PrintOptimized);
            writer.WriteBool("SplitPages", SplitPages);
            writer.WriteFloat("FontScale", FontScale);
            writer.WriteValue("PrintFit", PrintFit);
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Excel2007Export()
        {
            res = new MyRes("Export,Misc");
            pageBreaks = true;
            seamless = false;
            wysiwyg = true;
            printFitMode = PrintFitMode.NoScaling;
            sheetIDsWithPics = new List<string>();
        }
    }
}