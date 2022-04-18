using System;
using System.Collections.Generic;

namespace FastReport.RichTextParser
{

    /// <summary>
    /// This class represents a RTF text paragraph.
    /// </summary>
    internal class RTF_Paragraph : RTF_CommonRichElement
    {
        private Paragraph paragraph;

        public List<Run> Runs { get { return paragraph.runs; } }
        public Paragraph Paragraph { get { return paragraph; } }
        internal override RichObject RichObject
        {
            get
            {
                RichObject rich = new RichObject();
                rich.type = RichObject.Type.Paragraph;
                rich.pargraph = paragraph;
                rich.size = paragraph.size;
                return rich;
            }
        }

        public void Fix(RTF_Parser parser)
        {
            paragraph.format = parser.current_paragraph_format;
        }

        internal void AddString(RTF_Parser parser, string text)
        {
            Run run = new Run(text, parser.current_run_format);
            if (parser.ListItem)
            {
                if(parser.current_paragraph_format.list_id == null)
                    parser.current_paragraph_format.list_id = new List<Run>();
                parser.current_paragraph_format.list_id.Add(run);
            }
            else
                Runs.Add(run);
            paragraph.size += text.Length;
        }

        internal override bool Parse(RTF_Parser parser, RTF_Header header)
        {
            bool status = true;
            Style style;

            if (parser.Text.Length != 0)
            {
                AddString(parser, parser.Text);
            }

            switch (parser.Control)
            {
                case "pard":
                    parser.ResetParagraphFormat();
                    parser.insideTable = false;
                    break;
                case "intbl":
                    //System.Diagnostics.Trace.WriteLine(@"\intbl");
                    parser.insideTable = true;
                    break;
                case "tab":
                    AddString(parser, "\t");
                    break;
                case "line":
                    AddString(parser, "\r");
                    break;
                case "emdash":
                    AddString(parser, "—");
                    break;
                case "endash":
                    AddString(parser, "–");
                    break;
                case "s":
                    parser.ListItem = false; // Disable list
                    style = header.FindStyle((int)parser.Number);
                    parser.current_paragraph_format = style.paragraph_style;
                    parser.current_run_format = style.run_style;
                    break;
                case "widctlpar":
                    // Tell to the resterizer do not break first and last lines
                    break;
                default:
                    status = RTF_DocumentParser.ParseParagraphFormat(parser);
                    if (status == true)
                        break;
                    status = RTF_DocumentParser.ParseRunFormat(parser, header);
                    ////if (status == true)
                    ////    break;
                    ////if(parser.Status == ParserStatus.CloseBlock)
                    ////{
                    ////    return true;
                    ////}
                    break;
            }
            return status;
        }

        internal RTF_Paragraph(RTF_Parser parser)
        {
            paragraph = new Paragraph();
            paragraph.runs = new List<Run>();
            paragraph.size = 1;

            paragraph.format = parser.current_paragraph_format;
        }
    }
}
