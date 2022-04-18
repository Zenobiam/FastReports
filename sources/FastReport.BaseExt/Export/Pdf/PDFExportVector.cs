using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;

namespace FastReport.Export.Pdf
{
    public partial class PDFExport
    {
        #region Private Fields
        private CurvesInterpolationEnum curvesInterpolation = CurvesInterpolationEnum.Curves;
        private CurvesInterpolationEnum curvesInterpolationText = CurvesInterpolationEnum.Curves;
        private Dictionary<HashableByteArray, long> gradientHashSet;
        private GradientInterpolationPointsEnum gradientInterpolationPoints = GradientInterpolationPointsEnum.P128;
        private GradientQualityEnum gradientQuality = GradientQualityEnum.Medium;
        private List<long> pageAlphaShading;
        private bool svgAsPicture;

        //private int FPageShadingNumber;
        private List<long> pageShadings;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// The interpolation of curves (svg)
        /// </summary>
        public CurvesInterpolationEnum CurvesInterpolation
        {
            get
            {
                return curvesInterpolation;
            }
            set
            {
                curvesInterpolation = value;
            }
        }

        /// <summary>
        /// The interpolation of curves (text)
        /// </summary>
        public CurvesInterpolationEnum CurvesInterpolationText
        {
            get
            {
                return curvesInterpolationText;
            }
            set
            {
                curvesInterpolationText = value;
            }
        }

        /// <summary>
        /// Export svg object as image, not vector
        /// </summary>
        public bool SvgAsPicture
        {
            get { return svgAsPicture; }
            set { svgAsPicture = value; }
        }

        /// <summary>
        /// Gradient interpolation, high value will lead beautiful the gradient,
        /// but the file size will increase and the speed of work will decrease.
        /// </summary>
        public GradientInterpolationPointsEnum GradientInterpolationPoints
        {
            get
            {
                return gradientInterpolationPoints;
            }
            set
            {
                gradientInterpolationPoints = value;
            }
        }

        /// <summary>
        /// The quality of gradient, export as image or export as gradient grid
        /// </summary>
        public GradientQualityEnum GradientQuality
        {
            get
            {
                return gradientQuality;
            }
            set
            {
                gradientQuality = value;
            }
        }

        #endregion Public Properties

        #region Private Methods

        private void AddPDFPolylineVector(PolyLineObject obj, StringBuilder sb)
        {
            GraphicsPath path = obj.GetPath(null, 0, 0, obj.Width, obj.Height, 1, 1);
            SizeF rect = new SizeF(obj.Width, obj.Height);
            if (obj is PolygonObject)
            {
                
                FillPDFGraphicsPath(rect, path, obj.Fill.CreateBrush(new RectangleF(PointF.Empty,rect)), sb, curvesInterpolation,
                    new System.Drawing.Drawing2D.Matrix(PDF_DIVIDER, 0, 0, -PDF_DIVIDER, GetLeft(obj.AbsLeft), GetTop(obj.AbsTop)));
                StrokePDFGraphicsPath(rect, path, obj.Border, true, sb, curvesInterpolation,
                    new System.Drawing.Drawing2D.Matrix(PDF_DIVIDER, 0, 0, -PDF_DIVIDER, GetLeft(obj.AbsLeft), GetTop(obj.AbsTop)));
            }
            else
            {
                StrokePDFGraphicsPath(rect, path, obj.Border, false, sb, curvesInterpolation,
                    new System.Drawing.Drawing2D.Matrix(PDF_DIVIDER, 0, 0, -PDF_DIVIDER, GetLeft(obj.AbsLeft), GetTop(obj.AbsTop)));
            }
        }

