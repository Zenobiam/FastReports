using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Utils;
using FastReport.Map.Forms;

namespace FastReport.Map
{
    partial class MapObject : IHasEditor
    {
        #region Fields
        private bool needDesignerModify;
        private int doubleClickTickCount;
        private PointF doubleClickPos;
        #endregion // Fields

        #region Properties
        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new bool CanGrow
        {
            get { return base.CanGrow; }
            set { base.CanGrow = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new bool CanShrink
        {
            get { return base.CanShrink; }
            set { base.CanShrink = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new string Style
        {
            get { return base.Style; }
            set { base.Style = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new string EvenStyle
        {
            get { return base.EvenStyle; }
            set { base.EvenStyle = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new StylePriority EvenStylePriority
        {
            get { return base.EvenStylePriority; }
            set { base.EvenStylePriority = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new string HoverStyle
        {
            get { return base.HoverStyle; }
            set { base.HoverStyle = value; }
        }

        /// <inheritdoc/>
        public override bool IsSelected
        {
            get
            {
                if (Report == null)
                    return false;
                return Report.Designer.SelectedObjects.IndexOf(this) != -1 || IsInternalSelected;
            }
        }

        private bool IsInternalSelected
        {
            get
            {
                if (Report == null)
                    return false;
                SelectedObjectCollection selection = Report.Designer.SelectedObjects;
                return selection.Count > 0 && (
                  (selection[0] is MapLayer && (selection[0] as MapLayer).Map == this) ||
                  (selection[0] is ShapeBase && (selection[0] as ShapeBase).Map == this));
            }
        }
        #endregion // Properties

        #region Private Methods
        internal void GenerateRandomData()
        {
            if (IsEmpty)
                return;

            foreach (MapLayer layer in Layers)
            {
                layer.InitializeData();

                if (!String.IsNullOrEmpty(layer.SpatialColumn))
                {
                    double value = 0;
                    foreach (ShapeBase shape in layer.Shapes)
                    {
                        layer.AddValue(shape.SpatialValue, value);
                        value += 50;
                    }
                }
                else
                {
                    layer.AddValue("1", 0);
                    layer.AddValue("2", 1000);
                }

                layer.FinalizeData();
            }
        }

        private Base HitTest(PointF point)
        {
            for (int i = Layers.Count - 1; i >= 0; i--)
            {
                ShapeBase shape = Layers[i].HitTest(point);
                if (shape != null)
                    return shape;
            }
            return null;
        }
        #endregion // Private Methods

        #region Public Methods
        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            base.Draw(e);
            if (IsDesigning)
            {
                if (IsEmpty)
                {
                    string s = Res.Get("ComponentsMisc,Map,Hint");
                    Font font = new Font(DrawUtils.DefaultReportFont.Name, DrawUtils.DefaultFont.Size * e.ScaleX * 96f / DrawUtils.ScreenDpi, DrawUtils.DefaultFont.Style);
                    e.Graphics.DrawString(s, font, Brushes.Black,
                      new RectangleF(AbsLeft * e.ScaleX, AbsTop * e.ScaleY, Width * e.ScaleX, Height * e.ScaleY),
                      e.Cache.GetStringFormat(StringAlignment.Center, StringAlignment.Center, StringTrimming.None, StringFormatFlags.NoClip, 0, 0));
                }
                else
                {
                    try
                    {
                        SaveState();
                        GenerateRandomData();
                        DrawMap(e);
                    }
                    finally
                    {
                        RestoreState();
                    }
                }
            }
            else
                try
                {
                    SaveState();
                    DrawMap(e);
                }
                finally
                {
                    RestoreState();
                }

            DrawMarkers(e);
            Border.Draw(e, new RectangleF(AbsLeft, AbsTop, Width, Height));
            if (IsDesigning && IsSelected)
                e.Graphics.DrawImage(Res.GetImage(75), (int)(AbsLeft * e.ScaleX + 8), (int)(AbsTop * e.ScaleY - 8));
        }

        /// <inheritdoc/>
        public override SizeF GetPreferredSize()
        {
            if ((Page as ReportPage).IsImperialUnitsUsed)
                return new SizeF(Units.Inches * 4, Units.Inches * 4f);
            return new SizeF(Units.Millimeters * 80, Units.Millimeters * 80);
        }

        /// <inheritdoc/>
        public bool InvokeEditor()
        {
            using (MapEditorForm form = new MapEditorForm())
            {
                form.Map = this;
                return form.ShowDialog() == DialogResult.OK;
            }
        }

        /// <inheritdoc/>
        public override void OnBeforeInsert(int flags)
        {
            base.OnBeforeInsert(flags);
            // fill is reset by the designer's default formatting tool. Set it back.
            Fill = new SolidFill(Color.Gainsboro);
        }
        #endregion // Public Methods

        #region Designer mouse support
        /// <inheritdoc/>
        public override void HandleMouseHover(FRMouseEventArgs e)
        {
            if (IsSelected && new RectangleF(AbsLeft + 8, AbsTop - 8, 16, 16).Contains(new PointF(e.x, e.y)))
            {
                e.handled = true;
                e.cursor = Cursors.SizeAll;
            }
        }

        /// <inheritdoc/>
        public override void HandleMouseDown(FRMouseEventArgs e)
        {
            // allow doubleclick when polygon is selected
            bool doubleClick = Environment.TickCount - doubleClickTickCount < SystemInformation.DoubleClickTime &&
              new PointF(e.x, e.y).Equals(doubleClickPos);
            doubleClickTickCount = Environment.TickCount;
            doubleClickPos = new PointF(e.x, e.y);

            if (e.mode != WorkspaceMode2.None)
                return;

            // check move handle
            HandleMouseHover(e);
            if (e.handled)
            {
                // do base logic such as selecting/deselecting
                // and return with e.Mode = WorkspaceMode2.Move
                base.HandleMouseDown(e);
                e.handled = true;
                e.mode = WorkspaceMode2.Move;
            }
            else if (PointInObject(new PointF(e.x, e.y)))
            {
                e.handled = true;

                // hit test polygons
                Base obj = HitTest(new PointF(e.x, e.y));
                // pass rightclick and doubleclick to the map object
                if (obj == null || doubleClick || e.button == MouseButtons.Right)
                    obj = this;

                SelectedObjectCollection selection = Report.Designer.SelectedObjects;
                if (e.modifierKeys == Keys.Shift)
                {
                    // toggle selection
                    if (selection.IndexOf(obj) != -1)
                    {
                        if (selection.Count > 1)
                            selection.Remove(obj);
                    }
                    else
                        selection.Add(obj);
                }
                else
                {
                    // select the object if not selected yet
                    if (selection.IndexOf(obj) == -1)
                    {
                        selection.Clear();
                        selection.Add(obj);
                    }
                }

                e.mode = WorkspaceMode2.Custom;
                e.activeObject = this;
                isPanning = true;
                panned = false;
                e.delta = new PointF(0, 0);
            }
        }

        /// <inheritdoc/>
        public override void HandleMouseMove(FRMouseEventArgs e)
        {
            base.HandleMouseMove(e);
            if (!e.handled && e.button == MouseButtons.None)
            {
                // don't process if mouse is over move area
                HandleMouseHover(e);
                if (e.handled)
                {
                    e.handled = false;
                    return;
                }
                if (PointInObject(new PointF(e.x, e.y)))
                {
                    e.handled = true;
                }
                else
                {
                    // mouse leave, save changes if any
                    if (needDesignerModify)
                    {
                        Report.Designer.SetModified(this, "Change", Name);
                        needDesignerModify = false;
                    }
                }
            }

            if (isPanning && !IsEmpty)
            {
                OffsetX += e.delta.X / Zoom;
                OffsetY += e.delta.Y / Zoom;
                panned = true;
            }
        }

        /// <inheritdoc/>
        public override void HandleMouseUp(FRMouseEventArgs e)
        {
            base.HandleMouseUp(e);
            if (isPanning)
            {
                if (panned)
                    needDesignerModify = true;
            }
            isPanning = false;
            panned = false;
        }

        /// <inheritdoc/>
        public override void HandleMouseWheel(FRMouseEventArgs e)
        {
            if (IsSelected && !IsEmpty)
            {
                if (e.wheelDelta < 0)
                    ZoomOut();
                else
                    ZoomIn();
                needDesignerModify = true;
                e.handled = true;
            }
        }
        #endregion

    }
}