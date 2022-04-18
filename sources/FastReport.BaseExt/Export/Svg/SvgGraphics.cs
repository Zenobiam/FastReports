using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Text;
using FastReport.Utils;

namespace FastReport
{
    /// <summary>
    /// Specifies the image format in SVG export.
    /// </summary>
    public enum SVGImageFormat
    {
        /// <summary>
        /// Specifies the .png format.
        /// </summary>
        Png,

        /// <summary>
        /// Specifies the .jpg format.
        /// </summary>
        Jpeg
    }

    /// <summary>
    /// Drawing objects to a svg
    /// </summary>
    public class SvgGraphics : IGraphics
    {
        private XmlDocument xmlDocument;
        private Graphics graphics;
        private Bitmap internalImage;
        private XmlItem root;
        private SizeF sizeCache;
        private bool disposedValue = false; // To detect redundant calls
        private NumberFormatInfo numberFormat = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;
        private RectangleF viewBox;
        private RectangleF viewPort;
        private StringFormat measureFormat;
        private int imageId, patternId, clipId, imageFileId;
        private System.Drawing.Drawing2D.Matrix oldTransform;
        private Region oldClip;
        private bool needStateCheck;
        private Dictionary<Font, string> fontStyles;

        public XmlDocument XmlDocument { get { return xmlDocument; } }

        public SizeF Size
        {
            get { return sizeCache; }
            set
            {
                sizeCache = value;
                xmlDocument.Root.SetProp("width", GetString(value.Width));
                xmlDocument.Root.SetProp("height", GetString(value.Height));
            }
        }

        public RectangleF ViewBox
        {
            get { return viewBox; }
            set
            {
                viewBox = value;
                xmlDocument.Root.SetProp("viewBox", String.Format("{0} {1} {2} {3}",
                    GetString(value.Left),
                    GetString(value.Top),
                    GetString(value.Width),
                    GetString(value.Height)
                    ));
            }
        }

        public RectangleF ViewPort
        {
            get { return viewPort; }
            set
            {
                viewPort = value;
                xmlDocument.Root.SetProp("viewPort", String.Format("{0} {1} {2} {3}",
                    GetString(value.Left),
                    GetString(value.Top),
                    GetString(value.Width),
                    GetString(value.Height)
                    ));
            }
        }

        /// <summary>
        /// For setting namespace, clear all attributes on setting, therefore use this property before setting other svg options
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Attributes
        {
            get
            {
                foreach (XmlProperty prop in xmlDocument.Root.Properties)
                {
                    yield return new KeyValuePair<string, string>(prop.Key, prop.Value);
                }
            }

            set
            {
                xmlDocument.Root.Properties = null;
                foreach (KeyValuePair<string, string> kv in value)
                    xmlDocument.Root.SetProp(kv.Key, kv.Value);
            }
        }

        public float DpiX => 96;
        public float DpiY => 96;

        // TODO: the following 4 properties
        public TextRenderingHint TextRenderingHint { get; set; }
        public InterpolationMode InterpolationMode { get; set; }
        public SmoothingMode SmoothingMode { get; set; }
        public CompositingQuality CompositingQuality { get; set; }

        public Graphics Graphics => graphics;

        public System.Drawing.Drawing2D.Matrix Transform 
        { 
            get 
            {
                needStateCheck = true;
                return graphics.Transform; 
            } 
            set 
            {
                needStateCheck = true;
                graphics.Transform = value; 
            } 
        }

        public GraphicsUnit PageUnit { get; set; }

        public bool IsClipEmpty => graphics.IsClipEmpty;

        public Region Clip
        {
            get 
            {
                needStateCheck = true;
                return graphics.Clip; 
            }
            set 
            {
                needStateCheck = true;
                graphics.Clip = value; 
            }
        }

        public bool EmbeddedImages { get; set; }

        public SVGImageFormat SvgImageFormat { get; set; }
        
        public string ImageFilePrefix { get; set; }

