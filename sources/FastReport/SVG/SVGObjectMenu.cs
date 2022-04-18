using FastReport.Design;
using FastReport.Utils;
using System;
using System.Windows.Forms;

#pragma warning disable

namespace FastReport
{
    class SVGObjectMenu : ReportComponentBaseMenu
    {
        #region Fields
        public ContextMenuItem miSizeMode;
        public ContextMenuItem miAutoSize;
        public ContextMenuItem miCenterImage;
        public ContextMenuItem miNormal;
        public ContextMenuItem miStretchImage;
        public ContextMenuItem miZoom;
        private SelectedSVGObjects FPictureObjects;
        #endregion

        #region Private Methods
        private void miAutoSize_Click(object sender, EventArgs e)
        {
            FPictureObjects.SetSizeMode(PictureBoxSizeMode.AutoSize);
            Change();
        }

        private void miCenterImage_Click(object sender, EventArgs e)
        {
            FPictureObjects.SetSizeMode(PictureBoxSizeMode.CenterImage);
            Change();
        }

        private void miNormal_Click(object sender, EventArgs e)
        {
            FPictureObjects.SetSizeMode(PictureBoxSizeMode.Normal);
            Change();
        }

        private void miStretchImage_Click(object sender, EventArgs e)
        {
            FPictureObjects.SetSizeMode(PictureBoxSizeMode.StretchImage);
            Change();
        }

        private void miZoom_Click(object sender, EventArgs e)
        {
            FPictureObjects.SetSizeMode(PictureBoxSizeMode.Zoom);
            Change();
        }
        #endregion

        public SVGObjectMenu(Designer designer) : base(designer)
        {
            FPictureObjects = new SelectedSVGObjects(Designer);
            FPictureObjects.Update();

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

            bool enabled = FPictureObjects.Enabled;
            miAutoSize.Enabled = enabled;
            miCenterImage.Enabled = enabled;
            miNormal.Enabled = enabled;
            miStretchImage.Enabled = enabled;
            miZoom.Enabled = enabled;

            if (enabled)
            {
                PictureBoxSizeMode mode = FPictureObjects.First.SizeMode;
                miAutoSize.Checked = mode == PictureBoxSizeMode.AutoSize;
                miCenterImage.Checked = mode == PictureBoxSizeMode.CenterImage;
                miNormal.Checked = mode == PictureBoxSizeMode.Normal;
                miStretchImage.Checked = mode == PictureBoxSizeMode.StretchImage;
                miZoom.Checked = mode == PictureBoxSizeMode.Zoom;
            }
        }
    }
}


#pragma warning restore