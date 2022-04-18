using FastReport.SVG;
using Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace FastReport.Export.Pdf
{
    partial class PDFExport
    {
        private bool PDFExportV4(ReportComponentBase obj)
        {
            if (obj != null && obj is SVG.SVGObject && !svgAsPicture)
            {
                AddPDFSvgVector(obj as SVG.SVGObject, contentBuilder);
                obj.Dispose();
                obj = null;
                return true;
            }
            return false;
        }

        #region Private Methods

        private void AddPDFSvgVector(SVGObject obj, StringBuilder sb)
        {
            RectangleF rect = new RectangleF(obj.AbsLeft + obj.Padding.Left, obj.AbsTop + obj.Padding.Top, obj.Width - obj.Padding.Horizontal, obj.Height - obj.Padding.Vertical);
            PointF upperLeft;
            PointF upperRight;
            PointF lowerLeft;
            SvgDocument document = obj.Grayscale ? obj.SVGGrayscale : obj.SvgDocument;
            SizeF imageBounds = SizeF.Empty;
            try
            {
                imageBounds = document.GetDimensions();
            }
            catch (NullReferenceException)
            {
                DrawPDFBorder(obj.Border, obj.AbsLeft, obj.AbsTop, obj.Width, obj.Height, sb);
                return;
            }
            
            obj.GetImageAngleTransform(rect, imageBounds.Width, imageBounds.Height, 1, 1, 0, 0, out upperLeft, out upperRight, out lowerLeft);

            upperLeft = new PointF(GetLeft(upperLeft.X), GetTop(upperLeft.Y));
            upperRight = new PointF(GetLeft(upperRight.X), GetTop(upperRight.Y));
            lowerLeft = new PointF(GetLeft(lowerLeft.X), GetTop(lowerLeft.Y));

            //int imgWidth = (int)Math.Sqrt((upperLeft.X - upperRight.X) * (upperLeft.X - upperRight.X) + (upperLeft.Y - upperRight.Y) * (upperLeft.Y - upperRight.Y));
            //int imgHeight = (int)Math.Sqrt((upperLeft.X - lowerLeft.X) * (upperLeft.X - lowerLeft.X) + (upperLeft.Y - lowerLeft.Y) * (upperLeft.Y - lowerLeft.Y));

            if (!isPdfX())
                AddAnnot(obj);

            PDFSVGRenderer renderer = new PDFSVGRenderer(this, imageBounds);

            PointF vector1 = PointF.Subtract(upperRight, new SizeF(upperLeft));
            PointF vector2 = PointF.Subtract(lowerLeft, new SizeF(upperLeft));

            float[] m = new float[] {
                vector1.X / imageBounds.Width,
                vector1.Y / imageBounds.Height,
                vector2.X / imageBounds.Width,
                vector2.Y / imageBounds.Height,
                upperLeft.X,
                upperLeft.Y
            };

            document.Draw(renderer);
            sb.AppendLine("q");
            sb.Append(FloatToString(GetLeft(obj.AbsLeft))).Append(" ");
            sb.Append(FloatToString(GetTop(obj.AbsTop + obj.Height))).Append(" ");
            sb.Append(FloatToString((obj.Width) * PDF_DIVIDER)).Append(" ");
            sb.Append(FloatToString((obj.Height) * PDF_DIVIDER)).AppendLine(" re");
            sb.AppendLine("W");
            sb.AppendLine("n");

            sb.Append(FloatToStringSmart(m[0])).Append(" ")
                .Append(FloatToStringSmart(m[1])).Append(" ")
                .Append(FloatToStringSmart(m[2])).Append(" ")
                .Append(FloatToStringSmart(m[3])).Append(" ")
                .Append(FloatToStringSmart(m[4])).Append(" ")
                .Append(FloatToStringSmart(m[5])).AppendLine(" cm");

            sb.Append(renderer.GetString());
            sb.AppendLine("Q");
           
        }

        #endregion Private Methods

        #region Private Classes

        private class PDFSVGRenderer : ISvgRenderer
        {
            #region Private Fields

            private Stack<ISvgBoundable> _boundables = new Stack<ISvgBoundable>();
            private float dpiY = Utils.DrawUtils.ScreenDpi;
            private PDFExport export;

            //private int FImgHeight;
            //private int FImgWidth;
            private Region region;

            private SmoothingMode smoothingMode = SmoothingMode.Default;
            private System.Drawing.Drawing2D.Matrix transform = new System.Drawing.Drawing2D.Matrix();
            private StringBuilder sb = new StringBuilder();

            #endregion Private Fields

            //private System.Drawing.Drawing2D.Matrix FTransformFromOldToNew;

            #region Public Properties

            public float DpiY
            {
                get
                {
                    return 96;
                }
            }

            public SmoothingMode SmoothingMode
            {
                get
                {
                    return smoothingMode;
                }
                set
                {
                    smoothingMode = value;
                }
            }

            public System.Drawing.Drawing2D.Matrix Transform
            {
                get
                {
                    return transform .Clone() as System.Drawing.Drawing2D.Matrix;
                }
                set
                {
                    transform = value;
                }
            }

            #endregion Public Properties

            #region Public Constructors

            /// <summary>
            ///
            /// </summary>
            /// <param name="export">The pdf export</param>
            /// <param name="size">size of place for svg</param>
            public PDFSVGRenderer(PDFExport export, SizeF size)
            {
                this.export = export;

                //FTransformFromOldToNew.Scale(1, -1);
                //temp.Translate(upperLeft.X, upperLeft.Y);
                //FTransformFromOldToNew = temp;
                region = new Region(new RectangleF(0, 0, size.Width, size.Height));
            }

            #endregion Public Constructors

            #region Public Methods

            public void Dispose()
            {
                //TODO Dispose
                //throw new NotImplementedException();
            }

            public void DrawImage(System.Drawing.Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit graphicsUnit)
            {
                using (Bitmap bmp = new Bitmap((int)srcRect.Width, (int)srcRect.Height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.DrawImage(image, new RectangleF(0, 0, bmp.Width, bmp.Height), srcRect, graphicsUnit);
                        g.Flush();
                    }
                    long imageIndex = export.AppendPDFImage(bmp, export.jpegQuality);
                    if (imageIndex < 0) return;
                    export.AddImageToList(imageIndex);
                    PointF[] points = TransformPoints(Transform,
                        PointF.Add(destRect.Location, new SizeF(0, destRect.Size.Height)),
                        PointF.Add(destRect.Location, new SizeF(destRect.Size.Width, destRect.Size.Height)),
                        destRect.Location
                        );
                    //points = TransformPoints(FTransformFromOldToNew, points);
                    PointF offset = points[0];
                    PointF vector1 = PointF.Subtract(points[1], new SizeF(offset));
                    PointF vector2 = PointF.Subtract(points[2], new SizeF(offset));

                    sb.AppendLine("q");
                    // m00
                    sb.Append(export.FloatToString(vector1.X)).Append(" ");
                    // m01
                    sb.Append(export.FloatToString(vector1.Y)).Append(" ");
                    // m10
                    sb.Append(export.FloatToString(vector2.X)).Append(" ");
                    // m11
                    sb.Append(export.FloatToString(vector2.Y)).Append(" ");
                    // m02
                    sb.Append(export.FloatToString(offset.X)).Append(" ");
                    // m12
                    sb.Append(export.FloatToString(offset.Y)).Append(" ");
                    //clip
                    sb.AppendLine(" cm");
                    sb.AppendLine(ExportUtils.StringFormat("/Im{0} Do", imageIndex));
                    sb.AppendLine("Q");
                }
            }

            public void DrawImageUnscaled(System.Drawing.Image image, Point location)
            {
                DrawImage(image, new RectangleF(location, image.Size), new RectangleF(PointF.Empty, image.Size), GraphicsUnit.Pixel);
            }

            public void DrawPath(Pen pen, GraphicsPath path)
            {
                //GraphicsPath clone = path.Clone() as GraphicsPath;
                //clone.Transform(FTransform);
                //clone.Transform(FTransformFromOldToNew);
                //todo check scale
                //PointF[] points = new PointF[] { new PointF(1,1)};
                //Transform.TransformVectors(points);
                //FTransformFromOldToNew.TransformVectors(points);
                //pen.Width *= (float)Math.Sqrt(points[0].X * points[0].X + points[0].Y * points[0].Y) / 1.414213562373f;
                System.Drawing.Drawing2D.Matrix clone = transform.Clone() as System.Drawing.Drawing2D.Matrix;
                //clone.Multiply(FTransformFromOldToNew);
                export.StrokePDFGraphicsPath(path, pen, path.PointCount > 0 && ((path.PathTypes[path.PointCount - 1] & 0x80) == 0x80), sb, export.curvesInterpolation, clone);
            }

            public void FillPath(Brush brush, GraphicsPath path)
            {
                //GraphicsPath clone = path.Clone() as GraphicsPath;
                //clone.Transform(FTransform);
                //clone.Transform(FTransformFromOldToNew);
                System.Drawing.Drawing2D.Matrix clone = transform.Clone() as System.Drawing.Drawing2D.Matrix;
                //clone.Multiply(FTransformFromOldToNew);

                //clone.Transform(new System.Drawing.Drawing2D.Matrix(PDF_DIVIDER, 0, 0, -PDF_DIVIDER, FRectangle.Left + FExport.FMarginLeft, FRectangle.Top + FExport.FMarginWoBottom));
                //todo transform bounds rly todo!!! and brush???

                //PointF[] points = TransformPoints(Transform, GetBoundable().Location);
                //points = TransformPoints(FTransformFromOldToNew, points);

                export.FillPDFGraphicsPath(GetBoundable().Size, path, brush, sb, export.curvesInterpolation, clone);
            }

            public ISvgBoundable GetBoundable()
            {
                return _boundables.Peek();
            }

            public Region GetClip()
            {
                return region;
                //throw new NotImplementedException();
            }

            public ISvgBoundable PopBoundable()
            {
                return _boundables.Pop();
            }

            public void RotateTransform(float fAngle, MatrixOrder order = MatrixOrder.Append)
            {
                transform.Rotate(fAngle, order);
            }

            public void ScaleTransform(float sx, float sy, MatrixOrder order = MatrixOrder.Append)
            {
                transform.Scale(sx, sy, order);
            }

            public void SetBoundable(ISvgBoundable boundable)
            {
                _boundables.Push(boundable);
            }

            public void SetClip(Region region, CombineMode combineMode = CombineMode.Replace)
            {
                //TODO combineMode
                this.region = region;
            }

            public void TranslateTransform(float dx, float dy, MatrixOrder order = MatrixOrder.Append)
            {
                transform.Translate(dx, dy, order);
            }

            #endregion Public Methods

            #region Internal Methods

            internal string GetString()
            {
                return sb.ToString();
            }

            #endregion Internal Methods

            #region Private Methods

            private PointF[] TransformPoints(System.Drawing.Drawing2D.Matrix matrix, params PointF[] points)
            {
                matrix.TransformPoints(points);
                return points;
            }

            #endregion Private Methods
        }

        #endregion Private Classes
    }
}