        /// <summary>
        /// Initialize a new Graphics for SVG, it's rendered to xml, layer by layer, not one image,
        /// set the Size of this graphics in Size property
        /// </summary>
        public SvgGraphics(XmlDocument xmlDocument)
        {
            this.xmlDocument = xmlDocument;
            root = this.xmlDocument.Root;
            root.Name = "svg";
            root.SetProp("xmlns", "http://www.w3.org/2000/svg");

            // create "style" and "defs" items at the top of document
            root.FindItem("style").SetProp("type", "text/css");
            root.FindItem("defs");

            this.internalImage = new Bitmap(1, 1);
            this.graphics = Graphics.FromImage(internalImage);
            this.measureFormat = StringFormat.GenericTypographic;
            oldTransform = new System.Drawing.Drawing2D.Matrix();
            oldClip = new Region();
            fontStyles = new Dictionary<Font, string>();
            needStateCheck = true;
            EmbeddedImages = true;
            ImageFilePrefix = "Image";
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (graphics != null)
                        graphics.Dispose();
                    graphics = null;
                    if (internalImage != null)
                        internalImage.Dispose();
                    internalImage = null;
                    if (oldTransform != null)
                        oldTransform.Dispose();
                    oldTransform = null;
                    if (oldClip != null)
                        oldClip.Dispose();
                    oldClip = null;
                }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SVGGraphicsRenderer() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #region Internal methods
        private string GetString(float value)
        {
            return Math.Round(value, 2).ToString(numberFormat);
        }

        private float SpaceWidth(Font drawFont)
        {
            return graphics.MeasureString("1 2", drawFont, int.MaxValue, measureFormat).Width -
                graphics.MeasureString("12", drawFont, int.MaxValue, measureFormat).Width;
        }

        private string GetStringAlpha(Color color)
        {
            return GetString(color.A / 255f);
        }

        private string GetStringRGB(Color color)
        {
            return ColorTranslator.ToHtml(color);
        }

        private XmlItem AddItem(string name)
        {
            // these comparisons are quite expensive, try to avoid them with needStateCheck
            bool transformChanged = needStateCheck ? !oldTransform.Equals(Transform) : false;
            bool clipChanged = needStateCheck ? !oldClip.Equals(Clip, graphics) : false;

            if (transformChanged || clipChanged)
            {
                root = xmlDocument.Root.Add();
                root.Name = "g";
                // text positioning compatible with .net (commented out due to problems in some SVG readers). Replaced with top offset in AddString
                //root.SetProp("dominant-baseline", "text-before-edge");

                // transform
                float[] m = Transform.Elements;
                if (m.Length == 6 && (m[0] != 1f || m[1] != 0f || m[2] != 0f || m[3] != 1f || m[4] != 0f || m[5] != 0f))
                {
                    root.SetProp("transform", String.Format("matrix({0},{1},{2},{3},{4},{5})",
                        GetString(m[0]), GetString(m[1]), GetString(m[2]), GetString(m[3]), GetString(m[4]), GetString(m[5])));
                }

                // clip
                if (!Clip.IsInfinite(graphics))
                {
                    root.SetProp("clip-path", "url(#" + AddDefsClip() + ")");
                }

                oldTransform.Dispose();
                oldTransform = Transform.Clone();
                oldClip.Dispose();
                oldClip = Clip.Clone();
            }
            needStateCheck = false;

            XmlItem result = root.Add();
            result.Name = name;
            return result;
        }

        private string AddDefsClip()
        {
            System.Drawing.Drawing2D.Matrix identityMatrix = new System.Drawing.Drawing2D.Matrix();
            RectangleF[] clipRects = Clip.GetRegionScans(identityMatrix);
            identityMatrix.Dispose();

            XmlItem defsItem = xmlDocument.Root.FindItem("defs");
            
            // search for existing clip
            if (clipRects.Length == 1)
            {
                string xStr = GetString(clipRects[0].X);
                string yStr = GetString(clipRects[0].Y);
                string widthStr = GetString(clipRects[0].Width);
                string heightStr = GetString(clipRects[0].Height);

                foreach (XmlItem item in defsItem.Items)
                {
                    if (item.Name == "clipPath" && item.Count == 1)
                    {
                        if (item[0].GetProp("x") == xStr &&
                            item[0].GetProp("y") == yStr &&
                            item[0].GetProp("width") == widthStr &&
                            item[0].GetProp("height") == heightStr)
                            return item.GetProp("id");
                    }
                }
            }

            clipId++;
            string clipIdStr = "clip" + clipId.ToString();

            XmlItem clipItem = defsItem.Add();
            clipItem.Name = "clipPath";
            clipItem.SetProp("id", clipIdStr);

            foreach (RectangleF rect in clipRects)
            {
                XmlItem clipRectItem = clipItem.Add();
                clipRectItem.Name = "rect";
                clipRectItem.SetProp("x", GetString(rect.X));
                clipRectItem.SetProp("y", GetString(rect.Y));
                clipRectItem.SetProp("width", GetString(rect.Width));
                clipRectItem.SetProp("height", GetString(rect.Height));
            }

            return clipIdStr;
        }

