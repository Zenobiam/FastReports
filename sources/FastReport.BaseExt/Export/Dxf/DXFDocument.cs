using FastReport.Export.Dxf.Sections;
using FastReport.Export.Dxf.Sections.Tables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FastReport.Export.Dxf
{
    public class DxfDocument
    {
        #region Private Fields

        private SectionBlocks blocks;
        private StringBuilder docString;
        private SectionEntities entities;
        private SectionHeader header;
        private string name;
        private SectionTables tables;

        #endregion Private Fields

        #region Public Properties

        public SectionBlocks Blocks
        {
            get { return blocks; }
            set { blocks = value; }
        }

        public SectionEntities Entities
        {
            get { return entities; }
            set { entities = value; }
        }

        public SectionHeader Header
        {
            get { return header; }
            set { header = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public SectionTables Tables
        {
            get
            {
                if (tables == null)
                    tables = new SectionTables();
                return tables;
            }
            set { tables = value; }
        }

        #endregion Public Properties

        #region Public Constructors

        public DxfDocument()
        {
            docString = new StringBuilder();
            header = new SectionHeader();
            blocks = new SectionBlocks();
            tables = new SectionTables();
            entities = new SectionEntities();

            TableLayer layer1 = new TableLayer();
            Tables.Tables.Add(layer1);
        }

        #endregion Public Constructors

        #region Public Methods

        public void Clear()
        {
            name = string.Empty;
            header.Clear();
            blocks.Clear();
            tables.Clear();
            entities.Clear();
        }

        public void DrawLine(float x1, float y1, float x2, float y2, Color strokeColor, float strokeThickness)
        {
            DrawLine(x1, y1, x2, y2, strokeColor, strokeThickness, LineStyle.Solid);
        }

        public void DrawLine(float x1, float y1, float x2, float y2, Color strokeColor, float strokeThickness, LineStyle lineStyle)
        {
            if (Entities == null)
                throw new Exception("Entities section hasn't found!");
            else
            {
                List<LineStyle> styles = Entities.AddLine(x1, y1, x2, y2, strokeColor, strokeThickness, lineStyle);
                foreach (LineStyle s in styles)
                {
                    if (s == LineStyle.Dash)
                        Tables.Ltype.AddDashType();
                    else if (s == LineStyle.Dot)
                        Tables.Ltype.AddDotType();
                    else if (s == LineStyle.DashDot)
                        Tables.Ltype.AddDashDotType();
                    else if (s == LineStyle.DashDotDot)
                        Tables.Ltype.AddDashDotDotType();
                }
            }
        }

        public void Finish()
        {
            //Header.EndSection();
            //Entities.EndSection();
            //if(Tables != null)
            //    Tables.EndSection();
        }

        public override string ToString()
        {
            //StringBuilder DxfString = new StringBuilder();

            //DxfString.Append(Header.ToString()).Append("\n");
            //if (Tables != null)
            //    DxfString.Append(Tables.ToString()).Append("\n");

            //DxfString.Append(Entities.ToString()).Append("\n");

            // return DxfString.ToString();
            docString.Append("999\nCreated by FastReport.Net\n");
            Header.AppendTo(docString);
            docString.Append("\n");
            if (Tables != null)
            {
                Tables.AppendTo(docString);
                docString.Append("\n");
            }
            Blocks.AppendTo(docString);
            docString.Append("\n");
            Entities.AppendTo(docString);
            docString.Append("\n");
            docString.Append("0\nEOF");
            return docString.ToString();
        }

        #endregion Public Methods

        #region Internal Methods

        internal void DrawEllipse(float x, float y, float width, float height, Color ellipseColor, float ellipseWidth, LineStyle ellipseStyle)
        {
            if (Entities == null)
                throw new Exception("Entities section hasn't found!");
            else
            {
                List<LineStyle> styles = Entities.AddEllipse(x, y, width, height, ellipseColor, ellipseWidth, ellipseStyle);
                foreach (LineStyle s in styles)
                {
                    if (s == LineStyle.Dash)
                        Tables.Ltype.AddDashType();
                    else if (s == LineStyle.Dot)
                        Tables.Ltype.AddDotType();
                    else if (s == LineStyle.DashDot)
                        Tables.Ltype.AddDashDotType();
                    else if (s == LineStyle.DashDotDot)
                        Tables.Ltype.AddDashDotDotType();
                }
            }
        }

        internal void DrawPolygon(float x, float y, PointF[] points, byte[] pointTypes, Color polyLineColor,
           float polyLineWidth, LineStyle polyLineStyle)
        {
            DrawPolyLine(x, y, points, pointTypes, polyLineColor, polyLineWidth, polyLineStyle, true);
        }

        internal void DrawPolyLine(float x, float y, PointF[] points, byte[] pointTypes, Color polyLineColor,
                            float polyLineWidth, LineStyle polyLineStyle, bool isClosedPolyline)
        {
            if (Entities == null)
                throw new Exception("Entities section hasn't found!");
            else
            {
                List<LineStyle> styles = Entities.AddPolyLine(x, y, points, pointTypes, polyLineColor, polyLineWidth, polyLineStyle, isClosedPolyline);
                foreach (LineStyle s in styles)
                {
                    if (s == LineStyle.Dash)
                        Tables.Ltype.AddDashType();
                    else if (s == LineStyle.Dot)
                        Tables.Ltype.AddDotType();
                    else if (s == LineStyle.DashDot)
                        Tables.Ltype.AddDashDotType();
                    else if (s == LineStyle.DashDotDot)
                        Tables.Ltype.AddDashDotDotType();
                }
            }
        }

        internal void FillPolygon(float x, float y, PointF[] points, byte[] pointTypes, Color color)
        {
            Entities.AddHatch(x, y, points, pointTypes, color);
        }

        internal void FillRectangle(float x, float y, float width, float height, Color color)
        {
            //Entities.AddSolid(x, y, width, height, color);
            PointF[] points = new PointF[]
                {
                new PointF(0, 0),
                new PointF(width, 0),
                new PointF(width, height),
                new PointF(0, height)
                };
            FillPolygon(x, y, points, new byte[] { }, color);
        }

        #endregion Internal Methods
    }
}