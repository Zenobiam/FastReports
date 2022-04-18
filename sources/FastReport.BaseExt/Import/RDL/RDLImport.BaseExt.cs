using System;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using FastReport.MSChart;
using FastReport.Table;

namespace FastReport.Import.RDL
{
    /// <summary>
    /// Represents the RDL import plugin.
    /// </summary>
    public partial class RDLImport : ImportBase
    {
        #region Private Methods

        private void LoadBorderColorExt(XmlNode borderColorNode)
        {
            XmlNodeList nodeList = borderColorNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Default")
                {
                    if (component is MSChartObject)
                    {
                        (component as MSChartObject).Chart.BorderlineColor = UnitsConverter.ConvertColor(node.InnerText);
                    }
                    else if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.Color = UnitsConverter.ConvertColor(node.InnerText);
                    }
                }
                else if (node.Name == "Top")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.TopLine.Color = UnitsConverter.ConvertColor(node.InnerText);
                    }
                }
                else if (node.Name == "Left")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.LeftLine.Color = UnitsConverter.ConvertColor(node.InnerText);
                    }
                }
                else if (node.Name == "Right")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.RightLine.Color = UnitsConverter.ConvertColor(node.InnerText);
                    }
                }
                else if (node.Name == "Bottom")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.BottomLine.Color = UnitsConverter.ConvertColor(node.InnerText);
                    }
                }
            }
        }

        private void LoadBorderStyleExt(XmlNode borderStyleNode)
        {
            XmlNodeList nodeList = borderStyleNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Default")
                {
                    if (component is MSChartObject)
                    {
                        (component as MSChartObject).Chart.BorderlineDashStyle = UnitsConverter.ConvertBorderStyleToChartDashStyle(node.InnerText);
                    }
                    else if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.Lines = BorderLines.All;
                        (component as ReportComponentBase).Border.Style = UnitsConverter.ConvertBorderStyle(node.InnerText);
                    }
                }
                else if (node.Name == "Top")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.Lines |= BorderLines.Top;
                        (component as ReportComponentBase).Border.TopLine.Style = UnitsConverter.ConvertBorderStyle(node.InnerText);
                    }
                }
                else if (node.Name == "Left")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.Lines |= BorderLines.Left;
                        (component as ReportComponentBase).Border.LeftLine.Style = UnitsConverter.ConvertBorderStyle(node.InnerText);
                    }
                }
                else if (node.Name == "Right")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.Lines |= BorderLines.Right;
                        (component as ReportComponentBase).Border.RightLine.Style = UnitsConverter.ConvertBorderStyle(node.InnerText);
                    }
                }
                else if (node.Name == "Bottom")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.Lines |= BorderLines.Bottom;
                        (component as ReportComponentBase).Border.BottomLine.Style = UnitsConverter.ConvertBorderStyle(node.InnerText);
                    }
                }
            }
        }

        private void LoadBorderWidthExt(XmlNode borderWidthNode)
        {
            XmlNodeList nodeList = borderWidthNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Default")
                {
                    if (component is MSChartObject)
                    {
                        (component as MSChartObject).Chart.BorderlineWidth = (int)UnitsConverter.SizeToPixels(node.InnerText);
                    }
                    else if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.Width = UnitsConverter.SizeToPixels(node.InnerText);
                    }
                }
                else if (node.Name == "Top")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.TopLine.Width = UnitsConverter.SizeToPixels(node.InnerText);
                    }
                }
                else if (node.Name == "Left")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.LeftLine.Width = UnitsConverter.SizeToPixels(node.InnerText);
                    }
                }
                else if (node.Name == "Right")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.RightLine.Width = UnitsConverter.SizeToPixels(node.InnerText);
                    }
                }
                else if (node.Name == "Bottom")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.BottomLine.Width = UnitsConverter.SizeToPixels(node.InnerText);
                    }
                }
            }
        }

        private void LoadStyleExt(XmlNode styleNode)
        {
            FontStyle fontStyle = FontStyle.Regular;
            string fontFamily = "Arial";
            float fontSize = 10.0f;
            int paddingTop = 0;
            int paddingLeft = 2;
            int paddingRight = 2;
            int paddingBottom = 0;
            XmlNodeList nodeList = styleNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "BorderColor")
                {
                    LoadBorderColorExt(node);
                }
                else if (node.Name == "BorderStyle")
                {
                    LoadBorderStyleExt(node);
                }
                else if (node.Name == "BorderWidth")
                {
                    LoadBorderWidthExt(node);
                }
                else if (node.Name == "BackgroundColor")
                {
                    if (component is ShapeObject)
                    {
                        (component as ShapeObject).FillColor = UnitsConverter.ConvertColor(node.InnerText);
                    }
                    else if (component is MSChartObject)
                    {
                        (component as MSChartObject).Chart.BackColor = UnitsConverter.ConvertColor(node.InnerText);
                    }
                    else if (component is TableObject)
                    {
                        (component as TableObject).FillColor = UnitsConverter.ConvertColor(node.InnerText);
                    }
                    //else if (component is SubreportObject)
                    //{
                    //    (component as SubreportObject).FillColor = UnitsConverter.ConvertColor(node.InnerText);
                    //}
                }
                else if (node.Name == "BackgroundGradientType")
                {
                    if (component is MSChartObject)
                    {
                        (component as MSChartObject).Chart.BackGradientStyle = UnitsConverter.ConvertGradientType(node.InnerText);
                    }
                }
                else if (node.Name == "BackgroundGradientEndColor")
                {
                    if (component is MSChartObject)
                    {
                        (component as MSChartObject).Chart.BackSecondaryColor = UnitsConverter.ConvertColor(node.InnerText);
                    }
                }
                else if (node.Name == "FontStyle")
                {
                    fontStyle = UnitsConverter.ConvertFontStyle(node.InnerText);
                }
                else if (node.Name == "FontFamily")
                {
                    fontFamily = node.InnerText;
                }
                else if (node.Name == "FontSize")
                {
                    fontSize = Convert.ToSingle(UnitsConverter.ConvertFontSize(node.InnerText));
                }
                else if (node.Name == "TextAlign")
                {
                    if (component is TextObject)
                    {
                        (component as TextObject).HorzAlign = UnitsConverter.ConvertTextAlign(node.InnerText);
                    }
                }
                else if (node.Name == "VerticalAlign")
                {
                    if (component is TextObject)
                    {
                        (component as TextObject).VertAlign = UnitsConverter.ConvertVerticalAlign(node.InnerText);
                    }
                }
                else if (node.Name == "WritingMode")
                {
                    if (component is TextObject)
                    {
                        (component as TextObject).Angle = UnitsConverter.ConvertWritingMode(node.InnerText);
                    }
                }
                else if (node.Name == "Color")
                {
                    if (component is TextObject)
                    {
                        (component as TextObject).TextColor = UnitsConverter.ConvertColor(node.InnerText);
                    }
                }
                else if (node.Name == "PaddingLeft")
                {
                    paddingLeft = UnitsConverter.SizeToInt(node.InnerText, SizeUnits.Point);
                }
                else if (node.Name == "PaddingRight")
                {
                    paddingRight = UnitsConverter.SizeToInt(node.InnerText, SizeUnits.Point);
                }
                else if (node.Name == "PaddingTop")
                {
                    paddingTop = UnitsConverter.SizeToInt(node.InnerText, SizeUnits.Point);
                }
                else if (node.Name == "PaddingBottom")
                {
                    paddingBottom = UnitsConverter.SizeToInt(node.InnerText, SizeUnits.Point);
                }
            }
            if (component is TextObject)
            {
                (component as TextObject).Font = new Font(fontFamily, fontSize, fontStyle);
                (component as TextObject).Padding = new Padding(paddingLeft, paddingTop, paddingRight, paddingBottom);
            }
            else if (component is PictureObject)
            {
                (component as PictureObject).Padding = new Padding(paddingLeft, paddingTop, paddingRight, paddingBottom);
            }
