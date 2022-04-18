using System;
using System.IO;
using System.Data;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;
using Steema.TeeChart;
using Steema.TeeChart.Styles;
using FastReport.Data;
using FastReport.Utils;

namespace FastReport.SteemaTeeChart
{
    /// <summary>
    /// Represents the chart object based on Steema TeeChart control.
    /// </summary>
    /// <remarks>
    /// FastReport uses Steema TeeChart library to display charts.
    /// Product page at: https://www.steema.com/linkIn/TeeChartNETBusiness
    /// Home page at Nuget: https://www.nuget.org/packages/Steema.TeeChart.NET.Standard/
    /// <para>
    /// To access Steema TeeChart object, use <see cref="Chart"/> property.
    /// It allows you to set up chart appearance. For more information, refer to the Steema TeeChart documentation.
    /// </para>
    /// <para>
    /// Chart object may contain one or several <i>series</i>.
    /// </para>
    /// </remarks>
    public partial class TeeChartObject : ReportComponentBase, IHasEditor //, IParent
    {
        #region Fields

        //private SeriesCollection FSeries;
        private TChart FChart;
        private DataSourceBase FDataSource;
        private string FLabelField;
        private string FXValuesField;
        private string FYValuesField;

        #endregion // Fields

        #region Properties

        /*/// <summary>
        /// Gets the collection of <see cref="SeriesCollection"/> objects.
        /// </summary>
        [Browsable(false)]
        public SeriesCollection Series
        {
            get { return FSeries; }
        }*/

        /// <summary>
        /// Gets a reference to the Steema TChart object.
        /// </summary>
        [Category("Appearance")]
        public TChart Chart
        {
            get { return FChart; }
        }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        [Category("Data")]
        public DataSourceBase DataSource
        {
            get { return FDataSource; }
            set { FDataSource = value; }
        }

        /// <summary>
        /// Gets or sets the Datasource label field.
        /// </summary>
        [Editor(typeof(DataColumnRelatedToDataSourceEditor), typeof(UITypeEditor))]
        [Category("Data")]
        public string LabelField
        {
            get { return FLabelField; }
            set
            {
                FLabelField = value;
                if (!String.IsNullOrEmpty(FLabelField))
                {
                    FLabelField = FLabelField.Substring(FLabelField.LastIndexOf('.') + 1);
                }
            }
        }

        /// <summary>
        /// Gets or sets the field to use as a source for X values.
        /// </summary>
        [Editor(typeof(DataColumnRelatedToDataSourceEditor), typeof(UITypeEditor))]
        [Category("Data")]
        public string XValuesField
        {
            get { return FXValuesField; }
            set
            {
                FXValuesField = value;
                if (!String.IsNullOrEmpty(FXValuesField))
                {
                    FXValuesField = FXValuesField.Substring(FXValuesField.LastIndexOf('.') + 1);
                }
            }
        }

        /// <summary>
        /// Gets or sets the field to use as a source for Y values.
        /// </summary>
        [Editor(typeof(DataColumnRelatedToDataSourceEditor), typeof(UITypeEditor))]
        [Category("Data")]
        public string YValuesField
        {
            get { return FYValuesField; }
            set
            {
                FYValuesField = value;
                if (!String.IsNullOrEmpty(FYValuesField))
                {
                    FYValuesField = FYValuesField.Substring(FYValuesField.LastIndexOf('.') + 1);
                }
            }
        }

        //private BandBase ParentBand
        //{
        //    get
        //    {
        //        BandBase parentBand = this.Band;
        //        if (parentBand is ChildBand)
        //        {
        //            parentBand = (parentBand as ChildBand).GetTopParentBand;
        //        }
        //        return parentBand;
        //    }
        //}

        //private bool IsOnFooter
        //{
        //    get { return (ParentBand is GroupFooterBand || ParentBand is DataFooterBand); }
        //}

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TeeChartObject"/> with default settings.
        /// </summary>
        public TeeChartObject()
        {
            //FSeries = new SeriesCollection(this);
            FChart = new TChart(this);
            FLabelField = "";
            FXValuesField = "";
            FYValuesField = "";
        }

        #endregion // Constructors

        #region IParent Implementation

        /*/// <inheritdoc/>
        public void AddChild(Base child)
        {
            if (child is TeeChartSeries)
            {
                Series.Add(child as TeeChartSeries);
            }
        }

        /// <inheritdoc/>
        public bool CanContain(Base child)
        {
            return child is TeeChartSeries;
        }

        /// <inheritdoc/>
        public void GetChildObjects(ObjectCollection list)
        {
            foreach (TeeChartSeries series in Series)
            {
                list.Add(series);
            }
        }

        /// <inheritdoc/>
        public int GetChildOrder(Base child)
        {
            if (child is TeeChartSeries)
            {
                return Series.IndexOf(child as TeeChartSeries);
            }
            return 0;
        }

        /// <inheritdoc/>
        public void RemoveChild(Base child)
        {
            if (child is TeeChartSeries)
            {
                Series.Remove(child as TeeChartSeries);
            }
        }

        /// <inheritdoc/>
        public void SetChildOrder(Base child, int order)
        {
        }

        /// <inheritdoc/>
        public void UpdateLayout(float dx, float dy)
        {
        }*/

