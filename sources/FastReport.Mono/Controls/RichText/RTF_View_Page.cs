using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FastReport.RichTextParser;
//using static FastReport.RichTextParser.RTF_View;

namespace RichTextParser.RTF
{
    class RTF_View_Page
    {
        private List<RTF_View_Subpage> subpage_list = new List<RTF_View_Subpage>();
		internal List<RTF_View.CommonViewObject> view_objects = new List<RTF_View.CommonViewObject>();
        private Page page;
        internal int Width;
        Rectangle marigns;
        internal Rectangle view_box; // Set internal for RTF_View_Cursor
        internal RTF_View parent_control;

        internal Size size;

//        public int VerticalPosition { get; private set; }
        //public int PageObjectHeight { get; private set; }
        public int TotalSubpagesHeight { get; private set; }
        public Rectangle Margins { get { return marigns; } }
        public Rectangle ViewBox { get { return view_box; } }

        internal List<RTF_View_Subpage> Subpages
        {
            get
            {
                return subpage_list;
            }
        }


        private int length_of_page;
        public int Length
        {
            get { return length_of_page; }
            set
            {
                length_of_page = value;
            }
        }

        public RTF_View_Page(Page page)
        {
            this.page = page;
        }

        internal void PreparePage(Graphics g, RTF_View rtf_view, Size view_size)
        {
            parent_control = rtf_view;
            RichDocument doc = rtf_view.document;

            //            size = new Size(Twips2Pixels((int)doc.paper_width), Twips2Pixels((int)doc.paper_height));
            size = view_size;

#if false
            marigns.X = Twips2Pixels(doc.global_margin_left);
            marigns.Y = Twips2Pixels(doc.global_margin_top);
            marigns.Width = Twips2Pixels(doc.global_margin_left + doc.global_margin_right);
            marigns.Height = Twips2Pixels(doc.global_margin_top + doc.global_margin_bottom);
            this.Width = size.Width - marigns.Width;
#else
            this.Width = size.Width; // - marigns.Width;
#endif
            this.Length = 0;
            if (page.sequence.objects != null)
                foreach (RichObject rich_obj in page.sequence.objects)
                {
					RTF_View.CommonViewObject view_obj = null;

                    switch (rich_obj.type)
                    {
                        case RichObject.Type.Paragraph:
                            view_obj = new View_Paragraph(rtf_view);
                            break;
                        case RichObject.Type.Table:
                            view_obj = new View_Table(rtf_view);
                            break;
                        case RichObject.Type.Picture:
                            view_obj = new View_Picture(rtf_view);
                            break;
                    }

                     
                    int total_height = 0;
                    if (view_obj != null)
                    {
                        view_obj.Length = (int)rich_obj.size;
                        view_obj.Width = this.Width;
                        view_obj.SpitToRuns(rich_obj);
                        int height = view_obj.Prepare(g);
                        this.Length += view_obj.Length + 1;
                        total_height += height;
                        view_objects.Add(view_obj);
                        rtf_view.Add(view_obj);
                    }
                }
        }

        const int right_gap = 10;
        public const int top_gap = 10;

		internal bool DoNotClearPageBackground { get { return (parent_control.HelpersMode & RTF_View.VisaulHelpers.DoNotClearPageBackground) == 0; } }
        internal bool ExcludeMarings { get { return (parent_control.HelpersMode & RTF_View.VisaulHelpers.ExcludeMarings) != 0; } }
		internal bool ShowPageMargibs { get { return (parent_control.HelpersMode & RTF_View.VisaulHelpers.Margins) != 0; } }
        internal void InsertText(RTF_View_Position location, string text)
        {
            if (location.position_subpage != null)
            {
                if (location.position_rtf_object == null)
                {
                    View_Paragraph par = new View_Paragraph(this.parent_control);
                    location.position_rtf_object = par;
                    location.position_subpage.subpage_objects.Add(par);
                    par.Left = view_box.X;
                    par.Width = view_box.Width;
                    this.Length++;
                }
                else
                {
                    (location.position_rtf_object as View_Paragraph).Left = view_box.X;
                }
                location.position_rtf_object.InsertText(location.position_within_object, text);
                location.position_subpage.Length += text.Length;
                this.Length += text.Length;
            }
            else
                throw new NotImplementedException("Add subpage to empty string");
        }

