using System.Drawing;
using System.Text;
using FastReport;
using FastReport.RichTextParser;

/*
 *  This code translates an internal representation of 
 *  rich paragraph to HTML text
 * 
 */

namespace FastReport.Design.ImportPlugins.RTF
{
    internal partial class ImportRtf
    {
        private string GenerateHTMLText(TextObject memo, Paragraph paragraph)
        {
            memo.RightToLeft = paragraph.format.text_direction == FastReport.RichTextParser.ParagraphFormat.Direction.RighgToLeft;
            //memo.
            int run_num = 0;
            bool span_condition = false;
            StringBuilder sb = new StringBuilder();
            RunFormat format;

            string colorname = string.Empty;
            string fontname = string.Empty;
            string fontsize = string.Empty;
            Font current_font = memo.Font;

            int len; 
            foreach (Run run in paragraph.runs)
            {
                switch(run.text)
                {
                    case "\r":
                        len = 1;
                        break;
                    case "\t":

                        break;
                    default:
                        len = run.text.Length;
                        break;
                }
                len = run.text != "\r" ? run.text.Length : 1;
                format = run.format;
                if (run_num == 0)
                {
                    current_format = run.format;
                    memo.Font = GetFontFromRichStyle(rtf, current_format);
                    memo.TextColor = current_format.color;

                    if (format.underline)
                    {
                        sb.Append("<u>");
                        current_format.underline = true;
                    }
                    if (format.bold)
                    {
                        sb.Append("<b>");
                        current_format.bold = true;
                    }
                    if (format.italic)
                    {
                        sb.Append("<i>");
                        current_format.italic = true;
                    }
                }
                else
                {
                    if (current_format.bold != format.bold)
                    {
                        sb.Append(format.bold ? "<b>" : "</b>");
                        current_format.bold = format.bold;
                    }

                    if (current_format.italic != format.italic)
                    {
                        sb.Append(format.italic ? "<i>" : "</i>");
                        current_format.italic = format.italic;
                    }

                    if (current_format.underline != format.underline)
                    {
                        sb.Append(format.underline ? "<u>" : "</u>");
                        current_format.underline = format.underline;
                    }

                    if (current_format.script_type != format.script_type)
                    {
                        if (format.script_type == RunFormat.ScriptType.Subscript)
                            sb.Append("<sub>");
                        else if (format.script_type == RunFormat.ScriptType.Superscript)
                            sb.Append("<sup>");
                        else if (current_format.script_type == RunFormat.ScriptType.Subscript)
                            sb.Append("</sub>");
                        else if (current_format.script_type == RunFormat.ScriptType.Superscript)
                            sb.Append("</sup>");
                        current_format.script_type = format.script_type;
                    }

                    if (current_format.color != format.color)
                    {
                        colorname = string.Format("color:#{0:X2}{1:X2}{2:X2};", format.color.R, format.color.G, format.color.B);
                        current_format.color = format.color;
                    }

                    if (current_format.font_size != format.font_size)
                    {
                        int fs = run.format.font_size / 2;
                        fontsize = string.Format("font-size:{0}pt;", fs);
                        current_format.font_size = format.font_size;
                    }

                    Font fnt = GetFontFromRichStyle(rtf, format);
                    if (!current_font.FontFamily.Equals(fnt.FontFamily))
                    {
                        current_font = fnt;
                        fontname = string.Format("font-family:{0};", fnt.FontFamily.Name);
                    }
                    else
                        fontname = string.Empty;

                    if (colorname.Length > 0 || fontsize.Length > 0 || fontname.Length > 0)
                    {
                        sb.Append("<span style=\"");
                        if (colorname.Length > 0)
                            sb.Append(colorname);
                        if (fontsize.Length > 0)
                            sb.Append(fontsize);
                        if (fontname.Length > 0)
                            sb.Append(fontname);
                        sb.Append("\">");
                        span_condition = true;
                    }
                }

                switch (run.text)
                {
                    case "\r":
                        sb.Append("<br>");
                        break;

                    case "\t":
                        sb.Append("\t");
                        break;

                    default:
                        sb.Append(run.text);
                        break;
                }

                if (span_condition)
                {
                    sb.Append("</span>");
                    span_condition = false;
                }

                run_num++;
            }
            return sb.ToString();
        }
    }
}
