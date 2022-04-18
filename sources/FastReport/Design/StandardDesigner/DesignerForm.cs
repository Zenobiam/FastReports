using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using FastReport.Controls;
using FastReport.Forms;
using FastReport.Utils;
using FastReport.Design.PageDesigners.Page;
using FastReport.Design.ToolWindows;
using FastReport.Design.Toolbars;
using FastReport.DevComponents.DotNetBar;
using FastReport.DevComponents;

namespace FastReport.Design.StandardDesigner
{
    /// <summary>
    /// Represents standard designer's form.
    /// </summary>
    /// <remarks>
    /// This form contains the <see cref="DesignerControl"/>. Use the <see cref="Designer"/> 
    /// property to get access to this control.
    /// <para/>Usually you don't need to create an instance of this class. The designer can be called
    /// using the <see cref="FastReport.Report.Design()"/> method of 
    /// the <see cref="FastReport.Report"/> instance.
    /// <para/>If you decided to use this class, you need:
    /// <list type="bullet">
    ///   <item>
    ///     <description>create an instance of this class;</description>
    ///   </item>
    ///   <item>
    ///     <description>set the <b>Designer.Report</b> property to report that you need to design;</description>
    ///   </item>
    ///   <item>
    ///     <description>call either <b>ShowModal</b> or <b>Show</b> methods to display a form.</description>
    ///   </item>
    /// </list>
    /// </remarks>
    public partial class DesignerForm : Form, IDesignerPlugin
    {
        int newdpi = 0;
        bool isSclaing = false;

        #region Vars
        /// <summary>
        /// Gets a reference to the <see cref="Designer"/> control which is actually a designer.
        /// </summary>
        public DesignerControl Designer
        {
            get { return designer; }
        }

        /// <summary>
        /// Gets a list of File menu buttons
        /// </summary>
        public Dictionary<string, ButtonItem> Items
        {
            get
            {
                if (items == null)
                {
                    items = new Dictionary<string, ButtonItem>();
                    items.Add("btnFileNew", this.btnFileNew);
                    items.Add("btnFileOpen", this.btnFileOpen);
                    items.Add("btnFileSave", this.btnFileSave);
                    items.Add("btnFileSaveAs", this.btnFileSaveAs);
                    items.Add("btnFileSaveAll", this.btnFileSaveAll);
                    items.Add("btnFilePreview", this.btnFilePreview);
                    items.Add("btnFilePrinterSetup", this.btnFilePrinterSetup);
                    items.Add("btnFileSelectLanguage", this.btnFileSelectLanguage);
                    items.Add("btnWelcome", this.btnWelcome);
                    items.Add("btnHelp", this.btnHelp);
                    items.Add("btnAbout", this.btnAbout);
                    items.Add("btnFileClose", this.btnFileClose);
                }

                return items;
            }
        }

        private float Zoom
        {
            get { return ReportWorkspace.Scale; }
            set
            {
                if (Workspace != null)
                    Workspace.Zoom(value);
            }
        }

        private ReportWorkspace Workspace
        {
            get
            {
                if (designer.ActiveReportTab != null && designer.ActiveReportTab.ActivePageDesigner is ReportPageDesigner)
                    return (Designer.ActiveReportTab.ActivePageDesigner as ReportPageDesigner).Workspace;
                return null;
            }
        }

        private ReportPageDesigner ReportPageDesigner
        {
            get
            {
                //Fixed try catch 66fcd219-30f1-45e6-8ee5-ce65cfb9d35d
                //rly annoying freezes on designer start
                ReportTab tab = Designer.ActiveReportTab;
                if (tab != null)
                    return tab.ActivePageDesigner as ReportPageDesigner;
                return null;
            }
        }

        private PageBase Page
        {
            get
            {
                //Fixed try catch 66fcd219-30f1-45e6-8ee5-ce65cfb9d35d
                //rly annoying freezes on designer start
                ReportPageDesigner d = ReportPageDesigner;
                if (d != null)
                    return d.Page;
                return null;
            }
        }

        private DesignerControl designer;
        private Dictionary<string, ButtonItem> items;
        private Timer clipboardTimer;
        private Timer previewTimer;
        #endregion

        /// <summary>
        /// Creates a new instance of the <see cref="DesignerForm"/> class with default settings.
        /// </summary>
        public DesignerForm()
        {
            InitDesigner();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DesignerForm"/> class with default settings.
        /// </summary>
        /// <param name="welcome">enables welcome window</param>
        public DesignerForm(bool welcome)
        {
            InitDesigner();
            if (welcome)
                Shown += ShowWelcomeWindow;
        }

        private void InitDesigner()
        {
#if !MONO && !FRCORE
            //if (!Config.DisableHighDpi)
                UIStyleUtils.EnableHighDpi();
#endif
            RightToLeft = Config.RightToLeft ? RightToLeft.Yes : RightToLeft.No;
            InitializeComponent();
            if(Config.RightToLeft)
            {
                btnFind.ImagePosition = eImagePosition.Right;
                btnSelectAll.ImagePosition = eImagePosition.Right;
            }

            Config.MainForm = this;

            Font = DpiHelper.ConvertUnits(DrawUtils.DefaultFont, true);
            Icon = Config.DesignerSettings.Icon;

            designer = new DesignerControl(location, size, text);

            this.ribbonControl.AllowDrop = true;
            this.AllowDrop = true;

            Controls.Add(designer);
            designer.Dock = DockStyle.Fill;
            designer.BringToFront();
            designer.Plugins.Add(this);
            designer.ShowStatusBar = false;

            setupStatusBar();
            SetupControls();

            Localize();

            KeyPreview = true;
            KeyDown += DesignerForm_KeyDown;
            designer.UIStyle = Config.UIStyle;
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            if(!isSclaing && DpiHelper.HighDpiEnabled)
            {
                isSclaing = true;
                Scaleprop();
                GC.Collect();
                isSclaing = false;
            }
        }

        private void Scaleprop()
        {
            if (newdpi == 0)
                newdpi = DpiHelper.BaseDpi;
            if (!DpiHelper.ReInit(newdpi, this))
                return;
            Res.UpdateResourcres();
            RegisteredObjects.UpdateItemsImages();
            Config.MainForm = this;
            Designer.ReinitDpiSize();
            foreach (IDesignerPlugin plugin in designer.ActiveReportTab.Plugins)
                plugin.ReinitDpiSize();

            Workspace?.Refresh();
            ScaleRibbonControl();

            this.location.Width = DpiHelper.ConvertUnits(160);
            this.size.Width = DpiHelper.ConvertUnits(160);
            //DpiHelper.SetProcessDpiAwareness(Process_DPI_Awareness.Process_Per_Monitor_DPI_Aware);
        }

        private void ScaleRibbonControl()
        {
            // update designer initializing

            Bitmap cap = DpiHelper.ConvertBitmap(new Bitmap(32, 32));

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DesignerForm));
            this.ribbonControl.Font = DpiHelper.ConvertUnits(DrawUtils.DefaultFont, true);
            this.ribbonControl.ReinitDpiSize();
            this.ribbonControl.Size = DpiHelper.ConvertUnits((new Size(969, 140)));
            this.ribbonPanel1.Padding = DpiHelper.ConvertUnits(new System.Windows.Forms.Padding(3, 0, 3, 2));
            this.btnPreview.SubItemsExpandWidth = DpiHelper.ConvertUnits(14);
            this.btnReportOptions.SubItemsExpandWidth = DpiHelper.ConvertUnits(14);
            this.btnPanels.SubItemsExpandWidth = DpiHelper.ConvertUnits(14);
            this.ribbonPanel3.Padding = DpiHelper.ConvertUnits(new System.Windows.Forms.Padding(3, 0, 3, 2));
            UpdateButton(btnFile, DpiHelper.ConvertBitmap((Bitmap)resources.GetObject("btnFile.Image")));
            this.btnFile.ImageFixedSize = DpiHelper.ConvertUnits(new Size(16, 16));
            this.btnFile.ImagePaddingVertical = DpiHelper.ConvertUnits(1);
            UpdateButton(btnFileNew, DpiHelper.ConvertBitmap((Bitmap)resources.GetObject("btnFileNew.Image")));
            UpdateButton(btnFileOpen, DpiHelper.ConvertBitmap((Bitmap)resources.GetObject("btnFileOpen.Image")));
            UpdateButton(btnFileSave, DpiHelper.ConvertBitmap((Bitmap)resources.GetObject("btnFileSave.Image")));
            UpdateButton(btnFileSaveAs, DpiHelper.ConvertBitmap((Bitmap)resources.GetObject("btnFileSaveAs.Image")));
            UpdateButton(btnFileSaveAll, DpiHelper.ConvertBitmap((Bitmap)resources.GetObject("btnFileSaveAll.Image")));
            UpdateButton(btnFilePreview, DpiHelper.ConvertBitmap((Bitmap)resources.GetObject("btnFilePreview.Image")));
            UpdateButton(btnFilePrinterSetup, DpiHelper.ConvertBitmap((Bitmap)resources.GetObject("btnFilePrinterSetup.Image")));
            UpdateButton(btnFileClose, DpiHelper.ConvertBitmap((Bitmap)resources.GetObject("btnFileClose.Image")));
            this.cbxStyles.ComboWidth = DpiHelper.ConvertUnits(100, false);
            this.cbxStyles.DropDownHeight = DpiHelper.ConvertUnits(300);
            this.cbxStyles.DropDownWidth = DpiHelper.ConvertUnits(150);
            this.cbxStyles.ItemHeight = DpiHelper.ConvertUnits(14);
            this.cbxFontName.DropDownWidth = DpiHelper.ConvertUnits(370);
            this.cbxFontName.ItemHeight = DpiHelper.ConvertUnits(14);
            this.cbxFontName.ComboWidth = DpiHelper.ConvertUnits(103);
            this.cbxFontName.DropDownHeight = DpiHelper.ConvertUnits(300);
            this.cbxFontSize.ComboWidth = DpiHelper.ConvertUnits(50);
            this.cbxFontSize.DropDownHeight = DpiHelper.ConvertUnits(300);
            this.cbxFontSize.ItemHeight = DpiHelper.ConvertUnits(14);
            UpdateButton(btnTextColor, DpiHelper.ConvertBitmap((Bitmap)resources.GetObject("btnTextColor.Image")));
            UpdateButton(btnOptions, DpiHelper.ConvertBitmap((Bitmap)resources.GetObject("btnOptions.Image")));
            UpdateButton(btnFileExit, DpiHelper.ConvertBitmap((Bitmap)resources.GetObject("btnFileExit.Image")));
            UpdateButton(btnPreview, ResourceLoader.GetBitmap("buttons.report.png"));

