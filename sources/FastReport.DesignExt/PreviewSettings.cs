using System;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Preview;
using System.Drawing.Design;

namespace FastReport
{
    /// <summary>
    /// Specifies the set of buttons available in the preview.
    /// </summary>
    [Flags]
    [EditorAttribute("FastReport.TypeEditors.FlagsEditor, FastReport", typeof(UITypeEditor))]
    public enum PreviewButtons
    {
        /// <summary>
        /// No buttons visible.
        /// </summary>
        None = 0,

        /// <summary>
        /// The "Print" button is visible.
        /// </summary>
        Print = 1,

        /// <summary>
        /// The "Open" button is visible.
        /// </summary>
        Open = 2,

        /// <summary>
        /// The "Save" button is visible.
        /// </summary>
        Save = 4,

        /// <summary>
        /// The "Email" button is visible.
        /// </summary>
        Email = 8,

        /// <summary>
        /// The "Find" button is visible.
        /// </summary>
        Find = 16,

        /// <summary>
        /// The zoom buttons are visible.
        /// </summary>
        Zoom = 32,

        /// <summary>
        /// The "Outline" button is visible.
        /// </summary>
        Outline = 64,

        /// <summary>
        /// The "Page setup" button is visible.
        /// </summary>
        PageSetup = 128,

        /// <summary>
        /// The "Edit" button is visible.
        /// </summary>
        Edit = 256,

        /// <summary>
        /// The "Watermark" button is visible.
        /// </summary>
        Watermark = 512,

        /// <summary>
        /// The page navigator buttons are visible.
        /// </summary>
        Navigator = 1024,

        /// <summary>
        /// The "Close" button is visible.
        /// </summary>
        Close = 2048,

        /// <summary>
        /// The "Design" button is visible.
        /// </summary>
        Design = 4096,

        /// <summary>
        /// The "Copy Page" button is visible.
        /// </summary>
        CopyPage = 8192,

        /// <summary>
        /// The "Delete Page" button is visible.
        /// </summary>
        DeletePage = 16384,

        /// <summary>
        /// The "About" button is visible.
        /// </summary>
        About = 32768,

        /// <summary>
        /// All buttons are visible.
        /// </summary>
        // if you add something to this enum, don't forget to correct "All" member
        All = Print | Open | Save | Email | Find | Zoom | Outline | PageSetup | Edit |
         Watermark | Navigator | Close | CopyPage | DeletePage | About
    }

    /// <summary>
    /// Specifies the set of export buttons available in the preview.
    /// </summary>
    [Flags]
    [EditorAttribute("FastReport.TypeEditors.FlagsEditor, FastReport", typeof(UITypeEditor))]
    public enum PreviewExports
    {
        /// <summary>
        /// No exports visible.
        /// </summary>
        None = 0,

        /// <summary>
        /// The "Prepared" button is visible.
        /// </summary>
        Prepared = 1,

        /// <summary>
        /// The "PDFExport" button is visible.
        /// </summary>
        PDFExport = 2,

        /// <summary>
        /// The "RTFExport" button is visible.
        /// </summary>
        RTFExport = 4,

        /// <summary>
        /// The "HTMLExport" button is visible.
        /// </summary>
        HTMLExport = 8,

        /// <summary>
        /// The "MHTExport" button is visible.
        /// </summary>
        MHTExport = 16,

        /// <summary>
        /// The "XMLExport" export button is visible.
        /// </summary>
        XMLExport = 32,

        /// <summary>
        /// The "Excel2007Export" button is visible.
        /// </summary>
        Excel2007Export = 64,

        /// <summary>
        /// The "Excel2003Document" button is visible.
        /// </summary>
        Excel2003Document = 128,

        /// <summary>
        /// The "Word2007Export" button is visible.
        /// </summary>
        Word2007Export = 256,

        /// <summary>
        /// The "PowerPoint2007Export" button is visible.
        /// </summary>
        PowerPoint2007Export = 512,

        /// <summary>
        /// The "ODSExport" button is visible.
        /// </summary>
        ODSExport = 1024,

        /// <summary>
        /// The "ODTExport" button is visible.
        /// </summary>
        ODTExport = 2048,

        /// <summary>
        /// The "XPSExport" export button is visible.
        /// </summary>
        XPSExport = 4096,

