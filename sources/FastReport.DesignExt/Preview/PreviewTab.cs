using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using FastReport.Utils;
using FastReport.Forms;
using FastReport.Export.Email;
#if !MONO
using FastReport.DevComponents.DotNetBar;
using FastReport.DevComponents;
#else
using FastReport.Controls;
using FastReport.MonoCap;
#endif

namespace FastReport.Preview
{
#if !MONO
    internal class PreviewTab : TabItem
#else
    internal class PreviewTab : PageControlPage
#endif
    {
        private PreviewWorkspace workspace;
        private PreviewControl preview;
        private PreparedPages preparedPages;
        private bool fake;
        private UIStyle style;
        private Report report;
        private ReportPage detailReportPage;
        private string hyperlinkValue;
        private string saveInitialDirectory;

        #region Properties
        public Report Report
        {
            get { return report; }
        }

        public ReportPage DetailReportPage
        {
            get { return detailReportPage; }
        }

        public string HyperlinkValue
        {
            get { return hyperlinkValue; }
        }

        public SearchInfo SearchInfo
        {
            get { return Workspace.SearchInfo; }
            set { Workspace.SearchInfo = value; }
        }

        private PreviewWorkspace Workspace
        {
            get { return workspace; }
        }

        public PreviewControl Preview
        {
            get { return preview; }
        }

        public PreparedPages PreparedPages
        {
            get { return preparedPages; }
        }

        public int PageNo
        {
            get { return Workspace.PageNo; }
            set { Workspace.PageNo = value; }
        }

        public int PageCount
        {
            get { return Workspace.PageCount; }
        }

        public float Zoom
        {
            get { return Workspace.Zoom; }
            set { Workspace.Zoom = value; }
        }

        public bool Disabled
        {
            get { return Workspace.Disabled; }
        }

        public bool Fake
        {
            get { return fake; }
            set { fake = value; }
        }

        public UIStyle Style
        {
            get { return style; }
            set
            {
                style = value;
#if !MONO
                Workspace.ColorSchemeStyle = UIStyleUtils.GetDotNetBarStyle(value);
                Workspace.Office2007ColorTable = UIStyleUtils.GetOffice2007ColorScheme(value);
#endif
                Workspace.BackColor = Preview.BackColor;
            }
        }

        /// <summary>
        /// Gets or sets the initial directory that is displayed by a save file dialog.
        /// </summary>
        public string SaveInitialDirectory
        {
            get { return saveInitialDirectory; }
            set { saveInitialDirectory = value; }
        }
        #endregion

        #region Private Methods
        private void SetHyperlinkInfo(Hyperlink hyperlink)
        {
            if (hyperlink == null)
                return;
            hyperlinkValue = hyperlink.Value;
            if (hyperlink.Kind == HyperlinkKind.DetailPage)
                detailReportPage = Report.FindObject(hyperlink.DetailPageName) as ReportPage;
        }

        private void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            (sender as PreviewSearchForm).Dispose();
        }
        #endregion

        #region Protected Methods
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                workspace.Dispose();
                preparedPages.Dispose();
            }
        }
        #endregion

        #region Public Methods
        public void BindPreparedPages()
        {
            report.SetPreparedPages(preparedPages);
        }

        public void UpdatePages()
        {
            Workspace.UpdatePages();
        }

        public void PositionTo(int pageNo, PointF point)
        {
            Workspace.PositionTo(pageNo, point);
        }

        public void RefreshReport()
        {
            Workspace.RefreshReport();
        }

#if !MONO
        public void AddToTabControl(FastReport.DevComponents.DotNetBar.TabControl tabControl)
        {
            tabControl.Controls.Add(Workspace);
            tabControl.Tabs.Add(this);
        }
#else
        public void AddToTabControl(FRTabControl tabControl)
        {
            if (tabControl.Tabs != null)
                tabControl.Tabs.Add(this);
        }
#endif

        public void Focus()
        {
            Workspace.Focus();
        }

#if !MONO
        public void Refresh()
#else
        public override void Refresh()
