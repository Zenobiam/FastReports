using FastReport.Utils;

namespace FastReport.Design.ImportPlugins.RPT
{
    public class AssemblyInitializer : AssemblyInitializerBase
    {
        public AssemblyInitializer()
        {
            DesignerPlugins.Add(typeof(RPTImportPlugin));
        }
    }
}
