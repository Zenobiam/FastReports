using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Drawing.Imaging;
using FastReport.Utils;
using System.Drawing;

namespace FastReport.Export.Svg
{
    /// <summary>
    /// Represents the SVG export filter.
    /// </summary>
    public partial class SVGExport : ExportBase
    {
        private XmlDocument doc;
        
        #region Private Fields       
        private string path;
        private string fileNameWOext;
        private string extension;
        private string pageFileName;
        private SvgGraphics graphics;
        private int currentPage;
        private bool pictures;
        private bool embedPictures;
        private bool useWidthAndHeight;
        private bool useViewBox;
        private AspectRatio preserveAspectRatio;

        private float pageWidth;
        private float pageHeight;
        #endregion

        #region Properties
        /// <summary>
        /// Enable or disable the pictures in SVG export
        /// </summary>
        public bool Pictures
        {
            get { return pictures; }
            set { pictures = value; }
        }

        /// <summary>
        /// Gets or sets the image format used when exporting.
        /// </summary>
        public SVGImageFormat ImageFormat { get; set; }

        /// <summary>
        /// Embed images into svg
        /// </summary>
        public bool EmbedPictures
        {
            get { return embedPictures; }
            set { embedPictures = value; }
        }

        /// <summary>
        /// Gets or sets value indicating whether or not should to force uniform scaling of SVG document
        /// </summary>
        public AspectRatio PreserveAspectRatio
        {
            get { return preserveAspectRatio; }
            set { preserveAspectRatio = value; }
        }

        /// <summary>
        /// Gets or sets value indicating whether or not should be added 'viewBox' attribute to the svg tag
        /// </summary>
        public bool UseViewBox
        {
            get { return useViewBox; }
            set { useViewBox = value; }
        }

        /// <summary>
        /// Gets or sets value indicating whether or not should be added 'width' and 'height' attributes to the svg tag
        /// </summary>
        public bool UseWidthAndHeight
        {
            get { return useWidthAndHeight; }
            set { useWidthAndHeight = value; }
        }

        #endregion

        #region Private XML Methods
        private void Save(string filename)
        {
            doc.Save(filename);
        }

        private void Save(Stream stream)
        {
            doc.Save(stream);
        }

        private void CreateDoc(string imagePathAndPrefix)
        {
            doc = new XmlDocument();
            doc.AutoIndent = true;
            
            graphics = new SvgGraphics(doc);
            graphics.SvgImageFormat = this.ImageFormat;
            graphics.EmbeddedImages = this.EmbedPictures;
            if (!EmbedPictures)
                graphics.ImageFilePrefix = imagePathAndPrefix;
        }

        private void UpdateSize(float width, float height)
        {
            if (useWidthAndHeight)
                graphics.Size = new SizeF(width, height);
            if (useViewBox)
                graphics.ViewBox = new RectangleF(0, 0, width, height);
            if (preserveAspectRatio != null)
            {
                FastString ratioVal = new FastString(preserveAspectRatio.Align.ToString());
                if (preserveAspectRatio.MeetOrSlice != null)
                {
                    ratioVal.Append(" ").Append(preserveAspectRatio.MeetOrSlice.ToString());
                }
                doc.Root.SetProp("preserveAspectRatio", ratioVal.ToString());
            }
        }

        private void AddImageWatermark(ReportPage page)
        {
            page.Watermark.DrawImage(new FRPaintEventArgs(graphics, 1, 1, Report.GraphicCache),
                new RectangleF(-page.LeftMargin * Units.Millimeters, -page.TopMargin * Units.Millimeters, page.WidthInPixels, page.HeightInPixels),
                page.Report, false);
        }

        private void AddTextWatermark(ReportPage page)
        {
            if (string.IsNullOrEmpty(page.Watermark.Text))
                return;
            page.Watermark.DrawText(new FRPaintEventArgs(graphics, 1, 1, Report.GraphicCache),
                new RectangleF(-page.LeftMargin * Units.Millimeters, -page.TopMargin * Units.Millimeters, page.WidthInPixels, page.HeightInPixels),
                page.Report, false);
        }
        #endregion

        #region Protected Methods

