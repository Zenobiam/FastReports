using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using FastReport.Utils;
using FastReport.TypeEditors;
using FastReport.TypeConverters;
using FastReport.Design.PageDesigners.Page;

namespace FastReport
{
  /// <summary>
  /// The style of the report object markers.
  /// </summary>
  public enum MarkerStyle 
  { 
    /// <summary>
    /// Rectangle marker.
    /// </summary>
    Rectangle, 
    
    /// <summary>
    /// Small markers at the object's corners.
    /// </summary>
    Corners 
  }
  
  partial class ReportComponentBase
  {
    #region Properties
    /// <inheritdoc/>
    [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
    public override float Left
    {
      get { return base.Left; }
      set { base.Left = value; }
    }

    /// <inheritdoc/>
    [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
    public override float Top
    {
      get { return base.Top; }
      set { base.Top = value; }
    }

    /// <inheritdoc/>
    [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
    public override float Width
    {
      get { return base.Width; }
      set { base.Width = value; }
    }

    /// <inheritdoc/>
    [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
    public override float Height
    {
      get { return base.Height; }
      set { base.Height = value; }
    }
    #endregion

    #region Private Methods
    private bool ShouldSerializeHyperlink()
    {
      return !Hyperlink.Equals(new Hyperlink(null));
    }

    private bool ShouldSerializeBorder()
    {
      return !Border.Equals(new Border());
    }

    private bool ShouldSerializeCursor()
    {
      return Cursor != Cursors.Default;
    }

    private bool ShouldSerializeFill()
    {
      return !(Fill is SolidFill) || !Fill.IsTransparent;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Assigns a format from another, similar object.
    /// </summary>
    /// <param name="source">Source object to assign a format from.</param>
    public virtual void AssignFormat(ReportComponentBase source)
    {
      Border = source.Border.Clone();
      Fill = source.Fill.Clone();
      style = source.Style;
    }

    /// <inheritdoc/>
    public override void HandleMouseDown(FRMouseEventArgs e)
    {
      base.HandleMouseDown(e);
      // we will use FSavedBounds to keep the delta while moving the object between bands
      savedBounds.X = 0;
      savedBounds.Y = 0;
    }
    
    /// <inheritdoc/>
    public override void CheckParent(bool immediately)
    {
      if (!(Parent is ComponentBase) || !IsSelected || IsAncestor || Dock != DockStyle.None)
        return;

      if (immediately || 
        Left < 0 || Left > (Parent as ComponentBase).Width || 
        Top < 0 || Top > (Parent as ComponentBase).Height)
      {
        if (HasFlag(Flags.CanChangeParent))
        {
          ObjectCollection list = Page.AllObjects;
          for (int i = list.Count - 1; i >= 0; i--)
          {
            ComponentBase c = list[i] as ComponentBase;
            if (c == null || c == this || !(c is IParent))
              continue;

            if (c != null && (c as IParent).CanContain(this))
            {
              bool inside;
              int bandGap = ReportWorkspace.ClassicView && IsDesigning ? BandBase.HeaderSize : 4;
              if (c is BandBase)
                inside = AbsTop > c.AbsTop - bandGap && AbsTop < c.AbsBottom - 1;
              else
                inside = AbsLeft > c.AbsLeft - 1e-4 && AbsLeft < c.AbsRight - 1e-4 &&
                  AbsTop > c.AbsTop - 1e-4 && AbsTop < c.AbsBottom - 1e-4;

              if (inside)
              {
                if (Parent != c)
                {
                  float saveAbsTop = AbsTop;
                  float saveAbsLeft = AbsLeft;

                  // keep existing offsets if the object is not aligned to the grid
                  float gridXOffset = Converter.DecreasePrecision(Left - (int)(Left / Page.SnapSize.Width + 1e-4) * Page.SnapSize.Width, 2);
                  float gridYOffset = Converter.DecreasePrecision(Top - (int)(Top / Page.SnapSize.Height + 1e-4) * Page.SnapSize.Height, 2);

                  // move the object to the new parent
                  Left = (int)((AbsLeft - c.AbsLeft) / Page.SnapSize.Width + 1e-4) * Page.SnapSize.Width + gridXOffset;
                  Top = (int)((AbsTop - c.AbsTop) / Page.SnapSize.Height + 1e-4) * Page.SnapSize.Height + gridYOffset;
                  Parent = c;

                  // correct the delta
                  savedBounds.X += saveAbsLeft - AbsLeft;
                  savedBounds.Y += saveAbsTop - AbsTop;

                  // check delta
                  if (Math.Abs(savedBounds.X) > Page.SnapSize.Width)
                  {
                    float delta = Math.Sign(savedBounds.X) * Page.SnapSize.Width;
                    Left += delta;
                    savedBounds.X -= delta;
                  }
                  if (Math.Abs(savedBounds.Y) > Page.SnapSize.Height)
                  {
                    float delta = Math.Sign(savedBounds.Y) * Page.SnapSize.Height;
                    Top += delta;
                    savedBounds.Y -= delta * 0.9f;
                  }
                }
                break;
              }
            }  
          }
        }
        else
        {
          if (Left < 0)
            Left = 0;
          if (Left > (Parent as ComponentBase).Width)
            Left = (Parent as ComponentBase).Width - 2;
          if (Top < 0)
            Top = 0;
          if (Top > (Parent as ComponentBase).Height)
            Top = (Parent as ComponentBase).Height - 2;
        }
      }
    }
    
    /// <summary>
    /// Draws the object's markers.
    /// </summary>
    /// <param name="e">Draw event arguments.</param>
    public void DrawMarkers(FRPaintEventArgs e)
    {
      if (IsDesigning && Border.Lines != BorderLines.All)
        DrawMarkersInternal(e);
    }
    
    private void DrawMarkersInternal(FRPaintEventArgs e)
    {
      DrawMarkers(e, ReportWorkspace.MarkerStyle);
    }

    /// <summary>
    /// Draws the object's markers.
    /// </summary>
    /// <param name="e">Draw event arguments.</param>
    /// <param name="style">Marker style</param>
    public void DrawMarkers(FRPaintEventArgs e, MarkerStyle style)
    {
      IGraphics g = e.Graphics;
      if (style == MarkerStyle.Corners)
      {
        Pen p = Pens.Black;
        int x = (int)Math.Round(AbsLeft * e.ScaleX);
        int y = (int)Math.Round(AbsTop * e.ScaleY);
        int x1 = (int)Math.Round(AbsRight * e.ScaleX);
        int y1 = (int)Math.Round(AbsBottom * e.ScaleY);
        g.DrawLine(p, x, y, x + 3, y);
        g.DrawLine(p, x, y, x, y + 3);
        g.DrawLine(p, x, y1, x + 3, y1);
        g.DrawLine(p, x, y1, x, y1 - 3);
        g.DrawLine(p, x1, y, x1 - 3, y);
        g.DrawLine(p, x1, y, x1, y + 3);
        g.DrawLine(p, x1, y1, x1 - 3, y1);
        g.DrawLine(p, x1, y1, x1, y1 - 3);
      }
      else if (Math.Abs(Width) > 1 || Math.Abs(Height) > 1)
      {
        g.DrawRectangle(Pens.Gainsboro, AbsLeft * e.ScaleX, AbsTop * e.ScaleY, Width * e.ScaleX, Height * e.ScaleY);
      }
    }
    
    /// <inheritdoc/>
    public override ContextMenuBase GetContextMenu()
    {
      return new ReportComponentBaseMenu(Report.Designer);
    }

    /// <inheritdoc/>
    public override SizeF GetPreferredSize()
    {
      if (Page is ReportPage && (Page as ReportPage).IsImperialUnitsUsed)
        return new SizeF(Units.Inches * 1, Units.Inches * 0.2f);
      return base.GetPreferredSize();
    }
    
    /// <inheritdoc/>
    public override void OnAfterInsert(InsertFrom source)
    {
      if (this is IHasEditor && source == InsertFrom.NewObject && ReportWorkspace.EditAfterInsert)
        (this as IHasEditor).InvokeEditor();
    }
    #endregion
  }
}