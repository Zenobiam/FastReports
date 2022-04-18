using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Utils
{
    public static class HpglExtensions
    {
        public static PointF[] InvertY(this PointF[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].Y *= -1;
            }
            return points;
        }
    }
}
