using FastReport.Barcode;
using FastReport.Cloud.StorageClient.Box;
using FastReport.Cloud.StorageClient.Dropbox;
using FastReport.Cloud.StorageClient.FastCloud;
using FastReport.Cloud.StorageClient.Ftp;
using FastReport.Cloud.StorageClient.GoogleDrive;
using FastReport.Cloud.StorageClient.SkyDrive;
using FastReport.CrossView;
using FastReport.Data;
using FastReport.Design;
using FastReport.Dialog;
using FastReport.Gauge.Radial;
using FastReport.Gauge;
using FastReport.Map;
using FastReport.Matrix;
using FastReport.Messaging.Xmpp;
using FastReport.Table;
using FastReport.TypeConverters;
using FastReport.TypeEditors;
using FastReport.Utils;
using FastReport.Wizards;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System;

namespace FastReport
{
    /// <summary>
    /// The FastReport.dll assembly initializer.
    /// </summary>
    public sealed class AssemblyInitializerDesignExt : AssemblyInitializerBase
    {

        #region Public Constructors

        /// <summary>
        /// Registers all standard objects, wizards, export filters.
        /// </summary>
        public AssemblyInitializerDesignExt()
        {
            RegisterObjects();
            RegisterMethods();
        }


        #endregion Public Constructors

        #region Private Methods

		private void RegisterObjects()
        {
            // data items
#if !COMMUNITY
#if !MONO
            RegisteredObjects.AddConnection(typeof(MsAccessDataConnection));
            RegisteredObjects.AddConnection(typeof(OleDbDataConnection));
            RegisteredObjects.AddConnection(typeof(OdbcDataConnection));
#endif
#endif
            RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));

#if !COMMUNITY
            RegisteredObjects.Add(typeof(MaskedTextBoxControl), "DialogPage", 147);
            RegisteredObjects.Add(typeof(NumericUpDownControl), "DialogPage", 146);
            RegisteredObjects.Add(typeof(PanelControl), "DialogPage", 144);
            RegisteredObjects.Add(typeof(GridControl), "DialogPage", 122);
            RegisteredObjects.Add(typeof(DataSelectorControl), "DialogPage", 128);
            RegisteredObjects.Add(typeof(ListViewControl), "DialogPage", 203);
            RegisteredObjects.Add(typeof(RichTextBoxControl), "DialogPage", 205);
            RegisteredObjects.Add(typeof(TreeViewControl), "DialogPage", 204);

#endif

            // wizards
            RegisteredObjects.AddWizard(typeof(StandardReportWizard), 133, "Wizards,StandardReport", ItemWizardEnum.Report);
            RegisteredObjects.AddWizard(typeof(LabelWizard), 133, "Wizards,Label", ItemWizardEnum.Report);
            RegisteredObjects.AddWizard(typeof(BlankReportWizard), 134, "Wizards,BlankReport", ItemWizardEnum.Report);
            RegisteredObjects.AddWizard(typeof(InheritedReportWizard), 134, "Wizards,InheritedReport", ItemWizardEnum.Report);
            RegisteredObjects.AddWizard(typeof(NewPageWizard), 135, "Wizards,NewPage", ItemWizardEnum.ReportItem);
#if !COMMUNITY
            RegisteredObjects.AddWizard(typeof(NewDialogWizard), 136, "Wizards,NewDialog", ItemWizardEnum.ReportItem);
#endif
            RegisteredObjects.AddWizard(typeof(NewDataSourceWizard), 137, "Wizards,NewDataSource", ItemWizardEnum.ReportItem);
            RegisteredObjects.AddWizard(typeof(FastM1nesweeperWizard), 250, "Wizards,FastM1nesweeper", ItemWizardEnum.Game);


            // export categories
            //RegisteredObjects.AddExportCategory("Uncategorized", "");
            //RegisteredObjects.AddExportCategory("Office", "Export,ExportGroups,Office", 190);
            //RegisteredObjects.AddExportCategory("XML", "Export,ExportGroups,XML", 191);
            //RegisteredObjects.AddExportCategory("Web", "Export,ExportGroups,Web", 246);
            //RegisteredObjects.AddExportCategory("Image", "Export,ExportGroups,Image", 103);
            //RegisteredObjects.AddExportCategory("DataBase", "Export,ExportGroups,DataBase", 53);
            //RegisteredObjects.AddExportCategory("Print", "Export,ExportGroups,Print", 195);
            //RegisteredObjects.AddExportCategory("Other", "Export,ExportGroups,Other");

            //options.RegisterCategories();


#if !COMMUNITY
            // clouds
            //RegisteredObjects.AddCloud(typeof(FtpStorageClient), "Cloud,Ftp,Name");
            //RegisteredObjects.AddCloud(typeof(BoxStorageClient), "Cloud,Box,Name", 238);
            //RegisteredObjects.AddCloud(typeof(DropboxStorageClient), "Cloud,Dropbox,Name", 238);
            //RegisteredObjects.AddCloud(typeof(FastCloudStorageClient), "Cloud,FastCloud,Name", 238);
            //RegisteredObjects.AddCloud(typeof(GoogleDriveStorageClient), "Cloud,GoogleDrive,Name", 238);
            //RegisteredObjects.AddCloud(typeof(SkyDriveStorageClient), "Cloud,SkyDrive,Name", 238);
            ExportsOptions options = ExportsOptions.GetInstance();

            options.RegisterClouds();

            // messengers
            RegisteredObjects.AddMessenger(typeof(XmppMessenger), "Messaging,Xmpp,Name");

            options.RegisterMessengers();


#endif
        }

        private void RegisterMethods()
        {
            // Does nothing
        }

#endregion Private Methods
    }
}