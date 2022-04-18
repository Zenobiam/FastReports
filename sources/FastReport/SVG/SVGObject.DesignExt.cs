using FastReport.Forms;
using FastReport.Utils;
using System.Drawing;
using System.Windows.Forms;

#pragma warning disable

namespace FastReport.SVG
{
    public partial class SVGObject : IHasEditor
    {
        #region Public Methods

        /// <inheritdoc/>
        public override ContextMenuBase GetContextMenu()
        {
            return new SVGObjectMenu(Report.Designer);
        }

        /// <inheritdoc/>
        public override void HandleDragDrop(FRMouseEventArgs e)
        {
            DataColumn = (e.DragSource as SVGObject).DataColumn;
            dragAccept = false;
        }

        /// <inheritdoc/>
        public override void HandleDragOver(FRMouseEventArgs e)
        {
            if (PointInObject(new PointF(e.x, e.y)) && e.DragSource is SVGObject)
                e.handled = true;
            dragAccept = e.handled;
        }

        /// <summary>
        /// Invokes the object's editor.
        /// </summary>
        /// <returns><b>true</b> if object was edited succesfully.</returns>
        public override bool InvokeEditor()
        {
            using (SVGEditorForm form = new SVGEditorForm(this))
            {
                bool isOk = form.ShowDialog() == DialogResult.OK;
                Assign(form.SvgObject);
                return isOk ? true : false;
            }
        }

        #endregion Public Methods
    }
}

#pragma warning restore