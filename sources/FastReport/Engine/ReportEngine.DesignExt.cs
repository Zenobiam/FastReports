using FastReport.Utils;
using System;

namespace FastReport.Engine
{
    partial class ReportEngine
    {
        #region Private Methods

        private void ShowProgress()
        {
            if (!Config.ReportSettings.ShowProgress) return;
            string msg = Report.DoublePass && FirstPass ?
              Res.Get("Messages,GeneratingPageFirstPass") :
              Res.Get("Messages,GeneratingPage");
            Config.ReportSettings.OnProgress(Report, String.Format(msg, CurPage + 1), CurPage + 1, TotalPages);
        }

        #endregion Private Methods
    }
}