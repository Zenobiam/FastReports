using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using FastReport.Utils;
using FastReport.Map.Import.Shp;
using FastReport.Map.Import.Osm;
using System.Windows.Forms;

namespace FastReport.Map
{
    /// <summary>
    /// Represents a map object.
    /// </summary>
    public partial class MapObject : ReportComponentBase, IParent
    {
        #region Fields
        private float scale;
        private float zoom;
        private float minZoom;
        private float maxZoom;
        private float offsetX;
        private float offsetY;
        private LayerCollection layers;
        private ColorScale colorScale;
        private Padding padding;
        private bool mercatorProjection;
        #endregion // Fields

        #region Properties
        /// <summary>
        /// Gets or sets the path to folder containing shapefiles.
        /// </summary>
        /// <remarks>
        /// This property is used by the map editor when selecting a shapefile.
        /// </remarks>
        public static string ShapefileFolder;

        /// <summary>
        /// Gets or sets the map zoom.
        /// </summary>
        [DefaultValue(1.0f)]
        [Category("Appearance")]
        public float Zoom
        {
            get { return zoom; }
            set
            {
                if (value < MinZoom)
                    value = MinZoom;
                if (value > MaxZoom)
                    value = MaxZoom;
                zoom = value;
            }
        }

        /// <summary>
        /// Gets or sets minimum zoom value.
        /// </summary>
        [DefaultValue(1.0f)]
        [Category("Appearance")]
        public float MinZoom
        {
            get { return minZoom; }
            set { minZoom = value; }
        }

        /// <summary>
        /// Gets or sets maximum zoom value.
        /// </summary>
        [DefaultValue(50f)]
        [Category("Appearance")]
        public float MaxZoom
        {
            get { return maxZoom; }
            set { maxZoom = value; }
        }

        /// <summary>
        /// Gets or sets the X offset of the map.
        /// </summary>
        [DefaultValue(0f)]
        [Category("Layout")]
        public float OffsetX
        {
            get { return offsetX; }
            set { offsetX = value; }
        }

