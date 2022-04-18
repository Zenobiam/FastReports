using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data.Common;
using FastReport.TypeConverters;
using FastReport.Forms;
using FastReport.Data;
using FastReport.Utils;

namespace FastReport
{
    partial class ReportSettings
    {
        private ProgressForm progress;
        private bool showProgress = true;
        private bool showPerformance;

        /// <summary>
        /// Occurs before displaying a progress window.
        /// </summary>
        public event EventHandler StartProgress;

        /// <summary>
        /// Occurs after closing a progress window.
        /// </summary>
        public event EventHandler FinishProgress;

        /// <summary>
        /// Occurs after printing a report.
        /// </summary>
        public event EventHandler ReportPrinted;

        /// <summary>
        /// Occurs when progress state is changed.
        /// </summary>
        public event ProgressEventHandler Progress;

        /// <summary>
        /// Gets or sets a value that determines whether to show the progress window
        /// when perform time-consuming operations such as run, print, export.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowProgress
        {
            get { return showProgress; }
            set { showProgress = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether to show the information about
        /// the report performance (report generation time, memory consumed) in the 
        /// lower right corner of the preview window.
        /// </summary>
        [DefaultValue(false)]
        public bool ShowPerformance
        {
            get { return showPerformance; }
            set { showPerformance = value; }
        }

        internal void OnStartProgress(Report report)
        {
            progress = null;
            report.SetAborted(false);

            if (ShowProgress)
            {
                if (StartProgress != null)
                    StartProgress(report, EventArgs.Empty);
                else
                {
                    progress = new ProgressForm(report);
                    progress.ShowProgressMessage(Res.Get("Messages,PreparingData"));
                    progress.Show();
                    progress.Refresh();
                }
            }
        }

        internal void OnFinishProgress(Report report)
        {
            if (ShowProgress)
            {
                if (FinishProgress != null)
                    FinishProgress(report, EventArgs.Empty);
                else if (progress != null)
                {
                    progress.Close();
                    progress.Dispose();
                    progress = null;
                }
            }
        }

        internal void OnProgress(Report report, string message)
        {
            OnProgress(report, message, 0, 0);
        }

        internal void OnProgress(Report report, string message, int pageNumber, int totalPages)
        {
            if (ShowProgress)
            {
                if (Progress != null)
                    Progress(report, new ProgressEventArgs(message, pageNumber, totalPages));
                else if (progress != null)
                    progress.ShowProgressMessage(message);
            }
        }

        internal void OnReportPrinted(object sender)
        {
            if (ReportPrinted != null)
                ReportPrinted(sender, EventArgs.Empty);
        }

        internal void OnDatabaseLogin(DataConnectionBase sender, DatabaseLoginEventArgs e)
        {

            if (Config.DesignerSettings.ApplicationConnection != null &&
              sender.GetType() == Config.DesignerSettings.ApplicationConnectionType)
            {
                e.ConnectionString = Config.DesignerSettings.ApplicationConnection.ConnectionString;
            }
            if (DatabaseLogin != null)
                DatabaseLogin(sender, e);
        }
    }
}
