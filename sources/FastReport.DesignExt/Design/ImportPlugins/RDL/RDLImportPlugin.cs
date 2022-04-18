using FastReport.Import.RDL;

namespace FastReport.Design.ImportPlugins.RDL
{
    /// <summary>
    /// Represents the RDL import plugin.
    /// </summary>
    public partial class RDLImportPlugin : ImportPlugin
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RDLImportPlugin"/> class.
        /// </summary>
        public RDLImportPlugin() : base()
        {
            Import = new RDLImport();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RDLImportPlugin"/> class with a specified designer.
        /// </summary>
        /// <param name="designer">The report designer.</param>
        public RDLImportPlugin(Designer designer) : base(designer)
        {
            Import = new RDLImport();
        }

        #endregion // Constructors

        #region Protected Methods

        /// <inheritdoc/>
        protected override string GetFilter()
        {
            return new Utils.MyRes("FileFilters").Get("RdlFiles");
        }

        #endregion // Protected Methods

        #region Public Methods

        /// <inheritdoc/>
        public override void LoadReport(Report report, string filename)
        {
            Import.LoadReport(report, filename);
        }

        #endregion // Public Methods
    }
}
