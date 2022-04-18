using FastReport.Utils;
using Svg;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

#pragma warning disable

namespace FastReport.SVG
{
    /// <summary>
    /// SVG object
    /// </summary>
    public partial class SVGObject : PictureObjectBase, ICloneable
    {
        private readonly string svgTagPr = "<svg";
        #region Private fields

        private SvgDocument FSvgDocument;
        private SvgDocument FSVGGrayscale;
        private DateTime FLastDrawTime;
        private readonly TimeSpan FSpan;
        private Image FCachedImage;
        private Image FCachedGrayscaleImage;
        private string FSVGString;
        private SvgViewBox FViewBox;
        private SvgAspectRatio FAspectRatio;
        private float FLastImgWidth;
        private float FLastImgHeight;

        private float originalWidth;
        private float originalHeight;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets svg document
        /// </summary>
        [Browsable(false)]
        public SvgDocument SvgDocument
        {
            get { return FSvgDocument; }
        }

        /// <summary>
        /// Gets or sets ViewBox value
        /// </summary>
        [Browsable(false)]
        public SvgViewBox ViewBox
        {
            get { return FViewBox; }
            set { SetViewBox(value); }
        }

        /// <summary>
        /// Gets or sets AspectRatio value
        /// </summary>
        [Browsable(false)]
        public SvgAspectRatio AspectRatio
        {
            get { return FAspectRatio; }
            set { SetAspectRatio(value); }
        }

        /// <inheritdoc/>
        [DefaultValue(PictureBoxSizeMode.Zoom)]
        [Category("Behavior")]
        public override PictureBoxSizeMode SizeMode
        {
            get { return base.SizeMode; }
            set
            {
                base.SizeMode = value;
                if (SvgDocument != null) // if not PictureObject -> Assign();
                {
                    if (value == PictureBoxSizeMode.StretchImage || value == PictureBoxSizeMode.AutoSize)
                        AspectRatio = new SvgAspectRatio(SvgPreserveAspectRatio.none);
                    else
                        AspectRatio = new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid);
                }
            }
        }

