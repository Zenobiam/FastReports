using FastReport.Export.Dxf.Utils;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FastReport.Export.Dxf
{
    public partial class DxfExport
    {
        #region Public Methods

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

        #endregion Public Methods

        #region Private Methods

        private void AddArrow(CapSettings Arrow, float lineWidth, float x1, float y1, float x2, float y2, out float x3, out float y3, out float x4, out float y4)
        {
            float k1, a, b, c, d;
            float xp, yp;
            float wd = Arrow.Width * lineWidth;
            float ld = Arrow.Height * lineWidth;
            if (Math.Abs(x2 - x1) > 0)
            {
                k1 = (y2 - y1) / (x2 - x1);
                a = (float)(Math.Pow(k1, 2) + 1);
                b = 2 * (k1 * ((x2 * y1 - x1 * y2) / (x2 - x1) - y2) - x2);
                c = (float)(Math.Pow(x2, 2) + Math.Pow(y2, 2) - Math.Pow(ld, 2) +
                    Math.Pow((x2 * y1 - x1 * y2) / (x2 - x1), 2) -
                    2 * y2 * (x2 * y1 - x1 * y2) / (x2 - x1));
                d = (float)(Math.Pow(b, 2) - 4 * a * c);
                xp = (float)((-b + Math.Sqrt(d)) / (2 * a));
                if ((xp > x1) && (xp > x2) || (xp < x1) && (xp < x2))
                    xp = (float)((-b - Math.Sqrt(d)) / (2 * a));
                yp = xp * k1 + (x2 * y1 - x1 * y2) / (x2 - x1);
                if (y2 != y1)
                {
                    x3 = (float)(xp + wd * Math.Sin(Math.Atan(k1)));
                    y3 = (float)(yp - wd * Math.Cos(Math.Atan(k1)));
                    x4 = (float)(xp - wd * Math.Sin(Math.Atan(k1)));
                    y4 = (float)(yp + wd * Math.Cos(Math.Atan(k1)));
                }
                else
                {
                    x3 = xp; y3 = yp - wd;
                    x4 = xp; y4 = yp + wd;
                }
            }
            else
            {
                xp = x2; yp = y2 - ld;
                if ((yp > y1) && (yp > y2) || (yp < y1) && (yp < y2))
                    yp = y2 + ld;
                x3 = xp - wd; y3 = yp;
                x4 = xp + wd; y4 = yp;
            }
        }

        private void AddBorder(Border border, float left, float top, float width, float height)
        {
            //float mWidth = width / Units.Millimeters;
            //float mHeight = height / Units.Millimeters;
            //float mTop = top / Units.Millimeters;
            //float mLeft = left / Units.Millimeters;
            //float borderWidth = border.Width / Units.Millimeters;
            //float shadowrWidth = border.ShadowWidth / Units.Millimeters;
            float oTop = pageHeight * Units.Millimeters - top;
            if (border.Shadow)
            {
                PointF startPoint = new PointF(width + border.ShadowWidth, -height - border.ShadowWidth);
                PointF[] points = new PointF[6];
                points[0] = startPoint;
                points[1] = new PointF(startPoint.X - width, startPoint.Y);
                points[2] = new PointF(startPoint.X - width, startPoint.Y + border.ShadowWidth);
                points[3] = new PointF(startPoint.X - border.ShadowWidth, startPoint.Y + border.ShadowWidth);
                points[4] = new PointF(startPoint.X - border.ShadowWidth, startPoint.Y + height);
                points[5] = new PointF(startPoint.X, startPoint.Y + height);
                byte[] pTypes = new byte[6];

                AddPath(points, pTypes, border.ShadowColor, 1, LineStyle.Solid, left, oTop, true);
                // shadows fill
                AddRectangleFill(left + width, oTop - border.ShadowWidth, border.ShadowWidth, height, border.ShadowColor);
                AddRectangleFill(left + border.ShadowWidth, oTop - height, width, border.ShadowWidth, border.ShadowColor);
            }
            if (border.Lines != BorderLines.None)
            {
                if (border.Lines == BorderLines.All &&
                    border.LeftLine.Equals(border.RightLine) &&
                    border.TopLine.Equals(border.BottomLine) &&
                    border.LeftLine.Equals(border.TopLine))
                {
                    if (border.Width > 0 && border.Color != Color.Transparent)
                    {
                        PointF[] points = new PointF[4];
                        points[0] = new PointF(0, 0);
                        points[1] = new PointF(width, 0);
                        points[2] = new PointF(width, height);
                        points[3] = new PointF(0, height);
                        byte[] pTypes = new byte[6];
                        AddPath(points, pTypes, border.Color, 1, border.Style, left, oTop - height, true);

                        if (border.LeftLine.Style == LineStyle.Double)
                        {
                            points = new PointF[4];
                            points[0] = new PointF(0, 0);
                            points[1] = new PointF(width - 2 * 2, 0);
                            points[2] = new PointF(width - 2 * 2, height - 2 * 2);
                            points[3] = new PointF(0, height - 2 * 2);
                            pTypes = new byte[6];
                            AddPath(points, pTypes, border.Color, 1, border.Style, left + 2,
                                oTop - height + 2, true);
                        }
                    }
                }
                else
                {
                    float Left = left;
                    float Top = top;
                    float Right = left + width;
                    float Bottom = top + height;
                    Top -= 0.1f;
                    Bottom += 0.1f;

                    if ((border.Lines & BorderLines.Left) > 0)
                    {
                        LineObject line = new LineObject();
                        line.Left = Left;
                        line.Top = Top;
                        line.Width = 0;
                        line.Height = height;
                        line.Border.Color = border.LeftLine.Color;
                        line.Border.Width = border.LeftLine.Width;
                        line.Border.Style = border.LeftLine.Style;
                        AddLine(line);
                        if (border.LeftLine.Style == LineStyle.Double)
                        {
                            LineObject lineD = new LineObject();
                            lineD.Left = Left + 2;
                            lineD.Top = Top;
                            lineD.Width = 0;
                            lineD.Height = height;
                            lineD.Border.Color = border.LeftLine.Color;
                            lineD.Border.Width = border.LeftLine.Width;
                            lineD.Border.Style = border.LeftLine.Style;
                            AddLine(lineD);
                        }
                    }
                    if ((border.Lines & BorderLines.Right) > 0)
                    {
                        LineObject line = new LineObject();
                        line.Left = Right;
                        line.Top = Top;
                        line.Width = 0;
                        line.Height = height;
                        line.Border.Color = border.RightLine.Color;
                        line.Border.Width = border.RightLine.Width;
                        line.Border.Style = border.RightLine.Style;
                        AddLine(line);
                        if (border.RightLine.Style == LineStyle.Double)
                        {
                            LineObject lineD = new LineObject();
                            lineD.Left = Right - 2;
                            lineD.Top = Top;
                            lineD.Width = 0;
                            lineD.Height = height;
                            lineD.Border.Color = border.RightLine.Color;
                            lineD.Border.Width = border.RightLine.Width;
                            lineD.Border.Style = border.RightLine.Style;
                            AddLine(lineD);
                        }
                    }
                    if ((border.Lines & BorderLines.Top) > 0)
                    {
                        LineObject line = new LineObject();
                        line.Left = Left;
                        line.Top = Top;
                        line.Width = width;
                        line.Height = 0;
                        line.Border.Color = border.TopLine.Color;
                        line.Border.Width = border.TopLine.Width;
                        line.Border.Style = border.TopLine.Style;
                        AddLine(line);
                        if (border.RightLine.Style == LineStyle.Double)
                        {
                            LineObject lineD = new LineObject();
                            lineD.Left = Left - 2;
                            lineD.Top = Top;
                            lineD.Width = width;
                            lineD.Height = 0;
                            lineD.Border.Color = border.TopLine.Color;
                            lineD.Border.Width = border.TopLine.Width;
                            lineD.Border.Style = border.TopLine.Style;
                            AddLine(lineD);
                        }
                    }
                    if ((border.Lines & BorderLines.Bottom) > 0)
                    {
                        LineObject line = new LineObject();
                        line.Left = Left;
                        line.Top = Top + height;
                        line.Width = width;
                        line.Height = 0;
                        line.Border.Color = border.BottomLine.Color;
                        line.Border.Width = border.BottomLine.Width;
                        line.Border.Style = border.BottomLine.Style;
                        AddLine(line);
                        if (border.RightLine.Style == LineStyle.Double)
                        {
                            LineObject lineD = new LineObject();
                            lineD.Left = Left;
                            lineD.Top = Top + height + 2;
                            lineD.Width = width;
                            lineD.Height = 0;
                            lineD.Border.Color = border.BottomLine.Color;
                            lineD.Border.Width = border.BottomLine.Width;
                            lineD.Border.Style = border.BottomLine.Style;
                            AddLine(lineD);
                        }
                    }
                }
            }
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
                pathPoints = DxfExtMethods.InvertY(pathPoints);

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

        /// <summary>
        /// Add Line.
        /// </summary>
        private void AddLine(LineObject line)
        {
            float x1 = line.AbsLeft / Units.Millimeters;
            float y1 = pageHeight - line.AbsTop / Units.Millimeters;
            float x2 = x1 + line.Width / Units.Millimeters;
            float y2 = y1 - line.Height / Units.Millimeters;
            float Border = line.Border.Width / Units.Millimeters;

            if (line.StartCap.Style == CapStyle.Arrow)
            {
                float x3, y3, x4, y4;
                AddArrow(line.StartCap, Border, x2, y2, x1, y1, out x3, out y3, out x4, out y4);
                dxfDocument.DrawLine(x1, y1, x3, y3, line.Border.Color, Border, line.Border.Style);
                dxfDocument.DrawLine(x1, y1, x4, y4, line.Border.Color, Border, line.Border.Style);
            }
            if (line.EndCap.Style == CapStyle.Arrow)
            {
                float x3, y3, x4, y4;
                AddArrow(line.EndCap, Border, x1, y1, x2, y2, out x3, out y3, out x4, out y4);
                dxfDocument.DrawLine(x2, y2, x3, y3, line.Border.Color, Border, line.Border.Style);
                dxfDocument.DrawLine(x2, y2, x4, y4, line.Border.Color, Border, line.Border.Style);
            }
            dxfDocument.DrawLine(x1, y1, x2, y2, line.Border.Color, Border, line.Border.Style);
        }

        private void AddPath(PointF[] points, byte[] pointTypes, Color borderColor, float borderWidth,
            LineStyle borderLineStyle, float left, float top, bool isClosed)
        {
            float x = left / Units.Millimeters;
            float y = top / Units.Millimeters; ;

            for (int i = 0; i < points.Length; i++)
            {
                points[i].X /= Units.Millimeters;
                points[i].Y /= Units.Millimeters;
            }

            dxfDocument.DrawPolyLine(x, y, points, pointTypes, borderColor, borderWidth / Units.Millimeters,
                borderLineStyle, isClosed);
        }

        private void AddPolygon(PolygonObject polygonObject)
        {
            AddPolyLine(polygonObject);
        }

        private void AddPolyLine(PolyLineObject p)
        {
            bool isClosed = (p is PolygonObject) ? true : false;

            AddGraphicsPath(new GraphicsPath(p.PointsArray, p.PointTypesArray, System.Drawing.Drawing2D.FillMode.Winding), p.Border.Color, p.Border.Width,
               p.Border.Style, p.AbsLeft /*p.CenterX*/, pageHeight * Units.Millimeters - p.AbsTop /*- p.CenterY*/, isClosed, true, 0, new PointF(p.CenterX, p.CenterY));

            //AddPath(p.PointsArray.InvertY(), p.PointTypesArray, p.Border.Color, p.Border.Width,
            //    p.Border.Style, p.AbsLeft + p.CenterX, pageHeight * Units.Millimeters - p.AbsTop - p.CenterY, isClosed);
        }

        private void AddRectangleFill(float x, float y, float width, float height, Color color)
        {
            if (fillMode == DxfFillMode.Solid)
                dxfDocument.FillRectangle(x / Units.Millimeters, (y - height) / Units.Millimeters, width / Units.Millimeters, height / Units.Millimeters, color);
            else
            {
                PointF startPoint = new PointF(0, 0);
                PointF[] points = new PointF[6];
                byte[] pTypes = new byte[4];
                points[0] = startPoint;
                points[1] = new PointF(startPoint.X + width, startPoint.Y);
                points[2] = new PointF(startPoint.X + width, startPoint.Y - height);
                points[3] = new PointF(startPoint.X, startPoint.Y - height);
                AddPath(points, pTypes, color, 1, LineStyle.Solid, x, y, true);
            }
        }

        private void AddShape(ShapeObject shapeObject)
        {
            float shapeWidth = shapeObject.Width;// / Units.Millimeters;
            float shapeHeight = shapeObject.Height;// / Units.Millimeters;
            float shapeTop = shapeObject.AbsTop;// / Units.Millimeters;
            float shapeLeft = shapeObject.AbsLeft;// / Units.Millimeters;
            float shapeBorderWidth = shapeObject.Border.Width;// / Units.Millimeters;

            switch (shapeObject.Shape)
            {
                case ShapeKind.Rectangle:
                    {
                        PointF[] points = new PointF[4];
                        points[0] = new PointF(0, 0);
                        points[1] = new PointF(shapeWidth, 0);
                        points[2] = new PointF(shapeWidth, shapeHeight);
                        points[3] = new PointF(0, shapeHeight);
                        byte[] pTypes = new byte[4];

                        AddPath(points, new byte[4], shapeObject.Border.Color, shapeBorderWidth,
                        shapeObject.Border.Style, shapeLeft, pageHeight * Units.Millimeters - shapeTop - shapeHeight, true);
                    }
                    break;

                case ShapeKind.Triangle:
                    {
                        PointF[] points = new PointF[3];
                        points[0] = new PointF(shapeWidth / 2, 0);
                        points[1] = new PointF(shapeWidth, shapeHeight);
                        points[2] = new PointF(0, shapeHeight);
                        byte[] pTypes = new byte[3];

                        AddPath(points, new byte[3], shapeObject.Border.Color, shapeBorderWidth,
                        shapeObject.Border.Style, shapeLeft, pageHeight * Units.Millimeters - shapeTop - shapeHeight, true);
                    }
                    break;

                case ShapeKind.Diamond:
                    {
                        PointF[] points = new PointF[4];
                        points[0] = new PointF(shapeWidth / 2, 0);
                        points[1] = new PointF(shapeWidth, shapeHeight / 2);
                        points[2] = new PointF(shapeWidth / 2, shapeHeight);
                        points[3] = new PointF(0, shapeHeight / 2);
                        byte[] pTypes = new byte[4];

                        AddPath(points, new byte[4], shapeObject.Border.Color, shapeBorderWidth,
                        shapeObject.Border.Style, shapeLeft, pageHeight * Units.Millimeters - shapeTop - shapeHeight, true);
                    }
                    break;

                case ShapeKind.Ellipse:
                    {
                        dxfDocument.DrawEllipse(shapeLeft / Units.Millimeters, pageHeight - shapeTop / Units.Millimeters, shapeWidth / Units.Millimeters,
                            shapeHeight / Units.Millimeters, shapeObject.Border.Color, shapeBorderWidth / Units.Millimeters, shapeObject.Border.Style);
                        break;
                    }
            }
        }

        private void AddUnderlines(TextObject obj)
        {
            float lineHeight = obj.LineHeight == 0 ? obj.Font.GetHeight() * DrawUtils.ScreenDpiFX : obj.LineHeight;
            float curY = obj.AbsTop + lineHeight;
            float bottom = obj.AbsBottom;
            float left = obj.AbsLeft;
            float right = obj.AbsRight;
            float width = obj.Border.Width;
            while (curY < bottom)
            {
                LineObject underline = new LineObject();
                underline.Left = left;
                underline.Top = curY;
                underline.Width = obj.Width;
                underline.Height = 0;
                underline.Border = obj.Border;

                AddLine(underline);
                curY += lineHeight;
            }
        }

        #endregion Private Methods
    }
}