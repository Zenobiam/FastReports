using FastReport.Format;
using FastReport.Utils;
using System.Drawing;
using System.Windows.Forms;

namespace FastReport.Export
{
    class ExportIEMStyle
    {
        #region Private fields
        private Font font;
        private VertAlign vAlign;
        private HorzAlign hAlign;
        private FillBase textFill;
        private FillBase fill;
        private FormatBase format;
        private Border border;
        private Padding padding;
        private float firstTabOffset;
        private bool underlines;
        private int angle;
        private bool rTL;
        private bool wordWrap;
        private float lineHeight;
        private float paragraphOffset;
        private float fontWidthRatio;
        private bool forceJustify;
        #endregion

        #region Public Properties

        public bool WordWrap
        {
            get { return wordWrap; }
            set { wordWrap = value; }
        }
        public bool RTL
        {
            get { return rTL; }
            set { rTL = value; }
        }
        public Font Font
        {
            get { return font; }
            set { font = value; }
        }
        public VertAlign VAlign
        {
            get { return vAlign; }
            set { vAlign = value; }
        }
        public HorzAlign HAlign
        {
            get { return hAlign; }
            set { hAlign = value; }
        }
        public FillBase TextFill
        {
            get { return textFill; }
            set { textFill = value; }
        }
        public Color TextColor
        {
            get { return ExportUtils.GetColorFromFill(TextFill); }
        }
        public Color FillColor
        {
            get { return ExportUtils.GetColorFromFill(Fill); }
        }
        public FillBase Fill
        {
            get { return fill; }
            set { fill = value; }
        }
        public FormatBase Format
        {
            get { return format; }
            set { format = value; }
        }
        public Border Border
        {
            get { return border; }
            set { border = value; }
        }
        public Padding Padding
        {
            get { return padding; }
            set { padding = value; }
        }
        public float FirstTabOffset
        {
            get { return firstTabOffset; }
            set { firstTabOffset = value; }
        }
        public bool Underlines
        {
            get { return underlines; }
            set { underlines = value; }
        }
        public int Angle
        {
            get { return angle; }
            set { angle = value; }
        }
        public float LineHeight
        {
            get { return lineHeight; }
            set { lineHeight = value; }
        }

        public float ParagraphOffset
        {
            get { return paragraphOffset; }
            set { paragraphOffset = value; }
        }

        public float FontWidthRatio
        {
            get { return fontWidthRatio; }
            set { fontWidthRatio = value; }
        }

        public bool ForceJustify
        {
            get { return forceJustify; }
            set { forceJustify = value; }
        }

        #endregion

        public bool Equals(ExportIEMStyle Style)
        {
            return
                    (Style.HAlign == HAlign) ?
                    (Style.VAlign == VAlign) ?
                    Style.RTL == RTL ?
                    Style.WordWrap == WordWrap ?
                    Style.Border.Equals(Border) ?
                    Style.TextFill.Equals(TextFill) ?
                    Style.Fill.Equals(Fill) ?
                    Style.Font.Equals(Font) ?
                    Style.Format.Equals(Format) ?
                    Style.Padding.Equals(Padding) ?
                    (Style.FirstTabOffset == FirstTabOffset) ?
                    (Style.Underlines == Underlines) ?
                    (Style.Angle == Angle) ?
                    (Style.ParagraphOffset == ParagraphOffset) ?
                    (Style.FontWidthRatio == FontWidthRatio) ?
                    (Style.LineHeight == LineHeight) ?
                    (Style.ForceJustify == ForceJustify) ?
                    true : false : false : false : false : false : false : false : false : false : false : false : false : false : false : false : false : false;
        }

        public void Assign(ExportIEMStyle Style)
        {
            Font = Style.Font;
            VAlign = Style.VAlign;
            HAlign = Style.HAlign;
            Format = Style.Format.Clone();
            TextFill = Style.TextFill.Clone();
            RTL = Style.RTL;
            WordWrap = Style.WordWrap;
            Fill = Style.Fill.Clone();
            Border = Style.Border.Clone();
            Padding = Style.Padding;
            FirstTabOffset = Style.FirstTabOffset;
            Underlines = Style.Underlines;
            Angle = Style.Angle;
            LineHeight = Style.LineHeight;
            ParagraphOffset = Style.ParagraphOffset;
            FontWidthRatio = Style.FontWidthRatio;
            ForceJustify = Style.ForceJustify;
            //if (Style.ParagraphFormat != null) ParagraphFormat = Style.ParagraphFormat.MultiplyScale(1); else ParagraphFormat = null;
        }

        public ExportIEMStyle()
        {
            Font = DrawUtils.DefaultFont;
            Format = new GeneralFormat();
            VAlign = VertAlign.Top;
            HAlign = HorzAlign.Left;
            TextFill = new SolidFill();
            Fill = new SolidFill();
            (Fill as SolidFill).Color = Color.Transparent;
            Border = new Border();
            Padding = new Padding();                        
        }
    }
}