        /// <summary>
        /// Gets or sets grayscale svg document
        /// </summary>
        [Browsable(false)]
        public SvgDocument SVGGrayscale
        {
            get
            {
                if (FSVGGrayscale == null)
                    FSVGGrayscale = GetSVGGrayscale();
                return FSVGGrayscale;
            }
            set { FSVGGrayscale = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that the image should be displayed in grayscale mode.
        /// </summary>
        [DefaultValue(false)]
        [Category("Appearance")]
        public override bool Grayscale
        {
            get { return base.Grayscale; }
            set
            {
                base.Grayscale = value;
                if (!Grayscale && FSVGGrayscale != null)
                {
                    FSVGGrayscale = null;
                }
                if (value == true && (SvgDocument != null)) // if not PictureObject -> Assign();
                {
                    FSVGGrayscale = GetSVGGrayscale();
                }
            }
        }

        /// <summary>
        /// Returns SVG string
        /// </summary>
        [EditorAttribute("FastReport.TypeEditors.ExpressionEditor, FastReport", typeof(UITypeEditor))]
        public string SVGString
        {
            get { return FSVGString; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    FSVGString = value;
                    FSvgDocument = SvgDocument.FromSvg<SvgDocument>(FSVGString);
                    if (Grayscale)
                    {
                        SVGGrayscale = GetSVGGrayscale();
                    }
                    if (FSvgDocument != null)
                    {
                        originalWidth = this.Width;
                        originalHeight = this.Height;
                        FLastImgHeight = int.MinValue;
                        FLastImgWidth = int.MinValue;

                        if (FSvgDocument.Width.Type != SvgUnitType.Percentage)
                        {
                            originalWidth = FSvgDocument.Width;
                            FSvgDocument.Width = new SvgUnit(SvgUnitType.Percentage, 100);
                        }
                        if (FSvgDocument.Height.Type != SvgUnitType.Percentage)
                        {
                            originalHeight = FSvgDocument.Height;
                            FSvgDocument.Height = new SvgUnit(SvgUnitType.Percentage, 100);
                        }
                        if (FSvgDocument.ViewBox.Width == 0 || FSvgDocument.ViewBox.Height == 0)
                        {
                            FSvgDocument.ViewBox = new SvgViewBox(FSvgDocument.ViewBox.MinX,
                                FSvgDocument.ViewBox.MinY,
                                FSvgDocument.ViewBox.Width == 0 ? originalWidth : FSvgDocument.ViewBox.Width,
                                FSvgDocument.ViewBox.Height == 0 ? originalHeight : FSvgDocument.ViewBox.Height);
                        }
                    }

                }
            }
        }

        protected override float ImageHeight
        {
            get
            {
                if (SvgDocument == null) return 0;
                if (SizeMode == PictureBoxSizeMode.AutoSize)
                    return originalHeight;
                return SvgDocument.GetDimensions().Height;
            }
        }

        protected override float ImageWidth
        {
            get
            {
                if (SvgDocument == null) return 0;
                if (SizeMode == PictureBoxSizeMode.AutoSize)
                    return originalWidth;
                return SvgDocument.GetDimensions().Width;
            }
        }
        #endregion

        #region Private Methods

        private Color GetGrayscaleColor(Color color)
        {
            int grayscale = (int)((color.R * 0.299f) + (color.G * 0.587f) + (color.B * 0.114f));
            return Color.FromArgb(color.A, grayscale, grayscale, grayscale);

        }
        private void MakeElementGrayScale(SvgElement element)
        {
            if (element.Fill != null)
            {
                if (element.Fill is SvgGradientServer)
                    foreach (SvgGradientStop stop in (element.Fill as SvgGradientServer).Stops)
                    {
                        (stop.StopColor as SvgColourServer).Colour =
                            GetGrayscaleColor((stop.StopColor as SvgColourServer).Colour);
                    }
                else
                    ((element).Fill as SvgColourServer).Colour = GetGrayscaleColor(((element).Fill as SvgColourServer).Colour);
            }
            if ((element).Stroke != null)
                ((element).Stroke as SvgColourServer).Colour = GetGrayscaleColor(((element).Stroke as SvgColourServer).Colour);

            if (element.Children.Count > 0)
            {
                foreach (var item in element.Children)
                {
                    MakeElementGrayScale(item);
                }
            }
        }

        //private byte[] SvgToImgByteArray(SvgDocument svg)
        //{
        //    using (MemoryStream pictStr = new MemoryStream())
        //    {
        //        Image img = svg.Draw();
        //        img.Save(pictStr, ImageFormat.Png);
        //        return pictStr.ToArray();
        //    }
        //}

        private static string LoadURL(string url)
        {
            if (!String.IsNullOrEmpty(url))
            {
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                using (WebClient web = new WebClient())
                {
                    return Encoding.UTF8.GetString(web.DownloadData(url));
                }
            }
            return null;
        }

        public Image GetImage()
        {
            if (SvgDocument == null)
                return null;
            FLastDrawTime = DateTime.MinValue;
            Size size = SvgDocument.GetDimensions().ToSize();
            return GetImage(Grayscale, size.Width, size.Height);
        }

        private Image GetImage(bool isGrayscale, int imgWidth, int imgHeight)
        {
            Image image;
            float scaleY = imgHeight / FLastImgHeight;
            float scaleX = imgWidth / FLastImgWidth;
            const float minScale = 0.71f;
            const float maxScale = 1.41f;
            if (SystemFake.DateTime.Now - FLastDrawTime > FSpan || scaleY < minScale || scaleY > maxScale || scaleX < minScale || scaleX > maxScale
                || isGrayscale && FCachedGrayscaleImage == null || !isGrayscale && FCachedImage == null)
            {
                image = isGrayscale ? SVGGrayscale.Draw(imgWidth, imgHeight) : SvgDocument.Draw(imgWidth, imgHeight);
                FLastDrawTime = SystemFake.DateTime.Now;
                if (isGrayscale)
                {
                    if (FCachedGrayscaleImage != null)
                        FCachedGrayscaleImage.Dispose();
                    FCachedGrayscaleImage = image;
                }
                else
                {
                    if (FCachedImage != null)
                        FCachedImage.Dispose();
                    FCachedImage = image;
                }

                FLastImgHeight = imgHeight;
                FLastImgWidth = imgWidth;
            }
            else
            {
                if (isGrayscale)
                    image = FCachedGrayscaleImage;
                else
                    image = FCachedImage;
            }
            return image;
        }

        private string GetSvgString()
        {
            using (StringWriter sw = new StringWriter())
            {
                using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(sw))
                    SvgDocument.Write(writer);
                string svgStr = sw.ToString();
                sw.Flush();
                return svgStr;
            }
        }

