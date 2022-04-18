using FastReport.Export.Dxf.Sections.Entities;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FastReport.Export.Dxf.Sections
{
    public class SectionEntities : Section
    {
        #region Private Fields

        private List<EntityBase> entities;

        #endregion Private Fields

        #region Public Properties

        public List<EntityBase> Entities
        {
            get { return entities; }
            set { entities = value; }
        }

        #endregion Public Properties

        #region Public Constructors

        public SectionEntities() : base("ENTITIES")
        {
            Entities = new List<EntityBase>();
        }

        #endregion Public Constructors

        #region Public Methods

        public List<LineStyle> AddLine(float x1, float y1, float x2, float y2, Color strokeColor, float strokeThickness, LineStyle lineStyle)
        {
            Line line = new Line(x1, y1, x2, y2, strokeColor, strokeThickness, lineStyle);
            Entities.Add(line);
            return line.Styles;
        }

        public override void AppendTo(StringBuilder s)
        {
            StartSectionAppendTo(s);
            s.Append("\n");
            foreach (EntityBase e in Entities)
            {
                e.AppendTo(s);
                s.Append("\n");
            }
            EndSectionAppendTo(s);
        }

        #endregion Public Methods

        #region Internal Methods

        internal List<LineStyle> AddEllipse(float x, float y, float width, float height, Color ellipseColor, float ellipseWidth, LineStyle ellipseStyle)
        {
            Ellipse ellipse = new Ellipse(x, y, width, height, ellipseColor, ellipseWidth, ellipseStyle);
            Entities.Add(ellipse);
            return ellipse.Styles;
        }

        internal void AddHatch(float x, float y, PointF[] points, byte[] pointTypes, Color color)
        {
            Hatch hatch = new Hatch(x, y, points, pointTypes, color);
            Entities.Add(hatch);
        }

        internal List<LineStyle> AddPolyLine(float x, float y, PointF[] points, byte[] pointTypes, Color polyLineColor,
                                    float polyLineWidth, LineStyle polyLineStyle, bool isClosedPolyline)
        {
            LwPolyLine polyLine = new LwPolyLine(x, y, points, pointTypes, polyLineColor, polyLineWidth, polyLineStyle, isClosedPolyline);
            Entities.Add(polyLine);
            return polyLine.Styles;
        }

        internal void AddSolid(float x, float y, float width, float height, Color color)
        {
            Solid ellipse = new Solid(x, y, width, height, color);
            Entities.Add(ellipse);
        }

        #endregion Internal Methods
    }
}