using System.Xml;

namespace FastReport.Import.DevExpress
{
    partial class DevExpressImport
    {
        #region Private Methods

        partial void LoadRichText(XmlNode node, Base parent)
        {
            RichObject rich = ComponentsFactory.CreateRichObject(node.Name, parent);
            AddLocalizationItemsAttributes(node);
            LoadComponent(node, rich);
            LoadSize(node, rich);
            LoadBorder(node, rich.Border);
            ApplyStyle(node, rich);
        }

        #endregion Private Methods
    }
}

