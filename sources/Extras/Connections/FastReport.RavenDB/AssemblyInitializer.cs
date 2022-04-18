using FastReport.Utils;


namespace FastReport.Data
{
    public class AssemblyInitializer : AssemblyInitializerBase
    {
        public AssemblyInitializer()
        {
            RegisteredObjects.AddConnection(typeof(RavenDBDataConnection));
        }
    }
}