        private string GetViewBoxString(out int startVb, out int endVb)
        {
            if (string.IsNullOrEmpty(SVGString))
                throw new ArgumentNullException("SvgString is null");
            startVb = SVGString.IndexOf("viewBox");
            endVb = -1;
            if (startVb != -1)
            {
                startVb = SVGString.IndexOf("\"", startVb) + 1;
                endVb = SVGString.IndexOf("\"", startVb);
                string vb = SVGString.Substring(startVb, endVb - startVb);
                return vb;
            }
            else return null;
        }

        private SvgViewBox GetViewBox()
        {
            if (string.IsNullOrEmpty(SVGString))
                throw new ArgumentNullException("SvgString is null");
            SvgViewBox viewBox = new SvgViewBox();
            int startVb, endVb;
            string vb = GetViewBoxString(out startVb, out endVb);
            if (!string.IsNullOrEmpty(vb))
            {
                string[] vbArray = vb.Split(' ', ',');
                float minX;
                float minY;
                float width;
                float height;
                if (float.TryParse(vbArray[0], out minX) &&
                    float.TryParse(vbArray[1], out minY) &&
                    float.TryParse(vbArray[2], out width) &&
                    float.TryParse(vbArray[3], out height))
                {
                    viewBox.MinX = minX;
                    viewBox.MinY = minY;
                    viewBox.Width = width;
                    viewBox.Height = height;
                }
            }
            return viewBox;
        }

        private void SetViewBox(SvgViewBox viewBox)
        {
            int startVb, endVb;
            string oldVb = GetViewBoxString(out startVb, out endVb);
            string newVb = viewBox.MinX + " " + viewBox.MinY + " " + viewBox.Width + " " + viewBox.Height;
            if (!string.IsNullOrEmpty(oldVb))
            {
                SVGString = SVGString.Remove(startVb, endVb - startVb);
                SVGString = SVGString.Insert(startVb, newVb);
            }
            else
            {
                startVb = SVGString.IndexOf(svgTagPr) + svgTagPr.Length;
                SVGString = SVGString.Insert(startVb, " " + "viewBox=\"" + newVb + "\"");
            }
            FViewBox = viewBox;
        }

        private string GetAspectRatioString(out int startAr, out int endAr)
        {
            if (string.IsNullOrEmpty(SVGString))
                throw new ArgumentNullException("SvgString is null");
            startAr = SVGString.IndexOf("preserveAspectRatio");
            endAr = -1;
            if (startAr != -1)
            {
                startAr = SVGString.IndexOf("\"", startAr) + 1;
                endAr = SVGString.IndexOf("\"", startAr);
                string ar = SVGString.Substring(startAr, endAr - startAr);
                return ar;
            }
            else return null;
        }

        private SvgAspectRatio GetAspectRatio()
        {
            if (string.IsNullOrEmpty(SVGString))
                throw new ArgumentNullException("SvgString is null");
            SvgAspectRatio aspectRatio = new SvgAspectRatio();
            int startAr, endAr;
            string ar = GetAspectRatioString(out startAr, out endAr);
            if (!string.IsNullOrEmpty(ar))
            {
                SvgPreserveAspectRatio align;
                Enum.TryParse(ar, out align);
                aspectRatio.Align = align;
            }
            return aspectRatio;
        }

        private void SetAspectRatio(SvgAspectRatio aspectRatio)
        {
            int startAr, endAr;
            string oldAr = GetAspectRatioString(out startAr, out endAr);
            string newAr = aspectRatio.Align.ToString();
            if (!string.IsNullOrEmpty(oldAr))
            {
                SVGString = SVGString.Remove(startAr, endAr - startAr);
                SVGString = SVGString.Insert(startAr, newAr);
            }
            else
            {
                startAr = SVGString.IndexOf(svgTagPr) + svgTagPr.Length;
                SVGString = SVGString.Insert(startAr, " " + "preserveAspectRatio=\"" + newAr + "\"");
            }
            FAspectRatio = aspectRatio;
        }

