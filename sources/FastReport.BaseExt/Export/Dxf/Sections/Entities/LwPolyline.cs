using FastReport.Export.Dxf.Utils;
using System.Drawing;

namespace FastReport.Export.Dxf.Sections.Entities
{
    public class LwPolyLine : EntityBase
    {
        #region Private Fields

        private bool isClosedPolyline;
        private PointF[] points;
        private byte[] pointTypes;
        private Color polyLineColor;
        private LineStyle polyLineStyle;
        private float polyLineWidth;
        private float x;
        private float y;

        #endregion Private Fields

        #region Public Constructors

        public LwPolyLine(float x, float y, PointF[] points, byte[] pointTypes, Color polyLineColor, float polyLineWidth,
            LineStyle polyLineStyle, bool isClosedPolyline) : base()
        {
            this.points = points;
            this.pointTypes = pointTypes;
            this.polyLineColor = polyLineColor;
            this.polyLineWidth = polyLineWidth;
            this.polyLineStyle = polyLineStyle;
            this.x = x;
            this.y = y;
            this.isClosedPolyline = isClosedPolyline;
            InitGroups();
        }

        #endregion Public Constructors

        #region Private Methods

        private void AddVertex(PointF point)
        {
            AddPrimary2DPoint(x + point.X, y + point.Y);
        }

        private void InitGroups()
        {
            // 0
            // LWPOLYLINE
            // 8
            // Layer_1
            // 62
            // 7
            // 90
            // 4
            // 70
            // 1
            // 10
            // 42.763123
            // 20
            // 220.497360
            // 30
            // 0.0
            // 10
            // 117.598587
            // 20
            // 220.497360
            // 30
            // 0.0
            // 10
            // 117.598587
            // 20
            // 154.214515
            // 30
            // 0.0
            // 10
            // 42.763123
            // 20
            // 154.214515
            // 30
            // 0.0

            AddTypeName("LWPOLYLINE");
            AddGroup(8, "Layer_1");
            if (polyLineStyle != LineStyle.Solid)
                AddLineStyle(polyLineStyle);
            byte aciColor = ACIDictionary.GetAciColor(polyLineColor);
            AddColor(aciColor);
            AddEntityThickness(polyLineWidth);

            // Number of vertices
            AddGroup(90, points.Length);
            // Polyline flag (bit-coded); default is 0:
            // 1 = Closed; 128 = Plinegen
            if (isClosedPolyline)
                AddGroup(70, 1);
            else
                AddGroup(70, 0);
            //// Default start width (optional; default = 0)
            //AddGroup(40, 0);
            //// Default end width (optional; default = 0)
            //AddGroup(41, 0);
            // add VERTEX-es
            for (int i = 0; i < points.Length; i++)
            {
                PointF point = points[i];
                AddVertex(point);
            }
        }

        #endregion Private Methods
    }
}