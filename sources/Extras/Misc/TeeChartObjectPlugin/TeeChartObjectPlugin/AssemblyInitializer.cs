using FastReport.Utils;

namespace FastReport.SteemaTeeChart
{
    public class AssemblyInitializer : AssemblyInitializerBase
    {
        public AssemblyInitializer()
        {
            RegisteredObjects.Add(typeof(TeeChartObject), "ReportPage", 125);
        }
    }
}
