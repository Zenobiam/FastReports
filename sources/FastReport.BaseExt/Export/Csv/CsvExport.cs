using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using FastReport.Utils;
using FastReport.Export;

namespace FastReport.Export.Csv
{
    /// <summary>
    /// Represents the CSV export filter.
    /// </summary>
    public partial class CSVExport : ExportBase
    {
        #region Constants

        byte[] u_HEADER = { 239, 187, 191 };

        #endregion

        #region Private fields

        private ExportMatrix matrix;
        private string separator;
        private Encoding encoding;
        private bool dataOnly;
        private bool noQuotes;
        private bool escapeQuotes;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or set the resulting file encoding.
        /// </summary>
        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        /// <summary>
        /// Gets or set the separator character used in csv format.
        /// </summary>
        public string Separator
        {
            get { return separator; }
            set { separator = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether to export the databand rows only.
        /// </summary>
        public bool DataOnly
        {
            get { return dataOnly; }
            set { dataOnly = value; }
        }

        /// <summary>
        /// Gets or sets a value that disable quotation marks for text.
        /// </summary>
        public bool NoQuotes
        {
            get { return noQuotes; }
            set { noQuotes = value; }
        }

        /// <summary>
        /// Gets or sets a value that disable escaping quotation marks for text.
        /// </summary>
        public bool EscapeQuotes
        {
            get { return escapeQuotes; }
            set { escapeQuotes = value; }
        }

        #endregion

        #region Private Methods

        private void ExportCsvPage(Stream stream)
        {            
            int i, x, y;
            ExportIEMObject obj;            

            StringBuilder builder = new StringBuilder(matrix.Width * 64 * matrix.Height);
            for (y = 0; y < matrix.Height - 1; y++)
            {
                for (x = 0; x < matrix.Width; x++)
                {
                    i = matrix.Cell(x, y);
                    if (i != -1)
                    {
                        obj = matrix.ObjectById(i);
                        if (obj.Counter == 0)
                        {
                            // TODO: deprecated method?
                            if (obj.HtmlTags)
                                obj.Text = DeleteHtmlTags(obj.Text);
                            if (!noQuotes)
                            {
                                builder.Append("\"");
                                builder.Append(escapeQuotes ? obj.Text.Replace("\"", "\"\"") : obj.Text);
                                builder.Append("\"");
                            }
                            else
                                builder.Append(obj.Text);
                            obj.Counter = 1;
                        }
                        builder.Append(separator);
                    }
                }

                // remove the last separator in a row
                if (builder.ToString(builder.Length - separator.Length, separator.Length) == separator)
                    builder.Remove(builder.Length - separator.Length, separator.Length);
                builder.AppendLine();
            }
            // write the resulting string to a stream            
            byte[] bytes = encoding.GetBytes(builder.ToString());
            stream.Write(bytes, 0, bytes.Length);
        }

        string DeleteHtmlTags(string text)
        {
            return Regex.Replace(text, @"<[^>]*>", String.Empty);
        }

        #endregion        

        #region Protected Methods

        /// <inheritdoc/>
        protected override void Start()
        {
            base.Start();
            if (encoding == Encoding.UTF8)
                Stream.Write(u_HEADER, 0, 3);
        }

        /// <inheritdoc/>
        protected override void ExportPageBegin(ReportPage page)
        {
            base.ExportPageBegin(page);
            matrix = new ExportMatrix();
            matrix.Inaccuracy = 0.5f;
            matrix.PlainRich = true;
            matrix.AreaFill = false;
            matrix.CropAreaFill = true;
            matrix.Report = Report;
            matrix.Images = false;
            matrix.WrapText = false;
            matrix.DataOnly = dataOnly;
            matrix.ShowProgress = ShowProgress;
            matrix.AddPageBegin(page);
        }

        /// <inheritdoc/>
        protected override void ExportBand(Base band)
        {
            base.ExportBand(band);
            matrix.AddBand(band, this);
        }

        /// <inheritdoc/>
        protected override void ExportPageEnd(ReportPage page)
        {
            matrix.AddPageEnd(page);
            matrix.Prepare();
            ExportCsvPage(Stream);
            base.ExportPageEnd(page);
        }

        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            return new MyRes("FileFilters").Get("CsvFile");
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
          base.Serialize(writer);
          writer.WriteStr("Separator", Separator);
          writer.WriteBool("DataOnly", DataOnly);
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CSVExport"/> class.
        /// </summary>       
        public CSVExport()
        {                         
            separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            encoding = Encoding.Default;
            dataOnly = false;
            noQuotes = false;
        }
    }
}
