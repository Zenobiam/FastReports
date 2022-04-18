using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace FastReport.RichTextParser
{
    internal class RTF_Picture : RTF_CommonRichElement
    {
        internal Picture picture = new Picture();
        internal long metamapmode;

        public RTF_Picture()
        {
            picture.scalex = 100;
            picture.scaley = 100;
            subdata_counter = 1;
        }

        internal override RichObject RichObject
        {
            get
            {
                RichObject rich = new RichObject();
                rich.type = RichObject.Type.Picture;
                rich.picture = picture;
                rich.size = picture.size;
                return rich;
            }
        }

        private enum State { PictHeader, BlipImage, BlipMetafile };
        private State state = State.PictHeader;
        int subdata_counter;

        internal void CreateImage(Stream stream)
        {
            switch (state)
            {
                case State.PictHeader:
                    throw new NotImplementedException("Image format not supported (yet)");
                case State.BlipImage:
                    picture.image = Bitmap.FromStream(stream);
                    picture.width = picture.image.Width;
                    picture.height = picture.image.Height;
                    break;
                case State.BlipMetafile:
#if true
                    //picture.image =  new Metafile(stream);
                    picture.image = new Bitmap(stream);
                    if (metamapmode == 8)
                    {
                        picture.width = picture.image.Width;
                        picture.height = picture.image.Height;
                        if (picture.desired_height != 0)
                            picture.height = Twips2Pixels(picture.desired_height);
                        if (picture.desired_width != 0)
                            picture.height = Twips2Pixels(picture.desired_width);
                    }
#elif false
          using (var fileStream = File.Create("my.wmf"))
          {
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(fileStream);
          }
#else
          picture.image = new Bitmap(32, 32, PixelFormat.Format32bppArgb);
          using (Graphics graphics = Graphics.FromImage(picture.image))
          {
            graphics.FillEllipse(Brushes.Brown, new Rectangle(new Point(0, 0), picture.image.Size));
          }

#endif
                    break;
            }
        }

        private byte CharToHex(char ch)
        {
            if (char.IsDigit(ch))
                return (byte)(ch - '0');
            if (ch >= 'A' && ch <= 'F')
                return (byte)(10 + ch - 'A');
            if (ch >= 'a' && ch <= 'f')
                return (byte)(10 + ch - 'a');
            return 0;
        }

        internal bool ParseBlip(RTF_Parser parser, RTF_Header header)
        {
            int i, j;
            int len = parser.Text.Length;
            byte h = 0, l;
            byte[] raw_pict = new byte[len >> 1];
            for (i = j = 0; i < len; i++)
            {
                if ((i & 1) == 0)
                {
                    h = CharToHex(parser.Text[i]);
                }
                else
                {
                    l = CharToHex(parser.Text[i]);
                    raw_pict[j++] = (byte)((h << 4) | l);
                }
            }
            try
            {
                CreateImage(new MemoryStream(raw_pict));
            }
            catch (Exception e)
            {
                if (picture.desired_height != 0)
                    picture.height = Twips2Pixels(picture.desired_height);
                if (picture.desired_width != 0)
                    picture.width = Twips2Pixels(picture.desired_width);
                picture.image = new Bitmap(picture.width, picture.height);
                Graphics g = Graphics.FromImage(picture.image);
                g.DrawString("Unable load bitmap", new Font(FontFamily.GenericMonospace, 10), new SolidBrush(Color.Red), 5, 5); ;
                g.DrawString(e.Message, new Font(FontFamily.GenericMonospace, 8), new SolidBrush(Color.Red), 5, 20); ;
            }
            return false;
        }

        internal override bool Parse(RTF_Parser parser, RTF_Header header)
        {
            if (parser.Status == ParserStatus.OpenBlock)
            {
                subdata_counter++;
            }
            if (parser.Status == ParserStatus.CloseBlock)
//            if (parser.Status == ParserStatus.OpenBlock)
            {
                subdata_counter--;
            }
            switch (parser.Control)
            {
                case "picw":
                    picture.width = (int)parser.Number;
                    break;
                case "pich":
                    picture.height = (int)parser.Number;
                    break;
                case "picscalex":
                    picture.scalex = (int)parser.Number;
                    break;
                case "picscaley":
                    picture.scaley = (int)parser.Number;
                    break;
                case "pngblip":
                    state = State.BlipImage;
                    break;
                case "jpegblip":
                    state = State.BlipImage;
                    break;
                case "piccropt":
                    picture.crop_top = Twips2Pixels(parser.Number);
                    break;
                case "piccropl":
                    picture.crop_left = Twips2Pixels(parser.Number);
                    break;
                case "piccropb":
                    picture.crop_bottom = Twips2Pixels(parser.Number);
                    break;
                case "piccropr":
                    picture.crop_right = Twips2Pixels(parser.Number);
                    break;
                case "picprop":
                    picture.picprop = true;
                    break;
                case "picwgoal":
                case "picwGoal":
                    picture.desired_width = (int) parser.Number;
                    break;
                case "pichgoal":
                case "pichGoal":
                    picture.desired_height = (int)parser.Number;
                    break;
                case "wmetafile":
                    state = State.BlipMetafile;
                    metamapmode = parser.Number;
                    break;
                case "emfblip":
                    state = State.BlipMetafile;
                    metamapmode = parser.Number;
                    break;
                case "bliptag":
                    picture.tag = (int)parser.Number;
                    break;
                case "blipupi":
                    picture.units_per_inch = (int)parser.Number;
                    break;
                case "blipuid":
                    ;
                    break;
                case "":
                    if (parser.Status == ParserStatus.CloseBlock)
                    {
                        if (parser.Text.Length != 0)
                        {
                            if(subdata_counter <= 0)
                                return ParseBlip(parser, header);
                        }
                    }
                    break;
                case "sp":
                    // Word-2007 shape property
                    break;
                case "sn":
                    // Word-2007 shape property name
                    break;
                case "sv":
                    // Word-2007 shape property value
                    break;
                default:
#if DEBUG_RTF
          throw new NotImplementedException(parser.Control);
#else
                    break;
#endif
            }
            return true;
        }

    }
}