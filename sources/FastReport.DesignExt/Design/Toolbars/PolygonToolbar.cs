using System;
using System.Windows.Forms;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents.DotNetBar;
#endif

namespace FastReport.Design.Toolbars
{
    internal class PolygonToolbar : ToolbarBase
    {
        #region Fields
#if !MONO
        public ButtonItem btnMove;
        public ButtonItem btnPointer;
        public ButtonItem btnAddPoint;
        public ButtonItem btnAddBezier;
        public ButtonItem btnRemovePoint;
#else
        public ToolStripButton btnMove;
        public ToolStripButton btnPointer;
        public ToolStripButton btnAddPoint;
        public ToolStripButton btnAddBezier;
        public ToolStripButton btnRemovePoint;
#endif
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private void UpdateControls()
        {
            bool enabled = (Designer.SelectedObjects.Count == 1) && (Designer.SelectedObjects[0] is PolyLineObject);
            btnMove.Enabled = enabled;
            btnPointer.Enabled = enabled;
            btnAddBezier.Enabled = enabled;
            btnAddPoint.Enabled = enabled;
            btnRemovePoint.Enabled = enabled;
            if (!enabled)
                selectBtn(PolyLineObject.PolygonSelectionMode.MoveAndScale);
            else
            {
                PolyLineObject plobj = (Designer.SelectedObjects[0] as PolyLineObject);
                selectBtn(plobj.SelectionMode);
            }
        }

        private void selectBtn(PolyLineObject.PolygonSelectionMode index)
        {
            PolyLineObject plobj = null;
            if ((Designer.SelectedObjects.Count == 1) && (Designer.SelectedObjects[0] is PolyLineObject))
                plobj = (Designer.SelectedObjects[0] as PolyLineObject);
            btnPointer.Checked = false;
            btnAddBezier.Checked = false;
            btnAddPoint.Checked = false;
            btnRemovePoint.Checked = false;
            btnMove.Checked = false;
            switch (index)
            {
                case PolyLineObject.PolygonSelectionMode.Normal:
                    btnPointer.Checked = true;
                    break;
                case PolyLineObject.PolygonSelectionMode.AddBezier:
                    btnAddBezier.Checked = true;
                    break;
                case PolyLineObject.PolygonSelectionMode.AddToLine:
                    btnAddPoint.Checked = true;
                    break;
                case PolyLineObject.PolygonSelectionMode.Delete:
                    btnRemovePoint.Checked = true;
                    break;
                case PolyLineObject.PolygonSelectionMode.MoveAndScale:
                    btnMove.Checked = true;
                    break;
            }
        }
        #endregion

        #region Public Methods
        public override void SelectionChanged()
        {
            base.SelectionChanged();
            UpdateControls();
        }

        public override void UpdateContent()
        {
            base.UpdateContent();
            UpdateControls();
        }

        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Designer,Toolbar,Polygon");
            Text = res.Get("");

            SetItemText(btnMove, res.Get("MoveScale"));
            SetItemText(btnPointer, res.Get("Pointer"));
            SetItemText(btnAddPoint, res.Get("AddPoint"));
            SetItemText(btnAddBezier, res.Get("Bezier"));
            SetItemText(btnRemovePoint, res.Get("RemovePoint"));
        }

        public override void ReinitDpiSize()
        {
            base.ReinitDpiSize();
            btnMove.Image = Res.GetImage(256);
            btnPointer.Image = Res.GetImage(252);
            btnAddPoint.Image = Res.GetImage(253);
            btnAddBezier.Image = Res.GetImage(254);
            btnRemovePoint.Image = Res.GetImage(255);
        }
        #endregion


        public PolygonToolbar(Designer designer) : base(designer)
        {
            Name = "PolygonToolbar";

            btnMove = CreateButton("btnMove", Res.GetImage(256), Designer.CmdPolySelectMove.Invoke);
            btnPointer = CreateButton("btnPolygonPointer", Res.GetImage(252), Designer.CmdPolySelectPointer.Invoke);
            btnAddPoint = CreateButton("btnPolygonAddPoint", Res.GetImage(253), Designer.CmdPolySelectAddPoint.Invoke);
            btnAddBezier = CreateButton("btnPolygonAddPointToStart", Res.GetImage(254), Designer.CmdPolySelectBezier.Invoke);
            btnRemovePoint = CreateButton("btnPolygonRemovePoint", Res.GetImage(255), Designer.CmdPolySelectRemovePoint.Invoke);

#if !MONO
            Items.AddRange(new BaseItem[] { btnMove, btnPointer, btnAddPoint, btnAddBezier, btnRemovePoint });
#else
            Items.AddRange(new ToolStripItem[] { btnMove, btnPointer, btnAddPoint, btnAddBezier, btnRemovePoint });
#endif

            selectBtn(PolyLineObject.PolygonSelectionMode.Normal);
            Localize();
        }
    }
}