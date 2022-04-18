using FastReport.Export.Dxf.Utils;
using System.Drawing;

namespace FastReport.Export.Dxf.Sections.Entities
{
    public class PolyLine : EntityBase
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

        public PolyLine(float x, float y, PointF[] points, byte[] pointTypes, Color polyLineColor, float polyLineWidth,
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
            AddTypeName("VERTEX");
            AddPrimary2DPoint(x + point.X, y + point.Y);
            AddGroup(70, 0);
        }

        private void InitGroups()
        {
            // 0
            // POLYLINE
            // 6
            // DashDot
            // 66
            // 1
            // 10
            // 0
            // 20
            // 0
            // 30
            // 0
            // 70
            // 8
            // 40
            // 0
            // 41
            // 0
            //  0
            // VERTEX
            // 5
            // 402
            // 8
            // 0
            // 10
            // 3.43302574065077
            // 20
            // 4.42831471588149
            // 30
            // 8.45677694538694E-18
            // 70
            // 0
            // 0
            // VERTEX
            // 5
            // 403
            // 8
            // 0
            // 10
            // 10.2413793103448
            // 20
            // - 0.677950461389022
            // 30
            // 8.45677694538694E-18
            // 70
            // 0

            AddTypeName("POLYLINE");
            AddGroup(8, "Layer_1");
            if (polyLineStyle != LineStyle.Solid)
                AddLineStyle(polyLineStyle);
            // "Entities follow" flag (fixed)
            AddGroup(66, 1);
            // DXF: always 0
            // APP: a "dummy" point; the X and Y values are always 0, and the Z
            // value is the polyline's elevation (in OCS when 2D, WCS when 3D)
            AddGroup(10, 0);
            // DXF: always 0
            AddGroup(20, 0);
            // DXF: polyline's elevation (in OCS when 2D, WCS when 3D)
            AddGroup(30, 0);
            // Polyline flag (bit-coded); default is 0
            // 8 = This is a 3D polyline. 1 - closed (polygon)
            if (isClosedPolyline)
                AddGroup(70, 1);
            else
                AddGroup(70, 8);
            // Default start width (optional; default = 0)
            AddGroup(40, 0);
            // Default end width (optional; default = 0)
            AddGroup(41, 0);
            byte aciColor = ACIDictionary.GetAciColor(polyLineColor);
            AddColor(aciColor);
            AddEntityThickness(polyLineWidth);

            // add VERTEX-es
            for (int i = 0; i < points.Length; i++)
            {
                PointF point = points[i];
                AddVertex(point);
            }
            AddTypeName("SEQEND");
        }

        #endregion Private Methods
    }
}