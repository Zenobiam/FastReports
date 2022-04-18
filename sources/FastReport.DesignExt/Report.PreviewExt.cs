using FastReport.Forms;
using FastReport.Utils;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport
{
    partial class Report
    {
        #region Private Fields

        private EmailSettings emailSettings;
        private FastReport.Preview.PreviewControl preview;
        private Form previewForm;
        private PrintSettings printSettings;
        private bool isPreviewing;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets the email settings such as recipients, subject, message body.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory("Email")]
        public EmailSettings EmailSettings
        {
            get { return emailSettings; }
        }

        /// <summary>
        /// Gets or sets the report preview control.
        /// </summary>
        /// <remarks>
        /// Use this property to attach a custom preview to your report. To do this, place the PreviewControl
        /// control to your form and set the report's <b>Preview</b> property to this control.
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Preview.PreviewControl Preview
        {
            get { return preview; }
            set
            {
                preview = value;
                if (value != null)
                    value.SetReport(this);
            }
        }

        /// <summary>
        /// Gets the print settings such as printer name, copies, pages to print etc.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRCategory("Print")]
        public PrintSettings PrintSettings
        {
            get { return printSettings; }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPreviewing
        {
            get { return isPreviewing; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Prepares the report and prints it.
        /// </summary>
        public void Print()
        {
            if (Prepare())
                PrintPrepared();
        }

        /// <summary>
        /// Prints the report with the "Print" dialog.
        /// Report should be prepared using the <see cref="Prepare()"/> method.
        /// </summary>
        public void PrintPrepared()
        {
            if (PreparedPages != null)
                PreparedPages.Print();
        }

        /// <summary>
        /// Prints the report without the "Print" dialog.
        /// Report should be prepared using the <see cref="Prepare()"/> method.
        /// </summary>
        /// <param name="printerSettings">Printer-specific settings.</param>
        /// <example>
        /// Use the following code if you want to show the "Print" dialog, then print:
        /// <code>
        /// if (report.Prepare())
        /// {
        ///   PrinterSettings printerSettings = null;
        ///   if (report.ShowPrintDialog(out printerSettings))
        ///   {
        ///     report.PrintPrepared(printerSettings);
        ///   }
        /// }
        /// </code>
        /// </example>
        public void PrintPrepared(PrinterSettings printerSettings)
        {
            if (PreparedPages != null)
                PreparedPages.Print(printerSettings, 1);
        }

        /// <summary>
        /// Prepares the report and shows it in the preview window.
        /// </summary>
        public void Show()
        {
            Show(true, null);
        }

        /// <summary>
        /// Prepares the report and shows it in the preview window.
        /// </summary>
        /// <param name="modal">A value that specifies whether the preview window should be modal.</param>
        public void Show(bool modal)
        {
            Show(modal, null);
        }

        /// <summary>
        /// Prepares the report and shows it in the preview window.
        /// </summary>
        /// <param name="modal">A value that specifies whether the preview window should be modal.</param>
        /// <param name="owner">The owner of the preview window.</param>
        public void Show(bool modal, IWin32Window owner)
        {
            if (Prepare())
                ShowPrepared(modal, null, owner);
            else if (Preview != null)
            {
                Preview.Clear();
                Preview.Refresh();
            }
        }

        /// <summary>
        /// Prepares the report and shows it in the preview window.
        /// </summary>
        /// <param name="mdiParent">The main MDI form which will be a parent for the preview window.</param>
        public void Show(Form mdiParent)
        {
            if (Prepare())
                ShowPrepared(false, mdiParent, null);
            else if (Preview != null)
            {
                Preview.Clear();
                Preview.Refresh();
            }
        }

        /// <summary>
        /// Previews the report. The report should be prepared using the <see cref="Prepare()"/> method.
        /// </summary>
        public void ShowPrepared()
        {
            ShowPrepared(true);
        }

        /// <summary>
        /// Previews the prepared report.
        /// </summary>
        /// <param name="modal">A value that specifies whether the preview window should be modal.</param>
        public void ShowPrepared(bool modal)
        {
            ShowPrepared(modal, null, null);
        }

        /// <summary>
        /// Previews the prepared report.
        /// </summary>
        /// <param name="modal">A value that specifies whether the preview window should be modal.</param>
        /// <param name="owner">The owner of the preview window.</param>
        public void ShowPrepared(bool modal, IWin32Window owner)
        {
            ShowPrepared(modal, null, owner);
        }

        /// <summary>
        /// Previews the prepared report.
        /// </summary>
        /// <param name="mdiParent">The main MDI form which will be a parent for the preview window.</param>
        public void ShowPrepared(Form mdiParent)
        {
            ShowPrepared(false, mdiParent, null);
        }

        /// <summary>
        /// Shows the "Print" dialog.
        /// </summary>
        /// <param name="printerSettings">Printer-specific settings.</param>
        /// <returns><b>true</b> if the dialog was closed by "Print" button.</returns>
        /// <example>
        /// Use the following code if you want to show the "Print" dialog, then print:
        /// <code>
        /// if (report.Prepare())
        /// {
        ///   PrinterSettings printerSettings = null;
        ///   if (report.ShowPrintDialog(out printerSettings))
        ///   {
        ///     report.PrintPrepared(printerSettings);
        ///   }
        /// }
        /// </code>
        /// </example>
        public bool ShowPrintDialog(out PrinterSettings printerSettings)
        {
            printerSettings = null;

            using (PrinterSetupForm dialog = new PrinterSetupForm())
            {
                dialog.Report = this;
                dialog.PrintDialog = true;
                if (dialog.ShowDialog() != DialogResult.OK)
                    return false;
                printerSettings = dialog.PrinterSettings;
            }

            return true;
        }

        #endregion Public Methods

        #region Private Methods

        private void ClearPreparedPages()
        {
            if (preview != null)
                preview.ClearTabsExceptFirst();
            else
            if (preparedPages != null)
                preparedPages.Clear();
        }

        private void OnClosePreview(object sender, FormClosedEventArgs e)
        {
            previewForm.Dispose();
            previewForm = null;
            preview = null;
        }

        private void SavePreviewPicture()
        {
            ReportPage page = PreparedPages.GetCachedPage(0);
            float pageWidth = page.WidthInPixels;
            float pageHeight = page.HeightInPixels;
            float ratio = ReportInfo.PreviewPictureRatio;
            ReportInfo.Picture = new Bitmap((int)Math.Round(pageWidth * ratio), (int)Math.Round(pageHeight * ratio));

            using (Graphics g = Graphics.FromImage(ReportInfo.Picture))
            {
                FRPaintEventArgs args = new FRPaintEventArgs(g, ratio, ratio, GraphicCache);
                page.Draw(args);
            }
        }

        private void ShowPrepared(bool modal, Form mdiParent, IWin32Window owner)
        {
            // create preview form
            if (Preview == null)
            {
#if !MONO
                float multiplier = DpiHelper.Multiplier;
                if(Config.Root.FindItem("Forms").Find("PreviewForm") != -1)
                {
                    XmlItem xi = Config.Root.FindItem("Forms").FindItem("PreviewForm");
                    string left = xi.GetProp("Left");
                    string top = xi.GetProp("Top");
                    if(String.IsNullOrEmpty(left) || String.IsNullOrEmpty(top))
                    {
                        left = "0";
                        top = "0";
                    }
                    Screen screen = Screen.FromPoint(new Point( int.Parse(left) + 10, int.Parse(top) + 10));

                    uint dpi = screen.GetDpi();
                    multiplier = 1;
                    multiplier *= (dpi / 96f);
                }

                DpiHelper.RescaleWithNewDpi(() =>
                {
#endif
                    previewForm = new PreviewForm();

                    previewForm.MdiParent = mdiParent;
                    previewForm.ShowInTaskbar = Config.PreviewSettings.ShowInTaskbar;
                    previewForm.TopMost = Config.PreviewSettings.TopMost;
                    previewForm.Icon = Config.PreviewSettings.Icon;
#if !MONO
                }, multiplier);
#endif

                if (String.IsNullOrEmpty(Config.PreviewSettings.Text))
                {
                    previewForm.Text = String.IsNullOrEmpty(ReportInfo.Name) ? "" : ReportInfo.Name + " - ";
                    previewForm.Text += Res.Get("Preview");
                }
                else
                    previewForm.Text = Config.PreviewSettings.Text;

                previewForm.FormClosed += new FormClosedEventHandler(OnClosePreview);

                Preview = (previewForm as PreviewForm).Preview;
                Preview.UIStyle = Config.UIStyle;
                Preview.FastScrolling = Config.PreviewSettings.FastScrolling;
                Preview.Buttons = Config.PreviewSettings.Buttons;
                Preview.Exports = Config.PreviewSettings.Exports;
                Preview.Clouds = Config.PreviewSettings.Clouds;
                Preview.SaveInitialDirectory = Config.PreviewSettings.SaveInitialDirectory;
            }

            if (Config.ReportSettings.ShowPerformance)
                try
                {
                    // in case the format string is wrong, use try/catch
                    Preview.ShowPerformance(String.Format(Res.Get("Messages,Performance"), tickCount));
                }
                catch
                {
                }
            Preview.ClearTabsExceptFirst();
            if (PreparedPages != null)
                Preview.AddPreviewTab(this, GetReportName, null, true);

            Config.PreviewSettings.OnPreviewOpened(Preview);
            if (ReportInfo.SavePreviewPicture && PreparedPages.Count > 0)
                SavePreviewPicture();

            isPreviewing = true;
            if (previewForm != null && !previewForm.Visible)
            {
                if (modal)
                    previewForm.ShowDialog(owner);
                else
                {
                    if (mdiParent == null)
                        previewForm.Show(owner);
                    else
                        previewForm.Show();
                }
                isPreviewing = false;
            }
            
        }

#endregion Private Methods
    }
}