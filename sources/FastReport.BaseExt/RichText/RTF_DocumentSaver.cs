using System;
using System.Collections.Generic;
using System.IO;
//using System.Linq;
using System.Text;
using System.Drawing.Imaging;

namespace FastReport.RichTextParser
{
#pragma warning disable 1591
    /// <summary>
    /// Save RTF document to text stream
    /// </summary>
    public class RTF_DocumentSaver : IDisposable
    {
        const int PIC_BUFF_SIZE = 512;
        internal const string XCONV = "0123456789ABCDEF";
        private RichDocument doc;
        private ParagraphFormat.HorizontalAlign prev_align = ParagraphFormat.HorizontalAlign.Left;
        RunFormat current_format;

        public ImageFormat imageFormat;
        public bool DomesticCodepage = false;

        public RTF_DocumentSaver(RichDocument doc)
        {
            this.doc = doc;
            imageFormat = ImageFormat.Png;
        }

        private void SaveFontTable(StringBuilder s)
        {
            if (doc.font_list != null)
            {
                s.Append("{\\fonttbl");
                foreach (RFont f in doc.font_list)
                    s.AppendFormat("\n{{\\f{0}\\{1}\\fcharset{2} {3};}}", f.font_id, FamilyName(f), f.charset, f.FontName);
                s.Append("}\n");
            }
        }
        private void SaveColorTable(StringBuilder s)
        {
            if (doc.color_list != null)
            {
                s.Append("{\\colortbl;");
                foreach (System.Drawing.Color c in doc.color_list)
                    s.AppendFormat("\\red{0}\\green{1}\\blue{2};", c.R, c.G, c.B);

                s.Append("}\n");
            }
        }
        private void SaveStyles(StringBuilder s)
        {
            if (doc.style_list != null)
            {
                ParagraphFormat default_style = new ParagraphFormat();
                s.Append("{\\stylesheet");
                foreach (Style style in doc.style_list)
                {
                    s.Append("{\\s").Append(style.styledef);
                    SaveParagraphFormat(s, style.paragraph_style, default_style);
                    s.Append(" ").Append(style.stylename).Append('}');
                }
                s.Append("}\n");
            }
        }
        internal void SaveHeader(StringBuilder s)
        {
            s.Append("\\rtf1\\ansi");
            if (doc.codepage != 0)
                s.AppendFormat("\\ansicpg{0}", doc.codepage);
            if (doc.default_font != 0)
                s.AppendFormat("\\deff{0}", doc.default_font);
            if (doc.default_lang != 0)
                s.AppendFormat("\\deflang{0}", doc.default_lang);

            SaveFontTable(s);
            SaveColorTable(s);
            SaveStyles(s);
        }

        private string FamilyName(RFont font)
        {
            switch (font.family)
            {
                case RFont.Family.Nil:
                    return "fnil";
                case RFont.Family.Rroman:
                    return "froman";
                case RFont.Family.Swiss:
                    return "fswiss";
                case RFont.Family.Modern:
                    return "fmodern";
                case RFont.Family.Script:
                    return "fscript";
                case RFont.Family.Decor:
                    return "fdecor";
                case RFont.Family.Tech:
                    return "ftech";
                case RFont.Family.Bidi:
                    return "fbidi";
                default:
                    return "froman";
            }
        }

        internal void SaveDocumentBody(StringBuilder s)
        {
            s.AppendFormat("\\paperw{0}\\paperh{1}\\margl{2}\\margt{3}\\margr{4}\\margb{5}\\deftab{6}\n",
                doc.paper_width, doc.paper_height,
                doc.global_margin_left, doc.global_margin_top, doc.global_margin_right, doc.global_margin_bottom,
                doc.default_tab_width);
            if (doc.pages != null)
                foreach (Page p in doc.pages)
                    SavePage(s, p, false);
        }

        private void SavePage(StringBuilder s, Page page, bool v)
        {
            if (page.page_width != 0)
                s.AppendFormat(@"\pgwsxn{0}", page.page_width);
            if (page.page_heigh != 0)
                s.AppendFormat(@"\pghsxn{0}", page.page_heigh);
            if(page.margin_left != 0)
                s.AppendFormat(@"\marglsxn{0}", page.margin_left);
            if (page.margin_right != 0)
                s.AppendFormat(@"\margrsxn{0}", page.margin_right);
            if (page.margin_top != 0)
                s.AppendFormat(@"\margtsxn{0}", page.margin_top);
            if (page.margin_bottom != 0)
                s.AppendFormat(@"\margbsxn{0}", page.margin_bottom);
            SaveSequence(s, page.sequence, false);
        }

