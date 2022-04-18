using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FastReport.Export
{
    partial class ExportBase
    {
        #region Public Methods

        /// <summary>
        /// Exports the report to a file.
        /// </summary>
        /// <param name="report">Report to export.</param>
        /// <returns><b>true</b> if report was succesfully exported.</returns>
        /// <remarks>
        /// This method displays an export options dialog, then prompts a file name using standard "Open file"
        /// dialog. If both dialogs were closed by OK button, exports the report and returns <b>true</b>.
        /// </remarks>
        public bool Export(Report report)
        {
            SetReport(report);

            if (AllowSaveSettings)
                RestoreSettings();

            if (ShowDialog())
            {
                if (AllowSaveSettings)
                    SaveSettings();

                while (true)
                {
                    using (SaveFileDialog dialog = new SaveFileDialog())
                    {
                        dialog.Filter = FileFilter;
                        string defaultExt = dialog.Filter.Split(new char[] { '|' })[1];
                        dialog.DefaultExt = Path.GetExtension(defaultExt);
                        dialog.FileName = GetFileName(report) + "." + dialog.DefaultExt;
                        if (!string.IsNullOrEmpty(SaveInitialDirectory))
                            dialog.InitialDirectory = SaveInitialDirectory;
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            Config.DoEvent();

                            try
                            {
                                Export(report, dialog.FileName);
                                return true;
                            }
                            catch (IOException ex)
                            {
                                if (MessageBox.Show(ex.Message + "\r\n\r\n" + Res.Get("Messages,SaveToAnotherFile"),
                                  Res.Get("Messages,Error"),
                                  MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                                    return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Displays a dialog with export options.
        /// </summary>
        /// <returns><b>true</b> if dialog was closed with OK button.</returns>
        public virtual bool ShowDialog()
        {
            return true;
        }



        #endregion Public Methods

        #region Private Methods

        private void ShowPerformance(int exportTickCount)
        {
            if (Report.Preview != null && Config.ReportSettings.ShowPerformance)
                Report.Preview.ShowPerformance(String.Format(Res.Get("Export,Misc,Performance"), exportTickCount));
        }

        /// <summary>
        /// Gets a report page with OverlayBand if it is a Demo or Academic.
        /// </summary>
        /// <param name="page">The prepared report page</param>
        /// <returns>The prepared report page with OverlayBand.</returns>
        protected ReportPage GetOverlayPage(ReportPage page)
        {
            if (page != null)
            {
#if Demo
                OverlayBand band = new OverlayBand();
                band.Parent = page;
                band.Bounds = new RectangleF(0, -page.TopMargin * Units.Millimeters, 200, 20);
                TextObject text = new TextObject();
                text.Parent = band;
                text.Bounds = new RectangleF(0, 0, Units.Millimeters * 50, 20);
                text.Text = typeof(Double).Name[0].ToString() + typeof(Exception).Name[0].ToString() +
                    typeof(Math).Name[0].ToString() + typeof(Object).Name[0].ToString() + " " +
                    typeof(ValueType).Name[0].ToString() + typeof(Exception).Name[0].ToString() +
                    typeof(Rectangle).Name[0].ToString() + typeof(ShapeKind).Name[0].ToString() +
                    typeof(ICloneable).Name[0].ToString() + typeof(Object).Name[0].ToString() +
                    typeof(NonSerializedAttribute).Name[0].ToString();
#endif

#if Academic
      OverlayBand band = new OverlayBand();
      band.Parent = page;
      band.Bounds = new RectangleF(0, -page.TopMargin * Units.Millimeters, 200, 20);
      TextObject text = new TextObject();
      text.Parent = band;
      text.Bounds = new RectangleF(0, 0, Units.Millimeters * 50, 20);
      text.Text = typeof(Array).Name[0].ToString() + typeof(Char).Name[0].ToString() +
          typeof(Array).Name[0].ToString() + typeof(DateTime).Name[0].ToString() +
          typeof(Enum).Name[0].ToString() + typeof(Math).Name[0].ToString() +
          typeof(IComparable).Name[0].ToString() + typeof(Char).Name[0].ToString() +
          " " +
          typeof(LineObject).Name[0].ToString() + typeof(IComparable).Name[0].ToString() +
          typeof(Char).Name[0].ToString() + typeof(Enum).Name[0].ToString() +
          typeof(Nullable).Name[0].ToString() + typeof(ShapeObject).Name[0].ToString() +
          typeof(Enum).Name[0].ToString(); 
#endif
            }
            return page;
        }

        private int GetPagesCount(List<int> pages)
        {
#if Demo
            return 5;
#else
            return pages.Count;
#endif
        }

        #endregion Private Methods

#if Demo || Academic
        internal const bool HAVE_TO_WORK_WITH_OVERLAY = true;
#else
        internal const bool HAVE_TO_WORK_WITH_OVERLAY = false;
#endif
    }

}