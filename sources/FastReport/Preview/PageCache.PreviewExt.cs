using FastReport.Utils;

namespace FastReport.Preview
{
    partial class PageCache
    {
        #region Private Methods

        private int GetPageLimit()
        {
            return Config.PreviewSettings.PagesInCache;
        }

        #endregion Private Methods
    }
}