#if !FRCORE
            else if (component is MSChartObject)
            {
                (component as MSChartObject).Chart.Padding = new Padding(paddingLeft, paddingTop, paddingRight, paddingBottom);
            }
#endif
        }

        private void LoadThreeDProperties(XmlNode threeDPropertiesNode)
        {
            XmlNodeList nodeList = threeDPropertiesNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Enabled")
                {
                    (component as MSChartObject).Chart.ChartAreas[0].Area3DStyle.Enable3D = UnitsConverter.BooleanToBool(node.InnerText);
                }
                else if (node.Name == "Rotation")
                {
                    int rotation = Convert.ToInt32(node.InnerText);
                    if (rotation >= -180 && rotation <= 180)
                    {
                        (component as MSChartObject).Chart.ChartAreas[0].Area3DStyle.Rotation = rotation;
                    }
                    else
                    {
                        (component as MSChartObject).Chart.ChartAreas[0].Area3DStyle.Rotation = 30;
                    }
                }
                else if (node.Name == "Inclination")
                {
                    int inclination = Convert.ToInt32(node.InnerText);
                    if (inclination >= -90 && inclination <= 90)
                    {
                        (component as MSChartObject).Chart.ChartAreas[0].Area3DStyle.Inclination = inclination;
                    }
                    else
                    {
                        (component as MSChartObject).Chart.ChartAreas[0].Area3DStyle.Inclination = 30;
                    }
                }
                else if (node.Name == "Perspective")
                {
                    int perspective = Convert.ToInt32(node.InnerText);
                    if (perspective >= 0 && perspective <= 60)
                    {
                        (component as MSChartObject).Chart.ChartAreas[0].Area3DStyle.Perspective = perspective;
                    }
                    else
                    {
                        (component as MSChartObject).Chart.ChartAreas[0].Area3DStyle.Perspective = 0;
                    }
                }
                else if (node.Name == "DepthRatio")
                {
                    (component as MSChartObject).Chart.ChartAreas[0].Area3DStyle.PointDepth = Convert.ToInt32(node.InnerText);
                }
                else if (node.Name == "Shading")
                {
                    (component as MSChartObject).Chart.ChartAreas[0].Area3DStyle.LightStyle = UnitsConverter.ConvertShading(node.InnerText);
                }
                else if (node.Name == "GapDepth")
                {
                    (component as MSChartObject).Chart.ChartAreas[0].Area3DStyle.PointGapDepth = Convert.ToInt32(node.InnerText);
                }
                else if (node.Name == "WallThickness")
                {
                    int width = Convert.ToInt32(node.InnerText);
                    if (width < 0)
                    {
                        (component as MSChartObject).Chart.ChartAreas[0].Area3DStyle.WallWidth = 0;
                    }
                    else if (width > 30)
                    {
                        (component as MSChartObject).Chart.ChartAreas[0].Area3DStyle.WallWidth = 30;
                    }
                    else
                    {
                        (component as MSChartObject).Chart.ChartAreas[0].Area3DStyle.WallWidth = width;
                    }
                }
                else if (node.Name == "Clustered")
                {
                    (component as MSChartObject).Chart.ChartAreas[0].Area3DStyle.IsClustered = UnitsConverter.BooleanToBool(node.InnerText);
                }
            }
        }

        #endregion // Private Methods
    }
}
