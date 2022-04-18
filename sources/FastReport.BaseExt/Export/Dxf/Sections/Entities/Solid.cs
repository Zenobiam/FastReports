using FastReport.Export.Dxf.Utils;
using System.Drawing;

namespace FastReport.Export.Dxf.Sections.Entities
{
    internal class Solid : EntityBase
    {
        #region Private Fields

        private Color color;
        private float height;
        private float width;
        private float x;
        private float y;

        #endregion Private Fields

        #region Public Constructors

        public Solid(float x, float y, float width, float height, Color color) : base()
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.color = color;
            InitGroups();
        }

        #endregion Public Constructors

        #region Private Methods

        private void InitGroups()
        {
            // 0
            // SOLID
            // 8
            // barcode
            // 62
            // 18
            // 10
            // 1
            // 20
            // 2
            // 30
            // 0
            // 11
            // 2
            // 21
            // 2
            // 31
            // 0
            // 12
            // 1
            // 22
            // 1
            // 32
            // 0
            // 13
            // 2
            // 23
            // 1
            // 33
            // 0
            AddTypeName("SOLID");
            AddGroup(8, "Layer_1");
            byte aciColor = ACIDictionary.GetAciColor(color);
            AddColor(aciColor);
            AddPrimary2DPoint(x, y);
            AddSecondPoint(x + width, y);
            AddThirdPoint(x, y - height);
            AddFourthPoint(x + width, y - height);
        }

        #endregion Private Methods
    }
}