        private string AddDefsImage(Image bmp, float width, float height)
        {
            string href = "";
            using (MemoryStream stream = new MemoryStream())
            {
                bmp.Save(stream, ImageFormat.Png);
                href = "data:image/png; base64," + Convert.ToBase64String(stream.ToArray());
            }
            string widthStr = GetString(width);
            string heightStr = GetString(height);

            XmlItem defsItem = xmlDocument.Root.FindItem("defs");
            
            // search for existing image
            foreach (XmlItem item in defsItem.Items)
            {
                if (item.Name == "image")
                {
                    if (item.GetProp("width") == widthStr &&
                        item.GetProp("height") == heightStr &&
                        item.GetProp("href") == href)
                        return item.GetProp("id");
                }
            }

            imageId++;
            string result = "img" + imageId.ToString();

            XmlItem imgItem = defsItem.Add();
            imgItem.Name = "image";
            imgItem.SetProp("id", result);
            imgItem.SetProp("width", widthStr);
            imgItem.SetProp("height", heightStr);
            imgItem.SetProp("href", href);

            return result;
        }

        private string AddDefsImagePattern(Image bmp, float x, float y, float width, float height)
        {
            string xStr = GetString(x);
            string yStr = GetString(y);
            string widthStr = GetString(width);
            string heightStr = GetString(height);
            string imgId = AddDefsImage(bmp, width, height);

            XmlItem defsItem = xmlDocument.Root.FindItem("defs");
            
            // search for existing pattern
            foreach (XmlItem item in defsItem.Items)
            {
                if (item.Name == "pattern")
                {
                    if (item.GetProp("x") == xStr &&
                        item.GetProp("y") == yStr &&
                        item.GetProp("width") == widthStr &&
                        item.GetProp("height") == heightStr)
                    {
                        if (item.Count == 1 && item.Items[0].Name == "use" && item.Items[0].GetProp("href") == "#" + imgId)
                            return item.GetProp("id");
                    }
                }
            }

            patternId++;
            string result = "p" + patternId.ToString();

            XmlItem pItem = defsItem.Add();
            pItem.Name = "pattern";
            pItem.SetProp("id", result);
            pItem.SetProp("patternUnits", "userSpaceOnUse");
            pItem.SetProp("x", xStr);
            pItem.SetProp("y", yStr);
            pItem.SetProp("width", widthStr);
            pItem.SetProp("height", heightStr);
            XmlItem imgItem = pItem.Add();
            imgItem.Name = "use";
            imgItem.SetProp("href", "#" + imgId);

            return result;
        }

        private void AddStroke(List<XmlProperty> properties, Pen pen)
        {
            if (pen != null)
            {
                if (pen.Width != 1)
                    properties.Add(XmlProperty.Create("stroke-width", GetString(pen.Width)));
                properties.Add(XmlProperty.Create("stroke", GetStringRGB(pen.Color)));
                if (pen.Color.A < 255)
                    properties.Add(XmlProperty.Create("stroke-opacity", GetStringAlpha(pen.Color)));

                if (pen.DashStyle != DashStyle.Solid)
                {
                    string strokeDashArray = "";
                    foreach (float f in pen.DashPattern)
                    {
                        strokeDashArray += GetString(f * pen.Width) + " ";
                    }
                    strokeDashArray = strokeDashArray.Trim();
                    properties.Add(XmlProperty.Create("stroke-dasharray", strokeDashArray));
                }
            }
        }

