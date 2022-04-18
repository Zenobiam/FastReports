using FastReport;
using System.IO;
using FastReport.RichTextParser;
using System.Drawing;
using System.Collections.Generic;
using System;
using System.Text;
using FastReport.Table;
using System.Windows.Forms;

namespace FastReport.Design.ImportPlugins.RTF
{
    internal partial class ImportRtf
    {
        #region Transformation options
        public bool debugPageClient = false;
        public bool debugBandBorder = false;
        public bool debugMemoBorder = false;
        public bool convertPadding = true;
        public bool enableWYSIWYG = false;
        public bool allowExpressions = false;
        public bool exportHeader = true;
        public bool exportFooter = true;
        #endregion

        static int DpiX = 96;
        const float Divisor = 56.6929131f * 1.25f;

        Report report = null;
        string fileName = string.Empty;
        RichDocument rtf;

        int paragraph_counter;
        int tables_counter;
        int pictures_counter;
        int rows_counter;
        int columns_counter;
        int cell_counter;

        Font default_font = new Font(FontFamily.GenericSerif, 12, FontStyle.Regular);
        RunFormat current_format;

        internal Report Report { get { return report; } }

        private static float Twips2Millimeters(long twips)
        {
            return (float)(((double)twips) * 0.01763889);
        }
        private static float Twips2Pixels(long twips)
        {
            return (Twips2Millimeters(twips) * DpiX / 25.4f);
        }

        private Font GetFontFromRichStyle(RichDocument rtf, RunFormat format)
        {
            int font_idx = (int)format.font_idx;
            if (font_idx < rtf.font_list.Count)
            {
                RFont rf = rtf.font_list[font_idx];
                string Name = rf.FontName;
                FontStyle style = format.bold ? FontStyle.Bold : FontStyle.Regular;
                return new Font(rf.FontName, format.font_size / 2, style);
            }
            else
                return default_font;
        }