        private void AppendPDFGraphicsPath(GraphicsPath path, StringBuilder sb, CurvesInterpolationEnum curvesInterpolation)
        {

            PointF[] ps = path.PathPoints;
            byte[] pt = path.PathTypes;
            PointF startPoint = ps.Length > 0 ? ps[0] : new PointF();
            int interpolation = (int)curvesInterpolation;
            for (int i = 0; i < ps.Length; i++)
            {
                if ((pt[i] & 1) == 1)
                {
                    if ((pt[i] & 3) == 3 && i < ps.Length - 2)
                    {
                        if (interpolation > 0)
                        {
                            for (int dt = 1; dt <= interpolation; dt++)
                                AppendPDFGraphicsPathBezierInterpolate(ps[i - 1], ps[i], ps[i + 1], ps[i + 2], (float)dt / interpolation, sb);
                        }
                        else
                        sb.Append(FloatToString(ps[i].X)).Append(" ").Append(FloatToString(ps[i].Y)).Append(" ")
                          .Append(FloatToString(ps[i+1].X)).Append(" ").Append(FloatToString(ps[i+1].Y)).Append(" ")
                          .Append(FloatToString(ps[i+2].X)).Append(" ").Append(FloatToString(ps[i+2].Y)).Append(" c ");
                        i += 2;
                    }
                    else sb.Append(FloatToString(ps[i].X)).Append(" ").Append(FloatToString(ps[i].Y)).Append(" l ");
                }
                if (pt[i] == 0)
                    sb.Append(FloatToString(ps[i].X)).Append(" ").Append(FloatToString(ps[i].Y)).Append(" m ");

                //    switch (pt[i])
                //{
                //    case 0://start
                //        //sb.AppendLine();

                //        i++;
                //        break;

                //    case 1://line

                //        i++;
                //        break;

                //    //case 3://interpolate bezier
                //    //    for (float dt = 1; dt < 6; dt++)
                //    //        DrawPDFBezier(rect.Left, rect.Top, ps[i - 1], ps[i], ps[i + 1], ps[i + 2], dt / 5, sb);
                //    //    i += 3;
                //    //    break;

                //    default:
                //        i++;
                //        break;
                //}
                //fill
            }
            //DrawPDFBezier
            sb.AppendLine();
        }

        private void AppendPDFGraphicsPathBezierInterpolate(PointF p0, PointF p1, PointF p2, PointF p3, float t, StringBuilder sb)
        {
            float t1 = 1 - t;
            float px = t1 * t1 * t1 * p0.X + 3 * t1 * t1 * t * p1.X + 3 * t * t * t1 * p2.X + t * t * t * p3.X;
            float py = t1 * t1 * t1 * p0.Y + 3 * t1 * t1 * t * p1.Y + 3 * t * t * t1 * p2.Y + t * t * t * p3.Y;
            sb.Append(FloatToString(px)).Append(" ").Append(FloatToString(py)).Append(" l ");
        }

        private void DrawPDFVectorGradientFill(float left, float top, float width, float height, FillBase fill, StringBuilder sb)
        {
            unchecked
            {
                RectangleF rect = new RectangleF(0, 0, width, height);
                Brush brush = fill.CreateBrush(rect);
                sb.AppendLine("q");
                sb.Append("1 0 0 -1 ").Append(FloatToString(left)).Append(" ").Append(FloatToString(top)).AppendLine(" cm");
                DrawPDFVectorGradientFill(new SizeF(width, height), brush, sb);
                sb.AppendLine("Q");
            }
        }

        private void DrawPDFVectorGradientFill(SizeF size, Brush brush, StringBuilder sb)
        {
            if (gradientQuality != GradientQualityEnum.Image && ( brush is LinearGradientBrush || brush is PathGradientBrush ))
            {
                string shading = GetShadingBrush(size, brush);

                if (shading != null)
                {
                    sb.AppendLine("q");
                    sb.Append(FloatToString(size.Width)).Append(" 0 0 ").Append(FloatToString(-size.Height)).Append(" ").Append(FloatToString(0)).Append(" ").Append(FloatToString(size.Height)).AppendLine(" cm");
                    sb.AppendLine(shading);
                    sb.AppendLine("Q");
                }
            }
            else
            {
                string image = GetImageBrush(size, brush);
                sb.AppendLine("q");
                sb.Append(FloatToString(size.Width)).Append(" 0 0 ").Append(FloatToString(-size.Height)).Append(" ").Append(FloatToString(0)).Append(" ").Append(FloatToString(size.Height)).AppendLine(" cm");
                sb.AppendLine(image);
                sb.AppendLine("Q");
            }
        }

