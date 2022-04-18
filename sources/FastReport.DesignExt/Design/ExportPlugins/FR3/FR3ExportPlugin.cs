using System;
using System.IO;
using FastReport.Table;
using FastReport.Matrix;
using FastReport.Barcode;
#if MSCHART
using FastReport.MSChart;
#endif
using FastReport.Map;
using FastReport.Utils;

namespace FastReport.Design.ExportPlugins.FR3
{
    /// <summary>
    /// Represents the FR3 export plugin.
    /// </summary>
    public class FR3ExportPlugin : ExportPlugin
    {
        #region Fields

        private StreamWriter writer;
        //private ReportPage page;
        //private Base parent;
        //private ReportComponentBase component;

        #endregion // Fields

        #region Properties
        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FR3ExportPlugin"/> class.
        /// </summary>
        public FR3ExportPlugin() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FR3ExportPlugin"/> class with a specified designer.
        /// </summary>
        /// <param name="designer">The report designer.</param>
        public FR3ExportPlugin(Designer designer) : base(designer)
        {
        }

        #endregion // Constructors

        #region Private Methods

        private string ReplaceControlChars(string str)
        {
            str = str.Replace("&", "&amp;");
            str = str.Replace("'", "&apos;");
            str = str.Replace("\"", "&quot;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            str = str.Replace(Environment.NewLine, "&#13;&#10;");
            return str;
        }

        private void WriteEngineOptions()
        {
            writer.Write(" EngineOptions.DoublePass=\"" + Report.DoublePass.ToString() + "\"");
            writer.Write(" EngineOptions.UseFileCache=\"" + Report.UseFileCache.ToString() + "\"");
        }

        private void WriteReportOptions()
        {
            writer.Write(" ReportOptions.Author=\"" + Report.ReportInfo.Author + "\"");
            writer.Write(" ReportOptions.Description.Text=\"" + ReplaceControlChars(Report.ReportInfo.Description) + "\"");
        }

        private void WriteDataPage()
        {
            writer.WriteLine("  <TfrxDataPage Name=\"Data\" Height=\"1000\" Left=\"0\" Top=\"0\" Width=\"1000\"/>");
        }

        private void WriteBorder(Border border)
        {
            writer.Write(" Frame.Typ=\"" + UnitsConverter.ConvertBorderLines(border.Lines) + "\"");
            writer.Write(" Frame.LeftLine.Color=\"" + UnitsConverter.ColorToTColor(border.LeftLine.Color) + "\"");
            writer.Write(" Frame.TopLine.Color=\"" + UnitsConverter.ColorToTColor(border.TopLine.Color) + "\"");
            writer.Write(" Frame.RightLine.Color=\"" + UnitsConverter.ColorToTColor(border.RightLine.Color) + "\"");
            writer.Write(" Frame.BottomLine.Color=\"" + UnitsConverter.ColorToTColor(border.BottomLine.Color) + "\"");
            writer.Write(" Frame.LeftLine.Style=\"" + UnitsConverter.ConvertLineStyle(border.LeftLine.Style) + "\"");
            writer.Write(" Frame.TopLine.Style=\"" + UnitsConverter.ConvertLineStyle(border.TopLine.Style) + "\"");
            writer.Write(" Frame.RightLine.Style=\"" + UnitsConverter.ConvertLineStyle(border.RightLine.Style) + "\"");
            writer.Write(" Frame.BottomLine.Style=\"" + UnitsConverter.ConvertLineStyle(border.BottomLine.Style) + "\"");
        }

        private void WriteObject(ReportComponentBase obj, string type)
        {
            writer.Write("      <" + type + " Name=\"" + obj.Name + "\"");
            writer.Write(" Height=\"" + obj.Height.ToString() + "\"");
            writer.Write(" Left=\"" + obj.Left.ToString() + "\"");
            writer.Write(" Top=\"" + obj.Top.ToString() + "\"");
            writer.Write(" Width=\"" + obj.Width.ToString() + "\"");
            writer.Write(" Visible=\"" + obj.Visible.ToString() + "\"");
        }

        private void WriteTextObject(TextObject text)
        {
            WriteObject(text, "TfrxMemoView");
            writer.Write(" Color=\"" + UnitsConverter.ColorToTColor(text.FillColor) + "\"");
            writer.Write(" Font.Charset=\"1\"");
            writer.Write(" Font.Color=\"" + UnitsConverter.ColorToTColor(text.TextColor) + "\"");
            writer.Write(" Font.Height=\"" + UnitsConverter.ConvertFontSize(text.Font.Size) + "\"");
            writer.Write(" Font.Name=\"" + text.Font.Name + "\"");
            writer.Write(" Font.Style=\"" + UnitsConverter.ConvertFontStyle(text.Font.Style) + "\"");
            WriteBorder(text.Border);
            writer.Write(" HAlign=\"" + UnitsConverter.ConvertHorzAlign(text.HorzAlign) + "\"");
            writer.Write(" ParentFont=\"" + "False" + "\"");
            writer.Write(" VAlign=\"" + UnitsConverter.ConvertVertAlign(text.VertAlign) + "\"");
            writer.Write(" Text=\"" + ReplaceControlChars(text.Text) + "\"");
            writer.WriteLine("/>");
        }

        private void WritePictureObject(PictureObject pic)
        {
            WriteObject(pic, "TfrxPictureView");
            WriteBorder(pic.Border);
            writer.WriteLine("/>");
        }

        private void WriteLineObject(LineObject line)
        {
            WriteObject(line, "TfrxLineView");
            WriteBorder(line.Border);
            writer.Write(" Diagonal=\"" + line.Diagonal.ToString() + "\"");
            writer.WriteLine("/>");
        }

        private void WriteShapeObject(ShapeObject shape)
        {
            WriteObject(shape, "TfrxShapeView");
            writer.Write(" Color=\"" + UnitsConverter.ColorToTColor(shape.FillColor) + "\"");
            WriteBorder(shape.Border);
            writer.Write(" Shape=\"" + UnitsConverter.ConvertShapeKind(shape.Shape)+ "\"");
            writer.WriteLine("/>");
        }

        private void WriteTableObject(TableObject table)
        {
            writer.Write("      <TfrxTableObject Name=\"" + table.Name + "\"");
            writer.Write(" Height=\"" + table.Height.ToString() + "\"");
            writer.Write(" Left=\"" + table.Left.ToString() + "\"");
            writer.Write(" Top=\"" + table.Top.ToString() + "\"");
            writer.Write(" Width=\"" + table.Width.ToString() + "\"");
            writer.Write(" Visible=\"" + table.Visible.ToString() + "\"");
            writer.WriteLine(">");
            foreach (TableColumn column in table.Columns)
            {
                writer.Write("        <TfrxTableColumn Name=\"" + column.Name + "\"");
                writer.Write(" Width=\"" + column.Width.ToString() + "\"");
                writer.Write(" MinWidth=\"" + column.MinWidth.ToString() + "\"");
                writer.Write(" MaxWidth=\"" + column.MaxWidth.ToString() + "\"");
                writer.Write(" AutoSize=\"" + column.AutoSize.ToString() + "\"");
                writer.WriteLine("/>");
            }
            foreach (TableRow row in table.Rows)
            {
                writer.Write("        <TfrxTableRow Name=\"" + row.Name + "\"");
                writer.Write(" Height=\"" + row.Height.ToString() + "\"");
                writer.Write(" MinHeight=\"" + row.MinHeight.ToString() + "\"");
                writer.Write(" MaxHeight=\"" + row.MaxHeight.ToString() + "\"");
                writer.Write(" AutoSize=\"" + row.AutoSize.ToString() + "\"");
                writer.WriteLine(">");
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    writer.Write("          <TfrxTableCell Name=\"" + row[i].Name + "\"");
                    writer.Write(" Color=\"" + UnitsConverter.ColorToTColor(row[i].FillColor) + "\"");
                    writer.Write(" Font.Charset=\"1\"");
                    writer.Write(" Font.Color=\"" + UnitsConverter.ColorToTColor(row[i].TextColor) + "\"");
                    writer.Write(" Font.Height=\"" + UnitsConverter.ConvertFontSize(row[i].Font.Size) + "\"");
                    writer.Write(" Font.Name=\"" + row[i].Font.Name + "\"");
                    writer.Write(" Font.Style=\"" + UnitsConverter.ConvertFontStyle(row[i].Font.Style) + "\"");
                    writer.Write(" HAlign=\"" + UnitsConverter.ConvertHorzAlign(row[i].HorzAlign) + "\"");
                    writer.Write(" ParentFont=\"" + "False" + "\"");
                    writer.Write(" VAlign=\"" + UnitsConverter.ConvertVertAlign(row[i].VertAlign) + "\"");
                    writer.Write(" Text=\"" + ReplaceControlChars(row[i].Text) + "\"");
                    writer.WriteLine("/>");
                }
                writer.WriteLine("        </TfrxTableRow>");
            }
            writer.WriteLine("      </TfrxTableObject>");
        }

