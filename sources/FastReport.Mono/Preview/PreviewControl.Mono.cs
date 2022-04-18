using FastReport.Cloud.StorageClient;
using FastReport.Cloud.StorageClient.Box;
using FastReport.Cloud.StorageClient.Dropbox;
using FastReport.Cloud.StorageClient.FastCloud;
using FastReport.Cloud.StorageClient.Ftp;
using FastReport.Cloud.StorageClient.GoogleDrive;
using FastReport.Cloud.StorageClient.SkyDrive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Design;
using FastReport.Utils;
using FastReport.Export;
using FastReport.TypeEditors;
using FastReport.Forms;
using FastReport.Export.Email;
using FastReport.Controls;

using Box = FastReport.Cloud.StorageClient.Box;
using GoogleDrive = FastReport.Cloud.StorageClient.GoogleDrive;
using SkyDrive = FastReport.Cloud.StorageClient.SkyDrive;
using FastReport.Messaging;

namespace FastReport.Preview
{
    /// <summary>
    /// Represents a Windows Forms control used to preview a report.
    /// </summary>
    /// <remarks>
    /// To use this control, place it on a form and link it to a report using the report's
    /// <see cref="FastReport.Report.Preview"/> property. To show a report, call 
    /// the <b>Report.Show</b> method:
    /// <code>
    /// report1.Preview = previewControl1;
    /// report1.Show();
    /// </code>
    /// <para>Use this control's methods such as <see cref="Print"/>, <see cref="Save()"/> etc. to
    /// handle the preview. Call <see cref="Clear"/> method to clear the preview.</para>
    /// <para>You can specify whether the standard toolbar is visible in the <see cref="ToolbarVisible"/>
    /// property. The <see cref="StatusbarVisible"/> property allows you to hide/show the statusbar.
    /// </para>
    /// </remarks>
    [ToolboxItem(true), ToolboxBitmap(typeof(Report), "Resources.PreviewControl.bmp")]
    public partial class PreviewControl : UserControl
    {
        #region Fields
        private Report report;
        private List<PreviewTab> documents;
        private bool toolbarVisible;
        private bool statusbarVisible;
        private Color pageBorderColor;
        private Color activePageBorderColor;
        private PreviewButtons buttons;
        private PreviewExports exports;
        private PreviewClouds clouds;
        private bool updatingZoom;
        private Timer updateTimer;
        private float zoomToUpdate;
        private float defaultZoom;
        private bool locked;
        private PreviewTab currentPreview;
        private bool fastScrolling;
        private UIStyle uiStyle;
        private bool useBackColor;
        private FRTabControl tabControl;
        private Point pageOffset;
        private string saveInitialDirectory;
        #endregion

        #region Properties
        /// <summary>
        /// Occurs when current page number is changed.
        /// </summary>
        public event EventHandler PageChanged;

        /// <summary>
        /// Gets a reference to the report.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Report Report
        {
            get { return report; }
        }

        /// <summary>
        /// Obsolete. Gets or sets the color of page border.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Color PageBorderColor
        {
            get { return pageBorderColor; }
            set { pageBorderColor = value; }
        }

        /// <summary>
        /// Gets or sets the color of active page border.
        /// </summary>
        [DefaultValue(typeof(Color), "255, 199, 60")]
        public Color ActivePageBorderColor
        {
            get { return activePageBorderColor; }
            set { activePageBorderColor = value; }
        }

