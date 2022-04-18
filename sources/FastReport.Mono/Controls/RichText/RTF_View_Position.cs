using FastReport.RichTextParser;
using System;

namespace RichTextParser.RTF
{
    class RTF_View_Position
    {
        private int position = 0;
        private FastReport.RichTextParser.View_Paragraph.ParagraphLine line;
        internal RTF_View view;
        internal RTF_View_Page position_page = null;
        internal RTF_View_Subpage position_subpage = null;
        internal RTF_View.CommonViewObject position_rtf_object = null;
        internal int position_within_object = 0;

        // These moved from cursor object
        int position_in_run = 0;
        int position_in_line = 0;
        FastReport.RichTextParser.View_Paragraph.RunItem run = null;

        public int Position { get { return position; } }
        public FastReport.RichTextParser.View_Paragraph.ParagraphLine Line { get { return line; } set { line = value; } }
        public FastReport.RichTextParser.View_Paragraph.RunItem Run { get { return run; } }
        public int InRunPosition { get { return position_in_run; } }

        private void LookPositionInRun(int position)
        {
            Invalidate(position);
            position_in_run = position_within_object;

            if(position_rtf_object is FastReport.RichTextParser.View_Paragraph)
            {
            FastReport.RichTextParser.View_Paragraph par = position_rtf_object as FastReport.RichTextParser.View_Paragraph;
            foreach (FastReport.RichTextParser.View_Paragraph.ParagraphLine line in par.paragraph_lines)
            {
                if (position_in_run > line.Length)
                {
                    position_in_run -= line.lenght;
                    continue;
                }

                    Line = line;
                    run = null;
                foreach (FastReport.RichTextParser.View_Paragraph.RunItem r in line.runs)
                {
                    if (position_in_run > r.text.Length)
                    {
                        position_in_run -= r.text.Length;
                        continue;
                    }
                    run = r;
                    //                    cursor_rect.Y = (int)(run.Location.Y + this.Page.view_box.Top);
                    Line = line;
                    return;
                }
                break;
                }
            }
            else if (position_rtf_object is FastReport.RichTextParser.View_Table)
            {
                FastReport.RichTextParser.View_Table table = position_rtf_object as FastReport.RichTextParser.View_Table;
            }
            else if (position_rtf_object is FastReport.RichTextParser.View_Picture)
            {
                FastReport.RichTextParser.View_Picture picture = position_rtf_object as FastReport.RichTextParser.View_Picture;
            }
        }
        private void CalculatePositionInLine(FastReport.RichTextParser.View_Paragraph par)
        {
            position_in_line = position_within_object;
            foreach (FastReport.RichTextParser.View_Paragraph.ParagraphLine line in par.paragraph_lines)
            {
                if (position_in_line > line.Length)
                {
                    position_in_line -= line.lenght;
                    continue;
                }
                Line = line;
                break;
            }
        }