        //private Stream StreamFromString(string s)
        //{
        //    var stream = new MemoryStream();
        //    var writer = new StreamWriter(stream, Encoding.UTF8);
        //    writer.Write(s);
        //    writer.Flush();
        //    stream.Position = 0;
        //    return stream;
        //}

        #endregion

        #region Internal Methods
        internal static SvgDocument GetSvgDocument(string filePath)
        {
            SvgDocument document = SvgDocument.Open(filePath);
            return document;
        }
        internal void MakeGrayScale(SvgDocument svg)
        {
            foreach (var element in svg.Children)
            {
                MakeElementGrayScale(element);

                if (element.Children.Count > 0)
                {
                    foreach (var item in element.Children)
                    {
                        MakeElementGrayScale(item);
                    }
                }
            }
        }

        internal SvgDocument GetSVGGrayscale()
        {
            FSVGGrayscale = SvgDocument.FromSvg<SvgDocument>(FSVGString);
            MakeGrayScale(FSVGGrayscale);
            //GrayscaleHash = FSVGGrayscale.GetHashCode();
            return FSVGGrayscale;
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            SVGObject c = writer.DiffObject as SVGObject;
            base.Serialize(writer);
            if (writer.SerializeTo == SerializeTo.SourcePages ||
                writer.SerializeTo == SerializeTo.Preview ||
                   (String.IsNullOrEmpty(ImageLocation) && String.IsNullOrEmpty(DataColumn)))
            {
                if (!string.IsNullOrEmpty(SVGString) && SVGString != c.SVGString)
                {
                    writer.WriteValue("SvgData", Convert.ToBase64String(Encoding.UTF8.GetBytes(SVGString)));
                }
            }
        }

