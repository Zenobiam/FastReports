using FastReport.Cloud.StorageClient;
using FastReport.Cloud.StorageClient.Box;
using FastReport.Cloud.StorageClient.Dropbox;
using FastReport.Cloud.StorageClient.FastCloud;
using FastReport.Cloud.StorageClient.Ftp;
using FastReport.Cloud.StorageClient.GoogleDrive;
using FastReport.Cloud.StorageClient.SkyDrive;
using FastReport.DevComponents;
using FastReport.DevComponents.DotNetBar;
using FastReport.Export;
using FastReport.Export.Email;
using FastReport.Messaging;
using FastReport.Messaging.Xmpp;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Box = FastReport.Cloud.StorageClient.Box;
using GoogleDrive = FastReport.Cloud.StorageClient.GoogleDrive;
using SkyDrive = FastReport.Cloud.StorageClient.SkyDrive;
#if !DEBUG
using FastReport.Forms;
#endif


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
        private Point pageOffset;
        private string saveInitialDirectory;
        private float ratio = 0;
        #endregion

        #region Properties

        /// <summary>
        /// Occurs when current page number is changed.
        /// </summary>
        public event EventHandler PageChanged;

        /// <summary>
        /// Occurs when Print button clicked.
        /// </summary>
        public event EventHandler<PrintEventArgs> OnPrint;

        public class PrintEventArgs : EventArgs
        {
            private PreviewControl preview;

            public PreviewControl Preview 
            {
                get { return preview; }
            }

            public PrintEventArgs(PreviewControl preview)
            {
                this.preview = preview;
            }
        }

        /// <summary>
        /// Occurs when Export button clicked.
        /// </summary>
        public event EventHandler<ExportEventArgs> OnExport;


        public class ExportEventArgs : EventArgs
        {
            private ExportBase export;
            private Report report;

            public Report Report 
            {
                get { return report; }
            }

            public ExportBase Export 
            {
                get { return export; }
            }


            public ExportEventArgs(ExportBase export, Report report)
            {
                this.report = report;
                this.export = export;
            }
        }


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
                toolBar.Visible = value;
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
                statusBar.Visible = value;
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
                splitter.Visible = value;
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
        [DefaultValue(UIStyle.VisualStudio2012Light)]
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
        public Bar ToolBar
        {
            get { return toolBar; }
        }

        /// <summary>
        /// Gets the preview window's statusbar.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Bar StatusBar
        {
            get { return statusBar; }
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

        private void UpdateDeletePageButton()
        {
            // Disable "Delete Page" button if there is only one page in preview.
            if (currentPreview != null && currentPreview.PreparedPages != null && currentPreview.PreparedPages.Count > 0)
            {
                btnDeletePage.Enabled = currentPreview.PreparedPages.Count > 1;
            }
            else
                btnDeletePage.Enabled = false;
        }

        private void CreateList(List<ObjectInfo> list, ButtonItem button, EventHandler handler)
        {
            foreach (ObjectInfo info in list)
            {
                if (info.Object != null && info.Enabled && info.Object.IsSubclassOf(typeof(ExportBase)))
                {
                    ButtonItem item = new ButtonItem("", Res.TryGet(info.Text) + "...");
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
                            item.ImageIndex = info.ImageIndex;
                        }
                    }
                    button.SubItems.Add(item);
                }
            }
        }

        #if !COMMUNITY
        private ButtonItem createButtonItem(ExportsOptions.ExportsTreeNode node)
        {
            ButtonItem item = new ButtonItem("", node.ToString() + (node.ExportType == null ? "" : "..."));
            item.Tag = node.Tag;
            item.Name = node.Name;
            if (node.ImageIndex != -1)
            {
                Bitmap image = Res.GetImage(node.ImageIndex);
                // avoid errors when several preview are used in threads
                lock (image)
                {
                    item.Image = image;
                    item.ImageIndex = node.ImageIndex;
                }
            }
            return item;
        }

        private void CreateExportsList(List<ExportsOptions.ExportsTreeNode> list, ButtonItem button, EventHandler handler)
        {
            foreach (ExportsOptions.ExportsTreeNode node in list)
            {
                if (node.ExportType != null && node.Enabled &&
                    (node.ExportType.IsSubclassOf(typeof(ExportBase)) ||
                    node.ExportType.IsSubclassOf(typeof(CloudStorageClient)) ||
                    node.ExportType.IsSubclassOf(typeof(MessengerBase))))
                {
                    ButtonItem item = createButtonItem(node);
                    item.Click += handler;
                    button.SubItems.Add(item);
                }
                else if (node.Enabled)
                {
                    ButtonItem categoryItem = createButtonItem(node);
                    button.SubItems.Add(categoryItem);
                    CreateExportsList(node.Nodes, categoryItem, handler);
                }
            }
        }

        private void CreateCategoriesList(ButtonItem button, EventHandler handler)
        {
            ButtonItem saveNative = new ButtonItem("Prepared", Res.Get("Preview,SaveNative") + "...");
            saveNative.Click += handler;
            button.SubItems.Add(saveNative);

            ExportsOptions options = ExportsOptions.GetInstance();

            CreateExportsList(options.ExportsMenu, button, handler);
        }
