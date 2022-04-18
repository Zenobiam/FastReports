using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using FastReport.Utils;

namespace FastReport.Controls
{
  internal class SampleReportControl : Control
  {
    private Report report;
    private float zoom;
    private bool fullPagePreview;
    
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Report Report
    {
      get { return report; }
      set
      {
        report = value;
        Refresh();
      }
    }
    
    [DefaultValue(1f)]
    public float Zoom
    {
      get { return zoom; }
      set
      {
        zoom = value;
        Refresh();
      }
    }
    
    [DefaultValue(false)]
    public bool FullPagePreview
    {
      get { return fullPagePreview; }
      set
      {
        fullPagePreview = value;
        Refresh();
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics g = e.Graphics;
      
      if (report != null && report.Pages.Count > 0 && report.Pages[0] is ReportPage)
      {
        ReportPage page = report.Pages[0] as ReportPage;
        
        float zoom = this.zoom;
        if (FullPagePreview)
        {
          float pageWidth = page.WidthInPixels;
          float pageHeight = page.HeightInPixels;
          zoom = Math.Min((Width - 20) / pageWidth, (Height - 20) / pageHeight);
        }  
        
        FRPaintEventArgs args = new FRPaintEventArgs(g, zoom, zoom, report.GraphicCache);
        g.TranslateTransform(10, 10);
        page.Draw(args);
        g.TranslateTransform(-10, -10);
      }

      // draw control frame
      using (Pen p = new Pen(Color.FromArgb(127, 157, 185)))
      {
        g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
      }
    }
    
    public SampleReportControl()
    {
      zoom = 1;
      BackColor = SystemColors.AppWorkspace;
      SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
    }
  }
}
