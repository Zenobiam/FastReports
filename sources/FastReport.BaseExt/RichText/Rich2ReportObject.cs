using FastReport.Table;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FastReport.RichTextParser
{
    class RichText2ReportObject : IDisposable
    {
        static int DpiX = 96;
        Font default_font = new Font(FontFamily.GenericSerif, 12, FontStyle.Regular);
        RunFormat current_format;

        private static int Twips2Pixels(int twips)
        {
            return (int)(((double)twips) * (1.0 / 1440.0) * DpiX);
        }

        public void Dispose()
        {
        }

        private string GetRawText(Paragraph paragraph)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Run run in paragraph.runs)
                sb.Append(run.text);
            return sb.ToString();
        }

        private void GetHTMLText(FastReport.RichObject rich, ref TextObject clone, RichDocument rtf, int position, Paragraph paragraph)
        {
            int run_num = 0;
            bool span_condition = false;
            StringBuilder sb = new StringBuilder();
            RunFormat format;

            string colorname = String.Empty;
            string fontname = String.Empty;
            string fontsize = String.Empty;
            string backcolor = String.Empty;
            Font current_font = clone.Font;

            int len;
            foreach (Run run in paragraph.runs)
            {
                len = run.text != "\r" ? run.text.Length : 1;
                if (rich.ActualTextStart != 0 && position + len <= rich.ActualTextStart)
                {
                    position += len;
                    continue;
                }
                format = run.format;
                if (run_num == 0)
                {

                    current_format = run.format;
                    clone.Font = GetFontFromRichStyle(rtf, current_format);
                    current_font = clone.Font;
                    clone.TextColor = current_format.color;

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

                    if (current_format.BColor != null) 
                    {
                        if(current_format.BColor != Color.White)
                            backcolor = string.Format("background-color:#{0:X2}{1:X2}{2:X2}", format.BColor.R, format.BColor.G, format.BColor.B);
                    }

                    if (backcolor.Length > 0) 
                    {
                        sb.Append("<span style=\"");
                        sb.Append(backcolor);
                        sb.Append("\">");
                        span_condition = true;

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
                    if (current_format.BColor != format.BColor) 
                    {
                        backcolor = string.Format("background-color:#{0:X2}{1:X2}{2:X2}", format.BColor.R, format.BColor.G, format.BColor.B);
                        current_format.BColor = format.BColor;
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

                    if (colorname.Length > 0 || fontsize.Length > 0 || fontname.Length > 0 || backcolor.Length > 0)
                    {
                        sb.Append("<span style=\"");
                        if (colorname.Length > 0)
                            sb.Append(colorname);
                        if (fontsize.Length > 0)
                            sb.Append(fontsize);
                        if (fontname.Length > 0)
                            sb.Append(fontname);
                        if (backcolor.Length > 0)
                            sb.Append(backcolor);
                        sb.Append("\">");
                        span_condition = true;
                    }

                }
                if (run.text != "\r")
                    sb.Append(run.text);
                else if( run_num != 0 && run_num + 1 < paragraph.runs.Count)
                    sb.Append("<br>");

                position += len;
                if (rich.ActualTextLength != 0 && position >= rich.ActualTextStart + rich.ActualTextLength)
                    break;

                if (span_condition)
                {
                    sb.Append("</span>");
                    span_condition = false;
                }

                run_num++;
            }
            clone.Text = sb.ToString();
        }

        private FastReport.BorderLine TranslateBorderLine(BorderLine rtf_border_line)
        {
            FastReport.BorderLine border_line = new FastReport.BorderLine();
            switch (rtf_border_line.style)
            {
                case BorderLine.Style.Thin:
                    border_line.Style = LineStyle.Solid;
                    break;
                case BorderLine.Style.Thick:
                    border_line.Style = LineStyle.Solid;
                    break;
                case BorderLine.Style.Double:
                    border_line.Style = LineStyle.Double;
                    break;
                case BorderLine.Style.Dotted:
                    border_line.Style = LineStyle.Dot;
                    break;
                default:
                    border_line.Style = LineStyle.Solid;
                    break;
            }
            border_line.Color = rtf_border_line.color;
            border_line.Width = Twips2Pixels((int)rtf_border_line.width);
            return border_line;
        }

        private FastReport.Border TranslateBorders(Column rtf_column)
        {
            FastReport.Border border = new Border();
            border.Lines = BorderLines.None;

            if (rtf_column.border_top.width > 0)
            {
                border.TopLine = TranslateBorderLine(rtf_column.border_top);
                border.Lines |= BorderLines.Top;
            }

            if (rtf_column.border_right.width > 0)
            {
                border.RightLine = TranslateBorderLine(rtf_column.border_right);
                border.Lines |= BorderLines.Right;
            }

            if (rtf_column.border_left.width > 0)
            {
                border.LeftLine = TranslateBorderLine(rtf_column.border_left);
                border.Lines |= BorderLines.Left;
            }

            if (rtf_column.border_bottom.width > 0)
            {
                border.BottomLine = TranslateBorderLine(rtf_column.border_bottom);
                border.Lines |= BorderLines.Bottom;
            }

            return border;
        }

        private Font GetFontFromRichStyle(RichDocument rtf, RunFormat format)
        {
            int font_idx = (int)format.font_idx;
            if (font_idx < rtf.font_list.Count)
            {
                RFont rf = rtf.font_list[font_idx];
                string Name = rf.FontName;
#if false // Broke PDF export
        FontStyle style = format.bold ? FontStyle.Bold : FontStyle.Regular;
#else
                FontStyle style = FontStyle.Regular;
#endif
                return new Font(rf.FontName, format.font_size / 2, style);
            }
            else
                return default_font;
        }

        internal TextObject Paragraph2ReportObjects(FastReport.RichObject rich, RichDocument rtf, int position, Paragraph paragraph)
        {
            TextObject clone = new TextObject();

            clone.TextRenderType = TextRenderType.HtmlParagraph;
            clone.CanShrink = rich.CanShrink;
            clone.CanGrow = rich.CanGrow;
            clone.CanBreak = rich.CanBreak;
            clone.Left = 0; // Will set in another place
            clone.GrowToBottom = false; // Can't be set here;
            clone.ClientSize = rich.ClientSize;
            clone.TextColor = Color.Black;
            clone.FirstTabOffset = 48;
            clone.TabWidth = 48;

            clone.FillColor = rich.FillColor;

            if (paragraph.runs.Count > 0)
            {
                if(paragraph.format.tab_positions != null)
                {
                    int count = paragraph.format.tab_positions.Count;
                    if (count > 0)
                        clone.FirstTabOffset = paragraph.format.tab_positions[0] / 15;
                    if(count > 1)
                        clone.TabWidth = (paragraph.format.tab_positions[1] / 15) - clone.FirstTabOffset;
                }
                GetHTMLText(rich, ref clone, rtf, position, paragraph);
            }
            else
                clone.Font = default_font;

            switch (paragraph.format.align)
            {
                case ParagraphFormat.HorizontalAlign.Right:
                    clone.HorzAlign = HorzAlign.Right;
                    break;
                case ParagraphFormat.HorizontalAlign.Centered:
                    clone.HorzAlign = HorzAlign.Center;
                    break;
                case ParagraphFormat.HorizontalAlign.Justified:
                    clone.HorzAlign = HorzAlign.Justify;
                    break;
                default:
                    clone.HorzAlign = HorzAlign.Left;
                    break;
            }

            switch (paragraph.format.Valign)
            {
                case ParagraphFormat.VerticalAlign.Top:
                    clone.VertAlign = VertAlign.Top;
                    break;
                case ParagraphFormat.VerticalAlign.Center:
                    clone.VertAlign = VertAlign.Center;
                    break;
                case ParagraphFormat.VerticalAlign.Bottom:
                    clone.VertAlign = VertAlign.Bottom;
                    break;
                default:
                    clone.VertAlign = VertAlign.Top;
                    break;
            }

            clone.Border.Lines = BorderLines.None;
            int lineheight = paragraph.format.line_spacing;
            if (lineheight == 0)
                clone.LineHeight = (float)Math.Ceiling(clone.Font.Height * DrawUtils.ScreenDpiFX); // * 1.2f;
            else
            {
                switch (paragraph.format.lnspcmult)
                {
                    case ParagraphFormat.LnSpcMult.Exactly:
                        lineheight = (int)(lineheight / 240f);
                        break;
                    case ParagraphFormat.LnSpcMult.Multiply:
                        lineheight = Twips2Pixels(lineheight);
                        break;
                }
                clone.LineHeight = lineheight < 0 ? -lineheight : lineheight >= clone.Font.Height ? lineheight : clone.Font.Height;
            }
            clone.Padding = new Padding(rich.Padding.Left, 0, rich.Padding.Right, 0);
            clone.SetReport(rich.Report);

            return clone;
        }

        internal TableObject Table2ReportObjects(FastReport.RichObject rich, RichDocument rtf, Table rtf_table)
        {
            TableObject table = new TableObject();
            int idx = 0;
            uint prev_width = 0;
            IList<TranslationPropeties> row_properties = new List<TranslationPropeties>();

            foreach (Column rtf_column in rtf_table.columns)
            {
                TableColumn column = new TableColumn();
                column.Width = Twips2Pixels((int)(rtf_column.Width - prev_width));
                prev_width = rtf_column.Width;
                column.SetIndex(idx);
                TranslationPropeties prop = new TranslationPropeties(
                    TranslateBorders(rtf_column),
                    rtf_column.back_color);
                row_properties.Add(prop);
                table.Columns.Add(column);
                idx++;
            }
            foreach (TableRow rtf_row in rtf_table.rows)
            {
                int height = rtf_row.height;
                if (height < 0)
                    height = -height;
                FastReport.Table.TableRow row = new FastReport.Table.TableRow();

                int cell_idx = 0;
                float x_pos = 0;
                foreach (RichObjectSequence sequence in rtf_row.cells)
                {
                    TableColumn rtf_column = table.Columns[cell_idx];
                    TableCell cell = new TableCell();
                    TranslationPropeties prop = row_properties[cell_idx];
                    cell.Border = prop.border;
                    cell.FillColor = prop.background_color;
                    foreach (RichObject obj in sequence.objects)
                    {
                        TableCellData cell_data = new TableCellData();
                        cell_data.Objects = new ReportComponentCollection();

                        switch (obj.type)
                        {

                            case RichObject.Type.Paragraph:
                                TextObject text_paragraph = Paragraph2ReportObjects(rich, rtf, 0, obj.pargraph); // TODO: Fix "pos" argument
                                text_paragraph.Width = rtf_column.Width;
                                if (obj.pargraph.runs.Count > 0)
                                    text_paragraph.Height = text_paragraph.CalcHeight();
                                else
                                    text_paragraph.Height = height;

                                Padding p = text_paragraph.Padding;
                                p.Top = Twips2Pixels((int)obj.pargraph.format.space_before);
                                p.Bottom = Twips2Pixels((int)obj.pargraph.format.space_after);
                                p.Left = Twips2Pixels((int)obj.pargraph.format.left_indent);
                                p.Right = Twips2Pixels((int)obj.pargraph.format.right_indent);
                                text_paragraph.Padding = p;

                                cell_data.Objects.Add(text_paragraph); // = GetRawText(obj.pargraph);
                                break;

                            case RichObject.Type.Picture:
                                PictureObject picture = Picture2ReportObject(rtf, obj.picture);
                                cell_data.Objects.Add(picture);
                                break;

                            case RichObject.Type.Table:
                                TableObject subtable = Table2ReportObjects(rich, rtf, obj.table);
                                cell_data.Objects.Add(table);
                                break;
                        }
                        cell.CellData = cell_data;
                        cell.Left = x_pos;
                        cell.Height += height;
                    }
                    row.Height = (row.Height > cell.Height) ? row.Height : cell.Height;
                    row.AddChild(cell);
                    x_pos += rtf_column.Width;
                    cell_idx++;
                }
                table.Rows.Add(row);
                table.Height += row.Height;
            }
            return table;
        }

        internal PictureObject Picture2ReportObject(RichDocument rtf, Picture rtf_picture)
        {
            PictureObject picture = new PictureObject();
            picture.Image = rtf_picture.image;
            picture.Height = rtf_picture.height;
            picture.Width = rtf_picture.width;
            return picture;
        }

        internal List<ComponentBase> Page2ReportObjects(FastReport.RichObject rich, RichDocument rtf, Page page, int start_text_index, out float page_height)
        {
            int object_counter = 0;
            page_height = 0;
            int empty_paragraph_height = 0;
            float object_vertical_position = rich.Padding.Top; // Twips2Pixels(page.margin_top); //
            List<ComponentBase> clone_list = new List<ComponentBase>();
            foreach (RichObject obj in page.sequence.objects)
            {
                if(rich.ActualTextStart != 0 && start_text_index + obj.size <= rich.ActualTextStart)
                {
                    start_text_index += (int) obj.size;
                    continue;
                }
                switch (obj.type)
                {
                    case RichObject.Type.Paragraph:
                        if (obj.pargraph.runs.Count == 0)
                        {
                            start_text_index++; // TODO: Check position increment size
                            ParagraphFormat format = obj.pargraph.format;

                            int lnspc;
                            int line_height = 17; // Not calculated yet

                            if (format.lnspcmult == ParagraphFormat.LnSpcMult.Multiply)
                                lnspc = (int)(format.line_spacing / 240f);
                            else
                                lnspc = Twips2Pixels(format.line_spacing);

                            empty_paragraph_height = lnspc < 0 ? -lnspc : lnspc >= line_height ? lnspc : line_height;
                            empty_paragraph_height += Twips2Pixels(format.space_before + format.space_after);
#if !MONO_NO
                            object_vertical_position += empty_paragraph_height;
                            TextObject empty_paragraph = Paragraph2ReportObjects(rich, rtf, start_text_index, obj.pargraph);
                            empty_paragraph.Top = object_vertical_position;
                            empty_paragraph.Height = empty_paragraph_height;
                            ++object_counter;
                            clone_list.Add(empty_paragraph);
#else
                            if (page.sequence.objects.IndexOf(obj) == page.sequence.objects.Count - 1)
                            {
                                TextObject empty_paragraph = Paragraph2ReportObjects(rich, rtf, start_text_index, obj.pargraph);
                                empty_paragraph.Top = object_vertical_position;
                                empty_paragraph.Height = empty_paragraph_height;
                                ++object_counter;
                                clone_list.Add(empty_paragraph);
                            }
                            else
                                object_vertical_position += empty_paragraph_height;

#endif
                            continue;
                        }
                        empty_paragraph_height = 0;
                        TextObject text_paragraph = Paragraph2ReportObjects(rich, rtf, start_text_index, obj.pargraph);
                        ++object_counter;

                        ////text_paragraph.SetName("Paragraph" + object_counter.ToString() ); 
                        text_paragraph.Width = rich.Width;

                        Padding p = text_paragraph.Padding;
                        p.Top = Twips2Pixels((int)obj.pargraph.format.space_before);
                        p.Bottom = Twips2Pixels((int)obj.pargraph.format.space_after);
                        if (text_paragraph.HorzAlign != HorzAlign.Center)
                        {
                            p.Left += Twips2Pixels((int)obj.pargraph.format.left_indent);
                            p.Right += Twips2Pixels((int)obj.pargraph.format.right_indent);
                        }
                        else
                        {
                            p.Left = Twips2Pixels((int)obj.pargraph.format.left_indent);
                            p.Right = Twips2Pixels((int)obj.pargraph.format.right_indent);
                        }
                        text_paragraph.Padding = p;
                        text_paragraph.Top = object_vertical_position;
                        text_paragraph.Height = text_paragraph.CalcHeight() + p.Vertical;
                        object_vertical_position += text_paragraph.Height;
                        clone_list.Add(text_paragraph);
                        break;

                    case RichObject.Type.Picture:
                        {
                            PictureObject pict = Picture2ReportObject(rtf, obj.picture);
                            pict.Top = object_vertical_position;
                            object_vertical_position += pict.Height;
                            clone_list.Add(pict);
                        }
                        break;

                    case RichObject.Type.Table:
                        {
                            TableObject tbl = Table2ReportObjects(rich, rtf, obj.table);
                            tbl.Top = object_vertical_position;
                            object_vertical_position += tbl.Height;
                            clone_list.Add(tbl);
                        }
                        break;
                }

                start_text_index += (int)obj.size;
                if (rich.ActualTextLength != 0 && start_text_index >= rich.ActualTextStart + rich.ActualTextLength)
                    break;
            }

            foreach (ComponentBase obj in clone_list)
            {
                obj.SetReport(rich.Report);
                page_height += obj.Height;
            }
            return clone_list;
        }

        private float AssingClones(FastReport.RichObject rich, List<ComponentBase> clone_list)
        {
            float top = /*rich.Top + */ rich.Padding.Top;

            foreach (ComponentBase clone in clone_list)
            {
                clone.SetReport(rich.Report);
                clone.Top = top;
                top += clone.Height;
            }
            return top + rich.Padding.Bottom /* - rich.Top */;
        }

        internal List<ComponentBase> RichObject2ReportObjects(FastReport.RichObject rich, ref RichDocument rtf, out float total_height)
        {
            List<ComponentBase> clone_list = new List<ComponentBase>();

            int position = 0; 
            total_height = 0;
            if (rtf.pages != null)
                foreach (Page page in rtf.pages)
                {
                    if (position + page.size < rich.ActualTextStart)
                    {
                        position += (int)page.size;
                        continue;
                    }
                    float page_height;
                    List<ComponentBase> virtual_object_list = Page2ReportObjects(rich, rtf, page, position, out page_height);
                    foreach(ComponentBase obj in virtual_object_list)
                    {
                        if (obj is TextObject)
                        {
                            TextObject text_object = obj as TextObject;

                            if( !string.IsNullOrEmpty(text_object.Text))
                                text_object.Height = text_object.CalcHeight() + text_object.Padding.Vertical;
                            total_height += text_object.Height;
                            clone_list.Add(obj);
                        }
                        else if(obj is PictureObject)
                        {
                            PictureObject pic = obj as PictureObject;
                            total_height += pic.Height;
                            clone_list.Add(obj);
                        }
                        else if(obj is TableObject)
                        {
                            TableObject tbl = obj as TableObject;
                            tbl.Left = 0; // Fix me
                            total_height += tbl.Height;
                            clone_list.Add(obj);
                        }
                        else
                        {
                            throw new Exception("Rich2ReportObject.cs: object type not supported");
                        }
                    }
                    position += (int)page.size;
                    if (rich.ActualTextLength != 0 && position >= rich.ActualTextStart + rich.ActualTextLength)
                        break;
                }

            total_height = AssingClones(rich, clone_list);
            return clone_list;
        }
    }

#if READONLY_STRUCTS
    internal readonly struct TranslationPropeties
#else
    internal struct TranslationPropeties
#endif
    {
        internal readonly Border border;
        internal readonly Color background_color;

        public TranslationPropeties(Border border, Color background_color)
        {
            this.border = border;
            this.background_color = background_color;
        }
    }
}
