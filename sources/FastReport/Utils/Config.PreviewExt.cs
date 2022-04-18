using System;

namespace FastReport.Utils
{
    partial class Config
    {
        #region Private Fields

        private static PreviewSettings FPreviewSettings = new PreviewSettings();

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets the settings for the preview window.
        /// </summary>
        public static PreviewSettings PreviewSettings
        {
            get { return FPreviewSettings; }
            set { FPreviewSettings = value; }
        }

        #endregion Public Properties

        #region Private Methods

        private static void RestorePreviewSettings()
        {
            XmlItem xi = Root.FindItem("PreviewSettings");

            string exports = xi.GetProp("Exports");
            if (!String.IsNullOrEmpty(exports))
            {
                try
                {
                    PreviewSettings.Exports = (PreviewExports)Converter.FromString(typeof(PreviewExports), exports);
                }
                catch
                {
                    PreviewSettings.Exports = PreviewExports.All;
                }
            }

            string clouds = xi.GetProp("Clouds");
            if (!String.IsNullOrEmpty(clouds))
            {
                try
                {
                    PreviewSettings.Clouds = (PreviewClouds)Converter.FromString(typeof(PreviewClouds), clouds);
                }
                catch
                {
                    PreviewSettings.Clouds = PreviewClouds.All;
                }
            }

            string messengers = xi.GetProp("Messengers");
            if (!String.IsNullOrEmpty(messengers))
            {
                try
                {
                    PreviewSettings.Messengers = (PreviewMessengers)Converter.FromString(typeof(PreviewMessengers), messengers);
                }
                catch
                {
                    PreviewSettings.Messengers = PreviewMessengers.All;
                }
            }
        }

        private static void SavePreviewSettings()
        {
            XmlItem xi = Root.FindItem("PreviewSettings");
            xi.SetProp("Exports", Converter.ToString(PreviewSettings.Exports));
            xi.SetProp("Clouds", Converter.ToString(PreviewSettings.Clouds));
            xi.SetProp("Messengers", Converter.ToString(PreviewSettings.Messengers));
        }

        #endregion Private Methods
    }
}