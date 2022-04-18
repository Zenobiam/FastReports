using FastReport.Forms;
using FastReport.Import;

namespace FastReport.Design
{
    /// <summary>
    /// Base class for all import plugins.
    /// </summary>
    public class ImportPlugin : IDesignerPlugin
    {
        #region Fields

        private string filter;
        private Designer designer;
        private Report report;
        private ImportBase import;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets or sets the name of plugin.
        /// </summary>
        public string Name
        {
            get { return import.Name; }
        }

        /// <summary>
        /// Gets or sets the filter string used in the "Open File" dialog.
        /// </summary>
        public string Filter
        {
            get { return filter; }
            protected set { filter = value; }
        }

        /// <summary>
        /// Gets or sets reference to the designer.
        /// </summary>
        public Designer Designer
        {
            get { return designer; }
            protected set { designer = value; }
        }

        /// <summary>
        /// Gets or sets reference to the report.
        /// </summary>
        public Report Report
        {
            get { return report; }
            protected set { report = value; }
        }

        /// <inheritdoc/>
        public string PluginName
        {
            get { return import.Name; }
        }

        /// <summary>
        /// Gets or sets reference to the import.
        /// </summary>
        protected ImportBase Import
        {
            get { return import; }
            set { import = value; }
        }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportPlugin"/> class with default settings.
        /// </summary>
        public ImportPlugin()
        {
            filter = GetFilter();
            import = new ImportBase();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportPlugin"/> class with a specified designer.
        /// </summary>
        /// <param name="designer">The report designer.</param>
        public ImportPlugin(Designer designer) : this()
        {
            this.designer = designer;
        }

        #endregion // Constructors

        #region IDesignerPlugin Members

        /// <inheritdoc/>
        public void SaveState()
        {
        }

        /// <inheritdoc/>
        public void RestoreState()
        {
        }

        /// <inheritdoc/>
        public void SelectionChanged()
        {
        }

        /// <inheritdoc/>
        public void UpdateContent()
        {
        }

        /// <inheritdoc/>
        public void Lock()
        {
        }

        /// <inheritdoc/>
        public void Unlock()
        {
        }

        /// <inheritdoc/>
        public virtual void Localize()
        {
        }

        /// <inheritdoc/>
        public DesignerOptionsPage GetOptionsPage()
        {
            return null;
        }

        /// <inheritdoc/>
        public virtual void UpdateUIStyle()
        {
        }

        ///<inheritdoc/>
        public void ReinitDpiSize()
        {
        }

        #endregion // IDesignerPlugin Members

        #region Protected Methods

        /// <summary>
        /// Returns a file filter for a open dialog.
        /// </summary>
        /// <returns>String that contains a file filter, for example: "Bitmap image (*.bmp)|*.bmp"</returns>
        protected virtual string GetFilter()
        {
            return "";
        }

        #endregion // Protected Methods

        #region Public Methods

        /// <summary>
        /// Loads the specified file into specified report.
        /// </summary>
        /// <param name="report">Report object.</param>
        /// <param name="filename">File name.</param>
        public virtual void LoadReport(Report report, string filename)
        {
            report.Clear();
        }

        #endregion // Public Methods
    }
}