        private void AddFill(List<XmlProperty> properties, Brush brush)
        {
            if (brush == null)
            {
                properties.Add(XmlProperty.Create("fill", "none"));
            }
            else if (brush is SolidBrush)
            {
                SolidBrush b = brush as SolidBrush;
                if (b.Color != Color.Black)
                {
                    properties.Add(XmlProperty.Create("fill", GetStringRGB(b.Color)));
                    if (b.Color.A < 255)
                        properties.Add(XmlProperty.Create("fill-opacity", GetStringAlpha(b.Color)));
                }
            }
            else if (brush is LinearGradientBrush || brush is PathGradientBrush)
            {
                RectangleF rect = new RectangleF();
                WrapMode wrapMode = WrapMode.Tile;
                
                if (brush is LinearGradientBrush)
                {
                    rect = (brush as LinearGradientBrush).Rectangle;
                    wrapMode = (brush as LinearGradientBrush).WrapMode;
                }
                else if (brush is PathGradientBrush)
                {
                    rect = (brush as PathGradientBrush).Rectangle;
                    wrapMode = (brush as PathGradientBrush).WrapMode;
                }

                switch (wrapMode)
                {
                    case WrapMode.TileFlipX:
                        rect = new RectangleF(rect.X, rect.Y, rect.Width * 2, rect.Height);
                        break;
                    case WrapMode.TileFlipY:
                        rect = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height * 2);
                        break;
                    case WrapMode.TileFlipXY:
                        rect = new RectangleF(rect.X, rect.Y, rect.Width * 2, rect.Height * 2);
                        break;
                }

                using (Bitmap bmp = new Bitmap((int)rect.Width, (int)rect.Height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.Transparent);
                        g.TranslateTransform(-rect.X, -rect.Y);
                        g.FillRectangle(brush, rect);
                    }

                    properties.Add(XmlProperty.Create("fill", "url(#" + AddDefsImagePattern(bmp, rect.X, rect.Y, rect.Width, rect.Height) + ")"));
                }
            }
            else if (brush is HatchBrush)
            {
                HatchBrush b = brush as HatchBrush;
                using (Bitmap bmp = new Bitmap(8, 8))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.Transparent); 
                        g.FillRectangle(b, 0, 0, 8, 8);
                    }
                    
                    properties.Add(XmlProperty.Create("fill", "url(#" + AddDefsImagePattern(bmp, 0, 0, 8, 8) + ")"));
                }
            }
            else if (brush is TextureBrush)
            {
                // TODO wrap mode
                TextureBrush b = brush as TextureBrush;
                if (b.Image != null)
                    properties.Add(XmlProperty.Create("fill", "url(#" + AddDefsImagePattern(b.Image, 0, 0, b.Image.Width, b.Image.Height) + ")"));
            }
        }

        private void AddEllipse(Pen pen, Brush brush, float left, float top, float width, float height)
        {
            XmlItem obj = AddItem("ellipse");
            List<XmlProperty> properties = new List<XmlProperty>();
            properties.Add(XmlProperty.Create("cx", GetString(left + width / 2)));
            properties.Add(XmlProperty.Create("cy", GetString(top + height / 2)));
            properties.Add(XmlProperty.Create("rx", GetString(width / 2)));
            properties.Add(XmlProperty.Create("ry", GetString(height / 2)));
            AddStroke(properties, pen);
            AddFill(properties, brush);
            obj.Properties = properties.ToArray();
        }

        private void AddRectangle(Pen pen, Brush brush, float left, float top, float width, float height)
        {
            XmlItem obj = AddItem("rect");
            List<XmlProperty> properties = new List<XmlProperty>();
            properties.Add(XmlProperty.Create("x", GetString(left)));
            properties.Add(XmlProperty.Create("y", GetString(top)));
            properties.Add(XmlProperty.Create("width", GetString(width)));
            properties.Add(XmlProperty.Create("height", GetString(height)));
            AddStroke(properties, pen);
            AddFill(properties, brush);
            obj.Properties = properties.ToArray();
        }

        private void AddPolygon(Pen pen, Brush brush, PointF[] points)
        {
            if (points.Length == 0)
                return;

            XmlItem obj = AddItem("polygon");
            List<XmlProperty> properties = new List<XmlProperty>();
            StringBuilder sbPoints = new StringBuilder();
            foreach (PointF point in points)
                sbPoints.Append(GetString(point.X)).Append(" ").Append(GetString(point.Y)).Append(" ");
            if (sbPoints.Length > 0)
                sbPoints.Length--;
            properties.Add(XmlProperty.Create("points", sbPoints.ToString()));
            AddStroke(properties, pen);
            AddFill(properties, brush);
            obj.Properties = properties.ToArray();
        }

        private void AddPath(Pen pen, Brush brush, GraphicsPath path)
        {
            if (path.PointCount == 0)
                return;

            XmlItem obj = AddItem("path");
            List<XmlProperty> properties = new List<XmlProperty>();
            StringBuilder sbd = new StringBuilder();
            byte[] types = path.PathTypes;
            PointF[] points = path.PathPoints;
            int c_count = 0;

            for (int i = 0; i < points.Length; i++)
            {
                byte t = types[i];
                if ((t & 0x7) == 0)
                {
                    // MoveTo command
                    c_count = 0;
                    sbd.Append("M").Append(GetString(points[i].X)).Append(",").Append(GetString(points[i].Y)).Append(" ");
                }
                else if ((t & 0x7) == 1)
                {
                    // LineTo command
                    c_count = 0;
                    sbd.Append("L").Append(GetString(points[i].X)).Append(",").Append(GetString(points[i].Y)).Append(" ");
                }
                else if ((t & 0x7) == 3)
                {
                    // Cubic bezier curve command has 3 points. The C symbol must be added once at the start of sequence
                    if (c_count == 0)
                        sbd.Append("C");
                    c_count++;
                    if (c_count >= 3)
                        c_count = 0;
                    sbd.Append(GetString(points[i].X)).Append(",").Append(GetString(points[i].Y)).Append(" ");
                }
                if ((t & 0x80) != 0)
                {
                    // Close figure flag
                    sbd.Append("z ");
                }
            }
            if (sbd.Length > 0) 
                sbd.Length--;
            
            properties.Add(XmlProperty.Create("d", sbd.ToString()));
            AddStroke(properties, pen);
            AddFill(properties, brush);
            obj.Properties = properties.ToArray();
        }

        private void AddFontStyle(List<XmlProperty> properties, Font font)
        {
            if (!fontStyles.ContainsKey(font))
            {
                fontStyles.Add(font, "st" + fontStyles.Count.ToString());

                string styles = "\r\n  ";
                foreach (KeyValuePair<Font, string> kv in fontStyles)
                {
                    styles += "  ." + kv.Value + "{" +
                        "font-family:" + kv.Key.Name + ";" +
                        "font-size:" + GetString(kv.Key.Size / 0.75f) + "px;";
                    if ((kv.Key.Style & FontStyle.Italic) == FontStyle.Italic)
                        styles += "font-style:italic;";
                    if ((kv.Key.Style & FontStyle.Bold) == FontStyle.Bold)
                        styles += "font-weight:bold;";
                    string decoration = "";
                    if ((kv.Key.Style & FontStyle.Strikeout) != 0)
                        decoration = "line-through";
                    if ((kv.Key.Style & FontStyle.Underline) != 0)
                        decoration += " underline";
                    if (decoration != "")
                        styles += "text-decoration:" + decoration + ";";
                    styles += "}\r\n  ";
                }

                xmlDocument.Root.FindItem("style").Value = styles;
            }

            properties.Add(XmlProperty.Create("class", fontStyles[font]));
        }

        private void AddString(string text, Font font, Brush brush, float left, float top, string anchor, string direction)
        {
            XmlItem obj = AddItem("text");
            List<XmlProperty> properties = new List<XmlProperty>();
            properties.Add(XmlProperty.Create("x", GetString(left + SpaceWidth(font) / 2)));
            properties.Add(XmlProperty.Create("y", GetString(top + font.Height * 0.75f)));
            if (anchor != "")
                properties.Add(XmlProperty.Create("text-anchor", anchor));
            if (direction != "")
                properties.Add(XmlProperty.Create("direction", direction));
            AddFontStyle(properties, font);
            AddFill(properties, brush);
            
            obj.Value = text;
            obj.Properties = properties.ToArray();
        }

        #endregion

        #region Draw and measure text
        public void DrawString(string text, Font font, Brush brush, float left, float top)
        {
            AddString(text, font, brush, left, top, "", "");
        }

        public void DrawString(string text, Font font, Brush brush, RectangleF rect, StringFormat format)
        {
            // TODO: trimming
            if ((format.FormatFlags & StringFormatFlags.NoWrap) == 0)
                format.Trimming = StringTrimming.Word;
            float lineHeight = font.GetHeight(graphics);
            SizeF bound = new SizeF(rect.Width, lineHeight + 1);

            List<string> lines = new List<string>();
            string[] paragraphs = text.Split(new char[] { '\n' });
            for (int i = 0; i < paragraphs.Length; i++)
            {
                string s = paragraphs[i];
                if (s.Length > 0 && s[s.Length - 1] == '\r')
                    s = s.Remove(s.Length - 1);

                while (s.Length > 0)
                {
                    int charFill;
                    int linesFill;
                    graphics.MeasureString(s, font, bound, format, out charFill, out linesFill);
                    if (linesFill == 0)
                        break;
                    lines.Add(s.Substring(0, charFill));
                    s = s.Substring(charFill);
                }
            }

            float dx = 0;
            string anchor = "";
            if (format.Alignment == StringAlignment.Center)
            {
                dx = (rect.Width - SpaceWidth(font)) / 2;
                anchor = "middle";
            }
            else if (format.Alignment == StringAlignment.Far)
            {
                dx = rect.Width - SpaceWidth(font);
                anchor = "end";
            }

            float dy = 0;
            if (format.LineAlignment == StringAlignment.Center)
            {
                dy = (rect.Height - lines.Count * lineHeight) / 2;
            }
            else if (format.LineAlignment == StringAlignment.Far)
            {
                dy = rect.Height - lines.Count * lineHeight;
            }

            string direction = "";
            if ((format.FormatFlags & StringFormatFlags.DirectionRightToLeft) != 0)
            {
                direction = "rtl";
                if (format.Alignment == StringAlignment.Far)
                {
                    dx = 0;
                    anchor = "end";
                }
                else if (format.Alignment == StringAlignment.Near)
                {
                    dx = rect.Width - SpaceWidth(font);
                    anchor = "";
                }
            }

            for (int i = 0; i < lines.Count; i++)
            {
                AddString(lines[i], font, brush, rect.Left + dx, rect.Top + dy + lineHeight * i, anchor, direction);
            }
        }

        public void DrawString(string text, Font font, Brush brush, RectangleF rect)
        {
            DrawString(text, font, brush, rect, StringFormat.GenericDefault);
        }

        public void DrawString(string text, Font font, Brush brush, float left, float top, StringFormat format)
        {
            DrawString(text, font, brush, new RectangleF(left, top, 0, 0), format);
        }

        public void DrawString(string text, Font font, Brush brush, PointF point, StringFormat format)
        {
            DrawString(text, font, brush, point.X, point.Y, format);
        }

        public Region[] MeasureCharacterRanges(string text, Font font, RectangleF textRect, StringFormat format)
        {
            return this.graphics.MeasureCharacterRanges(text, font, textRect, format);
        }

        public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat format)
        {
            return this.graphics.MeasureString(text, font, layoutArea, format);
        }

        public SizeF MeasureString(string text, Font font)
        {
            return this.graphics.MeasureString(text, font);
        }

        public SizeF MeasureString(string text, Font font, SizeF size)
        {
            return this.graphics.MeasureString(text, font, size);
        }

        public SizeF MeasureString(string text, Font font, int width, StringFormat format)
        {
            return this.graphics.MeasureString(text, font, width, format);
        }

        public void MeasureString(string text, Font font, SizeF size, StringFormat format, out int charsFit, out int linesFit)
        {
            this.graphics.MeasureString(text, font, size, format, out charsFit, out linesFit);
        }
        #endregion

        #region Draw images
        public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)
        {
            if (image == null || image.Width == 0 || image.Height == 0)
                return;
            
            if (srcRect.X != 0 || srcRect.Y != 0 || srcRect.Width != image.Width || srcRect.Height != image.Height)
            {
                // crop source image
                using (Bitmap newImage = new Bitmap((int)srcRect.Width, (int)srcRect.Height))
                {
                    using (Graphics g = Graphics.FromImage(newImage))
                    {
                        g.Clear(Color.Transparent);
                        g.DrawImageUnscaled(image, 0, 0);
                    }
                    DrawImage(newImage, destRect);
                }
            }
            else
            {
                DrawImage(image, destRect);
            }
        }

        public void DrawImage(Image image, RectangleF rect)
        {
            DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawImage(Image image, float x, float y, float width, float height)
        {
            if (image == null || image.Width == 0 || image.Height == 0)
                return;

            XmlItem obj = AddItem("image");
            List<XmlProperty> properties = new List<XmlProperty>();
            properties.Add(XmlProperty.Create("x", GetString(x)));
            properties.Add(XmlProperty.Create("y", GetString(y)));
            properties.Add(XmlProperty.Create("width", GetString(width)));
            properties.Add(XmlProperty.Create("height", GetString(height)));

            string href = "";
            if (EmbeddedImages)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    image.Save(stream, SvgImageFormat == SVGImageFormat.Png ? ImageFormat.Png : ImageFormat.Jpeg);
                    href = "data:image/" + (SvgImageFormat == SVGImageFormat.Png ? "png" : "jpg") + "; base64," + Convert.ToBase64String(stream.ToArray());
                }
            }
            else
            {
                href = ImageFilePrefix + "." + imageFileId.ToString() + "." + (SvgImageFormat == SVGImageFormat.Png ? "png" : "jpg");
                image.Save(href);
                href = Path.GetFileName(href);
                imageFileId++;
            }
            properties.Add(XmlProperty.Create("href", href));

            obj.Properties = properties.ToArray();
        }

        public void DrawImage(Image image, PointF[] points)
        {
            if (image == null || image.Width == 0 || image.Height == 0)
                return;
            if (points.Length != 3)
                return;

            PointF p0 = points[0];
            PointF p1 = points[1];
            PointF p2 = points[2];

            RectangleF rect = new RectangleF(0, 0, image.Width, image.Height);
            float m11 = (p1.X - p0.X) / rect.Width;
            float m12 = (p1.Y - p0.Y) / rect.Width;
            float m21 = (p2.X - p0.X) / rect.Height;
            float m22 = (p2.Y - p0.Y) / rect.Height;
            IGraphicsState state = Save();
            MultiplyTransform(new System.Drawing.Drawing2D.Matrix(m11, m12, m21, m22, p0.X, p0.Y), MatrixOrder.Prepend);
            DrawImage(image, rect);
            Restore(state);
        }

        public void DrawImage(Image image, float x, float y)
        {
            if (image == null)
                return;
            DrawImage(image, x, y, image.Width, image.Height);
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
        {
            DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr);
        }

        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
        {
            using (Image newImage = new Bitmap(image.Width, image.Height))
            {
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    g.Clear(Color.Transparent);
                    g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, srcUnit, imageAttrs);
                }
                DrawImage(newImage, destRect, new RectangleF(srcX, srcY, srcWidth, srcHeight), srcUnit);
            }
        }

        public void DrawImageUnscaled(Image image, Rectangle rect)
        {
            if (image == null)
                return;
            DrawImage(image, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, null);
        }

        #endregion

        #region Draw geometry
        public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(x, y, width, height, startAngle, sweepAngle);
                AddPath(pen, null, path);
            }
        }

        public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddCurve(points, offset, numberOfSegments, tension);
                AddPath(pen, null, path);
            }
        }

        public void DrawEllipse(Pen pen, float left, float top, float width, float height)
        {
            AddEllipse(pen, null, left, top, width, height);
        }

        public void DrawEllipse(Pen pen, RectangleF rect)
        {
            DrawEllipse(pen, rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            //TODO add caps
            XmlItem obj = AddItem("line");
            List<XmlProperty> properties = new List<XmlProperty>();
            properties.Add(XmlProperty.Create("x1", GetString(x1)));
            properties.Add(XmlProperty.Create("y1", GetString(y1)));
            properties.Add(XmlProperty.Create("x2", GetString(x2)));
            properties.Add(XmlProperty.Create("y2", GetString(y2)));
            AddStroke(properties, pen);
            obj.Properties = properties.ToArray();
        }

        public void DrawLine(Pen pen, PointF p1, PointF p2)
        {
            DrawLine(pen, p1.X, p1.Y, p2.X, p2.Y);
        }

        public void DrawLines(Pen pen, PointF[] points)
        {
            if (points.Length < 2)
                return;
            for (int i = 0; i < points.Length - 1; i++)
            {
                DrawLine(pen, points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y);
            }
        }

        public void DrawPath(Pen outlinePen, GraphicsPath path)
        {
            AddPath(outlinePen, null, path);
        }

        public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPie(x, y, width, height, startAngle, sweepAngle);
                AddPath(pen, null, path);
            }
        }

        public void DrawPolygon(Pen pen, PointF[] points)
        {
            AddPolygon(pen, null, points);
        }

        public void DrawPolygon(Pen pen, Point[] points)
        {
            if (points.Length == 0)
                return;
            PointF[] pointsF = new PointF[points.Length];
            for (int i = 0; i < points.Length; i++)
                pointsF[i] = points[i];
            DrawPolygon(pen, pointsF);
        }

        public void DrawRectangle(Pen pen, float left, float top, float width, float height)
        {
            AddRectangle(pen, null, left, top, width, height);
        }

        public void DrawRectangle(Pen pen, Rectangle rect)
        {
            DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
        }
        #endregion

        #region Fill geometry
        public void FillEllipse(Brush brush, float left, float top, float width, float height)
        {
            AddEllipse(null, brush, left, top, width, height);
        }

        public void FillEllipse(Brush brush, RectangleF rect)
        {
            FillEllipse(brush, rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public void FillPath(Brush brush, GraphicsPath path)
        {
            AddPath(null, brush, path);
        }

        public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPie(x, y, width, height, startAngle, sweepAngle);
                AddPath(null, brush, path);
            }
        }

        public void FillPolygon(Brush brush, PointF[] points)
        {
            AddPolygon(null, brush, points);
        }

        public void FillPolygon(Brush brush, Point[] points)
        {
            if (points.Length == 0)
                return;
            PointF[] pointsF = new PointF[points.Length];
            for (int i = 0; i < points.Length; i++)
                pointsF[i] = points[i];
            FillPolygon(brush, pointsF);
        }

        public void FillRectangle(Brush brush, float left, float top, float width, float height)
        {
            AddRectangle(null, brush, left, top, width, height);
        }

        public void FillRectangle(Brush brush, RectangleF rect)
        {
            FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public void FillRegion(Brush brush, Region region)
        {
            // TODO: check this case
            //throw new NotImplementedException();
        }

        #endregion

        #region Transform
        public void MultiplyTransform(System.Drawing.Drawing2D.Matrix matrix, MatrixOrder order)
        {
            needStateCheck = true;
            graphics.MultiplyTransform(matrix, order);
        }

        public void ScaleTransform(float scaleX, float scaleY)
        {
            needStateCheck = true;
            graphics.ScaleTransform(scaleX, scaleY);
        }

        public void TranslateTransform(float left, float top)
        {
            needStateCheck = true;
            graphics.TranslateTransform(left, top);
        }

        public void RotateTransform(float angle)
        {
            needStateCheck = true;
            graphics.RotateTransform(angle);
        }

        #endregion

        #region State
        public IGraphicsState Save()
        {
            return new SvgGraphicsState(graphics.Save());
        }

        public void Restore(IGraphicsState state)
        {
            needStateCheck = true;
            if (state is SvgGraphicsState)
            {
                graphics.Restore((state as SvgGraphicsState).State);
            }
        }

        #endregion

        #region Clip
        public bool IsVisible(RectangleF rect)
        {
            return true;// measureGraphics.IsVisible(rect);
        }

        public void ResetClip()
        {
            needStateCheck = true;
            graphics.ResetClip();
        }

        public void SetClip(RectangleF rect)
        {
            needStateCheck = true;
            graphics.SetClip(rect);
        }

        public void SetClip(RectangleF rect, CombineMode combineMode)
        {
            needStateCheck = true;
            graphics.SetClip(rect, combineMode);
        }

        public void SetClip(GraphicsPath path, CombineMode combineMode)
        {
            needStateCheck = true;
            graphics.SetClip(path, combineMode);
        }

        #endregion


        public class SvgGraphicsState : IGraphicsState
        {
            public GraphicsState State { get; }

            public SvgGraphicsState(GraphicsState state)
            {
                State = state;
            }
        }
    }
}
