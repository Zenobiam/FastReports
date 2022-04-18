using RichTextParser.RTF;
using System;
using System.Windows.Forms;

namespace FastReport.RichTextParser
{
    internal partial class RTF_View
    {
        internal void InsertText(string text)
        {
            if (cursor.Page == null)
                cursor.Invalidate(); ;
            {
                cursor.Page.InsertText(cursor.Location, text);
                this.Lenght += text.Length;
                cursor.StepRight();
                this.Invalidate();
            }
        }

        private void PressBackspaceKey()
        {
            if (cursor.Location.position_rtf_object is View_Paragraph)
            {
                View_Paragraph par = cursor.Location.position_rtf_object as View_Paragraph;

                if (cursor.Location.position_within_object > 0)
                {
                    cursor.StepLeft();
                    par.RemoveText(cursor.Location.position_within_object, 1);
                    cursor.Location.position_subpage.Length--;
                    cursor.Page.Length--;
                    this.Lenght--;
                    this.Invalidate();
                }
                else
                {
                    int index = cursor.Location.position_subpage.subpage_objects.IndexOf(par);
                    if (index > 0)
                    {
                        CommonViewObject prev_object = cursor.Location.position_subpage.subpage_objects[--index];
                        if (cursor.Location.position_rtf_object is View_Paragraph)
                        {
                            View_Paragraph select_par = prev_object as View_Paragraph;
                            cursor.StepLeft();
                            foreach (View_Paragraph.RunItem run in par.paragraph_runs)
                            {
                                select_par.paragraph_runs.Add(run);
                            }
                            cursor.Location.position_subpage.subpage_objects.Remove(par);
                            this.Lenght--;
                            this.Invalidate();
                        }
                    }
                    else
                    {
                        int subpage_index = cursor.Page.Subpages.IndexOf(cursor.Location.position_subpage);
                        if (subpage_index > cursor.Page.Subpages.Count - 1)
                        {

                        }
                        else
                        {
                            int page_index = view_pages.IndexOf(cursor.Location.position_page);
                            if (page_index > view_pages.Count - 1)
                            {

                            }
                            else
                            {
                                // First paragraph on first page - do nothing
                            }
                        }
                    }
                }
            }
        }

