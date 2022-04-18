using FastReport.Utils;

namespace FastReport.CrossView
{
    partial class CrossViewHelper
    {
        private void OnProgressInternal()
        {
            Config.ReportSettings.OnProgress(Report, Res.Get("ComponentsMisc,CrossView,FillData"), 0, 0);
        }
    }
}
