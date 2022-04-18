using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FastReport.Utils;

namespace FastReport.Design
{
    internal class LastFormatting
    {
        private Border border;
        private FillBase fill;
        private Font font;
        private HorzAlign horzAlign;
        private VertAlign vertAlign;
        private FillBase textFill;
        private int angle;

        public Border Border
        {
            get { return border; }
            set { border = value; }
        }
        public FillBase Fill
        {
            get { return fill; }
            set { fill = value; }
        }
        public Font Font
        {
            get { return font; }
            set { font = value; }
        }
        public HorzAlign HorzAlign
        {
            get { return horzAlign; }
            set { horzAlign = value; }
        }
        public VertAlign VertAlign
        {
            get { return vertAlign; }
            set { vertAlign = value; }
        }
        public FillBase TextFill
        {
            get { return textFill; }
            set { textFill = value; }
        }
        public int Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public void SetFormatting(ReportComponentBase c)
        {
            if (c != null)
            {
                if (Border != null && c.FlagUseBorder)
                    c.Border = Border.Clone();
                if (c.FlagUseFill)
                    c.Fill = Fill.Clone();
            }
            if (c is TextObject)
            {
                TextObject c1 = c as TextObject;
                if (Font != null)
                    c1.Font = Font;
                c1.HorzAlign = HorzAlign;
                c1.VertAlign = VertAlign;
                c1.TextFill = TextFill.Clone();
                c1.Angle = Angle;
            }
        }

        public LastFormatting()
        {
            Border = new Border();
            Fill = new SolidFill();
            TextFill = new SolidFill(Color.Black);
            Font = Config.DesignerSettings.DefaultFont;
        }
    }
}