        //private void WriteMatrixObject(MatrixObject matrix)
        //{
        //}

        private void WriteBarcodeObject(BarcodeObject barcode)
        {
            WriteObject(barcode, "TfrxBarCodeView");
            writer.Write(" BarType=\"" + UnitsConverter.ConvertBarcodeType(barcode.Barcode) + "\"");
            WriteBorder(barcode.Border);
            writer.WriteLine("/>");
        }

        private void Write2DBarcodeObject(BarcodeObject barcode)
        {
            WriteObject(barcode, "TfrxBarcode2DView");
            writer.Write(" BarType=\"" + UnitsConverter.ConvertBarcodeType(barcode.Barcode) + "\"");
            WriteBorder(barcode.Border);
            writer.WriteLine("/>");
        }

        //private void WriteChartMSChartObject(MSChartObject chart)
        //{
        //}

        private void WriteRichObject(RichObject rich)
        {
            WriteObject(rich, "TfrxRichView");
            WriteBorder(rich.Border);
            writer.WriteLine("/>");
        }

        private void WriteCheckBoxObject(CheckBoxObject box)
        {
            WriteObject(box, "TfrxCheckBoxView");
            WriteBorder(box.Border);
            writer.Write(" Checked=\"" + box.Checked + "\"");
            writer.Write(" CheckStyle=\"" + UnitsConverter.ConvertCheckedSymbol(box.CheckedSymbol) + "\"");
            writer.Write(" CheckColor=\"" + UnitsConverter.ColorToTColor(box.CheckColor) + "\"");
            writer.WriteLine("/>");
        }