        #endregion // IParent Implementation

        #region IHasEditor Implementation

        /// <inheritdoc/>
        public bool InvokeEditor()
        {
            return FChart.ShowEditor();
        }

        #endregion // IHasEditor Implementation

        #region Private Methods
        #endregion // Private Methods

        #region Public Methods

        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);
            TeeChartObject src = source as TeeChartObject;

            DataSource = src.DataSource;
            LabelField = src.LabelField;
            XValuesField = src.XValuesField;
            YValuesField = src.YValuesField;

            using (MemoryStream stream = new MemoryStream())
            {
                src.Chart.Export.Template.IncludeData = true;
                src.Chart.Export.Template.Serialize(stream);
                stream.Position = 0;
                Chart.Import.Template.Load(stream);
            }
        }

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            //base.Draw(e);

            Rectangle chartRect = new Rectangle((int)Math.Round(AbsLeft), (int)Math.Round(AbsTop), (int)Math.Round(Width), (int)Math.Round(Height));
            GraphicsState state = e.Graphics.Save();

            try
            {
                if (IsPrinting)
                {
                    FChart.SetBounds((int)AbsLeft, (int)AbsTop, (int)Width, (int)Height);
                    using (Bitmap bmp = FChart.Bitmap)
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.ScaleTransform(e.ScaleX, e.ScaleY);
                        e.Graphics.DrawImage(bmp,
                            new RectangleF((int)Math.Round(AbsLeft * e.ScaleX), (int)Math.Round(AbsTop * e.ScaleY), (int)Math.Round(Width * e.ScaleX), (int)Math.Round(Height * e.ScaleY)),
                            new RectangleF(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                    }
                }
                else
                {
                    FChart.SetBounds((int)AbsLeft, (int)AbsTop, (int)Width, (int)Height);
                    using (Bitmap bmp = FChart.Bitmap)
                    using (Graphics g = Graphics.FromImage(FChart.Bitmap))
                    {
                        g.ScaleTransform(e.ScaleX, e.ScaleY);
                        e.Graphics.DrawImage(bmp, new RectangleF((int)Math.Round(AbsLeft * e.ScaleX), (int)Math.Round(AbsTop * e.ScaleY),
                            (int)Math.Round(Width * e.ScaleX), (int)Math.Round(Height * e.ScaleY)),
                            new RectangleF(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                    }
                }
            }
            catch (Exception ex)
            {
                using (Font font = new Font("Tahoma", 8))
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString(ex.Message, font, Brushes.Red, chartRect, sf);
                }
            }
            finally
            {
                e.Graphics.Restore(state);
            }
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            TeeChartObject c = writer.DiffObject as TeeChartObject;
            base.Serialize(writer);

            if (DataSource != null)
                writer.WriteRef("DataSource", DataSource);
            if (LabelField != c.LabelField)
                writer.WriteStr("LabelField", LabelField);
            if (XValuesField != c.XValuesField)
                writer.WriteStr("XValuesField", XValuesField);
            if (YValuesField != c.YValuesField)
                writer.WriteStr("YValuesField", YValuesField);

            using (MemoryStream stream = new MemoryStream())
            {
                Chart.Export.Template.IncludeData = true;
                Chart.Export.Template.Serialize(stream);
                stream.Position = 0;
                writer.WriteValue("ChartData", stream);
            }
        }

        /// <inheritdoc/>
        public override void Deserialize(FRReader reader)
        {
            base.Deserialize(reader);

            if (reader.HasProperty("ChartData"))
            {
                string streamStr = reader.ReadStr("ChartData");
                using (MemoryStream stream = Converter.FromString(typeof(Stream), streamStr) as MemoryStream)
                {
                    Chart.Import.Template.Load(stream);
                }
            }
        }

        /// <inheritdoc/>
        public override void GetData()
        {
            base.GetData();

            if (DataSource != null && (DataSource is TableDataSource)/* && !IsOnFooter*/)
            {
                DataSource.Init();

                DataTable baseTable = (DataSource as TableDataSource).Table;
                DataTable table = new DataTable(baseTable.TableName);
                table = baseTable.Clone();
                foreach (DataRow row in baseTable.Rows)
                {
                    table.ImportRow(row);
                }

                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(table);
                BindingSource bindingSource = new BindingSource();
                bindingSource.DataSource = dataSet;
                bindingSource.DataMember = dataSet.Tables[0].ToString();

                foreach (Series series in Chart.Series)
                {
                    try
                    {
                        series.DataSource = bindingSource;
                        series.LabelMember = LabelField;
                        series.XValues.DataMember = XValuesField;
                        series.YValues.DataMember = YValuesField;
                        series.CheckDataSource();
                    }
                    catch
                    {
                        series.DataSource = null;
                        series.LabelMember = "";
                        series.XValues.DataMember = "X";
                        series.YValues.DataMember = "Y";
                    }
                }
            }
        }

        #endregion // Public Methods
    }
}
