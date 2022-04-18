using FastReport.Utils;
using FastReport.Import.ListAndLabel;

namespace FastReport.Design.ImportPlugins.ListAndLabel
{
    /// <summary>
    /// Represents the List and Label import plugin.
    /// </summary>
    public class ListAndLabelImportPlugin : ImportPlugin
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ListAndLabelImportPlugin"/> class.
        /// </summary>
        public ListAndLabelImportPlugin() : base()
        {
            Import = new ListAndLabelImport();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListAndLabelImportPlugin"/> class with a specified designer.
        /// </summary>
        /// <param name="designer">The report designer.</param>
        public ListAndLabelImportPlugin(Designer designer) : base(designer)
        {
            Import = new ListAndLabelImport();
        }

        #endregion // Constructors

        #region Protected Methods

        ///<inheritdoc/>
        protected override string GetFilter()
        {
            return new FastReport.Utils.MyRes("FileFilters").Get("ListAndLabelFiles");
        }

        #endregion // Protected Methods

        #region Public Methods

        ///<inheritdoc/>
        public override void LoadReport(Report report, string filename)
        {
            Import.LoadReport(report, filename);
            if (!(Import as ListAndLabelImport).IsListAndLabelReport)
            {
                MyRes res = new MyRes("Messages");
                FRMessageBox.Error(res.Get("WrongListAndLabelFile"));
            }
        }

        #endregion // Public Methods
    }
}
