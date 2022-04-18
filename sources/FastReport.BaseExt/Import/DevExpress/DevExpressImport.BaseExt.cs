namespace FastReport.Import.DevExpress
{
    /// <summary>
    /// Represents the DevExpess import plugin.
    /// </summary>
    partial class DevExpressImport : ImportBase
    {
        #region Private Methods

        partial void LoadRichText(string name, Base parent)
        {
            string description = GetObjectDescription(name);
            RichObject rich = ComponentsFactory.CreateRichObject(name, parent);
            LoadComponent(description, rich);
            LoadSize(description, rich);
            LoadBorder(description, rich.Border);
            rich.Style = GetPropertyValue("StyleName", description).Replace("\"", "");
        }

        #endregion // Private Methods
    }
}
