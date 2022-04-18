using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using FastReport.Utils;
using FastReport.Forms;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Map.Forms
{
  internal partial class AddLayerForm : BaseDialogForm
  {
    private MapObject map;

    public MapObject Map
    {
      get { return map; }
      set { map = value; }
    }

    private void rbShapefile_CheckedChanged(object sender, EventArgs e)
    {
        tbShapefile.Enabled = rbShapefile.Checked;
        cbEmbed.Enabled = rbShapefile.Checked;
    }

    private void tbShapefile_ButtonClick(object sender, EventArgs e)
    {
        using (OpenFileDialog dialog = new OpenFileDialog())
        {
            dialog.Filter = Res.Get("FileFilters,ShpFile") + "|" + Res.Get("FileFilters,OsmFile");
            if (!String.IsNullOrEmpty(MapObject.ShapefileFolder))
                dialog.InitialDirectory = MapObject.ShapefileFolder;
            if (dialog.ShowDialog() == DialogResult.OK)
                tbShapefile.Text = dialog.FileName;
        }
    }

    private void AddLayerForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (DialogResult == DialogResult.OK)
      {
        if (rbShapefile.Checked && String.IsNullOrEmpty(tbShapefile.Text))
        {
          FRMessageBox.Error(Res.Get("Messages,FileNameEmpty"));
          e.Cancel = true;
        }
      }
    }

    private void AddLayerForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      if (DialogResult == DialogResult.OK)
      {
        if (rbShapefile.Checked)
        {
            if (Path.GetExtension(tbShapefile.Text).ToLower() == ".osm")
            {
                (Tag as MapEditorForm).EnableMercatorProtection(false);
            }
            map.Load(tbShapefile.Text);
            if (!cbEmbed.Checked)
                map.Layers[map.Layers.Count - 1].Shapefile = tbShapefile.Text;
        }
        else if (rbEmptyLayer.Checked)
        {
          MapLayer layer = new MapLayer();
          Map.Layers.Add(layer);
          layer.SpatialSource = SpatialSource.ApplicationData;
        }
      }
    }

    public override void Localize()
    {
      base.Localize();
      MyRes res = new MyRes("Forms,AddLayer");
      Text = res.Get("");
      lblSource.Text = res.Get("Source");
      rbShapefile.Text = res.Get("Shapefile");
      cbEmbed.Text = res.Get("Embed");
      rbEmptyLayer.Text = res.Get("Empty");
      tbShapefile.ImageIndex = 1;
    }

    public AddLayerForm()
    {
      InitializeComponent();
      Localize();
            Scale();
    }

        protected override void Scale()
        {
            base.Scale();
            btnCancel.Location = new Point(Width - btnCancel.Width - DpiHelper.ConvertUnits(10), Height - btnCancel.Height - DpiHelper.ConvertUnits(10) - SystemInformation.CaptionHeight);
            btnOk.Location = new Point(btnCancel.Left - DpiHelper.ConvertUnits(6) - btnOk.Width, btnCancel.Location.Y);
        }
    }
}

