using FastReport.MSChart;
using System;
using System.Drawing;
using FastReport.DataVisualization.Charting;
using System.Xml;

namespace FastReport.Import.RDL
{
    public partial class RDLImport
    {
        private void LoadBorderColorT(XmlNode borderColorNode, Title title)
        {
            XmlNodeList nodeList = borderColorNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Default")
                {
                    title.BorderColor = UnitsConverter.ConvertColor(node.InnerText);
                }
            }
        }

        private void LoadBorderStyleT(XmlNode borderStyleNode, Title title)
        {
            XmlNodeList nodeList = borderStyleNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Default")
                {
                    title.BorderDashStyle = UnitsConverter.ConvertBorderStyleToChartDashStyle(node.InnerText);
                }
            }
        }

        private void LoadBorderWidthT(XmlNode borderWidthNode, Title title)
        {
            XmlNodeList nodeList = borderWidthNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Default")
                {
                    title.BorderWidth = (int)UnitsConverter.SizeToPixels(node.InnerText);
                }
            }
        }

        private void LoadStyleT(XmlNode styleNode, Title title)
        {
            FontStyle fontStyle = FontStyle.Regular;
            string fontFamily = "Arial";
            float fontSize = 10.0f;
            string textAlign = "";
            string vertAlign = "";
            XmlNodeList nodeList = styleNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "BorderColor")
                {
                    LoadBorderColorT(node, title);
                }
                else if (node.Name == "BorderStyle")
                {
                    LoadBorderStyleT(node, title);
                }
                else if (node.Name == "BorderWidth")
                {
                    LoadBorderWidthT(node, title);
                }
                else if (node.Name == "BackgroundColor")
                {
                    title.BackColor = UnitsConverter.ConvertColor(node.InnerText);
                }
                else if (node.Name == "BackgroundGradientType")
                {
                    title.BackGradientStyle = UnitsConverter.ConvertGradientType(node.InnerText);
                }
                else if (node.Name == "BackgroundGradientEndColor")
                {
                    title.BackSecondaryColor = UnitsConverter.ConvertColor(node.InnerText);
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
                    textAlign = node.InnerText;
                }
                else if (node.Name == "VerticalAlign")
                {
                    vertAlign = node.InnerText;
                }
                else if (node.Name == "Color")
                {
                    title.ForeColor = UnitsConverter.ConvertColor(node.InnerText);
                }
            }
            title.Font = new Font(fontFamily, fontSize, fontStyle);
            title.Alignment = UnitsConverter.ConvertTextAndVerticalAlign(textAlign, vertAlign);
        }

        private void LoadTitleA(XmlNode titleNode, Axis axis)
        {
            XmlNodeList nodeList = titleNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Caption")
                {
                    axis.Title = node.InnerText;
                }
                else if (node.Name == "Style")
                {
                    string fontFamily = "Arial";
                    float fontSize = 10.0f;
                    FontStyle fontStyle = FontStyle.Regular;
                    foreach (XmlNode styleNode in node.ChildNodes)
                    {
                        if (styleNode.Name == "FontStyle")
                        {
                            fontStyle = UnitsConverter.ConvertFontStyle(styleNode.InnerText);
                        }
                        else if (styleNode.Name == "FontFamily")
                        {
                            fontFamily = styleNode.InnerText;
                        }
                        else if (styleNode.Name == "FontSize")
                        {
                            fontSize = Convert.ToSingle(UnitsConverter.ConvertFontSize(styleNode.InnerText));
                        }
                        else if (styleNode.Name == "TextAlign")
                        {
                            axis.TitleAlignment = UnitsConverter.ConvertTextAlignToStringAlignment(styleNode.InnerText);
                        }
                        else if (styleNode.Name == "Color")
                        {
                            axis.TitleForeColor = UnitsConverter.ConvertColor(styleNode.InnerText);
                        }
                    }
                    axis.TitleFont = new Font(fontFamily, fontSize, fontStyle);
                }
            }
        }

        private void LoadDataPoint(XmlNode dataPointNode)
        {
            XmlNodeList nodeList = dataPointNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Style")
                {
                    LoadStyleExt(node);
                }
            }
        }

        private void LoadDataPoints(XmlNode dataPointsNode)
        {
            XmlNodeList nodeList = dataPointsNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "DataPoint")
                {
                    LoadDataPoint(node);
                }
            }
        }

        private void LoadChartSeries(XmlNode chartSeriesNode)
        {
            XmlNodeList nodeList = chartSeriesNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "DataPoints")
                {
                    LoadDataPoints(node);
                }
            }
        }

        private void LoadChartData(XmlNode chartDataNode)
        {
            XmlNodeList nodeList = chartDataNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "ChartSeries")
                {
                    LoadChartSeries(node);
                }
            }
        }

        private void LoadLegend(XmlNode legendNode)
        {
            Legend legend = (component as MSChartObject).Chart.Legends[0];
            XmlNodeList nodeList = legendNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Visible")
                {
                    legend.Enabled = UnitsConverter.BooleanToBool(node.InnerText);
                }
                else if (node.Name == "Style")
                {
                    LoadStyleExt(node);
                }
                else if (node.Name == "Position")
                {
                    UnitsConverter.ConvertChartLegendPosition(node.InnerText, legend);
                }
                else if (node.Name == "Layout")
                {
                    legend.LegendStyle = UnitsConverter.ConvertChartLegendLayout(node.InnerText);
                }
                else if (node.Name == "InsidePlotArea")
                {
                    legend.IsDockedInsideChartArea = UnitsConverter.BooleanToBool(node.InnerText);
                }
            }
        }
        private void LoadTitle(XmlNode titleNode)
        {
            Title title = (component as MSChartObject).Chart.Titles[0];
            XmlNodeList nodeList = titleNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Caption")
                {
                    title.Text = node.InnerText;
                }
                else if (node.Name == "Style")
                {
                    LoadStyleT(node, title);
                }
            }
        }

        private void LoadGridLines(XmlNode gridLinesNode, Grid grid)
        {
            XmlNodeList nodeList = gridLinesNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "ShowGridLines")
                {
                    grid.Enabled = UnitsConverter.BooleanToBool(node.InnerText);
                }
                else if (node.Name == "Style")
                {
                    foreach (XmlNode subNode in node.ChildNodes)
                    {
                        if (subNode.Name == "BorderStyle")
                        {
                            grid.LineDashStyle = UnitsConverter.ConvertBorderStyleToChartDashStyle(subNode.FirstChild.InnerText);
                        }
                        else if (subNode.Name == "BorderColor")
                        {
                            grid.LineColor = UnitsConverter.ConvertColor(subNode.FirstChild.InnerText);
                        }
                        else if (subNode.Name == "BorderWidth")
                        {
                            grid.LineWidth = (int)UnitsConverter.SizeToPixels(subNode.FirstChild.InnerText);
                        }
                    }
                }
            }
        }

        private void LoadAxis(XmlNode axisNode, Axis axis)
        {
            XmlNodeList nodeList = axisNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Visible")
                {
                    axis.Enabled = UnitsConverter.ConvertAxisVisibleToAxisEnabled(node.InnerText);
                }
                else if (node.Name == "Title")
                {
                    LoadTitleA(node, axis);
                }
                else if (node.Name == "Margin")
                {
                    axis.IsMarginVisible = UnitsConverter.BooleanToBool(node.InnerText);
                }
                else if (node.Name == "MajorTickMarks")
                {
                    axis.MajorTickMark.TickMarkStyle = UnitsConverter.ConvertTickMarkStyle(node.InnerText);
                }
                else if (node.Name == "MinorTickMarks")
                {
                    axis.MinorTickMark.TickMarkStyle = UnitsConverter.ConvertTickMarkStyle(node.InnerText);
                }
                else if (node.Name == "MajorGridLines")
                {
                    LoadGridLines(node, axis.MajorGrid);
                }
                else if (node.Name == "MinorGridLines")
                {
                    LoadGridLines(node, axis.MinorGrid);
                }
                else if (node.Name == "Reverse")
                {
                    axis.IsReversed = UnitsConverter.BooleanToBool(node.InnerText);
                }
                else if (node.Name == "Interlaced")
                {
                    axis.IsInterlaced = UnitsConverter.BooleanToBool(node.InnerText);
                }
            }
        }

        private void LoadCategoryAxis(XmlNode categoryAxisNode)
        {
            XmlNodeList nodeList = categoryAxisNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Axis")
                {
                    LoadAxis(node, (component as MSChartObject).Chart.ChartAreas[0].AxisX);
                }
            }
        }

        private void LoadValueAxis(XmlNode valueAxisNode)
        {
            XmlNodeList nodeList = valueAxisNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Axis")
                {
                    LoadAxis(node, (component as MSChartObject).Chart.ChartAreas[0].AxisY);
                }
            }
        }


        partial void LoadChart(XmlNode chartNode)
        {
            component = ComponentsFactory.CreateMSChartObject(chartNode.Attributes["Name"].Value, parent);
            MSChartObject chart = (component as MSChartObject);
            chart.Series.Clear();
            MSChartSeries series = new MSChartSeries();
            chart.Series.Add(series);
            series.CreateUniqueName();
            chart.Chart.Series.Add(new Series());
            Legend legend = new Legend();
            chart.Chart.Legends.Add(legend);
            legend.Enabled = false;
            legend.BackColor = Color.Transparent;
            Title title = new Title();
            chart.Chart.Titles.Add(title);
            title.Visible = true;
            ChartArea chartArea = new ChartArea("Default");
            chart.Chart.ChartAreas.Add(chartArea);
            chartArea.BackColor = Color.Transparent;
            chartArea.AxisX = new Axis();
            chartArea.AxisX.IsMarginVisible = false;
            chartArea.AxisY = new Axis();
            chartArea.AxisY.IsMarginVisible = false;
            chartArea.Area3DStyle.LightStyle = LightStyle.None;

            XmlNodeList nodeList = chartNode.ChildNodes;
            LoadReportItem(nodeList);
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Type")
                {
                    series.SeriesSettings.ChartType = UnitsConverter.ConvertChartType(node.InnerText);
                }
                else if (node.Name == "Legend")
                {
                    LoadLegend(node);
                }
                else if (node.Name == "CategoryAxis")
                {
                    LoadCategoryAxis(node);
                }
                else if (node.Name == "ValueAxis")
                {
                    LoadValueAxis(node);
                }
                else if (node.Name == "Title")
                {
                    LoadTitle(node);
                }
                else if (node.Name == "Palette")
                {
                    chart.Chart.Palette = UnitsConverter.ConvertChartPalette(node.InnerText);
                }
                else if (node.Name == "ThreeDProperties")
                {
                    LoadThreeDProperties(node);
                }
            }
        }
    }
}
