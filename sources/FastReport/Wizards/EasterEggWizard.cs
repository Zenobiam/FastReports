using FastReport.Design;
using FastReport.Utils;
using System.IO;

namespace FastReport.Wizards
{
    internal abstract class EasterEggWizard : WizardBase
    {
        #region Protected Methods

        protected bool LoadStreamToDesigner(Stream s, Designer designer)
        {
            ReportTab reportTab = null;
            if (designer.MdiMode)
            {
                Report report = new Report();
                report.Designer = designer;
                reportTab = designer.CreateReportTab(report);
            }
            else
            {
                reportTab = designer.ActiveReportTab;
            }

            if (LoadStreamToReprotTab(s, designer, reportTab))
            {
                if (designer.MdiMode)
                    designer.AddReportTab(reportTab);
                return true;
            }
            else if (designer.MdiMode)
            {
                reportTab.Dispose();
            }
            return false;
        }

        protected virtual void ProcessPages(Report report)
        {
            foreach (PageBase page in report.Pages)
            {
                if (page is ReportPage)
                {
                    ReportPage reportPage = page as ReportPage;
                    switch (Config.ReportSettings.DefaultPaperSize)
                    {
                        case DefaultPaperSize.A4:
                            reportPage.PaperWidth = 210;
                            reportPage.PaperHeight = 297;
                            break;

                        case DefaultPaperSize.Letter:
                            reportPage.PaperWidth = 215.9f;
                            reportPage.PaperHeight = 279.4f;
                            break;
                    }
                }
            }
        }

        protected virtual bool ShowDialog()
        {
            return true;
        }

        #endregion Protected Methods

        #region Private Methods

        private bool LoadStreamToReprotTab(Stream s, Designer designer, ReportTab reportTab)
        {
            OpenSaveDialogEventArgs e = new OpenSaveDialogEventArgs(designer);

            bool result = reportTab.SaveCurrentFile();
            if (result)
            {
                result = ShowDialog();
                if (result)
                {
                    try
                    {
                        designer.Lock();
                        reportTab.Report.Load(s);
                        ProcessPages(reportTab.Report);
                        Config.DesignerSettings.OnReportLoaded(this, new ReportLoadedEventArgs(reportTab.Report));
                        result = true;
                    }
                    finally
                    {
                        reportTab.InitReport();
                        designer.Unlock();
                    }
                }
            }

            return result;
        }

        #endregion Private Methods
    }
}