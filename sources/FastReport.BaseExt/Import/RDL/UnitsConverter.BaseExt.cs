using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using FastReport.DataVisualization.Charting;

namespace FastReport.Import.RDL
{
    /// <summary>
    /// The RDL units converter.
    /// </summary>
    public static partial class UnitsConverter
    {
        #region Public Methods

        /// <summary>
        /// Converts the RDL GradientType to GradientStyle.
        /// </summary>
        /// <param name="gradientType">The RDL GradientType value.</param>
        /// <returns>The GradientStyle value.</returns>
        public static GradientStyle ConvertGradientType(string gradientType)
        {
            if (gradientType == "LeftRight")
            {
                return GradientStyle.LeftRight;
            }
            else if (gradientType == "TopBottom")
            {
                return GradientStyle.TopBottom;
            }
            else if (gradientType == "Center")
            {
                return GradientStyle.Center;
            }
            else if (gradientType == "DiagonalLeft")
            {
                return GradientStyle.DiagonalLeft;
            }
            else if (gradientType == "DiagonalRight")
            {
                return GradientStyle.DiagonalRight;
            }
            else if (gradientType == "HorizontalCenter")
            {
                return GradientStyle.HorizontalCenter;
            }
            else if (gradientType == "VerticalCenter")
            {
                return GradientStyle.VerticalCenter;
            }
            return GradientStyle.None;
        }

        /// <summary>
        /// Converts the RDL Chart.Type to SeriesChartType.
        /// </summary>
        /// <param name="chartType">The RDL Chart.Type value.</param>
        /// <returns>The SeriesChartType value.</returns>
        public static SeriesChartType ConvertChartType(string chartType)
        {
            if (chartType == "Area")
            {
                return SeriesChartType.Area;
            }
            else if (chartType == "Bar")
            {
                return SeriesChartType.Bar;
            }
            else if (chartType == "Doughnut")
            {
                return SeriesChartType.Doughnut;
            }
            else if (chartType == "Line")
            {
                return SeriesChartType.Line;
            }
            else if (chartType == "Pie")
            {
                return SeriesChartType.Pie;
            }
            else if (chartType == "Bubble")
            {
                return SeriesChartType.Bubble;
            }
            return SeriesChartType.Column;
        }

        /// <summary>
        /// Converts the RDL Chart.Palette to ChartColorPalette.
        /// </summary>
        /// <param name="chartPalette">The RDL Chart.Palette value.</param>
        /// <returns>The RDL ChartColorPalette value.</returns>
        public static ChartColorPalette ConvertChartPalette(string chartPalette)
        {
            if (chartPalette == "EarthTones")
            {
                return ChartColorPalette.EarthTones;
            }
            else if (chartPalette == "Excel")
            {
                return ChartColorPalette.Excel;
            }
            else if (chartPalette == "GrayScale")
            {
                return ChartColorPalette.Grayscale;
            }
            else if (chartPalette == "Light")
            {
                return ChartColorPalette.Light;
            }
            else if (chartPalette == "Pastel")
            {
                return ChartColorPalette.Pastel;
            }
            else if (chartPalette == "SemiTransparent")
            {
                return ChartColorPalette.SemiTransparent;
            }
            return ChartColorPalette.None;
        }

