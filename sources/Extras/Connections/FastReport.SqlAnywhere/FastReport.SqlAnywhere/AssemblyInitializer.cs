using FastReport.Utils;

namespace FastReport.SqlAnywhere
{
    class AssemblyInitializer : AssemblyInitializerBase
    {
        public AssemblyInitializer()
        {
            RegisteredObjects.AddConnection(typeof(SqlAnywhereDataConnection));
        }
    }
}
