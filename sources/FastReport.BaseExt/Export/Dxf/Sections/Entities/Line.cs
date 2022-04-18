using FastReport.Export.Dxf.Utils;
using System.Drawing;

namespace FastReport.Export.Dxf.Sections.Entities
{
    public class Line : EntityBase
    {
        #region Private Fields

        private LineStyle lineStyle;
        private Color strokeColor;
        private float strokeThickness;
        private float x1;
        private float x2;
        private float y1;
        private float y2;

        #endregion Private Fields

        #region Public Constructors

        public Line(float x1, float y1, float x2, float y2, Color strokeColor, float strokeThickness, LineStyle lineStyle)
            : base()
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            this.strokeColor = strokeColor;
            this.strokeThickness = strokeThickness;
            this.lineStyle = lineStyle;

            InitGroups();
        }

        #endregion Public Constructors

        #region Private Methods

        private void InitGroups()
        {
            AddTypeName("LINE");
            AddGroup(8, "Layer_1");
            AddPrimary2DPoint(x1, y1);
            AddSecondPoint(x2, y2);
            byte aciColor = ACIDictionary.GetAciColor(strokeColor);
            AddColor(aciColor);
            AddEntityThickness(strokeThickness);

            if (lineStyle != LineStyle.Solid)
                AddLineStyle(lineStyle);
        }

        #endregion Private Methods
    }
}