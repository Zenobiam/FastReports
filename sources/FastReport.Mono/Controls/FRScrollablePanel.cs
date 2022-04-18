using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace FastReport.Controls
{
  internal class FRScrollablePanel : UserControl
  {
    #region Fields
    private VScrollBar _VScrollBar = null;
    private HScrollBar _HScrollBar = null;
    private Control _Thumb = null;
    private bool _VScrollBarVisible;
    private bool _HScrollBarVisible;
    private bool _ThumbVisible;
    private Point _AutoScrollPosition = Point.Empty;
    private bool _AutoScroll = false;
    private Size _AutoScrollMinSize = Size.Empty;
    protected Color FColor1;
    protected Color FColor2;
    private bool FDefaultPaint;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the reference to internal vertical scroll-bar control if one is created or null if no scrollbar is visible.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public VScrollBar VScrollBar
    {
      get { return _VScrollBar; }
    }

    /// <summary>
    /// Gets the reference to internal horizontal scroll-bar control if one is created or null if no scrollbar is visible.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public HScrollBar HScrollBar
    {
      get { return _HScrollBar; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control enables the user to scroll to items placed outside of its visible boundaries.
    /// </summary>
    [Browsable(true), DefaultValue(false)]
    public new bool AutoScroll
    {
      get { return _AutoScroll; }
      set
      {
        if (_AutoScroll != value)
        {
          _AutoScroll = value;
          UpdateScrollBars();
        }
      }
    }

    /// <summary>
    /// Gets or sets the minimum size of the auto-scroll. Returns a Size that represents the minimum height and width of the scrolling area in pixels.
    /// This property is managed internally by control and should not be modified.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Size AutoScrollMinSize
    {
      get { return _AutoScrollMinSize; }
      set
      {
        _AutoScrollMinSize = value;
        UpdateScrollBars();
      }
    }

    /// <summary>
    /// Gets or sets the location of the auto-scroll position.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Point AutoScrollPosition
    {
      get { return _AutoScrollPosition; }
      set
      {
        if (!HScrollBarVisible)
          value.X = 0;
        else
          value.X = -Math.Max(Math.Min(-value.X, _HScrollBar.Maximum - _HScrollBar.LargeChange), _HScrollBar.Minimum);
          
        if (!VScrollBarVisible)
          value.Y = 0;
        else
          value.Y = -Math.Max(Math.Min(-value.Y, _VScrollBar.Maximum - _VScrollBar.LargeChange), _VScrollBar.Minimum);

        if (_AutoScrollPosition != value)
        {
          _AutoScrollPosition = value;
          if (_AutoScroll)
          {
            int deltaX = 0;
            int deltaY = 0;
            if (VScrollBarVisible && _VScrollBar.Value != -_AutoScrollPosition.Y)
            {
              deltaY = _VScrollBar.Value - (-_AutoScrollPosition.Y);
              _VScrollBar.Value = -_AutoScrollPosition.Y;
            }  
            if (HScrollBarVisible && _HScrollBar.Value != -_AutoScrollPosition.X)
            {
              deltaX = _HScrollBar.Value - (-_AutoScrollPosition.X);
              _HScrollBar.Value = -_AutoScrollPosition.X;
            }  
            
            Invalidate();
          }
        }
      }
    }

    [Browsable(true), Category("Behavior"), DefaultValue(false)]
    public bool DefaultPaint
    {
      get { return FDefaultPaint; }
      set { FDefaultPaint = value; }
    }

    private bool HScrollBarVisible
    {
      get { return _HScrollBarVisible; }
      set
      {
        _HScrollBarVisible = value;
        _HScrollBar.Visible = value;
      }
    }

    private bool VScrollBarVisible
    {
      get { return _VScrollBarVisible; }
      set
      {
        _VScrollBarVisible = value;
        _VScrollBar.Visible = value;
      }
    }

    private bool ThumbVisible
    {
      get { return _ThumbVisible; }
      set
      {
        _ThumbVisible = value;
        _Thumb.Visible = value;
      }
    }
    #endregion

    #region Private Methods
    private void UpdateColors()
    {
      FColor1 = SystemColors.AppWorkspace;
      FColor2 = FColor1;
      Refresh();
      UpdateScrollStyle();
    }

    private void UpdateScrollStyle()
    {
      _Thumb.BackColor = FColor1;
    }

    private void UpdateScrollBars()
    {
      if (!_AutoScroll)
      {
        VScrollBarVisible = false;
        HScrollBarVisible = false;
        ThumbVisible = false;
        return;
      }

      Rectangle innerBounds = this.ClientRectangle;
      // Check do we need vertical scrollbar
      Size scrollSize = _AutoScrollMinSize;
      if (scrollSize.Height > innerBounds.Height)
      {
        VScrollBarVisible = true;
        if (_VScrollBar.Minimum != 0)
          _VScrollBar.Minimum = 0;
        if (_VScrollBar.LargeChange != innerBounds.Height && innerBounds.Height > 0)
          _VScrollBar.LargeChange = innerBounds.Height;
        _VScrollBar.SmallChange = 100;
        if (_VScrollBar.Maximum != _AutoScrollMinSize.Height)
          _VScrollBar.Maximum = _AutoScrollMinSize.Height;
        if (_VScrollBar.Value != -_AutoScrollPosition.Y)
          _VScrollBar.Value = (Math.Min(_VScrollBar.Maximum, Math.Abs(_AutoScrollPosition.Y)));
      }
      else
      {
        VScrollBarVisible = false;
      }  

      // Check horizontal scrollbar
      if (scrollSize.Width > innerBounds.Width)
      {
        HScrollBarVisible = true;
        if (_HScrollBar.Minimum != 0)
          _HScrollBar.Minimum = 0;
        if (_HScrollBar.LargeChange != innerBounds.Width && innerBounds.Width > 0)
          _HScrollBar.LargeChange = innerBounds.Width;
        if (_HScrollBar.Maximum != _AutoScrollMinSize.Width)
          _HScrollBar.Maximum = _AutoScrollMinSize.Width;
        if (_HScrollBar.Value != -_AutoScrollPosition.X)
          _HScrollBar.Value = (Math.Min(_HScrollBar.Maximum, Math.Abs(_AutoScrollPosition.X)));
        _HScrollBar.SmallChange = 100;
      }
      else
      {
        HScrollBarVisible = false;
      }  

      UpdateScrollStyle();
      RepositionScrollBars();
    }

    private void VScrollBarScroll(object sender, ScrollEventArgs e)
    {
      if (_AutoScrollPosition.Y != -e.NewValue)
      {
        int delta = -e.NewValue - _AutoScrollPosition.Y;
        _AutoScrollPosition.Y = -e.NewValue;
        this.OnScroll(e);
        
        this.Invalidate();
      }
    }

    private void HScrollBarScroll(object sender, ScrollEventArgs e)
    {
      if (_AutoScrollPosition.X != -e.NewValue)
      {
        int delta = -e.NewValue - _AutoScrollPosition.X;
        _AutoScrollPosition.X = -e.NewValue;
        this.OnScroll(e);

        this.Invalidate();
      }
    }

    private void RepositionScrollBars()
    {
      Rectangle innerBounds = this.ClientRectangle;
      if (HScrollBarVisible)
      {
        int width = innerBounds.Width;
        if (VScrollBarVisible)
          width -= _VScrollBar.Width - 1;
        _HScrollBar.Bounds = new Rectangle(innerBounds.X, innerBounds.Bottom - _HScrollBar.Height + 1, width, _HScrollBar.Height);
      }

      if (VScrollBarVisible)
      {
        int height = innerBounds.Height;
        if (HScrollBarVisible)
          height -= _HScrollBar.Height - 1;
        _VScrollBar.Bounds = new Rectangle(innerBounds.Right - _VScrollBar.Width + 1, innerBounds.Y, _VScrollBar.Width, height);
      }

      if (VScrollBarVisible && HScrollBarVisible)
      {
        ThumbVisible = true;
        _Thumb.Bounds = new Rectangle(_HScrollBar.Bounds.Right, _VScrollBar.Bounds.Bottom, _VScrollBar.Width, _HScrollBar.Height);
      }
      else
      {
        ThumbVisible = false;
      }  
    }
    #endregion

    #region Protected Methods
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (_VScrollBar != null)
          _VScrollBar.Dispose();
        _VScrollBar = null;
        if (_HScrollBar != null)
          _HScrollBar.Dispose();
        _HScrollBar = null;
        if (_Thumb != null)
          _Thumb.Dispose();
        _Thumb = null;
      }
      base.Dispose(disposing);
    }
    
    protected override void OnMouseWheel(MouseEventArgs e)
    {
      if (VScrollBarVisible)
      {
        int newValue = _VScrollBar.Value + _VScrollBar.SmallChange * (e.Delta < 0 ? 1 : -1);
        if (newValue < _VScrollBar.Minimum)
          newValue = _VScrollBar.Minimum;
        if (newValue > _VScrollBar.Maximum - _VScrollBar.LargeChange + 1)
          newValue = _VScrollBar.Maximum - _VScrollBar.LargeChange + 1;
        ScrollEventType scType = e.Delta < 0 ? ScrollEventType.SmallIncrement : ScrollEventType.SmallDecrement;
        VScrollBarScroll(this, new ScrollEventArgs(scType, _VScrollBar.Value, newValue));
        _VScrollBar.Value = newValue;
      }
      base.OnMouseWheel(e);
    }

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      
      UpdateScrollBars();
      AutoScrollPosition = AutoScrollPosition;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      if (this.ClientRectangle.Width == 0 || this.ClientRectangle.Height == 0)
        return;
      if (DefaultPaint)
      {
        base.OnPaint(e);
        return;
      }

      using (SolidBrush brush = new SolidBrush(FColor1))
      {
        e.Graphics.FillRectangle(brush, ClientRectangle);
      }  
    }
    #endregion

    public FRScrollablePanel()
    {
      _VScrollBar = new VScrollBar();
      _VScrollBar.Width = SystemInformation.VerticalScrollBarWidth;
      _VScrollBar.Cursor = Cursors.Default;
      this.Controls.Add(_VScrollBar);
      _VScrollBar.Scroll += new ScrollEventHandler(VScrollBarScroll);
      // CreateControl is needed to avoid error in mdi child mode
      _VScrollBar.CreateControl();

      _HScrollBar = new HScrollBar();
      _HScrollBar.Height = SystemInformation.HorizontalScrollBarHeight;
      _HScrollBar.Cursor = Cursors.Default;
      this.Controls.Add(_HScrollBar);
      _HScrollBar.Scroll += new ScrollEventHandler(HScrollBarScroll);
      _HScrollBar.CreateControl();

      _Thumb = new Control();
      this.Controls.Add(_Thumb);
      _Thumb.CreateControl();

      SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
    }
  }
}
