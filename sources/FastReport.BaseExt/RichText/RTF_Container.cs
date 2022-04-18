using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace FastReport.RichTextParser
{
    internal static class ExportUtils
    {
        public static void Write(Stream stream, string value)
        {
            byte[] buf = Encoding.UTF8.GetBytes(value);
            stream.Write(buf, 0, buf.Length);
        }

        public static void WriteLn(Stream stream, string value)
        {
            Write(stream, value);
            stream.WriteByte(13);
            stream.WriteByte(10);
        }

    }

    /// <summary>
    /// This class represents a RTF run.
    /// </summary>
    internal class RTF_Run
    {
        private readonly Run run;

        internal RTF_Run(RunFormat f, string text)
        {
            run = new Run(text, f);
        }
    }

    internal abstract class RTF_CommonRichElement
    {
        static readonly int DpiX = 96;

        protected static int Twips2Pixels(int twips)
        {
            return (int)(((double)twips) * (1.0 / 1440.0) * DpiX);
        }

        protected static int Twips2Pixels(long twips)
        {
            return (int)(((double)twips) * (1.0 / 1440.0) * DpiX);
        }

        internal abstract RichObject RichObject { get; }

        internal abstract bool Parse(RTF_Parser parser, RTF_Header header);
    }

    internal class RTF_SequenceParser
    {
        private RichObjectSequence sequence;

        private RTF_Picture picture_parser = null;
        private RTF_Paragraph paragraph_parser = null;
        private RTF_Column curr_column = new RTF_Column();
        private RTF_Row current_row = null;

        private List<RichObjectSequence> cells_queue;
        RichObjectSequence cell_sequence;
        private Table table;
        private bool new_table = false;

        public RichObjectSequence Sequence { get { return sequence; } }
        public Table CurrentTable { get { return table; } }

        internal RTF_SequenceParser()
        {
            sequence.objects = new List<RichObject>();
            cells_queue = new List<RichObjectSequence>();
            cell_sequence.objects = new List<RichObject>();
        }
        /// <summary>
        /// Insert paragraph into list of paragraphs
        /// </summary>
        private void InsertParagraph(RTF_Parser parser)
        {
            paragraph_parser.Fix(parser);

            if (!parser.insideTable)
            {
                sequence.objects.Add(paragraph_parser.RichObject);
                sequence.size += paragraph_parser.RichObject.size;
            }
            else
            {
                cell_sequence.objects.Add(paragraph_parser.RichObject);
            }
            paragraph_parser = new RTF_Paragraph(parser);
        }

        private void InsertCell(RTF_Parser parser)
        {
            Fix(parser);
            cells_queue.Add(cell_sequence);
            cell_sequence = new RichObjectSequence();
            cell_sequence.objects = new List<RichObject>();
            paragraph_parser = new RTF_Paragraph(parser);
        }

        /// <summary>
        /// Insert row into list of paragraphs
        /// </summary>
        private void InsertRow()
        {
            // Move parsed cells to current row
            foreach (RichObjectSequence sequence in cells_queue)
            {
                current_row.AddCell(sequence);
            }
            cells_queue = new List<RichObjectSequence>();

            table.rows.Add(current_row.Row);
            current_row = new RTF_Row(this);
            if (new_table)
            {
                InsertTable();
                new_table = false;
            }
        }

        private void InsertTable()
        {
            RichObject rich = new RichObject();
            rich.type = RichObject.Type.Table;
            rich.table = table;
            sequence.objects.Add(rich);
            sequence.size += rich.size;
        }

        private void CreateTable()
        {
            new_table = true;
            table.columns = new List<Column>();
            table.rows = new List<TableRow>();
            current_row = new RTF_Row(this);
        }

        internal void Fix(RTF_Parser parser)
        {
            if (paragraph_parser != null)
            {
                paragraph_parser.Fix(parser);

                if (parser.insideTable)
                    cell_sequence.objects.Add(paragraph_parser.RichObject);
                /* Following code adds empty paragraph at end of page, so disable it now */
                else if (paragraph_parser.RichObject.type == RichObject.Type.Paragraph)
                {
                    if (paragraph_parser.RichObject.pargraph.runs.Count != 0)
                    {
                        sequence.objects.Add(paragraph_parser.RichObject);
                        sequence.size += paragraph_parser.RichObject.size;
                    }
                }

            }

            if (new_table)
            {
                InsertTable();
                new_table = false;
            }
            return;
        }


        internal bool Parse(RTF_Parser parser, RTF_Header header)
        {
            bool parsed = true;

            if (picture_parser != null)
            {
                parsed = picture_parser.Parse(parser, header);
                if (parsed)
                    return true;
                // 20210211: check if picture within paragraph
                if(paragraph_parser.Runs.Count > 0)
                {
                    sequence.objects.Add(paragraph_parser.RichObject);
                    paragraph_parser = new RTF_Paragraph(parser);
                }

                if (parser.insideTable)
                    cell_sequence.objects.Add(picture_parser.RichObject);
                else
                {
                    sequence.objects.Add(picture_parser.RichObject);
                    sequence.size += picture_parser.RichObject.size;
                }
                picture_parser = null;
                return true;
            }

            if (paragraph_parser == null)
                paragraph_parser = new RTF_Paragraph(parser);

            parsed = paragraph_parser.Parse(parser, header);
            if (parsed)
                return true;

            parsed = curr_column.Parse(parser, header);
            if (parsed)
                return true;

            if (current_row != null)
            {
                parsed = current_row.Parse(parser, header);
                if (parsed)
                    return true;
            }

            parsed = true;
            switch (parser.Control)
            {
                case "par":
                    InsertParagraph(parser);
                    parser.ListItem = false; // Disable list
                    break;
                case "cellx":
                    uint w = (uint)parser.Number;
                    curr_column.SetWidth(w);
                    table.columns.Add(curr_column.Column);
                    curr_column = new RTF_Column();
                    break;
                case "cell":
                    InsertCell(parser);
                    break;
                case "row":
                    InsertRow();
                    break;
                case "trowd":
                    CreateTable();
                    break;
                case "pict":
                    picture_parser = new RTF_Picture();
                    break;
                default:
                    ////if(parser.Status == ParserStatus.CloseBlock)
                    ////{
                    ////    InsertParagraph(parser);
                    ////    break;
                    ////}
                    parsed = false;
                    break;
            }
            return parsed;
        }
    }

    /// <summary>
    /// This class represents a RTF properies.
    /// </summary>
    class RTF_PageParser
    {
        private Page page;

        RichDocument document;
        RTF_SequenceParser sequence_parser = new RTF_SequenceParser();

        private RTF_SequenceParser page_header = null;
        private RTF_SequenceParser page_footer = null;

        public Page Page { get { return page; } }

        public RTF_PageParser(bool soft, RichDocument document)
        {
            page.soft_break = soft;
            page.margin_top = 0;
            page.margin_left = 0;
            page.margin_right = 0;
            page.margin_bottom = 0;
            page.sequence.objects = new List<RichObject>();

            this.document = document;
            
        }

        internal void Fix(RTF_Parser parser)
        {
            sequence_parser.Fix(parser);
            page.sequence = sequence_parser.Sequence;
            page.size = page.sequence.size;
        }

        static int indirection_count = 0;

        internal bool Parse(RTF_Parser parser, RTF_Header header)
        {
            bool parsed = false;

            if (page_header != null)
            {
                parsed = page_header.Parse(parser, header);
                if (!parsed)
                {
                    //this.page_header.AddString(parser, "\\" + parser.Control);
                }
                if (parser.Status == ParserStatus.CloseBlock)
                {
                    indirection_count--;
                    if (indirection_count < 0)
                    {
                        indirection_count = 0;
                        if (Page.header.objects == null)
                            this.page.header.objects = new List<RichObject>();
                        foreach (RichObject o in page_header.Sequence.objects)
                        {
                            this.Page.header.objects.Add(o);
                        }
                        page_header = null;
                        return false;
                    }
                }
                else if (parser.Status == ParserStatus.OpenBlock)
                {
                    indirection_count++;
                }
                return true;
            }

            if (page_footer != null)
            {
                parsed = page_footer.Parse(parser, header);
                if (!parsed)
                {
                    //this.page_footer.AddString(parser, "\\" + parser.Control);
                }
                if (parser.Status == ParserStatus.CloseBlock)
                {
                    indirection_count--;
                    if (indirection_count < 0)
                    {
                        indirection_count = 0;
                        if (Page.footer.objects == null)
                            this.page.footer.objects = new List<RichObject>();
                        foreach (RichObject o in page_footer.Sequence.objects)
                        {
                            this.Page.footer.objects.Add(o);
                        }
                        page_footer = null;
                        return false;
                    }
                }
                else if (parser.Status == ParserStatus.OpenBlock)
                {
                    indirection_count++;
                }
                return true;
            }

            parsed = sequence_parser.Parse(parser, header);

            if (!parsed)
            {
                parsed = true;
                switch (parser.Control)
                {
                    case "pgwsxn":
                        page.page_width = parser.Number;
                        break;
                    case "pghsxn":
                        page.page_heigh = parser.Number;
                        break;
                    case "marglsxn":
                        page.margin_left = parser.Number;
                        break;
                    case "margrsxn":
                        page.margin_right = parser.Number;
                        break;
                    case "margtsxn":
                        page.margin_top = parser.Number;
                        break;
                    case "margbsxn":
                        page.margin_bottom = parser.Number;
                        break;
                    case "headerr":
                    case "header":
                        page_header = new RTF_SequenceParser();
                        break;
                    case "footer":
                    case "footerr":
                        page_footer = new RTF_SequenceParser();
                        break;

                    default:
                        parsed = false;
                        break;
                }
            }
            return parsed;
        }
    }

    class RTF_BorderLine_Parser
    {
        internal BorderLine line;

        internal void Clear()
        {
            line.style = BorderLine.Style.Thin;
            line.width = 0;
            line.color = System.Drawing.Color.Black;
        }

        internal bool Parse(RTF_Parser parser, RTF_Header header)
        {
            bool parsed = true;
            switch (parser.Control)
            {
                case "brdrs":
                    line.style = BorderLine.Style.Thin;
                    break;
                case "brdrth":
                    line.style = BorderLine.Style.Thick;
                    break;
                case "brdrdb":
                    line.style = BorderLine.Style.Double;
                    break;
                case "brdrdot":
                    line.style = BorderLine.Style.Dotted;
                    break;
                case "brdrw":
                    line.width = (uint)parser.Number;
                    break;
                case "brdrcf":
                    {
                        int cidx = (int)parser.Number;
                        line.color = header.Document.color_list[cidx];
                        break;
                    }
                default:
                    parsed = false;
                    break;
            }
            return parsed;
        }
    }

    /// <summary>
    /// This class parses an entiry RTF document.
    /// </summary>
    public class RTF_DocumentParser : IDisposable
    {
        private RichDocument doc;

        int nested_block_count;
        bool skip_rtf_extension;

        enum GlobalMode { Header, Document }

        private RTF_PageParser curr_page;
        private RTF_Header header_parser;

        private Stack<RunFormat> run_formats_stack;
        private Stack<ParagraphFormat> parahraph_format_stack; // Do we need keep track of paragraphs format?

        public RichDocument Document { get { return doc; } }

#if false
        Dictionary<RTF_RunFormat, RTF_RunFormat> format_hash;

        internal RTF_RunFormat FindFormat(RTF_RunFormat key)
        {
            if (!format_hash.ContainsKey(key))
                return null;
            return format_hash[key];
        }
#endif

        internal static bool ParseParagraphFormat(RTF_Parser parser)
        {
            bool status = true;
            switch (parser.Control)
            {
                case "clvertalt":
                    parser.current_paragraph_format.Valign = ParagraphFormat.VerticalAlign.Top;
                    break;
                case "clvertalc":
                    parser.current_paragraph_format.Valign = ParagraphFormat.VerticalAlign.Center;
                    break;
                case "clvertalb":
                    parser.current_paragraph_format.Valign = ParagraphFormat.VerticalAlign.Bottom;
                    break;
                default:
                    parser.current_paragraph_format.Valign = ParagraphFormat.VerticalAlign.Center;
                    break;
            }
            switch (parser.Control)
            {
                case "qc":
                    parser.current_paragraph_format.align = ParagraphFormat.HorizontalAlign.Centered;
                    break;
                case "ql":
                    parser.current_paragraph_format.align = ParagraphFormat.HorizontalAlign.Left;
                    break;
                case "qr":
                    parser.current_paragraph_format.align = ParagraphFormat.HorizontalAlign.Right;
                    break;
                case "qj":
                    parser.current_paragraph_format.align = ParagraphFormat.HorizontalAlign.Justified;
                    break;
                case "qd":
                    parser.current_paragraph_format.align = ParagraphFormat.HorizontalAlign.Distributed;
                    break;
                case "qk":
                    parser.current_paragraph_format.align = ParagraphFormat.HorizontalAlign.Kashida;
                    break;
                case "qt":
                    parser.current_paragraph_format.align = ParagraphFormat.HorizontalAlign.Thai;
                    break;
                case "sl":
                    parser.current_paragraph_format.line_spacing = (int)parser.Number;
                    break;
                case "sb":
                    parser.current_paragraph_format.space_before = (int)parser.Number;
                    break;
                case "sa":
                    parser.current_paragraph_format.space_after = (int)parser.Number;
                    break;
                case "li":
                    parser.current_paragraph_format.left_indent = (int)parser.Number;
                    break;
                case "ri":
                    parser.current_paragraph_format.right_indent = (int)parser.Number;
                    break;
                case "fi":
                    parser.current_paragraph_format.first_line_indent = (int)parser.Number;
                    break;
                case "slmult":
                    parser.current_paragraph_format.lnspcmult = (ParagraphFormat.LnSpcMult)parser.Number;
                    break;
                case "pntext":
                    parser.ResetRunFormat();
                    parser.current_paragraph_format.list_id = new List<Run>();
                    parser.current_paragraph_format.pnstart = 1; // No support of nested numbering in this version
                    parser.ListItem = true;
                    break;
                case "ltrpar":
                    parser.current_paragraph_format.text_direction = ParagraphFormat.Direction.LeftToRight;
                    break;
                case "rtlpar":
                    parser.current_paragraph_format.text_direction = ParagraphFormat.Direction.RighgToLeft;
                    break;
                case "tx":
                    if (parser.current_paragraph_format.tab_positions == null)
                        parser.current_paragraph_format.tab_positions = new List<int>();
                    parser.current_paragraph_format.tab_positions.Add((int)parser.Number);
                    break;
                default:
                    status = false;
                    break;
            }
            return status;
        }

        internal static bool ParseRunFormat(RTF_Parser parser, RTF_Header header)
        {
            bool accepted = true;
            int cidx;
            switch (parser.Control)
            {
                case "b":
                    parser.current_run_format.bold = parser.HasValue ? (parser.Number == 0 ? false : true) : true;
                    break;
                case "i":
                    parser.current_run_format.italic = parser.HasValue ? (parser.Number == 0 ? false : true) : true;
                    break;
                case "cf":
                    cidx = (int) parser.Number;
                    if(cidx > 0)
                    {
                        if (cidx > header.Document.color_list.Count - 1)
                            cidx = header.Document.color_list.Count - 1;
                    }
                    parser.current_run_format.color = header.Document.color_list[cidx];
                    break;
                case "highlight":
                    cidx = (int)parser.Number;
                    parser.current_run_format.BColor = header.Document.color_list[cidx];
                    break;
                case "cbpat":
                    cidx = (int)parser.Number;
                    parser.current_run_format.FillColor = header.Document.color_list[cidx];
                    break;
                case "ul":
                    parser.current_run_format.underline = parser.HasValue ? (parser.Number == 0 ? false : true) : true;
                    break;
                case "f":
                    uint idx = header.GetFontID(parser.Number);
                    parser.current_run_format.font_idx = idx;
                    RFont rf = header.Document.font_list[(int)idx];
                    parser.font_charset = rf.charset;
                    if(rf.charset != 0)
                    {
                        parser.SelectCodepageByFontCharset(rf.charset);
                    }
                    break;
                case "fs":
                    parser.current_run_format.font_size = (int)parser.Number;
                    break;
                case "ulnone":
                    parser.current_run_format.underline = false;
                    break;
                case "plain":
                    parser.ResetRunFormat();
                    break;
                case "up":
                    if (parser.Number == 0)
                        parser.current_run_format.script_type = RunFormat.ScriptType.PlainText;
                    else
                        parser.current_run_format.script_type = RunFormat.ScriptType.Superscript;
                    break;
                case "dn":
                    if (parser.Number == 0)
                        parser.current_run_format.script_type = RunFormat.ScriptType.PlainText;
                    else
                        parser.current_run_format.script_type = RunFormat.ScriptType.Subscript;
                    break;
                case "super":
                    parser.current_run_format.script_type = RunFormat.ScriptType.Superscript;
                    break;
                case "sub":
                    parser.current_run_format.script_type = RunFormat.ScriptType.Subscript;
                    break;
                default:
                    accepted = false;
                    break;
            }
            return accepted;
        }

        /// <inheritdoc/>
        public void Load(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
                Load(stream);
        }

        /// <inheritdoc/>
        public Color GetFillColor() 
        {
            Color color = Color.White;
            foreach(RichObject obj in curr_page.Page.sequence.objects)
            {
                if (obj.type == RichObject.Type.Paragraph )
                {
                    if(obj.pargraph.runs.Count > 0)
                    {
                        color = obj.pargraph.runs[0].format.FillColor;
                        break;
                    }
                }
            }
            return color;
        }

        /// <inheritdoc/>
        public void Load(string rich_text)
        {
//            System.Diagnostics.Trace.WriteLine(rich_text);
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(rich_text);
            writer.Flush();
            stream.Position = 0;
            Load(stream);
        }

        /// <inheritdoc/>
        public void Load(Stream stream)
        {
            GlobalMode mode;
            ParserStatus status = ParserStatus.Collecting;
            int ch;
            RTF_Parser parser = new RTF_Parser();

            int block_sychro = 0;
            header_parser = new RTF_Header(doc);
            mode = GlobalMode.Header;

            while (true)    
            {
                ch = stream.ReadByte();
                if (ch == -1)
                {
                    if (status == ParserStatus.Collecting && parser.EndOfFile)
                    {
                        header_parser.Header = false;
                        doc = header_parser.Document;
                        mode = GlobalMode.Document;
                        Parse(parser);
                    }
                    break;
                }
                status = parser.ParseByte((char)ch);
                if (status == ParserStatus.Collecting)
                    continue;
                if (status == ParserStatus.OpenBlock)
                {
                    run_formats_stack.Push(parser.current_run_format);
                    ++block_sychro;
                }

                //if(parser.Control == "tx")
                //{
                //    System.Diagnostics.Trace.Write("tx");
                //}

                switch (mode)
                {
                    case GlobalMode.Header:
                        if (header_parser.Parse(parser) == true)
                            break;
                        doc = header_parser.Document;
                        mode = GlobalMode.Document;
                        Parse(parser);
                        break;

                    case GlobalMode.Document:
                        Parse(parser);
                        break;
                }
                if (status == ParserStatus.CloseBlock)
                {
                    --block_sychro;
                    parser.ListItem = false; // Disable list
                    if (block_sychro < 0)
                        throw new Exception("Document structure error");
                    parser.current_run_format = run_formats_stack.Pop();
                    if (block_sychro == 0)
                    {
                        if (mode == GlobalMode.Header)
                            Parse(parser);
                        break;
                    }
                }
            }
            AddPage(parser);
        }

        internal void Parse(RTF_Parser parser)
        {
            bool parsed = false;

#if false // debug
      if (parser.Status == ParserStatus.OpenBlock)
      {
        string dbg = String.Format("{{{0}", formats_stack.Count);
        System.Diagnostics.Trace.Write(dbg);
      }
      else if (parser.Status == ParserStatus.CloseBlock)
      {
        System.Diagnostics.Trace.Write(@"}");
      }

      //if (parser.Control == "cell")
      //  System.Diagnostics.Trace.WriteLine(@"Cell is not parsed yet");
    
#endif

            if (skip_rtf_extension)
            {
                switch (parser.Control)
                {
                    case "fldinst":
                        break;
                }
                if (parser.Status == ParserStatus.OpenBlock)
                    ++nested_block_count;
                else if (parser.Status == ParserStatus.CloseBlock)
                {
                    if (nested_block_count == 0)
                        throw new Exception("Document structure error");
                    --nested_block_count;
                    if (nested_block_count == 0)
                        skip_rtf_extension = false;
                }
                return;
            }

            parsed = curr_page.Parse(parser, this.header_parser);

            if (!parsed)
                switch (parser.Control)
                {
                    case "page":
                        AddPage(parser);
                        curr_page = new RTF_PageParser(false, doc);
                        break;
                    case "softpage":
                        doc.pages.Add(curr_page.Page);
                        curr_page = new RTF_PageParser(true, doc);
                        break;
                    case "paperw":
                        doc.paper_width = parser.Number;
                        break;
                    case "paperh":
                        doc.paper_height = parser.Number;
                        break;
                    case "margl":
                        doc.global_margin_left = parser.Number;
                        break;
                    case "margt":
                        doc.global_margin_top = parser.Number;
                        break;
                    case "margr":
                        doc.global_margin_right = parser.Number;
                        break;
                    case "margb":
                        doc.global_margin_bottom = parser.Number;
                        break;
                    case "deftab":
                        doc.default_tab_width = parser.Number;
                        break;
                    case "viewkind":
                        doc.view_kind = parser.Number;
                        break;

                    case "shp":
                        //// Shape must be parsed in another place
                        //skip_rtf_extension = true;
                        break;

                    case "":
                        if (parser.Delimiter == '*')
                        {
                            // Preivous version which ignore pictures in \*\shppict tag (20210211)
                            // Just ignore delimiter
                        }
                        break;
                    default:
                        ;
                        break;
                }
        }

        void AddPage(RTF_Parser parser)
        {
            curr_page.Fix(parser);
            Page pg = curr_page.Page;
            doc.pages.Add(pg);
            long sz = pg.sequence.size;
            doc.size += sz;
        }

        public void Dispose()
        {
            header_parser = null;
        }

        /// <summary>
        /// Get RTF structure based on range of elements
        /// </summary>
        public RichDocument GetRange(int Start, int Length)
        {
            RichDocument ranged_doc = new RichDocument();

            long position = 0;
            long finish = Start + Length;
            ranged_doc.pages = new List<Page>();
            foreach (Page page in doc.pages)
            {
                if (Start > position)
                {
                    position += page.size;
                    if (Start > position)
                        continue;
                    position -= page.size;
                }

                Page ranged_page = new Page();
                ranged_page.sequence.objects = new List<RichObject>();
                foreach (RichObject sequence in page.sequence.objects)
                {
                    position += sequence.size;
                    if (Start > position)
                        continue;
                    position -= sequence.size;

                    RichObject ranged_object = new RichObject();
                    switch (sequence.type)
                    {
                        case RichObject.Type.Paragraph:
                            {
                                ranged_object.type = RichObject.Type.Paragraph;
                                ranged_object.pargraph = new Paragraph();
                                ranged_object.pargraph.runs = new List<Run>();

                                Paragraph par = sequence.pargraph;
                                ranged_object.pargraph.format = par.format;
                                foreach (Run run in par.runs)
                                {
                                    position += run.text.Length;
                                    if (Start > position)
                                        continue;
                                    position -= run.text.Length;
                                    // Here is it
                                    string run_text;
                                    if (Start == position)
                                        run_text = run.text;
                                    else
                                    {
                                        int diff = (int)(Start - position);
                                        run_text = run.text.Substring(diff);
                                    }
                                    if (Length < run_text.Length)
                                        run_text = run_text.Substring(0, Length);

                                    Run ranged_run = new Run(run_text, run.format);
                                    ranged_object.pargraph.runs.Add(ranged_run);

                                    Length -= run_text.Length;
                                    if (Length == 0)
                                        break;
                                }
                            }
                            break;
                        case RichObject.Type.Table:
                            {
                                position += sequence.size;
                                if (Start > position)
                                    continue;
                                position -= sequence.size;
                                // TODO: split table
                                if (Length < sequence.size)
                                    Length = 0; // and this too
                            }
                            break;
                        case RichObject.Type.Picture:
                            {
                                position += sequence.size;
                                if (Start > position)
                                    continue;
                                position -= sequence.size;
                                // TODO: split picture?
                                if (Length < sequence.size)
                                    Length = 0; // and this too
                            }
                            break;
                    }

                    ranged_page.sequence.objects.Add(ranged_object);

                    if (Length == 0)
                        break;
                    if (Length < 0)
                        throw new Exception("Negative length in RTF_DocumentParser::GetRange()");
                }

                ranged_doc.pages.Add(ranged_page);

                if (Length == 0)
                    break;
                if (Length < 0)
                    throw new Exception("Negative length in RTF_DocumentParser::GetRange()");

                ranged_page = new Page();
                ranged_page.sequence.objects = new List<RichObject>();
            }

            ranged_doc.font_list = doc.font_list;
            ranged_doc.color_list = doc.color_list;
            ranged_doc.style_list = doc.style_list;

            ranged_doc.codepage = doc.codepage;
            ranged_doc.default_font = doc.default_font;
            ranged_doc.default_lang = doc.default_lang;
            ranged_doc.paper_width = doc.paper_width;
            ranged_doc.paper_height = doc.paper_height;
            ranged_doc.global_margin_left = doc.global_margin_left;
            ranged_doc.global_margin_top = doc.global_margin_top;
            ranged_doc.global_margin_right = doc.global_margin_right;
            ranged_doc.global_margin_bottom = doc.global_margin_bottom;
            ranged_doc.default_tab_width = doc.default_tab_width;
            ranged_doc.view_kind = doc.view_kind;

            return ranged_doc;
        }

        /// RichText document object
        /// </summary>
        public RTF_DocumentParser()
        {
            doc.paper_width = 12240;
            doc.paper_height = 15840;
            doc.global_margin_left = 1800;
            doc.global_margin_top = 1440;
            doc.global_margin_right = 1800;
            doc.global_margin_bottom = 1440;
            doc.default_tab_width = 720;
            doc.pages = new List<Page>();
            curr_page = new RTF_PageParser(false, doc);
            skip_rtf_extension = false;

            run_formats_stack = new Stack<RunFormat>();
            parahraph_format_stack = new Stack<ParagraphFormat>(); // Do we need keep track of paragraphs format?
        }
    }
}
