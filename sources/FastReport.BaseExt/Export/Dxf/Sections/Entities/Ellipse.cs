using FastReport.Export.Dxf.Utils;
using System.Drawing;

namespace FastReport.Export.Dxf.Sections.Entities
{
    internal class Ellipse : EntityBase
    {
        #region Private Fields

        private Color borderColor;
        private LineStyle borderStyle;
        private float borderWidth;
        private float height;
        private float width;
        private float x;
        private float y;

        #endregion Private Fields

        #region Public Constructors

        public Ellipse(float x, float y, float width, float height, Color borderColor, float borderWidth,
            LineStyle borderStyle) : base()
        {
            this.x = x;
            this.y = y;
            this.borderColor = borderColor;
            this.borderWidth = borderWidth;
            this.borderStyle = borderStyle;
            this.width = width;
            this.height = height;

            InitGroups();
        }

        #endregion Public Constructors

        #region Private Methods

        private void InitGroups()
        {
            // 0
            // ELLIPSE
            // 6
            // BYLAYER
            // 10
            // - 13.720
            // 20
            // 5.42
            // 30
            // 0
            // 11
            // 0
            // 21
            // 3.45
            // 31
            // 0
            // 40
            // 0.98
            // 41
            // 0
            // 42
            // 6.28
            AddTypeName("ELLIPSE");
            AddGroup(8, "Layer_1");
            byte aciColor = ACIDictionary.GetAciColor(borderColor);
            AddColor(aciColor);
            AddEntityThickness(borderWidth);
            if (borderStyle != LineStyle.Solid)
                AddLineStyle(borderStyle);

            float Ax = x;
            float Ay = y;

            // center
            float Cx = Ax + width / 2;
            float Cy = Ay - height / 2;
            // major axis
            float MjX = 0;
            float MjY = height / 2;
            // Ratio of minor axis to major axis
            float minToMajRatio = (Cx - Ax) / (Ay - Cy);

            // Center point (in WCS)
            AddPrimary2DPoint(Cx, Cy);
            // Endpoint of major axis, relative to the center (in WCS)
            AddSecondPoint(MjX, MjY);
            // Ratio of minor axis to major axis
            AddGroup(40, minToMajRatio);
            // Start parameter (this value is 0.0 for a full ellipse)
            AddGroup(41, 0);
            // End parameter (this value is 2pi for a full ellipse)
            AddGroup(42, 6.28);
        }

        #endregion Private Methods
    }
}