        /// <summary>
        /// The "CSVExport" button is visible.
        /// </summary>
        CSVExport = 8192,

        /// <summary>
        /// The "DBFExport" button is visible.
        /// </summary>
        DBFExport = 16384,

        /// <summary>
        /// The "TextExport" button is visible.
        /// </summary>
        TextExport = 32768,

        /// <summary>
        /// The "ZplExport" button is visible.
        /// </summary>
        ZplExport = 65536,

        /// <summary>
        /// The "ImageExport" button is visible.
        /// </summary>
        ImageExport = 131072,

        /// <summary>
        /// The "XAMLExport" button is visible.
        /// </summary>
        XAMLExport = 262144,

        /// <summary>
        /// The "SVGExport" button is visible.
        /// </summary>
        SVGExport = 524288,
        
        /// <summary>
        /// The "PPMLExport" button is visible.
        /// </summary>
        PPMLExport = 1048576,
        
        /// <summary>
        /// The "PSExport" button is visible.
        /// </summary>
        PSExport = 2097152,
        
        /// <summary>
        /// The "JsonExport" button is visible.
        /// </summary>
        JsonExport = 4194304,

        /// <summary>
        /// The "LaTeXExport" button is visible.
        /// </summary>
        LaTeXExport = 8388608,

        /// <summary>
        /// The "HpglExport" button is visible.
        /// </summary>
        HpglExport = 16777216,

        /// <summary>
        /// The All export buttons is visible.
        /// </summary>
        All = Prepared | PDFExport | RTFExport | HTMLExport | MHTExport | XMLExport | Excel2007Export | Excel2003Document |
              Word2007Export | PowerPoint2007Export | ODSExport | ODTExport | XPSExport | CSVExport | DBFExport | TextExport |
              ZplExport | ImageExport | XAMLExport | SVGExport | PPMLExport | PSExport | JsonExport | LaTeXExport | HpglExport
    }

    /// <summary>
    /// Specifies the set of export in clouds buttons available in the preview.
    /// </summary>
    [Flags]
    [EditorAttribute("FastReport.TypeEditors.FlagsEditor, FastReport", typeof(UITypeEditor))]
    public enum PreviewClouds
    {
        /// <summary>
        /// No exports in clouds visible.
        /// </summary>
        None = 0,
        
        /// <summary>
        /// The "Box" button is visible.
        /// </summary>
        Box = 1,
        
        /// <summary>
        /// The "Dropbox" button is visible.
        /// </summary>
        Dropbox = 2,
        
        /// <summary>
        /// The "FastCloud" button is visible.
        /// </summary>
        FastCloud = 4,
        
        /// <summary>
        /// The "Ftp" button is visible.
        /// </summary>
        Ftp = 8,
        
        /// <summary>
        /// The "GoogleDrive" button is visible.
        /// </summary>
        GoogleDrive = 16,
        
        /// <summary>
        /// The "SkyDrive" button is visible.
        /// </summary>
        SkyDrive = 32,
        
        /// <summary>
        /// The All export in clouds buttons is visible.
        /// </summary>
        All = Box | Dropbox | FastCloud | Ftp | GoogleDrive | SkyDrive
    }

    /// <summary>
    /// Specifies the set of export by messenger buttons available in the preview.
    /// </summary>
    [Flags]
    [EditorAttribute("FastReport.TypeEditors.FlagsEditor, FastReport", typeof(UITypeEditor))]
    public enum PreviewMessengers
    {
        /// <summary>
        /// No exports by messengers visible.
        /// </summary>
        None = 0,

        /// <summary>
        /// The "Xmpp" button is visible.
        /// </summary>
        Xmpp = 1,

        /// <summary>
        /// The All export my messengers buttons is visible.
        /// </summary>
        All = Xmpp
    }

    /// <summary>
    /// Contains some settings of the preview window.
    /// </summary>
    [TypeConverter(typeof(FastReport.TypeConverters.FRExpandableObjectConverter))]
    public class PreviewSettings
    {
        #region Fields
        private PreviewButtons buttons;
        private PreviewExports exports;
        private PreviewClouds clouds;
        private PreviewMessengers messengers;
        private int pagesInCache;
        private bool showInTaskbar;
        private bool topMost;
        private Icon icon;
        private string text;
        private bool fastScrolling;
        private bool allowPrintToFile;
        private string saveInitialDirectory;
        #endregion

