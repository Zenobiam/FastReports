using FastReport.Utils;

namespace FastReport.Data
{
    class AssemblyInitializer : AssemblyInitializerBase
    {
        public AssemblyInitializer()
        {
            RegisteredObjects.AddConnection(typeof(NosDbDataConnection));
        }
    }
}
