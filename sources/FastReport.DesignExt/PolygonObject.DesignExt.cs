using System.Drawing;
using System.Drawing.Drawing2D;
using FastReport.Utils;

namespace FastReport
{
    partial class PolygonObject
    {
        #region Public Methods
        /// <inheritdoc/>
        public override bool PointInObject(PointF point)
        {
            using (Pen pen = new Pen(Color.Black, 10))
            using (GraphicsPath path = getPolygonPath(pen,1,1))
            {
                return path.IsVisible(point);
            }
        }
        #endregion
    }
}