        internal void Invalidate(int position)
        {
            this.position = position;

            long i = position;

            if (i == view.Lenght)
            {
                // Cursor in a last position of the document
                if (view.Pages.Count == 0)
                {
                    throw new Exception("View page not declared");
                }
                position_page = view.Pages[view.Pages.Count - 1];
                if (position_page.Subpages.Count == 0)
                {
                    position_subpage = new RTF_View_Subpage(this.position_page);
                    position_page.Subpages.Add(position_subpage);
                }
                else
                {
                    position_subpage = position_page.Subpages[position_page.Subpages.Count - 1];
                }
                if (position_subpage.subpage_objects.Count == 0)
                {
                    position_rtf_object = new FastReport.RichTextParser.View_Paragraph(this.view);
                    position_rtf_object.Width = position_page.Width;
                    position_rtf_object.Top = position_page.ViewBox.Top;
                    position_subpage.subpage_objects.Add(position_rtf_object);
                    position_page.view_objects.Add(position_rtf_object);
                    view.Add(position_rtf_object);
                    position_within_object = 0;
                }
                else
                {
                    position_rtf_object = position_subpage.subpage_objects[position_subpage.subpage_objects.Count - 1];
                    position_within_object = position_rtf_object.Length;
                }
                return;
            }
            else foreach (RTF_View_Page page in view.Pages)
                {
                    if (i > page.Length)
                    {
                        i -= page.Length;
                        continue;
                    }
                    position_page = page;
                    foreach (RTF_View_Subpage subpage in page.Subpages)
                    {
                        if (i > subpage.Length)
                        {
                            i -= subpage.Length;
                            continue;
                        }
                        position_subpage = subpage;
                        foreach (RTF_View.CommonViewObject rtf_obj in subpage.subpage_objects)
                        {
                            if (i > rtf_obj.Length)
                            {
                                i -= rtf_obj.Length;
                                i--;
                                continue;
                            }
                            position_rtf_object = rtf_obj;
                            position_within_object = (int)i;
                            return;
                        }
                    }
               ;
                }
            throw new Exception("Position below document");
        }

        internal bool StepEndOfLine()
        {
            Invalidate(Position);
            position_in_run = -position_within_object;

            FastReport.RichTextParser.View_Paragraph par = position_rtf_object as FastReport.RichTextParser.View_Paragraph;
            foreach (FastReport.RichTextParser.View_Paragraph.ParagraphLine line in par.paragraph_lines)
            {
                if (position_in_run > line.Length)
                {
                    position_in_run -= line.lenght;
                    position_within_object += line.Length;
                    continue;
                }
                foreach (FastReport.RichTextParser.View_Paragraph.RunItem r in line.runs)
                {
                    position_in_run -= r.text.Length;
                    position_within_object += r.text.Length;
                    run = r; 
                }
                this.line = line;
                position -= position_in_run;
                position_in_run = run.text.Length;
                return true;
            }
            return false;
        }

        internal bool StepStartOfLine()
        {
            Invalidate(Position);
            position_in_run = position_within_object;

            FastReport.RichTextParser.View_Paragraph par = position_rtf_object as FastReport.RichTextParser.View_Paragraph;
            foreach (FastReport.RichTextParser.View_Paragraph.ParagraphLine line in par.paragraph_lines)
            {
                if (position_in_run > line.Length)
                {
                    position_in_run -= line.lenght;
                    continue;
                }
                this.line = line;
                position -= position_in_run;
                position_in_run = 0;
                return true;
            }
            return false;
        }

        internal bool SetToNextWord()
        {
            FastReport.RichTextParser.View_Paragraph par = position_rtf_object as FastReport.RichTextParser.View_Paragraph;
            if (this.run != null)
            {
                int run_index = par.paragraph_runs.IndexOf(this.run);
                for (bool act = true; act;)
                {
                    int term_pos = run.text.Substring(position_in_run).IndexOfAny(new char[] { ' ', '\t' });
                    if (term_pos == 0)
                    {
                        if (position_in_run < run.text.Length)
                        {
                            // Skip spaces here
                            position_in_run++;
                            position++;
                            continue;
                        }
                        else
                            throw new NotImplementedException("WTF?");
                    }
                    else
                        if (term_pos > position_in_run)
                        return Move(term_pos - position_in_run);

                    if (run_index < par.paragraph_runs.Count - 1)
                    {
                        this.run = par.paragraph_runs[++run_index];
                    }
                    else
                    {
                        throw new NotImplementedException("Crl + Right");
                    }
                    return true;
                }
            }
            throw new NotImplementedException("Crl + Right");
        }