        /// <inheritdoc/>
        public override void Deserialize(FRReader reader)
        {
            base.Deserialize(reader);
            if (reader.HasProperty("SvgData"))
            {
                SetSVGByContent(Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadStr("SvgData"))));
            }
        }

        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);
            SVGObject src = source as SVGObject;
            if (src != null)
            {
                if (src.SVGString != null)
                    SetSVGByContent(src.SVGString);
            }
        }

        /// <inheritdoc/>
        public override void LoadImage()
        {
            if (!String.IsNullOrEmpty(ImageLocation))
            {
                try
                {
                    Uri uri = CalculateUri();
                    if (uri.IsFile)
                        SetSVGByPath(uri.LocalPath);
                    else
                        SetSVGByContent(LoadURL(uri.ToString()));
                }
                catch
                {
                    SetSVGByContent("");
                }
            }
        }

        /// <inheritdoc/>
        public override void GetData()
        {
            base.GetData();
            if (!String.IsNullOrEmpty(DataColumn))
            {
                object data = Report.GetColumnValueNullable(DataColumn);
                if (data is byte[])
                {
                    SetSVGByContent(Encoding.UTF8.GetString((byte[])data));
                }
                if (data is string)
                {
                    try
                    {
                        SetSVGByContent(data.ToString());
                    }
                    catch
                    {
                        ImageLocation = data.ToString();
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void DrawImage(FRPaintEventArgs e)
        {
            IGraphics g = e.Graphics;

            if (SvgDocument==null)
            {
                DrawErrorImage(g, e);
                return;
            }

            float drawLeft = (AbsLeft + Padding.Left) * e.ScaleX;
            float drawTop = (AbsTop + Padding.Top) * e.ScaleY;
            float drawWidth = (Width - Padding.Horizontal) * e.ScaleX;
            float drawHeight = (Height - Padding.Vertical) * e.ScaleY;

            RectangleF drawRect = new RectangleF(
              drawLeft,
              drawTop,
              drawWidth,
              drawHeight);

            IGraphicsState state = g.Save();
            try
            {
                g.SetClip(drawRect);
                Report report = Report;
                if (report != null && report.SmoothGraphics)
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                }


                DrawImageInternal(e, drawRect);
            }
            finally
            {
                g.Restore(state);
            }
        }

        protected override void DrawImageInternal2(IGraphics graphics, PointF upperLeft, PointF upperRight, PointF lowerLeft)
        {
            int imgWidth = (int)Math.Sqrt((upperLeft.X - upperRight.X) * (upperLeft.X - upperRight.X) + (upperLeft.Y - upperRight.Y) * (upperLeft.Y - upperRight.Y));
            int imgHeight = (int)Math.Sqrt((upperLeft.X - lowerLeft.X) * (upperLeft.X - lowerLeft.X) + (upperLeft.Y - lowerLeft.Y) * (upperLeft.Y - lowerLeft.Y));

            Image image = GetImage(Grayscale, imgWidth, imgHeight);

            if(image != null)
                graphics.DrawImage(image, new PointF[] { upperLeft, upperRight, lowerLeft });
        }

        ///// <inheritdoc/>
        //internal override void DrawImageInternal(FRPaintEventArgs e, RectangleF drawRect)
        //{
        //    if (Image == null || SvgDocument == null)
        //        return;

        //    bool rotate = Angle == 90 || Angle == 270;
        //    float imageWidth = Image.Width;//rotate ? Image.Height : Image.Width;
        //    float imageHeight = Image.Height;//rotate ? Image.Width : Image.Height;


        //    PointF upperLeft;
        //    PointF upperRight;
        //    PointF lowerLeft;
        //    System.Drawing.Drawing2D.Matrix matrix = e.Graphics.Transform;
        //    GetImageAngleTransform(drawRect, imageWidth, imageHeight, e.ScaleX, e.ScaleY, matrix.OffsetX, matrix.OffsetY, out upperLeft, out upperRight, out lowerLeft);

        //    int imgWidth = (int)Math.Sqrt((upperLeft.X - upperRight.X) * (upperLeft.X - upperRight.X) + (upperLeft.Y - upperRight.Y) * (upperLeft.Y - upperRight.Y));
        //    int imgHeight = (int)Math.Sqrt((upperLeft.X - lowerLeft.X) * (upperLeft.X - lowerLeft.X) + (upperLeft.Y - lowerLeft.Y) * (upperLeft.Y - lowerLeft.Y));

        //    Image image;

        //    if (Grayscale)
        //    {
        //        if (FSVGGrayscale == null || GrayscaleHash != FSVGGrayscale.GetHashCode())
        //        {
        //            FSVGGrayscale = GetSVGGrayscale();
        //        }
        //        image = GetImage(true, imgWidth, imgHeight);
        //    }
        //    else
        //        image = GetImage(false, imgWidth, imgHeight);

        //    e.Graphics.DrawImage(image, new PointF[] { upperLeft, upperRight, lowerLeft });
        //}

        /// <summary>
        /// Returns clone of this object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            SVGObject clone = new SVGObject();
            clone.Assign(this);
            return clone;
        }

        /// <summary>
        /// Sets svg object by SvgDocument
        /// </summary>
        /// <param name="svg">SVG document</param>
        public void SetSVG(SvgDocument svg)
        {
            SetSVGByContent(svg.GetXML());
        }

        /// <summary>
        /// Sets svg object from specified path
        /// </summary>
        /// <param name="path">path to SVG file</param>
        public void SetSVGByPath(string path)
        {
            SetSVGByContent(File.ReadAllText(path));
        }

        /// <summary>
        /// Sets svg object from svg string
        /// </summary>
        /// <param name="content">SVG string</param>
        public void SetSVGByContent(string content)
        {
            SVGString = content;
        }

        protected override void ResetImageIndex()
        {
            
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (FCachedImage != null)
                {
                    FCachedImage.Dispose();
                    FCachedImage = null;
                }
                if(FCachedGrayscaleImage !=null)
                {
                    FCachedGrayscaleImage.Dispose();
                    FCachedGrayscaleImage = null;
                }

            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SVGObject"/> class with default settings.
        /// </summary>
        public SVGObject() : base()
        {
            FSpan = new TimeSpan(0, 0, 3);
            SetFlags(Flags.HasSmartTag, true);
            FLastDrawTime = DateTime.MinValue;
        }
    }
}


#pragma warning restore