using System;
using System.Drawing;
using System.Windows.Forms;

namespace RichTextParser.RTF
{
    class RTF_View_Cursor
    {
        Rectangle cursor_rect = new Rectangle(10, 10, 2, 20);
        bool need_invalidate = true;
        RTF_View_Position view_pos;

        internal int Position { get { return Location.Position; } }
        internal RTF_View_Position Location { get { return view_pos; } }
        internal RTF_View_Page Page { get { return view_pos.position_page; } }

        internal void Invalidate()
        {
            view_pos.Move(0);
            need_invalidate = true;
        }

        internal void OnPaint(PaintEventArgs e, PointF pos, bool on)
        {
            float x = 0;
            float y = 0;
            //if (need_invalidate)
            {
                if (view_pos.position_rtf_object != null)
                {
                    if(view_pos.position_subpage != null)
                    {
                        x = view_pos.position_subpage.client_rect.Left;
                        y = view_pos.position_subpage.client_rect.Top;
                    }
                    if(view_pos.Run != null)
                    {
                        string s = view_pos.Run.text.Substring(0, view_pos.InRunPosition);
                        StringFormat sf = StringFormat.GenericTypographic.Clone() as StringFormat;
                        sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                        SizeF sz = e.Graphics.MeasureString(s, view_pos.Run.font, (int)Location.position_rtf_object.Width, sf);
                        cursor_rect.X = (int)(x + view_pos.Run.Location.X + sz.Width /* + pos.X */ );
                        cursor_rect.Y = (int)(y + view_pos.Run.Location.Y + Location.position_rtf_object.Top - pos.Y );
                        need_invalidate = false;
                    }
                    else
                    {
                        cursor_rect.X = (int)(x + Location.position_rtf_object.Left);
                        cursor_rect.Y = (int)(y + Location.position_rtf_object.Top - pos.Y);
                    }
                }
                need_invalidate = false;
            }

            e.Graphics.FillRectangle(Brushes.Black, cursor_rect);
        }

        internal bool PressNextWord()
        {
            need_invalidate = view_pos.SetToNextWord();
            return need_invalidate;
        }

        internal bool PressPrevWord()
        {
            need_invalidate = view_pos.SetToPrevWord();
            return need_invalidate;
        }

        public bool StepLeft()
        {
            need_invalidate = view_pos.Move(-1);
            return need_invalidate;
        }

        public bool StepRight()
        {
            need_invalidate = view_pos.Move(1);
            return need_invalidate;
        }

        internal void StepDown()
        {
            need_invalidate = view_pos.StepDown();
        }

        internal void StepUp()
        {
            need_invalidate = view_pos.StepUp();
        }

        internal bool StepStartOfLine()
        {
            need_invalidate = view_pos.StepStartOfLine();
            return need_invalidate;
        }

        internal bool StepEndOfLine()
        {
            need_invalidate = view_pos.StepEndOfLine();
            return need_invalidate;
        }
        public RTF_View_Cursor(FastReport.RichTextParser.RTF_View view)
        {
            view_pos = new RTF_View_Position(view);
//            view_pos.Move(0);
            need_invalidate = true;
        }

    }
}
