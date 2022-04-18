using FastReport.Barcode;
using FastReport.Table;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;

namespace FastReport.Export.Dxf
{
    /// <summary>
    /// Variants of filling
    /// </summary>
    public enum DxfFillMode
    {
        /// <summary>
        /// Solid filling of hatch and solid objects
        /// </summary>
        Solid,
        /// <summary>
        /// Draw only borders of hatch and solid objects
        /// </summary>
        Border
    }

    public partial class DxfExport : ExportBase
    {
        #region Private Fields

        private float barcodesGap;
        private int currentPage;
        private DxfDocument dxfDocument;
        private IGraphics dxfMeasureGraphics;
        private string extension;
        private string fileNameWOext;
        private DxfFillMode fillMode;

        private string pageFileName;
        private float pageHeight;
        private string path;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets lines/polygons gap for barcodes object, in millimeters
        /// </summary>
        public float BarcodesGap
        {
            get { return barcodesGap / Units.Millimeters; }
            set { barcodesGap = value * Units.Millimeters; }
        }

        /// <summary>
        /// Gets or sets the dxf objects fill mode
        /// </summary>
        public DxfFillMode FillMode
        {
            get { return fillMode; }
            set { fillMode = value; }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DxfExport"/> class.
        /// </summary>
        public DxfExport()
        {
            dxfDocument = new DxfDocument();
            HasMultipleFiles = true;
            dxfMeasureGraphics = new GdiGraphics(new Bitmap(1, 1));
            fillMode = DxfFillMode.Border;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Export all report objects
        /// </summary>
        /// <param name="c"></param>
        public virtual void ExportObj(Base c)
        {
            if (c is ReportComponentBase && (c as ReportComponentBase).Exportable)
            {
                ReportComponentBase obj = c as ReportComponentBase;
                if (obj is CellularTextObject)
                    obj = (obj as CellularTextObject).GetTable();
                if (obj is TableCell) return;
                else if (obj is TableBase)
                {
                    TableBase table = obj as TableBase;
                    if (table.ColumnCount > 0 && table.RowCount > 0)
                    {
                        using (TextObject tableback = new TextObject())
                        {
                            tableback.Border = table.Border;
                            tableback.Fill = table.Fill;
                            tableback.FillColor = table.FillColor;
                            tableback.Left = table.AbsLeft;
                            tableback.Top = table.AbsTop;
                            float tableWidth = 0;
                            float tableHeight = 0;
                            for (int i = 0; i < table.ColumnCount; i++)
                                tableWidth += table[i, 0].Width;
                            for (int i = 0; i < table.RowCount; i++)
                                tableHeight += table.Rows[i].Height;
                            tableback.Width = (tableWidth < table.Width) ? tableWidth : table.Width;
                            tableback.Height = tableHeight;
                            AddTextObject(tableback, false);
                            // draw table border
                            AddBorder(tableback.Border, tableback.AbsLeft, tableback.AbsTop, tableback.Width, tableback.Height);
                        }
                        // draw cells
                        AddTable(table);
                    }
                }
                else if (obj is TextObject)
                    AddTextObject(obj as TextObject, true);
                else if (obj is BandBase)
                    AddBandObject(obj as BandBase);
                else if (obj is LineObject)
                    AddLine(obj as LineObject);
                else if (obj is PolygonObject)
                    AddPolygon(obj as PolygonObject);
                else if (obj is PolyLineObject)
                    AddPolyLine(obj as PolyLineObject);
                else if (obj is ShapeObject)
                    AddShape(obj as ShapeObject);
                //#if DOTNET_4
                //                else if (obj is SVGObject)
                //                    AddBarcodeFromSvg(obj as BarcodeObject);
                //#endif
                else if (obj is BarcodeObject)
                    AddBarcode(obj as BarcodeObject);

                //#if DOTNET_4
                //                else if (obj is SVGObject)
                //                    AddSVGObject(c as SVGObject);
                //#endif
                //                else
                //                    AddPictureObject(obj);
            }
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            base.Serialize(writer);
            writer.WriteBool("HasMultipleFiles", HasMultipleFiles);
            writer.WriteFloat("BarcodesGap", BarcodesGap);
            writer.WriteValue("FillMode", FillMode);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Export of Band
        /// </summary>
        /// <param name="band"></param>
        protected override void ExportBand(Base band)
        {
            base.ExportBand(band);
            ExportObj(band);
            foreach (Base c in band.ForEachAllConvectedObjects(this))
            {
                ExportObj(c);
            }
        }

        /// <summary>
        /// Begin exporting of page
        /// </summary>
        /// <param name="page"></param>
        protected override void ExportPageBegin(ReportPage page)
        {
            base.ExportPageBegin(page);
            pageHeight = ExportUtils.GetPageHeight(page);

            dxfDocument = new DxfDocument();

            // page borders
            if (page.Border.Lines != BorderLines.None)
            {
                using (TextObject pageBorder = new TextObject())
                {
                    pageBorder.Border = page.Border;
                    pageBorder.Left = 0;
                    pageBorder.Top = 0;
                    pageBorder.Width = (ExportUtils.GetPageWidth(page) - page.LeftMargin - page.RightMargin);
                    pageBorder.Height = (pageHeight - page.TopMargin - page.BottomMargin);
                    // AddTextObject(pageBorder, true);
                }
            }

            if (path != null && path != "")
                pageFileName = Path.Combine(path, fileNameWOext + currentPage.ToString() + extension);
            else
                pageFileName = null;
        }

        /// <summary>
        /// End exporting
        /// </summary>
        /// <param name="page"></param>
        protected override void ExportPageEnd(ReportPage page)
        {
            base.ExportPageEnd(page);

            dxfDocument.Finish();

            if (HasMultipleFiles)
            {
                //export to MultipleFiles
                if (Directory.Exists(path) && !string.IsNullOrEmpty(FileName))
                {
                    // desktop mode
                    if (currentPage == 0)
                    {
                        // save first page in parent Stream
                        Save(Stream);
                        Stream.Position = 0;
                        GeneratedStreams.Add(Stream);
                        GeneratedFiles.Add(FileName);
                    }
                    else
                    {
                        // save all page after first in files
                        Save(pageFileName);
                        GeneratedFiles.Add(pageFileName);
                    }
                }
                else if (string.IsNullOrEmpty(path))
                {
                    if (currentPage == 0)
                    {
                        // save first page in parent Stream
                        Save(Stream);
                        Stream.Position = 0;
                        GeneratedStreams.Add(Stream);
                        GeneratedFiles.Add(FileName);
                    }
                    else
                    {
                        // server mode, save in internal stream collection
                        MemoryStream pageStream = new MemoryStream();
                        Save(pageStream);
                        pageStream.Position = 0;
                        GeneratedStreams.Add(pageStream);
                        GeneratedFiles.Add(pageFileName);
                    }
                }
            }
            // increment page number
            currentPage++;
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            if (!HasMultipleFiles)
            {
                Save(Stream);
                Stream.Position = 0;
                GeneratedFiles.Add(FileName);
            }

            GeneratedFiles.Clear();
            GeneratedStreams.Clear();
        }

        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            return new MyRes("FileFilters").Get("DxfFile");
        }

        /// <inheritdoc/>
        protected override void Start()
        {
            currentPage = 0;
            GeneratedStreams = new List<Stream>();
            if (FileName != "" && FileName != null)
            {
                path = Path.GetDirectoryName(FileName);
                fileNameWOext = Path.GetFileNameWithoutExtension(FileName);
                extension = Path.GetExtension(FileName);
            }
            else
                fileNameWOext = "dxfreport";
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Add BandObject.
        /// </summary>
        private void AddBandObject(BandBase band)
        {
            using (TextObject newObj = new TextObject())
            {
                newObj.Left = band.AbsLeft;
                newObj.Top = band.AbsTop;
                newObj.Width = band.Width;
                newObj.Height = band.Height;
                newObj.Fill = band.Fill;
                newObj.Border = band.Border;
                AddTextObject(newObj, true);
            }
        }

        private void AddBarcode(BarcodeObject barcodeObject)
        {
            using (DXFGraphicsRenderer renderer = new DXFGraphicsRenderer(dxfDocument, fillMode, BarcodesGap))
            {
                bool error = false;
                string errorText = "";

                if (string.IsNullOrEmpty(barcodeObject.Text))
                {
                    error = true;
                    errorText = barcodeObject.NoDataText;
                }
                else
                    try
                    {
                        // That line makes barcode not an invalid
                        barcodeObject.UpdateAutoSize();
                    }
                    catch (Exception ex)
                    {
                        error = true;
                        errorText = ex.Message;
                    }

                if (!error)
                    barcodeObject.Barcode.DrawBarcode(renderer, new RectangleF(barcodeObject.AbsLeft / Units.Millimeters,
                     pageHeight - barcodeObject.AbsTop / Units.Millimeters, barcodeObject.Width / Units.Millimeters, barcodeObject.Height / Units.Millimeters));
                else
                {
                    renderer.DrawString(errorText, DrawUtils.DefaultReportFont, Brushes.Red,
                      new RectangleF(0, 0, barcodeObject.Width, barcodeObject.Height));
                }

                //barcodeObject.Barcode.DrawBarcode(renderer, new RectangleF(barcodeObject.AbsLeft,
                //    barcodeObject.AbsTop, barcodeObject.Width, barcodeObject.Height));
            }
        }

        private void AddTable(TableBase table)
        {
            float y = 0;
            for (int i = 0; i < table.RowCount; i++)
            {
                float x = 0;
                for (int j = 0; j < table.ColumnCount; j++)
                {
                    if (!table.IsInsideSpan(table[j, i]))
                    {
                        TableCell textcell = table[j, i];
                        textcell.Left = x;
                        textcell.Top = y;

                        Border oldBorder = textcell.Border.Clone();
                        textcell.Border.Lines = BorderLines.None;
                        if (textcell is TextObject)
                            AddTextObject(textcell as TextObject, /*false*/ true);
                        //else
                        //    AddPictureObject(textcell as ReportComponentBase);
                        textcell.Border = oldBorder;

                        AddBorder(textcell.Border, textcell.AbsLeft, textcell.AbsTop, textcell.Width, textcell.Height);
                    }
                    x += (table.Columns[j]).Width;
                }
                y += (table.Rows[i]).Height;
            }
        }

        /// <summary>
        /// Add TextObject.
        /// </summary>
        private void AddTextObject(TextObject obj, bool drawBorder)
        {
            float AbsLeft = obj.AbsLeft;
            float AbsTop = pageHeight * Units.Millimeters - obj.AbsTop;
            float Width = obj.Width;
            float Height = obj.Height;

            bool transformNeeded = obj.Angle != 0 || obj.FontWidthRatio != 1;
            if (transformNeeded)
                return;
            // draw background
            if (obj.FillColor != Color.Transparent)
                AddRectangleFill(AbsLeft, AbsTop, Width, Height, obj.FillColor);

            // WIP: draw text here
            //if (!string.IsNullOrWhiteSpace(obj.Text))
            //{
            //    GraphicsPath myPath = new GraphicsPath();

            //    // Set up all the string parameters.
            //    string stringText = obj.Text;
            //    FontFamily family = obj.Font.FontFamily;
            //    int fontStyle = (int)obj.Font.Style;
            //    float emSize = obj.Font.Size;
            //    PointF origin = new PointF(0, 0);
            //    StringFormat format = StringFormat.GenericDefault;

            //    // Add the string to the path.
            //    myPath.AddString(stringText,
            //        family,
            //        fontStyle,
            //        emSize,
            //        origin,
            //        format);

            //        AddGraphicsPath(myPath, obj.TextColor, 1, LineStyle.Solid, AbsLeft + Width / 2, AbsTop - Height / 2, true, true);
            //    }
            // if (obj.Clip)

            if (obj.Underlines)
                AddUnderlines(obj);

            if (!String.IsNullOrEmpty(obj.Text))
            {
                using (Font f = new Font(obj.Font.Name, obj.Font.Size * DrawUtils.ScreenDpiFX, obj.Font.Style))
                {
                    RectangleF textRect = new RectangleF(
                      obj.AbsLeft + obj.Padding.Left,
                      obj.AbsTop + obj.Padding.Top,
                      obj.Width - obj.Padding.Horizontal,
                      obj.Height - obj.Padding.Vertical);

                    // break the text to paragraphs, lines, words and runs
                    StringFormat format = obj.GetStringFormat(Report.GraphicCache /*cache*/, 0);
                    Brush textBrush = Report.GraphicCache.GetBrush(obj.TextColor);
                    AdvancedTextRenderer renderer = new AdvancedTextRenderer(obj.Text, dxfMeasureGraphics, f, textBrush, null,
                        textRect, format, obj.HorzAlign, obj.VertAlign, obj.LineHeight, obj.Angle, obj.FontWidthRatio,
                        obj.ForceJustify, obj.Wysiwyg, obj.HasHtmlTags, false, Zoom * DrawUtils.ScreenDpiFX, Zoom * DrawUtils.ScreenDpiFX, obj.InlineImageCache);
                    float w = f.Height * 0.1f; // to match .net char X offset
                                               // invert offset in case of rtl
                    if (obj.RightToLeft)
                        w = -w;
                    // we don't need this offset if text is centered
                    if (obj.HorzAlign == HorzAlign.Center)
                        w = 0;

                    //XmlElement textContainer = DrawTextContainer(obj.AbsLeft, obj.AbsTop, f, obj.TextColor);

                    // transform, rotate and scale coordinates if needed
                    if (transformNeeded)
                    {
                        textRect.X = -textRect.Width / 2;
                        textRect.Y = -textRect.Height / 2;
                        float angle = (float)(obj.Angle * Math.PI / 180);
                        float x = (obj.AbsLeft + obj.Width / 2);
                        float y = (obj.AbsTop + obj.Height / 2);
                        //AppndAngle(textContainer, angle, x, y);
                    }

                    // render
                    foreach (AdvancedTextRenderer.Paragraph paragraph in renderer.Paragraphs)
                        foreach (AdvancedTextRenderer.Line line in paragraph.Lines)
                        {
                            float lineOffset = 0;
                            float lineHeight = line.CalcHeight();
                            if (lineHeight > obj.Height)
                            {
                                if (obj.VertAlign == VertAlign.Center)
                                    lineOffset = -lineHeight / 2;//-
                                else if (obj.VertAlign == VertAlign.Bottom)
                                    lineOffset = -lineHeight;//-
                            }
                            /* foreach (RectangleF rect in line.Underlines)
                             {
                                 // DrawPDFUnderline(ObjectFontNumber, f, rect.Left, rect.Top, rect.Width, w,
                                 //    obj.TextColor, transformNeeded, sb);
                             }*/
                            /*foreach (RectangleF rect in line.Strikeouts)
                            {
                                 DrawStrikeout(f, rect.Left, rect.Top, rect.Width, w, obj.TextColor, transformNeeded);
                            }*/

                            foreach (AdvancedTextRenderer.Word word in line.Words)
                            {
                                //if (renderer.HtmlTags)
                                //    foreach (AdvancedTextRenderer.Run run in word.Runs)
                                //        using (Font fnt = run.GetFont())
                                //        {
                                //            AppndTspan(textContainer, run.Left, run.Top + lineOffset + lineHeight * 72 / 96/*magic*/,
                                //                w, run.Text, f);
                                //        }
                                //else
                                //AppndTspan(textContainer, word.Left, word.Top + lineOffset + lineHeight * 72 / 96/*magic*/,
                                //    w, word.Text, f);

                                // --
                                if (!string.IsNullOrWhiteSpace(word.Text))
                                {
                                    GraphicsPath myPath = new GraphicsPath();

                                    // Set up all the string parameters.
                                    string stringText = word.Text;
                                    FontFamily family = obj.Font.FontFamily;
                                    int fontStyle = (int)obj.Font.Style;
                                    float emSize = obj.Font.Size;
                                    PointF origin = new PointF(0, 0 /*+ lineOffset + lineHeight*//*magic*/);
                                    //  StringFormat format = StringFormat.GenericDefault;

                                    // Add the string to the path.
                                    myPath.AddString(stringText,
                                        family,
                                        fontStyle,
                                        emSize,
                                        origin,
                                        StringFormat.GenericDefault);

                                    float x = word.Left + w + (transformNeeded ? obj.AbsLeft : 0);
                                    float y = pageHeight * Units.Millimeters - word.Top - lineOffset - (transformNeeded ? obj.AbsTop : 0);

                                    float centerX = (obj.Width / 2) / Units.Millimeters;
                                    float centerY = (obj.Height / 2) / Units.Millimeters;
                                    PointF center = new PointF(centerX, centerY);

                                    float angle = (float)(obj.Angle * Math.PI / 180);

                                    AddGraphicsPath(myPath, obj.TextColor, 1 / 100.0f, LineStyle.Solid, x,
                                        y, true, true, angle, center);
                                }
                                // --
                            }
                        }
                }
            }

            if (drawBorder)
                AddBorder(obj.Border, obj.AbsLeft, obj.AbsTop, obj.Width, obj.Height);
        }

        private string FloatToString(double flt)
        {
            return ExportUtils.FloatToString(flt);
        }

        /// <summary>
        /// Save DXF file.
        /// </summary>
        private void Save(string filename)
        {
            string dxfString = dxfDocument.ToString();
            File.WriteAllText(filename, dxfString);
        }

        /// <summary>
        /// Save DXF stream.
        /// </summary>
        private void Save(Stream stream)
        {
            string dxfString = dxfDocument.ToString();
            byte[] encodedDfx = Encoding.UTF8.GetBytes(dxfString);
            stream.Write(encodedDfx, 0, encodedDfx.Length);
        }

        #endregion Private Methods
    }
}