        private string HorizontalAlignCode(ParagraphFormat.HorizontalAlign align)
        {
            switch (align)
            {
                case ParagraphFormat.HorizontalAlign.Centered: return "\\qc";
                case ParagraphFormat.HorizontalAlign.Left: return "\\ql";
                case ParagraphFormat.HorizontalAlign.Right: return "\\qr";
                case ParagraphFormat.HorizontalAlign.Justified: return "\\qj";
                case ParagraphFormat.HorizontalAlign.Distributed: return "\\qd";
                case ParagraphFormat.HorizontalAlign.Kashida: return "\\qk";
                case ParagraphFormat.HorizontalAlign.Thai: return "\\qt";
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

        private void SaveParagraphFormat(StringBuilder sb, ParagraphFormat format, ParagraphFormat prev_format)
        {
            if (format.align != prev_format.align)
                sb.Append(HorizontalAlignCode(format.align));
            if (format.line_spacing != prev_format.line_spacing)
                sb.Append("\\sl").Append(format.line_spacing);
            if (format.space_before != prev_format.space_before)
                sb.Append("\\sb").Append(format.space_before);
            if (format.space_after != prev_format.space_after)
                sb.Append("\\sa").Append(format.space_after);

            if (format.left_indent != prev_format.left_indent)
                sb.Append("\\li").Append(format.left_indent);
            if (format.right_indent != prev_format.right_indent)
                sb.Append("\\fi").Append(format.right_indent);
            if (format.first_line_indent != prev_format.first_line_indent)
                sb.Append("\\fi").Append(format.first_line_indent);

            if (format.lnspcmult != prev_format.lnspcmult)
                sb.Append("\\slmult").Append(format.lnspcmult);
        }

        private void SavePargraph(StringBuilder sb, Paragraph par, bool InTable)
        {
            if (par.runs.Count == 0)
            {
                if (!InTable)
                    sb.Append("\\pard\\par\n");
                else
                    sb.Append("\\pard\\intbl\\cell\n");
            }
            else
            {
                if (InTable)
                    sb.Append("\\intbl\n");

                if (par.format.align != prev_align)
                {
                    sb.AppendFormat("{0} ", HorizontalAlignCode(par.format.align));
                    prev_align = par.format.align;
                }

                sb.Append("\\sl").Append(par.format.line_spacing);
                sb.Append("\\sb").Append(par.format.space_before);
                sb.Append("\\sa").Append(par.format.space_after);

                sb.Append("\\li").Append(par.format.left_indent);
                sb.Append("\\fi").Append(par.format.right_indent);
                sb.Append("\\fi").Append(par.format.first_line_indent);

                sb.Append("\\slmult").Append(par.format.lnspcmult);


                foreach (Run r in par.runs)
                {
                    RunFormat fmt = r.format;
                    if (current_format.bold != fmt.bold)
                    {
                        sb.Append(fmt.bold ? "\\b" : "\\b0");
                        current_format.bold = fmt.bold;
                    }
                    if (current_format.underline != fmt.underline)
                    {
                        sb.Append(fmt.underline ? "\\ul" : "\\ulnone");
                        current_format.underline = fmt.underline;
                    }
                    if (current_format.italic != fmt.italic)
                    {
                        sb.Append(fmt.italic ? "\\i" : "\\i0");
                        current_format.italic = fmt.italic;
                    }
                    int curr_color_idx = doc.color_list.IndexOf(current_format.color);
                    int fmt_color_idx = doc.color_list.IndexOf(fmt.color);
                    if(fmt_color_idx == -1)
                    {
                        fmt_color_idx = doc.color_list.Count;
                        doc.color_list.Add(fmt.color);
                    }
                    if (curr_color_idx != fmt_color_idx)
                    {
                        sb.AppendFormat("\\cf{0}", fmt_color_idx);
                        current_format.color = fmt.color;
                    }
                    if (current_format.font_size != fmt.font_size)
                    {
                        sb.AppendFormat("\\fs{0}", fmt.font_size);
                        current_format.font_size = fmt.font_size;
                    }
                    if (current_format.font_idx != fmt.font_idx)
                    {
                        sb.AppendFormat("\\f{0}", fmt.font_idx);
                        current_format.font_idx = fmt.font_idx;
                    }
                    sb.Append(" ");
                    if (r.text.Length == 0)
                        sb.Append("\\tab ");
                    else
#if true
                        foreach (char ch in r.text)
                        {
                            if (ch < 128 || DomesticCodepage)
                                sb.Append(ch);
                            else
                            {
                                sb.Append("\\u" + Convert.ToUInt32(ch) + "?");
                            }
                        }
#else
                        s.Append(r.text);
#endif
                }
                if (!InTable)
                    sb.Append("\\par\n");
                else
                    sb.Append("\\cell\n");
            }
        }

        private void SavePicture(StringBuilder CellsStream, Picture pic, bool InTable)
        {
            CellsStream.Append("\\sb0\\li0\\sl0\\slmult0\\qc\\clvertalc {");
            float dx = (int)pic.width;
            float dy = (int)pic.height;
            CellsStream.Append("\\pict\\picw").Append(dx.ToString());
            CellsStream.Append("\\pich").Append(dy.ToString());
            CellsStream.Append("\\picwgoal").Append(pic.desired_width);
            CellsStream.Append("\\pichgoal").Append(pic.desired_height);
            CellsStream.Append("\\picscalex").Append(pic.scalex);
            CellsStream.Append("\\picscaley").Append(pic.scaley);
            if (imageFormat == System.Drawing.Imaging.ImageFormat.Jpeg)
            {
                CellsStream.Append("\\jpegblip\r\n");
            }
            else
            {
                CellsStream.Append("\\pngblip\r\n");
                imageFormat = System.Drawing.Imaging.ImageFormat.Png;
            }
            int n;
            byte[] picbuff = new Byte[PIC_BUFF_SIZE];
            MemoryStream pic_stream = new MemoryStream();
            pic.image.Save(pic_stream, imageFormat);
            pic_stream.Position = 0;
            do
            {
                n = pic_stream.Read(picbuff, 0, PIC_BUFF_SIZE);
                for (int z = 0; z < n; z++)
                {
                    CellsStream.Append(XCONV[picbuff[z] >> 4]);
                    CellsStream.Append(XCONV[picbuff[z] & 0xF]);
                }
                CellsStream.Append("\r\n");
            }
            while (n == PIC_BUFF_SIZE);
            CellsStream.Append("}");
        }

        private void SaveTable(StringBuilder s, Table tbl, bool InTable)
        {
            s.Append("\\trowd");
            foreach (Column col in tbl.columns)
                SaveColumn(s, col);
            foreach (TableRow row in tbl.rows)
                SaveRow(s, row);
        }

        private void SaveColumn(StringBuilder s, Column col)
        {
            // "clcbpat" - background color
            s.AppendFormat("{0}\\cellx{1}", VerticalAlignCode(col.valign), col.Width);
        }

        private void SaveRow(StringBuilder s, TableRow row)
        {
            s.AppendFormat("\\trgaph{0}\\trrh{1}\\trpaddl{2}\\trpaddr{3}",
              row.trgaph, row.height, row.default_pad_left, row.default_pad_right);

            foreach (RichObjectSequence seq in row.cells)
            {
                SaveSequence(s, seq, true);
                s.AppendLine("\\cell");
            }
            s.Append("\\row");
        }

        private void SaveSequence(StringBuilder s, RichObjectSequence seq, bool InTable)
        {
            foreach (RichObject robj in seq.objects)
            {
                switch (robj.type)
                {
                    case RichObject.Type.Paragraph:
                        SavePargraph(s, robj.pargraph, InTable);
                        break;
                    case RichObject.Type.Picture:
                        SavePicture(s, robj.picture, InTable);
                        break;
                    case RichObject.Type.Table:
                        SaveTable(s, robj.table, InTable);
                        break;
                }
            }
        }

        public void Save(Stream stream)
        {
            StringBuilder s = new StringBuilder();
            stream.WriteByte((byte)'{');
            SaveHeader(s);
            SaveDocumentBody(s);
            ExportUtils.Write(stream, s.ToString());
            stream.WriteByte((byte)'}');
        }

        public void Dispose()
        {
            // Perhaps that everything clean
        }

    }
#pragma warning restore 1591
}