            UpdateButton(btnFilePreview, cap);
            UpdateButton(btnWelcome, cap);
            UpdateButton(btnHelp, cap);
            UpdateButton(btnAbout, cap);
            UpdateButton(btnFileSelectLanguage, cap);

            //foreach (ButtonItem item in itemContainer3.SubItems)
            //    item.UpdateDpiDependencies();

            //update images
            UpdateButton(btnUndo, Res.GetImage(8));
            UpdateButton(btnRedo, Res.GetImage(9));
            UpdateButton(btnCut, Res.GetImage(5));
            UpdateButton(btnCopy, Res.GetImage(6));
            UpdateButton(btnPaste, ResourceLoader.GetBitmap("buttons.007.png"));
            UpdateButton(btnFormatPainter, Res.GetImage(18));
            UpdateButton(btnTextColor, Res.GetImage(23));
            UpdateButton(btnBold, Res.GetImage(20));
            UpdateButton(btnItalic, Res.GetImage(21));
            UpdateButton(btnUnderline, Res.GetImage(22));
            UpdateButton(btnAlignLeft, Res.GetImage(25));
            UpdateButton(btnAlignCenter, Res.GetImage(26));
            UpdateButton(btnAlignRight, Res.GetImage(27));
            UpdateButton(btnJustify, Res.GetImage(28));
            UpdateButton(btnAlignTop, Res.GetImage(29));
            UpdateButton(btnAlignMiddle, Res.GetImage(30));
            UpdateButton(btnAlignBottom, Res.GetImage(31));
            UpdateButton(btnTextRotation, Res.GetImage(64));

            UpdateButton(btnTopLine, Res.GetImage(32));
            UpdateButton(btnBottomLine, Res.GetImage(33));
            UpdateButton(btnLeftLine, Res.GetImage(34));
            UpdateButton(btnRightLine, Res.GetImage(35));
            UpdateButton(btnAllLines, Res.GetImage(36));
            UpdateButton(btnNoLines, Res.GetImage(37));

            UpdateButton(btnFillColor, Res.GetImage(38));
            UpdateButton(btnFillProps, Res.GetImage(141));

            UpdateButton(btnLineColor, Res.GetImage(39));

            UpdateButton(btnLineWidth, Res.GetImage(71));

            UpdateButton(btnLineStyle, Res.GetImage(85));

            UpdateButton(btnBorderProps, Res.GetImage(40));

            //-------------------------------------------------------------------
            // Format
            //-------------------------------------------------------------------

            UpdateButton(btnHighlight, ResourceLoader.GetBitmap("buttons.024.png"));
            UpdateButton(btnFormat, ResourceLoader.GetBitmap("buttons.019.png"));

            //-------------------------------------------------------------------
            // Styles
            //-------------------------------------------------------------------

            UpdateButton(btnStyles, Res.GetImage(87));

            //-------------------------------------------------------------------
            // Editing
            //-------------------------------------------------------------------

            UpdateButton(btnFind, Res.GetImage(181));
            UpdateButton(btnReplace, ResourceLoader.GetBitmap("buttons.069.png"));
            UpdateButton(btnSelectAll, ResourceLoader.GetBitmap("buttons.100.png"));

            //-------------------------------------------------------------------
            // Polygon
            //-------------------------------------------------------------------

            UpdateButton(btnPolyMove, Res.GetImage(256));
            UpdateButton(btnPolyPointer, Res.GetImage(252));
            UpdateButton(btnPolyAddPoint, Res.GetImage(253));
            UpdateButton(btnPolyAddBezier, Res.GetImage(254));
            UpdateButton(btnPolyRemovePoint, Res.GetImage(255));

            UpdateButton(btnReportOptions, ResourceLoader.GetBitmap("buttons.Report1.png"));

            UpdateButton(btnDataChoose, ResourceLoader.GetBitmap("buttons.ChooseData1.png"));
            UpdateButton(btnDataAdd, ResourceLoader.GetBitmap("buttons.AddDataSource.png"));

            UpdateButton(btnAddPage, Res.GetImage(10));
            UpdateButton(btnCopyPage, Res.GetImage(6));
            UpdateButton(btnAddDialog, Res.GetImage(11));
            UpdateButton(btnDeletePage, Res.GetImage(12));
            UpdateButton(btnPageSetup, ResourceLoader.GetBitmap("buttons.PageSetup.png"));

            UpdateButton(btnConfigureBands, ResourceLoader.GetBitmap("buttons.Bands.png"));
            UpdateButton(btnGroupExpert, ResourceLoader.GetBitmap("buttons.Grouping.png"));

            UpdateButton(btnAlignToGrid, ResourceLoader.GetBitmap("buttons.098.png"));
            UpdateButton(btnFitToGrid, ResourceLoader.GetBitmap("buttons.FitToGrid.png"));
            UpdateButton(btnAlignLefts, Res.GetImage(41));
            UpdateButton(btnAlignCenters, Res.GetImage(42));
            UpdateButton(btnAlignRights, Res.GetImage(45));
            UpdateButton(btnAlignTops, Res.GetImage(46));
            UpdateButton(btnAlignMiddles, Res.GetImage(47));
            UpdateButton(btnAlignBottoms, Res.GetImage(50));
            UpdateButton(btnSameWidth, Res.GetImage(83));
            UpdateButton(btnSameHeight, Res.GetImage(84));
            UpdateButton(btnSameSize, Res.GetImage(91));
            UpdateButton(btnSpaceHorizontally, Res.GetImage(44));
            UpdateButton(btnIncreaseHorizontalSpacing, Res.GetImage(92));
            UpdateButton(btnDecreaseHorizontalSpacing, Res.GetImage(93));
            UpdateButton(btnRemoveHorizontalSpacing, Res.GetImage(94));
            UpdateButton(btnSpaceVertically, Res.GetImage(49));
            UpdateButton(btnIncreaseVerticalSpacing, Res.GetImage(95));
            UpdateButton(btnDecreaseVerticalSpacing, Res.GetImage(96));
            UpdateButton(btnRemoveVerticalSpacing, Res.GetImage(97));
            UpdateButton(btnCenterHorizontally, Res.GetImage(43));
            UpdateButton(btnCenterVertically, Res.GetImage(48));
            UpdateButton(btnBringToFront, ResourceLoader.GetBitmap("buttons.BringToFront.png"));
            UpdateButton(btnSendToBack, ResourceLoader.GetBitmap("buttons.SendToBack.png"));

            UpdateButton(btnAlignment, ResourceLoader.GetBitmap("buttons.AlignMenu.png"));
            UpdateButton(btnSize, ResourceLoader.GetBitmap("buttons.SizeMenu.png"));
            UpdateButton(btnSpacing, ResourceLoader.GetBitmap("buttons.SpacingMenu.png"));

            UpdateButton(btnGroup, ResourceLoader.GetBitmap("buttons.Group.png"));
            UpdateButton(btnUngroup, ResourceLoader.GetBitmap("buttons.Ungroup.png"));

            UpdateButton(btnViewGrid, ResourceLoader.GetBitmap("buttons.ViewGridlines.png"));
            UpdateButton(btnViewGuides, ResourceLoader.GetBitmap("buttons.ViewGuides.png"));

            // CheckBoxes
            btnReportTitle.UpdateDpiDependencies();
            btnReportSummary.UpdateDpiDependencies();
            btnPageHeader.UpdateDpiDependencies();
            btnPageFooter.UpdateDpiDependencies();
            btnColumnHeader.UpdateDpiDependencies();
            btnColumnFooter.UpdateDpiDependencies();
            btnOverlay.UpdateDpiDependencies();
            btnReportTitle.RecalcSize();
            btnReportSummary.RecalcSize();
            btnPageHeader.RecalcSize();
            btnPageFooter.RecalcSize();
            btnColumnHeader.RecalcSize();
            btnColumnFooter.RecalcSize();
            btnOverlay.RecalcSize();

            //view
            UpdateButton(btnUnits, ResourceLoader.GetBitmap("buttons.013.png"));
            UpdateButton(btnPanels, ResourceLoader.GetBitmap("buttons.Panels.png"));

            //this.btnFile.UpdateDpiDependencies();
            designer.UpdateUIStyle();
        }

        private void UpdateButton(ButtonItem button, Bitmap bitmap)
        {
            button.Image = bitmap;
            button.UpdateDpiDependencies();
        }

        /// <inheritdoc/>
        protected override void WndProc(ref Message m)
        {
            const int WM_ACTIVATEAPP = 0x001C;
            const int WM_DPICHANGED = 0x02E0;
            if (m.Msg == WM_ACTIVATEAPP)
            {
                if (m.WParam == IntPtr.Zero)
                {
                    PopupManager.CloseAllPopups();
                }
            }
            else if (m.Msg == WM_DPICHANGED)
            {
                // new dpi
                int wp = (int)m.WParam >> 16;
                newdpi = wp;
                SystemEvents_DisplaySettingsChanged(null, null);
            }
            base.WndProc(ref m);
        }

