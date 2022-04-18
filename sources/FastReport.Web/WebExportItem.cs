using System;

namespace FastReport.Web
{
    /// <summary>
    /// Class for export item description
    /// </summary>
    [Serializable]
    public class WebExportItem
    {
        private string reportID;
        private byte[] file;
        private string fileName;
        private string format;
        private string contentType;

        /// <summary>
        /// Report ID
        /// </summary>
        public string ReportID
        {
            get { return reportID; }
            set { reportID = value; }
        }

        /// <summary>
        /// Binary data of exported files
        /// </summary>
        public byte[] File
        {
            get { return file; }
            set { file = value; }
        }

        /// <summary>
        /// Name of exported file
        /// </summary>
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        /// <summary>
        /// Format of exported file
        /// </summary>
        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        /// <summary>
        /// MIME type of exported file
        /// </summary>
        public string ContentType
        {
            get { return contentType; }
            set { contentType = value; }
        }
    }
}