        /// <summary>
        /// Gets or sets the first page offset from the top left corner of the control.
        /// </summary>
        public Point PageOffset
        {
            get { return pageOffset; }
            set { pageOffset = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the toolbar is visible.
        /// </summary>
        [DefaultValue(true)]
        public bool ToolbarVisible
        {
            get { return toolbarVisible; }
            set
            {
                toolbarVisible = value;
                toolStrip.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the statusbar is visible.
        /// </summary>
        [DefaultValue(true)]
        public bool StatusbarVisible
        {
            get { return statusbarVisible; }
            set
            {
                statusbarVisible = value;
                statusStrip.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the outline control is visible.
        /// </summary>
        [DefaultValue(false)]
        public bool OutlineVisible
        {
            get { return outlineControl.Visible; }
            set
            {
                splitContainer1.Panel1Collapsed = !value;
                outlineControl.Visible = value;
                btnOutline.Checked = value;
            }
        }

        /// <summary>
        /// Specifies the set of buttons available in the toolbar.
        /// </summary>
        [DefaultValue(PreviewButtons.All)]
        public PreviewButtons Buttons
        {
            get { return buttons; }
            set
            {
                buttons = value;
                UpdateButtons();
            }
        }

        /// <summary>
        /// Specifies the set of exports that will be available in the preview's "save" menu.
        /// </summary>
        [DefaultValue(PreviewExports.All)]
        public PreviewExports Exports
        {
            get { return exports; }
            set
            {
                exports = value;
                UpdateButtons();
            }
        }

        /// <summary>
        /// Specifies the set of exports in clouds that will be available in the preview's "save" menu.
        /// </summary>
        [DefaultValue(PreviewClouds.All)]
        public PreviewClouds Clouds
        {
            get { return clouds; }
            set
            {
                clouds = value;
                UpdateButtons();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the fast scrolling method should be used.
        /// </summary>
        /// <remarks>
        /// If you enable this property, the gradient background will be disabled.
        /// </remarks>
        [DefaultValue(false)]
        public bool FastScrolling
        {
            get { return fastScrolling; }
            set { fastScrolling = value; }
        }

        /// <summary>
        /// Gets or sets the visual style.
        /// </summary>
        [DefaultValue(UIStyle.Office2007Blue)]
        public UIStyle UIStyle
        {
            get { return uiStyle; }
            set
            {
                uiStyle = value;
                UpdateUIStyle();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating that the BackColor property must be used to draw the background area.
        /// </summary>
        /// <remarks>
        /// By default, the background area is drawn using the color defined in the current <b>UIStyle</b>.
        /// </remarks>
        [DefaultValue(false)]
        public bool UseBackColor
        {
            get { return useBackColor; }
            set
            {
                useBackColor = value;
                UpdateUIStyle();
            }
        }

        /// <summary>
        /// Gets the preview window's toolbar.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolStrip ToolBar
        {
            get { return toolStrip; }
        }

        /// <summary>
        /// Gets the preview window's statusbar.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StatusStrip StatusBar
        {
            get { return statusStrip; }
        }

        /// <summary>
        /// Gets or sets the initial directory that is displayed by a save file dialog.
        /// </summary>
        public string SaveInitialDirectory
        {
            get { return saveInitialDirectory; }
            set { saveInitialDirectory = value; }
        }

        internal float DefaultZoom
        {
            get { return defaultZoom; }
        }

        internal PreviewTab CurrentPreview
        {
            get { return currentPreview; }
        }

        private bool IsPreviewEmpty
        {
            get { return CurrentPreview == null || CurrentPreview.Disabled; }
        }
        #endregion

        #region Private Methods
        private void CreateExportList(ToolStripMenuItem button, EventHandler handler)
        {
            List<ObjectInfo> list = new List<ObjectInfo>();
            RegisteredObjects.Objects.EnumItems(list);
            CreateList(list, button, handler);
        }

        private void CreateCategoriesList(ToolStripMenuItem button, EventHandler handler)
        {
            ToolStripMenuItem saveNative = new ToolStripMenuItem(Res.Get("Preview,SaveNative") + "...");
            saveNative.Click += handler;
            button.DropDownItems.Add(saveNative);

            ExportsOptions options = ExportsOptions.GetInstance();
            CreateExportsList(options.ExportsMenu, button, handler);
        }
        
        private void CreateExportsList(List<ExportsOptions.ExportsTreeNode> list, ToolStripMenuItem button, EventHandler handler)
        {
            foreach (ExportsOptions.ExportsTreeNode node in list)
            {
                if (node.ExportType != null && node.Enabled &&
                    (node.ExportType.IsSubclassOf(typeof(ExportBase)) ||
                    node.ExportType.IsSubclassOf(typeof(CloudStorageClient)) ||
                    node.ExportType.IsSubclassOf(typeof(MessengerBase))))
                {
                    ToolStripMenuItem item = createButtonItem(node);
                    item.Click += handler;
                    item.Font = DrawUtils.DefaultFont; // workaround Mono behavior
                    button.DropDownItems.Add(item);
                }
                else if (node.Enabled)
                {
                    ToolStripMenuItem categoryItem = createButtonItem(node);
                    button.DropDownItems.Add(categoryItem);
                    CreateExportsList(node.Nodes, categoryItem, handler);
                }
            }
        }
        
        private ToolStripMenuItem createButtonItem(ExportsOptions.ExportsTreeNode node)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(node.ToString() + (node.ExportType == null ? "" : "..."));
            item.Tag = node.Tag;
            item.Name = node.Name;
            if (node.ImageIndex != -1)
            {
                Bitmap image = Res.GetImage(node.ImageIndex);
                // avoid errors when several preview are used in threads
                lock (image)
                {
                    item.Image = image;
                }
            }
            return item;
        }

        private void CreateList(List<ObjectInfo> list, ToolStripMenuItem button, EventHandler handler)
        {
            foreach (ObjectInfo info in list)
            {
                if (info.Object != null && info.Enabled && info.Object.IsSubclassOf(typeof(ExportBase)))
                {
                    ToolStripDropDownButton item = new ToolStripDropDownButton(Res.TryGet(info.Text) + "...");
                    item.Tag = info;
                    item.Name = info.Object.Name;
                    item.Click += handler;
                    if (info.ImageIndex != -1)
                    {
                        Bitmap image = Res.GetImage(info.ImageIndex);
                        // avoid errors when several preview are used in threads
                        lock (image)
                        {
                            item.Image = image;
                        }
                    }
                    button.DropDownItems.Add(item);
                }
            }
        }
        
        private void CreateCloudList(ToolStripMenuItem button, EventHandler handler)
        {
            ExportsOptions options = ExportsOptions.GetInstance();

            if (options.CloudMenu.Enabled)
            {
                ToolStripMenuItem categoryItem = createButtonItem(options.CloudMenu);
                //categoryItem.BeginGroup = true;
                button.DropDownItems.Add(categoryItem);

                CreateExportsList(options.CloudMenu.Nodes, categoryItem, handler);
            }
        }

        private void UpdateButtons()
        {
            btnPrint.Available = (Buttons & PreviewButtons.Print) != 0;
            btnOpen.Available = (Buttons & PreviewButtons.Open) != 0;
            btnSave.Available = (Buttons & PreviewButtons.Save) != 0;
            btnDesign.Available = (Buttons & PreviewButtons.Design) != 0;
#if Basic
      btnEmail.Available = false;
      btnEmailMapi.Available = false;
#else
            btnEmail.Available = (Buttons & PreviewButtons.Email) != 0 && !Config.EmailSettings.UseMAPI;
#endif
            btnFind.Available = (Buttons & PreviewButtons.Find) != 0;

            btnOutline.Available = (Buttons & PreviewButtons.Outline) != 0;
            btnPageSetup.Available = (Buttons & PreviewButtons.PageSetup) != 0;
#if Basic
      btnEdit.Available = false;
#else
            btnEdit.Available = (Buttons & PreviewButtons.Edit) != 0;
#endif
            btnWatermark.Available = (Buttons & PreviewButtons.Watermark) != 0;

            btnFirst.Available = (Buttons & PreviewButtons.Navigator) != 0;
            btnPrior.Available = (Buttons & PreviewButtons.Navigator) != 0;
            cbxPageNo.Available = (Buttons & PreviewButtons.Navigator) != 0;
            lblTotalPages.Available = (Buttons & PreviewButtons.Navigator) != 0;
            btnNext.Available = (Buttons & PreviewButtons.Navigator) != 0;
            btnLast.Available = (Buttons & PreviewButtons.Navigator) != 0;
            btnClose.Available = (Buttons & PreviewButtons.Close) != 0;
        }

        private void Export_Click(object sender, EventArgs e)
        {
            if (IsPreviewEmpty)
                return;

            ObjectInfo info = (sender as ToolStripMenuItem).Tag as ObjectInfo;
            if (info == null)
                Save();
            else
            {
                ExportBase export = Activator.CreateInstance(info.Object) as ExportBase;
                export.CurPage = CurrentPreview.PageNo;
                export.AllowSaveSettings = true;
                export.ShowProgress = true;
                try
                {
                    export.Export(Report);
                }
#if! DEBUG
        catch (Exception ex)
        {
          using (ExceptionForm form = new ExceptionForm(ex))
          {
            form.ShowDialog();
          }
        }
#endif
                finally
                {
                }
            }
        }
        private void Design()
        {
            if (Report == null)
            {
                return;
            }

            using (Report designedReport = new Report())
            {
                Report.PreparedPages.Clear();
                Report.PreparedPages.ClearPageCache();

                designedReport.FileName = Report.FileName;
                if (!String.IsNullOrEmpty(Report.FileName))
                    designedReport.Load(designedReport.FileName);
                else
                    using (MemoryStream repStream = new MemoryStream())
                    {
                        Report.Save(repStream);
                        repStream.Position = 0;
                        designedReport.Load(repStream);
                    }

                Report.Dictionary.ReRegisterData(designedReport.Dictionary);

                //string s = designedReport.SaveToString();
                //File.WriteAllText(@"c:\1\rep1.frx", s);
                //designedReport.LoadFromString(s);

                if (designedReport.Design())
                {
                    Report.LoadFromString(designedReport.SaveToString());
                    Report.PreparedPages.Clear();
                    if (CurrentPreview != null)
                    {
                        Report.Preview = CurrentPreview.Preview;
                        Report.Show();
                    }
                }
            }
        }

        private void SaveToCloud_Click(object sender, EventArgs e)
        {
            if (IsPreviewEmpty)
            {
                return;
            }

            ObjectInfo info = (sender as ToolStripMenuItem).Tag as ObjectInfo;
            if (info != null)
            {
                CloudStorageClient client = Activator.CreateInstance(info.Object) as CloudStorageClient;
                if (client is FtpStorageClient)
                {
                    XmlItem xi = Config.Root.FindItem("FtpServer").FindItem("StorageSettings");
                    string server = xi.GetProp("FtpServer");
                    string username = xi.GetProp("FtpUsername");
                    FtpStorageClientForm form = new FtpStorageClientForm(server, username, "", Report);
                    form.ShowDialog();
                }
                else if (client is BoxStorageClient)
                {
                    XmlItem xi = Config.Root.FindItem("BoxCloud").FindItem("StorageSettings");
                    string id = xi.GetProp("ClientId");
                    string secret = xi.GetProp("ClientSecret");
                    if (String.IsNullOrEmpty(id) || String.IsNullOrEmpty(secret))
                    {
                        Box.ClientInfoForm clientInfoDialog = new Box.ClientInfoForm();
                        clientInfoDialog.ShowDialog();
                        id = clientInfoDialog.Id;
                        secret = clientInfoDialog.Secret;
                    }
                    BoxStorageClientForm form = new BoxStorageClientForm(new SkyDrive.ClientInfo("", id, secret), Report);
                    form.ShowDialog();
                }
                else if (client is DropboxStorageClient)
                {
                    XmlItem xi = Config.Root.FindItem("DropboxCloud").FindItem("StorageSettings");
                    string accessToken = xi.GetProp("AccessToken");
                    if (String.IsNullOrEmpty(accessToken))
                    {
                        ApplicationInfoForm appInfoDialog = new ApplicationInfoForm();
                        appInfoDialog.ShowDialog();
                        accessToken = appInfoDialog.AccessToken;
                    }
                    DropboxStorageClientForm form = new DropboxStorageClientForm(accessToken, Report);
                    form.ShowDialog();
                }
                else if (client is FastCloudStorageClient)
                {
                    FastCloudStorageClientSimpleForm form = new FastCloudStorageClientSimpleForm(Report);
                    form.ShowDialog();
                }
                else if (client is GoogleDriveStorageClient)
                {
                    XmlItem xi = Config.Root.FindItem("GoogleDriveCloud").FindItem("StorageSettings");
                    string id = xi.GetProp("ClientId");
                    string secret = xi.GetProp("ClientSecret");
                    if (String.IsNullOrEmpty(id) || String.IsNullOrEmpty(secret))
                    {
                        GoogleDrive.ClientInfoForm clientInfoDialog = new GoogleDrive.ClientInfoForm();
                        clientInfoDialog.ShowDialog();
                        id = clientInfoDialog.Id;
                        secret = clientInfoDialog.Secret;
                    }
                    GoogleDriveStorageClientForm form = new GoogleDriveStorageClientForm(new SkyDrive.ClientInfo("", id, secret), Report);
                    form.ShowDialog();
                }
                else if (client is SkyDriveStorageClient)
                {
                    XmlItem xi = Config.Root.FindItem("SkyDriveCloud").FindItem("StorageSettings");
                    string id = xi.GetProp("ClientId");
                    string secret = xi.GetProp("ClientSecret");
                    if (String.IsNullOrEmpty(id) || String.IsNullOrEmpty(secret))
                    {
                        SkyDrive.ClientInfoForm appInfoDialog = new SkyDrive.ClientInfoForm();
                        appInfoDialog.ShowDialog();
                        id = appInfoDialog.Id;
                        secret = appInfoDialog.Secret;
                    }
                    SkyDriveStorageClientForm form = new SkyDriveStorageClientForm(new SkyDrive.ClientInfo("", id, secret), Report);
                    form.ShowDialog();
                }
            }
        }

        private void FUpdateTimer_Tick(object sender, EventArgs e)
        {
            updatingZoom = true;

            string value = ((int)(zoomToUpdate * 100)).ToString() + "%";
            cbxZoom.Text = value;

            updatingZoom = false;
            updateTimer.Stop();
        }

        private void tabControl1_TabClosed(object sender, EventArgs e)
        {
            DeleteTab(currentPreview);
        }

        private void tabControl1_TabChanged(object sender, EventArgs e)
        {
            if (locked)
                return;
            currentPreview = tabControl.SelectedTab as PreviewTab;
            if (currentPreview != null && !currentPreview.Fake)
            {
                UpdateOutline();
                UpdateZoom(currentPreview.Zoom);
                UpdatePageNumbers(currentPreview.PageNo, currentPreview.PageCount);
            }
        }

        private void tabControl_Resize(object sender, EventArgs e)
        {
            foreach (PreviewTab tab in documents)
            {
                tab.UpdatePages();
            }
        }

        private bool CanDisposeTabReport(PreviewTab tab)
        {
            if (tab == null || tab.Report == null)
                return false;

            // if the preview is owned by Report, do not dispose
            if (report == tab.Report)
                return false;

            // check if the same Report is used in other tabs
            foreach (PreviewTab t in documents)
            {
                if (t != tab && t.Report == tab.Report)
                {
                    return false;
                }
            }
            return true;
        }

        private void Localize()
        {
            MyRes res = new MyRes("Preview");
            btnPrint.Text = res.Get("PrintText");
            btnPrint.ToolTipText = res.Get("Print");
            btnOpen.ToolTipText = res.Get("Open");
            btnDesign.Text = res.Get("Design");
            btnDesign.ToolTipText = res.Get("Design");
            btnSave.ToolTipText = res.Get("Save");
            btnSave.Text = res.Get("SaveText");
            btnEmail.ToolTipText = res.Get("Email");
            btnFind.ToolTipText = res.Get("Find");
            btnZoomOut.ToolTipText = Res.Get("Designer,Toolbar,Zoom,ZoomOut");
            cbxZoom.Items.AddRange(new string[] {
        "25%", "50%", "75%", "100%", "150%", "200%", 
        Res.Get("Designer,Toolbar,Zoom,PageWidth"), 
        Res.Get("Designer,Toolbar,Zoom,WholePage") });
            btnZoomIn.ToolTipText = Res.Get("Designer,Toolbar,Zoom,ZoomIn");
            btnOutline.ToolTipText = res.Get("Outline");
            btnPageSetup.ToolTipText = res.Get("PageSetup");
            btnEdit.ToolTipText = res.Get("Edit");
            btnWatermark.ToolTipText = res.Get("Watermark");
            btnFirst.ToolTipText = res.Get("First");
            btnPrior.ToolTipText = res.Get("Prior");
            btnNext.ToolTipText = res.Get("Next");
            lblTotalPages.Text = String.Format(Res.Get("Misc,ofM"), 1);
            btnLast.ToolTipText = res.Get("Last");
            btnClose.Text = Res.Get("Buttons,Close");

            btnPrint.Image = Res.GetImage(195);
            btnOpen.Image = Res.GetImage(1);
            btnSave.Image = Res.GetImage(2);
            btnDesign.Image = Res.GetImage(68);
            btnEmail.Image = Res.GetImage(200);
            btnZoomIn.Image = Res.GetImage(192);
            btnZoomOut.Image = Res.GetImage(193);
            btnFind.Image = Res.GetImage(181);
            btnOutline.Image = Res.GetImage(196);
            btnPageSetup.Image = Res.GetImage(13);
            btnEdit.Image = Res.GetImage(198);
            btnWatermark.Image = Res.GetImage(194);
            btnFirst.Image = Res.GetImage(185);
            btnPrior.Image = Res.GetImage(186);
            btnNext.Image = Res.GetImage(187);
            btnLast.Image = Res.GetImage(188);
        }

        private void Init()
        {
            outlineControl.SetPreview(this);
            updateTimer = new Timer();
            updateTimer.Interval = 50;
            updateTimer.Tick += new EventHandler(FUpdateTimer_Tick);
            pageBorderColor = Color.FromArgb(80, 80, 80);
            activePageBorderColor = Color.FromArgb(255, 199, 60);
            pageOffset = new Point(10, 10);
            defaultZoom = 1;
            buttons = PreviewButtons.All;
            exports = PreviewExports.All;
            clouds = PreviewClouds.All;
            Font = DrawUtils.Default96Font;
            toolStrip.Font = Font;
            statusStrip.Font = Font;
            cbxZoom.Font = Font;
            cbxPageNo.Font = Font;
            CreateCategoriesList(btnSave, new EventHandler(Export_Click));
            //CreateExportList(btnEmail, new EventHandler(btnEmail_Click));
            CreateCloudList(btnSave, new EventHandler(SaveToCloud_Click));
            RestoreState();
            UpdateButtons();
        }

        private void RestoreState()
        {
            XmlItem xi = Config.Root.FindItem("Preview");

            string zoom = xi.GetProp("Zoom");
            if (!String.IsNullOrEmpty(zoom))
                defaultZoom = (float)Converter.FromString(typeof(float), zoom);

            string width = xi.GetProp("OutlineWidth");
            if (!String.IsNullOrEmpty(width))
                splitContainer1.SplitterDistance = int.Parse(width);
        }

        private void SaveState()
        {
            Clear();
            outlineControl.Hide();

            XmlItem xi = Config.Root.FindItem("Preview");
            xi.SetProp("Zoom", Converter.ToString(Zoom));
            xi.SetProp("OutlineWidth", splitContainer1.SplitterDistance.ToString());
        }

        private void UpdateUIStyle()
        {
            //TODO: �� ������ 
            toolStrip.Renderer = statusStrip.Renderer = UIStyleUtils.GetToolStripRenderer(UIStyle);
            if (!UseBackColor)
                BackColor = UIStyleUtils.GetColorTable(UIStyle).WorkspaceBackColor;
            tabControl.Style = UIStyle;

            foreach (PreviewTab tab in documents)
            {
                tab.Style = UIStyle;
            }
        }

        private void UpdateOutline()
        {
            outlineControl.PreparedPages = currentPreview.PreparedPages;
            OutlineVisible = !currentPreview.PreparedPages.Outline.IsEmpty;
        }

        private void AddFakeTab()
        {
            PreviewTab tab = new PreviewTab(this, null, "", null);
            tab.Fake = true;
            documents.Add(tab);
            tab.AddToTabControl(tabControl);
        }

        private void UpdateTabsVisible()
        {
            tabControl.ShowTabs = documents.Count > 1 && !documents[0].Fake;
        }

        private PreviewTab FindTab(string text)
        {
            foreach (PreviewTab tab in documents)
            {
                if (tab.Text == text)
                    return tab;
            }

            return null;
        }

        private PreviewTab FindTabByHyperlinkValue(string value)
        {
            foreach (PreviewTab tab in documents)
            {
                if (tab.HyperlinkValue == value)
                    return tab;
            }

            return null;
        }
        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
                updateTimer.Dispose();
                SaveState();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Public Methods
        internal void SetReport(Report report)
        {
            this.report = report;
        }

        internal void UpdatePageNumbers(int pageNo, int totalPages)
        {
            //TODO: cbxPageNo.Text = pageNo.ToString();
            lblStatus.Text = String.Format(Res.Get("Misc,PageNofM"), pageNo, totalPages);
            cbxPageNo.Text = pageNo.ToString();
            lblTotalPages.Text = String.Format(Res.Get("Misc,ofM"), totalPages);
            if (PageChanged != null)
                PageChanged(this, EventArgs.Empty);
        }

        internal void UpdateZoom(float zoom)
        {
            zoomToUpdate = zoom;
            updateTimer.Start();
        }

        internal void UpdateUrl(string url)
        {
            lblUrl.Text = url;
        }

        internal void ShowPerformance(string text)
        {
            lblPerformance.Text = text;
        }

        internal void DoClick()
        {
            OnClick(EventArgs.Empty);
        }

        // Clears all tabs except the first one. This method is used in the report.Prepare.
        // It is needed to avoid flickering when using stand-alone PreviewControl. 
        // When report is prepared and ShowPrepared method is called, the "fake" tab will
        // be replaced with the new tab.
        internal void ClearTabsExceptFirst()
        {
            while (documents.Count > 1)
            {
                DeleteTab(documents[documents.Count - 1]);
            }
            if (documents.Count == 1)
                documents[0].Fake = true;
        }

        internal PreviewTab AddPreviewTab(Report report, string text, Hyperlink hyperlink, bool setActive)
        {
            PreviewTab tab = new PreviewTab(this, report, text, hyperlink);
            documents.Add(tab);
            //report.PreparedPages.ClearPageCache();
            //OutlineVisible = !report.PreparedPages.Outline.IsEmpty;
            tab.AddToTabControl(tabControl);
            tab.UnlockLayout();
            UpdateTabsVisible();
            tab.UpdatePages();

            if (setActive)
            {
                // do not stole the focus
                //                tabControl.TabStrip.AutoSelectAttachedControl = false;
                tabControl.SelectedTab = tab;
                //                tabControl.TabStrip.AutoSelectAttachedControl = true;
            }
            else
                tabControl.Refresh();

            if (documents.Count == 2 && documents[0].Fake)
                DeleteTab(documents[0]);
            return tab;
        }

        /// <summary>
        /// Adds a new report tab to the preview control.
        /// </summary>
        /// <param name="report">The <b>Report</b> object that contains the prepared report.</param>
        /// <param name="text">The title for the new tab.</param>
        /// <remarks>
        /// Prepare the report using its <b>Prepare</b> method before you pass it to the <b>report</b> parameter.
        /// </remarks>
        public void AddTab(Report report, string text)
        {
            if (this.report == null)
                SetReport(report);
            AddPreviewTab(report, text, null, true);
        }

        /// <summary>
        /// Switches to the tab with specified text.
        /// </summary>
        /// <param name="text">Text of the tab.</param>
        /// <returns><b>true</b> if the tab with specified text exists, or <b>false</b> if there is no such tab.</returns>
        public bool SwitchToTab(string text)
        {
            PreviewTab tab = FindTab(text);
            if (tab != null)
            {
                tabControl.SelectedTab = tab;
                return true;
            }

            return false;
        }

        internal bool SwitchToTab(Hyperlink hyperlink)
        {
            PreviewTab tab = FindTabByHyperlinkValue(hyperlink.Value);
            if (tab != null)
            {
                tabControl.SelectedTab = tab;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Deletes the report tab with specified text.
        /// </summary>
        /// <param name="text">The text of the tab.</param>
        public void DeleteTab(string text)
        {
            PreviewTab tab = FindTab(text);
            if (tab != null)
                DeleteTab(tab);
        }

        /// <summary>
        /// Checks if the tab with specified text exists.
        /// </summary>
        /// <param name="text">The text of the tab.</param>
        /// <returns><b>true</b> if the tab exists.</returns>
        public bool TabExists(string text)
        {
            return FindTab(text) != null;
        }

        internal void DeleteTab(PreviewTab tab)
        {
            if (CanDisposeTabReport(tab))
                tab.Report.Dispose();
            documents.Remove(tab);
            tabControl.Tabs.Remove(tab);
            tab.Dispose();
            UpdateTabsVisible();
        }

        /// <summary>
        /// Displays the text in the status bar.
        /// </summary>
        /// <param name="text">Text to display.</param>
        public void ShowStatus(string text)
        {
            lblStatus.Text = text;
            statusStrip.Refresh();
        }

        internal void Lock()
        {
            locked = true;
        }

        internal void Unlock()
        {
            locked = false;
        }

        /// <summary>
        /// Sets the focus to the preview control.
        /// </summary>
        public new void Focus()
        {
            if (currentPreview != null)
                currentPreview.Focus();
        }
        #endregion

        #region Event handlers

        private void btnDesign_Click(object sender, EventArgs e)
        {
            Design();
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            Print();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            Load();
        }

        private void btnEmail_Click(object sender, EventArgs e)
        {
            SendEmail();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            Find();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            ZoomOut();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }

        private void cbxZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingZoom)
                return;
            if (cbxZoom.SelectedIndex == cbxZoom.Items.Count - 2)
                ZoomPageWidth();
            else if (cbxZoom.SelectedIndex == cbxZoom.Items.Count - 1)
                ZoomWholePage();
            else
                Zoom = Converter.StringToFloat(cbxZoom.Text, true) / 100;
            CurrentPreview.Focus();
        }

        private void cbxPageNo_Click(object sender, EventArgs e)
        {
            cbxPageNo.Items.Clear();
            for (int i = 1; i <= PageCount; i++)
            {
                cbxPageNo.Items.Add(i);
            }
        }

        private void cbxZoom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Zoom = Converter.StringToFloat(cbxZoom.Text, true) / 100;
                CurrentPreview.Focus();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditPage();
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            First();
        }

        private void btnPrior_Click(object sender, EventArgs e)
        {
            Prior();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            Next();
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            Last();
        }

        private void cbxPageNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    PageNo = int.Parse(cbxPageNo.Text);
                }
                catch
                {
                    PageNo = PageCount;
                }
                CurrentPreview.Focus();
            }
        }

        private void tbPageNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b' && (e.KeyChar < '0' || e.KeyChar > '9'))
                e.Handled = true;
        }

        private void btnWatermark_Click(object sender, EventArgs e)
        {
            EditWatermark();
        }

        private void btnOutline_Click(object sender, EventArgs e)
        {
            OutlineVisible = btnOutline.Checked;
        }

        private void btnPageSetup_Click(object sender, EventArgs e)
        {
            PageSetup();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (FindForm() != null)
                FindForm().Close();
        }

        private void cbxPageNo_TextChanged(object sender, EventArgs e)
        {
            var a = sender as ToolStripItem;

            if (cbxPageNo.SelectedIndex >= 0 && a.Text != string.Empty)
            {
                int pagecbx = int.Parse(a.Text);
                PageNo = pagecbx;
            }

            //TODO: cbxPageNo.SelectAll();
            //cbxPageNo.SelectAll();
        }
        #endregion

        #region Preview commands
        /// <summary>
        /// Prints the current report.
        /// </summary>
        /// <returns><b>true</b> if report was printed; <b>false</b> if user cancels the "Print" dialog.</returns>
        public bool Print()
        {
            if (CurrentPreview == null)
                return false;
            return CurrentPreview.Print();
        }

        /// <summary>
        /// Saves the current report to a .fpx file using the "Save FIle" dialog.
        /// </summary>
        public void Save()
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.Save();
        }

        /// <summary>
        /// Saves the current report to a specified .fpx file.
        /// </summary>
        public void Save(string fileName)
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.Save(fileName);
        }

        /// <summary>
        /// Saves the current report to a stream.
        /// </summary>
        public void Save(Stream stream)
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.Save(stream);
        }

        private bool PreLoad()
        {
            if (CurrentPreview == null)
                return false;
            if (documents.Count == 1 && documents[0].Fake)
            {
                Report report = new Report();
                report.SetPreparedPages(new PreparedPages(report));
                AddTab(report, "");
            }
            return true;
        }

        private void PostLoad()
        {
            UpdateOutline();
        }

        /// <summary>
        /// Loads the report from a .fpx file using the "Open File" dialog.
        /// </summary>
        public new void Load()
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.Load();
            UpdateOutline();
        }

