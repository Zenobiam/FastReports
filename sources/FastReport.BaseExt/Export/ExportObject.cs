using System;
using System.Collections.Generic;
using System.IO;

namespace FastReport.Export
{
    class ExportIEMObject
    {
        #region Private fields

        private TextRenderType textRenderType;
        private string text;
        private string originalText;
        private string uRL;
        private int styleIndex;
        private ExportIEMStyle style;
        private bool isText;
        private bool isRichText;
        private float left;
        private float top;
        private float width;
        private float height;
        private ExportIEMObject parent;
        private int counter;
        private System.Drawing.Image metafile;
        private MemoryStream pictureStream;
        private bool isBand;
        private List<string> wrappedText;
        private string hash;
        private bool isBase;
        private object value;
        private bool isNumeric;
        private bool isDateTime;
        private bool isPercent;
        private bool isSvg;
        private Utils.InlineImageCache inlineImageCache;
        private ParagraphFormat paragraphFormat;
        private float tabWidth;

        private int fx;
        private int fy;
        private int fdx;
        private int fdy;
        private bool exist;

        #endregion

        #region Private methods

        #endregion

        #region Public properties

        public bool Base
        {
            get { return isBase; }
            set { isBase = value; }
        }

        public string Hash
        {
            get { return hash; }
            set { hash = value; }
        }

        public TextRenderType TextRenderType
        {
            get { return textRenderType; }
            set { textRenderType = value; }
        }

        /// <summary>
        /// This property for internal use only.
        /// </summary>
        public bool HtmlTags
        {
            get
            {
                switch (textRenderType)
                {
                    case TextRenderType.HtmlTags:
                    case TextRenderType.HtmlParagraph:
                        return true;
                    default:
                        return false;
                }
            }
            set { textRenderType = value ? TextRenderType.HtmlTags : TextRenderType.Default; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        public string OriginalText
        {
            get { return originalText; }
            set { originalText = value; }
        }

        public List<string> WrappedText
        {
            get { return wrappedText; }
            set { wrappedText = value; }
        }
        public string URL
        {
            get { return uRL; }
            set { uRL = value; }
        }
        public int StyleIndex
        {
            get { return styleIndex; }
            set { styleIndex = value; }
        }
        public bool IsText
        {
            get { return isText; }
            set { isText = value; }
        }
        public bool IsRichText
        {
            get { return isRichText; }
            set { isRichText = value; }
        }
        public bool IsSvg
        {
            get { return isSvg; }
            set { isSvg = value; }
        }
        public float Left
        {
            get { return left; }
            set { left = value; }
        }
        public float Top
        {
            get { return top; }
            set { top = value; }
        }
        public float Width
        {
            get { return width; }
            set { width = value; }
        }
        public float Height
        {
            get { return height; }
            set { height = value; }
        }
        public ExportIEMObject Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        public ExportIEMStyle Style
        {
            get { return style; }
            set { style = value; }
        }
        public int Counter
        {
            get { return counter; }
            set { counter = value; }
        }
        public System.Drawing.Image Metafile
        {
            get { return metafile; }
            set { metafile = value; }
        }
        public MemoryStream PictureStream
        {
            get { return pictureStream; }
            set { pictureStream = value; }
        }
        public bool IsBand
        {
            get { return isBand; }
            set { isBand = value; }
        }
        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public bool IsNumeric
        {
            get { return isNumeric; }
            set { isNumeric = value; }
        }
        public bool IsDateTime
        {
            get { return isDateTime; }
            set { isDateTime = value; }
        }

        public bool IsPercent
        {
            get { return isPercent; }
            set { isPercent = value; }
        }

        public int x
        {
            get { return fx; }
            set { fx = value; }
        }
        public int y
        {
            get { return fy; }
            set { fy = value; }
        }
        public int dx
        {
            get { return fdx; }
            set { fdx = value; }
        }
        public int dy
        {
            get { return fdy; }
            set { fdy = value; }
        }

        public bool Exist
        {
            get { return exist; }
            set { exist = value; }
        }

        public Utils.InlineImageCache InlineImageCache
        {
            get { return inlineImageCache; }
            set { inlineImageCache = value; }
        }

        public ParagraphFormat ParagraphFormat
        {
            get { return paragraphFormat; }
            set { paragraphFormat = value; }
        }
        public float TabWidth
        {
            get { return tabWidth; }
            set { tabWidth = value; }
        }
        #endregion

        public ExportIEMObject()
        {
            isText = true;
            isNumeric = false;
            text = String.Empty;
            isBase = true;
            originalText = null;
        }
    }
}
