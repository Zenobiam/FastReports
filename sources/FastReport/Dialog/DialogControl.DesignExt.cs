using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using FastReport.Utils;
using System.ComponentModel;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Dialog
{
    partial class DialogControl : IHasEditor
    {
        #region Fields

        private Control drawControl;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a drawing <b>Control</b>.
        /// </summary>
        [Browsable(false)]
        internal Control DrawControl
        {
            get { return drawControl; }
            set { drawControl = value; }
        }

        /// <summary>
        /// The scale value
        /// </summary>
        protected float DpiScale
        {
            get
            {
                return DpiHelper.Multiplier;
            }
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// Determines whether is necessary to serialize the <b>BackColor</b> property.
        /// </summary>
        /// <returns><b>true</b> if serialization is necessary.</returns>
        protected virtual bool ShouldSerializeBackColor()
        {
            return BackColor != SystemColors.Control;
        }

        /// <summary>
        /// Determines whether is necessary to serialize the <b>Cursor</b> property.
        /// </summary>
        /// <returns><b>true</b> if serialization is necessary.</returns>
        protected virtual bool ShouldSerializeCursor()
        {
            return Cursor != Cursors.Default;
        }

        ///// <summary>
        ///// Determines whether is necessary to serialize the <b>Font</b> property.
        ///// </summary>
        ///// <returns><b>true</b> if serialization is necessary.</returns>
        //protected virtual bool ShouldSerializeFont()
        //{
        //    return Font.Name != "Tahoma" || Font.Size != 8 || Font.Style != FontStyle.Regular;
        //}

        /// <summary>
        /// Determines whether is necessary to serialize the <b>ForeColor</b> property.
        /// </summary>
        /// <returns><b>true</b> if serialization is necessary.</returns>
        protected virtual bool ShouldSerializeForeColor()
        {
            return ForeColor != SystemColors.ControlText;
        }

        /// <inheritdoc/>
        protected override SelectionPoint[] GetSelectionPoints()
        {
            Size PrefSize = Control.PreferredSize;
            RectangleF bounds = Control.Bounds;
            return new SelectionPoint[] {
        new SelectionPoint(AbsLeft - 2, AbsTop - 2, SizingPoint.LeftTop),
        new SelectionPoint(AbsLeft + Width + 1, AbsTop - 2, SizingPoint.RightTop),
        new SelectionPoint(AbsLeft - 2, AbsTop + Height + 1, SizingPoint.LeftBottom),
        new SelectionPoint(AbsLeft + Width + 1, AbsTop + Height + 1, SizingPoint.RightBottom),
        new SelectionPoint(AbsLeft + Width / 2, AbsTop - 2, SizingPoint.TopCenter),
        new SelectionPoint(AbsLeft + Width / 2, AbsTop + Height + 1, SizingPoint.BottomCenter),
        new SelectionPoint(AbsLeft - 2, AbsTop + Height / 2, SizingPoint.LeftCenter),
        new SelectionPoint(AbsLeft + Width + 1, AbsTop + Height / 2, SizingPoint.RightCenter) };
        }

        /// <summary>
        /// Draws the selection point.
        /// </summary>
        /// <param name="g"><b>Graphics</b> object to draw on.</param>
        /// <param name="p"><see cref="Pen"/> object.</param>
        /// <param name="b"><see cref="Brush"/> object.</param>
        /// <param name="x">Left coordinate.</param>
        /// <param name="y">Top coordinate.</param>
        protected void DrawSelectionPoint(Graphics g, Pen p, Brush b, float x, float y)
        {
            x = (int)x;
            y = (int)y;
            g.FillRectangle(b, x - 2, y - 2, 5, 5);
            g.DrawLine(p, x - 2, y - 3, x + 2, y - 3);
            g.DrawLine(p, x - 2, y + 3, x + 2, y + 3);
            g.DrawLine(p, x - 3, y - 2, x - 3, y + 2);
            g.DrawLine(p, x + 3, y - 2, x + 3, y + 2);
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void CheckParent(bool immediately)
        {
            if (!IsSelected || IsAncestor || !HasFlag(Flags.CanChangeParent))
                return;

            if (immediately || (!(Parent is DialogPage) && (
              Left < 0 || Left > (Parent as ComponentBase).Width || Top < 0 || Top > (Parent as ComponentBase).Height)))
            {
                ObjectCollection list = Page.AllObjects;
                int i = 0;
                while (i < list.Count)
                {
                    if (list[i] is ParentControl && list[i] != this)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                list.Insert(0, Page);

                for (i = list.Count - 1; i >= 0; i--)
                {
                    ComponentBase c = list[i] as ComponentBase;
                    if ((c as IParent).CanContain(this))
                        if (AbsLeft > c.AbsLeft - 1e-4 && AbsLeft < c.AbsRight - 1e-4 &&
                          AbsTop > c.AbsTop - 1e-4 && AbsTop < c.AbsBottom - 1e-4)
                        {
                            if (Parent != c)
                            {
                                Left = (int)Math.Round((AbsLeft - c.AbsLeft) / Page.SnapSize.Width) * Page.SnapSize.Width;
                                Top = (int)Math.Round((AbsTop - c.AbsTop) / Page.SnapSize.Height) * Page.SnapSize.Height;
                                Parent = c;
                            }
                            break;
                        }
                }
            }
        }

        /// <inheritdoc/>
        public override void DrawSelection(FRPaintEventArgs e)
        {
            IGraphics g = e.Graphics;
            bool firstSelected = Report.Designer.SelectedObjects.IndexOf(this) == 0;
            Pen p = firstSelected ? Pens.Black : Pens.White;
            Brush b = firstSelected ? Brushes.White : Brushes.Black;
            SelectionPoint[] selectionPoints = GetSelectionPoints();

            Pen pen = e.Cache.GetPen(Color.Gray, 1, DashStyle.Dot);
            g.DrawRectangle(pen, AbsLeft * e.ScaleX - 2, AbsTop * e.ScaleY - 2, Width * e.ScaleX + 3, Height * e.ScaleY + 3);
            if (selectionPoints.Length == 1)
                DrawSelectionPoint(e, p, b, selectionPoints[0].x, selectionPoints[0].y);
            else
            {
                foreach (SelectionPoint pt in selectionPoints)
                {
                    DrawSelectionPoint(e, p, b, pt.x, pt.y);
                }
            }
        }

        /// <summary>
        /// Creates the empty event handler for the <b>ClickEvent</b> event in the report's script.
        /// </summary>
        /// <returns><b>true</b> if event handler was created successfully.</returns>
        public bool InvokeEditor()
        {
            Report.Designer.ActiveReportTab.SwitchToCode();
            if (String.IsNullOrEmpty(ClickEvent))
            {
                string newEventName = Name + "_Click";
                if (Report.CodeHelper.AddHandler(typeof(EventHandler), newEventName))
                {
                    ClickEvent = newEventName;
                    return true;
                }
                else
                    return false;
            }
            else
            {
                Report.CodeHelper.LocateHandler(ClickEvent);
            }
            return false;
        }

        /// <inheritdoc/>
        public override void OnBeforeInsert(int flags)
        {
            base.OnBeforeInsert(flags);
            try
            {
                Text = BaseName;
            }
            catch
            {
            }
        }
        #endregion

        ///// <summary>
        ///// Gets the absolute bounding rectangle of the object.
        ///// </summary>
        //[Browsable(false)]
        //public new RectangleF AbsBounds
        //{
        //    get { return new RectangleF(AbsLeft, AbsTop, Width, Height); }
        //}
    }

    static class DialogControlDesignExt
    {
        public static void DrawDesign(DialogControl obj, FRPaintEventArgs e)
        {
            if (obj.Control.Width > 0 && obj.Control.Height > 0)
            {
                using (Bitmap bmp = DrawUtils.DrawToBitmap(obj.Control, true))
                {
                    e.Graphics.DrawImage(bmp, (int)obj.AbsLeft, (int)obj.AbsTop);
                }
            }
        }
    }
}