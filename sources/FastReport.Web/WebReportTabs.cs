
namespace FastReport.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class ReportTab
    {
        private Report fReport;
        private WebReportProperties fProperties;
        private string fName;
        private bool fNeedParent;

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return fName; }            
            set { fName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Report Report
        {
            get { return fReport; }
            set { fReport = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public WebReportProperties Properties
        {
            get { return fProperties; }
            set { fProperties = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool NeedParent
        {
            get { return fNeedParent; }
            set { fNeedParent = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ReportTab()
        {
            fReport = new Report();
            fProperties = new WebReportProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        public ReportTab(Report report)
        {
            fReport = report;
            fProperties = new WebReportProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        public ReportTab(Report report, WebReportProperties properties)
        {
            fReport = report;
            fProperties = properties;
            fNeedParent = false;
        }
    }
}