        private void DesignerForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 0x9 && e.Control)
            {
                if (e.Shift)
                    Designer.ActiveReportTab.CtrlShiftTab();
                else
                    Designer.ActiveReportTab.CtrlTab();

                e.Handled = true;
            }
        }

        private void ShowWelcomeWindow(object s, EventArgs e)
        {
            Shown -= ShowWelcomeWindow;

            if (Config.WelcomeEnabled &&
                Config.WelcomeShowOnStartup &&
                String.IsNullOrEmpty(designer.Report.FileName))
            {
                Designer.cmdWelcome.Invoke();
            }
        }

        #region Utils
        private void CreateButton(ButtonItem button, Bitmap image, EventHandler click)
        {
            button.Image = image;
            button.Click += click;
            button.RecalcSize();
        }

        private void SetItemText(BaseItem item, string text)
        {
            SetItemText(item, text, text);
        }

        private void SetItemText(BaseItem item, string text, string tooltip)
        {
            item.Text = text;
            item.Tooltip = tooltip;
        }
        #endregion

        #region Setup Controls
        private void SetupControls()
        {
            SetupFileControls();
            SetupHomeControls();
            SetupReportControls();
            SetupLayoutControls();
            SetupViewControls();
        }

        private void SetupFileControls()
        {
            Bitmap cap = DpiHelper.ConvertBitmap(new Bitmap(32, 32));

            btnFile.PopupOpen += miFile_PopupOpen;

            btnFileNew.Click += Designer.cmdNew.Invoke;

            //btnFileOpen.Image = Res.GetImage(1);
            btnFileOpen.Click += Designer.cmdOpen.Invoke;

            btnFileClose.Click += Designer.cmdClose.Invoke;

            //btnFileSave.Image = Res.GetImage(2);
            btnFileSave.Click += Designer.cmdSave.Invoke;

            btnFileSaveAs.Click += Designer.cmdSaveAs.Invoke;

            //btnFileSaveAll.Image = Res.GetImage(178);
            btnFileSaveAll.Click += Designer.cmdSaveAll.Invoke;

            //btnFilePageSetup = CreateMenuItem(Designer.cmdPageSetup.Invoke);

            btnFilePrinterSetup.Click += Designer.cmdPrinterSetup.Invoke;

            btnFilePreview.Image = cap;
            btnFilePreview.Click += btnPreview_Click;

            btnFileSelectLanguage.Click += Designer.cmdSelectLanguage.Invoke;
            btnFileSelectLanguage.Image = cap;

            btnFileExit.Click += Designer.Exit;

            btnOptions.Click += Designer.cmdOptions.Invoke;

            btnWelcome.Visible = Designer.cmdWelcome.Enabled;
            btnWelcome.Click += Designer.cmdWelcome.Invoke;
            btnWelcome.Image = cap;

            btnHelp.Click += Designer.cmdHelpContents.Invoke;
            btnHelp.Image = cap;

            btnAbout.Click += Designer.cmdAbout.Invoke;
            btnAbout.Image = cap;
        }

        private void SetupHomeControls()
        {
            //-------------------------------------------------------------------
            // Undo
            //-------------------------------------------------------------------

            CreateButton(btnUndo, Res.GetImage(8), Designer.cmdUndo.Invoke);
            CreateButton(btnRedo, Res.GetImage(9), Designer.cmdRedo.Invoke);

            //-------------------------------------------------------------------
            // Clipboard
            //-------------------------------------------------------------------

            CreateButton(btnCut, Res.GetImage(5), Designer.cmdCut.Invoke);
            CreateButton(btnCopy, Res.GetImage(6), Designer.cmdCopy.Invoke);
            CreateButton(btnPaste, ResourceLoader.GetBitmap("buttons.007.png"), Designer.cmdPaste.Invoke);
            CreateButton(btnFormatPainter, Res.GetImage(18), Designer.cmdFormatPainter.Invoke);

            clipboardTimer = new Timer();
            clipboardTimer.Interval = 500;
            clipboardTimer.Tick += clipboardTimer_Tick;
            clipboardTimer.Start();

            //-------------------------------------------------------------------
            // Text
            //-------------------------------------------------------------------

            cbxFontName.FontSelected += cbxName_FontSelected;
            cbxFontSize.SizeSelected += cbxSize_SizeSelected;
            btnTextColor.Click += btnColor_Click;
            btnTextColor.ImageIndex = 23;
            btnTextColor.SetStyle(designer.UIStyle);

            CreateButton(btnBold, Res.GetImage(20), btnBold_Click);
            CreateButton(btnItalic, Res.GetImage(21), btnItalic_Click);
            CreateButton(btnUnderline, Res.GetImage(22), btnUnderline_Click);
            CreateButton(btnAlignLeft, Res.GetImage(25), btnLeft_Click);
            CreateButton(btnAlignCenter, Res.GetImage(26), btnCenter_Click);
            CreateButton(btnAlignRight, Res.GetImage(27), btnRight_Click);
            CreateButton(btnJustify, Res.GetImage(28), btnJustify_Click);
            CreateButton(btnAlignTop, Res.GetImage(29), btnTop_Click);
            CreateButton(btnAlignMiddle, Res.GetImage(30), btnMiddle_Click);
            CreateButton(btnAlignBottom, Res.GetImage(31), btnBottom_Click);
            CreateButton(btnTextRotation, Res.GetImage(64), btnRotation_Click);

            //-------------------------------------------------------------------
            // Border and Fill
            //-------------------------------------------------------------------

            CreateButton(btnTopLine, Res.GetImage(32), btnTopLine_Click);
            CreateButton(btnBottomLine, Res.GetImage(33), btnBottomLine_Click);
            CreateButton(btnLeftLine, Res.GetImage(34), btnLeftLine_Click);
            CreateButton(btnRightLine, Res.GetImage(35), btnRightLine_Click);
            CreateButton(btnAllLines, Res.GetImage(36), btnAll_Click);
            CreateButton(btnNoLines, Res.GetImage(37), btnNone_Click);

            btnFillColor.ImageIndex = 38;
            btnFillColor.DefaultColor = Color.Transparent;
            btnFillColor.Click += btnFillColor_Click;

            CreateButton(btnFillProps, Res.GetImage(141), btnFillProps_Click);

            btnLineColor.ImageIndex = 39;
            btnLineColor.DefaultColor = Color.Black;
            btnLineColor.Click += btnLineColor_Click;

            btnLineWidth.Image = Res.GetImage(71);
            btnLineWidth.WidthSelected += cbxWidth_WidthSelected;

            btnLineStyle.Image = Res.GetImage(85);
            btnLineStyle.StyleSelected += cbxLineStyle_StyleSelected;

            CreateButton(btnBorderProps, Res.GetImage(40), btnBorderProps_Click);

            //-------------------------------------------------------------------
            // Format
            //-------------------------------------------------------------------

            CreateButton(btnHighlight, ResourceLoader.GetBitmap("buttons.024.png"), btnHighlight_Click);
            CreateButton(btnFormat, ResourceLoader.GetBitmap("buttons.019.png"), btnFormat_Click);

            //-------------------------------------------------------------------
            // Styles
            //-------------------------------------------------------------------

            cbxStyles.StyleSelected += cbxStyle_StyleSelected;
            CreateButton(btnStyles, Res.GetImage(87), Designer.cmdReportStyles.Invoke);

            //-------------------------------------------------------------------
            // Editing
            //-------------------------------------------------------------------

            CreateButton(btnFind, Res.GetImage(181), Designer.cmdFind.Invoke);
            CreateButton(btnReplace, ResourceLoader.GetBitmap("buttons.069.png"), Designer.cmdReplace.Invoke);
            CreateButton(btnSelectAll, ResourceLoader.GetBitmap("buttons.100.png"), Designer.cmdSelectAll.Invoke);

            //-------------------------------------------------------------------
            // Polygon
            //-------------------------------------------------------------------

            CreateButton(btnPolyMove, Res.GetImage(256), Designer.CmdPolySelectMove.Invoke);
            CreateButton(btnPolyPointer, Res.GetImage(252), Designer.CmdPolySelectPointer.Invoke);
            CreateButton(btnPolyAddPoint, Res.GetImage(253), Designer.CmdPolySelectAddPoint.Invoke);
            CreateButton(btnPolyAddBezier, Res.GetImage(254), Designer.CmdPolySelectBezier.Invoke);
            CreateButton(btnPolyRemovePoint, Res.GetImage(255), Designer.CmdPolySelectRemovePoint.Invoke);


            //-------------------------------------------------------------------


        }

        private void SetupReportControls()
        {
            CreateButton(btnReportOptions, ResourceLoader.GetBitmap("buttons.Report1.png"), Designer.cmdReportSettings.Invoke);
            CreateButton(btnPreview, ResourceLoader.GetBitmap("buttons.report.png"), btnPreview_Click);

            CreateButton(btnDataChoose, ResourceLoader.GetBitmap("buttons.ChooseData1.png"), Designer.cmdChooseData.Invoke);
            CreateButton(btnDataAdd, ResourceLoader.GetBitmap("buttons.AddDataSource.png"), Designer.cmdAddData.Invoke);

            CreateButton(btnAddPage, Res.GetImage(10), Designer.cmdNewPage.Invoke);
            CreateButton(btnCopyPage, Res.GetImage(6), Designer.cmdCopyPage.Invoke);
            CreateButton(btnAddDialog, Res.GetImage(11), Designer.cmdNewDialog.Invoke);
            CreateButton(btnDeletePage, Res.GetImage(12), Designer.cmdDeletePage.Invoke);
            CreateButton(btnPageSetup, ResourceLoader.GetBitmap("buttons.PageSetup.png"), Designer.cmdPageSetup.Invoke);

            CreateButton(btnConfigureBands, ResourceLoader.GetBitmap("buttons.Bands.png"), miInsertBands_Click);
            CreateButton(btnGroupExpert, ResourceLoader.GetBitmap("buttons.Grouping.png"), miReportGroupExpert_Click);

            btnReportTitle.Click += miReportTitle_Click;
            btnReportSummary.Click += miReportSummary_Click;
            btnPageHeader.Click += miPageHeader_Click;
            btnPageFooter.Click += miPageFooter_Click;
            btnColumnHeader.Click += miColumnHeader_Click;
            btnColumnFooter.Click += miColumnFooter_Click;
            btnOverlay.Click += miOverlay_Click;

            previewTimer = new Timer();
            previewTimer.Interval = 20;
            previewTimer.Tick += previewTimer_Tick;
        }

        private void SetupLayoutControls()
        {
            CreateButton(btnAlignToGrid, ResourceLoader.GetBitmap("buttons.098.png"), btnAlignToGrid_Click);
            CreateButton(btnFitToGrid, ResourceLoader.GetBitmap("buttons.FitToGrid.png"), btnSizeToGrid_Click);
            CreateButton(btnAlignLefts, Res.GetImage(41), btnAlignLefts_Click);
            CreateButton(btnAlignCenters, Res.GetImage(42), btnAlignCenters_Click);
            CreateButton(btnAlignRights, Res.GetImage(45), btnAlignRights_Click);
            CreateButton(btnAlignTops, Res.GetImage(46), btnAlignTops_Click);
            CreateButton(btnAlignMiddles, Res.GetImage(47), btnAlignMiddles_Click);
            CreateButton(btnAlignBottoms, Res.GetImage(50), btnAlignBottoms_Click);
            CreateButton(btnSameWidth, Res.GetImage(83), btnSameWidth_Click);
            CreateButton(btnSameHeight, Res.GetImage(84), btnSameHeight_Click);
            CreateButton(btnSameSize, Res.GetImage(91), btnSameSize_Click);
            CreateButton(btnSpaceHorizontally, Res.GetImage(44), btnSpaceHorizontally_Click);
            CreateButton(btnIncreaseHorizontalSpacing, Res.GetImage(92), btnIncreaseHorizontalSpacing_Click);
            CreateButton(btnDecreaseHorizontalSpacing, Res.GetImage(93), btnDecreaseHorizontalSpacing_Click);
            CreateButton(btnRemoveHorizontalSpacing, Res.GetImage(94), btnRemoveHorizontalSpacing_Click);
            CreateButton(btnSpaceVertically, Res.GetImage(49), btnSpaceVertically_Click);
            CreateButton(btnIncreaseVerticalSpacing, Res.GetImage(95), btnIncreaseVerticalSpacing_Click);
            CreateButton(btnDecreaseVerticalSpacing, Res.GetImage(96), btnDecreaseVerticalSpacing_Click);
            CreateButton(btnRemoveVerticalSpacing, Res.GetImage(97), btnRemoveVerticalSpacing_Click);
            CreateButton(btnCenterHorizontally, Res.GetImage(43), btnCenterHorizontally_Click);
            CreateButton(btnCenterVertically, Res.GetImage(48), btnCenterVertically_Click);
            CreateButton(btnBringToFront, ResourceLoader.GetBitmap("buttons.BringToFront.png"), Designer.cmdBringToFront.Invoke);
            CreateButton(btnSendToBack, ResourceLoader.GetBitmap("buttons.SendToBack.png"), Designer.cmdSendToBack.Invoke);

            btnAlignment.Image = ResourceLoader.GetBitmap("buttons.AlignMenu.png");
            btnSize.Image = ResourceLoader.GetBitmap("buttons.SizeMenu.png");
            btnSpacing.Image = ResourceLoader.GetBitmap("buttons.SpacingMenu.png");

            CreateButton(btnGroup, ResourceLoader.GetBitmap("buttons.Group.png"), Designer.cmdGroup.Invoke);
            CreateButton(btnUngroup, ResourceLoader.GetBitmap("buttons.Ungroup.png"), Designer.cmdUngroup.Invoke);
        }

        private void SetupViewControls()
        {
            CreateButton(btnViewGrid, ResourceLoader.GetBitmap("buttons.ViewGridlines.png"), MenuViewGrid_Click);
            CreateButton(btnViewGuides, ResourceLoader.GetBitmap("buttons.ViewGuides.png"), MenuViewGuides_Click);
            btnAutoGuides.Click += MenuViewAutoGuides_Click;
            btnDeleteHGuides.Click += MenuViewDeleteHGuides_Click;
            btnDeleteVGuides.Click += MenuViewDeleteVGuides_Click;

            designer.PropertiesWindow.VisibleChanged += delegate (object s, EventArgs e)
            {
                btnProperties.Checked = designer.PropertiesWindow.Visible;
            };
            designer.DataWindow.VisibleChanged += delegate (object s, EventArgs e)
            {
                btnData.Checked = designer.DataWindow.Visible;
            };
            designer.ReportTreeWindow.VisibleChanged += delegate (object s, EventArgs e)
            {
                btnReportTree.Checked = designer.ReportTreeWindow.Visible;
            };
            designer.MessagesWindow.VisibleChanged += delegate (object s, EventArgs e)
            {
                 btnMessages.Checked = designer.MessagesWindow.Visible;
            };

            btnPanels.Image = ResourceLoader.GetBitmap("buttons.Panels.png");

            btnProperties.Image = Res.GetImage(68);
            btnProperties.Click += delegate (object s, EventArgs e)
            {
                if (designer.PropertiesWindow.Visible)
                    designer.PropertiesWindow.Hide();
                else
                    designer.PropertiesWindow.Show();
            };
            btnData.Image = Res.GetImage(72);
            btnData.Click += delegate (object s, EventArgs e)
            {
                if (designer.DataWindow.Visible)
                    designer.DataWindow.Hide();
                else
                    designer.DataWindow.Show();
            };
            btnReportTree.Image = Res.GetImage(189);
            btnReportTree.Click += delegate (object s, EventArgs e)
            {
                if (designer.ReportTreeWindow.Visible)
                    designer.ReportTreeWindow.Hide();
                else
                    designer.ReportTreeWindow.Show();
            };
            btnMessages.Image = Res.GetImage(70);
            btnMessages.Click += delegate (object s, EventArgs e)
            {
                if (designer.MessagesWindow.Visible)
                    designer.MessagesWindow.Hide();
                else
                    designer.MessagesWindow.Show();
            };

            btnUnits.Image = ResourceLoader.GetBitmap("buttons.013.png");
            btnUnitsMillimeters.Click += miViewUnits_Click;
            btnUnitsCentimeters.Click += miViewUnits_Click;
            btnUnitsInches.Click += miViewUnits_Click;
            btnUnitsHundrethsOfInch.Click += miViewUnits_Click;
        }
        #endregion

        #region Update Controls

        private void UpdateControls()
        {
            UpdateFileControls();
            UpdateHomeControls();
            UpdateReportControls();
            UpdateLayoutControls();
            UpdateViewControls();
        }

        private void UpdateFileControls()
        {
            btnFileNew.Enabled = Designer.cmdNew.Enabled;
            btnFileOpen.Enabled = Designer.cmdOpen.Enabled;
            btnFileClose.Enabled = Designer.cmdClose.Enabled;
            btnFileClose.Visible = Designer.MdiMode;
            btnFileSave.Enabled = Designer.cmdSave.Enabled;
            btnFileSaveAs.Enabled = Designer.cmdSaveAs.Enabled;
            btnFileSaveAll.Visible = Designer.MdiMode;
            btnFileSaveAll.Enabled = Designer.cmdSaveAll.Enabled;
            //btnFilePageSetup.Enabled = Designer.cmdPageSetup.Enabled;
            btnFilePrinterSetup.Enabled = Designer.cmdPrinterSetup.Enabled;
            btnFilePreview.Enabled = Designer.cmdPreview.Enabled;
            btnWelcome.Enabled = Designer.cmdWelcome.Enabled;
            btnHelp.Enabled = Designer.cmdHelpContents.Enabled;
        }

        private void UpdateHomeControls()
        {
            //-------------------------------------------------------------------
            // Undo
            //-------------------------------------------------------------------

            btnUndo.Enabled = Designer.cmdUndo.Enabled;
            btnRedo.Enabled = Designer.cmdRedo.Enabled;

            //-------------------------------------------------------------------
            // Clipboard
            //-------------------------------------------------------------------

            btnCut.Enabled = Designer.cmdCut.Enabled;
            btnCopy.Enabled = Designer.cmdCopy.Enabled;
            btnPaste.Enabled = Designer.cmdPaste.Enabled;
            btnFormatPainter.Enabled = Designer.cmdFormatPainter.Enabled;
            btnFormatPainter.Checked = Designer.FormatPainter;

            //-------------------------------------------------------------------
            // Text
            //-------------------------------------------------------------------

            bool enabled = Designer.SelectedTextObjects.Enabled;

            cbxFontName.Enabled = enabled;
            cbxFontSize.Enabled = enabled;
            btnBold.Enabled = enabled;
            btnItalic.Enabled = enabled;
            btnUnderline.Enabled = enabled;
            btnAlignLeft.Enabled = enabled;
            btnAlignCenter.Enabled = enabled;
            btnAlignRight.Enabled = enabled;
            btnJustify.Enabled = enabled;
            btnAlignTop.Enabled = enabled;
            btnAlignMiddle.Enabled = enabled;
            btnAlignBottom.Enabled = enabled;
            btnTextColor.Enabled = enabled;
            btnTextRotation.Enabled = enabled;

            if (enabled)
            {
                TextObject text = Designer.SelectedTextObjects.First;

                cbxFontName.FontName = text.Font.Name;
                cbxFontSize.FontSize = text.Font.Size;
                btnBold.Checked = text.Font.Bold;
                btnItalic.Checked = text.Font.Italic;
                btnUnderline.Checked = text.Font.Underline;
                btnAlignLeft.Checked = text.HorzAlign == HorzAlign.Left;
                btnAlignCenter.Checked = text.HorzAlign == HorzAlign.Center;
                btnAlignRight.Checked = text.HorzAlign == HorzAlign.Right;
                btnJustify.Checked = text.HorzAlign == HorzAlign.Justify;
                btnAlignTop.Checked = text.VertAlign == VertAlign.Top;
                btnAlignMiddle.Checked = text.VertAlign == VertAlign.Center;
                btnAlignBottom.Checked = text.VertAlign == VertAlign.Bottom;
                if (text.TextFill is SolidFill)
                    btnTextColor.Color = (text.TextFill as SolidFill).Color;
            }
            else
            {
                btnBold.Checked = false;
                btnItalic.Checked = false;
                btnUnderline.Checked = false;
                btnAlignLeft.Checked = false;
                btnAlignCenter.Checked = false;
                btnAlignRight.Checked = false;
                btnJustify.Checked = false;
                btnAlignTop.Checked = false;
                btnAlignMiddle.Checked = false;
                btnAlignBottom.Checked = false;
            }

            //-------------------------------------------------------------------
            // Border and Fill
            //-------------------------------------------------------------------

            enabled = Designer.SelectedReportComponents.Enabled;
            bool simple = Designer.SelectedReportComponents.SimpleBorder;
            bool useBorder = Designer.SelectedReportComponents.BorderEnabled;

            bool borderEnabled = enabled && !simple && useBorder;
            btnTopLine.Enabled = borderEnabled;
            btnBottomLine.Enabled = borderEnabled;
            btnLeftLine.Enabled = borderEnabled;
            btnRightLine.Enabled = borderEnabled;
            btnAllLines.Enabled = borderEnabled;
            btnNoLines.Enabled = borderEnabled;
            btnFillColor.Enabled = enabled && Designer.SelectedReportComponents.FillEnabled;
            btnFillProps.Enabled = enabled && Designer.SelectedReportComponents.FillEnabled;
            btnLineColor.Enabled = enabled && useBorder;
            btnLineWidth.Enabled = enabled && useBorder;
            btnLineStyle.Enabled = enabled && useBorder;
            btnBorderProps.Enabled = borderEnabled;

            if (enabled)
            {
                Border border = Designer.SelectedReportComponents.First.Border;
                btnTopLine.Checked = (border.Lines & BorderLines.Top) != 0;
                btnBottomLine.Checked = (border.Lines & BorderLines.Bottom) != 0;
                btnLeftLine.Checked = (border.Lines & BorderLines.Left) != 0;
                btnRightLine.Checked = (border.Lines & BorderLines.Right) != 0;
                btnLineColor.Color = border.Color;
                if (Designer.SelectedReportComponents.First.Fill is SolidFill)
                    btnFillColor.Color = (Designer.SelectedReportComponents.First.Fill as SolidFill).Color;
                btnLineWidth.LineWidth = border.Width;
                btnLineStyle.LineStyle = border.Style;
            }

            //-------------------------------------------------------------------
            // Format
            //-------------------------------------------------------------------

            btnHighlight.Enabled = Designer.SelectedTextObjects.Enabled;
            btnFormat.Enabled = Designer.SelectedTextObjects.Enabled;

            //-------------------------------------------------------------------
            // Editing
            //-------------------------------------------------------------------

            btnFind.Enabled = Designer.cmdFind.Enabled;
            btnReplace.Enabled = Designer.cmdReplace.Enabled;
            btnSelectAll.Enabled = Designer.cmdSelectAll.Enabled;

            //-------------------------------------------------------------------
            // Styles
            //-------------------------------------------------------------------

            enabled = Designer.SelectedReportComponents.Enabled;

            cbxStyles.Enabled = enabled;
            cbxStyles.Report = Designer.ActiveReport;
            if (enabled)
                cbxStyles.Style = Designer.SelectedReportComponents.First.Style;


            //-------------------------------------------------------------------
            // Polygon
            //-------------------------------------------------------------------

            enabled = (Designer.SelectedObjects.Count == 1) && (Designer.SelectedObjects[0] is PolyLineObject);

            btnPolyMove.Enabled = enabled;
            btnPolyPointer.Enabled = enabled;
            btnPolyAddBezier.Enabled = enabled;
            btnPolyAddPoint.Enabled = enabled;
            btnPolyRemovePoint.Enabled = enabled;
            if (!enabled)
                selectBtn(PolyLineObject.PolygonSelectionMode.MoveAndScale);
            else
            {
                PolyLineObject plobj = (Designer.SelectedObjects[0] as PolyLineObject);
                selectBtn(plobj.SelectionMode);
            }


            //-------------------------------------------------------------------
        }

        private void selectBtn(PolyLineObject.PolygonSelectionMode index)
        {
            PolyLineObject plobj = null;
            if ((Designer.SelectedObjects.Count == 1) && (Designer.SelectedObjects[0] is PolyLineObject))
                plobj = (Designer.SelectedObjects[0] as PolyLineObject);
            btnPolyMove.Checked = false;
            btnPolyPointer.Checked = false;
            btnPolyAddBezier.Checked = false;
            btnPolyAddPoint.Checked = false;
            btnPolyRemovePoint.Checked = false;
            switch (index)
            {
                case PolyLineObject.PolygonSelectionMode.Normal:
                    btnPolyPointer.Checked = true;
                    break;
                case PolyLineObject.PolygonSelectionMode.AddBezier:
                    btnPolyAddBezier.Checked = true;
                    break;
                case PolyLineObject.PolygonSelectionMode.AddToLine:
                    btnPolyAddPoint.Checked = true;
                    break;
                case PolyLineObject.PolygonSelectionMode.Delete:
                    btnPolyRemovePoint.Checked = true;
                    break;
                case PolyLineObject.PolygonSelectionMode.MoveAndScale:
                    btnPolyMove.Checked = true;
                    break;
            }
        }

        private void UpdateReportControls()
        {
            btnPreview.Enabled = Designer.cmdPreview.Enabled;
            btnReportOptions.Enabled = Designer.cmdReportSettings.Enabled;
            btnDataChoose.Enabled = Designer.cmdChooseData.Enabled;
            btnDataAdd.Enabled = Designer.cmdAddData.Enabled;

            btnAddPage.Enabled = Designer.cmdNewPage.Enabled;
            btnCopyPage.Enabled = Designer.cmdCopyPage.Enabled;
            btnAddDialog.Enabled = Designer.cmdNewDialog.Enabled;
            btnDeletePage.Enabled = Designer.cmdDeletePage.Enabled;
            btnPageSetup.Enabled = Designer.cmdPageSetup.Enabled;

            bool bandsEnabled = Designer.cmdInsertBand.Enabled;
            btnConfigureBands.Enabled = bandsEnabled;
            btnGroupExpert.Enabled = bandsEnabled;

            ReportPage page = null;

            try
            {
                //see fix # 66fcd219-30f1-45e6-8ee5-ce65cfb9d35d
                //rly annoying freezes on designer start
                page = Page as ReportPage;
            }
            catch
            {

            }

            if (page != null)
            {
                bool isSubreport = page.Subreport != null;

                btnReportTitle.Enabled = bandsEnabled && !isSubreport;
                btnReportSummary.Enabled = bandsEnabled && !isSubreport;
                btnPageHeader.Enabled = bandsEnabled && !isSubreport;
                btnPageFooter.Enabled = bandsEnabled && !isSubreport;
                btnColumnHeader.Enabled = bandsEnabled && !isSubreport;
                btnColumnFooter.Enabled = bandsEnabled && !isSubreport;
                btnOverlay.Enabled = bandsEnabled && !isSubreport;

                btnReportTitle.Checked = page.ReportTitle != null;
                btnReportSummary.Checked = page.ReportSummary != null;
                btnPageHeader.Checked = page.PageHeader != null;
                btnPageFooter.Checked = page.PageFooter != null;
                btnColumnHeader.Checked = page.ColumnHeader != null;
                btnColumnFooter.Checked = page.ColumnFooter != null;
                btnOverlay.Checked = page.Overlay != null;
            }
            else
            {
                btnReportTitle.Enabled = false;
                btnReportSummary.Enabled = false;
                btnPageHeader.Enabled = false;
                btnPageFooter.Enabled = false;
                btnColumnHeader.Enabled = false;
                btnColumnFooter.Enabled = false;
                btnOverlay.Enabled = false;
            }
        }

        private void UpdateLayoutControls()
        {
            bool oneObjSelected = Designer.SelectedComponents.Count > 0;
            bool threeObjSelected = Designer.SelectedComponents.Count >= 3;
            bool severalObjSelected = Designer.SelectedComponents.Count > 1;
            bool canChangeOrder = Designer.SelectedComponents.Count > 0 &&
                Designer.SelectedComponents.First.HasFlag(Flags.CanChangeOrder);
            bool canMove = Designer.SelectedComponents.Count > 0 && Designer.SelectedComponents.First.HasFlag(Flags.CanMove);
            bool canResize = Designer.SelectedComponents.Count > 0 &&
                Designer.SelectedComponents.First.HasFlag(Flags.CanResize);

            btnAlignToGrid.Enabled = oneObjSelected && canMove;
            btnAlignLefts.Enabled = severalObjSelected && canMove;
            btnAlignCenters.Enabled = severalObjSelected && canMove;
            btnAlignRights.Enabled = severalObjSelected && canMove;
            btnAlignTops.Enabled = severalObjSelected && canMove;
            btnAlignMiddles.Enabled = severalObjSelected && canMove;
            btnAlignBottoms.Enabled = severalObjSelected && canMove;
            btnSameWidth.Enabled = severalObjSelected && canResize;
            btnSameHeight.Enabled = severalObjSelected && canResize;
            btnSameSize.Enabled = severalObjSelected && canResize;
            btnFitToGrid.Enabled = oneObjSelected && canResize;
            btnSpaceHorizontally.Enabled = threeObjSelected && canMove;
            btnIncreaseHorizontalSpacing.Enabled = severalObjSelected && canMove;
            btnDecreaseHorizontalSpacing.Enabled = severalObjSelected && canMove;
            btnRemoveHorizontalSpacing.Enabled = severalObjSelected && canMove;
            btnSpaceVertically.Enabled = threeObjSelected && canMove;
            btnIncreaseVerticalSpacing.Enabled = severalObjSelected && canMove;
            btnDecreaseVerticalSpacing.Enabled = severalObjSelected && canMove;
            btnRemoveVerticalSpacing.Enabled = severalObjSelected && canMove;
            btnCenterHorizontally.Enabled = oneObjSelected && canMove;
            btnCenterVertically.Enabled = oneObjSelected && canMove;
            btnBringToFront.Enabled = canChangeOrder;
            btnSendToBack.Enabled = canChangeOrder;

            btnGroup.Enabled = Designer.cmdGroup.Enabled;
            btnUngroup.Enabled = Designer.cmdUngroup.Enabled;
        }

        private void UpdateViewControls()
        {
            if (Workspace != null)
            {
                btnViewGrid.Enabled = true;
                btnViewGuides.Enabled = true;
                btnViewGrid.Checked = ReportWorkspace.ShowGrid;
                btnViewGuides.Checked = ReportWorkspace.ShowGuides;

                btnAutoGuides.Enabled = true;
                bool autoGuides = ReportWorkspace.AutoGuides;
                btnAutoGuides.Checked = autoGuides;
                btnDeleteHGuides.Enabled = !autoGuides;
                btnDeleteVGuides.Enabled = !autoGuides;
            }
            else
            {
                btnViewGrid.Enabled = false;
                btnViewGuides.Enabled = false;
                btnAutoGuides.Enabled = false;
                btnDeleteHGuides.Enabled = false;
                btnDeleteVGuides.Enabled = false;
            }

            btnProperties.Checked = designer.PropertiesWindow.Visible;
            btnData.Checked = designer.DataWindow.Visible;
            btnReportTree.Checked = designer.ReportTreeWindow.Visible;
            btnMessages.Checked = designer.MessagesWindow.Visible;

            btnUnitsMillimeters.Checked = ReportWorkspace.Grid.GridUnits == PageUnits.Millimeters;
            btnUnitsCentimeters.Checked = ReportWorkspace.Grid.GridUnits == PageUnits.Centimeters;
            btnUnitsInches.Checked = ReportWorkspace.Grid.GridUnits == PageUnits.Inches;
            btnUnitsHundrethsOfInch.Checked = ReportWorkspace.Grid.GridUnits == PageUnits.HundrethsOfInch;
        }

        #endregion Update Controls

        #region Localization
        private void LocalizeFile()
        {
            MyRes res = new MyRes("Designer,Menu");

            btnFile.Text = res.Get("File");
            btnFileNew.Text = res.Get("File,New");

            btnFileOpen.Text = res.Get("File,Open");
            btnFileOpen.Tooltip = Res.Get("Designer,Toolbar,Standard,Open");

            btnFileClose.Text = res.Get("File,Close");

            btnFileSave.Text = res.Get("File,Save");
            btnFileSave.Tooltip = Res.Get("Designer,Toolbar,Standard,Save");

            btnFileSaveAs.Text = res.Get("File,SaveAs");

            btnFileSaveAll.Text = res.Get("File,SaveAll");
            btnFileSaveAll.Tooltip = Res.Get("Designer,Toolbar,Standard,SaveAll");

            //btnFilePageSetup.Text = res.Get("File,PageSetup");
            btnFilePrinterSetup.Text = res.Get("File,PrinterSetup");
            btnFilePreview.Text = res.Get("File,Preview");
            btnFileSelectLanguage.Text = res.Get("File,SelectLanguage");
            btnFileExit.Text = res.Get("File,Exit");
            btnOptions.Text = res.Get("View,Options");

            btnWelcome.Text = Res.Get("Designer,Welcome,Button");
            btnHelp.Text = res.Get("Help,Contents");
            btnAbout.Text = res.Get("Help,About");
        }

        private void localizeHome()
        {
            MyRes res = new MyRes("Designer,Toolbar,Standard");

            //SetItemText(btnNew, res.Get("New"));
            //SetItemText(btnOpen, res.Get("Open"));
            //SetItemText(btnSave, res.Get("Save"));
            //SetItemText(btnSaveAll, res.Get("SaveAll"));
            //SetItemText(btnPreview, res.Get("Preview"));

            SetItemText(btnAddPage, res.Get("NewPage"));
            SetItemText(btnCopyPage, res.Get("CopyPage"));
            SetItemText(btnAddDialog, res.Get("NewDialog"));
            SetItemText(btnDeletePage, res.Get("DeletePage"));
            SetItemText(btnPageSetup, res.Get("PageSetup"));
            SetItemText(btnFormatPainter, res.Get("FormatPainter"));

            res = new MyRes("Designer,Menu,Edit");

            SetItemText(btnCut, res.Get("Cut"), Res.Get("Designer,Toolbar,Standard,Cut"));
            SetItemText(btnCopy, res.Get("Copy"), Res.Get("Designer,Toolbar,Standard,Copy"));
            SetItemText(btnPaste, res.Get("Paste"), Res.Get("Designer,Toolbar,Standard,Paste"));
            SetItemText(btnUndo, res.Get("Undo"), Res.Get("Designer,Toolbar,Standard,Undo"));
            SetItemText(btnRedo, res.Get("Redo"), Res.Get("Designer,Toolbar,Standard,Redo"));

            res = new MyRes("Designer,Toolbar,Text");

            SetItemText(cbxFontName, res.Get("Name"));
            SetItemText(cbxFontSize, res.Get("Size"));
            SetItemText(btnBold, res.Get("Bold"));
            SetItemText(btnItalic, res.Get("Italic"));
            SetItemText(btnUnderline, res.Get("Underline"));
            SetItemText(btnAlignLeft, res.Get("Left"));
            SetItemText(btnAlignCenter, res.Get("Center"));
            SetItemText(btnAlignRight, res.Get("Right"));
            SetItemText(btnJustify, res.Get("Justify"));
            SetItemText(btnAlignTop, res.Get("Top"));
            SetItemText(btnAlignMiddle, res.Get("Middle"));
            SetItemText(btnAlignBottom, res.Get("Bottom"));
            SetItemText(btnTextColor, res.Get("Color"));
            SetItemText(btnHighlight, res.Get("Highlight"));
            SetItemText(btnTextRotation, res.Get("Angle"));

            res = new MyRes("Designer,Toolbar,Border");

            SetItemText(btnTopLine, res.Get("Top"));
            SetItemText(btnBottomLine, res.Get("Bottom"));
            SetItemText(btnLeftLine, res.Get("Left"));
            SetItemText(btnRightLine, res.Get("Right"));
            SetItemText(btnAllLines, res.Get("All"));
            SetItemText(btnNoLines, res.Get("None"));
            SetItemText(btnFillColor, res.Get("FillColor"));
            SetItemText(btnFillProps, res.Get("FillStyle"));
            SetItemText(btnLineColor, res.Get("LineColor"));
            SetItemText(btnLineWidth, res.Get("Width"));
            SetItemText(btnLineStyle, res.Get("Style"));
            SetItemText(btnBorderProps, res.Get("Props"));

            SetItemText(btnStyles, Res.Get("Designer,Menu,Report,Styles"));
            SetItemText(btnFormat, Res.Get("ComponentMenu,TextObject,Format"));
            SetItemText(btnSelectAll, Res.Get("Designer,Menu,Edit,SelectAll"));
            SetItemText(btnFind, Res.Get("Designer,Menu,Edit,Find"));
            SetItemText(btnReplace, Res.Get("Designer,Menu,Edit,Replace"));


            //polygons

            
            res = new MyRes("Designer,Toolbar,Polygon");
            barPolygon.Text = res.Get("");

            SetItemText(btnPolyMove, res.Get("MoveScale"));
            SetItemText(btnPolyPointer, res.Get("Pointer"));
            SetItemText(btnPolyAddPoint, res.Get("AddPoint"));
            SetItemText(btnPolyAddBezier, res.Get("Bezier"));
            SetItemText(btnPolyRemovePoint, res.Get("RemovePoint"));
        }

        private void LocalizeReport()
        {
            SetItemText(btnPreview, Res.Get("Designer,Menu,File,Preview"), Res.Get("Designer,Toolbar,Standard,Preview"));
            SetItemText(btnReportOptions, Res.Get("Designer,Menu,Report,Options"));

            SetItemText(btnDataChoose, Res.Get("Designer,Menu,Data,Choose"));
            SetItemText(btnDataAdd, Res.Get("Designer,Menu,Data,Add"));

            SetItemText(btnAddPage, Res.Get("Designer,Toolbar,Standard,NewPage"));
            SetItemText(btnCopyPage, Res.Get("Designer,Toolbar,Standard,CopyPage"));
            SetItemText(btnAddDialog, Res.Get("Designer,Toolbar,Standard,NewDialog"));
            SetItemText(btnDeletePage, Res.Get("Designer,Toolbar,Standard,DeletePage"));
            SetItemText(btnPageSetup, Res.Get("Designer,Toolbar,Standard,PageSetup"));

            MyRes res = new MyRes("Designer,Menu,Report");
            SetItemText(btnConfigureBands, res.Get("Bands"));
            SetItemText(btnGroupExpert, res.Get("GroupExpert"));

            res = new MyRes("Objects,Bands");
            SetItemText(btnReportTitle, res.Get("ReportTitle"));
            SetItemText(btnReportSummary, res.Get("ReportSummary"));
            SetItemText(btnPageHeader, res.Get("PageHeader"));
            SetItemText(btnPageFooter, res.Get("PageFooter"));
            SetItemText(btnColumnHeader, res.Get("ColumnHeader"));
            SetItemText(btnColumnFooter, res.Get("ColumnFooter"));
            SetItemText(btnOverlay, res.Get("Overlay"));
        }

        private void localizeLayout()
        {
            MyRes res = new MyRes("Designer,Toolbar,Layout");

            SetItemText(btnAlignToGrid, res.Get("AlignToGrid"));
            SetItemText(btnAlignLefts, res.Get("Left"));
            SetItemText(btnAlignCenters, res.Get("Center"));
            SetItemText(btnAlignRights, res.Get("Right"));
            SetItemText(btnAlignTops, res.Get("Top"));
            SetItemText(btnAlignMiddles, res.Get("Middle"));
            SetItemText(btnAlignBottoms, res.Get("Bottom"));
            SetItemText(btnSameWidth, res.Get("SameWidth"));
            SetItemText(btnSameHeight, res.Get("SameHeight"));
            SetItemText(btnSameSize, res.Get("SameSize"));
            SetItemText(btnFitToGrid, res.Get("SizeToGrid"));
            SetItemText(btnSpaceHorizontally, res.Get("SpaceHorizontally"));
            SetItemText(btnIncreaseHorizontalSpacing, res.Get("IncreaseHorizontalSpacing"));
            SetItemText(btnDecreaseHorizontalSpacing, res.Get("DecreaseHorizontalSpacing"));
            SetItemText(btnRemoveHorizontalSpacing, res.Get("RemoveHorizontalSpacing"));
            SetItemText(btnSpaceVertically, res.Get("SpaceVertically"));
            SetItemText(btnIncreaseVerticalSpacing, res.Get("IncreaseVerticalSpacing"));
            SetItemText(btnDecreaseVerticalSpacing, res.Get("DecreaseVerticalSpacing"));
            SetItemText(btnRemoveVerticalSpacing, res.Get("RemoveVerticalSpacing"));
            SetItemText(btnCenterHorizontally, res.Get("CenterHorizontally"));
            SetItemText(btnCenterVertically, res.Get("CenterVertically"));
            SetItemText(btnBringToFront, res.Get("BringToFront"));
            SetItemText(btnSendToBack, res.Get("SendToBack"));

            SetItemText(btnGroup, Res.Get("Designer,Toolbar,Standard,Group"));
            SetItemText(btnUngroup, Res.Get("Designer,Toolbar,Standard,Ungroup"));
        }

        private void LocalizeView()
        {
            MyRes res = new MyRes("Designer,Menu,View");
            SetItemText(btnViewGrid, res.Get("Grid"));
            SetItemText(btnViewGuides, res.Get("Guides"));
            SetItemText(btnAutoGuides, res.Get("AutoGuides"));
            SetItemText(btnDeleteHGuides, res.Get("DeleteHGuides"));
            SetItemText(btnDeleteVGuides, res.Get("DeleteVGuides"));

            SetItemText(btnProperties, Res.Get("Designer,ToolWindow,Properties"));
            SetItemText(btnData, Res.Get("Designer,ToolWindow,Dictionary"));
            SetItemText(btnReportTree, Res.Get("Designer,ToolWindow,ReportTree"));
            SetItemText(btnMessages, Res.Get("Designer,ToolWindow,Messages"));

            res = new MyRes("Designer,Menu,View");
            btnUnits.Text = res.Get("Units");

            res = new MyRes("Forms,ReportPageOptions");
            btnUnitsMillimeters.Text = res.Get("Millimeters");
            btnUnitsCentimeters.Text = res.Get("Centimeters");
            btnUnitsInches.Text = res.Get("Inches");
            btnUnitsHundrethsOfInch.Text = res.Get("HundrethsOfInch");
        }
        #endregion

        #region Methods
        #region Form Methods
        private void DesignerForm_Load(object sender, EventArgs e)
        {
            // bug/inconsistent behavior in .Net: if we set WindowState to Maximized, the
            // Load event will be fired *after* the form is shown.
            bool maximized = Config.RestoreFormState(this, true);
            // under some circumstances, the config file may contain wrong winodw position (-32000)
            if (!maximized && (Left < -10 || Top < -10))
                maximized = true;
            Designer.RestoreConfig();
            if (maximized)
                WindowState = FormWindowState.Maximized;

            Config.DesignerSettings.OnDesignerLoaded(Designer, EventArgs.Empty);
            Designer.StartAutoSave();
        }

        private void DesignerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Designer.CloseAll())
            {
                e.Cancel = true;
            }
            else
            {
                Config.SaveFormState(this);
                Designer.SaveConfig();
            }
        }

        private void DesignerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Config.DesignerSettings.OnDesignerClosed(Designer, EventArgs.Empty);
            Designer.StopAutoSave();
        }

        private void setupStatusBar()
        {
            slider.ValueChanged += zoom_ValueChanged;

            location.Image = Res.GetImage(62);
            size.Image = Res.GetImage(63);

            zoom1.Image = ResourceLoader.GetBitmap("ZoomPageWidth.png");
            zoom1.Click += delegate (object sender, EventArgs e)
            {
                if (Workspace != null)
                    Workspace.FitPageWidth();
            };

            zoom2.Image = ResourceLoader.GetBitmap("ZoomWholePage.png");
            zoom2.Click += delegate (object sender, EventArgs e)
            {
                if (Workspace != null)
                    Workspace.FitWholePage();
            };

            zoom3.Image = ResourceLoader.GetBitmap("Zoom100.png");
            zoom3.Click += delegate (object sender, EventArgs e)
            {
                if (Workspace != null)
                    Zoom = DpiHelper.ConvertUnits(1);
            };
        }

        private void zoom_ValueChanged(object sender, EventArgs e)
        {
            //if (FUpdatingZoom)
            //    return;

            int val = slider.Value;
            if (val < 100)
                val = (int)Math.Round(val * 0.75f) + 25;
            else
                val = (val - 100) * 4 + 100;

            zoomLabel.Text = val.ToString() + "%";
            Zoom = (val * DpiHelper.Multiplier) / 100f;
            //FZoomTimer.Start();
        }

        private void UpdateZoom()
        {
            //FUpdatingZoom = true;

            int zoom = (int)(Zoom * 100 / DpiHelper.Multiplier);
            zoomLabel.Text = zoom.ToString() + "%";
            if (zoom < 100)
                zoom = (int)Math.Round((zoom - 25) / 0.75f);
            else if (zoom > 100)
                zoom = (zoom - 100) / 4 + 100;
            this.slider.Value = zoom;

            //FUpdatingZoom = false;
        }
        #endregion
        #region File Methods
        private void miFile_PopupOpen(object sender, PopupOpenEventArgs e)
        {
            // clear existing recent items
            for (int i = 0; i < itemContainer23.SubItems.Count; i++)
            {
                BaseItem item = itemContainer23.SubItems[i];

                if (item is ButtonItem)
                {
                    item.Dispose();
                    itemContainer23.SubItems.Remove(item);
                    i--;
                }
            }

            // add new items
            if (Designer.cmdRecentFiles.Enabled && Designer.RecentFiles.Count > 0)
            {
                foreach (string file in Designer.RecentFiles)
                {
                    ButtonItem menuItem = new ButtonItem();
                    menuItem.Text = Path.GetFileName(file);
                    menuItem.Tag = file;
                    menuItem.Tooltip = file;
                    menuItem.Click += recentFile_Click;
                    itemContainer23.SubItems.Insert(1, menuItem);
                }
            }
        }

        private void recentFile_Click(object sender, EventArgs e)
        {
            Designer.UpdatePlugins(null);
            Designer.cmdOpen.LoadFile((sender as ButtonItem).Tag as string);
        }
        #endregion
        #region Home Methods
        //-------------------------------------------------------------------
        // Clipboard
        //-------------------------------------------------------------------

        private void clipboardTimer_Tick(object sender, EventArgs e)
        {
            btnPaste.Enabled = Designer.cmdPaste.Enabled;
        }

        //-------------------------------------------------------------------
        // Text
        //-------------------------------------------------------------------

        private void cbxName_FontSelected(object sender, EventArgs e)
        {
            (Designer.ActiveReportTab.ActivePageDesigner as ReportPageDesigner).Workspace.Focus();
            Designer.SelectedTextObjects.SetFontName(cbxFontName.FontName);
        }

        private void cbxSize_SizeSelected(object sender, EventArgs e)
        {
            (Designer.ActiveReportTab.ActivePageDesigner as ReportPageDesigner).Workspace.Focus();
            Designer.SelectedTextObjects.SetFontSize(cbxFontSize.FontSize);
        }

        private void btnBold_Click(object sender, EventArgs e)
        {
            btnBold.Checked = !btnBold.Checked;
            Designer.SelectedTextObjects.ToggleFontStyle(FontStyle.Bold, btnBold.Checked);
        }

        private void btnItalic_Click(object sender, EventArgs e)
        {
            btnItalic.Checked = !btnItalic.Checked;
            Designer.SelectedTextObjects.ToggleFontStyle(FontStyle.Italic, btnItalic.Checked);
        }

        private void btnUnderline_Click(object sender, EventArgs e)
        {
            btnUnderline.Checked = !btnUnderline.Checked;
            Designer.SelectedTextObjects.ToggleFontStyle(FontStyle.Underline, btnUnderline.Checked);
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetHAlign(HorzAlign.Left);
        }

        private void btnCenter_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetHAlign(HorzAlign.Center);
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetHAlign(HorzAlign.Right);
        }

        private void btnJustify_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetHAlign(HorzAlign.Justify);
        }

        private void btnTop_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetVAlign(VertAlign.Top);
        }

        private void btnMiddle_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetVAlign(VertAlign.Center);
        }

        private void btnBottom_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetVAlign(VertAlign.Bottom);
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetTextColor(btnTextColor.DefaultColor);
        }

        private void btnRotation_Click(object sender, EventArgs e)
        {
            AnglePopup popup = new AnglePopup(Designer.FindForm());
            popup.Angle = Designer.SelectedTextObjects.First.Angle;
            popup.AngleChanged += popup_RotationChanged;
            popup.Show(this, barText.Right, barText.Bottom);
        }

        private void popup_RotationChanged(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetAngle((sender as AnglePopup).Angle);
        }

        //-------------------------------------------------------------------
        // Border and Fill
        //-------------------------------------------------------------------

        private void btnTopLine_Click(object sender, EventArgs e)
        {
            btnTopLine.Checked = !btnTopLine.Checked;
            Designer.SelectedReportComponents.ToggleLine(BorderLines.Top, btnTopLine.Checked);
        }

        private void btnBottomLine_Click(object sender, EventArgs e)
        {
            btnBottomLine.Checked = !btnBottomLine.Checked;
            Designer.SelectedReportComponents.ToggleLine(BorderLines.Bottom, btnBottomLine.Checked);
        }

        private void btnLeftLine_Click(object sender, EventArgs e)
        {
            btnLeftLine.Checked = !btnLeftLine.Checked;
            Designer.SelectedReportComponents.ToggleLine(BorderLines.Left, btnLeftLine.Checked);
        }

        private void btnRightLine_Click(object sender, EventArgs e)
        {
            btnRightLine.Checked = !btnRightLine.Checked;
            Designer.SelectedReportComponents.ToggleLine(BorderLines.Right, btnRightLine.Checked);
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            Designer.SelectedReportComponents.ToggleLine(BorderLines.All, true);
        }

        private void btnNone_Click(object sender, EventArgs e)
        {
            Designer.SelectedReportComponents.ToggleLine(BorderLines.All, false);
        }

        private void btnLineColor_Click(object sender, EventArgs e)
        {
            Designer.SelectedReportComponents.SetLineColor(btnLineColor.DefaultColor);
        }

        private void btnFillColor_Click(object sender, EventArgs e)
        {
            Designer.SelectedReportComponents.SetColor(btnFillColor.DefaultColor);
        }

        private void btnFillProps_Click(object sender, EventArgs e)
        {
            Designer.SelectedReportComponents.InvokeFillEditor();
        }

        private void cbxWidth_WidthSelected(object sender, EventArgs e)
        {
            Designer.SelectedReportComponents.SetWidth(btnLineWidth.LineWidth);
        }

        private void cbxLineStyle_StyleSelected(object sender, EventArgs e)
        {
            Designer.SelectedReportComponents.SetLineStyle(btnLineStyle.LineStyle);
        }

        private void btnBorderProps_Click(object sender, EventArgs e)
        {
            Designer.SelectedReportComponents.InvokeBorderEditor();
        }

        //-------------------------------------------------------------------
        // Format
        //-------------------------------------------------------------------

        private void btnHighlight_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.InvokeHighlightEditor();
        }

        private void btnFormat_Click(object sender, EventArgs e)
        {
            using (FormatEditorForm form = new FormatEditorForm())
            {
                SelectedTextBaseObjects FTextObjects = new SelectedTextBaseObjects(designer);
                FTextObjects.Update();

                form.TextObject = FTextObjects.First;
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    FTextObjects.SetFormat(form.Formats);
                    Designer.SetModified(null, "Change");
                    //Change();
                }
            }
        }

        //-------------------------------------------------------------------
        // Styles
        //-------------------------------------------------------------------

        private void cbxStyle_StyleSelected(object sender, EventArgs e)
        {
            (Designer.ActiveReportTab.ActivePageDesigner as ReportPageDesigner).Workspace.Focus();
            Designer.SelectedReportComponents.SetStyle(cbxStyles.Style);
        }

        //-------------------------------------------------------------------
        #endregion
        #region Report Methods

        private void btnPreview_Click(object sender, EventArgs e)
        {
            previewTimer.Start();
        }

        private void previewTimer_Tick(object sender, EventArgs e)
        {
            previewTimer.Stop();
            Designer.cmdPreview.Invoke(sender, e);
        }

        private void miInsertBands_Click(object sender, EventArgs e)
        {
            using (ConfigureBandsForm form = new ConfigureBandsForm(Designer))
            {
                form.Page = Page as ReportPage;
                form.ShowDialog(this);
            }
        }

        private void miReportGroupExpert_Click(object sender, EventArgs e)
        {
            using (GroupExpertForm form = new GroupExpertForm(Designer))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    Designer.SetModified(null, "ChangeReport");
            }
        }

        private void miReportTitle_Click(object sender, EventArgs e)
        {
            ReportPage page = Page as ReportPage;
            if ((sender as CheckBoxItem).Checked)
            {
                page.ReportTitle = new ReportTitleBand();
                ReportPageDesigner.SetDefaults(page.ReportTitle);
            }
            else
            {
                page.ReportTitle = null;
            }
            ReportPageDesigner.Change();
        }

        private void miReportSummary_Click(object sender, EventArgs e)
        {
            ReportPage page = Page as ReportPage;
            if ((sender as CheckBoxItem).Checked)
            {
                page.ReportSummary = new ReportSummaryBand();
                ReportPageDesigner.SetDefaults(page.ReportSummary);
            }
            else
            {
                page.ReportSummary = null;
            }
            ReportPageDesigner.Change();
        }

        private void miPageHeader_Click(object sender, EventArgs e)
        {
            ReportPage page = Page as ReportPage;
            if ((sender as CheckBoxItem).Checked)
            {
                page.PageHeader = new PageHeaderBand();
                ReportPageDesigner.SetDefaults(page.PageHeader);
            }
            else
            {
                page.PageHeader = null;
            }
            ReportPageDesigner.Change();
        }

        private void miPageFooter_Click(object sender, EventArgs e)
        {
            ReportPage page = Page as ReportPage;
            if ((sender as CheckBoxItem).Checked)
            {
                page.PageFooter = new PageFooterBand();
                ReportPageDesigner.SetDefaults(page.PageFooter);
            }
            else
            {
                page.PageFooter = null;
            }
            ReportPageDesigner.Change();
        }

        private void miColumnHeader_Click(object sender, EventArgs e)
        {
            ReportPage page = Page as ReportPage;
            if ((sender as CheckBoxItem).Checked)
            {
                page.ColumnHeader = new ColumnHeaderBand();
                ReportPageDesigner.SetDefaults(page.ColumnHeader);
            }
            else
            {
                page.ColumnHeader = null;
            }
            ReportPageDesigner.Change();
        }

        private void miColumnFooter_Click(object sender, EventArgs e)
        {
            ReportPage page = Page as ReportPage;
            if ((sender as CheckBoxItem).Checked)
            {
                page.ColumnFooter = new ColumnFooterBand();
                ReportPageDesigner.SetDefaults(page.ColumnFooter);
            }
            else
            {
                page.ColumnFooter = null;
            }
            ReportPageDesigner.Change();
        }

        private void miOverlay_Click(object sender, EventArgs e)
        {
            ReportPage page = Page as ReportPage;
            if ((sender as CheckBoxItem).Checked)
            {
                page.Overlay = new OverlayBand();
                ReportPageDesigner.SetDefaults(page.Overlay);
            }
            else
            {
                page.Overlay = null;
            }
            ReportPageDesigner.Change();
        }

        #endregion
        #region Layout Methods
        private void btnAlignToGrid_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.AlignToGrid();
        }

        private void btnAlignLefts_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.AlignLeft();
        }

        private void btnAlignCenters_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.AlignCenter();
        }

        private void btnAlignRights_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.AlignRight();
        }

        private void btnAlignTops_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.AlignTop();
        }

        private void btnAlignMiddles_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.AlignMiddle();
        }

        private void btnAlignBottoms_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.AlignBottom();
        }

        private void btnSameWidth_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.SameWidth();
        }

        private void btnSameHeight_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.SameHeight();
        }

        private void btnSameSize_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.SameSize();
        }

        private void btnCenterHorizontally_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.CenterHorizontally();
        }

        private void btnCenterVertically_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.CenterVertically();
        }

        private void btnSizeToGrid_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.SizeToGrid();
        }

        private void btnSpaceHorizontally_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.SpaceHorizontally();
        }

        private void btnIncreaseHorizontalSpacing_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.IncreaseHorizontalSpacing();
        }

        private void btnDecreaseHorizontalSpacing_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.DecreaseHorizontalSpacing();
        }

        private void btnRemoveHorizontalSpacing_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.RemoveHorizontalSpacing();
        }

        private void btnSpaceVertically_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.SpaceVertically();
        }

        private void btnIncreaseVerticalSpacing_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.IncreaseVerticalSpacing();
        }

        private void btnDecreaseVerticalSpacing_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.DecreaseVerticalSpacing();
        }

        private void btnRemoveVerticalSpacing_Click(object sender, EventArgs e)
        {
            Designer.SelectedComponents.RemoveVerticalSpacing();
        }
        #endregion
        #region View Methods
        private void MenuViewGrid_Click(object sender, EventArgs e)
        {
            ReportWorkspace.ShowGrid = btnViewGrid.Checked;
            Workspace.Refresh();
        }

        private void MenuViewGuides_Click(object sender, EventArgs e)
        {
            ReportWorkspace.ShowGuides = btnViewGuides.Checked;
            Workspace.Refresh();
        }

        private void MenuViewAutoGuides_Click(object sender, EventArgs e)
        {
            ReportWorkspace.AutoGuides = btnAutoGuides.Checked;
            Workspace.Refresh();
            UpdateControls();
        }

        private void MenuViewDeleteHGuides_Click(object sender, EventArgs e)
        {
            Workspace.DeleteHGuides();
        }

        private void MenuViewDeleteVGuides_Click(object sender, EventArgs e)
        {
            Workspace.DeleteVGuides();
        }

        private void miViewUnits_Click(object sender, EventArgs e)
        {
            if (sender == btnUnitsMillimeters)
                ReportWorkspace.Grid.GridUnits = PageUnits.Millimeters;
            else if (sender == btnUnitsCentimeters)
                ReportWorkspace.Grid.GridUnits = PageUnits.Centimeters;
            else if (sender == btnUnitsInches)
                ReportWorkspace.Grid.GridUnits = PageUnits.Inches;
            else
                ReportWorkspace.Grid.GridUnits = PageUnits.HundrethsOfInch;

            UpdateContent();
        }

        private void openfrx_DragEnter(object sender, DragEventArgs e)
        {
            string[] objects = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (objects != null) 
            if (objects.Length == 1)
            {
                if (Path.GetExtension(objects[0]).ToLower() == ".frx")
                {
                    e.Effect = DragDropEffects.Move;
                }
            }
        }

        private void openfrx_DragDrop(object sender, DragEventArgs e)
        {
            string[] objects = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (objects.Length == 1)
            {
                if (Path.GetExtension(objects[0]).ToLower() == ".frx")
                {
                    Designer.cmdOpen.LoadFile(objects[0]);
                }
            }
        }
        #endregion
        #endregion

        #region IDesignerPlugin
        /// <inheritdoc/>
        public string PluginName
        {
            get { return Name; }
        }

        /// <inheritdoc/>
        public void SaveState()
        {
        }

        /// <inheritdoc/>
        public void RestoreState()
        {
        }

        /// <inheritdoc/>
        public void SelectionChanged()
        {
            UpdateContent();
        }

        /// <inheritdoc/>
        public void UpdateContent()
        {
            UpdateZoom();
            UpdateControls();
        }

        /// <inheritdoc/>
        public void Lock()
        {
        }

        /// <inheritdoc/>
        public void Unlock()
        {
            UpdateContent();
        }

        /// <inheritdoc/>
        public void Localize()
        {
            LocalizeFile();
            localizeHome();
            LocalizeReport();
            localizeLayout();
            LocalizeView();

            MyRes res = new MyRes("Designer,Ribbon");

            btnFile.Text = res.Get("File");//.ToUpper();
            tabHome.Text = res.Get("Home");//.ToUpper();
            tabReport.Text = res.Get("Report");//.ToUpper();
            tabLayout.Text = res.Get("Layout");//.ToUpper();
            tabView.Text = res.Get("View");//.ToUpper();

            lblRecent.Text = res.Get("Recent");
            barReport.Text = res.Get("Report");
            barLayout.Text = res.Get("Layout");
            barView.Text = res.Get("View");
            barClipboard.Text = res.Get("Clipboard");
            barText.Text = res.Get("Text");
            barBorderAndFill.Text = res.Get("BorderAndFill");
            barFormat.Text = res.Get("Format");
            barStyles.Text = res.Get("Styles");
            barEditing.Text = res.Get("Editing");
            barData.Text = res.Get("Data");
            barPages.Text = res.Get("Pages");
            barBands.Text = res.Get("Bands");
            btnAlignment.Text = res.Get("Alignment");
            btnSize.Text = res.Get("Size");
            btnSpacing.Text = res.Get("Spacing");
            btnPanels.Text = res.Get("Panels");

            //trying to refresh controls
            //RibbonTabItem tab = ribbonControl.SelectedRibbonTabItem;
            //ribbonControl.SelectedRibbonTabItem = tabHome;
            //ribbonControl.SelectedRibbonTabItem = tabReport;
            //ribbonControl.SelectedRibbonTabItem = tabLayout;
            //ribbonControl.SelectedRibbonTabItem = tabView;
            //ribbonControl.SelectedRibbonTabItem = tab;
            //tabHome.Select();
            //tabReport.Select();
            //tabLayout.Select();
            //tabView.Select();
            //tab.Select();

            UpdateContent();
        }

        /// <inheritdoc/>
        public DesignerOptionsPage GetOptionsPage()
        {
            return null;
        }

        /// <inheritdoc/>
        public void UpdateUIStyle()
        {
            if (Config.UseRibbon)
            {
                designer.ShowMainMenu = false;
                ribbonControl.Visible = true;

                foreach (Bar bar in designer.DotNetBarManager.Bars)
                    if (bar is ToolbarBase)
                        bar.Hide();
            }
            else
            {
                designer.ShowMainMenu = true;
                ribbonControl.Visible = false;
            }

            btnTextColor.SetStyle(designer.UIStyle);
            btnFillColor.SetStyle(designer.UIStyle);
            btnLineColor.SetStyle(designer.UIStyle);

            statusBar.Refresh();
        }

        /// <inheritdoc/>
        public void ReinitDpiSize()
        {
            // status bar
            this.statusBar.Font = DpiHelper.ConvertUnits(new System.Drawing.Font("Segoe UI", 9F), true);
            this.statusBar.Size = DpiHelper.ConvertUnits(new System.Drawing.Size(969, 26), true);
            location.Image = Res.GetImage(62);
            size.Image = Res.GetImage(63);
            zoom1.Image = ResourceLoader.GetBitmap("ZoomPageWidth.png");
            zoom2.Image = ResourceLoader.GetBitmap("ZoomWholePage.png");
            zoom3.Image = ResourceLoader.GetBitmap("Zoom100.png");

            this.zoomLabel.Width = DpiHelper.ConvertUnits(40);
            slider.UpdateDpiDependencies();
            this.statusBar.UpdateDpiDependencies();
        }
        #endregion
    }
}