        private void WriteCellularTextObject(CellularTextObject text)
        {
            WriteObject(text, "TfrxCellularText");
            WriteBorder(text.Border);
            writer.Write(" Color=\"" + UnitsConverter.ColorToTColor(text.FillColor) + "\"");
            writer.Write(" Font.Charset=\"1\"");
            writer.Write(" Font.Color=\"" + UnitsConverter.ColorToTColor(text.TextColor) + "\"");
            writer.Write(" Font.Height=\"" + UnitsConverter.ConvertFontSize(text.Font.Size) + "\"");
            writer.Write(" Font.Name=\"" + text.Font.Name + "\"");
            writer.Write(" Font.Style=\"" + UnitsConverter.ConvertFontStyle(text.Font.Style) + "\"");
            writer.Write(" HAlign=\"" + UnitsConverter.ConvertHorzAlign(text.HorzAlign) + "\"");
            writer.Write(" ParentFont=\"" + "False" + "\"");
            writer.Write(" VAlign=\"" + UnitsConverter.ConvertVertAlign(text.VertAlign) + "\"");
            writer.Write(" WordWrap=\"" + text.WordWrap.ToString() + "\"");
            writer.Write(" Text=\"" + ReplaceControlChars(text.Text) + "\"");
            writer.WriteLine("/>");
        }

        private void WriteZipCodeObject(ZipCodeObject zip)
        {
            WriteObject(zip, "TfrxZipCodeView");
            writer.Write(" DigitCount=\"" + zip.SegmentCount.ToString() + "\"");
            writer.Write(" ShowMarkers=\"" + zip.ShowMarkers.ToString() + "\"");
            writer.Write(" ShowGrid=\"" + zip.ShowGrid.ToString() + "\"");
            writer.Write(" Fill.BackColor=\"" + UnitsConverter.ColorToTColor(zip.FillColor) + "\"");
            writer.Write(" mmDigitHeight=\"" + (zip.SegmentHeight / Units.Millimeters).ToString() + "\"");
            writer.Write(" mmDigitWidth=\"" + (zip.SegmentWidth / Units.Millimeters).ToString()  + "\"");
            writer.Write(" mmSpacing=\"" + (zip.Spacing / Units.Millimeters).ToString() + "\"");
            writer.Write(" Text=\"" + zip.Text + "\"");
            writer.WriteLine("/>");
        }