        internal bool SetToPrevWord()
        {
            FastReport.RichTextParser.View_Paragraph par = this.position_rtf_object as FastReport.RichTextParser.View_Paragraph;
            if (this.run != null)
            {
                int run_index = par.paragraph_runs.IndexOf(this.run);
                for (bool act = true; act;)
                {
                    int term_pos = run.text.Substring(0, position_in_run).LastIndexOfAny(new char[] { ' ', '\t' });
                    if (term_pos > -1 && term_pos < position_in_run)
                        return Move(term_pos - position_in_run);

                    if (run_index > 0)
                    {
                        this.run = par.paragraph_runs[--run_index];
                    }
                    else
                    {
                        throw new NotImplementedException("Crl + Right");
                    }
                    return true;
                }
            }
            throw new NotImplementedException("Crl + Right");
        }


        internal bool StepDown()
        {
            FastReport.RichTextParser.View_Paragraph par = position_rtf_object as FastReport.RichTextParser.View_Paragraph;
            if (par == null)
                return false;
            int prev_position_in_line = position_in_line;
            CalculatePositionInLine(par);

            if (prev_position_in_line > position_in_line)
            {
                position_in_line = prev_position_in_line;
            }
            else
            {
                prev_position_in_line = position_in_line;
            }

            int position_diff = Line.Length - position_in_line;
            int line_idx = par.paragraph_lines.IndexOf(Line);
            if (line_idx < par.paragraph_lines.Count - 1)
            {
                Line = par.paragraph_lines[++line_idx];
                if (Line.Length < position_in_line)
                {
                    position_in_line = Line.Length;
                }
                position_diff += position_in_line;
                Move(position_diff);
            }
            else
            {
                int par_idx = this.position_subpage.subpage_objects.IndexOf(par);
                if (par_idx < this.position_subpage.subpage_objects.Count - 1)
                {
                    position_diff++;
                    par = this.position_subpage.subpage_objects[++par_idx] as FastReport.RichTextParser.View_Paragraph;
                    position_rtf_object = par;
                    Line = par.paragraph_lines[0];
                    if (Line.Length < position_in_line)
                    {
                        position_in_line = Line.Length;
                    }
                    position_diff += position_in_line;
                    Move(position_diff);
                }
                else
                {
                    // Last line of last paragraph - do nothing
                }
            }
            return true;
        }

        internal bool StepUp()
        {
            FastReport.RichTextParser.View_Paragraph par = position_rtf_object as FastReport.RichTextParser.View_Paragraph;
            if (par == null)
                return false;
            int prev_position_in_line = position_in_line;
            CalculatePositionInLine(par);

            if (prev_position_in_line > position_in_line)
            {
                position_in_line = prev_position_in_line;
            }
            else
            {
                prev_position_in_line = position_in_line;
            }

            int position_diff = -position_in_line;
            int line_idx = par.paragraph_lines.IndexOf(Line);
            if (line_idx > 0)
            {
                Line = par.paragraph_lines[--line_idx];
                if (Line.Length < position_in_line)
                {
                    position_in_line = Line.Length;
                }
                position_diff -= Line.Length - position_in_line;
                Move(position_diff);
            }
            else
            {
                int par_idx = this.position_subpage.subpage_objects.IndexOf(par);
                if (par_idx > 0)
                {
                    position_diff--;
                    par = this.position_subpage.subpage_objects[--par_idx] as FastReport.RichTextParser.View_Paragraph;
                    position_rtf_object = par;
                    Line = par.paragraph_lines[par.paragraph_lines.Count - 1];
                    if (Line.Length < position_in_line)
                    {
                        position_in_line = Line.Length;
                    }
                    position_diff -= Line.Length - position_in_line;
                    Move(position_diff);
                }
                else
                {
                    // First line of first paragraph - do nothing
                }
            }
            return true;
        }

        public bool Move(int step_count)
        {
            int position = Position + step_count;
            if (position >= 0 && position <= this.view.Lenght)
            {
                LookPositionInRun(position);
                this.Invalidate(position);
                return true;
            }
            return false;
        }


        public RTF_View_Position(RTF_View view)
        {
            this.view = view;
        }
    }
}