        private FastReport.BorderLine TranslateBorderLine(FastReport.RichTextParser.BorderLine rtf_border_line)
        {
            FastReport.BorderLine border_line = new FastReport.BorderLine();
            switch (rtf_border_line.style)
            {
                case FastReport.RichTextParser.BorderLine.Style.Thin:
                    border_line.Style = LineStyle.Solid;
                    break;
                case FastReport.RichTextParser.BorderLine.Style.Thick:
                    border_line.Style = LineStyle.Solid;
                    break;
                case FastReport.RichTextParser.BorderLine.Style.Double:
                    border_line.Style = LineStyle.Double;
                    break;
                case FastReport.RichTextParser.BorderLine.Style.Dotted:
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

        private TextObject CreateParagraph(Paragraph rtf_paragraph, float Width)
        {
            TextObject paragraph = new TextObject();
            paragraph.Width = Width;
            paragraph.SetReport(this.report);
            paragraph.TextRenderType = TextRenderType.HtmlParagraph;
            paragraph.CanGrow = true;
            paragraph.CanBreak = true;
            paragraph.CanShrink = false;
            paragraph.GrowToBottom = true;
            paragraph.Wysiwyg = enableWYSIWYG;
            paragraph.GrowToBottom = false;
            paragraph.AllowExpressions = allowExpressions;
            if(rtf_paragraph.format.first_line_indent > 0)
                paragraph.FirstTabOffset = rtf_paragraph.format.first_line_indent;

            ++paragraph_counter;

            paragraph.Name = "p" + paragraph_counter.ToString();
            if (rtf_paragraph.format.first_line_indent < 0)
                // paragraph.FirstTabOffset = -rtf_paragraph.format.first_line_indent;
                paragraph.FirstTabOffset = 0; // Check it
                //throw new Exception("This exception should be never happen due to negative first line indent handled early");
            else
                paragraph.FirstTabOffset = rtf_paragraph.format.first_line_indent;

            if (rtf_paragraph.runs.Count > 0)
            {
                if(rtf_paragraph.format.list_id != null && rtf_paragraph.format.list_id.Count > 0)
                {
                    paragraph.Text = string.Empty;
                    foreach(Run r in rtf_paragraph.format.list_id)
                    {
                        if (r.text == "\t")
                            paragraph.Text += " ";
                        else
                            paragraph.Text += r.text;
                    }
                    paragraph.Text += GenerateHTMLText(paragraph, rtf_paragraph);
                }
                else
                    paragraph.Text = GenerateHTMLText(paragraph, rtf_paragraph);
            }
            else
                paragraph.Font = default_font;
            switch (rtf_paragraph.format.align)
            {
                case FastReport.RichTextParser.ParagraphFormat.HorizontalAlign.Right:
                    paragraph.HorzAlign = HorzAlign.Right;
                    break;
                case FastReport.RichTextParser.ParagraphFormat.HorizontalAlign.Centered:
                    paragraph.HorzAlign = HorzAlign.Center;
                    break;
                case FastReport.RichTextParser.ParagraphFormat.HorizontalAlign.Justified:
                    paragraph.HorzAlign = HorzAlign.Justify;
                    break;
                default:
                    paragraph.HorzAlign = HorzAlign.Left;
                    break;
            }

            paragraph.Border.Lines = BorderLines.None;
            int lineheight = rtf_paragraph.format.line_spacing;
            if (lineheight == 0)
                paragraph.LineHeight = paragraph.Font.Height; // * 1.2f;
            else
            {
                switch (rtf_paragraph.format.lnspcmult)
                {
                    case FastReport.RichTextParser.ParagraphFormat.LnSpcMult.Exactly:
                        lineheight = (int)(lineheight / 240f);
                        break;
                    case FastReport.RichTextParser.ParagraphFormat.LnSpcMult.Multiply:
                        lineheight = (int)Twips2Millimeters(lineheight);
                        break;
                }
                paragraph.LineHeight = lineheight < 0 ? -lineheight : lineheight >= paragraph.Font.Height ? lineheight : paragraph.Font.Height;
            }

            if(convertPadding)
            {
                Padding p = paragraph.Padding;
                p.Top = (int)Twips2Pixels(rtf_paragraph.format.space_before);
                p.Bottom = (int)Twips2Pixels(rtf_paragraph.format.space_after);
                p.Left = (int)Twips2Pixels(rtf_paragraph.format.left_indent);
                p.Right = (int)Twips2Pixels(rtf_paragraph.format.right_indent);
                paragraph.Padding = p;
            }

            if (debugMemoBorder && paragraph.Border.Lines == BorderLines.None)
            {
                paragraph.Border.Color = Color.Red;
                paragraph.Border.Width = 1;
                paragraph.Border.Lines = BorderLines.All;
            }

            paragraph.Height = paragraph.CalcHeight() + paragraph.Padding.Vertical;
            return paragraph;
        }

        private TableObject CreateTable(FastReport.RichTextParser.Table rtf_table)
        {
            TableObject table = new TableObject();
            ++tables_counter;
            table.Name = "tbl_" + tables_counter.ToString();
            int idx = 0;
            uint prev_width = 0;

            IList<TranslationPropeties> row_properties = new List<TranslationPropeties>();
            foreach (Column rtf_column in rtf_table.columns)
            {
                TableColumn column = new TableColumn();
                column.Name = "cm_" + ++columns_counter;
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

            foreach (FastReport.RichTextParser.TableRow rtf_row in rtf_table.rows)
            {
                int height = rtf_row.height;
                if (height < 0)
                    height = -height;
                FastReport.Table.TableRow row = new FastReport.Table.TableRow();
                row.Name = "rw_" + ++rows_counter;
                row.Height += height /25;
                int cell_idx = 0;
                float x_pos = 0;
                float max_cell_height = 0;
                int colnum = 0;

                foreach (RichObjectSequence sequence in rtf_row.cells)
                {
                    float y_pos = 0;
                    Column column = rtf_table.columns[colnum++];
                    TableColumn rtf_column = table.Columns[cell_idx];
                    TableCell cell = new TableCell();
                    ++cell_counter;
                    cell.Name = "cl_" + cell_counter;
                    TranslationPropeties prop = row_properties[cell_idx];
                    cell.Border = prop.border;
                    cell.FillColor = prop.background_color;

                    foreach (FastReport.RichTextParser.RichObject obj in sequence.objects)
                    {
                        switch (obj.type)
                        {
                            case FastReport.RichTextParser.RichObject.Type.Paragraph:
                                if (obj.pargraph.runs.Count == 0)
                                {
                                    y_pos += 13.8f;  // This is not correct value. Need to be calculated
                                    continue;
                                }
                                TextObject text_paragraph = CreateParagraph(obj.pargraph, column.Width);
                                switch(column.valign)
                                {
                                    case Column.VertAlign.Top:
                                        text_paragraph.VertAlign = VertAlign.Top;
                                        break;
                                    case Column.VertAlign.Center:
                                        text_paragraph.VertAlign = VertAlign.Center;
                                        break;
                                    case Column.VertAlign.Bottom:
                                        text_paragraph.VertAlign = VertAlign.Bottom;
                                        break;
                                }
                                //text_paragraph.VertAlign
                                text_paragraph.Width = rtf_column.Width;
                                text_paragraph.Height = text_paragraph.CalcHeight();
                                text_paragraph.Top = y_pos;
                                y_pos += text_paragraph.Height;

                                Padding p = text_paragraph.Padding;
                                p.Top = (int)Twips2Millimeters(obj.pargraph.format.space_before);
                                p.Bottom = (int)Twips2Millimeters(obj.pargraph.format.space_after);
                                p.Left = (int)Twips2Millimeters(obj.pargraph.format.left_indent)+3;
                                p.Right = (int)Twips2Millimeters(obj.pargraph.format.right_indent)+3;
                                text_paragraph.Padding = p;

                                text_paragraph.Parent = cell;
                                cell.Objects.Add(text_paragraph); // = GetRawText(obj.pargraph);
                                break;

                            case FastReport.RichTextParser.RichObject.Type.Picture:
                                PictureObject picture = CreatePicture(obj.picture);
                                //cell.Objects.Add(picture);
                                picture.Parent = cell;
                                break;

                            case FastReport.RichTextParser.RichObject.Type.Table:
                                TableObject subtable = CreateTable(obj.table);
                                cell.Objects.Add(table);
                                break;
                        }
                    }
                    cell.Parent = row;

                    cell.Left = x_pos;
                    if (max_cell_height < y_pos)
                        max_cell_height = y_pos;
                    cell.Height = max_cell_height;

                    row.Height = (row.Height > max_cell_height) ? row.Height : max_cell_height;

                    if (cell.Objects != null && cell.Objects.Count == 1)
                    {
                        ReportComponentBase obj = cell.Objects[0] as ReportComponentBase;

                        if (obj is TextObject)
                        {
                            TextObject text_cell = obj as TextObject;
                            switch(column.valign)
                            {
                                case Column.VertAlign.Top:
                                    text_cell.VertAlign = VertAlign.Top;
                                    break;
                                case Column.VertAlign.Center:
                                    text_cell.VertAlign = VertAlign.Center;
                                    break;
                                case Column.VertAlign.Bottom:
                                    text_cell.VertAlign = VertAlign.Bottom;
                                    break;
                            }

                        }
                        obj.Height = row.Height;
                    }

                    x_pos += rtf_column.Width;
                    cell_idx++;
                }
                row.Parent = table;
                table.Rows.Add(row);
                table.Height +=row.Height;
            }
            return table;
        }

        private PictureObject CreatePicture(FastReport.RichTextParser.Picture rtf_picture)
        {
            PictureObject pic = new PictureObject();
            pic.Image = rtf_picture.image;
            pic.SetReport(this.report);
            pic.Name = "pic" + ++pictures_counter;

            if (rtf_picture.desired_width != 0)
            {
              if(rtf_picture.scalex == 0)
                pic.Width = Twips2Pixels(rtf_picture.desired_width);
              else
                pic.Width = Twips2Pixels(rtf_picture.desired_width * rtf_picture.scalex / 100);
            }

            else
                pic.Width = Twips2Pixels(rtf_picture.width);

            if (rtf_picture.desired_height != 0)
            {
                if (rtf_picture.scaley != 0)
                    pic.Height = Twips2Pixels(rtf_picture.desired_height);
                else
                    pic.Height = Twips2Pixels(rtf_picture.desired_height * rtf_picture.scaley / 100);
            }
            else
                pic.Height = Twips2Pixels(rtf_picture.height);
            return pic;
        }

        #region Create  RTF objects on Band

        private float CreateNegativeFirstLineIndent(BandBase band, Paragraph rtf_paragraph)
        {
            List<Run> left_runs = new List<Run>();
            List<Run> right_runs = new List<Run>();
            Font left_font = null;

            for(int i = 0; i<rtf_paragraph.runs.Count; ++i)
            {
                switch(i)
                {
                    case 0:
                        left_runs.Add(rtf_paragraph.runs[i]);
                        left_font = GetFontFromRichStyle(this.rtf, rtf_paragraph.runs[i].format);
                        if (left_font.Style != FontStyle.Regular)
                            left_font = new Font(left_font, FontStyle.Regular);
                        break;
                    case 1:
                        if (rtf_paragraph.runs[i].text != "\t")
                            throw new Exception("State machine error");
                        break;
                    default:
                        right_runs.Add(rtf_paragraph.runs[i]);
                        break;
                }
            }

            TableObject table = new TableObject();
            table.Top = band.Height;
            table.Width = band.Width;
            table.SetReport(this.report);

            TableColumn left_column = new TableColumn();
            left_column.Width = Twips2Pixels(-rtf_paragraph.format.first_line_indent);
            left_column.SetIndex(0);
            //TranslationPropeties prop = new TranslationPropeties();
            //prop.border = TranslateBorders(column.);
            //prop.background_color = column.back_color;
            //row_properties.Add(prop);
            table.Columns.Add(left_column);

            TableColumn right_column = new TableColumn();
            right_column.Width = band.Width - Twips2Pixels(-rtf_paragraph.format.first_line_indent);
            right_column.SetIndex(1);
            table.Columns.Add(right_column);

            TableCellData left_cell_data = new TableCellData();
            left_cell_data.Objects = new ReportComponentCollection();

            TableCellData right_cell_data = new TableCellData();
            right_cell_data.Objects = new ReportComponentCollection();

            TextObject left_text = new TextObject();
            ++paragraph_counter;
            left_text.Width = left_column.Width;
            left_text.SetReport(this.report);
            left_text.TextRenderType = TextRenderType.HtmlParagraph;
            left_text.CanGrow = true;
            left_text.CanBreak = false;
            left_text.CanShrink = false;
            left_text.GrowToBottom = true;
            left_text.Wysiwyg = enableWYSIWYG;
            left_text.AllowExpressions = allowExpressions;
            left_text.Name = "p" + paragraph_counter.ToString();
            rtf_paragraph.runs = left_runs;
            left_text.Font = left_font;
            left_text.Text = GenerateHTMLText(left_text, rtf_paragraph);
            left_text.Font = left_font;

            TableCell left_cell = new TableCell();
            left_cell_data.Objects.Add(left_text);
            left_cell.CellData = left_cell_data;
            left_cell.Left = 0;
            left_text.SetParent(left_cell);

            TextObject right_text = new TextObject();
            ++paragraph_counter;
            right_text.Width = right_column.Width;
            right_text.SetReport(this.report);
            right_text.TextRenderType = TextRenderType.HtmlParagraph;
            right_text.CanGrow = true;
            right_text.CanBreak = true;
            right_text.CanShrink = false;
            right_text.GrowToBottom = true;
            right_text.Wysiwyg = enableWYSIWYG;
            right_text.AllowExpressions = allowExpressions;
            right_text.Name = "p" + paragraph_counter.ToString();
            rtf_paragraph.runs = right_runs;
            right_text.Font = left_font;
            right_text.Text = GenerateHTMLText(right_text, rtf_paragraph);
            right_text.Font = left_font;
            float height = right_text.CalcHeight();
            left_text.Height = height;
            right_text.Height = height;


            TableCell right_cell = new TableCell();
            right_cell_data.Objects.Add(right_text);
            right_cell.CellData = right_cell_data;
            right_text.SetParent(right_cell);

            FastReport.Table.TableRow row = new FastReport.Table.TableRow();
            row.Top = band.Height;
            row.Height = height;
            row.AddChild(left_cell);
            row.AddChild(right_cell);

            table.Rows.Add(row);
            table.Height = height+150;

            band.Objects.Add(table);
            band.Height += table.Height;

            return height;
        }

        private float TransformTabstopsToTables(BandBase band, Paragraph rtf_paragraph)
        {
            TableObject table = new TableObject();
            table.Name = "t" + ++tables_counter;
            FastReport.Table.TableRow row = new FastReport.Table.TableRow();
            row.Name = "r" + ++rows_counter;

            int tabstsopcount = rtf_paragraph.format.tab_positions.Count;
            int tabcount = 0;

            int prev_pos = 0;
            int idx = 0;
            int left = 0;
            float cellHeight = 0;

            table.Top = band.Bottom;

            foreach (Run r in rtf_paragraph.runs)
            {
                if (r.text == "\t")
                    ++tabcount;
                else
                {
                    TableColumn column = new TableColumn();
                    TableCell cell = new TableCell();

                    if (idx < rtf_paragraph.format.tab_positions.Count)
                    {
                        left = rtf_paragraph.format.tab_positions[idx];
                        column.Width = Twips2Pixels(left - prev_pos);
                    }
                    else
                    {
                        column.Width = (band.Width - Twips2Pixels(prev_pos));
                    }

                    column.Name = "cm_" + ++columns_counter;
                    prev_pos = left;
                    column.SetIndex(idx);

                    cell.SetReport(report);
                    ++cell_counter;
                    cell.Name = "cl_" + cell_counter;
                    cell.Parent = row;
                    cell.Text = r.text;
                    cell.Font = GetFontFromRichStyle(rtf, r.format);

                    float chg = cell.CalcHeight();
                    if (chg > cellHeight)
                        cellHeight = chg;

                    table.Columns.Add(column);
                    idx++;
                }
            }
            row.Height = cellHeight;
            table.Height = row.Height;
            band.Height += row.Height;
            table.Rows.Add(row);
            row.Parent = table;
            band.Objects.Add(table);

            return 20;
        }
        private float CreateParagraph(BandBase band, Paragraph rtf_paragraph)
        {
            if (rtf_paragraph.format.tab_positions != null && rtf_paragraph.format.tab_positions.Count > 0)
            {
                return TransformTabstopsToTables(band, rtf_paragraph);
            }

            if (rtf_paragraph.format.first_line_indent < 0)
            {
                if(rtf_paragraph.runs.Count > 2)
                    if(rtf_paragraph.runs[1].text == "\t")
                        return CreateNegativeFirstLineIndent(band, rtf_paragraph);
            }

            if (rtf_paragraph.runs.Count != 0)
            {
                TextObject paragraph = CreateParagraph(rtf_paragraph, band.Width);
                paragraph.Top = band.Height;
//                paragraph.TabWidth = Twips2Pixels(916);
                band.Objects.Add(paragraph);
                band.Height += paragraph.Height;
                return paragraph.Height;
            }
            TextObject empty_paragraph = new TextObject();
            empty_paragraph.Width = band.Width;
            empty_paragraph.Text = "W";
            empty_paragraph.CanGrow = true;
            empty_paragraph.SetReport(report);
            float height = empty_paragraph.CalcHeight();
            empty_paragraph = null;
            band.Height += height;
            return height;
        }
        private float CreateTable(BandBase band, FastReport.RichTextParser.Table rtf_table)
        {
            TableObject table = CreateTable(rtf_table);
            table.Parent = band;
            table.Top = band.Height;
            table.Height = table.CalcHeight();
            band.Objects.Add(table);
            band.Height += table.Height;
            return table.Height;
        }
        private float CreatePicture(BandBase band, FastReport.RichTextParser.Picture rtf_picture)
        {
            PictureObject pic = CreatePicture(rtf_picture);
            pic.Height = pic.CalcHeight();
            pic.Top = band.Height;
            band.Objects.Add(pic);
            band.Height += pic.Height;
            return pic.Height;
        }
#endregion

        private float FillBand(BandBase band, string name, RichObjectSequence objects_on_band)
        {
            float height = 0;
            band.Name = name;
            try
            {
                foreach (FastReport.RichTextParser.RichObject rtf_obj in objects_on_band.objects)
                {
                    switch (rtf_obj.type)
                    {
                        case FastReport.RichTextParser.RichObject.Type.Paragraph:
                            height += CreateParagraph(band, rtf_obj.pargraph);
                            break;
                        case FastReport.RichTextParser.RichObject.Type.Table:
                            if(rtf_obj.table.rows.Count != 0)
                                height += CreateTable(band, rtf_obj.table);
                            break;
                        case FastReport.RichTextParser.RichObject.Type.Picture:
                            height += CreatePicture(band, rtf_obj.picture);
                            break;
                    }
                }

                if (debugBandBorder && band.Border.Lines == BorderLines.None)
                {
                    band.Border.Color = Color.Green;
                    band.Border.Width = 1;
                    band.Border.Lines = BorderLines.All;
                }

            }
            catch (Exception excp)
            {
                // Don't use Windows.Forms API (MessageBox etc.) in shared files
                throw new Exception(excp.Message);
            }
            return height;
        }

        private void ImportRtfPage(int page_num, Page rtf_page)
        {
            float vertical_position_counter = 0;
            // float horizontal_position_counter = 0;
            ReportPage page = new ReportPage();
            if(debugPageClient)
            {
                page.Border.Width = 1;
                page.Border.Color = Color.Blue;
                page.Border.Lines = BorderLines.All;
            }

            //float hc = rtf.global_margin_top / 25.4f;
            //float vc = rtf.global_margin_right / 31.75f;

            page.Width = Twips2Pixels(rtf_page.page_width != 0 ? rtf_page.page_width : rtf.paper_width);
            page.Height = Twips2Pixels(rtf_page.page_heigh != 0 ? rtf_page.page_heigh : rtf.paper_height);

            page.LeftMargin = (rtf_page.margin_left != 0 ? rtf_page.margin_left : rtf.global_margin_left ) / Divisor;
            page.RightMargin = (rtf_page.margin_right != 0 ? rtf_page.margin_right : rtf.global_margin_right) / Divisor;

            page.TopMargin = (rtf_page.margin_top != 0 ? rtf_page.margin_top : rtf.global_margin_top) / Divisor;
            page.BottomMargin = (rtf_page.margin_bottom != 0 ? rtf_page.margin_bottom : rtf.global_margin_bottom) / Divisor;

            float BandWidth = page.Width - (page.LeftMargin + page.RightMargin) * DpiX / 25.4f;
            page.Name = "page_" + page_num;
            page.PaperWidth = 260;

            if (exportHeader & (rtf_page.header.objects != null) && (rtf_page.header.objects.Count > 0))
            {

                page.PageHeader = new PageHeaderBand();
                page.PageHeader.Width = BandWidth;
                vertical_position_counter += FillBand(page.PageHeader, "PageHeader", rtf_page.header);
            }

            if ((rtf_page.sequence.objects != null) && (rtf_page.sequence.objects.Count > 0))
            {
                DataBand dataBand = new DataBand();
                dataBand.CanGrow = true;
                dataBand.Width = BandWidth;
                dataBand.Top = vertical_position_counter;
                vertical_position_counter += FillBand(dataBand, "rtf_page", rtf_page.sequence);
                page.Bands.Add(dataBand);
            }

            if (exportFooter & (rtf_page.footer.objects != null) && (rtf_page.footer.objects.Count > 0))
            {
                page.PageFooter = new PageFooterBand();
                page.PageFooter.Width = BandWidth;
                vertical_position_counter += FillBand(page.PageFooter, "PageFooter", rtf_page.footer);
            }
            report.Pages.Add(page);
        }

        internal Report CreateReport()
        {
            int page_num = 0;
            try
            {
                report = new Report();

                foreach (Page rtf_page in rtf.pages)
                    ImportRtfPage(++page_num, rtf_page);

                report.ReportInfo.CreatorVersion = ("rtf2frx converter ver 0.5");
                report.ReportInfo.Created = SystemFake.DateTime.Now;
            }
            catch(Exception excpt)
            {
                report = null;
                fileName = excpt.Message;
            }
            return report;
        }
        internal void ResetProperties()
        {
            paragraph_counter = 0;
            tables_counter = 0;
            pictures_counter = 0;
            rows_counter = 0;
        }
        internal ImportRtf(string filename)
        {
            using (Stream rich_stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
            using (RTF_DocumentParser parser = new RTF_DocumentParser())
            {
                parser.Load(rich_stream);
                rtf = parser.Document;
                this.fileName = filename;
            }
        }
    }
}
