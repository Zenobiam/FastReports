using System;
using System.Drawing;
using System.IO;
using System.Web;
using System.Windows.Forms;
using FastReport.Design;

namespace FastReport.Utils
{
    partial class Config
    {
        #region Private Fields
#if MONO
		const UIStyle DEFAULT_UISTYLE = UIStyle.Office2003;
#else
        const UIStyle DEFAULT_UISTYLE = UIStyle.VisualStudio2012Light;
#endif

        private static Control mainForm;
        private static DesignerSettings FDesignerSettings = new DesignerSettings();
        private static bool FSplashScreenEnabled = false;
        private static UIStyle FUIStyle = DEFAULT_UISTYLE;
        private static bool FUseRibbon = true;
        private static bool processEvents = false;
#if COMMUNITY
        private static Image splashScreen;
        private static Image welcomeScreen;
#endif

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets the main form of application.
        /// </summary>
        public static Control MainForm { get { return mainForm; } set { mainForm = value; } }

        /// <summary>
        /// Gets or sets the settings for the report designer window.
        /// </summary>
        public static DesignerSettings DesignerSettings
        {
            get { return FDesignerSettings; }
            set { FDesignerSettings = value; }
        }

        /// <summary>
        /// Gets or sets the UI style.
        /// </summary>
        /// <remarks>
        /// This property affects both designer and preview windows.
        /// </remarks>
        public static UIStyle UIStyle
        {
            get { return FUIStyle; }
            set { FUIStyle = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Ribbon UI should be used
        /// </summary>
        public static bool UseRibbon
        {
            get
            {
#if COMMUNITY
                return false;
#else
                return FUseRibbon;
#endif
            }
            set { FUseRibbon = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether SplashScreen should be displayed while loading designer
        /// </summary>
        public static bool SplashScreenEnabled
        {
            get { return FSplashScreenEnabled; }
            set { FSplashScreenEnabled = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Welcome window feature enabled.
        /// If false, interface elements associated with the Welcome window will not be visible.
        /// </summary>
        public static bool WelcomeEnabled
        {
            get { return Root.FindItem("Designer").FindItem("Welcome").GetProp("Enabled") != "False"; }
            set { Root.FindItem("Designer").FindItem("Welcome").SetProp("Enabled", Converter.ToString(value)); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Welcome window shoud be displayed on startup
        /// </summary>
        public static bool WelcomeShowOnStartup
        {
            get { return Root.FindItem("Designer").FindItem("Welcome").GetProp("ShowOnStartup") != "False"; }
            set { Root.FindItem("Designer").FindItem("Welcome").SetProp("ShowOnStartup", Converter.ToString(value)); }
        }

        /// <summary>
        /// Gets the folder to store auto save files
        /// </summary>
        public static string AutoSaveFolder
        {
            get { return Path.Combine(GetTempFolder(), "FastReport"); }
        }

        /// <summary>
        /// Gets the autosaved report
        /// </summary>
        public static string AutoSaveFile
        {
            get { return Path.Combine(AutoSaveFolder, "autosave.frx"); }
        }

        /// <summary>
        /// Gets the autosaved report path
        /// </summary>
        public static string AutoSaveFileName
        {
            get { return Path.Combine(AutoSaveFolder, "autosave.txt"); }
        }

        /// <summary>
        /// Is necessary to process abort and some other events in parallel 
        /// </summary>
        public static bool ProcessEvents
        {
            get { return processEvents; }
            set { processEvents = value; }
        }

        /// <summary>
        /// Gets a value indicating that the ASP.NET hosting permission level is set to full trust.
        /// </summary>
        public static bool FullTrust
        {
            get
            {
                return GetCurrentTrustLevel() == AspNetHostingPermissionLevel.Unrestricted;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether to disable some functionality to run in web mode.
        /// </summary>
        /// <remarks>
        /// Use this property if you use FastReport in ASP.Net. Set this property to <b>true</b> <b>before</b>
        /// you access any FastReport.Net objects.
        /// </remarks>
        public static bool WebMode
        {
            get
            {
                return FWebMode;
            }
            set
            {
                FWebMode = value;

                if (value)
                    ReportSettings.ShowProgress = false;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Restores the form state from the configuration file.
        /// </summary>
        /// <param name="form">The form to restore.</param>
        public static void RestoreFormState(Form form)
        {
            RestoreFormState(form, false);
        }

        /// <summary>
        /// Saves the form state to the configuration file.
        /// </summary>
        /// <param name="form">The form to save.</param>
        public static void SaveFormState(Form form)
        {
            SaveFormState(form.Name, form.WindowState == FormWindowState.Maximized, form.WindowState == FormWindowState.Minimized,
            form.Location, form.Size);
        }

        /// <summary>
        /// Saves the form state to the configuration file.
        /// </summary>
        /// <param name="formName">The name of the form.</param>
        /// <param name="isMaximized">True if the form is in maximized state.</param>
        /// <param name="isMinimized">True if the form is in minimized state.</param>
        /// <param name="location">The location of the form.</param>
        /// <param name="size">The size of the form.</param>
        public static void SaveFormState(String formName, bool isMaximized, bool isMinimized, Point location, Size size)
        {
            XmlItem xi = FDoc.Root.FindItem("Forms").FindItem(formName);
            xi.SetProp("Maximized", isMaximized ? "1" : "0");
            xi.SetProp("Left", isMinimized ? "0" : location.X.ToString());
            xi.SetProp("Top", isMinimized ? "0" : location.Y.ToString());
            xi.SetProp("Width", size.Width.ToString());
            xi.SetProp("Height", size.Height.ToString());
        }

        #endregion Public Methods

        #region Internal Methods

        // we need this to prevent form.Load event to be fired *after* the form is displayed.
        // Used in the StandardDesignerForm.Load event
        internal static bool RestoreFormState(Form form, bool ignoreWindowState)
        {
            XmlItem xi = FDoc.Root.FindItem("Forms").FindItem(form.Name);
            string left = xi.GetProp("Left");
            string top = xi.GetProp("Top");
            string width = xi.GetProp("Width");
            string height = xi.GetProp("Height");

            // Get current screen working area
            Rectangle screenWorkingArea = Screen.GetWorkingArea(form);
            int windowLeftPosition = screenWorkingArea.Left;
            int windowTopPosition = screenWorkingArea.Top;
            int windowWidth = screenWorkingArea.Width;
            int windowHeight = screenWorkingArea.Height;

            // Get saved left and top positions
            if (left != "" && top != "")
            {
                windowLeftPosition = int.Parse(left);
                windowTopPosition = int.Parse(top);
                form.Location = new Point(windowLeftPosition, windowTopPosition);
            }

            // Get saved width and height
            if (width != "" && height != "")
            {
                windowWidth = int.Parse(width);
                windowHeight = int.Parse(height);
                form.Size = new Size(windowWidth, windowHeight);
            }

            Rectangle formRect = new Rectangle(windowLeftPosition, windowTopPosition,
                windowWidth, windowHeight);

            // Check a visibility of form rectangle on any screen
            if (!IsVisibleOnAnyScreen(formRect))
            {
                form.StartPosition = FormStartPosition.WindowsDefaultLocation;
                form.Location = new Point(screenWorkingArea.Left, screenWorkingArea.Top);
            }

            // Set the window state
            if (!ignoreWindowState)
                form.WindowState = xi.GetProp("Maximized") == "1" ?
                    FormWindowState.Maximized : FormWindowState.Normal;

            return xi.GetProp("Maximized") == "1";
        }

        internal static void DoEvent()
        {
            if (ProcessEvents && !WebMode)
            {
                System.Windows.Forms.Application.DoEvents();
            }
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// Checks the visibility of rectangle area on currently connected screens with small gap.
        /// </summary>
        /// <param name="rect">Rectanle area for checking.</param>
        /// <returns>True for visible rect.</returns>
        private static bool IsVisibleOnAnyScreen(Rectangle rect)
        {
            Rectangle formRect = new Rectangle(rect.Left + 10, rect.Top + 10, rect.Width - 20, rect.Height - 20);
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(formRect))
                {
                    return true;
                }
            }
            return false;
        }

        private static AspNetHostingPermissionLevel GetCurrentTrustLevel()
        {
            foreach (AspNetHostingPermissionLevel trustLevel in
                new AspNetHostingPermissionLevel[] {
              AspNetHostingPermissionLevel.Unrestricted,
              AspNetHostingPermissionLevel.High,
              AspNetHostingPermissionLevel.Medium,
              AspNetHostingPermissionLevel.Low,
              AspNetHostingPermissionLevel.Minimal
            })
            {
                try
                {
                    new AspNetHostingPermission(trustLevel).Demand();
                }
                catch (System.Security.SecurityException)
                {
                    continue;
                }
                return trustLevel;
            }
            return AspNetHostingPermissionLevel.None;
        }

        private static void SaveUIStyle()
        {
            XmlItem xi = Root.FindItem("UIStyleNew");
            xi.SetProp("Style", Converter.ToString(UIStyle));
            xi.SetProp("Ribbon", Converter.ToString(UseRibbon));
        }

        private static void RestoreUIStyle()
        {
            XmlItem xi = Root.FindItem("UIStyleNew");
            string style = xi.GetProp("Style");

            if (!String.IsNullOrEmpty(style))
            {
                try
                {
                    UIStyle = (UIStyle)Converter.FromString(typeof(UIStyle), style);
                }
                catch
                {
                    UIStyle = DEFAULT_UISTYLE;
                }
            }

            string ribbon = xi.GetProp("Ribbon");

            if (!String.IsNullOrEmpty(ribbon))
            {
                FUseRibbon = ribbon != "False";
            }
        }

#if !COMMUNITY
        private static void SaveExportOptions()
        {
            ExportsOptions.GetInstance().SaveExportOptions();
        }

        private static void RestoreExportOptions()
        {
            ExportsOptions.GetInstance().RestoreExportOptions();
        }
#endif

#if COMMUNITY
        public static Image SplashScreen
        {
            get
            {
                return splashScreen;
            }
            set
            {
                splashScreen = value;
            }
        }

        public static Image WelcomeScreen
        {
            get
            {
                return welcomeScreen;
            }
            set
            {
                welcomeScreen = value;
            }
        }
#endif

#endregion Private Methods
    }
}