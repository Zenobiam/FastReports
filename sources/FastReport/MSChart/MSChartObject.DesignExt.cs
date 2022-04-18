using System;
using System.Collections.Generic;
using System.Text;
using FastReport.DataVisualization.Charting;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections;
using FastReport;
using FastReport.Utils;
using FastReport.Data;
using FastReport.TypeEditors;
using System.Drawing.Drawing2D;

namespace FastReport.MSChart
{
  partial class MSChartObject : IHasEditor
  {
    #region Properties
    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override Border Border
    {
      get { return base.Border; }
      set { base.Border = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override FillBase Fill
    {
      get { return base.Fill; }
      set { base.Fill = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new string Style
    {
      get { return base.Style; }
      set { base.Style = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new string EvenStyle
    {
      get { return base.EvenStyle; }
      set { base.EvenStyle = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new string HoverStyle
    {
      get { return base.HoverStyle; }
      set { base.HoverStyle = value; }
    }
    #endregion
    
    #region Public Methods
    /// <inheritdoc/>
    public override SizeF GetPreferredSize()
    {
      if ((Page as ReportPage).IsImperialUnitsUsed)
        return new SizeF(Units.Inches * 3, Units.Inches * 2);
      return new SizeF(Units.Millimeters * 80, Units.Millimeters * 50);
    }

    /// <inheritdoc/>
    public override void OnBeforeInsert(int flags)
    {
      base.OnBeforeInsert(flags);
      MSChartSeries series = AddSeries(SeriesChartType.Column);
      series.CreateDummyData();
    }

    /// <inheritdoc/>
    public override void OnAfterInsert(InsertFrom source)
    {
      base.OnAfterInsert(source);
      if (source == InsertFrom.NewObject)
        Series[0].CreateUniqueName();
    }

    /// <inheritdoc/>
    public bool InvokeEditor()
    {
      using (MSChartObjectEditorForm form = new MSChartObjectEditorForm())
      {
        form.ChartObject = this;
        return form.ShowDialog() == DialogResult.OK;
      }
    }

        /// <inheritdoc/>
        public override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (Hyperlink.Kind == HyperlinkKind.DetailPage || Hyperlink.Kind == HyperlinkKind.DetailReport)
            {
                Chart.Size = new Size((int)Width, (int)Height);
                HitTestResult hitTest = Chart.HitTest(e.X, e.Y);
                if (hitTest.ChartElementType == ChartElementType.DataPoint)
                {
                    HotPoint = hitTest.Series.Points[hitTest.PointIndex];
                    Hyperlink.Value = HotPoint.AxisLabel;
                    Cursor = Cursors.Hand;
                }
                else
                {
                    HotPoint = null;
                    Hyperlink.Value = "";
                    Cursor = Cursors.Default;
                }
            }
        }

        /// <inheritdoc/>
        public override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            HotPoint = null;
        }

    #endregion
   }


  partial class SparklineObject
  {
    /// <inheritdoc/>
    public override SizeF GetPreferredSize()
    {
      if (Page is ReportPage && (Page as ReportPage).IsImperialUnitsUsed)
        return new SizeF(Units.Inches * 1, Units.Inches * 0.2f);
      return new SizeF(Units.Millimeters * 25, Units.Millimeters * 5);
    }

    /// <inheritdoc/>
    public override void OnBeforeInsert(int flags)
    {
      MSChartSeries series = AddSeries(SeriesChartType.FastLine);
      series.CreateDummyData();
      Chart.BorderSkin.SkinStyle = BorderSkinStyle.None;
      Chart.BorderlineDashStyle = ChartDashStyle.NotSet;
      Chart.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;
      Chart.ChartAreas[0].AxisY.Enabled = AxisEnabled.False;
      Chart.Legends[0].Enabled = false;
    }
  }
}
