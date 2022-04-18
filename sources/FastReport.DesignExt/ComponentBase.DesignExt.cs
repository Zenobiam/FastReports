using FastReport.Utils;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport
{
    partial class ComponentBase
    {
#region Public Methods

        /// <summary>
        /// Corrects the object's size and sizing point if the size becomes negative.
        /// </summary>
        /// <param name="e">Current mouse state.</param>
        /// <para>Typically you don't need to use or override this method.</para>
        /// <para>This method is called by the FastReport designer to check if the object's size becomes negative
        /// when resizing the object by the mouse. Method must correct the object's size and/or position to
        /// make it positive, also change the sizing point if needed.</para>
        public virtual void CheckNegativeSize(FRMouseEventArgs e)
        {
            if (Width < 0 && Height < 0)
            {
                e.sizingPoint = SizingPointHelper.SwapDiagonally(e.sizingPoint);
                Bounds = new RectangleF(Right, Bottom, -Width, -Height);
            }
            else if (Width < 0)
            {
                e.sizingPoint = SizingPointHelper.SwapHorizontally(e.sizingPoint);
                Bounds = new RectangleF(Right, Top, -Width, Height);
            }
            else if (Height < 0)
            {
                e.sizingPoint = SizingPointHelper.SwapVertically(e.sizingPoint);
                Bounds = new RectangleF(Left, Bottom, Width, -Height);
            }
            else
                return;
            Cursor.Current = SizingPointHelper.ToCursor(e.sizingPoint);
        }

        /// <summary>
        /// Checks if the object is inside its parent.
        /// </summary>
        /// <param name="immediately">if <b>true</b>, check now independent of any conditions.</param>
        /// <remarks>
        /// <para>Typically you don't need to use or override this method.</para>
        /// <para>When you move an object with the mouse, it may be moved outside its parent. If so, this method
        /// must find a new parent for the object and correct it's <b>Left</b>, <b>Top</b> and <b>Parent</b>
        /// properties. If <b>immediately</b> parameter is <b>false</b>, you can optimize the method
        /// to search for new parent only if the object's bounds are outside parent. If this parameter is
        /// <b>true</b>, you must skip any optimizations and search for a parent immediately.</para>
        /// </remarks>
        public virtual void CheckParent(bool immediately)
        {
        }

        /// <summary>
        /// Draws the object.
        /// </summary>
        /// <param name="e">Paint event args.</param>
        /// <remarks>
        /// <para>This method is widely used in the FastReport. It is called each time when the object needs to draw
        /// or print itself.</para>
        /// <para>In order to draw the object correctly, you should multiply the object's bounds by the <b>scale</b>
        /// parameter.</para>
        /// <para><b>cache</b> parameter is used to optimize the drawing speed. It holds all items such as
        /// pens, fonts, brushes, string formats that was used before. If the item with requested parameters
        /// exists in the cache, it will be returned (instead of create new item and then dispose it).</para>
        /// </remarks>
        public virtual void Draw(FRPaintEventArgs e)
        {
            if (IsDesigning)
            {
                if (IsAncestor)
                    e.Graphics.DrawImage(Res.GetImage(99), (int)(AbsRight * e.ScaleX - 9), (int)(AbsTop * e.ScaleY + 2));
            }
        }

        /// <summary>
        /// Draw the frame around the object to indicate that it accepts the drag&amp;drop operation.
        /// </summary>
        /// <param name="e">Paint event args.</param>
        /// <param name="color">The color of frame.</param>
        public virtual void DrawDragAcceptFrame(FRPaintEventArgs e, Color color)
        {
            RectangleF rect = new RectangleF(AbsLeft * e.ScaleX, AbsTop * e.ScaleY, Width * e.ScaleX, Height * e.ScaleY);
            Pen p = e.Cache.GetPen(color, 1, DashStyle.Dot);
            for (int i = 0; i < 3; i++)
            {
                e.Graphics.DrawRectangle(p, rect.Left, rect.Top, rect.Width, rect.Height);
                rect.Inflate(-1, -1);
            }
        }

        /// <summary>
        /// Draw the selection points.
        /// </summary>
        /// <param name="e">Paint event args.</param>
        /// <remarks>
        /// This method draws a set of selection points returned by the <see cref="GetSelectionPoints"/> method.
        /// </remarks>
        public virtual void DrawSelection(FRPaintEventArgs e)
        {
            if (Page == null)
                return;
            bool firstSelected = Report.Designer.SelectedObjects.IndexOf(this) == 0;
            Pen p = firstSelected ? Pens.Black : Pens.White;
            Brush b = firstSelected ? Brushes.White : Brushes.Black;
            SelectionPoint[] selectionPoints = GetSelectionPoints();
            foreach (SelectionPoint pt in selectionPoints)
            {
                DrawSelectionPoint(e, p, b, pt.x, pt.y);
            }
        }

        /// <inheritdoc/>
        public override ContextMenuBase GetContextMenu()
        {
            return new ComponentBaseMenu(Report.Designer);
        }

        /// <summary>
        /// Gets the preferred size of an object.
        /// </summary>
        /// <returns>Preferred size.</returns>
        /// <remarks>
        /// This method is called by the FastReport designer when you insert a new object.
        /// </remarks>
        public virtual SizeF GetPreferredSize()
        {
            return new SizeF(Units.Millimeters * 25, Units.Millimeters * 5);
        }

        /// <summary>
        /// Returns a "smart tag" menu.
        /// </summary>
        /// <remarks>
        /// "Smart tag" is a little button that appears near the object's top-right corner when we are in the
        /// designer and move the mouse over the object. When you click that button you will see a popup window
        /// where you can set up some properties of the object. FastReport uses smart tags to quickly choose
        /// the datasource (for a band) or data column (for objects).
        /// </remarks>
        public virtual SmartTagBase GetSmartTag()
        {
            return null;
        }

        /// <summary>
        /// Handles double click event in the designer.
        /// </summary>
        /// <remarks>
        /// This method is called when the user doubleclicks the object in the designer. Typical implementation
        /// invokes the object's editor (calls the <b>InvokeEditor</b> method) and sets the designer's
        /// <b>Modified</b> flag.
        /// </remarks>
        public virtual void HandleDoubleClick()
        {
            if (HasFlag(Flags.CanEdit) && !HasRestriction(Restrictions.DontEdit) &&
              this is IHasEditor && (this as IHasEditor).InvokeEditor())
                Report.Designer.SetModified(this, "Change");
        }

        /// <summary>
        /// Handles the DragDrop event in the designer.
        /// </summary>
        /// <param name="e">Current mouse state.</param>
        /// <remarks>
        /// This method is called when the user drops an item from the Data Tree window into this object.
        /// This method should copy the information from the <b>e.DraggedObject</b> object and set the
        /// <b>e.Handled</b> flag to <b>true</b> to complete the drag operation.
        /// </remarks>
        public virtual void HandleDragDrop(FRMouseEventArgs e)
        {
        }

        /// <summary>
        /// Handles the DragOver event in the designer.
        /// </summary>
        /// <param name="e">Current mouse state.</param>
        /// <remarks>
        /// This method is called when the user drags an item from the Data Tree window. This method should
        /// check that the mouse (<b>e.X, e.Y</b>) is inside the object, then set the <b>e.Handled</b> flag
        /// to <b>true</b> if an item can be dragged into this object.
        /// </remarks>
        public virtual void HandleDragOver(FRMouseEventArgs e)
        {
        }

        /// <summary>
        /// Handles KeyDown event in the designer.
        /// </summary>
        /// <param name="sender">The designer's workspace.</param>
        /// <param name="e">Keyboard event parameters.</param>
        /// <remarks>
        /// This method is called when the user presses any key in the designer. Typical implementation
        /// does nothing.
        /// </remarks>
        public virtual void HandleKeyDown(Control sender, KeyEventArgs e)
        {
        }

        /// <summary>
        /// Handles MouseDown event that occurs when the user clicks the mouse in the designer.
        /// </summary>
        /// <remarks>
        ///   <para>This method is called when the user press the mouse button in the designer.
        ///     The standard implementation does the following:</para>
        ///   <list type="bullet">
        ///     <item>checks if the mouse pointer is inside the object;</item>
        ///     <item>add an object to the selected objects list of the designer;</item>
        ///     <item>sets the <b>e.Handled</b> flag to <b>true</b>.</item>
        ///   </list>
        /// </remarks>
        /// <param name="e">Current mouse state.</param>
        public virtual void HandleMouseDown(FRMouseEventArgs e)
        {
            if (PointInObject(new PointF(e.x, e.y)))
            {
                SelectedObjectCollection selection = Report.Designer.SelectedObjects;
                if (e.modifierKeys == Keys.Shift)
                {
                    // toggle selection
                    if (selection.IndexOf(this) != -1)
                    {
                        if (selection.Count > 1)
                            selection.Remove(this);
                    }
                    else
                        selection.Add(this);
                }
                else
                {
                    // select the object if not selected yet
                    if (selection.IndexOf(this) == -1)
                    {
                        selection.Clear();
                        selection.Add(this);
                    }
                }
                e.handled = true;
                e.mode = WorkspaceMode2.Move;
            }
        }

        /// <summary>
        /// Handles MouseMove event that occurs when the user moves the mouse in the designer.
        /// </summary>
        /// <remarks>
        ///   <para>This method is called when the user moves the mouse in the designer. Typical
        ///     use of this method is to change the mouse cursor to <b>SizeAll</b> when it is over
        ///     an object. The standard implementation does the following:</para>
        ///   <list type="bullet">
        ///     <item>checks if the mouse pointer is inside the object;</item>
        ///     <item>changes the cursor shape (<b>e.Cursor</b> property);</item>
        ///     <item>sets the <b>e.Handled</b> flag to <b>true</b>.</item>
        ///   </list>
        /// </remarks>
        /// <param name="e">Current mouse state.</param>
        public virtual void HandleMouseHover(FRMouseEventArgs e)
        {
            if (PointInObject(new PointF(e.x, e.y)))
            {
                e.handled = true;
                e.cursor = Cursors.SizeAll;
            }
        }

        /// <summary>
        /// Handles MouseMove event that occurs when the user moves the mouse in the designer.
        /// </summary>
        /// <remarks>
        ///   <para>This method is called when the user moves the mouse in the designer. The
        ///     standard implementation does the following:</para>
        ///   <list type="bullet">
        ///     <item>
        ///             if mouse button is not pressed, check that mouse pointer is inside one of
        ///             the selection points returned by the <see cref="GetSelectionPoints"/>
        ///             method and set the <b>e.SizingPoint</b> member to the corresponding sizing
        ///             point;
        ///         </item>
        ///     <item>if mouse button is pressed, and <b>e.SizingPoint</b> member is not
        ///         <b>SizingPoint.None</b>, resize the object.</item>
        ///   </list>
        /// </remarks>
        /// <param name="e">Current mouse state.</param>
        public virtual void HandleMouseMove(FRMouseEventArgs e)
        {
            if (!IsSelected)
                return;

            if (e.button == MouseButtons.None)
            {
                PointF point = new PointF(e.x, e.y);
                e.sizingPoint = SizingPoint.None;
                SelectionPoint[] selectionPoints = GetSelectionPoints();
                foreach (SelectionPoint pt in selectionPoints)
                {
                    if (PointInSelectionPoint(pt.x, pt.y, point))
                    {
                        e.sizingPoint = pt.sizingPoint;
                        break;
                    }
                }
                if (e.sizingPoint != SizingPoint.None)
                {
                    e.handled = true;
                    e.mode = WorkspaceMode2.Size;
                    e.cursor = SizingPointHelper.ToCursor(e.sizingPoint);
                }
            }
            else if (!IsParentSelected)
            {
                if (e.mode == WorkspaceMode2.Move)
                {
                    Left += e.delta.X;
                    Top += e.delta.Y;
                }
                else if (e.mode == WorkspaceMode2.Size)
                {
                    if ((e.modifierKeys & Keys.Shift) > 0)
                    {
                        bool wider = Math.Abs(e.delta.X) > Math.Abs(e.delta.Y);
                        float width = Width;
                        float height = Height;

                        switch (e.sizingPoint)
                        {
                            case SizingPoint.LeftTop:
                                if (wider)
                                {
                                    Left += e.delta.Y;
                                    Width -= e.delta.Y;
                                    Top += Height - (Height * Width / width);
                                    Height = Height * Width / width;
                                }
                                else
                                {
                                    Top += e.delta.X;
                                    Height -= e.delta.X;
                                    Left += Width - (Width * Height / height);
                                    Width = Width * Height / height;
                                }
                                break;

                            case SizingPoint.LeftBottom:
                                if (wider)
                                {
                                    Left -= e.delta.Y;
                                    Width += e.delta.Y;
                                    Height = Height * Width / width;
                                }
                                else
                                {
                                    Height -= e.delta.X;
                                    Left += Width - (Width * Height / height);
                                    Width = Width * Height / height;
                                }
                                break;

                            case SizingPoint.RightTop:
                                if (wider)
                                {
                                    Width -= e.delta.Y;
                                    Top += Height - (Height * Width / width);
                                    Height = Height * Width / width;
                                }
                                else
                                {
                                    Height += e.delta.X;
                                    Top -= e.delta.X;
                                    Width = Width * Height / height;
                                }
                                break;

                            case SizingPoint.RightBottom:
                                if (wider)
                                {
                                    Width += e.delta.Y;
                                    Height = Height * Width / width;
                                }
                                else
                                {
                                    Height += e.delta.X;
                                    Width = Width * Height / height;
                                }
                                break;

                            case SizingPoint.TopCenter:
                                Top += e.delta.Y;
                                Height -= e.delta.Y;
                                Width = Width * Height / height;
                                break;

                            case SizingPoint.BottomCenter:
                                Height += e.delta.Y;
                                Width = Width * Height / height;
                                break;

                            case SizingPoint.LeftCenter:
                                Left += e.delta.X;
                                Width -= e.delta.X;
                                Height = Height * Width / width;
                                break;

                            case SizingPoint.RightCenter:
                                Width += e.delta.X;
                                Height = Height * Width / width;
                                break;
                        }
                    }
                    else
                    {
                        switch (e.sizingPoint)
                        {
                            case SizingPoint.LeftTop:
                                Left += e.delta.X;
                                Width -= e.delta.X;
                                Top += e.delta.Y;
                                Height -= e.delta.Y;
                                break;

                            case SizingPoint.LeftBottom:
                                Left += e.delta.X;
                                Width -= e.delta.X;
                                Height += e.delta.Y;
                                break;

                            case SizingPoint.RightTop:
                                Width += e.delta.X;
                                Top += e.delta.Y;
                                Height -= e.delta.Y;
                                break;

                            case SizingPoint.RightBottom:
                                Width += e.delta.X;
                                Height += e.delta.Y;
                                break;

                            case SizingPoint.TopCenter:
                                Top += e.delta.Y;
                                Height -= e.delta.Y;
                                break;

                            case SizingPoint.BottomCenter:
                                Height += e.delta.Y;
                                break;

                            case SizingPoint.LeftCenter:
                                Left += e.delta.X;
                                Width -= e.delta.X;
                                break;

                            case SizingPoint.RightCenter:
                                Width += e.delta.X;
                                break;
                        }
                    }
                    CheckNegativeSize(e);
                }
                CheckParent(false);
            }
        }

        /// <summary>
        /// Handles MouseUp event that occurs when the user releases the mouse button in the designer.
        /// </summary>
        /// <remarks>
        ///   <para>This method is called when the user releases the mouse button in the
        ///     designer. The standard implementation does the following:</para>
        ///   <list type="bullet">
        ///     <item>if <b>e.Mode</b> is <b>WorkspaceMode2.SelectionRect</b>, checks if object
        ///         is inside the selection rectangle and sets <b>e.Handled</b> flag if so;</item>
        ///     <item>
        ///             checks that object is inside its parent (calls the
        ///             <see cref="CheckParent"/> method).
        ///         </item>
        ///   </list>
        /// </remarks>
        /// <param name="e">Current mouse state.</param>
        public virtual void HandleMouseUp(FRMouseEventArgs e)
        {
            if (e.mode == WorkspaceMode2.SelectionRect)
            {
                if (e.selectionRect.IntersectsWith(new RectangleF(AbsLeft, AbsTop, Width, Height)))
                    e.handled = true;
            }
            else if (e.mode == WorkspaceMode2.Move || e.mode == WorkspaceMode2.Size)
            {
                if (IsSelected)
                    CheckParent(true);
            }
        }

        /// <summary>
        /// Handles mouse wheel event.
        /// </summary>
        /// <param name="e">Current mouse state.</param>
        public virtual void HandleMouseWheel(FRMouseEventArgs e)
        {
        }

        /// <summary>
        /// Checks if given point is inside the object's bounds.
        /// </summary>
        /// <param name="point">point to check.</param>
        /// <returns><b>true</b> if <b>point</b> is inside the object's bounds.</returns>
        /// <remarks>
        /// You can override this method if your objectis not of rectangular form.
        /// </remarks>
        public virtual bool PointInObject(PointF point)
        {
            return AbsBounds.Contains(point);
        }

#endregion Public Methods

#region Protected Methods

        /// <summary>
        /// Draws the selection point.
        /// </summary>
        /// <param name="e">Paint event args.</param>
        /// <param name="p"><see cref="Pen"/> object.</param>
        /// <param name="b"><see cref="Brush"/> object.</param>
        /// <param name="x">Left coordinate.</param>
        /// <param name="y">Top coordinate.</param>
        protected virtual void DrawSelectionPoint(FRPaintEventArgs e, Pen p, Brush b, float x, float y)
        {
            IGraphics g = e.Graphics;
            x = (float)Math.Round(x * e.ScaleX);
            y = (float)Math.Round(y * e.ScaleY);
            g.FillRectangle(b, x - DpiHelper.ConvertUnits(2), y - DpiHelper.ConvertUnits(2), DpiHelper.ConvertUnits(4), DpiHelper.ConvertUnits(4));
            g.DrawRectangle(p, x - DpiHelper.ConvertUnits(2), y - DpiHelper.ConvertUnits(2), DpiHelper.ConvertUnits(4), DpiHelper.ConvertUnits(4));
        }

        /// <summary>
        /// Gets the object's selection points.
        /// </summary>
        /// <returns>Array of <see cref="SelectionPoint"/> objects.</returns>
        /// <remarks>
        /// <para>Selection point is a small square displayed at the object's sides when object is selected
        /// in the designer. You can drag this square by the mouse to change the object's size. For example,
        /// the <b>TextObject</b> has eight selection points to change its width and height by the mouse.</para>
        /// <para>If you are developing a new component for FastReport, you may override this method
        /// if your object has non-standard set of selection points. For example, if an object has something like
        /// "AutoSize" property, it would be good to disable all selection points if that property is <b>true</b>,
        /// to disable resizing of the object by the mouse.</para>
        /// </remarks>
        protected virtual SelectionPoint[] GetSelectionPoints()
        {
            return new SelectionPoint[] {
                new SelectionPoint(AbsLeft, AbsTop, SizingPoint.LeftTop),
                new SelectionPoint(AbsLeft + Width, AbsTop, SizingPoint.RightTop),
                new SelectionPoint(AbsLeft, AbsTop + Height, SizingPoint.LeftBottom),
                new SelectionPoint(AbsLeft + Width, AbsTop + Height, SizingPoint.RightBottom),
                new SelectionPoint(AbsLeft + Width / 2, AbsTop, SizingPoint.TopCenter),
                new SelectionPoint(AbsLeft + Width / 2, AbsTop + Height, SizingPoint.BottomCenter),
                new SelectionPoint(AbsLeft, AbsTop + Height / 2, SizingPoint.LeftCenter),
                new SelectionPoint(AbsLeft + Width, AbsTop + Height / 2, SizingPoint.RightCenter) };
        }

        /// <summary>
        /// Gets a value indicating that given point is inside selection point.
        /// </summary>
        /// <param name="x">point's x coordinate.</param>
        /// <param name="y">point's y coordinate.</param>
        /// <param name="point">selection point.</param>
        /// <returns><b>true</b> if <b>(x,y)</b> is inside the <b>point</b></returns>
        protected bool PointInSelectionPoint(float x, float y, PointF point)
        {
            return x >= point.X - 3 && x <= point.X + 3 && y >= point.Y - 3 && y <= point.Y + 3;
        }

#endregion Protected Methods
    }
}