        internal void OnPaint(PaintEventArgs e, PointF pos)
        {
            RectangleF screen_box = view_box;

            screen_box.Offset(0, (int) -pos.Y);
            Width = view_box.Width; // size.Width;

            pos.Y = screen_box.Y;
            int vertical_pixel_counter = view_box.Height;
            int i = 1;
            int skip_gap = 0;

            int page_view_gap = marigns.Height + top_gap;

            int page_top = -marigns.Top; // +
                // (((parent_control.HelpersMode & VisaulHelpers.ExcludeMarings) != VisaulHelpers.ExcludeMarings) ? top_gap : 0);

            foreach (RTF_View_Subpage virtual_page in this.subpage_list)
            {
                skip_gap += size.Height + top_gap;
                //if (skip_gap + view_box.Height >= top_line)
                {
                    virtual_page.OnPaint(e, pos);
                }

                pos.Y = screen_box.Y + (size.Height + top_gap) * i++;
                if (pos.Y > screen_box.Height)
                    break;
                vertical_pixel_counter += size.Height + top_gap;
            }

//            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(80, 255, 0, 0)), view_box);
        }

        internal void InvalidatePage(Graphics g, int Left, int vertical_position)
        {
            //this.VerticalPosition = vertical_position;
            //this.PageObjectHeight = 0;

            if (this.ExcludeMarings)
            {
                view_box = new Rectangle(
                    Left,
                    vertical_position,
                    size.Width,
                    size.Height
                    );
            }
            else
            {
                view_box = new Rectangle(
					Left + (this.page.margin_left == 0 ? marigns.X : RTF_View.Twips2Pixels((int)this.page.margin_left)),
					top_gap + vertical_position + ((this.page.margin_top == 0) ? marigns.Y : RTF_View.Twips2Pixels((int)this.page.margin_top)),
                    size.Width - marigns.Width,
                    size.Height - marigns.Height
                    );
            }


            int height_counter = view_box.Height;
            subpage_list.Clear();

            RTF_View_Subpage virtual_page = new RTF_View_Subpage(this);

            this.TotalSubpagesHeight = 0;
            int obj_top = view_box.Top;
            int vpage_top = obj_top;
			foreach (RTF_View.CommonViewObject richobject in this.view_objects)
            {
                richobject.Left = view_box.Left;
                richobject.Top = obj_top;
                richobject.Width = view_box.Width;
                int h = richobject.Prepare(g);

                if (richobject is View_Picture)
                {
                    System.Diagnostics.Debug.WriteLine("Picture");
                }

                height_counter -= h;
                if (height_counter < 0)
                {
                    virtual_page.Top = vpage_top;
                    vpage_top += size.Height + top_gap;
                    //virtual_page.height = size.Height + top_gap;
                    subpage_list.Add(virtual_page);
                    virtual_page = new RTF_View_Subpage(this);
                    height_counter = view_box.Height - h;
                    obj_top = (size.Height + top_gap) * subpage_list.Count;
                    this.TotalSubpagesHeight += size.Height + top_gap;
                    richobject.Top = obj_top;
                }
                obj_top += h;
                virtual_page.subpage_objects.Add(richobject);
                vertical_position += h;
                //this.PageObjectHeight += h;
                virtual_page.Length += richobject.Length + 1;
            }

            virtual_page.Top = vpage_top;
///            virtual_page.height = size.Height + top_gap;
            subpage_list.Add(virtual_page);
            this.TotalSubpagesHeight += size.Height + top_gap;
        }
    }
}
