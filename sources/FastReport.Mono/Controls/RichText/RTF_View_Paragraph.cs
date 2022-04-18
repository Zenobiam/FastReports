using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FastReport.RichTextParser
{
    internal class View_Paragraph : RTF_View.CommonViewObject
    {
        internal class RunItem
        {
            private Run run;
            private RectangleF box;
            internal string text { get { return run.text; } }
            internal Color color { get { return run.format.color; } }
            internal RunFormat.ScriptType script_type { get { return run.format.script_type; } }

            internal float width { get { return box.Width; } set { box.Width = value; } }
            internal float X { get { return box.X; } set { box.X = value; } }
            //internal bool term;
            internal Font font;
            internal bool split;  // Run occupies several strings
            internal float Y { set { box.Y = value; } }

            internal SizeF Size
            {
                get { return box.Size; }
                set { box.Size = value; }
            }

            public PointF Location
            {
                get { return box.Location; }
                //set
                //{
                //  box.Location = value;
                //}
            }

            public float ScriptypeLocation
            {
                set { box = new RectangleF(new PointF(box.X, value), box.Size); }
            }

            public float Height { get { return box.Height; } set { box.Height = value; } }

            public float Width { get { return box.Width; } }

            public void InsertText(int position, string text)
            {
                run.text = run.text.Insert(position, text);
            }

            public void RemoveText(int postion, int count)
            {
                run.text = run.text.Remove(postion, count);
            }

            public RunItem(string text, RunItem run)
            {
                this.run = run.run;
                this.run.text = text;
                this.font = run.font;
                //this.term = last;
                this.Height = run.Height;
            }

            public RunItem(RTF_View view, Run run)
            {
                this.run = run;
                this.font = view.GetFont(run.format);
                if (run.format.script_type != RunFormat.ScriptType.PlainText)
                {
                    font = new Font(font.Name, font.Size / 1.3f,
                            font.Style, font.Unit,
                            font.GdiCharSet, font.GdiVerticalFont);
                }
            }

            internal void DrawString(Graphics g, PointF pos, SolidBrush brush, StringFormat sf)
            {
                float y = pos.Y + Location.Y;
                float x = pos.X + this.X;

#if false // Enable to debug run splitiing
                if (box.Height < 3)
                    g.FillRectangle(Brushes.RoyalBlue, new RectangleF(x, y + font.Height - 4, box.Width, 4));
                else
                if (box.Width < 3)
                    g.DrawRectangle(Pens.RoyalBlue, x, y, 10, box.Height);
                else
                if (split)
                    g.DrawRectangle(Pens.Cyan, x, y, box.Width, box.Height);
                else
                    g.DrawRectangle(Pens.Purple, x, y, box.Width, box.Height);
#endif
                g.DrawString(run.text, font, brush, x, y, sf);
            }

            internal RunItem SplitIntoTwoRuns(int dispose)
            {
                RunItem new_run = new RunItem(this.text.Substring(dispose), this);
                this.run.text = this.text.Substring(0, dispose);
                return new_run;
            }
        }

        internal void ShiftLinesDown(int top)
        {
#if true
#elif false
            this.Top = top;
#else
            foreach (ParagraphLine line in paragraph_lines)
            {
                line.Top = top;
                top += (int)line.height;
            }
#endif
        }

        internal class ParagraphLine
        {
            private float _width;
            internal List<RunItem> runs;
            internal float min_width;

            private float h;
            internal float height
            {
                set { h = value; }
                get { return h; }
            }

            internal int lenght;

            internal float Width { get { return _width; } }
            internal int Length { get { return lenght; } }

            internal int Top
            {
                set
                {
                    foreach (RunItem run in runs)
                        run.Y = value + run.Location.Y;
                }
            }

            internal void AddRun(RunItem run)
            {
                runs.Add(run);
                min_width += run.width;
                lenght += run.text.Length;
                _width += run.width;
            }

            internal ParagraphLine(float y)
            {
                runs = new List<RunItem>();
                _width = 0;
                min_width = 0;
                lenght = 0;
            }

            internal int FindSting(string findWhat)
            {
                int position = -1;
                int prev_size = 0;
                foreach (RunItem run in runs)
                {
                    if (run.text.Contains(findWhat))
                    {
                        position = prev_size + run.text.IndexOf(findWhat);
                        break;

                    }
                    prev_size += run.text.Length;
                }
                return position;
            }

            internal RunItem MeasureRun(float par_left, float par_width, Graphics g, RunItem run, StringFormat sf)
            {
                RunItem perenos = null;

                run.Size = g.MeasureString(run.text, run.font, run.Location, sf);
                if (run.Size.Height == 0)
                {
                    float h = g.MeasureString("fj", run.font, run.Location, sf).Height;
                    run.Size = new Size(0, (int)h);
                }
                height = height < run.Height ? run.Height : height;

                if (Width + run.Width >= par_width)
                {
                    perenos = TrySplitRun(par_width, g, run, sf);
                    if (perenos != null)
                    {
                        perenos.X = par_left;
                    }
                }
                else
                    AddRun(run);

                return perenos;
            }

            private RunItem TrySplitRun(float par_width, Graphics g, RunItem run, StringFormat sf)
            {
                SizeF box = new SizeF();
                RunItem perenos = null;

                StringBuilder word_builder = new StringBuilder();
                List<string> words = new List<string>();
                foreach (char ch in run.text)
                {
                    word_builder.Append(ch);
                    switch (ch)
                    {
                        case ' ':
                            words.Add(word_builder.ToString());
                            word_builder.Clear();
                            break;

                        case '-':
                            words.Add(word_builder.ToString());
                            word_builder.Clear();
                            break;
                    }
                }
                words.Add(word_builder.ToString());

                StringBuilder left = new StringBuilder();
                StringBuilder right = new StringBuilder();
                bool over = false;
                int counter = words.Count;
                float control_width = Width;

                foreach (string word in words)
                {
                    if (over)
                    {
                        right.Append(word);
                    }
                    else if (word.Length != 0)
                    {
                        box = g.MeasureString(word, run.font, zero_point, sf);
                        if (control_width + box.Width < par_width)
                        {
                            control_width += box.Width;
                            left.Append(word);
                        }
                        else
                        {
                            over = true;
                            right.Append(word);
                        }
                    }
                }

                string l = left.ToString();
                string r = right.ToString();

                if (left.Length > 0)
                {
                    perenos = new RunItem(l, run);
                    perenos.width = box.Width;
                    perenos.Height = box.Height;
                    run.split = true;
                    perenos.split = true;
                    perenos.X = run.Location.X;
                    AddRun(perenos);
                }

                if (right.Length > 0)
                {
                    perenos = new RunItem(r, run);
                    perenos.width = run.Width;
                    perenos.Height = box.Height;
                    run.split = true;
                    perenos.split = true;
                }
                else
                    perenos = null;

                _width = control_width;
                return perenos;
            }
        }

        internal List<RunItem> paragraph_runs;
        internal List<ParagraphLine> paragraph_lines;

        private ParagraphFormat format;
        private float _current_line_space;
        private float _current_left;
        private float _current_width;
        private RunItem list_tip = null;

        internal Column.VertAlign valign = Column.VertAlign.Top; // Vertical align used for table cells
        static PointF zero_point = new PointF(0, 0);

        internal override int Prepare(Graphics g)
        {
            if (this.Width == 0)
                return 0;

            SplitToLines(g);
            OrderHorizontally(g);
            return OrderVertically();
        }

        internal int OrderVertically()
        {
            float y = format.space_before / 10;

            foreach (ParagraphLine line in paragraph_lines)
            {
                if (format.line_spacing == 0)
                    _current_line_space = (int)line.height; // (line.height * 1.2); // Multiply 1.2 cause empty line in view box due to difference in view height calculation
                else
                {
                    int lnspc;

                    if (format.lnspcmult == ParagraphFormat.LnSpcMult.Multiply)
                        lnspc = (int)(format.line_spacing / 240f);
                    else
                        lnspc = RTF_View.Twips2Pixels(format.line_spacing);

                    _current_line_space = lnspc < 0 ? -lnspc : lnspc >= line.height ? lnspc : (int)line.height;
                }

                float max_hg = 0;
                foreach (RunItem run in line.runs)
                {
                    run.Y = y;
                    if (max_hg < run.Height)
                        max_hg = run.Height;
                }

                foreach (RunItem run in line.runs)
                {
                    switch (run.script_type)
                    {
                        case RunFormat.ScriptType.Subscript:
                            run.ScriptypeLocation = run.Height / 3f;
                            break;
                        case RunFormat.ScriptType.Superscript:
                            run.ScriptypeLocation = -run.Height / 3f;
                            break;
                    }
                    run.Y = run.Location.Y + (max_hg - run.Height) / 1.2f;
                    run.Height = max_hg;
                }

                y += _current_line_space;
            }

            Height = (int)(y + format.space_after / 10); // В целом удовлетворительно, но лучше улучшить

            return (int)Height;
        }

        internal void SplitToLines(Graphics g)
        {
            ParagraphLine line = new ParagraphLine(0);
            StringFormat sf = (StringFormat)StringFormat.GenericTypographic.Clone(); ;
            sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

            paragraph_lines.Clear();

            //_current_left = this.Left + format.left_indent;
            _current_left = this.Left + this.view.Left + format.left_indent;
            _current_width = this.Width - (format.left_indent + format.right_indent);

            PointF location = new PointF(_current_left + RTF_View.Twips2Pixels(format.first_line_indent), format.space_before);

            float current_line_height = 20; // TODO: Fix

            if (paragraph_runs.Count == 0)
            {
                line.height = 20; // TODO: fix it with height of font of previous paragraphe
                paragraph_lines.Add(line);
                return;
            }

            bool first_line = true; // false; //  

            if (this.list_tip != null)
            {
                list_tip.X = location.X;
                list_tip.Y = format.space_before / 10;
                list_tip.font = paragraph_runs[0].font;
            }

            float tab_width = 0;

            foreach (RunItem run in paragraph_runs)
            {
                if (run.text == "\t")
                {   // Tabulation
                    tab_width = RTF_View.Twips2Pixels((int)view.document.default_tab_width);
                    //          tab_width = (location.X + tab_width + 1) % tab_width;  // But why it does not work?
                    RunItem item = new RunItem("\t", run);
                    item.Size = new SizeF(tab_width, current_line_height);
                    item.X = location.X + tab_width;
                    line.AddRun(item);
                    location.X += item.Width;
                    continue;
                }

                if (run.text == "\r")
                {
                    if (line.height == 0)
                        line.height = current_line_height;
                    paragraph_lines.Add(line);
                    line = new ParagraphLine(0);
                    location.X = _current_left;
                    continue;
                }

                run.X = location.X;
                RunItem perenos = run;
                do
                {
                    RunItem prev_run = perenos;
                    if (first_line)
                    {
                        first_line = false;
                        int first_line_indent = (this.format.list_id == null) ? RTF_View.Twips2Pixels(format.first_line_indent) : 0;
                        if (format.align != ParagraphFormat.HorizontalAlign.Centered)
                        {
                            location.X = _current_left + first_line_indent + tab_width;
                            tab_width = 0;
                            run.X = location.X;
                        }
                        //                        perenos = line.MeasureRun(_current_left, _current_width - first_line_indent, g, perenos, sf);
                    }
                    //                    else
                    perenos = line.MeasureRun(location.X /*_current_left*/, _current_width, g, perenos, sf);

                    if (perenos == null)
                    {
                        prev_run.X = location.X;
                        location.X += prev_run.Width;
                        break;
                    }

                    if (line.runs.Count == 0)
                    {
                        // cannot split current run - it wider of paragraph
                        line.AddRun(perenos);
                        break;
                    }

                    paragraph_lines.Add(line);
                    line = new ParagraphLine(0);

                    location.X = _current_left;
                }
                while (perenos != null);
            }
            paragraph_lines.Add(line);
        }

        internal void OrderHorizontally(Graphics g)
        {
            //if (format.align == ParagraphFormat.HorizontalAlign.Left)
            //    return;

            float space_width = 10;
            float x = 0;
            float left = _current_left;
            bool first_line = true;
            float orig_space_width = space_width;
            int line_counter = paragraph_lines.Count;
            int tab_width = RTF_View.Twips2Pixels((int)view.document.default_tab_width);
            foreach (ParagraphLine ln in paragraph_lines)
            {
                float width = _current_width;
                line_counter--;
                if (first_line)
                {
                    first_line = false;
                    if (format.align != ParagraphFormat.HorizontalAlign.Centered && format.list_id == null)
                        left += RTF_View.Twips2Pixels(format.first_line_indent);
                    // Check if Paragraph started with tabulation
                    if(ln.runs.Count > 1)
                    {
                        RunItem r = ln.runs[0];
                        if (r.text == "\t")
                            left += tab_width;
                    }
                }
                else
                    left = _current_left;

                bool need_recalc = true;

                switch (format.align)
                {
                    case ParagraphFormat.HorizontalAlign.Centered:
                        x = left + (width - ln.Width) / 2;
                        break;

                    case ParagraphFormat.HorizontalAlign.Justified:
                    case ParagraphFormat.HorizontalAlign.Distributed:
                        x = left;
                        if (line_counter == 0)
                            break;

                        StringFormat sf = (StringFormat)StringFormat.GenericTypographic.Clone(); ;
                        //sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                        List<RunItem> runs = ln.runs;
                        ln.runs = new List<RunItem>();
                        float words_width = 0;
                        int space_count = 0;
                        char last_char = (char)0;

                        foreach (RunItem r in runs)
                        {
                            StringBuilder word_builder = new StringBuilder();
                            RunItem nri = null;
                            if (r.text == "\t")
                            {
                                tab_width = RTF_View.Twips2Pixels((int)view.document.default_tab_width);
                                width -= tab_width;
                                left += tab_width - r.Width;
                            }
                            else
                                foreach (char ch in r.text)
                                {
                                    word_builder.Append(ch);
                                    if (ch == ' ')
                                    {
                                        nri = new RunItem(word_builder.ToString(), r);
                                        nri.width = g.MeasureString(nri.text, r.font, r.Location, sf).Width;
                                        words_width += nri.width;
                                        ln.runs.Add(nri);
                                        word_builder.Clear();
                                        space_count++;
                                    }
                                    last_char = ch;
                                }
                            if (word_builder.Length > 0)
                            {
                                nri = new RunItem(word_builder.ToString(), r);
                                nri.width = g.MeasureString(nri.text, r.font, r.Location, sf).Width;
                                words_width += nri.width;
                                ln.runs.Add(nri);
                                word_builder.Clear();
                            }
                        }

                        if (last_char == ' ')
                            space_count--;

                        if (space_count != 0)
                        {
                            need_recalc = false;
                            space_width = (width - words_width) / space_count;
                            x = left;
                            foreach (RunItem r in ln.runs)
                            {
                                r.X = x;
                                x += r.width;
                                if (r.text.LastIndexOf(' ') != -1)
                                    x += space_width;
                            }
                        }
                        else
                            x = left + (width - ln.Width) / 2;
                        break;

                    case ParagraphFormat.HorizontalAlign.Left:
                        x = left;
                        break;

                    case ParagraphFormat.HorizontalAlign.Right:
                        x = left + width - ln.Width - format.right_indent;
                        break;
                }

                if (need_recalc)
                    for (int i = 0; i < ln.runs.Count; i++)
                    {
                        RunItem r = ln.runs[i];
                        r.X = x;
                        x += r.width; // + (r.term ? 0 : space_width);
                    }
            }
        }

        internal override void UpdatePads(TableRow row, int top, int pos_x, int dx)
        {
            if (row.default_pad_left != 0 && format.left_indent == 0)
                format.left_indent = RTF_View.Twips2Pixels(row.default_pad_left);

            if (row.default_pad_right != 0 && format.right_indent == 0)
                format.right_indent = RTF_View.Twips2Pixels(row.default_pad_right);

            Top = top;
            Left = pos_x;
            Width = dx;
        }

        internal override int Paint(Graphics g, PointF pos)
        {
            if ((this.view.HelpersMode & RTF_View.VisaulHelpers.Paragraphs) != 0)
            {
                g.DrawRectangle(Pens.LightPink, pos.X + this.Left, pos.Y, this.Width, this.Height != 0 ? Height : 3);  // Draw red frame around of paragraph
            }

            float par_H = pos.Y;
            SolidBrush brush = new SolidBrush(Color.Black);
            StringFormat sf = new StringFormat(StringFormat.GenericTypographic);

            int NotLastLine = paragraph_lines.Count;

            if (this.list_tip != null)
            {
                this.list_tip.DrawString(g, pos, brush, sf);
            }

            foreach (ParagraphLine ln in paragraph_lines)
            {
                --NotLastLine;

                foreach (RunItem r in ln.runs)
                {

                    brush.Color = r.color;
                    r.DrawString(g, pos, brush, sf);
                }
                par_H += (int)ln.height;
            }
            return (int)par_H;
        }

        public override void Dispose()
        {
            paragraph_lines = null;
            paragraph_runs = null;
        }

#if false
    internal override int FindString(string findWhat, out int line_num, out int position)
    {
      int abs_position = -1;
      int prev_size = 0;
      position = 0;
      line_num = 0;
      foreach (ParagraphLine line in paragraph_lines)
      {
        position = line.FindSting(findWhat);
        if (position >= 0)
        {
          line_num++;
          abs_position = prev_size + position;
          break;
        }
        line_num++;
        prev_size += line.Length;
      }
      return abs_position;
    }
#endif

        internal void SplitParagraph(Paragraph cell)
        {
            if (cell.runs != null)
            {
                int backcounter = cell.runs.Count;
                foreach (Run parser_run in cell.runs)
                {
                    --backcounter;
                    RunItem view_run = new RunItem(view, parser_run);
                    paragraph_runs.Add(view_run);
                }
            }
            this.format = cell.format;
            this.format.right_indent = RTF_View.Twips2Pixels(cell.format.right_indent);
            this.format.left_indent = RTF_View.Twips2Pixels(cell.format.left_indent);
            if (this.format.list_id != null)
            {
                this.list_tip = new RunItem(view, this.format.list_id[0]);
            }
        }

        internal override void SpitToRuns(RichObject parser_object)
        {
            this.format = parser_object.pargraph.format;
            SplitParagraph(parser_object.pargraph);
        }

        internal int RemoveText(int position, int count)
        {
            using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
            {
                foreach (RunItem run in this.paragraph_runs)
                {
                    if (count == 0)
                        break;
                    if (position < run.text.Length)
                    {
                        int chunk_len = run.text.Length - position;
                        if (chunk_len > count)
                            chunk_len = count;
                        run.RemoveText(position, chunk_len);
                        count -= chunk_len;
                        this.Length -= chunk_len;
                        position = 0;
                        continue;
                    }
                    position -= run.text.Length;
                }
                this.Prepare(g);
            }
            return count;
        }

        internal override void InsertText(int position_within_object, string text)
        {
            RunItem run = null;

            if (this.paragraph_runs.Count == 0)
            {
                Run r = new Run();
                r.text = text;
                r.format.font_size = 20;
                r.format.color = Color.Black;
                run = new RunItem(this.view, r);
                this.paragraph_runs.Add(run);
            }
            else foreach (RunItem r in this.paragraph_runs)
                {
                    int position_within_run = position_within_object;
                    if (position_within_run > r.text.Length)
                    {
                        position_within_run -= r.text.Length;
                        continue;
                    }

                    //if (position_within_run == run.text.Length)
                    //{

                    //}
                    //else
                    {
                        r.InsertText(position_within_run, text);
                        run = r;
                        break;
                    }
                }
            if (run != null) using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
                {
                    this.Prepare(g);
                    this.Length += text.Length;
                }
        }

        internal View_Paragraph(RTF_View parent)
        {
            view = parent;
            paragraph_runs = new List<RunItem>();
            paragraph_lines = new List<ParagraphLine>();
        }

        internal View_Paragraph SplitIntoTwoParagraphs(int dispose)
        {
            //return this;
            View_Paragraph new_paragraph = new View_Paragraph(this.view);
            new_paragraph.format = this.format;
            new_paragraph.valign = this.valign;
            new_paragraph._current_left = this._current_left;
            new_paragraph._current_line_space = this._current_line_space;
            new_paragraph._current_width = this._current_width;
            new_paragraph.Width = this.Width;
            new_paragraph.Left = this.Left;

            int remove_from = 0;

            foreach (RunItem r in this.paragraph_runs)
            {
                if (dispose > r.text.Length)
                {
                    dispose -= r.text.Length;
                    this.Length -= r.text.Length;
                    remove_from++;
                    continue;
                }


                if (dispose != 0)
                {
                    remove_from++;
                    RunItem new_run = r.SplitIntoTwoRuns(dispose);
                    //                    int run_idx = 
                    new_paragraph.paragraph_runs.Add(new_run);
                    this.Length -= new_run.text.Length;
                    new_paragraph.Length += new_run.text.Length;
                    dispose = 0;
                }
                else
                {
                    new_paragraph.paragraph_runs.Add(r);
                    new_paragraph.Length += r.text.Length;
                }
            }
            this.paragraph_runs.RemoveRange(remove_from, paragraph_runs.Count - remove_from);

            using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
            {
                this.Prepare(g);
                new_paragraph.Top = this.Bottom;
                new_paragraph.Prepare(g);
            }

            return new_paragraph;
        }
    }

}