        /// <summary>
        /// Added graphics path to pdf,
        /// </summary>
        /// <param name="size">size of rect for gradient filling</param>
        /// <param name="path">path, with positions in pdf scaling</param>
        /// <param name="brush">Any brush</param>
        /// <param name="curvesInterpolation">Interpolation value</param>
        /// <param name="sb"></param>
        /// <param name="matrixTransform">matrix for transform to  pdf scale</param>
        private void FillPDFGraphicsPath(SizeF size, GraphicsPath path, Brush brush, StringBuilder sb, CurvesInterpolationEnum curvesInterpolation,
            System.Drawing.Drawing2D.Matrix matrixTransform)
        {
            if (path.PointCount == 0) return;
            sb.AppendLine("q");
            if (brush is SolidBrush)
                GetPDFFillColor((brush as SolidBrush).Color, sb);
            float[] m = matrixTransform.Elements;
            sb.Append(FloatToString(m[0])).Append(" ")
                .Append(FloatToString(m[1])).Append(" ")
                .Append(FloatToString(m[2])).Append(" ")
                .Append(FloatToString(m[3])).Append(" ")
                .Append(FloatToString(m[4])).Append(" ")
                .Append(FloatToString(m[5])).AppendLine(" cm");
            AppendPDFGraphicsPath(path, sb, curvesInterpolation);
            if (brush is SolidBrush)
                sb.AppendLine("f");
            else
            {
                sb.AppendLine("W n");
                DrawPDFVectorGradientFill(size, brush, sb);
            }
            sb.AppendLine("Q");
        }

        private string GetImageBrush(SizeF size, Brush brush)
        {
            float printZoom = printOptimized ? 4 : 1;
            int bitmapWidth = (int)Math.Round(size.Width * printZoom);
            int bitmapHeight = (int)Math.Round(size.Height * printZoom);

            // check for max bitmap object size
            {
                // 2GB (max .net object size) / 4 (Format32bppArgb is 4 bytes)
                // see http://stackoverflow.com/a/29175905/4667434
                const ulong maxPixels = 536870912;

                if ((ulong)bitmapWidth * (ulong)bitmapHeight >= maxPixels)
                {
                    bitmapWidth = (int)size.Width;
                    bitmapHeight = (int)size.Height;
                }

                if ((ulong)bitmapWidth * (ulong)bitmapHeight >= maxPixels)
                {
                    return null;
                }
            }

            using (Bitmap bmp = new Bitmap(bitmapWidth, bitmapHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Transparent);
                    //g.TranslateTransform(0, size.Height);
                    g.ScaleTransform(bitmapWidth / size.Width,  bitmapHeight / size.Height);
                    //g.TranslateTransform(-rect.Left, -rect.Top);

                    g.FillRectangle(brush, new RectangleF(PointF.Empty, size ));
                    g.Flush();
                }
                
                long imageIndex = AppendPDFImage(bmp, jpegQuality);
                if (imageIndex < 0) return null;
                AddImageToList(imageIndex);
                return ExportUtils.StringFormat("/Im{0} Do", imageIndex);
            }
        }

        private string GetShadingBrush(SizeF size, Brush brush)
        {
            if (gradientQuality == GradientQualityEnum.Image) return null;
            //RectangleF rect;
            //if (brush is LinearGradientBrush)
            //    rect = (brush as LinearGradientBrush).Rectangle;
            //else if (brush is PathGradientBrush)
            //    rect = (brush as PathGradientBrush).Rectangle;
            //else return -1;

            int points = (int)gradientInterpolationPoints;
            int bmpWidth = (int)gradientQuality;
            if (points > bmpWidth) points = bmpWidth;
            using (Bitmap bmp = new Bitmap(bmpWidth, bmpWidth))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Transparent);
                    g.ScaleTransform(bmpWidth / size.Width, bmpWidth / size.Height);