        private void PressEnterKey()
        {
            if (cursor.Location.position_rtf_object is View_Paragraph)
            {
                View_Paragraph par = cursor.Location.position_rtf_object as View_Paragraph;
                int index = cursor.Location.position_subpage.subpage_objects.IndexOf(par);
                int dispose = cursor.Location.position_within_object;

                View_Paragraph new_paragraph = par.SplitIntoTwoParagraphs(dispose);
                this.view_objects.Insert(++index, new_paragraph);

                cursor.Location.position_subpage.subpage_objects.Insert(index, new_paragraph);
                cursor.Location.position_subpage.Length++;

                int onpage_idx = cursor.Page.view_objects.IndexOf(par);
                if (onpage_idx < 0)
                    throw new Exception("Page object error");
                cursor.Page.view_objects.Insert(++onpage_idx, new_paragraph);
                cursor.Page.Length++;
                this.Lenght++;
            }
            using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bmp))
                full_document_height = InvalidateView(g, ClientRectangle);
            cursor.StepRight();
        }

        private void CopyFromClipboard()
        {
            RichDocument clipboard_context;
            string text = Clipboard.GetText(TextDataFormat.Rtf);
            using (RTF_DocumentParser parser = new RTF_DocumentParser())
            {
                parser.Load(text);
                clipboard_context = parser.Document;
#if true
                if (clipboard_context.pages != null)
                    foreach (Page page in clipboard_context.pages)
                    {
                        foreach (RichObject rtf_item in page.sequence.objects)
                        {
                            CommonViewObject view_obj = null;

                            switch (rtf_item.type)
                            {
                                case RichObject.Type.Paragraph:
                                    Paragraph par = rtf_item.pargraph;

                                    /// Debugging and test
                                    /// 
                                    view_obj = new View_Paragraph(this);


                                    break;
                                case RichObject.Type.Picture:
                                    Picture pic = rtf_item.picture;
                                    view_obj = new View_Picture(this);
                                    break;

                                case RichObject.Type.Table:
                                    Table table = rtf_item.table;
                                    view_obj = new View_Table(this);
                                    break;
                            }

                            if (view_obj != null)
                            {
                                view_obj.Length = (int)rtf_item.size;
                                //                                this.Length += view_obj.Length;
                                view_obj.SpitToRuns(rtf_item);
                                //                                int height = view_obj.Prepare(g);
                                //                                total_height += height;
                                view_objects.Add(view_obj);
                            }
                        }
                    }
#else
#endif
            }
            //full_document_height = InvalidateView(ClientRectangle);
        }
        #region User control functions
        protected override bool ProcessDialogKey(Keys keyData)
        {
            return false;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            Scaler = 1;

            //      this.KeyPress += new KeyPressEventHandler(PanelKeyPressEventHandler);

            cursor = new RTF_View_Cursor(this);
            cursor_timer.Interval = 500;
            cursor_timer.Tick += CursorTimer_Tick;

            this.KeyDown += new KeyEventHandler(PanelKeyEventHandler);
            this.Scroll += new ScrollEventHandler(PanelVerticaEventHandler);
            this.KeyPress += new KeyPressEventHandler(PanelKeyPressEventHandler);

            //            view_rect = (Parent as TabPageRTF).ClientRectangle;
            //view_rect = Parent.ClientRectangle;

            Dock = DockStyle.Fill;
            AutoScroll = true;
        }

        internal void PanelKeyPressEventHandler(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= 32 && e.KeyChar < 126)
            {
#if COPY_OF_CODE_EDITOR
                if (selection.Active)
                {
                    undoredo.AddTextBlock(selection.start_line, selection.start_col, selection.GetText());
                    selection.DeleteSelectedText();
                }
                undoredo.AddText(cursor.X, LineY, e.KeyChar.ToString());
                PutCharacter(e.KeyChar.ToString());
                CurrentLine.Invalidate();
                Invalidate();
#else
                InsertText(e.KeyChar.ToString());
                //                InvalidateView(view_rect);
                e.Handled = true;
#endif
            }
        }

        public void PanelKeyEventHandler(Object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.PageDown:
                    if (top_line + this.ClientSize.Height < this.AutoScrollMinSize.Height)
                    {
                        top_line += this.ClientSize.Height;
                        if (top_line > this.AutoScrollMinSize.Height)
                            top_line = this.AutoScrollMinSize.Height - this.ClientSize.Height;
                        if (top_line < 0)
                            top_line = 0;

                        this.Top = -top_line;
                        this.Refresh();
                    }
                    break;

                case Keys.PageUp:
                    if (top_line > 0)
                    {
                        top_line -= this.ClientSize.Height;
                        if (top_line < 0)
                            top_line = 0;
                        this.Top = -top_line;

                        this.Refresh();
                    }
                    break;

                case Keys.Enter:
                    break;

                default:
                    break;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            bool success;
            switch (e.KeyCode)
            {
                case Keys.Back:
                    PressBackspaceKey();
                    break;

                case Keys.Enter:
                    PressEnterKey();
                    break;

                case Keys.Right:
                    success = e.Control ? this.cursor.PressNextWord() : this.cursor.StepRight();
                    if (success)
                    {
                        cursor_shown = true;
                        Invalidate();
                    }
                    break;

                case Keys.Left:
                    success = e.Control ? this.cursor.PressPrevWord() : this.cursor.StepLeft();
                    if (success)
                    {
                        cursor_shown = true;
                        Invalidate();
                    }
                    break;

                case Keys.V:
                    if (e.Control)
                    {
                        CopyFromClipboard();
                    }
                    break;
                case Keys.Next:
                    if (top_line + 2 * this.Height < full_document_height)
                    {
                        top_line += this.Height;
                        this.Invalidate();
                    }
                    else
                    {
                        if (full_document_height - top_line < 2 * this.Height)
                        {
                            top_line = full_document_height - this.Height;
                            if (top_line < 0)
                                top_line = 0;
                            else
                                this.Invalidate();
                        }
                    }
                    break;
                case Keys.Prior:
                    if (top_line == 0)
                        break;
                    top_line -= this.Height;
                    if (top_line < 0)
                        top_line = 0;
                    this.Invalidate();
                    break;
                case Keys.Home:
                    cursor.StepStartOfLine();
                    this.Invalidate();
                    break;
                case Keys.End:
                    cursor.StepEndOfLine();
                    this.Invalidate();
                    break;
                case Keys.Down:
                    cursor.StepDown();
                    this.Invalidate();
                    break;
                case Keys.Up:
                    cursor.StepUp();
                    this.Invalidate();
                    break;
            }
        }

        private void PanelVerticaEventHandler(object sender, ScrollEventArgs e)
        {
            top_line = (e.NewValue);
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Focus();
        }
        #endregion

    }
}
