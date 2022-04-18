using FastReport.Export.Hpgl;
using FastReport.Export.Hpgl.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;

namespace FastReport.Utils
{
    internal class HpglGraphicsRenderer : IGraphics
    {
        private HpglDocument hpglDocument;
        private Graphics measureGraphics;
        private Bitmap internalImage;
        private System.Drawing.Drawing2D.Matrix transformMatrix;
        private HpglFillMode fillMode;
        private float barcodesGap;

        float IGraphics.DpiY {
            get
            {
                throw new NotImplementedException();
            }
        }

        TextRenderingHint IGraphics.TextRenderingHint 
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            } 
        }
        InterpolationMode IGraphics.InterpolationMode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        SmoothingMode IGraphics.SmoothingMode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        Graphics IGraphics.Graphics {
            get
            {
                throw new NotImplementedException();
            }
        }

        System.Drawing.Drawing2D.Matrix IGraphics.Transform
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        GraphicsUnit IGraphics.PageUnit
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            } 
        }

        bool IGraphics.IsClipEmpty {
            get
            {
                throw new NotImplementedException();
            }
        }

        Region IGraphics.Clip
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        float IGraphics.DpiX {
            get
            {
                throw new NotImplementedException();
            }
        }

        CompositingQuality IGraphics.CompositingQuality
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            } 
        }

        public HpglGraphicsRenderer(HpglDocument hpglDocument, HpglFillMode fillMode, float barcodesGap)
        {
            this.internalImage = new Bitmap(1, 1);
            this.hpglDocument = hpglDocument;
            this.measureGraphics = Graphics.FromImage(internalImage);
            transformMatrix = new System.Drawing.Drawing2D.Matrix();
            this.fillMode = fillMode;
            this.barcodesGap = barcodesGap;
        }


        private LineStyle GetLineStyle(DashStyle dashStyle)
        {
            LineStyle lineStyle;
            switch (dashStyle)
            {
                case DashStyle.Dash:
                    lineStyle = LineStyle.Dash; break;
                case DashStyle.DashDot:
                    lineStyle = LineStyle.DashDot; break;
                case DashStyle.DashDotDot:
                    lineStyle = LineStyle.DashDotDot; break;
                case DashStyle.Dot:
                    lineStyle = LineStyle.Dot; break;
                case DashStyle.Solid:
                default:
                    lineStyle = LineStyle.Solid; break;
            }
            return lineStyle;
        }

        public void Dispose()
        {
            //if (hpglDocument != null)
            //{
            //    hpglDocument.Clear();
            //    hpglDocument = null;
            //}

            if (measureGraphics != null)
            {
                measureGraphics.Dispose();
                measureGraphics = null;
            }
        }

        public void DrawEllipse(Pen pen, float left, float top, float width, float height)
        {
            LineStyle lineStyle = GetLineStyle(pen.DashStyle);
            hpglDocument.DrawEllipse(left + transformMatrix.OffsetX, transformMatrix.OffsetY - top, width, height, pen.Color, pen.Width, lineStyle);
        }

        private PointF GetOrtogonalPoint(PointF a, PointF b, float bc)
        {
            float x2x1 = a.X - b.X;
            float y2y1 = a.Y - b.Y;
            float ab = (float)Math.Sqrt(x2x1 * x2x1 + y2y1 * y2y1);
            float v1x = (b.X - a.X) / ab;
            float v1y = (b.Y - a.Y) / ab;
            float v3x = (v1y > 0 ? -v1y : v1y) * bc;
            float v3y = (v1x > 0 ? v1x : -v1x) * bc;

            PointF c = new PointF();
            c.X = a.X + v3x;
            c.Y = a.Y + v3y;
            return c;
        }

        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            LineStyle lineStyle = GetLineStyle(pen.DashStyle);

            //if(fillMode != HpglFillMode.Border)
            //    hpglDocument.DrawLine(x1 + transformMatrix.OffsetX, transformMatrix.OffsetY - y1, x2 + transformMatrix.OffsetX, transformMatrix.OffsetY - y2, pen.Color, pen.Width, lineStyle);
            //else
            {
                PointF[] points = new PointF[4];
                points[0] = GetOrtogonalPoint(new PointF(x1, y1), new PointF(x2, y2), pen.Width / 2);
                points[1] = GetOrtogonalPoint(new PointF(x2, y2), new PointF(x1, y1), pen.Width / 2);
                points[2] = GetOrtogonalPoint(new PointF(x2, y2), new PointF(x1, y1), -pen.Width / 2);
                points[3] = GetOrtogonalPoint(new PointF(x1, y1), new PointF(x2, y2), -pen.Width / 2);

                SolidBrush brush = (fillMode == HpglFillMode.Border) ? null : new SolidBrush(pen.Color);

                hpglDocument.DrawPolygon(transformMatrix.OffsetX, transformMatrix.OffsetY, points.InvertY(), new byte[points.Length],
                    (pen.Brush as SolidBrush).Color, 1 / 100.0f, LineStyle.Solid, brush);
            }
        }

        public void DrawString(string text, Font drawFont, Brush brush, float left, float top)
        {
            //ToDo - baseLine
            DrawStringInternal(text, drawFont, brush, left, top, 0);
        }

        public void DrawString(string text, Font drawFont, Brush brush, float left, float top, float baseLine)
        {
            DrawStringInternal(text, drawFont, brush, left, top, baseLine);
        }

        public void DrawString(string text, Font drawFont, Brush brush, RectangleF rectangleF)
        {
            //ToDo - baseLine
            DrawStringInternal(text, drawFont, brush, rectangleF.X, rectangleF.Y, 0);
        }

        private void DrawStringInternal(string text, Font font, Brush brush, float left, float top, float baseLine)
        {
            if (!String.IsNullOrEmpty(text))
            {
                    GraphicsPath myPath = new GraphicsPath();
                    PointF origin = new PointF(0, 0);

                    // Add the string to the path.
                    myPath.AddString(text,
                        font.FontFamily,
                         (int)font.Style,
                        font.Size,
                        origin,
                        StringFormat.GenericDefault);

                    if (brush is SolidBrush)
                        AddGraphicsPath(myPath, (brush as SolidBrush).Color, 1 / 100.0f, LineStyle.Solid, transformMatrix.OffsetX + left,
                        transformMatrix.OffsetY - top, true, true, 0, new PointF(0, 0));
            }
        }
        private void FillGraphicsPath(float x, float y, GraphicsPath path, Brush brush, bool invertY)
        {
            List<PointF> points = new List<PointF>();
            List<byte> pTypes = new List<byte>();
            byte[] pathTypes = path.PathTypes;
            PointF[] pathPoints = path.PathPoints;
            if (invertY)
                pathPoints = pathPoints.InvertY();

            for (int i = 0; i < pathTypes.Length; i++)
            {
                byte pType = pathTypes[i];
                PointF point = pathPoints[i];
                if (pType == 0 && points.Count > 0)
                {
                    if (fillMode == HpglFillMode.Solid)
                        hpglDocument.FillPolygon(x, y, points.ToArray(), pTypes.ToArray(), brush);
                    else
                        hpglDocument.DrawPolygon(x, y, points.ToArray(), pTypes.ToArray(), (brush as SolidBrush).Color, 1 / 100.0f, LineStyle.Solid);
                    points.Clear();
                    pTypes.Clear();
                }
                points.Add(point);
                pTypes.Add(pType);
                if (i == pathTypes.Length - 1)
                {
                    if (fillMode == HpglFillMode.Solid)
                        hpglDocument.FillPolygon(0, 0, points.ToArray(), pTypes.ToArray(), brush);
                    else
                        hpglDocument.DrawPolygon(0, 0, points.ToArray(), pTypes.ToArray(), (brush as SolidBrush).Color, 1 / 100.0f, LineStyle.Solid);
                }
            }
        }
        public static PointF[] RotateVector(PointF[] vector, double angle, PointF center)
        {
            PointF[] rotatedVector = new PointF[vector.Length];
            for (int i = 0; i < vector.Length; i++)
            {
                rotatedVector[i].X = (float)(center.X + (vector[i].X - center.X) * Math.Cos(angle) + (center.Y - vector[i].Y) * Math.Sin(angle));
                rotatedVector[i].Y = (float)(center.Y + (vector[i].X - center.X) * Math.Sin(angle) + (vector[i].Y - center.Y) * Math.Cos(angle));
            }
            return rotatedVector;
        }
        private void AddGraphicsPath(GraphicsPath path, Color borderColor, float borderWith,
            LineStyle borderLineStyle, float left, float top, bool isClosed, bool invertY, float angle, PointF center)
        {
            List<PointF> points = new List<PointF>();
            List<byte> pTypes = new List<byte>();
            byte[] pathTypes = path.PathTypes;
            PointF[] pathPoints = path.PathPoints;

            if (angle != 0)
            {
                // rotate
                pathPoints = RotateVector(pathPoints, angle, center);
            }
            if (invertY)
                pathPoints = pathPoints.InvertY();

            for (int i = 0; i < pathTypes.Length; i++)
            {
                byte pType = pathTypes[i];
                PointF point = pathPoints[i];
                if (pType == 0 && points.Count > 0)
                {
                    AddPath(points.ToArray(), pTypes.ToArray(), borderColor, borderWith,
                       borderLineStyle, left, top, isClosed);
                    points.Clear();
                    pTypes.Clear();
                }
                points.Add(point);
                pTypes.Add(pType);
                if (i == pathTypes.Length - 1)
                {
                    AddPath(points.ToArray(), pTypes.ToArray(), borderColor, borderWith,
                       borderLineStyle, left, top, isClosed);
                }

            }
        }

        private void AddPath(PointF[] points, byte[] pointTypes, Color borderColor, float borderWidth,
           LineStyle borderLineStyle, float left, float top, bool isClosed)
        {
            float x = left;
            float y = top;
            hpglDocument.DrawPolyLine(x, y, points, pointTypes, borderColor, borderWidth / Units.Millimeters,
                borderLineStyle, isClosed);
        }


        public void FillPath(Brush brush, GraphicsPath path)
        {
            if (fillMode == HpglFillMode.Solid && brush is SolidBrush)
                FillGraphicsPath(transformMatrix.OffsetX, transformMatrix.OffsetY, path, brush, true);
            else
                AddGraphicsPath(path, (brush as SolidBrush).Color, 1 / 100.0f, LineStyle.Solid, transformMatrix.OffsetX,
                      transformMatrix.OffsetY, true, true, 0, new PointF(0, 0));              
        }

        public void FillPolygon(Brush brush, PointF[] points)
        {
            if (fillMode == HpglFillMode.Solid && brush is SolidBrush)
                hpglDocument.FillPolygon(transformMatrix.OffsetX, transformMatrix.OffsetY, points.InvertY(), new byte[points.Length], brush);
            else
                hpglDocument.DrawPolygon(transformMatrix.OffsetX, transformMatrix.OffsetY, points.InvertY(), new byte[points.Length], (brush as SolidBrush).Color, 1 / 100.0f, LineStyle.Solid);

            //if (brush is SolidBrush)
            //    hpglDocument.FillPolygon(transformMatrix.OffsetX, transformMatrix.OffsetY, points.InvertY(), new byte[points.Length], (brush as SolidBrush).Color);
        }

        private void TransformPoints(System.Drawing.Drawing2D.Matrix fTransformMatrix, PointF[] points)
        {
            float a = fTransformMatrix.Elements[0]; // scale horizontal
            float b = fTransformMatrix.Elements[1]; // lean vert
            float c = fTransformMatrix.Elements[2]; // lean horizontal
            float d = fTransformMatrix.Elements[3]; //scale vertical
            float e = fTransformMatrix.Elements[4]; // x offset
            float f = 0;// fTransformMatrix.Elements[5]; // y offset
            for (int i = 0; i < points.Length; i++)
            {
                float x0 = points[i].X;
                float y0 = points[i].Y;
                float x1 = a * x0 + c * y0 + e;
                float y1 = b * x0 + d * y0 + f;

                points[i].X = x1;
                points[i].Y = y1;
            }
        }

        public void FillRectangle(Brush brush, float left, float top, float width, float height)
        {
            if (brush is SolidBrush)
            {        
                //if o degies 
               // hpglDocument.FillRectangle(left + transformMatrix.OffsetX, transformMatrix.OffsetY - top - height, width, height, (brush as SolidBrush).Color);

                //else
                PointF[] points = new PointF[4];
                points[0] = new PointF(left, top);
                points[1] = new PointF(left + width, top);
                points[2] = new PointF(left + width, top + height);
                points[3] = new PointF(left, top + height);
                TransformPoints(transformMatrix, points);
                points = points.InvertY();

                if (fillMode == HpglFillMode.Solid && brush is SolidBrush)
                    hpglDocument.FillPolygon(0, transformMatrix.OffsetY, points, new byte[points.Length], brush);
                else
                    hpglDocument.DrawPolygon(0, transformMatrix.OffsetY, points, new byte[points.Length], (brush as SolidBrush).Color, 1 / 100.0f, LineStyle.Solid);
            }              
        }

        public SizeF MeasureString(string text, Font drawFont)
        {
            return this.measureGraphics.MeasureString(text, drawFont);
        }

        public void Restore(IGraphicsState state)
        {
            if (state is HpglFGraphicsRendererState)
            {
                HpglFGraphicsRendererState gState = state as HpglFGraphicsRendererState;
                transformMatrix = gState.Matrix;
            }
        }

        public void RotateTransform(float angle)
        {
            transformMatrix.Rotate(angle);
        }

        public IGraphicsState Save()
        {
            return new HpglFGraphicsRendererState(transformMatrix);
        }

        public void TranslateTransform(float left, float top)
        {
            transformMatrix.Translate(left, top);
        }

        void IGraphics.SetClip(RectangleF textRect)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawPath(Pen outlinePen, GraphicsPath path)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawString(string text, Font font, Brush textBrush, RectangleF textRect, StringFormat format)
        {
            throw new NotImplementedException();
        }

        void IGraphics.FillEllipse(Brush brush, float x, float y, float dx, float dy)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawImage(Image image, float x, float y)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawLines(Pen pen, PointF[] pointFs)
        {
            throw new NotImplementedException();
        }

        void IGraphics.FillRectangle(Brush brush, RectangleF drawRect)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawPolygon(Pen pen, PointF[] diaPoints)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawRectangle(Pen borderPen, float left, float top, float width, float height)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawString(string text, Font font, Brush brush, float left, float top, StringFormat format)
        {
            throw new NotImplementedException();
        }

        SizeF IGraphics.MeasureString(string text, Font font, SizeF size)
        {
            throw new NotImplementedException();
        }

        SizeF IGraphics.MeasureString(string text, Font font, int v, StringFormat format)
        {
            throw new NotImplementedException();
        }

        void IGraphics.MeasureString(string text, Font font, SizeF size, StringFormat format, out int charsFit, out int linesFit)
        {
            throw new NotImplementedException();
        }

        Region[] IGraphics.MeasureCharacterRanges(string text, Font font, RectangleF textRect, StringFormat format)
        {
            throw new NotImplementedException();
        }

        bool IGraphics.IsVisible(RectangleF objRect)
        {
            throw new NotImplementedException();
        }

        void IGraphics.ResetClip()
        {
            throw new NotImplementedException();
        }

        void IGraphics.ScaleTransform(float scaleX, float scaleY)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawImage(Image image, RectangleF rect1, RectangleF rect2, GraphicsUnit unit)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawImage(Image image, RectangleF rect)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawImage(Image image, float x, float y, float width, float height)
        {
            throw new NotImplementedException();
        }

        void IGraphics.SetClip(RectangleF displayRect, CombineMode intersect)
        {
            throw new NotImplementedException();
        }

        void IGraphics.FillPolygon(Brush brush, Point[] points)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawPolygon(Pen pen, Point[] points)
        {
            throw new NotImplementedException();
        }

        void IGraphics.MultiplyTransform(System.Drawing.Drawing2D.Matrix matrix, MatrixOrder prepend)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawLine(Pen pen, PointF p1, PointF p2)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawImage(Image image, PointF[] points)
        {
            throw new NotImplementedException();
        }

        void IGraphics.FillEllipse(Brush brush, RectangleF pointerCircle)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawEllipse(Pen pen, RectangleF pointerCircle)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
        {
            throw new NotImplementedException();
        }

        SizeF IGraphics.MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawImageUnscaled(Image image, Rectangle rect)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawArc(Pen pen, float x, float y, float dx, float dy, float v1, float v2)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        void IGraphics.DrawRectangle(Pen green, Rectangle rectangle)
        {
            throw new NotImplementedException();
        }

        void IGraphics.FillPie(Brush brush, float x, float y, float dx, float dy, float v1, float v2)
        {
            throw new NotImplementedException();
        }

        void IGraphics.FillRegion(Brush brush, Region region)
        {
            throw new NotImplementedException();
        }

        void IGraphics.SetClip(GraphicsPath path, CombineMode combineMode)
        {
            throw new NotImplementedException();
        }
    }

    public class HpglFGraphicsRendererState : IGraphicsState
    {
        private System.Drawing.Drawing2D.Matrix matrix;

        public System.Drawing.Drawing2D.Matrix Matrix
        {
            get
            {
                return matrix;
            }
        }

        public HpglFGraphicsRendererState(System.Drawing.Drawing2D.Matrix matrix)
        {
            this.matrix = matrix;
        }
    }
}