                    g.FillRectangle(brush, new RectangleF(PointF.Empty, size));
                    g.Flush();
                }

                bool alpha;
                long pos;
                switch (ColorSpace)
                {
                    case PdfColorSpace.RGB:
                        pos = GetShadingBrushRGB(points, bmp, out alpha);
                        break;

                    case PdfColorSpace.CMYK:
                        pos = GetShadingBrushCMYK(points, bmp, out alpha);
                        break;

                    default:
                        throw new NotImplementedException("3a4d1849-8309-4452-ade3-173f21458fe1");
                }

                pageShadings.Add(pos);
                if (alpha)
                {
                    long maskPos = GetShadingBrushAlpha(size, points, bmp);
                    if (maskPos >= 0)
                    {
                        pageAlphaShading.Add(maskPos);
                        return ExportUtils.StringFormat("/s{0} gs /sh{1} sh", pageAlphaShading.Count, pageShadings.Count);
                    }
                    // /s6 << /ca 1 /Type /ExtGState /AIS false /SMask << /Type Mask /G << >> /S /Luminosity >> /CA 1 >>
                }

                //FPageShadings.Append(" /sh").Append(++FPageShadingNumber).Append(" ").Append(ObjNumberRef(pos));
                return ExportUtils.StringFormat("/sh{0} sh", pageShadings.Count);
            }
        }

        private long GetShadingBrushAlpha(SizeF size, int points, Bitmap bmp)
        {
            long posMask = GetShadingBrushGrayAlpha(points, bmp);
            long pos = UpdateXRef();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ObjNumber(pos));
            sb.AppendLine("<< /Group << /Type /Group /CS /DeviceGray /S /Transparency /I true >> ");
            sb.Append("/Type /XObject /Subtype /Form /FormType 1 /Resources << /Shading << /sh").Append(posMask).Append(" ")
                .Append(ObjNumberRef(posMask)).AppendLine(" >> /ExtGState << /a0 << /ca 1 /CA 1 >> >> >>");
            sb.Append("/BBox [0 0 ").Append(FloatToString(size.Width)).Append(" ").Append(FloatToString(size.Height)).AppendLine(" ]");
            //stream
            string stringStream = ExportUtils.StringFormat("/a0 gs /sh{0} sh", posMask);
            WriteLn(pdf, sb.ToString());
            using (MemoryStream tempContentStream = new MemoryStream())
            {
                Write(tempContentStream, stringStream);
                tempContentStream.Position = 0;
                WritePDFStream(pdf, tempContentStream, posMask, compressed, encrypted, false, true);
            }
            return pos;
        }

        private long GetShadingBrushCMYK(int size, Bitmap bmp, out bool alpha)
        {
            alpha = false;
            long pos = UpdateXRef();
            int bmpWidth = bmp.Width;
            int bmpHeight = bmp.Height;
            StringBuilder sh = new StringBuilder();
            sh.AppendLine(ObjNumber(pos));
            sh.Append("<< /ShadingType 5 /ColorSpace");
            sh.Append(" /DeviceCMYK");
            sh.Append(" /VerticesPerRow ").Append(size);// FGradientInterpolationPoints);
            sh.Append(" /BitsPerCoordinate 8");
            sh.Append(" /BitsPerComponent 8");
            sh.Append(" /Decode [0 1.0 0 1.0 0 1.0 0 1.0 0 1.0 0 1.0]");
            using (MemoryStream dataSource = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(dataSource))
                {
                    for (int j = size - 1; j >= 0; j--)
                        for (int i = 0; i < size; i++)
                        {
                            writer.Write((byte)(i * 255f / (size - 1)));
                            writer.Write((byte)((size - j - 1) * 255f / (size - 1)));
                            Color c = bmp.GetPixel(i * bmpWidth / size, j * bmpHeight / size);
                            if (c.A < 0xFF)
                                alpha = true;

                            float fred = ((float)c.R) / 255f;
                            float fgreen = ((float)c.G) / 255f;
                            float fblue = ((float)c.B) / 255f;

                            float fblack = 1 - Math.Max(fred, Math.Max(fgreen, fblue));
                            float fcyan = (1 - fred - fblack) / (1 - fblack);
                            float fmagenta = (1 - fgreen - fblack) / (1 - fblack);
                            float fyellow = (1 - fblue - fblack) / (1 - fblack);

                            byte black = (byte)(fblack * 255);
                            byte cyan = (byte)(fcyan * 255);
                            byte magenta = (byte)(fmagenta * 255);
                            byte yellow = (byte)(fyellow * 255);

                            writer.Write(cyan);
                            writer.Write(magenta);
                            writer.Write(yellow);
                            writer.Write(black);
                        }

                    writer.Flush();
                    dataSource.Position = 0;
                    HashableByteArray hashableByteArray = new HashableByteArray(dataSource.ToArray());
                    long pos2 = 0;
                    if (gradientHashSet.TryGetValue(hashableByteArray, out pos2))
                        return pos2;
                    else gradientHashSet[hashableByteArray] = pos;
                    WriteLn(pdf, sh.ToString());
                    WritePDFStream(pdf, dataSource, pageShadings.Count, true, encrypted, false, true);
                }
            }
            return pos;
        }

        private long GetShadingBrushGrayAlpha(int size, Bitmap bmp)
        {
            long pos = UpdateXRef();
            int bmpWidth = bmp.Width;
            int bmpHeight = bmp.Height;
            StringBuilder sh = new StringBuilder();
            sh.AppendLine(ObjNumber(pos));
            sh.Append("<< /ShadingType 5 /ColorSpace");
            sh.Append(" /DeviceGray");
            sh.Append(" /VerticesPerRow ").Append(size);// FGradientInterpolationPoints);
            sh.Append(" /BitsPerCoordinate 8");
            sh.Append(" /BitsPerComponent 8");
            sh.Append(" /Decode [0 1.0 0 1.0 0 1.0]");

            using (MemoryStream dataSource = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(dataSource))
                {
                    for (int j = size - 1; j >= 0; j--)
                        for (int i = 0; i < size; i++)
                        {
                            writer.Write((byte)(i * 255f / (size - 1)));
                            writer.Write((byte)((size - j - 1) * 255f / (size - 1)));
                            Color c = bmp.GetPixel(i * bmpWidth / size, j * bmpHeight / size);
                            writer.Write(c.A);
                        }
                    writer.Flush();
                    dataSource.Position = 0;
                    HashableByteArray hashableByteArray = new HashableByteArray(dataSource.ToArray());
                    long pos2 = 0;
                    if (gradientHashSet.TryGetValue(hashableByteArray, out pos2))
                        return pos2;
                    else gradientHashSet[hashableByteArray] = pos;
                    WriteLn(pdf, sh.ToString());
                    WritePDFStream(pdf, dataSource, pageAlphaShading.Count, true, encrypted, false, true);
                }
            }
            return pos;
        }

        private long GetShadingBrushRGB(int size, Bitmap bmp, out bool alpha)
        {
            alpha = false;
            long pos;
            int bmpWidth = bmp.Width;
            int bmpHeight = bmp.Height;

            using (MemoryStream dataSource = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(dataSource))
                {
                    for (int j = size - 1; j >= 0; j--)
                        for (int i = 0; i < size; i++)
                        {
                            writer.Write((byte)(i * 255f / (size - 1)));
                            writer.Write((byte)((size - j - 1) * 255f / (size - 1)));
                            Color c = bmp.GetPixel(i * bmpWidth / size, j * bmpHeight / size);
                            if (c.A < 0xFF)
                                alpha = true;
                            writer.Write(c.R);
                            writer.Write(c.G);
                            writer.Write(c.B);
                        }
                    writer.Flush();
                    dataSource.Position = 0;
                    HashableByteArray hashableByteArray = new HashableByteArray(dataSource.ToArray());
                    long pos2 = 0;
                    if (gradientHashSet.TryGetValue(hashableByteArray, out pos2))
                        return pos2;
                    else
                    {
                        pos = UpdateXRef();
                        gradientHashSet[hashableByteArray] = pos;

                        //begin init
                        
                        StringBuilder sh = new StringBuilder();
                        sh.AppendLine(ObjNumber(pos));
                        sh.Append("<< /ShadingType 5 /ColorSpace");
                        sh.Append(" /DeviceRGB");
                        sh.Append(" /VerticesPerRow ").Append(size);// FGradientInterpolationPoints);
                        sh.Append(" /BitsPerCoordinate 8");
                        sh.Append(" /BitsPerComponent 8");
                        sh.Append(" /Decode [0 1.0 0 1.0 0 1.0 0 1.0 0 1.0]");
                        //end

                        WriteLn(pdf, sh.ToString());
                        WritePDFStream(pdf, dataSource, pos, true, encrypted, false, true);
                    }
                }
            }
            return pos;
        }

        /// <summary>
        /// returns true if this gradient is fillable by gradient grid
        /// </summary>
        /// <returns></returns>
        private bool IsFillableGradientGrid(FillBase fillBase)
        {
            if (gradientQuality == GradientQualityEnum.Image)
                return false;
            if (fillBase is LinearGradientFill || fillBase is PathGradientFill)
                return true;
            return false;
        }

        private void StrokePDFGraphicsPath(SizeF rect, GraphicsPath path, Border border, bool closed, StringBuilder sb, CurvesInterpolationEnum curvesInterpolation,
            System.Drawing.Drawing2D.Matrix matrixTransform)
        {
            if (border.Color.A == 0)
                return;
            float[] m = matrixTransform.Elements;
            sb.AppendLine("q");
            sb.Append(FloatToString(m[0])).Append(" ")
                .Append(FloatToString(m[1])).Append(" ")
                .Append(FloatToString(m[2])).Append(" ")
                .Append(FloatToString(m[3])).Append(" ")
                .Append(FloatToString(m[4])).Append(" ")
                .Append(FloatToString(m[5])).AppendLine(" cm");
            GetPDFStrokeColor(border.Color, sb);
            //sb.Append(FloatToString(border.Width * PDF_DIVIDER)).AppendLine(" w").AppendLine("1 J");
            sb.Append(FloatToString(border.Width)).AppendLine(" w").AppendLine("0 J");
            sb.AppendLine(DrawPDFDash(border.Style, border.Width));
            AppendPDFGraphicsPath(path, sb, curvesInterpolation);
            if (closed)
                sb.AppendLine("s");
            else
                sb.AppendLine("S");
            sb.AppendLine("Q");
        }

        private void StrokePDFGraphicsPath(GraphicsPath path, Pen pen, bool closed, StringBuilder sb, CurvesInterpolationEnum curvesInterpolation,
            System.Drawing.Drawing2D.Matrix matrixTransform)
        {
            try { if (pen.Color.A == 0) return; }
            catch { return; }
            float[] m = matrixTransform.Elements;
            sb.AppendLine("q");
            sb.Append(FloatToString(m[0])).Append(" ")
                .Append(FloatToString(m[1])).Append(" ")
                .Append(FloatToString(m[2])).Append(" ")
                .Append(FloatToString(m[3])).Append(" ")
                .Append(FloatToString(m[4])).Append(" ")
                .Append(FloatToString(m[5])).AppendLine(" cm");
            GetPDFStrokeColor(pen.Color, sb);

            sb.Append(FloatToString(pen.Width)).AppendLine(" w").AppendLine("0 J");
            LineStyle style;
            switch (pen.DashStyle)
            {
                case DashStyle.Dot: style = LineStyle.Dot; break;
                case DashStyle.Dash: style = LineStyle.Dash; break;
                case DashStyle.DashDot: style = LineStyle.DashDot; break;
                case DashStyle.DashDotDot: style = LineStyle.DashDotDot; break;
                default: style = LineStyle.Solid; break;
            }
            sb.AppendLine(DrawPDFDash(style, pen.Width * PDF_DIVIDER));
            AppendPDFGraphicsPath(path, sb, curvesInterpolation);
            if (closed)
                sb.AppendLine("s");
            else
                sb.AppendLine("S");
            sb.AppendLine("Q");
        }

        private void WriteFunctionTypeColor(int num, Color c, StringBuilder sb)
        {
            sb.Append(" /C").Append(num).Append(" [ ");
            switch (ColorSpace)
            {
                case PdfColorSpace.RGB:
                    GetPDFColor(c, sb);
                    break;

                case PdfColorSpace.CMYK:
                    GetCMYKColor(c, sb);
                    break;

                default:
                    throw new NotImplementedException();
            }
            sb.Append(" ]");
        }

        #endregion Private Methods

        #region Public Enums

        /// <summary>
        /// The enum of curves interpolation
        /// </summary>
        public enum CurvesInterpolationEnum
        {
            /// <summary>
            /// Export as curves, without interpolation
            /// </summary>
            Curves,

            /// <summary>
            /// Two points
            /// </summary>
            P002 = 2,

            /// <summary>
            /// Four points
            /// </summary>
            P004 = 4,

            /// <summary>
            /// Eight points
            /// </summary>
            P008 = 8,

            /// <summary>
            /// Sixteen points
            /// </summary>
            P016 = 16
        }

        /// <summary>
        /// The enum of gradient interpolation points
        /// </summary>
        public enum GradientInterpolationPointsEnum : int
        {
            /// <summary>
            /// Two points
            /// </summary>
            P002 = 2,

            /// <summary>
            /// Four points
            /// </summary>
            P004 = 4,

            /// <summary>
            /// Eight points
            /// </summary>
            P008 = 8,

            /// <summary>
            /// Sixteen points
            /// </summary>
            P016 = 16,

            /// <summary>
            /// Thirty two points
            /// </summary>
            P032 = 32,

            /// <summary>
            /// Sixty four points
            /// </summary>
            P064 = 64,

            /// <summary>
            /// One hundred and twenty eight points
            /// </summary>
            P128 = 128,

            /// <summary>
            /// Two hundred and fifty six points
            /// </summary>
            P256 = 256,
        }

        /// <summary>
        /// The quality of gradient export
        /// </summary>
        public enum GradientQualityEnum : int
        {
            /// <summary>
            /// Export as image
            /// </summary>
            Image = 0,

            /// <summary>
            /// Export as low quality gradient grid, max size of interpolation points is 32
            /// </summary>
            Low = 32,

            /// <summary>
            /// Export as medium quality gradient grid, max size of interpolation points is 128
            /// </summary>
            Medium = 128,

            /// <summary>
            /// Export as high quality gradient grid, max size of interpolation points is 256
            /// </summary>
            High = 256,
        }

        #endregion Public Enums

        #region Private Classes

        private class HashableByteArray
        {
            #region Private Fields

            private int hash;
            private byte[] innerArray;

            #endregion Private Fields

            #region Public Properties

            public byte[] InnerArray
            {
                get
                {
                    return innerArray;
                }
            }

            #endregion Public Properties

            #region Public Constructors

            public HashableByteArray(byte[] arr)
            {
                hash = -1668713423;
                innerArray = arr;

                for (int i = 0; i < arr.Length; i++) unchecked
                {
                    hash = hash * -1521134295 + arr[i].GetHashCode();
                }
            }

            #endregion Public Constructors

            #region Public Methods

            public override bool Equals(object obj)
            {
                return Equals(obj as HashableByteArray);
            }

            public bool Equals(HashableByteArray obj)
            {
                if (obj == null || GetHashCode() != obj.GetHashCode() || innerArray == null || obj.innerArray == null || innerArray.Length != obj.innerArray.Length)
                    return false;
                for (int i = 0; i < innerArray.Length; i++)
                    if (innerArray[i] != obj.innerArray[i])
                        return false;
                return true;
            }

            public override int GetHashCode()
            {
                return hash;
            }

            #endregion Public Methods
        }

       

        #endregion Private Classes
    }
}