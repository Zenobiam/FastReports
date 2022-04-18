using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Import;
using FastReport.Utils;
using FastReport.DataVisualization.Charting;
using FastReport.MSChart;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace FastReport.Design.ImportPlugins.RPT
{
    /// <summary>
    /// Represents the RPT import plugin.
    /// </summary>
    public class RPTImportPlugin : ImportPlugin
    {
        #region Fields

        private FastReport.ReportPage page;
        private CrystalDecisions.CrystalReports.Engine.ReportClass crystalReport;

        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RPTImportPlugin"/> class.
        /// </summary>
        public RPTImportPlugin() : base()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RPTImportPlugin"/> class with a specified designer.
        /// </summary>
        public RPTImportPlugin(Designer designer) : base(designer)
        {
            Initialize();
        }

        #endregion // Constructors

        #region Private Methods

        private void Initialize()
        {
            crystalReport = new CrystalDecisions.CrystalReports.Engine.ReportClass();
        }

        private void LoadReportInfo()
        {
            if (crystalReport.SummaryInfo != null)
            {
                CrystalDecisions.CrystalReports.Engine.SummaryInfo summaryInfo = crystalReport.SummaryInfo;
                Report.ReportInfo.Name = UnitsConverter.ConvertString(crystalReport.Name);
                Report.ReportInfo.Author = UnitsConverter.ConvertString(summaryInfo.ReportAuthor);
                Report.ReportInfo.Description = UnitsConverter.ConvertString(summaryInfo.ReportComments);
            }
            string filename = crystalReport.FileName.Remove(0, crystalReport.FileName.LastIndexOf("//") + 2);
            Report.ReportInfo.Created = File.GetCreationTime(filename);
            Report.ReportInfo.Modified = File.GetLastWriteTime(filename);
        }

        private void LoadPrintSettings()
        {
            if (crystalReport.PrintOptions != null)
            {
                CrystalDecisions.CrystalReports.Engine.PrintOptions printOptions = crystalReport.PrintOptions;
                Report.PrintSettings.Printer = printOptions.PrinterName;
                if (!String.IsNullOrEmpty(Report.PrintSettings.Printer))
                {
                    Report.PrintSettings.SavePrinterWithReport = true;
                }
                Report.PrintSettings.Duplex = UnitsConverter.ConvertPrinterDuplex(printOptions.PrinterDuplex);
            }
        }

        private void LoadPageSettings()
        {
            if (crystalReport.PrintOptions != null)
            {
                CrystalDecisions.CrystalReports.Engine.PrintOptions printOptions = crystalReport.PrintOptions;
                UnitsConverter.ConvertPaperSize(printOptions.PaperSize, printOptions.PaperOrientation, page);
                page.TopMargin = UnitsConverter.TwipsToMillimeters(printOptions.PageMargins.topMargin);
                page.LeftMargin = UnitsConverter.TwipsToMillimeters(printOptions.PageMargins.leftMargin);
                page.RightMargin = UnitsConverter.TwipsToMillimeters(printOptions.PageMargins.rightMargin);
                page.BottomMargin = UnitsConverter.TwipsToMillimeters(printOptions.PageMargins.bottomMargin);
            }
        }

        private void LoadComponent(CrystalDecisions.CrystalReports.Engine.ReportObject obj, FastReport.ComponentBase comp)
        {
            if (obj != null && comp != null)
            {
                comp.Left = UnitsConverter.TwipsToPixels(obj.Left);
                comp.Top = UnitsConverter.TwipsToPixels(obj.Top);
                comp.Width = UnitsConverter.TwipsToPixels(obj.Width);
                comp.Height = UnitsConverter.TwipsToPixels(obj.Height);
            }
        }

        private void LoadTextObject(CrystalDecisions.CrystalReports.Engine.TextObject crystalText, FastReport.TextObject fastText)
        {
            if (crystalText != null && fastText != null)
            {
                LoadComponent(crystalText, fastText);
                fastText.CanGrow = crystalText.ObjectFormat.EnableCanGrow;
                fastText.Font = crystalText.Font;
                fastText.TextColor = crystalText.Color;
                fastText.FillColor = crystalText.Border.BackgroundColor;
                fastText.Border.Color = crystalText.Border.BorderColor;
                fastText.Border.Lines = UnitsConverter.ConvertBorderToBorderLines(crystalText.Border);
                fastText.Border.TopLine.Style = UnitsConverter.ConvertLineStyle(crystalText.Border.TopLineStyle);
                fastText.Border.LeftLine.Style = UnitsConverter.ConvertLineStyle(crystalText.Border.LeftLineStyle);
                fastText.Border.RightLine.Style = UnitsConverter.ConvertLineStyle(crystalText.Border.RightLineStyle);
                fastText.Border.BottomLine.Style = UnitsConverter.ConvertLineStyle(crystalText.Border.BottomLineStyle);
                fastText.Border.Shadow = crystalText.Border.HasDropShadow;
                fastText.HorzAlign = UnitsConverter.ConvertAlignment(crystalText.ObjectFormat.HorizontalAlignment);
                fastText.Text = crystalText.Text;
            }
        }

        private void LoadTextObject(CrystalDecisions.CrystalReports.Engine.FieldObject crystalField, FastReport.TextObject fastText)
        {
            if (crystalField != null && fastText != null)
            {
                LoadComponent(crystalField, fastText);
                fastText.CanGrow = crystalField.ObjectFormat.EnableCanGrow;
                fastText.Font = crystalField.Font;
                fastText.TextColor = crystalField.Color;
                fastText.FillColor = crystalField.Border.BackgroundColor;
                fastText.Border.Color = crystalField.Border.BorderColor;
                fastText.Border.Lines = UnitsConverter.ConvertBorderToBorderLines(crystalField.Border);
                fastText.Border.TopLine.Style = UnitsConverter.ConvertLineStyle(crystalField.Border.TopLineStyle);
                fastText.Border.LeftLine.Style = UnitsConverter.ConvertLineStyle(crystalField.Border.LeftLineStyle);
                fastText.Border.RightLine.Style = UnitsConverter.ConvertLineStyle(crystalField.Border.RightLineStyle);
                fastText.Border.BottomLine.Style = UnitsConverter.ConvertLineStyle(crystalField.Border.BottomLineStyle);
                fastText.Border.Shadow = crystalField.Border.HasDropShadow;
                fastText.HorzAlign = UnitsConverter.ConvertAlignment(crystalField.ObjectFormat.HorizontalAlignment);
            }
        }

        private void LoadLineObject(CrystalDecisions.CrystalReports.Engine.LineObject crystalLine, FastReport.LineObject fastLine)
        {
            if (crystalLine != null && fastLine != null)
            {
                LoadComponent(crystalLine, fastLine);
                fastLine.Border.Color = crystalLine.LineColor;
                if (crystalLine.LineThickness == 0)
                {
                    fastLine.Border.Width = 0.25f;
                }
                else if (crystalLine.LineThickness == 10)
                {
                    fastLine.Border.Width = 0.5f;
                }
                else
                {
                    fastLine.Border.Width = UnitsConverter.TwipsToPixels(crystalLine.LineThickness);
                }
                fastLine.Border.Style = UnitsConverter.ConvertLineStyle(crystalLine.LineStyle);
            }
        }

        private void LoadShapeObject(CrystalDecisions.CrystalReports.Engine.BoxObject crystalBox, FastReport.ShapeObject fastRectangle)
        {
            if (crystalBox != null && fastRectangle != null)
            {
                LoadComponent(crystalBox, fastRectangle);
                fastRectangle.FillColor = crystalBox.FillColor;
                fastRectangle.Border.Color = crystalBox.LineColor;
                fastRectangle.Border.Shadow = crystalBox.Border.HasDropShadow;
                fastRectangle.Border.ShadowColor = crystalBox.LineColor;
                if (crystalBox.LineThickness == 0)
                {
                    fastRectangle.Border.Width = 0.25f;
                }
                else if (crystalBox.LineThickness == 10)
                {
                    fastRectangle.Border.Width = 0.5f;
                }
                else
                {
                    fastRectangle.Border.Width = UnitsConverter.TwipsToPixels(crystalBox.LineThickness);
                }
                fastRectangle.Border.Style = UnitsConverter.ConvertLineStyle(crystalBox.LineStyle);
            }
        }

        private void LoadPictureObject(CrystalDecisions.CrystalReports.Engine.PictureObject crystalPicture, FastReport.PictureObject fastPicture)
        {
            if (crystalPicture != null && fastPicture != null)
            {
                LoadComponent(crystalPicture, fastPicture);
                fastPicture.Border.Color = crystalPicture.Border.BorderColor;
                fastPicture.FillColor = crystalPicture.Border.BackgroundColor;
                fastPicture.Border.Lines = UnitsConverter.ConvertBorderToBorderLines(crystalPicture.Border);
                fastPicture.Border.TopLine.Style = UnitsConverter.ConvertLineStyle(crystalPicture.Border.TopLineStyle);
                fastPicture.Border.LeftLine.Style = UnitsConverter.ConvertLineStyle(crystalPicture.Border.LeftLineStyle);
                fastPicture.Border.RightLine.Style = UnitsConverter.ConvertLineStyle(crystalPicture.Border.RightLineStyle);
                fastPicture.Border.BottomLine.Style = UnitsConverter.ConvertLineStyle(crystalPicture.Border.BottomLineStyle);
                fastPicture.Border.Shadow = crystalPicture.Border.HasDropShadow;
                fastPicture.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
        
        private void LoadMSChartObject(CrystalDecisions.CrystalReports.Engine.ChartObject crystalChart, FastReport.MSChart.MSChartObject fastChart)
        {
            if (crystalChart != null && fastChart != null)
            {
                LoadComponent(crystalChart, fastChart);
                fastChart.Series.Clear();
                MSChartSeries series = new MSChartSeries();
                fastChart.Series.Add(series);
                series.CreateUniqueName();
                fastChart.Chart.BorderlineColor = crystalChart.Border.BorderColor;
                fastChart.Chart.BorderlineDashStyle = UnitsConverter.ConvertLineStyleToChartDashStyle(crystalChart.Border.TopLineStyle);

                // the code below should be removed or commented if you don't have installed MSChart
                //==================================================================================
                fastChart.Chart.Series.Add(new Series());
                fastChart.Chart.Series[0].ChartType = SeriesChartType.Column;
                Legend legend = new Legend();
                fastChart.Chart.Legends.Add(legend);
                legend.Enabled = false;
                legend.BackColor = Color.Transparent;
                Title title = new Title();
                fastChart.Chart.Titles.Add(title);
                title.Visible = true;
                ChartArea chartArea = new ChartArea("Default");
                fastChart.Chart.ChartAreas.Add(chartArea);
                chartArea.BackColor = Color.Transparent;
                chartArea.AxisX = new Axis();
                chartArea.AxisX.IsMarginVisible = false;
                chartArea.AxisY = new Axis();
                chartArea.AxisY.IsMarginVisible = false;
                chartArea.Area3DStyle.LightStyle = LightStyle.None;
                //==================================================================================
            }
        }

        private void LoadMapObject(CrystalDecisions.CrystalReports.Engine.MapObject crystalMap, FastReport.Map.MapObject fastMap)
        {
            if (crystalMap != null && fastMap != null)
            {
                LoadComponent(crystalMap, fastMap);
                fastMap.Border.Color = crystalMap.Border.BorderColor;
                fastMap.FillColor = crystalMap.Border.BackgroundColor;
                fastMap.Border.Lines = UnitsConverter.ConvertBorderToBorderLines(crystalMap.Border);
                fastMap.Border.TopLine.Style = UnitsConverter.ConvertLineStyle(crystalMap.Border.TopLineStyle);
                fastMap.Border.LeftLine.Style = UnitsConverter.ConvertLineStyle(crystalMap.Border.LeftLineStyle);
                fastMap.Border.RightLine.Style = UnitsConverter.ConvertLineStyle(crystalMap.Border.RightLineStyle);
                fastMap.Border.BottomLine.Style = UnitsConverter.ConvertLineStyle(crystalMap.Border.BottomLineStyle);
                fastMap.Border.Shadow = crystalMap.Border.HasDropShadow;
            }
        }

        private void LoadSubreportObject(CrystalDecisions.CrystalReports.Engine.SubreportObject crystalSubreport, FastReport.SubreportObject fastSubreport)
        {
            if (crystalSubreport != null && fastSubreport != null)
            {
                LoadComponent(crystalSubreport, fastSubreport);
                fastSubreport.Border.Color = crystalSubreport.Border.BorderColor;
                fastSubreport.FillColor = crystalSubreport.Border.BackgroundColor;
                fastSubreport.Border.Lines = UnitsConverter.ConvertBorderToBorderLines(crystalSubreport.Border);
                fastSubreport.Border.TopLine.Style = UnitsConverter.ConvertLineStyle(crystalSubreport.Border.TopLineStyle);
                fastSubreport.Border.LeftLine.Style = UnitsConverter.ConvertLineStyle(crystalSubreport.Border.LeftLineStyle);
                fastSubreport.Border.RightLine.Style = UnitsConverter.ConvertLineStyle(crystalSubreport.Border.RightLineStyle);
                fastSubreport.Border.BottomLine.Style = UnitsConverter.ConvertLineStyle(crystalSubreport.Border.BottomLineStyle);
                fastSubreport.Border.Shadow = crystalSubreport.Border.HasDropShadow;
                ReportPage subPage = ComponentsFactory.CreateReportPage(Report);
                DataBand subBand = ComponentsFactory.CreateDataBand(subPage);
                subBand.Height = 2.0f * FastReport.Utils.Units.Centimeters;
                fastSubreport.ReportPage = subPage;
            }
        }

        private void LoadComponents(CrystalDecisions.CrystalReports.Engine.ReportObjects reportObjects, FastReport.BandBase band)
        {
            foreach (CrystalDecisions.CrystalReports.Engine.ReportObject obj in reportObjects)
            {
                if (obj is CrystalDecisions.CrystalReports.Engine.TextObject)
                {
                    FastReport.TextObject text = ComponentsFactory.CreateTextObject(obj.Name, band);
                    LoadTextObject((obj as CrystalDecisions.CrystalReports.Engine.TextObject), text);
                }
                else if (obj is CrystalDecisions.CrystalReports.Engine.FieldObject)
                {
                    FastReport.TextObject text = ComponentsFactory.CreateTextObject(obj.Name, band);
                    LoadTextObject((obj as CrystalDecisions.CrystalReports.Engine.FieldObject), text);
                }
                else if (obj is CrystalDecisions.CrystalReports.Engine.LineObject)
                {
                    FastReport.LineObject line = ComponentsFactory.CreateLineObject(obj.Name, band);
                    LoadLineObject((obj as CrystalDecisions.CrystalReports.Engine.LineObject), line);
                }
                else if (obj is CrystalDecisions.CrystalReports.Engine.BoxObject)
                {
                    FastReport.ShapeObject rectangle = ComponentsFactory.CreateShapeObject(obj.Name, band);
                    LoadShapeObject((obj as CrystalDecisions.CrystalReports.Engine.BoxObject), rectangle);
                }
                else if (obj is CrystalDecisions.CrystalReports.Engine.PictureObject)
                {
                    FastReport.PictureObject picture = ComponentsFactory.CreatePictureObject(obj.Name, band);
                    LoadPictureObject((obj as CrystalDecisions.CrystalReports.Engine.PictureObject), picture);
                }
                else if (obj is CrystalDecisions.CrystalReports.Engine.ChartObject)
                {
                    FastReport.MSChart.MSChartObject chart = ComponentsFactory.CreateMSChartObject(obj.Name, band);
                    LoadMSChartObject((obj as CrystalDecisions.CrystalReports.Engine.ChartObject), chart);
                }
                else if (obj is CrystalDecisions.CrystalReports.Engine.MapObject)
                {
                    FastReport.Map.MapObject map = ComponentsFactory.CreateMapObject(obj.Name, band);
                    LoadMapObject((obj as CrystalDecisions.CrystalReports.Engine.MapObject), map);
                }
                else if (obj is CrystalDecisions.CrystalReports.Engine.SubreportObject)
                {
                    FastReport.SubreportObject subreport = ComponentsFactory.CreateSubreportObject(obj.Name, band);
                    LoadSubreportObject((obj as CrystalDecisions.CrystalReports.Engine.SubreportObject), subreport);
                }
            }
        }

        private void LoadBand(CrystalDecisions.CrystalReports.Engine.Section section, FastReport.BandBase band)
        {
            if (section != null && band != null)
            {
                band.Height = UnitsConverter.TwipsToPixels(section.Height);
                band.FillColor = section.SectionFormat.BackgroundColor;
                band.StartNewPage = section.SectionFormat.EnableNewPageBefore;
                band.PrintOnBottom = section.SectionFormat.EnablePrintAtBottomOfPage;
                LoadComponents(section.ReportObjects, band);
            }
        }

        private void LoadChildBand(CrystalDecisions.CrystalReports.Engine.Section section, FastReport.BandBase band)
        {
            if (section != null && band != null)
            {
                if (band.Child == null)
                {
                    LoadBand(section, ComponentsFactory.CreateChildBand(band));
                }
                else
                {
                    LoadChildBand(section, band.Child);
                }
            }
        }

        private void LoadReportTitle(CrystalDecisions.CrystalReports.Engine.Section section)
        {
            if (section != null)
            {
                if (page.ReportTitle == null)
                {
                    LoadBand(section, ComponentsFactory.CreateReportTitleBand(page));
                }
                else
                {
                    LoadChildBand(section, page.ReportTitle);
                }
            }
        }

        private void LoadPageHeader(CrystalDecisions.CrystalReports.Engine.Section section)
        {
            if (section != null)
            {
                if (page.PageHeader == null)
                {
                    LoadBand(section, ComponentsFactory.CreatePageHeaderBand(page));
                }
                else
                {
                    LoadChildBand(section, page.PageHeader);
                }
            }
        }

        private void LoadReportSummary(CrystalDecisions.CrystalReports.Engine.Section section)
        {
            if (section != null)
            {
                if (page.ReportSummary == null)
                {
                    LoadBand(section, ComponentsFactory.CreateReportSummaryBand(page));
                }
                else
                {
                    LoadChildBand(section, page.ReportSummary);
                }
            }
        }

        private void LoadPageFooter(CrystalDecisions.CrystalReports.Engine.Section section)
        {
            if (section != null)
            {
                if (page.PageFooter == null)
                {
                    LoadBand(section, ComponentsFactory.CreatePageFooterBand(page));
                }
                else
                {
                    LoadChildBand(section, page.PageFooter);
                }
            }
        }

        private void LoadData(CrystalDecisions.CrystalReports.Engine.Section section)
        {
            LoadBand(section, ComponentsFactory.CreateDataBand(page));
        }

        private void LoadSections()
        {
            if (crystalReport.ReportDefinition == null || crystalReport.ReportDefinition.Sections == null)
                return;

            CrystalDecisions.CrystalReports.Engine.Sections sections = crystalReport.ReportDefinition.Sections;
            FastReport.GroupHeaderBand groupHeader = null;
            FastReport.DataBand detail = null;
            Stack<FastReport.GroupHeaderBand> stack = new Stack<FastReport.GroupHeaderBand>();
            for (int i = 0; i < sections.Count; i++)
            {
                CrystalDecisions.CrystalReports.Engine.Section section = sections[i];
                if (section.Kind == CrystalDecisions.Shared.AreaSectionKind.ReportHeader)
                {
                    LoadReportTitle(section);
                }
                else if (section.Kind == CrystalDecisions.Shared.AreaSectionKind.PageHeader)
                {
                    LoadPageHeader(section);
                }
                else if (section.Kind == CrystalDecisions.Shared.AreaSectionKind.GroupHeader)
                {
                    if (groupHeader != null)
                    {
                        stack.Push(groupHeader);
                    }
                    groupHeader = new FastReport.GroupHeaderBand();
                    if (stack.Count > 0)
                    {
                        stack.Peek().AddChild(groupHeader);
                    }
                    else
                    {
                        page.Bands.Add(groupHeader);
                    }
                    groupHeader.CreateUniqueName();
                    LoadBand(section, groupHeader);
                }
                else if (section.Kind == CrystalDecisions.Shared.AreaSectionKind.Detail && groupHeader != null)
                {
                    detail = new DataBand();
                    if (groupHeader.Data == null)
                    {
                        groupHeader.Data = detail;
                    }
                    else
                    {
                        groupHeader.Data.AddChild(detail);
                    }
                    detail.CreateUniqueName();
                    LoadBand(section, detail);
                }
                else if (section.Kind == CrystalDecisions.Shared.AreaSectionKind.GroupFooter && groupHeader != null)
                {
                    FastReport.GroupFooterBand groupFooter = new FastReport.GroupFooterBand();
                    groupHeader.GroupFooter = groupFooter;
                    groupFooter.Name = groupHeader.Name.Replace(groupHeader.BaseName, groupFooter.BaseName);
                    if (String.IsNullOrEmpty(groupFooter.Name))
                    {
                        groupFooter.CreateUniqueName();
                    }
                    LoadBand(section, groupFooter);
                    if (stack.Count > 0)
                    {
                        groupHeader = stack.Pop();
                    }
                }
                else if (section.Kind == CrystalDecisions.Shared.AreaSectionKind.PageFooter)
                {
                    LoadPageFooter(section);
                }
                else if (section.Kind == CrystalDecisions.Shared.AreaSectionKind.ReportFooter)
                {
                    LoadReportSummary(section);
                }
                else
                {
                    LoadData(section);
                }
            }
        }

        private void LoadReport()
        {
            page = ComponentsFactory.CreateReportPage(Report);
            LoadReportInfo();
            LoadPrintSettings();
            LoadPageSettings();
            LoadSections();
        }

        #endregion // Private Methods

        #region Protected Methods

        /// <inheritdoc/>
        protected override string GetFilter()
        {
            return new FastReport.Utils.MyRes("FileFilters").Get("RptFile");
        }

        #endregion // Protected Methods

        #region Public Methods

        /// <inheritdoc/>
        public override void LoadReport(FastReport.Report report, string filename)
        {
            if (report != null && crystalReport != null)
            {
                crystalReport.FileName = filename;
                bool crystalReportLoaded = true;
                try
                {
                    crystalReport.Load();
                }
                catch
                {
                    crystalReportLoaded = false;
                    MyRes res = new MyRes("Messages");
                    FRMessageBox.Error(res.Get("WrongCrystalReportsFile"));
                }
                if (crystalReportLoaded)
                {
                    Report = report;
                    Report.Clear();
                    LoadReport();
                }
            }
        }

        #endregion // Public Methods
    }
}
