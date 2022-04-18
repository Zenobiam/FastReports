using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using FastReport.Utils;
using FastReport.RichTextParser;

namespace FastReport.Export.RichText
{

    /// <summary>
    /// Specifies the image format in RTF export.
    /// </summary>
    public enum RTFImageFormat
    {
        /// <summary>
        /// Specifies the .png format.
        /// </summary>
        Png,

        /// <summary>
        /// Specifies the .jpg format.
        /// </summary>
        Jpeg,

        /// <summary>
        /// Specifies the .emf format.
        /// </summary>
        Metafile
    }

    /// <summary>
    /// Represents the RTF export filter.
    /// </summary>
    public partial class RTFExport : ExportBase
    {
        #region Constants
        const float Xdivider = 15.05F;
        const float Ydivider1 = 14.8F;
        const float Ydivider2 = 14.8F;
        const float Ydivider3 = 14.7F;
        const float MargDivider = 56.695239F;
        const float FONT_DIVIDER = 15F;
        const float IMAGE_DIVIDER = 25.3F;
        const int PIC_BUFF_SIZE = 512;
        #endregion

        #region Private fields
        private List<string> colorTable;
        private bool pageBreaks;
        private List<string> fontTable;
        private ExportMatrix matrix;
        private bool wysiwyg;
        private string creator;
        private bool autoSize;
        private MyRes res;
        private RTFImageFormat imageFormat;
        private bool pictures;
        private string tempFile;
        private int jpegQuality;
        private float dpiFactor;
        private float yDiv;
        private Color textColor;
        private bool keepRichText;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the quality of Jpeg images in RTF file.
        /// </summary>
        /// <remarks>
        /// Default value is 90. This property will be used if you select Jpeg 
        /// in the <see cref="ImageFormat"/> property.
        /// </remarks>
        public int JpegQuality
        {
            get { return jpegQuality; }
            set { jpegQuality = value; }
        }

