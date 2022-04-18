using FastReport.Export.Dxf.Utils;
using System.Drawing;

namespace FastReport.Export.Dxf.Sections.Entities
{
    public class Hatch : EntityBase
    {
        #region Private Fields

        private Color color;
        private PointF[] points;
        private byte[] pointTypes;
        private float x;
        private float y;

        #endregion Private Fields

        #region Public Constructors

        public Hatch(float x, float y, PointF[] points, byte[] pointTypes, Color color)
        {
            this.x = x;
            this.y = y;
            this.points = points;
            this.pointTypes = pointTypes;
            this.color = color;

            InitGroups();
        }

        #endregion Public Constructors

        #region Private Methods

        private void AddInsert()
        {
            // 0
            // INSERT
            // 8
            // 0
            // 6
            // BYLAYER
            AddTypeName("INSERT");
            AddGroup(8, "Layer_1");
        }

        private void AddVertex(PointF point)
        {
            AddPrimary2DPoint(x + point.X, y + point.Y);
        }

        private void InitGroups()
        {
            // 0
            // HATCH
            // 62
            // 1
            // 10
            // 0
            // 20
            // 0
            // 210
            //0
            //220
            // 0
            // 30
            // 0
            // 210
            // 0
            // 220
            // 0
            // 230
            // 1
            // 2
            // SOLID
            // 70
            // 1
            // 91
            // 1
            // 92
            // 2
            // 72
            // 0
            // 73
            // 1
            // 93
            // 6
            // 10
            // 20.1140162880412
            // 20
            // -2.24903557651093
            // 10
            // 22.320617231033
            // 20
            // 9.42048864123446
            // 10
            // 34.5417916845264
            // 20
            // 9.42048864123446
            // 10
            // 37.6583987659518
            // 20
            // -2.21084733387416
            // 10
            // 30.8499785683669
            // 20
            // -11.8816973853408
            // 10
            // 26.5214379415101
            // 20
            // -11.8816973853408
            // 75
            // 0
            // 76
            // 1
            // 47
            // 1E-6
            // 98
            // 0

            AddTypeName("HATCH");
            AddGroup(8, "Layer_1");
            byte aciColor = ACIDictionary.GetAciColor(color);
            AddColor(aciColor);
            AddPrimary2DPoint(0, 0);
            // Extrusion direction
            AddGroup(210, 0);
            AddGroup(220, 0);
            AddName("SOLID");
            // Solid fill flag (solid fill = 1; pattern fill = 0)
            AddGroup(70, 1);
            // Number of boundary paths (loops)
            AddGroup(91, 1);
            // Boundary path type flag (bit coded):
            // 0 = Default; 1 = External; 2 = Polyline;
            // 4 = Derived; 8 = Textbox; 16 = Outermost
            AddGroup(92, 7);//why 7? - becouse Inkscape
            // Edge type (only if boundary is not a polyline):
            // 1 = Line; 2 = Circular arc; 3 = Elliptic arc; 4 = Spline
            AddGroup(72, 0);
            // Is counterclockwise flag
            AddGroup(73, 1);
            // Number of polyline vertices
            AddGroup(93, points.Length);
            // Hatch style:
            // 0 = Hatch "odd parity" area(Normal style)
            // 1 = Hatch outermost area only(Outer style)
            // 2 = Hatch through entire area(Ignore style)
            //AddGroup(75, 0);
            //// Hatch pattern type:
            //// 0 = User - defined; 1 = Predefined; 2 = Custom
            //AddGroup(76, 1);

            // add vertices
            for (int i = 0; i < points.Length; i++)
            {
                PointF point = points[i];
                AddVertex(point);
            }
            // 0 = User - defined; 1 = Predefined; 2 = Custom
            AddGroup(47, "1E-6");
            // 0 = User - defined; 1 = Predefined; 2 = Custom
            AddGroup(98, 0);

            AddInsert();
        }

        #endregion Private Methods
    }
}