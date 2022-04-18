using FastReport.RichTextParser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RichTextParser.RTF
{
    internal class RTF_View_Subpage
    {
        private RTF_View_Page page;
        //internal int height;
        internal RectangleF client_rect;
        internal int Top;
        internal List<RTF_View.CommonViewObject> subpage_objects = new List<RTF_View.CommonViewObject>();
        public long Length { get; internal set; }

        private RectangleF DrawPageBackground(PaintEventArgs e, PointF pos)
        {
            RectangleF page_rect;
            RectangleF screen_box = page.ViewBox;
            Brush back_brush = new SolidBrush(Color.FromArgb(255, Color.White));
            Brush border_brush = new SolidBrush(Color.Black);

            if (page.ExcludeMarings)
            {
                if (page.DoNotClearPageBackground)
                {
                    page_rect = new RectangleF(e.ClipRectangle.X, pos.Y, page.Width, page.size.Height);
                    e.Graphics.FillRectangle(back_brush, page_rect);
                    // Draw page view area
                    if (page.ShowPageMargibs)
                        e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.DarkBlue)), screen_box.X, screen_box.Y, screen_box.Width, screen_box.Height);
                }
                page_rect = new RectangleF(pos.X, pos.Y, page.Width, page.size.Height);
            }
            else
            {
                page_rect = new RectangleF(pos.X, this.Top + pos.Y - page.Margins.Top, page.size.Width, page.size.Height);
                if (page.DoNotClearPageBackground)
                {
                    e.Graphics.FillRectangle(border_brush, pos.X + 3, page_rect.Bottom, page_rect.Width, 3);
                    e.Graphics.FillRectangle(border_brush, page_rect.Right, page_rect.Y + 2, 3, page_rect.Height);
                    e.Graphics.FillRectangle(back_brush, page_rect);
                    // Draw page view area
                    if (page.ShowPageMargibs)
                    {
                        screen_box.Offset(pos.X, pos.Y - page.Margins.Top); /// - top_gap);
                        e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.DarkBlue)), screen_box.X, screen_box.Y, screen_box.Width, screen_box.Height);
                    }
                }
            }

            return page_rect;
        }

        internal void OnPaint(PaintEventArgs e, PointF pos)
        {
            client_rect = DrawPageBackground(e, pos);

            pos.Y = 0;

            foreach (RTF_View.CommonViewObject rich_object in subpage_objects)
            {
                PointF obj_pos = new PointF(client_rect.X, client_rect.Y + pos.Y);
                rich_object.Paint(e.Graphics, obj_pos);
                pos.Y += rich_object.Height;
            }
        }

        public RTF_View_Subpage(RTF_View_Page view_page)
        {
            this.page = view_page;
        }
    };

}
