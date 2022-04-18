using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Controls
{
  internal class AngleControl : Control
  {
    private int angle;
    private bool showBorder;
    private NumericUpDown udAngle;
    private bool changed;
    private bool updating;
    
    public event EventHandler AngleChanged;
    
        public NumericUpDown NumericAngle { get { return udAngle; } }

    public int Angle
    {
      get { return angle; }
      set 
      {
        updating = true;
        if (value < 0)
          value += 360;
        angle = value % 360; 
        udAngle.Value = angle;
        Refresh();
        updating = false;
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

    public bool Changed
    {
      get { return changed; }
      set { changed = value; }
    }

    private void RotateTo(int x, int y)
    {
      int size = Math.Min(Width, Height - 30);
      int cx = size / 2;
      int cy = cx;
      int r = x - cx == 0 ? (y > cy ? 90 : 270) : (int)(Math.Atan2((y - cy) , (x - cx)) * 180 / Math.PI);
      Angle = (int)Math.Round(r / 15f) * 15;
    }

    private void udAngle_ValueChanged(object sender, EventArgs e)
    {
      if (updating)
        return;
      Angle = (int)udAngle.Value;
      Change();
    }
    
    private void Change()
    {
      changed = true;
      if (AngleChanged != null)
        AngleChanged(this, EventArgs.Empty);
    }
    
    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics g = e.Graphics;
      Pen p = null;
      
      // draw control border
      if (showBorder)
      {
        using (p = new Pen(Color.FromArgb(127, 157, 185)))
        {
          g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
        }
      }
      
      g.SmoothingMode = SmoothingMode.AntiAlias;
      // draw ticks
      int size = Math.Min(Width, Height - 30);
      int cx = size / 2;
      int cy = cx;
      int radius = size / 2 - 10;
      p = new Pen(Color.Silver);
      p.DashStyle = DashStyle.Dot;
      g.DrawEllipse(p, 10, 10, size - 20, size - 20);
      p.Dispose();
      for (int i = 0; i < 360; i+= 45)
      {
        Rectangle rect = new Rectangle(cx + (int)(Math.Cos(Math.PI / 180 * i) * radius) - 2,
          cy + (int)(Math.Sin(Math.PI / 180 * i) * radius) - 2, DpiHelper.ConvertUnits(4), DpiHelper.ConvertUnits(4));
        g.FillEllipse(i == angle ? Brushes.DarkOrange : SystemBrushes.Window, rect);
        g.DrawEllipse(i == angle ? Pens.DarkOrange : Pens.Black, rect);
      }
      
      // draw sample
      using (StringFormat sf = new StringFormat())
      {
        sf.Alignment = StringAlignment.Center;
        sf.LineAlignment = StringAlignment.Center;
        StandardTextRenderer.Draw(Res.Get("Misc,Sample"), new GdiGraphics(g, false), Font, SystemBrushes.WindowText, null,
          new RectangleF(cx - radius + 1, cy - radius + 1, radius * 2, radius * 2),
          sf, angle, 1);
      }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      RotateTo(e.X, e.Y);
    }
    
    protected override void OnMouseMove(MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
        RotateTo(e.X, e.Y);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      Change();
    }

    protected override void OnLayout(LayoutEventArgs levent)
    {
      base.OnLayout(levent);
            udAngle.Location = new Point(10, Height - udAngle.Height - DpiHelper.ConvertUnits(8));
      udAngle.Size = new Size(Width - 20, 20);
    }

    public AngleControl()
    {
      udAngle = new NumericUpDown();
      udAngle.Maximum = 360;
      udAngle.Increment = 15;
      udAngle.ValueChanged += new EventHandler(udAngle_ValueChanged);
      Controls.Add(udAngle);
      showBorder = true;
      SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
      Size = new Size(100, 130);
      BackColor = SystemColors.Window;
      Font = DrawUtils.DefaultFont;
    }

  }
}
