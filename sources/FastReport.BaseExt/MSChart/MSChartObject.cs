using System;
using System.Collections.Generic;
using FastReport.DataVisualization.Charting;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Data;
using System.Drawing.Drawing2D;
using System.Drawing.Design;

namespace FastReport.MSChart
{
  /// <summary>
  /// Represents the chart object based on Microsoft Chart control.
  /// </summary>
  /// <remarks>
  /// FastReport uses Microsoft Chart library to display charts. This library is included 
  /// in .Net Framework 4.0. For .Net 3.5 it is available as a separate download here:
  /// http://www.microsoft.com/downloads/details.aspx?FamilyID=130f7986-bf49-4fe5-9ca8-910ae6ea442c
  /// <para/><note type="caution">This library requires .Net Framework 3.5 SP1.</note>
  /// <para/>To access Microsoft Chart object, use the <see cref="Chart"/> property. It allows you
  /// to set up chart appearance. For more information on available properties, refer to the
  /// MS Chart documentation.
  /// <para/>Chart object may contain one or several <i>series</i>. Each series is represented by two objects: 
  /// <list type="bullet">
  ///   <item>
  ///     <description>the <b>Series</b> that is handled by MS Chart. It is stored in the
  ///       <b>Chart.Series</b> collection;</description>
  ///   </item>
  ///   <item>
  ///     <description>the <see cref="MSChartSeries"/> object that provides data for MS Chart series.
  ///       It is stored in the <see cref="Series"/> collection.</description>
  ///   </item>
  /// </list>
  /// <para/>Do not operate series objects directly. To add or remove series, use 
  /// the <see cref="AddSeries"/> and <see cref="DeleteSeries"/> methods. These methods 
  /// handle <b>Series</b> and <b>MSChartSeries</b> in sync.
  /// <para/>If you have a chart object on your Form and want to print it in FastReport, use
  /// the <see cref="AssignChart"/> method.
  /// </remarks>
  public partial class MSChartObject : ReportComponentBase, IParent
  {
    #region Fields
    private SeriesCollection series;
    private Chart chart;
    private DataSourceBase dataSource;
    private string filter;
    private bool alignXValues;
    private string autoSeriesColumn;
    private string autoSeriesColor;
    private SortOrder autoSeriesSortOrder;
    private bool startAutoSeries;
    private MemoryStream originalChartStream;
    private DataPoint hotPoint;
    private bool autoSeriesForce;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the collection of <see cref="MSChartSeries"/> objects.
    /// </summary>
    [Browsable(false)]
    public SeriesCollection Series
    {
      get { return series; }
    }

    /// <summary>
    /// Gets a reference to the MS Chart object.
    /// </summary>
    [Category("Appearance")]
    public Chart Chart
    {
      get { return chart; }
    }

    /// <summary>
    /// Gets or set Force automatically created series.
    /// </summary>
    [Category("Data")]
    [DefaultValue(false)]
    public bool AutoSeriesForce
    {
      get { return autoSeriesForce; }
      set { autoSeriesForce = value; }
    }

    /// <summary>
    /// Gets or sets the data source.
    /// </summary>
    [Category("Data")]
    public DataSourceBase DataSource
    {
      get { return dataSource; }
      set { dataSource = value; }
    }