        /// <summary>
        /// Converts the RDL Chart.Legend.Position to Legend.Docking and Legend.Alignment.
        /// </summary>
        /// <param name="chartLegendPosition">The RDL Chart.Legend.Position value.</param>
        /// <param name="legend">The Legend instance to convert to.</param>
        public static void ConvertChartLegendPosition(string chartLegendPosition, Legend legend)
        {
            if (chartLegendPosition == "TopLeft")
            {
                legend.Docking = Docking.Top;
                legend.Alignment = StringAlignment.Near;
            }
            else if (chartLegendPosition == "TopCenter")
            {
                legend.Docking = Docking.Top;
                legend.Alignment = StringAlignment.Center;
            }
            else if (chartLegendPosition == "TopRight")
            {
                legend.Docking = Docking.Top;
                legend.Alignment = StringAlignment.Far;
            }
            else if (chartLegendPosition == "LeftTop")
            {
                legend.Docking = Docking.Left;
                legend.Alignment = StringAlignment.Near;
            }
            else if (chartLegendPosition == "LeftCenter")
            {
                legend.Docking = Docking.Left;
                legend.Alignment = StringAlignment.Center;
            }
            else if (chartLegendPosition == "LeftBottom")
            {
                legend.Docking = Docking.Left;
                legend.Alignment = StringAlignment.Far;
            }
            else if (chartLegendPosition == "RightTop")
            {
                legend.Docking = Docking.Right;
                legend.Alignment = StringAlignment.Near;
            }
            else if (chartLegendPosition == "RightCenter")
            {
                legend.Docking = Docking.Right;
                legend.Alignment = StringAlignment.Center;
            }
            else if (chartLegendPosition == "RightBottom")
            {
                legend.Docking = Docking.Right;
                legend.Alignment = StringAlignment.Far;
            }
            else if (chartLegendPosition == "BottomLeft")
            {
                legend.Docking = Docking.Bottom;
                legend.Alignment = StringAlignment.Near;
            }
            else if (chartLegendPosition == "BottomCenter")
            {
                legend.Docking = Docking.Bottom;
                legend.Alignment = StringAlignment.Center;
            }
            else if (chartLegendPosition == "BottomRight")
            {
                legend.Docking = Docking.Bottom;
                legend.Alignment = StringAlignment.Far;
            }
        }

        /// <summary>
        /// Converts the RDL Chart.Legend.Layout to LegendStyle.
        /// </summary>
        /// <param name="chartLegendLayout">The RDL Chart.Legend.Layout value.</param>
        /// <returns>The LegendStyle value.</returns>
        public static LegendStyle ConvertChartLegendLayout(string chartLegendLayout)
        {
            if (chartLegendLayout == "Table")
            {
                return LegendStyle.Table;
            }
            else if (chartLegendLayout == "Row")
            {
                return LegendStyle.Row;
            }
            return LegendStyle.Column;
        }

        /// <summary>
        /// Converts the RDL BorderStyle to ChartDashStyle.
        /// </summary>
        /// <param name="borderStyle">The RDL BorderStyle value.</param>
        /// <returns>The ChartDashStyle value.</returns>
        public static ChartDashStyle ConvertBorderStyleToChartDashStyle(string borderStyle)
        {
            if (borderStyle == "Dotted")
            {
                return ChartDashStyle.Dot;
            }
            else if (borderStyle == "Dashed")
            {
                return ChartDashStyle.Dash;
            }
            return ChartDashStyle.Solid;
        }

        /// <summary>
        /// Converts the RDL Axis.Visible to AxisEnabled.
        /// </summary>
        /// <param name="axisVisible">The RDL Axis.Visible value.</param>
        /// <returns>The AxisEnabled value.</returns>
        public static AxisEnabled ConvertAxisVisibleToAxisEnabled(string axisVisible)
        {
            if (axisVisible.ToLower() == "true")
            {
                return AxisEnabled.True;
            }
            else if (axisVisible.ToLower() == "false")
            {
                return AxisEnabled.False;
            }
            return AxisEnabled.Auto;
        }

        /// <summary>
        /// Converts the RDL TickMarkStyle to TickMarkStyle.
        /// </summary>
        /// <param name="tickMarkStyle">The RDL TickMarkStyle value.</param>
        /// <returns>The TickMarkStyle value.</returns>
        public static TickMarkStyle ConvertTickMarkStyle(string tickMarkStyle)
        {
            if (tickMarkStyle == "Inside")
            {
                return TickMarkStyle.InsideArea;
            }
            else if (tickMarkStyle == "Outside")
            {
                return TickMarkStyle.OutsideArea;
            }
            else if (tickMarkStyle == "Cross")
            {
                return TickMarkStyle.AcrossAxis;
            }
            return TickMarkStyle.None;
        }

        /// <summary>
        /// Converts the RDL Shading to LightStyle.
        /// </summary>
        /// <param name="shading">The RDL Shading value.</param>
        /// <returns>The LightStyle value.</returns>
        public static LightStyle ConvertShading(string shading)
        {
            if (shading == "Simple")
            {
                return LightStyle.Simplistic;
            }
            else if (shading == "Real")
            {
                return LightStyle.Realistic;
            }
            return LightStyle.None;
        }

        #endregion // Public Methods
    }
}
