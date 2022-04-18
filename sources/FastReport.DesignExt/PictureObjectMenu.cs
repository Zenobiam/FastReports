using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FastReport.Design;
using FastReport.Utils;

namespace FastReport
{
  internal class PictureObjectMenu : ReportComponentBaseMenu
  {
    #region Fields
    public ContextMenuItem miSizeMode;
    public ContextMenuItem miAutoSize;
    public ContextMenuItem miCenterImage;
    public ContextMenuItem miNormal;
    public ContextMenuItem miStretchImage;
    public ContextMenuItem miZoom;
    private SelectedPictureObjects pictureObjects;
    #endregion
    
    #region Private Methods
    private void miAutoSize_Click(object sender, EventArgs e)
    {
      pictureObjects.SetSizeMode(PictureBoxSizeMode.AutoSize);
      Change();
    }

    private void miCenterImage_Click(object sender, EventArgs e)
    {
      pictureObjects.SetSizeMode(PictureBoxSizeMode.CenterImage);
      Change();
    }

    private void miNormal_Click(object sender, EventArgs e)
    {
      pictureObjects.SetSizeMode(PictureBoxSizeMode.Normal);
      Change();
    }

    private void miStretchImage_Click(object sender, EventArgs e)
    {
      pictureObjects.SetSizeMode(PictureBoxSizeMode.StretchImage);
      Change();
    }

    private void miZoom_Click(object sender, EventArgs e)
    {
      pictureObjects.SetSizeMode(PictureBoxSizeMode.Zoom);
      Change();
    }
    #endregion
    
    public PictureObjectMenu(Designer designer) : base(designer)
    {
      pictureObjects = new SelectedPictureObjects(Designer);
      pictureObjects.Update();
      
      MyRes res = new MyRes("ComponentMenu,PictureObject");
      miSizeMode = CreateMenuItem(res.Get("SizeMode"));
      miSizeMode.BeginGroup = true;
      miAutoSize = CreateMenuItem(res.Get("AutoSize"), new EventHandler(miAutoSize_Click));
      miCenterImage = CreateMenuItem(res.Get("CenterImage"), new EventHandler(miCenterImage_Click));
      miNormal = CreateMenuItem(res.Get("Normal"), new EventHandler(miNormal_Click));
      miStretchImage = CreateMenuItem(res.Get("StretchImage"), new EventHandler(miStretchImage_Click));
      miZoom = CreateMenuItem(res.Get("Zoom"), new EventHandler(miZoom_Click));

      int insertPos = Items.IndexOf(miCut);
      Items.Insert(insertPos, miSizeMode);

      miSizeMode.DropDownItems.AddRange(new ContextMenuItem[] {
        miAutoSize, miCenterImage, miNormal, miStretchImage, miZoom });

      bool enabled = pictureObjects.Enabled;
      miAutoSize.Enabled = enabled;
      miCenterImage.Enabled = enabled;
      miNormal.Enabled = enabled;
      miStretchImage.Enabled = enabled;
      miZoom.Enabled = enabled;

      if (enabled)
      {
        PictureBoxSizeMode mode = pictureObjects.First.SizeMode;
        miAutoSize.Checked = mode == PictureBoxSizeMode.AutoSize;
        miCenterImage.Checked = mode == PictureBoxSizeMode.CenterImage;
        miNormal.Checked = mode == PictureBoxSizeMode.Normal;
        miStretchImage.Checked = mode == PictureBoxSizeMode.StretchImage;
        miZoom.Checked = mode == PictureBoxSizeMode.Zoom;
      }  
      
    }
  }
}
