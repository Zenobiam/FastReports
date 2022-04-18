using FastReport.Utils;
using System.Drawing;

namespace FastReport
{
    partial class StyleBase
    {
        #region Private Methods

        private Font GetDefaultFontInternal()
        {
            return Config.DesignerSettings.DefaultFont;
        }

        #endregion Private Methods
    }
}