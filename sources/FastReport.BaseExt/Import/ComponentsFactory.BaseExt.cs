#if MSCHART
using FastReport.MSChart;
#endif
using FastReport.Map;

namespace FastReport.Import
{
    /// <summary>
    /// The components factory.
    /// </summary>
    public static partial class ComponentsFactory
    {
        #region Objects

        /// <summary>
        /// Creates a RichObject instance with specified name and parent.
        /// </summary>
        /// <param name="name">The name of the RichObject instance.</param>
        /// <param name="parent">The parent of the RichObject instance.</param>
        /// <returns>The RichObject instance.</returns>
        public static RichObject CreateRichObject(string name, Base parent)
        {
            RichObject rich = new RichObject();
            rich.Name = name;
            rich.Parent = parent;
            return rich;
        }

#if MSCHART
        /// <summary>
        /// Creates a MSChartObject instance with specified name and parent.
        /// </summary>
        /// <param name="name">The name of the MSChartObject instance.</param>
        /// <param name="parent">The parent of the MSChartObject instance.</param>
        /// <returns>The MSChartObject instance.</returns>
        public static MSChartObject CreateMSChartObject(string name, Base parent)
        {
            MSChartObject chart = new MSChartObject();
            chart.Name = name;
            chart.Parent = parent;
            return chart;
        }

        /// <summary>
        /// Creates a SparklineObject instance with specified name and parent.
        /// </summary>
        /// <param name="name">The name of the SparlineObject instance.</param>
        /// <param name="parent">The parent of the SparlineObject instance.</param>
        /// <returns></returns>
        public static SparklineObject CreateSparklineObject(string name, Base parent)
        {
            SparklineObject sparkline = new SparklineObject();
            sparkline.Name = name;
            sparkline.Parent = parent;
            return sparkline;
        }
#endif

        /// <summary>
        /// Creates a MapObject instance with specified name and parent.
        /// </summary>
        /// <param name="name">The name of the MapObject instance.</param>
        /// <param name="parent">The parent of the MapObject instance.</param>
        /// <returns>The MapObject instance.</returns>
        public static MapObject CreateMapObject(string name, Base parent)
        {
            MapObject map = new MapObject();
            map.Name = name;
            map.Parent = parent;
            return map;
        }

        #endregion // Objects
    }
}