        /// <summary>
        /// Gets or sets the image format that will be used to save pictures in RTF file.
        /// </summary>
        /// <remarks>
        /// Default value is <b>Metafile</b>. This format is better for exporting such objects as
        /// <b>MSChartObject</b> and <b>ShapeObject</b>.
        /// </remarks>
        public RTFImageFormat ImageFormat
        {
            get { return imageFormat; }
            set { imageFormat = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that pictures are enabled.
        /// </summary>
        public bool Pictures
        {
            get { return pictures; }
            set { pictures = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that page breaks are enabled.
        /// </summary>
        public bool PageBreaks
        {
            get { return pageBreaks; }
            set { pageBreaks = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the wysiwyg mode should be used 
        /// for better results.
        /// </summary>
        /// <remarks>
        /// Default value is <b>true</b>. In wysiwyg mode, the resulting rtf file will look
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
        /// Gets or sets the creator of the document.
        /// </summary>
        public string Creator
        {
            get { return creator; }
            set { creator = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the rows in the resulting table 
        /// should calculate its height automatically.
        /// </summary>
        /// <remarks>
        /// Default value for this property is <b>false</b>. In this mode, each row in the
        /// resulting table has fixed height to get maximum wysiwyg. If you set it to <b>true</b>,
        /// the height of resulting table will be calculated automatically by the Word processor.
        /// The document will be more editable, but less wysiwyg.
        /// </remarks>
        public bool AutoSize
        {
            get { return autoSize; }
            set { autoSize = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the repot's RichObject  will be
        /// translated as picture or joined to generated RTF.
        /// </summary>
        /// <remarks>
        /// Default value for this property is <b>false</b>. In this mode, each RichObject
        /// will be embedded as a picture. This is default behavior.  If you set it to <b>true</b>,
        /// the RichObject will be incorporated as a navive part of document. This is experimetal
        /// feature.
        /// </remarks>
        public bool EmbedRichObject
        {
            get { return keepRichText; }
            set { keepRichText = value; }
        }
        #endregion

        #region Private Methods

        private string GetRTFBorders(ExportIEMStyle Style)
        {

            //// +debug
            //Style.Border.Lines = BorderLines.All;
            //Style.Border.Width = 1;
            //// -debug

            StringBuilder result = new StringBuilder(256);
            // top
            if ((Style.Border.Lines & BorderLines.Top) > 0)
                result.Append("\\clbrdrt").
                    Append(GetRTFLineStyle(Style.Border.TopLine.Style)).
                    Append("\\brdrw").
                    Append(((int)Math.Round(Style.Border.TopLine.Width * 20)).ToString()).
                    Append("\\brdrcf").
                    Append(GetRTFColorFromTable(GetRTFColor(Style.Border.TopLine.Color)));
            // left
            if ((Style.Border.Lines & BorderLines.Left) > 0)
                result.Append("\\clbrdrl").
                    Append(GetRTFLineStyle(Style.Border.LeftLine.Style)).
                    Append("\\brdrw").
                    Append(((int)Math.Round(Style.Border.LeftLine.Width * 20)).ToString()).
                    Append("\\brdrcf").
                    Append(GetRTFColorFromTable(GetRTFColor(Style.Border.LeftLine.Color)));
            // bottom
            if ((Style.Border.Lines & BorderLines.Bottom) > 0)
                result.Append("\\clbrdrb").
                    Append(GetRTFLineStyle(Style.Border.BottomLine.Style)).
                    Append("\\brdrw").
                    Append(((int)Math.Round(Style.Border.BottomLine.Width * 20)).ToString()).
                    Append("\\brdrcf").
                    Append(GetRTFColorFromTable(GetRTFColor(Style.Border.BottomLine.Color)));
            // right
            if ((Style.Border.Lines & BorderLines.Right) > 0)
                result.Append("\\clbrdrr").
                    Append(GetRTFLineStyle(Style.Border.RightLine.Style)).
                    Append("\\brdrw").
                    Append(((int)Math.Round(Style.Border.RightLine.Width * 20)).ToString()).
                    Append("\\brdrcf").
                    Append(GetRTFColorFromTable(GetRTFColor(Style.Border.RightLine.Color)));
            return result.ToString();
        }

        private string GetRTFLineStyle(LineStyle lineStyle)
        {
            switch (lineStyle)
            {
                case LineStyle.Dash:
                    return "\\brdrdash";
                case LineStyle.DashDot:
                    return "\\brdrdashd";
                case LineStyle.DashDotDot:
                    return "\\brdrdashdd";
                case LineStyle.Dot:
                    return "\\brdrdot";
                case LineStyle.Double:
                    return "\\brdrdb";
                default:
                    return "\\brdrs";
            }
        }

        private string GetRTFColor(Color c)
        {
            StringBuilder result = new StringBuilder(64);
            result.Append("\\red").Append(Convert.ToString(c.R)).
                Append("\\green").Append(Convert.ToString(c.G)).
                Append("\\blue").Append(Convert.ToString(c.B)).Append(";");
            return result.ToString();
        }

        private string GetRTFFontStyle(FontStyle f)
        {
            StringBuilder result = new StringBuilder(8);
            if ((f & FontStyle.Italic) != 0)
                result.Append("\\i");
            if ((f & FontStyle.Bold) != 0)
                result.Append("\\b");
            if ((f & FontStyle.Underline) != 0)
                result.Append("\\ul");
            return result.ToString();
        }

        private string GetRTFColorFromTable(string f)
        {
            string Result;
            int i = colorTable.IndexOf(f);
            if (i != -1)
                Result = (i + 1).ToString();
            else
            {
                colorTable.Add(f);
                Result = colorTable.Count.ToString();
            }
            return Result;
        }

        private string GetRTFFontName(string f)
        {
            string Result;
            int i = fontTable.IndexOf(f);
            if (i != -1)
                Result = (i).ToString();
            else
            {
                fontTable.Add(f);
                Result = (fontTable.Count - 1).ToString();
            }
            return Result;
        }

        private string GetRTFHAlignment(HorzAlign HAlign)
        {
            switch (HAlign)
            {
                case HorzAlign.Right:
                    return "\\qr";
                case HorzAlign.Center:
                    return "\\qc";
                case HorzAlign.Justify:
                    return "\\qj";
                default:
                    return "\\ql";
            }
        }

        private string GetRTFVAlignment(VertAlign VAlign)
        {
            switch (VAlign)
            {
                case VertAlign.Top:
                    return "\\clvertalt";
                case VertAlign.Center:
                    return "\\clvertalc";
                default:
                    return "\\clvertalb";
            }
        }

        private string StrToRTFSlash(string Value)
        {
            StringBuilder Result = new StringBuilder();
            for (int i = 0; i < Value.Length; i++)
            {
                if (Value[i] == '\\')
                    Result.Append("\\\\");
                else if (Value[i] == '{')
                    Result.Append("\\{");
                else if (Value[i] == '}')
                    Result.Append("\\}");
                else if ((Value[i] == '\r') && (i < (Value.Length - 1)) && (Value[i + 1] == '\n'))
                {
                    Result.Append("\\line\r\n");
                    i++;
                }
                else
                    Result.Append(Value[i]);
            }
            return Result.ToString();
        }

        private string ParseHtmlTags(string s)
        {

            int Index = 0;
            int Begin = 0;
            int End = 0;
            string Tag;
            string Text;
            string result;
            string TagClose = "";
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

            Stack<CurrentStyle> style_stack = new Stack<CurrentStyle>();


            Begin = s.IndexOfAny(new char[1] { '<' }, Index);

            if (Begin == -1) result = s;
            else
            {
                result = "";
                while (Begin != -1)
                {
                    if (Begin != 0 && Index == 0)
                    {
                        if (Index == 0)
                        {
                            result += s.Substring(Index, Begin);
                        }
                    }

                    End = s.IndexOfAny(new char[1] { '>' }, Begin + 1);
                    if (End == -1) break;

                    Tag = s.Substring(Begin + 1, End - Begin - 1);

                    bool CloseTag = Tag.StartsWith("/");

                    if (CloseTag) Tag = Tag.Remove(0, 1);

                    string[] items = Tag.Split(' ');

                    Tag = items[0].ToUpper();
                    TagClose = "";

                    bool PutOnStack = true;

                    if (!CloseTag)
                    {
                        current_style.LastTag = Tag;
                        switch (Tag)
                        {
                            case "B":
                                current_style.Bold = true;
                                TagClose = "\\b ";
                                break;
                            case "I":
                                current_style.Italic = true;
                                TagClose = "\\i ";
                                break;
                            case "U":
                                current_style.Underline = true;
                                TagClose = "\\ul ";
                                break;
                            case "STRIKE": current_style.Strike = true; break;
                            case "SUB": current_style.Sub = true; break;
                            case "SUP": current_style.Sup = true; break;
                            case "FONT":
                                {
                                    if (items.Length > 1)
                                    {
                                        string[] attrs = items[1].Split('=');
                                        if (attrs[0] == "color")
                                        {
                                            TagClose = "\\cf" + GetRTFColorFromTable(GetRTFColor(System.Drawing.ColorTranslator.FromHtml(attrs[1].Replace("\"", "")))) + " ";

                                        }
                                    }
                                }
                                /*current_style.Font = items[1];*/
                                //                            ParseFont(items[1], current_style, out current_style);
                                break;
                            default:
                                TagClose = Tag;
                                PutOnStack = false;
                                break;
                        }
                        if (PutOnStack) style_stack.Push(current_style);
                    }
                    else
                    {
                        if (style_stack.Count > 0)
                        {
                            previos_style = style_stack.Pop();
#if false
                            if (previos_style.LastTag != Tag)
                            {
                                throw new Exception("Unaligned HTML TAGS");
                            }
#endif
                            switch (Tag)
                            {
                                case "B": TagClose = "\\b0 "; break;
                                case "I": TagClose = "\\i0 "; break;
                                case "U": TagClose = "\\ul0 "; break;
                                case "STRIKE": break;
                                case "SUB": break;
                                case "SUP": break;
                                case "FONT":
                                    TagClose = "\\cf" + GetRTFColorFromTable(GetRTFColor((textColor))) + " ";
                                    /*current_style.Font = items[1];*/
                                    //                            ParseFont(items[1], current_style, out current_style);
                                    break;
                                default:
                                    throw new Exception("Unsupported HTML TAG");
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
                    result += TagClose + Text;
                }
            }

            return result;
        }

        private string StrToRTFUnicodeEx(string Value, TextRenderType textRenderType)
        {
            Value = StrToRTFUnicode(StrToRTFSlash(Value));
            switch (textRenderType)
            {
                case TextRenderType.HtmlParagraph:
                    //TODO DETRAV RTF
                    break;
                case TextRenderType.HtmlTags:
                    Value = ParseHtmlTags(Value);
                    break;
            }
            return Value;
        }

        private string StrToRTFUnicode(string Value)
        {
            StringBuilder Result = new StringBuilder(128);
            foreach (UInt16 c in Value)
            {
                if (c > 127)
                    Result.Append("\\u").Append(c.ToString()).Append("\\'3f");
                else
                    Result.Append((char)c);
            }
            return Result.ToString();
        }

        private void Prepare()
        {
            int i;
            ExportIEMObject Obj;
            for (int y = 0; y < matrix.Height; y++)
                for (int x = 0; x < matrix.Width; x++)
                {
                    i = matrix.Cell(x, y);
                    if (i != -1)
                    {
                        Obj = matrix.ObjectById(i);
                        if (Obj.Counter != -1)
                        {
                            Obj.Counter = -1;
                            if (Obj.Style != null)
                            {
                                GetRTFColorFromTable(GetRTFColor(Obj.Style.FillColor));
                                GetRTFColorFromTable(GetRTFColor(Obj.Style.Border.LeftLine.Color));
                                GetRTFColorFromTable(GetRTFColor(Obj.Style.Border.RightLine.Color));
                                GetRTFColorFromTable(GetRTFColor(Obj.Style.Border.TopLine.Color));
                                GetRTFColorFromTable(GetRTFColor(Obj.Style.Border.BottomLine.Color));
                                GetRTFColorFromTable(GetRTFColor(Obj.Style.TextColor));
                                GetRTFFontName(Obj.Style.Font.Name);
                            }
                        }
                    }
                }
        }

        private string SetPageProp(int Page)
        {
            StringBuilder result = new StringBuilder(64);
            result.Append("\\pgwsxn").
                Append(((int)Math.Round(matrix.PageWidth(Page) * MargDivider)).ToString()).
                Append("\\pghsxn").
                Append(((int)Math.Round(matrix.PageHeight(Page) * MargDivider)).ToString()).
                Append("\\marglsxn").
                Append(((int)Math.Round(matrix.PageLMargin(Page) * MargDivider)).ToString()).
                Append("\\margrsxn").
                Append(((int)Math.Round(matrix.PageRMargin(Page) * MargDivider)).ToString()).
                Append("\\margtsxn").
                Append(((int)Math.Round(matrix.PageTMargin(Page) * MargDivider)).ToString()).
                Append("\\margbsxn").
                Append(((int)Math.Round(matrix.PageBMargin(Page) * MargDivider)).ToString()).
                Append(matrix.Landscape(Page) ? "\\lndscpsxn" : String.Empty);
            return result.ToString();
        }

        private void Write(Stream stream, string str)
        {
            byte[] buff = Converter.StringToByteArray(str);
            stream.Write(buff, 0, buff.Length);
        }

        private void WriteLine(Stream stream, string str)
        {
            Write(stream, str);
            Write(stream, "\r\n");
        }

        private Stream GetTempFileStream()
        {
            tempFile = Path.Combine(Config.GetTempFolder(), Path.GetRandomFileName());
            return new FileStream(tempFile, FileMode.Create);
        }

        private void DeleteTempFile()
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }

        private string GetRTFString(ExportIEMObject obj, float drow)
        {
            ExportIEMStyle style = obj.Style;
            string text = obj.Text;
            TextRenderType textRenderType = obj.TextRenderType;

            StringBuilder CellsStream = new StringBuilder();
            
            CellsStream.Append(GetRTFHAlignment(style.HAlign));

            if (!String.IsNullOrEmpty(obj.URL))
            {
                CellsStream.Append("{\\field{\\*\\fldinst HYPERLINK \"" + obj.URL + "\"}{\\fldrslt");
            }
            else
            {
                CellsStream.Append("{");
            }
            
            CellsStream.Append("\\f").Append(GetRTFFontName(style.Font.Name));

            string s = StrToRTFUnicodeEx(ExportUtils.TruncReturns(text), textRenderType);

            double fh = style.Font.Height * dpiFactor * yDiv * 0.98;
            double lh = style.LineHeight * 8.0;

            if (s.Length > 0)
            {
                CellsStream.Append("\\fs").Append((Math.Round(style.Font.Size * 2)).ToString());
                CellsStream.Append(GetRTFFontStyle(style.Font.Style)).Append("\\cf");
                CellsStream.Append(GetRTFColorFromTable(GetRTFColor((style.TextColor))));
                CellsStream.Append(style.RTL ? "\\rtlch" : String.Empty);

                CellsStream.Append("\\sb").
                    Append(((int)Math.Round(style.Padding.Top * yDiv)).ToString()).
                    Append("\\sa").
                    Append(((int)Math.Round(style.Padding.Bottom * yDiv)).ToString()).
                    Append("\\li").
                    Append(((int)Math.Round((style.Padding.Left) * Xdivider)).ToString()).
                    Append("\\ri").
                    Append(((int)Math.Round((style.Padding.Right) * Xdivider)).ToString()).
                    Append("\\sl-").
                    Append(((int)Math.Round((fh + lh))).ToString()).
                    Append("\\slmult0 ");
                if (s.StartsWith("\t") && style.FirstTabOffset != 0)
                {
                    // replace first tab symbol with \\fi
                    s = "\\fi" + ((int)Math.Round((style.FirstTabOffset) * Xdivider)).ToString() + " " + s.Remove(0, 1);
                    // convert multiline text to rtf paragraphs. \\fi can be applied to a paragraph only
                    s = s.Replace("\\line\r\n\t", "\\par ");
                }
                CellsStream.Append(s);
            }
            else
            {
                int j = (int)(drow / FONT_DIVIDER);
                j = j > 20 ? 20 : j;
                CellsStream.Append("\\fs").Append(j.ToString());
            }
            if (!String.IsNullOrEmpty(obj.URL))
            {
                CellsStream.Append("}");
            }
            CellsStream.Append("\\cell}");
            return CellsStream.ToString();
        }

        private string GetRTFMetafile(ExportIEMObject Obj)
        {
            byte[] picbuff = new Byte[PIC_BUFF_SIZE];
            string scale = ((int)(100 / dpiFactor)).ToString();
            StringBuilder CellsStream = new StringBuilder(256);
            if (!String.IsNullOrEmpty(Obj.URL))
            {
                CellsStream.Append("{\\field{\\*\\fldinst HYPERLINK \"" + Obj.URL + "\"}{\\fldrslt");
            }
            else
            {
                CellsStream.Append("{");
            }
            Obj.PictureStream.Position = 0;
            if (pictures && Config.FullTrust && (imageFormat == RTFImageFormat.Metafile))
            {
                scale = (int.Parse(scale) * dpiFactor).ToString();
                CellsStream.Append("\\sb0\\li0\\sl0\\slmult0\\qc\\clvertalc {");
                float dx = Obj.Width * 15;
                float dy = Obj.Height * 15;
                CellsStream.Append("\\pict\\picw").Append(dx.ToString());
                CellsStream.Append("\\pich").Append(dy.ToString());
                CellsStream.Append("\\picscalex").Append(scale);
                CellsStream.Append("\\picscaley").Append(scale);
                CellsStream.Append("\\picwGoal").Append(Convert.ToString(dx));
                CellsStream.Append("\\pichGoal").Append(Convert.ToString(dy));
                CellsStream.Append("\\emfblip\r\n");
                int n;
                do
                {
                    n = Obj.PictureStream.Read(picbuff, 0, PIC_BUFF_SIZE);
                    for (int z = 0; z < n; z++)
                    {
                        CellsStream.Append(ExportUtils.XCONV[picbuff[z] >> 4]);
                        CellsStream.Append(ExportUtils.XCONV[picbuff[z] & 0xF]);
                    }
                    CellsStream.Append("\r\n");
                }
                while (n == PIC_BUFF_SIZE);
                CellsStream.Append("}");
            }
            else if (pictures && (imageFormat != RTFImageFormat.Metafile))
            {
                CellsStream.Append("\\sb0\\li0\\sl0\\slmult0\\qc\\clvertalc {");
                float dx = (int)Obj.Width;
                float dy = (int)Obj.Height;
                CellsStream.Append("\\pict\\picw").Append(dx.ToString());
                CellsStream.Append("\\pich").Append(dy.ToString());
                CellsStream.Append("\\picscalex").Append(scale);
                CellsStream.Append("\\picscaley").Append(scale);
                CellsStream.Append("\\");
                CellsStream.Append(imageFormat == RTFImageFormat.Jpeg ? "jpegblip\r\n" : "pngblip\r\n");
                int n;
                do
                {
                    n = Obj.PictureStream.Read(picbuff, 0, PIC_BUFF_SIZE);
                    for (int z = 0; z < n; z++)
                    {
                        CellsStream.Append(ExportUtils.XCONV[picbuff[z] >> 4]);
                        CellsStream.Append(ExportUtils.XCONV[picbuff[z] & 0xF]);
                    }
                    CellsStream.Append("\r\n");
                }
                while (n == PIC_BUFF_SIZE);
                CellsStream.Append("}");
            }
            if (!String.IsNullOrEmpty(Obj.URL))
            {
                CellsStream.Append("}");
            }
            CellsStream.Append("\\cell}\r\n");
            return CellsStream.ToString();
        }

        private string GetRTFHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\\rtf1\\ansi");
            sb.Append("{\\fonttbl");
            for (int i = 0; i < fontTable.Count; i++)
                sb.Append("{\\f" + Convert.ToString(i) + " " + fontTable[i] + "}");
            sb.Append("}");
            sb.Append("{\\colortbl;");
            for (int i = 0; i < colorTable.Count; i++)
                sb.Append(colorTable[i]);
            sb.Append("}");
            return sb.ToString();
        }

        private string GetRTFMetaInfo()
        {
            StringBuilder buf = new StringBuilder(256);
            buf.Append("{\\info{\\title ").Append(StrToRTFUnicodeEx(Report.ReportInfo.Name, TextRenderType.Default)).
                Append("}{\\author ").Append(StrToRTFUnicodeEx(creator, TextRenderType.Default)).
                Append("}{\\creatim\\yr").Append(String.Format("{0:yyyy}", SystemFake.DateTime.Now)).
                Append("\\mo").Append(String.Format("{0:MM}", SystemFake.DateTime.Now)).
                Append("\\dy").Append(String.Format("{0:dd}", SystemFake.DateTime.Now)).
                Append("\\hr").Append(String.Format("{0:HH}", SystemFake.DateTime.Now)).
                Append("\\min").Append(String.Format("{0:mm}", SystemFake.DateTime.Now)).AppendLine("}}");
            return buf.ToString();
        }

        struct CurrentStyle
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
        };

        #region Export RichObject to RTF
        private RichDocument rtf;
        private RunFormat current_format;
        private RichTextParser.ParagraphFormat.HorizontalAlign prev_align = RichTextParser.ParagraphFormat.HorizontalAlign.Left;

        private string HorizontalAlignCode(RichTextParser.ParagraphFormat.HorizontalAlign align)
        {
            switch (align)
            {
                case RichTextParser.ParagraphFormat.HorizontalAlign.Centered: return "\\qc";
                case RichTextParser.ParagraphFormat.HorizontalAlign.Left: return "\\ql";
                case RichTextParser.ParagraphFormat.HorizontalAlign.Right: return "\\qr";
                case RichTextParser.ParagraphFormat.HorizontalAlign.Justified: return "\\qj";
                case RichTextParser.ParagraphFormat.HorizontalAlign.Distributed: return "\\qd";
                case RichTextParser.ParagraphFormat.HorizontalAlign.Kashida: return "\\qk";
                case RichTextParser.ParagraphFormat.HorizontalAlign.Thai: return "\\qt";
                default: return "";
            }
        }

        private string VerticalAlignCode(Column.VertAlign valign)
        {
            switch (valign)
            {
                case Column.VertAlign.Top: return "\\clvertalt";
                case Column.VertAlign.Center: return "\\clvertalc";
                case Column.VertAlign.Bottom: return "\\clvertalb";
                default: return "";
            }
        }

        private void SavePargraph(StringBuilder s, RichTextParser.Paragraph par, bool InTable)
        {
            if (par.runs.Count == 0)
            {
                if (!InTable)
                    s.Append("\\pard\\par\n");
                else
                    s.Append("\\pard\\intbl\\cell\n");
            }
            else
            {
                if (InTable)
                    s.Append("\\intbl\n");

                if (par.format.align != prev_align)
                {
                    s.AppendFormat("{0} ", HorizontalAlignCode(par.format.align));
                    prev_align = par.format.align;
                }

                foreach (Run r in par.runs)
                {
                    RunFormat fmt = r.format;
                    if (current_format.bold != fmt.bold)
                    {
                        s.Append(fmt.bold ? "\\b" : "\\b0");
                        current_format.bold = fmt.bold;
                    }
                    if (current_format.underline != fmt.underline)
                    {
                        s.Append(fmt.underline ? "\\ul" : "\\ulnone");
                        current_format.underline = fmt.underline;
                    }
                    if (current_format.italic != fmt.italic)
                    {
                        s.Append(fmt.italic ? "\\i" : "\\i0");
                        current_format.italic = fmt.italic;
                    }
                    if(current_format.color.ToArgb() != fmt.color.ToArgb())
                    {
                        string color_def = GetRTFColor(fmt.color);
                        int i = colorTable.IndexOf(color_def);
                        if (i != -1)
                        {
                            s.AppendFormat("\\cf{0}", i+1);
                        }
                        else
                        {
                            colorTable.Add(color_def);
                            s.AppendFormat("\\cf{0}", colorTable.Count);
                        }
                        current_format.color = fmt.color;
                    }
                    if (current_format.font_size != fmt.font_size)
                    {
                        s.AppendFormat("\\fs{0}", fmt.font_size);
                        current_format.font_size = fmt.font_size;
                    }
                    if (current_format.font_idx != fmt.font_idx)
                    {
                        string font_name = rtf.font_list[(int)fmt.font_idx].FontName;
                        int i = fontTable.IndexOf(font_name);
                        if (i != -1)
                        {
                            s.AppendFormat("\\f{0}", i);
                        }
                        else
                        {
                            fontTable.Add(font_name);
                            s.AppendFormat("\\f{0}", (fontTable.Count - 1));
                        }
                        current_format.font_idx = fmt.font_idx;
                    }
                    s.Append(" ");
                    if (r.text == "\t")
                        s.Append("\\tab ");
                    else
                        s.Append(StrToRTFUnicode(r.text));
                }
                if (!InTable)
                    s.Append("\\par\n");
                else
                    s.Append("\\cell\n");
            }
        }

        private void SaveColumn(StringBuilder s, Column col)
        {
            // "clcbpat" - background color
            s.AppendFormat("{0}\\cellx{1}", VerticalAlignCode(col.valign), col.Width);
        }

        private void SaveRow(StringBuilder s, TableRow row, bool InTable)
        {
            s.AppendFormat("\\trgaph{0}\\trrh{1}\\trpaddl{2}\\trpaddr{3}",
              row.trgaph, row.height, row.default_pad_left, row.default_pad_right);

            foreach (RichObjectSequence seq in row.cells)
            {
                SaveSequence(s, seq, true);
                if(!InTable)
                    s.AppendLine("\\cell");
                else
                    s.AppendLine("\\nestcell");
            }
            if (!InTable)
                s.Append("\\row");
            else
                s.Append("\\nestrow");
        }

        private void SaveTable(StringBuilder s, RichTextParser.Table tbl, bool InTable)
        {
            s.Append("\\trowd");
            foreach (Column col in tbl.columns)
                SaveColumn(s, col);
            foreach (TableRow row in tbl.rows)
                SaveRow(s, row, InTable);
       }

        private void SavePicture(StringBuilder CellsStream, Picture pic, bool InTable)
        {
            CellsStream.Append("\\sb0\\li0\\sl0\\slmult0\\qc\\clvertalc {");
            float dx = (int)pic.width;
            float dy = (int)pic.height;
            CellsStream.Append("\\pict\\picw").Append(dx.ToString());
            CellsStream.Append("\\pich").Append(dy.ToString());
            CellsStream.Append("\\picscalex").Append(pic.scalex);
            CellsStream.Append("\\picscaley").Append(pic.scaley);
            System.Drawing.Imaging.ImageFormat format;
            if(imageFormat == RTFImageFormat.Jpeg)
            {
                CellsStream.Append("\\jpegblip\r\n");
                format = System.Drawing.Imaging.ImageFormat.Jpeg;
            }
            else
            {
                CellsStream.Append("\\pngblip\r\n");
                format = System.Drawing.Imaging.ImageFormat.Png;
            }
            int n;
            byte[] picbuff = new Byte[PIC_BUFF_SIZE];
            MemoryStream pic_stream = new MemoryStream();
            pic.image.Save(pic_stream, format);
            pic_stream.Seek(0, SeekOrigin.Begin);
            do
            {
                n = pic_stream.Read(picbuff, 0, PIC_BUFF_SIZE);
                for (int z = 0; z < n; z++)
                {
                    CellsStream.Append(ExportUtils.XCONV[picbuff[z] >> 4]);
                    CellsStream.Append(ExportUtils.XCONV[picbuff[z] & 0xF]);
                }
                CellsStream.Append("\r\n");
            }
            while (n == PIC_BUFF_SIZE);
            CellsStream.Append("}");
        }

        private void SaveSequence(StringBuilder s, RichObjectSequence seq, bool InTable)
        {
            for(int i = 0; i < seq.objects.Count; i++)
            {
                if (!InTable && i == seq.objects.Count - 1)
                    InTable = true;

                switch (seq.objects[i].type)
                {
                    case RichTextParser.RichObject.Type.Paragraph:
                        SavePargraph(s, seq.objects[i].pargraph, InTable);
                        break;
                    case RichTextParser.RichObject.Type.Picture:
                        SavePicture(s, seq.objects[i].picture, InTable);
                        break;
                    case RichTextParser.RichObject.Type.Table:
                        SaveTable(s, seq.objects[i].table, InTable);
                        break;
                }
            }
        }

        private void TranslateEmbeddedRTF(ExportIEMObject Obj, StringBuilder CellsStream)
        {
            current_format.color = Color.FromArgb(0, 0, 0, 0);
            current_format.BColor = Color.FromArgb(0, 0, 0, 0);
            current_format.FillColor = Color.FromArgb(0, 0, 0, 0);
            current_format.bold = false;
            current_format.italic = false;
            current_format.underline = false;
            current_format.font_size = 0;
            current_format.font_idx = 65536;
            current_format.script_type = RunFormat.ScriptType.PlainText;

            using (RTF_DocumentParser parser = new RTF_DocumentParser())
            {
                parser.Load(Obj.Text);
                rtf = parser.Document;
            }

            foreach (Page page in rtf.pages)
            {
                CellsStream.AppendFormat(@"\margl{0}\margr{1}\margt{2}\margb{3}", 
                    200, // page.margin_left, 
                    page.margin_right, 
                    600, // page.margin_top, 
                    page.margin_bottom);
                SaveSequence(CellsStream, page.sequence, false);
            }
        }
#endregion

        private void ExportRTF(Stream stream)
        {
            int i, j, x, fx, fy, dx, dy, pbk;
            int dcol, drow, xoffs;
            ExportIEMObject Obj;
            Prepare();
            //Write a header is below now   

            pbk = 0;
            Write(stream, SetPageProp(pbk));

            if (ShowProgress)
                Config.ReportSettings.OnProgress(Report, res.Get("SavePage") + " " + (pbk + 1).ToString());

            for (int y = 0; y < matrix.Height - 1; y++)
            {
                if (pageBreaks)
                    if (pbk < matrix.PagesCount)
                        if (matrix.PageBreak(pbk) <= matrix.YPosById(y))
                        {
                            //                            WriteLine(stream, "\\pagebb\\sect");
                            WriteLine(stream, "\\pard\\sect");
                            pbk++;
                            if (pbk < matrix.PagesCount)
                                Write(stream, SetPageProp(pbk));

                            if (ShowProgress)
                                Config.ReportSettings.OnProgress(Report,
                                res.Get("SavePage") + " " + (pbk + 1).ToString());

                        }
                if (pbk == matrix.PagesCount - 1)
                    yDiv = Ydivider3;
                else if (pbk > 0)
                    yDiv = Ydivider1;
                else
                    yDiv = Ydivider2;
                drow = (int)Math.Round((matrix.YPosById(y + 1) - matrix.YPosById(y)) * yDiv);

                StringBuilder buff = new StringBuilder(512);
                buff.Append(autoSize ? "\\trrh" : "\\trrh-" + (drow).ToString() + "\\trgaph15");

                xoffs = (int)Math.Round(matrix.XPosById(0));
                StringBuilder CellsStream = new StringBuilder();

                for (x = 0; x <= matrix.Width - 2; x++)
                {
                    i = matrix.Cell(x, y);
                    if (i != -1)
                    {
                        Obj = matrix.ObjectById(i);
                        matrix.ObjectPos(i, out fx, out fy, out dx, out dy);
                        if (Obj.Counter == -1)
                        {
                            if (dy > 1)
                                buff.Append("\\clvmgf");
                            if (Obj.Style != null)
                            {
                                buff.Append("\\clcbpat").Append(GetRTFColorFromTable(GetRTFColor(Obj.Style.FillColor)));
                                buff.Append(GetRTFVAlignment(Obj.Style.VAlign)).Append(GetRTFBorders(Obj.Style))
                                    .Append("\\cltxlrtb");
                                if (Obj.Style.Angle == 90)
                                    buff.Append(" \\cltxtbrl ");
                                else if (Obj.Style.Angle == 270)
                                    buff.Append(" \\cltxbtlr ");
                            }

                            dcol = (int)Math.Round((Obj.Left + Obj.Width - xoffs) * Xdivider);

                            buff.Append("\\cellx").Append(dcol.ToString());

                            if (Obj.IsText)
                            {
                                if (Obj.IsRichText)
                                {
                                    // Write a rich text
                                    StringBuilder EmneddedRichStream = new StringBuilder();
                                    TranslateEmbeddedRTF(Obj, EmneddedRichStream);
                                    string debug = EmneddedRichStream.ToString();
                                    CellsStream.Append(EmneddedRichStream);
                                }
                                else
                                {
                                    // Write a text 
                                    textColor = Obj.Style.TextColor;
                                    CellsStream.AppendLine(GetRTFString(Obj, drow));
                                }
                            }
                            else
                            {
                                // Write a picture
                                CellsStream.Append(GetRTFMetafile(Obj));
                            }
                            Obj.Counter = y + 1;
                        }
                        else
                        {
                            if ((dy > 1) && (Obj.Counter != y + 1))
                            {
                                buff.Append("\\clvmrg").
                                    Append((Obj.Style != null) ? GetRTFBorders(Obj.Style) : String.Empty).
                                    Append("\\cltxlrtb");
                                dcol = (int)Math.Round((Obj.Left + Obj.Width - xoffs) * Xdivider);
                                buff.Append("\\cellx").Append(dcol.ToString());
                                j = (int)(drow / FONT_DIVIDER);
                                j = j > 20 ? 20 : j;
                                CellsStream.Append("{\\fs").Append(j.ToString());
                                CellsStream.AppendLine("\\cell}");
                                Obj.Counter = y + 1;
                            }
                        }
                    }
                }
                if (CellsStream.Length > 0)
                {
                    WriteLine(stream, "\\trowd");
                    WriteLine(stream, buff.ToString());
                    WriteLine(stream, "\\pard\\intbl");
                    WriteLine(stream, CellsStream.ToString());
                    WriteLine(stream, "\\pard\\intbl{\\trowd");
                    WriteLine(stream, buff.ToString());
                    WriteLine(stream, "\\row}");
                }
            }
            WriteLine(stream, "\\pard\\fs2\\par}"); //insert empty text with minimum size for avoiding creating a new page
            WriteLine(stream, "}");

#region Write a header           
            byte[] b = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(b, 0, (int)stream.Length);
            stream.SetLength(0);
            WriteLine(stream, GetRTFHeader());
            WriteLine(stream, GetRTFMetaInfo());
            stream.Write(b, 0, b.Length);
#endregion

            stream.Flush();
        }

#endregion

#region Protected Methods
        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            return new MyRes("FileFilters").Get("RtfFile");
        }

        /// <inheritdoc/>
        protected override void Start()
        {
            base.Start();

            colorTable = new List<string>();
            fontTable = new List<string>();
            matrix = new ExportMatrix();
            if (wysiwyg)
                matrix.Inaccuracy = 0.5f;
            else
                matrix.Inaccuracy = 10;
            matrix.RotatedAsImage = false;
            matrix.PlainRich = false;
            matrix.AreaFill = true;
            matrix.CropAreaFill = true;
            matrix.ShowProgress = ShowProgress;
            matrix.Report = Report;
            if (imageFormat != RTFImageFormat.Metafile)
            {
                matrix.FullTrust = false;
                matrix.ImageFormat = imageFormat == RTFImageFormat.Jpeg ?
                  System.Drawing.Imaging.ImageFormat.Jpeg : System.Drawing.Imaging.ImageFormat.Png;
                matrix.JpegQuality = jpegQuality;
            }
            matrix.KeepRichText = keepRichText;
        }

        /// <inheritdoc/>
        protected override void ExportPageBegin(ReportPage page)
        {
            base.ExportPageBegin(page);
            matrix.AddPageBegin(page);
        }

        /// <inheritdoc/>
        protected override void ExportBand(Base band)
        {
            base.ExportBand(band);
            matrix.AddBand(band, this);
        }

        /// <inheritdoc/>
        protected override void ExportPageEnd(ReportPage page)
        {
            matrix.AddPageEnd(page);
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            matrix.Prepare();
            ExportRTF(Stream);
        }

#endregion

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            base.Serialize(writer);
            writer.WriteBool("Wysiwyg", Wysiwyg);
            writer.WriteBool("PageBreaks", PageBreaks);
            writer.WriteBool("Pictures", Pictures);
            writer.WriteValue("ImageFormat", ImageFormat);
            writer.WriteValue("KeeoRich", EmbedRichObject);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RTFExport"/> class.
        /// </summary>
        public RTFExport()
        {
            dpiFactor = 96f / DrawUtils.ScreenDpi;
            pageBreaks = true;
            wysiwyg = true;
            autoSize = false;
            pictures = true;
            imageFormat = RTFImageFormat.Metafile;
            jpegQuality = 90;
            creator = "FastReport";
            res = new MyRes("Export,Misc");
        }
    }
}