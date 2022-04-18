using FastReport.Import.DevExpress;

namespace FastReport.Design.ImportPlugins.DevExpress
{
    /// <summary>
    /// Represents the DevExpess import plugin.
    /// </summary>
    partial class DevExpressImportPlugin : ImportPlugin
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DevExpressImportPlugin"/> class.
        /// </summary>
        public DevExpressImportPlugin() : base()
        {
            Import = new DevExpressImport();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DevExpressImportPlugin"/> class with a specified designer.
        /// </summary>
        /// <param name="designer">The report designer.</param>
        public DevExpressImportPlugin(Designer designer) : base(designer)
        {
            Import = new DevExpressImport();
        }

        #endregion // Constructors

        #region Protected Methods

        ///<inheritdoc/>
        protected override string GetFilter()
        {
            return new FastReport.Utils.MyRes("FileFilters").Get("DevExpressFiles");
        }

        #endregion // Protected Methods

        #region Public Methods

        public override void LoadReport(Report report, string filename)
        {
            Import.LoadReport(report, filename);
        }

        #endregion // Public Methods
    }
}