        /// <summary>
        /// Loads the report from a specified .fpx file.
        /// </summary>
        public new void Load(string fileName)
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.Load(fileName);
            UpdateOutline();
        }

        /// <summary>
        /// Load the report from a stream.
        /// </summary>
        /// <param name="stream">The stream to load from.</param>
        public new void Load(Stream stream)
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.Load(stream);
            UpdateOutline();
        }

        /// <summary>
        /// Sends an email.
        /// </summary>
        public void SendEmail()
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.SendEmail();
        }

        /// <summary>
        /// Finds the text in the current report using the "Find Text" dialog.
        /// </summary>
        public void Find()
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.Find();
        }

        /// <summary>
        /// Finds the specified text in the current report.
        /// </summary>
        /// <param name="text">Text to find.</param>
        /// <param name="matchCase">A value indicating whether the search is case-sensitive.</param>
        /// <param name="wholeWord">A value indicating whether the search matches whole words only.</param>
        /// <returns><b>true</b> if text found.</returns>
        public bool Find(string text, bool matchCase, bool wholeWord)
        {
            if (CurrentPreview == null)
                return false;
            return CurrentPreview.Find(text, matchCase, wholeWord);
        }

        /// <summary>
        /// Finds the next occurence of text specified in the <b>Find</b> method.
        /// </summary>
        /// <returns><b>true</b> if text found.</returns>
        public bool FindNext()
        {
            if (CurrentPreview == null)
                return false;
            return CurrentPreview.FindNext();
        }

        /// <summary>
        /// Navigates to the first page.
        /// </summary>
        public void First()
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.First();
        }

        /// <summary>
        /// Navigates to the previuos page.
        /// </summary>
        public void Prior()
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.Prior();
        }

        /// <summary>
        /// Navigates to the next page.
        /// </summary>
        public void Next()
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.Next();
        }

        /// <summary>
        /// Navigates to the last page.
        /// </summary>
        public void Last()
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.Last();
        }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        /// <remarks>
        /// This value is 1-based.
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PageNo
        {
            get
            {
                if (CurrentPreview == null)
                    return 1;
                return CurrentPreview.PageNo;
            }
            set
            {
                if (CurrentPreview == null)
                    return;
                CurrentPreview.PageNo = value;
            }
        }

        /// <summary>
        /// Gets the pages count in the current report.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PageCount
        {
            get
            {
                if (CurrentPreview == null)
                    return 0;
                return CurrentPreview.PageCount;
            }
        }

        /// <summary>
        /// Gets or sets the zoom factor.
        /// </summary>
        /// <remarks>
        /// <b>1</b> corresponds to 100% zoom.
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Zoom
        {
            get
            {
                if (CurrentPreview == null)
                    return 1;
                return CurrentPreview.Zoom;
            }
            set
            {
                if (CurrentPreview != null)
                    CurrentPreview.Zoom = value;
            }
        }

        /// <summary>
        /// Zooms in.
        /// </summary>
        public void ZoomIn()
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.ZoomIn();
        }

        /// <summary>
        /// Zooms out.
        /// </summary>
        public void ZoomOut()
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.ZoomOut();
        }

        /// <summary>
        /// Zooms to fit the page width.
        /// </summary>
        public void ZoomPageWidth()
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.ZoomPageWidth();
        }

        /// <summary>
        /// Zooms to fit the whole page.
        /// </summary>
        public void ZoomWholePage()
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.ZoomWholePage();
        }

        /// <summary>
        /// Edits the current page in the designer.
        /// </summary>
        public void EditPage()
        {
#if Basic
      throw new Exception("The FastReport.Net Basic edition does not support this feature.");
#else
            if (CurrentPreview == null)
                return;
            CurrentPreview.EditPage();
#endif
        }

        /// <summary>
        /// Edits the watermark.
        /// </summary>
        public void EditWatermark()
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.EditWatermark();
        }

        /// <summary>
        /// Edits the page settings.
        /// </summary>
        public void PageSetup()
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.PageSetup();
        }

        /// <summary>
        /// Navigates to the specified position inside a specified page.
        /// </summary>
        /// <param name="pageNo">The page number (1-based).</param>
        /// <param name="point">The position inside a page, in pixels.</param>
        public void PositionTo(int pageNo, PointF point)
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.PositionTo(pageNo, point);
        }

        /// <summary>
        /// Clears the preview.
        /// </summary>
        public void Clear()
        {
            while (documents.Count > 0)
            {
                DeleteTab(documents[0]);
            }

            lblStatus.Text = "";
            cbxPageNo.Text = "";
        }

        /// <summary>
        /// Refresh the report.
        /// </summary>
        public void RefreshReport()
        {
            if (CurrentPreview == null)
                return;
            CurrentPreview.RefreshReport();
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewControl"/> class.
        /// </summary>
        public PreviewControl()
        {
            // we need this to ensure that static constructor of the Report was called.
            Report report = new Report();
            report.Dispose();

            documents = new List<PreviewTab>();
            InitializeComponent();

            tabControl = new FRTabControl();
            tabControl.Parent = splitContainer1.Panel2;
            tabControl.ShowCaption = false;
            tabControl.TabOrientation = TabOrientation.Top;
            tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            tabControl.Name = "tabControl";
            tabControl.Resize += new System.EventHandler(this.tabControl_Resize);
            tabControl.SelectedTabChanged += new EventHandler(this.tabControl1_TabChanged);
            tabControl.TabClosed += new EventHandler(this.tabControl1_TabClosed);

            // mono fix
            //btnOpen.AutoSize = false;
            //btnOpen.Size = new Size(23, 22);

            toolbarVisible = true;
            statusbarVisible = true;
            OutlineVisible = false;
            UIStyle = Config.UIStyle;
            Localize();
            Init();
            AddFakeTab();
        }
    }
}
