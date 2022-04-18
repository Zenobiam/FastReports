using System.Drawing;

namespace FastReport.Export.Dxf.Utils
{
    public static class DxfExtMethods
    {
        #region Public Methods

        public static PointF[] InvertY(PointF[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].Y *= -1;
            }
            return points;
        }

        #endregion Public Methods
    }
}