using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FastReport.Map;
using FastReport.Utils;
using FastReport.Forms;
using System.Drawing;
using System.Reflection;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Map.Forms
{
  internal partial class MapEditorForm : BaseDialogForm
  {
    #region Fields
    private MapObject map;
    private MapObject originalMap;
    private MapEditorControl mapEditor;
    private LayerEditorControl layerEditor;
    #endregion // Fields

    #region Properties

    public MapObject Map
    {
      get { return map; }
      set
      {
        originalMap = value;
        map = new MapObject();
        map.AssignAll(originalMap, true);
        map.SetReport(originalMap.Report);
        map.SetDesigning(true);
        foreach (MapLayer layer in map.Layers)
        {
          if (!layer.IsShapefileEmbedded)
            layer.LoadShapefile(layer.Shapefile);
        }
        PopulateMapTree(map);
      }
    }

    #endregion // Properties

    #region Private Methods
    private void Init()
    {
      btnUp.Image = Res.GetImage(208);
      btnDown.Image = Res.GetImage(209);
      tvMap.ImageList = Res.GetImages();
      pnSample.GetType().GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(pnSample, new object[] { ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true });
    }

    private void PopulateMapTree(object select)
    {
      tvMap.Nodes.Clear();

      TreeNode mapNode = tvMap.Nodes.Add(Res.Get("Forms,MapEditor,Map"));
      mapNode.Tag = map;
      mapNode.ImageIndex = 153;
      mapNode.SelectedImageIndex = mapNode.ImageIndex;

      for (int i = 0; i < map.Layers.Count; i++)
      {
        MapLayer layer = map.Layers[i];
        TreeNode layerNode = mapNode.Nodes.Add(Res.Get("Forms,MapEditor,Layer") + " " + (i + 1).ToString());
        layerNode.Tag = layer;
        layerNode.ImageIndex = 169;
        layerNode.SelectedImageIndex = layerNode.ImageIndex;
        if (select == layer)
          tvMap.SelectedNode = layerNode;
      }

      mapNode.Expand();
      if (select == map)
        tvMap.SelectedNode = mapNode;
    }

    private void ShowProperties(object selected)
    {
      if (selected is MapObject)
      {
        if (mapEditor == null)
        {
          mapEditor = new MapEditorControl();
                    ScaleControl(mapEditor);
                    mapEditor.Font = DpiHelper.ConvertUnits(DrawUtils.DefaultFont, true);
          mapEditor.Location = new Point(pcMap.Right + pcMap.Left, pcMap.Top);
          mapEditor.Parent = this;
          mapEditor.Map = map;
          mapEditor.Changed += new EventHandler(RefreshSample);
        }
        if (layerEditor != null)
          layerEditor.Hide();
        mapEditor.Show();
      }
      else if (selected is MapLayer)
      {
        if (layerEditor == null)
        {
          layerEditor = new LayerEditorControl();
                    ScaleControl(layerEditor);
          layerEditor.Location = new Point(pcMap.Right + pcMap.Left, pcMap.Top);
          layerEditor.Parent = this;
          layerEditor.Changed += new EventHandler(RefreshSample);
        }
        if (mapEditor != null)
          mapEditor.Hide();
        layerEditor.Layer = selected as MapLayer;
        layerEditor.Show();
      }
    }

    private void RefreshSample()
    {
      pnSample.Refresh();
    }

    private void RefreshSample(object sender, EventArgs e)
    {
      RefreshSample();
    }

    #endregion // Private Methods

    #region Internal Methods

    internal void EnableMercatorProtection(bool enable)
    {
        mapEditor.EnableMercatorProtection(enable);
    }

    #endregion // Internal Methods

    #region Public Methods

    public override void Localize()
    {
      base.Localize();
      MyRes res = new MyRes("Forms,MapEditor");
      Text = res.Get("");
      btnAdd.Text = res.Get("Add");
      btnDelete.Text = res.Get("Delete");
      lblHint.Text = res.Get("Hint");
    }

    #endregion // Public Methods

    #region Events Handlers
    private void MapEditorForm_Shown(object sender, EventArgs e)
    {
      tvMap.Focus();
    }

    private void MapEditorForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      if (DialogResult == DialogResult.OK)
      {
        originalMap.AssignAll(Map, true);
        foreach (MapLayer layer in originalMap.Layers)
        {
          if (!layer.IsShapefileEmbedded)
            layer.LoadShapefile(layer.Shapefile);
        }
        originalMap.CreateUniqueNames();
      }
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      using (AddLayerForm form = new AddLayerForm())
      {
        form.Map = map;
        form.Tag = this;
        if (form.ShowDialog() == DialogResult.OK)
        {
            if (map.Layers.Count > 0)
            {
                PopulateMapTree(map.Layers[map.Layers.Count - 1]);
                RefreshSample();
            }
        }
      }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      MapLayer layer = tvMap.SelectedNode.Tag as MapLayer;
      map.Layers.Remove(layer);
      if (map.Layers.Count == 0)
      {
          EnableMercatorProtection(true);
      }
      if (map.Layers.Count > 0)
      {
          PopulateMapTree(map.Layers[map.Layers.Count - 1]);
      }
      else
      {
          PopulateMapTree(map);
      }
      RefreshSample();
    }

    private void btnUp_Click(object sender, EventArgs e)
    {
      MapLayer layer = tvMap.SelectedNode.Tag as MapLayer;
      int index = map.Layers.IndexOf(layer);
      map.Layers.RemoveAt(index);
      map.Layers.Insert(index - 1, layer);
      PopulateMapTree(layer);
      RefreshSample();
    }

    private void btnDown_Click(object sender, EventArgs e)
    {
      MapLayer layer = tvMap.SelectedNode.Tag as MapLayer;
      int index = map.Layers.IndexOf(layer);
      map.Layers.RemoveAt(index);
      map.Layers.Insert(index + 1, layer);
      PopulateMapTree(layer);
      RefreshSample();
    }

    private void tvMap_AfterSelect(object sender, TreeViewEventArgs e)
    {
      object selected = tvMap.SelectedNode.Tag;
      btnDelete.Enabled = selected is MapLayer;
      btnUp.Enabled = selected is MapLayer && tvMap.SelectedNode.Index > 0;
      btnDown.Enabled = selected is MapLayer && tvMap.SelectedNode.Index < map.Layers.Count - 1;
      ShowProperties(selected);
    }

    private void pnSample_Paint(object sender, PaintEventArgs e)
    {
      RectangleF saveBounds = map.Bounds;
      
      try
      {
        if (map.Layers.Count > 0)
        {
          map.Bounds = new RectangleF(1, 1, pnSample.Width - 2, pnSample.Height - 2);
          map.Draw(new FRPaintEventArgs(e.Graphics, 1, 1, map.Report.GraphicCache));
        }
      }
      catch (Exception ex)
      {
        using (StringFormat sf = new StringFormat())
        {
          sf.Alignment = StringAlignment.Center;
          sf.LineAlignment = StringAlignment.Center;
          e.Graphics.DrawString(ex.Message, Font, Brushes.Red, pnSample.DisplayRectangle, sf);
        }
      }
      
      map.Bounds = saveBounds;
    }

    #endregion // Events Handlers

    public MapEditorForm()
    {
      InitializeComponent();
      Init();
      Localize();
            Scale();
    }
#if !MONO
        protected override void UpdateResources()
        {
            btnUp.Image = GetImage(208);
            btnDown.Image = GetImage(209);
            tvMap.ImageList = Res.GetImages(FormRatio);
        }
#endif
    }
}
