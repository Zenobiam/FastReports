using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace FastReport.RichTextParser
{

    /// <summary>
    /// This class represents a RTF document header.
    /// </summary>
    /// <remarks>
    /// Refer to RTF-1.7 spec for detail description
    /// </remarks>
    class RTF_Header
    {
        private RichDocument doc;

        private enum HeaderParserState
        {
            Starting,
            Signature,
            GlobalProperties,
            SubitemFirstLevel,
            FontTable,
            ColorTable,
            StyleSheet,
            DocInfoSection,
            SkipFormatExtension
        }
        private enum StyleState { Wait, Parse };

        private bool header_active;
        private int recursion_counter;
        private HeaderParserState tag_state;
        private StyleState style_state;
        private Dictionary<long, uint> font_ids;

        public RichDocument Document { get { return doc; } }

        public RTF_Header(RichDocument doc)
        {
            this.doc = doc;
            this.doc.font_list = new List<RFont>();
            this.doc.color_list = new List<Color>();
            this.doc.color_list.Add(Color.Black);
            this.doc.style_list = new List<Style>();
            font_ids = new Dictionary<long, uint>();
            tag_state = HeaderParserState.Starting;
            style_state = StyleState.Wait;
        }

        internal bool Header
        {
            get { return header_active; }
            set
            {
                if(value == false)
                {
                    if (doc.color_list.Count == 0)
                        doc.color_list.Add(Color.Black);
                    // Most simple RTF document does not include fonts, so we need add at least one
                    if (doc.font_list.Count == 0)
                    {
                        RFont f = new RFont();
                        f.family = RFont.Family.Rroman;
                        f.FontName = "Arial";
                        doc.font_list.Add(f);
                    }
                }
                header_active = value;
            }
        }

        internal uint GetFontID(long id)
        {
            uint result;
            if (font_ids.ContainsKey(id))
                result = font_ids[id];
            else
                result = 0;
            return result;
        }

        /// <summary>
        /// Parser of RTF header.
        /// </summary>
        /// <remarks>
        /// Return false on finish of header
        /// </remarks>
        public bool Parse(RTF_Parser parser)
        {
            if (parser.Status == ParserStatus.OpenBlock)
                ++recursion_counter;
            if (parser.Status == ParserStatus.CloseBlock)
            {
                if (recursion_counter == 0)
                    return false;
                --recursion_counter;
            }

            switch (tag_state)
            {
                case HeaderParserState.Starting:
                    if (parser.Status != ParserStatus.OpenBlock || parser.Text.Length != 0 || parser.Control.Length != 0)
                        throw new DecoderFallbackException("Not a RichText format");
                    tag_state = HeaderParserState.Signature;
                    Header = true;
                    break;
                case HeaderParserState.Signature:
                    if (parser.Control != "rtf")
                        throw new Exception("Document format error");
                    if (parser.Number > 1)
                        throw new Exception("Unsupported RTF format version");
                    tag_state = HeaderParserState.GlobalProperties;
                    break;

                case HeaderParserState.GlobalProperties:
                    switch (parser.Control)
                    {
                        case "ansi":
                            // Do nothing?
                            break;
                        case "ansicpg":
                            doc.codepage = parser.Number;
                            break;
                        case "deff":
                            doc.default_font = parser.Number;
                            break;
                        case "deflang":
                            doc.default_lang = parser.Number;
                            break;
                        case "pard":
                        case "cf":
                        case "fs":
                            Header = false;
                            break;
                    }
                    if (parser.Status == ParserStatus.OpenBlock)
                        tag_state = HeaderParserState.SubitemFirstLevel;
                    else if (parser.Status == ParserStatus.CloseBlock)
                        Header = false;
                    break;

                case HeaderParserState.SubitemFirstLevel:
                    {
                        switch (parser.Control)
                        {
                            case "fonttbl":
                                if(parser.Status == ParserStatus.CloseBlock)
                                {
                                    // Empty font table
                                    break;
                                }
                                tag_state = HeaderParserState.FontTable;
                                font_state = FontInfoState.FontID;
                                doc.font_list.Clear(); // Delete default font
                                break;
                            case "colortbl":
                                tag_state = HeaderParserState.ColorTable;
                                parser.override_default_color = parser.Delimiter != ' ';
                                break;
                            case "noqfpromote":
                                // Ignore control word for moving info and stylesheet sections into header
                                // how many such commands will be here? Who knows?
                                break;
                            case "info":
                                tag_state = HeaderParserState.DocInfoSection;
                                break;
                            case "stylesheet":
                                if (parser.Status == ParserStatus.CloseBlock)
                                    tag_state = HeaderParserState.SubitemFirstLevel;
                                else
                                {
                                    tag_state = HeaderParserState.StyleSheet;
                                    style = new Style();
                                    parser.ResetParagraphFormat();
                                    parser.ResetRunFormat();
                                    style_state = StyleState.Parse;
                                }
                                break;
                            case "mmathPr":
                                tag_state = HeaderParserState.SkipFormatExtension;
                                break;
                            case "":
                                if (parser.Delimiter == '*')
                                {
                                    tag_state = HeaderParserState.SkipFormatExtension;
                                }
                                break;
                            default:
                                Header = false;
                                break;
                        }
                    }
                    break;

                case HeaderParserState.FontTable:
                    ParseFontTable(parser);
                    break;

                case HeaderParserState.ColorTable:
                    ParseColorTable(parser);
                    break;

                case HeaderParserState.DocInfoSection:
                    if (parser.Status == ParserStatus.CloseBlock)
                    {
                        if (recursion_counter == 1)
                            tag_state = HeaderParserState.SubitemFirstLevel; ;
                    }
                    break;

                case HeaderParserState.StyleSheet:
                    ParserStyleTable(parser);
                    break;

                case HeaderParserState.SkipFormatExtension:
                    if (parser.Status == ParserStatus.CloseBlock)
                    {
                        if (recursion_counter == 1)
                            tag_state = HeaderParserState.SubitemFirstLevel;
                    }
                    break;
            }
            return Header;
        }

        public Style FindStyle(int styledef)
        {
            foreach (Style style in doc.style_list)
            {
                if (style.styledef == styledef)
                    return style;
            }
            return doc.style_list[0];
        }

        Style style;

        private void ParserStyleTable(RTF_Parser parser)
        {
            bool parsed = false;
            switch (style_state)
            {
                case StyleState.Wait:
                    if (parser.Status == ParserStatus.OpenBlock)
                    {
                        style = new Style();
                        parser.ResetParagraphFormat();
                        parser.ResetRunFormat();
                        style_state = StyleState.Parse;
                    }
                    if (parser.Status == ParserStatus.CloseBlock)
                    {
                        if (recursion_counter == 1)
                            tag_state = HeaderParserState.SubitemFirstLevel; ;
                    }
                    break;
                case StyleState.Parse:
                    // Parse style here
                    switch (parser.Control)
                    {
                        case "s":
                            style.styledef = (int)parser.Number;
                            break;
                        case "sbasedon":
                            break;
                        default:
                            parsed = RTF_DocumentParser.ParseParagraphFormat(parser);
                            if (parsed)
                                break;
                            parsed = RTF_DocumentParser.ParseRunFormat(parser, this);
                            break;
                    }

                    if (parser.Status == ParserStatus.CloseBlock)
                    {
                        if (parser.Text.Length != 0)
                        {
                            style.stylename = parser.Text;
                        }
                        // Add new style here
                        style.paragraph_style = parser.current_paragraph_format;
                        style.run_style = parser.current_run_format;
                        style_state = StyleState.Wait;
                        doc.style_list.Add(style);
                        if (recursion_counter == 1)
                            tag_state = HeaderParserState.SubitemFirstLevel;
                        break;
                    }
                    break;
            }
        }

        byte color_red;
        byte color_green;
        byte color_blue;

        void ParseColorTable(RTF_Parser parser)
        {
            if (parser.Status != ParserStatus.CloseBlock)
            {
                if (parser.Number > 255)
                    throw new Exception("Color value out of range");
                byte cl = (byte)parser.Number;
                switch (parser.Control)
                {
                    case "red":
                        color_red = cl;
                        break;
                    case "green":
                        color_green = cl;
                        break;
                    case "blue":
                        color_blue = cl;
                        break;
                }
//              if (parser.Delimiter == ';')
                if (parser.Delimiter != '\\')
                {
                    Color c = Color.FromArgb(color_red, color_green, color_blue);
                    if (parser.override_default_color)
                    {
                        doc.color_list[0] = c;
                    }
                    else
                    {
                        doc.color_list.Add(c);
                    }
                    parser.override_default_color = false;
                }
            }
            else
            {
                if (recursion_counter == 1)
                    tag_state = HeaderParserState.SubitemFirstLevel; ;
            }
        }

        enum FontInfoState
        {
            FontID,
            CheckThemAll,
        }

        FontInfoState font_state;
        RFont font;

        internal bool ParseFontAttributes(RTF_Parser parser)
        {
            bool status = true;

            switch (parser.Control)
            {
                case "fnil":
                    font.family = RFont.Family.Nil;
                    break;
                case "froman":
                    font.family = RFont.Family.Rroman;
                    break;
                case "fswiss":
                    font.family = RFont.Family.Swiss;
                    break;
                case "fmodern":
                    font.family = RFont.Family.Modern;
                    break;
                case "fscript":
                    font.family = RFont.Family.Script;
                    break;
                case "fdecor":
                    font.family = RFont.Family.Decor;
                    break;
                case "ftech":
                    font.family = RFont.Family.Tech;
                    break;
                case "fbidi":
                    font.family = RFont.Family.Bidi;
                    break;
                case "fcharset":
                    font.charset = parser.Number;
                    break;
                default:
                    // Here is many options are skipped off
                    // it is better to collect them in debug mode
                    status = false;
                    break;
            }
            return status;
        }

        void ParseFontTable(RTF_Parser parser)
        {
            // Propagate exit
            if (recursion_counter == 1 && parser.Status == ParserStatus.CloseBlock)
            {
                tag_state = HeaderParserState.SubitemFirstLevel;
                return;
            }

            int back = recursion_counter;
            if (parser.Status == ParserStatus.CloseBlock)
                ++back;
            else if (parser.Status == ParserStatus.OpenBlock)
                --back;

            switch (font_state)
            {
                case FontInfoState.FontID:
                    switch (parser.Control)
                    {
                        case "f":
                            font = new RFont();
                            font.font_id = (uint)doc.font_list.Count;
                            if (!font_ids.ContainsKey(parser.Number))
                                font_ids.Add(parser.Number, font.font_id);
                            else
                                Debug.WriteLine("Duplication of font_id: " + font.font_id.ToString());
                            font_state = FontInfoState.CheckThemAll;
                            break;
                        case "flomajor":
                        case "fhimajor":
                        case "fdbmajor":
                        case "fbimajor":
                        case "flominor":
                        case "fhiminor":
                        case "fdbminor":
                        case "fbiminor":
                            //  We just ignore these extensions of modern versions of MS Word 
                            break;
                        default:
                            if (parser.Status != ParserStatus.OpenBlock)
                                throw new Exception("RTF unknown font tag");
                            break;
                    }
                    break;

                case FontInfoState.CheckThemAll:
                    if (back == 3)
                    {
                        if (parser.Status != ParserStatus.CloseBlock)
                        {
                            ParseFontAttributes(parser);
                        }
                        else
                        {
                            string str = parser.Text;
                            int len = str.Length;
                            if (len > 0)
                            {
                                if (str[len - 1] == ';')
                                    font.FontName = str.TrimEnd(';');
                                doc.font_list.Add(font);
                                font_state = FontInfoState.FontID;
                            }
                            else
                                throw new Exception("RTF malformed font name");
                        }
                    }
                    break;
            }
            return;
        }
    }

}
