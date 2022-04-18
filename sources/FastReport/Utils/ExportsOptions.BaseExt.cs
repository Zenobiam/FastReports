using System;
using System.Collections.Generic;
using FastReport.Export.Image;
using FastReport.Export.RichText;
using FastReport.Export.Xml;
using FastReport.Export.Html;
using FastReport.Export.Mht;
using FastReport.Export.Odf;
using FastReport.Export.Pdf;
using FastReport.Export.Csv;
using FastReport.Export.Dbf;
using FastReport.Export.Dxf;
using FastReport.Export.XAML;
using FastReport.Export.Svg;
using FastReport.Export.Ppml;
using FastReport.Export.PS;
using FastReport.Export.BIFF8;
using FastReport.Export.OoXML;
using FastReport.Export.Json;
using FastReport.Export.LaTeX;
using FastReport.Export.Text;
using FastReport.Export.Zpl;
using FastReport.Cloud.StorageClient;
using FastReport.Messaging;
using FastReport.Export;
using FastReport.Cloud.StorageClient.Ftp;
using FastReport.Cloud.StorageClient.Box;
using FastReport.Cloud.StorageClient.Dropbox;
using FastReport.Cloud.StorageClient.FastCloud;
using FastReport.Cloud.StorageClient.GoogleDrive;
using FastReport.Cloud.StorageClient.SkyDrive;
using FastReport.Messaging.Xmpp;
using FastReport.Export.Hpgl;

namespace FastReport.Utils
{
#if !COMMUNITY
    public partial class ExportsOptions
    {
        public partial class ExportsTreeNode
        {
            private const string CLOUD_ITEM_PREFIX = "Cloud,";
            private const string MESSENGER_ITEM_PREFIX = "Messaging,";

            public override string ToString()
            {
                string resPath = "";
                if (exportType == null)
                {
                    resPath = CATEGORY_PREFIX + name;
                }
                else
                {
                    if (exportType.IsSubclassOf(typeof(ExportBase)))
                    {
                        resPath = EXPORT_ITEM_PREFIX + name + EXPORT_ITEM_POSTFIX;
                    }
                    else
                    {
                        resPath = (exportType.IsSubclassOf(typeof(CloudStorageClient))
                            ? CLOUD_ITEM_PREFIX : MESSENGER_ITEM_PREFIX) + name + ITEM_POSTFIX;
                    }
                }
                return Res.Get(resPath);
            }
        }

        /// <summary>
        /// Default Exports menu
        /// </summary>
        /// <returns>Tree that contains default Exports menu</returns>
        public List<ExportsTreeNode> DefaultExports()
        {
            List<ExportsTreeNode> defaultMenu = new List<ExportsTreeNode>();

            defaultMenu.Add(new ExportsTreeNode("Pdf", typeof(PDFExport), 201, true));
            defaultMenu.Add(new ExportsTreeNode("Office", 190, true));
            defaultMenu[1].Nodes.Add(new ExportsTreeNode("RichText", typeof(RTFExport), 190, true));
            defaultMenu[1].Nodes.Add(new ExportsTreeNode("Xlsx", typeof(Excel2007Export), 191, true));
            defaultMenu[1].Nodes.Add(new ExportsTreeNode("Xls", typeof(Excel2003Document), 191, true));
            defaultMenu[1].Nodes.Add(new ExportsTreeNode("Docx", typeof(Word2007Export), 190, true));
            defaultMenu[1].Nodes.Add(new ExportsTreeNode("Pptx", typeof(PowerPoint2007Export), true));
            defaultMenu[1].Nodes.Add(new ExportsTreeNode("Ods", typeof(ODSExport), true));
            defaultMenu[1].Nodes.Add(new ExportsTreeNode("Odt", typeof(ODTExport), true));
            defaultMenu.Add(new ExportsTreeNode("XML", 191, true));
            defaultMenu[2].Nodes.Add(new ExportsTreeNode("Xml", typeof(XMLExport), true));
            defaultMenu[2].Nodes.Add(new ExportsTreeNode("Xaml", typeof(XAMLExport), true));
            defaultMenu.Add(new ExportsTreeNode("Web", 246, true));
            defaultMenu[3].Nodes.Add(new ExportsTreeNode("Html", typeof(HTMLExport), true));
            defaultMenu[3].Nodes.Add(new ExportsTreeNode("Mht", typeof(MHTExport), true));
            defaultMenu.Add(new ExportsTreeNode("Image", 103, true));
            defaultMenu[4].Nodes.Add(new ExportsTreeNode("Image", typeof(ImageExport), true));
            defaultMenu[4].Nodes.Add(new ExportsTreeNode("Svg", typeof(SVGExport), true));
            defaultMenu.Add(new ExportsTreeNode("DataBase", 53, true));
            defaultMenu[5].Nodes.Add(new ExportsTreeNode("Csv", typeof(CSVExport), true));
            defaultMenu[5].Nodes.Add(new ExportsTreeNode("Dbf", typeof(DBFExport), true));
            defaultMenu[5].Nodes.Add(new ExportsTreeNode("Json", typeof(JsonExport), true));
            defaultMenu.Add(new ExportsTreeNode("Print", 195, true));
            defaultMenu[6].Nodes.Add(new ExportsTreeNode("Text", typeof(TextExport), true));
            defaultMenu[6].Nodes.Add(new ExportsTreeNode("Zpl", typeof(ZplExport), true));
            defaultMenu[6].Nodes.Add(new ExportsTreeNode("Ppml", typeof(PPMLExport), true));
            defaultMenu[6].Nodes.Add(new ExportsTreeNode("PS", typeof(PSExport), true));
            defaultMenu.Add(new ExportsTreeNode("Other", true));
            defaultMenu[7].Nodes.Add(new ExportsTreeNode("Xps", typeof(XPSExport), true));
            defaultMenu[7].Nodes.Add(new ExportsTreeNode("LaTeX", typeof(LaTeXExport), true));
            defaultMenu[7].Nodes.Add(new ExportsTreeNode("Dxf", typeof(DxfExport), true));
            //defaultMenu[7].Nodes.Add(new ExportsTreeNode("Hpgl", typeof(HpglExport), true));

            return defaultMenu;
        }

