using System;
using System.Collections.Generic;
using System.Drawing;



namespace FastReport.Fonts
{
#pragma warning disable CS1591  // Missing XML comment for publicly visible type or member
    public class FastGraphicsPath
    {
        public enum PointType : byte
        {
            /// <summary>
            ///  Indicates that the point is the start of a figure.
            /// </summary>
            Start = 0x00,
            /// <summary>
            /// Indicates that the point is one of the two endpoints of a line.
            /// </summary>
            Connect = 0x01,
            /// <summary>
            /// Indicates that the point is an endpoint or control point of a cubic Bézier spline.
            /// </summary>
            Bezier = 0x03,
            /// <summary>
            /// Masks all bits except for the three low-order bits, which indicate the point type.
            /// </summary>
            All = 0x07,
            /// <summary>
            /// Specifies that the point is a marker.
            /// </summary>
            Marker = 0x20,
            /// <summary>
            /// Specifies that the point is the last point in a closed subpath (figure).
            /// </summary>
            Last = 0x80,

        }
        private List<PointF> points = new List<PointF>();
        private List<PointType> pointTypes = new List<PointType>();
        private bool startNewFigure = false;

        public byte[] PathTypes
        {
            get
            {
                byte[] result = new byte[pointTypes.Count];
                int i = 0;
                foreach( PointType type in pointTypes)
                {
                    result[i] = (byte)type;
                    i++;
                }
                return result;
            }
        }

        public PointF[] PathPoints
        {
            get
            {
                return points.ToArray();
            }
        }

        private FastFillMode fillMode;

        public FastFillMode FillMode
        {
            get
            {
                return fillMode;
            }
            set
            {
                fillMode = value;
            }
        }

        public int PointCount { get { return points.Count; } }

        public FastGraphicsPath(FastFillMode fastFillMode /*= FastFillMode.Alternate*/)
        {
            FillMode = fastFillMode;
        }

        public void AddPath(FastGraphicsPath fastGraphicsPath, bool connect)
        {
            if (points.Count == 0)
            {
                points.AddRange(fastGraphicsPath.points);
                pointTypes.AddRange(fastGraphicsPath.pointTypes);
            }
            else
            {
                int i = -1;
                foreach (PointF point in fastGraphicsPath.points)
                {
                    i++;
                    if (connect)
                    {
                        if (points[points.Count - 1] == point)
                        {
                            connect = false;
                            continue;
                        }
                        else
                        {
                            points.Add(point);
                            pointTypes.Add(PointType.Connect);
                        }
                        connect = false;
                    }
                    else
                    {
                        points.Add(point);
                        pointTypes.Add(fastGraphicsPath.pointTypes[i]);
                    }
                }
            }
        }

        public void Transform(float m00, float m01, float m10, float m11, float v1, float v2)
        {
            List<PointF> points2 = new List<PointF>(points.Capacity);
            foreach(PointF point in points)
            {
                points2.Add(Transform(point, m00, m01, m10, m11, v1, v2));
            }
            points = points2;
        }

        private PointF Transform(PointF point, float m00, float m01, float m10, float m11, float v1, float v2)
        {
            return new PointF(point.X * m00 + v1 + point.Y*m10, point.Y * m11 + v2 + point.X*m01);
        }

        public void StartFigure()
        {
            startNewFigure = true;
        }

        public void AddLine(PointF pntStart, PointF pntEnd)
        {
            if(startNewFigure || points.Count == 0)
            {
                points.Add(pntStart);
                pointTypes.Add(PointType.Start);
                
            }
            else if (points[points.Count-1] != pntStart)
            {
                points.Add(pntStart);
                pointTypes.Add(PointType.Connect);
            }
            points.Add(pntEnd);
            pointTypes.Add(PointType.Connect);
            startNewFigure = false;
        }

        public void AddBezier(PointF pntStart, PointF pnt1, PointF pnt2, PointF pntEnd)
        {
            if (startNewFigure || points.Count == 0)
            {
                points.Add(pntStart);
                pointTypes.Add(PointType.Start);

            }
            else if (points[points.Count - 1] != pntStart)
            {
                points.Add(pntStart);
                pointTypes.Add(PointType.Connect);
            }
            points.Add(pnt1);
            pointTypes.Add(PointType.Bezier);
            points.Add(pnt2);
            pointTypes.Add(PointType.Bezier);
            points.Add(pntEnd);
            pointTypes.Add(PointType.Bezier);
            startNewFigure = false;
        }
    }
}