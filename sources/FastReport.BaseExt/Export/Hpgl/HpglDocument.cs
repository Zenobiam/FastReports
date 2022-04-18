using FastReport.Export.Hpgl.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl
{
    public class HpglDocument
    {
        StringBuilder docString;
        private List<CommandBase> commands;
        private string separator;
        private string terminator;

        public HpglDocument()
        {
            docString = new StringBuilder();
            commands = new List<CommandBase>();
            separator = ",";
            terminator = ";";
        }

        public void Start(SizeF pageSize)
        {
            commands.Initialize();
            commands.InputP1P2(new PointF(0, 0), new PointF(pageSize.Width, pageSize.Height));
            commands.Scale(0, 100, 0, 100);
        }

        public void Clear()
        {
            docString.Clear();
            commands.Clear();
        }

        public override string ToString()
        {
            foreach (CommandBase cmnd in commands)
            {
                cmnd.AppendTo(docString);
                docString.Append("\n");
            }
            return docString.ToString();
        }

        public void DrawLine(float x1, float y1, float x2, float y2, Color strokeColor, float strokeThickness)
        {
            DrawLine(x1, y1, x2, y2, strokeColor, strokeThickness, LineStyle.Solid);
        }

        LinePattern GetPattern(LineStyle lineStyle)
        {
            LinePattern pattern = LinePattern.None;
            switch (lineStyle)
            {
                case LineStyle.Solid:
                    pattern = LinePattern.None; break;
                case LineStyle.Dash:
                    pattern = LinePattern.LongDashed; break;
                case LineStyle.DashDot:
                    pattern = LinePattern.ShortDashed; break;
                case LineStyle.DashDotDot:
                    pattern = LinePattern.ShortDotted; break;
                case LineStyle.Dot:
                    pattern = LinePattern.Dotted; break;
            }
            return pattern;
        }

        public void DrawLine(float x1, float y1, float x2, float y2, Color strokeColor, float strokeThickness, LineStyle lineStyle)
        {
            //commands.SelectPen();
            commands.PenThickness(strokeThickness);
            LinePattern pattern = GetPattern(lineStyle);
            commands.LineType(pattern);
            commands.PlotAbsolute(x1, y1);
            commands.PenDown();
            commands.PlotAbsolute(x2, y2);
            commands.PenUp();
        }

        internal void DrawPolyLine(float x, float y, PointF[] points, byte[] pointTypes, Color polyLineColor,
            float polyLineWidth, LineStyle polyLineStyle, bool isClosedPolyline, Brush brush, PolygonType mode)
        {
            bool isPenDown = false;
            commands.PenThickness(polyLineWidth);
            LinePattern pattern = GetPattern(polyLineStyle);
            commands.LineType(pattern);

            // set fill mode: solid, hatch
            ShadingModel shading = ShadingModel.None;
            if (brush != null)
            {
                if (brush.GetType() == typeof(SolidBrush))
                {
                }
                else if (brush.GetType() == typeof(HatchBrush))
                {
                    shading = ShadingModel.Hatching;
                }
            }      

            if (isClosedPolyline)
            {
                if (brush != null)
                    commands.FillType(shading);
                commands.PlotAbsolute(x + points[points.Length - 1].X, y + points[points.Length - 1].Y);
                commands.PolygonMode(mode);
                commands.PenDown();
                isPenDown = true;
            }   
            //else
            //    commands.PolygonMode(PolygonType.Cleared);
            foreach (PointF point in points)
            {
                commands.PlotAbsolute(x + point.X, y + point.Y);
                if (!isClosedPolyline && !isPenDown)
                {
                    commands.PenDown();
                    isPenDown = true;
                }
            }
            if (isClosedPolyline && brush != null)
            {
                if ((brush is SolidBrush))
                {
                    if ((brush as SolidBrush).Color != Color.Transparent)
                        commands.FillPolygon();
                }
                else
                    commands.FillPolygon();
            }
            commands.EdgePolygon();
            commands.PenUp();
        }

        internal void DrawPolyLine(float x, float y, PointF[] points, byte[] pointTypes, Color polyLineColor,
           float polyLineWidth, LineStyle polyLineStyle, bool isClosedPolyline, Brush brush)
        {
            DrawPolyLine(x, y, points, pointTypes, polyLineColor, polyLineWidth, polyLineStyle, isClosedPolyline, brush, PolygonType.Cleared);
        }

            internal void DrawPolyLine(float x, float y, PointF[] points, byte[] pointTypes, Color polyLineColor,
            float polyLineWidth, LineStyle polyLineStyle, bool isClosedPolyline)
        {
            DrawPolyLine(x, y, points, pointTypes, polyLineColor, polyLineWidth, polyLineStyle, isClosedPolyline, null);
        }

        internal void DrawPolygon(float x, float y, PointF[] points, byte[] pointTypes, Color polyLineColor,
        float polyLineWidth, LineStyle polyLineStyle, Brush brush, PolygonType mode)
        {
            DrawPolyLine(x, y, points, pointTypes, polyLineColor, polyLineWidth, polyLineStyle, true, brush, mode);
        }

        internal void DrawPolygon(float x, float y, PointF[] points, byte[] pointTypes, Color polyLineColor,
        float polyLineWidth, LineStyle polyLineStyle, Brush brush)
        {
            DrawPolyLine(x, y, points, pointTypes, polyLineColor, polyLineWidth, polyLineStyle, true, brush, PolygonType.Cleared);
        }

        internal void DrawPolygon(float x, float y, PointF[] points, byte[] pointTypes, Color polyLineColor,
        float polyLineWidth, LineStyle polyLineStyle)
        {
            DrawPolyLine(x, y, points, pointTypes, polyLineColor, polyLineWidth, polyLineStyle, true, null);
        }

        internal void FillPolygon(float x, float y, PointF[] points, byte[] pointTypes, Brush brush)
        {
            DrawPolygon(x, y, points, pointTypes, (brush as SolidBrush).Color, 0.1f, LineStyle.Solid, brush, PolygonType.Cleared);
        }
        internal void FillRectangle(float x, float y, float width, float height, Brush brush)
        {
            //Entities.AddSolid(x, y, width, height, color);
            PointF[] points = new PointF[]
                {
                new PointF(0, 0),
                new PointF(width, 0),
                new PointF(width, height),
                new PointF(0, height)
                };
            FillPolygon(x, y, points, new byte[] { }, brush);
        }

        internal void DrawEllipse(float x, float y, float width, float height, Color ellipseColor, float ellipseWidth, LineStyle ellipseStyle)
        {
            //convert ellipse to polylines
            float a = width / 2;
            float b = height / 2;
            float step = 0.1f;
            int pointsCount = (int)Math.Ceiling(width / step);
            PointF[] pointsTop = new PointF[pointsCount + 1];
            PointF[] pointsBot = new PointF[pointsCount + 1];
            float xCur = -a;
            float yTop;
            for (int i = 0; i < pointsCount; i++)
            {
                yTop = (float)Math.Sqrt(Math.Pow(b, 2) - (Math.Pow(xCur, 2) * Math.Pow(b, 2) / Math.Pow(a, 2)));
                pointsTop[i] = new PointF(xCur, yTop);
                pointsBot[i] = new PointF(xCur, -yTop);
                xCur += step;
            }

            yTop = (float)Math.Sqrt(Math.Pow(b, 2) - (Math.Pow(a, 2) * Math.Pow(b, 2) / Math.Pow(a, 2)));
            pointsTop[pointsCount] = new PointF(a, yTop);
            pointsBot[pointsCount] = new PointF(a, -yTop);

            DrawPolyLine(x + a, y - b, pointsTop, new byte[pointsTop.Length], ellipseColor, ellipseWidth, ellipseStyle, false);
            DrawPolyLine(x + a, y - b, pointsBot, new byte[pointsTop.Length], ellipseColor, ellipseWidth, ellipseStyle, false);
        }
    }
}
