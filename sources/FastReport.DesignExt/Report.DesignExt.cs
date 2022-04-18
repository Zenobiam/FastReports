using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Security;
using System.Drawing.Printing;
using FastReport.Utils;
using FastReport.Code;
using FastReport.Data;
using FastReport.Engine;
using FastReport.Dialog;
using FastReport.Export;
using FastReport.Design;
using FastReport.Forms;
using FastReport.Design.StandardDesigner;

namespace FastReport
{
    [ToolboxItem(true), ToolboxBitmap(typeof(Report), "Resources.Report.bmp")]
    [Designer("FastReport.VSDesign.ReportComponentDesigner, FastReport.VSDesign, Version=1.0.0.0, Culture=neutral, PublicKeyToken=db7e5ce63278458c, processorArchitecture=MSIL")]
    [DesignerSerializer("FastReport.VSDesign.ReportCodeDomSerializer, FastReport.VSDesign, Version=1.0.0.0, Culture=neutral, PublicKeyToken=db7e5ce63278458c, processorArchitecture=MSIL", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    partial class Report
    {
        #region Fields
        private Designer designer;
        private Form designerForm;
#if !MONO
        private SplashForm splashForm;
#endif

#endregion

#region Properties
        /// <summary>
        /// Gets a reference to the report designer.
        /// </summary>
        /// <remarks>
        /// This property can be used when report is designing. In other cases it returns <b>null</b>.
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Designer Designer
        {
            get { return designer; }
            set { designer = value; }
        }

        internal bool IsVSDesignMode
        {
            get { return DesignMode; }
        }


#endregion

#region Private Methods
        private void OnCloseDesigner(Object sender, FormClosedEventArgs e)
        {
            modified = designer.Modified;
            designerForm.Dispose();
            designerForm = null;
            designer = null;
            foreach (Base c in AllObjects)
                c.SetDesigning(false);
        }

#if !MONO
        private void ShowSplashScreen()
        {
            splashForm = new SplashForm();
            Application.AddMessageFilter(splashForm);
            splashForm.Show();
            Config.DesignerSettings.DesignerLoaded += HideSplashScreen;
        }

        private void HideSplashScreen(object sender, EventArgs e)
        {
            Config.DesignerSettings.DesignerLoaded -= HideSplashScreen;
            splashForm.Hide();

            // without message filter and delay all events that were triggered on the splashscreen will be redirected to the main form
            Timer t = new Timer();
            t.Interval = 500;
            t.Tick += delegate (object sender_, EventArgs e_)
            {
                t.Stop();
                Application.RemoveMessageFilter(splashForm);
            };
            t.Start();
        }
#endif
#endregion

#region Public Methods
        /// <summary>
        /// Runs the report designer.
        /// </summary>
        /// <returns><b>true</b> if report was modified, otherwise <b>false</b>.</returns>
        public bool Design()
        {
            return Design(true);
        }

        /// <summary>
        /// Runs the report designer.
        /// </summary>
        /// <param name="modal">A value indicates whether the designer should run modally.</param>
        /// <returns><b>true</b> if report was modified, otherwise <b>false</b>.</returns>
        public bool Design(bool modal)
        {
            return Design(modal, null);
        }

        /// <summary>
        /// Runs the report designer.
        /// </summary>
        /// <param name="mdiParent">The main MDI form which will be a parent for the designer.</param>
        /// <returns><b>true</b> if report was modified, otherwise <b>false</b>.</returns>
        public bool Design(Form mdiParent)
        {
            return Design(false, mdiParent);
        }

        private bool Design(bool modal, Form mdiParent)
        {
            if (designer != null)
                return false;

            EnsureInit();

#if !MONO
            if (Config.SplashScreenEnabled)
                ShowSplashScreen();
#endif
            designerForm = new DesignerForm(true);
            (designerForm as DesignerForm).Designer.Report = this;

            designerForm.MdiParent = mdiParent;
            designerForm.ShowInTaskbar = Config.DesignerSettings.ShowInTaskbar;
            designerForm.FormClosed += new FormClosedEventHandler(OnCloseDesigner);

            if (modal)
                designerForm.ShowDialog();
            else
                designerForm.Show();

            return modified;
        }

        internal bool DesignPreviewPage()
        {
            Designer saveDesigner = Designer;

            Form designerForm = new DesignerForm(
#if MONO
			    false 
#endif
			);
            designer = (designerForm as DesignerForm).Designer;

            try
            {
                designer.Restrictions.DontChangePageOptions = true;
                designer.Restrictions.DontChangeReportOptions = true;
                designer.Restrictions.DontCreatePage = true;
                designer.Restrictions.DontCreateReport = true;
                designer.Restrictions.DontDeletePage = true;
                designer.Restrictions.DontCopyPage = true;
                designer.Restrictions.DontEditData = true;
                designer.Restrictions.DontInsertBand = true;
                designer.Restrictions.DontLoadReport = true;
                designer.Restrictions.DontPreviewReport = true;
                designer.Restrictions.DontShowRecentFiles = true;
                designer.IsPreviewPageDesigner = true;
                designer.Report = this;
                designer.SelectionChanged(null);
                designerForm.ShowDialog();
            }
            finally
            {
                modified = designer.Modified;
                designerForm.Dispose();
                designer = saveDesigner;
            }
            return modified;
        }
#endregion

        private string ShowPaswordForm(string password)
        {
            using (AskPasswordForm form = new AskPasswordForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                    return form.Password;
            }
            return password;
        }

        private void SerializeDesign(FRWriter writer, Report c)
        {
            PrintSettings.Serialize(writer, c.PrintSettings);
            EmailSettings.Serialize(writer, c.EmailSettings);
        }

        private void InitDesign()
        {
            printSettings = new PrintSettings();
            emailSettings = new EmailSettings();
        }

        private void ClearDesign()
        {
            PrintSettings.Clear();
            EmailSettings.Clear();
        }

        private void DisposeDesign()
        {
            printSettings.Dispose();
        }
    }
}