#endif
        {
            Workspace.Refresh();
        }

        public void UnlockLayout()
        {
            Workspace.UnlockLayout();
        }
        #endregion

        #region Preview commands
        public bool Print()
        {
            if (Disabled)
                return false;
            return preparedPages.Print(PageNo);
        }

        public void Save()
        {
            if (Disabled)
                return;

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.FileName = Path.GetFileNameWithoutExtension(Path.GetFileName(Report.FileName)) + ".fpx";
                dialog.Filter = Res.Get("FileFilters,PreparedReport");
                dialog.DefaultExt = "fpx";
                if (!string.IsNullOrEmpty(SaveInitialDirectory))
                    dialog.InitialDirectory = SaveInitialDirectory;
                if (dialog.ShowDialog() == DialogResult.OK)
                    Save(dialog.FileName);
            }
        }

        public void Save(string fileName)
        {
            if (Disabled)
                return;

            preparedPages.Save(fileName);
        }

        public void Save(Stream stream)
        {
            if (Disabled)
                return;

            preparedPages.Save(stream);
        }

        public void Load()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = Res.Get("FileFilters,PreparedReport");
                if (dialog.ShowDialog() == DialogResult.OK)
                    Load(dialog.FileName);
            }
        }

        public void Load(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                Load(stream);
            }
        }

        public void Load(Stream stream)
        {
            preparedPages.Load(stream);
            UpdatePages();
            First();
        }

        public void SendEmail()
        {
            EmailExport export = new EmailExport(Report);
            export.Account = Config.EmailSettings;
            export.Address = Report.EmailSettings.FirstRecipient;
            export.CC = Report.EmailSettings.CCRecipients;
            export.Subject = Report.EmailSettings.Subject;
            export.MessageBody = Report.EmailSettings.Message;

            if (export.ShowDialog() == DialogResult.OK)
            {
                Config.DoEvent();
                try
                {
                    export.SendEmail(Report);
                }
                catch (Exception e)
                {
                    FRMessageBox.Error(e.Message);
                }
            }
        }

        public void First()
        {
            PageNo = 1;
        }

        public void Prior()
        {
            PageNo--;
        }

        public void Next()
        {
            PageNo++;
        }

        public void Last()
        {
            PageNo = PageCount;
        }

        public void ZoomIn()
        {
            if (Disabled)
                return;

            Zoom += 0.25f;
        }

        public void ZoomOut()
        {
            if (Disabled)
                return;

            Zoom -= 0.25f;
        }

        public void ZoomWholePage()
        {
            if (Disabled)
                return;

            Workspace.ZoomWholePage();
        }

        public void ZoomPageWidth()
        {
            if (Disabled)
                return;

            Workspace.ZoomPageWidth();
        }

        public void Find()
        {
            if (Disabled)
                return;

            PreviewSearchForm form = new PreviewSearchForm();
            form.Owner = preview.FindForm();
            form.PreviewTab = this;
            form.FormClosed += new FormClosedEventHandler(form_FormClosed);
            form.Show();
        }

        public bool Find(string text, bool matchCase, bool wholeWord)
        {
            if (Disabled)
                return false;

            SearchInfo = new SearchInfo(this);
            return SearchInfo.Find(text, matchCase, wholeWord);
        }

        public bool FindNext(string text, bool matchCase, bool wholeWord)
        {
            if (Disabled)
                return false;

            if (SearchInfo != null)
                return SearchInfo.FindNext(text, matchCase, wholeWord);
            return false;
        }

        public bool FindNext()
        {
            if (Disabled)
                return false;

            if (SearchInfo != null)
                return SearchInfo.FindNext();
            return false;
        }

        public void EditPage()
        {
            if (Disabled)
                return;

            using (Report report = new Report())
            {
                ReportPage page = preparedPages.GetPage(PageNo - 1);

                OverlayBand overlay = new OverlayBand();
                overlay.Name = "Overlay";
                overlay.Width = page.WidthInPixels - (page.LeftMargin + page.RightMargin) * Units.Millimeters;
                overlay.Height = page.HeightInPixels - (page.TopMargin + page.BottomMargin) * Units.Millimeters;

                // remove bands, convert them to Text objects if necessary
                ObjectCollection allObjects = page.AllObjects;
                foreach (Base c in allObjects)
                {
                    if (c is BandBase)
                    {
                        BandBase band = c as BandBase;
                        if (band.HasBorder || band.HasFill)
                        {
                            TextObject textObj = new TextObject();
                            textObj.Bounds = band.Bounds;
                            textObj.Border = band.Border.Clone();
                            textObj.Fill = band.Fill.Clone();
                            overlay.Objects.Add(textObj);
                        }

                        for (int i = 0; i < band.Objects.Count; i++)
                        {
                            ReportComponentBase obj = band.Objects[i];
                            if (!(obj is BandBase))
                            {
                                obj.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                obj.Dock = DockStyle.None;
                                obj.Left = obj.AbsLeft;
                                obj.Top = obj.AbsTop;
                                overlay.Objects.Add(obj);
                                i--;
                            }
                        }
                    }
                }

                page.Clear();
                page.Overlay = overlay;
                report.Pages.Add(page);
                page.SetReport(report);
                page.SetRunning(false);

                // temporary change paper width if page has unlimited width
                float currentPaperWidth = page.PaperWidth;
                if (page.UnlimitedWidth)
                {
                    page.PaperWidth = page.UnlimitedWidthValue / Units.Millimeters;
                }

                if (report.DesignPreviewPage())
                {
                    page = report.Pages[0] as ReportPage;

                    // restore paper width if page has unlimited width
                    if (page.UnlimitedWidth)
                    {
                        page.PaperWidth = currentPaperWidth;
                    }

                    preparedPages.ModifyPage(PageNo - 1, page);
                    Refresh();
                }
            }
        }

        public void CopyPage()
        {
            if (!Disabled)
            {
                preparedPages.CopyPage(PageNo - 1);
                UpdatePages();
            }
        }

        public void DeletePage()
        {
            if (!Disabled && preparedPages.Count > 1)
            {
                preparedPages.RemovePage(PageNo - 1);
                UpdatePages();
            }

        }

        public void EditWatermark()
        {
            if (Disabled)
                return;

            ReportPage page = preparedPages.GetPage(PageNo - 1);
            using (WatermarkEditorForm editor = new WatermarkEditorForm())
            {
                editor.Watermark = page.Watermark;
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    if (editor.ApplyToAll)
                    {
                        // get original report page
                        ReportPage originalPage = page.OriginalComponent.OriginalComponent as ReportPage;
                        // no original page - probably we load the existing .fpx file
                        if (originalPage == null)
                        {
                            // apply watermark in a fast way
                            preparedPages.ApplyWatermark(editor.Watermark);
                            Refresh();
                            return;
                        }
                        // update the report template and refresh a report
                        originalPage.Watermark = editor.Watermark.Clone();
                        RefreshReport();
                    }
                    else
                    {
                        page.Watermark = editor.Watermark;
                        preparedPages.ModifyPage(PageNo - 1, page);
                        Refresh();
                    }
                }
            }
        }

        public void PageSetup()
        {
            if (Disabled)
                return;

            using (PreviewPageSetupForm form = new PreviewPageSetupForm())
            {
                ReportPage page = preparedPages.GetPage(PageNo - 1);
                form.Page = page;
                // get original report page
                ReportPage originalPage = page.OriginalComponent.OriginalComponent as ReportPage;
                // no original page - probably we load the existing .fpx file
                if (originalPage == null)
                    form.ApplyToAllEnabled = false;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    // avoid weird visual effects
                    Refresh();

                    if (form.ApplyToAll)
                    {
                        // update the report template and refresh a report
                        originalPage.Landscape = page.Landscape;
                        originalPage.PaperWidth = page.PaperWidth;
                        originalPage.PaperHeight = page.PaperHeight;
                        originalPage.UnlimitedHeight = page.UnlimitedHeight;
                        originalPage.UnlimitedWidth = page.UnlimitedWidth;
                        originalPage.LeftMargin = page.LeftMargin;
                        originalPage.TopMargin = page.TopMargin;
                        originalPage.RightMargin = page.RightMargin;
                        originalPage.BottomMargin = page.BottomMargin;
                        RefreshReport();
                    }
                    else
                    {
                        // update current page only
                        preparedPages.ModifyPage(PageNo - 1, page);
                        UpdatePages();
                    }
                }
            }
        }
        #endregion

        public PreviewTab(PreviewControl preview, Report report, string text, Hyperlink hyperlink)
        {
            this.preview = preview;
            this.report = report;
            if (this.report != null)
                preparedPages = this.report.PreparedPages;
            else
                preparedPages = new PreparedPages(null);

            workspace = new PreviewWorkspace(this);
#if !MONO
            AttachedControl = workspace;
#else
            Controls.Add(workspace);
#endif
            workspace.Dock = DockStyle.Fill;

#if MONO
            Dock = DockStyle.Fill;
#endif
            SetHyperlinkInfo(hyperlink);
            Text = text;
            Zoom = 1;// preview.Zoom;
            Style = preview.UIStyle;
            First();
        }
    }


    internal class SearchInfo
    {
        private PreviewTab previewTab;
        private string text;
        private bool matchCase;
        private bool wholeWord;

        public int pageNo;
        public int objNo;
        public int rangeNo;
        public CharacterRange[] ranges;
        public bool visible;

        public bool Find(string text, bool matchCase, bool wholeWord)
        {
            this.text = text;
            this.matchCase = matchCase;
            this.wholeWord = wholeWord;
            pageNo = previewTab.PageNo;
            rangeNo = -1;
            return FindNext();
        }

        public bool FindNext(string text, bool matchCase, bool wholeWord)
        {
            this.text = text;
            this.matchCase = matchCase;
            this.wholeWord = wholeWord;
            return FindNext();
        }

        public bool FindNext()
        {
            visible = true;
            for (; pageNo <= previewTab.PageCount; pageNo++)
            {
                ReportPage page = previewTab.PreparedPages.GetPage(pageNo - 1);
                ObjectCollection pageObjects = page.AllObjects;
                for (; objNo < pageObjects.Count; objNo++)
                {
                    ISearchable obj = pageObjects[objNo] as ISearchable;
                    if (obj != null)
                    {
                        ranges = obj.SearchText(text, matchCase, wholeWord);
                        if (ranges != null)
                        {
                            rangeNo++;
                            if (rangeNo < ranges.Length)
                            {
                                previewTab.PositionTo(pageNo, (obj as ComponentBase).AbsBounds.Location);
                                previewTab.Refresh();
                                return true;
                            }
                        }
                    }
                    rangeNo = -1;
                }
                objNo = 0;
            }
            pageNo = 1;
            visible = false;
            return false;
        }

        public SearchInfo(PreviewTab tab)
        {
            previewTab = tab;
        }
    }
}