        #region Properties
        /// <summary>
        /// Occurs when the standard preview window opened.
        /// </summary>
        /// <remarks>
        /// You may use this event to change the standard preview window, for example, add an own button to it.
        /// The <b>sender</b> parameter in this event is the <b>PreviewControl</b>.
        /// </remarks>
        public event EventHandler PreviewOpened;

        /// <summary>
        /// Gets or sets a set of buttons that will be visible in the preview's toolbar.
        /// </summary>
        /// <example>
        /// Here is an example how you can disable the "Print" and "EMail" buttons:
        /// <code>
        /// Config.PreviewSettings.Buttons = PreviewButtons.Open | 
        /// PreviewButtons.Save | 
        /// PreviewButtons.Find | 
        /// PreviewButtons.Zoom | 
        /// PreviewButtons.Outline | 
        /// PreviewButtons.PageSetup | 
        /// PreviewButtons.Edit | 
        /// PreviewButtons.Watermark | 
        /// PreviewButtons.Navigator | 
        /// PreviewButtons.Close;
        /// </code>
        /// </example>
        [DefaultValue(PreviewButtons.All)]
        public PreviewButtons Buttons
        {
            get { return buttons; }
            set { buttons = value; }
        }

        /// <summary>
        /// Specifies the set of exports that will be available in the preview's "save" menu.
        /// </summary>
        [DefaultValue(PreviewExports.All)]
        public PreviewExports Exports
        {
            get { return exports; }
            set { exports = value; }
        }

        /// <summary>
        /// Specifies the set of exports in clouds that will be available in the preview's "save" menu.
        /// </summary>
        [DefaultValue(PreviewClouds.All)]
        public PreviewClouds Clouds
        {
            get { return clouds; }
            set { clouds = value; }
        }

        /// <summary>
        /// Specifies the set of exports by messengers that will be available in the preview's "save" menu.
        /// </summary>
        [DefaultValue(PreviewMessengers.All)]
        public PreviewMessengers Messengers
        {
            get { return messengers; }
            set { messengers = value; }
        }

        /// <summary>
        /// Gets or sets the number of prepared pages that can be stored in the memory cache during preview.
        /// </summary>
        /// <remarks>
        /// Decrease this value if your prepared report contains a lot of pictures. This will
        /// save the RAM memory.
        /// </remarks>
        [DefaultValue(50)]
        public int PagesInCache
        {
            get { return pagesInCache; }
            set { pagesInCache = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the preview window is displayed in the Windows taskbar. 
        /// </summary>
        [DefaultValue(false)]
        public bool ShowInTaskbar
        {
            get { return showInTaskbar; }
            set { showInTaskbar = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the preview window should be displayed as a topmost form. 
        /// </summary>
        [DefaultValue(false)]
        public bool TopMost
        {
            get { return topMost; }
            set { topMost = value; }
        }

        /// <summary>
        /// Gets or sets the icon for the preview window.
        /// </summary>
        public Icon Icon
        {
            get
            {
                if (icon == null)
                    icon = ResourceLoader.GetIcon("icon16.ico");
                return icon;
            }
            set { icon = value; }
        }

        /// <summary>
        /// Gets or sets the text for the preview window.
        /// </summary>
        /// <remarks>
        /// If no text is set, the default text "Preview" will be used.
        /// </remarks>
        public string Text
        {
            get { return text; }
            set { text = value; }
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
        /// Enables or disables the "Print to file" feature in the print dialog.
        /// </summary>
        [DefaultValue(true)]
        public bool AllowPrintToFile
        {
            get { return allowPrintToFile; }
            set { allowPrintToFile = value; }
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

        internal void OnPreviewOpened(PreviewControl sender)
        {
            if (PreviewOpened != null)
                PreviewOpened(sender, EventArgs.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the <b>PreviewSettings</b> class with default settings. 
        /// </summary>
        public PreviewSettings()
        {
            buttons = PreviewButtons.All;
            exports = PreviewExports.All;
            clouds = PreviewClouds.All;
            messengers = PreviewMessengers.All;
            pagesInCache = 50;
            text = "";
            allowPrintToFile = true;
        }
    }
}