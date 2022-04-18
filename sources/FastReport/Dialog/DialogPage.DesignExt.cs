using FastReport.Design.PageDesigners.Dialog;
using FastReport.Utils;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Dialog
{
    partial class DialogPage
    {
        #region Private Fields

        private Bitmap formBitmap;

        #endregion Private Fields

        #region Public Properties

        /// <inheritdoc/>
        [Browsable(false)]
        public override float Left
        {
            get { return base.Left; }
            set { base.Left = value; }
        }

        /// <inheritdoc/>
        public override SizeF SnapSize
        {
            get { return new SizeF(DialogWorkspace.Grid.SnapSize, DialogWorkspace.Grid.SnapSize); }
        }

        /// <inheritdoc/>
        [Browsable(false)]
        public override float Top
        {
            get { return base.Top; }
            set { base.Top = value; }
        }

        #endregion Public Properties

        #region Internal Properties

        [Browsable(false)]
        internal Bitmap FormBitmap
        {
            get
            {
                if (formBitmap == null)
                    ResetFormBitmap();
                return formBitmap;
            }
        }

        [Browsable(false)]
        internal int CaptionHeight
        {
            get
            {
                return Form.Size.Height - Form.ClientSize.Height;
            }
        }

        #endregion Internal Properties

        #region Public Methods

        /// <inheritdoc/>
        public override void DrawSelection(FRPaintEventArgs e)
        {
            Pen pen = e.Cache.GetPen(Color.Gray, 1, DashStyle.Dot);
            //e.Graphics.DrawRectangle(pen, Left - Offset().X - 2, Top - Offset().Y - 2, (Width + 3) * e.ScaleX + 3, (ClientSize.Height + 2) * e.ScaleY + CaptionHeight);
            e.Graphics.DrawRectangle(pen, Left - Offset().X - 2, Top - Offset().Y - 2, (Width - Offset().X + 2) * e.ScaleX, (ClientSize.Height + 2) * e.ScaleY + CaptionHeight);
            SelectionPoint[] selectionPoints = GetSelectionPoints();
            foreach (SelectionPoint pt in selectionPoints)
            {
                DrawSelectionPoint(e, pt.x, pt.y);
            }
        }

        /// <inheritdoc/>
        public override Type GetPageDesignerType()
        {
            return typeof(DialogPageDesigner);
        }

        /// <inheritdoc/>
        public override void HandleDoubleClick()
        {
            Report.Designer.ActiveReportTab.SwitchToCode();
            if (String.IsNullOrEmpty(LoadEvent))
            {
                string newEventName = Name + "_Load";
                if (Report.CodeHelper.AddHandler(typeof(EventHandler), newEventName))
                {
                    LoadEvent = newEventName;
                    Report.Designer.SetModified(null, "Change");
                }
            }
            else
            {
                Report.CodeHelper.LocateHandler(LoadEvent);
            }
        }

        /// <inheritdoc/>
        public override void HandleMouseDown(FRMouseEventArgs e)
        {
            e.handled = true;
            e.mode = WorkspaceMode2.SelectionRect;
            e.activeObject = this;
        }

        /// <inheritdoc/>
        public override void HandleMouseHover(FRMouseEventArgs e)
        {
            base.HandleMouseHover(e);
            if (e.handled)
                e.cursor = Cursors.Default;
        }

        /// <inheritdoc/>
        public override void HandleMouseUp(FRMouseEventArgs e)
        {
            base.HandleMouseUp(e);
            if (e.mode == WorkspaceMode2.SelectionRect)
            {
                SelectedObjectCollection selection = Report.Designer.SelectedObjects;
                selection.Clear();
                // find objects inside the selection rect
                foreach (DialogComponentBase c in Controls)
                {
                    e.handled = false;
                    c.HandleMouseUp(e);
                    // object is inside
                    if (e.handled)
                        selection.Add(c);
                }
                if (selection.Count == 0)
                    selection.Add(this);
            }
        }

        /// <inheritdoc/>
        public override void SetDefaults()
        {
            ButtonControl btnOk = new ButtonControl();
            btnOk.Parent = this;
            btnOk.Name = CreateButtonName("btnOk");
            btnOk.Text = Res.Get("Buttons,OK");
            btnOk.Location = new Point((int)ClientSize.Width - 166, (int)ClientSize.Height - 31);
            btnOk.Size = new Size(75, 23);
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            ButtonControl btnCancel = new ButtonControl();
            btnCancel.Parent = this;
            btnCancel.Name = CreateButtonName("btnCancel");
            btnCancel.Text = Res.Get("Buttons,Cancel");
            btnCancel.Location = new Point((int)ClientSize.Width - 83, (int)ClientSize.Height - 31);
            btnCancel.Size = new Size(75, 23);
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            AcceptButton = btnOk;
            CancelButton = btnCancel;

            base.SetDefaults();
        }

        #endregion Public Methods

        #region Internal Methods

        internal void ResetFormBitmap()
        {
            if (formBitmap != null)
            {
                formBitmap.Dispose();
                formBitmap = null;
            }
            if (!IsDesigning)
                return;

            Size ts = Form.Size;
            Form.ClientSize = DpiHelper.ConvertUnits(Form.ClientSize);
            formBitmap = DrawUtils.DrawToBitmap(Form, false);
            Form.Size = ts;
            // WinXP form has round edges which are filled black (DrawToBitmap issue/bug).
            DrawUtils.FloodFill(formBitmap, 0, 0, Color.FromArgb(0, 0, 0), Color.White);
            DrawUtils.FloodFill(formBitmap, formBitmap.Width - 1, 0, Color.FromArgb(0, 0, 0), Color.White);
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc/>
        protected override SelectionPoint[] GetSelectionPoints()
        {
            return new SelectionPoint[] {
        new SelectionPoint(Width - Offset().X + 1, Height - Offset().Y + 1, SizingPoint.RightBottom),
        new SelectionPoint(Width / 2 - Offset().X, Height - Offset().Y + 1, SizingPoint.BottomCenter),
        new SelectionPoint(Width - Offset().X + 1, Height / 2 - Offset().Y, SizingPoint.RightCenter) };
        }

        #endregion Protected Methods

        #region Private Methods

        private void DrawSelectionPoint(FRPaintEventArgs e, float x, float y)
        {
            x = (float)Math.Round(x * e.ScaleX);
            y = (float)Math.Round(y * e.ScaleY);
            Pen p = Pens.Black;
            IGraphics g = e.Graphics;
            g.FillRectangle(Brushes.White, x - 2, y - 2, 5, 5);
            g.DrawLine(p, x - 2, y - 3, x + 2, y - 3);
            g.DrawLine(p, x - 2, y + 3, x + 2, y + 3);
            g.DrawLine(p, x - 3, y - 2, x - 3, y + 2);
            g.DrawLine(p, x + 3, y - 2, x + 3, y + 2);
        }

        private Point Offset()
        {
            Point offset = new Point(0, 0);
            offset = Form.PointToScreen(offset);
            offset.X -= Form.Left;
            offset.Y -= Form.Top;
            return offset;
        }

        private bool ShouldSerializeBackColor()
        {
            return BackColor != SystemColors.Control;
        }

        //private bool ShouldSerializeFont()
        //{
        //    return Font.Name != "Tahoma" || Font.Size != 8 || Font.Style != FontStyle.Regular;
        //}

        #endregion Private Methods
    }
}