        private void WriteMapObject(MapObject map)
        {
            writer.Write("      <TfrxMapView Name=\"" + map.Name + "\"");
            writer.Write(" Height=\"" + map.Height.ToString() + "\"");
            writer.Write(" Left=\"" + map.Left.ToString() + "\"");
            writer.Write(" Top=\"" + map.Top.ToString() + "\"");
            writer.Write(" Width=\"" + map.Width.ToString() + "\"");
            writer.Write(" Visible=\"" + map.Visible.ToString() + "\"");
            WriteBorder(map.Border);
            writer.Write(" Fill.BackColor=\"" + UnitsConverter.ColorToTColor(map.FillColor) + "\"");
            writer.Write(" Zoom=\"" + map.Zoom.ToString() + "\"");
            writer.Write(" MinZoom=\"" + map.MinZoom.ToString() + "\"");
            writer.Write(" MaxZoom=\"" + map.MaxZoom.ToString() + "\"");
            writer.Write(" OffsetX=\"" + map.OffsetX.ToString() + "\"");
            writer.Write(" OffsetY=\"" + map.OffsetY.ToString() + "\"");
            writer.Write(" MercatorProjection=\"" + map.MercatorProjection.ToString() + "\"");

            writer.Write(" ColorScale.Visible=\"" + map.ColorScale.Visible.ToString() + "\"");
            writer.Write(" ColorScale.BorderColor=\"" + UnitsConverter.ColorToTColor(map.ColorScale.BorderColor) + "\"");
            writer.Write(" ColorScale.BorderWidth=\"" + map.ColorScale.Border.Width.ToString() + "\"");
            writer.Write(" ColorScale.Dock=\"" + UnitsConverter.ConvertScaleDock(map.ColorScale.Dock) + "\"");
            writer.Write(" ColorScale.Font.Charset=\"1\"");
            writer.Write(" ColorScale.Font.Color=\"" + UnitsConverter.ColorToTColor(map.ColorScale.TextColor) + "\"");
            writer.Write(" ColorScale.Font.Height=\"" + UnitsConverter.ConvertFontSize(map.ColorScale.Font.Size) + "\"");
            writer.Write(" ColorScale.Font.Name=\"" + map.ColorScale.Font.Name + "\"");
            writer.Write(" ColorScale.Font.Style=\"" + UnitsConverter.ConvertFontStyle(map.ColorScale.Font.Style) + "\"");
            writer.Write(" ColorScale.TitleFont.Charset=\"1\"");
            writer.Write(" ColorScale.TitleFont.Color=\"" + UnitsConverter.ColorToTColor(map.ColorScale.TitleColor) + "\"");
            writer.Write(" ColorScale.TitleFont.Height=\"" + UnitsConverter.ConvertFontSize(map.ColorScale.TitleFont.Size) + "\"");
            writer.Write(" ColorScale.TitleFont.Name=\"" + map.ColorScale.TitleFont.Name + "\"");
            writer.Write(" ColorScale.TitleFont.Style=\"" + UnitsConverter.ConvertFontStyle(map.ColorScale.TitleFont.Style) + "\"");
            writer.WriteLine(">");

            foreach (MapLayer layer in map.Layers)
            {
                writer.Write("        <TfrxMapFileLayer Name=\"" + layer.Name + "\"");
                writer.Write(" ColorRanges.RangeCount=\"" + layer.ColorRanges.RangeCount.ToString() + "\"");
                writer.Write(" ColorRanges.StartColor=\"" + UnitsConverter.ColorToTColor(layer.ColorRanges.StartColor) + "\"");
                writer.Write(" ColorRanges.MiddleColor=\"" + UnitsConverter.ColorToTColor(layer.ColorRanges.MiddleColor) + "\"");
                writer.Write(" ColorRanges.EndColor=\"" + UnitsConverter.ColorToTColor(layer.ColorRanges.EndColor) + "\"");

                writer.Write(" DefaultShapeStyle.BorderColor=\"" + UnitsConverter.ColorToTColor(layer.DefaultShapeStyle.BorderColor) + "\"");
                writer.Write(" DefaultShapeStyle.BorderStyle=\"" + UnitsConverter.ConvertDashStyle(layer.DefaultShapeStyle.BorderStyle) + "\"");
                writer.Write(" DefaultShapeStyle.BorderWidth=\"" + layer.DefaultShapeStyle.BorderWidth.ToString() + "\"");
                writer.Write(" DefaultShapeStyle.FillColor=\"" + UnitsConverter.ColorToTColor(layer.DefaultShapeStyle.FillColor) + "\"");
                writer.Write(" DefaultShapeStyle.PointSize=\"" + layer.DefaultShapeStyle.PointSize.ToString() + "\"");

                writer.Write(" Operation=\"" + UnitsConverter.ConvertTotalType(layer.Function) + "\"");
                writer.Write(" LabelKind=\"" + UnitsConverter.ConvertMapLabelKind(layer.LabelKind) + "\"");
                writer.Write(" MapPalette=\"" + UnitsConverter.ConvertMapPalette(layer.Palette) + "\"");
                
                writer.Write(" SizeRanges.RangeCount=\"" + layer.SizeRanges.RangeCount.ToString() + "\"");
                writer.Write(" SizeRanges.Visible=\"True\"");
                writer.Write(" SizeRanges.StartSize=\"" + layer.SizeRanges.StartSize.ToString() + "\"");
                writer.Write(" SizeRanges.EndSize=\"" + layer.SizeRanges.EndSize.ToString() + "\"");
                
                writer.Write(" LabelColumn=\"" + layer.SpatialColumn + "\"");
                writer.Write(" LayerTags.Text=\"\"");
                writer.Write(" MapAccuracy=\"" + layer.Accuracy.ToString() + "\"");
                writer.Write(" MapFileName=\"" + layer.Shapefile + "\"");
                writer.Write(" PixelAccuracy=\"0\"");

                writer.WriteLine("/>");
            }

            writer.WriteLine("      </TfrxMapView>");
        }

