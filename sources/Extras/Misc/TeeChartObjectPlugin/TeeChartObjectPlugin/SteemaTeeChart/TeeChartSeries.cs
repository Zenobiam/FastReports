using System.Drawing.Design;
using System.ComponentModel;
using FastReport.TypeEditors;

namespace FastReport.SteemaTeeChart
{
    /// <summary>
    /// Represents Steema TeeChart Series wrapper.
    /// </summary>
    /// <remarks>
    /// This class provides a data for Steema TeeChart Series.
    /// <para>
    /// You don't need to create an instance of this class directly.
    /// Instead, use the <see cref="TeeChartObject.AddSeries"/> method.
    /// </para>
    /// </remarks>
    public class TeeChartSeries : Base
    {
        #region Fields

        private string FFilter;
        private SortBy FSortBy;
        private ChartSortOrder FSortOrder;
        private GroupBy FGroupBy;
        private Collect FCollect;
        private float FCollectValue;
        private PieExplode FPieExplode;
        private string FPieExplodeValue;

        private string FXValue;
        private string FYValue1;
        private string FYValue2;
        private string FYValue3;
        private string FYValue4;
        private string FColor;
        private string FLabel;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets or sets the data filter expression.
        /// </summary>
        /// <remarks>
        /// The filter is applied for this series only. You can also use the <see cref="TeeChartObject.Filter"/>
        /// property to set a filter that will be applied to all series in a chart.
        /// </remarks>
        [Editor(typeof(ExpressionEditor), typeof(UITypeEditor))]
        [Category("Data")]
        public string Filter
        {
            get { return FFilter; }
            set { FFilter = value; }
        }

        /// <summary>
        /// Gets or sets the sort method used to sort data points.
        /// </summary>
        /// <remarks>
        /// You have to specify the <see cref="SortOrder"/> property as well.
        /// Data points in this series will be sorted according selected sort criteria and order.
        /// </remarks>
        [Category("Data")]
        [DefaultValue(SortBy.None)]
        public SortBy SortBy
        {
            get { return FSortBy; }
            set { FSortBy = value; }
        }

        /// <summary>
        /// Gets or sets the sort used to sort data points.
        /// </summary>
        /// <remarks>
        /// You have to specify the <see cref="SortBy"/> property as well.
        /// Data points in this series will be sorted according selected sort criteria and order.
        /// </remarks>
        [Category("Data")]
        [DefaultValue(ChartSortOrder.Ascending)]
        public ChartSortOrder SortOrder
        {
            get { return FSortOrder; }
            set { FSortOrder = value; }
        }

        /// <summary>
        /// Gets or sets the group method used to group data points.
        /// </summary>
        /// <remarks>
        /// This property is mainly used when series is filled with data with several identical X values.
        /// In this case, you need to set the property to <b>XValue</b>. All identical data points will be 
        /// grouped into one point, their Y values will be summarized. You can choose the summary function
        /// using the <see cref="GroupFunction"/> property.
        /// </remarks>
        [Category("Data")]
        [DefaultValue(GroupBy.None)]
        public GroupBy GroupBy
        {
            get { return FGroupBy; }
            set { FGroupBy = value; }
        }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TeeChartSeries"/> with default settings.
        /// </summary>
        public TeeChartSeries()
        {
            BaseName = "Series";

            FFilter = "";
            FPieExplodeValue = "";

            FXValue = "";
            FYValue1 = "";
            FYValue2 = "";
            FYValue3 = "";
            FYValue4 = "";
            FColor = "";
            FLabel = "";
        }

        #endregion // Constructors
    }

    /// <summary>
    /// Specifies how the series points are sorted.
    /// </summary>
    public enum SortBy
    {
        /// <summary>
        /// Points are not sorted.
        /// </summary>
        None,

        /// <summary>
        /// Points are sorted by X value.
        /// </summary>
        XValue,

        /// <summary>
        /// Points are sorted by Y value.
        /// </summary>
        YValue
    }

    /// <summary>
    /// Specifies the directions in wich the series points are sorted.
    /// </summary>
    public enum ChartSortOrder
    {
        /// <summary>
        /// Points are sorted in ascending order.
        /// </summary>
        Ascending,

        /// <summary>
        /// Points are sorted in descending order.
        /// </summary>
        Descending
    }

    /// <summary>
    /// Specifies how the series points are grouped.
    /// </summary>
    public enum GroupBy
    {
        /// <summary>
        /// Points are not grouped.
        /// </summary>
        None,

        /// <summary>
        /// Points are grouped by X value.
        /// </summary>
        XValue,

        /// <summary>
        /// Points are grouped by Years.
        /// </summary>
        Years,

        /// <summary>
        /// Points are grouped by Months.
        /// </summary>
        Months,

        /// <summary>
        /// Points are grouped by Weeks.
        /// </summary>
        Weeks,

        /// <summary>
        /// Points are grouped by Days.
        /// </summary>
        Days,

        /// <summary>
        /// Points are grouped by Hours.
        /// </summary>
        Hours,

        /// <summary>
        /// Points are grouped by Minutes.
        /// </summary>
        Minutes,

        /// <summary>
        /// Points are grouped by Seconds.
        /// </summary>
        Seconds,

        /// <summary>
        /// Points are grouped by Milliseconds.
        /// </summary>
        Milliseconds
    }

    /// <summary>
    /// Specifies which pie value to explode.
    /// </summary>
    public enum PieExplode
    {
        /// <summary>
        /// Don't explode pie values.
        /// </summary>
        None,

        /// <summary>
        /// Explode the biggest value.
        /// </summary>
        BiggestValue,

        /// <summary>
        /// Explode the lowest value.
        /// </summary>
        LowestValue,

        /// <summary>
        /// Explode the value specified in the <see cref="TeeChartSeries.PieExplodeValue"/> property.
        /// </summary>
        SpecificValue
    }

    /// <summary>
    /// Specifies which data points to collect into one point.
    /// </summary>
    public enum Collect
    {
        /// <summary>
        /// Don't collect points.
        /// </summary>
        None,

        /// <summary>
        /// Show top N points (<i>N</i> value is specified in the <see cref="TeeChartSeries.CollectValue"/>
        /// property), collect other points into one.
        /// </summary>
        TopN,

        /// <summary>
        /// Show bottom N points (<i>N</i> value is specified in the <see cref="TeeChartSeries.CollectValue"/>
        /// property), collect other points into one.
        /// </summary>
        BottomN,

        /// <summary>
        /// Collect points which have Y value less than specified in the
        /// <see cref="TeeChartSeries.CollectValue"/> property.
        /// </summary>
        LessThan,

        /// <summary>
        /// Collect points which have Y value less than percent specified in the
        /// <see cref="TeeChartSeries.CollectValue"/> property.
        /// </summary>
        LessThanPercent,

        /// <summary>
        /// Collect points which have Y value greather than specified in the
        /// <see cref="TeeChartSeries.CollectValue"/> property.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Collect points which have Y value greater than percent specified in the
        /// <see cref="TeeChartSeries.CollectValue"/> property.
        /// </summary>
        GreaterThanPercent
    }
}