        /// <inheritdoc/>
        protected override void Start()
        {
            base.Start();

            GeneratedStreams = new List<Stream>();
            currentPage = 0;
            pageWidth = 0;
            pageHeight = 0;

            if (FileName != "" && FileName != null)
            {
                path = Path.GetDirectoryName(FileName);
                fileNameWOext = Path.GetFileNameWithoutExtension(FileName);
                extension = Path.GetExtension(FileName);
            }
            else
            {
                path = "";
                fileNameWOext = "svgreport";
            }

            if (!HasMultipleFiles)
                CreateDoc(Path.Combine(path, fileNameWOext));
        }

        /// <summary>
        /// Begin exporting of page
        /// </summary>
        /// <param name="page"></param>
        protected override void ExportPageBegin(ReportPage page)
        {
            base.ExportPageBegin(page);

            if (path != null && path != "")
                pageFileName = Path.Combine(path, fileNameWOext + currentPage.ToString() + extension);
            else
                pageFileName = null;

            if (HasMultipleFiles)
            {
                CreateDoc(Path.Combine(path, fileNameWOext + currentPage.ToString()));
                UpdateSize(page.WidthInPixels, page.HeightInPixels);
            }

            graphics.TranslateTransform(page.LeftMargin * Units.Millimeters, page.TopMargin * Units.Millimeters);
            // export bottom watermark
            if (page.Watermark.Enabled && !page.Watermark.ShowImageOnTop)
                AddImageWatermark(page);
            if (page.Watermark.Enabled && !page.Watermark.ShowTextOnTop)
                AddTextWatermark(page);
        }

        /// <inheritdoc/>
        protected override void ExportBand(Base band)
        {
            base.ExportBand(band);
            ExportObj(band);
            foreach (Base c in band.AllObjects)
            {
                ExportObj(c);
            }
        }

        private void ExportObj(Base obj)
        {
            if (obj is ReportComponentBase && (obj as ReportComponentBase).Exportable)
                (obj as ReportComponentBase).Draw(new FRPaintEventArgs(graphics, 1, 1, Report.GraphicCache));
        }

        /// <summary>
        /// End exporting
        /// </summary>
        /// <param name="page"></param>
        protected override void ExportPageEnd(ReportPage page)
        {
            base.ExportPageEnd(page);

            // export top watermark
            if (page.Watermark.Enabled && page.Watermark.ShowImageOnTop)
                AddImageWatermark(page);
            if (page.Watermark.Enabled && page.Watermark.ShowTextOnTop)
                AddTextWatermark(page);

            if (HasMultipleFiles)
            {
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
                        // save all pages after first in files
                        Save(pageFileName);
                        GeneratedFiles.Add(pageFileName);
                    }
                }
                else if (string.IsNullOrEmpty(path))
                {
                    // server mode, save in internal stream collection
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
                        MemoryStream pageStream = new MemoryStream();
                        Save(pageStream);
                        pageStream.Position = 0;
                        GeneratedStreams.Add(pageStream);
                        GeneratedFiles.Add(pageFileName);
                    }
                }
            }
            else
            {
                graphics.TranslateTransform(0, page.HeightInPixels);
                if (pageWidth < page.WidthInPixels)
                    pageWidth = page.WidthInPixels;
                pageHeight += page.HeightInPixels;
            }

            // increment page number
            currentPage++;
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            if (!HasMultipleFiles)
            {
                UpdateSize(pageWidth, pageHeight);
                Save(Stream);
                Stream.Position = 0;
                GeneratedFiles.Add(FileName);
            }

            graphics.Dispose();
            graphics = null;
            doc.Dispose();
            doc = null;
            GeneratedFiles.Clear();
            GeneratedStreams.Clear();
        }

        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            return new MyRes("FileFilters").Get("SVGFile");
        }
        #endregion

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            base.Serialize(writer);
            writer.WriteValue("ImageFormat", ImageFormat);
            writer.WriteBool("HasMultipleFiles", HasMultipleFiles);
            writer.WriteValue("EmbedPictures", EmbedPictures);
        }




        /// <summary>
        /// Initializes a new instance of the <see cref="SVGExport"/> class.
        /// </summary>
        public SVGExport()
        {
            HasMultipleFiles = false;
            pictures = true;
            ImageFormat = SVGImageFormat.Png;
            preserveAspectRatio = null;
            useWidthAndHeight = false;
            useViewBox = true;
        }
    }
}