        private void WriteObjects(BandBase band)
        {
            foreach (ReportComponentBase c in band.Objects)
            {
                if (c is CellularTextObject)
                {
                    WriteCellularTextObject(c as CellularTextObject);
                }
                else if (c is TextObject)
                {
                    WriteTextObject(c as TextObject);
                }
                else if (c is PictureObject)
                {
                    WritePictureObject(c as PictureObject);
                }
                else if (c is LineObject)
                {
                    WriteLineObject(c as LineObject);
                }
                else if (c is ShapeObject)
                {
                    WriteShapeObject(c as ShapeObject);
                }
                //else if (c is SubreportObject)
                //{
                //}
                else if (c is TableObject)
                {
                    WriteTableObject(c as TableObject);
                }
                //else if (c is MatrixObject)
                //{
                //}
                else if (c is BarcodeObject)
                {
                    if ((c as BarcodeObject).Barcode is Barcode2DBase)
                    {
                        Write2DBarcodeObject(c as BarcodeObject);
                    }
                    else
                    {
                        WriteBarcodeObject(c as BarcodeObject);
                    }
                }
                else if (c is RichObject)
                {
                    WriteRichObject(c as RichObject);
                }
                else if (c is CheckBoxObject)
                {
                    WriteCheckBoxObject(c as CheckBoxObject);
                }
#if MSCHART
                else if (c is MSChartObject)
                {
                }
#endif
                else if (c is ZipCodeObject)
                {
                    WriteZipCodeObject(c as ZipCodeObject);
                }
                else if (c is MapObject)
                {
                    WriteMapObject(c as MapObject);
                }
            }
        }

        private void WriteChild(ChildBand child)
        {
            if (child != null)
            {
                WriteBand(child, "TfrxChild");
            }
        }

