using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Controls
{
  internal class LineStyleControl : Control
  {
    private LineStyle[] styles;
    private LineStyle style;
    private float lineWidth;
    private Color lineColor;
    private bool showBorder;
        private float controlRatio;

    public event EventHandler StyleSelected;
    
    public LineStyle Style
    {
      get { return style; }
      set
      {
        style = value;
        Refresh();
      }
    }
    
    public float LineWidth
    {
      get { return lineWidth; }
      set
      {
        lineWidth = value;
        Refresh();
      }
    }

    public Color LineColor
    {
      get { return lineColor; }
      set
      {
        lineColor = value;
        Refresh();
      }
    }

    public bool ShowBorder
    {
      get { return showBorder; }
      set 
      { 
        showBorder = value;
        Refresh();
      }  
    }

    private void DrawHighlight(Graphics g, Rectangle rect)
    {
      Brush brush = new SolidBrush(Color.FromArgb(193, 210, 238));
      g.FillRectangle(brush, rect);
      Pen pen = new Pen(Color.FromArgb(49, 106, 197));
      g.DrawRectangle(pen, rect);
      brush.Dispose();
      pen.Dispose();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics g = e.Graphics;
      Pen p = null;
      // draw control border
      if (showBorder)
      {
        p = new Pen(Color.FromArgb(127, 157, 185));
        g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
        p.Dispose();
      }
      // draw items
      for (int i = 0; i < styles.Length; i++)
      {
        // highlight active style
        if (this.styles[i] == style)
                    DrawHighlight(g, new Rectangle((int)(controlRatio * 4), i * (int)(controlRatio * 15) + (int)(controlRatio * 4), Width - (int)(controlRatio * 9), (int)(controlRatio * 15)));
        p = new Pen(lineColor, lineWidth < 1.5f ? 1.5f : lineWidth);
        DashStyle[] dashStyles = new DashStyle[] { 
          DashStyle.Solid, DashStyle.Dash, DashStyle.Dot, DashStyle.DashDot, DashStyle.DashDotDot, DashStyle.Solid };
        p.DashStyle = dashStyles[(int)this.styles[i]];
        if (this.styles[i] == LineStyle.Double)
        {
          p.Width *= 2.5f;
          p.CompoundArray = new float[] { 0, 0.4f, 0.6f, 1 };
        }  
        g.DrawLine(p, (int)(controlRatio * 8), i * (int)(controlRatio * 15) + (int)(controlRatio * 12), Width - (int)(controlRatio * 8), i * (int)(controlRatio * 15) + (int)(controlRatio * 12));
        p.Dispose();
      }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      int i = (e.Y - (int)(4 * controlRatio)) / (int)(controlRatio * 15);
      if (i < 0)
        i = 0;
      if (i > styles.Length - 1)
        i = styles.Length - 1;
      
      Style = styles[i];
      if (StyleSelected != null)
        StyleSelected(this, EventArgs.Empty);
    }

        public void UpdateDpiDependencies(float ratio)
        {
            controlRatio = ratio;
        }

    public LineStyleControl()
    {
      styles = new LineStyle[] {
        LineStyle.Solid, LineStyle.Dash, LineStyle.Dot, LineStyle.DashDot, LineStyle.DashDotDot, LineStyle.Double };
      lineColor = Color.Black;
      lineWidth = 1;
      showBorder = true;
      BackColor = SystemColors.Window;
      SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
      Size = new Size(70, 100);
            controlRatio = DpiHelper.Multiplier;
    }
  }
}
