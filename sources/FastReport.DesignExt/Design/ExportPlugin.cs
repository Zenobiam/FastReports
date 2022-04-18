using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Forms;

namespace FastReport.Design
{
    /// <summary>
    /// Base class for all export plugins.
    /// </summary>
    public class ExportPlugin : IDesignerPlugin
    {
        #region Fields

        private string name;
        private string filter;
        private Designer designer;
        private Report report;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets or sets the name of plugin.
        /// </summary>
        public string Name
        {
            get { return name; }
            protected set { name = value; }
        }

        /// <summary>
        /// Gets or sets the filter string used in the "Save File" dialog.
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

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportPlugin"/> class with default settings.
        /// </summary>
        public ExportPlugin()
        {
            filter = GetFilter();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportPlugin"/> class with a specified designer.
        /// </summary>
        /// <param name="designer">The report designer.</param>
        public ExportPlugin(Designer designer) : this()
        {
            this.designer = designer;
        }

        #endregion // Constructors

        #region IDesignerPlugin Members

        /// <inheritdoc/>
        public string PluginName
        {
            get { return name; }
        }

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
        public void Localize()
        {
        }

        /// <inheritdoc/>
        public DesignerOptionsPage GetOptionsPage()
        {
            return null;
        }

        /// <inheritdoc/>
        public void UpdateUIStyle()
        {
        }

        ///<inheritdoc/>
        public void ReinitDpiSize()
        {
        }

        #endregion // IDesignerPlugin Members

        #region Protected Methods

        /// <summary>
        /// Returns a file filter for a save dialog.
        /// </summary>
        /// <returns>String that contains a file filter, for example: "Bitmap image (*.bmp)|*.bmp"</returns>
        protected virtual string GetFilter()
        {
            return "";
        }

        #endregion // Protected Methods

        #region Public Methods

        /// <summary>
        /// Saves the specified report into specified file.
        /// </summary>
        /// <param name="report">Report object.</param>
        /// <param name="filename">File name.</param>
        public virtual void SaveReport(Report report, string filename)
        {
        }

        #endregion // Public Methods
    }
}
