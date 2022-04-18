namespace FastReport.Design.ImportPlugins.RTF
{
    /// <summary>
    /// Import RichTextFile to a report
    /// </summary>
    public class RTFImportPlugin : ImportPlugin
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RTFImportPlugin"/> class.
        /// </summary>
        public RTFImportPlugin() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RTFImportPlugin"/> class with a specified designer.
        /// </summary>
        /// <param name="designer">The report designer.</param>
        public RTFImportPlugin(Designer designer) : base(designer)
        {
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc/>
        protected override string GetFilter()
        {
            return new Utils.MyRes("FileFilters").Get("RtfFile");
        }

        #endregion // Protected Methods

        #region Public Methods

        /// <inheritdoc/>
        public override void LoadReport(Report report, string filename)
        {
                ImportRtf rtf_convertor = new ImportRtf(filename);
                rtf_convertor.ResetProperties();
                Report = rtf_convertor.CreateReport();
                Report.FileName = filename + ".frx";
                Report.Save(Report.FileName);
                Designer.cmdOpen.LoadFile(Report.FileName);
        }

        #endregion // Public Methods
    }
}