        private void WriteBand(BandBase band, string type)
        {
            writer.Write("    <" + type + " Name=\"" + band.Name + "\"");
            writer.Write(" Height=\"" + band.Height.ToString() + "\"");
            writer.Write(" Left=\"" + band.Left.ToString() + "\"");
            writer.Write(" Top=\"" + band.Top.ToString() + "\"");
            writer.Write(" Width=\"" + band.Width.ToString() + "\"");
            if (band.Child != null)
            {
                writer.Write(" Child=\"" + band.Child.Name + "\"");
            }
            writer.WriteLine(">");
            WriteObjects(band);
            writer.WriteLine("    </" + type + ">");
            WriteChild(band.Child);
        }

        private void WriteReportTitle(ReportTitleBand title)
        {
            if (title != null)
            {
                WriteBand(title, "TfrxReportTitle");
            }
        }

        private void WritePageHeader(PageHeaderBand header)
        {
            if (header != null)
            {
                WriteBand(header, "TfrxPageHeader");
            }
        }

        private void WriteDataBands(BandCollection bands)
        {
            foreach (BandBase band in bands)
                WriteBand(band);
        }

        private void WriteBand(BandBase band)
        {
                if (band is DataBand)
                    WriteBand(band, "TfrxMasterData");
                else if (band is GroupHeaderBand)
                    WriteBand(band, "TfrxGroupHeader");
                else if (band is GroupFooterBand)
                    WriteBand(band, "TfrxGroupFooter");
            if (band.ChildObjects.Count > 0)
            {
                foreach (object b in band.ChildObjects)
                    if(b is BandBase)
                        WriteBand(b as BandBase);
            }
        }

        private void WritePageFooter(PageFooterBand footer)
        {
            if (footer != null)
            {
                WriteBand(footer, "TfrxPageFooter");
            }
        }

        private void WriteReportSummary(ReportSummaryBand summary)
        {
            if (summary != null)
            {
                WriteBand(summary, "TfrxReportSummary");
            }
        }

        private void WriteBands(ReportPage page)
        {
            WriteReportTitle(page.ReportTitle);
            WritePageHeader(page.PageHeader);
            WriteDataBands(page.Bands);
            WritePageFooter(page.PageFooter);
            WriteReportSummary(page.ReportSummary);
        }

        private void WriteReportPage(ReportPage page)
        {
            writer.Write("  <TfrxReportPage Name=\"" + page.Name + "\"");
            writer.Write(" PaperWidth=\"" + page.PaperWidth.ToString() + "\"");
            writer.Write(" PaperHeight=\"" + page.PaperHeight.ToString() + "\"");
            writer.Write(" PaperSize=\"" + page.RawPaperSize.ToString() + "\"");
            writer.Write(" LeftMargin=\"" + page.LeftMargin.ToString() + "\"");
            writer.Write(" RightMargin=\"" + page.RightMargin.ToString() + "\"");
            writer.Write(" TopMargin=\"" + page.TopMargin.ToString() + "\"");
            writer.Write(" BottomMargin=\"" + page.BottomMargin.ToString() + "\"");
            writer.Write(" ColumnWidth=\"" + page.Columns.Width.ToString() + "\"");
            writer.Write(" ColumnPositions.Text=\"" + "" + "\"");
            writer.Write(" HGuides.Text=\"" + "" + "\"");
            writer.Write(" VGuides.Text=\"" + "" + "\"");
            writer.WriteLine(">");
            WriteBands(page);
            writer.WriteLine("  </TfrxReportPage>");
        }

        private void WriteReportPages()
        {
            foreach (ReportPage p in Report.Pages)
            {
                WriteReportPage(p);
            }
        }

        private void WriteReport()
        {
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\"?>");
            writer.Write("<TfrxReport Version=\"4.8.222\" DotMatrixReport=\"False\"");
            WriteEngineOptions();
            WriteReportOptions();
            writer.WriteLine(">");
            WriteDataPage();
            WriteReportPages();
            writer.WriteLine("</TfrxReport>");
        }

        #endregion // Private Methods

        #region Protected Methods

        /// <inheritdoc/>
        protected override string GetFilter()
        {
            return new FastReport.Utils.MyRes("FileFilters").Get("Fr3File");
        }

        #endregion // Protected Methods

        #region Public Methods

        /// <inheritdoc/>
        public override void SaveReport(Report report, string filename)
        {
            Report = report;
            using (writer = new StreamWriter(new FileStream(filename, FileMode.Create)))
            {
                WriteReport();
            }
        }

        #endregion // Public Methods
    }
}
