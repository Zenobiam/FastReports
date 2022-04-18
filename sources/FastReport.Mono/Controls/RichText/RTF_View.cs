using FastReport.Controls;
using RichTextParser.RTF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FastReport.RichTextParser
{
    internal class RtfDocumentPosition
    {
        RichDocument document;
        int dispose_from_begin_file;

        int run_position;
        int page_index = 0;
        int object_index = 0;
        int run_index = 0;

        private void InvalidatePositionInDocument(int position)
        {
            long i = position;
            foreach(Page page in document.pages)
            {
                if(i >= page.size)
                {
                    i -= page.size;
                    page_index++;
                    continue;
                }
                foreach(RichObject rtf_obj in page.sequence.objects)
                {
                    if(i >= rtf_obj.size)
                    {
                        i -= rtf_obj.size;
                        object_index++;
                        continue;
                    }
                    switch(rtf_obj.type)
                    {
                        case RichObject.Type.Paragraph:
                            foreach(Run run in rtf_obj.pargraph.runs)
                            {
                                if(i >= run.text.Length)
                                {
                                    i -= run.text.Length;
                                    run_index++;
                                    continue;
                                }

                                run_position = (int) (run.text.Length - i);
                                return;
                            }
                            break;
                        case RichObject.Type.Picture:
                            throw new Exception("Check and fix picture position");
                            break;
                        case RichObject.Type.Table:
                            throw new Exception("Check and fix table position");
                            break;
                    }
                }
            }
            throw new Exception("Position below document");
        }


        public RtfDocumentPosition(int position, RichDocument doc)
        {
            dispose_from_begin_file = position;
            this.document = doc;
        }
    };

    /// <summary>
    /// FastReport RichTextView custom control.
    /// </summary>
    internal partial class RTF_View : UserControl
    {
        #region Private properties
        int top_line = 0;
        int full_document_height = 0;
        //internal Rectangle view_rect;
        SolidBrush br = new SolidBrush(Color.Black);
        StringFormat sf = new StringFormat(StringFormat.GenericTypographic);
        float font_scale;
        float view_scale = 1f;
        // bool show_margins = true;
        bool cursor_enabled = false;

        RFont[] fonts = null;
        internal List<Color> colors { get { return document.color_list; } }
        internal RichDocument document;
        private List<CommonViewObject> view_objects = new List<CommonViewObject>();
        private List<RTF_View_Page> view_pages = new List<RTF_View_Page>();

        //private Timer cursor_timer;
        private Timer cursor_timer = new Timer();
        private RTF_View_Cursor cursor;
        #endregion

        public enum VisaulHelpers
        {
            TotalOff,
            Margins,
            Paragraphs,
            MarginsAndParagraphs,
            ExcludeMarings, // Show editable area of view
            DoNotClearPageBackground = 0x8
        }

        private VisaulHelpers vh_mode = VisaulHelpers.MarginsAndParagraphs;

        public List<RTF_View_Page> Pages { get { return view_pages; } }

        public VisaulHelpers HelpersMode
        {
            get
            {
                return vh_mode;
            }
            set
            {
                vh_mode = value;
                using (Bitmap bmp = new Bitmap(1, 1))
                using (Graphics g = Graphics.FromImage(bmp))
                    full_document_height = InvalidateView(g, ClientRectangle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty
        {
            get { return document.pages == null; }
        }

        // For selection
        public int StartPosition;
        public int StopPosition;

        /// <summary>
        /// 
        /// </summary>
        public float Scaler { get { return font_scale; } internal set { font_scale = value; } }

        public float ViewScale { get { return view_scale; } internal set { view_scale = value; } }

        private void InvalidatePageSize(RTF_View_Page page)
        {
            if (page.ExcludeMarings)
            {
                page.size.Width = this.Width;
                page.size.Height = this.Height;
            }
            else
            {
                page.size.Width = Twips2Pixels((int)document.paper_width);
                page.size.Height = Twips2Pixels((int)document.paper_height);
            }
        }

        /// <summary>
        /// Markup documnet
        /// </summary>
        public int InvalidateView(Graphics g, Rectangle view_area)
        {
            int view_height = 0;
            int vertical_position = view_area.Y;
            this.Width = view_area.Width;
            this.Height = view_area.Height;

            foreach (RTF_View_Page page in this.view_pages)
            {
                InvalidatePageSize(page);
                page.InvalidatePage(g, view_area.X, vertical_position);
                vertical_position += page.TotalSubpagesHeight;
                view_height += page.TotalSubpagesHeight;

                if (cursor != null)
                    cursor.Invalidate();
            }

            return view_height;
        }

        public bool EnableCursor
        {
            get { return cursor_enabled; } set
            {
                if(value != cursor_enabled)
                {
                    if(value == true)
                    {
                        cursor_timer.Enabled = true;

                        ////RTF_View_Position pos = new RTF_View_Position(this);
                        ////pos.Invalidate(0);

                        cursor_timer.Start();
                    }
                    else
                    {
                        cursor_timer.Enabled = false;
                    }
                    cursor_enabled = value;
                }
            }
        }

        public int Lenght { get; internal set; }

        static bool cursor_shown = false;

        public void CursorTimer_Tick(object sender, EventArgs e)
        {
            if(!cursor_shown)
            {
                cursor_shown = true;
            }
            else
            {
                cursor_shown = false;
            }
//            update_cursor_only = true;
            this.Invalidate();
        }
        private void PreppareSequence(Page page, bool split2pages)
        {
            foreach (RichObject rich_obj in page.sequence.objects)
            {
                CommonViewObject view_obj = null;
                switch (rich_obj.type)
                {
                    case RichObject.Type.Paragraph:
                        view_obj = new View_Paragraph(this);
                        break;
                    case RichObject.Type.Table:
                        view_obj = new View_Table(this);
                        break;
                    case RichObject.Type.Picture:
                        view_obj = new View_Picture(this);
                        break;
                }
            }
        }

        private void PrepareDocumentFonts()
        {
            int idx = 0;

            if (document.font_list == null)
                document.font_list = new List<RFont>();
            if (document.font_list.Count == 0)
            {
                RFont f = new RFont();
                f.FontName = "Arial";
                f.font_id = 0;
                f.family = RFont.Family.Rroman;
                fonts = new RFont[1];
                fonts[0] = f;
            }
            else
            {
                fonts = new RFont[document.font_list.Count];

                idx = 0;

                this.Lenght = 0;
                foreach (RFont f in document.font_list)
                {
                    fonts[idx++] = f;
                }
            }
        }

        /// <summary>
        /// Prepare fonts and colors and split paragraphs to runs
        /// </summary>
        public void PrepareView()
        {
            using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                if (document.pages != null)
                {
                    PrepareDocumentFonts();


                    // --------  Split pargraphs to runs   -------- 
                    foreach (Page page in document.pages)
                    {
                        RTF_View_Page view_page = new RTF_View_Page(page);
                        view_page.PreparePage(g, this, ClientSize);
                        this.Lenght += view_page.Length;
                        this.view_pages.Add(view_page);
                    }
                }
                if (this.view_pages.Count == 0)
                {
                    // Create an empty RTF page
                    Page page = new Page();
                    page.page_width = 256;
                    page.page_heigh = 512;
                    RTF_View_Page view_page = new RTF_View_Page(page);
                    view_page.PreparePage(g, this, ClientSize);
                    this.Lenght += view_page.Length;
                    this.view_pages.Add(view_page);

                }
            }
        }

        internal Font GetFont(RunFormat rf)
        {
            FontStyle style =
                (rf.bold ? FontStyle.Bold : FontStyle.Regular) |
                (rf.italic ? FontStyle.Italic : FontStyle.Regular) |
                (rf.underline ? FontStyle.Underline : FontStyle.Regular);

            string font_name;

            if(fonts == null)
            {
                fonts = new RFont[0];
            }

            if (fonts.Length <= rf.font_idx)
                font_name = "Arial";
            else
                font_name = fonts[rf.font_idx].FontName;

            float scale = this.Scaler == 0 ? 1 : this.Scaler;
            return new Font(font_name, rf.font_size / 2 * scale, style, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        }

        internal void Add(CommonViewObject rtf_object)
        {
            this.view_objects.Add(rtf_object);
        }

        internal void LoadDocument(RichDocument document, Rectangle clientArea)
        {
            this.document = document;
            PrepareView();
        }
        internal void LoadFile(FileStream file)
        {
            using (RTF_DocumentParser parser = new RTF_DocumentParser())
            {
                parser.Load(file);
                this.document = parser.Document;
            }
            PrepareView();
            using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bmp))
                full_document_height = InvalidateView(g, ClientRectangle);
            AutoScrollMinSize = new Size(this.ClientRectangle.Width, full_document_height);
        }

        internal void LoadData(byte[] rtf_data)
        {
            using (RTF_DocumentParser parser = new RTF_DocumentParser())
            {
                parser.Load(rtf_data);
                this.document = parser.Document;
            }
            PrepareView();
        }

        internal void LoadText(string Text)
        {

            using (RTF_DocumentParser parser = new RTF_DocumentParser())
            {
                if (Text != null && !Text.StartsWith(@"{\rtf"))
                    Text = @"{\rtf " + Text + "}";

                parser.Load(Text);
                this.document = parser.Document;
            }
            PrepareView();
        }

        static float DpiX = 0;

        public static void FixDPI(int dpi)
        {
            DpiX = dpi;
        }

        public static int Twips2Pixels(long twips)
        {
            return (int)(((double)twips * DpiX) / 1440.0);
        }

        public static int Twips2Pixels(int twips)
        {
            return (int)(((double)twips * DpiX) / 1440.0);
        }


        /// <summary>
        /// 
        /// </summary>
        internal int SelectionHeight(int textStart, int textLength)
        {
            int height = 0;
            RTF_View_Position begin = new RTF_View_Position(this);
            RTF_View_Position end = new RTF_View_Position(this);

            begin.Invalidate(textStart);
            end.Invalidate(textStart + textLength);

            height = end.position_rtf_object.Bottom - begin.position_rtf_object.Top;

            return height;
        }

        public void FormatRange(int start, int length, int view_height, out int chars_fit, out int height)
        {
            chars_fit = 0;
            height = 0;

            int StartPostion = start;

            int range_border = 0;
            bool skip = false;

            using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bmp))
                foreach (RTF_View_Page vpage in this.view_pages)
                {
                    foreach (CommonViewObject robj in vpage.view_objects)
                    {
                        if (range_border < StartPostion)
                        {
                            range_border += robj.Length;
                            skip = true;
                            continue;
                        }
                        else
                            skip = false;

                        if (skip)
                            continue;
#if true
                        if (robj is View_Paragraph)
                        {
                            View_Paragraph par = robj as View_Paragraph;

                            if (view_height >= height + par.Height)
                            {
                                chars_fit += robj.Length;
                                range_border += robj.Length;
                                height += robj.Height;
                            }
                            else
                            {
                                foreach (View_Paragraph.ParagraphLine line in par.paragraph_lines)
                                {
                                    if (view_height < height + line.height)
                                    {
                                        return;
                                    }
                                    height += (int)line.height;
                                    chars_fit += line.Length;
                                    range_border += line.lenght;
                                }
                                chars_fit++;
                            }
                        }
                        else
#endif
                        {
                            if (view_height < height + robj.Height)
                            {
                                break;
                            }
                            chars_fit += robj.Length;
                            range_border += robj.Length;
                            height += robj.Height;
                        }
                    }
                    range_border++;
                    chars_fit++;
                    if (!skip)
                        break;
                }
        }

        private void CreateDocumentFromView()
        {
            document.pages.Clear();
            document.color_list = new List<Color>();
            foreach(RTF_View_Page view_page in view_pages)
            {
                Page doc_page = new Page();
                doc_page.sequence.objects = new List<RichObject>();
                doc_page.margin_top = view_page.Margins.Top;
                foreach (RTF_View_Subpage sub_page in view_page.Subpages)
                {
                    foreach(CommonViewObject rtf_object in sub_page.subpage_objects)
                    {
                        if (rtf_object is View_Paragraph)
                        {
                            View_Paragraph par = rtf_object as View_Paragraph;

                            RichObject rti_par = new RichObject();
                            rti_par.type = RichObject.Type.Paragraph;
                            rti_par.pargraph = new Paragraph();
                            rti_par.pargraph.runs = new List<Run>();

                            foreach(View_Paragraph.RunItem run in par.paragraph_runs)
                            {
                                Run r = new Run();
                                r.text = run.text;
                                r.format.bold = run.font.Bold;
                                rti_par.pargraph.runs.Add(r);
                            }

                            doc_page.sequence.objects.Add(rti_par);
                        }
                        else
                            throw new Exception(rtf_object.GetType().ToString());
                    }
                }
                //doc_page.sequence;
                document.pages.Add(doc_page);
            }
        }

        internal void Save(FileStream file)
        {
            CreateDocumentFromView();
            using (RTF_DocumentSaver writer = new RTF_DocumentSaver(document))
                writer.Save(file);
        }

        internal abstract class CommonViewObject : IDisposable
        {
            private Rectangle box = new Rectangle();

            internal int Left { get { return box.X; } set { box.X = value; } }
            internal int Top
            { get { return box.Y; } set
                {
                    box.Y = value;
                }
            }
            internal int Bottom { get { return box.Bottom; } }
            internal int Height { get { return box.Height; } set { box.Height = value; } }
            public float Width
            {
                get { return box.Width; }
                set { box.Width = (int)value; }
            }

            public int Length { get; internal set; }

            //public int Length { get { throw new Exception("Paragraph length not calculated"); /* return lenght; */} }

            internal RTF_View view;

            internal void DrawBorder(Graphics g, int left, int top, int width, int height, Column col)
            {
                Pen pen = new Pen(Color.Black);

                if (col.border_top.width != 0)
                {
                    pen.Width = Twips2Pixels((int)col.border_top.width);
                    pen.Color = col.border_top.color;
                    g.DrawLine(pen, left, top, left + width, top);
                }
                if (col.border_left.width != 0)
                {
                    pen.Width = Twips2Pixels((int)col.border_left.width);
                    pen.Color = col.border_left.color;
                    g.DrawLine(pen, left, top, left, top + height);
                }
                if (col.border_bottom.width != 0)
                {
                    pen.Width = Twips2Pixels((int)col.border_bottom.width);
                    pen.Color = col.border_bottom.color;
                    g.DrawLine(pen, left, top + height, left + width, top + height);
                }
                if (col.border_right.width != 0)
                {
                    pen.Width = Twips2Pixels((int)col.border_right.width);
                    pen.Color = col.border_right.color;
                    g.DrawLine(pen, left + width, top, left + width, top + height);
                }
            }

            internal abstract void SpitToRuns(RichObject parser_object);
            internal abstract int Prepare(Graphics g);
            internal abstract int Paint(Graphics g, PointF pos);
            internal abstract void UpdatePads(TableRow row, int top, int pos_x, int dx);

            //      internal abstract int FindString(string findWhat, out int line, out int position);
            public abstract void Dispose();
            internal abstract void InsertText(int position_within_object, string text);
        }

        /// <summary>
        /// Draw object 
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            if ((this.HelpersMode & VisaulHelpers.DoNotClearPageBackground) == 0)
              e.Graphics.FillRectangle(Brushes.DarkGray, e.ClipRectangle); // Clear editor background

            DpiX = e.Graphics.DpiX;

            //      e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            e.Graphics.ScaleTransform(ViewScale, ViewScale);

            PointF pos = new PointF(0, top_line);

            foreach (RTF_View_Page page in this.view_pages)
            {
                //if (page.VerticalPosition > top_line + this.ClientSize.Height)
                //{
                //    // Page is below of the view area - finish drawing
                //    break;
                //}

                //int debug_gap = page.VerticalPosition - top_line;

                //if (page.VerticalPosition + page.TotalSubpagesHeight < top_line)
                //{
                //    // Page is above of view area
                //    continue;
                //}

                PaintEventArgs pea = new PaintEventArgs(e.Graphics, e.ClipRectangle);

                if (!page.ExcludeMarings)
                    pos.X = (this.Width - page.size.Width) / 2;

                page.OnPaint(e, pos);
                if (!page.ExcludeMarings)
                    pos.Y += RTF_View_Page.top_gap;
            }

            if (cursor != null && cursor_shown)
            {
                cursor.OnPaint(e, pos, true);
            }

            //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(20, 255, 0, 0)), e.ClipRectangle);
        }


        internal void DoPaint(PaintEventArgs e)
        {
            OnPaint(e);
        }

        internal void CreateDocument()
        {
            RichDocument doc = new RichDocument();
            doc.pages = new List<Page>();
            doc.paper_width = 11905; // 210 mm in twips
            doc.paper_height = 16837; // 297 mm in twips

            Page page = new Page();
            page.sequence = new RichObjectSequence();

            //page.page_heigh = 300;
            //page.page_width = 500;
            page.margin_left = 10;
            page.margin_right = 10;

            RichObject  rtf_object = new RichObject();
            rtf_object.type = RichObject.Type.Paragraph;
            rtf_object.pargraph = new Paragraph();
            rtf_object.pargraph.format.align = ParagraphFormat.HorizontalAlign.Left;
            page.sequence.objects = new List<RichObject>();
            page.sequence.objects.Add(rtf_object);
            doc.pages.Add(page);

            LoadDocument(doc, ClientRectangle);
        }
    }
}