    /// <summary>
    /// Gets or sets the filter expression.
    /// </summary>
    /// <remarks>
    /// This filter will be applied to all series in chart. You may also use the series'
    /// <see cref="MSChartSeries.Filter"/> property to filter each series individually.
    /// </remarks>
    [Category("Data")]
    [Editor("FastReport.TypeEditors.ExpressionEditor, FastReport", typeof(UITypeEditor))]
    public string Filter
    {
      get { return filter; }
      set { filter = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating that all series' data point should be aligned by its X value.
    /// </summary>
    /// <remarks>
    /// Using this property is necessary to print stacked type series. These series must have
    /// equal number of data points, and the order of data points must be the same for all series.
    /// </remarks>
    [Category("Data")]
    [DefaultValue(false)]
    public bool AlignXValues
    {
      get { return alignXValues; }
      set { alignXValues = value; }
    }

    /// <summary>
    /// Gets or set the data column or expression for automatically created series.
    /// </summary>
    /// <remarks>
    /// In order to create auto-series, you need to define one series that will be used as a 
    /// template for new series, and set up the <see cref="AutoSeriesColumn"/> property.
    /// The value of this property will be a name of new series. If there is no series 
    /// with such name yet, the new series will be added.
    /// </remarks>
    [Category("Data")]
    [Editor("FastReport.TypeEditors.ExpressionEditor, FastReport", typeof(UITypeEditor))]
    public string AutoSeriesColumn
    {
      get { return autoSeriesColumn; }
      set { autoSeriesColumn = value; }
    }

    /// <summary>
    /// Gets or set the color for auto-series.
    /// </summary>
    /// <remarks>
    /// If no color is specified, the new series will use the palette defined in the chart.
    /// </remarks>
    [Category("Data")]
    [Editor("FastReport.TypeEditors.ExpressionEditor, FastReport", typeof(UITypeEditor))]
    public string AutoSeriesColor
    {
      get { return autoSeriesColor; }
      set { autoSeriesColor = value; }
    }

    /// <summary>
    /// Gets or sets sort order for auto-series.
    /// </summary>
    [Category("Data")]
    [DefaultValue(SortOrder.None)]
    public SortOrder AutoSeriesSortOrder
    {
      get { return autoSeriesSortOrder; }
      set { autoSeriesSortOrder = value; }
    }

    private DataPoint HotPoint
    {
      get { return hotPoint; }
      set
      {
        if (hotPoint != value)
        {
          if (Page != null)
            Page.Refresh();
        }
        hotPoint = value;
      }
    }

    private BandBase ParentBand
    {
      get
      {
        BandBase parentBand = this.Band;
        if (parentBand is ChildBand)
          parentBand = (parentBand as ChildBand).GetTopParentBand;
        return parentBand;
      }
    }

    private bool IsOnFooter
    {
      get { return ParentBand is GroupFooterBand || ParentBand is DataFooterBand; }
    }
    #endregion
    
    #region Private Methods
    private void SetChartDefaults()
    {
      ChartArea area = new ChartArea("Default");
      chart.ChartAreas.Add(area);
      area.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
      area.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
      area.AxisX2.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
      area.AxisY2.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
      
      Legend legend = new Legend("Default");
      chart.Legends.Add(legend);
      
      Title title = new Title();
      chart.Titles.Add(title);
      title.Visible = false;
      
      chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
      chart.BorderlineColor = Color.DarkGray;
      chart.BorderlineDashStyle = ChartDashStyle.Solid;
      chart.BorderlineWidth = 2;
    }

    private void ClearAutoSeries()
    {
        startAutoSeries = true;
        if (!String.IsNullOrEmpty(autoSeriesColumn) && !autoSeriesForce)
        {
            for (int i = 1; i < Chart.Series.Count; i++)
            {
                DeleteSeries(i);
            }
        }
    }
    
    private MSChartSeries CloneSeries(MSChartSeries source,int number)
    {
      MSChartSeries series = AddSeries(source.SeriesSettings.ChartType);
      series.Assign(source);
      
      Chart tempChart = new Chart();
      originalChartStream.Position = 0;
      tempChart.Serializer.Content = SerializationContents.All;
      tempChart.Serializer.Load(originalChartStream);
      
      Series tempSeries = tempChart.Series[number];
      tempChart.Series.Remove(tempSeries);
      tempSeries.Name = "";
      
      Series tempSeries1 = chart.Series[chart.Series.Count - 1];
      chart.Series.Remove(tempSeries1);
      
      chart.Series.Add(tempSeries);
      tempSeries.Points.Clear();
      
      tempChart.Dispose();
      tempSeries1.Dispose();
      
      return series;
    }

    private IEnumerable<MSChartSeries> MakeAutoSeries(object autoSeriesKey, MSChartSeries[] serieses)
    {
      if (serieses == null)
      {
        string seriesName = autoSeriesKey.ToString();
        MSChartSeries series = null;
        if (startAutoSeries)
          series = Series[0];
        else
        {
          bool found = false;
          foreach (MSChartSeries s in Series)
          {
            if (s.SeriesSettings.Name == seriesName)
            {
              series = s;
              found = true;
              break;
            }
          }

          if (!found)
          {
            series = CloneSeries(Series[0], 0);
            if (!String.IsNullOrEmpty(AutoSeriesColor))
            {
              object color = Report.Calc(AutoSeriesColor);
              if (color is Color)
                series.SeriesSettings.Color = (Color)color;
            }
          }
        }

        series.SeriesSettings.Name = seriesName;
        startAutoSeries = false;
        yield return series;
      }
      else
      {
        for (int i = 0; i < serieses.Length; i++)
        {
          string seriesName = autoSeriesKey.ToString();
          if (!String.IsNullOrEmpty(Series[i].AutoSeriesColumn))
            seriesName = Report.Calc(Series[i].AutoSeriesColumn).ToString();
          MSChartSeries series = null;
          if (startAutoSeries || !serieses[i].AutoSeriesForce)
            series = serieses[i];
          else
          {
            bool found = false;
            foreach (MSChartSeries s in Series)
            {
              if (s.SeriesSettings.Name == seriesName)
              {
                series = s;
                found = true;
                break;
              }
            }

            if (!found)
            {
              series = CloneSeries(serieses[i], i);
              if (!String.IsNullOrEmpty(AutoSeriesColor))
              {
                object color = Report.Calc(AutoSeriesColor);
                if (color is Color)
                  series.SeriesSettings.Color = (Color)color;
              }
            }
          }
          if (serieses[i].AutoSeriesForce)
            series.SeriesSettings.Name = seriesName;

          yield return series;
        }
        startAutoSeries = false;
      }
    }
    
    private void SortAutoSeries()
    {
      // create a list of series
      List<SeriesInfo> sortedList = new List<SeriesInfo>();
      foreach (MSChartSeries series in Series)
      {
        sortedList.Add(new SeriesInfo(series));
      }
      
      sortedList.Sort(new SeriesComparer(AutoSeriesSortOrder));
      
      // delete original series
      while (Series.Count > 0)
      {
        Series.RemoveAt(0);
        chart.Series.RemoveAt(0);
      }
      
      // add them in correct order
      foreach (SeriesInfo info in sortedList)
      {
        Series.Add(info.Series);
        chart.Series.Add(info.ChartSeries);
      }
    }

    private void WireEvents(bool wire)
    {
      DataBand dataBand = null;
      if (ParentBand is GroupFooterBand)
        dataBand = ((ParentBand as GroupFooterBand).Parent as GroupHeaderBand).GroupDataBand;
      else if (ParentBand is DataFooterBand)
        dataBand = ParentBand.Parent as DataBand;

      // wire/unwire events
      if (dataBand != null)
      {
        if (wire)
          dataBand.BeforePrint += new EventHandler(dataBand_BeforePrint);
        else
          dataBand.BeforePrint -= new EventHandler(dataBand_BeforePrint);
      }
    }

    private void dataBand_BeforePrint(object sender, EventArgs e)
    {
      bool firstRow = (sender as DataBand).IsFirstRow;
      if (firstRow)
        Series.ResetData();

      object match = true;
      if (!String.IsNullOrEmpty(Filter))
        match = Report.Calc(Filter);

      if (match is bool && (bool)match == true)
        Series.ProcessData();
    }
    #endregion
    
    #region Protected Methods
    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
      if (disposing && chart != null)
      {
        chart.Dispose();
        chart = null;
      }
      base.Dispose(disposing);
    }
    #endregion
    
    #region Public Methods
    /// <summary>
    /// Adds a new series.
    /// </summary>
    /// <param name="chartType">The type of series.</param>
    /// <returns>The new <b>MSChartSeries</b> object.</returns>
    public MSChartSeries AddSeries(SeriesChartType chartType)
    {
      if (chart.ChartAreas.Count == 0)
        SetChartDefaults();
      
      Series chartSeries = new Series();
      chartSeries.ChartType = chartType;
      chart.Series.Add(chartSeries);
      
      MSChartSeries series = new MSChartSeries();
      Series.Add(series);
      series.CreateUniqueName();
      
      return series;
    }
    
    /// <summary>
    /// Deletes a series at a specified index.
    /// </summary>
    /// <param name="index">Index of series.</param>
    public void DeleteSeries(int index)
    {
        if (index >= 0 && index < chart.Series.Count)
        {
            Series series = chart.Series[index];
            chart.Series.RemoveAt(index);
            series.Dispose();
        }
        if (index >= 0 && index < Series.Count)
        {
            Series.RemoveAt(index);
        }
    }

    /// <summary>
    /// Assigns chart appearance, series and data from the
    /// <b>System.Windows.Forms.DataVisualization.Charting.Chart</b> object.
    /// </summary>
    /// <param name="sourceChart">Chart object to assign data from.</param>
    /// <remarks>
    /// Use this method if you have a chart in your application and want to print it in FastReport.
    /// To do this, put an empty MSChartObject in your report and execute the following code:
    /// <code>
    /// report.Load("...");
    /// MSChartObject reportChart = report.FindObject("MSChart1") as MSChartObject;
    /// reportChart.AssignChart(applicationChart);
    /// report.Show();
    /// </code>
    /// </remarks>
    public void AssignChart(Chart sourceChart)
    {
      using (MemoryStream ms = new MemoryStream())
      {
        sourceChart.Serializer.Content = SerializationContents.All;
        sourceChart.Serializer.Save(ms);
        ms.Position = 0;
        Chart.Serializer.Load(ms);
      }
    }

    /// <inheritdoc/>
    public override void Assign(Base source)
    {
      base.Assign(source);
      MSChartObject src = source as MSChartObject;

      DataSource = src.DataSource;
      Filter = src.Filter;
      AlignXValues = src.AlignXValues;
      AutoSeriesForce = src.AutoSeriesForce;
      AutoSeriesColumn = src.AutoSeriesColumn;
      AutoSeriesColor = src.AutoSeriesColor;
      AutoSeriesSortOrder = src.AutoSeriesSortOrder;

      using (MemoryStream stream = new MemoryStream())
      {
        src.Chart.Serializer.Content = SerializationContents.All;
        src.Chart.Serializer.Save(stream);
        stream.Position = 0;
        Chart.Serializer.Reset();
        Chart.Serializer.Load(stream);
      }  
    }

    private Font NewFontDpi(Font prototype)
    {
      return new Font(prototype.Name, prototype.Size * 96f / DrawUtils.ScreenDpi, prototype.Style);
    }

    private Font OldFontDpi(Font prototype)
    {
      return new Font(prototype.Name, prototype.Size * DrawUtils.ScreenDpi / 96f, prototype.Style);
    }

    /// <inheritdoc/>
    public override void Draw(FRPaintEventArgs e)
    {
      base.Draw(e);

      Rectangle chartRect = new Rectangle((int)Math.Round(AbsLeft), (int)Math.Round(AbsTop),
        (int)Math.Round(Width), (int)Math.Round(Height));

      IGraphicsState state = e.Graphics.Save();
      try
      {
        if (IsPrinting)
        {
/*          chartRect = new Rectangle((int)Math.Round(AbsLeft * e.ScaleX), (int)Math.Round(AbsTop * e.ScaleY),
            (int)Math.Round(Width * e.ScaleX), (int)Math.Round(Height * e.ScaleY));
          
          // workaround the MS Chart bug - series border is not scaled properly
          int[] borderWidths = new int[Series.Count];
          for (int i = 0; i < Series.Count; i++)
          {
            int borderWidth = Series[i].SeriesSettings.BorderWidth;
            borderWidths[i] = borderWidth;
            Series[i].SeriesSettings.BorderWidth = (int)Math.Round(borderWidth * e.ScaleX);
          }
          FChart.Printing.PrintPaint(e.Graphics, chartRect);
          for (int i = 0; i < Series.Count; i++)
          {
            Series[i].SeriesSettings.BorderWidth = borderWidths[i];
          }*/

          // PrintPaint method is buggy when printing directly on printer's canvas.
          // We use temp bitmap instead.
          using (Bitmap bmp = new Bitmap((int)Math.Round(Width * e.ScaleX), (int)Math.Round(Height * e.ScaleY)))
          using (GdiGraphics g = new GdiGraphics(bmp))
          {
            g.ScaleTransform(e.ScaleX, e.ScaleY);
            chart.Printing.PrintPaint(g, new Rectangle(0, 0, (int)Math.Round(Width), (int)Math.Round(Height)));
            e.Graphics.DrawImage(bmp, new RectangleF((int)Math.Round(AbsLeft * e.ScaleX), (int)Math.Round(AbsTop * e.ScaleY),
              (int)Math.Round(Width * e.ScaleX), (int)Math.Round(Height * e.ScaleY)),
              new RectangleF(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
          }
        }
        else
        {
          e.Graphics.ScaleTransform(e.ScaleX, e.ScaleY);

          Color saveBackSecondaryColor = Color.Empty;
          ChartHatchStyle saveBackHatchStyle = ChartHatchStyle.None;
          Color saveBorderColor = Color.Empty;
          ChartDashStyle saveBorderStyle = ChartDashStyle.NotSet;
          int saveBorderWidth = 0;
          if (HotPoint != null)
          {
            saveBackSecondaryColor = HotPoint.BackSecondaryColor;
            saveBackHatchStyle = HotPoint.BackHatchStyle;
            saveBorderColor = HotPoint.BorderColor;
            saveBorderStyle = HotPoint.BorderDashStyle;
            saveBorderWidth = HotPoint.BorderWidth;
            
            HotPoint.BackHatchStyle = ChartHatchStyle.LightUpwardDiagonal;
            HotPoint.BackSecondaryColor = Color.White;
            HotPoint.BorderColor = Color.Orange;
            HotPoint.BorderDashStyle = ChartDashStyle.Solid;
            HotPoint.BorderWidth = 2;
          }

          // scale chart fonts
          if (DrawUtils.ScreenDpi != 96)
          {
            foreach (Title t in chart.Titles) t.Font = NewFontDpi(t.Font);
            foreach (Legend l in chart.Legends) { l.Font = NewFontDpi(l.Font); l.TitleFont = NewFontDpi(l.TitleFont); }
            foreach (ChartArea a in chart.ChartAreas) foreach (Axis ax in a.Axes) { ax.LabelStyle.Font = NewFontDpi(ax.LabelStyle.Font); ax.TitleFont = NewFontDpi(ax.TitleFont); }
            foreach (Series s in chart.Series) s.Font = NewFontDpi(s.Font); 
          }

          chart.Printing.PrintPaint(e.Graphics, chartRect);

          // set chart fonts back
          if (DrawUtils.ScreenDpi != 96)
          {
            foreach (Title t in chart.Titles) t.Font = OldFontDpi(t.Font);
            foreach (Legend l in chart.Legends) { l.Font = OldFontDpi(l.Font); l.TitleFont = OldFontDpi(l.TitleFont); }
            foreach (ChartArea a in chart.ChartAreas) foreach (Axis ax in a.Axes) { ax.LabelStyle.Font = OldFontDpi(ax.LabelStyle.Font); ax.TitleFont = OldFontDpi(ax.TitleFont); }
            foreach (Series s in chart.Series) s.Font = OldFontDpi(s.Font);
          }

          if (HotPoint != null)
          {
            HotPoint.BackSecondaryColor = saveBackSecondaryColor;
            HotPoint.BackHatchStyle = saveBackHatchStyle;
            HotPoint.BorderColor = saveBorderColor;
            HotPoint.BorderDashStyle = saveBorderStyle;
            HotPoint.BorderWidth = saveBorderWidth;
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
      MSChartObject c = writer.DiffObject as MSChartObject;
      base.Serialize(writer);

      if (DataSource != null)
        writer.WriteRef("DataSource", DataSource);
      if (Filter != c.Filter)
        writer.WriteStr("Filter", Filter);
      if (AlignXValues != c.AlignXValues)
        writer.WriteBool("AlignXValues", AlignXValues);
      if (AutoSeriesColumn != c.AutoSeriesColumn)
        writer.WriteStr("AutoSeriesColumn", AutoSeriesColumn);
      if (AutoSeriesColor != c.AutoSeriesColor)
        writer.WriteStr("AutoSeriesColor", AutoSeriesColor);
      if (AutoSeriesSortOrder != c.AutoSeriesSortOrder)
        writer.WriteValue("AutoSeriesSortOrder", AutoSeriesSortOrder);
      if(AutoSeriesForce)
        writer.WriteBool("AutoSeriesForce", AutoSeriesForce);

      using (MemoryStream stream = new MemoryStream())
      {
        chart.Serializer.Content = SerializationContents.All;
        chart.Serializer.Save(stream);
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
          chart.Serializer.Reset();
          chart.Serializer.Load(stream);
        }
      }
    }

    #endregion

    #region Report Engine
    /// <inheritdoc/>
    public override void InitializeComponent()
    {
      base.InitializeComponent();
      WireEvents(true);
    }

    /// <inheritdoc/>
    public override void FinalizeComponent()
    {
      base.FinalizeComponent();
      WireEvents(false);
    }

    /// <inheritdoc/>
    public override string[] GetExpressions()
    {
      List<string> expressions = new List<string>();
      expressions.AddRange(base.GetExpressions());

      if (!String.IsNullOrEmpty(Filter))
        expressions.Add(Filter);

      return expressions.ToArray();
    }

    /// <inheritdoc/>
    public override void SaveState()
    {
      base.SaveState();
      if (!String.IsNullOrEmpty(AutoSeriesColumn))
      {
        ClearAutoSeries();
        originalChartStream = new MemoryStream();
        chart.Serializer.Content = SerializationContents.All;
        chart.Serializer.Save(originalChartStream);
      } 
    }

    /// <inheritdoc/>
    public override void RestoreState()
    {
      base.RestoreState();
      ClearAutoSeries();
      if (originalChartStream != null)
      {
        originalChartStream.Dispose();
        originalChartStream = null;
      }
    }
    
    /// <inheritdoc/>
    public override void GetData()
    {
      base.GetData();

      MSChartSeries[] serieses = null;
      if (AutoSeriesForce)
      {
        serieses = new MSChartSeries[Series.Count];
        for (int i = 0; i < Series.Count; i++)
          serieses[i] = Series[i];
      }

      if (DataSource != null && !IsOnFooter)
      {
        Series.ResetData();
        DataSource.Init(Filter);
        DataSource.First();

        while (DataSource.HasMoreRows)
        {
          if (!String.IsNullOrEmpty(AutoSeriesColumn))
          {
            object autoSeriesKey = Report.Calc(AutoSeriesColumn);
            if (autoSeriesKey != null)
            {
              foreach (MSChartSeries series in MakeAutoSeries(autoSeriesKey, serieses))
                series.ProcessData();
            }
          }
          else
            Series.ProcessData();
          
          DataSource.Next();
        }
      }

      Series.FinishData();
      
      if (AlignXValues)
        Chart.AlignDataPointsByAxisLabel();
      if (!String.IsNullOrEmpty(AutoSeriesColumn) && AutoSeriesSortOrder != SortOrder.None)
        SortAutoSeries();  
    }
    #endregion

    #region IParent Members
    /// <inheritdoc/>
    public bool CanContain(Base child)
    {
      return child is MSChartSeries;
    }

    /// <inheritdoc/>
    public void GetChildObjects(ObjectCollection list)
    {
      foreach (MSChartSeries series in Series)
      {
        list.Add(series);
      }
    }

    /// <inheritdoc/>
    public void AddChild(Base child)
    {
      if (child is MSChartSeries)
        Series.Add(child as MSChartSeries);
    }

    /// <inheritdoc/>
    public void RemoveChild(Base child)
    {
      if (child is MSChartSeries)
        Series.Remove(child as MSChartSeries);
    }

    /// <inheritdoc/>
    public int GetChildOrder(Base child)
    {
      if (child is MSChartSeries)
        return Series.IndexOf(child as MSChartSeries);
      return 0;
    }

    /// <inheritdoc/>
    public void SetChildOrder(Base child, int order)
    {
    }

    /// <inheritdoc/>
    public void UpdateLayout(float dx, float dy)
    {
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="MSChartObject"/> with default settings.
    /// </summary>
    public MSChartObject()
    {
      series = new SeriesCollection(this);
      chart = new Chart();
      FlagUseFill = false;
      FlagUseBorder = false;
      FlagProvidesHyperlinkValue = true;
    }
  }


  /// <summary>
  /// Represents the small chart object (called sparkline) fully based on MSChartObject.
  /// </summary>
  public partial class SparklineObject : MSChartObject
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SparklineObject"/> with default settings.
    /// </summary>
    public SparklineObject()
    {
    }
  }
}