#endif

        private void CreateExportList(ButtonItem button, EventHandler handler)
        {
            List<ObjectInfo> list = new List<ObjectInfo>();
            RegisteredObjects.Objects.EnumItems(list);

            ButtonItem saveNative = new ButtonItem("Prepared", Res.Get("Preview,SaveNative") + "...");
            saveNative.Click += handler;
            button.SubItems.Add(saveNative);

            CreateList(list, button, handler);
        }

        private void CreateCloudList(ButtonItem button, EventHandler handler)
        {
            //List<ObjectInfo> list = new List<ObjectInfo>();
            //RegisteredObjects.Objects.EnumItems(list);
            //bool firstTime = true;

            //ButtonItem menuItem = new ButtonItem("Cloud", Res.Get("Export,ExportGroups,Cloud"));

            //foreach (ObjectInfo info in list)
            //{
            //    if (info.Object != null && info.Enabled && info.Object.IsSubclassOf(typeof(CloudStorageClient)))
            //    {
            //        ButtonItem item = new ButtonItem("", Res.TryGet(info.Text) + "...");
            //        item.Tag = info;
            //        item.Name = info.Object.Name.Replace("StorageClient", "");
            //        item.Click += handler;
            //        if (info.ImageIndex != -1)
            //        {
            //            Bitmap image = Res.GetImage(info.ImageIndex);
            //            // avoid errors when several preview are user in threads
            //            lock (image)
            //            {
            //                item.Image = image;
            //            }
            //        }
            //        if (firstTime)
            //        {
            //            menuItem.BeginGroup = true;
            //            menuItem.Image = Res.GetImage(238);
            //            button.SubItems.Add(menuItem);
            //        }
            //        menuItem.SubItems.Add(item);
            //        firstTime = false;
            //    }
            //}

#if !COMMUNITY
            ExportsOptions options = ExportsOptions.GetInstance();

            if (options.CloudMenu.Enabled)
            {
                ButtonItem categoryItem = createButtonItem(options.CloudMenu);
                categoryItem.BeginGroup = true;
                button.SubItems.Add(categoryItem);

                CreateExportsList(options.CloudMenu.Nodes, categoryItem, handler);
            }
#endif
        }

        private void CreateMessengersList(ButtonItem button, EventHandler handler)
        {
            //List<ObjectInfo> list = new List<ObjectInfo>();
            //RegisteredObjects.Objects.EnumItems(list);
            //bool firstTime = true;

            //ButtonItem menuItem = new ButtonItem("Messengers", Res.Get("Export,ExportGroups,Messengers"));

            //foreach (ObjectInfo info in list)
            //{
            //    if (info.Object != null && info.Enabled && info.Object.IsSubclassOf(typeof(MessengerBase)))
            //    {
            //        ButtonItem item = new ButtonItem("", Res.TryGet(info.Text) + "...");
            //        item.Tag = info;
            //        item.Name = info.Object.Name.Replace("Messenger", "");
            //        item.Click += handler;
            //        if (info.ImageIndex != -1)
            //        {
            //            Bitmap image = Res.GetImage(info.ImageIndex);
            //            // avoid errors when several preview are user in threads
            //            lock (image)
            //            {
            //                item.Image = image;
            //            }
            //        }
            //        if (firstTime)
            //        {
            //            menuItem.BeginGroup = true;
            //            button.SubItems.Add(menuItem);
            //        }
            //        menuItem.SubItems.Add(item);
            //        firstTime = false;
            //    }
            //}
#if !COMMUNITY
            ExportsOptions options = ExportsOptions.GetInstance();

            if (options.MessengerMenu.Enabled)
            {
                ButtonItem categoryItem = createButtonItem(options.MessengerMenu);
                categoryItem.BeginGroup = true;
                button.SubItems.Add(categoryItem);

                CreateExportsList(options.MessengerMenu.Nodes, categoryItem, handler);
            }
#endif
        }

        private void RemoveSubItems(ButtonItem button)
        {
            bool firstTime = true;
            for (int i = 0; i < button.SubItems.Count; i++)
            {
                if (button.SubItems[i].SubItems.Count > 0)
                {
                    RemoveSubItems(button.SubItems[i] as ButtonItem);
                    if (button.SubItems[i].SubItems.Count == 0)
                    {
                        button.SubItems.Remove(button.SubItems[i].Name);
                        --i;
                    }
                    continue;
                }

                string itm_nm = button.SubItems[i].Name;
                if (Enum.IsDefined(typeof(PreviewExports), itm_nm) && exports != PreviewExports.All)
                {
                    if (exports != PreviewExports.None)
                    {
                        foreach (PreviewExports nm in Enum.GetValues(typeof(PreviewExports)))
                        {
                            if (!((exports & nm) == nm) && itm_nm == nm.ToString())
                            {
                                button.SubItems.Remove(button.SubItems[i].Name);
                                i--;
                                break;
                            }
                        }
                    }
                    else
                    {
                        button.SubItems.Remove(button.SubItems[i].Name);
                        i--;
                    }
                }
                else if (Enum.IsDefined(typeof(PreviewClouds), itm_nm) && clouds != PreviewClouds.All)
                {
                    if (clouds != PreviewClouds.None)
                    {
                        foreach (PreviewClouds nm in Enum.GetValues(typeof(PreviewClouds)))
                        {
                            if (!((clouds & nm) == nm) && itm_nm == nm.ToString())
                            {
                                button.SubItems.Remove(button.SubItems[i].Name);
                                i--;
                                break;
                            }
                            else if (firstTime && button.SubItems[i].BeginGroup != true)
                            {
                                button.SubItems[i].BeginGroup = true;
                                firstTime = false;
                            }
                        }
                    }
                    else
                    {
                        button.SubItems.Remove(button.SubItems[i].Name);
                        i--;
                    }
                }
            }
        }

        private void UpdateButtons()
        {
#if COMMUNITY
            btnPrint.Visible = false;
#else
            btnPrint.Visible = (Buttons & PreviewButtons.Print) != 0;
#endif
            btnOpen.Visible = (Buttons & PreviewButtons.Open) != 0;
            btnSave.Visible = (Buttons & PreviewButtons.Save) != 0;

#if COMMUNITY
            btnEmail.Visible = false;
            btnEmailMapi.Visible = false;
#else
            btnEmail.Visible = (Buttons & PreviewButtons.Email) != 0 && !Config.EmailSettings.UseMAPI;
            btnEmailMapi.Visible = (Buttons & PreviewButtons.Email) != 0 && Config.EmailSettings.UseMAPI;
#endif

            btnFind.Visible = (Buttons & PreviewButtons.Find) != 0;

            btnOutline.Visible = (Buttons & PreviewButtons.Outline) != 0;
            btnPageSetup.Visible = (Buttons & PreviewButtons.PageSetup) != 0;

            btnDesign.Visible = (Buttons & PreviewButtons.Design) != 0;
            btnEdit.Visible = (Buttons & PreviewButtons.Edit) != 0;
            btnCopyPage.Visible = (Buttons & PreviewButtons.CopyPage) != 0;
            btnDeletePage.Visible = (Buttons & PreviewButtons.DeletePage) != 0;

            btnWatermark.Visible = (Buttons & PreviewButtons.Watermark) != 0;

            btnFirst.Visible = (Buttons & PreviewButtons.Navigator) != 0;
            btnPrior.Visible = (Buttons & PreviewButtons.Navigator) != 0;
            tbPageNo.Visible = (Buttons & PreviewButtons.Navigator) != 0;
            lblTotalPages.Visible = (Buttons & PreviewButtons.Navigator) != 0;
            btnNext.Visible = (Buttons & PreviewButtons.Navigator) != 0;
            btnLast.Visible = (Buttons & PreviewButtons.Navigator) != 0;

            btnClose.Visible = (Buttons & PreviewButtons.Close) != 0;

            btnAbout.Visible = (Buttons & PreviewButtons.About) != 0;

            if (!(exports == PreviewExports.All && clouds == PreviewClouds.All))
            {
                RemoveSubItems(btnSave);
            }
        }

        private void Export_Click(object sender, EventArgs e)
        {
            if (IsPreviewEmpty)
                return;

            ObjectInfo info = (sender as ButtonItem).Tag as ObjectInfo;
            if (info == null)
                Save();
            else
            {
                ExportBase export = Activator.CreateInstance(info.Object) as ExportBase;
                export.CurPage = CurrentPreview.PageNo;
                export.AllowSaveSettings = true;
                export.ShowProgress = true;
                if (!string.IsNullOrEmpty(SaveInitialDirectory))
                    export.SaveInitialDirectory = SaveInitialDirectory;
                try
                {
                    if (OnExport != null)
                        OnExport(sender, new ExportEventArgs(export, CurrentPreview.Report));
                    else
                        export.Export(CurrentPreview.Report);
                }
#if !DEBUG
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

        private void SaveToCloud_Click(object sender, EventArgs e)
        {
            if (IsPreviewEmpty)
            {
                return;
            }

            ObjectInfo info = (sender as ButtonItem).Tag as ObjectInfo;
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
                        if (clientInfoDialog.DialogResult != DialogResult.OK)
                            return;
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

        private void SendViaMessenger_Click(object sender, EventArgs e)
        {
            if (IsPreviewEmpty)
            {
                return;
            }

            ObjectInfo info = (sender as ButtonItem).Tag as ObjectInfo;
            if (info != null)
            {
                MessengerBase messenger = Activator.CreateInstance(info.Object) as MessengerBase;
                if (messenger is XmppMessenger)
                {
                    XmlItem xi = Config.Root.FindItem("XmppMessenger").FindItem("MessengerSettings");
                    string jidFrom = xi.GetProp("JidFrom");
                    string password = xi.GetProp("Password");
                    string jidTo = xi.GetProp("JidTo");
                    XmppMessengerForm form = new XmppMessengerForm(jidFrom, password, jidTo, Report);
                    form.ShowDialog();
                }
                else
                {
                    Type telegramMessenger = RegisteredObjects.FindType("TelegramMessenger");
                    if (telegramMessenger != null && messenger.GetType() == telegramMessenger)
                        try
                        {
                            MethodInfo InvokeInfoFormEditor = telegramMessenger.GetMethod("InvokeForm");
                            object[] parametrs = new object[1];
                            parametrs[0] = Report;
                            object o = Activator.CreateInstance(telegramMessenger);
                            InvokeInfoFormEditor.Invoke(o, parametrs);
                        }
                        catch (Exception ex)
                        {
                            FRMessageBox.Error(ex.Message);
                        }
                }
            }
        }

        private void Email_Click(object sender, EventArgs e)
        {
            if (IsPreviewEmpty)
                return;

            List<string> fileNames = new List<string>();
            ObjectInfo info = (sender as ButtonItem).Tag as ObjectInfo;

            if (info == null)
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = Res.Get("FileFilters,PreparedReport");
                    dialog.DefaultExt = "*.fpx";
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        Save(dialog.FileName);
                        fileNames.Add(dialog.FileName);
                    }
                }
            }
            else
            {
                ExportBase export = Activator.CreateInstance(info.Object) as ExportBase;
                export.CurPage = CurrentPreview.PageNo;
                export.AllowOpenAfter = false;
                export.ShowProgress = true;
                export.Export(CurrentPreview.Report);
                fileNames = export.GeneratedFiles;
            }

            if (fileNames.Count > 0)
            {
                Form form = FindForm();
                string[] recipientAddresses = CurrentPreview.Report.EmailSettings.Recipients == null ?
                  new string[] { } : CurrentPreview.Report.EmailSettings.Recipients;
                int error = MAPI.SendMail(form == null ? IntPtr.Zero : form.Handle, fileNames.ToArray(),
                  CurrentPreview.Report.EmailSettings.Subject,
                  CurrentPreview.Report.EmailSettings.Message, new string[] { }, recipientAddresses);
                if (error > 1)
                {
                    MessageBox.Show("MAPISendMail failed! " + MAPI.GetErrorText(error));
                }
            }
        }

        private void FUpdateTimer_Tick(object sender, EventArgs e)
        {
            updatingZoom = true;

            int zoom = (int)(zoomToUpdate * 100);
            slZoom.Text = zoom.ToString() + "%";
            if (zoom < 100)
                zoom = (int)Math.Round((zoom - 25) / 0.75f);
            else if (zoom > 100)
                zoom = (zoom - 100) / 4 + 100;
            slZoom.Value = zoom;

            updatingZoom = false;
            updateTimer.Stop();
        }

        private void tabControl1_TabItemClose(object sender, TabStripActionEventArgs e)
        {
            DeleteTab(currentPreview);
            e.Cancel = true;
            tabControl.RecalcLayout();
        }

        private void tabControl1_SelectedTabChanged(object sender, FastReport.DevComponents.DotNetBar.TabStripTabChangedEventArgs e)
        {
            if (locked)
                return;
            currentPreview = tabControl.SelectedTab as PreviewTab;
            if (currentPreview != null && !currentPreview.Fake)
            {
                currentPreview.BindPreparedPages();
                UpdateOutline();
                UpdateZoom(currentPreview.Zoom / DpiHelper.Multiplier);
                UpdatePageNumbers(currentPreview.PageNo, currentPreview.PageCount);
                UpdateDeletePageButton();
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

        private void CalcMult()
        {
            ratio = DpiHelper.GetMultiplierForScreen(Screen.FromControl(this));
        }

        private void SetToolTip(BaseItem item, string tttext)
        {
            item.Tooltip = tttext;

            if (ratio == 0)
                return;
            item.toolTipFont = new Font(SystemFonts.DefaultFont.Name, SystemFonts.DefaultFont.Size * ratio / DrawUtils.ScreenDpiFX, FontStyle.Regular);
        }

        private void Localize()
        {
            MyRes res = new MyRes("Preview");
            btnDesign.Text = res.Get("DesignText");
            SetToolTip(btnDesign, res.Get("Design"));
            btnPrint.Text = res.Get("PrintText");
            SetToolTip(btnPrint, res.Get("Print"));
            SetToolTip(btnOpen, res.Get("Open"));
            SetToolTip(btnSave, res.Get("Save"));
            btnSave.Text = res.Get("SaveText");
            SetToolTip(btnEmail, res.Get("Email"));
            SetToolTip(btnEmailMapi, res.Get("Email"));
            SetToolTip(btnFind, res.Get("Find"));
            SetToolTip(btnOutline, res.Get("Outline"));
            SetToolTip(btnPageSetup, res.Get("PageSetup"));
            SetToolTip(btnEdit, res.Get("Edit"));
            SetToolTip(btnCopyPage, res.Get("CopyPage"));
            SetToolTip(btnDeletePage, res.Get("DeletePage"));
            SetToolTip(btnWatermark, res.Get("Watermark"));
            SetToolTip(btnFirst, res.Get("First"));
            SetToolTip(btnPrior, res.Get("Prior"));
            SetToolTip(btnNext, res.Get("Next"));
            lblTotalPages.Text = String.Format(Res.Get("Misc,ofM"), 1);
            SetToolTip(btnLast, res.Get("Last"));
            btnClose.Text = Res.Get("Buttons,Close");
            btnAbout.Text = new MyRes("Designer,Menu").Get("Help,About");

            btnDesign.Image = Res.GetImage(68);
            btnPrint.Image = Res.GetImage(195);
            btnOpen.Image = Res.GetImage(1);
            btnSave.Image = Res.GetImage(2);
            btnEmail.Image = Res.GetImage(200);
            btnEmailMapi.Image = Res.GetImage(200);
            btnFind.Image = Res.GetImage(181);
            btnOutline.Image = Res.GetImage(196);
            btnPageSetup.Image = Res.GetImage(13);
            btnEdit.Image = Res.GetImage(198);
            btnCopyPage.Image = Res.GetImage(6);
            btnDeletePage.Image = Res.GetImage(12);
            btnWatermark.Image = Res.GetImage(194);
            btnFirst.Image = Res.GetImage(185);
            btnPrior.Image = Res.GetImage(186);
            btnNext.Image = Res.GetImage(187);
            btnLast.Image = Res.GetImage(188);
            btnZoomPageWidth.Image = ResourceLoader.GetBitmap("ZoomPageWidth.png");
            btnZoomWholePage.Image = ResourceLoader.GetBitmap("ZoomWholePage.png");
            btnZoom100.Image = ResourceLoader.GetBitmap("Zoom100.png");
            toolBar.UpdateDpiDependencies();
            statusBar.UpdateDpiDependencies();
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
            Font = DpiHelper.ConvertUnits(DrawUtils.Default96Font);
            toolBar.Font = Font;
            statusBar.Font = Font;
#if !COMMUNITY
            CreateCategoriesList(btnSave, new EventHandler(Export_Click));
#endif
            CreateExportList(btnEmailMapi, new EventHandler(Email_Click));
            CreateCloudList(btnSave, new EventHandler(SaveToCloud_Click));
            CreateMessengersList(btnSave, new EventHandler(SendViaMessenger_Click));
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
                outlineControl.Width = int.Parse(width);
        }

        private void SaveState()
        {
            // Clear();
            outlineControl.Hide();

            XmlItem xi = Config.Root.FindItem("Preview");
            xi.SetProp("Zoom", Converter.ToString(Zoom));
            xi.SetProp("OutlineWidth", outlineControl.Width.ToString());
        }

        private void UpdateUIStyle()
        {
            switch (UIStyle)
            {
                case UIStyle.Office2003:
                case UIStyle.Office2007Blue:
                case UIStyle.Office2010Blue:
                    StyleManager.ChangeStyle(eStyle.Office2010Blue, Color.Empty);
                    break;
                case UIStyle.Office2007Silver:
                case UIStyle.Office2010Silver:
                    StyleManager.ChangeStyle(eStyle.Office2010Silver, Color.Empty);
                    break;
                case UIStyle.Office2007Black:
                case UIStyle.Office2010Black:
                    StyleManager.ChangeStyle(eStyle.Office2010Black, Color.Empty);
                    break;
                case UIStyle.Office2013:
                    StyleManager.ChangeStyle(eStyle.Office2013, Color.Empty);
                    break;
                case UIStyle.VisualStudio2005:
                case UIStyle.VisualStudio2010:
                    StyleManager.ChangeStyle(eStyle.VisualStudio2010Blue, Color.Empty);
                    break;
                case UIStyle.VisualStudio2012Light:
                    StyleManager.ChangeStyle(eStyle.VisualStudio2012Light, Color.Empty);
                    break;
                case UIStyle.VistaGlass:
                    StyleManager.ChangeStyle(eStyle.Windows7Blue, Color.Empty);
                    break;
            }

            //UIStyleUtils.UpdateUIStyle();

            eDotNetBarStyle style = UIStyleUtils.GetDotNetBarStyle(UIStyle);
            toolBar.Style = style;
            statusBar.Style = style;
            tabControl.Style = UIStyleUtils.GetTabStripStyle(UIStyle);
            outlineControl.Style = UIStyle;

            foreach (PreviewTab tab in documents)
            {
                tab.Style = UIStyle;
            }
        }

        private void UpdateOutline()
        {
            try
            {
                outlineControl.PreparedPages = currentPreview.PreparedPages;
                OutlineVisible = !currentPreview.PreparedPages.Outline.IsEmpty;
            }
            catch
            {
            }
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
            tabControl.TabsVisible = documents.Count > 1 && !documents[0].Fake;
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

#if !MONO
        internal void UpdateDpiDependecies(float ratio)
        {
            CalcMult();
            Localize();
            Font = DpiHelper.ConvertUnits(DrawUtils.Default96Font);
            toolBar.Font = Font;
            statusBar.Font = Font;
            UpdateImages();
            toolBar.UpdateDpiDependencies();
        }

        internal void UpdateImages()
        {
            btnDesign.Image = Res.GetImage(68, ratio);
            btnPrint.Image = Res.GetImage(195, ratio);
            btnOpen.Image = Res.GetImage(1, ratio);
            btnSave.Image = Res.GetImage(2, ratio);
            btnEmail.Image = Res.GetImage(200, ratio);
            btnEmailMapi.Image = Res.GetImage(200, ratio);
            btnFind.Image = Res.GetImage(181, ratio);
            btnOutline.Image = Res.GetImage(196, ratio);
            btnPageSetup.Image = Res.GetImage(13, ratio);
            btnEdit.Image = Res.GetImage(198, ratio);
            btnCopyPage.Image = Res.GetImage(6, ratio);
            btnDeletePage.Image = Res.GetImage(12, ratio);
            btnWatermark.Image = Res.GetImage(194, ratio);
            btnFirst.Image = Res.GetImage(185, ratio);
            btnPrior.Image = Res.GetImage(186, ratio);
            btnNext.Image = Res.GetImage(187, ratio);
            btnLast.Image = Res.GetImage(188, ratio);
            foreach (BaseItem item in btnSave.SubItems)
                UpdateImageInSaveBtn(item);
            btnSave.UpdateDpiDependencies();
        }

        private void UpdateImageInSaveBtn(BaseItem item)
        {
            if (item is ButtonItem)
                if ((item as ButtonItem).ImageIndex != -1)
                    (item as ButtonItem).Image = Res.GetImage((item as ButtonItem).ImageIndex, ratio);
                else
                    (item as ButtonItem).SubItemsImageSize = new Size((int)(16 * ratio), (int)(16 * ratio));
            foreach (BaseItem subitem in item.SubItems)
                UpdateImageInSaveBtn(subitem);
        }
#endif

        internal void UpdatePageNumbers(int pageNo, int totalPages)
        {
            lblStatus.Text = String.Format(Res.Get("Misc,PageNofM"), pageNo, totalPages);
            tbPageNo.Text = pageNo.ToString();
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
                tabControl.TabStrip.AutoSelectAttachedControl = false;
                tabControl.SelectedTab = tab;
                tabControl.TabStrip.AutoSelectAttachedControl = true;
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
            AddTab(report, text, true);
        }

        /// <summary>
        /// Adds a new report tab to the preview control.
        /// </summary>
        /// <param name="report">The <b>Report</b> object that contains the prepared report.</param>
        /// <param name="text">The title for the new tab.</param>
        /// <param name="setActive">If <b>true</b>, makes the new tab active.</param>
        /// <remarks>
        /// Prepare the report using its <b>Prepare</b> method before you pass it to the <b>report</b> parameter.
        /// </remarks>
        public void AddTab(Report report, string text, bool setActive)
        {
            if (this.report == null)
                SetReport(report);
            AddPreviewTab(report, text, null, setActive);
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
            statusBar.Refresh();
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
            if (OnPrint != null)
                OnPrint(sender, new PrintEventArgs(this));
            else
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

        private void slZoom_ValueChanged(object sender, EventArgs e)
        {
            if (updatingZoom)
                return;

            int val = slZoom.Value;
            if (val < 100)
                val = (int)Math.Round(val * 0.75f) + 25;
            else
                val = (val - 100) * 4 + 100;

            Zoom = val / 100f;
            slZoom.Text = val.ToString() + "%";
        }

        private void btnZoomPageWidth_Click(object sender, EventArgs e)
        {
            ZoomPageWidth();
        }

        private void btnZoomWholePage_Click(object sender, EventArgs e)
        {
            ZoomWholePage();
        }

        private void btnZoom100_Click(object sender, EventArgs e)
        {
            Zoom = 1;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditPage();
        }

        private void btnCopyPage_Click(object sender, EventArgs e)
        {
            CopyPage();
        }

        private void btnDeletePage_Click(object sender, EventArgs e)
        {
            DeletePage();
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

        private void tbPageNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    PageNo = int.Parse(tbPageNo.Text);
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

        private void tbPageNo_Click(object sender, EventArgs e)
        {
            tbPageNo.SelectAll();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            using (Forms.AboutForm aboutForm = new Forms.AboutForm())
            {
                aboutForm.ShowDialog(this);
            }
        }

        private void ScaleHighDpi()
        {
            slZoom.Width = DpiHelper.ConvertUnits(slZoom.Width);
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
            if (!string.IsNullOrEmpty(SaveInitialDirectory))
                CurrentPreview.SaveInitialDirectory = SaveInitialDirectory;
            CurrentPreview.Save();
        }

        /// <summary>
        /// Saves the current report to a specified .fpx file.
        /// </summary>
        public void Save(string fileName)
        {
            if (CurrentPreview == null)
                return;
            if (!string.IsNullOrEmpty(SaveInitialDirectory))
                CurrentPreview.SaveInitialDirectory = SaveInitialDirectory;
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
            UpdateDeletePageButton();
        }

        /// <summary>
        /// Loads the report from a .fpx file using the "Open File" dialog.
        /// </summary>
        public new void Load()
        {
            if (!PreLoad())
                return;
            CurrentPreview.Load();
            PostLoad();
        }

        /// <summary>
        /// Loads the report from a specified .fpx file.
        /// </summary>
        public new void Load(string fileName)
        {
            if (!PreLoad())
                return;
            CurrentPreview.Load(fileName);
            PostLoad();
        }

        /// <summary>
        /// Load the report from a stream.
        /// </summary>
        /// <param name="stream">The stream to load from.</param>
        public new void Load(Stream stream)
        {
            if (!PreLoad())
                return;
            CurrentPreview.Load(stream);
            PostLoad();
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
            if (CurrentPreview == null)
                return;
            CurrentPreview.EditPage();
        }

        /// <summary>
        /// Copies the current page in preview.
        /// </summary>
        public void CopyPage()
        {
            if (CurrentPreview != null)
            {
                CurrentPreview.CopyPage();
                UpdateDeletePageButton();
            }
        }

        /// <summary>
        /// Removes the current page in preview.
        /// </summary>
        public void DeletePage()
        {
            if (CurrentPreview != null)
            {
                CurrentPreview.DeletePage();
                UpdateDeletePageButton();
            }
        }

        /// <summary>
        /// Edits the current report in the designer.
        /// </summary>
        public void Design()
        {
            if (Report == null)
                return;
            using (Report designedReport = new Report())
            {
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

                if (designedReport.Design())
                {
                    Report.PreparedPages.Clear();
                    Report.PreparedPages.ClearPageCache();
                    Report.LoadFromString(designedReport.SaveToString());
                    if (CurrentPreview != null)
                    {
                        Report.Preview = CurrentPreview.Preview;
                        Report.Show();
                    }
                }
            }
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
            tbPageNo.Text = "";
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
            BarUtilities.UseTextRenderer = true;
            documents = new List<PreviewTab>();
            InitializeComponent();
            ScaleHighDpi();
            toolbarVisible = true;
            statusbarVisible = true;
            OutlineVisible = false;
            UIStyle = Config.UIStyle;
            Localize();
            Init();
            AddFakeTab();
            RightToLeft = Config.RightToLeft ? RightToLeft.Yes : RightToLeft.No;
        }
    }
}
