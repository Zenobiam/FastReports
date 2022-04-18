using FastReport.Forms;
using FastReport.Utils;
using System.Drawing;
using System.Windows.Forms;

namespace FastReport
{
    partial class PictureObject : IHasEditor
    {
        #region Private Methods

        private bool ShouldSerializeImage()
        {
            return Image != null;
        }

        private bool ShouldSerializePadding()
        {
            return Padding != new System.Windows.Forms.Padding();
        }

        private bool ShouldSerializeTransparentColor()
        {
            return TransparentColor != Color.Transparent;
        }

        #endregion Private Methods

        #region Public Methods

        /// <inheritdoc/>
        public override void HandleDragDrop(FRMouseEventArgs e)
        {
            PictureObject dragSource = e.DragSource as PictureObject;
            if (dragSource != null)
            {
                if (dragSource.Image != null) // drag from OS
                    Image = dragSource.Image;
                else                          // drag from DictionaryWindow
                {
                    DataColumn = dragSource.DataColumn;
                    if (Image != null)
                    {
                        Image.Dispose();
                        Image = null;
                    }
                }
            }
            dragAccept = false;
        }

        /// <inheritdoc/>
        public override void HandleDragOver(FRMouseEventArgs e)
        {
            if (PointInObject(new PointF(e.x, e.y)) && e.DragSource is PictureObject)
                e.handled = true;
            dragAccept = e.handled;
        }

        /// <summary>
        /// Invokes the object's editor.
        /// </summary>
        /// <returns><b>true</b> if object was edited succesfully.</returns>
        public override bool InvokeEditor()
        {
            using (PictureEditorForm form = new PictureEditorForm(this))
            {
                return form.ShowDialog() == DialogResult.OK;
            }
        }

        #endregion Public Methods
    }
}