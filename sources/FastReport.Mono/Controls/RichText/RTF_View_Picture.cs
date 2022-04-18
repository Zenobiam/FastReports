using FastReport.RichTextParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FastReport.RichTextParser
{
    internal class View_Picture : RTF_View.CommonViewObject
    {
        internal Picture picture;
        int width;
        int height;
        int left;

        public View_Picture(RTF_View view)
        {
            base.view = view;
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        internal override void InsertText(int position_within_object, string text)
        {
            throw new Exception("insert tezt to a picture position.");
        }

        //internal override int FindString(string findWhat, out int line, out int position)
        //{
        //  line = 0;
        //  position = 0;
        //  return 0;
        //}

        internal override int Paint(Graphics g, PointF pos)
        {
#if false
            int width = (picture.image.Width) * picture.scalex / 100 + (picture.crop_left + picture.crop_right);
            int height = (picture.image.Height) * picture.scaley / 100 + (picture.crop_top + picture.crop_bottom);
            g.DrawImage(picture.image, base.Left + picture.crop_left, top + picture.crop_top, width, height);
//            g.DrawImage(picture.image, base.Left, top, width - (picture.crop_left + picture.crop_right), height - (picture.crop_top + picture.crop_bottom));
#elif false
            g.DrawImage(picture.image, left, top + picture.crop_top, width, height);
            //            g.DrawImage(picture.image, base.Left, top, width - (picture.crop_left + picture.crop_right), height - (picture.crop_top + picture.crop_bottom));
#else
            width = (RTF_View.Twips2Pixels(picture.desired_width) + picture.crop_left + picture.crop_right)  * picture.scalex / 100;
            height = (RTF_View.Twips2Pixels(picture.desired_height) + picture.crop_top + picture.crop_bottom) * picture.scaley / 100 ;
            g.DrawImage(picture.image, pos.X + base.Left + picture.crop_left, pos.Y + picture.crop_top, width, height);
//            g.DrawImage(picture.image, base.Left, top, width - (picture.crop_left + picture.crop_right), height - (picture.crop_top + picture.crop_bottom));
#endif
            return this.Height;
        }

        internal override int Prepare(Graphics g)
        {
            //int len = this.Length;
            return this.Height;
        }

        internal override void SpitToRuns(RichObject parser_object)
        {
            this.picture = parser_object.picture;
#if false
            width = picture.width * picture.scalex / 100 + (picture.crop_left + picture.crop_right);
            height = picture.height * picture.scaley / 100 + (picture.crop_top + picture.crop_bottom);
#else
            width = RTF_View.Twips2Pixels(picture.desired_width) * picture.scalex / 100 + (picture.crop_left + picture.crop_right);
            height = RTF_View.Twips2Pixels(picture.desired_height) * picture.scaley / 100 + (picture.crop_top + picture.crop_bottom);
            //width = (picture.desired_width + picture.crop_left + picture.crop_right);
            //height = (picture.desired_height + picture.crop_top + picture.crop_bottom);
#endif
            left = base.Left + picture.crop_left;

            this.Height = height;// picture.height * picture.scaley / 100;
            this.Width = width; // picture.width * picture.scalex / 100;
        }

        internal override void UpdatePads(TableRow row, int top, int pos_x, int dx)
        {
            this.Left = pos_x;
            this.Top = top;
        }
    }
}
