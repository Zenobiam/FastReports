using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Drawing.Design;
using FastReport.Utils;
using FastReport.Design;
using FastReport.Design.PageDesigners.Page;
using FastReport.TypeEditors;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport
{
    partial class BandBase
    {
#region Fields

        internal static int HeaderSize { get { return 20; } }

#endregion

#region Properties

        /// <inheritdoc/>
        [Browsable(false)]
        public override float Left
        {
            get { return base.Left; }
            set { base.Left = value; }
        }

        /// <inheritdoc/>
        [Browsable(false)]
        public override float Top
        {
            get { return base.Top; }
            set { base.Top = value; }
        }

        /// <inheritdoc/>
        [Browsable(false)]
        public override float Width
        {
            get { return base.Width; }
            set { base.Width = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public override DockStyle Dock
        {
            get { return base.Dock; }
            set { base.Dock = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public override AnchorStyles Anchor
        {
            get { return base.Anchor; }
            set { base.Anchor = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new bool GrowToBottom
        {
            get { return base.GrowToBottom; }
            set { base.GrowToBottom = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new ShiftMode ShiftMode
        {
            get { return base.ShiftMode; }
            set { base.ShiftMode = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [DefaultValue(false)]
        public new bool CanBreak
        {
            get { return base.CanBreak; }
            set { base.CanBreak = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new BreakableComponent BreakTo
        {
            get { return base.BreakTo; }
            set { base.BreakTo = value; }
        }

        internal bool CanDelete
        {
            get
            {
                if (Page != null)
                {
                    // do not delete the sole band on the page
                    ObjectCollection pageObjects = Page.AllObjects;
                    ObjectCollection bands = new ObjectCollection();
                    foreach (Base obj in pageObjects)
                    {
                        // fix: it was possible to delete the band if it has a child band
                        if (obj is BandBase && (obj == this || !(obj is ChildBand)))
                            bands.Add(obj);
                    }
                    return bands.Count > 1;
                }
                else
                    return false;
            }
        }

        internal float DesignWidth
        {
            get
            {
                ReportPage page = Page as ReportPage;
                if (page != null && page.ExtraDesignWidth)
                {
                    if (page.Columns.Count <= 1 || !IsColumnDependentBand)
                        return Width * 5;
                }
                return Width;
            }
        }

#endregion

#region Public Methods

        /// <inheritdoc/>
        public override void DrawSelection(FRPaintEventArgs e)
        {
            DrawSelectionPoint(e, Pens.Black, Brushes.White, Left + Width / 2, Top + Height + 2);
            DrawSelectionPoint(e, Pens.Black, Brushes.White, Left + Width / 2, Top + 2);
        }

        /// <inheritdoc/>
        public override bool PointInObject(PointF point)
        {
            if (ReportWorkspace.ClassicView && IsDesigning)
                return new RectangleF(Left, Top - (HeaderSize - 1), DesignWidth, Height + HeaderSize - 1).Contains(point);
            return new RectangleF(Left, Top, DesignWidth, Height).Contains(point);
        }

        /// <inheritdoc/>
        public override void HandleMouseDown(FRMouseEventArgs e)
        {
            base.HandleMouseDown(e);
            if (e.handled)
            {
                if (e.modifierKeys != Keys.Shift)
                {
                    e.mode = WorkspaceMode2.SelectionRect;
                    e.activeObject = this;
                }
            }
        }

        /// <inheritdoc/>
        public override void HandleMouseHover(FRMouseEventArgs e)
        {
            base.HandleMouseHover(e);
            if (e.handled)
                e.cursor = Cursors.Default;
        }

        /// <inheritdoc/>
        public override void HandleMouseMove(FRMouseEventArgs e)
        {
            PointF point = new PointF(e.x, e.y);
            if (e.button == MouseButtons.None)
            {
                e.sizingPoint = SizingPoint.None;
                if (point.Y > Bounds.Bottom && point.Y < Bounds.Bottom + 4)
                {
                    e.mode = WorkspaceMode2.Size;
                    e.sizingPoint = SizingPoint.BottomCenter;
                    e.cursor = Cursors.HSplit;
                    e.handled = true;
                }
                if (point.Y > Bounds.Top && point.Y < Bounds.Top + 4 && point.X > (Bounds.Left + Bounds.Right) / 2 - 2 && point.X < (Bounds.Left + Bounds.Right) / 2 + 2)
                {
                    e.mode = WorkspaceMode2.Size;
                    e.sizingPoint = SizingPoint.TopCenter;
                    e.cursor = Cursors.HSplit;
                    e.handled = true;
                }
            }
            else
            {
                if (e.activeObject == this && e.mode == WorkspaceMode2.Size)
                {
                    if (e.sizingPoint == SizingPoint.BottomCenter)
                    {
                        Height += e.delta.Y;
                        FixHeight();
                    }
                    if (e.sizingPoint == SizingPoint.TopCenter)
                    {
                        Height -= e.delta.Y;
                        FixHeightWithComponentsShift(e.delta.Y);
                    }
                    e.handled = true;
                }
            }
        }

        /// <inheritdoc/>
        public override void HandleMouseUp(FRMouseEventArgs e)
        {
            if (e.mode == WorkspaceMode2.SelectionRect)
            {
                ObjectCollection selectedList = new ObjectCollection();
                // find objects inside the selection rect
                for (int i = 0; i < Report.Designer.Objects.Count; i++)
                {
                    Base c = Report.Designer.Objects[i];
                    if (c is ComponentBase && !(c is BandBase))
                    {
                        e.handled = false;
                        (c as ComponentBase).HandleMouseUp(e);
                        // object is inside
                        if (e.handled)
                            selectedList.Add(c);
                    }
                }
                if (selectedList.Count > 0)
                    selectedList.CopyTo(Report.Designer.SelectedObjects);
            }
            FixHeight();
        }

        /// <inheritdoc/>
        public override SizeF GetPreferredSize()
        {
            SizeF result = new SizeF(0, 0);
            switch (ReportWorkspace.Grid.GridUnits)
            {
                case PageUnits.Millimeters:
                    result = new SizeF(Units.Millimeters * 10, Units.Millimeters * 10);
                    break;
                case PageUnits.Centimeters:
                    result = new SizeF(Units.Centimeters * 1, Units.Centimeters * 1);
                    break;
                case PageUnits.Inches:
                    result = new SizeF(Units.Inches * 0.5f, Units.Inches * 0.5f);
                    break;
                case PageUnits.HundrethsOfInch:
                    result = new SizeF(Units.HundrethsOfInch * 50, Units.HundrethsOfInch * 50);
                    break;
            }
            return result;
        }

        /// <inheritdoc/>
        public override void Delete()
        {
            if (CanDelete)
                Dispose();
        }

        internal virtual string GetInfoText()
        {
            return "";
        }

        internal void DrawBandHeader(Graphics g, RectangleF rect, bool drawTopLine)
        {
            Color color1 = Color.Empty;

            if (this is GroupHeaderBand || this is GroupFooterBand)
                color1 = Color.FromArgb(144, 228, 0);
            else if (this is DataBand)
                color1 = Color.FromArgb(255, 144, 0);
      else
      {  
        // select appropriate gray color
        switch (Report.Designer.UIStyle)
        {
          case UIStyle.VisualStudio2005:
            color1 = SystemColors.Control;
            break;
            
          case UIStyle.Office2003:
            color1 = Color.FromArgb(145, 180, 230);
            break;

          case UIStyle.Office2007Blue:
            color1 = Color.FromArgb(170, 210, 255);
            break;
          
          case UIStyle.Office2007Silver:
            color1 = Color.FromArgb(202, 203, 204);
            break;

          case UIStyle.Office2007Black:
            color1 = Color.FromArgb(143, 150, 160);
            break;
          
          case UIStyle.VistaGlass:
            color1 = Color.FromArgb(190, 200, 227);
            break;    
        }
      }

            Color color2 = Color.FromArgb(100, color1);
            Color color3 = Color.FromArgb(180, Color.White);
            Color color4 = Color.Transparent;

            g.FillRectangle(Brushes.White, rect);

            using (LinearGradientBrush b = new LinearGradientBrush(rect, color1, color2, 90))
            {
                g.FillRectangle(b, rect);
            }

            rect.Height /= 3;
            using (LinearGradientBrush b = new LinearGradientBrush(rect, color3, color4, 90))
            {
                g.FillRectangle(b, rect);
            }

            if (drawTopLine)
            {
                using (Pen p = new Pen(color1))
                {
                    g.DrawLine(p, rect.Left, rect.Top, rect.Right, rect.Top);
                }
            }
        }

        /// <inheritdoc/>
        public override ContextMenuBase GetContextMenu()
        {
            return new BandBaseMenu(Report.Designer);
        }

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            UpdateWidth();
            if (IsDesigning)
            {
                IGraphics g = e.Graphics;

                RectangleF bounds = Bounds;
                bounds.X *= e.ScaleX;
                bounds.Y *= e.ScaleY;
                bounds.Width = DesignWidth * e.ScaleX;
                bounds.Height *= e.ScaleY;

                if (FastReport.Design.PageDesigners.Page.ReportWorkspace.ClassicView && Width != 0)
                {
                    RectangleF fillRect = new RectangleF(bounds.Left, bounds.Top - (BandBase.HeaderSize - 1) * e.ScaleY,
                      bounds.Width, (BandBase.HeaderSize - 1) * e.ScaleY);
                    if (bounds.Top == BandBase.HeaderSize)
                    {
                        fillRect.Y = 0;
                        fillRect.Height += e.ScaleY;
                    }
                    DrawBandHeader(g.Graphics, fillRect, true);

                    ObjectInfo info = RegisteredObjects.FindObject(this);
                    string text = Res.Get(info.Text);
                    if (GetInfoText() != "")
                        text += ": " + GetInfoText();
                    fillRect.X += 4;
                    TextRenderer.DrawText(g.Graphics, text, DpiHelper.ConvertUnits(DrawUtils.Default96Font),
                      new Rectangle((int)fillRect.Left, (int)fillRect.Top, (int)fillRect.Width, (int)fillRect.Height),
                      SystemColors.WindowText, TextFormatFlags.VerticalCenter);
                }

                g.FillRectangle(SystemBrushes.Window, bounds.Left, (int)Math.Round(bounds.Top),
                  bounds.Width, bounds.Height + (FastReport.Design.PageDesigners.Page.ReportWorkspace.ClassicView ? 1 : 4));
                DrawBackground(e);
                if (FastReport.Design.PageDesigners.Page.ReportWorkspace.ShowGrid)
                    FastReport.Design.PageDesigners.Page.ReportWorkspace.Grid.Draw(g.Graphics, bounds, e.ScaleX);

                if (!FastReport.Design.PageDesigners.Page.ReportWorkspace.ClassicView)
                {
                    Pen pen = e.Cache.GetPen(Color.Silver, 1, DashStyle.Dot);
                    g.DrawLine(pen, bounds.Left, bounds.Bottom + 1, bounds.Right + 1, bounds.Bottom + 1);
                    g.DrawLine(pen, bounds.Left + 1, bounds.Bottom + 2, bounds.Right + 1, bounds.Bottom + 2);
                    g.DrawLine(pen, bounds.Left, bounds.Bottom + 3, bounds.Right + 1, bounds.Bottom + 3);
                }
            }
            else
            {
                DrawBackground(e);
                Border.Draw(e, new RectangleF(AbsLeft, AbsTop, Width, Height));
            }
        }

#endregion

    }
}