        /// <summary>
        /// Gets or sets the Y offset of the map.
        /// </summary>
        [DefaultValue(0f)]
        [Category("Layout")]
        public float OffsetY
        {
            get { return offsetY; }
            set { offsetY = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating that mercator projection must be used to view the map.
        /// </summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool MercatorProjection
        {
            get { return mercatorProjection; }
            set { mercatorProjection = value; }
        }

        /// <summary>
        /// Gets the color scale settings.
        /// </summary>
        [Category("Appearance")]
        public ColorScale ColorScale
        {
            get { return colorScale; }
        }

        /// <summary>
        /// Gets or sets a collection of map layers.
        /// </summary>
        [Browsable(false)]
        public LayerCollection Layers
        {
            get { return layers; }
            set { layers = value; }
        }

        /// <summary>
        /// Gets or sets padding within the map.
        /// </summary>
        [Category("Layout")]
        public Padding Padding
        {
            get { return padding; }
            set { padding = value; }
        }

        internal float ScaleG
        {
            get { return scale * Zoom; }
        }

        internal float OffsetXG
        {
            get { return -((Width * Zoom - Width) / 2) + offsetX * Zoom; }
        }

        internal float OffsetYG
        {
            get { return -((Height * Zoom - Height) / 2) + offsetY * Zoom; }
        }

        internal bool IsEmpty
        {
            get { return Layers.Count == 0; }
        }
        #endregion // Properties

        #region Private Methods
        private void DrawLayers(FRPaintEventArgs e)
        {
            IGraphics g = e.Graphics;
            RectangleF drawRect = new RectangleF(
              (AbsLeft + Padding.Left) * e.ScaleX,
              (AbsTop + Padding.Top) * e.ScaleY,
              (Width - Padding.Horizontal) * e.ScaleX,
              (Height - Padding.Vertical) * e.ScaleY);
            IGraphicsState state = g.Save();
            try
            {
                g.SetClip(drawRect);
                foreach (MapLayer layer in layers)
                {
                    layer.SetPrinting(IsPrinting);
                    layer.Draw(e);
                }
            }
            finally
            {
                g.Restore(state);
            }
        }

        private void DrawScales(FRPaintEventArgs e)
        {
            ColorScale.Data = null;

            // find the layer which ColorRanges is set to show in the color scale
            foreach (MapLayer layer in Layers)
            {
                if (layer.ColorRanges.ShowInColorScale)
                {
                    ColorScale.Data = layer.ColorRanges;
                    break;
                }
            }

            if (ColorScale.Visible)
                ColorScale.Draw(e, this);
        }

        internal void DrawMap(FRPaintEventArgs e)
        {
            if (IsEmpty)
                return;

            SmoothingMode saveMode = e.Graphics.SmoothingMode;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            MapLayer layer = Layers[0];
            double layerWidth = layer.Box.MaxX - layer.Box.MinX;
            double layerHeight = MercatorProjection ?
              CoordinateConverter.ConvertMercator(layer.Box.MaxY) - CoordinateConverter.ConvertMercator(layer.Box.MinY) :
              layer.Box.MaxY - layer.Box.MinY;

            float scaleX = (Width - Padding.Horizontal) / (float)layerWidth;
            float scaleY = (Height - Padding.Vertical) / (float)layerHeight;
            scale = scaleX < scaleY ? scaleX : scaleY;

            DrawLayers(e);
            e.Graphics.SmoothingMode = saveMode;
            DrawScales(e);
        }

        private void ZoomIn()
        {
            Zoom *= 1.2f;
        }

        private void ZoomOut()
        {
            Zoom /= 1.2f;
        }
        #endregion // Private Methods

        #region IParent Members
        /// <inheritdoc/>
        public bool CanContain(Base child)
        {
            return child is MapLayer;
        }

        /// <inheritdoc/>
        public void GetChildObjects(ObjectCollection list)
        {
            foreach (MapLayer layer in layers)
            {
                list.Add(layer);
            }
        }

        /// <inheritdoc/>
        public void AddChild(Base child)
        {
            if (child is MapLayer)
                layers.Add(child as MapLayer);
        }

        /// <inheritdoc/>
        public void RemoveChild(Base child)
        {
            if (child is MapLayer)
                layers.Remove(child as MapLayer);
        }

        /// <inheritdoc/>
        public int GetChildOrder(Base child)
        {
            if (child is MapLayer)
                return layers.IndexOf(child as MapLayer);
            return 0;
        }

        /// <inheritdoc/>
        public void SetChildOrder(Base child, int order)
        {
            int oldOrder = child.ZOrder;
            if (oldOrder != -1 && order != -1 && order != oldOrder)
            {
                if (child is MapLayer)
                {
                    if (order > layers.Count)
                    {
                        order = layers.Count;
                    }
                    if (order >= oldOrder)
                    {
                        order--;
                    }
                    layers.Remove(child as MapLayer);
                    layers.Insert(order, child as MapLayer);
                }
            }
        }

        /// <inheritdoc/>
        public void UpdateLayout(float dx, float dy)
        {
            // do nothing
        }

        #endregion // IParent Members

        #region Report Engine
        /// <inheritdoc/>
        public override void SaveState()
        {
            base.SaveState();
            foreach (MapLayer layer in Layers)
            {
                layer.SaveState();
            }
        }

        /// <inheritdoc/>
        public override void RestoreState()
        {
            base.RestoreState();
            foreach (MapLayer layer in Layers)
            {
                layer.RestoreState();
            }
        }

        /// <inheritdoc/>
        public override void GetData()
        {
            base.GetData();
            foreach (MapLayer layer in Layers)
            {
                layer.GetData();
            }
        }

        /// <inheritdoc/>
        public override void InitializeComponent()
        {
            base.InitializeComponent();
            foreach (MapLayer layer in Layers)
            {
                layer.InitializeComponent();
            }
        }

        /// <inheritdoc/>
        public override void FinalizeComponent()
        {
            base.FinalizeComponent();
            foreach (MapLayer layer in Layers)
            {
                layer.FinalizeComponent();
            }
        }
        #endregion // Report Engine

        #region Public Methods
        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);
            MapObject src = source as MapObject;
            MinZoom = src.MinZoom;
            MaxZoom = src.MaxZoom;
            Zoom = src.Zoom;
            OffsetX = src.OffsetX;
            OffsetY = src.OffsetY;
            Padding = src.Padding;
            MercatorProjection = src.MercatorProjection;
            ColorScale.Assign(src.ColorScale);
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            MapObject c = writer.DiffObject as MapObject;
            base.Serialize(writer);

            if (MinZoom != c.MinZoom)
                writer.WriteFloat("MinZoom", MinZoom);
            if (MaxZoom != c.MaxZoom)
                writer.WriteFloat("MaxZoom", MaxZoom);
            if (Zoom != c.Zoom)
                writer.WriteFloat("Zoom", Zoom);
            if (OffsetX != c.OffsetX)
                writer.WriteFloat("OffsetX", OffsetX);
            if (OffsetY != c.OffsetY)
                writer.WriteFloat("OffsetY", OffsetY);
            if (Padding != c.Padding)
                writer.WriteValue("Padding", Padding);
            if (MercatorProjection != c.MercatorProjection)
                writer.WriteBool("MercatorProjection", MercatorProjection);
            ColorScale.Serialize(writer, "ColorScale", c.ColorScale);
        }

        /// <summary>
        /// Loads a map from file.
        /// </summary>
        /// <param name="filename">Name of file that contains a map.</param>
        public void Load(string filename)
        {
            string extension = Path.GetExtension(filename).ToLower();
            if (extension == ".shp")
            {
                using (ShpMapImport import = new ShpMapImport())
                {
                    import.ImportMap(this, null, filename);
                    if (Layers.Count > 0)
                        Layers[Layers.Count - 1].Simplify(0.03);
                    CreateUniqueNames();
                }
            }
            else if (extension == ".osm")
            {
                mercatorProjection = false;
                using (OsmMapImport import = new OsmMapImport())
                {
                    import.ImportMap(this, null, filename);
                    if (Layers.Count > 0)
                        Layers[Layers.Count - 1].Simplify(0.03);
                    CreateUniqueNames();
                }
            }
        }

        /// <summary>
        /// Creates unique names for all contained objects such as layers, shapes, etc.
        /// </summary>
        public void CreateUniqueNames()
        {
            if (Report != null)
            {
                FastNameCreator nameCreator = new FastNameCreator(Report.AllNamedObjects);
                foreach (MapLayer layer in layers)
                {
                    if (String.IsNullOrEmpty(layer.Name))
                        nameCreator.CreateUniqueName(layer);
                    layer.CreateUniqueNames();
                }
            }
        }

        #endregion // Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="MapObject"/> class.
        /// </summary>
        public MapObject()
        {
            layers = new LayerCollection(this);
            zoom = 1;
            minZoom = 1;
            maxZoom = 50;
            padding = new Padding();
            colorScale = new ColorScale();
            colorScale.Dock = ScaleDock.BottomLeft;
            mercatorProjection = true;
            Fill = new SolidFill(Color.Gainsboro);
            SetFlags(Flags.InterceptsPreviewMouseEvents, true);
            FlagProvidesHyperlinkValue = true;
        }

    }
}