        private ExportsTreeNode cloudNodes;
        private ExportsTreeNode messengerNodes;

        /// <summary>
        /// All cloud exports available in the preview.
        /// </summary>
        public ExportsTreeNode CloudMenu
        {
            get { return cloudNodes; }
        }

        /// <summary>
        /// All messengers exports available in the preview.
        /// </summary>
        public ExportsTreeNode MessengerMenu
        {
            get { return messengerNodes; }
        }

        /// <summary>
        /// Default cloud exports menu.
        /// </summary>
        /// <returns>Tree that contains default cloud exports menu.</returns>
        public ExportsTreeNode DefaultCloud()
        {
            ExportsTreeNode cloudNodes = new ExportsTreeNode("Cloud", 238, false);
            cloudNodes.Nodes.Add(new ExportsTreeNode("Ftp", typeof(FtpStorageClient), false));
            cloudNodes.Nodes.Add(new ExportsTreeNode("Box", typeof(BoxStorageClient), 238, false));
            cloudNodes.Nodes.Add(new ExportsTreeNode("Dropbox", typeof(DropboxStorageClient), 238, false));
            cloudNodes.Nodes.Add(new ExportsTreeNode("FastCloud", typeof(FastCloudStorageClient), 238, false));
            cloudNodes.Nodes.Add(new ExportsTreeNode("GoogleDrive", typeof(GoogleDriveStorageClient), 238, false));
            cloudNodes.Nodes.Add(new ExportsTreeNode("SkyDrive", typeof(SkyDriveStorageClient), 238, false));

            return cloudNodes;
        }

        /// <summary>
        /// Default messengers.
        /// </summary>
        /// <returns></returns>
        public ExportsTreeNode DefaultMessengers()
        {
            ExportsTreeNode res = new ExportsTreeNode("Messengers", false);
            res.Nodes.Add(new ExportsTreeNode("Xmpp", typeof(XmppMessenger), false));

            return res;
        }

        /// <summary>
        /// Register all clouds.
        /// </summary>
        public void RegisterClouds()
        {
            foreach (ExportsTreeNode node in cloudNodes.Nodes)
            {
                RegisteredObjects.AddCloud(node.ExportType, node.ToString(), node.ImageIndex);
                List<ObjectInfo> list = new List<ObjectInfo>();
                RegisteredObjects.Objects.EnumItems(list);
                node.Tag = list[list.Count - 1];
            }
        }

        /// <summary>
        /// Register all messengers.
        /// </summary>
        public void RegisterMessengers()
        {
            foreach (ExportsTreeNode node in messengerNodes.Nodes)
            {
                RegisteredObjects.AddMessenger(node.ExportType, node.ToString());
                List<ObjectInfo> list = new List<ObjectInfo>();
                RegisteredObjects.Objects.EnumItems(list);
                node.Tag = list[list.Count - 1];
            }
        }

        private XmlItem SaveItem(ExportsTreeNode node)
        {
            XmlItem newItem = new XmlItem();
            newItem.Name = node.Name;
            if (node.ExportType != null)
            {
                newItem.SetProp("ExportType", node.ExportType.FullName);
            }
            if (node.ImageIndex != -1)
            {
                newItem.SetProp("Icon", node.ImageIndex.ToString());
            }
            newItem.SetProp("Enabled", node.Enabled.ToString());
            return newItem;
        }

        private void SaveMenuTree(XmlItem xi, List<ExportsTreeNode> nodes)
        {
            xi.Items.Clear();

            foreach (ExportsTreeNode node in nodes)
            {
                XmlItem newItem = SaveItem(node);
                xi.Items.Add(newItem);
                if (node.Nodes.Count != 0)
                {
                    SaveMenuTree(newItem, node.Nodes);
                }
            }
        }

