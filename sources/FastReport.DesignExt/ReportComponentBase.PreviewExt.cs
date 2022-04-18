using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using FastReport.Utils;
using FastReport.TypeEditors;
using FastReport.TypeConverters;

namespace FastReport
{
  partial class ReportComponentBase
  {

    #region Properties
    /// <summary>
    /// This event occurs when the user moves the mouse over the object in the preview window.
    /// </summary>
    public event MouseEventHandler MouseMove;

    /// <summary>
    /// This event occurs when the user releases the mouse button in the preview window.
    /// </summary>
    public event MouseEventHandler MouseUp;

    /// <summary>
    /// This event occurs when the user clicks the mouse button in the preview window.
    /// </summary>
    public event MouseEventHandler MouseDown;

    /// <summary>
    /// This event occurs when the mouse enters the object's bounds in the preview window.
    /// </summary>
    public event EventHandler MouseEnter;

    /// <summary>
    /// This event occurs when the mouse leaves the object's bounds in the preview window.
    /// </summary>
    public event EventHandler MouseLeave;
    #endregion

    #region Public Methods
    /// <summary>
    /// Copies event handlers from another similar object.
    /// </summary>
    /// <param name="source">The object to copy handlers from.</param>
    public virtual void AssignPreviewEvents(Base source)
    {
      ReportComponentBase src = source as ReportComponentBase;
      if (src == null)
        return;
      Click = src.Click;
      MouseMove = src.MouseMove;
      MouseUp = src.MouseUp;
      MouseDown = src.MouseDown;
      MouseEnter = src.MouseEnter;
      MouseLeave = src.MouseLeave;
    }

    internal void ApplyHoverStyle()
    {
      ApplyStyle(HoverStyle);
    }

    /// <summary>
    /// This method fires the <b>MouseMove</b> event and the script code connected to the <b>MouseMoveEvent</b>.
    /// </summary>
    /// <param name="e">Event data.</param>
    public virtual void OnMouseMove(MouseEventArgs e)
    {
      if (MouseMove != null)
        MouseMove(this, e);
      InvokeEvent(MouseMoveEvent, e);
    }

    /// <summary>
    /// This method fires the <b>MouseUp</b> event and the script code connected to the <b>MouseUpEvent</b>.
    /// </summary>
    /// <param name="e">Event data.</param>
    public virtual void OnMouseUp(MouseEventArgs e)
    {
      if (MouseUp != null)
        MouseUp(this, e);
      InvokeEvent(MouseUpEvent, e);
    }

    /// <summary>
    /// This method fires the <b>MouseDown</b> event and the script code connected to the <b>MouseDownEvent</b>.
    /// </summary>
    /// <param name="e">Event data.</param>
    public virtual void OnMouseDown(MouseEventArgs e)
    {
      if (MouseDown != null)
        MouseDown(this, e);
      InvokeEvent(MouseDownEvent, e);
    }

    /// <summary>
    /// This method fires the <b>MouseEnter</b> event and the script code connected to the <b>MouseEnterEvent</b>.
    /// </summary>
    /// <param name="e">Event data.</param>
    public virtual void OnMouseEnter(EventArgs e)
    {
      if (!String.IsNullOrEmpty(HoverStyle))
      {
        SaveStyle();
        ApplyHoverStyle();
        if (Page != null)
          Page.Refresh();
      }
      if (MouseEnter != null)
        MouseEnter(this, e);
      InvokeEvent(MouseEnterEvent, e);
    }

    /// <summary>
    /// This method fires the <b>MouseLeave</b> event and the script code connected to the <b>MouseLeaveEvent</b>.
    /// </summary>
    /// <param name="e">Event data.</param>
    public virtual void OnMouseLeave(EventArgs e)
    {
      if (!String.IsNullOrEmpty(HoverStyle))
      {
        RestoreStyle();
        if (Page != null)
          Page.Refresh();
      }
      if (MouseLeave != null)
        MouseLeave(this, e);
      InvokeEvent(MouseLeaveEvent, e);
    }

    /// <summary>
    /// This method is fired when the user scrolls the mouse in the preview window.
    /// </summary>
    /// <param name="e">Event data.</param>
    public virtual void OnMouseWheel(MouseEventArgs e)
    {
    }
    #endregion
  }
}