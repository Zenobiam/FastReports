using FastReport.Utils;

namespace FastReport.SteemaTeeChart
{
    /// <summary>
    /// Represents a collection of <see cref="TeeChartSeries"/> objects.
    /// </summary>
    public class SeriesCollection : FRCollectionBase
    {
        #region Properties

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">Index of an element.</param>
        /// <returns>The element at the specified index.</returns>
        public TeeChartSeries this[int index]
        {
            get { return List[index] as TeeChartSeries; }
        }

        #endregion // Properties

        #region Constructors

        internal SeriesCollection(Base owner) : base(owner)
        {
        }

        #endregion // Constructors

        #region Public Methods

        /// <summary>
        /// Resets series data.
        /// </summary>
        public void ResetData()
        {
            foreach (TeeChartSeries series in this)
            {
                //series.ClearValues(); // нужно реализовать TeeChartSeries.ClearValues()
            }
        }

        /// <summary>
        /// Processes the current data row.
        /// </summary>
        public void ProcessData()
        {
            foreach (TeeChartSeries series in this)
            {
                //series.ProcessData(); // нужно реализовать TeeChartSeries.ProcessData()
            }
        }

        /// <summary>
        /// Finishes the series data.
        /// </summary>
        public void FinishData()
        {
            foreach (TeeChartSeries series in this)
            {
                //series.FinishData(); // нужно реализовать TeeChartSeries.FinishData()
            }
        }

        #endregion // Public Methods
    }
}