        private void SaveExportOption(string option)
        {
            XmlItem options = Config.Root.FindItem(option);

            List<ExportsTreeNode> nodes = null;
            if (option == "ExportOptions")
            {
                nodes = menuNodes;
            }
            else if (option == "Cloud")
            {
                options.SetProp("Enabled", cloudNodes.Enabled.ToString());
                options.SetProp("Icon", cloudNodes.ImageIndex.ToString());
                nodes = cloudNodes.Nodes;
            }
            else
            {
                options.SetProp("Enabled", messengerNodes.Enabled.ToString());
                nodes = messengerNodes.Nodes;
            }
            SaveMenuTree(options, nodes);
        }

        private ExportsTreeNode RestoreItem(XmlItem item)
        {
            Type exportType = null;
            string typeProp = item.GetProp("ExportType");
            if (!string.IsNullOrEmpty(typeProp))
            {
                exportType = Type.GetType(typeProp);
            }

            string imageIndexProp = item.GetProp("Icon");
            int imageIndex = -1;
            if (!string.IsNullOrEmpty(imageIndexProp))
            {
                int.TryParse(imageIndexProp, out imageIndex);
            }

            string enabledProp = item.GetProp("Enabled");
            bool enabled = true;
            if (!string.IsNullOrEmpty(enabledProp) && Config.PreviewSettings.Exports != PreviewExports.All)
            {
                bool.TryParse(enabledProp, out enabled);
            }

            bool isExport = item.Name != "Cloud" && item.Name != "Messengers" &&
                (exportType != null && !exportType.IsSubclassOf(typeof(CloudStorageClient)) &&
                !exportType.IsSubclassOf(typeof(MessengerBase)) || exportType == null);
            if (isExport && Config.PreviewSettings.Exports != PreviewExports.All && exportType != null)
            {
                enabled = Config.PreviewSettings.Exports.ToString().ToLower().Contains(exportType.Name.ToLower());
            }

            bool isCloud = item.Name != "Cloud" && item.Name != "Messengers" &&
                exportType != null && exportType.IsSubclassOf(typeof(CloudStorageClient));
            if (isCloud && Config.PreviewSettings.Clouds != PreviewClouds.All && exportType != null)
            {
                enabled = Config.PreviewSettings.Clouds.ToString().Contains(exportType.Name.Replace("StorageClient", ""));
            }

            bool isMessenger = item.Name != "Cloud" && item.Name != "Messengers" &&
                exportType != null && exportType.IsSubclassOf(typeof(MessengerBase));
            if (isMessenger && Config.PreviewSettings.Messengers != PreviewMessengers.All && exportType != null)
            {
                enabled = Config.PreviewSettings.Messengers.ToString().Contains(exportType.Name.Replace("Messenger", ""));
            }

            return new ExportsTreeNode(item.Name, exportType, imageIndex, enabled, isExport);
        }

        private void RestoreMenuTree(XmlItem xi, List<ExportsTreeNode> nodes)
        {
            foreach (XmlItem item in xi.Items)
            {
                ExportsTreeNode currentNode = RestoreItem(item);
                nodes.Add(currentNode);
                if (item.Items.Count > 0)
                {
                    RestoreMenuTree(item, currentNode.Nodes);
                    currentNode.Enabled = false;
                    foreach (ExportsTreeNode subNode in currentNode.Nodes)
                    {
                        if (subNode.Enabled)
                        {
                            currentNode.Enabled = true;
                            break;
                        }
                    }
                }
            }
        }

        private void RestoreOptionsFor(string option)
        {
            XmlItem options = Config.Root.FindItem(option);

            List<ExportsTreeNode> nodes = null;
            if (option == "ExportOptions")
            {
                nodes = menuNodes;
                if (options.Items.Count == 0)
                {
                    menuNodes = DefaultExports();
                }
            }
            else if (option == "Cloud")
            {
                cloudNodes = RestoreItem(options);
                nodes = cloudNodes.Nodes;
                if (options.Items.Count == 0)
                {
                    cloudNodes = DefaultCloud();
                }
                if (Config.PreviewSettings.Clouds == PreviewClouds.None)
                {
                    cloudNodes.Enabled = false;
                }
            }
            else
            {
                messengerNodes = RestoreItem(options);
                nodes = messengerNodes.Nodes;
                if (options.Items.Count == 0)
                {
                    messengerNodes = DefaultMessengers();
                }
                if (Config.PreviewSettings.Messengers == PreviewMessengers.None)
                {
                    messengerNodes.Enabled = false;
                }
            }
            if (options.Items.Count != 0)
            {
                RestoreMenuTree(options, nodes);
            }
        }

        private void RestoreOptions()
        {
            RestoreOptionsFor("ExportOptions");
            RestoreOptionsFor("Cloud");
            RestoreOptionsFor("Messengers");
        }

        private void SaveOptions()
        {
            SaveExportOption("ExportOptions");
            SaveExportOption("Cloud");
            SaveExportOption("Messengers");
        }
    }
#endif
}
