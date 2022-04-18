using FastReport.Utils;
using System;
using System.Collections.Generic;
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
    partial class PolyLineObject : IHasEditor
    {
        //Current selected point, for move
        //internal PolyLineObjectToolBar toolbar; //deprecated

        #region Private Fields

        private PolyPoint currentPoint;
        private PolyPoint drawSelectedPointLine;
        private int drawSelectedPointLineNumber;
        private int selectedPoint;

        #endregion Private Fields

        #region Public Properties

        /// <inheritdoc/>
        public override float Height
        {
            get { return base.Height; }
            set
            {
                if (base.Height == 0)
                {
                    base.Height = value;
                    return;
                }
                if (IsSelected)
                {
                    if (SelectionMode == PolygonSelectionMode.MoveAndScale)
                    {
                        float oldHeight = base.Height;
                        if (oldHeight == value) return;
                        if (pointsCollection.Count < 2)
                            return;
                        float scaleY = value / oldHeight;
                        if (float.IsInfinity(scaleY))
                            return;
                        if (float.IsNaN(scaleY))
                            return;
                        if (scaleY == 0)
                            return;
                        center.Y = center.Y * scaleY;
                        foreach (PolyPoint point in pointsCollection)
                        {
                            point.ScaleY(scaleY);
                        }
                        base.Height = value;
                    }
                    else
                        RecalculateBounds();
                }
                else
                    base.Height = value;
            }
        }

        /// <inheritdoc/>
        public override float Width
        {
            get { return base.Width; }
            set
            {
                if (base.Width == 0)
                {
                    base.Width = value;
                    return;
                }
                if (IsSelected)
                {
                    if (SelectionMode == PolygonSelectionMode.MoveAndScale)
                    {
                        float oldWidth = base.Width;
                        if (oldWidth == value) return;
                        if (pointsCollection.Count < 2)
                            return;
                        float scaleX = value / oldWidth;
                        if (float.IsInfinity(scaleX))
                            return;
                        if (float.IsNaN(scaleX))
                            return;
                        if (scaleX == 0)
                            return;
                        center.X = center.X * scaleX;
                        foreach (PolyPoint point in pointsCollection)
                        {
                            point.ScaleX(scaleX);
                        }
                        base.Width = value;
                    }
                    else
                        RecalculateBounds();
                }
                else
                    base.Width = value;
            }
        }

        #endregion Public Properties

        #region Internal Properties

        internal PolygonSelectionMode SelectionMode
        {
            get
            {
                return polygonSelectionMode;
            }
            set
            {
                if (value != polygonSelectionMode)
                {
                    polygonSelectionMode = value;
                    try
                    {
                        if (IsSelected)
                        {
                            Report.Designer.SelectionChanged(this);
                        }
                    }
                    catch
                    {
                    }

                    polygonSelectionMode = value;
                }
            }
        }

        #endregion Internal Properties

        #region Public Methods

        /// <summary>
        /// Add point to end of polyline and recalculate bounds after add.
        /// Can be first point.
        /// Deprecated, use insert point
        /// </summary>
        /// <param name="localX">local x - relative to left-top point</param>
        /// <param name="localY">local y - relative to left-top point</param>
        [Obsolete]
        public void AddPointToEnd(float localX, float localY)
        {
            insertPoint(pointsCollection.Count, localX, localY, 1);
            if (Report != null)
                if (Report.Designer != null)
                    Report.Designer.SetModified(this, "Change");
        }

        /// <summary>
        /// Add point to start of polyline and recalculate bounds after add
        /// Can be first point
        /// Deprecated, use insert point
        /// </summary>
        /// <param name="localX">local x - relative to left-top point</param>
        /// <param name="localY">local y - relative to left-top point</param>
        [Obsolete]
        public void AddPointToStart(float localX, float localY)
        {
            insertPoint(0, localX, localY, 1);
            if (Report != null)
                if (Report.Designer != null)
                    Report.Designer.SetModified(this, "Change");
        }

        /// <inheritdoc/>
        public override void CheckNegativeSize(FRMouseEventArgs e)
        {
            // do nothing
        }

        /// <inheritdoc/>
        public override void DrawSelection(FRPaintEventArgs e)
        {
            if (SelectionMode == PolygonSelectionMode.AddToLine && drawSelectedPointLineNumber >= 0 && pointsCollection.Count > 0)
            {
                if (drawSelectedPointLine != null)
                {
                    float absLeft = AbsLeft;
                    float absTop = AbsTop;
                    Pen p = e.Cache.GetPen(Color.DarkGray, 1, DashStyle.Solid);
                    if (drawSelectedPointLineNumber > 0 && drawSelectedPointLineNumber - 1 < pointsCollection.Count || this is PolygonObject)
                    {
                        e.Graphics.DrawLine(p, GetPointF(drawSelectedPointLine, e.ScaleX, e.ScaleY, absLeft , absTop), GetPointF(drawSelectedPointLineNumber - 1, e.ScaleX, e.ScaleY, absLeft, absTop));
                    }

                    if (drawSelectedPointLineNumber < pointsCollection.Count || this is PolygonObject)
                    {
                        e.Graphics.DrawLine(p, GetPointF(drawSelectedPointLine, e.ScaleX, e.ScaleY, absLeft, absTop), GetPointF(drawSelectedPointLineNumber, e.ScaleX, e.ScaleY, absLeft, absTop));
                    }
                }
            }

            if (SelectionMode == PolygonSelectionMode.MoveAndScale)
            {
                base.DrawSelection(e);
            }
            else
            {
                Pen help_p = e.Cache.GetPen(Color.Black, 1, DashStyle.Dash);
                if (Page == null)
                    return;
                bool firstSelected = Report.Designer.SelectedObjects.IndexOf(this) == 0;
                Pen p = firstSelected ? Pens.Black : Pens.White;
                Brush b = firstSelected ? Brushes.White : Brushes.Black;
                SelectionPoint[] selectionPoints = GetSelectionPoints();
                IGraphics g = e.Graphics;
                SelectionPoint lastPoint = null;
                foreach (SelectionPoint pt in selectionPoints)
                {
                    switch (pt.sizingPoint)
                    {
                        case SizingPoint.LeftTop:

                            {
                                float x = pt.x * e.ScaleX;
                                float y = pt.y * e.ScaleY;
                                g.FillEllipse(Brushes.White, x - 5, y - 5, 10, 10);
                                g.FillEllipse(Brushes.Red, x - 3, y - 3, 6, 6);
                                g.DrawEllipse(Pens.Gray, x - 3, y - 3, 6, 6);
                                lastPoint = pt;
                            }
                            break;

                        case SizingPoint.RightTop:
                            {
                                float x = pt.x * e.ScaleX;
                                float y = pt.y * e.ScaleY;
                                if (lastPoint != null)
                                {
                                    e.Graphics.DrawLine(help_p, lastPoint.x * e.ScaleX, lastPoint.y * e.ScaleY, x, y);
                                }
                                g.FillEllipse(Brushes.White, x - 3, y - 3, 6, 6);
                                g.FillEllipse(Brushes.Blue, x - 2, y - 2, 4, 4);
                                g.DrawEllipse(Pens.Gray, x - 2, y - 2, 4, 4);
                            }
                            break;

                        default:
                            lastPoint = pt;
                            DrawSelectionPoint(e, p, b, pt.x, pt.y);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Replace points with star
        /// </summary>
        /// <param name="spikes">Minimum value is 3</param>
        public void DrawStar(int spikes)
        {
            if (spikes < 3) spikes = 3;
            const float outerRadius = 50;
            const float innerRadius = 30;

            float rot = (float)(Math.PI / 2 * 3);
            const float cx = 0;
            const float cy = 0;
            float x = cx;
            float y = cy;
            float step = (float)(Math.PI / spikes);
            base.Width = 100;
            base.Height = 100;
            center = new PointF(50, 50);

            pointsCollection.Clear();

            pointsCollection.Add(new PolyPoint(cx, cy - outerRadius));
            for (int i = 0; i < spikes; i++)
            {
                x = cx + (float)Math.Cos(rot) * outerRadius;
                y = cy + (float)Math.Sin(rot) * outerRadius;
                pointsCollection.Add(new PolyPoint(x, y));
                rot += step;

                x = cx + (float)Math.Cos(rot) * innerRadius;
                y = cy + (float)Math.Sin(rot) * innerRadius;
                pointsCollection.Add(new PolyPoint(x, y));
                rot += step;
            }
        }

        /// <inheritdoc/>
        public override SizeF GetPreferredSize()
        {
            return new SizeF(0, 0);
        }

        /// <inheritdoc/>
        public override void HandleKeyDown(Control sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (SelectionMode != PolygonSelectionMode.MoveAndScale)
                {
                    if (pointsCollection.Count > 0)
                    {
                        DeletePointByIndex(selectedPoint);
                        selectedPoint--;
                        Report.Designer.Refresh();
                        e.Handled = true;
                    }
                }
            }
            if (!e.Handled)
                base.HandleKeyDown(sender, e);
        }

        /// <inheritdoc/>
        public override void HandleMouseDown(FRMouseEventArgs e)
        {
            //if (toolbar.IsVisible && this.IsSelected)
            //{
            //    if (toolbar.ClickPublic(new PointF(AbsLeft, AbsTop < 20 ? 20 : AbsTop), new PointF(e.x, e.y)))
            //    {
            //        e.handled = true;
            //        return;
            //    }
            //}

            if (e.button == MouseButtons.Left && currentPoint == null && this.IsSelected)
            {
                float absLeft = AbsLeft;
                float absTop = AbsTop;
                PointF mousePoint = new PointF(e.x, e.y);
                PointF mousePointAligned = new PointF((int)((e.x - absLeft - center.X) / Page.SnapSize.Width) * Page.SnapSize.Width, (int)((e.y - absTop - center.Y) / Page.SnapSize.Height) * Page.SnapSize.Height);
                switch (SelectionMode)
                {
                    case PolygonSelectionMode.Normal:
                        for (int i = 0; i < pointsCollection.Count; i++)
                        {
                            PolyPoint point = pointsCollection[i];
                            if (point.LeftCurve != null && PointInSelectionPoint(absLeft + point.X + center.X + point.LeftCurve.X, absTop + point.Y + center.Y + point.LeftCurve.Y, mousePoint))
                            {
                                currentPoint = point.LeftCurve;
                                e.mode = WorkspaceMode2.Custom;
                                e.handled = true;
                                break;
                            }
                            else if (point.RightCurve != null && PointInSelectionPoint(absLeft + point.X + center.X + point.RightCurve.X, absTop + point.Y + center.Y + point.RightCurve.Y, mousePoint))
                            {
                                currentPoint = point.RightCurve;
                                e.mode = WorkspaceMode2.Custom;
                                e.handled = true;
                                break;
                            }
                            else if (PointInSelectionPoint(absLeft + point.X + center.X, absTop + point.Y + center.Y, mousePoint))
                            {
                                currentPoint = point;
                                e.mode = WorkspaceMode2.Custom;
                                e.handled = true;
                                break;
                            }
                        }
                        break;

                    case PolygonSelectionMode.AddToLine:
                        for (int i = 0; i < pointsCollection.Count; i++)
                        {
                            PolyPoint point = pointsCollection[i];
                            if (PointInSelectionPoint(absLeft + point.X + center.X, absTop + point.Y + center.Y, mousePoint))
                            {
                                currentPoint = point;
                                e.mode = WorkspaceMode2.Custom;
                                e.handled = true;
                                break;
                            }
                        }
                        if (!e.handled)
                        {
                            e.handled = true;
                            PolyPoint result = InsertPointByLocation(mousePointAligned.X, mousePointAligned.Y, 25);
                            if (result != null)
                            {
                                e.mode = WorkspaceMode2.Custom;
                                currentPoint = result;
                            }
                        }
                        break;

                    case PolygonSelectionMode.AddBezier:
                        for (int i = 0; i < pointsCollection.Count; i++)
                        {
                            PolyPoint point = pointsCollection[i];

                            if (point.LeftCurve != null)
                            {
                                if (PointInSelectionPoint(absLeft + point.X + center.X + point.LeftCurve.X, absTop + point.Y + center.Y + point.LeftCurve.Y, mousePoint))
                                {
                                    currentPoint = point.LeftCurve;
                                    e.mode = WorkspaceMode2.Custom;
                                    e.handled = true;
                                    break;
                                }
                            }
                            else if (i == selectedPoint)
                            {
                                if (i > 0 || (this is PolygonObject))
                                {
                                    PolyPoint pseudoPoint = GetPseudoPoint(point, pointsCollection[i - 1]);
                                    if (PointInSelectionPoint(absLeft + pseudoPoint.X + center.X, absTop + pseudoPoint.Y + center.Y, mousePoint))
                                    {
                                        pseudoPoint.X -= point.X;
                                        pseudoPoint.Y -= point.Y;
                                        point.LeftCurve = pseudoPoint;
                                        currentPoint = pseudoPoint;
                                        e.mode = WorkspaceMode2.Custom;
                                        e.handled = true;
                                        break;
                                    }
                                }
                            }

                            if (point.RightCurve != null)
                            {
                                if (PointInSelectionPoint(absLeft + point.X + center.X + point.RightCurve.X, absTop + point.Y + center.Y + point.RightCurve.Y, mousePoint))
                                {
                                    currentPoint = point.RightCurve;
                                    e.mode = WorkspaceMode2.Custom;
                                    e.handled = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (i < pointsCollection.Count || (this is PolygonObject))
                                {
                                    PolyPoint pseudoPoint = GetPseudoPoint(point, pointsCollection[i + 1]);
                                    if (PointInSelectionPoint(absLeft + pseudoPoint.X + center.X, absTop + pseudoPoint.Y + center.Y, mousePoint))
                                    {
                                        pseudoPoint.X -= point.X;
                                        pseudoPoint.Y -= point.Y;
                                        point.RightCurve = pseudoPoint;
                                        currentPoint = pseudoPoint;
                                        e.mode = WorkspaceMode2.Custom;
                                        e.handled = true;
                                        break;
                                    }
                                }
                            }

                            if (PointInSelectionPoint(absLeft + point.X + center.X, absTop + point.Y + center.Y, mousePoint))
                            {
                                currentPoint = point;
                                e.mode = WorkspaceMode2.Custom;
                                e.handled = true;
                                break;
                            }
                        }
                        break;

                    case PolygonSelectionMode.Delete:
                        for (int i = 0; i < pointsCollection.Count; i++)
                        {
                            PolyPoint point = pointsCollection[i];

                            if (point.LeftCurve != null && PointInSelectionPoint(absLeft + point.X + center.X + point.LeftCurve.X, absTop + point.Y + center.Y + point.LeftCurve.Y, mousePoint))
                            {
                                point.LeftCurve = null;
                                currentPoint = null;
                                e.mode = WorkspaceMode2.None;
                                e.handled = true;
                                return;
                            }
                            else if (point.RightCurve != null && PointInSelectionPoint(absLeft + point.X + center.X + point.RightCurve.X, absTop + point.Y + center.Y + point.RightCurve.Y, mousePoint))
                            {
                                point.RightCurve = null;
                                currentPoint = null;
                                e.mode = WorkspaceMode2.None;
                                e.handled = true;
                                return;
                            }
                            else if (PointInSelectionPoint(absLeft + point.X + center.X, absTop + point.Y + center.Y, mousePoint))
                            {
                                RemovePointAt(i);
                                currentPoint = null;
                                if (i < selectedPoint)
                                    selectedPoint--;
                                e.mode = WorkspaceMode2.None;
                                e.handled = true;
                                return;
                            }
                        }
                        break;

                    case PolygonSelectionMode.MoveAndScale:
                        if (pointsCollection.Count < 2)
                        {
                            for (int i = 0; i < pointsCollection.Count; i++)
                            {
                                PolyPoint point = pointsCollection[i];
                                if (PointInSelectionPoint(absLeft + point.X + center.X, absTop + point.Y + center.Y, mousePoint))
                                {
                                    currentPoint = point;
                                    e.mode = WorkspaceMode2.Custom;
                                    e.handled = true;
                                    break;
                                }
                            }
                        }
                        break;
                }
            }

            if (!e.handled)
                base.HandleMouseDown(e);
        }

        /// <inheritdoc/>
        public override void HandleMouseMove(FRMouseEventArgs e)
        {
            if (currentPoint != null)
            {
                currentPoint.X += e.delta.X;
                currentPoint.Y += e.delta.Y;
                //points[currentPoint] = point;
                RecalculateBounds();
                e.mode = WorkspaceMode2.Custom;
                e.handled = true;
            }

            if (!e.handled && SelectionMode == PolygonSelectionMode.AddToLine)
            {
                PolyPoint mousePointAligned = new PolyPoint((int)((e.x - AbsLeft - center.X) / Page.SnapSize.Width) * Page.SnapSize.Width, (int)((e.y - AbsTop - center.Y) / Page.SnapSize.Height) * Page.SnapSize.Height);
                drawSelectedPointLineNumber = GetBestPointPosition(mousePointAligned.X, mousePointAligned.Y);
                if (drawSelectedPointLine == null || !drawSelectedPointLine.Near(mousePointAligned))
                {
                    drawSelectedPointLine = mousePointAligned;
                    Report.Designer.Refresh();
                }
            }
            else
            {
                drawSelectedPointLineNumber = -1;
            }

            if (!e.handled)
            {
                base.HandleMouseMove(e);
            }

            if (e.handled)
            {
                e.cursor = Cursors.Cross;
            }

            if (IsSelected)
            {
                switch (SelectionMode)
                {
                    case PolygonSelectionMode.AddToLine:
                    case PolygonSelectionMode.AddBezier:
                        e.handled = true;
                        e.mode = WorkspaceMode2.Custom;
                        break;
                }
            }

            //if (toolbar != null && toolbar.IsVisible)
            //{
            //    //toolbar.MouseMove(e.x, e.y);
            //}
        }

        /// <inheritdoc/>
        public override void HandleMouseUp(FRMouseEventArgs e)
        {
            if (currentPoint != null)
            {
                RecalculateBounds();
                int index = pointsCollection.IndexOf(currentPoint);
                if (index >= 0)
                    selectedPoint = index;
                currentPoint = null;
                if (Report != null)
                    if (Report.Designer != null)
                        Report.Designer.SetModified(this, "Change");
            }

            if (IsSelected)
            {
                int mode = (int)SelectionMode;
                if (0 < mode)
                {
                    e.handled = true;
                }
            }
            if (!e.handled)
                base.HandleMouseUp(e);
        }

        /// <summary>
        /// Insert point to desired place of polyline
        /// Recalculate bounds after insert
        /// </summary>
        /// <param name="index">Index of place from zero to count</param>
        /// <param name="localX">local x - relative to left-top point</param>
        /// <param name="localY">local y - relative to left-top point</param>
        public void InsertPointByIndex(int index, float localX, float localY)
        {
            insertPoint(index, localX, localY, 0);
            if (Report != null)
                if (Report.Designer != null)
                    Report.Designer.SetModified(this, "Change");
        }

        /// <summary>
        /// Insert point to near line
        /// Recalculate bounds after insert
        /// </summary>
        /// <param name="localX">local x - relative to left-top point</param>
        /// <param name="localY">local y - relative to left-top point</param>
        /// <param name="maxDistance">depricated</param>
        /// <returns>Index of inserted point</returns>
        public PolyPoint InsertPointByLocation(float localX, float localY, float maxDistance)
        {
            PolyPoint result;
            int pointNumber = -1;
            if (pointsCollection.Count == 0)
            {
                result = addPoint(0, 0, 0);
                pointNumber = 0;
            }
            else
            {
                pointNumber = GetBestPointPosition(localX, localY);
                result = insertPoint(pointNumber, localX, localY, 1);
            }
            RecalculateBounds();
            if (pointNumber >= 0)
                if (Report != null)
                    if (Report.Designer != null)
                        Report.Designer.SetModified(this, "Change");
            return result;
        }

        /// <inheritdoc/>
        public bool InvokeEditor()
        {
            if (IsSelected)
            {
                if (SelectionMode == PolygonSelectionMode.MoveAndScale)
                {
                    Report report = Report;
                    if (report != null)
                    {
                        SelectionMode = PolygonSelectionMode.Normal;
                    }
                    return true;
                }
                else if (SelectionMode == PolygonSelectionMode.Normal)
                {
                    Report report = Report;
                    if (report != null)
                    {
                        SelectionMode = PolygonSelectionMode.MoveAndScale;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc/>
        public override void OnAfterInsert(InsertFrom source)
        {
            base.OnAfterInsert(source);
            if (Report != null)
            {
                SelectedObjectCollection selectionObjects = Report.Designer.SelectedObjects;
                selectionObjects.Clear();
                selectionObjects.Add(this);
            }
            if (pointsCollection.Count < 2)
                SelectionMode = PolygonSelectionMode.AddToLine;
            if (pointsCollection.Count > 1)
                SelectionMode = PolygonSelectionMode.MoveAndScale;
        }

        /// <inheritdoc/>
        public override void OnBeforeInsert(int flags)
        {
            float rotation = (float)((flags & 0xF) / 8.0 * Math.PI);
            int numberOfEdges = (flags & 0xF0) >> 4;
            if (numberOfEdges >= 5)
            {
                float x, y;

                pointsCollection.Clear();

                for (int i = 0; i < numberOfEdges; i++)
                {
                    base.Width = 100;
                    base.Height = 100;
                    x = 50 + (float)(50 * Math.Cos(rotation + 2 * Math.PI * i / numberOfEdges));
                    y = 50 + (float)(50 * Math.Sin(rotation + 2 * Math.PI * i / numberOfEdges));

                    pointsCollection.Add(new PolyPoint(x, y));
                }
            }
            else
            {
                pointsCollection.Add(new PolyPoint(0, 0));
                Width = 5;
                Height = 5;
            }
        }

        /// <inheritdoc/>
        public override bool PointInObject(PointF point)
        {
            using (Pen pen = new Pen(Color.Black, 10))
            using (GraphicsPath path = new GraphicsPath())
            {
                float absLeft = AbsLeft;
                float absTop = AbsTop;
                if (pointsCollection.Count > 1 && currentPoint == null)
                    for (int i = 0; i < pointsCollection.Count - 1; i++)
                    {
                        PolyPoint prev = pointsCollection[i];
                        PolyPoint next = pointsCollection[i + 1];
                        path.AddLine(absLeft + prev.X + center.X, absTop + prev.Y + center.Y, absLeft + next.X + center.X, absTop + next.Y + center.Y);
                    }
                else
                    path.AddLine(absLeft -1 + CenterX, absTop -1 + CenterY, AbsRight+ 1 + CenterX, AbsBottom +1 + CenterY);

                return path.IsOutlineVisible(point, pen);
            }
        }

        /// <summary>
        /// Delete point from polyline by index
        /// Recalculate bounds after remove
        /// </summary>
        /// <param name="index">Index of point in polyline</param>
        public void RemovePointAt(int index)
        {
            deletePoint(index);
            if (Report != null)
                if (Report.Designer != null)
                    Report.Designer.SetModified(this, "Change");
        }

        /// <inheritdoc/>
        public override void SelectionChanged()
        {
            base.SelectionChanged();
            if (!IsSelected)
                SelectionMode = PolygonSelectionMode.MoveAndScale;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Delete point from polyline by index.
        /// Recalculate bounds.
        /// </summary>
        /// <param name="index">Index of point in polyline</param>
        protected void DeletePointByIndex(int index)
        {
            pointsCollection.Remove(index);
            RecalculateBounds();
        }

        /// <inheritdoc/>
        protected override void DrawSelectionPoint(FRPaintEventArgs e, Pen p, Brush b, float x, float y)
        {
            if (SelectionMode == PolygonSelectionMode.MoveAndScale)
            {
                base.DrawSelectionPoint(e, p, b, x, y);
            }
            else
            {
                IGraphics g = e.Graphics;
                x = (float)Math.Round(x * e.ScaleX);
                y = (float)Math.Round(y * e.ScaleY);
                g.FillEllipse(b, x - DpiHelper.ConvertUnits(3), y - DpiHelper.ConvertUnits(3), DpiHelper.ConvertUnits(6), DpiHelper.ConvertUnits(6));
                g.DrawEllipse(p, x - DpiHelper.ConvertUnits(3), y - DpiHelper.ConvertUnits(3), DpiHelper.ConvertUnits(6), DpiHelper.ConvertUnits(6));
            }
        }

        /// <inheritdoc/>
        protected override SelectionPoint[] GetSelectionPoints()
        {
            List<SelectionPoint> selectionPoints = new List<SelectionPoint>();
            if (pointsCollection.Count == 0)
            {
                return new SelectionPoint[] {
                new SelectionPoint(AbsLeft + CenterX, AbsTop + CenterY,SizingPoint.None)
                };
            }
            if (SelectionMode == PolygonSelectionMode.MoveAndScale && pointsCollection.Count > 1)
            {
                return base.GetSelectionPoints();
            }

            if (pointsCollection.Count > 0)
                selectedPoint = (pointsCollection.Count + selectedPoint) % pointsCollection.Count;

            float absLeft = AbsLeft;
            float absTop = AbsTop;

            for (int i = 0; i < pointsCollection.Count; i++)
            {
                PolyPoint point = pointsCollection[i];

                selectionPoints.Add(new SelectionPoint(point.X + absLeft + center.X, point.Y + absTop + center.Y,

                    i == selectedPoint ? SizingPoint.LeftTop : SizingPoint.RightBottom));

                if (SelectionMode != PolygonSelectionMode.AddToLine)
                {
                    if (point.LeftCurve != null)
                    {
                        selectionPoints.Add(new SelectionPoint(point.X + absLeft + center.X + point.LeftCurve.X, point.Y + absTop + center.Y + point.LeftCurve.Y, SizingPoint.RightTop));
                    }
                    else if (SelectionMode == PolygonSelectionMode.AddBezier && i == selectedPoint)
                    {
                        if (i > 0 || (this is PolygonObject))
                        {
                            PolyPoint pseudoPoint = GetPseudoPoint(point, pointsCollection[i - 1]);
                            selectionPoints.Add(new SelectionPoint(pseudoPoint.X + absLeft + center.X, pseudoPoint.Y + absTop + center.Y, SizingPoint.RightTop));
                        }
                    }

                    if (point.RightCurve != null)
                    {
                        selectionPoints.Add(new SelectionPoint(point.X + absLeft + center.X + point.RightCurve.X, point.Y + absTop + center.Y + point.RightCurve.Y, SizingPoint.RightTop));
                    }
                    else if (SelectionMode == PolygonSelectionMode.AddBezier && i == selectedPoint)
                    {
                        if (i < pointsCollection.Count - 1 || (this is PolygonObject))
                        {
                            PolyPoint pseudoPoint = GetPseudoPoint(point, pointsCollection[i + 1]);
                            selectionPoints.Add(new SelectionPoint(pseudoPoint.X + absLeft + center.X, pseudoPoint.Y + absTop + center.Y, SizingPoint.RightTop));
                        }
                    }
                }
            }

            return selectionPoints.ToArray();
        }

        #endregion Protected Methods

        #region Private Methods

        private void DrawDesign0(FRPaintEventArgs e)
        {
            if (IsSelected && SelectionMode == PolyLineObject.PolygonSelectionMode.MoveAndScale)
            {
                Pen pen = e.Cache.GetPen(Color.Gray, 1, DashStyle.Dot);
                e.Graphics.DrawRectangle(pen, AbsLeft * e.ScaleX, AbsTop * e.ScaleY, Width * e.ScaleX, Height * e.ScaleY);
            }
        }

        private void DrawDesign1(FRPaintEventArgs e)
        {
        }

        /// <summary>
        /// Returns best new point position based on mouse
        /// </summary>
        /// <returns></returns>
        private int GetBestPointPosition(float localX, float localY)
        {
            if (pointsCollection.Count > 0)
            {
                selectedPoint = (pointsCollection.Count + selectedPoint) % pointsCollection.Count;
                //add near select point
                //check if point is first or last
                if (selectedPoint == pointsCollection.Count - 1 && !(this is PolygonObject))
                {
                    return pointsCollection.Count;
                }
                else if (selectedPoint == 0 && !(this is PolygonObject))
                {
                    return 0;
                }
                else
                {
                    PolyPoint local = new PolyPoint(localX, localY);
                    PolyPoint pLeft = pointsCollection[selectedPoint - 1];
                    PolyPoint pCenter = pointsCollection[selectedPoint];
                    PolyPoint pRight = pointsCollection[selectedPoint + 1];

                    //float pLeft_pLocal =
                    //    ((pLeft.X + CenterX - localX) * (pLeft.X + CenterX - localX) + (pLeft.Y + CenterY - localY) * (pLeft.Y + CenterY - localY));

                    //float pRight_pLocal =
                    //    ((pRight.X + CenterX - localX) * (pRight.X + CenterX - localX) + (pRight.Y + CenterY - localY) * (pRight.Y + CenterY - localY));

                    //float pLeft_pCenter = ((pLeft.X - pCenter.X) * (pLeft.X - pCenter.X) + (pLeft.Y - pCenter.Y) * (pLeft.Y - pCenter.Y));
                    //float pRight_pCenter = ((pRight.X - pCenter.X) * (pRight.X - pCenter.X) + (pRight.Y - pCenter.Y) * (pRight.Y - pCenter.Y));

                    //float distance1 = pLeft_pLocal/ pLeft_pCenter;
                    //float distance2 = pRight_pLocal / pRight_pCenter;

                    float distance1 = MATH_pnt_seg_dist(local, pLeft, pCenter);
                    float distance2 = MATH_pnt_seg_dist(local, pCenter, pRight);

                    if (distance1 < distance2)
                    {
                        return selectedPoint;
                    }
                    else
                    {
                        return selectedPoint + 1;
                    }
                }
            }
            return 0;
        }

        private PointF GetPointF(int index, float scaleX, float scaleY, float absLeft, float absTop)
        {
            return GetPointF(pointsCollection[index], scaleX, scaleY, absLeft, absTop);
        }

        private PointF GetPointF(PolyPoint point, float scaleX, float scaleY, float absLeft, float absTop)
        {
            return new PointF(
                (point.X + absLeft + center.X) * scaleX,
                (point.Y + absTop + center.Y) * scaleY
                );
        }

        private void InitDesign()
        {
            currentPoint = null;
            SelectionMode = PolygonSelectionMode.MoveAndScale;
            //toolbar = new PolyLineObjectToolBar(this);
        }

        private float MATH_dot(float aX, float aY, float bX, float bY)
        {
            return aX * bX + aY * bY;
        }

        private float MATH_length_squared(PolyPoint a, PolyPoint b)
        {
            return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
        }

        private float MATH_pnt_seg_dist(PolyPoint p, PolyPoint v, PolyPoint w)
        {
            // Return minimum distance between line segment vw and point p
            float l2 = MATH_length_squared(v, w);  // i.e. |w-v|^2 -  avoid a sqrt
            if (l2 == 0.0) return MATH_length_squared(p, v);   // v == w case
                                                               // Consider the line extending the segment, parameterized as v + t (w - v).
                                                               // We find projection of point p onto the line.
                                                               // It falls where t = [(p-v) . (w-v)] / |w-v|^2
                                                               // We clamp t from [0,1] to handle points outside the segment vw.
            float t = Math.Max(0, Math.Min(1, MATH_dot(p.X - v.X, p.Y - v.Y, w.X - v.X, w.Y - v.Y) / l2));
            PolyPoint projection = new PolyPoint(v.X + t * (w.X - v.X), v.Y + t * (w.Y - v.Y));  // Projection falls on the segment
            return MATH_length_squared(p, projection);
        }

        #endregion Private Methods
    }
}