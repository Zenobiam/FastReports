using FastReport.Utils;
using System.Drawing;
using System.Windows.Forms;

namespace FastReport
{
    partial class PictureObjectBase
    {
        #region Internal Fields

        internal bool dragAccept;

        #endregion Internal Fields

        #region Private Methods

        private void DrawDesign(FRPaintEventArgs e)
        {
            if (dragAccept)
                DrawDragAcceptFrame(e, Color.Silver);
        }

        #endregion Private Methods

        #region Protected Methods

        /// <summary>
        /// Draw an error image to Graphics g, when the image is designing
        /// </summary>
        /// <param name="g"></param>
        /// <param name="e"></param>
        protected void DrawErrorImage(IGraphics g, FRPaintEventArgs e)
        {
            if (IsDesigning)
                g.DrawImage(Res.GetImage(103), (int)(AbsLeft * e.ScaleX) + 3, (int)(AbsTop * e.ScaleY) + 3);
            else if (ShowErrorImage)
                g.DrawImage(Res.GetImage(80), (int)(AbsLeft * e.ScaleX) + 3, (int)(AbsTop * e.ScaleY) + 3);
        }

        /// <inheritdoc/>
        protected override SelectionPoint[] GetSelectionPoints()
        {
            if (SizeMode == PictureBoxSizeMode.AutoSize && ImageWidth != 0 && ImageHeight != 0)
                return new SelectionPoint[] { new SelectionPoint(AbsLeft, AbsTop, SizingPoint.LeftTop) };
            return base.GetSelectionPoints();
        }

        #endregion Protected Methods

        #region Public Methods

        /// <inheritdoc/>
        public override ContextMenuBase GetContextMenu()
        {
            return new PictureObjectMenu(Report.Designer);
        }

        /// <inheritdoc/>
        public override SizeF GetPreferredSize()
        {
            if ((Page as ReportPage).IsImperialUnitsUsed)
                return new SizeF(Units.Inches * 1, Units.Inches * 1);
            return new SizeF(Units.Millimeters * 20, Units.Millimeters * 20);
        }

        /// <inheritdoc/>
        public override SmartTagBase GetSmartTag()
        {
            return new PictureObjectSmartTag(this);
        }

        /// <summary>
        /// Invokes the object's editor.
        /// </summary>
        /// <returns><b>true</b> if object was edited succesfully.</returns>
        public abstract bool InvokeEditor();

        #endregion Public Methods
    }
}